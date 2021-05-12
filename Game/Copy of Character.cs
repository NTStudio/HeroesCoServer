using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using NewestCOServer.Features;
using System.IO;

namespace NewestCOServer.Game
{
    //public enum PKMode : byte
    // {
    //      PK = 0, Peace, Team, Capture
    // }
    public enum PKMode : byte
    {
        PK = 0,
        Peace = 1,
        Team = 2,
        Capture = 3
    }
    public enum StatusEffectEn : ulong
    {
        Normal = 0x0,
        BlueName = 0x1,
        Poisoned = 0x2,
        Gone = 0x4,
        XPStart = 0x10,
        TeamLeader = 0x40,
        Accuracy = 0x80,
        Shield = 0x100,
        Stigma = 0x200,
        Dead = 0x420,
        Invisible = 0x400000,
        RedName = 0x4000,
        BlackName = 0x8000,
        SuperMan = 0x40000,
        Cyclone = 0x800000,
        Cyclone = 0x800000,
        Fly = 0x8000000,
        Pray = 0x40000000,
        Blessing = 8589934592,
        TopGuildLeader = 17179869184,
        TopDeputyLeader = 34359738368,
        MonthlyPKChampion = 68719476736,
        WeeklyPKChampion = 137438953472,
        TopWarrior = 274877906944,
        TopTrojan = 549755813888,
        TopArcher = 1099511627776,
        TopWaterTaoist = 2199023255552,
        TopFireTaoist = 4398046511104,
        TopNinja = 8796093022208,
        ShurikenVortex = 70368744177664,
        FatalStrike = 140737488355328,
        Flashy = 281474976710656,
        Ride = 1125899906842624,
        Curse = 0x100000000
        /*
         * 0000000000000000 NULL NULL
0000000000000001 NULL NULL
0000000000000002 poisonstate NULL
0000000000000004 NULL NULL
0000000000000008 NULL NULL
0000000000000010 NULL NULL
0000000000000020 NULL NULL
0000000000000040 NULL TeamLeader
0000000000000080 attackfast40 NULL
0000000000000100 zf2-e307 NULL
0000000000000200 attackup40 NULL
0000000000000400 NULL NULL
0000000000000800 NULL NULL
0000000000004000 NULL NULL
0000000000008000 NULL NULL
0000000000010000 NULL NULL
0000000000020000 Reflect NULL
0000000000040000 SuperSoldier NULL
0000000000080000 BodyShield NULL
0000000000100000 GodBelieve NULL
0000000000200000 NULL NULL
0000000000400000 NULL NULL
0000000000800000 Tornado NULL
0000000001000000 NULL NULL
0000000002000000 ReflectMagic NULL
0000000004000000 Dodge NULL
0000000008000000 NULL NULL
0000000010000000 NULL NULL
0000000040000000 LuckDiffuse NULL
0000000080000000 LuckAbsorb NULL
0000000100000000 curse NULL
0000000200000000 bless NULL
0000000400000000 gamemain NULL
0000000800000000 gameassistant NULL
0000001000000000 gamemonth NULL
0000002000000000 gameweek NULL
0000004000000000 gamefighter NULL
0000008000000000 gamewarrior NULL
0000010000000000 gamebow NULL
0000020000000000 gamewater NULL
0000040000000000 gamefire NULL
0000080000000000 gamegulp NULL
800000000000 endureXPstate NULL
400000000000 cyclonehandcycle NULL
1000000000000 PKchampion NULL

         * */
    }
    public class StatusEffect
    {
        public ArrayList Array;
        public StatusEffectEn Value;
        Character Entity;

        public StatusEffect(Character C)
        {
            Entity = C;
            Array = new ArrayList();
            Value = StatusEffectEn.Normal;
        }
        public void Add(StatusEffectEn SE)
        {
            if (!Array.Contains(SE))
            {
                Array.Add(SE);
                Value |= SE;
                World.Spawn(Entity, false);
            }
            if (Entity != null && Entity.MyClient != null)
                Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
        }

        public bool Contains(StatusEffectEn SE)
        {
            return Array.Contains(SE);
        }
        public void Remove(StatusEffectEn SE)
        {
            if (Array.Contains(SE))
            {
                Array.Remove(SE);
                Value -= SE;
                Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
                World.Spawn(Entity, false);
            }
        }
        public void Clear()
        {
            Array = new ArrayList();
            Value = StatusEffectEn.Normal;
            Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
        }
    }
    public class PoisonType
    {
        public int Times;
        public int SpellLevel;
        public DateTime LastAttack;
        public PoisonType(int spelllvl)
        {
            Times = 19;
            SpellLevel = spelllvl;
            LastAttack = DateTime.Now;
        }
    }
    public enum Status : byte
    {
        HP = 0,
        MaxHP = 1,
        MP = 2,
        MaxMP = 3,
        Silvers = 4,
        Experience = 5,
        PKPoints = 6,
        Class = 7,
        Stamina = 8,
        WHMoney = 9,
        StatPoints = 10,
        Mesh = 11,
        Level = 12,
        Spirit = 13,
        Vitality = 14,
        Strength = 15,
        Agility = 16,
        BlessTime = 17,
        DoubleExpTime = 18,
        CurseTime = 20,
        RebirthCount = 22,
        Effect = 25,
        Hair = 26,
        XPCircle = 27,
        LuckyTime = 28,
        CPs = 29,
        EE1 = 30,
       // countskill = 54,
        OnlineTraining = 31,/// value = 1 is for tg 
        PotFromMentor = 36,
        CPSDOnators = 37,// no sure ???????
        Merchant = 38,
        VIPLevel = 39,
        QuizPts = 40,
        Elighten = 41,
    }
    public class Location2
    {
        Character C;
        ushort xX;
        ushort xY;
        ushort xMap;
        ushort xMapcopy;

        public ushort PreviousX;
        public ushort PreviousY;
        ushort xPreviousMap;
        public DateTime LastJump;
        public Location2(Character CC)
        {
            C = CC;
        }
        public ushort X
        {
            get { return xX; }
            set
            {
                xX = value;
            }
        }
        public ushort Y
        {
            get { return xY; }
            set
            {
                xY = value;
            }
        }
        public ushort Map
        {
            get { return xMap; }
            set
            {
                xMap = value;
            }
        }
        public ushort PreviousMap
        {
            get { return xPreviousMap; }
            set
            {
                xPreviousMap = value;
            }
        }
        
        public void Walk(byte Dir)
        {
            PreviousX = X;
            PreviousY = Y;

            if (Dir == 0)
                Y += 1;
            if (Dir == 2)
                X -= 1;
            if (Dir == 4)
                Y -= 1;
            if (Dir == 6)
            {
                X += 1;
            }
            if (Dir == 1)
            {
                X -= 1;
                Y += 1;
            }
            if (Dir == 3)
            {
                X -= 1;
                Y -= 1;
            }
            if (Dir == 5)
            {
                X += 1;
                Y -= 1;
            }
            if (Dir == 7)
            {
                X += 1;
                Y += 1;
            }
        }
        public bool AbleToJump(ushort NX, ushort NY, bool Speed)
        {//450
            if (MyMath.PointDistance(NX, NY, X, Y) < 22 && (DateTime.Now > LastJump.AddMilliseconds(450) || Speed))
            {
                LastJump = DateTime.Now;
                DMap DM = ((DMap)DMaps.H_DMaps[Map]);
                if (DM != null)
                {
                    DMapCell New = DM.GetCell(NX, NY);
                    if (Map == 1038)
                    {
                        DMapCell Old = DM.GetCell(X, Y);
                        if (New.High)
                            if (!Old.High)
                            { return false; }
                    }
                    if (!New.NoAccess)
                        return true;
                }
                else return true;
                return false;
            }
            return false;
        }
        public void Jump(ushort NX, ushort NY)
        {
            LastJump = DateTime.Now;
            PreviousX = X;
            PreviousY = Y;
            X = NX;
            Y = NY;
        }
        public static implicit operator Location(Location2 l)
        {
            Location L = new Location()
            {
                X = l.X,
                Y = l.Y,
                Map = l.Map,
                PreviousMap = l.PreviousMap,
                LastJump = l.LastJump,
                PreviousX = l.PreviousX,
                PreviousY = l.PreviousY
            };
            return L;
        }
    }
    public struct Location
    {
        public ushort X;
        public ushort Y;
        public ushort Map;
        public ushort MapCopy;

        public ushort PreviousX;
        public ushort PreviousY;
        public ushort PreviousMap;
        public DateTime LastJump;


        public void SteedWalk(byte Dir)
        {
            PreviousX = X;
            PreviousY = Y;

            if (Dir == 0)
              Y +=  1;
            if (Dir == 2)
                X -= 1;
            if (Dir == 4)
                Y -= 1;
            if (Dir == 6)
            {
              X += 1;

            }
            if (Dir == 1)
            {
                X -= 1;
                Y += 1;
            }
            if (Dir == 3)
            {
                X -= 1;
                Y -= 1;
            }
            if (Dir == 5)
            {
                X += 1;
                Y -= 1;
            }
            if (Dir == 7)
            {
                X += 1;
                Y += 1;
            }
        }
        public void Walk(byte Dir)
        {
            PreviousX = X;
            PreviousY = Y;

            if (Dir == 0)
                Y += 1;
            if (Dir == 2)
                X -= 1;
            if (Dir == 4)
                Y -= 1;
            if (Dir == 6)
                X += 1;
            if (Dir == 1)
            {
                X -= 1;
                Y += 1;
            }
            if (Dir == 3)
            {
                X -= 1;
                Y -= 1;
            }
            if (Dir == 5)
            {
                X += 1;
                Y -= 1;
            }
            if (Dir == 7)
            {
                X += 1;
                Y += 1;
            }
        }
        public bool AbleToJump(ushort NX, ushort NY, bool Speed)
        {
            if (MyMath.PointDistance(NX, NY, X, Y) < 22 && (DateTime.Now > LastJump.AddMilliseconds(450) || Speed))
            {
                LastJump = DateTime.Now;
                DMap DM = ((DMap)DMaps.H_DMaps[Map]);
                if (DM != null)
                {
                    DMapCell New = DM.GetCell(NX, NY);
                    if (Map == 1038)
                    {
                        DMapCell Old = DM.GetCell(X, Y);
                        if (New.High)
                            if (!Old.High)
                            { return false; }
                    }
                    if (!New.NoAccess)
                        return true;
                }
                else return true;
                return false;
            }
            return false;
        }
        public void Jump(ushort NX, ushort NY)
        {
            LastJump = DateTime.Now;
            PreviousX = X;
            PreviousY = Y;
            X = NX;
            Y = NY;
        }
    }
    public struct ItemIDManipulation
    {
        uint ID;
        public ItemIDManipulation(uint id)
        {
            ID = id;
        }

        public Item.ItemQuality Quality
        {
            get
            {
                return (Item.ItemQuality)Digit(6);
            }
        }
        public Item.ArmorColor Color
        {
            get
            {
                return (Item.ArmorColor)Digit(4);
            }
        }
        public void QualityChange(Item.ItemQuality Quality)
        {
            ChangeDigit(6, (byte)Quality);
        }
        public void ColorChange(Item.ArmorColor Col)
        {
            ChangeDigit(4, (byte)Col);
        }
        public uint Part(byte From, byte To)
        {
            string Item = Convert.ToString(ID);
            string type = Item.Remove(0, From);
            type = type.Remove(To - From, Item.Length - To);
            return uint.Parse(type);
        }
        public static uint Part(uint ID, byte From, byte To)
        {
            if (ID != 0)
            {
                string Item = Convert.ToString(ID);
                string type = Item.Remove(0, From);
                type = type.Remove(To - From, Item.Length - To);
                return uint.Parse(type);
            }
            return 0;
        }
        public byte Digit(byte Place)
        {
            return (byte)Part((byte)(Place - 1), Place);
        }
        public static byte Digit(uint ID, byte Place)
        {
            return (byte)Part(ID, (byte)(Place - 1), Place);
        }
        public void ChangeDigit(byte Place, byte To)
        {
            string Item = Convert.ToString(ID);
            string N = Item.Remove(Place - 1, Item.Length - Place + 1) + To.ToString();
            N += Item.Remove(0, Place);
            ID = uint.Parse(N);
        }
        public void LowestLevel(byte Pos)
        {
            ChangeDigit(4, 0);
            if (Pos == 1 || Pos == 2 || Pos == 3)
                ChangeDigit(5, 0);
            else if (Pos == 8 || Pos == 6)
                ChangeDigit(5, 1);
            else
                ChangeDigit(5, 2);
        }
        public void IncreaseLevel()
        {
            if (ID != 0)
            {
                if (Database.DatabaseItems.ContainsKey(ID))
                {
                    DatabaseItem Item = (DatabaseItem)Database.DatabaseItems[ID];
                    byte Level = Item.LevReq;
                    string Type = Item.ID.ToString().Remove(2, Item.ID.ToString().Length - 2);
                    uint WeirdThing = Convert.ToUInt32(Type);
                    if (WeirdThing <= 60 && WeirdThing >= 42)//weapon
                    {
                        if (Level < 130)
                        {
                            if (Level >= 120)
                            {
                                Level++;
                                foreach (DatabaseItem I in Database.DatabaseItems.Values)
                                {
                                    if (I.ID / 1000 == Item.ID / 1000)
                                        if (I.ID % 10 == Item.ID % 10)
                                            if (I.LevReq == Level)
                                            { ID = I.ID; return; }
                                }
                            }
                            else
                            {
                            Again:
                                Level++;
                                foreach (DatabaseItem I in Database.DatabaseItems.Values)
                                {
                                    if (I.ID / 1000 == Item.ID / 1000)
                                        if (I.ID % 10 == Item.ID % 10)
                                            if (I.LevReq == Level)
                                            { ID = I.ID; return; }
                                }
                                goto Again;
                            }
                        }
                    }
                    else
                    {
                        if (WeirdThing == 20)
                            return;
                    Again:
                        Level++;
                        foreach (DatabaseItem I in Database.DatabaseItems.Values)
                        {
                            if (I.ID / 1000 == Item.ID / 1000)
                                if (I.ID % 10 == Item.ID % 10)
                                    if (I.LevReq == Level)
                                    { ID = I.ID; return; }
                        }
                        goto Again;
                    }
                }
            }
        }
        public uint ToID()
        {
            return ID;
        }
        public uint ToComposeID(byte EqPos)
        {
            uint id = ID;

            if (EqPos == 1)
            {
                ChangeDigit(4, 0);
                ChangeDigit(6, 0);
            }
            else if (EqPos == 3)
            {
                ChangeDigit(3, (byte)(Digit(3) - 5));
                ChangeDigit(4, 0);
                ID += 1;
            }
            else if (EqPos == 2 || EqPos == 6 || EqPos == 8)
                ChangeDigit(6, 0);
            else if (EqPos == 4 || EqPos == 5)
            {
                if (Part(0, 3) == 500 || Part(0, 3) == 421 || Part(0, 3) == 601)
                    ChangeDigit(6, 0);
                else if (Digit(1) == 9)
                    ChangeDigit(4, 0);
                else if (Digit(1) == 4 || Digit(1) == 5)
                {
                    ChangeDigit(3, Digit(1));
                    ChangeDigit(2, Digit(1));
                    ChangeDigit(1, Digit(1));
                    ChangeDigit(6, 0);
                }
            }
            else if (EqPos == 10 || EqPos == 11)
                ChangeDigit(6, 0);

            uint ret = ID;
            ID = id;
            return ret;
        }
    }
    public struct Equipment
    {
        public Item HeadGear;
        public Item Necklace;
        public Item Armor;
        public Item LeftHand;
        public Item RightHand;
        public Item Ring;
        public Item Garment;
        public Item Boots;
        public Item Gourd;
        public Item Fan;
        public Item Tower;
        public Item Steed;

        public void Open()
        {
            HeadGear = new Item();
            Necklace = new Item();
            Armor = new Item();
            LeftHand = new Item();
            RightHand = new Item();
            Ring = new Item();
            Garment = new Item();
            Boots = new Item();
            Gourd = new Item();
            Fan = new Item();
            Tower = new Item();
            Steed = new Item();
        }
        public byte GetSlot(uint UID)
        {
            if (HeadGear.UID == UID) return 1;
            if (Necklace.UID == UID) return 2;
            if (Armor.UID == UID) return 3;
            if (RightHand.UID == UID) return 4;
            if (LeftHand.UID == UID) return 5;
            if (Ring.UID == UID) return 6;
            if (Gourd.UID == UID) return 7;
            if (Boots.UID == UID) return 8;
            if (Garment.UID == UID) return 9;
            if (Fan.UID == UID) return 10;
            if (Tower.UID == UID) return 11;
            if (Steed.UID == UID) return 12;
            return 0;
        }

        public Item Get(byte Pos)
        {

            if (Pos == 1) return HeadGear;
            else if (Pos == 2) return Necklace;
            else if (Pos == 3) return Armor;
            else if (Pos == 4) return RightHand;
            else if (Pos == 5) return LeftHand;
            else if (Pos == 6) return Ring;
            else if (Pos == 7) return Gourd;
            else if (Pos == 8) return Boots;
            else if (Pos == 9) return Garment;
            else if (Pos == 10) return Fan;
            else if (Pos == 11) return Tower;
            else if (Pos == 12) return Steed;
            return new Item();
        }
        public void UnEquip(byte Pos, Character C)
        {
            if (C.Loc.Map != 1090)
            {
                C.MyClient.SendPacket(Packets.ItemPacket(Get(Pos).UID, Pos, 6));

                if (Pos == 1) HeadGear = new Item();
                if (Pos == 2) Necklace = new Item();
                if (Pos == 3) Armor = new Item();
                if (Pos == 4) RightHand = new Item();
                if (Pos == 5) LeftHand = new Item();
                if (Pos == 6) Ring = new Item();
                if (Pos == 7) Gourd = new Item();
                if (Pos == 8) Boots = new Item();
                if (Pos == 9) Garment = new Item();
                if (Pos == 10) Fan = new Item();
                if (Pos == 11) Tower = new Item();
                if (Pos == 12) Steed = new Item();
                
            }
            else
            {
                C.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Unable use this");
                return;
            }
        }
        public void Replace(byte Pos, Item I, Character C)
        {
            if (C.Loc.Map != 1090)
            {

                try
                {
                    if (I.ID == 0)
                    {
                        uint UID = Get(Pos).UID;
                        C.MyClient.SendPacket(Packets.ItemPacket(UID, Pos, 6));
                        C.MyClient.SendPacket(Packets.ItemPacket(UID, 0, 3));
                    }
                    else
                    {
                        if (I.UID == 0)
                            I.UID = (uint)Program.Rnd.Next(10000000);
                        C.MyClient.SendPacket(Packets.AddItem(I, Pos));
                        C.MyClient.SendPacket(Packets.UpdateItem(I, Pos));
                    }

                    if (Pos == 1) HeadGear = I;
                    else if (Pos == 2) Necklace = I;
                    else if (Pos == 3) Armor = I;
                    else if (Pos == 4) RightHand = I;
                    else if (Pos == 5) LeftHand = I;
                    else if (Pos == 6) Ring = I;
                    else if (Pos == 7) Gourd = I;
                    else if (Pos == 8) Boots = I;
                    else if (Pos == 9) Garment = I;
                    else if (Pos == 10) Fan = I;
                    else if (Pos == 11) Tower = I;
                    else if (Pos == 12) Steed = I;
                }
                catch (Exception Exc) { Program.WriteLine(Exc); }
            }
            else
            {
                C.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "here unable use thas on the map");
                return;
            }
        }

        public void Send(Main.GameClient GC, bool Stats)
        {
            if (HeadGear.ID != 0) GC.SendPacket(Packets.AddItem(HeadGear, 1));
            if (Necklace.ID != 0) GC.SendPacket(Packets.AddItem(Necklace, 2));
            if (Armor.ID != 0) GC.SendPacket(Packets.AddItem(Armor, 3));
            if (RightHand.ID != 0) GC.SendPacket(Packets.AddItem(RightHand, 4));
            if (LeftHand.ID != 0) GC.SendPacket(Packets.AddItem(LeftHand, 5));
            if (Ring.ID != 0) GC.SendPacket(Packets.AddItem(Ring, 6));
            if (Gourd.ID != 0) GC.SendPacket(Packets.AddItem(Gourd, 7));
            if (Boots.ID != 0) GC.SendPacket(Packets.AddItem(Boots, 8));
            if (Garment.ID != 0) GC.SendPacket(Packets.AddItem(Garment, 9));
            if (Fan.ID != 0) GC.SendPacket(Packets.AddItem(Fan, 10));
            if (Tower.ID != 0) GC.SendPacket(Packets.AddItem(Tower, 11));
            if (Steed.ID != 0) GC.SendPacket(Packets.AddItem(Steed, 12));

            if (Stats)
            {
                GC.MyChar.EquipStats(1, true);
                GC.MyChar.EquipStats(2, true);
                GC.MyChar.EquipStats(3, true);
                GC.MyChar.EquipStats(4, true);
                GC.MyChar.EquipStats(5, true);
                GC.MyChar.EquipStats(6, true);
                GC.MyChar.EquipStats(7, true);
                GC.MyChar.EquipStats(8, true);
                GC.MyChar.EquipStats(9, true);
                GC.MyChar.EquipStats(10, true);
                GC.MyChar.EquipStats(11, true);
                GC.MyChar.EquipStats(12, true);
                GC.MyChar.LoadedEquipmentHPAdd = true;
            }
        }
        public void SendView(uint Viewed, Main.GameClient GC)
        {
            if (HeadGear.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, HeadGear, 1));
            if (Necklace.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Necklace, 2));
            if (Armor.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Armor, 3));
            if (RightHand.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, RightHand, 4));
            if (LeftHand.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, LeftHand, 5));
            if (Ring.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Ring, 6));
            if (Gourd.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Gourd, 7));
            if (Boots.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Boots, 8));
            if (Garment.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Garment, 9));
            if (Fan.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Fan, 10));
            if (Tower.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Tower, 11));
            if (Steed.ID != 0) GC.SendPacket(Packets.AddViewItem(Viewed, Steed, 12));
        }
      public string WriteThis()
        {

            string equips = HeadGear.WriteThis() + "~" + Necklace.WriteThis() + "~" + Armor.WriteThis()
                + "~" + RightHand.WriteThis() + "~" + LeftHand.WriteThis() + "~" + Ring.WriteThis() + "~"
                + Gourd.WriteThis() + "~" + Boots.WriteThis() + "~" + Garment.WriteThis() + "~" + Fan.WriteThis() + "~" + Tower.WriteThis() + "~"
                + Steed.WriteThis(); 
           return equips;

        }
        public void ReadThis(string Equips)
        {
            string[] equips = Equips.Split('~');
            HeadGear = new Item();
            Necklace = new Item();
            Armor = new Item();
            RightHand = new Item();
            LeftHand = new Item();
            Ring = new Item();
            Gourd = new Item();
            Boots = new Item();
            Garment = new Item();
            Fan = new Item();
            Tower = new Item();
            Steed = new Item();
            try
            {
                HeadGear.ReadThis(equips[0]);
                Necklace.ReadThis(equips[1]);
                Armor.ReadThis(equips[2]);
                RightHand.ReadThis(equips[3]);
                LeftHand.ReadThis(equips[4]);
                Ring.ReadThis(equips[5]);
                Gourd.ReadThis(equips[6]);
                Boots.ReadThis(equips[7]);
                Garment.ReadThis(equips[8]);
                Fan.ReadThis(equips[9]);
                Tower.ReadThis(equips[10]);
                Steed.ReadThis(equips[11]);
            }
            catch { }
        }
        public static implicit operator Item[](Equipment e)
        {
            Item[] eqs = new Item[11];
            for (byte x = 0; x < 12; x++)
            {
                try
                {
                    eqs[x] = e.Get(x);
                }
                catch { }
            }
            return eqs;
        }
    }
    
    public struct Banks
    {
        //public ArrayList Inventory = new ArrayList(40);
        public ArrayList TCWarehouse;
        public ArrayList PCWarehouse;
        public ArrayList ACWarehouse;
        public ArrayList DCWarehouse;
        public ArrayList BIWarehouse;
        public ArrayList SCWarehouse;
        public ArrayList MAWarehouse;

       public string WriteThis()
        {
            string whs = "";

            foreach (Item I in TCWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";

            foreach (Item I in PCWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";


            foreach (Item I in ACWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";


            foreach (Item I in DCWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";


            foreach (Item I in BIWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";


            foreach (Item I in MAWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";



            foreach (Item I in SCWarehouse)
                whs += I.WriteThis() + "~";
            whs += "#";

            return whs;
        }
       public void ReadThis(string line)
       {
           string[] whs = line.Split('#');

           TCWarehouse = new ArrayList(20);
           if (whs.Length >= 1)
           {
               string[] tc = whs[0].Split('~');
               for (byte i = 0; i < tc.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(tc[i]);
                   if (I.ID != 0)
                       TCWarehouse.Add(I);
               }
           }

           PCWarehouse = new ArrayList(20);
           if (whs.Length >= 2)
           {
               string[] pc = whs[1].Split('~');
               for (byte i = 0; i < pc.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(pc[i]);
                   if (I.ID != 0)
                       PCWarehouse.Add(I);
               }
           }
           ACWarehouse = new ArrayList(20);
           if (whs.Length >= 3)
           {
               string[] ac = whs[2].Split('~');
               for (byte i = 0; i < ac.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(ac[i]);
                   if (I.ID != 0)
                       ACWarehouse.Add(I);
               }
           }

           DCWarehouse = new ArrayList(20);
           if (whs.Length >= 4)
           {
               string[] dc = whs[3].Split('~');
               for (byte i = 0; i < dc.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(dc[i]);
                   if (I.ID != 0)
                       DCWarehouse.Add(I);
               }
           }
           BIWarehouse = new ArrayList(20);
           if (whs.Length >= 5)
           {
               string[] bi = whs[4].Split('~');
               for (byte i = 0; i < bi.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(bi[i]);
                   if (I.ID != 0)
                       BIWarehouse.Add(I);
               }
           }
           MAWarehouse = new ArrayList(40);
           if (whs.Length >= 6)
           {
               string[] ma = whs[5].Split('~');
               for (byte i = 0; i < ma.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(ma[i]);
                   if (I.ID != 0)
                       MAWarehouse.Add(I);
               }
           }

           SCWarehouse = new ArrayList(20);
           if (whs.Length >= 7)
           {
               string[] sc = whs[6].Split('~');
               for (byte i = 0; i < sc.Length; i++)
               {
                   Item I = new Item();
                   I.ReadThis(sc[i]);
                   if (I.ID != 0)
                       SCWarehouse.Add(I);
               }
           }
       }
    }
    public class Item
    {
        private static uint ItemUIDStart = 1;
        private static uint ItemUIDFinish = uint.MaxValue - 1;
        private static uint ItemNextID
        {
            get
            {
                if (ItemUIDStart == ItemUIDFinish)
                    ItemUIDFinish = 1;
                return ItemUIDStart++;
            }
        }
        public static void GetGemEffect(ref EquipStats E, Gem G)
        {
            switch (G)
            {
                case Gem.NormalGloryGem:
                    {
                        E.MeleeDamageDecrease += 100;
                        E.MagicDamageDecrease += 100;
                        break;
                    }
                case Gem.RefinedGloryGem:
                    {
                        E.MeleeDamageDecrease += 300;
                        E.MagicDamageDecrease += 300;
                        break;
                    }
                case Gem.SuperGloryGem:
                    {
                        E.MeleeDamageDecrease += 500;
                        E.MagicDamageDecrease += 500;
                        break;
                    }
                case Gem.NormalThunderGem:
                    {
                        E.MeleeDamageIncrease += 100;
                        E.MagicDamageIncrease += 100;
                        break;
                    }
                case Gem.RefinedThunderGem:
                    {
                        E.MeleeDamageIncrease += 300;
                        E.MagicDamageIncrease += 300;
                        break;
                    }
                case Gem.SuperThunderGem:
                    {
                        E.MeleeDamageIncrease += 500;
                        E.MagicDamageIncrease += 500;
                        break;
                    }
                case Gem.NormalDragonGem:
                    {
                        E.GemExtraAttack += 0.05;
                        break;
                    }
                case Gem.RefinedDragonGem:
                    {
                        E.GemExtraAttack += 0.1;
                        break;
                    }
                case Gem.SuperDragonGem:
                    {
                        E.GemExtraAttack += 0.15;
                        break;
                    }
                case Gem.SuperTortoiseGem:
                    {
                        E.MDef2 += 600;
                        break;
                    }
                case Gem.RefinedTortoiseGem:
                    {
                        E.MDef2 += 400;
                        break;
                    }
                case Gem.NormalTortoiseGem:
                    {
                        E.MDef2 += 200;
                        break;
                    }
                case Gem.NormalRainbowGem:
                    {
                        E.GemExtraExp += 0.1;
                        break;
                    }
                case Gem.RefinedRainbowGem:
                    {
                        E.GemExtraExp += 0.15;
                        break;
                    }
                case Gem.SuperRainbowGem:
                    {
                        E.GemExtraExp += 0.25;
                        break;
                    }
                case Gem.NormalPhoenixGem:
                    {
                        E.GemExtraMAttack += 0.05;
                        break;
                    }
                case Gem.RefinedPhoenixGem:
                    {
                        E.GemExtraMAttack += 0.1;
                        break;
                    }
                case Gem.SuperPhoenixGem:
                    {
                        E.GemExtraMAttack += 0.15;
                        break;
                    }
            }
        }
        public enum ArmorColor
        {
            Black = 2,
            Orange,
            LightBlue,
            Red,
            Blue,
            Yellow,
            Purple,
            White
        }
        public enum GarmentColor
        {
            Black = 2,
            Orange,
            LightBlue,
            Red,
            Blue,
            Yellow,
            Purple,
            White
        }
        public enum ItemQuality
        {
            Fixed = 0,
            NoUpgrade = 1,
            Simple = 3,
            Poor,
            Normal,
            Refined,
            Unique,
            Elite,
            Super
        }
        public enum Gem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,

            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,

            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,

            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,

            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,

            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,

            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,

            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,

            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,

            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,

            NoSocket = 0,
            EmptySocket = 255
        }
        public enum RebornEffect
        {
            None = 0,
            Poison = 0xC8,
            HP = 0xC9,
            MP = 0xCA,
            Shield = 0xCB,
            Horsie = 0x64
        }
        public uint ID;
        //uint uID;
        public uint UID;
        //  {
        //    get { return uID; }
        //  set { uID = ItemNextID; }
        // }
        public uint Position;
        public byte Plus;
        public byte Bless;
        public byte Enchant;
        public Gem Soc1;
        public Gem Soc2;
        public ushort MaxDur;
        public ushort CurDur;
        public int Suspicious;
        public bool FreeItem;
        public byte Locked;
        public uint Time;
        public string NameReward;
        public string NameClain;
        public uint LockedDays;
        public uint TalismanProgress;
        public byte[] RBG = new byte[4];
        public ushort Progress;
        public RebornEffect Effect;
        public ArmorColor Color;
        public byte Pot
        {
            get
            {
                byte pot = 0;
                byte Quality = ItemIDManipulation.Digit(ID, 6);
                if (Quality >= 5)
                    pot += (byte)(ItemIDManipulation.Digit(ID, 6) - 5);
                pot += Plus;
                if (Soc1 != Gem.NoSocket) pot++;
                if (Soc2 != Gem.NoSocket) pot++;
                byte soc1 = (byte)Soc1;
                byte soc2 = (byte)Soc2;
                if (soc1 % 10 == 3) pot++;
                if (soc2 % 10 == 3) pot++;
                if (ItemIDManipulation.Digit(ID, 1) == 5) pot *= 2;

                return pot;
            }
        }
        public static bool IsArrow(uint ID)
        {
            if (ID == 1050000 ||
                ID == 1050001 ||
                ID == 1050002 ||
                ID == 1050020 ||
                ID == 1050021 ||
                ID == 1050022 ||
                ID == 1050023 ||
                ID == 1050030 ||
                ID == 1050031 ||
                ID == 1050032 ||
                ID == 1050033 ||
                ID == 1050040 ||
                ID == 1050041 ||
                ID == 1050042 ||
                ID == 1050043 ||
                ID == 1050050 ||
                ID == 1050051 ||
                ID == 1050052 ||
                ID == 1051000 ||
                ID == 1051000)
                return true;
            else
                return false;
        }
        public static bool EquipPassLvlReq(Item Item, Character C)
        {
            if (C.Level < Item.DBInfo.LevReq)
                return false;
            else
                return true;
        }
        public static bool EquipPassRbReq(Item Item, Character C)
        {
            if (Item.DBInfo.LevReq < 71 && C.Reborns > 0 && C.Level >= 70)
                return true;
            else
                return false;
        }
        public static bool EquipPassStatsReq(Item Item, Character C)
        {
            if (C.Str >= Item.DBInfo.StrNeed && C.Agi >= Item.DBInfo.AgiNeed)
                return true;
            else
                return false;
        }
        public static bool EquipPassJobReq(Item Item, Character C)
        {
            switch (Item.DBInfo.Class)
            {
                #region Trojan
                case 10: if (C.Job <= 15 && C.Job >= 10) return true; break;
                case 11: if (C.Job <= 15 && C.Job >= 11) return true; break;
                case 12: if (C.Job <= 15 && C.Job >= 12) return true; break;
                case 13: if (C.Job <= 15 && C.Job >= 13) return true; break;
                case 14: if (C.Job <= 15 && C.Job >= 14) return true; break;
                case 15: if (C.Job == 15) return true; break;
                #endregion
                #region Warrior
                case 20: if (C.Job <= 25 && C.Job >= 20) return true; break;
                case 21: if (C.Job <= 25 && C.Job >= 21) return true; break;
                case 22: if (C.Job <= 25 && C.Job >= 22) return true; break;
                case 23: if (C.Job <= 25 && C.Job >= 23) return true; break;
                case 24: if (C.Job <= 25 && C.Job >= 24) return true; break;
                case 25: if (C.Job == 25) return true; break;
                #endregion
                #region Archer
                case 40: if (C.Job <= 45 && C.Job >= 40) return true; break;
                case 41: if (C.Job <= 45 && C.Job >= 41) return true; break;
                case 42: if (C.Job <= 45 && C.Job >= 42) return true; break;
                case 43: if (C.Job <= 45 && C.Job >= 43) return true; break;
                case 44: if (C.Job <= 45 && C.Job >= 44) return true; break;
                case 45: if (C.Job == 45) return true; break;
                #endregion
                #region Ninja
                case 50: if (C.Job <= 55 && C.Job >= 50) return true; break;
                case 51: if (C.Job <= 55 && C.Job >= 51) return true; break;
                case 52: if (C.Job <= 55 && C.Job >= 52) return true; break;
                case 53: if (C.Job <= 55 && C.Job >= 53) return true; break;
                case 54: if (C.Job <= 55 && C.Job >= 54) return true; break;
                case 55: if (C.Job == 55) return true; break;
                #endregion
                #region Taoist
                case 190: if (C.Job >= 100) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static bool EquipPassSexReq(Item Item, Character C)
        {
            int ClientSex = C.Body % 3000 < 1005 ? 1 : 2;
            if (Item.DBInfo.GenderReq == 2 && ClientSex == 2)
                return true;
            if (Item.DBInfo.GenderReq != 2)
                return true;
            return false;
        }
        public bool CanEquip(Character C)
        {
            bool pass = false;
            if (EquipPassRbReq(this, C))
                pass = true;
            else
                if (EquipPassJobReq(this, C)) if (EquipPassStatsReq(this, C)) if (EquipPassLvlReq(this, C)) if (EquipPassSexReq(this, C)) pass = true;
            if (!pass)
                return false;



            DatabaseItem DI = DBInfo;
            ItemIDManipulation E = new ItemIDManipulation(ID);
            E.ChangeDigit(4, 0);
            uint ID2 = E.ToID();
            if (!EquipPassRbReq(this, C))
            {
                if (DI.ProfReq != 0)
                {
                    if (E.Digit(1) == 4 || E.Digit(1) == 5 || E.Digit(1) == 6)
                    {
                        if (C.Profs.Contains((ushort)E.Part(0, 3)))
                        {
                            Prof P = (Prof)C.Profs[(ushort)E.Part(0, 3)];
                            if (P.Lvl < DI.ProfReq)
                                return false;
                        }
                        else
                            return false;
                    }
                }
            }
            if (ID2 == 137010)
                return false;

            return true;
        }
        public DatabaseItem DBInfo
        {
            get
            {
                if (Database.DatabaseItems.Contains(ID))
                    return (DatabaseItem)Database.DatabaseItems[ID];
                return new DatabaseItem();
            }
        }
       public string WriteThis()
        {
            string item = "";
            if (ID != 0 && UID == 0) UID = (uint)Program.Rnd.Next(10000000);
            if (ID != 0 && UID == 0) UID = (uint)World.Rnd.Next(10000000);
            item = UID + "-" + ID + "-" + Plus + "-" + Bless + "-" + Enchant + "-"
                + (byte)Soc1 + "-" + (byte)Soc2 + "-" + MaxDur + "-" + CurDur + "-"
                + (FreeItem == true ? "1" : "0") + "-" + (uint)TalismanProgress + "-" + (ushort)Progress + "-" + (byte)Color + "-" + Locked + "-" + (ulong)LockedDays + "-" + RBG[0] + "-" + RBG[1] + "-" + RBG[2] + "-" + RBG[3] + "-" + (byte)Effect;
            return item;
        }
        public void ReadThis(string I)
        {
            string[] split = I.Split('-');
            if (split.Length < 19)
            {
                UID = 0;
                ID = 0;
                return;
            }
            UID = Convert.ToUInt32(split[0]);

            if (UID == 0) UID = (uint)Program.Rnd.Next(10000000);
            if (UID == 0) UID = (uint)World.Rnd.Next(10000000);

            ID = Convert.ToUInt32(split[1]);
            if (Database.DatabaseItems.Contains(ID))
            {
                Character C = new Character();
                Plus = Convert.ToByte(split[2]);
                Bless = Convert.ToByte(split[3]);
                Enchant = Convert.ToByte(split[4]);
                Soc1 = (Gem)Convert.ToByte(split[5]);
                Soc2 = (Gem)Convert.ToByte(split[6]);
                MaxDur = Convert.ToUInt16(split[7]);
                CurDur = Convert.ToUInt16(split[8]);
                FreeItem = Convert.ToBoolean(Convert.ToByte(split[9]));
                TalismanProgress = Convert.ToUInt32(split[10]);
                Progress = Convert.ToUInt16(split[11]);
                Color = (ArmorColor)Convert.ToByte(split[12]);
                Locked = Convert.ToByte(split[13]);
                LockedDays = Convert.ToUInt32(split[14]);
                RBG[0]= Convert.ToByte(split[15]);
                RBG[1] = Convert.ToByte(split[16]);
                RBG[2] = Convert.ToByte(split[17]);
                RBG[3] = Convert.ToByte(split[18]);
                Effect = (RebornEffect)Convert.ToByte(split[19]);

             /*   if (Locked == 2)
                {

                    int myDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                    C.MyClient.SendPacket(Packets.ItemLock(UID, 1, 3, (uint)LockedDays));
                    if (myDate >= (int)LockedDays)
                    {
                        LockedDays = 0;
                        Locked = 0;
                        Database.SaveItems(this, C);

                        C.MyClient.LocalMessage(2000,System.Drawing.Color.Red ,"Congratulations! successful Unlocked " + DBInfo.Name + "");
                    }
                }*/
            }
            else
            {
                UID = 0;
                ID = 0;
            }
        }
    }
    public enum Ranks : byte
    {
        Serf = 0,
        Knight = 1,
        Baron = 3,
        Earl = 5,
        Duke = 7,
        Prince = 9,
        King = 12
    }

    public struct Prof
    {
        public ushort ID;
        public byte Lvl;
        public uint Exp;

        public string WriteThis()
        {
            return ID + "-" + Lvl + "-" + Exp;
        }
        public void ReadThis(System.IO.BinaryReader I)
        {
            ID = I.ReadUInt16();
            Lvl = I.ReadByte();
            Exp = I.ReadUInt32();
        }
    }
    public class Skill
    {
        public ushort ID;
        public byte Lvl;
        public uint Exp;

        public Features.SkillsClass.SkillInfo Info
        {
            get
            {
                if (Features.SkillsClass.SkillInfos.Contains(ID + " " + Lvl))
                    return (Features.SkillsClass.SkillInfo)Features.SkillsClass.SkillInfos[ID + " " + Lvl];
                return new SkillsClass.SkillInfo();
            }
        }
    }

    public struct TradePartner
    {

        public uint UID;
        public string WriteThis()
        {
            return UID + "*" + Name + "*" + ProbationStartedOn.Ticks;
        }

        public void ReadThis(string line)
        {
            string[] split = line.Split('*');
            if (split.Length == 3)
            {
                UID = uint.Parse(split[0]);
                Name = split[1];
                ProbationStartedOn = DateTime.FromBinary(long.Parse(split[2]));
            }
        }
        public bool Online
        {
            get
            {
                return World.H_Chars.ContainsKey(UID);
            }
        }
        public Character Info
        {
            get
            {
                if (Online)
                    return (Character)World.H_Chars[UID];
                return null;
            }
        }
        public string Name
        {
            get;
            set;
        }
        public bool StillOnProbation
        {
            get
            {
                return ProbationStartedOn.AddDays(3) >= DateTime.Now;
            }
        }
        public DateTime ProbationStartedOn
        {
            get;
            set;
        }
    }


    public struct Friend
    {
        public uint UID;
        string name;

        /* public string WriteThis()
         {
             return UID + "*" + Name;
         }
         public void ReadThis(System.IO.BinaryReader I)
         {
             UID = I.ReadUInt32();
             name = I.ReadString();
         }*/

        public bool Online
        {
            get
            {
                return World.H_Chars.Contains(UID);
            }
        }
        public Character Info
        {
            get
            {
                if (Online)
                    return (Character)World.H_Chars[UID];
                return null;
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    public struct Enemy
    {
        public uint UID;
        string name;
        public bool Online
        {
            get
            {
                return World.H_Chars.Contains(UID);
            }
        }
        public Character Info
        {
            get
            {
                if (Online)
                    return (Character)World.H_Chars[UID];
                return null;
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
    public struct EquipStats
    {
        //fan atack - tower def  = dmg tallismans
        public uint FanMagicAtack;
        public uint TowerMagicDef;
        public uint FanAtack;
        public uint TowerDef;
        public uint minatk;
        public uint maxatk;
        public uint matk;
        public ushort defense;
        public double GemExtraExp;
        public double GemExtraAttack;
        public double GemExtraMAttack;
        public uint MaxHP;
        public uint MaxMP;
        public byte Dodge;
        ushort _MDef1;
        public ushort MDef2;
        public ushort ExtraDex;
        public ushort eq_pot;
        public uint MagicDamageDecrease;
        public uint MeleeDamageDecrease;
        public uint MagicDamageIncrease;
        public uint MeleeDamageIncrease;
        public byte TotalBless;
        public byte AddRideSpeed;
        public ushort AddVigor;

        public ushort MDef1
        {
            set
            {
                _MDef1 = value;
            }
            get
            {
                if (_MDef1 > 100)
                    return 100;
                else
                    return _MDef1;
            }
        }

        public static EquipStats operator +(EquipStats Eqp, EquipStats eqp)
        {
            Eqp.FanMagicAtack += eqp.FanMagicAtack;
            Eqp.FanAtack += eqp.FanAtack;
            Eqp.TowerMagicDef += eqp.TowerMagicDef;
            Eqp.TowerDef += eqp.TowerDef;
            Eqp.minatk += eqp.minatk;
            Eqp.maxatk += eqp.maxatk;
            Eqp.matk += eqp.matk;
            Eqp.defense += eqp.defense;
            Eqp.GemExtraExp += eqp.GemExtraExp;
            Eqp.GemExtraAttack += eqp.GemExtraAttack;
            Eqp.GemExtraMAttack += eqp.GemExtraMAttack;
            Eqp.MaxHP += eqp.MaxHP;
            Eqp.MaxMP += eqp.MaxMP;
            Eqp.Dodge += eqp.Dodge;
            Eqp.MDef1 += eqp.MDef1;
            Eqp.MDef2 += eqp.MDef2;
            Eqp.ExtraDex += eqp.ExtraDex;
            Eqp.eq_pot += eqp.eq_pot;
            Eqp.MagicDamageDecrease += eqp.MagicDamageDecrease;
            Eqp.MeleeDamageDecrease += eqp.MeleeDamageDecrease;
            Eqp.MagicDamageIncrease += eqp.MagicDamageIncrease;
            Eqp.MeleeDamageIncrease += eqp.MeleeDamageIncrease;
            Eqp.TotalBless += eqp.TotalBless;
            Eqp.AddRideSpeed += eqp.AddRideSpeed;
            Eqp.AddVigor += eqp.AddVigor;
            return Eqp;
        }
        public static EquipStats operator -(EquipStats Eqp, EquipStats eqp)
        {
            Eqp.FanMagicAtack -= eqp.FanMagicAtack;
            Eqp.FanAtack -= eqp.FanAtack;
            Eqp.TowerMagicDef -= eqp.TowerMagicDef;
            Eqp.TowerDef -= eqp.TowerDef;
            Eqp.minatk -= eqp.minatk;
            Eqp.maxatk -= eqp.maxatk;
            Eqp.matk -= eqp.matk;
            Eqp.defense -= eqp.defense;
            Eqp.GemExtraExp -= eqp.GemExtraExp;
            Eqp.GemExtraAttack -= eqp.GemExtraAttack;
            Eqp.GemExtraMAttack -= eqp.GemExtraMAttack;
            Eqp.MaxHP -= eqp.MaxHP;
            Eqp.MaxMP -= eqp.MaxMP;
            Eqp.Dodge -= eqp.Dodge;
            Eqp.MDef1 -= eqp.MDef1;
            Eqp.MDef2 -= eqp.MDef2;
            Eqp.ExtraDex -= eqp.ExtraDex;
            Eqp.eq_pot -= eqp.eq_pot;
            Eqp.MagicDamageDecrease -= eqp.MagicDamageDecrease;
            Eqp.MeleeDamageDecrease -= eqp.MeleeDamageDecrease;
            Eqp.MagicDamageIncrease -= eqp.MagicDamageIncrease;
            Eqp.MeleeDamageIncrease -= eqp.MeleeDamageIncrease;
            Eqp.TotalBless -= eqp.TotalBless;
            Eqp.AddRideSpeed -= eqp.AddRideSpeed;
            Eqp.AddVigor -= eqp.AddVigor;
            return Eqp;
        }
    }
    public class Nobility
    {
        private Character C;
        public ulong Donation;
        public int ListPlace;
        private Ranks NobilityID;
        public Nobility(Character c)
        {
            C = c;
        }
        public Ranks Rank
        {
            get { return NobilityID; }
            set
            {
                NobilityID = value;
                if (C.Loaded)
                    C.SendScreen(Packets.Packet(PacketType.NobilityPacket, C));
                    //C.SendScreen(Packets.Donators(C));
            }
        }
    }
    public enum MerchantTypes : byte
    {
        Asking = 1,
        Not = 0,
        Yes = 255
    }
    public struct AttackMemorise
    {
        public bool Attacking;
        public DateTime LastAttack;
        public uint Target;
        public byte AtkType;
        public ushort Skill;
        public ushort SX;
        public ushort SY;
    }
    public struct Buff
    {
        public StatusEffectEn StEff;
        public SkillsClass.ExtraEffect Eff;
        public float Value;
        public ushort Lasts;
        public uint Transform;
        public DateTime Started;
    }

    public class Character
    {
        System.Timers.Timer Timer = new System.Timers.Timer();
        public enum JobName : byte
        {
            InternTrojan = 10,
            Trojan = 11,
            VeteranTrojan = 12,
            TigerTrojan = 13,
            DragonTrojan = 14,
            TrojanMaster = 15,
            InternWarrior = 20,
            Warrior = 21,
            BrassWarrior = 22,
            SilverWarrior = 23,
            GoldWarrior = 24,
            WarriorMaster = 25,
            InternArcher = 40,
            Archer = 41,
            EagleArcher = 42,
            TigerArcher = 43,
            DragonArcher = 44,
            ArcherMaster = 45,
            InternNinja = 50,
            Ninja = 51,
            MiddleNinja = 52,
            DarkNinja = 53,
            MysticNinja = 54,
            NinjaMaster = 55,
            Taoist = 101,
            ArcanumTaoist = 122,
            ArcanumWizard = 123,
            ArcanumSaint = 124,
            ArcanumMaster = 125,
            WaterTaoist = 132,
            WaterWizard = 133,
            WaterMaster = 134,
            WaterSaint = 135,
            FireTaoist = 142,
            FireWizard = 143,
            FireMaster = 144,
            FireSaint = 145
        }
        public bool Ghost = false;
        public uint InteractionType = 0;
        public uint InteractionWith = 0;
        public bool InteractionInProgress = false;
        public ushort InteractionX = 0;
        public ushort InteractionY = 0;
        public bool InteractionSet = false;
        public string AccountName;
        public bool Mining = false;
        public DateTime ElightenRequestTime = DateTime.Now;
        public DateTime LastMine = DateTime.Now;
        public DateTime LastBuffRemove = DateTime.Now;
        public DateTime Nobilit = DateTime.Now;
        public DateTime ExpPotionUsed = DateTime.Now;
        public DateTime LoggedOn = DateTime.Now;
        public DateTime LastLogin = DateTime.Now;
        public double TrainTimeLeft;
        public bool InOTG = false;
        public DateTime LastProtection = DateTime.Now;
        public bool Lottery = false;

        public Companion MyCompanion;
        public Companion MyCompanion1;
        public Companion MyCompanion2;
        public Companion MyCompanion3;
        public Companion MyCompanion4;
        public Companion MyCompanion5;
        public Companion MyCompanion6;
        public Companion MyCompanion7;
        public Companion MyCompanion8;
        public Companion MyCompanion9;
        public Companion MyCompanion10;
        public Companion MyCompanion11;

        public byte SuperGem = 0;
        public bool InPKT = false;
        public byte addBless = 0;
        public byte LotteryUsed = 0;
        public static string Question;
        public static string Answer;
        public static string InputAwnserNpcQuiz = "";
        private byte _Reborns;
        public bool Protection = false;
        public bool DoubleExp;
        public int DoubleExpLeft;
        public int BlessingLasts;
        public DateTime BlessingStarted;
        public int PrayTimeLeft;
        public int secondeimunity = 0;
        public DateTime InvencibleTime = DateTime.Now;
        public Nobility Nobility;
      

        public string DuelEnemy = ""; // 1v1
        public bool InDuell = false; // 1v1
        public byte Bet = 0; // 1v1
        public int dmjoin = 0;
        public int dmred = 0;
        public int dmblack = 0;
        public int dmblue = 0;
        public int dmwhite = 0;
        public int srjoin = 0;
        public byte OnlineTrainingPts = 0;
        public DateTime LastPts = DateTime.Now;

        public ushort EnhligtehnRequest = 0;
        public DateTime LastSaves = DateTime.Now;
        public uint RequestFriends = 0;
        //Trade <<
        public uint TradingWith = 0;
        public ArrayList TradeSide = new ArrayList(20);
        public uint TradingSilvers = 0;
        public uint TradingCPs = 0;
        public bool Trading = false;
        public bool ClickedOK = false;
        // >>

        public DateTime SteedRaceTime;
        public int HideQuest = 0;
        public int FreeGear = 0;
        public int House = 0;
        public int Flori = 0;
        public int quest1 = 0;
        public uint EntityID;
        public byte rb = 0;
        public int MonsterHunter = 0;
        public bool MHunter = false;
        public string Hunter = "";
        public int HonorPoints = 0;
        public byte ExpBallsUsedToday = 0;
        public Random Rnd = new Random();
        public int XPKO = 0;
        public int TotalKO = 0;
        public bool Alive = true;
        public byte Direction = 0;
        public byte Action = 100;
        public byte Killmap = 0;
        public int cp = 0; // steed cps wait
        public byte DbUsedToday = 0;
        public int QuestTc = 0;
        public int queststatictc = 0;
        public int questtcnr = 0;
        public string PlayerLanguage = "en-US";
       
        
        public int _TopTrojan = 0;
        public int TopTrojan
        {
            get { return _TopTrojan; }
            set
            {
                _TopTrojan = value;
                if (value >= 1)
                {
                    Database.SaveTop(this);
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopTrojan);
                }
            }
        }
        public int _TopWarrior = 0;
        public int TopWarrior
        {
            get { return _TopWarrior; }
            set
            {
                _TopWarrior = value;
                if (value >= 1)
                {
                    Database.SaveTop(this);
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopWarrior);
                }
            }
        }


        public int _TopNinja = 0;

        public int TopNinja
        {
            get { return _TopNinja; }
            set
            {
                _TopNinja = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopNinja);
                    Database.SaveTop(this);
                }
            }
        }

        public Features.Teams MyTDmTeam = new Features.Teams();
        public int _TopWaterTaoist = 0;
        public int TopWaterTaoist
        {
            get { return _TopWaterTaoist; }
            set
            {
                _TopWaterTaoist = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopWaterTaoist);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopArcher = 0;
        public int TopArcher
        {
            get { return _TopArcher; }
            set
            {
                _TopArcher = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopArcher);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopGuildLeader = 0;
        public int TopGuildLeader
        {
            get { return _TopGuildLeader; }
            set
            {
                _TopWarrior = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopGuildLeader);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopFireTaoist = 0;
        public int TopFireTaoist
        {
            get { return _TopFireTaoist; }
            set
            {
                _TopFireTaoist = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopFireTaoist);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopDeputyLeader = 0;
        public int TopDeputyLeader
        {
            get { return _TopDeputyLeader; }
            set
            {
                _TopDeputyLeader = value;
                if (value >= 1)
                {
                    StatEff.Add(NewestCOServer.Game.StatusEffectEn.TopDeputyLeader);
                    Database.SaveTop(this);
                }
            }
        }
        public int _WeeklyPKChampion = 0;
        public int WeeklyPKChampion
        {
            get { return _WeeklyPKChampion; }
            set
            {
                _WeeklyPKChampion = value;
                  if (value >= 1)
                      StatEff.Add(NewestCOServer.Game.StatusEffectEn.WeeklyPKChampion);
            }
        }


        //banned users
        public int banned = 0;
        public string BanBy = "";
        public int DisKO = 0;
        public bool DisQuest = false;
        public int flames = 0;

        public Dictionary<uint, string> TradePartners = new Dictionary<uint, string>();
        public long TimePartner = 0;
        public uint RequestPartnerWith = 0;

        public bool invincibles = false;
        public DateTime SecondTimer = DateTime.Now;
        public int rebornquest = 0;
        public Location Loc;
        public StatusEffect StatEff;
        public int Top = 0;
        public PKMode PKMode = PKMode.Capture;
        bool _BlueName = false;
        public Features.Team MyTeam;
        public bool TeamLeader = false;
        public AttackMemorise AtkMem;
        byte _Stamina = 0;
        ushort _Vigor = 0;
        public uint Donation;
        public QuizShow.Info QuizShowInfo;
        public Guild MyGuild;
        public EquipStats EqStats;
        public byte oldlevel = 0;
        public Main.GameClient MyClient;
        public string Name;
        private string spouse = "None";
        public string Spouse
        {
            get { return spouse; }
            set
            {
                spouse = value;
            }
        }
        string wHPassword = "0";
        public string WHPassword
        {
            get { return wHPassword; }
            set
            {
                wHPassword = value;
            }

        }
        public byte PEBUsedToday = 0;
        public bool CurseStart;
        public int CurseExpLeft;
        public DateTime CurseUser = DateTime.Now;
        public string TempPass = "";
        public DateTime AtackTime = DateTime.Now;
        public int WHErrors = 0;
        public uint e_ko = 0;
        public Exterminator e_quest;
        public bool WHOpen = false;


        public bool Loaded = false;
        public string Enemigo = ""; // 1v1
        public DateTime BlueNameStarted;
        public bool verificprolalanoob = false;//stf
        public byte BlueNameLasts;
        public bool CanReflect = false;
        public byte PkPuntos = 0; // 1v1
        public bool Luchando = false; // 1v1
        public int Apuesta = 0; // 1v1
        public uint datetounlock = 0;
        public byte ga = 0;
        //Counter-kill
        public bool CounterKillOn = false;
        public DateTime LastCounterKill;
        public int CounterKillRate = 0;
        public int CounterKillTime = 0;


        public DateTime LastPKPLost = DateTime.Now;
        public DateTime LastXP = DateTime.Now;
        public DateTime LastStamina = DateTime.Now;
        public DateTime DeathHit = DateTime.Now;
        private uint _UniversityPoints = 0;
        byte _PreviousJob1;
        public byte PreviousJob1
        {
            get { return _PreviousJob1; }
            set
            {
                _PreviousJob1 = value;
                if (value < byte.MaxValue)
                    Database.SavePreviousjob1(this, value);
            }
        }
        byte _PreviousJob2;
        public byte PreviousJob2
        {
            get { return _PreviousJob2; }
            set
            {
                _PreviousJob2 = value; if (value < byte.MaxValue)
                    Database.SavePreviousjob2(this, value);
            }
        }
        public bool Reborn
        {
            get { return Reborns > 0; }
        }
        public ulong VP;

        byte viplevel = 0;
        public byte VipLevel
        {
            get { return viplevel; }
            set
            {
                viplevel = value;
                if (Loaded)
                {
                    try
                    {
                        if (value < byte.MaxValue)
                            Database.savevip(this, value);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.VIPLevel, value));
                    }
                    catch { }
                }
            }
        }
        uint _CPSDOnate;
        ushort _Avatar;
        ushort _Body;
        ushort _Hair;
        public MerchantTypes Merchant;
        byte _Job;
        byte _Level;
        ulong _Experience;
        ushort _Str;
        ushort _Agi;
        ushort _Vit;
        ushort _Spi;
        ushort _StatPoints;
        ushort _CurHP;
        ushort _CurMP;
        uint _Silvers;
        uint _WHSilvers;
        ushort _PKPoints;
        uint _CPs;
        public uint LuckyTime = 0;
        public bool GettingLuckyTime = false;
        public bool Prayer = false;
        public Character ThePrayer;


        public DateTime lastJumpTime = DateTime.Now;
        public short lastJumpDistance = 0;

        public bool FlowerExist = false;
        public string FlowerType = "";
        public string FlowerName = "";
        public Struct.Flowers Flowers = new Struct.Flowers();
        public DateTime PrayDT;
        public DateTime UnableToUseDrugs;
        public ushort UnableToUseDrugsFor;
        public byte ElightenAdd = 0;
        public ulong ElighemPoints = 0;
        public PoisonType PoisonedInfo = null;
        public uint TradePartnerWith = 0;
        public bool VortexOn = false;
        public DateTime LastVortexAttk = DateTime.Now;

        public bool LoadedEquipmentHPAdd = false;

        public uint GuildDonation;
        public GuildRank GuildRank;
        public MemberInfo MembInfo;

        public Dictionary<uint, Game.Item> ConfiscatorClain = new Dictionary<uint, Game.Item>();
        public Dictionary<uint, Game.Item> ConfiscatorReward = new Dictionary<uint, Game.Item>();
        public Dictionary<uint, Game.Item> Inventory = new Dictionary<uint, Game.Item>();
        public Equipment Equips;
        //   public ArrayList Inventory;
        public Banks Warehouses;
        public Hashtable Skills;
        public Hashtable Profs;
        public Hashtable Friends;
        //public Hashtable TradePartner;
   //     public Hashtable Partners;

        public Dictionary<uint, TradePartner> Partners = new Dictionary<uint, TradePartner>();
        public Hashtable Enemies;

        public ArrayList Buffs;

        public PersonalShops.Shop MyShop;

        public bool Superman
        {
            get
            {
                if (StatEff.Contains(StatusEffectEn.SuperMan))
                    return true;
                return false;
            }
        }

        public bool Cyclone
        {
            get
            {
                if (StatEff.Contains(StatusEffectEn.Cyclone))
                    return true;
                return false;
            }
        }

        public byte Reborns
        {
            get { return _Reborns; }
            set
            {
                _Reborns = value;
                if (Loaded)
                {
                    if (value < byte.MaxValue)
                        Database.SaveReborn(this, value);
                    SendScreen(Packets.Status(EntityID, Status.RebirthCount, Reborns));
                }
            }
        }
        public void SendScreen(byte[] Data)
        {
            Game.Character[] Chars = new Game.Character[Game.World.H_Chars.Count];
            Game.World.H_Chars.Values.CopyTo(Chars, 0);
            foreach (Character C in Chars)
                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                    C.MyClient.SendPacket2(Data);
            Chars = null;
        }
        public ushort Potency
        {
            get
            {
                string s = ((ushort)Equips.HeadGear.Soc1).ToString();
                int prePotency = 0;
                prePotency += Level + 5 * Reborns;
                for (byte x = 1; x < 12; x++)
                {
                    Item I = Equips.Get(x);
                    if (I.UID != 0)
                        prePotency += I.Pot;
                }
                Item Ia = Equips.Get(12);
                prePotency += Ia.Plus;
                if ((Equips.RightHand.ID >= 421003 && Equips.RightHand.ID <= 421339) && ((Job <= 145 && Job >= 140) || Job == 135) && Equips.LeftHand.UID == 0)
                    prePotency += Equips.RightHand.Pot;
                prePotency += (byte)Nobility.Rank;
                return (ushort)prePotency;
            }
        }
        public byte LevReqForPromote
        {
            get
            {
                sbyte n = -1;
                if (Job >= 10 && Job <= 15)
                    n = (sbyte)(Job - 10);
                else if (Job >= 20 && Job <= 25)
                    n = (sbyte)(Job - 20);
                else if (Job >= 40 && Job <= 45)
                    n = (sbyte)(Job - 40);
                else if (Job >= 50 && Job <= 55)
                    n = (sbyte)(Job - 50);
                else if (Job >= 100)
                {
                    if (Job <= 101)
                        n = (sbyte)(Job - 100);
                    else if (Job >= 132 && Job <= 135)
                        n = (sbyte)(Job - 130);
                    else if (Job >= 142 && Job <= 145)
                        n = (sbyte)(Job - 140);
                }
                if (n == 0)
                    return 15;
                else if (n == 1)
                    return 40;
                else if (n == 2)
                    return 70;
                else if (n == 3)
                    return 100;
                else if (n == 4)
                    return 110;
                else
                    return 0;
            }
        }
        public uint UniversityPoints
        {
            get { return _UniversityPoints; }
            set
            {
                _UniversityPoints = value;
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        Database.SaveUniversity(this, value);
                    SendScreen(Packets.Status(EntityID, Status.QuizPts, _UniversityPoints));
                }
            }
        }
        public uint CpsDonate
        {
            get { return _CPSDOnate; }
            set
            {
                _CPSDOnate = value;
                if (Loaded)
                    MyClient.SendPacket(Packets.Status(EntityID, Status.CPSDOnators, _CPSDOnate));
            }
        }
        public byte Stamina
        {
            get { return _Stamina; }
            set
            {
                _Stamina = value;
                if (_Stamina > 100 && BlessingLasts == 0) _Stamina = 100;
                if (_Stamina > 150 && BlessingLasts > 0) _Stamina = 150;
                if (Loaded)
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Stamina, _Stamina));
            }
        }
        public ushort Vigor
        {
            get { return _Vigor; }
            set
            {
                _Vigor = value;
                if (_Vigor > MaxVigor) _Vigor = MaxVigor;
                if (Loaded)
                    MyClient.SendPacket(Packets.Vigor(_Vigor));
            }
        }
        public ushort MaxVigor
        {
            get { return (ushort)(30 + EqStats.AddVigor); }
        }
        public ushort Avatar
        {
            get { return _Avatar; }
            set
            {
                _Avatar = value;
                if (Loaded)
                {
                    Database.SaveAvatar(this, value);
                    SendScreen(Packets.Status(EntityID, Status.Mesh, uint.Parse(_Avatar.ToString() + _Body.ToString())));
             
                }
            }
        }
        public ushort Body
        {
            get { return _Body; }
            set
            {
                _Body = value; if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.savebody(this, value);
                    SendScreen(Packets.Status(EntityID, Status.Mesh, uint.Parse(_Avatar.ToString() + _Body.ToString())));
                }
            }
        }
        public uint Mesh
        {
            get
            {
                if (Alive)
                    return uint.Parse(_Avatar.ToString() + _Body.ToString());
                else
                {
                    if (Body == 1003 || Body == 1004)
                        return uint.Parse(Convert.ToString(Avatar) + 1098.ToString());
                    else
                        return uint.Parse(Convert.ToString(Avatar) + 1099.ToString());
                }
            }
        }
        public ushort Hair
        {
            get { return _Hair; }
            set
            {
                _Hair = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveHair(this, value);
                    SendScreen(Packets.Status(EntityID, Status.Hair, _Hair));
                }
            }
        }
        public byte Job
        {
            get { return _Job; }
            set
            {
                _Job = value;
                if (Loaded)
                {
                    if (value < byte.MaxValue)
                        Database.SaveJob(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Class, _Job));
                    if (!Reborn && Level <= 120)
                        Database.GetStats(this);
                }
            }
        }
        public byte Level
        {
            get { return _Level; }
            set
            {
                byte PrevLev = _Level;
                _Level = value;
                if (MembInfo != null)
                    MembInfo.Level = _Level;
                if (Loaded)
                {
                    if (value < byte.MaxValue)
                        Database.SaveLevel(this, value);
                    World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 92));
                    SendScreen(Packets.Status(EntityID, Status.Level, _Level));

                    if (!Reborn && PrevLev < 120)
                        Database.GetStats(this);
                }
            }
        }
        public ulong Experience
        {
            get { return _Experience; }
            set
            {
                _Experience = value;
                if (Loaded)
                {
                //   if (value < ulong.MaxValue)
                 //       Database.SaveExperience(this, value); 
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Experience, _Experience));
                }
            }
        }
        public ushort Str
        {
            get { return _Str; }
            set
            {
                _Str = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveStr(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Strength, _Str));
                }
            }
        }
        public ushort Agi
        {
            get { return _Agi; }
            set
            {
                _Agi = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveAgi(this, value);

                    MyClient.SendPacket(Packets.Status(EntityID, Status.Agility, _Agi));
                }
            }
        }
        public ushort Vit
        {
            get { return _Vit; }
            set
            {
                _Vit = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveVit(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Vitality, _Vit));
                }
            }
        }
        public ushort Spi
        {
            get { return _Spi; }
            set
            {
                _Spi = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveSpi(this, value);

                    MyClient.SendPacket(Packets.Status(EntityID, Status.Spirit, _Spi));
                }
            }
        }
        public ushort StatPoints
        {
            get { return _StatPoints; }
            set
            {
                _StatPoints = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveCharStatus(this, value);

                    MyClient.SendPacket(Packets.Status(EntityID, Status.StatPoints, _StatPoints));
                }
            }
        }
        public ushort CurHP
        {
            get { return _CurHP; }
            set
            {
                _CurHP = value;
                if (LoadedEquipmentHPAdd)
                    if (_CurHP > MaxHP)
                        _CurHP = MaxHP;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.savelife(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.HP, _CurHP));
                }
            }
        }
        public ushort CurMP
        {
            get { return _CurMP; }
            set
            {
                _CurMP = value;
                if (LoadedEquipmentHPAdd)
                    if (_CurMP > MaxMP)
                        _CurMP = MaxMP;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.savemana(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.MP, _CurMP));
                }
            }
        }
        public uint Silvers
        {
            get { return _Silvers; }
            set
            {
                _Silvers = value;
                if (_Silvers > 999999999)
                {
                    if (Name == "HeroesOnline")
                    {
                        _Silvers = 999999999;
                        MyClient.LocalMessage(2000, System.Drawing.Color.Yellow,"You Cant Get Over 1000M Silvers In Your Inventory.");
                    }
                    else
                    {
                        _Silvers = 100000;
                        Program.WriteLine("ALERRRRRRRRRRRRRRRRRRT (SILVERS) ERRRRRRORR" + Name);
                    }
                }
                if (Loaded)
                {

                    if (value < uint.MaxValue)
                        Database.SaveSilver(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Silvers, _Silvers));
                }
            }

        }
        public uint CPs
        {
            get { return _CPs; }
            set
            {
                _CPs = value;
                if (_CPs > 999999999)
                {
                    if (Name == "HeroesOnline")
                    {
                        _CPs = 999999999;
                        MyClient.LocalMessage(2000, System.Drawing.Color.Yellow,"You Cant Get Over 1000M Cps In Your Inventory.");
                    }
                    else
                    {
                        _CPs = 10000;
                        Program.WriteLine("ALERRRRRRRRRRRRRRRRRRT (CPs) ERRRRRRORR" + Name);
                    }
                }
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        Database.SaveCps(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.CPs, _CPs));
                }
            }
        }
        public uint WHSilvers
        {
            get { return _WHSilvers; }
            set
            {
                _WHSilvers = value;
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        Database.SaveWhSilver(this, value);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.WHMoney, _WHSilvers));
                }
            }
        }
        public ushort PKPoints
        {
            get { return _PKPoints; }
            set
            {
                ushort Prev = _PKPoints;
                _PKPoints = value;
                if (Prev <= 99 && _PKPoints >= 100)
                {
                    StatEff.Add(StatusEffectEn.RedName);
                }
                else if (Prev >= 100 && _PKPoints <= 99)
                {
                    StatEff.Remove(StatusEffectEn.RedName);//remove effect
                }
                else if (Prev <= 199 && _PKPoints >= 200)
                {
                    StatEff.Add(StatusEffectEn.BlackName);
                }
                else if (Prev >= 200 && _PKPoints <= 199)
                {
                    StatEff.Remove(StatusEffectEn.BlackName);//remove effect
                }
                if (Loaded)
                {
                    MyClient.SendPacket(Packets.Status(EntityID, Status.PKPoints, _PKPoints));
                }
            }
        }
        public bool BlueName
        {
            get { return _BlueName; }
            set
            {
                _BlueName = value;
                if (_BlueName == true)
                {
                    StatEff.Add(StatusEffectEn.BlueName);
                    BlueNameStarted = DateTime.Now;
                }
                else
                    StatEff.Remove(StatusEffectEn.BlueName);
            }
        }
        public short AtkFrequence
        {
            get
            {
                short t = 900;
                t -= (short)(Agi + EqStats.ExtraDex);
                if (StatEff.Contains(StatusEffectEn.Cyclone) && !StatEff.Contains(StatusEffectEn.Ride))
                    t /= 3;
                if (StatEff.Contains(StatusEffectEn.SuperMan))
                    t /= 2;
                if (StatEff.Contains(StatusEffectEn.FatalStrike))
                    t /= 2;
                t = (short)Math.Max((int)t, 200);
                return t;
            }
        }

        public Character()
        {
            EqStats = new EquipStats();
            EqStats.GemExtraMAttack = 1;
            EqStats.GemExtraExp = 1;
            EqStats.GemExtraAttack = 1;
            StatEff = new StatusEffect(this);
            AtkMem = new AttackMemorise();
            AtkMem.Attacking = false;
            AtkMem.LastAttack = DateTime.Now;
            AtkMem.Target = 0;
            Buffs = new ArrayList();
            QuizShowInfo = new QuizShow.Info();
            QuizShowInfo.QNo = 1;
            QuizShowInfo.Score = 0;
            QuizShowInfo.Time = 0;
            QuizShowInfo.Answers = new byte[Features.QuizShow.Questions.Count];
            Nobility = new Nobility(this);
            Timer = new System.Timers.Timer();
            Timer.Interval = 300;
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            Timer.Start();
        }

        public DateTime LastSave = DateTime.Now;
        //public DateTime LastSave2 = DateTime.Now;
        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!Program.EndSession)
                    if (MyClient != null)
                        Step();
            }
            catch { }
            try
            {
                if (Loaded)
                    if (DateTime.Now > LastSave.AddSeconds(10))
                    {
                        LastSave = DateTime.Now;
                        Database.SaveCharacter(this, MyClient.AuthInfo.Account);
                    }
            }
            catch { }
        }

        void Step()
        {
            {
                {
                    try
                    {
                        if (this != null)
                        {
                            try
                            {
                                if (Loaded)
                                    if (DateTime.Now > LastSave.AddSeconds(10) && World.H_Chars.Contains(EntityID))
                                    {
                                        LastSave = DateTime.Now;
                                        Database.SaveCharacter(this, MyClient.AuthInfo.Account);
                                    }
                            }
                            catch { }
                            try
                            {
                                #region Toxic Fog || Poisoned
                                if (PoisonedInfo != null)
                                {
                                    int DamagePercent = 15 + PoisonedInfo.SpellLevel * 3;
                                    if (DateTime.Now > PoisonedInfo.LastAttack.AddSeconds(3))
                                    {
                                        if (CurHP <= (uint)(MaxHP / 4))
                                        {
                                            PoisonedInfo = null;
                                            StatEff.Remove(StatusEffectEn.Poisoned);
                                            goto Step;
                                        }
                                        PoisonedInfo.Times--;
                                        PoisonedInfo.LastAttack = DateTime.Now;
                                        uint Dmg = (uint)(CurHP * (DamagePercent) / 50);
                                        //  uint Dmg = (uint)(CurHP * (10 + PoisonedInfo.SpellLevel * 10) / 100);
                                        if (Dmg == 1)
                                        {
                                            PoisonedInfo = null;
                                            StatEff.Remove(StatusEffectEn.Poisoned);
                                            goto Step;
                                        }
                                        TakeAttack(this, Dmg, AttackType.Melee, false);
                                        if (PoisonedInfo.Times == 0)
                                        {
                                            PoisonedInfo = null;
                                            StatEff.Remove(StatusEffectEn.Poisoned);
                                            goto Step;
                                        }
                                    }
                                }
                                #endregion

                            Step:
                               // if (this is Robot)
                                 //   ((Robot)this).RobotStep();

                                #region Blessing
                                if (BlessingLasts > 0 && DateTime.Now > LastPts.AddMinutes(1))
                                {
                                    //Chaar.OnlineTrainingPts += 10;
                                    LastPts = DateTime.Now;
                                    //Chaar.MyClient.AddSend(Packets.Status(Chaar.EntityID, Status.OnlineTraining, 3));
                                    if (OnlineTrainingPts == 100)
                                    {
                                        OnlineTrainingPts = 0;
                                        IncreaseExp(ExpBallExp / 90 * World.Server.ExperienceRate, false);
                                        MyClient.SendPacket(Packets.Status(EntityID, Status.OnlineTraining, 4));
                                        MyClient.SendPacket(Packets.Status(EntityID, Status.OnlineTraining, 0));
                                    }
                                }
                                if (BlessingLasts > 0 && DateTime.Now > BlessingStarted.AddDays(BlessingLasts))
                                {
                                    BlessingLasts = 0;
                                    StatEff.Remove(StatusEffectEn.Blessing);
                                    MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "Thank you for buying Heaven Blessing but it has expired.");
                                }
                                #endregion
                                #region Curse
                                if (CurseStart && DateTime.Now > CurseUser.AddSeconds(CurseExpLeft))
                                {
                                    CurseExpLeft = 0;
                                    CurseStart = false;
                                    MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "Curse ended.");
                                }
                                #endregion
                                #region ExpPotion
                                if (DoubleExp && DateTime.Now > ExpPotionUsed.AddSeconds(DoubleExpLeft))
                                {
                                    DoubleExpLeft = 0;
                                    DoubleExp = false;
                                    MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "Your Double exp has ended :(.");
                                }
                                #endregion
                                #region Body & avatar dead
                                if (!Ghost && !Alive && DateTime.Now > DeathHit.AddSeconds(3))
                                {
                                    Ghost = true;
                                    MyClient.SendPacket(Packets.Status(EntityID, Status.Hair, 0));
                                    string Avt = "0";
                                    if (Avatar.ToString().Length == 1)
                                        Avt = "00" + Avatar.ToString();
                                    else if (Avatar.ToString().Length == 2)
                                        Avt = "0" + Avatar.ToString();
                                    else Avt = Avatar.ToString();
                                    if (Body == 1003 || Body == 1004)
                                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, uint.Parse("98" + Avt + Body.ToString())));
                                    else
                                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, uint.Parse("99" + Avt + Body.ToString())));
                                    World.Spawn(this, false);
                                }
                                #endregion
                                #region Stamina | LukyTime | Revive | effect
                                if ((DateTime.Now > SecondTimer.AddMilliseconds(1000)))
                                {
                                    try
                                    {
                                        CurHP = Math.Min(CurHP, MaxHP);
                                    }
                                    catch { }
                                    if (!GettingLuckyTime)
                                    {
                                        try
                                        {
                                            if (LuckyTime > 0)
                                                LuckyTime--;
                                            if (this != null)
                                                MyClient.SendPacket(Packets.Status(EntityID, Status.LuckyTime, LuckyTime * 1000));
                                            foreach (Character C in World.H_Chars.Values)
                                                if (C.StatEff.Contains(StatusEffectEn.Pray) && Loc.Map == C.Loc.Map && MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= 3)
                                                {
                                                    ThePrayer = C;
                                                    GettingLuckyTime = true;
                                                    PrayDT = DateTime.Now;
                                                    StatEff.Add(StatusEffectEn.Blessing);
                                                    MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Are Siting In LuckTime Now.");
                                                    break;
                                                }
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        if (!Prayer)
                                        {
                                            if (ThePrayer != null && Game.World.H_Chars.Contains(ThePrayer.EntityID) && ThePrayer.StatEff.Contains(StatusEffectEn.Pray) && Loc.Map == ThePrayer.Loc.Map && MyMath.PointDistance(Loc.X, Loc.Y, ThePrayer.Loc.X, ThePrayer.Loc.Y) <= 3)
                                            {
                                                MyClient.SendPacket(Packets.Status(EntityID, Status.LuckyTime, (uint)((DateTime.Now - PrayDT).TotalSeconds + LuckyTime) * 1000));
                                            }
                                            else
                                            {
                                                LuckyTime += (uint)(DateTime.Now - PrayDT).TotalSeconds;
                                                GettingLuckyTime = false;
                                                StatEff.Remove(StatusEffectEn.Blessing);
                                                MyClient.SendPacket(Packets.Status(EntityID, Status.LuckyTime, LuckyTime * 1000));
                                            }
                                        }
                                        else
                                        {
                                            if (!StatEff.Contains(StatusEffectEn.Pray))
                                            {
                                                Prayer = false;
                                                GettingLuckyTime = false;
                                                LuckyTime += (uint)((DateTime.Now - PrayDT).TotalSeconds * 3);
                                                MyClient.SendPacket(Packets.Status(EntityID, Status.LuckyTime, LuckyTime * 1000));
                                            }
                                        }
                                    }
                                    try
                                    {
                                        if ((DateTime.Now > LastStamina.AddMilliseconds(1000) && (Stamina < 150) && Stamina <= 150 && !StatEff.Contains(NewestCOServer.Game.StatusEffectEn.Fly)))
                                        {
                                            if (Action == 250)
                                                Stamina += 15;
                                            else
                                                Stamina += 5;
                                            LastStamina = DateTime.Now;
                                        }
                                        SecondTimer = DateTime.Now;
                                    }
                                    catch { }
                                }
                                #endregion
                                #region Mining
                                if (Mining && DateTime.Now > LastMine.AddSeconds(2) && Inventory.Count < 40)
                                {
                                    LastMine = DateTime.Now;
                                    Features.Mining.Swing(this);
                                }
                                #endregion
                                #region XpSkill
                                if (DateTime.Now > LastXP.AddSeconds(300 - (XPKO * 3)))
                                {
                                    try
                                    {
                                        List<Item> Remove = new List<Item>();
                                        foreach (Item I in Inventory.Values)
                                            if (!Database.DatabaseItems.Contains(I.ID))
                                                Remove.Add(I);
                                        foreach (Item I in Remove)
                                            Inventory.Remove(I.UID);
                                        Remove.Clear();
                                    }
                                    catch { }
                                    if (Alive && !StatEff.Contains(StatusEffectEn.XPStart))
                                    {
                                        LastXP = DateTime.Now;
                                        StatEff.Add(StatusEffectEn.XPStart);
                                        Buffs.Add(new Buff() { StEff = StatusEffectEn.XPStart, Lasts = 20, Started = DateTime.Now, Eff = Features.SkillsClass.ExtraEffect.None });
                                    }
                                }
                                #endregion
                                #region PkPoints & protection time
                                if (PKPoints > 0 && DateTime.Now > LastPKPLost.AddMinutes(4))
                                {
                                    PKPoints--;
                                    LastPKPLost = DateTime.Now;
                                }
                                if (Protection && DateTime.Now > LastProtection.AddSeconds(10))
                                {
                                    Protection = false;
                                }
                                #endregion
                                #region BlueName || Team Coord
                                if (BlueName && DateTime.Now > BlueNameStarted.AddSeconds(BlueNameLasts))
                                {
                                    BlueName = false;
                                    BlueNameLasts = 0;
                                }

                                if (TeamLeader && MyTeam != null && DateTime.Now > MyTeam.LastCoords.AddSeconds(3))
                                    MyTeam.LeaderCoords();
                                #endregion
                                #region Effect Buffer and SKills

                                if (DateTime.Now > LastBuffRemove.AddMilliseconds(300))
                                {
                                    LastBuffRemove = DateTime.Now;
                                    ArrayList BDelete = new ArrayList();

                                    foreach (Buff B in Buffs)
                                    {
                                        ushort Time = B.Lasts;
                                        if (B.Eff == Features.SkillsClass.ExtraEffect.Cyclone || B.Eff == Features.SkillsClass.ExtraEffect.Superman)
                                        {
                                            Time = (ushort)(B.Lasts + XPKO);
                                            if (Time > 60)
                                                Time = 60;
                                        }
                                        if (DateTime.Now > B.Started.AddSeconds(Time))
                                        {
                                            if (B.Eff == Features.SkillsClass.ExtraEffect.ShurikenVortex)
                                                VortexOn = false;
                                            BDelete.Add(B);
                                        }
                                    }
                                    bool had = false;
                                    bool stillhas = false;
                                    foreach (Buff B in BDelete)
                                    {
                                        RemoveBuff(B);
                                        if (B.Eff == Features.SkillsClass.ExtraEffect.Cyclone || B.Eff == Features.SkillsClass.ExtraEffect.Superman)
                                        {
                                            had = true;
                                            if (BuffOf(Features.SkillsClass.ExtraEffect.Cyclone).Eff == Features.SkillsClass.ExtraEffect.Cyclone || BuffOf(Features.SkillsClass.ExtraEffect.Cyclone).Eff == Features.SkillsClass.ExtraEffect.Superman)
                                                stillhas = true;
                                        }
                                    }
                                    if (had)
                                    {
                                        if (!stillhas)
                                        {
                                            World.NewKO(Name, TotalKO);
                                            TotalKO = 0;
                                        }
                                    }
                                }
                                if (VortexOn && Alive)
                                {
                                    if (DateTime.Now > LastVortexAttk.AddSeconds(1))
                                    {
                                        LastVortexAttk = DateTime.Now;
                                        ushort Dist = 13;
                                        Skill S = (Game.Skill)Skills[(ushort)6010];
                                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                                        Features.SkillsClass.SkillUse SU = new NewestCOServer.Features.SkillsClass.SkillUse();
                                        SU.Init(this, S.ID, S.Lvl, Loc.X, Loc.Y);
                                        SU.Info.ExtraEff = Features.SkillsClass.ExtraEffect.None;
                                        SU.Info.Damageing = Features.SkillsClass.DamageType.Melee;
                                        if (MapMobs != null)
                                            foreach (Mob M in MapMobs.Values)
                                            {
                                                if (M.Alive)
                                                {
                                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                                        if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                                            SU.MobTargets.Add(M, SU.GetDamage(M));
                                                }
                                            }
                                        foreach (NPC C in World.H_NPCs.Values)
                                        {
                                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                                            {
                                                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                                    if (!SU.NPCTargets.Contains(C))
                                                        SU.NPCTargets.Add(C, SU.GetDamage(C));
                                            }
                                        }

                                        if (MyMath.PointDistance(Loc.X, Loc.Y, Features.GuildWars.ThePole.Loc.X, Features.GuildWars.ThePole.Loc.Y) <= Dist && Features.GuildWars.War && MyGuild != Features.GuildWars.LastWinner)
                                            SU.MiscTargets.Add(Features.GuildWars.ThePole.EntityID, SU.GetDamage(Features.GuildWars.ThePole.CurHP));

                                        if (MyMath.PointDistance(Loc.X, Loc.Y, Features.GuildWars.TheRightGate.Loc.X, Features.GuildWars.TheRightGate.Loc.Y) <= Dist)
                                            SU.MiscTargets.Add(Features.GuildWars.TheRightGate.EntityID, SU.GetDamage(Features.GuildWars.TheRightGate.CurHP));

                                        if (MyMath.PointDistance(Loc.X, Loc.Y, Features.GuildWars.TheLeftGate.Loc.X, Features.GuildWars.TheLeftGate.Loc.Y) <= Dist)
                                            SU.MiscTargets.Add(Features.GuildWars.TheLeftGate.EntityID, SU.GetDamage(Features.GuildWars.TheLeftGate.CurHP));

                                        SU.Use();
                                    }
                                }
                                #endregion
                                #region Attacking
                                if (AtkMem.Attacking)
                                {
                                    Action = 100;
                                    if (Alive)
                                    {

                                        if (AtkMem.AtkType != 21 && DateTime.Now > AtkMem.LastAttack.AddMilliseconds(AtkFrequence))
                                        {
                                            Game.Mob PossMob = null;
                                            Game.Character PossChar = null;

                                            if (Game.World.H_Mobs.Contains(Loc.Map))
                                            {
                                                Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                                                if (MapMobs.Contains(AtkMem.Target))
                                                    PossMob = (Game.Mob)MapMobs[AtkMem.Target];
                                                else if (Game.World.H_Chars.Contains(AtkMem.Target))
                                                    PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                            }
                                            else
                                            {
                                                if (Game.World.H_Chars.Contains(AtkMem.Target))
                                                    PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                            }
                                            if (PossMob != null || PossChar != null)
                                            {
                                                uint Damage = PrepareAttack((byte)AtkMem.AtkType, true);
                                                if (PossMob != null && PossMob.Alive && (MyMath.PointDistance(Loc.X, Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 2 || AtkMem.AtkType == 28 && MyMath.PointDistance(Loc.X, Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 15))
                                                {
                                                    if (!WeaponSkill(PossMob.Loc.X, PossMob.Loc.Y, PossMob.EntityID))
                                                        PossMob.TakeAttack(this, ref Damage, (NewestCOServer.Game.AttackType)AtkMem.AtkType, false);
                                                }
                                                else if (PossChar != null && ((PossChar.StatEff.Contains(Game.StatusEffectEn.Fly) && PossChar.StatEff.Contains(Game.StatusEffectEn.Ride) || !PossChar.StatEff.Contains(Game.StatusEffectEn.Fly)) && !PossChar.StatEff.Contains(Game.StatusEffectEn.Invisible) || AtkMem.AtkType != 2) && PossChar.Alive && (MyMath.PointDistance(Loc.X, Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 2 || AtkMem.AtkType == 28 && MyMath.PointDistance(Loc.X, Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 15))
                                                {
                                                    if (!WeaponSkill(PossChar.Loc.X, PossChar.Loc.Y, PossChar.EntityID))
                                                        PossChar.TakeAttack(this, Damage, (NewestCOServer.Game.AttackType)AtkMem.AtkType, false);
                                                }
                                                else
                                                {
                                                    AtkMem.Target = 0;
                                                    AtkMem.Attacking = false;
                                                }
                                            }
                                            else if (AtkMem.Target >= 6700 && AtkMem.Target <= 6702)
                                            {
                                                AtkMem.LastAttack = DateTime.Now;
                                                uint Damage = PrepareAttack((byte)(NewestCOServer.Game.AttackType)AtkMem.AtkType, true);
                                                if (AtkMem.Target == 6700)
                                                {
                                                    if (Features.GuildWars.War)
                                                    {
                                                        if (!WeaponSkill(Features.GuildWars.ThePole.Loc.X, Features.GuildWars.ThePole.Loc.Y, Features.GuildWars.ThePole.EntityID))
                                                            Features.GuildWars.ThePole.TakeAttack(this, Damage, AtkMem.AtkType);
                                                    }
                                                    else
                                                    {
                                                        AtkMem.Target = 0;
                                                        AtkMem.Attacking = false;
                                                    }
                                                }
                                                else if (AtkMem.Target == 6701)
                                                {
                                                    if (!WeaponSkill(Features.GuildWars.TheLeftGate.Loc.X, Features.GuildWars.TheLeftGate.Loc.Y, Features.GuildWars.TheLeftGate.EntityID))
                                                        Features.GuildWars.TheLeftGate.TakeAttack(this, Damage, AtkMem.AtkType);
                                                }
                                                else
                                                {
                                                    if (!WeaponSkill(Features.GuildWars.TheRightGate.Loc.X, Features.GuildWars.TheRightGate.Loc.Y, Features.GuildWars.TheRightGate.EntityID))
                                                        Features.GuildWars.TheRightGate.TakeAttack(this, Damage, AtkMem.AtkType);
                                                }
                                            }
                                            if (PossChar == null && PossMob == null)
                                            {
                                                NPC PossNPC = (NPC)Game.World.H_NPCs[AtkMem.Target];
                                                if (PossNPC != null && PossNPC.Flags == 21 && (MyMath.PointDistance(Loc.X, Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 3 || AtkMem.AtkType == 28 && MyMath.PointDistance(Loc.X, Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 15))
                                                {
                                                    if (DateTime.Now > AtkMem.LastAttack.AddMilliseconds(AtkFrequence))
                                                    {
                                                        uint Damage = PrepareAttack((byte)AtkMem.AtkType, true);
                                                        if (!WeaponSkill(PossNPC.Loc.X, PossNPC.Loc.Y, PossNPC.EntityID))
                                                            PossNPC.TakeAttack(this, Damage, (NewestCOServer.Game.AttackType)AtkMem.AtkType, false);
                                                    }
                                                }
                                            }
                                        }
                                        else if (DateTime.Now > AtkMem.LastAttack.AddMilliseconds(1000))
                                        {
                                            Game.Mob PossMob = null;
                                            Game.Character PossChar = null;
                                            Game.Companion PossCompanion = null;
                                            if (Game.World.H_Mobs.Contains(Loc.Map))
                                            {
                                                Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                                                if (MapMobs.Contains(AtkMem.Target))
                                                    PossMob = (Game.Mob)MapMobs[AtkMem.Target];
                                                else if (Game.World.H_Chars.Contains(AtkMem.Target))
                                                    PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                                else if (Game.World.H_Companions.Contains(AtkMem.Target))
                                                    PossCompanion = (Game.Companion)Game.World.H_Companions[AtkMem.Target];
                                            }
                                            else if (Game.World.H_Chars.Contains(AtkMem.Target))
                                            {

                                                PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                            }
                                            else if (Game.World.H_Companions.Contains(AtkMem.Target))
                                            {

                                                PossCompanion = (Game.Companion)Game.World.H_Companions[AtkMem.Target];
                                            }
                                            if (AtkMem.Skill != 0 && Skills.Contains(AtkMem.Skill) && PossChar == null)
                                            {
                                                AtkMem.LastAttack = DateTime.Now;
                                                Skill S = (Skill)Skills[AtkMem.Skill];
                                                if (Features.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                                                {
                                                    Features.SkillsClass.SkillUse SU = new NewestCOServer.Features.SkillsClass.SkillUse();
                                                    SU.Init(this, S.ID, S.Lvl, AtkMem.SX, AtkMem.SY);
                                                    if (SU.Info.ID != 0)
                                                    {
                                                        if (AtkMem.Skill == 6001)
                                                            AddSkillExp(6001, 10);
                                                        SU.GetTargets(AtkMem.Target);
                                                        SU.Use();
                                                    }
                                                    else
                                                        AtkMem.Attacking = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region CoolEffect
                                if (Action == 230)
                                {
                                    //PacketHandling.CoolEffect.ActiveCool(MyClient);
                                }
                                #endregion
                            }
                            catch (Exception Exc)
                            {
                                Program.WriteLine(Exc);
                            }
                        }
                    }
                    catch { }
                    /*World.laststep = DateTime.Now;
                timer1:
                    if (DateTime.Now > World.laststep.AddMilliseconds(200))
                        goto step1;
                    else
                        goto timer1;*/
                }
            }
        }
        public void RebornCharacter(byte ToJob)
        {
            try
            {
                for (byte i = 1; i < 9; i++)
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6 || i == 8)
                    {
                        Item I = Equips.Get(i);
                        if (I.ID != 0)
                        {
                            EquipStats(i, false);
                            ItemIDManipulation IDM = new ItemIDManipulation(I.ID);
                            IDM.LowestLevel(i);
                            I.ID = IDM.ToID();
                            Equips.Replace(i, I, this);
                            EquipStats(i, true);
                        }
                    }
                    else
                    {
                        Item I = Equips.Get(i);
                        Equips.Replace(i, I, this);
                    }
            }
            catch { }
            // Item It = new Item();
            if (Equips.RightHand.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);

            }
            if (Equips.Fan.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);

            }
            if (Equips.Tower.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);

            }
            if (Level > 130)
                oldlevel = Level;
            Reborns += 1;
            byte ExtraStat = 0;
            if (Level >= 120)
                ExtraStat = (byte)((-120 + Level) * 3 + Reborns * 10 + 45);
            else
                ExtraStat = (byte)(Reborns * 10);
            StatPoints = ExtraStat;
            Level = 15;
            Experience = 0;
            foreach (Skill S in Skills.Values)
            {
                MyClient.SendPacket(Packets.GeneralData(EntityID, S.ID, 0, 0, 109));
            }
            foreach (Prof P in Profs.Values)
            {
                MyClient.SendPacket(Packets.GeneralData(EntityID, P.ID, 0, 0, 108));
            }

            Skills.Clear();
            Profs.Clear();
            Database.deleteprofreborn(this);
            Database.deletepsekillreborn(this);
            #region Ninja
            if (Job == 55)
            {
                PreviousJob1 = 55;
                if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 6004 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 6001 });
                }
            }
            #endregion
            #region Trojan
            if (Job == 15)
            {
                PreviousJob1 = 15;
                if (ToJob == 41 || ToJob == 142 || ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 3050 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1270 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                }

            }
            #endregion
            #region Warrior
            if (Job == 25)
            {
                PreviousJob1 = 25;
                if (ToJob == 41 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1025 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region WaterTao
            if (Job == 135)
            {
                PreviousJob1 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                }
                else if (ToJob == 11 || ToJob == 21 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 3090 });
                }
            }
            #endregion
            #region Archer
            if (Job == 45)
            {
                PreviousJob1 = 45;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 5000 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 5002 });
                }
            }
            #endregion
            #region FireTao
            if (Job == 145)
            {
                PreviousJob1 = 145;
                if (ToJob == 11 || ToJob == 21 || ToJob == 41 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 3080 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1120 });
                }
            }
            #endregion
            Job = ToJob;
            if (Reborns >= 1)
                NewSkill(new Skill() { ID = 4000 });

            Database.GetStats(this);
            MyClient.LocalMessage(2000,System.Drawing.Color.Yellow , "Congratulations! You are now reborn. All your skills and proficiency are gone.");
            World.SendMsgToAll("SYSTEM", Name + " has got " + Reborns.ToString() + " reborn!", 2011, 0, System.Drawing.Color.Red);
            Teleport(1002, 439, 390);
            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
            Database.SaveCharacter(this, MyClient.AuthInfo.Account);
            MyClient.Disconnect();
        }
        public void RebornCharacter2(byte ToJob)
        {
            try
            {
                for (byte i = 1; i < 9; i++)
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6 || i == 8)
                    {
                        Item I = Equips.Get(i);
                        if (I.ID != 0)
                        {
                            EquipStats(i, false);
                            ItemIDManipulation IDM = new ItemIDManipulation(I.ID);
                            IDM.LowestLevel(i);
                            I.ID = IDM.ToID();
                            Equips.Replace(i, I, this);
                            EquipStats(i, true);
                        }
                    }
                    else
                    {
                        Item I = Equips.Get(i);
                        Equips.Replace(i, I, this);
                    }
            }
            catch { }
            //  Item IT = new Item();
            if (Equips.RightHand.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);
            }
            if (Equips.Fan.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);
            }
            if (Equips.Tower.ID != 0)
            {
                AddFullItem(Equips.Get(5).ID, Equips.Get(5).Bless, Equips.Get(5).Plus, Equips.Get(5).Enchant, Equips.Get(5).Soc1, Equips.Get(5).Soc2, Equips.Get(5).Color, Equips.Get(5).Progress, Equips.Get(5).TalismanProgress, Equips.Get(5).Effect, Equips.Get(5).FreeItem, Equips.Get(5).CurDur, Equips.Get(5).MaxDur, Equips.Get(5).Suspicious, Equips.Get(5).Locked, Equips.Get(5).LockedDays, Equips.Get(5).RBG[0], Equips.Get(5).RBG[1], Equips.Get(5).RBG[2], Equips.Get(5).RBG[3]);
                EquipStats(5, false);
                Game.World.Spawn(this, false);
                Equips.UnEquip(5, this);
            }
            if (Level > 130)
                oldlevel = Level;
            Reborns += 1;
            byte ExtraStat = 0;
            if (Level >= 120)
                ExtraStat = (byte)((-120 + Level) * 3 + Reborns * 10 + 45);
            else
                ExtraStat = (byte)(Reborns * 10);
            StatPoints = ExtraStat;
            Level = 15;
            Experience = 0;
            foreach (Skill S in Skills.Values)
            {
                MyClient.SendPacket(Packets.GeneralData(EntityID, S.ID, 0, 0, 109));
            }
            foreach (Prof P in Profs.Values)
            {
                MyClient.SendPacket(Packets.GeneralData(EntityID, P.ID, 0, 0, 108));
            }

            Skills.Clear();
            Profs.Clear();
            Database.deleteprofreborn(this);
            Database.deletepsekillreborn(this);

            #region Archer2
            #region Arch-Arch
            if (PreviousJob1 == 45 && Job == 45)
            {
                PreviousJob2 = 45;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 5000 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 5002 });
                }
            }
            #endregion
            #region Arch-Fire
            else if (PreviousJob1 == 45 && Job == 145)
            {
                PreviousJob2 = 145;
                if (ToJob == 11 || ToJob == 21 || ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 5002 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 10010 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1120 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 3080 });
                }
            }
            #endregion
            #region Arch-Tro
            if (PreviousJob1 == 45 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 132 || ToJob == 142 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 3050 });
                    NewSkill(new Skill() { ID = 5002 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 5100 });
                }

            }

            #endregion
            #region Arch-War
            if (PreviousJob1 == 45 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 21 || ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1025 });
                }
            }


            #endregion
            #region Arch-Water
            if (PreviousJob1 == 45 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                }
                else if (ToJob == 11 || ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 3090 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 10010 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                }
            }
            #endregion
            #region Arch-Nin
            if (PreviousJob1 == 45 && Job == 55)
            {
                PreviousJob2 = 55;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 6001 });
                }
                else if (ToJob == 11 || ToJob == 21 || ToJob == 132 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 6001 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 6004 });
                }
            }
            #endregion
            #endregion
            #region Trojan2
            #region Tro-Arch
            if (PreviousJob1 == 15 && Job == 45)
            {
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 5000 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 5002 });
                }
            }

            #endregion
            #region Tro-Fire
            if (PreviousJob1 == 15 && Job == 145)
            {
                PreviousJob2 = 145;
                if (ToJob == 41 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 11 || ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 3080 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1120 });
                }
            }
            #endregion
            #region Tro-Tro
            if (PreviousJob1 == 15 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3080 });
                }
                else if (ToJob == 11 || ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1120 });
                }
            }


            #endregion
            #region Tro-War
            if (PreviousJob1 == 15 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1025 });
                }
            }
            #endregion
            #region Tro-Water
            if (PreviousJob1 == 15 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1090 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1085 });
                }
                else if (ToJob == 21 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1090 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3090 });
                }
            }
            #endregion
            #region Tro-Nin
            if (PreviousJob1 == 15 && Job == 55)
            {
                PreviousJob2 = 55;
                if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 6000 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 6002 });
                    NewSkill(new Skill() { ID = 6003 });
                    NewSkill(new Skill() { ID = 6004 });
                    NewSkill(new Skill() { ID = 6011 });
                    NewSkill(new Skill() { ID = 6010 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 6001 });
                }
            }
            #endregion
            #endregion
            #region Ninja2
            #region Nin-Arch
            if (PreviousJob1 == 55 && Job == 45)
            {
                PreviousJob2 = 45;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 5000 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 5002 });
                }
                else if (ToJob == 11 || ToJob == 21 || ToJob == 142 || ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 5002 });
                }
            }
            #endregion
            #region Nin-Fire
            {
                PreviousJob2 = 145;
                if (PreviousJob1 == 55 && Job == 145)
                {
                    if (ToJob == 11 || ToJob == 21 || ToJob == 41 || ToJob == 51)
                    {
                        NewSkill(new Skill() { ID = 6001 });
                        NewSkill(new Skill() { ID = 1000 });
                        NewSkill(new Skill() { ID = 1001 });
                        NewSkill(new Skill() { ID = 1005 });
                        NewSkill(new Skill() { ID = 1195 });
                    }
                    else if (ToJob == 142)
                    {
                        NewSkill(new Skill() { ID = 6001 });
                        NewSkill(new Skill() { ID = 3080 });
                        NewSkill(new Skill() { ID = 1000 });
                    }
                    else if (ToJob == 132)
                    {
                        NewSkill(new Skill() { ID = 1120 });
                        NewSkill(new Skill() { ID = 6001 });
                    }
                }
            }
            #endregion
            #region Nin-Tro
            if (PreviousJob1 == 55 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 51 || ToJob == 132 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 6001 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 6001 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 5100 });
                }

            }
            #endregion
            #region Nin-War
            if (PreviousJob1 == 55 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1040 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1025 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1040 });
                }
            }
            #endregion
            #region Nin-Water
            if (PreviousJob1 == 55 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                }
                else if (ToJob == 11 || ToJob == 21 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 3090 });
                }
            }
            #endregion
            #region Nin-Nin
            if (PreviousJob1 == 55 && Job == 55)
            {
                PreviousJob2 = 55;
                if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 6000 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 6002 });
                    NewSkill(new Skill() { ID = 6003 });
                    NewSkill(new Skill() { ID = 6004 });
                    NewSkill(new Skill() { ID = 6010 });
                    NewSkill(new Skill() { ID = 6011 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 6001 });
                }
            }
            #endregion
            #endregion
            #region Fire2
            #region Fire-Arch
            if (PreviousJob1 == 145 && Job == 45)
            {
                PreviousJob2 = 45;
                NewSkill(new Skill() { ID = 1000 });
                NewSkill(new Skill() { ID = 1001 });
                NewSkill(new Skill() { ID = 1005 });
                NewSkill(new Skill() { ID = 1195 });
                NewSkill(new Skill() { ID = 5002 });
            }
            #endregion
            #region Fire-Fire
            if (PreviousJob1 == 145 && Job == 145)
            {
                PreviousJob2 = 145;
                if (ToJob == 41 || ToJob == 11 || ToJob == 51 || ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 3080 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 3080 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 3080 });
                    NewSkill(new Skill() { ID = 1120 });
                }
            }
            #endregion
            #region Fire-Tro
            if (PreviousJob1 == 145 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 142 || ToJob == 132 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3050 });
                }
            }
            #endregion
            #region Fire-War
            if (PreviousJob1 == 145 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1025 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                }
            }
            #endregion
            #region Fire-Water
            if (PreviousJob1 == 145 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1075 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1175 });
                }
                else if (ToJob == 11 || ToJob == 21 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });

                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 3090 });
                    NewSkill(new Skill() { ID = 1120 });
                }
            }
            #endregion
            #region Fire-Nin
            if (PreviousJob1 == 145 && Job == 55)
            {
                PreviousJob2 = 55;
                if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 3080 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
            }
            #endregion
            #endregion
            #region War2
            #region War-Arch
            if (PreviousJob1 == 25 && Job == 45)
            {
                PreviousJob2 = 45;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 5000 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 5002 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 5002 });
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region War-Fire
            if (PreviousJob1 == 25 && Job == 145)
            {
                PreviousJob2 = 145;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3080 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 25)
                {
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1120 });
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region War-Tro
            if (PreviousJob1 == 25 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 142 || ToJob == 132 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3050 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 5100 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region War-War
            if (PreviousJob1 == 25 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41 || ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 3060 });

                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1015 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1025 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region War-Water
            if (PreviousJob1 == 25 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1025 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3090 });
                    NewSkill(new Skill() { ID = 1280 });
                    NewSkill(new Skill() { ID = 1350 });
                }
                else if (ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
            }
            #endregion
            #region War-Nin
            if (PreviousJob1 == 25 && Job == 55)
            {
                PreviousJob2 = 55;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 1320 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 6001 });
                    NewSkill(new Skill() { ID = 6002 });

                }
            }
            #endregion
            #endregion
            #region Water2
            #region Water-Arch
            if (PreviousJob1 == 135 && Job == 45)
            {
                PreviousJob2 = 45;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 5000 });
                    NewSkill(new Skill() { ID = 5002 });
                }
                else
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 5000 });
                    NewSkill(new Skill() { ID = 5002 });
                }
            }
            #endregion
            #region Water-Fire
            if (PreviousJob1 == 135 && Job == 145)
            {
                PreviousJob2 = 145;
                if (ToJob == 11 || ToJob == 21 | ToJob == 41 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1000 });
                    NewSkill(new Skill() { ID = 1001 });
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1195 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1120 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 3080 });
                }
            }

            #endregion
            #region Water-Tro
            if (PreviousJob1 == 135 && Job == 15)
            {
                PreviousJob2 = 15;
                if (ToJob == 41 || ToJob == 142 || ToJob == 132 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1190 });
                    NewSkill(new Skill() { ID = 1110 });
                    NewSkill(new Skill() { ID = 1270 });
                    NewSkill(new Skill() { ID = 5100 });
                }
                else if (ToJob == 11)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3050 });
                }
            }
            #endregion
            #region Water-War
            if (PreviousJob1 == 135 && Job == 25)
            {
                PreviousJob2 = 25;
                if (ToJob == 41)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1350 });
                    NewSkill(new Skill() { ID = 1280 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 11 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1015 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1320 });
                }
                else if (ToJob == 21)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3060 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 1020 });
                    NewSkill(new Skill() { ID = 1040 });
                    NewSkill(new Skill() { ID = 3060 });
                    NewSkill(new Skill() { ID = 1025 });
                }
            }
            #endregion
            #region Water-Water
            if (PreviousJob1 == 135 && Job == 135)
            {
                PreviousJob2 = 135;
                if (ToJob == 11 || ToJob == 21 || ToJob == 41 || ToJob == 51)
                {
                    NewSkill(new Skill() { ID = 1005 });
                    NewSkill(new Skill() { ID = 1085 });
                    NewSkill(new Skill() { ID = 1090 });
                    NewSkill(new Skill() { ID = 1095 });
                    NewSkill(new Skill() { ID = 1195 });
                    NewSkill(new Skill() { ID = 3090 });
                }
                else if (ToJob == 132)
                {
                    NewSkill(new Skill() { ID = 3090 });
                }
                else if (ToJob == 142)
                {
                    NewSkill(new Skill() { ID = 1050 });
                    NewSkill(new Skill() { ID = 1075 });
                    NewSkill(new Skill() { ID = 1055 });
                    NewSkill(new Skill() { ID = 1175 });
                    NewSkill(new Skill() { ID = 3090 });
                }
            }

            #endregion
            #region Water-Nin
            if (PreviousJob1 == 135 && Job == 55)
            {
                PreviousJob2 = 55;
                NewSkill(new Skill() { ID = 1005 });
                NewSkill(new Skill() { ID = 1085 });
                NewSkill(new Skill() { ID = 1090 });
                NewSkill(new Skill() { ID = 1095 });
                NewSkill(new Skill() { ID = 1195 });
                NewSkill(new Skill() { ID = 6001 });
            }
            #endregion
            #endregion
            Job = ToJob;
            NewSkill(new Skill() { ID = 9876 });
            Database.GetStats(this);
            MyClient.LocalMessage(2000,System.Drawing.Color.Yellow ,"Congratulations! You are now reborn. All your skills and proficiency are gone.");
            World.SendMsgToAll("SYSTEM", Name + " has got " + Reborns.ToString() + " reborn!", 2011, 0, System.Drawing.Color.Red);
            Teleport(1002, 439, 390);
            StatPoints += 150;
            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
            Database.SaveCharacter(this, MyClient.AuthInfo.Account);
            MyClient.Disconnect();
            
        }
        public bool AddProfLevels(ushort Wep)
        {
            if (Profs.Contains(Wep))
            {
                Prof P = (Prof)Profs[Wep];
                if (P.Lvl < 12)
                {
                    int Req = 0;
                    #region Switch Req
                    switch (P.Lvl)
                    {
                        case 0:
                        case 1: Req = 1; break;
                        case 2: Req = 1; break;
                        case 3: Req = 1; break;
                        case 4: Req = 1; break;
                        case 5: Req = 2; break;
                        case 6: Req = 3; break;
                        case 7: Req = 3; break;
                        case 8: Req = 5; break;
                        case 9: Req = 10; break;
                        case 10: Req = 11; break;
                        case 11: Req = 12; break;//
                    }
                    #endregion
                    if (InventoryContains(723700, (byte)Req, MyClient))
                    {
                        for (byte i = 0; i < Req; i++)
                            RemoveItemID(723700, MyClient);
                    }
                    else
                    {
                        PacketHandling.NPCDialog.ErrorMsg("You don't have the required ExpBalls!", MyClient);
                        return false;
                    }
                    Profs.Remove(Wep);
                    P.Lvl++;
                    P.Exp = 0;
                    Profs.Add(Wep, P);
                    MyClient.SendPacket(Packets.Prof(P));
                    return true;
                }
                else
                {
                    PacketHandling.NPCDialog.ErrorMsg("You can't upgrade that prof anymore!", MyClient);
                    return false;
                }
            }
            else
            {
                PacketHandling.NPCDialog.ErrorMsg("You don't have a level of this prof, go get it first!", MyClient);//now try... that should work... kk
                return false;
            }
        }
        public uint PromoteItems
        {
            get
            {
                uint e = 0;
                if (Job == 41)
                    e = 1072031;
                else
                {
                    sbyte n = 0;
                    if (Job >= 10 && Job <= 15)
                        n = (sbyte)(Job - 10);
                    else if (Job >= 20 && Job <= 25)
                        n = (sbyte)(Job - 20);
                    else if (Job >= 40 && Job <= 45)
                        n = (sbyte)(Job - 40);
                    else if (Job >= 50 && Job <= 55)
                        n = (sbyte)(Job - 50);
                    else if (Job >= 100)
                    {
                        if (Job <= 101)
                            n = (sbyte)(Job - 100);
                        else if (Job >= 132 && Job <= 135)
                            n = (sbyte)(Job - 130);
                        else if (Job >= 142 && Job <= 145)
                            n = (sbyte)(Job - 140);
                    }
                    if (n == 0 || n == 1) return 1;
                    if (n == 2)
                        e = 1080001;
                    else if (n == 3)
                        e = 1088001;
                    else if (n == 4)
                        e = 721080;
                    else
                        e = 0;
                }
                return e;
            }
        }
        public bool CanBeMeleed
        {
            get
            {
                if (StatEff.Array.Contains(StatusEffectEn.Fly) || StatEff.Array.Contains(StatusEffectEn.Invisible))
                    return false;
                return true;
            }
        }
        public bool PKAble(PKMode PK, Character C)
        {
            if (PK == PKMode.PK)
                return true;
            else if (PK == PKMode.Capture)
                return BlueName;
            else if (PK == PKMode.Team)
            {
                Friend f = new Friend();
                f.Name = C.Name;
                f.UID = C.EntityID;
                Features.MemberInfo M = new Features.MemberInfo();
                M.MembID = C.EntityID;
                M.MembName = C.Name;
                if (MyTeam != null)
                    return !MyTeam.Members.Contains(C);
                else if (Friends.Contains(f.UID))
                    return Friends.Contains(new Friend { UID = C.EntityID, Name = C.Name });
                else if (C.MyGuild == null)
                    return true;
                else if (MyGuild.Allies.ContainsKey(C.MyGuild.GuildID))
                    return !MyGuild.Allies.ContainsKey(C.MyGuild.GuildID);
                else if (MyGuild != null)
                    return MyGuild.Members.Contains(M.MembID);
                else
                    return true;
            }
            return false;
        }
        /*public void Teleport(ushort Map, ushort X, ushort Y)
        {
            
            if (DMaps.H_DMaps2.Contains(Map2))
            {
                Map2 = Convert.ToUInt16(DMaps.H_DMaps2[Map]);
            }
            World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
            MyClient.SendPacket(Packets.GeneralData(EntityID, Map2, X, Y, 86));

            Loc.X = X;
            Loc.Y = Y;
            if (Map != 700)
                Loc.PreviousMap = Loc.Map;
            Loc.Map = Map;

            ushort MapStatus = DMaps.GetMapStatus(Map);
            if (Map >= 10000)
                MapStatus = 65535;
            MyClient.SendPacket(Packets.MapStatus(Map2, MapStatus));

            World.Spawns(this, false);
            Database.savemap(this);
        }*/
        public void Teleport(ushort Map, ushort X, ushort Y)
        {
            ushort Map2 = Map;
            World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
            MyClient.SendPacket(Packets.GeneralData(EntityID, Map, X, Y, 86));
            LastProtection = DateTime.Now;
            Protection = true;
            if (MyCompanion != null)
                MyCompanion.Dissappear();
            if (MyCompanion1 != null)
                MyCompanion1.Dissappear();
            if (MyCompanion2 != null)
                MyCompanion2.Dissappear();
            if (MyCompanion3 != null)
                MyCompanion3.Dissappear();
            if (MyCompanion4 != null)
                MyCompanion4.Dissappear();
            if (MyCompanion5 != null)
                MyCompanion5.Dissappear();
            if (MyCompanion6 != null)
                MyCompanion6.Dissappear();
            if (MyCompanion7 != null)
                MyCompanion7.Dissappear();
            if (MyCompanion8 != null)
                MyCompanion8.Dissappear();
            if (MyCompanion9 != null)
                MyCompanion9.Dissappear();
            if (MyCompanion10 != null)
                MyCompanion10.Dissappear();
            if (MyCompanion11 != null)
                MyCompanion11.Dissappear();
            if (Map != 1707 && Name != "HeroesOnline" && Name != "Hragon")
                Loc.MapCopy = 0;
            if (Loc.Map == 1707 && (Name == "HeroesOnline" || Name == "Hragon"))
                Loc.MapCopy = 0;
            if (Loc.Map == 1707 && Luchando == true)
                if (Map != 1707)
                {
                    foreach (Character C in World.H_Chars.Values)
                    {
                        if (C != null)
                            if (C.Name == Enemigo)
                            {
                                C.CPs += (uint)C.Apuesta;
                                C.CPs += (uint)C.Apuesta;
                                World.pvpclear(C);
                                C.Teleport(1002, 429, 378);
                                C.MyClient.LocalMessage(2011, System.Drawing.Color.Violet,"The opprent has retreated out  u WON THE BATTELE " + (Apuesta) * 2 + "CPs congratulations");
                            }
                    }
                    World.pvpclear(this);
                }
            if ((Loc.Map == 1018 || Loc.Map == 1082) && InPKT && Map != 1082)
            {
                InPKT = false;
                if (Alive && Features.PKTournament.PKTHash.ContainsKey(EntityID))
                    Features.PKTournament.PKTHash.Remove(EntityID);
            }
            Loc.X = X;
            Loc.Y = Y;
            AtkMem.Target = 0;
            AtkMem.Attacking = false;
            if (Map != 700)
                Loc.PreviousMap = Loc.Map;
            Loc.Map = Map;
            if (Map != 700)
                Loc.PreviousMap = Loc.Map;
            Loc.Map = Map;

            ushort MapStatus = DMaps.GetMapStatus(Map);
            if (Map >= 10000)
                MapStatus = 65535;
            MyClient.SendPacket(Packets.MapStatus(Map2, MapStatus));

            Database.savemap(this);
            if (Loc.Map == 1036)
                MyClient.SendPacket(Packets.MapStatus(Loc.Map, 30));
            else
                MyClient.SendPacket(Packets.MapStatus(Loc.Map, 2080));
            World.Spawns(this, false);

           // if (this is Robot)
                //((Robot)this).OnTeleport();
        }
        public void AddSkillExp(ushort ID, uint Amount)
        {
            if (Skills.Contains(ID))
            {
                Skill S = (Skill)Skills[ID];
                Features.SkillsClass.SkillInfo Info = S.Info;
                if (Info.UpgReqExp != 0 && Info.ID == ID && Level >= Info.UpgReqLvl)
                {
                    Amount *= World.Server.SkillExpRate;
                    Skills.Remove(ID);
                    S.Exp += Amount;
                    if (S.Exp >= Info.UpgReqExp)
                    {
                        S.Lvl++;
                        S.Exp = 0;
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Congratulations! Your skill level has increased.");
                        Database.SaveSkill(S, this);
                    }
                    Skills.Add(ID, S);
                    MyClient.SendPacket(Packets.Skill(S));
                }
            }
        }
        public Buff BuffOf(SkillsClass.ExtraEffect E)
        {
            if (Buffs.Count > 0)
            {
                Buff[] Bufffs = new Buff[Buffs.Count];
                if (Buffs.Count > 0)
                {
                    Buffs.CopyTo(Bufffs, 0);
                }
                foreach (Buff B in Bufffs)
                    if (B.Eff == E)
                        return B;
                Bufffs = null;

            }
            return new Buff();
        }
        CastEffect transEffect;
        static ulong transValue;
        public void AddBuff(Buff B)
        {

            Buff ExBuff = BuffOf(B.Eff);
            if (ExBuff.Eff == B.Eff)
                Buffs.Remove(ExBuff);
            if (B.Eff == SkillsClass.ExtraEffect.Transform)
            {
                switch (B.Transform)
                {
                    case 2000:
                    case 2001:
                    case 2002:
                    case 2003:
                    case 2010:
                    case 2011:
                    case 2012:
                    case 2013:
                        transValue = (ulong)2141000000;
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, transValue));
                        break;
                    case 2005:
                    case 2006:
                    case 2007:
                    case 2008:
                    case 2009:
                    case 2040:
                    case 2041:
                    case 2042:
                    case 2043:
                        transValue = (ulong)2131000000;
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, transValue));
                        break;
                    case 2020:
                    case 2021:
                    case 2022:
                    case 2023:
                    case 2024:
                        transValue = (ulong)2071000000;
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, transValue));
                        break;
                    case 2030:
                    case 2031:
                    case 2032:
                    case 2033:
                    case 2034:
                        transValue = (ulong)2171000000;
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, transValue));
                        break;

                    default:
                        transValue = (ulong)(B.Transform * 10000000 + Avatar * 10000 + Mesh);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, transValue));
                        break;
                }
                transEffect = new CastEffect();
                transEffect.countdown = B.Lasts * 10;
                transEffect.B = B;
                transEffect.C = this;
                transEffect.transValue = transValue;
                transEffect.executeEffect();
            }
            Buffs.Add(B);
            StatEff.Add(B.StEff);
        }

        public void RemoveBuff(Buff B)
        {
            Buffs.Remove(B);
            StatEff.Remove(B.StEff);
            if (B.Eff == SkillsClass.ExtraEffect.Transform)
            {
                transEffect.timer.Dispose();
                transEffect.countdown = 0;
                Body = Body;
                Hair = Hair;
                Equips.Send(MyClient, false);
            }
        }
    
        public void AddFullItem(uint ID, byte Bless, byte PLus, byte enchant, Game.Item.Gem Soc1, Game.Item.Gem Soc2, Item.ArmorColor color, ushort progres, uint talismanpro, Item.RebornEffect efect, bool FreeItem, ushort curdur, ushort maxdur, int sus, byte look, uint LockedDay, byte r, byte b, byte g, byte floor)
        {
            Item I = new Item();
            I.Bless = Bless;
            I.FreeItem = FreeItem;
            I.Plus = PLus;
            I.Soc1 = Soc1;
            I.Soc2 = Soc2;
            I.Color = color;
            I.RBG[0] = r;
            I.RBG[1] = b;
            I.RBG[2] = g;
            I.RBG[3] = floor;
            I.Progress = progres;
            if (I.ID == 300000)
            {
                I.TalismanProgress = BitConverter.ToUInt32(I.RBG, 0);
            }
            else
                I.TalismanProgress = talismanpro;
            I.Effect = efect;
            I.Enchant = enchant;
            I.ID = ID;
            I.MaxDur = curdur;
            I.CurDur = maxdur;
            I.Locked = look;
            I.LockedDays = LockedDay;
            I.Suspicious = sus;
            I.UID = (uint)Rnd.Next(1, 9999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 9999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            if (Loaded)
                MyClient.SendPacket(Packets.AddItem(I, 0));

        }
        public void AddItemPos(uint ID)
        {
            Item I = Equips.Get(9);
            I.ID = ID;
            I.UID = (uint)Rnd.Next(1, 9999999);
            //bool created = Database.NewItem(I, this);
           // while (!created)
           //// {
           //     I.UID = (uint)Rnd.Next(1, 9999999);
           //     created = Database.NewItem(I, this);
           // }
            Equips.Replace(9, I, this);
            EquipStats(9, true);
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            MyClient.SendPacket(Packets.AddItem(I, 9));
            Database.SaveCharacter(this, MyClient.AuthInfo.Account);
        }
        public void AddItem(uint ID, byte Plus, byte ench, byte bless, Item.Gem soc1, Item.Gem soc2)
        {
            Item I = new Item();
            I.Plus = Plus;
            I.ID = ID;
            I.UID = (uint)Rnd.Next(10000000);
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            I.Enchant = ench;
            I.Bless = bless;
            I.Soc1 = soc1;
            I.Soc2 = soc2;
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            I.UID = (uint)Rnd.Next(1, 9999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 9999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            MyClient.SendPacket(Packets.AddItem(I, 0));
        }
        public void AddItem(uint ID)
        {
            Item I = new Item();
            I.ID = ID;
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            if (I.ID == 300000)
            {
                I.TalismanProgress = BitConverter.ToUInt32(I.RBG, 0);
            }
            else
                I.TalismanProgress = I.ID;
            I.UID = (uint)Rnd.Next(1, 99999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 99999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            MyClient.SendPacket(Packets.AddItem(I, 0));
        }
        public void AddItemStone(uint ID,byte Plus)
        {
            Item I = new Item();
            I.ID = ID;
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            I.Plus = Plus;
            if (I.ID == 300000)
            {
                I.TalismanProgress = BitConverter.ToUInt32(I.RBG, 0);
            }
            else
                I.TalismanProgress = I.ID;
            I.UID = (uint)Rnd.Next(1, 99999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 99999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            MyClient.SendPacket(Packets.AddItem(I, 0));
        }
        public void AddItem(uint ID, byte Plus)
        {
            //     Item.UID = Database.Database.ItemUID;
            Item I = new Item();
            I.Plus = Plus;
            I.ID = ID;
            I.MaxDur = I.DBInfo.Durability;
            I.CurDur = I.MaxDur;
            if (I.ID == 300000)
            {
                I.TalismanProgress = BitConverter.ToUInt32(I.RBG, 0);
            }
            else
                I.TalismanProgress = I.ID;
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 9999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            MyClient.SendPacket(Packets.AddItem(I, 0)); Database.NewItem(I, this);
            Database.NewItem(I, this);
        }
        public static void GenerateRandomQuestion()
        {

            if (File.Exists(@"C:\OldCODB\Questions.txt"))
            {
                Character.Question = ""; //needs reset
                Character.Answer = "";
                string[] Lines = File.ReadAllLines(@"C:\OldCODB\Questions.txt");
                List<string> Words = new List<string>();
                foreach (string line in Lines)
                {
                    foreach (string s in line.Split('\n'))
                        Words.Add(s);
                }
                string[] WordsArray = new string[Words.Count];
                Words.CopyTo(WordsArray);
                Random Rnd = new Random();
                int RandomLine = Rnd.Next(0, 12);
                string LineToConvert = WordsArray[RandomLine];
                string[] ConvertedString = LineToConvert.Split(' ');
                Character.Question = ConvertedString[0].Replace("~", " ");
                Character.Answer = ConvertedString[1];

            }
            else
            {
                Console.WriteLine("Questions.txt doesnt exist.");
            }
        }
        public bool RemoveItemID(uint ItemID, Main.GameClient GC)
        {
            foreach (Game.Item Item in GC.MyChar.Inventory.Values)
                if (Item.ID == ItemID)
                {
                    Database.DeleteItem(Item.UID, GC.MyChar);
                    GC.MyChar.Inventory.Remove(Item.UID);
                    GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(Item.UID, 0, 3));
                    return true;
                }
            return false;
        }
        public void RemoveItemI(uint ItemUID, byte Amount, Main.GameClient GC)
        {
            List<Game.Item> RemoveList = new List<Game.Item>(Amount);
            int Count = 0;
            foreach (Game.Item Item in GC.MyChar.Inventory.Values)
                if (Item.UID == ItemUID)
                {
                    if (Count < Amount)
                    {
                        RemoveList.Add(Item);
                        GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(Item.UID, 0, 3));
                        Database.DeleteItem(Item.UID, GC.MyChar);
                        Count++;
                    }
                    else
                        break;
                }
            foreach (Game.Item Item in RemoveList)
                GC.MyChar.Inventory.Remove(Item.UID);
        }
       /* public Item FindInvItem(uint UID)
        {
            foreach (Item I in Inventory.Values)
                if (I.UID == UID)
                    return I;
            return new Item();
        }*/
        public bool InventoryContains(uint ItemID, byte Amount, Main.GameClient GC)
        {
            int got = 0;
            foreach (Game.Item Item in GC.MyChar.Inventory.Values)
                if (Item.ID == ItemID)
                    got++;
            if (got >= Amount)
                return true;
            if (ItemID == 1088000)
                GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "I cant do it, I need my dragonball");
            if (ItemID == 1088001)
                GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "I cant do it, I need my meteor");
            return false;
        }
        public ushort MaxHP
        {
            get
            {
                double Rt = (double)(Vit * 24 + Str * 3 + Agi * 3 + Spi * 3);
                if (Job == 11)
                    Rt *= 1.05;
                if (Job == 12)
                    Rt *= 1.08;
                if (Job == 13)
                    Rt *= 1.1;
                if (Job == 14)
                    Rt *= 1.12;
                if (Job == 15)
                    Rt *= 1.15;
                Rt += EqStats.MaxHP;
                ushort Rtt = 0;
                if (Rt > ushort.MaxValue)
                    Rtt = ushort.MaxValue;
                else
                    Rtt = (ushort)Rt;
                return Rtt;
            }
        }
        public ushort MaxMP
        {
            get
            {
                ushort mp = 0;
                if (Job != 100 && Job != 101)
                    mp = (ushort)(Spi * 15);
                else
                    mp = (ushort)(Spi * 5);

                if (Job == 133 || Job == 143)
                    mp = (ushort)((double)mp * 4 / 3);
                if (Job == 134 || Job == 144)
                    mp = (ushort)((double)mp * 5 / 3);
                if (Job == 135 || Job == 145)
                    mp *= 2;

                return (ushort)(mp + EqStats.MaxMP);
            }
        }
        public bool WeaponSkill(ushort AX, ushort AY, uint T)
        {
            if (Equips.LeftHand.ID != 0 || Equips.RightHand.ID != 0)
            {
                bool WepSkill = MyMath.ChanceSuccess(60);
                if (WepSkill)
                {
                    ushort SkillID = 0;
                    ushort SkillID2 = 0;
                    #region SkillSELECTOR
                    if (Equips.RightHand != null)
                    {
                        if (Equips.LeftHand != null)
                        {
                            switch (Equips.RightHand.ID / 1000)
                            {
                                case 420:                       //Sword
                                case 421: SkillID = 5030; break;//Backsword
                                case 430: SkillID = 7000; break;//Hook
                                case 440: SkillID = 7040; break;//Whip
                                case 450: SkillID = 7010; break;//Axe
                                case 460: SkillID = 5040; break;//Hammer
                                case 480: SkillID = 7020; break;//Club
                                case 481: SkillID = 7030; break;//Scepter
                                case 490: SkillID = 1290; break;//Dagger
                                case 510: SkillID = 1250; break;//Glaive
                                case 540: SkillID = 1300; break;//LongHammer
                                case 560: SkillID = 1260; break;//Spear
                                case 580: SkillID = 5020; break;//Halberd
                                case 561: SkillID = 5010; break;//Wand
                                case 530: SkillID = 5050; break;//Poleaxe
                            }
                            switch (Equips.LeftHand.ID / 1000)
                            {
                                case 420:                       //Sword
                                case 421: SkillID2 = 5030; break;//Backsword
                                case 430: SkillID2 = 7000; break;//Hook
                                case 440: SkillID2 = 7040; break;//Whip
                                case 450: SkillID2 = 7010; break;//Axe
                                case 460: SkillID2 = 5040; break;//Hammer
                                case 480: SkillID2 = 7020; break;//Club
                                case 481: SkillID2 = 7030; break;//Scepter
                                case 490: SkillID2 = 1290; break;//Dagger
                                case 510: SkillID2 = 1250; break;//Glaive
                                case 540: SkillID2 = 1300; break;//LongHammer
                                case 560: SkillID2 = 1260; break;//Spear
                                case 580: SkillID2 = 5020; break;//Halberd
                                case 561: SkillID2 = 5010; break;//Wand
                                case 530: SkillID2 = 5050; break;//Poleaxe
                            }
                        }
                        else
                        {
                            switch (Equips.RightHand.ID / 1000)
                            {
                                case 420:                       //Sword
                                case 421: SkillID = 5030; break;//Backsword
                                case 430: SkillID = 7000; break;//Hook
                                case 440: SkillID = 7040; break;//Whip
                                case 450: SkillID = 7010; break;//Axe
                                case 460: SkillID = 5040; break;//Hammer
                                case 480: SkillID = 7020; break;//Club
                                case 481: SkillID = 7030; break;//Scepter
                                case 490: SkillID = 1290; break;//Dagger
                                case 510: SkillID = 1250; break;//Glaive
                                case 540: SkillID = 1300; break;//LongHammer
                                case 560: SkillID = 1260; break;//Spear
                                case 580: SkillID = 5020; break;//Halberd
                                case 561: SkillID = 5010; break;//Wand
                                case 530: SkillID = 5050; break;//Poleaxe
                            }
                        }
                    }
                    #endregion
                    Skill Skill1 = null;
                    try
                    {
                        if (Skills.Contains(SkillID))
                            Skill1 = (Skill)Skills[SkillID];
                    }
                    catch { }
                    Skill Skill2 = null;
                    try
                    {
                        if (Skills.Contains(SkillID2))
                            Skill2 = (Skill)Skills[SkillID2];
                    }
                    catch { }
                    if (Skill1 == null && Skill2 == null)
                        return false;
                    int Skill1Chance = 0;
                    int Skill2Chance = 0;
                    if (Skill1 != null)
                    {
                        switch (Skill1.ID)
                        {
                            #region Sword/Backsword
                            case 5030:
                                switch (Skill1.Lvl)
                                {
                                    case 0: Skill1Chance = 33;
                                        break;
                                    case 1: Skill1Chance = 38;
                                        break;
                                    case 2: Skill1Chance = 43;
                                        break;
                                    case 3: Skill1Chance = 48;
                                        break;
                                    case 4: Skill1Chance = 53;
                                        break;
                                    case 5: Skill1Chance = 58;
                                        break;
                                    case 6: Skill1Chance = 63;
                                        break;
                                    case 7: Skill1Chance = 68;
                                        break;
                                    case 8: Skill1Chance = 73;
                                        break;
                                    case 9: Skill1Chance = 83;
                                        break;
                                }
                                break;
                            #endregion
                            #region Rage | Wand | Spear | Glaive | Poleaxe | Halberd | Whip | Longhammer
                            case 7020:
                            case 5010:
                            case 1260:
                            case 1250:
                            case 5050:
                            case 5020:
                            case 7040:
                            case 1300:
                                switch (Skill1.Lvl)
                                {
                                    case 0: Skill1Chance = 20;
                                        break;
                                    case 1: Skill1Chance = 23;
                                        break;
                                    case 2: Skill1Chance = 26;
                                        break;
                                    case 3: Skill1Chance = 29;
                                        break;
                                    case 4: Skill1Chance = 31;
                                        break;
                                    case 5: Skill1Chance = 34;
                                        break;
                                    case 6: Skill1Chance = 37;
                                        break;
                                    case 7: Skill1Chance = 40;
                                        break;
                                    case 8: Skill1Chance = 13;
                                        break;
                                    case 9: Skill1Chance = 45;
                                        break;
                                }
                                break;
                            #endregion
                            #region Dagger
                            case 1290:
                            case 7030:
                                switch (Skill1.Lvl)
                                {
                                    case 0: Skill1Chance = 10;
                                        break;
                                    case 1: Skill1Chance = 10;
                                        break;
                                    case 2: Skill1Chance = 11;
                                        break;
                                    case 3: Skill1Chance = 11;
                                        break;
                                    case 4: Skill1Chance = 12;
                                        break;
                                    case 5: Skill1Chance = 12;
                                        break;
                                    case 6: Skill1Chance = 13;
                                        break;
                                    case 7: Skill1Chance = 13;
                                        break;
                                    case 8: Skill1Chance = 14;
                                        break;
                                    case 9: Skill1Chance = 15;
                                        break;
                                }
                                break;
                            #endregion
                            #region Hammer | Hook | Axe
                            case 5040:
                            case 7000:
                            case 7010:
                                switch (Skill1.Lvl)
                                {
                                    case 0: Skill1Chance = 10;
                                        break;
                                    case 1: Skill1Chance = 12;
                                        break;
                                    case 2: Skill1Chance = 14;
                                        break;
                                    case 3: Skill1Chance = 16;
                                        break;
                                    case 4: Skill1Chance = 18;
                                        break;
                                    case 5: Skill1Chance = 20;
                                        break;
                                    case 6: Skill1Chance = 22;
                                        break;
                                    case 7: Skill1Chance = 24;
                                        break;
                                    case 8: Skill1Chance = 26;
                                        break;
                                    case 9: Skill1Chance = 28;
                                        break;
                                }
                                break;
                            #endregion
                        }
                    }
                    if (Skill2 != null)
                    {
                        switch (Skill2.ID)
                        {
                            #region Sword/Backsword
                            case 5030:
                                switch (Skill2.Lvl)
                                {
                                    case 0: Skill2Chance = 33;
                                        break;
                                    case 1: Skill2Chance = 38;
                                        break;
                                    case 2: Skill2Chance = 43;
                                        break;
                                    case 3: Skill2Chance = 48;
                                        break;
                                    case 4: Skill2Chance = 53;
                                        break;
                                    case 5: Skill2Chance = 58;
                                        break;
                                    case 6: Skill2Chance = 63;
                                        break;
                                    case 7: Skill2Chance = 68;
                                        break;
                                    case 8: Skill2Chance = 73;
                                        break;
                                    case 9: Skill2Chance = 83;
                                        break;
                                }
                                break;
                            #endregion
                            #region Rage | Wand | Spear | Glaive | Poleaxe | Halberd | Whip | Longhammer
                            case 7020:
                            case 5010:
                            case 1260:
                            case 1250:
                            case 5050:
                            case 5020:
                            case 7040:
                            case 1300:
                                switch (Skill2.Lvl)
                                {
                                    case 0: Skill2Chance = 20;
                                        break;
                                    case 1: Skill2Chance = 23;
                                        break;
                                    case 2: Skill2Chance = 26;
                                        break;
                                    case 3: Skill2Chance = 29;
                                        break;
                                    case 4: Skill2Chance = 31;
                                        break;
                                    case 5: Skill2Chance = 34;
                                        break;
                                    case 6: Skill2Chance = 37;
                                        break;
                                    case 7: Skill2Chance = 40;
                                        break;
                                    case 8: Skill2Chance = 13;
                                        break;
                                    case 9: Skill2Chance = 45;
                                        break;
                                }
                                break;
                            #endregion
                            #region Dagger
                            case 1290:
                            case 7030:
                                switch (Skill2.Lvl)
                                {
                                    case 0: Skill2Chance = 10;
                                        break;
                                    case 1: Skill2Chance = 10;
                                        break;
                                    case 2: Skill2Chance = 11;
                                        break;
                                    case 3: Skill2Chance = 11;
                                        break;
                                    case 4: Skill2Chance = 12;
                                        break;
                                    case 5: Skill2Chance = 12;
                                        break;
                                    case 6: Skill2Chance = 13;
                                        break;
                                    case 7: Skill2Chance = 13;
                                        break;
                                    case 8: Skill2Chance = 14;
                                        break;
                                    case 9: Skill2Chance = 15;
                                        break;
                                }
                                break;
                            #endregion
                            #region Hammer | Hook | Axe
                            case 5040:
                            case 7000:
                            case 7010:
                                switch (Skill2.Lvl)
                                {
                                    case 0: Skill2Chance = 10;
                                        break;
                                    case 1: Skill2Chance = 12;
                                        break;
                                    case 2: Skill2Chance = 14;
                                        break;
                                    case 3: Skill2Chance = 16;
                                        break;
                                    case 4: Skill2Chance = 18;
                                        break;
                                    case 5: Skill2Chance = 20;
                                        break;
                                    case 6: Skill2Chance = 22;
                                        break;
                                    case 7: Skill2Chance = 24;
                                        break;
                                    case 8: Skill2Chance = 26;
                                        break;
                                    case 9: Skill2Chance = 28;
                                        break;
                                }
                                break;
                            #endregion
                        }
                    }
                    bool S1 = new Random().Next(0, 100) <= Skill1Chance;
                    bool S2 = new Random().Next(0, 100) <= Skill2Chance;
                    if (S1)
                    { if (Skill1 != null) { DoWeaponSkill(Skill1, T); return true; } }
                    else if (S2)
                    {
                        if (Skill2 != null)
                        {
                            DoWeaponSkill(Skill2, T);
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
        public void DoWeaponSkill(Skill Skill, uint Target)
        {
            if (Loc.Map == 1036)
                return;
            if (!Alive)
                return;
            if (EntityID == Target)
                return;

            ushort X = 0, Y = 0;
            Character CharacterTarget = null;
            if (World.H_Chars.ContainsKey(Target))
            { CharacterTarget = (Character)World.H_Chars[Target]; X = CharacterTarget.Loc.X; Y = CharacterTarget.Loc.Y; }
            Mob TargetMonster = null;
            foreach (Hashtable HMapMobs in World.H_Mobs.Values)
                if (HMapMobs.ContainsKey(Target))
                {
                    TargetMonster = (Mob)HMapMobs[Target]; X = TargetMonster.Loc.X; Y = TargetMonster.Loc.Y;
                }
            NPC TargetNpc = null; if (World.H_NPCs.Contains(Target))
            { TargetNpc = (NPC)World.H_NPCs[Target]; X = TargetNpc.Loc.X; Y = TargetNpc.Loc.Y; }

            if (Target == GuildWars.ThePole.EntityID)
            {
                X = GuildWars.ThePole.Loc.X;
                Y = GuildWars.ThePole.Loc.Y;
            }
            if (Target == GuildWars.TheLeftGate.EntityID)
            {
                X = GuildWars.TheLeftGate.Loc.X;
                Y = GuildWars.TheLeftGate.Loc.Y;
            }
            if (Target == GuildWars.TheRightGate.EntityID)
            {
                X = GuildWars.TheRightGate.Loc.X;
                Y = GuildWars.TheRightGate.Loc.Y;
            }
            Features.SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
            SU.Init(this, Skill.ID, Skill.Lvl, X, Y);
            if (TargetMonster != null)
                if (StatEff.Contains(Game.StatusEffectEn.FatalStrike))
                {
                    Shift((ushort)(X), (ushort)(Y));
                }
            switch (Skill.ID)
            {
                #region WeaponSkill
                #region MultipleTargets
                case 7020:
                    {
                        ushort Dist = 2;
                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                        if (MapMobs != null)
                            foreach (Mob M in MapMobs.Values)
                            {
                                if (M.Alive)
                                {
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                        if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                            SU.MobTargets.Add(M, SU.GetDamage(M));
                                }
                            }
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                        if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                            if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !World.NoPKMaps.Contains(Loc.Map))
                                                SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (NPC C in World.H_NPCs.Values)
                        {
                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                            {
                                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                    if (!SU.NPCTargets.Contains(C))
                                        SU.NPCTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Dist && GuildWars.War && MyGuild != GuildWars.LastWinner && MyGuild != null)
                            SU.MiscTargets.Add(GuildWars.ThePole.EntityID, SU.GetDamage(GuildWars.ThePole.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Dist)
                            SU.MiscTargets.Add(GuildWars.TheRightGate.EntityID, SU.GetDamage(GuildWars.TheRightGate.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Dist)
                            SU.MiscTargets.Add(GuildWars.TheLeftGate.EntityID, SU.GetDamage(GuildWars.TheLeftGate.CurHP));
                    }
                    break;
                case 5010:
                    {
                        ushort Dist = 2;
                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                        if (MapMobs != null)
                            foreach (Mob M in MapMobs.Values)
                            {
                                if (M.Alive)
                                {
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                        if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                            SU.MobTargets.Add(M, SU.GetDamage(M));
                                }
                            }
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist) if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                            if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !World.NoPKMaps.Contains(Loc.Map))
                                                SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (NPC C in World.H_NPCs.Values)
                        {
                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                            {
                                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                    if (!SU.NPCTargets.Contains(C))
                                        SU.NPCTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Dist && GuildWars.War && MyGuild != GuildWars.LastWinner && MyGuild != null)
                            SU.MiscTargets.Add(GuildWars.ThePole.EntityID, SU.GetDamage(GuildWars.ThePole.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Dist)
                            SU.MiscTargets.Add(GuildWars.TheRightGate.EntityID, SU.GetDamage(GuildWars.TheRightGate.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Dist)
                            SU.MiscTargets.Add(GuildWars.TheLeftGate.EntityID, SU.GetDamage(GuildWars.TheLeftGate.CurHP));

                    }
                    break;
                #endregion
                #region SpeedGun
                case 1260:
                    {
                        ushort Dist = 5;
                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                        if (MapMobs != null)
                            foreach (Mob M in MapMobs.Values)
                            {
                                if (M.Alive)
                                {
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                        if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y))
                                            if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                                SU.MobTargets.Add(M, SU.GetDamage(M));
                                }
                            }
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist) if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                            if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y))
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !World.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (NPC C in World.H_NPCs.Values)
                        {
                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                            {
                                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                    if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y))
                                        if (!SU.NPCTargets.Contains(C))
                                            SU.NPCTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Dist && GuildWars.War && MyGuild != GuildWars.LastWinner && MyGuild != null)
                            if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.ThePole.EntityID, SU.GetDamage(GuildWars.ThePole.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Dist)
                            if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.TheRightGate.EntityID, SU.GetDamage(GuildWars.TheRightGate.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Dist)
                            if (MyMath.PointDirecton(Loc.X, Loc.Y, X, Y) == MyMath.PointDirecton(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.TheLeftGate.EntityID, SU.GetDamage(GuildWars.TheLeftGate.CurHP));
                        break;
                    }
                #endregion
                #region Single Target
                case 5030:
                case 1290:
                case 5040:
                case 7000:
                case 7010:
                case 7030:
                    {
                        #region Phoenix, Penetration, Boom,Seizer,Earthquake,Celestial
                        ushort Dist = 2;
                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                        if (MapMobs != null)
                            foreach (Mob M in MapMobs.Values)
                            {
                                if (M.Alive)
                                {
                                    if (M.EntityID == Target)
                                        if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                            if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                                SU.MobTargets.Add(M, SU.GetDamage(M));
                                }
                            }
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (C.EntityID == Target)
                                        if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist) if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !World.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (NPC C in World.H_NPCs.Values)
                        {
                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                            {
                                if (C.EntityID == Target)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                        if (!SU.NPCTargets.Contains(C))
                                            SU.NPCTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Dist && GuildWars.War && MyGuild != GuildWars.LastWinner && MyGuild != null)
                            if (GuildWars.ThePole.EntityID == Target)
                                SU.MiscTargets.Add(GuildWars.ThePole.EntityID, SU.GetDamage(GuildWars.ThePole.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Dist)
                            if (GuildWars.TheRightGate.EntityID == Target)
                                SU.MiscTargets.Add(GuildWars.TheRightGate.EntityID, SU.GetDamage(GuildWars.TheRightGate.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Dist)
                            if (GuildWars.TheLeftGate.EntityID == Target)
                                SU.MiscTargets.Add(GuildWars.TheLeftGate.EntityID, SU.GetDamage(GuildWars.TheLeftGate.CurHP));

                        break;
                        #endregion
                    }
                #endregion
                #region FrontSpell
                case 1250:
                case 5050:
                case 5020:
                case 1300:
                case 7040:
                    {
                        ushort Dist = 3;
                        Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                        if (MapMobs != null)
                            foreach (Mob M in MapMobs.Values)
                            {
                                if (M.Alive)
                                {
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                        if (SU.InSector(M.Loc.X, M.Loc.Y))
                                            if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                                SU.MobTargets.Add(M, SU.GetDamage(M));
                                }
                            }
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                        if (SU.InSector(C.Loc.X, C.Loc.Y)) if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !World.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (NPC C in World.H_NPCs.Values)
                        {
                            if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                            {
                                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                    if (SU.InSector(C.Loc.X, C.Loc.Y))
                                        if (!SU.NPCTargets.Contains(C))
                                            SU.NPCTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Dist && GuildWars.War && MyGuild != GuildWars.LastWinner && MyGuild != null)
                            if (SU.InSector(GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.ThePole.EntityID, SU.GetDamage(GuildWars.ThePole.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Dist)
                            if (SU.InSector(GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.TheRightGate.EntityID, SU.GetDamage(GuildWars.TheRightGate.CurHP));

                        if (MyMath.PointDistance(Loc.X, Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Dist)
                            if (SU.InSector(GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y))
                                SU.MiscTargets.Add(GuildWars.TheLeftGate.EntityID, SU.GetDamage(GuildWars.TheLeftGate.CurHP));

                        break;
                    }
                #endregion
                #endregion
            }
            SU.Use();
        }
        public void Shift(ushort X, ushort Y)
        {
            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(X, Y, Loc.X, Loc.Y) / 45 % 8)) - 1 % 8);
            byte Dir = (byte)((int)ToDir % 8);
            if (Dir == 0)
                Y += 1;
            if (Dir == 2)
                X -= 1;
            if (Dir == 4)
                Y -= 1;
            if (Dir == 6)
                X += 1;
            if (Dir == 1)
            {
                X -= 1;
                Y += 1;
            }
            if (Dir == 3)
            {
                X -= 1;
                Y -= 1;
            }
            if (Dir == 5)
            {
                X += 1;
                Y -= 1;
            }
            if (Dir == 7)
            {
                X += 1;
                Y += 1;
            }
            if (MyMath.PointDistance(X, Y, Loc.X, Loc.Y) < 20)
            {
                World.Action(this, Packets.GeneralData(EntityID, 0, X, Y, 0x9c));
                Loc.X = X;
                Loc.Y = Y;
                Direction = Dir;

                World.Spawns(this, true);
            }
        }

        public void IncreaseExp(uint Amount, bool isTeamExp)
        {
            if (Loc.Map == 1039)
            {
                if (Amount > Level / 2)
                    Amount /= (uint)(Level / 2);
            }
            if (Level < 137)
            {
                Amount = (uint)(Amount * EqStats.GemExtraExp * World.Server.ExperienceRate);
                Amount = (uint)(Amount * (EqStats.eq_pot + Level + 100 + Reborns * 5) / 100);
                if (World.KOBoard[0].Name == Name) Amount *= 2;
                if (DoubleExp) Amount *= 2;
                if (BlessingLasts > 0) Amount = (uint)((double)Amount * 1.2);
                if (Reborns >= 2)
                    Amount /= 3;
                if (MyGuild != null)
                    Amount = (Amount * (100 + MyGuild.Wins)) / 100;

                bool noobexp = false;
                bool marrieexp = false;

                #region checkmyteam for extras
                if (MyTeam != null && isTeamExp)
                {
                    foreach (Character C in MyTeam.Members)
                    {
                        if (C != null)
                        {
                            if (EntityID != C.EntityID)
                            {
                                if (Level > C.Level + 20)
                                    noobexp = true;

                                if (C.Spouse == C.Name)
                                    marrieexp = true;
                            }
                        }
                    }
                }
                #endregion

                if (noobexp)
                    Amount *= 2;
                if (marrieexp)
                    Amount *= 2;

                #region Experience points Messages
                if (!noobexp && !marrieexp && !isTeamExp && Loc.Map != 1039)
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , Amount + " MORE!, experiance from killing monsters :P.");
                else if (!noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , Amount + " team experience points gained.");
                else if (noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You gained  " + Amount + " team experience points with additional rewarding experience points due to low level teammates.");
                else if (!noobexp && marrieexp && isTeamExp && Loc.Map != 1039)
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You gained  " + Amount + " team experience points with additional rewarding experience points due to marriage teammates.");
                else if (noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You gained  " + Amount + " team experience points with additional rewarding experience points due to low level and marriage teammates.");
                #endregion

                ulong CurExp = Experience;
                byte CurLevel = Level;

                CurExp += Amount;
                while (CurLevel < 135 && CurExp > Database.LevelExp[CurLevel])
                {
                    CurExp -= Database.LevelExp[CurLevel];
                    CurLevel++;


                    if (CurLevel >= 3)
                    {
                        if (Job <= 55 && Job >= 50)
                            NewSkill(new Skill() { ID = 6011 });
                        else if (Job <= 45 && Job >= 40)
                            NewSkill(new Skill() { ID = 8002 });
                        else if (Job >= 10 && Job <= 15)
                            NewSkill(new Skill() { ID = 1110 });
                        else if (Job >= 20 && Job <= 25)
                            NewSkill(new Skill() { ID = 1025 });
                    }
                }
                if (CurLevel > Level)
                {
                    World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 92));
                    if (Reborn || CurLevel >= 120)
                    {
                        if (Reborn)
                            StatPoints += (ushort)((CurLevel - Level) * 3);
                        else
                        {
                            if (Level < 120)
                                Level = 120;
                            StatPoints += (ushort)((CurLevel - Level) * 3);
                        }
                    }
                    if (Level >= 125)
                    {
                        Game.World.SendMsgToAll(" " + Name + " say to all", "CONGRATULATIONS! " + Name + " has just achieved level " + Level + "! Great job!", 2011, 0, System.Drawing.Color.Red);
                        //CONGRATULATIONS! " + Client.Char.Name + " has just achieved level 130! Great job!
                    }
                    CurHP = MaxHP;
                }
                if (CurLevel > Level)
                    Level = CurLevel;
                if (Level >= 130)
                    if (oldlevel > Level)
                        Level = oldlevel;
                Experience = CurExp;
            }
        }

        public void AddProfExp(ushort Wep, uint Amount)
        {
            if (Profs.Contains(Wep))
            {
                Prof P = (Prof)Profs[Wep];
                if (P.Lvl < 20)
                {
                    Profs.Remove(Wep);
                    Amount *= World.Server.ProfExpRate;
                    P.Exp += Amount;
                    if (P.Exp >= Database.ProfExp[P.Lvl])
                    {
                        P.Lvl++;
                        P.Exp = 0;
                        MyClient.LocalMessage(2000,System.Drawing.Color.Yellow ,"Your proficiency level has gone up!.");
                        Database.SaveProfs(P, this);
                    }
                    Profs.Add(Wep, P);
                    MyClient.SendPacket(Packets.Prof(P));
                }
            }
            else
            {
                Prof P = new Prof();
                P.ID = Wep;
                P.Lvl = 0;
                P.Exp = 0;
                NewProf(P);
            }
        }
        public void NewProf(Prof P)
        {
            if (!Profs.Contains(P.ID))
            {
                Profs.Add(P.ID, P);
                MyClient.SendPacket(Packets.Prof(P));
                Database.AddProfs(P.ID, P.Lvl, P.Exp, this);
            }
        }
        public void RWProf(Prof P)
        {
            if (Profs.Contains(P.ID))
            {
                Profs.Remove(P.ID);
                Database.DelProfs(P, this);
            }
            Profs.Add(P.ID, P);
            Database.AddProfs(P.ID, P.Lvl, P.Exp, this);
            MyClient.SendPacket(Packets.Prof(P));
        }
        public void NewSkill(Skill S)
        {
            if (!Skills.Contains(S.ID))
            {
                Skills.Add(S.ID, S);
                MyClient.SendPacket(Packets.Skill(S));
                Database.AddSkills(S.ID, S.Lvl, S.Exp, this);
            }
            if (S.Lvl >= 1)
                RWSkill(S);
        }
        public void RWSkill(Skill S)
        {
            if (Skills.Contains(S.ID))
            {
                Skills.Remove(S.ID);
                Database.DelSkills(S, this);
            }
            Skills.Add(S.ID, S);
            MyClient.SendPacket(Packets.Skill(S));
            Database.AddSkills(S.ID, S.Lvl, S.Exp, this);
        }
        public void InitAngry(bool _kind ,Game.Character C)
        {
            if (VipLevel < 3)
            {
                Random _rand = new Random();
                #region Silver drop
                int DropSilver = _rand.Next(0, (int)Silvers);
                if (DropSilver > 1000)
                    DropSilver /= 10;
                if (Loc.Map == 1038 || Loc.Map == 1005 || Loc.Map == 6000 || CurHP > 1) { return; }
                DroppedItem DI2 = new DroppedItem();
                DI2.DropTime = DateTime.Now;
                DI2.UID = (uint)Rnd.Next(10000000);
                DI2.Loc = new Location();
                DI2.Loc = Loc;
                DI2.Loc.Map = Loc.Map;
                DI2.Info = new Item();
                DI2.Info.UID = (uint)Rnd.Next(10000000);
                DI2.UID = (uint)Rnd.Next(10000000);
                DI2.Silvers = (uint)DropSilver;
                if (DI2.Silvers < 10)
                    DI2.Info.ID = 1090000;
                else if (DI2.Silvers < 100)
                    DI2.Info.ID = 1090010;
                else if (DI2.Silvers < 1000)
                    DI2.Info.ID = 1090020;
                else if (DI2.Silvers < 3000)
                    DI2.Info.ID = 1091000;
                else if (DI2.Silvers < 10000)
                    DI2.Info.ID = 1091010;
                else
                    DI2.Info.ID = 1091020;
                if (DI2.FindPlace((Hashtable)Game.World.H_Items[Loc.Map]))
                { DI2.Drop(); Silvers -= (uint)DropSilver; }
                #endregion
                #region Item drop
                byte _val1 = (byte)_rand.Next(Inventory.Count);
                List<Item> _list1 = new List<Item>();
                for (byte _val2 = 0; _val2 < _val1; _val2++)
                {
                    if (Loc.Map == 1038 || Loc.Map == 1005 || Loc.Map == 6000 || CurHP > 1) { return; }
                    byte _val3 = (byte)_rand.Next(Inventory.Count);
                    Item _item = Inventory[_val3] as Item;
                    if (_item.FreeItem || _item.Locked == 1)
                        //  if (_item.FreeItem || _item.Locked)
                        continue;
                    if (_item.ID == 300000)
                    {
                        return;
                    }
                    if (Merchant == MerchantTypes.Yes)
                    {
                        Game.DroppedItem DI = new NewestCOServer.Game.DroppedItem();
                        DI.Info = _item;
                        DI.DropTime = DateTime.Now;
                        DI.Loc = Loc;
                        DI.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                        DI.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                        DI.UID = (uint)Program.Rnd.Next(10000000);
                        if (!DI.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) continue;
                        DI.Drop();
                        //   RemoveItem(_item.UID, 1, MyClient);
                        Inventory.Remove(_item.UID);
                        MyClient.SendPacket(Packets.ItemPacket(_item.UID, 0, 3));
                        Database.DeleteItem(_item.UID,this);
                    }
                    else
                    {
                        if (_item.Pot <= 8)
                        {
                            Game.DroppedItem DI = new NewestCOServer.Game.DroppedItem();
                            DI.Info = _item;
                            DI.DropTime = DateTime.Now;
                            DI.Loc = Loc;
                            DI.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                            DI.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                            DI.UID = (uint)Program.Rnd.Next(10000000);
                            if (!DI.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) continue;
                            DI.Drop();
                            Inventory.Remove(_item.UID);
                            MyClient.SendPacket(Packets.ItemPacket(_item.UID, 0, 3));
                            Database.DeleteItem(_item.UID,this);
                        }
                    }
                }
                #endregion
                #region Equipment drop
                if (_kind)
                {
                    if (PKPoints > 99)
                    {

                        if (Loc.Map == 1038 || Loc.Map == 1005 || Loc.Map == 6000 || CurHP > 1) { return; }
                        _val1 = 0;
                        Item[] _equipment = Equips;
                        foreach (Item _equip in _equipment)
                        {
                            if (_equip.ID == 300000)
                            { return; }
                            if (_equip.FreeItem || _equip.Locked == 1 || _equip.Locked == 2)
                            { return; }
                            if (_val1 == 2)
                                return;
                            if (_equip.ID == 0)
                                continue;
                            if (_equip.FreeItem)
                                continue;
                            if (MyMath.ChanceSuccess(10))
                            {
                                EquipStats(Equips.GetSlot(_equip.UID), false);
                                Equips.UnEquip(Equips.GetSlot(_equip.UID), this);
                                MyClient.SendPacket(Packets.ItemPacket(_equip.UID, 0, 3));
                                Game.DroppedItemConfiscator DI = new NewestCOServer.Game.DroppedItemConfiscator();
                                DI.Info = _equip;
                                DI.DropTime = DateTime.Now;
                                DI.Loc = Loc;
                                DI.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                                DI.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                                DI.UID = (uint)Program.Rnd.Next(10000000);
                                if (!DI.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) continue;
                                Database.SaveCharacter(this, MyClient.AuthInfo.Account);
                                return;
                            }
                        }
                    }
                }
                #endregion
            }

        }
        #region MONSTER ATAKER
        public void TakeAttack(Mob Attacker, uint Damage, AttackType AT)
        {
            if (AtkMem.Target == 0)
            {
                AtkMem.Target = Attacker.EntityID;
            }
            if (Damage != 0)
            {
                Extra.BlessEffect.Handler(MyClient);
                //Extra.Durability.DefenceDurability(MyClient);
                if (Protection) Damage = 0;
                if (BuffOf(SkillsClass.ExtraEffect.Scapegoat).Eff == SkillsClass.ExtraEffect.Scapegoat && MyMath.ChanceSuccess(30))
                {
                    Buff B = BuffOf(SkillsClass.ExtraEffect.Scapegoat);
                    //RemoveBuff(B);
                    uint Dmg = (uint)(PrepareAttack(2, false) * B.Value);
                    Attacker.TakeAttack(this, ref Dmg, AttackType.Scapegoat, false);
                    return;//Will not be damaged
                }
                if (CanReflect)
                {
                    if (MyMath.ChanceSuccess(20))
                    {
                        Attacker.GetReflect(ref Damage, AT);

                        MyClient.AddSend(Packets.String(EntityID, 10, "MagicReflect"));
                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                        {
                            Game.Character Chaar = (Game.Character)DE.Value;
                            if (Chaar.Name != MyClient.MyChar.Name)
                            {

                                Chaar.MyClient.AddSend(Packets.String(EntityID, 10, "MagicReflect"));

                            }
                        }
                        if (Protection) Damage = 0;
                        return;
                    }
                }
                if (AT == AttackType.Melee)
                {

                    ushort Def = EqStats.defense;
                    Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                    if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                        Def = (ushort)(Def * Shield.Value);

                    if (Def >= Damage)
                        Damage = 1;
                    else
                        Damage -= Def;
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.MeleeDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.MeleeDamageDecrease;
                }
                else if (AT == AttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * (((double)(110 - EqStats.Dodge) / 550 / 5200)));
                    if (EqStats.Dodge >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.Dodge;
                    Damage *= 2 / 3;
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.MeleeDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.MeleeDamageDecrease;
                }
                else
                {
                    if (Job >= 130 && Job <= 145)
                        Damage = (uint)(Damage * 4);
                    else
                        Damage = (uint)(Damage * 2);
                    if (EqStats.MagicDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.MagicDamageDecrease;

                    Damage = (uint)((double)Damage * (((double)(100 - EqStats.MDef1) / 100)));
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.MDef2 >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.MDef2;
                }
            }
            if (AT != AttackType.Magic && Action == 250)
            {
                if (Stamina > 30)
                    Stamina -= 20;
                else
                    Stamina = 0;
            }
            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                Damage = 1;
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;
                if (AT == AttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.MagicSkill, Attacker.MagicLvl, Loc.X, Loc.Y).Get);
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT).Get);
            }
            else
            {
                //InitAngry(false);
                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                if (!World.FreePKMaps.Contains(Loc.Map))
                    LoseInvItemsAndSilvers();
                Alive = false;
                CurHP = 0;

                if (AT == AttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.MagicSkill, Attacker.MagicLvl, Loc.X, Loc.Y).Get);
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT).Get);
                World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AttackType.Kill).Get);
                StatEff.Remove(StatusEffectEn.Poisoned);
                PoisonedInfo = null;
                VortexOn = false;
                StatEff.Remove(StatusEffectEn.Cyclone);
                StatEff.Remove(StatusEffectEn.FatalStrike);
                StatEff.Remove(StatusEffectEn.BlueName);
                //StatEff.Remove(StatusEffectEn.Flashy);
                StatEff.Remove(StatusEffectEn.Fly);
                StatEff.Remove(StatusEffectEn.ShurikenVortex);
                BlueName = false;
                StatEff.Remove(StatusEffectEn.SuperMan);
                StatEff.Remove(StatusEffectEn.XPStart);
                StatEff.Remove(StatusEffectEn.Ride);
                StatEff.Remove(StatusEffectEn.DragonCyclone);
                StatEff.Add(StatusEffectEn.Dead);
                DeathHit = DateTime.Now;
                if (MyCompanion != null)
                    MyCompanion.Dissappear();
                if (MyCompanion1 != null)
                    MyCompanion1.Dissappear();
                if (MyCompanion2 != null)
                    MyCompanion2.Dissappear();
                if (MyCompanion3 != null)
                    MyCompanion3.Dissappear();
                if (MyCompanion4 != null)
                    MyCompanion4.Dissappear();
                if (MyCompanion5 != null)
                    MyCompanion5.Dissappear();
                if (MyCompanion6 != null)
                    MyCompanion6.Dissappear();
                if (MyCompanion7 != null)
                    MyCompanion7.Dissappear();
                if (MyCompanion8 != null)
                    MyCompanion8.Dissappear();
                if (MyCompanion9 != null)
                    MyCompanion9.Dissappear();
                if (MyCompanion10 != null)
                    MyCompanion10.Dissappear();
                if (MyCompanion11 != null)
                    MyCompanion11.Dissappear();
                World.Spawn(this, false);
            }
        }
        #endregion
        #region GUARDS ATACKERS
        public void TakeAttack(Companion Attacker, uint Damage, AttackType AT)
        {
            if (World.NoPKMaps.Contains(Attacker.Loc.Map))
                return;
            if (Damage != 0)
            {
                Extra.Durability.DefenceDurability(MyClient);
                if (!BlueName && PKPoints < 100 && !World.FreePKMaps.Contains(Loc.Map))
                {
                    Attacker.Owner.BlueName = true;
                    if (Attacker.Owner.BlueNameLasts < 15)
                        Attacker.Owner.BlueNameLasts = 15;
                }
                if (Protection) Damage = 0;
                if (AT == AttackType.Melee)
                {
                    ushort Def = EqStats.defense;
                    Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                    if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                        Def = (ushort)(Def * Shield.Value);

                    if (Def >= Damage)
                        Damage = 1;
                    else
                        Damage -= Def;

                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                  //  if (EqStats.TowerMeleeDamageDecrease >= Damage)
                    //    Damage = 1;
                  //  else
                    //    Damage -= EqStats.TowerMeleeDamageDecrease;
                }
                else if (AT == AttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * (((double)(110 - EqStats.Dodge) / 100)));
                    if (EqStats.Dodge >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.Dodge;
                    Damage *= 2 / 3;

                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                   // if (EqStats.TowerMeleeDamageDecrease >= Damage)
                     //   Damage = 1;
                   // else
                       // Damage -= EqStats.TowerMeleeDamageDecrease;
                }
                else
                {
                   // if (EqStats.TowerMagicMeleeDamageIncrease >= Damage)
                  //      Damage = 1;
                  //  else
                    //    Damage -= EqStats.TowerMagicMeleeDamageIncrease;

                    Damage = (uint)((double)Damage * (((double)(100 - EqStats.MDef1) / 100)));
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.MDef2 >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.MDef2;
                }
            }
            if (AT != AttackType.Magic && Action == 250)
            {
                if (Stamina > 30)
                    Stamina -= 30;
                else
                    Stamina = 0;
            }
            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                Damage = 1;
            Action = 100;
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;
                if (AT == AttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
            }
            else
            {
                InitAngry(false,this);
                if (!World.FreePKMaps.Contains(Loc.Map))
                    Attacker.Owner.PKPoints += 10;

                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                if (!World.FreePKMaps.Contains(Loc.Map))
                    LoseInvItemsAndSilvers();
                Alive = false;
                CurHP = 0;

                if (AT == AttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AttackType.Kill));

                StatEff.Remove(StatusEffectEn.Cyclone);
                StatEff.Remove(StatusEffectEn.FatalStrike);
                StatEff.Remove(StatusEffectEn.BlueName);
                StatEff.Remove(StatusEffectEn.Flashy);
                StatEff.Remove(StatusEffectEn.ShurikenVortex);
                BlueName = false;
                StatEff.Remove(StatusEffectEn.SuperMan);
                StatEff.Remove(StatusEffectEn.XPStart);
                StatEff.Remove(StatusEffectEn.Ride);
                StatEff.Add(StatusEffectEn.Dead);
                DeathHit = DateTime.Now;
            }
        }
        #endregion
        #region Players ATACKERS
        public void GetReflect(ref uint Damage, AttackType AT)
        {
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;

                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
            }
            else
            {
                InitAngry(false,this);
                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                AtkMem.Attacking = false;
                AtkMem.Target = 0;

                if (!World.FreePKMaps.Contains(Loc.Map))
                {
                    LoseInvItemsAndSilvers();
                    //if (PKPoints >= 30)
                      //  LoseEquips();
                }
                Alive = false;
                CurHP = 0;

                //    World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AttackType.Kill));

                StatEff.Remove(StatusEffectEn.Cyclone);
                StatEff.Remove(StatusEffectEn.FatalStrike);
                StatEff.Remove(StatusEffectEn.BlueName);
                StatEff.Remove(StatusEffectEn.Flashy);
                StatEff.Remove(StatusEffectEn.ShurikenVortex);
                BlueName = false;
                StatEff.Remove(StatusEffectEn.SuperMan);
                StatEff.Remove(StatusEffectEn.XPStart);
                StatEff.Remove(StatusEffectEn.Ride);
                StatEff.Add(StatusEffectEn.Dead);
                DeathHit = DateTime.Now;
            }
        }
        public void TakeAttack(Character Attacker, uint Damage, AttackType AT, bool IsSkill)
        {
            
            if (Attacker.PKMode == PKMode.Peace)
                return;
            if (World.NoPKMaps.Contains(Attacker.Loc.Map))
                return;
            if ((Alive)&& this.PKAble(Attacker.PKMode,Attacker))
            {
                Extra.BlessEffect.Handler(MyClient);
                Extra.Durability.DefenceDurability(MyClient);
                if (Protection) Damage = 0;
                if (AT == AttackType.Melee && !IsSkill)
                    if (AT == AttackType.Melee || AT == AttackType.Ranged || AT == AttackType.FatalStrike || AT == AttackType.Magic && IsSkill)
                        if (BuffOf(SkillsClass.ExtraEffect.Fly).Eff == SkillsClass.ExtraEffect.Fly)
                            return;
                #region CounterKill
                if (BuffOf(SkillsClass.ExtraEffect.Scapegoat).Eff == SkillsClass.ExtraEffect.Scapegoat && MyMath.ChanceSuccess(4))
                {
                    Buff B = BuffOf(SkillsClass.ExtraEffect.Scapegoat);
                    uint Dmg = (uint)(PrepareAttack(2, false) * B.Value);
                    Attacker.TakeAttack(this, Dmg, AttackType.Scapegoat, false);
                    MyClient.SendPacket(Packets.AttackPacket(EntityID, Attacker.EntityID, Attacker.Loc.X, Attacker.Loc.Y, 0, 44));
                    Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 260);
                }
                #endregion
                if(Attacker.Loc.Map == 1090)
                    if (Attacker.MyTDmTeam.TeamName == MyTDmTeam.TeamName)
                    {
                        Damage = 0;
                        return;
                    }

                if (CanReflect)
                {
                    if (MyMath.ChanceSuccess(4))
                    {
                        Attacker.GetReflect(ref Damage, AT);
                        MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));
                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                        {
                            Game.Character Chaar = (Game.Character)DE.Value;
                            if (Chaar.Name != MyClient.MyChar.Name)
                            {
                                Chaar.MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));
                                Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 260);
                            }
                        }
                       // Damage = 0;
                        return;
                    }
                }
                Console.WriteLine(Attacker.Name);
                if (BuffOf(SkillsClass.ExtraEffect.Scapegoat).Eff == SkillsClass.ExtraEffect.Scapegoat && MyMath.ChanceSuccess(60))
                {
                    Buff B = BuffOf(SkillsClass.ExtraEffect.Scapegoat);
                    RemoveBuff(B);
                    uint Dmg = (uint)(PrepareAttack(2, false) * B.Value);
                    Attacker.TakeAttack(this, Dmg, AttackType.Scapegoat, false);
                    //World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 44));
                    return;//Will not be damaged
                }
                if (World.NoPKMaps.Contains(Loc.Map))
                    Damage = 0;
                if (this != Attacker)
                {
                    if (!BlueName && PKPoints < 30 && !World.FreePKMaps.Contains(Loc.Map))
                    {
                        Attacker.BlueName = true;
                        if (Attacker.BlueNameLasts < 15)
                            Attacker.BlueNameLasts = 15;
                    }
                }
                if (AT != AttackType.Magic && Attacker.BuffOf(SkillsClass.ExtraEffect.Superman).Eff == SkillsClass.ExtraEffect.Superman)
                    Damage *= 2;
                if (AT != AttackType.Magic && !IsSkill)
                {
                    ushort _Agi = (ushort)(Attacker.Agi + Attacker.EqStats.ExtraDex);

                    Buff Accuracy = Attacker.BuffOf(SkillsClass.ExtraEffect.Accuracy);
                    if (Accuracy.Eff == SkillsClass.ExtraEffect.Accuracy)
                        _Agi = (ushort)(_Agi * Accuracy.Value);

                    double MissValue = Rnd.Next(_Agi - 25, _Agi + 25);
                    if (MissValue <= EqStats.Dodge)
                        Damage = 0;
                }
                if (AT != AttackType.Magic && Action == 250)
                {
                    if (Stamina > 30)
                        Stamina -= 30;
                    else
                        Stamina = 0;
                }
                Action = 100;
                if (Damage != 0 && !IsSkill)
                {
                    if (AT == AttackType.Melee)
                    {
                        ushort Def = EqStats.defense;
                        Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                        if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                            Def = (ushort)(Def * Shield.Value);

                        if (Def >= Damage)
                            Damage = 1;
                        else
                            Damage -= Def;
                        if ((this.Job >= 130 && this.Job <= 145) || (this.Job >= 40 && this.Job <= 45))
                        {

                            if (Attacker.Potency < (this.Potency + 20))
                            {
                                if ((Attacker.Potency + 2) < (this.Potency + 20))
                                {
                                    if ((Attacker.Potency + 4) < (this.Potency + 20))
                                    {
                                        if ((Attacker.Potency + 6) < (this.Potency + 20))
                                        {
                                            if ((Attacker.Potency + 8) < (this.Potency + 20))
                                            {
                                                if ((Attacker.Potency + 10) < (this.Potency + 20))
                                                {
                                                    if ((Attacker.Potency + 15) < (this.Potency + 20))
                                                    {
                                                        if ((Attacker.Potency + 20) < (this.Potency + 20))
                                                        {
                                                            if ((Attacker.Potency + 25) < (this.Potency + 20))
                                                            {
                                                                if ((Attacker.Potency + 30) < (this.Potency + 20))
                                                                {
                                                                    Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 470);
                                                                }
                                                                else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 435);
                                                            }
                                                            else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 355);
                                                        }
                                                        else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 345);
                                                    }
                                                    else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 335);
                                                }
                                                else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 325);
                                            }
                                            else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 315);
                                        }
                                        else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 305);
                                    }
                                    else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 267);
                                }
                                else Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 244);
                            }

                            else if (Attacker.Potency == (this.Potency + 20))
                            {
                                Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 160);
                            }
                            else
                                Damage = (uint)((double)Damage * (100 - (this.EqStats.TotalBless + 7)) / 80);

                        }
                        else if (Attacker.Potency > this.Potency)
                        {
                            if (Attacker.Potency > (this.Potency + 2))
                            {
                                if (Attacker.Potency > (this.Potency + 4))
                                {
                                    if (Attacker.Potency > (this.Potency + 6))
                                    {
                                        if (Attacker.Potency > (this.Potency + 8))
                                        {
                                            if (Attacker.Potency > (this.Potency + 10))
                                            {
                                                if (Attacker.Potency > (this.Potency + 15))
                                                {
                                                    if (Attacker.Potency > (this.Potency + 20))
                                                    {
                                                        if (Attacker.Potency > (this.Potency + 25))
                                                        {
                                                            if (Attacker.Potency > (this.Potency + 30))
                                                            {
                                                                if (Attacker.Potency > (this.Potency + 35))
                                                                {
                                                                    if (Attacker.Potency > (this.Potency + 40))
                                                                    {
                                                                        if (Attacker.Potency > (this.Potency + 45))
                                                                        {
                                                                            Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 69);
                                                                        }
                                                                        else
                                                                            Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 70);
                                                                    }
                                                                    else
                                                                        Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 74);
                                                                }
                                                                else
                                                                    Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 75);
                                                            }
                                                            else
                                                                Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 76);
                                                        }
                                                        else
                                                            Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 77);
                                                    }
                                                    else
                                                        Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 78);
                                                }
                                                else
                                                    Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 80);
                                            }
                                            else
                                                Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 80);
                                        }
                                        else
                                            Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 90);
                                    }
                                    else
                                        Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 105);
                                }
                                else
                                    Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 110);//here
                            }
                            else
                                Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 110);//here
                        }

                        else if (Attacker.Potency < this.Potency)
                        {

                            if ((Attacker.Potency + 2) < this.Potency)
                            {
                                if ((Attacker.Potency + 4) < this.Potency)
                                {
                                    if ((Attacker.Potency + 6) < this.Potency)
                                    {
                                        if ((Attacker.Potency + 8) < this.Potency)
                                        {
                                            if ((Attacker.Potency + 10) < this.Potency)
                                            {
                                                if ((Attacker.Potency + 15) < this.Potency)
                                                {
                                                    if ((Attacker.Potency + 20) < this.Potency)
                                                    {
                                                        if ((Attacker.Potency + 25) < this.Potency)
                                                        {
                                                            if ((Attacker.Potency + 30) < this.Potency)
                                                            {
                                                                Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 470);
                                                            }
                                                            else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 435);
                                                        }
                                                        else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 355);
                                                    }
                                                    else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 345);
                                                }
                                                else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 335);
                                            }
                                            else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 325);
                                        }
                                        else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 325);
                                    }
                                    else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 250);
                                }
                                else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 220);
                            }
                            else Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 210);

                        }

                        else if (Attacker.Potency == this.Potency)
                        {
                            Damage = (uint)((double)Damage * (100 - this.EqStats.TotalBless) / 40);
                        }
                        else
                            Console.WriteLine("Nu este Distribuit?");
                        //Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                        Damage += Attacker.EqStats.FanMagicMeleeDamageIncrease;

                        if (EqStats.TowerMeleeDamageDecrease >= Damage)
                            Damage = 1;
                        else
                            Damage -= EqStats.TowerMeleeDamageDecrease;
                    }
                    else if (AT == AttackType.Ranged)
                    {
                        Damage = (uint)((double)Damage * (((double)(110 - EqStats.Dodge) / 110))) + (Attacker.EqStats.FanAtack - EqStats.TowerDef);
                        if (EqStats.Dodge >= Damage)
                            Damage = 1;
                        else
                            Damage -= EqStats.Dodge;
                        Damage = Damage * 2 / 3;
                        Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 110) + (Attacker.EqStats.FanAtack - EqStats.TowerDef);

                        Damage += Attacker.EqStats.FanMagicMeleeDamageIncrease;

                        if (EqStats.TowerMeleeDamageDecrease >= Damage)
                            Damage = 1;
                        else
                            Damage -= EqStats.TowerMeleeDamageDecrease;
                        
                    }
                    else
                    {
                        Damage = (uint)((double)Damage * (((double)(100 - EqStats.MDef1) / 100)));
                        if (EqStats.MDef2 >= Damage)
                            Damage = 1;
                        else
                            Damage -= EqStats.MDef2;
                        Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);


                        Damage += Attacker.EqStats.FanMagicDamageIncrease;

                        if (EqStats.FanMagicDamageIncrease >= Damage)
                            Damage = 1;
                        else
                            Damage -= EqStats.FanMagicDamageIncrease;
                    }
                }
                if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                    Damage = 1;
                if (Damage < CurHP)
                {
                    CurHP -= (ushort)Damage;
               
                    
                        if (AT != AttackType.Magic && !IsSkill)
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                   
                   
                }
                else
                {
                    InitAngry(true,Attacker);
                    Attacker.AtkMem.Attacking = false;
                    Attacker.AtkMem.Target = 0;
                    AtkMem.Attacking = false;
                    AtkMem.Target = 0;

                    if (Loc.Map == 1090)
                    {
                        uint lol = 1;
                        if (Game.World.TDMScore.Contains(Attacker.MyTDmTeam.teamID))
                        {
                            Game.World.TDMScore[Attacker.MyTDmTeam.teamID] = (uint)Game.World.TDMScore[Attacker.MyTDmTeam.teamID] + lol;
                        }
                        else
                            Game.World.TDMScore.Add(Attacker.MyTDmTeam.teamID, lol);




                        // if (Attacker.dmblack == 1) { World.Dmscore.blackscore += 1; }
                        // else if (Attacker.dmblue == 1) { World.Dmscore.bluescore += 1; }
                        // else if (Attacker.dmred == 1) { World.Dmscore.redscore += 1; }
                        // Attacker.CPs += 5;//on kill                        
                    }
                    Game.World.ClassPkWar.RemovePlayersTorament(this);
                    if (Loc.Map == 1038)
                    {
                        Attacker.CPs += 5;
                        Attacker.HonorPoints += 5;
                        Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your have 5 honor points and 5 cps for kill enemy.");
                    }
                    if (Loc.Map == 6001)
                    {
                        Attacker.CPs += 5;
                        Attacker.HonorPoints += 5;
                        Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your have 5 honor points and 5 cps for kill enemy.");
                    }
                    if (Loc.Map == 1005)
                    {
                        Attacker.CPs += 5;
                        Attacker.HonorPoints += 5;
                        Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your have 5 honor points and 5 cps for kill enemy.");
                    }

                    if (!World.FreePKMaps.Contains(Loc.Map))
                    {
                        LoseInvItemsAndSilvers();
                        if (PKPoints >= 30)
                            LoseEquips(Attacker);
                        if (!BlueName)
                        {
                            Attacker.BlueNameLasts += 45;
                            //Attacker.HonorPoints += 10;
                            if (Attacker.Enemies.Contains(EntityID))
                            {
                                Attacker.PKPoints += 5;
                                Attacker.HonorPoints += 50;
                                //Attacker.PKPoints += 5;
                            }
                            else
                                Attacker.PKPoints += 10;
                            //Attacker.HonorPoints += 5;
                        }
                        if (!Enemies.Contains(Attacker.EntityID))
                        {
                            Enemies.Add(Attacker.EntityID, new Enemy() { UID = Attacker.EntityID, Name = Attacker.Name });
                            MyClient.SendPacket(Packets.FriendEnemyPacket(Attacker.EntityID, Attacker.Name, 19, 1));
                            Database.SaveEnemy(Attacker.EntityID, Attacker.Name, this);
                        }
                    }
                    Alive = false;
                    CurHP = 0;

                    if (AT != AttackType.Magic && !IsSkill)
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AttackType.Kill));

                    StatEff.Remove(StatusEffectEn.Cyclone);
                    StatEff.Remove(StatusEffectEn.FatalStrike);
                    StatEff.Remove(StatusEffectEn.BlueName);
                    StatEff.Remove(StatusEffectEn.Flashy);
                    StatEff.Remove(StatusEffectEn.ShurikenVortex);
                    BlueName = false;
                    StatEff.Remove(StatusEffectEn.SuperMan);
                    StatEff.Remove(StatusEffectEn.XPStart);
                    StatEff.Remove(StatusEffectEn.Ride);
                    StatEff.Add(StatusEffectEn.Dead);
                    DeathHit = DateTime.Now;

                    if (PKPoints >= 100)
                        World.SendMsgToAll("SYSTEM", Attacker.Name + " has captured " + Name + " and sent him to jail.", 2000, 0, System.Drawing.Color.Red);
                    if (PKPoints >= 30)
                        World.SendMsgToAll("SYSTEM", Attacker.Name + " has captured " + Name + " and sent him to jail.", 2000, 0, System.Drawing.Color.Red);

                    if (Attacker.BlessingLasts == 0)
                    {
                        if (Loc.Map != 1005 || Loc.Map != 1038 || Loc.Map != 6001)
                        {
                            if (BlessingLasts > 0)
                            {
                                CurseUser = DateTime.Now;
                                CurseStart = true;
                                CurseExpLeft += 300;

                                Attacker.MyClient.SendPacket(Packets.Status(EntityID, Status.CurseTime, (ulong)CurseExpLeft));
                                // Attacker.MyClient.SendPacket(Packets.String(EntityID, 10, "curse"));
                            }
                        }
                    }


                }
            }
        }
        #endregion
        public uint PrepareAttack(byte AtkType, bool ArrowCost)
        {
            AtkMem.LastAttack = DateTime.Now;
            AttackType A = (AttackType)AtkType;

            bool EnoughArrows = true;
            if (A == AttackType.Ranged && ArrowCost)
            {
                if (Loc.Map != 1039)
                {
                    if (Equips.LeftHand.ID != 0 && Item.IsArrow(Equips.LeftHand.ID))
                    {
                        Equips.LeftHand.CurDur -= 1;
                        if (Equips.LeftHand.CurDur == 0)
                        {
                            MyClient.SendPacket(Packets.ItemPacket(Equips.LeftHand.UID, 5, 6));
                            MyClient.SendPacket(Packets.ItemPacket(Equips.LeftHand.UID, 0, 3));
                            Equips.LeftHand = new Item();
                        }
                        else
                            MyClient.SendPacket(Packets.AddItem(Equips.LeftHand, 5));
                    }
                    else
                    {
                        AtkMem.Attacking = false;
                        EnoughArrows = false;
                    }
                }
            }

            if (EnoughArrows)
            {
                /*
               if (A == AttackType.Melee || A == AttackType.Ranged)
                {
                    uint Damage = (uint)Rnd.Next((int)EqStats.minatk, (int)EqStats.maxatk);
                    Damage = (uint)((Damage + Str) * EqStats.GemExtraAttack);
                    Buff Stig = BuffOf(SkillsClass.ExtraEffect.Stigma);
                    if (Stig.Eff == SkillsClass.ExtraEffect.Stigma)
                        Damage = (uint)(Damage * Stig.Value);
                    return Damage;
                }
                 */
                if (A == AttackType.Melee || A == AttackType.Ranged)
                {
                    uint Damage = (uint)Rnd.Next((int)EqStats.minatk, (int)EqStats.maxatk);
                    Damage = (uint)((Damage + Str) * EqStats.GemExtraAttack);
                    Buff Stig = BuffOf(SkillsClass.ExtraEffect.Stigma);
                    if (Stig.Eff == SkillsClass.ExtraEffect.Stigma)
                        Damage = (uint)(Damage * Stig.Value);
                    return Damage;
                }
                else
                {
                    uint Damage = (uint)(EqStats.matk * EqStats.GemExtraMAttack);
                    return Damage;
                }
            }
            return 0;
        }
        public uint ExpBallExp
        {
            get
            {
                if (Level < 30)
                    return (uint)(15000 + Level * 430);
                else if (Level < 50)
                    return (uint)(40000 + Level * 430);
                else if (Level < 80)
                    return (uint)(30000 + Level * 500);
                else if (Level < 80)
                    return (uint)(30000 + Level * 600);
                else if (Level < 100)
                    return (uint)(30000 + Level * 700);
                else if (Level < 110)
                    return (uint)(30000 + Level * 900);
                else if (Level < 120)
                    return (uint)(30000 + Level * 1100);
                else if (Level < 125)
                    return (uint)(30000 + Level * 1500);
                else if (Level < 130)
                    return (uint)(30000 + Level * 1000);
                else
                    return (uint)(30000 + Level * 1000);
            }
        }
        public uint PEBExp
        {
            get
            {
                if (Level > 25)
                    return (uint)(Level + 500000);
                else if (Level > 45)
                    return (uint)(Level + 250000);
                else if (Level > 65)
                    return (uint)(Level + 150000);
                else if (Level > 85)
                    return (uint)(Level + 100000);
                else if (Level > 115)
                    return (uint)(Level + 50000);
                else if (Level > 120)
                    return (uint)(Level + 25000);
                else
                    return (uint)(Level * .10);
            }
        }
        public void UseItem(Item I)
        {
            try
            {
                if (I.ID >= 1000000 && I.ID <= 1002050)
                {
                    if (!(DateTime.Now > UnableToUseDrugs.AddSeconds(UnableToUseDrugsFor)))
                    {
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You've been poisoned. You cannot use drugs for a while.");
                        return;
                    }
                }
                if (I.DBInfo.Name == "MoonBox")
                {
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , "MoonBox openning not yet added. Its just for promoting.");
                    return;
                }
                switch (I.ID)
                {

                    case 729910:
                        { CPs += 1; RemoveItemI(I.UID, 1, MyClient); break; }
                    case 729912:
                        { CPs += 6; RemoveItemI(I.UID, 1, MyClient); break; }
                    case 729911:
                        { CPs += 3; RemoveItemI(I.UID, 1, MyClient); break; }




                    #region trojan
                    case 720121:
                        {
                            if (Inventory.Count <= 33)
                            {

                                RemoveItemI(720121, 1, MyClient);
                                AddItem(130109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(118109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(120249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(150249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;
                                AddItem(410339, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //blada
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //club

                            }
                            break;
                        }
                    #endregion
                    #region wariors
                    case 720122:
                        {
                            if (Inventory.Count <= 31)
                            {
                                RemoveItemI(720122, 1, MyClient);
                                AddItem(111109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(131199, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(141199, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Robe;
                                AddItem(900109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(120249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(150249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;
                                AddItem(410339, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //blada
                                AddItem(131109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //blada
                            }
                            break;
                        }
                    #endregion
                    #region EasterBronzePack
                    case 720123:
                        {
                            if (Inventory.Count <= 32)
                            {
                                RemoveItemI(720123, 1, MyClient);
                                AddItem(123109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(112109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(601339, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Robe;
                                AddItem(601339, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(120249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(135109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(150249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;
                            }
                            break;
                        }
                    #endregion
                    #region EasterBronzePack
                    case 720124:
                        {
                            if (Inventory.Count <= 34)
                            {
                                RemoveItemI(720124, 1, MyClient);
                                AddItem(134109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(114109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(152259, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Robe;
                                AddItem(121249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(421339, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;

                            }
                            break;
                        }
                    #endregion
                    #region EasterBronzePack
                    case 720125:
                        {
                            if (Inventory.Count <= 34)
                            {
                                RemoveItemI(720125, 1, MyClient);
                                AddItem(113109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Earring;
                                AddItem(133109, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(120249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Necklace;
                                AddItem(150249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Ring;
                                AddItem(160249, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;
                                AddItem(500329, 12, 255, 7, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem); //Boots;

                            }
                            break;
                        }
                    #endregion
                    #region FlamesStones
                    case 729960:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 1º Flame Cristal is (350,327)");
                            break;
                        }
                    case 729961:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 2º Flame Cristal is (317,270)");
                            break;
                        }
                    case 729962:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 3º Flame Cristal is (236,291)");
                            break;
                        }
                    case 729963:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 4º Flame Cristal is (194,168)");
                            break;
                        }
                    case 729964:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 5º Flame Cristal is (115,53)");
                            break;
                        }
                    case 729965:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 6º Flame Cristal is (316,378)");
                            break;
                        }
                    case 729966:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 7º Flame Cristal is (136,182)");
                            break;
                        }
                    case 729967:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 8º Flame Cristal is (38,94)");
                            break;
                        }
                    case 729968:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 9º Flame Cristal is in Twin City (350,321)");
                            break;
                        }
                    case 729969:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "The 10º Flame Cristal is (62,59)");
                            break;
                        }
                    case 729970:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Go to the FlameToist in Twin City (355,325)");
                            break;
                        }
                    #endregion


                    #region SteedPacks
                    case 723855:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 1;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 0;
                            It.RBG[1]= 150;
                            It.RBG[2] = 255;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
    
                            break;
                        }
                    case 723856:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 1;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                           AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays,It.RBG[0],It.RBG[1],It.RBG[2],It.RBG[3]);
                      
                            break;
                        }
                    case 723859:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 1;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 0;
                            It.RBG[2] = 150;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723860:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 3;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 0;
                            It.RBG[1] = 150;
                            It.RBG[2] = 255;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723861:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 3;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723862:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 3;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 0;
                            It.RBG[2] = 150;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723863:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 6;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 0;
                            It.RBG[1] = 150;
                            It.RBG[2] = 255;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723864:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 6;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723865:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 6;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 0;
                            It.RBG[2] = 150;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723900:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 0;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 0;
                            It.RBG[1] = 150;
                            It.RBG[2] = 255;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723901:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 0;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    case 723902:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.DBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 0;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = NewestCOServer.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 0;
                            It.RBG[2] = 150;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemI(I.UID, 1, MyClient);
                            AddFullItem(It.ID, It.Bless, It.Plus, It.Enchant, It.Soc1, It.Soc2, It.Color, It.Progress, It.TalismanProgress, It.Effect, It.FreeItem, It.CurDur, It.MaxDur, It.Suspicious, It.Locked, It.LockedDays, It.RBG[0], It.RBG[1], It.RBG[2], It.RBG[3]);
                            break;
                        }
                    #endregion
                    #region exceptionalToken
                    case 723701:
                        {
                            if (Reborns == 1)
                            {
                                rebornquest = 1;
                                MyClient.MessageToAll(2011, "CONGRATION! " + Name + " has finish 2nd Reborn Quest",System.Drawing.Color.White);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.MessageToAll(2005, "Sorry, but you dont is 1st reborn for using this",System.Drawing.Color
                                .White);

                            break;
                        }
                    #endregion
                    #region PointCard
                    case 780000:
                        {
                            CPs += 80;
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region NeteorTTear
                    case 723711:
                        {
                            if (Inventory.Count < 34)
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Congration!you have 5 MetTear");
                                RemoveItemI(I.UID, 1, MyClient);

                                for (int i = 0; i < 5; i++)
                                    AddItem(1088002);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry,but you inventory is full");
                            break;
                        }
                    #endregion
                    #region LifeFruitBasket
                    case 723725:
                        {
                            if (Inventory.Count < 30)
                            {
                                for (int x = 0; x < 10; x++)
                                    AddItem(723726);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            break;
                        }
                    #endregion
                    #region LifeFruit
                    case 723726:
                        {
                            CurHP = MaxHP;
                            CurMP = MaxMP;
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region Blacktulip
                    case 723584:
                        {
                            if (Equips.Armor.ID != 0)
                            {
                                Equips.Armor.Color = Item.ArmorColor.Black;
                                Equips.Replace(3, Equips.Armor, this);
                                World.Spawn(this, true);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You've gotta wear an armor...");
                                return;
                            }
                            break;
                        }
                    #endregion
                    #region Amrita Box
                    case 720010://Amrita
                        {
                            uint DrugID = 1000030;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region Panacea Box
                    case 720011://Panacea
                        {
                            uint DrugID = 1002000;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region Ginseng Box
                    case 720012://Ginseng 
                        {
                            uint DrugID = 1002010;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region +3stonePatck
                    case 723832:
                        {
                            if (Inventory.Count < 31)
                            {
                                AddItem(723700);
                                AddItem(723833);
                                AddItem(752099);
                                RemoveItemI(I.UID, 1, MyClient);
                                for (int i = 0; i < 5; i++)
                                    AddItemStone(730003,3);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry,but you inventory is full");
                            break;
                        }
                    #endregion
                    #region Vanilla Box
                    case 720013://Vanilla 
                        {
                            uint DrugID = 1002020;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region RecoveryPill Box
                    case 720014://RecoveryPill 
                        {
                            uint DrugID = 1001030;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion RecoveryPill
                    #region SoulPill Box
                    case 720015://SoulPill 
                        {
                            uint DrugID = 1001040;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region RefreshingPill Box
                    case 720016://RefreshingPill 
                        {
                            uint DrugID = 1002030;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region ChantPill Box
                    case 720017://ChantPill 
                        {
                            uint DrugID = 1002040;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region Mil.Ginseng Box
                    case 721330:
                        {
                            uint DrugID = 1002050;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                AddItem(DrugID);
                                AddItem(DrugID);
                                AddItem(DrugID);
                            }
                            break;
                        }
                    #endregion
                    #region NinjaAmulet
                    case 723583:
                        {
                            if (Body == 1004)
                                Body -= 1;
                            else if (Body == 1003)
                                Body += 1;
                            if (Body == 2002)
                                Body -= 1;
                            else if (Body == 2001)
                                Body += 1;

                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region WindSpells
                    case 1060025:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 411, 704);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060026:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 96, 323);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060027:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 795, 465);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060028:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 538, 772);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060029:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 734, 452);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060031:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 824, 601);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060032:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 491, 731);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060033:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 106, 394);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060034:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1000, 225, 205);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060035:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1000, 793, 549);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060037:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1001, 470, 366);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060038:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 67, 423);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    #endregion
                    case 725016:
                        {
                            if (Level >= 80)
                            {
                                NewSkill(new Skill() { ID = 1360 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use NightDevil at your Level!");
                            }
                            break;
                        }
                    #region PrayingStone(S)
                    case 723833:
                    case 1200000:
                        {
                            BlessingLasts = 3;
                            BlessingStarted = DateTime.Now;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "bless"));
                            MyClient.SendPacket(Packets.String(EntityID, 10, "zf2-e129"));
                            StatEff.Add(StatusEffectEn.Blessing);
                            MyClient.SendPacket(Packets.Status(EntityID, Status.BlessTime, 3 * 60 * 60 * 24));
                            MyClient.SendPacket(Packets.Status(EntityID, Status.OnlineTraining, 0));
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region PrayingStone(M)
                    case 1200001:
                        {
                            BlessingLasts = 7;
                            BlessingStarted = DateTime.Now;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "bless")); MyClient.SendPacket(Packets.String(EntityID, 10, "zf2-e129"));
                            StatEff.Add(StatusEffectEn.Blessing);
                            MyClient.SendPacket(Packets.Status(EntityID, Status.BlessTime, 7 * 60 * 60 * 24));
                            MyClient.SendPacket(Packets.Status(EntityID, Status.OnlineTraining, 0));
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region PrayingStone(L)
                    case 1200002:
                        {
                            BlessingLasts = 30;
                            BlessingStarted = DateTime.Now;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "bless")); MyClient.SendPacket(Packets.String(EntityID, 10, "zf2-e129"));
                            StatEff.Add(StatusEffectEn.Blessing);
                            MyClient.SendPacket(Packets.Status(EntityID, Status.BlessTime, 30 * 60 * 60 * 24));
                            MyClient.SendPacket(Packets.Status(EntityID, Status.OnlineTraining, 0));
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region ExpPotion
                    case 723017:
                        {
                            ExpPotionUsed = DateTime.Now;
                            DoubleExp = true;
                            DoubleExpLeft = 3600;

                            // MyClient.SendPacket(Packets.Status(EntityID, Status.flower, pote));
                           // MyClient.SendPacket(Packets.Status(EntityID, Status.PotFromMentor, pote));
                           //   MyClient.SendPacket(Packets.Status(EntityID, Status.countskill, (ulong)4));
                            MyClient.SendPacket(Packets.Status(EntityID, Status.DoubleExpTime, (ulong)DoubleExpLeft));
                            //MyClient.SendPacket(Packets.Status(EntityID, Status.ClanBatPower, 26033380));
                            //     MyClient.SendPacket(Packets.Status(EntityID, Status.a, 26033380));
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region ExpBall
                    case 723834:
                    case 722136:
                    case 723911:
                    case 723700:
                        {
                            if (ExpBallsUsedToday < 10)
                            {
                                if (Level < 137)
                                {
                                    IncreaseExp(ExpBallExp, false);

                                    RemoveItemI(I.UID, 1, MyClient);
                                    ExpBallsUsedToday++;
                                    MyClient.SendPacket(Packets.String(EntityID, 10, "fam_gain_special"));
                                    ElighemPoints += 10;
                                    MyClient.SendPacket(Packets.Status(EntityID, Status.Elighten, (ulong)ElighemPoints));
                                }
                                else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but you dont have hight level for using expball");
                            }
                            else
                                MyClient.LocalMessage(2000,System.Drawing.Color.Yellow ,"You can only use 10 Exp Balls in one day.");
                            break;
                        }
                    #endregion
                    #region PowerExpBall
                    case 723744:
                        {
                            if (PEBUsedToday < 10)
                            {
                                if (Level < 137)
                                {
                                    IncreaseExp(PEBExp, false);
                                    RemoveItemI(I.UID, 1, MyClient);
                                    PEBUsedToday++;
                                    MyClient.SendPacket(Packets.String(EntityID, 10, "fam_gain_special "));
                                }
                            }
                            else
                                MyClient.LocalMessage(2000,System.Drawing.Color.Yellow ,"You can only use 10 PowerEXPBalls in one day.");
                            break;
                        }
                    #endregion
                    #region Pentenice amulet
                    case 723727:
                        {
                            if (PKPoints >= 30)
                            {
                                PKPoints -= 30;
                                RemoveItemI(I.UID, 1, MyClient);
                                MyClient.LocalMessage(2000,System.Drawing.Color.Yellow ,"You have used the pk amulet!!");
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but you dont have pkp for using this");
                            break;
                        }
                    #endregion
                        
                    #region +1Stone Pack
                    case 723712:
                        {
                            if (Inventory.Count < 35)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                               // for (int i = 0; i < 5; i++)
                                AddItemStone(730001,1);
                                AddItemStone(730001,1);
                                AddItemStone(730001,1);
                                AddItemStone(730001,1);
                                AddItemStone(730001,1);
                            }
                            break;
                        }
                    #endregion
                    #region Class1MoneyBag
                    case 723713:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 300000;
                            break;
                        }
                    #endregion
                    #region Class2MoneyBag
                    case 723714:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 800000;
                            break;
                        }
                    #endregion
                    #region Class3MoneyBag
                    case 723715:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 1200000;
                            break;
                        }
                    #endregion
                    #region Class4MoneyBag
                    case 723716:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 1800000;
                            break;
                        }
                    #endregion
                    #region Class5MoneyBag
                    case 723717:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 5000000;
                            break;
                        }
                    #endregion
                    #region Class6MoneyBag
                    case 723718:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 20000000;
                            break;
                        }
                    #endregion
                    #region Class7MoneyBag
                    case 723719:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 25000000;
                            break;
                        }
                    #endregion
                    #region Class8MoneyBag
                    case 723720:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 80000000;
                            break;
                        }
                    #endregion
                    #region Class9MoneyBag
                    case 723721:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 100000000;
                            break;
                        }
                    #endregion
                    #region Class10MoneyBag
                    case 723722:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 300000000;
                            break;
                        }
                    #endregion
                    #region Class11MoneyBag
                    case 7237223:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 600000000;
                            break;
                        }
                    #endregion
                    #region TopMoneyBag
                    case 723723:
                        {
                            RemoveItemI(I.UID, 1, MyClient);
                            Silvers += 500000000;
                            break;
                        }
                    #endregion
                    #region DBScroll
                    case 720028:
                        {
                            if (Inventory.Count < 30)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                for (int i = 0; i < 10; i++)
                                    AddItem(1088000);
                            }
                            break;
                        }
                    #endregion
                    #region MeteorScroll
                    case 720027:
                        {
                            if (Inventory.Count < 30)
                            {
                                RemoveItemI(I.UID, 1, MyClient);
                                for (int i = 0; i < 10; i++)
                                    AddItem(1088001);
                            }
                            break;
                        }
                    #endregion
                    #region MP Pots//Replace your entire potion code with mine.
                    case 1001000:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 70;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;
                        }

                    case 1001010:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 200;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;

                        }
                    case 1001020:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 450;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;
                        }
                    case 1001030:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 1000;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;
                        }
                    case 1001040:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 2000;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                        }
                        break;
                    case 1002030:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 3000;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;
                        }
                    case 1002040:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 4500; if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You MP is currently full.");
                            }
                            break;
                        }
                    #endregion
                    #region HP Pots
                    case 1000000:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 70;
                                if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1000010:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 150; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1000020:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 250; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1000030:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 500; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002000:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 800; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002010:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 1200; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002020:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 2000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red ,"You HP is currently full.");
                            }
                            break;
                        }
                    case 1002050:
                        {
                            if (CurHP < MaxHP)
                            {
                                CurHP += 3000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You HP is currently full.");
                            }
                            break;
                        }
                    #endregion
                    #region SkillBooks
                    case 725000:
                        {
                            if (Spi >= 30)
                            {
                                NewSkill(new Skill() { ID = 1000 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but you need spirit by 30 ");
                            break;
                        }
                    case 725001:
                        {
                            if (Job >= 130 && Job <= 135 || Job >= 140 && Job <= 145 && Level >= 40 || Job == 100 || Job == 101)
                            {
                                NewSkill(new Skill() { ID = 1001 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for WaterTaoist or FireTaoist or you dont have level 40+");
                            break;
                        }
                    case 725002:
                        {
                            if (Job >= 140 && Job <= 145 && Level >= 90)
                            {
                                NewSkill(new Skill() { ID = 1002 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for FireTaoist or you dont have level 90+");
                            break;
                        }
                    case 725003:
                        {
                            if (Job >= 130 && Job <= 135 || Job >= 140 && Job <= 145 || Job == 100 || Job == 101)
                            {
                                NewSkill(new Skill() { ID = 1005 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for WaterTaoist or FireTaoist");
                            break;
                        }
                    case 725004:
                        {
                            if (Job >= 130 && Job <= 135 || Job >= 140 && Job <= 145 && Level >= 15 || Job == 100 || Job == 101)
                            {
                                NewSkill(new Skill() { ID = 1010 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for WaterTaoist or FireTaoist or you dont have level 15+");
                            break;
                        }
                    case 725005:
                        {
                            if (Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1045 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but need level 40 for using the skill");
                            break;
                        }
                    case 725010:
                        {
                            if (Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1046 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but need level 40 for using the skill");
                            break;
                        }
                    case 725011:
                        {
                            NewSkill(new Skill() { ID = 1250 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725012:
                        {
                            NewSkill(new Skill() { ID = 1260 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725013:
                        {
                            NewSkill(new Skill() { ID = 1290 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725014:
                        {
                            NewSkill(new Skill() { ID = 1300 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725015:
                        {
                            if (Job >= 140 && Job <= 145)
                            {
                                NewSkill(new Skill() { ID = 1350 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for FireTaoist");
                            break;
                        }
                    case 725019:
                        {
                            NewSkill(new Skill() { ID = 1385 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725020:
                        {
                            NewSkill(new Skill() { ID = 1390 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725021:
                        {
                            NewSkill(new Skill() { ID = 1395 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725022:
                        {
                            NewSkill(new Skill() { ID = 1400 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725023:
                        {
                            NewSkill(new Skill() { ID = 1405 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725024:
                        {
                            NewSkill(new Skill() { ID = 1410 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725025:
                        {
                            NewSkill(new Skill() { ID = 1320 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725026:
                        {
                            NewSkill(new Skill() { ID = 5010 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725027:
                        {
                            NewSkill(new Skill() { ID = 5020 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725028:
                        {
                            NewSkill(new Skill() { ID = 5001 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725029:
                        {
                            NewSkill(new Skill() { ID = 5030 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725030:
                        {
                            NewSkill(new Skill() { ID = 5040 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725031:
                        {
                            NewSkill(new Skill() { ID = 5050 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725040:
                        {
                            NewSkill(new Skill() { ID = 7000 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725041:
                        {
                            NewSkill(new Skill() { ID = 7010 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725042:
                        {
                            NewSkill(new Skill() { ID = 7020 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725043:
                        {
                            NewSkill(new Skill() { ID = 7030 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 725044:
                        {
                            NewSkill(new Skill() { ID = 7040 });
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060100:
                        {
                            if (Job >= 140 && Job <= 145 && Level >= 82)
                            {
                                NewSkill(new Skill() { ID = 1160 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for FireTaoist and need level 82");
                            break;
                        }
                    case 1060101:
                        {
                            if (Job >= 140 && Job <= 145 && Level >= 84)
                            {
                                NewSkill(new Skill() { ID = 1165 });
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but the skill is for FireTaoist and need level 84");
                            break;
                        }
                    #endregion
                    #region Teleport Scrolls
                    case 1060020:
                        {
                            Game.World.ClassPkWar.RemovePlayersTorament(this);
                            if (Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1090 && Loc.Map != 5000 && Loc.Map != 1707)
                            {
                                Teleport(1002, 429, 378);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use teleport scrolls here.");
                            break;
                        }
                    case 1060021:
                        {
                            Game.World.ClassPkWar.RemovePlayersTorament(this);
                            if (Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1090 && Loc.Map != 5000 && Loc.Map != 1707)
                            {
                                Teleport(1000, 500, 650);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use teleport scrolls here.");
                            break;
                        }
                    case 1060022:
                        {
                            Game.World.ClassPkWar.RemovePlayersTorament(this);
                            if (Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1090 && Loc.Map != 5000 && Loc.Map != 1707)
                            {
                                Teleport(1020, 565, 562);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use teleport scrolls here.");
                            break;
                        }
                    case 1060023:
                        {
                            Game.World.ClassPkWar.RemovePlayersTorament(this);
                            if (Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1090 && Loc.Map != 5000 && Loc.Map != 1707)
                            {
                                Teleport(1011, 188, 264);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use teleport scrolls here.");
                            break;
                        }
                    case 1060024:
                        {
                            Game.World.ClassPkWar.RemovePlayersTorament(this);
                            if (Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1090 && Loc.Map != 5000 && Loc.Map != 1707)
                            {
                                Teleport(1015, 717, 571);
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Cannot use teleport scrolls here.");
                            break;
                        }
                    #endregion
                    #region HairDye Pots
                    case 1060030:
                        {
                            Hair = ushort.Parse("3" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060040:
                        {
                            Hair = ushort.Parse("9" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060050:
                        {
                            Hair = ushort.Parse("8" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060060:
                        {
                            Hair = ushort.Parse("7" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060070:
                        {
                            Hair = ushort.Parse("6" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060080:
                        {
                            Hair = ushort.Parse("5" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    case 1060090:
                        {
                            Hair = ushort.Parse("4" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemI(I.UID, 1, MyClient);
                            break;
                        }
                    #endregion
                    #region Fireworks
                    case 720030: //fireworks
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
                            RemoveItemI(I.UID, 1, MyClient);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    case 720031: //fireworks EndlessLove
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "firework-2love"));
                            RemoveItemI(I.UID, 1, MyClient);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    case 720032: //fireworks MyWish
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "firework-like"));
                            RemoveItemI(I.UID, 1, MyClient);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    #endregion
                    #region PenitenceAmulet
                    case 720128:
                        {
                            if (PKPoints >= 30)
                            {
                                PKPoints -= 30;
                                RemoveItemI(I.UID, 1, MyClient);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Sorry, but you dont have more pkp for using this");
                            break;
                        }
                    #endregion
                    default:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Unable to use the item.");
                            MyClient.LocalMessage(2011,System.Drawing.Color.Yellow, "   ItemID: " + I.ID);
                            break;
                        }
                }
            }
            catch (Exception Exc) { Program.WriteLine(Exc); }
        }
        public void EquipStats(byte Pos, bool Equip)
        {
            try
            {
                Item I = Equips.Get(Pos);
                if (I.ID != 0)
                {
                    DatabaseItem D1 = I.DBInfo;

                    ItemIDManipulation IMan = new ItemIDManipulation(I.ID);
                    uint ComposeID = IMan.ToComposeID(Pos);
                    DatabasePlusItem D2;
                    if (Database.DatabasePlusItems.Contains(ComposeID.ToString() + I.Plus.ToString()))
                        D2 = (DatabasePlusItem)Database.DatabasePlusItems[ComposeID.ToString() + I.Plus.ToString()];
                    else D2 = new DatabasePlusItem();

                    EquipStats E = new EquipStats();
                  
                    if (Pos != 12)
                        E.Dodge = (byte)(D1.Dodge + D2.Dodge);
                    else
                        E.AddRideSpeed = D2.Dodge;

                    if (Pos != 12)
                        E.ExtraDex = (ushort)(D1.DexGives + D2.Dex);
                    else
                        E.AddVigor = D2.Dex;
                    if (Pos == 12)
                    {
                     
                        E.MaxHP += 100;
                    }
                    if (Pos == 5)
                    {
                        D1.MinAtk /= 2;
                        D1.MaxAtk /= 2;
                    }
                    if (Pos != 10 && Pos != 11)
                    {
                        E.defense = (ushort)(D1.Defense + D2.Defense);
                        E.matk = (uint)(D1.MagicAttack + D2.MAtk);
                        E.minatk = D2.MinAtk + D1.MinAtk;
                        E.maxatk = D2.MaxAtk + D1.MaxAtk;
                        E.MDef1 = (ushort)D1.MagicDefense;
                        E.MDef2 = D2.MDef;
                    }
                    if (Pos == 10)
                    {
                        if (I.ID == 201003)
                        {
                            E.FanMagicMeleeDamageIncrease = 300 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)150 + D2.MAtk;
                        }
                        else if (I.ID == 201004)
                        {
                            E.FanMagicMeleeDamageIncrease = 300 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)150 + D2.MAtk;
                        }
                        else if (I.ID == 201005)
                        {
                            E.FanMagicMeleeDamageIncrease = 300 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)150 + D2.MAtk;
                        }
                        else if (I.ID == 201006)
                        {
                            E.FanMagicMeleeDamageIncrease = 500 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)250 + D2.MAtk;
                        }
                        else if (I.ID == 201007)
                        {
                            E.FanMagicMeleeDamageIncrease = 700 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)300 + D2.MAtk;
                        }
                        else if (I.ID == 201008)
                        {
                            E.FanMagicMeleeDamageIncrease = 900 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)450 + D2.MAtk;
                        }
                        else if (I.ID == 201009)
                        {
                            E.FanMagicMeleeDamageIncrease = 1200 + D2.MaxAtk;
                            E.FanMagicDamageIncrease = (uint)750 + D2.MAtk;
                           // Console.WriteLine(" atack {0} def {1} name {2}", E.FanAtack, E.FanMagicAtack, I.DBInfo.Name);
                        }
                        
                    }
                  else if (Pos == 11)
                    {
                        if (I.ID == 202003)
                        {
                            E.TowerMeleeDamageDecrease = (uint)250 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)100 + D2.MDef;
                        }
                        else if (I.ID == 202004)
                        {
                            E.TowerMeleeDamageDecrease = (uint)250 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)100 + D2.MDef;
                        }
                        else if (I.ID == 202005)
                        {
                            E.TowerMeleeDamageDecrease = (uint)250 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)100 + D2.MDef;
                        }
                        else if (I.ID == 202006)
                        {
                            E.TowerMeleeDamageDecrease  = (uint)400 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)150 + D2.MDef;
                        }
                        else if (I.ID == 202007)
                        {
                            E.TowerMeleeDamageDecrease = (uint)550 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)200 + D2.MDef;
                        }
                        else if (I.ID == 202008)
                        {
                            E.TowerMeleeDamageDecrease = (uint)700 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)300 + D2.MDef;
                        }
                        else if (I.ID == 202009)
                        {
                            E.TowerMeleeDamageDecrease = (uint)1100 + D2.Defense;
                            E.TowerMagicMeleeDamageIncrease = (uint)600 + D2.MDef;
                        }
                    }
                   // else
                   // {
                   //     E.FanMagicDamageIncrease = (uint)(D1.MagicAttack + D2.MAtk);
                   //     E.TowerMagicMeleeDamageIncrease = (uint)(D1.MagicDefense + D2.MDef);
                   //     E.FanMagicMeleeDamageIncrease = (uint)(D2.MinAtk + D1.MinAtk);
                   //     E.TowerMeleeDamageDecrease = (uint)(D1.Defense + D2.Defense);
                   // }
                    Item.GetGemEffect(ref E, I.Soc1);
                    Item.GetGemEffect(ref E, I.Soc2);
                    if (Equips.Tower.ID != I.ID && Equips.Fan.ID != I.ID)
                        E.MaxHP += I.Enchant;
                    E.MaxHP += D2.HP;
                    E.eq_pot += I.Pot;
                    E.TotalBless += I.Bless;

                    if (I.ID == 2100045)//MagicalBottle
                    {
                        E.MaxMP += 400;
                    }
                    if (I.ID == 2100025)//MiraculousGourd
                    {
                        E.MaxHP += 800;
                        E.MaxMP += 800;
                    }

                    if (I.ID == 2100075)//GOLD PRIZE
                    {
                        E.maxatk += 1000;
                        E.defense += 1000;
                        E.MaxHP += 1500;
                        E.MaxMP += 1500;
                    }
                    if (I.ID == 2100065)//SILVER PRIZE
                    {
                        E.MaxHP += 1200;
                        E.MaxMP += 1200;
                    }
                    if (I.ID == 2100055)//BRONZE PRIZE
                    {
                        E.MaxHP += 900;
                        E.MaxMP += 900;
                    }
                    if (I.ID == 150000)
                        E.MaxHP += 800;
                    if (Equip) EqStats += E;
                    else EqStats -= E;
                }
            }
            catch (Exception Exc) { Program.WriteLine(Exc); }
        }
        public void LoseEquips(Game.Character C)
        {
            try
            {
                for (byte i = 0; i < 12; i++)
                {
                    Item I = Equips.Get(i);
                    if ((I.ID != 0 && !I.FreeItem) && I.Locked==0)
                    {
                        if (MyMath.ChanceSuccess(25))
                        {
                            Equips.Replace(i, new Item(), this);
                            DroppedItem D = new DroppedItem();
                            //    DroppedItemConfiscator D = new DroppedItemConfiscator();
                            D.Info = I;
                            D.UID = (uint)(Rnd.Next(10000000));
                            D.Loc = new Location();
                            D.Loc.X = Loc.X;
                            D.Loc.Y = Loc.Y;
                            D.Loc.Map = Loc.Map;
                            D.DropTime = DateTime.Now;

                            if (D.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                                D.Drop();
                            Database.SaveCharacter(this, this.MyClient.AuthInfo.Account);
                            Database.SaveCharacter(C, C.MyClient.AuthInfo.Account);
                        /*    this.MyClient.SendPacket(Packets.ConfiscatorReward(I,C,(ushort)PacketHandling.Confiscator.CpsItem(I),C.Name));
                            I.NameClain = this.Name;
                            I.NameReward = C.Name;
                            Database.ConfiscatorClain(I, C);
                            Database.ConfiscatorReward(I, this);
                            this.ConfiscatorReward.Add(I.UID,I);
                            C.ConfiscatorClain.Add(I.UID,I);
                            C.MyClient.SendPacket(Packets.ConfiscatorClain(I, this, (ushort)PacketHandling.Confiscator.CpsItem(I),this.Name));
                       */
                          
                        }
                    }
                }
            }
            catch (Exception Exc) { Program.WriteLine(Exc); }
        }
        public void LoseInvItemsAndSilvers()
        {
            try
            {
                    double Pc = (double)((Level & 15) + ((double)Level / 10));
                    uint SilversLose = (uint)(Silvers * Pc / 100);

                    DroppedItem DI = new DroppedItem();
                    DI.Info = new Item();
                    DI.Silvers = SilversLose;
                    Silvers -= SilversLose;

                    if (DI.Silvers < 10)
                        DI.Info.ID = 1090000;
                    else if (DI.Silvers < 100)
                        DI.Info.ID = 1090010;
                    else if (DI.Silvers < 1000)
                        DI.Info.ID = 1090020;
                    else if (DI.Silvers < 3000)
                        DI.Info.ID = 1091000;
                    else if (DI.Silvers < 10000)
                        DI.Info.ID = 1091010;
                    else
                        DI.Info.ID = 1091020;
                    DI.UID = (uint)Rnd.Next(10000000);
                    DI.DropTime = DateTime.Now;
                    DI.Loc = Loc;
                    if (DI.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                        DI.Drop();

                    ArrayList ItemsLost = new ArrayList();
                    foreach (Item I in Inventory.Values)
                    {
                        if (MyMath.ChanceSuccess(Pc) && !I.FreeItem)
                            ItemsLost.Add(I);
                    }
                    foreach (Item I in ItemsLost)
                    {
                        Database.DeleteItem(I.UID, this);
                        Inventory.Remove(I.UID);
                        MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                       // RemoveItemI(I.UID, 1, MyClient);
                        DroppedItem DI2 = new DroppedItem();
                        DI2.UID = (uint)Rnd.Next(10000000);
                        DI2.DropTime = DateTime.Now;
                        DI2.Loc = Loc;
                        DI2.Info = I;
                        DI2.Loc.X = (ushort)(DI2.Loc.X + Rnd.Next(4) - Rnd.Next(4));
                        DI2.Loc.Y = (ushort)(DI2.Loc.Y + Rnd.Next(4) - Rnd.Next(4));

                        if (DI2.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                            DI2.Drop();
                    }
                    Database.SaveCharacter(this, this.MyClient.AuthInfo.Account);
            }
            catch (Exception Exc) { Program.WriteLine(Exc); }
        }
    }
}
