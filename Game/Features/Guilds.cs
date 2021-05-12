using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Server.Game;

namespace Server.Features
{
    public enum GuildRank : byte
    {
        Member = 50,
        InternMgr = 60,
        DeputyMgr = 70,
        BranchMgr = 80,
        DeputyLeader = 90,
        GuildLeader = 100
    }
    public class MemberInfo
    {
        public uint MembID;
        public string MembName;
        public uint Donation;
        public byte Level;
        public GuildRank Rank;
        public ushort MyGuildID;

        public string MemberString
        {
            get
            {
                string e = "";
                e += MembName + ' ' + Level + ' ' + Convert.ToByte(World.H_Chars.Contains(MembID));
                e = Convert.ToChar((byte)e.Length) + e;
                return e;
           }
        }
        public Character Info
        {
            get
            {
                return (Character)World.H_Chars[MembID];
            }
        }
       public void WriteThis(BinaryWriter BW)
        {
            BW.Write(MembID);
            BW.Write(MembName);
            BW.Write(Donation);
            BW.Write(Level);
            BW.Write((byte)Rank);
            BW.Write(MyGuildID);
        }
        public void ReadThis(BinaryReader BR)
        {
            MembID = BR.ReadUInt32();
            MembName = BR.ReadString();
            Donation = BR.ReadUInt32();
            Level = BR.ReadByte();
            Rank = (GuildRank)BR.ReadByte();
            MyGuildID = BR.ReadUInt16();
        }
    }
    public class Guilds
    {
       // public static Hashtable AllTheGuilds = new Hashtable();
        public static Dictionary<uint, Features.Guild> AllTheGuilds = new Dictionary<uint, Features.Guild>();

        public static bool ValidName(string Name)
        {
            if (Name.Length < 3 || Name.Length > 16)
                return false;
            foreach (Guild G in AllTheGuilds.Values)
                if (G.GuildName == Name)
                    return false;
            return true;
        }
        public static void SaveGuilds()
        {
            FileStream FS = new FileStream(Program.ConquerPath + @"Guilds.dat", FileMode.OpenOrCreate);
            BinaryWriter BW = new BinaryWriter(FS);
            BW.Write(AllTheGuilds.Count);
            foreach (Guild G in AllTheGuilds.Values)
                G.SaveThis(BW);
            if (Extra.GuildWars.LastWinner != null)
                BW.Write(Extra.GuildWars.LastWinner.GuildID);
            else
                BW.Write((ushort)0);

            BW.Close();
            FS.Close();
        }
        public static void LoadGuilds()
        {
            try
            {
                if (File.Exists(Program.ConquerPath + @"Guilds.dat"))
                {
                    FileStream FS = new FileStream(Program.ConquerPath + @"Guilds.dat", FileMode.Open);
                    BinaryReader BR = new BinaryReader(FS);
                    int guildscount = 0;
                    int GuildsCount = BR.ReadInt32();
                    for (int i = 0; i < GuildsCount; i++)
                    {
                        Guild G = new Guild(BR);
                        AllTheGuilds.Add(G.GuildID, G);
                        guildscount++;
                    }
                    foreach (Guild G in AllTheGuilds.Values)
                    {
                        Database.LoadGuildEnemies(G);
                        Database.LoadGuildAllis(G);
                    }
                    try
                    {
                        Extra.GuildWars.LastWinner = (Guild)AllTheGuilds[BR.ReadUInt16()];
                    }
                    catch { }
                    Console.WriteLine("Guilds loading " + guildscount.ToString());
                }
            }
            catch { }
        }
        public static void CreateNewGuild(string GName, ushort GID, Character Creator)
        {
            Guild G = new Guild(GID, GName);
            MemberInfo M = new MemberInfo();
            M.Rank = GuildRank.GuildLeader;
            M.MembID = Creator.EntityID;
            M.Level = Creator.Level;
            M.MembName = Creator.Name;
            M.Donation = 1000000;
            M.MyGuildID = GID;
            Creator.MyGuild = G;
            Creator.GuildDonation = 1000000;
            Creator.GuildRank = GuildRank.GuildLeader;
            G.Creator = M;
            Creator.MembInfo = M;
            ((Hashtable)G.Members[(byte)100]).Add(M.MembID, M);
            AllTheGuilds.Add(G.GuildID, G);
        }
    }
    public class Guild
    {
        public MemberInfo Creator;
        public Hashtable Members = new Hashtable();
        public uint Fund;
        public uint Wins;
        public ushort GuildID;
        public string GuildName;
        public string Bulletin = "A guild";


        public Dictionary<int, Guild> Allies = new Dictionary<int, Guild>(5);
        public Dictionary<uint, Guild> Enemies = new Dictionary<uint, Guild>(5);

        public Guild(ushort guildid, string guildname)
        {
            Fund = 1000000;
            GuildID = guildid;
            GuildName = guildname;

            Hashtable CreatorHt = new Hashtable();
            Hashtable DLs = new Hashtable();
            Hashtable Membs = new Hashtable();
            Members.Add((byte)100, CreatorHt);
            Members.Add((byte)90, DLs);
            Members.Add((byte)50, Membs);
        }
       public void SaveThis(BinaryWriter BW)
        {
            Creator.WriteThis(BW);
            BW.Write(MembersCount);
            foreach (Hashtable H in Members.Values)
                foreach (MemberInfo M in H.Values)
                    M.WriteThis(BW);

            BW.Write(Fund);
            BW.Write(GuildID);
            BW.Write(GuildName);
            BW.Write(Bulletin);
            BW.Write(Wins);
        }
        public Guild(BinaryReader BR)
        {
            Creator = new MemberInfo();
            Creator.ReadThis(BR);
            int MembCount = BR.ReadInt32();
            Hashtable CreatorHt = new Hashtable();
            Hashtable DLs = new Hashtable();
            Hashtable NMs = new Hashtable();
            for (int i = 0; i < MembCount; i++)
            {
                MemberInfo M = new MemberInfo();
                M.ReadThis(BR);
                if (M.Rank == GuildRank.GuildLeader)
                    CreatorHt.Add(M.MembID, M);
                else if (M.Rank == GuildRank.DeputyLeader)
                    DLs.Add(M.MembID, M);
                else if (M.Rank == GuildRank.Member)
                    NMs.Add(M.MembID, M);
            }
            Members.Add((byte)100, CreatorHt);
            Members.Add((byte)90, DLs);
            Members.Add((byte)50, NMs);
            Fund = BR.ReadUInt32();
            GuildID = BR.ReadUInt16();
            GuildName = BR.ReadString();
            Bulletin = BR.ReadString();
            Wins = BR.ReadUInt32();


        }
        public int MembersCount
        {
            get
            {
                int e = 0;
                foreach (Hashtable H in Members.Values)
                    foreach (MemberInfo M in H.Values)
                        e++;
                return e;
            }
        }
        public void Disband()
        {
            foreach (DictionaryEntry DE in (Hashtable)Members[(byte)50])
            {
                MemberInfo M = (MemberInfo)DE.Value;
                Character C = M.Info;
                if (C != null)
                {
                    C.MyClient.SendPacket(Packets.SendGuild(GuildID, 19));
                    C.MyGuild = null;
                    C.GuildRank = 0;
                    C.GuildDonation = 0;
                    World.Spawn(C, false);
                }
            }
            foreach (DictionaryEntry DE in (Hashtable)Members[(byte)90])
            {
                MemberInfo M = (MemberInfo)DE.Value;
                Character C = M.Info;
                if (C != null)
                {
                    C.MyClient.SendPacket(Packets.SendGuild(GuildID, 19));
                    C.MyGuild = null;
                    C.GuildRank = 0;
                    C.GuildDonation = 0;
                    World.Spawn(C, false);
                }
            }
            foreach (DictionaryEntry DE in (Hashtable)Members[(byte)100])
            {
                MemberInfo M = (MemberInfo)DE.Value;
                Character C = M.Info;
                if (C != null)
                {
                    C.MyClient.SendPacket(Packets.SendGuild(GuildID, 19));
                    C.MyGuild = null;
                    C.GuildRank = 0;
                    C.GuildDonation = 0;
                    World.Spawn(C, false);
                }
            }
            Members = null;
            Creator = null;
            World.WorldMessage("SYSTEM", GuildName + " has been disbanded.", 2000, 0, System.Drawing.Color.Red);
            Guilds.AllTheGuilds.Remove(GuildID);
            Guilds.SaveGuilds();
        }
        public bool AddMember(MemberInfo I, bool New)
        {
            if (Find(I.MembID) == null)
            {
                ((Hashtable)Members[(byte)50]).Add(I.MembID, I);
                if (New)
                {
                    GuildMsg("SYSTEM", "ALL", I.MembName + " has joined our guild.", 0);
                    Features.Guilds.SaveGuilds();
                }
                return true;
            }
            else return false;
        }
        public void NewBulletin(byte[] Data, string B)
        {
            Bulletin = B;
            foreach (Hashtable H in Members.Values)
            {
                foreach (MemberInfo M in H.Values)
                {
                    if (World.H_Chars.Contains(M.MembID))
                    {
                        Character C = (Character)World.H_Chars[M.MembID];
                        C.MyClient.SendPacket2(Data);
                    }
                }
            }
        }
        public MemberInfo Find(uint UID)
        {
            foreach (Hashtable H in Members.Values)
            {
                if (H.Contains(UID))
                    return (MemberInfo)H[UID];
            }
            return null;
        }
        public void MemberLeaves(uint MID, bool Kick)
        {
            MemberInfo M = Find(MID);
            if (M != null)
            {
                ((Hashtable)Members[(byte)M.Rank]).Remove(MID);
                if (Kick)
                    GuildMsg("SYSTEM", "ALL", M.MembName + " has been kicked out of our guild.", 0);
                else
                    GuildMsg("SYSTEM", "ALL", M.MembName + " has left our guild.", 0);
            }
        }
        public void MemberLeaves(string Name, bool Kick)
        {
            MemberInfo M = MembOfName(Name);
            if (M != null)
            {
                ((Hashtable)Members[(byte)M.Rank]).Remove(M.MembID);
                if (Kick)
                    GuildMsg("SYSTEM", "ALL", M.MembName + " has been kicked out of our guild.", 0);
                else
                    GuildMsg("SYSTEM", "ALL", M.MembName + " has left our guild.", 0);

                Character C = M.Info;
                if (C != null)
                {
                    C.MyGuild = null;
                    C.GuildDonation = 0;
                    C.GuildRank = (GuildRank)0;
                    C.MyClient.SendPacket(Packets.SendGuild(0, 19));
                    World.Spawn(C, false);
                }
            }
            Guilds.SaveGuilds();
        }
        public void GuildMsg(string From, string To, string Msg, uint Mesh)
        {
            foreach (Hashtable H in Members.Values)
            {
                foreach (MemberInfo M in H.Values)
                {
                    if (World.H_Chars.Contains(M.MembID))
                    {
                        Character C = (Character)World.H_Chars[M.MembID];
                        C.MyClient.SendPacket(Packets.ChatMessage(C.MyClient.MessageID, From, To, Msg, 2004, Mesh, System.Drawing.Color.White));
                    }
                }
            }
        }
        public void GuildMsg(byte[] Data, uint Sender)
        {
            foreach (Hashtable H in Members.Values)
            {
                foreach (MemberInfo M in H.Values)
                {
                    if (World.H_Chars.Contains(M.MembID))
                    {
                        Character C = (Character)World.H_Chars[M.MembID];
                        if (C.EntityID != Sender)
                        {
                            C.MyClient.SendPacket2(Data);
                       
                        }
                    }
                }
            }
        }
        public MemberInfo MembOfName(string Name)
        {
            foreach (MemberInfo M in ((Hashtable)Members[(byte)50]).Values)
                if (M.MembName == Name)
                    return M;
            return null;
        }
        public MemberInfo deputysOfName(string Name)
        {
            foreach (MemberInfo M in ((Hashtable)Members[(byte)90]).Values)
                if (M.MembName == Name)
                    return M;
            return null;
        }
    }
}