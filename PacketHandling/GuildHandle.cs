using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server.Game;

namespace Server.PacketHandling
{
    public class GuildHandle
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
             byte   Type = Data[4];
            switch (Type)
            {
                case 1://Join request
                    {
                        if (GC.MyChar.MyGuild == null && GC.MyChar.Job < 160)
                        {
                            uint UID = BitConverter.ToUInt32(Data, 8);
                            Character Chr = (Character)World.H_Chars[UID];

                            if (Chr.MyGuild != null && (Chr.GuildRank == Features.GuildRank.GuildLeader || Chr.GuildRank == Features.GuildRank.DeputyLeader))
                                Chr.MyClient.SendPacket(Packets.SendGuild(GC.MyChar.EntityID, 1)); Features.Guilds.SaveGuilds();
                        }
                        break;
                    }
                case 2://Receive join request
                    {
                        if (GC.MyChar.MyGuild != null && (GC.MyChar.GuildRank == Features.GuildRank.DeputyLeader || GC.MyChar.GuildRank == Features.GuildRank.GuildLeader))
                        {
                            uint UID = BitConverter.ToUInt32(Data, 8);
                            if (World.H_Chars.Contains(UID))
                            {
                                Character Chr = (Character)World.H_Chars[UID];
                                if (Chr.MyGuild == null && Chr.Job < 160)
                                {
                                    Features.MemberInfo M = new Features.MemberInfo();
                                    M.Donation = 0;
                                    M.Level = Chr.Level;
                                    M.MembID = Chr.EntityID;
                                    M.MembName = Chr.Name;
                                    M.MyGuildID = GC.MyChar.MyGuild.GuildID;
                                    M.Rank = Features.GuildRank.Member;
                                    Chr.MembInfo = M;
                                    Chr.GuildRank = Features.GuildRank.Member;
                                    Chr.GuildDonation = 0;
                                    Chr.MyGuild = GC.MyChar.MyGuild;
                                    Chr.MyGuild.GuildID = GC.MyChar.MyGuild.GuildID;
                                    GC.MyChar.MyGuild.AddMember(M, true);
                                    Features.Guilds.SaveGuilds(); 
                                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                    {
                                        Features.Guild TGuild = Guilds.Value;
                                        if (Chr.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                        {
                                            Chr.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                                        }
                                    }
                                    try
                                    {
                                        foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                        {

                                            Features.Guild TGuild = Guilds.Value;
                                            if (GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                            {
                                                GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 7));
                                            }
                                        }
                                    }
                                    catch { }
                                    Chr.MyClient.SendPacket(Packets.GuildInfo(Chr.MyGuild, Chr));
                                    Chr.MyClient.SendPacket(Packets.String(Chr.MyGuild.GuildID, (byte)StringType.GuildName, Chr.MyGuild.GuildName));
                                    World.Spawn(Chr, false); 
                                    Features.Guilds.SaveGuilds();
                                }
                            }
                        }
                        break;
                    }
                case 7:
                case 9:
                    {
                        GC.MyChar.MyClient.SendPacket2(Data);
                        break;
                    }
                case 3:
                    {
                        if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank != Features.GuildRank.GuildLeader)
                        {
                            GC.SendPacket(Packets.SendGuild(GC.MyChar.MyGuild.GuildID, 19));
                            GC.MyChar.MyGuild.MemberLeaves(GC.MyChar.EntityID, false);
                            GC.MyChar.GuildRank = 0;
                            GC.MyChar.GuildDonation = 0;
                            GC.MyChar.MyGuild = null;
                            World.Spawn(GC.MyChar, false); Features.Guilds.SaveGuilds();
                        }
                        break;
                    }
                case 11://Donate
                    {
                        if (GC.MyChar.MyGuild != null)
                        {
                            uint Amount = BitConverter.ToUInt32(Data, 8);
                            if (GC.MyChar.Silvers >= Amount)
                            {
                                GC.MyChar.Silvers -= Amount;
                                GC.MyChar.MembInfo.Donation += Amount;
                                GC.MyChar.GuildDonation += Amount;
                                GC.MyChar.MyGuild.Fund += Amount;
                                GC.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                                GC.MyChar.MyGuild.GuildMsg("SYSTEM", "ALL", GC.MyChar.Name + " donated " + Amount.ToString() + " to " + GC.MyChar.MyGuild.GuildName + ".", 0);
                                Features.Guilds.SaveGuilds();
                            }
                        }
                        break;
                    }
                case 12://Guild status
                    {
                        if (GC.MyChar.MyGuild != null)
                        {
                            try
                            {
                                foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                {

                                    Features.Guild TGuild = Guilds.Value;
                                    if (GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                    {
                                        GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                                    }
                                }
                            }
                            catch { }
                            try
                            {
                                foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                {

                                    Features.Guild TGuild = Guilds.Value;
                                    if (GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                    {
                                        GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 7));
                                    }
                                }
                            }
                            catch { }
                            GC.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                            GC.SendPacket(Packets.ChatMessage(GC.MessageID, "SYSTEM", GC.MyChar.Name, GC.MyChar.MyGuild.Bulletin, 2111, 0, System.Drawing.Color.White));
                            GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 97));// متشالة من السرفر 5355
                            GC.SendPacket(Packets.ChatMessage(GC.MessageID, "SYSTEM", GC.MyChar.Name, "While being in this guild you gain " + GC.MyChar.MyGuild.Wins + "% more experience.", 2005, 0, System.Drawing.Color.White));
                            Features.Guilds.SaveGuilds();
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
    public class GetGuildMembers
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            if (GC.MyChar.MyGuild != null)
            {
                //(char)Length Level ON
                string[] MembersStringArr = new string[256];
                for (int i = 0; i < 256; i++)
                    MembersStringArr[i] = "";
                int Count = 0;
                Hashtable CurHt = (Hashtable)GC.MyChar.MyGuild.Members[(byte)100];
                foreach (Features.MemberInfo M in CurHt.Values)
                {
                    MembersStringArr[M.Level] += M.MemberString;
                    Count++;
                }
                CurHt = (Hashtable)GC.MyChar.MyGuild.Members[(byte)90];
                foreach (Features.MemberInfo M in CurHt.Values)
                {
                    MembersStringArr[M.Level] += M.MemberString;
                    Count++;
                }
                CurHt = (Hashtable)GC.MyChar.MyGuild.Members[(byte)50];
                foreach (Features.MemberInfo M in CurHt.Values)
                {
                    MembersStringArr[M.Level] += M.MemberString;
                    Count++;
                }
                string MembersString = "";
                for (int i = 255; i > 0; i--)
                    if (MembersStringArr[i].Length > 0)
                        MembersString += MembersStringArr[i];
                GC.SendPacket(Packets.StringGuild(GC.MyChar.MyGuild.GuildID, 11, MembersString, (byte)Count));
            }
        }
    }
    public class GuildMembInfo
    {
        public static unsafe void Handle(GameClient GC, byte[] Data)
        {
            string Name = "";
            for (int i = 9; i < 28; i++)
            {
                if (Data[i] != 0)
                    Name += Convert.ToChar(Data[i]);
                else break;
            }
            foreach (Features.MemberInfo M in ((Hashtable)GC.MyChar.MyGuild.Members[(byte)GC.MyChar.MembInfo.Rank]).Values)
            {
                if (M.MembName == Name)
                {
                    fixed (byte* p = Data)
                    {
                        *((uint*)(p + 4)) = (uint)M.Donation;
                        *(p + 8) = (byte)M.Rank;
                    }
                }
            }
            byte[] P = Data;
            GC.SendPacket2(P);
        }
    }
}
