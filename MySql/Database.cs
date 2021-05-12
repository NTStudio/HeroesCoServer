using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using Server.Game;
using System.Data;

namespace Server
{
    public struct CompanionInfo
    {
        public uint MinAttack;
        public uint MaxAttack;
        public ushort Defence;
        public byte Level;
        public uint SkillUses;
        public ushort HP;
        public uint Mesh;
        public string Name;
    }
    public struct Shop
    {
        public uint ShopID;
        public byte Type;
        public byte MoneyType;
        public byte ItemAmount;
        public ArrayList Items;
    }
    public struct SkillLearn
    {
        public ushort ID;
        public byte Lvl;
        public byte LevelReq;

        public Game.Skill ToSkill()
        {
            Game.Skill S = new Game.Skill();
            S.ID = ID;
            S.Lvl = Lvl;
            S.Exp = 0;
            return S;
        }
    }
    public struct DatabasePlusItem
    {
        public uint ID;
        public byte Plus;
        public ushort HP;
        public uint MinAttack;
        public uint MaxAttack;
        public ushort Defense;
        public ushort MagicAttack;
        public ushort MagicDefense;
        public ushort Agility;//Vigor Add
        public byte Dodge;//Or Add Ride Speed

        public void ReadThis(string Line)
        {
            string[] Info = Line.Split(' ');
            ID = uint.Parse(Info[0]);
            Plus = byte.Parse(Info[1]);
            HP = ushort.Parse(Info[2]);
            MinAttack = uint.Parse(Info[3]);
            MaxAttack = uint.Parse(Info[4]);
            Defense = ushort.Parse(Info[5]);
            MagicAttack = ushort.Parse(Info[6]);
            MagicDefense = ushort.Parse(Info[7]);
            Agility = ushort.Parse(Info[8]);
            Dodge = byte.Parse(Info[9]);
        }
    }
    public struct DatabaseItem
    {
        public uint ID;
        public string Name;
        public byte Class;
        public byte Prof;
        public byte Level;
        public byte Job;
        public ushort StrNeed;
        public ushort AgiNeed;
        public uint Worth;
        public ushort MinAtk;
        public ushort MaxAtk;
        public uint Defense;
        public uint MagicDefense;
        public uint MagicAttack;
        public byte Dodge;
        public byte AgilityGives;
        public uint CPsWorth;
        public ushort Durability;
        public ushort HPAdd;
        public ushort MPAdd;

        public void WriteThis(BinaryWriter BW)
        {
            BW.Write(ID);
            BW.Write(Name);
            BW.Write(Class);
            BW.Write(Prof);
            BW.Write(Level);
            BW.Write(Job);
            BW.Write(StrNeed);
            BW.Write(AgiNeed);
            BW.Write(Worth);
            BW.Write(MinAtk);
            BW.Write(MaxAtk);
            BW.Write(Defense);
            BW.Write(MagicDefense);
            BW.Write(MagicAttack);
            BW.Write(Dodge);
            BW.Write(AgilityGives);
            BW.Write(CPsWorth);
            BW.Write(Durability);
        }
        public void ReadThis(BinaryReader BR)
        {
            ID = BR.ReadUInt32();
            Name = BR.ReadString();
            Class = BR.ReadByte();
            Prof = BR.ReadByte();
            Level = BR.ReadByte();
            Job = BR.ReadByte();
            StrNeed = BR.ReadUInt16();
            AgiNeed = BR.ReadUInt16();
            Worth = BR.ReadUInt32();
            MinAtk = BR.ReadUInt16();
            MaxAtk = BR.ReadUInt16();
            Defense = BR.ReadUInt32();
            MagicDefense = BR.ReadUInt32();
            MagicAttack = BR.ReadUInt32();
            Dodge = BR.ReadByte();
            AgilityGives = BR.ReadByte();
            CPsWorth = BR.ReadUInt32();
            Durability = BR.ReadUInt16();
        }
    }

    public class Database
    {
        #region CreateConnection
        public static void CreateConnection(string database, string user, string password)
        {
            mysqlDatabase = database;
            mysqlUser = user;
            mysqlPassword = password;
        }

        public static string mysqlDatabase = "";
        public static string mysqlUser = "";
        public static string mysqlPassword = "";

        private static MySqlConnection mySqlConnection;
        public static MySqlConnection MySqlConnection
        {
            get
            {
                if (mySqlConnection == null)
                    mySqlConnection = new MySqlConnection("Server=127.0.0.1;Database='" + mysqlDatabase + "';Username='" + mysqlUser + "';Password='" + mysqlPassword + "';");
                return mySqlConnection;
            }
        }
        #endregion

        public Random Rndom = new Random();
        public static ushort[][] RevPoints;
        public static ushort[][] Portals;
        public static Hashtable DatabaseItems;
        public static Hashtable DatabasePlusItems;
        public static Hashtable Shops;
        public static uint[] ProfExp;
        public static ulong[] LevelExp;
        public static int npcss = 0;
        public static int monst = 0;
        public static Hashtable DefaultCoords = new Hashtable();
        public static Hashtable SkillForLearning = new Hashtable();
        public static ushort[] StonePts = new ushort[9] { 0, 10, 40, 120, 360, 1080, 3240, 9720, 29160 };
        public static ushort[] ComposePts = new ushort[13] { 20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700, 5500, 9000, 33000 };
        public static ushort[] SocPlusExtra = new ushort[9] { 6, 30, 70, 240, 740, 2240, 6670, 20000, 60000 };
        public static ArrayList GWOn = new ArrayList() { 0, 3, 6, 9, 12, 15, 18, 21 };
        public static Hashtable CompanionInfos = new Hashtable();
        private static Dictionary<byte, string> ArcherStats = new Dictionary<byte, string>();
        private static Dictionary<byte, string> NinjaStats = new Dictionary<byte, string>();
        private static Dictionary<byte, string> MonkStats = new Dictionary<byte, string>();
        private static Dictionary<byte, string> WarriorStats = new Dictionary<byte, string>();
        private static Dictionary<byte, string> TrojanStats = new Dictionary<byte, string>();
        private static Dictionary<byte, string> TaoistStats = new Dictionary<byte, string>();

        public static void Dispose()
        {
            RevPoints = null;
            Portals = null;
            DatabaseItems.Clear();
            DatabasePlusItems.Clear();
            Shops.Clear();
            ProfExp = null;
            LevelExp = null;
            DefaultCoords.Clear();
            SkillForLearning.Clear();
            StonePts = null;
            ComposePts = null;
            SocPlusExtra = null;
            GWOn.Clear();
            CompanionInfos.Clear();
            ArcherStats.Clear();
            NinjaStats.Clear();
            MonkStats.Clear();
            WarriorStats.Clear();
            TrojanStats.Clear();
            TaoistStats.Clear();
        }
        public static void LoadCompanions()
        {
            if (File.Exists(Program.ConquerPath + @"Companions.txt"))
            {
                string[] Lines = File.ReadAllLines(Program.ConquerPath + @"Companions.txt");

                foreach (string Line in Lines)
                {
                    string[] Info = Line.Split(' ');
                    CompanionInfo C = new CompanionInfo();
                    uint Type = uint.Parse(Info[0]);
                    C.MinAttack = uint.Parse(Info[1]);
                    C.MaxAttack = uint.Parse(Info[2]);
                    C.Defence = ushort.Parse(Info[3]);
                    C.Level = byte.Parse(Info[4]);
                    C.SkillUses = uint.Parse(Info[5]);
                    C.HP = ushort.Parse(Info[6]);
                    C.Mesh = uint.Parse(Info[7]);
                    C.Name = Info[8];
                    CompanionInfos.Add(Type, C);
                }
            }
            Console.WriteLine("Guards Loaded");
        }
        public static void LoadPromotedSkills()
        {
            int skillcount = 0;
            ArrayList Warrior = new ArrayList();
            Warrior.Add(new SkillLearn() { ID = (ushort)1025, LevelReq = (byte)3 }); skillcount++;//Superman[XP]
            Warrior.Add(new SkillLearn() { ID = (ushort)1015, LevelReq = (byte)15 }); skillcount++;//Accuracy[XP]
            Warrior.Add(new SkillLearn() { ID = (ushort)1020, LevelReq = (byte)15 }); skillcount++;//Shield[XP]
            Warrior.Add(new SkillLearn() { ID = (ushort)1040, LevelReq = (byte)15 }); skillcount++;//Roar[XP]
            Warrior.Add(new SkillLearn() { ID = (ushort)10470, LevelReq = (byte)40 }); skillcount++;//ShieldBlock
            Warrior.Add(new SkillLearn() { ID = (ushort)1051, LevelReq = (byte)60 }); skillcount++;//Dash
            SkillForLearning.Add((byte)2, Warrior);

            //ArrayList Trojan = new ArrayList();
            //Trojan.Add(new SkillLearn() { ID = (ushort)1110, LevelReq = (byte)3 }); skillcount++;//Cyclone[XP]
            //Trojan.Add(new SkillLearn() { ID = (ushort)1015, LevelReq = (byte)15 }); skillcount++;//Accuracy[XP]
            //Trojan.Add(new SkillLearn() { ID = (ushort)1115, LevelReq = (byte)40 }); skillcount++;//Hercules
            //Trojan.Add(new SkillLearn() { ID = (ushort)1190, LevelReq = (byte)40 }); skillcount++;//SpiritHealing
            //Trojan.Add(new SkillLearn() { ID = (ushort)1270, LevelReq = (byte)40 }); skillcount++;//Golem[XP]
            //SkillForLearning.Add((byte)1, Trojan);

            ArrayList Archer = new ArrayList();
            Archer.Add(new SkillLearn() { ID = (ushort)8002, LevelReq = (byte)3 }); skillcount++;//Fly[XP]
            Archer.Add(new SkillLearn() { ID = (ushort)8001, LevelReq = (byte)25 }); skillcount++;//ScatterFire
            Archer.Add(new SkillLearn() { ID = (ushort)8000, LevelReq = (byte)45 }); skillcount++;//RapidFire
            Archer.Add(new SkillLearn() { ID = (ushort)8003, LevelReq = (byte)70 }); skillcount++;//AdvanceFly
            Archer.Add(new SkillLearn() { ID = (ushort)8030, LevelReq = (byte)70 }); skillcount++;//ArrowRain[XP]
            Archer.Add(new SkillLearn() { ID = (ushort)9000, LevelReq = (byte)75 }); skillcount++;//Intensify
            SkillForLearning.Add((byte)4, Archer);

            ArrayList Ninja = new ArrayList();
            Ninja.Add(new SkillLearn() { ID = (ushort)6011, LevelReq = (byte)3 }); skillcount++;//FatalStrike[XP]
            Ninja.Add(new SkillLearn() { ID = (ushort)6000, LevelReq = (byte)40 }); skillcount++;//TwofoldBlades
            Ninja.Add(new SkillLearn() { ID = (ushort)6010, LevelReq = (byte)40 }); skillcount++;//ShurikenVortex[XP]
            Ninja.Add(new SkillLearn() { ID = (ushort)6001, LevelReq = (byte)70 }); skillcount++;//ToxicFog
            Ninja.Add(new SkillLearn() { ID = (ushort)6004, LevelReq = (byte)110 }); skillcount++;//ArcherBane
            SkillForLearning.Add((byte)5, Ninja);

            ArrayList Monk = new ArrayList();
            Monk.Add(new SkillLearn() { ID = (ushort)10390, LevelReq = (byte)3 }); skillcount++;//Oblivion[XP]
            Monk.Add(new SkillLearn() { ID = (ushort)10415, LevelReq = (byte)15 }); skillcount++;//WhirlwindKick
            Monk.Add(new SkillLearn() { ID = (ushort)10395, LevelReq = (byte)20 }); skillcount++;//TyrantAura
            Monk.Add(new SkillLearn() { ID = (ushort)10410, LevelReq = (byte)20 }); skillcount++;//FendAura
            Monk.Add(new SkillLearn() { ID = (ushort)10381, LevelReq = (byte)40 }); skillcount++;//RadiantPalm
            Monk.Add(new SkillLearn() { ID = (ushort)10400, LevelReq = (byte)40 }); skillcount++;//Serenity
            Monk.Add(new SkillLearn() { ID = (ushort)10425, LevelReq = (byte)70 }); skillcount++;//Tranquility
            Monk.Add(new SkillLearn() { ID = (ushort)10430, LevelReq = (byte)100 }); skillcount++;//Compassion
            SkillForLearning.Add((byte)6, Monk);

            ArrayList Taoist = new ArrayList();
            Taoist.Add(new SkillLearn() { ID = (ushort)1000, LevelReq = (byte)1 }); skillcount++;//Thunder
            Taoist.Add(new SkillLearn() { ID = (ushort)1005, LevelReq = (byte)1 }); skillcount++;//Cure
            Taoist.Add(new SkillLearn() { ID = (ushort)1010, LevelReq = (byte)3 }); skillcount++;//Lightning[XP]
            Taoist.Add(new SkillLearn() { ID = (ushort)1125, LevelReq = (byte)15 }); skillcount++;//Volcano[XP]
            SkillForLearning.Add((byte)10, Taoist);

            ArrayList WaterTaoist = new ArrayList();
            //WaterTaoist.Add(new SkillLearn() { ID = (ushort)1000, LevelReq = (byte)1 }); skillcount++;//Thunder
            //WaterTaoist.Add(new SkillLearn() { ID = (ushort)1005, LevelReq = (byte)1 }); skillcount++;//Cure
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1010, LevelReq = (byte)3 }); skillcount++;//Lightning[XP]
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1125, LevelReq = (byte)15 }); skillcount++;//Volcano[XP]
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1001, LevelReq = (byte)40 }); skillcount++;//Fire
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1195, LevelReq = (byte)45 }); skillcount++;//Meditation

            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1095, LevelReq = (byte)40 }); skillcount++;//Stigma
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1090, LevelReq = (byte)40 }); skillcount++;//MagicShield
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1055, LevelReq = (byte)40 }); skillcount++;//HealingRain
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1050, LevelReq = (byte)45 }); skillcount++;//Revive[XP]
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1085, LevelReq = (byte)50 }); skillcount++;//StarofAccuracy
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1280, LevelReq = (byte)50 }); skillcount++;//WaterElf[XP]
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1075, LevelReq = (byte)60 }); skillcount++;//Invisibility
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1100, LevelReq = (byte)70 }); skillcount++;//Pray
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1175, LevelReq = (byte)80 }); skillcount++;//AdvancedCure
            WaterTaoist.Add(new SkillLearn() { ID = (ushort)1170, LevelReq = (byte)95 }); skillcount++;//Nectar
            SkillForLearning.Add((byte)13, WaterTaoist);

            ArrayList FireTaoist = new ArrayList();
            //FireTaoist.Add(new SkillLearn() { ID = (ushort)1000, LevelReq = (byte)1 }); skillcount++;//Thunder
            //FireTaoist.Add(new SkillLearn() { ID = (ushort)1005, LevelReq = (byte)1 }); skillcount++;//Cure
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1010, LevelReq = (byte)3 }); skillcount++;//Lightning[XP]
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1125, LevelReq = (byte)15 }); skillcount++;//Volcano[XP]
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1001, LevelReq = (byte)40 }); skillcount++;//Fire
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1195, LevelReq = (byte)45 }); skillcount++;//Meditation

            FireTaoist.Add(new SkillLearn() { ID = (ushort)1180, LevelReq = (byte)50 }); skillcount++;//FireMeteor
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1150, LevelReq = (byte)55 }); skillcount++;//FireBall
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1120, LevelReq = (byte)65 }); skillcount++;//FireCircle
            FireTaoist.Add(new SkillLearn() { ID = (ushort)5001, LevelReq = (byte)70 }); skillcount++;//SpeedLightning[XP]
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1160, LevelReq = (byte)80 }); skillcount++;//Bomb
            FireTaoist.Add(new SkillLearn() { ID = (ushort)1165, LevelReq = (byte)85 }); skillcount++;//FireofHell
            SkillForLearning.Add((byte)14, FireTaoist);

            Console.WriteLine("Promoted Skills Loaded");
        }
        public static void SaveKOs()
        {
            FileStream FS = new FileStream(Program.ConquerPath + @"KOBoard.dat", FileMode.OpenOrCreate);
            BinaryWriter BW = new BinaryWriter(FS);
            short lenghth = 0;
            for (int i = 0; i < Game.World.KOBoard.Length; i++)
                if (Game.World.KOBoard[i].Name != "")
                    lenghth++;
            BW.Write(lenghth);
            for (int i2 = 0; i2 < lenghth; i2++)
                Game.World.KOBoard[i2].WriteThis(BW);
            BW.Close();
            FS.Close();
        }
        public static void LoadKOs()
        {
            if (System.IO.File.Exists(Program.ConquerPath + @"KOBoard.dat"))
            {
                FileStream FS = new FileStream(Program.ConquerPath + @"KOBoard.dat", FileMode.Open);
                BinaryReader BR = new BinaryReader(FS);
                short leghnth = BR.ReadInt16();
                for (int i = 0; i < leghnth; i++)
                {
                    Game.World.KOBoard[i].ReadThis(BR);
                    //Console.WriteLine("KOs " + i);

                }
                BR.Close();
                FS.Close();
                Console.WriteLine("KOs Loaded");
            }
        }
        public static void SaveEmpire()
        {
            try
            {
                FileStream FS = new FileStream(Program.ConquerPath + @"Nobility.dat", FileMode.OpenOrCreate);
                BinaryWriter BW = new BinaryWriter(FS);

                for (int i = 0; i < Game.World.EmpireBoard.Length; i++)
                    Game.World.EmpireBoard[i].WriteThis(BW);

                BW.Close();
                FS.Close();
            }
            catch { }
        }
        public static void LoadEmpire()
        {
            try
            {
                if (System.IO.File.Exists(Program.ConquerPath + @"Nobility.dat"))
                {
                    FileStream FS = new FileStream(Program.ConquerPath + @"Nobility.dat", FileMode.Open);
                    BinaryReader BR = new BinaryReader(FS);
                    int nobiliticount = 0;

                    for (int i = 0; i < Game.World.EmpireBoard.Length; i++)
                        Game.World.EmpireBoard[i].ReadThis(BR);
                    nobiliticount++;
                    BR.Close();
                    FS.Close();
                    Console.WriteLine("Empire Loaded");
                }
            }
            catch { }
        }
        public static void LoadShops()
        {
            Shops = new Hashtable();

            IniFile I = new IniFile(Program.ConquerPath + @"Shop.dat");
            int ShopAmount = I.ReadInt32("Header", "Amount");

            for (int i = 0; i < ShopAmount; i++)
            {
                Shop S = new Shop();
                S.ShopID = I.ReadUInt32("Shop" + i.ToString(), "ID");
                S.Type = I.ReadByte("Shop" + i.ToString(), "Type");
                S.MoneyType = I.ReadByte("Shop" + i.ToString(), "MoneyType");
                S.ItemAmount = I.ReadByte("Shop" + i.ToString(), "ItemAmount");
                S.Items = new ArrayList(S.ItemAmount);
                for (int e = 0; e < S.ItemAmount; e++)
                    S.Items.Add(I.ReadUInt32("Shop" + i.ToString(), "Item" + e.ToString()));

                Shops.Add(S.ShopID, S);
            }
            I.Close();
            Console.WriteLine("Shops Loaded");
        }
        public static void LoadItems()
        {
            if (File.Exists(Program.ConquerPath + @"Items\itemtype.txt"))
            {
                int itemcount = 0;
                //int start = System.Environment.TickCount;
                TextReader TR = new StreamReader(Program.ConquerPath + @"Items\itemtype.txt");
                string Items = TR.ReadToEnd();
                TR.Close();
                DatabaseItems = new Hashtable();
                Items = Items.Replace("\r", "");
                string[] AllItems = Items.Split('\n');
                foreach (string _item in AllItems)
                {
                    string _item_ = _item.Trim();
                    if (_item_.IndexOf("//", 0, 2) != 0)
                    {
                        string[] data = _item_.Split(' ');
                        if (data.Length == 40)
                        {
                            try
                            {
                                DatabaseItem NewItem = new DatabaseItem();
                                NewItem.ID = Convert.ToUInt32(data[0]);
                                NewItem.Name = data[1].Trim();
                                NewItem.Class = Convert.ToByte(data[2]);
                                NewItem.Prof = Convert.ToByte(data[3]);
                                NewItem.Level = Convert.ToByte(data[4]);
                                NewItem.Job = Convert.ToByte(data[5]);
                                NewItem.StrNeed = Convert.ToUInt16(data[6]);
                                NewItem.AgiNeed = Convert.ToUInt16(data[7]);

                                //uint test = Convert.ToUInt32(data[8]);
                                //test = Convert.ToUInt32(data[9]);

                                //test = Convert.ToUInt32(data[10]);
                                //test = Convert.ToUInt32(data[11]);

                                NewItem.Worth = Convert.ToUInt32(data[12]);
                                NewItem.MaxAtk = Convert.ToUInt16(data[14]);
                                NewItem.MinAtk = Convert.ToUInt16(data[15]);
                                NewItem.Defense = Convert.ToUInt32(data[16]);
                                NewItem.AgilityGives = Convert.ToByte(data[17]);
                                NewItem.Dodge = Convert.ToByte(data[18]);
                                NewItem.HPAdd = Convert.ToUInt16(data[19]);
                                NewItem.MPAdd = Convert.ToUInt16(data[20]);
                                NewItem.Durability = Convert.ToUInt16(data[22]);

                                //test = Convert.ToUInt32(data[23]);
                                //test = Convert.ToUInt32(data[24]);
                                //test = Convert.ToUInt32(data[25]);
                                //test = Convert.ToUInt32(data[26]);
                                //test = Convert.ToUInt32(data[27]);
                                //test = Convert.ToUInt32(data[28]);

                                NewItem.MagicAttack = Convert.ToUInt32(data[29]);
                                NewItem.MagicDefense = Convert.ToUInt32(data[30]);

                                //test = Convert.ToUInt32(data[31]);
                                //test = Convert.ToUInt32(data[32]);
                                //test = Convert.ToUInt32(data[33]);
                                //test = Convert.ToUInt32(data[34]);
                                //test = Convert.ToUInt32(data[35]);

                                NewItem.CPsWorth = Convert.ToUInt32(data[36]);

                                //string Tes = (data[37]);
                                //Tes = (data[38]);
                                //test = Convert.ToUInt32(data[39]);

                                DatabaseItems.Add(NewItem.ID, NewItem);
                                itemcount++;
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error [" + data[0] + "] | [" + data.Length + "]");
                        }
                    }
                }
                Console.WriteLine("Items Loaded " + itemcount);
            }
        }
        public static void LoadItemsPlus()
        {
            string[] ItemAdd = File.ReadAllLines(Program.ConquerPath + @"Items\ItemAdd.ini");
            DatabasePlusItems = new Hashtable();
            int countplus = 0;

            foreach (string S in ItemAdd)
            {
                DatabasePlusItem I = new DatabasePlusItem();
                I.ReadThis(S);
                DatabasePlusItems.Add(I.ID.ToString() + I.Plus.ToString(), I);
                countplus++;
            }
            Console.WriteLine("ItemsPlusInformation Loaded");
        }
        public static void LoadLevelExp()
        {
            LevelExp = new ulong[141];
            LevelExp[0] = 0;
            string[] exp = System.IO.File.ReadAllLines(Program.ConquerPath + @"ExpNeed.txt");

            for (byte i = 1; i < exp.Length; i++)
            {
                LevelExp[i] = UInt32.Parse(exp[i]);
            }
            //LevelExp[130] = 8589134588;
            LevelExp[131] = 25767403764;
            LevelExp[132] = 77302211292;
            LevelExp[133] = 231906633876;
            LevelExp[134] = 347859950814;
            LevelExp[135] = 347859950814;
            LevelExp[136] = 782684889332;
            LevelExp[137] = 1174027333998;
            LevelExp[138] = 1174027333998;
            LevelExp[139] = 1174027333998;
            LevelExp[140] = 1174027333998;

            Console.WriteLine("LevelEXP Loaded");
        }
        public static void LoadProfExp()
        {
            ProfExp = new uint[20];
            ProfExp[0] = 0;
            ProfExp[1] = 100;
            ProfExp[2] = 600;
            ProfExp[3] = 250;
            ProfExp[4] = 6400;
            ProfExp[5] = 1600;
            ProfExp[6] = 4000;
            ProfExp[7] = 1000;
            ProfExp[8] = 22000;
            ProfExp[9] = 4000;
            ProfExp[10] = 10000;
            ProfExp[11] = 95000;
            ProfExp[12] = 14000;
            ProfExp[13] = 21000;
            ProfExp[14] = 32500;
            ProfExp[15] = 48000;
            ProfExp[16] = 721250;
            ProfExp[17] = 108275;
            ProfExp[18] = 162363;
            ProfExp[19] = 210000;
            Console.WriteLine("ItemsProfEXP Loaded");
        }
        public static void LoadRevivePoints()
        {
            RevPoints = new ushort[32][];
            RevPoints[0] = new ushort[4] { 1002, 1002, 430, 380 };
            RevPoints[1] = new ushort[4] { 1005, 1005, 50, 50 };
            RevPoints[2] = new ushort[4] { 1006, 1002, 430, 380 };
            RevPoints[3] = new ushort[4] { 1008, 1002, 430, 380 };
            RevPoints[4] = new ushort[4] { 1009, 1002, 430, 380 };
            RevPoints[5] = new ushort[4] { 1010, 1002, 430, 380 };
            RevPoints[6] = new ushort[4] { 1007, 1002, 430, 380 };
            RevPoints[7] = new ushort[4] { 1004, 1002, 430, 380 };
            RevPoints[8] = new ushort[4] { 1028, 1002, 430, 380 };
            RevPoints[9] = new ushort[4] { 1037, 1002, 430, 380 };
            RevPoints[10] = new ushort[4] { 1038, 1002, 438, 398 };
            RevPoints[11] = new ushort[4] { 1015, 1015, 717, 577 };
            RevPoints[12] = new ushort[4] { 1001, 1000, 499, 650 };
            RevPoints[13] = new ushort[4] { 1000, 1000, 499, 650 };
            RevPoints[14] = new ushort[4] { 1013, 1011, 193, 266 };
            RevPoints[15] = new ushort[4] { 1011, 1011, 193, 266 };
            RevPoints[16] = new ushort[4] { 1076, 1011, 193, 266 };
            RevPoints[17] = new ushort[4] { 1014, 1011, 193, 266 };
            RevPoints[18] = new ushort[4] { 1020, 1020, 566, 562 };
            RevPoints[19] = new ushort[4] { 1075, 1020, 566, 656 };
            RevPoints[20] = new ushort[4] { 1012, 1020, 566, 656 };
            RevPoints[21] = new ushort[4] { 6000, 6000, 29, 73 };
            RevPoints[22] = new ushort[4] { 1730, 1002, 430, 380 };
            RevPoints[23] = new ushort[4] { 1731, 1002, 430, 380 };
            RevPoints[24] = new ushort[4] { 1732, 1002, 430, 380 };
            RevPoints[25] = new ushort[4] { 1733, 1002, 430, 380 };
            RevPoints[26] = new ushort[4] { 1734, 1002, 430, 380 };
            RevPoints[27] = new ushort[4] { 1735, 1002, 430, 380 };
            RevPoints[28] = new ushort[4] { 1099, 1002, 430, 380 };
            RevPoints[29] = new ushort[4] { 1037, 1037, 243, 247 };
            RevPoints[30] = new ushort[4] { 1021, 1037, 243, 247 };
            RevPoints[31] = new ushort[4] { 1700, 1700, 619, 643 };
            Console.WriteLine("Maps RevivePoints Loaded");
        }
        public static void LoadNPCs()
        {
            string[] FNPCs = File.ReadAllLines(Program.ConquerPath + @"NPCs.txt");
            int lincount = 0;
            foreach (string Line in FNPCs)
            {
                lincount++;
                string[] Info = Line.Split(' ');
                if (!Line.StartsWith("#"))
                {
                    Game.NPC N = new Server.Game.NPC(Line);
                    Game.World.H_NPCs.Add(N.EntityID, N);
                    npcss++;
                }
            }
            Console.WriteLine("NPCs Loaded");
            FNPCs = null;
        }
        public static void LoadPortals()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("portals");
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                PacketHandling.Struct1.Portal Port = new PacketHandling.Struct1.Portal();
                Port.ID = d.ReadInt32("PortalID");
                Port.StartMap = d.ReadInt32("StartMap");
                Port.StartX = d.ReadInt32("StartX");
                Port.StartY = d.ReadInt32("StartY");
                Port.EndMap = d.ReadInt32("EndMap");
                Port.EndX = d.ReadInt32("EndX");
                Port.EndY = d.ReadInt32("EndY");
                string PID = Port.StartX + "," + Port.StartY + "," + Port.StartMap;
                if (!World.Portals.ContainsKey(PID))
                    World.Portals.Add(PID, Port);
                else
                {
                    int a = 0;
                    while (a < 10)
                    {
                    }
                }
            }
            Console.WriteLine("Maps Portals Loaded");
        }
        public static void LoadDefaultCoords()
        {
            DefaultCoords.Add((ushort)1002, new Game.Vector2() { X = 430, Y = 380 });
            DefaultCoords.Add((ushort)1015, new Game.Vector2() { X = 717, Y = 571 });
            DefaultCoords.Add((ushort)1000, new Game.Vector2() { X = 500, Y = 650 });
            DefaultCoords.Add((ushort)1011, new Game.Vector2() { X = 188, Y = 264 });
            DefaultCoords.Add((ushort)1020, new Game.Vector2() { X = 565, Y = 562 });

            Console.WriteLine("Maps DefaultCoords Loaded");
        }
        public static void LoadLottoItems()
        {
            string[] Lotto = System.IO.File.ReadAllLines(Program.ConquerPath + @"Lotto.txt");
            int lotericount = 0;
            for (short Cur = 0; Cur < Lotto.Length; Cur++)
            {
                if (Lotto[Cur] != null && Lotto[Cur] != "")
                {
                    string[] Item = Lotto[Cur].Split(',');
                    Game.Item TheItem = new Server.Game.Item();
                    TheItem.ID = uint.Parse(Item[0]);
                    TheItem.Plus = byte.Parse(Item[1]);
                    TheItem.Soc1 = (Server.Game.Item.Gem)byte.Parse(Item[2]);
                    TheItem.Soc2 = (Server.Game.Item.Gem)byte.Parse(Item[3]);
                    if (DatabaseItems.ContainsKey(TheItem.ID))
                    {
                        DatabaseItem DI = (DatabaseItem)DatabaseItems[TheItem.ID];
                        TheItem.CurDur = TheItem.MaxDur = DI.Durability;
                        Game.World.H_LottoItems.Add(Cur, TheItem);
                        lotericount++;
                    }
                }
            }
            Console.WriteLine("LotteryItems Loaded");
        }
        public static void LoadPKStatus(Game.Character C)
        {
            if (C.PKPoints > 99 && C.PKPoints < 200)
            {
                C.StatEff.Add(StatusEffectEn.RedName); return;
            }
            if (C.PKPoints > 199)
                C.StatEff.Add(StatusEffectEn.BlackName);
            return;
        }

        public static void LoadEnemys(Character C)
        {
            C.Enemies = new Hashtable();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("Enemys").Where("EntityID", C.EntityID);
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                Game.Enemy F = new Game.Enemy();
                F.UID = d.ReadUInt32("EnemyID");
                F.Name = d.ReadString("EnemyName");
                if (!C.Enemies.Contains(F.UID))
                    C.Enemies.Add(F.UID, F);
            }
        }
        public static void SaveEnemys(uint uid, string name, Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("Enemy").Insert("EntityID", C.EntityID).Insert("EnemyName", name).Insert("UID", uid).Execute();
        }
        public static void DeleteEnemys(uint UID, Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("Enemy", "EntityID", C.EntityID).And("UID", UID).Execute();
        }
        public static void LoadFriends(Character C)
        {
            C.Friends = new Hashtable();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("Friends").Where("EntityID", C.EntityID);
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                Game.Friend F = new Game.Friend();
                F.UID = d.ReadUInt32("FriendID");
                F.Name = d.ReadString("FriendName");
                if (!C.Friends.Contains(F.UID))
                    C.Friends.Add(F.UID, F);
            }

        }
        public static void SaveFriends(Friend F, Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("Friends").Insert("EntityID", C.EntityID).Insert("FriendName", F.Name).Insert("UID", F.UID).Execute();

        }
        public static void DeleteFriends(Friend F, Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("Friends", "EntityID", F.UID).And("UID", C.EntityID).Execute();
            MySqlCommand Cmds = new MySqlCommand(MySqlCommandType.DELETE);
            Cmds.Delete("Friends", "EntityID", C.EntityID).And("UID", F.UID).Execute();
        }

        public static void LoadMonsters()
        {
            string[] FMobs = File.ReadAllLines(Program.ConquerPath + @"Monsters\MonstersID.txt");
            Hashtable Mobs = new Hashtable(FMobs.Length);
            for (int i = 0; i < FMobs.Length; i++)
            {
                if (!FMobs[i].StartsWith("#"))
                {
                    Game.Mob M = new Game.Mob(FMobs[i]);
                    Mobs.Add(M.MobID, M);
                }
            }
            Console.WriteLine("MonstersID Done");

            string[] FSpawns = File.ReadAllLines(Program.ConquerPath + @"Monsters\MonstersSpawns.txt");
            foreach (string Spawn in FSpawns)
            {
                if (!Spawn.StartsWith("#"))
                {
                    string[] SpawnInfo = Spawn.Split(' ');
                    int MobID = int.Parse(SpawnInfo[0]);
                    int Count = int.Parse(SpawnInfo[1]);
                    ushort Map = ushort.Parse(SpawnInfo[2]);
                    ushort XFrom = ushort.Parse(SpawnInfo[3]);
                    ushort YFrom = ushort.Parse(SpawnInfo[4]);
                    ushort XTo = ushort.Parse(SpawnInfo[5]);
                    ushort YTo = ushort.Parse(SpawnInfo[6]);

                    if (!Game.World.H_Mobs.Contains(Map))
                        Game.World.H_Mobs.Add(Map, new Hashtable());
                    Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Map];

                    DMap D = (DMap)DMaps.H_Maps[Map];

                    for (int i = 0; i < Count; i++)
                    {
                        Game.Mob _Mob = new Server.Game.Mob((Game.Mob)Mobs[MobID]);
                        _Mob.Loc = new Server.Game.Location();
                        _Mob.Loc.Map = Map;
                        _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                        _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));

                        while (D != null && D.GetCell(_Mob.Loc.X, _Mob.Loc.Y).NoAccess)
                        {
                            _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                            _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));
                        }

                        _Mob.StartLoc = _Mob.Loc;
                        _Mob.EntityID = (uint)Program.Rnd.Next(400000, 600000);
                        while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 700000);

                        MapMobs.Add(_Mob.EntityID, _Mob);
                        monst++;
                    }
                }
            }
            Console.WriteLine("MonstersSpawns Done");
        }
        public static void LoadDragons()
        {
            string[] FMobs = File.ReadAllLines(Program.ConquerPath + @"Dragons\DragonsID.txt");
            Hashtable Mobs = new Hashtable(FMobs.Length);
            for (int i = 0; i < FMobs.Length; i++)
            {
                if (!FMobs[i].StartsWith("#"))
                {
                    Game.Mob M = new Server.Game.Mob(FMobs[i]);
                    Mobs.Add(M.MobID, M);
                }
            }
            Console.WriteLine("DragonsID Done");
            int monst = 0;

            string[] FSpawns = File.ReadAllLines(Program.ConquerPath + @"Dragons\DragonsSpawns.txt");
            foreach (string Spawn in FSpawns)
            {
                if (!Spawn.StartsWith("#"))
                {
                    string[] SpawnInfo = Spawn.Split(' ');
                    int MobID = int.Parse(SpawnInfo[0]);
                    int Count = int.Parse(SpawnInfo[1]);
                    ushort Map = ushort.Parse(SpawnInfo[2]);
                    ushort XFrom = ushort.Parse(SpawnInfo[3]);
                    ushort YFrom = ushort.Parse(SpawnInfo[4]);
                    ushort XTo = ushort.Parse(SpawnInfo[5]);
                    ushort YTo = ushort.Parse(SpawnInfo[6]);
                    if (!Game.World.H_Mobs.Contains(Map))
                        Game.World.H_Mobs.Add(Map, new Hashtable());
                    Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Map];

                    DMap D = (DMap)DMaps.H_Maps[Map];

                    for (int i = 0; i < Count; i++)
                    {
                        Game.Mob _Mob = new Server.Game.Mob((Game.Mob)Mobs[MobID]);
                        _Mob.Loc = new Server.Game.Location();
                        _Mob.Loc.Map = Map;
                        _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                        _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));

                        while (D != null && D.GetCell(_Mob.Loc.X, _Mob.Loc.Y).NoAccess)
                        {
                            _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                            _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));
                        }
                        _Mob.StartLoc = _Mob.Loc;
                        _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                        while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                        MapMobs.Add(_Mob.EntityID, _Mob);
                        monst++;
                    }
                }
                Program.WriteMessage("Dragons Loaded " + monst.ToString());
            }
            Console.WriteLine("DragonsSpawns Done");
        }
        public static void LoadDengun(uint Dim, string dengun)
        {
            string[] FMobs = File.ReadAllLines(Program.ConquerPath + @"Denguns\" + dengun + ".txt");
            Hashtable Mobs = new Hashtable(FMobs.Length);
            for (int i = 0; i < FMobs.Length; i++)
            {
                if (!FMobs[i].StartsWith("#"))
                {
                    Game.Mob M = new Server.Game.Mob(FMobs[i]);
                    Mobs.Add(M.MobID, M);
                }
            }
            int monst = 0;

            string[] FSpawns = File.ReadAllLines(Program.ConquerPath + @"Denguns\" + dengun + "Spawns.txt");
            foreach (string Spawn in FSpawns)
            {
                if (!Spawn.StartsWith("#"))
                {
                    string[] SpawnInfo = Spawn.Split(' ');
                    int MobID = int.Parse(SpawnInfo[0]);
                    int Count = int.Parse(SpawnInfo[1]);
                    ushort Map = ushort.Parse(SpawnInfo[2]);
                    ushort XFrom = ushort.Parse(SpawnInfo[3]);
                    ushort YFrom = ushort.Parse(SpawnInfo[4]);
                    ushort XTo = ushort.Parse(SpawnInfo[5]);
                    ushort YTo = ushort.Parse(SpawnInfo[6]);
                    if (!Game.World.H_Mobs.Contains(Map))
                        Game.World.H_Mobs.Add(Map, new Hashtable());
                    Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Map];

                    DMap D = (DMap)DMaps.H_Maps[Map];

                    for (int i = 0; i < Count; i++)
                    {
                        Game.Mob _Mob = new Server.Game.Mob((Game.Mob)Mobs[MobID]);
                        _Mob.Loc = new Server.Game.Location();
                        _Mob.Loc.Map = Map;
                        _Mob.Loc.MapDimention = Dim;
                        _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                        _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));

                        while (D != null && D.GetCell(_Mob.Loc.X, _Mob.Loc.Y).NoAccess)
                        {
                            _Mob.Loc.X = (ushort)Program.Rnd.Next(Math.Min(XFrom, XTo), Math.Max(XFrom, XTo));
                            _Mob.Loc.Y = (ushort)Program.Rnd.Next(Math.Min(YFrom, YTo), Math.Max(YFrom, YTo));
                        }
                        _Mob.StartLoc = _Mob.Loc;
                        _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                        while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                        MapMobs.Add(_Mob.EntityID, _Mob);
                        monst++;
                    }
                }
                Program.WriteMessage("Dengun Loaded " + monst.ToString());
            }
        }

        public static void LoadCharacterStats()
        {
            IniFile F = new IniFile(Program.ConquerPath + @"Stats.txt");
            for (byte lvl = 1; lvl < 122; lvl++)
            {
                string job = "Archer[" + lvl + "]";
                string Data = F.ReadString("Stats", job);
                ArcherStats.Add(lvl, Data);
                job = "Ninja[" + lvl + "]";
                Data = F.ReadString("Stats", job);
                NinjaStats.Add(lvl, Data);
                job = "Warrior[" + lvl + "]";
                Data = F.ReadString("Stats", job);
                WarriorStats.Add(lvl, Data);
                job = "Trojan[" + lvl + "]";
                Data = F.ReadString("Stats", job);
                TrojanStats.Add(lvl, Data);
                job = "Taoist[" + lvl + "]";
                Data = F.ReadString("Stats", job);
                TaoistStats.Add(lvl, Data);
            }
            Console.WriteLine("Character Loaded");
        }

        public static void GetStats(Game.Character character)
        {
            string Job = "";
            switch (character.Job)
            {
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15: Job = "Trojan"; break;
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25: Job = "Warrior"; break;
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45: Job = "Archer"; break;
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55: Job = "Ninja"; break;
                default: Job = "Taoist"; break;
            }
            byte lvl = character.Level;
            if (lvl > 120)
                lvl = 120;

            string[] Data = null;
            if (Job == "Trojan")
                Data = TrojanStats[lvl].Split(',');
            else if (Job == "Warrior")
                Data = WarriorStats[lvl].Split(',');
            else if (Job == "Archer")
                Data = ArcherStats[lvl].Split(',');
            else if (Job == "Ninja")
                Data = NinjaStats[lvl].Split(',');
            else if (Job == "Taoist")
                Data = TaoistStats[lvl].Split(',');

            character.Str = Convert.ToUInt16(Data[0]);
            character.Vit = Convert.ToUInt16(Data[1]);
            character.Agi = Convert.ToUInt16(Data[2]);
            character.Spi = Convert.ToUInt16(Data[3]);
        }
        public static void GetInitialStats(byte inJob, ref ushort Str, ref ushort Agi, ref ushort Vit, ref ushort Spi)
        {
            string Job = "";
            switch (inJob)
            {
                case 10: Job = "Trojan"; break;
                case 20: Job = "Warrior"; break;
                case 40: Job = "Archer"; break;
                case 50: Job = "Ninja"; break;
                default: Job = "Taoist"; break;
            }
            byte lvl = 1;
            string[] Data = null;
            if (Job == "Trojan")
                Data = TrojanStats[lvl].Split(',');
            else if (Job == "Warrior")
                Data = WarriorStats[lvl].Split(',');
            else if (Job == "Archer")
                Data = ArcherStats[lvl].Split(',');
            else if (Job == "Ninja")
                Data = NinjaStats[lvl].Split(',');
            else if (Job == "Taoist")
                Data = TaoistStats[lvl].Split(',');

            Str = Convert.ToUInt16(Data[0]);
            Vit = Convert.ToUInt16(Data[1]);
            Agi = Convert.ToUInt16(Data[2]);
            Spi = Convert.ToUInt16(Data[3]);
        }

        public static AuthWorker.AuthInfo Authenticate(string User, string Password)
        {
            AuthWorker.AuthInfo Info = new AuthWorker.AuthInfo();
            Info.Account = User;
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);

                cmd.Select("Accounts").Where("AccountID", User);

                MySqlReader r = new MySqlReader(cmd);
                if (r.Read())
                {
                    string RealAccount = r.ReadString("AccountID");
                    if (User == RealAccount)
                    {

                        string RealPassword = r.ReadString("Password");

                        RealPassword = PassCrypto.EncryptPassword(RealPassword);

                        if (RealPassword == "" || RealPassword == Password)
                        {
                            if (RealPassword == "")
                            {

                                MySqlCommand cms = new MySqlCommand(MySqlCommandType.UPDATE);
                                cms.Update("accounts").Set("Password", Password).Where("AccountID", RealAccount).Execute();
                            }
                            Info.Status = r.ReadString("Status");


                            Info.Character = r.ReadString("Character");
                            if (Info.Character == "")
                                Info.LogonType = 2;
                            else
                                Info.LogonType = 1;
                            //Console.WriteLine();
                        }
                        else
                            Info.LogonType = 255;
                    }
                    else
                        Info.LogonType = 255;
                }
                else
                    Info.LogonType = 255;
            }
            catch { Info.LogonType = 255; }
            return Info;
        }
        public static string CreateCharacter(string Account, string Name, ushort Body, byte Job)
        {
            try
            {
                Console.WriteLine("!! " + Account + " " + Name + " " + Body + " " + Job);
                Game.Character C = new Server.Game.Character();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                    cmd.Select("Characters").Where("Name", Name);
                    MySqlReader r = new MySqlReader(cmd);
                    if (r.Read())
                    {
                        return "Name in use.";
                    }
                }
                catch { }
                int CPs = 1000;
                int Silvers = 1000000;
                ushort Str = 0, Agi = 0, Vit = 0, Spi = 0;
                GetInitialStats(Job, ref Str, ref Agi, ref Vit, ref Spi);
                ushort HP = (ushort)(Vit * 24 + Str * 3 + Agi * 3 + Spi * 3);
                byte Avatar = 0;
                if (Body == 1003 || Body == 1004 || Body == 2003 || Body == 2004)
                { Avatar = 1; }
                else if (Body == 1001 || Body == 1002 || Body == 2001 || Body == 2002)
                { Avatar = 201; }
                MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.INSERT);
                cmd2.Insert("characters").Insert("Account", Account).Insert("Name", Name).Insert("Avatar", Avatar).Insert("Body", Body).Insert("Hair", (410 + (Program.Rnd.Next(5) * 100))).Insert("Job", Job).Insert("Str", Str).Insert("Agi", Agi).Insert("Vit", Vit).Insert("Spi", Spi).Insert("CurHP", HP).Insert("CPs", CPs).Insert("Silvers", Silvers);
                cmd2.Execute();
                MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd3.Update("accounts").Set("Character", Name).Where("AccountID", Account).Execute();
                uint carid = 0;
                Game.Item Item = new Game.Item();
                MySqlCommand cmd4 = new MySqlCommand(MySqlCommandType.SELECT);
                cmd4.Select("characters").Where("Name", Name);
                MySqlReader rs = new MySqlReader(cmd4);
                if (rs.Read())
                {
                    carid = rs.ReadUInt32("EntityID");
                }
                Console.WriteLine("Creating Done");
            }
            catch (Exception exc) { Program.WriteMessage(exc); return "Error! Try again."; }

            return "ANSWER_OK";
        }
        public static void SaveCharacter(Game.Character C, string Acc)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                int DoubleExp = C.DoubleExpLeft;
                if (C.DoubleExp)
                    DoubleExp -= (int)(DateTime.Now - C.ExpPotionUsed).TotalSeconds;
                int gid = 0;
                if (C.MyGuild != null)
                    gid = C.MyGuild.GuildID;
                if (C.GettingLuckyTime)
                {
                    if (!C.Prayer)
                        C.LuckyTime += (uint)(DateTime.Now - C.PrayDT).TotalSeconds;
                    else
                        C.LuckyTime += (uint)(DateTime.Now - C.PrayDT).TotalSeconds * 3;
                    C.PrayDT = DateTime.Now;
                }

                cmd.Update("characters")
                    .Set("Level", C.Level)
                    .Set("OLDLevel", C.oldlevel)
                    .Set("Experience", C.Experience)
                    .Set("Silvers", C.Silvers)
                    .Set("CPs", C.CPs)
                    .Set("WHSilvers", C.WHSilvers)
                    .Set("Avatar", C.Avatar)
                    .Set("Body", C.Body)
                    .Set("Hair", C.Hair)
                    .Set("Map", C.Loc.Map)
                    .Set("X", C.Loc.X)
                    .Set("Y", C.Loc.Y)
                    .Set("PreviousMap", C.Loc.PreviousMap)
                    .Set("Job", C.Job)
                    .Set("StatPoints", C.StatusPoints)
                    .Set("Vit", C.Vit)
                    .Set("Str", C.Str)
                    .Set("Spi", C.Spi)
                    .Set("Agi", C.Agi)
                    .Set("CurHP", C.CurHP)
                    .Set("CurMP", C.CurMP)
                    .Set("PKPoints", C.PKPoints)

                    .Set("NobilityDonation", C.Nobility.Donation)
                    .Set("GuildID", gid)
                    .Set("GuildDonation", C.GuildDonation)
                    .Set("GuildRank", (byte)C.GuildRank)
                    .Set("Warehouses", C.Warehouses.WriteThis())
                    .Set("WarehousesPass", C.WHPassword)
                    .Set("Equips", C.Equips.WriteThis())

                    .Set("Spouse", C.Spouse)
                    .Set("VipLevel", C.VipLevel)
                    .Set("DoubleExp", C.DoubleExp.ToString())
                    .Set("DoubleExpLeft", DoubleExp)
                    .Set("LuckyTime", C.LuckyTime)
                    .Set("ExpBallUsedToday", C.ExpBallsUsedToday)
                    .Set("DBUsedToday", C.DbUsedToday)
                    .Set("LotteryUsedToday", C.LotteryUsed)
                    .Set("BlessingStarted", C.BlessingStarted.Ticks)
                    .Set("BlessingLasts", C.BlessingLasts)
                    .Set("ElightenAdd", C.ElightenAdd)
                    .Set("ElighemPoints", C.ElighemPoints)
                    .Set("EnhligtehnRequest", C.EnhligtehnRequest)
                    .Set("Merchant", (byte)C.Merchant)
                    .Set("WarFlames", C.flames)
                    .Set("DisCityKO", C.DisKO)
                    .Set("House", C.House)
                    .Set("VP", C.VP)
                    .Set("AvaLasts", C.avaLasts)
                    .Set("AvatarLevel", C.AvatarLevel)
                    .Set("AvatarAccount", C.robotaccount + "~" + C.robotPassword)

                    .Where("EntityID", C.EntityID).Execute();
                SaveSkills(C);
                SaveProfs(C);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        public static Game.Character LoadCharacter(string Name, ref string Account, bool robot)
        {
            foreach (DictionaryEntry DE in World.H_Chars)
            {
                Character Chaar = (Character)DE.Value;
                if (Chaar.Name == Name)
                {
                    if (Chaar != null)
                    {
                        Chaar.MyClient.Disconnect();
                        Console.WriteLine("was already opend Disconnect");
                    }
                }
            }
            Game.Character C = new Server.Game.Character();
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                cmd.Select("Characters").Where("Name", Name);
                MySqlReader r = new MySqlReader(cmd);
                if (r.Read())
                {
                    C.Name = Name;
                    if (robot)
                        C.isAvatar = robot;
                    Account = r.ReadString("Account");
                    C.EntityID = r.ReadUInt32("EntityID");
                    C.Level = r.ReadByte("Level");
                    C.oldlevel = r.ReadByte("OLDLevel");
                    C.Experience = r.ReadUInt64("Experience");
                    C.Silvers = r.ReadUInt32("Silvers");
                    C.CPs = r.ReadUInt32("CPs");
                    C.WHSilvers = r.ReadUInt32("WHSilvers");
                    C.Avatar = r.ReadUInt16("Avatar");
                    C.Body = r.ReadUInt16("Body");
                    C.Hair = r.ReadUInt16("Hair");
                    C.Loc = new Server.Game.Location();
                    C.Loc.Map = r.ReadUInt16("Map");
                    C.Loc.X = r.ReadUInt16("X");
                    C.Loc.Y = r.ReadUInt16("Y");
                    C.Loc.PreviousMap = r.ReadUInt16("PreviousMap");
                    C.Loc.MapDimention = 0;
                    C.Job = r.ReadByte("Job");
                    C.PreviousJob1 = r.ReadByte("PreviousJob");
                    C.PreviousJob2 = r.ReadByte("PreviousJob2");
                    C.Reborns = r.ReadByte("Reborns");
                    C.StatusPoints = r.ReadUInt16("StatPoints");
                    C.Str = r.ReadUInt16("Str");
                    C.Agi = r.ReadUInt16("Agi");
                    C.Vit = r.ReadUInt16("Vit");
                    C.Spi = r.ReadUInt16("Spi");
                    C.TopTrojan = r.ReadUInt16("TopTrojan");
                    C.TopWarrior = r.ReadUInt16("TopWarrior");
                    C.TopArcher = r.ReadUInt16("TopArcher");
                    C.TopWaterTaoist = r.ReadUInt16("TopWaterTaoist");
                    C.TopFireTaoist = r.ReadUInt16("TopFireTaoist");
                    C.TopNinja = r.ReadUInt16("TopNinja");
                    C.TopMonthly = r.ReadUInt16("MonthlyPKChampion");
                    C.TopGuildLeader = r.ReadUInt16("TopGuildLeader");
                    C.TopDeputyLeader = r.ReadUInt16("TopDeputyLeader");
                    C.UniversityPoints = r.ReadUInt32("UniversityPoints");
                    C.CurHP = r.ReadUInt16("CurHP");
                    C.CurMP = r.ReadUInt16("CurMP");
                    C.PKPoints = r.ReadUInt16("PKPoints");

                    #region Nobility
                    C.Nobility.Donation = r.ReadUInt64("NobilityDonation");
                    C.Nobility.ListPlace = -1;
                    if (C.Nobility.Donation >= 3000000)
                    {
                        C.Nobility.ListPlace = 50;
                        for (int i = 49; i >= 0; i--)
                        {
                            if (C.Nobility.Donation >= Game.World.EmpireBoard[i].Donation)
                                C.Nobility.ListPlace--;
                        }
                        if (C.Nobility.ListPlace < 50)
                        {
                            if (C.Nobility.Donation >= 30000000 && C.Nobility.Donation <= 100000000)
                                C.Nobility.Rank = Game.Ranks.Knight;
                            else if (C.Nobility.Donation >= 100000000 && C.Nobility.Donation <= 200000000)
                                C.Nobility.Rank = Game.Ranks.Baron;
                            else if (C.Nobility.Donation >= 200000000 && C.Nobility.Donation <= 300000000)
                                C.Nobility.Rank = Game.Ranks.Earl;
                            else if (C.Nobility.ListPlace >= 15 && C.Nobility.ListPlace <= 50)
                                C.Nobility.Rank = Game.Ranks.Duke;
                            else if (C.Nobility.ListPlace >= 3 && C.Nobility.ListPlace <= 15)
                                C.Nobility.Rank = Game.Ranks.Prince;
                            else if (C.Nobility.ListPlace <= 3)
                                C.Nobility.Rank = Game.Ranks.King;
                        }
                    }
                    Game.EmpireInfo G = new EmpireInfo();
                    G.Donation = C.Donation;
                    Database.SaveEmpire();
                    #endregion

                    #region Warehouses
                    C.WHPassword = r.ReadString("WarehousesPass");
                    string Warehouses = r.ReadString("Warehouses").ToString();
                    if (Warehouses == null)
                        Warehouses = "";
                    C.Warehouses = new Server.Game.Banks();
                    C.Warehouses.ReadThis(Warehouses);
                    #endregion

                    #region Guild
                    ushort GID = r.ReadUInt16("GuildID");
                    if (Features.Guilds.AllTheGuilds.ContainsKey(GID))
                    {
                        C.MyGuild = (Features.Guild)Features.Guilds.AllTheGuilds[GID];

                        uint Don = r.ReadUInt32("GuildDonation"); ;
                        byte GR = r.ReadByte("GuildRank");
                        try
                        {
                            if (((Hashtable)C.MyGuild.Members[GR]).Contains(C.EntityID))
                            {
                                C.GuildDonation = Don;
                                C.GuildRank = (Features.GuildRank)GR;

                                C.MembInfo = (Features.MemberInfo)((Hashtable)C.MyGuild.Members[GR])[C.EntityID];
                                C.MembInfo.Level = C.Level;
                                C.GuildDonation = C.MembInfo.Donation;
                                C.GuildRank = C.MembInfo.Rank;
                            }
                            else
                                C.MyGuild = null;
                        }
                        catch { C.MyGuild = null; }
                    }
                    #endregion

                    #region Equips
                    C.Equips = new Server.Game.Equipment();
                    string Equipment = r.ReadString("Equips").ToString();
                    if (Equipment == null)
                        Equipment = "";
                    C.Equips.ReadThis(Equipment);
                    #endregion

                    #region Avatar
                    C.AvatarLevel = r.ReadByte("AvatarLevel");
                    string avaacc = r.ReadString("AvatarAccount").ToString();
                    if (avaacc == null)
                        avaacc = "";
                    string[] ava = avaacc.Split('~');
                    try
                    {
                        C.robotaccount = ava[0];
                        C.robotPassword = ava[1];
                    }
                    catch { }
                    #endregion

                    C.Spouse = r.ReadString("Spouse").ToString();
                    C.VipLevel = r.ReadByte("VipLevel");
                    C.DoubleExp = r.ReadBoolean("DoubleExp");
                    C.DoubleExpLeft = r.ReadInt32("DoubleExpLeft");
                    C.LuckyTime = r.ReadUInt32("LuckyTime");
                    C.ExpBallsUsedToday = r.ReadByte("ExpBallUsedToday");
                    C.DbUsedToday = r.ReadByte("DBUsedToday");
                    C.LotteryUsed = r.ReadByte("LotteryUsedToday");
                    C.BlessingStarted = DateTime.FromBinary(r.ReadInt64("BlessingStarted"));
                    C.BlessingLasts = r.ReadInt32("BlessingLasts");
                    C.ElightenAdd = r.ReadByte("ElightenAdd");
                    C.ElighemPoints = r.ReadUInt64("ElighemPoints");
                    C.EnhligtehnRequest = r.ReadUInt16("EnhligtehnRequest");
                    C.Merchant = (Server.Game.MerchantTypes)r.ReadByte("Merchant");
                    C.flames = r.ReadUInt16("WarFlames");
                    C.DisKO = r.ReadUInt16("DisCityKO");
                    C.House = r.ReadUInt16("House");
                    C.VP = r.ReadUInt64("VP");
                    C.avaLasts = r.ReadInt32("AvaLasts");
                    C.Loaded = true;
                    Database.LoadCharacterProfs(C);
                    Database.LoadCharacterSkills(C);
                }
                else
                {
                    Console.WriteLine("Character Not Found.");
                }
            }
            catch (Exception exc) { Program.WriteMessage(exc); }
            return C;
        }
        public static Game.Avatar LoadAvatar(string Name, ref string Account, bool robot)
        {
            foreach (DictionaryEntry DE in World.H_Chars)
            {
                Character Chaar = (Character)DE.Value;
                if (Chaar.Name == Name)
                {
                    if (Chaar != null)
                    {
                        Chaar.MyClient.Disconnect();
                        Console.WriteLine("was already opend Disconnect");
                    }
                }
            }
            Game.Avatar C = new Server.Game.Avatar();
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                cmd.Select("Characters").Where("Name", Name);
                MySqlReader r = new MySqlReader(cmd);
                if (r.Read())
                {
                    C.Name = Name;
                    if (robot)
                        C.isAvatar = robot;
                    Account = r.ReadString("Account");
                    C.EntityID = r.ReadUInt32("EntityID");
                    C.Level = r.ReadByte("Level");
                    C.oldlevel = r.ReadByte("OLDLevel");
                    C.Experience = r.ReadUInt64("Experience");
                    C.Silvers = r.ReadUInt32("Silvers");
                    C.CPs = r.ReadUInt32("CPs");
                    C.WHSilvers = r.ReadUInt32("WHSilvers");
                    C.Avatar = r.ReadUInt16("Avatar");
                    C.Body = r.ReadUInt16("Body");
                    C.Hair = r.ReadUInt16("Hair");
                    C.Loc = new Server.Game.Location();
                    C.Loc.Map = r.ReadUInt16("Map");
                    C.Loc.X = r.ReadUInt16("X");
                    C.Loc.Y = r.ReadUInt16("Y");
                    C.Loc.PreviousMap = r.ReadUInt16("PreviousMap");
                    C.Loc.MapDimention = 0;
                    C.Job = r.ReadByte("Job");
                    C.PreviousJob1 = r.ReadByte("PreviousJob");
                    C.PreviousJob2 = r.ReadByte("PreviousJob2");
                    C.Reborns = r.ReadByte("Reborns");
                    C.StatusPoints = r.ReadUInt16("StatPoints");
                    C.Str = r.ReadUInt16("Str");
                    C.Agi = r.ReadUInt16("Agi");
                    C.Vit = r.ReadUInt16("Vit");
                    C.Spi = r.ReadUInt16("Spi");
                    C.TopTrojan = r.ReadUInt16("TopTrojan");
                    C.TopWarrior = r.ReadUInt16("TopWarrior");
                    C.TopArcher = r.ReadUInt16("TopArcher");
                    C.TopWaterTaoist = r.ReadUInt16("TopWaterTaoist");
                    C.TopFireTaoist = r.ReadUInt16("TopFireTaoist");
                    C.TopNinja = r.ReadUInt16("TopNinja");
                    C.TopMonthly = r.ReadUInt16("MonthlyPKChampion");
                    C.TopGuildLeader = r.ReadUInt16("TopGuildLeader");
                    C.TopDeputyLeader = r.ReadUInt16("TopDeputyLeader");
                    C.UniversityPoints = r.ReadUInt32("UniversityPoints");
                    C.CurHP = r.ReadUInt16("CurHP");
                    C.CurMP = r.ReadUInt16("CurMP");
                    C.PKPoints = r.ReadUInt16("PKPoints");

                    #region Nobility
                    C.Nobility.Donation = r.ReadUInt64("NobilityDonation");
                    C.Nobility.ListPlace = -1;
                    if (C.Nobility.Donation >= 3000000)
                    {
                        C.Nobility.ListPlace = 50;
                        for (int i = 49; i >= 0; i--)
                        {
                            if (C.Nobility.Donation >= Game.World.EmpireBoard[i].Donation)
                                C.Nobility.ListPlace--;
                        }
                        if (C.Nobility.ListPlace < 50)
                        {
                            if (C.Nobility.Donation >= 30000000 && C.Nobility.Donation <= 100000000)
                                C.Nobility.Rank = Game.Ranks.Knight;
                            else if (C.Nobility.Donation >= 100000000 && C.Nobility.Donation <= 200000000)
                                C.Nobility.Rank = Game.Ranks.Baron;
                            else if (C.Nobility.Donation >= 200000000 && C.Nobility.Donation <= 300000000)
                                C.Nobility.Rank = Game.Ranks.Earl;
                            else if (C.Nobility.ListPlace >= 15 && C.Nobility.ListPlace <= 50)
                                C.Nobility.Rank = Game.Ranks.Duke;
                            else if (C.Nobility.ListPlace >= 3 && C.Nobility.ListPlace <= 15)
                                C.Nobility.Rank = Game.Ranks.Prince;
                            else if (C.Nobility.ListPlace <= 3)
                                C.Nobility.Rank = Game.Ranks.King;
                        }
                    }
                    Game.EmpireInfo G = new EmpireInfo();
                    G.Donation = C.Donation;
                    Database.SaveEmpire();
                    #endregion

                    #region Warehouses
                    C.WHPassword = r.ReadString("WarehousesPass");
                    string Warehouses = r.ReadString("Warehouses").ToString();
                    if (Warehouses == null)
                        Warehouses = "";
                    C.Warehouses = new Server.Game.Banks();
                    C.Warehouses.ReadThis(Warehouses);
                    #endregion

                    #region Guild
                    ushort GID = r.ReadUInt16("GuildID");
                    if (Features.Guilds.AllTheGuilds.ContainsKey(GID))
                    {
                        C.MyGuild = (Features.Guild)Features.Guilds.AllTheGuilds[GID];

                        uint Don = r.ReadUInt32("GuildDonation"); ;
                        byte GR = r.ReadByte("GuildRank");
                        try
                        {
                            if (((Hashtable)C.MyGuild.Members[GR]).Contains(C.EntityID))
                            {
                                C.GuildDonation = Don;
                                C.GuildRank = (Features.GuildRank)GR;

                                C.MembInfo = (Features.MemberInfo)((Hashtable)C.MyGuild.Members[GR])[C.EntityID];
                                C.MembInfo.Level = C.Level;
                                C.GuildDonation = C.MembInfo.Donation;
                                C.GuildRank = C.MembInfo.Rank;
                            }
                            else
                                C.MyGuild = null;
                        }
                        catch { C.MyGuild = null; }
                    }
                    #endregion

                    #region Equips
                    C.Equips = new Server.Game.Equipment();
                    string Equipment = r.ReadString("Equips").ToString();
                    if (Equipment == null)
                        Equipment = "";
                    C.Equips.ReadThis(Equipment);
                    #endregion

                    #region Avatar
                    C.AvatarLevel = r.ReadByte("AvatarLevel");
                    string avaacc = r.ReadString("AvatarAccount").ToString();
                    if (avaacc == null)
                        avaacc = "";
                    string[] ava = avaacc.Split('~');
                    try
                    {
                        C.robotaccount = ava[0];
                        C.robotPassword = ava[1];
                    }
                    catch { }
                    #endregion

                    C.Spouse = r.ReadString("Spouse").ToString();
                    C.VipLevel = r.ReadByte("VipLevel");
                    C.DoubleExp = r.ReadBoolean("DoubleExp");
                    C.DoubleExpLeft = r.ReadInt32("DoubleExpLeft");
                    C.LuckyTime = r.ReadUInt32("LuckyTime");
                    C.ExpBallsUsedToday = r.ReadByte("ExpBallUsedToday");
                    C.DbUsedToday = r.ReadByte("DBUsedToday");
                    C.LotteryUsed = r.ReadByte("LotteryUsedToday");
                    C.BlessingStarted = DateTime.FromBinary(r.ReadInt64("BlessingStarted"));
                    C.BlessingLasts = r.ReadInt32("BlessingLasts");
                    C.ElightenAdd = r.ReadByte("ElightenAdd");
                    C.ElighemPoints = r.ReadUInt64("ElighemPoints");
                    C.EnhligtehnRequest = r.ReadUInt16("EnhligtehnRequest");
                    C.Merchant = (Server.Game.MerchantTypes)r.ReadByte("Merchant");
                    C.flames = r.ReadUInt16("WarFlames");
                    C.DisKO = r.ReadUInt16("DisCityKO");
                    C.House = r.ReadUInt16("House");
                    C.VP = r.ReadUInt64("VP");
                    C.avaLasts = r.ReadInt32("AvaLasts");
                    C.Loaded = true;
                    Database.LoadCharacterProfs(C);
                    Database.LoadCharacterSkills(C);
                }
                else
                {
                    Console.WriteLine("Character Not Found.");
                }
            }
            catch (Exception exc) { Program.WriteMessage(exc); }
            return C;
        }
        public static void SaveAva(Game.Character C)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                int DoubleExp = C.DoubleExpLeft;
                if (C.DoubleExp)
                    DoubleExp -= (int)(DateTime.Now - C.ExpPotionUsed).TotalSeconds;
                int gid = 0;
                if (C.MyGuild != null)
                    gid = C.MyGuild.GuildID;
                if (C.GettingLuckyTime)
                {
                    if (!C.Prayer)
                        C.LuckyTime += (uint)(DateTime.Now - C.PrayDT).TotalSeconds;
                    else
                        C.LuckyTime += (uint)(DateTime.Now - C.PrayDT).TotalSeconds * 3;
                    C.PrayDT = DateTime.Now;
                }
                cmd.Update("characters")
                    .Set("Level", C.Level)
                    .Set("OLDLevel", C.oldlevel)
                    .Set("Experience", C.Experience)

                    .Set("CurHP", C.CurHP)
                    .Set("CurMP", C.CurMP)
                    .Set("PKPoints", C.PKPoints)

                    .Set("DoubleExp", C.DoubleExp.ToString())
                    .Set("DoubleExpLeft", DoubleExp)
                    .Set("LuckyTime", C.LuckyTime)

                    .Set("BlessingStarted", C.BlessingStarted.Ticks)
                    .Set("BlessingLasts", C.BlessingLasts)
                    .Set("ElightenAdd", C.ElightenAdd)
                    .Set("ElighemPoints", C.ElighemPoints)
                    .Set("EnhligtehnRequest", C.EnhligtehnRequest)

                    .Set("DisCityKO", C.DisKO)

                    .Where("EntityID", C.EntityID).Execute();
                SaveProfs(C);
                SaveSkills(C);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }

        #region Items
        public static bool LoadCharacterItems(Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("items").Where("CharID", C.EntityID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                {
                    Game.Item Item = new Game.Item();
                    Item.Bless = r.ReadByte("Minus");
                    Item.CurDur = r.ReadUInt16("Dura");
                    Item.ID = r.ReadUInt32("ItemID");
                    Item.MaxDur = r.ReadUInt16("MaxDura");
                    Item.Plus = r.ReadByte("Plus");
                    Item.Position = r.ReadByte("Position");
                    Item.Soc1 = (Item.Gem)r.ReadByte("Soc1");
                    Item.Soc2 = (Item.Gem)r.ReadByte("Soc2");
                    Item.UID = r.ReadUInt32("ItemUID");
                    Item.Enchant = r.ReadByte("Enchant");
                    Item.Color = (Item.ArmorColor)r.ReadByte("Color");
                    Item.Suspicious = r.ReadInt16("Suspicious");
                    Item.FreeItem = r.ReadBoolean("Free");
                    Item.Locked = r.ReadByte("Locked");
                    Item.Progress = r.ReadUInt16("Progress");
                    Item.LockedDays = r.ReadUInt32("LockedDay");
                    Item.TalismanProgress = r.ReadUInt32("SocketProgress");
                    //C.Equips.Armor = Item;
                    if (Item.ID == 300000)
                    {
                        Item.RBG[0] = r.ReadByte("X");
                        Item.RBG[1] = r.ReadByte("Y");
                        Item.RBG[2] = r.ReadByte("Z");
                        Item.RBG[3] = r.ReadByte("floor");
                        Item.TalismanProgress = BitConverter.ToUInt32(Item.RBG, 0);
                    }
                    Item.Effect = (Item.RebornEffect)r.ReadByte("Effect");

                    C.Inventory.Add(Item.UID, Item);
                    C.MyClient.SendPacket(Packets.AddItem(Item, 0));
                    if (Item.Locked == 2)
                    {

                        int myDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                        C.MyClient.SendPacket(Packets.ItemLock(Item.UID, 1, 3, (uint)Item.LockedDays));
                        if (myDate >= (int)Item.LockedDays)
                        {
                            Item.LockedDays = 0;
                            Item.Locked = 0;
                            Database.UpdateItem(Item);

                            C.MyClient.LocalMessage(2000, System.Drawing.Color.Red, "Congratulations! successful Unlocked " + Item.ItemDBInfo.Name + "");
                        }
                    }

                }
            }
            return true;
        }
        public static void UpdateItem(Game.Item Item)
        {
            try
            {
                MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd2.Update("items")
                    .Set("Position", Item.Position)
                    .Set("ItemID", Item.ID)
                    .Set("Plus", Item.Plus)
                    .Set("Progress", Item.Progress)
                    .Set("Minus", Item.Bless)
                    .Set("Enchant", Item.Enchant)
                    .Set("Soc1", (byte)Item.Soc1)
                    .Set("Soc2", (byte)Item.Soc2)
                    .Set("Dura", Item.CurDur)
                    .Set("MaxDura", Item.MaxDur)
                    .Set("Color", (byte)Item.Color)
                    .Set("Locked", Item.Locked)
                    .Set("Free", Item.FreeItem)
                    .Set("SocketProgress", Item.TalismanProgress)
                    .Set("Suspicious", Item.Suspicious)
                    .Set("LockedDay", Item.LockedDays)
                    .Set("X", Item.RBG[0])
                    .Set("Y", Item.RBG[1])
                    .Set("Z", Item.RBG[2])
                    .Set("floor", Item.RBG[3])
                    .Set("Effect", (byte)Item.Effect)
                    .Where("ItemUID", Item.UID).Execute();//.And("CharID", C.EntityID)
            }
            catch (Exception Exe) { Program.WriteMessage(Exe); }
        }
        public static void TradeItem(Game.Item Item, uint ToCharID)
        {
            try
            {
                MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd2.Update("items").Set("Position", Item.Position).Set("CharID", ToCharID).Set("ItemID", Item.ID).Set("Plus", Item.Plus).Set("Progress", Item.Progress).Set("Minus", Item.Bless).Set("Enchant", Item.Enchant).Set("Soc1", (byte)Item.Soc1).Set("Soc2", (byte)Item.Soc2).Set("Dura", Item.CurDur).Set("MaxDura", Item.MaxDur).Set("Color", (byte)Item.Color).Set("Locked", Item.Locked).Set("Free", Item.FreeItem).Set("SocketProgress", Item.TalismanProgress).Set("Suspicious", Item.Suspicious).Set("LockedDay", Item.LockedDays)
                    .Set("X", Item.RBG[0])
                    .Set("Y", Item.RBG[1]).Set("Z", Item.RBG[2]).Set("floor", Item.RBG[3]).Set("Effect", (byte)Item.Effect)
                    .Where("ItemUID", Item.UID).Execute();//.And("CharID", C.EntityID)
            }
            catch (Exception Exe) { Program.WriteMessage(Exe); }
        }
        public static void DeleteItem(uint UID, uint CharID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("items", "ItemUID", UID).And("CharID", CharID).Execute();
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }


        public static bool NewItem(Game.Item Item, Game.Character GC)
        {
            foreach (Hashtable H in World.H_Items.Values)
            {
                foreach (DroppedItem I in H.Values)
                    if (I.UID == Item.UID)
                    {
                        // Program.WriteMessage("Looping DroppedItem efect " + Item.UID);
                        return false;

                    }
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("items").Insert("ItemUID", Item.UID).Insert("Position", Item.Position).Insert("CharID", GC.EntityID).Insert("ItemID", Item.ID).Insert("Plus", Item.Plus).Insert("Minus", Item.Bless).Insert("Enchant", Item.Enchant).Insert("Soc1", (byte)Item.Soc1).Insert("Soc2", (byte)Item.Soc2).Insert("Dura", Item.CurDur).Insert("MaxDura", Item.MaxDur).Insert("LockedDay", Item.LockedDays).Insert("Color", (byte)Item.Color).Insert("Suspicious", Item.Suspicious).Insert("Free", Item.FreeItem).Insert("Locked", Item.Locked).Insert("Progress", Item.Progress).Insert("SocketProgress", Item.TalismanProgress).Insert("X", Item.RBG[0])
                    .Insert("Y", Item.RBG[1]).Insert("Z", Item.RBG[2]).Insert("floor", Item.RBG[3]).Insert("Effect", (byte)Item.Effect);
                cmd.Execute();
            }
            catch (Exception e)
            {
                { Program.WriteMessage(e); }
                Program.WriteMessage("Looping DB efect " + Item.UID);
                return false;

            }
            return true;
        }
        public static bool ReputItemInHand(Game.Item Item, Game.Character GC)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("items").Insert("ItemUID", Item.UID).Insert("Position", Item.Position).Insert("CharID", GC.EntityID).Insert("ItemID", Item.ID).Insert("Plus", Item.Plus).Insert("Minus", Item.Bless).Insert("Enchant", Item.Enchant).Insert("Soc1", (byte)Item.Soc1).Insert("Soc2", (byte)Item.Soc2).Insert("Dura", Item.CurDur).Insert("MaxDura", Item.MaxDur).Insert("LockedDay", Item.LockedDays).Insert("Color", (byte)Item.Color).Insert("Suspicious", Item.Suspicious).Insert("Free", Item.FreeItem).Insert("Locked", Item.Locked).Insert("Progress", Item.Progress).Insert("SocketProgress", Item.TalismanProgress).Insert("X", Item.RBG[0])
                    .Insert("Y", Item.RBG[1]).Insert("Z", Item.RBG[2]).Insert("floor", Item.RBG[3]).Insert("Effect", (byte)Item.Effect);
                cmd.Execute();
            }
            catch
            {
                try
                {
                    MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd2.Update("items").Set("CharID", GC.EntityID).Set("Position", Item.Position).Set("ItemID", Item.ID).Set("Plus", Item.Plus).Set("Progress", Item.Progress).Set("Minus", Item.Bless).Set("Enchant", Item.Enchant).Set("Soc1", (byte)Item.Soc1).Set("Soc2", (byte)Item.Soc2).Set("Dura", Item.CurDur).Set("MaxDura", Item.MaxDur).Set("Color", (byte)Item.Color).Set("Locked", Item.Locked).Set("Free", Item.FreeItem).Set("SocketProgress", Item.TalismanProgress).Set("Suspicious", Item.Suspicious).Set("LockedDay", Item.LockedDays)
                        .Set("X", Item.RBG[0])
                        .Set("Y", Item.RBG[1]).Set("Z", Item.RBG[2]).Set("floor", Item.RBG[3]).Set("Effect", (byte)Item.Effect)
                        .Where("ItemUID", Item.UID).And("CharID", 0).Execute();
                }
                catch
                {
                    Random Rnd = new Random();
                    Item.UID = (uint)Rnd.Next(1, 999999999);
                    bool created = NewItem(Item, GC);
                    while (!created)
                    {
                        Item.UID = (uint)Rnd.Next(1, 999999999);
                        created = NewItem(Item, GC);
                    }
                }

            }
            return true;
        }
        public static void KeepItemAwyFromHand(uint UID, Game.Character C)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("items").Set("CharID", 0).Where("ItemUID", UID).Execute();//"ItemUID").Set(
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }
        public static void Fixinventory(uint UID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("items", "CharID", UID).Execute();
                MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.DELETE);
                cmd2.Delete("items", "ItemUID", UID).Execute();
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }
        #endregion
        #region Partners
        public static void LoadCharacterPartners(Character C)
        {
            //   C.Partners = new Hashtable();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("TradePartner").Where("CharID", C.EntityID);
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                Game.TradePartner TP = new Game.TradePartner();
                TP.UID = d.ReadUInt32("PartenerID");
                TP.Name = d.ReadString("PartenerName");
                long date = d.ReadInt64("TimeStart");
                C.TimePartner = date;
                TP.ProbationStartedOn = DateTime.FromBinary(date);
                if (!C.Partners.ContainsKey(TP.UID))
                    C.Partners.Add(TP.UID, TP);
            }

        }
        public static void SavePartner(uint uid, string name, long timestart, Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("TradePartner").Insert("CharID", C.EntityID).Insert("PartenerName", name).Insert("PartenerID", uid).Insert("TimeStart", timestart).Execute();

        }
        public static void DeletePartner(uint UID, uint IDYou)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("TradePartner", "CharID", IDYou).And("PartenerID", UID).Execute();
        }
        #endregion
        #region Skills
        public static void LoadCharacterSkills(Character C)
        {
            C.Skills = new Hashtable();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("skills").Where("EntityID", C.EntityID).And("Type", "spell");
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                Game.Skill SC = new Game.Skill();
                SC.ID = d.ReadUInt16("ID");
                SC.Lvl = d.ReadByte("Level");
                SC.Exp = d.ReadUInt32("Experience");
                if (!C.Skills.ContainsKey(SC.ID))
                {
                    C.Skills.Add(SC.ID, SC);
                }
            }
        }
        public static void AddSkills(Character Gc)
        {
            foreach (Game.Skill S in Gc.Skills.Values)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                    cmd.Select("skills").Where("ID", S.ID).And("EntityID", Gc.EntityID);
                    MySqlReader r = new MySqlReader(cmd);
                    if (r.Read())
                    {
                        if (r.ReadUInt16("ID") == S.ID && r.ReadUInt32("EntityID") == Gc.EntityID)
                            continue;
                    }
                }
                catch { }
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("skills").Insert("Level", S.Lvl).Insert("Experience", S.Exp).Insert("EntityID", Gc.EntityID)
                        .Insert("Type", "spell").Insert("ID", S.ID).Execute();
                }
            }
        }
        public static void SaveSkills(Character C)
        {

            try
            {
                foreach (Game.Skill p in C.Skills.Values)
                    if (p != null)
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                            cmd.Update("skills").Set("Level", p.Lvl).Set("Experience", p.Exp).Where("EntityID", C.EntityID).And("ID", p.ID).Execute();
                        }
                        catch { continue; }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }
        public static void DelSkills(Skill p, Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("skills", "EntityID", C.EntityID).And("ID", p.ID).Execute();
        }
        #endregion
        #region Profs
        public static void LoadCharacterProfs(Character C)
        {
            C.Profs = new Hashtable();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("profs").Where("EntityID", C.EntityID).And("Type", "prof");
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                Game.Prof SC = new Game.Prof();
                SC.ID = d.ReadUInt16("ID");
                SC.Lvl = d.ReadByte("Level");
                SC.Exp = d.ReadUInt32("Experience");
                if (!C.Profs.Contains(SC.ID))
                    C.Profs.Add(SC.ID, SC);
            }
        }
        public static void AddProfs(Character Gc)
        {
            foreach (Game.Prof p in Gc.Profs.Values)
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                    cmd.Select("profs").Where("ID", p.ID).And("EntityID", Gc.EntityID);
                    MySqlReader r = new MySqlReader(cmd);
                    if (r.Read())
                    {
                        if (r.ReadUInt16("ID") == p.ID && r.ReadUInt32("EntityID") == Gc.EntityID)
                            continue;
                    }
                }
                catch { }
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("profs").Insert("Level", p.Lvl).Insert("Experience", p.Exp).Insert("EntityID", (long)Gc.EntityID)
                        .Insert("Type", "prof").Insert("ID", p.ID).Execute();
                }
            }
        }
        public static void SaveProfs(Character C)
        {
            try
            {
                foreach (Game.Prof p in C.Profs.Values)

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                        cmd.Update("profs").Set("Level", p.Lvl).Set("Experience", p.Exp).Where("EntityID", C.EntityID).And("ID", p.ID).Execute();
                    }
                    catch { continue; }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }
        public static void DelProfs(Prof p, Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("profs", "EntityID", C.EntityID).And("ID", p.ID).Execute();
        }
        #endregion
        #region Reset Reporning Skills/Profs
        public static void deleteprofreborn(Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("profs", "EntityID", C.EntityID).Execute();

        }
        public static void deletepsekillreborn(Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("skills", "EntityID", C.EntityID).Execute();
        }
        #endregion

        #region Guild
        public static void CreateGuild(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("guildenemies").Insert("GuildID", C.MyGuild.GuildID).Execute();
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.INSERT);
            cmd2.Insert("guildallies").Insert("GuildID", C.MyGuild.GuildID).Execute();
        }
        public static void DeleteGuild(Game.Character C)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd.Delete("guildenemies", "GuildID", C.MyGuild.GuildID).Execute();
            MySqlCommand Cmd2 = new MySqlCommand(MySqlCommandType.DELETE);
            Cmd2.Delete("guildallies", "GuildID", C.MyGuild.GuildID).Execute();
        }

        public static void LoadGuildAllis(Features.Guild G)
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guildallies").Where("GuildID", G.GuildID);
            MySqlReader d = new MySqlReader(cmd);
            if (d.Read())
            {
                if (d.ReadUInt16("AlliesId1") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("AlliesId1") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Allies.ContainsKey(TGuild.GuildID))
                                G.Allies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("AlliesId2") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("AlliesId2") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Allies.ContainsKey(TGuild.GuildID))
                                G.Allies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("AlliesId3") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("AlliesId3") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Allies.ContainsKey(TGuild.GuildID))
                                G.Allies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("AlliesId4") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("AlliesId4") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Allies.ContainsKey(TGuild.GuildID))
                                G.Allies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("AlliesId5") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("AlliesId5") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Allies.ContainsKey(TGuild.GuildID))
                                G.Allies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
            }
        }
        public static void LoadGuildEnemies(Features.Guild G)
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guildenemies").Where("GuildID", G.GuildID);
            MySqlReader d = new MySqlReader(cmd);
            if (d.Read())
            {
                if (d.ReadUInt16("EnemiesId1") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("EnemiesId1") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Enemies.ContainsKey(TGuild.GuildID))
                                G.Enemies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("EnemiesId2") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("EnemiesId2") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Enemies.ContainsKey(TGuild.GuildID))
                                G.Enemies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("EnemiesId3") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("EnemiesId3") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Enemies.ContainsKey(TGuild.GuildID))
                                G.Enemies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("EnemiesId4") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("EnemiesId4") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Enemies.ContainsKey(TGuild.GuildID))
                                G.Enemies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
                if (d.ReadUInt16("EnemiesId5") != 0)
                {
                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                    {

                        Features.Guild TGuild = Guilds.Value;
                        if (d.ReadUInt16("EnemiesId5") == TGuild.GuildID)
                        {
                            //C.MyClient.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            if (!G.Enemies.ContainsKey(TGuild.GuildID))
                                G.Enemies.Add(TGuild.GuildID, TGuild);
                        }
                    }
                }
            }
        }

        public static void AddAllisToGuild(Game.Character C, Features.Guild G)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.SELECT);
            cmd2.Select("guildallies").Where("GuildID", C.MyGuild.GuildID);
            MySqlReader r = new MySqlReader(cmd2);
            if (r.Read())
            {
                if (r.ReadUInt16("AlliesId1") == 0)
                {
                    cmd3.Update("guildallies").Set("AlliesId1", G.GuildID).Set("AlliesName1", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("AlliesId2") == 0)
                {
                    cmd3.Update("guildallies").Set("AlliesId2", G.GuildID).Set("AlliesName2", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("AlliesId3") == 0)
                {
                    cmd3.Update("guildallies").Set("AlliesId3", G.GuildID).Set("AlliesName3", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("AlliesId4") == 0)
                {
                    cmd3.Update("guildallies").Set("AlliesId4", G.GuildID).Set("AlliesName4", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("AlliesId5") == 0)
                {
                    cmd3.Update("guildallies").Set("AlliesId5", G.GuildID).Set("AlliesName5", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
            }
        }
        public static void AddEnemiesToGuild(Game.Character C, Features.Guild G)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.SELECT);
            cmd2.Select("guildenemies").Where("GuildID", C.MyGuild.GuildID);
            MySqlReader r = new MySqlReader(cmd2);
            if (r.Read())
            {
                if (r.ReadUInt16("EnemiesId1") == 0)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId1", G.GuildID).Set("EnemiesName1", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId2") == 0)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId2", G.GuildID).Set("EnemiesName2", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId3") == 0)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId3", G.GuildID).Set("EnemiesName3", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId4") == 0)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId4", G.GuildID).Set("EnemiesName4", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId5") == 0)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId5", G.GuildID).Set("EnemiesName5", G.GuildName).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
            }
        }

        public static void DeleteEnemiesFromGuild(Game.Character C, Features.Guild G)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.SELECT);
            cmd2.Select("guildenemies").Where("GuildID", C.MyGuild.GuildID);
            MySqlReader r = new MySqlReader(cmd2);
            if (r.Read())
            {
                if (r.ReadUInt16("EnemiesId1") == G.GuildID)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId1", 0).Set("EnemiesName1", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId2") == G.GuildID)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId2", 0).Set("EnemiesName2", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId3") == G.GuildID)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId3", 0).Set("EnemiesName3", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId4") == G.GuildID)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId4", 0).Set("EnemiesName4", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
                else if (r.ReadUInt16("EnemiesId5") == G.GuildID)
                {
                    cmd3.Update("guildenemies").Set("EnemiesId5", 0).Set("EnemiesName5", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                }
            }
        }
        public static void DeleteAllisFromGuild(Game.Character C, Features.Guild G)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.SELECT);
            cmd2.Select("guildallies").Where("GuildID", C.MyGuild.GuildID);
            MySqlReader r = new MySqlReader(cmd2);
            if (r.Read())
            {
                try
                {
                    if (r.ReadUInt16("AlliesId1") == G.GuildID)
                    {
                        cmd3.Update("guildallies").Set("AlliesId1", 0).Set("AlliesName1", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                    }
                    else if (r.ReadUInt16("AlliesId2") == G.GuildID)
                    {
                        cmd3.Update("guildallies").Set("AlliesId2", 0).Set("AlliesName2", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                    }
                    else if (r.ReadUInt16("AlliesId3") == G.GuildID)
                    {
                        cmd3.Update("guildallies").Set("AlliesId3", 0).Set("AlliesName3", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                    }
                    else if (r.ReadUInt16("AlliesId4") == G.GuildID)
                    {
                        cmd3.Update("guildallies").Set("AlliesId4", 0).Set("AlliesName4", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                    }
                    else if (r.ReadUInt16("AlliesId5") == G.GuildID)
                    {
                        cmd3.Update("guildallies").Set("AlliesId5", 0).Set("AlliesName5", null).Where("GuildID", C.MyGuild.GuildID).Execute();
                    }
                }
                catch { Console.WriteLine("ERORRRRRRRRRRRRRRRRRRRRRR ALLIES"); }
            }
        }

        public static void DeleteAllisFromOtherGuild(Game.Character C, Features.Guild G)
        {
            MySqlCommand cmd4 = new MySqlCommand(MySqlCommandType.UPDATE);
            MySqlCommand cmd5 = new MySqlCommand(MySqlCommandType.SELECT);
            cmd5.Select("guildallies").Where("GuildID", G.GuildID);
            MySqlReader s = new MySqlReader(cmd5);
            if (s.Read())
            {
                try
                {
                    if (s.ReadUInt16("AlliesId1") == C.MyGuild.GuildID)
                    {
                        cmd4.Update("guildallies").Set("AlliesId1", 0).Set("AlliesName1", null).Where("GuildID", G.GuildID).Execute();
                    }
                    else if (s.ReadUInt16("AlliesId2") == C.MyGuild.GuildID)
                    {
                        cmd4.Update("guildallies").Set("AlliesId2", 0).Set("AlliesName2", null).Where("GuildID", G.GuildID).Execute();
                    }
                    else if (s.ReadUInt16("AlliesId3") == C.MyGuild.GuildID)
                    {
                        cmd4.Update("guildallies").Set("AlliesId3", 0).Set("AlliesName3", null).Where("GuildID", G.GuildID).Execute();
                    }
                    else if (s.ReadUInt16("AlliesId4") == C.MyGuild.GuildID)
                    {
                        cmd4.Update("guildallies").Set("AlliesId4", 0).Set("AlliesName4", null).Where("GuildID", G.GuildID).Execute();
                    }
                    else if (s.ReadUInt16("AlliesId5") == C.MyGuild.GuildID)
                    {
                        cmd4.Update("guildallies").Set("AlliesId5", 0).Set("AlliesName5", null).Where("GuildID", G.GuildID).Execute();
                    }
                }
                catch { Console.WriteLine("ERORRRRRRRRRRRRRRRRRRRRRR ALLIES"); }
            }
        }
        #endregion

        #region Reset Top ClassPK War
        public static void ResetTopTrojan()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopTrojan);
                    Chaar.TopTrojan = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopTrojan", 0).Where("EntityID", 0, true).Execute();
        }
        public static void ResetTopWar()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopWarrior);
                    Chaar.TopWarrior = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopWarrior", 0).Where("EntityID", 0, true).Execute();
        }
        public static void ResetTopArcher()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopArcher);
                    Chaar.TopArcher = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopArcher", 0).Where("EntityID", 0, true).Execute();
        }
        public static void ResetTopWater()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopWaterTaoist);
                    Chaar.TopWaterTaoist = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopWaterTaoist", 0).Where("EntityID", 0, true).Execute();
        }
        public static void ResetTopFire()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopFireTaoist);
                    Chaar.TopFireTaoist = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopFireTaoist", 0).Where("EntityID", 0, true).Execute();
        }
        public static void ResetTopNinja()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopNinja);
                    Chaar.TopNinja = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopNinja", 0).Where("EntityID", 0, true).Execute();
        }
        #endregion
        #region Reset Top WeeklyPK War
        public static void ResetTopWeeklyPK()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopWeekly);
                    Chaar.TopWeekly = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("WeeklyPKChampion", 0).Where("EntityID", 0, true).Execute();
        }
        #endregion
        #region Reset Top MonthlyPK War
        public static void ResetTopMonthlyPK()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopMonthly);
                    Chaar.TopMonthly = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("MonthlyPKChampion", 0).Where("EntityID", 0, true).Execute();
        }
        #endregion

        #region Reset Top Guild
        public static void ResetTopGuild()
        {
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character Chaar = (Game.Character)DE.Value;
                {
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopDeputyLeader);
                    Chaar.StatEff.Remove(Server.Game.StatusEffectEn.TopGuildLeader);
                    Chaar.TopGuildLeader = 0;
                    Chaar.TopDeputyLeader = 0;
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopGuildLeader", 0).Where("EntityID", 0, true).Execute();
            MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd2.Update("characters").Set("TopDeputyLeader", 0).Where("EntityID", 0, true).Execute();
            {
                StreamWriter sw = new StreamWriter(Program.ConquerPath + @"Tops/TopDeputy.txt");
                sw = new StreamWriter(Program.ConquerPath + @"Tops/TopMember.txt");
                sw.Close();
            }
        }
        #endregion

        #region Reset EXPBall/Elighten/DBUsed/Lottry
        public static void ResetExpBall()
        {
            if (!File.Exists(Program.ConquerPath + @"Time//" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + ".txt"))
            {
                System.IO.File.Create(Program.ConquerPath + @"Time//" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + ".txt").Close();
                foreach (DictionaryEntry DE in Game.World.H_Chars)
                {
                    Game.Character Chaar = (Game.Character)DE.Value;
                    {
                        Chaar.LotteryUsed = 0;
                        Chaar.DbUsedToday = 0;
                        Chaar.ExpBallsUsedToday = 0;
                        Chaar.ElighemPoints = 0;
                        Chaar.ElightenAdd = 1;
                        Chaar.EnhligtehnRequest = 0;
                    }
                }
                MySqlCommand cmd1 = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd1.Update("characters").Set("ExpBallUsedToday", 0).Set("ElightenAdd", 1).Set("EnhligtehnRequest", 0).Set("ElighemPoints", 0).Set("DBUsedToday", 0).Set("LotteryUsedToday", 0).Where("EntityID", 0, true).Execute();
            }
        }
        #endregion

        #region Save Steps
        public static void savebody(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Body", C.Body).Where("EntityID", C.EntityID).Execute();
        }
        public static void savevip(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("VipLevel", C.VipLevel).Where("EntityID", C.EntityID).Execute();
        }
        public static void savelife(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("CurHP", C.CurHP).Where("EntityID", C.EntityID).Execute();
        }
        public static void savemana(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("CurMP", C.CurMP).Where("EntityID", C.EntityID).Execute();
        }
        public static void savemap(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Map", C.Loc.Map).Set("X", C.Loc.X).Set("Y", C.Loc.Y).Set("PreviousMap", C.Loc.PreviousMap).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveHair(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Hair", C.Hair).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveEqupsAndWH(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Warehouses", C.Warehouses.WriteThis()).Set("Equips", C.Equips.WriteThis()).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveJob(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Job", C.Job).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveLevel(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Level", C.Level).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveStr(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Str", C.Str).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveAgi(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Agi", C.Agi).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveVit(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Vit", C.Vit).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveSpi(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Spi", C.Spi).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveCharStatus(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("StatPoints", C.StatusPoints).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveAvatar(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Avatar", C.Avatar).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveSilver(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Silvers", C.Silvers).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveCps(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("CPs", C.CPs).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveWhSilver(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("WHSilvers", C.WHSilvers).Where("EntityID", C.EntityID).Execute();
        }
        public static void SavePkPoints(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("PKPoints", C.PKPoints).Where("EntityID", C.EntityID).Execute();
        }
        public static void SavePreviousjob1(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("PreviousJob", C.PreviousJob1).Where("EntityID", C.EntityID).Execute();
        }
        public static void SavePreviousjob2(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("PreviousJob2", C.PreviousJob2).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveReborn(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("Reborns", C.Reborns).Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveTop(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("TopTrojan", C.TopTrojan).Set("TopWarrior", C.TopWarrior).Set("TopNinja", C.TopNinja)
                        .Set("TopWaterTaoist", C.TopWaterTaoist).Set("TopArcher", C.TopArcher).Set("TopGuildLeader", C.TopGuildLeader)
                        .Set("TopFireTaoist", C.TopFireTaoist).Set("TopDeputyLeader", C.TopDeputyLeader).Set("WeeklyPKChampion", C.TopWeekly)
                        .Where("EntityID", C.EntityID).Execute();
        }
        public static void SaveUniversity(Game.Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("characters").Set("UniversityPoints", C.UniversityPoints).Where("EntityID", C.EntityID).Execute();
        }
        #endregion

        #region Confiscator
        public static bool Reward(Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("Confiscator").Where("CharID2", C.EntityID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                {
                    Game.Item Item = new Game.Item();
                    Item.Bless = r.ReadByte("Minus");
                    Item.CurDur = r.ReadUInt16("Dura");
                    Item.ID = r.ReadUInt32("ItemID");
                    Item.MaxDur = r.ReadUInt16("MaxDura");
                    Item.Plus = r.ReadByte("Plus");
                    Item.Position = r.ReadByte("Position");
                    Item.Soc1 = (Item.Gem)r.ReadByte("Soc1");
                    Item.Soc2 = (Item.Gem)r.ReadByte("Soc2");
                    Item.UID = r.ReadUInt32("ItemUID");
                    Item.Enchant = r.ReadByte("Enchant");
                    Item.Color = (Item.ArmorColor)r.ReadByte("Color");
                    Item.Suspicious = r.ReadInt16("Suspicious");
                    Item.FreeItem = r.ReadBoolean("Free");
                    Item.Locked = r.ReadByte("Locked");
                    Item.Progress = r.ReadUInt16("Progress");
                    Item.LockedDays = r.ReadUInt32("LockedDay");
                    Item.TalismanProgress = r.ReadUInt32("SocketProgress");
                    //C.Equips.Armor = Item;
                    if (Item.ID == 300000)
                    {
                        Item.RBG[0] = r.ReadByte("X");
                        Item.RBG[1] = r.ReadByte("Y");
                        Item.RBG[2] = r.ReadByte("Z");
                        Item.RBG[3] = r.ReadByte("floor");
                        Item.TalismanProgress = BitConverter.ToUInt32(Item.RBG, 0);
                    }
                    Item.Effect = (Item.RebornEffect)r.ReadByte("Effect");
                    Item.NameReward = r.ReadString("Name2");
                    Item.Time = r.ReadUInt32("Time");
                    if (Item.Position == 1)
                        continue;
                    C.ConfiscatorReward.Add(Item.UID, Item);

                    C.MyClient.SendPacket(Packets.ConfiscatorReward(Item, C, (ushort)Features.Confiscator.CpsItem(Item), Item.NameReward));

                }
            }
            return true;
        }
        public static void update_CI_position(uint UID, Game.Character C)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("Confiscator").Set("Position", 1).Where("ItemUID", UID).And("CharID2", C.EntityID).Execute();
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }
        public static void DeleteReward(uint UID, Game.Character C)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("Confiscator", "ItemUID", UID).And("CharID", C.EntityID).Execute();
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }
        public static void DeleteClain(uint UID, Game.Character C)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("Confiscator", "ItemUID", UID).And("CharID", C.EntityID).Execute();
            }
            catch (Exception Exe)
            {
                Program.WriteMessage(Exe);
            }
        }
        public static bool Clain(Character C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("Confiscator").Where("CharID", C.EntityID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                {
                    Game.Item Item = new Game.Item();
                    Item.Bless = r.ReadByte("Minus");
                    Item.CurDur = r.ReadUInt16("Dura");
                    Item.ID = r.ReadUInt32("ItemID");
                    Item.MaxDur = r.ReadUInt16("MaxDura");
                    Item.Plus = r.ReadByte("Plus");
                    Item.Position = r.ReadByte("Position");
                    Item.Soc1 = (Item.Gem)r.ReadByte("Soc1");
                    Item.Soc2 = (Item.Gem)r.ReadByte("Soc2");
                    Item.UID = r.ReadUInt32("ItemUID");
                    Item.Enchant = r.ReadByte("Enchant");
                    Item.Color = (Item.ArmorColor)r.ReadByte("Color");
                    Item.Suspicious = r.ReadInt16("Suspicious");
                    Item.FreeItem = r.ReadBoolean("Free");
                    Item.Locked = r.ReadByte("Locked");
                    Item.Progress = r.ReadUInt16("Progress");
                    Item.LockedDays = r.ReadUInt32("LockedDay");
                    Item.TalismanProgress = r.ReadUInt32("SocketProgress");
                    //C.Equips.Armor = Item;
                    if (Item.ID == 300000)
                    {
                        Item.RBG[0] = r.ReadByte("X");
                        Item.RBG[1] = r.ReadByte("Y");
                        Item.RBG[2] = r.ReadByte("Z");
                        Item.RBG[3] = r.ReadByte("floor");
                        Item.TalismanProgress = BitConverter.ToUInt32(Item.RBG, 0);
                    }
                    Item.Effect = (Item.RebornEffect)r.ReadByte("Effect");
                    Item.NameClain = r.ReadString("Name1");
                    Item.Time = r.ReadUInt32("Time");
                    C.ConfiscatorClain.Add(Item.UID, Item);
                    if (Item.Position == 1)
                        C.MyClient.SendPacket(Packets.Sac(Item, C, (ushort)Features.Confiscator.CpsItem(Item), Item.NameClain));
                    else
                        C.MyClient.SendPacket(Packets.ConfiscatorClain(Item, C, (ushort)Features.Confiscator.CpsItem(Item), Item.NameClain));

                }
            }
            return true;
        }
        public static bool ConfiscatorReward(Game.Item Item, Game.Character GC)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("Confiscator").Insert("ItemUID", Item.UID).Insert("Position", Item.Position).Insert("CharID", GC.EntityID).Insert("ItemID", Item.ID).Insert("Plus", Item.Plus).Insert("Minus", Item.Bless).Insert("Enchant", Item.Enchant).Insert("Soc1", (byte)Item.Soc1).Insert("Soc2", (byte)Item.Soc2).Insert("Dura", Item.CurDur).Insert("MaxDura", Item.MaxDur).Insert("LockedDay", Item.LockedDays).Insert("Color", (byte)Item.Color).Insert("Suspicious", Item.Suspicious).Insert("Free", Item.FreeItem).Insert("Locked", Item.Locked).Insert("Progress", Item.Progress).Insert("SocketProgress", Item.TalismanProgress).Insert("X", Item.RBG[0])
                    .Insert("Y", Item.RBG[1]).Insert("Z", Item.RBG[2]).Insert("floor", Item.RBG[3]).Insert("Effect", (byte)Item.Effect).Insert("Time", Item.Time).Insert("Name1", Item.NameReward);
                cmd.Execute();
            }
            catch
            {
                return false;

            }
            return true;
        }
        public static bool ConfiscatorC(Game.Item Item, Game.Character victum, Game.Character Killer)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("Confiscator").Insert("ItemUID", Item.UID).Insert("Position", Item.Position).Insert("CharID2", victum.EntityID).Insert("CharID", Killer.EntityID).Insert("ItemID", Item.ID).Insert("Plus", Item.Plus).Insert("Minus", Item.Bless).Insert("Enchant", Item.Enchant).Insert("Soc1", (byte)Item.Soc1).Insert("Soc2", (byte)Item.Soc2).Insert("Dura", Item.CurDur).Insert("MaxDura", Item.MaxDur).Insert("LockedDay", Item.LockedDays).Insert("Color", (byte)Item.Color).Insert("Suspicious", Item.Suspicious).Insert("Free", Item.FreeItem).Insert("Locked", Item.Locked).Insert("Progress", Item.Progress).Insert("SocketProgress", Item.TalismanProgress).Insert("X", Item.RBG[0])
                    .Insert("Y", Item.RBG[1]).Insert("Z", Item.RBG[2]).Insert("floor", Item.RBG[3]).Insert("Effect", (byte)Item.Effect).Insert("Time", Item.Time).Insert("Name1", Item.NameClain).Insert("Name2", Item.NameReward);
                cmd.Execute();
            }
            catch
            {
                return false;

            }
            return true;
        }
        #endregion
    }

    public class IniFile
    {
        public string path;
        public IniFile(string INIPath)
        {
            path = INIPath;
            if (File.Exists(path))
            {
                Read();
            }
        }
        public void Read()
        {
            #region IniSectionSelect
            string[] Lines = File.ReadAllLines(path);
            string Ssection = "";
            foreach (string Line in Lines)
            {
                if (Line.Length > 0)
                {
                    if (Line[0] == '[' && Line[Line.Length - 1] == ']')
                    {
                        Ssection = Line;
                        IniSectionStructure Section = new IniSectionStructure();
                        Section.SectionName = Ssection;
                        Section.Variables = new Dictionary<string, IniValueStructure>();
                        Sections.Add(Ssection, Section);
                    }
                    else
                    {
                        IniValueStructure IvS = new IniValueStructure();
                        IvS.Variable = Line.Split('=')[0];
                        IvS.Value = Line.Split('=')[1];
                        IniSectionStructure Section = null;
                        Sections.TryGetValue(Ssection, out Section);
                        if (Section != null)
                        {
                            if (!Section.Variables.ContainsKey(IvS.Variable))
                                Section.Variables.Add(IvS.Variable, IvS);
                        }
                    }
                }
            }
            #endregion
        }
        Dictionary<string, IniSectionStructure> Sections = new Dictionary<string, IniSectionStructure>();
        public void Close()
        {
            Sections.Clear();
        }
        public void Save()
        {
            string Text = "";
            foreach (IniSectionStructure Section in Sections.Values)
            {
                Text += Section.SectionName + "\r\n";
                foreach (IniValueStructure IVS in Section.Variables.Values)
                {
                    Text += IVS.Variable + "=" + IVS.Value + "\r\n";
                }
            }
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();
                File.WriteAllText(path, Text);
            }
            else
            {
                File.Create(path).Close();
                File.WriteAllText(path, Text);
            }
        }
        class IniValueStructure
        {
            public string Variable;
            public string Value;
        }
        class IniSectionStructure
        {
            public Dictionary<string, IniValueStructure> Variables;
            public string SectionName;
        }
        private void IniWriteValue(string ssection, string Key, string Value)
        {
            string section = "[" + ssection + "]";
            IniSectionStructure _Section = null;
            Sections.TryGetValue(section, out _Section);
            if (_Section != null)
            {
                IniValueStructure IVS = null;
                _Section.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                {
                    if (IVS.Variable == Key)
                    {
                        IVS.Value = Value;
                    }
                }
                else
                {
                    _Section.Variables.Add(Key, new IniValueStructure() { Value = Value, Variable = Key });
                }
            }
            else
            {
                _Section = new IniSectionStructure() { SectionName = section, Variables = new Dictionary<string, IniValueStructure>() };
                Sections.Add(section, _Section);
                IniValueStructure IVS = null;
                _Section.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                {
                    if (IVS.Variable == Key)
                    {
                        IVS.Value = Value;
                    }
                }
                else
                {
                    _Section.Variables.Add(Key, new IniValueStructure() { Value = Value, Variable = Key });
                }
            }
        }

        #region Read
        public byte ReadByte(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return byte.Parse(IVS.Value);
            }
            return 0;
        }
        public sbyte ReadSbyte(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return sbyte.Parse(IVS.Value);
            }
            return 0;
        }
        public short ReadInt16(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return short.Parse(IVS.Value);
            }
            return 0;
        }
        public int ReadInt32(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return int.Parse(IVS.Value);
            }
            return 0;
        }
        public long ReadInt64(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return long.Parse(IVS.Value);
            }
            return 0;
        }
        public ushort ReadUInt16(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return ushort.Parse(IVS.Value);
            }
            return 0;
        }
        public uint ReadUInt32(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return uint.Parse(IVS.Value);
            }
            return 0;
        }
        public ulong ReadUInt64(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return ulong.Parse(IVS.Value);
            }
            return 0;
        }
        public double ReadDouble(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return double.Parse(IVS.Value);
            }
            return 0;
        }
        public float ReadFloat(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return float.Parse(IVS.Value);
            }
            return 0;
        }
        public string ReadString(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return IVS.Value;
            }
            return "";
        }
        public bool ReadBoolean(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return byte.Parse(IVS.Value) == 1 ? true : false; ;
            }
            return false;
        }
        #endregion
        #region Write
        public void WriteString(string Section, string Key, string Value)
        {
            IniWriteValue(Section, Key, Value);
        }
        public void WriteInteger(string Section, string Key, byte Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, ulong Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, double Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, long Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, float Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteBoolean(string Section, string Key, bool Value)
        {
            IniWriteValue(Section, Key, (Value == true ? 1 : 0).ToString());
        }
        #endregion
    }
}