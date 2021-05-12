using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server.Game;

namespace Server
{
    public class DropRates
    {
        public static double

        DragonBall = 1,
        Meteor = 3,
        Silver = 10,

        Item = 0,
        PlusOne = 0,
        Refined = 0,
        Unique = 0,
        Elite = 0,
        Super = 0,
        OneSoc = 0,
        TwoSoc = 0,

        NormalGem = 0,
        RefinedGem = 0,
        SuperGem = 0;

        public static Hashtable Specifics = new Hashtable();
        public static Hashtable EquipDrops = new Hashtable();

        public struct RateItemInfo
        {
            public int MonsterID;
            public uint ItemID;
            public byte Plus;
            public byte Bless;
            public byte Sockets;
            public double DropChance;
        }
        public static void Load()
        {
            string[] Lines = File.ReadAllLines(Program.ConquerPath + @"Monsters\MonstersDrop.txt");
            int Current = 0;
            ArrayList CurArr = null;
            foreach (string Line in Lines)
            {
                if (!Line.StartsWith("#"))
                {
                    if (Line.Length > 0)
                    {
                        string[] E = Line.Split(' ');
                        if (E.Length == 5)
                        {
                            if (!Specifics.Contains(Current))
                            {
                                CurArr = new ArrayList();
                                RateItemInfo R = new RateItemInfo();
                                Current = int.Parse(E[0]);
                                R.MonsterID = Current;
                                R.ItemID = uint.Parse(E[1]);
                                R.Plus = byte.Parse(E[2]);
                                R.Bless = byte.Parse(E[3]);
                                R.Sockets = byte.Parse(E[4]);
                                R.DropChance = double.Parse(E[5]);
                                CurArr.Add(R);
                                Specifics.Add(Current, CurArr);
                            }
                        }
                    }
                }
            }

            /*IniFile iNi = new IniFile(Program.ConquerPath + @"DropRates.ini");
            DragonBall = iNi.ReadDouble("Rates", "DragonBall");
            Meteor = iNi.ReadDouble("Rates", "Meteor");
            PlusOne = iNi.ReadDouble("Rates", "PlusOne"); 
            Refined = iNi.ReadDouble("Rates", "Refined"); 
            Unique = iNi.ReadDouble("Rates", "Unique"); 
            Elite = iNi.ReadDouble("Rates", "Elite"); 
            Super = iNi.ReadDouble("Rates", "Super"); 
            OneSoc = iNi.ReadDouble("Rates", "OneSoc"); 
            TwoSoc = iNi.ReadDouble("Rates", "TwoSoc"); 
            Gem = iNi.ReadDouble("Rates", "Gem"); 
            Silver = iNi.ReadDouble("Rates", "Silver"); 
            CP10 = iNi.ReadDouble("Rates", "CP10"); 
            CP30 = iNi.ReadDouble("Rates", "CP30"); 
            CP60 = iNi.ReadDouble("Rates", "CP60"); 
            PlusOneStone = iNi.ReadDouble("Rates", "PlusOneStone"); 
            PlusTwoStone = iNi.ReadDouble("Rates", "PlusTwoStone"); 
            CPMiniBag = iNi.ReadDouble("Rates", "CPMiniBag"); 
            CPBag = iNi.ReadDouble("Rates", "CPBag"); 
            CPBackpack = iNi.ReadDouble("Rates", "CPBackpack"); 
            CleanWater = iNi.ReadDouble("Rates", "CleanWater"); 
            Item = iNi.ReadDouble("Rates", "Item"); 
            PointCard = iNi.ReadDouble("Rates", "PointCard");

            Lines = File.ReadAllLines(Program.ConquerPath + @"EquipDrops.txt");
            foreach (string Line in Lines)
            {
                string[] E = Line.Split(' ');
                byte Lev = byte.Parse(E[0]);
                if (!EquipDrops.Contains(Lev))
                    EquipDrops.Add(Lev, new ArrayList());
                uint ID = uint.Parse(E[1]);
                ((ArrayList)EquipDrops[Lev]).Add(ID);
            }*/
        }
    }
}
