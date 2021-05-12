using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Server.Game
{
    public enum ConquerAngle : byte
    {
        SouthWest = 0,
        West = 1,
        NorthWest = 2,
        North = 3,
        NorthEast = 4,
        East = 5,
        SouthEast = 6,
        South = 7
    }
    public enum ConquerAction : byte
    {
        None = 0x00,
        Cool = 0xE6,
        Kneel = 0xD2,
        Sad = 0xAA,
        Happy = 0x96,
        Angry = 0xA0,
        Lie = 0x0E,
        Dance = 0x01,
        Wave = 0xBE,
        Bow = 0xC8,
        Sit = 0xFA,
        Jump = 0x64
    }
    public enum StringType
    {
        GuildName = 3,
        Spouse = 6,
        Effect = 10,
        GuildList = 11,
        ViewEquipSpouse = 16,
        Sound = 20,
        GuildAllies = 21,
        GuildEnemies = 22
    }

    public struct BroadCastMessage
    {
        public string Name;
        public string Message;
        public byte Place;
    }
    public struct Vector2
    {
        public ushort X;
        public ushort Y;

        public Vector2(ushort X, ushort Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    public struct EmpireInfo
    {
        public string Name;
        public ulong Donation;
        public uint ID;
        public void WriteThis(System.IO.BinaryWriter BW)
        {
            if (Name == null)
                Name = "";
            BW.Write(Name.Length);
            BW.Write(Encoding.ASCII.GetBytes(Name));
            BW.Write(Donation);
            BW.Write(ID);
        }
        public void ReadThis(System.IO.BinaryReader BR)
        {
            try
            {
                Name = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                Donation = BR.ReadUInt64();
                ID = BR.ReadUInt32();
            }
            catch
            {
                Name = "";
                Donation = 0;
                ID = 0;
            }
        }
    }
    public struct KOInfo
    {
        public string Name;
        public int KillCount;
        public uint KOID;

        public void WriteThis(System.IO.BinaryWriter BW)
        {
            if (Name == null)
                Name = "";
            BW.Write(Name.Length);
            BW.Write(Encoding.ASCII.GetBytes(Name));
            BW.Write(KillCount);
            BW.Write(KOID);
        }
        public void ReadThis(System.IO.BinaryReader BR)
        {
            try
            {
                Name = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                KillCount = BR.ReadInt32();
                KOID = BR.ReadUInt32();
            }
            catch
            {
                Name = "";
                KillCount = 0;
                KOID = 0;
            }
        }
    }
    public struct DroppedItem
    {
        public Item Info;
        public Location Loc;
        public uint Silvers;
        public uint Owner;
        public DateTime DropTime;
        public uint UID;
        public bool someBodyDropedIt;

        public void Drop()
        {
            if (Info.Locked == 0 && Info.ID != 0 && UID != 0 && Info.ItemDBInfo.ID != 0)
            {
                if (Info.UID == 0)
                    Info.UID = (uint)World.Rnd.Next(10000000);
                if (!World.H_Items.Contains(Loc.Map))
                    World.H_Items.Add(Loc.Map, new Hashtable());

                Hashtable Map = (Hashtable)World.H_Items[Loc.Map];

                while (Map.Contains(UID))
                {
                    UID = (uint)World.Rnd.Next(10000000);
                }
                if (!Map.Contains(UID))
                {
                    World.Action(this, Packets.ItemDrop(this));
                    Map.Add(UID, this);
                }
            }
        }
        public bool FindPlace(Hashtable Map)
        {
            if (Map == null) return true;
            DMap DM = (DMap)DMaps.H_Maps[Loc.Map];
            bool FoundPlace = true;
            for (short x = -1; x < 2; x++)
            {
                for (short y = -1; y < 2; y++)
                {
                    try
                    {
                        foreach (DroppedItem D in Map.Values)
                        {
                            FoundPlace = true;
                            if ((D.Loc.X == (ushort)(Loc.X + x) && D.Loc.Y == (ushort)(Loc.Y + y)))
                            {
                                FoundPlace = false;
                                break;
                            }
                            if (DM != null)
                                if (DM.GetCell((ushort)(Loc.X + x), (ushort)(Loc.Y + y)).NoAccess)
                                    FoundPlace = false;
                        }
                    }
                    catch { FoundPlace = false; }
                    if (FoundPlace)
                    {
                        Loc.X = (ushort)(Loc.X + x);
                        Loc.Y = (ushort)(Loc.Y + y);
                        break;
                    }
                }
                if (FoundPlace)
                    break;
            }

            if (!FoundPlace)
                return false;
            return true;
        }
        public void Dissappear()
        {
            if (((Hashtable)World.H_Items[Loc.Map]).Contains(UID))
            {
                World.Action(this, Packets.ItemDropRemove(UID, Info.ID, Loc.X, Loc.Y));
                ((Hashtable)World.H_Items[Loc.Map]).Remove(UID);
                if (someBodyDropedIt == true)
                    Database.DeleteItem(UID, 0);

            }
        }
    }
    public struct DroppedItemConfiscator
    {
        public Item Info;
        public Location Loc;
        public uint Silvers;
        public uint Owner;
        public DateTime DropTime;
        public uint UID;

        public void Drop()
        {
            if (Info.ID != 0 && UID != 0 && Info.ItemDBInfo.ID != 0)
            {
                if (Info.UID == 0)
                    Info.UID = (uint)World.Rnd.Next(10000000);
                if (!World.H_Items.Contains(Loc.Map))
                    World.H_Items.Add(Loc.Map, new Hashtable());

                Hashtable Map = (Hashtable)World.H_Items[Loc.Map];

                for (byte i = 0; i < 10; i++)
                {
                    if (Map.Contains(UID))
                        UID = (uint)World.Rnd.Next(10000000);
                    else break;
                }
                if (!Map.Contains(UID))
                {
                    World.Action(this, Packets.DropConfiscatorItem(this));
                    //Map.Add(UID, this);
                }
            }
        }
        public bool FindPlace(Hashtable Map)
        {
            if (Map == null) return true;
            DMap DM = (DMap)DMaps.H_Maps[Loc.Map];
            bool FoundPlace = true;
            for (short x = -1; x < 2; x++)
            {
                for (short y = -1; y < 2; y++)
                {
                    try
                    {
                        foreach (DroppedItem D in Map.Values)
                        {
                            FoundPlace = true;
                            if ((D.Loc.X == (ushort)(Loc.X + x) && D.Loc.Y == (ushort)(Loc.Y + y)))
                            {
                                FoundPlace = false;
                                break;
                            }
                            if (DM != null)
                                if (DM.GetCell((ushort)(Loc.X + x), (ushort)(Loc.Y + y)).NoAccess)
                                    FoundPlace = false;
                        }
                    }
                    catch { FoundPlace = false; }
                    if (FoundPlace)
                    {
                        Loc.X = (ushort)(Loc.X + x);
                        Loc.Y = (ushort)(Loc.Y + y);
                        break;
                    }
                }
                if (FoundPlace)
                    break;
            }
            if (!FoundPlace)
                return false;
            return true;
        }
        public void Dissappear()
        {
            if (((Hashtable)World.H_Items[Loc.Map]).Contains(UID))
            {
                World.Action(this, Packets.ItemDropRemove(UID, Info.ID, Loc.X, Loc.Y));
                ((Hashtable)World.H_Items[Loc.Map]).Remove(UID);
            }
        }
    }

    public static class World
    {
        public static Character C;
        public static Random Rnd = new Random();

        public static Dictionary<int, GameClient> ClientPool = new Dictionary<int, GameClient>();
        public static Dictionary<uint, Extra.Teams> AllTeams = new Dictionary<uint, Extra.Teams>();
        public static Dictionary<string, PacketHandling.Struct1.Portal> Portals = new Dictionary<string, PacketHandling.Struct1.Portal>();
        public static Dictionary<uint, Extra.Exterminator> DemonExterm = new Dictionary<uint, Extra.Exterminator>();
        public static Dictionary<uint, Features.Struct.Flowers> AllFlowers = new Dictionary<uint, Features.Struct.Flowers>();

        public static Hashtable H_Chars = new Hashtable();
        public static Hashtable H_Mobs = new Hashtable();
        public static Hashtable H_Items = new Hashtable();
        public static Hashtable H_LottoItems = new Hashtable();
        public static Hashtable H_NPCs = new Hashtable();
        public static Hashtable H_PShops = new Hashtable();
        public static Hashtable H_Companions = new Hashtable();

        public static KOInfo[] KOBoard = new KOInfo[500];
        public static EmpireInfo[] EmpireBoard = new EmpireInfo[50];
        public static BroadCastMessage[] BroadCasts = new BroadCastMessage[100];
        public static BroadCastMessage CurrentBC = new BroadCastMessage();
        public static DateTime LastBroadCast = DateTime.Now;
        public static DateTime kosSaved = DateTime.Now;

        public static bool flame10 = false;
        public static bool flamestonequest = false;

        public static byte BroadCastCount = 0;
        public static byte dragonsALIVE = 0;
        public static byte dragon1MSG = 0;
        public static byte dragon2MSG = 0;
        public static ushort MapDimentions = 0;
        public static uint ScreenColor = 0;
        public static int Playersmax = 500;

        public static class ServerInfo
        {
            public static string ServerName = "Heroes";
            public static string ServerIP = "192.168.1.100";
            public static string WebSite = "Http://www.Heroes-C-Online.com";
            public static uint ExperienceRate = 20;
            public static uint ProfExpRate = 5;
            public static uint SkillExpRate = 10;
            public static int AuthPort = 9958;
        }

        public class ClassPkWar
        {
            public static bool ClassPKTime = false;

            public class AddMap
            {
                public static bool InTimeRange = false;
                public static bool timer = false;

                public static Hashtable TrojanMap = new Hashtable();
                public static Hashtable WarriorMap = new Hashtable();
                public static Hashtable ArcherMap = new Hashtable();
                public static Hashtable NinjaMap = new Hashtable();
                public static Hashtable MonkMap = new Hashtable();
                public static Hashtable WaterTaoistMap = new Hashtable();
                public static Hashtable FireTaoistMap = new Hashtable();
                public static Hashtable DarkTaoistMap = new Hashtable();
            }

            public static void RemovePlayersFromWar(Game.Character C)
            {
                {
                    if (Game.World.ClassPkWar.AddMap.TrojanMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.TrojanMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.WarriorMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.WarriorMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.ArcherMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.ArcherMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.NinjaMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.NinjaMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.MonkMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.MonkMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.WaterTaoistMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.WaterTaoistMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.FireTaoistMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.FireTaoistMap.Remove(C.EntityID);

                    if (Game.World.ClassPkWar.AddMap.DarkTaoistMap.Contains(C.EntityID))
                        Game.World.ClassPkWar.AddMap.DarkTaoistMap.Remove(C.EntityID);
                }
            }
        }

        public class WeeklyAndMonthlyPkWar
        {
            public static bool WeeklyWar = false;
            public static bool MonthlyWar = false;

            public class AddMap
            {
                public static bool InTimeRange = false;
                public static bool timer = false;

                public static Hashtable TopWeeklyPK = new Hashtable();

                public static Hashtable TopMonthlyPK = new Hashtable();
            }
            public static void RemovePlayersFromWar(Game.Character C)
            {
                {
                    if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Contains(C.EntityID))
                        Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Remove(C.EntityID);

                    if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Contains(C.EntityID))
                        Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Remove(C.EntityID);

                }
            }
        }

        public class DeathMatch
        {
            public static Hashtable TDMScore = new Hashtable();

            public static bool DeathMatchStart = false;
            public static bool DeathMatchScore = false;

            public static int teamred = 0;
            public static int teamblue = 0;
            public static int teamblack = 0;
            public static int teamwhite = 0;
        }

        public class DisCity
        {
            public static bool DisCityStart = false;
            public static byte PlayerInStage2 = 0;
            public static byte PlayerInStage3 = 0;
        }

        public class SteedTornament
        {
            public static int cps = 0;
            public static bool sr = false;
        }

        public static void pvpclear(Character C)
        {
            C.Apuesta = 0;
            C.Enemigo = "";
            C.Luchando = false;
            C.PkPuntos = 0;
            C.VSmaptogo = 1707;
            C.FreeBattel = false;
            C.VsLimetTimeLastes = 5;
        }
        public static void NewKO(string Name, int KO)
        {
            try
            {
                if (KO > 0)
                {
                    for (int i = 499; i >= 0; i--)
                    {
                        if (KOBoard[i].Name == Name)
                        {
                            if (KO < KOBoard[i].KillCount)
                            {
                                Character C = (Character)CharacterFromName(Name);
                                C.MyClient.LocalMessage(2000, System.Drawing.Color.Red, Name + "You Have Killed " + KO + " Monsters With XP but Your Need More To encrease.");
                                C.MyClient.LocalMessage(2000, System.Drawing.Color.Red, Name + " your KO Rank Your Highst Rank Is " + KOBoard[i].KillCount + " .");

                                return;
                            }
                            else
                            {
                                for (int i2 = i; i2 < 499; i2++)
                                    KOBoard[i2] = KOBoard[i2 + 1];
                            }
                        }
                    }

                    int MyPlace = 500;
                    for (int i = 499; i >= 0; i--)
                    {
                        if (KO >= KOBoard[i].KillCount)
                            MyPlace--;
                    }
                    if (MyPlace < 500)
                    {
                        for (int i = 498; i >= MyPlace; i--)
                            KOBoard[i + 1] = KOBoard[i];
                        KOInfo K = new KOInfo();
                        K.Name = Name;
                        K.KillCount = KO;
                        K.KOID = (uint)Rnd.Next(10000000);
                        KOBoard[MyPlace] = K;
                        WorldMessage("Server", Name + " Has Killed " + KO + " Monsters With XP Skill and ranked " + (MyPlace + 1) + " on the KO board.", 2000, 0, System.Drawing.Color.Red);
                    }
                }
            }
            catch { }
        }
        public static void NewEmpire(Character C)
        {
            try
            {
                if (C.Nobility.Donation >= 3000000)
                {
                    int MyPlace = 50;
                    for (int i = 49; i >= 0; i--)
                    {
                        if (C.Nobility.Donation >= EmpireBoard[i].Donation)
                            MyPlace--;
                    }
                    if (MyPlace < 50)
                    {
                        if (C.Nobility.Donation >= 30000000 && C.Nobility.Donation <= 100000000)
                            C.Nobility.Rank = Ranks.Knight;
                        else if (C.Nobility.Donation >= 100000000 && C.Nobility.Donation <= 200000000)
                            C.Nobility.Rank = Ranks.Baron;
                        else if (C.Nobility.Donation >= 200000000 && C.Nobility.Donation <= 300000000)
                            C.Nobility.Rank = Ranks.Earl;
                        else if (MyPlace >= 15 && MyPlace <= 50)
                            C.Nobility.Rank = Ranks.Duke;
                        else if (MyPlace >= 3 && MyPlace <= 15)
                            C.Nobility.Rank = Ranks.Prince;
                        else if (MyPlace <= 3)
                            C.Nobility.Rank = Ranks.King;

                        for (int i = 0; i < 50; i++)
                            if (EmpireBoard[i].ID == C.EntityID)
                                C.Nobility.ListPlace = i;
                        if (C.Nobility.ListPlace != -1)//if the player already exists in the top
                            for (int i = C.Nobility.ListPlace - 1; i >= MyPlace; i--)
                                EmpireBoard[i + 1] = EmpireBoard[i];//then just push everyone back who WERE before me
                        else
                            for (int i = 48; i >= MyPlace; i--)
                            {
                                EmpireBoard[i + 1] = EmpireBoard[i];
                            }
                        EmpireBoard[MyPlace].ID = C.EntityID;
                        EmpireBoard[MyPlace].Donation = C.Nobility.Donation;
                        EmpireBoard[MyPlace].Name = C.Name;
                        C.Nobility.ListPlace = MyPlace;
                        C.Nobility.Rank = C.Nobility.Rank;

                    }
                }
            }
            catch { }
        }

        public static void WorldMessage(string Name, string Message, ushort ChatType, uint Mesh, System.Drawing.Color Color)
        {
            try
            {
                foreach (DictionaryEntry DE in H_Chars)
                {
                    Character C = (Character)DE.Value;
                    C.MyClient.SendPacket(Packets.ChatMessage(C.MyClient.MessageID, C.Name, "Server", Message, ChatType, C.Mesh, Color));
                }
            }
            catch { }
        }

        public static void Load_Single_Monster(string info, ushort Map, ushort X, ushort Y, ushort Dim, out Mob Mob)
        {
            Hashtable Mobs = new Hashtable();

            Game.Mob M = new Server.Game.Mob(info);
            Mobs.Add(M.MobID, M);

            if (!Game.World.H_Mobs.Contains(Map))
                Game.World.H_Mobs.Add(Map, new Hashtable());
            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Map];

            DMap D = (DMap)DMaps.H_Maps[Map];


            Game.Mob _Mob = new Server.Game.Mob((Game.Mob)Mobs[M.MobID]);
            _Mob.Loc = new Server.Game.Location();
            _Mob.Loc.Map = Map;
            _Mob.Loc.MapDimention = Dim;
            _Mob.Loc.X = X;
            _Mob.Loc.Y = Y;

            while (D != null && D.GetCell(_Mob.Loc.X, _Mob.Loc.Y).NoAccess)
            {
                _Mob.Loc.X = (ushort)Program.Rnd.Next(X - 5, X + 5);
                _Mob.Loc.Y = (ushort)Program.Rnd.Next(Y - 5, Y + 5);
            }
            _Mob.StartLoc = _Mob.Loc;
            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
            while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

            MapMobs.Add(_Mob.EntityID, _Mob);
            //Spawn(M, false);
            Mob = _Mob;

        }
        public static Character CharacterFromName(string Name)
        {
            foreach (Character C in H_Chars.Values)
                if (C.Name == Name)
                    return C;
            return null;
        }
        public static Character CharacterFromNameAndStatus(string Name)
        {
            foreach (Character C in H_Chars.Values)
                if (C.Name + C.MyClient.AuthInfo.Status == Name)
                    return C;
            return null;
        }
        public static NPC NPCFromLoc(Location Loc)
        {
            try
            {
                foreach (NPC N in H_NPCs.Values)
                    if (N.Loc.Map == Loc.Map && N.Loc.X == Loc.X && N.Loc.Y == Loc.Y)
                        return N;
                return null;
            }
            catch { return null; }
        }

        public static void Spawn(Companion M, bool Check)
        {
            try
            {
                byte[] P = Packets.SpawnEntity(M);
                foreach (Character CC in H_Chars.Values)
                    if (CC.Loc.MapDimention == M.Loc.MapDimention && CC.Loc.Map == M.Loc.Map && MyMath.InBox(M.Loc.X, M.Loc.Y, CC.Loc.X, CC.Loc.Y, 16) && (!MyMath.InBox(M.Loc.PreviousX, M.Loc.PreviousY, CC.Loc.X, CC.Loc.Y, 16) || !Check))
                        CC.MyClient.SendPacket(P);
            }
            catch { }
        }
        public static void Spawn(Mob M, bool Check)
        {
            try
            {
                byte[] P = Packets.SpawnEntity(M);
                foreach (Character CC in H_Chars.Values)
                    if (CC.Loc.MapDimention == M.Loc.MapDimention && CC.Loc.Map == M.Loc.Map && MyMath.InBox(M.Loc.X, M.Loc.Y, CC.Loc.X, CC.Loc.Y, 16) && (!MyMath.InBox(M.Loc.PreviousX, M.Loc.PreviousY, CC.Loc.X, CC.Loc.Y, 16) || !Check))
                        CC.MyClient.SendPacket(P);
            }
            catch { }
        }
        public static void Spawn(NPC M)
        {
            try
            {
                byte[] P;
                if (M.CurHP == 0)
                    P = Packets.SpawnNPC(M);
                else
                    P = Packets.SpawnNPCWithHP(M);
                foreach (Character CC in H_Chars.Values)
                    if (CC.Loc.Map == M.Loc.Map && MyMath.InBox(M.Loc.X, M.Loc.Y, CC.Loc.X, CC.Loc.Y, 13))
                    {
                        CC.MyClient.SendPacket(P);
                    }
            }
            catch { }
        }
        public static void Spawn(PacketHandling.MarketShops.Shop M)
        {
            try
            {
                byte[] P = Packets.SpawnNamedNPC(M.NPCInfo, M.Name);
                byte[] P2 = Packets.ChatMessage(26514, M.Owner.Name, "ALL", M.Hawk, 2104, 0, System.Drawing.Color.White);
                foreach (Character CC in H_Chars.Values)
                    if (CC != M.Owner && CC.Loc.Map == M.NPCInfo.Loc.Map && MyMath.InBox(M.NPCInfo.Loc.X, M.NPCInfo.Loc.Y, CC.Loc.X, CC.Loc.Y, 13))
                    {
                        CC.MyClient.SendPacket(P);
                        if (M.Hawk != "")
                            CC.MyClient.SendPacket(P2);
                    }
            }
            catch { }
        }
        public static void Spawn(Character C, bool Check)
        {
            try
            {
                byte[] P = Packets.SpawnEntity(C);
                foreach (Character CC in H_Chars.Values)
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 18) && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, CC.Loc.X, CC.Loc.Y, 18) || !Check))
                    {
                        CC.MyClient.SendPacket(P);
                        try
                        {
                            if (C.MyGuild.Enemies.ContainsKey(CC.MyGuild.GuildID))
                            {
                                C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildEnemies, CC.MyGuild.GuildName));
                            }
                            if (C.MyGuild.Allies.ContainsKey(CC.MyGuild.GuildID))
                            {
                                C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildAllies, CC.MyGuild.GuildName));
                            }
                        }
                        catch { }
                        if (C.MyGuild != null)
                        {
                            C.MyClient.SendPacket(Packets.String(C.MyGuild.GuildID, (byte)StringType.GuildName, C.MyGuild.GuildName));
                        }
                    }
            }
            catch { }
        }
        public static void Spawns(Character C, bool Check)
        {
            try
            {

                byte[] CSpawn = Packets.SpawnEntity(C);
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC != C && CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 18) && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, CC.Loc.X, CC.Loc.Y, 15) || !Check))
                    {
                        C.MyClient.SendPacket(Packets.SpawnEntity(CC));
                        try
                        {
                            if (CC.MyGuild.Enemies.ContainsKey(C.MyGuild.GuildID) && !CC.MyClient.Robot)
                            {
                                C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildEnemies, CC.MyGuild.GuildName));
                            }
                            if (CC.MyGuild.Allies.ContainsKey(CC.MyGuild.GuildID) && !CC.MyClient.Robot)
                            {
                                C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildAllies, CC.MyGuild.GuildName));
                            }
                        }
                        catch { }
                        if (CC.MyGuild != null && !CC.MyClient.Robot)
                        {
                            C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildName, CC.MyGuild.GuildName));
                        }
                        // if (CC.MyClient.Robot)
                        //  C.MyClient.SendPacket(Packets.String(CC.MyGuild.GuildID, (byte)StringType.GuildName, CC.Owner.Name + ",Guard"));
                        CC.MyClient.SendPacket(CSpawn);
                        try
                        {
                            if (C.MyGuild.Enemies.ContainsKey(CC.MyGuild.GuildID) && !C.MyClient.Robot)
                            {
                                CC.MyClient.SendPacket(Packets.String(C.MyGuild.GuildID, (byte)StringType.GuildEnemies, C.MyGuild.GuildName));
                            }
                            if (C.MyGuild.Allies.ContainsKey(CC.MyGuild.GuildID) && !C.MyClient.Robot)
                            {
                                CC.MyClient.SendPacket(Packets.String(C.MyGuild.GuildID, (byte)StringType.GuildAllies, C.MyGuild.GuildName));
                            }
                        }
                        catch { }
                        if (C.MyGuild != null && !C.MyClient.Robot)
                        {
                            CC.MyClient.SendPacket(Packets.String(C.MyGuild.GuildID, (byte)StringType.GuildName, C.MyGuild.GuildName));
                        }
                        //if(C.MyClient.Robot)
                        //    CC.MyClient.SendPacket(Packets.String(C.MyGuild.GuildID, (byte)StringType.GuildName, C.Owner.Name+",Guard"));


                    }//le am azi
                    if (CC.InteractionInProgress && CC.InteractionWith != C.EntityID && CC.InteractionSet)
                    {
                        if (CC.Body == 1003 || CC.Body == 1004)
                        {
                            if (CC.InteractionX == CC.Loc.X && CC.Loc.Y == CC.InteractionY)
                            {
                                C.MyClient.SendPacket(Packets.AttackPacket(CC.EntityID, CC.InteractionWith, CC.Loc.X, CC.Loc.Y, CC.InteractionType, 49));
                            }
                        }
                        else
                        {
                            if (Game.World.H_Chars.ContainsKey(CC.InteractionWith))
                            {
                                Character Cs = Game.World.H_Chars[CC.InteractionWith] as Character;
                                if (Cs.Loc.X == CC.InteractionX && Cs.Loc.Y == CC.InteractionY)
                                {
                                    C.MyClient.SendPacket(Packets.AttackPacket(CC.EntityID, CC.InteractionWith, CC.Loc.X, CC.Loc.Y, CC.InteractionType, 49));
                                }
                            }
                        }
                    }
                }
                Hashtable MapMobs = (Hashtable)H_Mobs[C.Loc.Map];
                if (MapMobs != null)
                {
                    foreach (Mob M in MapMobs.Values)
                        if (M.Alive && MyMath.InBox(C.Loc.X, C.Loc.Y, M.Loc.X, M.Loc.Y, 16) && M.Loc.MapDimention == C.Loc.MapDimention && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, M.Loc.X, M.Loc.Y, 16) || !Check))
                        {
                            C.MyClient.SendPacket(Packets.SpawnEntity(M));
                        }
                }
                Hashtable MapItems = (Hashtable)H_Items[C.Loc.Map];
                if (MapItems != null)
                {
                    foreach (DroppedItem DI in MapItems.Values)
                        if (MyMath.InBox(C.Loc.X, C.Loc.Y, DI.Loc.X, DI.Loc.Y, 14) && DI.Loc.MapDimention == C.Loc.MapDimention && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, DI.Loc.X, DI.Loc.Y, 14) || !Check))
                            C.MyClient.SendPacket(Packets.ItemDrop(DI));
                }
                foreach (NPC N in H_NPCs.Values)
                    if (N.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, N.Loc.X, N.Loc.Y, 14) && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, N.Loc.X, N.Loc.Y, 14) || !Check))
                    {
                        if (N.MaxHP == 0)
                            C.MyClient.SendPacket(Packets.SpawnNPC(N));
                        else
                            C.MyClient.SendPacket(Packets.SpawnNPCWithHP(N));
                    }

                foreach (PacketHandling.MarketShops.Shop S in H_PShops.Values)
                    if (S.NPCInfo.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, S.NPCInfo.Loc.X, S.NPCInfo.Loc.Y, 14) && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, S.NPCInfo.Loc.X, S.NPCInfo.Loc.Y, 14) || !Check))
                    {
                        C.MyClient.SendPacket(Packets.SpawnNamedNPC(S.NPCInfo, S.Name));
                        if (S.Hawk != "")
                            C.MyClient.SendPacket(Packets.ChatMessage(26514, S.Owner.Name, "ALL", S.Hawk, 2104, 0, System.Drawing.Color.White));
                    }
                foreach (Companion Cmp in H_Companions.Values)
                    if ((Cmp.Loc.Map == C.Loc.Map) && MyMath.InBox(C.Loc.X, C.Loc.Y, Cmp.Loc.X, Cmp.Loc.Y, 20) && Cmp.Loc.MapDimention == C.Loc.MapDimention && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, Cmp.Loc.X, Cmp.Loc.Y, 20) || !Check))
                        C.MyClient.SendPacket(Packets.SpawnEntity(Cmp));
                Extra.GuildWars.ThePole.Spawn(C, Check);
                Extra.GuildWars.TheLeftGate.Spawn(C, Check);
                Extra.GuildWars.TheRightGate.Spawn(C, Check);
            }
            catch { }
        }

        public static void Action(NPC C, byte[] Data)
        {
            try
            {
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 16))
                    {
                        CC.MyClient.SendPacket2(Data);
                    }
                }
            }
            catch { }
        }
        public static void Action(Companion C, byte[] Data)
        {
            try
            {
                foreach (Character CC in H_Chars.Values)
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 16))
                        CC.MyClient.SendPacket2(Data);
            }
            catch { }
        }
        public static void Action(Character C, byte[] Data)
        {
            try
            {
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 20))
                        CC.MyClient.SendPacket2(Data);
                }
            }
            catch { }
        }
        public static void Action(Mob C, byte[] Data)
        {
            try
            {
                byte[] P = Data;
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 16))
                    {
                        CC.MyClient.SendPacket2(P);
                    }
                }
            }
            catch { }
        }

        public static void Action(DroppedItem C, byte[] Data)
        {
            try
            {
                byte[] P = Data;
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 16))
                    {
                        CC.MyClient.SendPacket2(P);
                    }
                }
            }
            catch { }
        }
        public static void Action(DroppedItemConfiscator C, byte[] Data)
        {
            try
            {
                byte[] P = Data;
                foreach (Character CC in H_Chars.Values)
                {
                    if (CC.Loc.MapDimention == C.Loc.MapDimention && CC.Loc.Map == C.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, CC.Loc.X, CC.Loc.Y, 16))
                    {
                        CC.MyClient.SendPacket2(P);
                    }
                }
            }
            catch { }
        }

    }
}
