using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Server.Features;
using Server.Extra;
using Server.Skills;
using System.IO;

namespace Server.Game
{
    public enum PKMode : byte
    {
        PK = 0,
        Peace = 1,
        Team = 2,
        Capture = 3
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
    public enum MerchantTypes : byte
    {
        Asking = 1,
        Not = 0,
        Yes = 255
    }
    [Flags]
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
        DragonCyclone = 0x800000,
        Fly = 0x8000000,
        Pray = 0x40000000,
        Blessing = 8589934592,
        TopGuildLeader = 17179869184,
        TopDeputyLeader = 34359738368,
        TopMonthly = 68719476736,
        TopWeekly = 137438953472,
        TopWarrior = 274877906944,
        TopTrojan = 549755813888,
        TopArcher = 1099511627776,
        TopWaterTaoist = 2199023255552,
        TopFireTaoist = 4398046511104,
        TopNinja = 72057594037927936,
        ShurikenVortex = 70368744177664,
        FatalStrike = 140737488355328,
        Flashy = 281474976710656,
        Ride = 1125899906842624,
        attack35 = 72057594037927936,
        Curse = 0x100000000
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
    public class StatusEffect
    {
        public ArrayList Array;
        public StatusEffectEn Value;
        Character Entity;
        Mob MEntity;
        Companion GEntity;
        public StatusEffect(Character C)
        {
            Entity = C;
            Array = new ArrayList();
            Value = StatusEffectEn.Normal;
        }
        public StatusEffect(Mob C)
        {
            MEntity = C;
            Array = new ArrayList();
            Value = StatusEffectEn.Normal;
        }
        public StatusEffect(Companion G)
        {
            GEntity = G;
            Array = new ArrayList();
            Value = (ulong)StatusEffectEn.Normal;
        }
        public bool Contains(StatusEffectEn SE)
        {
            return Array.Contains(SE);
        }
        public void Add(StatusEffectEn SE)
        {
            if (!Array.Contains(SE))
            {
                Array.Add(SE);
                Value |= SE;
                if (Entity != null)
                    World.Spawn(Entity, false);
                if (MEntity != null)
                    World.Spawn(MEntity, false);
                if (GEntity != null)
                    World.Spawn(GEntity, false);
            }
            if (Entity != null && Entity.MyClient != null)
            {
                Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
            }
        }
        public void Remove(StatusEffectEn SE)
        {
            if (Array.Contains(SE))
            {
                Array.Remove(SE);
                Value -= SE;
                if (Entity != null)
                    Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
                if (Entity != null)
                    World.Spawn(Entity, false);
                if (MEntity != null)
                    World.Spawn(MEntity, false);
                if (GEntity != null)
                    World.Spawn(GEntity, false);
            }
        }
        public void Clear()
        {
            Array = new ArrayList();
            Value = StatusEffectEn.Normal;
            if (Entity != null)
                Entity.MyClient.SendPacket(Packets.Status(Entity.EntityID, Status.Effect, (ulong)Value));
            if (Entity != null)
                World.Spawn(Entity, false);
            if (MEntity != null)
                World.Spawn(MEntity, false);
            if (GEntity != null)
                World.Spawn(GEntity, false);
        }
    }
    
    public class Item
    {
        public uint ID;
        public uint UID;

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
        public byte[] RBG = new byte[4];
        public uint Progress;
        public uint TalismanProgress;
        public ArmorColor Color;
        public RebornEffect Effect;

        public DatabaseItem ItemDBInfo
        {
            get
            {
                if (Database.DatabaseItems.Contains(ID))
                    return (DatabaseItem)Database.DatabaseItems[ID];
                return new DatabaseItem();
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
        public enum ItemQuality
        {
            Fixed = 0,
            NoUpgrade = 1,
            Simple = 3,
            Poor = 4,
            Normal = 5,
            Refined = 6,
            Unique = 7,
            Elite = 8,
            Super = 9
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

        public byte ItemPotency
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
        public static void GetGemDamage(ref EquipStats E, Gem G)
        {
            switch (G)
            {
                case Gem.NormalGloryGem:
                    {
                        E.TotalDamageDecrease += 100;
                        E.TotalMagicDamageDecrease += 100;
                        break;
                    }
                case Gem.RefinedGloryGem:
                    {
                        E.TotalDamageDecrease += 300;
                        E.TotalMagicDamageDecrease += 300;
                        break;
                    }
                case Gem.SuperGloryGem:
                    {
                        E.TotalDamageDecrease += 500;
                        E.TotalMagicDamageDecrease += 500;
                        break;
                    }
                case Gem.NormalThunderGem:
                    {
                        E.TotalDamageIncrease += 100;
                        E.TotalMagicDamageIncrease += 100;
                        break;
                    }
                case Gem.RefinedThunderGem:
                    {
                        E.TotalDamageIncrease += 300;
                        E.TotalMagicDamageIncrease += 300;
                        break;
                    }
                case Gem.SuperThunderGem:
                    {
                        E.TotalDamageIncrease += 500;
                        E.TotalMagicDamageIncrease += 500;
                        break;
                    }
                case Gem.NormalDragonGem:
                    {
                        E.TotalGemExtraAttack += 0.05;
                        break;
                    }
                case Gem.RefinedDragonGem:
                    {
                        E.TotalGemExtraAttack += 0.10;
                        break;
                    }
                case Gem.SuperDragonGem:
                    {
                        E.TotalGemExtraAttack += 0.15;
                        break;
                    }
                case Gem.SuperTortoiseGem:
                    {
                        E.TotalMDef2 += 0.02;
                        break;
                    }
                case Gem.RefinedTortoiseGem:
                    {
                        E.TotalMDef2 += 0.04;
                        break;
                    }
                case Gem.NormalTortoiseGem:
                    {
                        E.TotalMDef2 += 0.06;
                        break;
                    }
                case Gem.NormalRainbowGem:
                    {
                        E.TotalGemExtraEXP += 0.10;
                        break;
                    }
                case Gem.RefinedRainbowGem:
                    {
                        E.TotalGemExtraEXP += 0.15;
                        break;
                    }
                case Gem.SuperRainbowGem:
                    {
                        E.TotalGemExtraEXP += 0.25;
                        break;
                    }
                case Gem.NormalPhoenixGem:
                    {
                        E.TotalGemExtraMagicAttack += 0.05;
                        break;
                    }
                case Gem.RefinedPhoenixGem:
                    {
                        E.TotalGemExtraMagicAttack += 0.10;
                        break;
                    }
                case Gem.SuperPhoenixGem:
                    {
                        E.TotalGemExtraMagicAttack += 0.15;
                        break;
                    }
            }
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

            DatabaseItem DI = ItemDBInfo;
            ItemIDManipulation E = new ItemIDManipulation(ID);
            E.ChangeDigit(4, 0);
            uint ID2 = E.ToID();
            if (!EquipPassRbReq(this, C))
            {
                if (DI.Prof != 0)
                {
                    if (E.Digit(1) == 4 || E.Digit(1) == 5 || E.Digit(1) == 6)
                    {
                        if (C.Profs.Contains((ushort)E.Part(0, 3)))
                        {
                            Prof P = (Prof)C.Profs[(ushort)E.Part(0, 3)];
                            if (P.Lvl < DI.Prof)
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
            if (C.Level < Item.ItemDBInfo.Level)
                return false;
            else
                return true;
        }
        public static bool EquipPassRbReq(Item Item, Character C)
        {
            if (Item.ItemDBInfo.Level < 71 && C.Reborns > 0 && C.Level >= 70)
                return true;
            else
                return false;
        }
        public static bool EquipPassStatsReq(Item Item, Character C)
        {
            if (C.Str >= Item.ItemDBInfo.StrNeed && C.Agi >= Item.ItemDBInfo.AgiNeed)
                return true;
            else
                return false;
        }
        public static bool EquipPassJobReq(Item Item, Character C)
        {
            switch (Item.ItemDBInfo.Class)
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
                case 190: if (C.Job >= 100 && C.Job <= 145) return true; break;
                #endregion
                #region Avatar
                case 160: if (C.Job <= 165 && C.Job >= 160) return true; break;
                case 161: if (C.Job <= 165 && C.Job >= 161) return true; break;
                case 162: if (C.Job <= 165 && C.Job >= 162) return true; break;
                case 163: if (C.Job <= 165 && C.Job >= 163) return true; break;
                case 164: if (C.Job <= 165 && C.Job >= 164) return true; break;
                case 165: if (C.Job == 165) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static bool EquipPassSexReq(Item Item, Character C)
        {
            int ClientSex = C.Body % 3000 < 1005 ? 1 : 2;
            if (Item.ItemDBInfo.Job == 2 && ClientSex == 2)
                return true;
            if (Item.ItemDBInfo.Job != 2)
                return true;
            return false;
        }


        private static uint ItemUIDStart = 1;//Not Work
        private static uint ItemUIDFinish = uint.MaxValue - 1;//Not Work
        private static uint ItemNextID//Not Work
        {
            get
            {
                if (ItemUIDStart == ItemUIDFinish)
                    ItemUIDFinish = 1;
                return ItemUIDStart++;
            }
        }

        public string WriteThis2()
        {
            string item = "";
            if (ID != 0 && UID == 0) UID = (uint)Program.Rnd.Next(1, 99999999);
            if (ID != 0 && UID == 0) UID = (uint)World.Rnd.Next(10000000);
            item = UID + "-" + ID + "-" + Plus + "-" + Bless + "-" + Enchant + "-"
                + (byte)Soc1 + "-" + (byte)Soc2 + "-" + MaxDur + "-" + CurDur + "-"
                + (FreeItem == true ? "1" : "0") + "-" + TalismanProgress + "-" + Progress + "-" + (byte)Color + "-" + Locked;
            return item;
        }
        public void ReadThis2(string I)
        {
            string[] split = I.Split('-');
            if (split.Length < 12)
            {
                UID = 0;
                ID = 0;
                return;
            }
            UID = Convert.ToUInt32(split[0]);

            if (UID == 0) UID = (uint)Program.Rnd.Next(1, 99999999);
            if (UID == 0) UID = (uint)World.Rnd.Next(10000000);

            ID = Convert.ToUInt32(split[1]);
            if (Database.DatabaseItems.Contains(ID))
            {
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
            }
            else
            {
                UID = 0;
                ID = 0;
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
                RBG[0] = Convert.ToByte(split[15]);
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
                    byte Level = Item.Level;
                    string Type = Item.ID.ToString().Remove(2, Item.ID.ToString().Length - 2);
                    uint WeirdThing = Convert.ToUInt32(Type);
                    if (WeirdThing <= 60 && WeirdThing >= 42)//weapon
                    {
                        if (Level < 160)
                        {
                            if (Level >= 120 && Level < 130)
                            {
                                Level++;
                                foreach (DatabaseItem I in Database.DatabaseItems.Values)
                                {
                                    if (I.ID / 1000 == Item.ID / 1000)
                                        if (I.ID % 10 == Item.ID % 10)
                                            if (I.Level == Level)
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
                                            if (I.Level == Level)
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
                                    if (I.Level == Level)
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

            if (EqPos == 1 || EqPos == 2 || EqPos == 3 || EqPos == 6 || EqPos == 8 || EqPos == 10 || EqPos == 11)
            {
                ChangeDigit(6, 0);
            }
            else if (EqPos == 4 || EqPos == 5)
            {
                if (Part(0, 3) == 500 || Part(0, 3) == 421 || Part(0, 3) == 601 || Part(0, 3) == 610)
                    ChangeDigit(6, 0);
                else if (Digit(1) == 9)
                    ChangeDigit(6, 0);
                else if (Digit(1) == 4 || Digit(1) == 5)
                {
                    ChangeDigit(3, Digit(1));
                    ChangeDigit(2, Digit(1));
                    ChangeDigit(1, Digit(1));
                    ChangeDigit(6, 0);
                }
            }
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
            {
                C.MyClient.SendPacket(Packets.ItemPacket(Get(Pos).UID, Pos, 6));
                C.ReturnItemToInventory(Get(Pos));

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
        }
        public void Dissappear(byte Pos, Character C)
        {
            C.MyClient.SendPacket(Packets.ItemPacket(Get(Pos).UID, Pos, 6));

            C.MyClient.SendPacket(Packets.ItemPacket(Get(Pos).UID, Pos, 3));

            Database.DeleteItem(Get(Pos).UID,C.EntityID);

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
                        Console.WriteLine("a7aaaaaaaaaaaah");
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
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            else
            {
                C.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "here unable use thas on the map");
                return;
            }
        }

        public void Send(GameClient GC, bool Stats)
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
        public void SendView(uint Viewed, GameClient GC)
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
        public void ReadThis2(string Equips)
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
                HeadGear.ReadThis2(equips[0]);
                Necklace.ReadThis2(equips[1]);
                Armor.ReadThis2(equips[2]);
                RightHand.ReadThis2(equips[3]);
                LeftHand.ReadThis2(equips[4]);
                Ring.ReadThis2(equips[5]);
                Gourd.ReadThis2(equips[6]);
                Boots.ReadThis2(equips[7]);
                Garment.ReadThis2(equips[8]);
                Fan.ReadThis2(equips[9]);
                Tower.ReadThis2(equips[10]);
                Steed.ReadThis2(equips[11]);
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
    public struct EquipStats
    {
        //public uint FanMagicAtack;
        //public uint TowerMagicDefense;
        //public uint FanAttack;
        //public uint TowerDefense;
        public uint TotalMinAttack;
        public uint TotalMaxAttack;
        public uint TotalMagicAttack;
        public ushort TotalDefense;
        public double TotalGemExtraEXP;
        public double TotalGemExtraAttack;
        public double TotalGemExtraMagicAttack;
        public uint TotalHP;
        public uint TotalMP;
        public ushort TotalAgility;
        public byte TotalDodge;
        ushort _TotalMDef1;
        public ushort TotalMDef1
        {
            set
            {
                _TotalMDef1 = value;
            }
            get
            {
                if (_TotalMDef1 > 100)
                    return 100;
                else
                    return _TotalMDef1;
            }
        }
        public double TotalMDef2;// TortoiseGem

        public ushort TotalEquipPotency;

        public uint TotalDamageIncrease;
        public uint TotalDamageDecrease;
        public uint TotalMagicDamageIncrease;
        public uint TotalMagicDamageDecrease;

        public byte TotalBless;

        public byte TotalRideSpeed;
        public ushort TotalVigor;

        public static EquipStats operator +(EquipStats Eqp, EquipStats eqp)
        {
            //Eqp.FanMagicAtack += eqp.FanMagicAtack;
            //Eqp.FanAttack += eqp.FanAttack;
            //Eqp.TowerMagicDefense += eqp.TowerMagicDefense;
            //Eqp.TowerDefense += eqp.TowerDefense;
            Eqp.TotalMinAttack += eqp.TotalMinAttack;
            Eqp.TotalMaxAttack += eqp.TotalMaxAttack;
            Eqp.TotalMagicAttack += eqp.TotalMagicAttack;
            Eqp.TotalDefense += eqp.TotalDefense;
            Eqp.TotalGemExtraEXP += eqp.TotalGemExtraEXP;
            Eqp.TotalGemExtraAttack += eqp.TotalGemExtraAttack;
            Eqp.TotalGemExtraMagicAttack += eqp.TotalGemExtraMagicAttack;
            Eqp.TotalHP += eqp.TotalHP;
            Eqp.TotalMP += eqp.TotalMP;
            Eqp.TotalDodge += eqp.TotalDodge;
            Eqp.TotalMDef1 += eqp.TotalMDef1;
            Eqp.TotalMDef2 += eqp.TotalMDef2;
            Eqp.TotalAgility += eqp.TotalAgility;
            Eqp.TotalEquipPotency += eqp.TotalEquipPotency;
            Eqp.TotalMagicDamageDecrease += eqp.TotalMagicDamageDecrease;
            Eqp.TotalDamageDecrease += eqp.TotalDamageDecrease;
            Eqp.TotalMagicDamageIncrease += eqp.TotalMagicDamageIncrease;
            Eqp.TotalDamageIncrease += eqp.TotalDamageIncrease;
            Eqp.TotalBless += eqp.TotalBless;
            Eqp.TotalRideSpeed += eqp.TotalRideSpeed;
            Eqp.TotalVigor += eqp.TotalVigor;
            return Eqp;
        }
        public static EquipStats operator -(EquipStats Eqp, EquipStats eqp)
        {
            //Eqp.FanMagicAtack -= eqp.FanMagicAtack;
            //Eqp.FanAttack -= eqp.FanAttack;
            //Eqp.TowerMagicDefense -= eqp.TowerMagicDefense;
            //Eqp.TowerDefense -= eqp.TowerDefense;
            Eqp.TotalMinAttack -= eqp.TotalMinAttack;
            Eqp.TotalMaxAttack -= eqp.TotalMaxAttack;
            Eqp.TotalMagicAttack -= eqp.TotalMagicAttack;
            Eqp.TotalDefense -= eqp.TotalDefense;
            Eqp.TotalGemExtraEXP -= eqp.TotalGemExtraEXP;
            Eqp.TotalGemExtraAttack -= eqp.TotalGemExtraAttack;
            Eqp.TotalGemExtraMagicAttack -= eqp.TotalGemExtraMagicAttack;
            Eqp.TotalHP -= eqp.TotalHP;
            Eqp.TotalMP -= eqp.TotalMP;
            Eqp.TotalDodge -= eqp.TotalDodge;
            Eqp.TotalMDef1 -= eqp.TotalMDef1;
            Eqp.TotalMDef2 -= eqp.TotalMDef2;
            Eqp.TotalAgility -= eqp.TotalAgility;
            Eqp.TotalEquipPotency -= eqp.TotalEquipPotency;
            Eqp.TotalMagicDamageDecrease -= eqp.TotalMagicDamageDecrease;
            Eqp.TotalDamageDecrease -= eqp.TotalDamageDecrease;
            Eqp.TotalMagicDamageIncrease -= eqp.TotalMagicDamageIncrease;
            Eqp.TotalDamageIncrease -= eqp.TotalDamageIncrease;
            Eqp.TotalBless -= eqp.TotalBless;
            Eqp.TotalRideSpeed -= eqp.TotalRideSpeed;
            Eqp.TotalVigor -= eqp.TotalVigor;
            return Eqp;
        }
    }
    public struct Banks
    {
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
        public void ReadThis2(string line)
        {
            string[] whs = line.Split('#');

            TCWarehouse = new ArrayList(20);
            if (whs.Length >= 1)
            {
                string[] tc = whs[0].Split('~');
                for (byte i = 0; i < tc.Length; i++)
                {
                    Item I = new Item();
                    I.ReadThis2(tc[i]);
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
                    I.ReadThis2(pc[i]);
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
                    I.ReadThis2(ac[i]);
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
                    I.ReadThis2(dc[i]);
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
                    I.ReadThis2(bi[i]);
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
                    I.ReadThis2(ma[i]);
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
                    I.ReadThis2(sc[i]);
                    if (I.ID != 0)
                        SCWarehouse.Add(I);
                }
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
    public struct Location
    {
        public ushort X;
        public ushort Y;
        public ushort Map;
        public uint MapDimention;

        public ushort PreviousX;
        public ushort PreviousY;
        public ushort PreviousMap;
        public DateTime LastJump;

        public void SteedWalk(byte Dir)
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
                DMap DM = ((DMap)DMaps.H_Maps[Map]);
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
    public struct Friend
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
    public class PoisonType
    {
        public int Times;
        public int SpellLevel;
        public DateTime LastAttack;
        public PoisonType(int spelllvl)
        {
            Times = 10;
            SpellLevel = spelllvl;
            LastAttack = DateTime.Now;
        }
    }
    public class Prof
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

        public SkillsClass.SkillInfo Info
        {
            get
            {
                if (SkillsClass.SkillInfos.Contains(ID + " " + Lvl))
                    return (SkillsClass.SkillInfo)SkillsClass.SkillInfos[ID + " " + Lvl];
                return new SkillsClass.SkillInfo();
            }
        }
        public void ReadThis(System.IO.BinaryReader I)
        {
            ID = I.ReadUInt16();
            Lvl = I.ReadByte();
            Exp = I.ReadUInt32();
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

    public class Character
    {
        //System.Timers.Timer Timer;
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

            InternTaoist = 100,
            Taoist = 101,
            WaterTaoist = 132,
            WaterSaint = 133,
            WaterMaster = 134,
            WaterWizard = 135,
            FireTaoist = 142,
            FireSaint = 143,
            FireMaster = 144,
            FireWizard = 145,
            
            Bender = 160,
            AirBender = 161,
            WaterBender = 162,
            EarthBender = 163,
            FireBender = 164,
            Avatar = 165
        }

        #region VS Arena
        public DateTime PVPCuonter;
        public byte PkPuntos = 0; // 1v1
        public bool Luchando = false; // 1v1
        public int Apuesta = 0; // 1v1
        public string Enemigo = ""; // 1v1
        public bool FreeBattel = false; // 1v1
        public ushort VSmaptogo = 1707; // 1v1
        public byte VsLimetTimeLastes = 5; // 1v1
        public byte VSpets = 5;// 1v1
        #endregion
        #region DeathMatch
        public Extra.Teams MyTDmTeam = new Extra.Teams();
        public int dmjoin = 0;
        public int dmred = 0;
        public int dmblack = 0;
        public int dmblue = 0;
        public int dmwhite = 0;
        public int srjoin = 0;
        public bool InPKT = false;
        #endregion

        public byte mtype = 0;
        public byte mtype2 = 0;
        public uint InteractionType = 0;
        public uint InteractionWith = 0;
        public ushort InteractionX = 0;
        public ushort InteractionY = 0;
        public bool InteractionSet = false;
        public bool InteractionInProgress = false;

        public string AccountName;
        public uint EntityID;
        public bool Alive = true;
        public bool Ghost = false;
        public bool Mining = false;
        public byte Action = 100;
        public byte Direction = 0;
        public int House = 0;
        public bool dropedAnEquep = false;
        public bool Lottery = false;
        public byte LotteryUsed = 0;
        public byte ExpBallsUsedToday = 0;
        public byte PEBUsedToday = 0;
        public byte DbUsedToday = 0;
        public bool InOTG = false;
        public double TrainTimeLeft;
        public byte OnlineTrainingPts = 0;
        public bool Protection = false;


        public ushort EnhligtehnRequest = 0;
        public byte ElightenAdd = 0;
        public ulong ElighemPoints = 0;
        public DateTime ElightenRequestTime = DateTime.Now;
        public DateTime LastMine = DateTime.Now;
        public DateTime LastBuffRemove = DateTime.Now;
        public DateTime ExpPotionUsed = DateTime.Now;
        public DateTime LoggedOn = DateTime.Now;
        public DateTime LastLogin = DateTime.Now;
        public DateTime LastProtection = DateTime.Now;
        public DateTime LastTele = DateTime.Now;
        public DateTime BlueNameStarted;
        //public DateTime AtackTime = DateTime.Now;
        public DateTime LastPKPLost = DateTime.Now;
        public DateTime LastXP = DateTime.Now;
        public DateTime LastStamina = DateTime.Now;
        public DateTime DeathHit = DateTime.Now;

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

        public Avatar MyAvatar;
        public Character Owner;
       
        public Dengun myDengun1;
        //public Avatar avatar;
        #region ROBOT

        public string robotaccount = "";
        public string robotPassword = "";
        public bool OwnerSigned;
        public byte SummonCoast;
        public bool AvaRunning = false;
        public DateTime AvaSummoned;
        public int avaLasts;
        public bool isAvatar = false;

        //public bool IkilledMyAva = false;
        public bool MyAvatarKilledMe = false;

        public bool ownerpk = false;
        public byte AvatarLevel = 0;
        public DateTime lastHPUse = DateTime.Now;
        public DateTime lastMPUse = DateTime.Now;
        public string Process = "";

        #endregion

        
        public bool highstmna = false;
        public byte superGem = 0;
        public byte addBless = 0;
        public bool DoubleExp;
        public int DoubleExpLeft;
        public int BlessingLasts;
        public DateTime BlessingStarted;
        public DateTime LastPts = DateTime.Now;
        public int PrayTimeLeft;
        public Nobility Nobility;
        public uint RequestFriends = 0;

        #region Trade
        public uint TradingWith = 0;
        public ArrayList TradeSide = new ArrayList(20);
        public uint TradingSilvers = 0;
        public uint TradingCPs = 0;
        public bool Trading = false;
        public bool ClickedOK = false;
        #endregion

        public int banned = 0;
        public string BanBy = "";
        public int DisKO = 0;
        public int flames = 0;
        public DateTime HeartTimer = DateTime.Now;
        public int rebornquest = 0;
        public DateTime SteedRaceTime;

        public int FreeGear = 0;
        public int Flori = 0;
        public int quest1 = 0;

        #region Beginner Quests
        public int MonsterHunted = 0;
        public string MonsterName = "";
        public bool AbleToHunt = false;
        #endregion

        public int HonorPoints = 0;
        public Random Rnd = new Random();
        public int XPKO = 0;
        public int TotalKO = 0;
        public byte Killmap = 0;
        public int cp = 0;
        public Exterminator e_quest;
        public string PlayerLanguage = "en-US";

        public uint LuckyTime = 0;
        public bool GettingLuckyTime = false;
        public bool Prayer = false;
        public Character ThePrayer;
        public DateTime PrayDT;

        public DateTime lastJumpTime = DateTime.Now;
        public short lastJumpDistance = 0;
        public bool FlowerExist = false;
        public string FlowerType = "";
        public string FlowerName = "";
        public DateTime kingsMove;
        public bool guardsArmy = false;
        public Struct.Flowers Flowers = new Struct.Flowers();
        public PacketHandling.MarketShops.Shop MyShop;
        public DateTime UnableToUseDrugs;
        public ushort UnableToUseDrugsFor;
        public PoisonType PoisonedInfo = null;
        public uint TradePartnerWith = 0;
        public bool VortexOn = false;
        public DateTime LastVortexAttk = DateTime.Now;
        public bool LoadedEquipmentHPAdd = false;

        public Dictionary<uint, string> TradePartners = new Dictionary<uint, string>();
        public long TimePartner = 0;
        public uint RequestPartnerWith = 0;

        public Dictionary<uint, Game.Item> ConfiscatorClain = new Dictionary<uint, Game.Item>();
        public Dictionary<uint, Game.Item> ConfiscatorReward = new Dictionary<uint, Game.Item>();
        public Dictionary<uint, Game.Item> Inventory = new Dictionary<uint, Game.Item>();
        public Dictionary<uint, TradePartner> Partners = new Dictionary<uint, TradePartner>();        
        public Equipment Equips;
        public Banks Warehouses;
        public GuildRank GuildRank;
        public MemberInfo MembInfo;
        public Hashtable Skills;
        public Hashtable Profs;
        public Hashtable Friends;
        public Hashtable Enemies;

        public ArrayList Buffs;
        public Location Loc;
        public StatusEffect StatEff;

        public Features.Team MyTeam;
        public bool TeamLeader = false;

        public uint Donation;
        public uint GuildDonation;
        public Guild MyGuild;
        public EquipStats EqStats;
        public byte oldlevel = 0;
        public GameClient MyClient;

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

        public string TempPass = "";
        public bool WHOpen = false;
        public int WHOpenChance = 0;

        public bool CurseStart;
        public int CurseExpLeft;
        public DateTime CurseUser = DateTime.Now;

        public AttackMemorise AtkMem;
        public MerchantTypes Merchant;

        public bool Loaded = false;
        public byte BlueNameLasts;
        public bool CanReflect = false;
        public ulong VP;

        public void SendScreen(byte[] Data)
        {
            Game.Character[] Chars = new Game.Character[Game.World.H_Chars.Count];
            Game.World.H_Chars.Values.CopyTo(Chars, 0);
            foreach (Character C in Chars)
                if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                    C.MyClient.SendPacket2(Data);
            Chars = null;
        }
        
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopTrojan);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopWarrior);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopNinja);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopWaterTaoist = 0;
        public int TopWaterTaoist
        {
            get { return _TopWaterTaoist; }
            set
            {
                _TopWaterTaoist = value;
                if (value >= 1)
                {
                    StatEff.Add(Server.Game.StatusEffectEn.TopWaterTaoist);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopArcher);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopFireTaoist);
                    Database.SaveTop(this);
                }
            }
        }
        public int _TopWeekly = 0;
        public int TopWeekly
        {
            get { return _TopWeekly; }
            set
            {
                _TopWeekly = value;
                  if (value >= 1)
                      StatEff.Add(Server.Game.StatusEffectEn.TopWeekly);
            }
        }
        public int _TopMonthly = 0;
        public int TopMonthly
        {
            get { return _TopMonthly; }
            set
            {
                _TopMonthly = value;
                if (value >= 1)
                    StatEff.Add(Server.Game.StatusEffectEn.TopMonthly);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopGuildLeader);
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
                    StatEff.Add(Server.Game.StatusEffectEn.TopDeputyLeader);
                    Database.SaveTop(this);
                }
            }
        }

        byte _PreviousJob1;
        public byte PreviousJob1
        {
            get { return _PreviousJob1; }
            set
            {
                _PreviousJob1 = value;
                if (value < byte.MaxValue)
                    Database.SavePreviousjob1(this);
            }
        }
        byte _PreviousJob2;
        public byte PreviousJob2
        {
            get { return _PreviousJob2; }
            set
            {
                _PreviousJob2 = value; if (value < byte.MaxValue)
                    Database.SavePreviousjob2(this);
            }
        }
        private uint _UniversityPoints = 0;
        public uint UniversityPoints
        {
            get { return _UniversityPoints; }
            set
            {
                _UniversityPoints = value;
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        //Database.SaveUniversity(this);
                        SendScreen(Packets.Status(EntityID, Status.QuizPts, _UniversityPoints));
                }
            }
        }

        uint _CPSDonate;
        public uint CPSDonate
        {
            get { return _CPSDonate; }
            set
            {
                _CPSDonate = value;
                if (Loaded)
                    MyClient.SendPacket(Packets.Status(EntityID, Status.CPSDOnators, _CPSDonate));
            }
        }
        byte _Stamina = 0;
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
        ushort _Vigor = 0;
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
            get { return (ushort)(30 + EqStats.TotalVigor); }
        }
        ushort _Avatar;
        public ushort Avatar
        {
            get { return _Avatar; }
            set
            {
                _Avatar = value;
                if (Loaded)
                {
                    Database.SaveAvatar(this);
                    SendScreen(Packets.Status(EntityID, Status.Mesh, uint.Parse(_Avatar.ToString() + _Body.ToString())));

                }
            }
        }
        ushort _Body;
        public ushort Body
        {
            get { return _Body; }
            set
            {
                _Body = value; if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.savebody(this);
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
        ushort _Hair;
        public ushort Hair
        {
            get { return _Hair; }
            set
            {
                _Hair = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        Database.SaveHair(this);
                    SendScreen(Packets.Status(EntityID, Status.Hair, _Hair));
                }
            }
        }
        byte _Job;
        public byte Job
        {
            get { return _Job; }
            set
            {
                _Job = value;
                if (Loaded)
                {
                    if (value < byte.MaxValue)
                        Database.SaveJob(this);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.Class, _Job));
                    if (!Reborn && Level <= 120)
                        Database.GetStats(this);
                }
            }
        }
        byte _Level;
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
                        //Database.SaveLevel(this);
                        World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 92));
                    SendScreen(Packets.Status(EntityID, Status.Level, _Level));

                    if (!Reborn && PrevLev < 120)
                        Database.GetStats(this);
                }
            }
        }
        ulong _Experience;
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
        ushort _Str;
        public ushort Str
        {
            get { return _Str; }
            set
            {
                _Str = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        //Database.SaveStr(this);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Strength, _Str));
                }
            }
        }
        ushort _Agi;
        public ushort Agi
        {
            get { return _Agi; }
            set
            {
                _Agi = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        //Database.SaveAgi(this);

                        MyClient.SendPacket(Packets.Status(EntityID, Status.Agility, _Agi));
                }
            }
        }
        ushort _Vit;
        public ushort Vit
        {
            get { return _Vit; }
            set
            {
                _Vit = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        //Database.SaveVit(this);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Vitality, _Vit));
                }
            }
        }
        ushort _Spi;
        public ushort Spi
        {
            get { return _Spi; }
            set
            {
                _Spi = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        //Database.SaveSpi(this);

                        MyClient.SendPacket(Packets.Status(EntityID, Status.Spirit, _Spi));
                }
            }
        }
        ushort _StatusPoints;
        public ushort StatusPoints
        {
            get { return _StatusPoints; }
            set
            {
                _StatusPoints = value;
                if (Loaded)
                {
                    if (value < ushort.MaxValue)
                        //Database.SaveCharStatus(this);

                        MyClient.SendPacket(Packets.Status(EntityID, Status.StatPoints, _StatusPoints));
                }
            }
        }
        ushort _CurHP;
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
                        //Database.savelife(this);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.HP, _CurHP));
                }
            }
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
                Rt += EqStats.TotalHP;
                ushort Rtt = 0;
                if (Rt > ushort.MaxValue)
                    Rtt = ushort.MaxValue;
                else
                    Rtt = (ushort)Rt;
                return Rtt;
            }
        }
        ushort _CurMP;
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
                        //Database.savemana(this);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.MP, _CurMP));
                }
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

                return (ushort)(mp + EqStats.TotalMP);
            }
        }
        uint _Silvers;
        public uint Silvers
        {
            get { return _Silvers; }
            set
            {
                _Silvers = value;
                if (_Silvers > 999999999)
                {
                    if (Name == "Server")
                    {
                        _Silvers = 999999999;
                        MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You Cant Get Over 1000M Silvers In Your Inventory.");
                    }
                    else
                    {
                        _Silvers = 999999999;
                        Program.WriteMessage("ALERRRRRRRRRRRRRRRRRRT (SILVERS) ERRRRRRORR" + Name);
                    }
                }
                if (Loaded)
                {

                    if (value < uint.MaxValue)
                        //Database.SaveSilver(this, value);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.Silvers, _Silvers));
                }
            }

        }
        uint _CPs;
        public uint CPs
        {
            get { return _CPs; }
            set
            {
                _CPs = value;
                if (_CPs > 999999999)
                {
                    if (Name == "Server")
                    {
                        _CPs = 999999999;
                        MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You Cant Get Over 1000M Cps In Your Inventory.");
                    }
                    else
                    {
                        _CPs = 999999999;
                        Program.WriteMessage("ALERRRRRRRRRRRRRRRRRRT (CPs) ERRRRRRORR" + Name);

                    }
                }
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        // Database.SaveCps(this, value);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.CPs, _CPs));
                }
            }
        }
        uint _WHSilvers;
        public uint WHSilvers
        {
            get { return _WHSilvers; }
            set
            {
                _WHSilvers = value;
                if (Loaded)
                {
                    if (value < uint.MaxValue)
                        Database.SaveWhSilver(this);
                    MyClient.SendPacket(Packets.Status(EntityID, Status.WHMoney, _WHSilvers));
                }
            }
        }
        ushort _PKPoints;
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
        byte _Viplevel = 0;
        public byte VipLevel
        {
            get { return _Viplevel; }
            set
            {
                _Viplevel = value;
                if (Loaded)
                {
                    try
                    {
                        if (value < byte.MaxValue)
                            Database.savevip(this);
                        MyClient.SendPacket(Packets.Status(EntityID, Status.VIPLevel, value));
                    }
                    catch { }
                }
            }
        }
        bool _BlueName = false;
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
                short t = 800;
                t -= (short)(Agi + EqStats.TotalAgility);
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
        private byte _Reborns;
        public byte Reborns
        {
            get { return _Reborns; }
            set
            {
                _Reborns = value;
                if (Loaded)
                {
                    if (value < byte.MaxValue)
                        Database.SaveReborn(this);
                    SendScreen(Packets.Status(EntityID, Status.RebirthCount, Reborns));
                }
            }
        }
        public bool Reborn
        {
            get { return Reborns > 0; }
        }

        public Character()
        {
            EqStats = new EquipStats();
            EqStats.TotalGemExtraMagicAttack = 1;
            EqStats.TotalGemExtraEXP = 1;
            EqStats.TotalGemExtraAttack = 1;
            StatEff = new StatusEffect(this);
            AtkMem = new AttackMemorise();
            AtkMem.Attacking = false;
            AtkMem.LastAttack = DateTime.Now;
            AtkMem.Target = 0;
            Buffs = new ArrayList();
            Nobility = new Nobility(this);

           
            //Timer = new System.Timers.Timer();
            //Timer.Interval = 150;
            //Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            //Timer.Start();
        }
        public DateTime LastSaves = DateTime.Now;
        public DateTime LastSave = DateTime.Now;
        public DateTime LastSave2 = DateTime.Now;

        //void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        if (!Program.EndSession)
        //            if (MyClient != null)
        //                Step();
        //    }
        //    catch { }
        //}

        public void Step()
        {
            try
            {
                if (this != null)
                {



                    if (MyAvatarKilledMe && DateTime.Now > DeathHit.AddSeconds(4))
                    {
                        PacketHandling.ForceReviveHere.Handle(MyClient);
                        MyAvatarKilledMe = false;
                    }
                    try
                    {
                        if (Loaded)
                            if (DateTime.Now > LastSave2.AddMinutes(5))
                            {
                                
                                Database.SaveSkills(this);
                                Database.SaveProfs(this);
                                LastSave2 = DateTime.Now;
                            }
                    }
                    catch (Exception e) { Program.WriteMessage(e); }
                    
                    try
                    {
                        if (Loaded && !MyClient.Robot)
                            if (DateTime.Now > LastSave.AddSeconds(30) && World.H_Chars.Contains(EntityID))
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
                                if (CurHP <= (uint)(MaxHP / 7))
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
                                //TakeAttack(this, Dmg, AttackType.Melee, false);
                                GetPoisond(SkillsClass.DamageType.Melee);
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
                        #region robot
                        if (isAvatar)
                        {
                            if (Owner == null && OwnerSigned)
                            {
                                MyClient.Disconnect();
                                Console.WriteLine("Owner is null [DC]");
                            }
                            if (OwnerSigned)
                            {
                                ((Avatar)this).step();
                            }
                            
                            
                        }
                        #endregion
                        #region Blessing
                        if (BlessingLasts > 0 && DateTime.Now > LastPts.AddMinutes(1))
                        {
                            //Chaar.OnlineTrainingPts += 10;
                            LastPts = DateTime.Now;
                            //Chaar.MyClient.SendPacket(Packets.Status(Chaar.EntityID, Status.OnlineTraining, 3));
                            if (OnlineTrainingPts == 100)
                            {
                                OnlineTrainingPts = 0;
                                IncreaseExp(EXPBall / 90 * World.ServerInfo.ExperienceRate, false);
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
                        #region Enlighten
                        if (EnhligtehnRequest >= 100 && DateTime.Now > ElightenRequestTime.AddMinutes(20))
                        {
                            EnhligtehnRequest -= 100;
                            ElightenRequestTime = DateTime.Now;
                            MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You time englighten is " + EnhligtehnRequest / 5 + " mintes of " + (EnhligtehnRequest + 100) / 5 + " ");
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
                            LastBuffRemove = DateTime.Now;
                            Ghost = true;
                            MyClient.SendPacket(Packets.Status(EntityID, Status.Hair, 0));
                            string Avt = "0";
                            if (Avatar.ToString().Length == 1)
                                Avt = "00" + Avatar.ToString();
                            else if (Avatar.ToString().Length == 2)
                                Avt = "0" + Avatar.ToString();

                            else Avt = Avatar.ToString(); Console.WriteLine(Avt);
                            if (Body == 1003 || Body == 1004)
                                MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, uint.Parse("98" + Avt + Body.ToString())));
                            else
                                MyClient.SendPacket(Packets.Status(EntityID, Status.Mesh, uint.Parse("99" + Avt + Body.ToString())));
                            World.Spawn(this, false);
                        }
                        #endregion
                        #region Stamina | LukyTime | Revive | effect
                        if ((DateTime.Now > HeartTimer.AddMilliseconds(1000)))
                        {
                            if (CurHP < MaxHP && Action == 250)
                            {
                                CurHP += (ushort)(MaxHP * 0.0033);
                            }
                            else if (CurHP < MaxHP && Action != 250)
                            {
                                CurHP += (ushort)(MaxHP * 0.0015);

                            }
                            try
                            {
                                CurHP = Math.Min(CurHP, MaxHP);
                            }
                            catch { }
                            try
                            {
                                foreach (Item I in Inventory.Values)
                                    if (!Database.DatabaseItems.Contains(I.ID) || I.ID == 0)
                                        Inventory.Remove(I.UID);
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
                                        if (C.StatEff.Contains(StatusEffectEn.Pray) && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention && MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= 3)
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
                                    if (ThePrayer != null && Game.World.H_Chars.Contains(ThePrayer.EntityID) && ThePrayer.StatEff.Contains(StatusEffectEn.Pray) && Loc.Map == ThePrayer.Loc.Map && Loc.MapDimention == ThePrayer.Loc.MapDimention && MyMath.PointDistance(Loc.X, Loc.Y, ThePrayer.Loc.X, ThePrayer.Loc.Y) <= 3)
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
                                if (Vigor < MaxVigor)

                                    Vigor += 30;
                                if ((DateTime.Now > LastStamina.AddMilliseconds(1000) && (Stamina < 150) && Stamina <= 150 && !StatEff.Contains(Server.Game.StatusEffectEn.Fly)) && !isAvatar)
                                {
                                    if (Action == 250)
                                        Stamina += 15;
                                    else
                                        Stamina += 5;
                                    LastStamina = DateTime.Now;
                                }
                                if (isAvatar && DateTime.Now > LastStamina.AddMilliseconds(1000) && (Stamina < 150) && Stamina <= 150)
                                {
                                    Stamina += 10;
                                    LastStamina = DateTime.Now;
                                }
                                HeartTimer = DateTime.Now;
                            }
                            catch { }
                            if (myDengun1 != null)
                            {
                                if (myDengun1.Hero == this)
                                {
                                    myDengun1.step();
                                }
                            }
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
                            if (Alive && !StatEff.Contains(StatusEffectEn.XPStart))
                            {
                                LastXP = DateTime.Now;
                                StatEff.Add(StatusEffectEn.XPStart);
                                Buffs.Add(new Buff() { StEff = StatusEffectEn.XPStart, Lasts = 20, Started = DateTime.Now, Eff = SkillsClass.ExtraEffect.None });
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
                                if (B.Eff == SkillsClass.ExtraEffect.Cyclone || B.Eff == SkillsClass.ExtraEffect.Superman)
                                {
                                    Time = (ushort)(B.Lasts + XPKO);
                                    if (Time > 60)
                                        Time = 60;
                                }
                                if (DateTime.Now > B.Started.AddSeconds(Time))
                                {
                                    if (B.Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                                        VortexOn = false;
                                    BDelete.Add(B);
                                }
                            }
                            bool had = false;
                            bool stillhas = false;
                            foreach (Buff B in BDelete)
                            {
                                RemoveBuff(B);
                                if (B.Eff == SkillsClass.ExtraEffect.Cyclone || B.Eff == SkillsClass.ExtraEffect.Superman)
                                {
                                    had = true;
                                    if (BuffOf(SkillsClass.ExtraEffect.Cyclone).Eff == SkillsClass.ExtraEffect.Cyclone || BuffOf(SkillsClass.ExtraEffect.Cyclone).Eff == SkillsClass.ExtraEffect.Superman)
                                        stillhas = true;
                                }
                            }
                            if (had)
                            {
                                if (!stillhas)
                                {
                                    if (Job < 160)
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
                                //Hashtable MapMobs = (Hashtable)World.H_Mobs[Loc.Map];
                                Skills.SkillsClass.SkillUse SU = new Server.Skills.SkillsClass.SkillUse();
                                SU.Init(this, S.ID, S.Lvl, Loc.X, Loc.Y);
                                SU.Info.ExtraEff = SkillsClass.ExtraEffect.None;
                                SU.Info.Damageing = SkillsClass.DamageType.Melee;
                                SU.Info.Targetting = SkillsClass.TargetType.Range;
                                SU.GetTargets(1);
                                //if (MapMobs != null)
                                //    foreach (Mob M in MapMobs.Values)
                                //    {
                                //        if (M.Alive)
                                //        {
                                //            if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= Dist)
                                //                if (PKMode == PKMode.PK || !M.NeedsPKMode && !SU.MobTargets.Contains(M))
                                //                    SU.MobTargets.Add(M, SU.GetDamage(M));
                                //        }
                                //    }
                                //foreach (NPC C in World.H_NPCs.Values)
                                //{
                                //    if ((C.Flags == 21 || C.Flags == 22) && Level >= C.Level)
                                //    {
                                //        if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                //            if (!SU.NPCTargets.Contains(C))
                                //                SU.NPCTargets.Add(C, SU.GetDamage(C));
                                //    }
                                //}
                                //foreach (Character C in World.H_Chars.Values)
                                //{

                                //    {
                                //        if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist)
                                //            if (C.EntityID != EntityID)
                                //                SU.GetTargets(C.EntityID);
                                //    }
                                //}

                                if (MyMath.PointDistance(Loc.X, Loc.Y, Extra.GuildWars.ThePole.Loc.X, Extra.GuildWars.ThePole.Loc.Y) <= Dist && Extra.GuildWars.War && MyGuild != Extra.GuildWars.LastWinner)
                                    SU.MiscTargets.Add(Extra.GuildWars.ThePole.EntityID, SU.GetDamage(Extra.GuildWars.ThePole.CurHP));

                                if (MyMath.PointDistance(Loc.X, Loc.Y, Extra.GuildWars.TheRightGate.Loc.X, Extra.GuildWars.TheRightGate.Loc.Y) <= Dist)
                                    SU.MiscTargets.Add(Extra.GuildWars.TheRightGate.EntityID, SU.GetDamage(Extra.GuildWars.TheRightGate.CurHP));

                                if (MyMath.PointDistance(Loc.X, Loc.Y, Extra.GuildWars.TheLeftGate.Loc.X, Extra.GuildWars.TheLeftGate.Loc.Y) <= Dist)
                                    SU.MiscTargets.Add(Extra.GuildWars.TheLeftGate.EntityID, SU.GetDamage(Extra.GuildWars.TheLeftGate.CurHP));

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
                                    Game.Companion PossCompanin = null;

                                    if (Game.World.H_Mobs.Contains(Loc.Map))
                                    {
                                        Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                                        if (MapMobs.Contains(AtkMem.Target))
                                            PossMob = (Game.Mob)MapMobs[AtkMem.Target];
                                        else if (Game.World.H_Chars.Contains(AtkMem.Target))
                                            PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                        else if (Game.World.H_Companions.Contains(AtkMem.Target))
                                            PossCompanin = (Game.Companion)Game.World.H_Companions[AtkMem.Target];
                                    }
                                    else
                                    {
                                        if (Game.World.H_Chars.Contains(AtkMem.Target))
                                            PossChar = (Game.Character)Game.World.H_Chars[AtkMem.Target];
                                        else if (Game.World.H_Companions.Contains(AtkMem.Target))
                                            PossCompanin = (Game.Companion)Game.World.H_Companions[AtkMem.Target];
                                    }
                                    if ((PossCompanin != null && PossCompanin.Owner.PKAble(PKMode, this)) || PossMob != null || PossChar != null)
                                    {
                                        AtkMem.LastAttack = DateTime.Now;
                                        uint Damage = PrepareAttack((byte)AtkMem.AtkType, true);
                                        if (PossMob != null && PossMob.Alive && (MyMath.PointDistance(Loc.X, Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 2 || AtkMem.AtkType == 28 || StatEff.Contains(StatusEffectEn.FatalStrike) && MyMath.PointDistance(Loc.X, Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 15))
                                        {
                                            if (!WeaponSkill(PossMob.Loc.X, PossMob.Loc.Y, PossMob.EntityID))
                                                PossMob.TakeAttack(this, ref Damage, (Server.Game.MobAttackType)AtkMem.AtkType, false);
                                        }
                                        else if (PossChar != null && ((PossChar.StatEff.Contains(Game.StatusEffectEn.Fly) && PossChar.StatEff.Contains(Game.StatusEffectEn.Ride) || !PossChar.StatEff.Contains(Game.StatusEffectEn.Fly)) && !PossChar.StatEff.Contains(Game.StatusEffectEn.Invisible) || AtkMem.AtkType != 2) && PossChar.Alive && (MyMath.PointDistance(Loc.X, Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 2 || AtkMem.AtkType == 28 && MyMath.PointDistance(Loc.X, Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 15))
                                        {
                                            if (!WeaponSkill(PossChar.Loc.X, PossChar.Loc.Y, PossChar.EntityID))
                                                PossChar.TakeAttack(this, Damage, (Server.Game.MobAttackType)AtkMem.AtkType, false);
                                        }
                                        else if (PossCompanin != null && PossCompanin.Alive && (MyMath.PointDistance(Loc.X, Loc.Y, PossCompanin.Loc.X, PossCompanin.Loc.Y) <= 2 || AtkMem.AtkType == 28 && MyMath.PointDistance(Loc.X, Loc.Y, PossCompanin.Loc.X, PossCompanin.Loc.Y) <= 15))
                                        {
                                            if (!WeaponSkill(PossCompanin.Loc.X, PossCompanin.Loc.Y, PossCompanin.EntityID))
                                                PossCompanin.TakeAttack(this, ref Damage, (Server.Game.MobAttackType)AtkMem.AtkType, false);
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
                                        uint Damage = PrepareAttack((byte)(Server.Game.MobAttackType)AtkMem.AtkType, true);
                                        if (AtkMem.Target == 6700)
                                        {
                                            if (Extra.GuildWars.War)
                                            {
                                                if (!WeaponSkill(Extra.GuildWars.ThePole.Loc.X, Extra.GuildWars.ThePole.Loc.Y, Extra.GuildWars.ThePole.EntityID))
                                                    Extra.GuildWars.ThePole.TakeAttack(this, Damage, AtkMem.AtkType);
                                            }
                                            else
                                            {
                                                AtkMem.Target = 0;
                                                AtkMem.Attacking = false;
                                            }
                                        }
                                        else if (AtkMem.Target == 6701)
                                        {
                                            if (!WeaponSkill(Extra.GuildWars.TheLeftGate.Loc.X, Extra.GuildWars.TheLeftGate.Loc.Y, Extra.GuildWars.TheLeftGate.EntityID))
                                                Extra.GuildWars.TheLeftGate.TakeAttack(this, Damage, AtkMem.AtkType);
                                        }
                                        else
                                        {
                                            if (!WeaponSkill(Extra.GuildWars.TheRightGate.Loc.X, Extra.GuildWars.TheRightGate.Loc.Y, Extra.GuildWars.TheRightGate.EntityID))
                                                Extra.GuildWars.TheRightGate.TakeAttack(this, Damage, AtkMem.AtkType);
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
                                                    PossNPC.TakeAttack(this, Damage, (Server.Game.MobAttackType)AtkMem.AtkType, false);
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
                                        if (SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                                        {
                                            Skills.SkillsClass.SkillUse SU = new Server.Skills.SkillsClass.SkillUse();
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
                            PacketHandling.Effects.CoolEffect.ActiveCool(MyClient);
                        }
                        #endregion
                    }
                    catch (Exception e) { Program.WriteMessage(e); }
                }
            }
            catch (Exception e) { Program.WriteMessage(e); }
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
                        prePotency += I.ItemPotency;
                }
                Item Ia = Equips.Get(12);
                prePotency += Ia.Plus;
                if ((Equips.RightHand.ID >= 421003 && Equips.RightHand.ID <= 421339) && ((Job <= 145 && Job >= 140) || Job == 135) && Equips.LeftHand.UID == 0)
                    prePotency += Equips.RightHand.ItemPotency;
                prePotency += (byte)Nobility.Rank;
                Item G = Equips.Get(7);
                prePotency -= G.ItemPotency;
                return (ushort)prePotency;
            }
        }
        public ushort KPotency
        {
            get
            {
                int prePotency = 0;
                prePotency += (byte)Nobility.Rank;
                return (ushort)prePotency;
            }
        }
        public uint PromoteLevel
        {
            get
            {
                uint Level = 0;
                if (Job == 10 || Job == 20 || Job == 40 || Job == 50 || Job == 100)
                {
                    Level = 15;
                }
                else if (Job == 11 || Job == 21 || Job == 41 || Job == 51 || Job == 101)
                {
                    Level = 40;
                }
                else if (Job == 12 || Job == 22 || Job == 42 || Job == 52 || Job == 132 || Job == 142)
                {
                    Level = 70;
                }
                else if (Job == 13 || Job == 23 || Job == 43 || Job == 53 || Job == 133 || Job == 143)
                {
                    Level = 100;
                }
                else if (Job == 14 || Job == 24 || Job == 44 || Job == 54 || Job == 134 || Job == 144)
                {
                    Level = 110;
                }
                return Level;
            }
        }
        public uint PromoteItems
        {
            get
            {
                uint Item = 0;
                if (Job == 10 || Job == 20 || Job == 40 || Job == 50 || Job == 100)
                {
                    Item = 1088001; // Meteor
                }
                else if (Job == 11 || Job == 21 || Job == 41 || Job == 51 || Job == 101)
                {
                    Item = 1088002; // MeteorTear
                }
                else if (Job == 12 || Job == 22 || Job == 42 || Job == 52 || Job == 132 || Job == 142)
                {
                    Item = 1088000; // DragonBall
                }
                else if (Job == 13 || Job == 23 || Job == 43 || Job == 53 || Job == 133 || Job == 143)
                {
                    Item = 1080001; // Emerald
                }
                else if (Job == 14 || Job == 24 || Job == 44 || Job == 54 || Job == 134 || Job == 144)
                {
                    Item = 721080; // MoonBox
                }
                return Item;
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
                EquipStats(5, false);
                
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);

            }
            if (Equips.Fan.ID != 0)
            {
                EquipStats(5, false);
                
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);

            }
            if (Equips.Tower.ID != 0)
            {
                EquipStats(5, false);
               
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);

            }
            if (Level > 130)
                oldlevel = Level;
            Reborns += 1;
            byte ExtraStatusPoints = 0;
            if (Level >= 120)
                ExtraStatusPoints = (byte)((-120 + Level) * 3 + Reborns * 10 + 45);
            else
                ExtraStatusPoints = (byte)(Reborns * 10);
            StatusPoints = ExtraStatusPoints;
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
            World.WorldMessage("SYSTEM", Name + " has got " + Reborns.ToString() + " reborn!", 2011, 0, System.Drawing.Color.Red);
            Teleport(1002, 439, 390);
            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
            Database.SaveCharacter(this, MyClient.AuthInfo.Account);
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
                EquipStats(5, false);
                
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);
            }
            if (Equips.Fan.ID != 0)
            {
                EquipStats(5, false);
                
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);
            }
            if (Equips.Tower.ID != 0)
            {
                EquipStats(5, false);
                
                Equips.UnEquip(5, this);
                Game.World.Spawn(this, false);
            }
            if (Level > 130)
                oldlevel = Level;
            Reborns += 1;
            byte ExtraStat = 0;
            if (Level >= 120)
                ExtraStat = (byte)((-120 + Level) * 3 + Reborns * 10 + 45);
            else
                ExtraStat = (byte)(Reborns * 10);
            StatusPoints = ExtraStat;
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
            World.WorldMessage("SYSTEM", Name + " has got " + Reborns.ToString() + " reborn!", 2011, 0, System.Drawing.Color.Red);
            Teleport(1002, 439, 390);
            StatusPoints += 150;
            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
            Database.SaveCharacter(this, MyClient.AuthInfo.Account);
        }

        public void Initalize(string acc)
        {
            //if (SearchingLevPlace == true)
            //{ }

            MyClient = new Server.GameClient();

            MyClient.Robot = true;
            MyClient.AuthInfo.Account = acc;
            MyClient.MyChar = this;

            if (Equips.RightHand.ID != 0)
            {
                if (ItemIDManipulation.Part(Equips.RightHand.ID, 0, 3) == 500)
                    AtkMem.AtkType = 28;
                else
                    AtkMem.AtkType = 2;
            }
            else
                AtkMem.AtkType = 2;
            //Equips.Send(MyClient, true);


            LoggedOn = DateTime.Now;

            if (DoubleExp && DoubleExpLeft > 0)
                ExpPotionUsed = DateTime.Now;
            else
                DoubleExp = false;

            if (BlessingLasts > 0 && InOTG)
            {
                InOTG = false;
                ushort MinutesTrained = (ushort)((LoggedOn - LastLogin).TotalMinutes);
                uint ExpAdd = (uint)(EXPBall * ((double)MinutesTrained / 900));
                ExpAdd *= Game.World.ServerInfo.ProfExpRate;
                IncreaseExp(ExpAdd, false);
                TrainTimeLeft -= MinutesTrained;

            }
            if (!Game.World.H_Chars.Contains(EntityID))
            {
                Game.World.H_Chars.Add(EntityID, this);
                Game.World.Spawns(this, false);
                //World.NewEmpire(this);
                Protection = true;
                LastProtection = DateTime.Now;
                Program.WriteMessage(Name + " Has Logged In");
            }
        }

        public PKMode PKMode = PKMode.Capture;
        public bool PKAble(PKMode PK, Character Attacker)
        {
            if (Attacker.EntityID == EntityID)
            {
                return false;
            }
            if (MyAvatar != null && ownerpk == false && Attacker.EntityID == MyAvatar.EntityID)
            {
                return false;
            }
            if (isAvatar && Owner != null && Attacker.ownerpk == false && Attacker.EntityID == Owner.EntityID)
            {
                return false;
            }
            if (PK == PKMode.PK)
                return true;
            else if (PK == PKMode.Capture)
            {
                if (BlueName || StatEff.Contains(StatusEffectEn.RedName) || StatEff.Contains(StatusEffectEn.BlackName))
                    return true;
                else
                    return false;
            }
            else if (PK == PKMode.Team)
            {
                Friend f = new Friend();
                f.Name = Attacker.Name;
                f.UID = Attacker.EntityID;

                Features.MemberInfo M = new Features.MemberInfo();
                M.MembID = Attacker.EntityID;
                M.MembName = Attacker.Name;

                if (MyTeam != null)
                    return !MyTeam.Members.Contains(Attacker);
                else if (Friends.Contains(f.UID))
                    return Friends.Contains(new Friend { UID = Attacker.EntityID, Name = Attacker.Name });

                else if (MyGuild.Allies.ContainsKey(Attacker.MyGuild.GuildID))
                    return !MyGuild.Allies.ContainsKey(Attacker.MyGuild.GuildID);

                else if (MyGuild != null)
                    return MyGuild.Members.Contains(M.MembID);

                else
                    return true;
                
            }
            return false;
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
        public void Teleport(ushort ToMap, ushort X, ushort Y)
        {
            World.Action(this, Packets.SkillUse(EntityID, 0, 0, 5131, 3, Loc.X, Loc.Y));
            World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
            if (!MyClient.Robot)
                MyClient.SendPacket(Packets.GeneralData(EntityID, ToMap, X, Y, 86));
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
            if (Loc.Map != ToMap)
            {
                if ((Loc.Map == 1707 || Loc.Map == 1068) && Luchando == true)
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
                                C.MyClient.LocalMessage(2011, System.Drawing.Color.Violet, "The opprent has retreated out  u WON THE BATTELE " + (Apuesta) * 2 + "CPs congratulations");
                            }
                    }
                    World.pvpclear(this);
                }
                if ((Loc.Map == 1018 || Loc.Map == 1082) && InPKT)
                {
                    InPKT = false;
                    if (Alive && Extra.PKTournament.PKTHash.ContainsKey(EntityID))
                        Extra.PKTournament.PKTHash.Remove(EntityID);
                }
                if (Game.World.ClassPkWar.AddMap.InTimeRange == true)
                    World.ClassPkWar.RemovePlayersFromWar(this);
            }
            if (!DMaps.DimMaps.Contains(ToMap))
                Loc.MapDimention = 0;


            Loc.X = X;
            Loc.Y = Y;
            AtkMem.Target = 0;
            AtkMem.Attacking = false;
            if (ToMap != 700)
                Loc.PreviousMap = Loc.Map;
            Loc.Map = ToMap;

            if (ToMap >= 10000)
                MyClient.SendPacket(Packets.MapStatus(ToMap, 65535));
            if (Loc.Map == 1036)
                MyClient.SendPacket(Packets.MapStatus(Loc.Map, 30));
            else
                MyClient.SendPacket(Packets.MapStatus(Loc.Map, 2080));
            World.Spawns(this, false);
            LastTele = DateTime.Now;
            World.Action(this, Packets.SkillUse(EntityID, 0, 0, 5131, 4, Loc.X, Loc.Y));
            if (myDengun1 != null)
            {
                if (myDengun1.Hero == this && MyTeam != null)
                    myDengun1.DengunMembers.Remove(EntityID);
                myDengun1.TransfeerLeader();
                myDengun1 = null;
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
        PacketHandling.Effects.CastEffect transEffect;
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
                transEffect = new PacketHandling.Effects.CastEffect();
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

        public void ReturnItemToInventory(Item I)
        {
            
            bool created = Database.ReputItemInHand(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 99999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            if (Loaded)
                MyClient.SendPacket(Packets.AddItem(I, 0));

        }
        public void EquipGermant(uint ID)
        {
            Item I = new Item();
            I.ID = ID;
            I.UID = (uint)Rnd.Next(1, 9999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 9999999);
                created = Database.NewItem(I, this);
            }
            Equips.UnEquip(9,this);
            EquipStats(9, false);
            Equips.Replace(9,I,this);
            EquipStats(9, true);
            I.MaxDur = I.ItemDBInfo.Durability;
            I.CurDur = I.MaxDur;
            MyClient.SendPacket(Packets.AddItem(I, 9));
            
        }
        public void UpdateItem(Game.Item I)
        {
            Database.UpdateItem(I);
            MyClient.SendPacket(Packets.UpdateItem(I, 0));
        }
        public void TradeItem(Game.Item I,Character To)
        {
            Inventory.Remove(I.UID);
            MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
            Database.TradeItem(I, To.EntityID);
            To.Inventory.Add(I.UID, I);
            if (To.Loaded)
                To.MyClient.SendPacket(Packets.AddItem(I, 0));
        }

        public void LoseItemsFromEquips(Game.Character C)
        {
            try
            {
                if (MyClient.AuthInfo.Status == "")
                {
                    for (byte i = 0; i < 12; i++)
                    {
                        Item I = Equips.Get(i);
                        if (I.ID != 0 && !I.FreeItem && I.ID != 300000 && I.ID < 2100055)
                        {
                            if (MyMath.ChanceSuccess(15) && dropedAnEquep == false || MyMath.ChanceSuccess(7))
                            {
                                dropedAnEquep = true;
                                Equips.Replace(i, new Item(), this);
                                DroppedItemConfiscator D = new DroppedItemConfiscator();
                                D.Info = I;
                                D.UID = I.UID;
                                if (I.UID == 0)
                                    //D.UID = (uint)(Rnd.Next(10000000));
                                    D.Loc = new Location();
                                D.Loc.MapDimention = Loc.MapDimention;
                                D.Loc.X = Loc.X;
                                D.Loc.Y = Loc.Y;
                                D.Loc.Map = Loc.Map;
                                D.DropTime = DateTime.Now;

                                if (D.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                                    D.Drop();
                                Database.SaveCharacter(this, this.MyClient.AuthInfo.Account);
                                Database.SaveCharacter(C, C.MyClient.AuthInfo.Account);
                                DateTime ItemTimeDroped = DateTime.Now.AddDays(5);
                                I.Time = (uint)Convert.ToInt32(ItemTimeDroped.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                                this.MyClient.SendPacket(Packets.ConfiscatorReward(I, C, (ushort)Features.Confiscator.CpsItem(I), C.Name));
                                I.NameClain = this.Name;
                                I.NameReward = C.Name;
                                Database.ConfiscatorC(I, this, C);
                                //Database.ConfiscatorReward(I, this);
                                this.ConfiscatorReward.Add(I.UID, I);
                                C.ConfiscatorClain.Add(I.UID, I);
                                C.MyClient.SendPacket(Packets.ConfiscatorClain(I, this, (ushort)Features.Confiscator.CpsItem(I), this.Name));

                            }
                        }
                    }
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        public void LoseInventoryItemsAndSilvers()
        {
            try
            {
                if (MyClient.AuthInfo.Status == "")
                {
                    double Pc = (double)((Level & 15) + ((double)Level / 5));
                    Pc = (double)(Pc / 2);
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
                    DI.Loc.MapDimention = Loc.MapDimention;
                    if (DI.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                        DI.Drop();

                    ArrayList ItemsLost = new ArrayList();
                    foreach (Item I in Inventory.Values)
                    {
                        if (MyMath.ChanceSuccess(Pc) && !I.FreeItem && I.Plus < 7 && I.ID < 2100055 && I.ID != 300000)
                            ItemsLost.Add(I);
                    }
                    foreach (Item I in ItemsLost)
                    {
                        Database.KeepItemAwyFromHand(I.UID, this);
                        Inventory.Remove(I.UID);
                        MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                        // RemoveItemI(I.UID,  1);
                        DroppedItem DI2 = new DroppedItem();
                        //DI2.UID = (uint)Rnd.Next(10000000);
                        DI2.DropTime = DateTime.Now;
                        DI2.Loc = Loc;
                        DI2.Loc.MapDimention = Loc.MapDimention;
                        DI2.Info = I;
                        DI2.Loc.X = (ushort)(DI2.Loc.X + Rnd.Next(4) - Rnd.Next(4));
                        DI2.Loc.Y = (ushort)(DI2.Loc.Y + Rnd.Next(4) - Rnd.Next(4));

                        if (DI2.FindPlace((Hashtable)World.H_Items[Loc.Map]))
                            DI2.Drop();
                    }
                    //Database.SaveCharacter(this, this.MyClient.AuthInfo.Account);
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }

        public void CreateItem(uint ID, byte Plus, byte ench, byte bless, Item.Gem soc1, Item.Gem soc2, Item.ArmorColor color)
        {
            Item I = new Item();
            I.Plus = Plus;
            I.ID = ID;
            I.MaxDur = I.ItemDBInfo.Durability;
            I.CurDur = I.MaxDur;
            I.Enchant = ench;
            I.Bless = bless;
            I.Soc1 = soc1;
            I.Soc2 = soc2;
            I.Color = color;
            I.UID = (uint)Rnd.Next(1, 999999999);
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 9999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            MyClient.SendPacket(Packets.AddItem(I, 0));
        }
        public void CreateItemIDAmount(uint ID, byte Amount)
        {
            for (byte i = 0; i < Amount; i++)
            {
                Item I = new Item();
                I.ID = ID;
                I.MaxDur = I.ItemDBInfo.Durability;
                I.CurDur = I.MaxDur;
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
        }
        public void CreateItemIDPlus(uint ID, byte Plus)
        {
            Item I = new Item();
            I.ID = ID;
            I.MaxDur = I.ItemDBInfo.Durability;
            I.CurDur = I.MaxDur;
            I.Plus = Plus;
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
        public void CreateReadyItem(Game.Item I)
        {
            bool created = Database.NewItem(I, this);
            while (!created)
            {
                I.UID = (uint)Rnd.Next(1, 999999999);
                created = Database.NewItem(I, this);
            }
            Inventory.Add(I.UID, I);
            if (Loaded)
                MyClient.SendPacket(Packets.AddItem(I, 0));
        }

        public void RemoveItemUIDAmount(uint ItemUID, byte Amount)
        {
            List<Game.Item> RemoveList = new List<Game.Item>(Amount);
            int Count = 0;
            foreach (Game.Item Item in Inventory.Values)
                if (Item.UID == ItemUID)
                {
                    if (Count < Amount)
                    {
                        RemoveList.Add(Item);
                        MyClient.SendPacket(Packets.ItemPacket(Item.UID, 0, 3));
                        Database.DeleteItem(Item.UID,EntityID);
                        Count++;
                    }
                    else
                        break;
                }
            foreach (Game.Item Item in RemoveList)
               Inventory.Remove(Item.UID);
        }
        public void RemoveItemIDAmount(uint ItemID, byte Amount)
        {
            List<Game.Item> RemoveList = new List<Game.Item>(Amount);
            int Count = 0;
            foreach (Game.Item Item in Inventory.Values)
                if (Item.ID == ItemID)
                {
                    if (Count < Amount)
                    {
                        RemoveList.Add(Item);
                        MyClient.SendPacket(Packets.ItemPacket(Item.UID, 0, 3));
                        Database.DeleteItem(Item.UID,EntityID);
                        Count++;
                    }
                    else
                        break;
                }
            foreach (Game.Item Item in RemoveList)
                Inventory.Remove(Item.UID);
        }
        public void RemoveItemIDAmountPlus(uint ItemID, byte Amount, byte Plus)
        {
            List<Game.Item> RemoveList = new List<Game.Item>(Amount);
            int Count = 0;
            foreach (Game.Item Item in Inventory.Values)
                if (Item.ID == ItemID && Item.Plus == Plus)
                {
                    if (Count < Amount)
                    {

                        RemoveList.Add(Item);
                        MyClient.SendPacket(Packets.ItemPacket(Item.UID, 0, 3));
                        Database.DeleteItem(Item.UID,EntityID);
                        Count++;
                    }
                    else
                        break;
                }
            foreach (Game.Item Item in RemoveList)
                Inventory.Remove(Item.UID);
        }

        public bool FindInventoryItemIDAmount(uint ItemID, byte Amount)
        {
            int got = 0;
            foreach (Game.Item Item in Inventory.Values)
                if (Item.ID == ItemID)
                    got++;
            if (got >= Amount)
                return true;

            return false;
        }
        public bool FindInventoryItemIDAmountPlus(uint ItemID, byte Amount, byte Plus)
        {
            int got = 0;
            foreach (Game.Item Item in Inventory.Values)
                if (Item.ID == ItemID && Item.Plus == Plus)
                    got++;
            if (got >= Amount)
                return true;

            return false;
        }

        public Item FindInventoryItemUID(uint ItemUID)
        {
            foreach (Item I in Inventory.Values)
                if (I.UID == ItemUID)
                    return I;
            return new Item();
        }
        public Item FindInventoryItemID(uint ItemID)
        {
            foreach (Item I in Inventory.Values)
                if (I.ID == ItemID)
                    return I;
            return new Item();
        }
        
        public bool WeaponSkill(ushort AX, ushort AY, uint T)
        {
            if (Equips.LeftHand.ID != 0 || Equips.RightHand.ID != 0)
            {
                if (isAvatar && Owner != null)
                    if (Owner.AvatarLevel < 4)
                    {
                        return false;
                    }
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
            Skills.SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
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
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention)
                                        if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                            if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                                SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (Companion C in World.H_Companions.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention)
                                        //if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                        if (C.Owner.PKAble(PKMode, this) && !SU.CompanionTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                                SU.CompanionTargets.Add(C, SU.GetDamage(C));
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
                                            if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
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
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (Companion C in World.H_Companions.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention)
                                        //if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                        if (C.Owner.PKAble(PKMode, this) && !SU.CompanionTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                            SU.CompanionTargets.Add(C, SU.GetDamage(C));
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
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (Companion C in World.H_Companions.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention)
                                        //if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                        if (C.Owner.PKAble(PKMode, this) && !SU.CompanionTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                            SU.CompanionTargets.Add(C, SU.GetDamage(C));
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
                                                if (C.PKAble(PKMode, this) && !SU.PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                                    SU.PlayerTargets.Add(C, SU.GetDamage(C));
                            }
                        }
                        foreach (Companion C in World.H_Companions.Values)
                        {
                            if (C.Alive)
                            {
                                if (C.EntityID != EntityID)
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= Dist && Loc.Map == C.Loc.Map && Loc.MapDimention == C.Loc.MapDimention)
                                        //if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                                        if (C.Owner.PKAble(PKMode, this) && !SU.CompanionTargets.Contains(C) && !DMaps.NoPKMaps.Contains(Loc.Map))
                                            SU.CompanionTargets.Add(C, SU.GetDamage(C));
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
            if (Level < 137 ||((Job > 159 && Job < 166) && Level > 100))
            {
                Amount = (uint)(Amount * EqStats.TotalGemExtraEXP * World.ServerInfo.ExperienceRate);
                Amount = (uint)(Amount * (EqStats.TotalEquipPotency + Level + 100 + Reborns * 5) / 100);
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
                if (!isAvatar)
                {
                    if (!noobexp && !marrieexp && !isTeamExp && Loc.Map != 1039)
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, Amount + " MORE!, experiance from killing monsters :P.");
                    else if (!noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, Amount + " team experience points gained.");
                    else if (noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You gained  " + Amount + " team experience points with additional rewarding experience points due to low level teammates.");
                    else if (!noobexp && marrieexp && isTeamExp && Loc.Map != 1039)
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You gained  " + Amount + " team experience points with additional rewarding experience points due to marriage teammates.");
                    else if (noobexp && !marrieexp && isTeamExp && Loc.Map != 1039)
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You gained  " + Amount + " team experience points with additional rewarding experience points due to low level and marriage teammates.");
                }
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
                            StatusPoints += (ushort)((CurLevel - Level) * 3);
                        else
                        {
                            if (Level < 120)
                                Level = 120;
                            StatusPoints += (ushort)((CurLevel - Level) * 3);
                        }
                    }
                    if (Level >= 120)
                    {
                        Game.World.WorldMessage(" " + Name + " say to all", "CONGRATULATIONS! " + Name + " has just achieved level " + Level + "! Great job!", 2011, 0, System.Drawing.Color.Red);
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

        public void TakeDimention()
        {
            uint preDim = 0;
            World.MapDimentions++;
            preDim = World.MapDimentions;
            Loc.MapDimention = preDim;
            //return preDim;
        }

        public void AddProfExp(ushort Wep, uint Amount)
        {
            if (isAvatar)
                return;
            if (Profs.Contains(Wep))
            {
                Prof P = (Prof)Profs[Wep];
                if (P.Lvl < 20)
                {
                    Profs.Remove(Wep);
                    Amount *= World.ServerInfo.ProfExpRate;
                    P.Exp += Amount;
                    if (P.Exp >= Database.ProfExp[P.Lvl])
                    {
                        P.Lvl++;
                        P.Exp = 0;
                        MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "Your proficiency level has gone up!.");
                        //Database.SaveProfs(this);
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
                Database.AddProfs(this);
            }
        }
        public void RemoveProf(Prof P)
        {
            if (Profs.Contains(P.ID))
            {
                Profs.Remove(P.ID);
                Database.DelProfs(P, this);
            }
            Profs.Add(P.ID, P);
            Database.AddProfs(this);
            MyClient.SendPacket(Packets.Prof(P));
        }

        public void AddSkillExp(ushort ID, uint Amount)
        {
            if (Skills.Contains(ID))
            {
                Skill S = (Skill)Skills[ID];
                Skills.SkillsClass.SkillInfo Info = S.Info;
                if (Info.UpgReqExp != 0 && Info.ID == ID && Level >= Info.UpgReqLvl)
                {
                    Amount *= World.ServerInfo.SkillExpRate;
                    Skills.Remove(ID);
                    S.Exp += Amount;
                    if (S.Exp >= Info.UpgReqExp)
                    {
                        S.Lvl++;
                        S.Exp = 0;
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! Your skill level has increased.");
                        //Database.SaveSkill(S, this);
                    }
                    Skills.Add(ID, S);
                    MyClient.SendPacket(Packets.Skill(S));
                }
            }
        }
        public void NewSkill(Skill S)
        {
            if (!Skills.Contains(S.ID))
            {
                Skills.Add(S.ID, S);
                MyClient.SendPacket(Packets.Skill(S));
                Database.AddSkills(this);
            }
            if (S.Lvl >= 1)
                RemoveSkill(S);
        }
        public void RemoveSkill(Skill S)
        {
            if (Skills.Contains(S.ID))
            {
                Skills.Remove(S.ID);
                Database.DelSkills(S, this);
            }
            Skills.Add(S.ID, S);
            MyClient.SendPacket(Packets.Skill(S));
            Database.AddSkills(this);
        }

        public uint PrepareAttack(byte AtkType, bool ArrowCost)
        {
            AtkMem.LastAttack = DateTime.Now;
            MobAttackType A = (MobAttackType)AtkType;

            bool EnoughArrows = true;
            if (A == MobAttackType.Ranged && ArrowCost)
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
                if (A == MobAttackType.Melee || A == MobAttackType.Ranged)
                {
                    uint Damage = (uint)Rnd.Next((int)EqStats.TotalMinAttack, (int)EqStats.TotalMaxAttack);
                    Damage = (uint)((Damage + Str) * EqStats.TotalGemExtraAttack);
                    Buff Stig = BuffOf(SkillsClass.ExtraEffect.Stigma);
                    if (Stig.Eff == SkillsClass.ExtraEffect.Stigma)
                        Damage = (uint)(Damage * 1.12);
                    return Damage;
                }
                else
                {
                    uint Damage = (uint)(EqStats.TotalMagicAttack * EqStats.TotalGemExtraMagicAttack);
                    return Damage;
                }
            }
            return 0;
        }
        #region MONSTER ATAKER
        public uint TakeAttack(Mob Attacker, uint Damage, MobAttackType AT, bool IsSkill)
        {
            if (AtkMem.Target == 0)
            {
                AtkMem.Target = Attacker.EntityID;
            }
            if (Damage != 0)
            {
                PacketHandling.Effects.BlessEffect.Handler(MyClient);
                //Extra.Durability.DefenceDurability(MyClient);
                if (Protection) Damage = 0;
                if (BuffOf(SkillsClass.ExtraEffect.Scapegoat).Eff == SkillsClass.ExtraEffect.Scapegoat && MyMath.ChanceSuccess(30))
                {
                    Buff B = BuffOf(SkillsClass.ExtraEffect.Scapegoat);
                    //RemoveBuff(B);
                    uint Dmg = (uint)(PrepareAttack(2, false) * B.Value);
                    Attacker.TakeAttack(this, ref Dmg, MobAttackType.Scapegoat, false);
                    return 0;//Will not be damaged
                }
                if (CanReflect)
                {
                    if (MyMath.ChanceSuccess(20))
                    {
                        Attacker.GetReflect(ref Damage, AT);

                        MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));
                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                        {
                            Game.Character Chaar = (Game.Character)DE.Value;
                            if (Chaar.Name != MyClient.MyChar.Name)
                            {

                                Chaar.MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));

                            }
                        }
                        if (Protection) Damage = 0;
                        return 0;
                    }
                }
                if (AT == MobAttackType.Melee)
                {

                    ushort Def = EqStats.TotalDefense;
                    Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                    if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                        Def = (ushort)(Def * Shield.Value);

                    else if (Def >= Damage+15)
                    {
                        Damage = (uint)Program.Rnd.Next(15, 20);
                    }
                    else
                        Damage -= Def;
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalDamageDecrease >= Damage)
                        Damage = (uint)Program.Rnd.Next(10, 15);
                    else
                        Damage -= EqStats.TotalDamageDecrease;
                }
                else if (AT == MobAttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * (((double)(110 - EqStats.TotalDodge) / 550 / 5200)));
                    if (EqStats.TotalDodge >= Damage+15)
                        Damage = (uint)Program.Rnd.Next(15, 20);
                    else
                        Damage -= EqStats.TotalDodge;
                    Damage *= 2 / 3;
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalDamageDecrease >= Damage)
                        Damage = (uint)Program.Rnd.Next(10, 15);
                    else
                        Damage -= EqStats.TotalDamageDecrease;
                }
                else
                {
                    if (Job >= 130 && Job <= 145)
                        Damage = (uint)(Damage * 4);
                    else
                        Damage = (uint)(Damage * 2);
                    if (EqStats.TotalMagicDamageDecrease >= Damage)
                        Damage = 10;
                    else
                        Damage -= EqStats.TotalMagicDamageDecrease;

                    Damage = (uint)((double)Damage * (((double)(100 - EqStats.TotalMDef1) / 100)));
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalMDef2 >= Damage+15)
                        Damage = 15;
                    else
                        Damage -= (uint)EqStats.TotalMDef2;
                }
            }
            if (AT != MobAttackType.Magic && Action == 250)
            {
                if (Stamina > 30)
                    Stamina -= 20;
                else
                    Stamina = 0;
            }
            if (AT == MobAttackType.Melee && !IsSkill)
            {
                double blocked = (double)(EqStats.TotalDodge + 70) - Attacker.Level;
                // Console.WriteLine(blocked + " ,level= " + Attacker.Level + " ,dodge= " + EqStats.Dodge);
                if (MyMath.ChanceSuccess(blocked))
                {
                    Damage = 0;
                }
            }
            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                Damage = 1;
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;
                if (!IsSkill)
                {
                    if (AT == MobAttackType.Magic)
                        World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.Skill, Attacker.SkillLvl, Loc.X, Loc.Y));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                }
            }
            else
            {
                //InitAngry(false);
                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                if (!DMaps.FreePKMaps.Contains(Loc.Map))
                    LoseInventoryItemsAndSilvers();
                Alive = false;
                CurHP = 0;

                if (!IsSkill)
                {
                    if (AT == MobAttackType.Magic)
                        World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.Skill, Attacker.SkillLvl, Loc.X, Loc.Y));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                }
                World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
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
                LastXP = DateTime.Now;
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
            return Damage;
        }
        #endregion
        #region GUARDS ATACKERS
        public void TakeAttack(Companion Attacker, uint Damage, MobAttackType AT)
        {
            if (Damage != 0)
            {
                if (!BlueName && PKPoints < 100 && !DMaps.FreePKMaps.Contains(Loc.Map))
                {
                    Attacker.Owner.BlueName = true;
                    if (Attacker.Owner.BlueNameLasts < 15)
                        Attacker.Owner.BlueNameLasts = 15;
                }
                if (Protection) Damage = 0;
                if (AT == MobAttackType.Melee)
                {
                    ushort Def = EqStats.TotalDefense;
                    Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                    if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                        Def = (ushort)(Def * Shield.Value);

                    if (Def >= Damage)
                        Damage = 1;
                    else
                        Damage -= Def;

                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.TotalDamageDecrease;
                }
                else if (AT == MobAttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * (((double)(110 - EqStats.TotalDodge) / 550 / 5200)));
                    if (EqStats.TotalDodge >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.TotalDodge;
                    Damage *= 2 / 3;

                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.TotalDamageDecrease;
                }
                else
                {
                    if (EqStats.TotalMagicDamageDecrease >= Damage)
                        Damage = 1;
                    else
                        Damage -= EqStats.TotalMagicDamageDecrease;

                    Damage = (uint)((double)Damage * (((double)(100 - EqStats.TotalMDef1) / 100)));
                    Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                    if (EqStats.TotalMDef2 >= Damage)
                        Damage = 1;
                    else
                        Damage -= (uint)EqStats.TotalMDef2;
                }
            }
            if (AT != MobAttackType.Magic && Action == 250)
            {
                if (Stamina > 30)
                    Stamina -= 20;
                else
                    Stamina = 0;
            }
            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                Damage = 1;
            Action = 100;
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;
                if (AT == MobAttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
            }
            else
            {
                //InitAngry(true);
                if (!DMaps.FreePKMaps.Contains(Loc.Map))
                    Attacker.Owner.PKPoints += 10;

                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                if (!DMaps.FreePKMaps.Contains(Loc.Map))
                    LoseInventoryItemsAndSilvers();
                Alive = false;
                CurHP = 0;

                if (AT == MobAttackType.Magic)
                    World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                else
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
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
                LastXP = DateTime.Now;
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
        #region Players ATACKERS
        public void GetReflect(ref uint Damage, MobAttackType AT)
        {
            if (Damage < CurHP)
            {
                CurHP -= (ushort)Damage;

                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
            }
            else
            {
                //InitAngry(true);
                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                AtkMem.Attacking = false;
                AtkMem.Target = 0;
                Alive = false;
                CurHP = 0;

                //    World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT).Get);
                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
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
                LastXP = DateTime.Now;
                DeathHit = DateTime.Now;
            }
        }
        public void GetPoisond(Skills.SkillsClass.DamageType AT)
        {

           
            int DamagePercent = 10 + PoisonedInfo.SpellLevel * 2;
            uint Dmg = (uint)(CurHP * (DamagePercent) / 150);
            if (Dmg < 1)
                Dmg = 1;
            CurHP -= (ushort)Dmg;
            World.Action(this, Packets.AttackPacket(155151, EntityID, Loc.X, Loc.Y, Dmg, (byte)AT));

        }
        public void TakeAttack(Character Attacker, uint Damage, MobAttackType AT, bool IsSkill)
        {
            if (Alive)
            {
                PacketHandling.Effects.BlessEffect.Handler(MyClient);
                //Extra.Durability.DefenceDurability(MyClient);
                if (!DMaps.NoPKMaps.Contains(Attacker.Loc.Map) && PKAble(Attacker.PKMode, Attacker))
                {
                    if (Protection && DMaps.NoPKMaps.Contains(Loc.Map) && !PKAble(Attacker.PKMode, Attacker)) Damage = 0;

                    if (Attacker.StatEff.Contains(StatusEffectEn.Ride) && Attacker.Equips.Steed.Plus < 12)
                    {
                        Attacker.StatEff.Remove(StatusEffectEn.Fly);
                        Attacker.StatEff.Remove(StatusEffectEn.Ride);
                    }
                    if (Attacker.Loc.Map == 1090)
                        if (Attacker.MyTDmTeam.TeamName == MyTDmTeam.TeamName)
                        {
                            Damage = 0;
                            return;
                        }
                    if (BuffOf(SkillsClass.ExtraEffect.Scapegoat).Eff == SkillsClass.ExtraEffect.Scapegoat && MyMath.ChanceSuccess(25))
                    {
                        Buff B = BuffOf(SkillsClass.ExtraEffect.Scapegoat);
                        //RemoveBuff(B);
                        uint Dmg = (uint)(PrepareAttack(2, false) * 2);
                        Attacker.TakeAttack(this, Dmg, MobAttackType.Scapegoat, false);
                        return;//Will not be damaged
                    }
                    if (DMaps.NoPKMaps.Contains(Loc.Map) && !PKAble(Attacker.PKMode, Attacker))
                        Damage = 0;
                    if (this != Attacker)
                    {
                        {
                            AtkMem.Target = Attacker.EntityID;
                            if (Attacker.AtkMem.Target == 0)
                                Attacker.AtkMem.Target = EntityID;
                        }
                        if (PKPoints < 100 && !DMaps.FreePKMaps.Contains(Loc.Map) && !DMaps.NoPKMaps.Contains(Loc.Map))
                        {
                            if ((!isAvatar && (MyAvatar == null || Attacker.EntityID != MyAvatar.EntityID)) || (isAvatar && (Owner != null && Attacker.EntityID != Owner.EntityID)))
                            {
                                Attacker.BlueName = true;
                                if (Attacker.BlueNameLasts < 15)
                                    Attacker.BlueNameLasts = 15;
                            }
                           
                        }
                    }
                    if (AT != MobAttackType.Magic && Attacker.BuffOf(SkillsClass.ExtraEffect.Superman).Eff == SkillsClass.ExtraEffect.Superman)
                        Damage *= 2;
                    if (AT != MobAttackType.Magic && !IsSkill)
                    {
                        ushort _Agi = (ushort)(Attacker.Agi + Attacker.EqStats.TotalAgility);

                        Buff Accuracy = Attacker.BuffOf(SkillsClass.ExtraEffect.Accuracy);
                        if (Accuracy.Eff == SkillsClass.ExtraEffect.Accuracy)
                            _Agi = (ushort)(_Agi * Accuracy.Value);

                        double MissValue = Rnd.Next(_Agi - 25, _Agi + 25);
                        if (MissValue <= EqStats.TotalDodge)
                            Damage = 0;
                    }
                    if (AT != MobAttackType.Magic && Action == 250)
                    {
                        if (Stamina > 30)
                            Stamina -= 20;
                        else
                            Stamina = 0;
                    }
                    Action = 100;
                    if (Damage != 0 && !IsSkill)
                    {
                        if (AT == MobAttackType.Melee)
                        {
                            if (Attacker.Job > 129 && Attacker.Job < 136)
                            { Damage = (uint)(Damage * 1.30); }
                            /* if (Job == 25)
                            {
                                Damage = (uint)(Damage * 0.83);
                            }*/
                            Damage = (uint)(Damage * MyMath.PotencyDifference(Attacker.Potency, Potency));
                            Damage = (uint)(Damage * MyMath.PotencyDifference(Attacker.KPotency, KPotency));

                            #region potencey effect
                            //if (Attacker.KPotency - 2 == KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.5);
                            //}
                            //if (Attacker.KPotency - 4 == KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.10);
                            //}
                            //if (Attacker.KPotency - 5 == KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.13);
                            //}
                            //if (Attacker.KPotency - 6 == KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.17);
                            //}
                            //if (Attacker.KPotency - 7 >= KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.20);
                            //}
                            //if (KPotency - 2 == Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.95);
                            //}
                            //if (KPotency - 4 == Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.90);
                            //}
                            //if (KPotency - 5 == Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.87);
                            //}
                            //if (KPotency - 6 == Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.85);
                            //}
                            //if (KPotency - 7 >= Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.82);
                            //}
                            ///*if (User.KPotency - 1 >= C.Potency && User.Potency >= 305)
                            //{
                            //    Damage = (uint)(Damage * 1.03);
                            //}
                            //if (C.KPotency - 1 >= User.Potency && C.Potency >= 305)
                            //{
                            //    Damage = (uint)(Damage * 0.97);
                            //}*/
                            //if (Attacker.KPotency - 3 == KPotency)
                            //{
                            //    Damage = (uint)(Damage * 1.10);
                            //}
                            //if (KPotency - 3 == Attacker.KPotency)
                            //{
                            //    Damage = (uint)(Damage * 0.90);
                            //}
                            //// normal potencey effect:::::::::>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<</////\\\\\////\\\\////
                            //if (Attacker.Potency - 2 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.13);
                            //}
                            //if (Attacker.Potency - 3 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.17);
                            //}
                            //if (Attacker.Potency - 4 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.21);
                            //}
                            //if (Attacker.Potency - 5 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.25);
                            //}
                            //if (Attacker.Potency - 6 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.28);
                            //}
                            //if (Attacker.Potency - 7 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.32);
                            //}
                            //if (Attacker.Potency - 8 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.35);
                            //}
                            //if (Attacker.Potency - 9 >= Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.38);
                            //}
                            //if (Potency - 2 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.90);
                            //}
                            //if (Potency - 3 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.87);
                            //}
                            //if (Potency - 4 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.83);
                            //}
                            //if (Potency - 5 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.80);
                            //}
                            //if (Potency - 6 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.77);
                            //}
                            //if (Potency - 7 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.75);
                            //}
                            //if (Potency - 8 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.73);
                            //}
                            //if (Potency - 9 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.71);
                            //}
                            //if (Potency - 10 >= Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.69);
                            //}
                            //if (Attacker.Potency - 1 == Potency)
                            //{
                            //    Damage = (uint)(Damage * 1.05);
                            //}
                            //if (Potency - 1 == Attacker.Potency)
                            //{
                            //    Damage = (uint)(Damage * 0.95);
                            //}
                            #endregion
                            //ushort Def = EqStats.defense;

                            ushort Def = 3000;
                            if (Job > 19 && Job < 26)
                                Def += 3500;
                            Buff Shield = BuffOf(SkillsClass.ExtraEffect.MagicShield);
                            if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                                Def = (ushort)(Def * Shield.Value);

                            if (Def >= Damage)
                                Damage = 1;
                            else
                                Damage -= Def;

                            Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                            Damage += Attacker.EqStats.TotalDamageIncrease;
                            if (EqStats.TotalDamageDecrease >= Damage)
                                Damage = 1;
                            else
                                Damage -= EqStats.TotalDamageDecrease;
                            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                                Damage = 1;
                        }
                        else if (AT == MobAttackType.Ranged)
                        {

                            Damage = (uint)(Damage * 0.02);
                            //Damage = (uint)(Damage * (((uint)dod2 - EqStats.Dodge) / (uint)dod));
                            if (Attacker.Potency - 2 >= Potency && Attacker.Potency >= 310)
                            {
                                Damage = (uint)(Damage * 1.14);
                            }
                            if (Attacker.Potency - 4 >= Potency)
                            {
                                Damage = (uint)(Damage * 1.13);
                            }
                            if (Attacker.Potency - 6 >= Potency)
                            {
                                Damage = (uint)(Damage * 1.12);
                            }
                            if (Potency - 2 >= Attacker.Potency && Potency >= 310)
                            {
                                Damage = (uint)(Damage * 0.79);
                            }
                            if (Potency - 4 >= Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.86);
                            }
                            if (Potency - 6 >= Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.94);
                            }
                            if (Attacker.Potency - 1 >= Potency && Attacker.Potency >= 305)
                            {
                                Damage = (uint)(Damage * 1.10);
                            }
                            if (Potency - 1 >= Attacker.Potency && Potency >= 305)
                            {
                                Damage = (uint)(Damage * 0.90);
                            }
                            //Damage = (uint)((double)Damage * ((100 - EqStats.Dodge) / 100));
                            if (EqStats.TotalDodge >= Damage)
                                Damage = 1;
                            else
                                Damage -= EqStats.TotalDodge;
                            Damage = Damage * 2 / 3;
                            Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);

                            Damage += Attacker.EqStats.TotalDamageIncrease;

                            if (EqStats.TotalDamageDecrease >= Damage)
                                Damage = 1;
                            else
                                Damage -= EqStats.TotalDamageDecrease;
                            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                                Damage = 1;
                        }
                        else
                        {
                            Damage = (uint)(Damage * 1.15);
                            #region potencey effect
                            if (Attacker.KPotency - 2 == KPotency)
                            {
                                Damage = (uint)(Damage * 1.15);
                            }
                            if (Attacker.KPotency - 4 == KPotency)
                            {
                                Damage = (uint)(Damage * 1.23);
                            }
                            if (Attacker.KPotency - 5 == KPotency)
                            {
                                Damage = (uint)(Damage * 1.26);
                            }
                            if (Attacker.KPotency - 6 == KPotency)
                            {
                                Damage = (uint)(Damage * 1.29);
                            }
                            if (Attacker.KPotency - 7 >= KPotency)
                            {
                                Damage = (uint)(Damage * 1.32);
                            }
                            if (KPotency - 2 == Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.91);
                            }
                            if (KPotency - 4 == Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.82);
                            }
                            if (KPotency - 5 == Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.80);
                            }
                            if (KPotency - 6 == Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.75);
                            }
                            if (KPotency - 7 >= Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.72);
                            }
                            /*if (User.KPotency - 1 >= C.Potency && User.Potency >= 305)
                            {
                                Damage = (uint)(Damage * 1.03);
                            }
                            if (C.KPotency - 1 >= User.Potency && C.Potency >= 305)
                            {
                                Damage = (uint)(Damage * 0.97);
                            }*/
                            if (Attacker.KPotency - 3 == KPotency)
                            {
                                Damage = (uint)(Damage * 1.13);
                            }
                            if (KPotency - 3 == Attacker.KPotency)
                            {
                                Damage = (uint)(Damage * 0.88);
                            }
                            // normal potencey effect:::::::::>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<</////\\\\\////\\\\////
                            if (Attacker.Potency - 2 == Potency)
                            {
                                Damage = (uint)(Damage * 1.10);
                            }
                            if (Attacker.Potency - 3 == Potency)
                            {
                                Damage = (uint)(Damage * 1.13);
                            }
                            if (Attacker.Potency - 4 == Potency)
                            {
                                Damage = (uint)(Damage * 1.15);
                            }
                            if (Attacker.Potency - 5 == Potency)
                            {
                                Damage = (uint)(Damage * 1.18);
                            }
                            if (Attacker.Potency - 6 == Potency)
                            {
                                Damage = (uint)(Damage * 1.22);
                            }
                            if (Attacker.Potency - 7 == Potency)
                            {
                                Damage = (uint)(Damage * 1.25);
                            }
                            if (Attacker.Potency - 8 == Potency)
                            {
                                Damage = (uint)(Damage * 1.27);
                            }
                            if (Attacker.Potency - 9 >= Potency)
                            {
                                Damage = (uint)(Damage * 1.30);
                            }
                            if (Potency - 2 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.90);
                            }
                            if (Potency - 3 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.87);
                            }
                            if (Potency - 4 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.85);
                            }
                            if (Potency - 5 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.82);
                            }
                            if (Potency - 6 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.79);
                            }
                            if (Potency - 7 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.77);
                            }
                            if (Potency - 8 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.75);
                            }
                            if (Potency - 9 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.73);
                            }
                            if (Potency - 10 >= Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.71);
                            }
                            if (Attacker.Potency - 1 == Potency)
                            {
                                Damage = (uint)(Damage * 1.07);
                            }
                            if (Potency - 1 == Attacker.Potency)
                            {
                                Damage = (uint)(Damage * 0.93);
                            }
                            #endregion
                            Damage = (uint)((double)Damage * (((double)(100 - EqStats.TotalMDef1) / 100)));
                            if (EqStats.TotalMDef2 >= Damage)
                                Damage = 1;
                            else
                                Damage -= (uint)EqStats.TotalMDef2;
                            Damage = (uint)((double)Damage * (100 - EqStats.TotalBless) / 100);


                            Damage += Attacker.EqStats.TotalMagicDamageIncrease;

                            if (EqStats.TotalMagicDamageIncrease >= Damage)
                                Damage = 1;
                            else
                                Damage -= EqStats.TotalMagicDamageIncrease;
                            if (BuffOf(SkillsClass.ExtraEffect.ShurikenVortex).Eff == SkillsClass.ExtraEffect.ShurikenVortex)
                                Damage = 1;
                        }
                    }
                    //if (BuffOf(SkillsClass.ExtraEffect.Fly).Eff == SkillsClass.ExtraEffect.Fly)
                    //Damage = 0;
                    #region DM
                    if (dmblack >= 1 && Attacker.dmblack >= 1)
                    {
                        Damage = 0;

                    }
                    if (dmblue >= 1 && Attacker.dmblue >= 1)
                    {
                        Damage = 0;

                    }
                    if (dmred >= 1 && Attacker.dmred >= 1)
                    {
                        Damage = 0;

                    }
                    #endregion
                    if (CanReflect)
                    {
                        if (MyMath.ChanceSuccess(17))
                        {
                            Damage = (uint)(Damage * 0.40);
                            Attacker.GetReflect(ref Damage, AT);


                            MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));
                            foreach (DictionaryEntry DE in Game.World.H_Chars)
                            {
                                Game.Character Chaar = (Game.Character)DE.Value;
                                if (Chaar.Name != Name)
                                {


                                    Chaar.MyClient.SendPacket(Packets.String(EntityID, 10, "MagicReflect"));

                                }
                            }
                            Damage = 0;
                            return;
                        }
                    }
                    if (Damage < CurHP)
                    {
                        CurHP -= (ushort)Damage;
                        if (AT != MobAttackType.Magic && !IsSkill)
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                        if (AT == MobAttackType.Scapegoat)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 43));
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                        }
                    }
                    else
                    {
                        //InitAngry(true);
                        if (Attacker.AtkMem.Target == EntityID)
                        {
                            Attacker.AtkMem.Attacking = false;
                            Attacker.AtkMem.Target = 0;
                        }
                        if (Attacker.MyAvatar != null)
                            Attacker.MyAvatar.AtkMem.Target = 0;
                        if (Attacker.Owner != null && Attacker.Owner.AtkMem.Target == EntityID)
                            Attacker.Owner.AtkMem.Target = 0;
                        AtkMem.Attacking = false;
                        AtkMem.Target = 0;

                        if (Loc.Map == 1090)
                        {
                            uint lol = 1;
                            if (Game.World.DeathMatch.TDMScore.Contains(Attacker.MyTDmTeam.TeamID))
                            {
                                Game.World.DeathMatch.TDMScore[Attacker.MyTDmTeam.TeamID] = (uint)Game.World.DeathMatch.TDMScore[Attacker.MyTDmTeam.TeamID] + lol;
                            }
                            else
                                Game.World.DeathMatch.TDMScore.Add(Attacker.MyTDmTeam.TeamID, lol);

                            /*if (Attacker.dmblack == 1) { World.Dmscore.blackscore += 1; }
                            else if (Attacker.dmblue == 1) { World.Dmscore.bluescore += 1; }
                            else if (Attacker.dmred == 1) { World.Dmscore.redscore += 1; }
                            Attacker.CPs += 400;//on kill  */
                        }
                        Game.World.ClassPkWar.RemovePlayersFromWar(this);
                        if (Attacker.Luchando)
                            if (Attacker.Loc.Map == 1707 || Attacker.Loc.Map == 1068)
                            {
                                Game.Character J = Game.World.CharacterFromName(Attacker.Enemigo);
                                Attacker.PkPuntos++;
                                Attacker.MyClient.LocalMessage(2011, System.Drawing.Color.White, Attacker.Name + ": " + Attacker.PkPuntos + " points. " + J.Name + ": " + J.PkPuntos + " points");
                                J.MyClient.LocalMessage(2011, System.Drawing.Color.White, Attacker.Name + ": " + Attacker.PkPuntos + " points. " + J.Name + ": " + J.PkPuntos + " points");
                                {
                                    if (Attacker.PkPuntos == Attacker.VSpets)
                                    {
                                        Game.World.WorldMessage(Attacker.Name, Attacker.Name + " (" + Attacker.PkPuntos + ")" + " has won " + Attacker.Enemigo + " (" + J.PkPuntos + ")" + " in a battle of 1 v 1! The winner has won: " + (Attacker.Apuesta) * 2 + " CPs", 2011, 0, System.Drawing.Color.White);
                                        Attacker.Luchando = false;
                                        J.Luchando = false;
                                        Attacker.PkPuntos = 0;
                                        J.PkPuntos = 0;
                                        Attacker.Enemigo = "";
                                        J.Enemigo = "";
                                        Attacker.CPs += (uint)Attacker.Apuesta;
                                        Attacker.CPs += (uint)Attacker.Apuesta;
                                        J.Teleport(1002, 430, 383);
                                        Attacker.Teleport(1002, 430, 371);
                                        Attacker.Action = 230;
                                    }
                                }
                            }
                        /*if (Loc.Map == 1038)
                        {
                            Attacker.CPs += 5;
                            Attacker.HonorPoints += 5;
                            Attacker.MyClient.LocalMessage(2005,System.Drawing.Color.White, "Your have 5 honor points and 5 cps for kill enemy.");
                        }
                        if (Loc.Map == 6001)
                        {
                            Attacker.CPs += 5;
                            Attacker.HonorPoints += 5;
                            Attacker.MyClient.LocalMessage(2005,System.Drawing.Color.White, "Your have 5 honor points and 5 cps for kill enemy.");
                        }
                        if (Loc.Map == 1005)
                        {
                            Attacker.CPs += 5;
                            Attacker.HonorPoints += 5;
                            Attacker.MyClient.LocalMessage(2005,System.Drawing.Color.White, "Your have 5 honor points and 5 cps for kill enemy.");
                        }*/
                        #region char take atttack char

                        if (!DMaps.FreePKMaps.Contains(Loc.Map) && !Attacker.isAvatar && !isAvatar)
                        {
                            Attacker.HonorPoints += 100;
                            HonorPoints -= 100;
                            MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "you lost :" + 100 +" Honor Points");
                            Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "you claimed :" + 100 + " Honor Points");
                            if (PKPoints >= 200)
                                LoseItemsFromEquips(Attacker);
                            if (Attacker.PKPoints < 1500)
                            World.WorldMessage("SYSTEM", Attacker.Name + " Has Killed " + Name + " And Got Some CPs .", 2011, 0, System.Drawing.Color.White);
                            LoseInventoryItemsAndSilvers();
                            if (PKPoints >= 200)
                                if (Attacker.PKPoints < 1500)
                                    if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                        Attacker.CPs += 100000;
                            if (PKPoints >= 100 && PKPoints < 200)
                                if (Attacker.PKPoints < 1500)
                                    if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                        Attacker.CPs += 30000;
                            if (!BlueName)
                            {
                                Attacker.BlueNameLasts += 45;
                                if (Attacker.Enemies.Contains(EntityID))
                                {
                                    Attacker.PKPoints += 5;
                                    if (Attacker.PKPoints < 1500)
                                        if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                            Attacker.CPs += 3500;
                                }
                                else
                                    Attacker.PKPoints += 10;
                                if (Attacker.PKPoints < 1500)
                                    if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                        Attacker.CPs += 2150;
                            }
                            if (!Enemies.Contains(Attacker.EntityID))
                            {
                                Enemies.Add(Attacker.EntityID, new Enemy() { UID = Attacker.EntityID, Name = Attacker.Name });
                                MyClient.SendPacket(Packets.FriendEnemyPacket(Attacker.EntityID, Attacker.Name, 19, 1));
                            }
                            if (PKPoints >= 100)
                            {
                                Teleport(6000, 32, 72);
                                World.WorldMessage("SYSTEM", Attacker.Name + " has captured " + Name + " and sent him to jail.", 2011, 0, System.Drawing.Color.White);
                            }
                        }
                        #endregion
                        #region char take attack robot
                        if (!DMaps.FreePKMaps.Contains(Loc.Map) && Attacker.isAvatar && !isAvatar)
                        {
                            if (Attacker.Owner.EntityID != EntityID)
                            {
                                if (PKPoints >= 200)
                                    LoseItemsFromEquips(Attacker);
                                if (Attacker.PKPoints < 1500)
                                    World.WorldMessage("SYSTEM", Attacker.Owner.Name + "`Avatar Has Killed " + Name + " And Got Some CPs .", 2011, 0, System.Drawing.Color.White);
                                LoseInventoryItemsAndSilvers();
                                if (PKPoints >= 200)
                                    if (Attacker.PKPoints < 1500)
                                        if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                            Attacker.CPs += 100000;
                                if (PKPoints >= 100 && PKPoints < 200)
                                    if (Attacker.PKPoints < 1500)
                                        if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                            Attacker.CPs += 30000;
                                if (!BlueName)
                                {
                                    Attacker.BlueNameLasts += 45;
                                    if (Attacker.Owner.Enemies.Contains(EntityID))
                                    {
                                        Attacker.PKPoints += 5;
                                        if (Attacker.PKPoints < 1500)
                                            if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                                Attacker.Owner.CPs += 3500;
                                    }
                                    else
                                        Attacker.PKPoints += 10;
                                    if (Attacker.PKPoints < 1500)
                                        if (Level >= 120 && Attacker.Level >= 120 && Attacker.Reborns == 2)
                                            Attacker.Owner.CPs += 2150;
                                }

                                if (PKPoints >= 100)
                                {
                                    Teleport(6000, 32, 72);
                                    World.WorldMessage("SYSTEM", Attacker.Name + " has captured " + Name + " and sent him to jail.", 2011, 0, System.Drawing.Color.White);
                                }
                            }
                            else
                            {
                                MyAvatarKilledMe = true;
                            }
                        }
                        #endregion
                        #region robot take attack robot
                        if (!DMaps.FreePKMaps.Contains(Loc.Map) && Attacker.isAvatar && isAvatar)
                        {
                            
                        }
                        #endregion
                        #region robot take attack char
                        if (!DMaps.FreePKMaps.Contains(Loc.Map) && !Attacker.isAvatar && isAvatar)
                        {
                            if (Owner != null && Attacker.EntityID == Owner.EntityID)
                            {
                                ((Avatar)this).reviveSpeed = 4;
                               
                            }
                        }
                        #endregion
                        
                        Alive = false;
                        CurHP = 0;

                        if (AT != MobAttackType.Magic && !IsSkill)
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
                        if (AT == MobAttackType.Scapegoat)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 43));
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                        }
                        VortexOn = false;
                        StatEff.Remove(StatusEffectEn.Cyclone);
                        StatEff.Remove(StatusEffectEn.FatalStrike);
                        StatEff.Remove(StatusEffectEn.BlueName);
                        //StatEff.Remove(StatusEffectEn.Flashy);
                        StatEff.Remove(StatusEffectEn.ShurikenVortex);
                        BlueName = false;
                        StatEff.Remove(StatusEffectEn.SuperMan);
                        StatEff.Remove(StatusEffectEn.Fly);
                        StatEff.Remove(StatusEffectEn.XPStart);
                        StatEff.Remove(StatusEffectEn.Ride);
                        StatEff.Remove(StatusEffectEn.DragonCyclone);
                        StatEff.Remove(StatusEffectEn.Poisoned);
                        StatEff.Add(StatusEffectEn.Dead);
                        DeathHit = DateTime.Now;
                        LastXP = DateTime.Now;
                        PoisonedInfo = null;
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
                        World.Spawns(this, false);
                        /* if (Attacker.BlessingLasts == 0)
                         {
                             if (Loc.Map != 1005 || Loc.Map != 1038 || Loc.Map != 6001)
                             {
                                 if (BlessingLasts > 0)
                                 {
                                     CurseUser = DateTime.Now;
                                     CurseStart = true;
                                     CurseExpLeft += 300;
                                     Attacker.MyClient.SendPacket(Packets.Status(EntityID, Status.CurseTime, (ulong)CurseExpLeft));
                                     // Attacker.MyClient.SendPacket(Packets.String(EntityID, 10, "curse").Get);
                                 }
                             }
                         }*/
                    }
                }
            }
        }
        #endregion

        public void EquipStats(byte Pos, bool Equip)
        {
            try
            {
                Item I = Equips.Get(Pos);
                if (I.ID != 0)
                {
                    DatabaseItem D1 = I.ItemDBInfo;

                    ItemIDManipulation IMan = new ItemIDManipulation(I.ID);
                    uint ComposeID = IMan.ToComposeID(Pos);
                    DatabasePlusItem D2;
                    if (Database.DatabasePlusItems.Contains(ComposeID.ToString() + I.Plus.ToString()))
                        D2 = (DatabasePlusItem)Database.DatabasePlusItems[ComposeID.ToString() + I.Plus.ToString()];
                    else D2 = new DatabasePlusItem();

                    EquipStats E = new EquipStats();

                    if (Pos != 12)
                        E.TotalDodge = (byte)(D1.Dodge + D2.Dodge);
                    else
                        E.TotalRideSpeed = D2.Dodge;

                    if (Pos != 12)
                        E.TotalAgility = (ushort)(D1.AgilityGives + D2.Agility);
                    else
                        E.TotalVigor = D2.Agility;
                    if (Pos == 12)
                    {

                        E.TotalHP += 100;
                    }
                    if (Pos == 5)
                    {
                        D1.MinAtk /= 2;
                        D1.MaxAtk /= 2;
                    }
                    if (Pos != 10 && Pos != 11)
                    {
                        E.TotalDefense = (ushort)(D1.Defense + D2.Defense);
                        E.TotalMagicAttack = (uint)(D1.MagicAttack + D2.MagicAttack);
                        E.TotalMinAttack = D2.MinAttack + D1.MinAtk;
                        E.TotalMaxAttack = D2.MaxAttack + D1.MaxAtk;
                        E.TotalMDef1 = (ushort)D1.MagicDefense;
                        E.TotalMDef2 = D2.MagicDefense;
                    }
                    else
                    {
                        E.TotalMagicDamageIncrease = (uint)(D1.MagicAttack + D2.MagicAttack);
                        E.TotalMagicDamageDecrease = (uint)(D1.MagicDefense + D2.MagicDefense);
                        E.TotalDamageIncrease = (uint)(D2.MinAttack + D1.MinAtk);
                        E.TotalDamageDecrease = (uint)(D1.Defense + D2.Defense);
                    }
                    Item.GetGemDamage(ref E, I.Soc1);
                    Item.GetGemDamage(ref E, I.Soc2);
                    if (Equips.Tower.ID != I.ID && Equips.Fan.ID != I.ID)
                        E.TotalHP += I.Enchant;
                    E.TotalHP += D2.HP;
                    E.TotalEquipPotency += I.ItemPotency;
                    E.TotalBless += I.Bless;

                    if (I.ID == 2100045)//MagicalBottle
                    {
                        E.TotalMP += 400;
                    }
                    if (I.ID == 2100025)//MiraculousGourd
                    {
                        E.TotalHP += 800;
                        E.TotalMP += 800;
                    }

                    if (I.ID == 2100075)//GOLD PRIZE
                    {
                        E.TotalMaxAttack += 1000;
                        E.TotalDefense += 1000;
                        E.TotalHP += 1500;
                        E.TotalMP += 1500;
                    }
                    if (I.ID == 2100065)//SILVER PRIZE
                    {
                        E.TotalHP += 1200;
                        E.TotalMP += 1200;
                    }
                    if (I.ID == 2100055)//BRONZE PRIZE
                    {
                        E.TotalHP += 900;
                        E.TotalMP += 900;
                    }
                    if (I.ID == 150000)
                        E.TotalHP += 800;
                    if (Equip) EqStats += E;
                    else EqStats -= E;
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }

        public uint EXPBall
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
        public uint PowerEXPBall
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
                if (I.ID >= 1000000 && I.ID <= 1003000)
                {
                    if (!(DateTime.Now > UnableToUseDrugs.AddSeconds(UnableToUseDrugsFor)))
                    {
                        MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You've been poisoned. You cannot use drugs for a while.");
                        return;
                    }
                }
                if (I.ItemDBInfo.Name == "MoonBox")
                {
                    MyClient.LocalMessage(2005, System.Drawing.Color.Red , "MoonBox openning not yet added. Its just for promoting.");
                    return;
                }
                switch (I.ID)
                {
                    #region Trojan Beginner Items
                    case 723776:
                        {
                            if (Inventory.Count <= 33)
                            {
                                RemoveItemIDAmount(723776,  1);
                                CreateItem(130059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Robe
                                CreateItem(117059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Earring
                                CreateItem(120099, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);//Neck
                                CreateItem(150119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Ring
                                CreateItem(160119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //boots
                                CreateItem(202009, 10, 0, 0, Item.Gem.SuperGloryGem, Item.Gem.SuperGloryGem, 0); //Star
                                CreateItem(201009, 10, 0, 0, Item.Gem.SuperThunderGem, Item.Gem.SuperThunderGem, 0); //Fan
                                CreateItem(480139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                                CreateItem(410139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                            }
                            break;
                        }
                    #endregion
                    #region Ninja Beginner Items
                    case 723775:
                        {
                            if (Inventory.Count <= 33)
                            {
                                RemoveItemIDAmount(723775,  1);
                                CreateItem(135059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Robe
                                CreateItem(117059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Earring
                                CreateItem(120099, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);//Neck
                                CreateItem(150119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Ring
                                CreateItem(160119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //boots
                                CreateItem(202009, 10, 0, 0, Item.Gem.SuperGloryGem, Item.Gem.SuperGloryGem, 0); //Star
                                CreateItem(201009, 10, 0, 0, Item.Gem.SuperThunderGem, Item.Gem.SuperThunderGem, 0); //Fan
                                CreateItem(601139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                                CreateItem(601139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                            }
                            break;
                        }
                    #endregion
                    #region Warrior Beginner Items
                    case 723774:
                        {
                            if (Inventory.Count <= 33)
                            {
                                RemoveItemIDAmount(723774,  1);
                                CreateItem(131059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Robe
                                CreateItem(117059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Earring
                                CreateItem(120099, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);//Neck
                                CreateItem(150119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Ring
                                CreateItem(160119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //boots
                                CreateItem(202009, 10, 0, 0, Item.Gem.SuperGloryGem, Item.Gem.SuperGloryGem, 0); //Star
                                CreateItem(201009, 10, 0, 0, Item.Gem.SuperThunderGem, Item.Gem.SuperThunderGem, 0); //Fan
                                CreateItem(480139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                                CreateItem(410139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);
                                CreateItem(900049, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);

                            }
                            break;
                        }
                    #endregion
                    #region Archer Beginner Items
                    case 723773:
                        {
                            if (Inventory.Count <= 33)
                            {
                                RemoveItemIDAmount(723773,  1);
                                CreateItem(133049, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Robe
                                CreateItem(117059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Earring
                                CreateItem(120099, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);//Neck
                                CreateItem(150119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Ring
                                CreateItem(160119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //boots
                                CreateItem(202009, 10, 0, 0, Item.Gem.SuperGloryGem, Item.Gem.SuperGloryGem, 0); //Star
                                CreateItem(201009, 10, 0, 0, Item.Gem.SuperThunderGem, Item.Gem.SuperThunderGem, 0); //Fan
                                CreateItem(500129, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Fan
                            }
                            break;
                        }
                    #endregion
                    #region Taoist Beginner Items
                    case 723772:
                        {
                            if (Inventory.Count <= 33)
                            {
                                RemoveItemIDAmount(723772,  1);
                                CreateItem(134059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Robe
                                CreateItem(117059, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Earring
                                CreateItem(121099, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0);//Neck
                                CreateItem(152129, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Ring
                                CreateItem(160119, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //boots
                                CreateItem(202009, 10, 0, 0, Item.Gem.SuperGloryGem, Item.Gem.SuperGloryGem, 0); //Star
                                CreateItem(201009, 10, 0, 0, Item.Gem.SuperThunderGem, Item.Gem.SuperThunderGem, 0); //Fan
                                CreateItem(421139, 10, 0, 0, Item.Gem.SuperDragonGem, Item.Gem.SuperDragonGem, 0); //Fan
                            }
                            break;
                        }
                    #endregion

                    #region FlamesStones
                    case 729960:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 1º Flame Cristal is (350,327)");
                            break;
                        }
                    case 729961:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 2º Flame Cristal is (317,270)");
                            break;
                        }
                    case 729962:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 3º Flame Cristal is (236,291)");
                            break;
                        }
                    case 729963:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 4º Flame Cristal is (194,168)");
                            break;
                        }
                    case 729964:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 5º Flame Cristal is (115,53)");
                            break;
                        }
                    case 729965:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 6º Flame Cristal is (316,378)");
                            break;
                        }
                    case 729966:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 7º Flame Cristal is (136,182)");
                            break;
                        }
                    case 729967:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 8º Flame Cristal is (38,94)");
                            break;
                        }
                    case 729968:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 9º Flame Cristal is in Twin City (350,321)");
                            break;
                        }
                    case 729969:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "The 10º Flame Cristal is (62,59)");
                            break;
                        }
                    case 729970:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "Go to the FlameToist in Twin City (355,325)");
                            break;
                        }
                    #endregion
                    #region SteedPacks
                    case 723855://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 1;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;//2
                            It.RBG[1] = 150;//1
                            It.RBG[2] = 0;//3
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It);
                            break;
                        }
                    case 723856:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 1;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It);
                            break;
                        }
                    case 723859://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 2;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 150;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723860:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 2;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723861://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 3;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 150;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723862:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 3;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723863://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 4;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 150;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723864:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 4;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723865://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 5;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 150;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723900:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 5;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723901://RedDragon
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 6;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 255;
                            It.RBG[1] = 150;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    case 723902:
                        {
                            Item It = new Item();
                            It.ID = 300000;
                            It.MaxDur = It.ItemDBInfo.Durability;
                            It.CurDur = It.MaxDur;
                            It.Plus = 6;
                            It.UID = (uint)Program.Rnd.Next(int.MaxValue);
                            It.Effect = Server.Game.Item.RebornEffect.Horsie;
                            It.RBG[0] = 150;
                            It.RBG[1] = 225;
                            It.RBG[2] = 0;
                            It.RBG[3] = 0;
                            It.TalismanProgress = BitConverter.ToUInt32(It.RBG, 0);
                            RemoveItemUIDAmount(I.UID,  1);
                            CreateReadyItem(It); 
                            break;
                        }
                    #endregion
                    #region exceptionalToken
                    case 723701:
                        {
                            if (Reborns == 1)
                            {
                                rebornquest = 1;
                                MyClient.LocalMessage(2011, System.Drawing.Color.Green, "CONGRATION! " + Name + " has finish 2nd Reborn Quest");
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Sorry, but you dont is 1st reborn for using this");

                            break;
                        }
                    #endregion
                    #region PointCard
                    case 780000:
                        {
                            CPs += 80;
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region LifeFruitBasket
                    case 723725:
                        {
                            if (Inventory.Count < 30)
                            {
                                for (int x = 0; x < 10; x++)
                                    CreateItemIDAmount(723726, 1);
                                RemoveItemIDAmount(723725,  1);
                            }
                            break;
                        }
                    #endregion
                    #region LifeFruit
                    case 723726:
                        {
                            CurHP = MaxHP;
                            CurMP = MaxMP;
                            RemoveItemIDAmount(I.ID,  1);
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
                                RemoveItemIDAmount(723584,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You've gotta wear an armor...");
                                return;
                            }
                            break;
                        }
                    #endregion

                    #region HP Pots
                    case 1000030:
                        {
                            if(DateTime.Now < lastHPUse.AddSeconds(2))
                                return;
                            if (CurHP < MaxHP)
                            {
                                CurHP += 500; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                                lastHPUse = DateTime.Now;
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002000:
                        {
                            if (DateTime.Now < lastHPUse.AddSeconds(2))
                                return;
                            if (CurHP < MaxHP)
                            {
                                CurHP += 1000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                                lastHPUse = DateTime.Now;
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002010:
                        {
                            if (DateTime.Now < lastHPUse.AddSeconds(2))
                                return;
                            if (CurHP < MaxHP)
                            {
                                CurHP += 2000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                                lastHPUse = DateTime.Now;
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002020:
                        {
                            if (DateTime.Now < lastHPUse.AddSeconds(2))
                                return;
                            if (CurHP < MaxHP)
                            {
                                CurHP += 3000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                                lastHPUse = DateTime.Now;
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You HP is currently full.");
                            }
                            break;
                        }
                    case 1002050://UltraHP
                        {
                            if (DateTime.Now < lastHPUse.AddSeconds(2))
                                return;

                            if (CurHP < MaxHP)
                            {

                                CurHP += 5000; if (CurHP > MaxHP)
                                {
                                    CurHP = MaxHP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                                lastHPUse = DateTime.Now;
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You HP is currently full.");
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
                                RemoveItemIDAmount(I.ID,  1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region Panacea Box
                    case 720011:
                        {
                            uint DrugID = 1002000;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemIDAmount(I.ID,  1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
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
                                RemoveItemIDAmount(I.ID,  1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region Vanilla Box
                    case 720013://Vanilla 
                        {
                            uint DrugID = 1002020;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemIDAmount(I.ID,  1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region UltraHP Box
                    case 720018://UltraHP 
                        {
                            uint DrugID = 1002050;
                            if (Inventory.Count <= 35)
                            {
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 5);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region MP Pots
                    case 1001030:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 500;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "You MP is currently full.");
                            }
                            break;
                        }
                    case 1001040:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 1000;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "You MP is currently full.");
                            }
                        }
                        break;
                    case 1002030:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 2000;
                                if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "You MP is currently full.");
                            }
                            break;
                        }
                    case 1002040:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 3000; if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "You MP is currently full.");
                            }
                            break;
                        }
                    case 1002060:
                        {
                            if (CurMP < MaxMP)
                            {
                                CurMP += 5000; if (CurMP > MaxMP)
                                {
                                    CurMP = MaxMP;
                                }
                                RemoveItemIDAmount(I.ID, 1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Blue, "You MP is currently full.");
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
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
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
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region RefreshingPill
                    case 720016:
                        {
                            uint DrugID = 1002030;
                            if (Inventory.Count <= 37)
                            {
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
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
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 3);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
                            }
                            break;
                        }
                    #endregion
                    #region UltraMP Box
                    case 720019://UltraMP 
                        {
                            uint DrugID = 1002060;
                            if (Inventory.Count <= 35)
                            {
                                RemoveItemIDAmount(I.ID, 1);
                                CreateItemIDAmount(DrugID, 5);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "No Place In Your Items.");
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

                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region WindSpells
                    case 1060025:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 411, 704);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }


                    #region WindSpells2
                    case 1060026:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 96, 323);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060027:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1002, 795, 465);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060028:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 538, 772);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060029:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 734, 452);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060031:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 824, 601);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060032:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 491, 731);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060033:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1020, 106, 394);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060034:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1000, 225, 205);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060035:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1000, 793, 549);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060037:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1001, 470, 366);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    case 1060038:
                        {
                            if (Job >= 130 && Job <= 135)
                            {
                                Teleport(1011, 67, 423);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "You Cannot use because you are not water!");
                            }
                            break;
                        }
                    #endregion
                    case 725016:
                        {
                            if (Level >= 80)
                            {
                                NewSkill(new Skill() { ID = 1360 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                            {
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use NightDevil at your Level!");
                            }
                            break;
                        }
                    #endregion
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
                            RemoveItemIDAmount(1200000,  1);
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
                            RemoveItemIDAmount(1200001,  1);
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
                            RemoveItemIDAmount(1200002,  1);
                            break;
                        }
                    #endregion
                    #region ExpPotion
                    case 723017:
                        {
                            if (Job >= 11 && Job <= 150)
                            {
                                ExpPotionUsed = DateTime.Now;
                                DoubleExp = true;
                                DoubleExpLeft = 3600;

                                // MyClient.SendPacket(Packets.Status(EntityID, Status.flower, pote));
                                //MyClient.SendPacket(Packets.Status(EntityID, Status.PotFromMentor, pote));
                                MyClient.SendPacket(Packets.Status(EntityID, Status.DoubleExpTime, (ulong)DoubleExpLeft));
                                //MyClient.SendPacket(Packets.Status(EntityID, Status.ClanBatPower, 26033380));
                                //     MyClient.SendPacket(Packets.Status(EntityID, Status.a, 26033380));
                                RemoveItemIDAmount(723017,  1);
                            }
                            break;
                        }
                    #endregion
                    #region ExpBall
                    case 723834:
                    case 722136:
                    case 723911:
                    case 723700:
                        {
                            if (Job >= 11 && Job <= 150 && ExpBallsUsedToday < 10)
                            {
                                if (Level < 137)
                                {
                                    IncreaseExp(EXPBall, false);

                                    RemoveItemIDAmount(I.ID,  1);
                                    ExpBallsUsedToday++;
                                    //MyClient.SendPacket(Packets.String(EntityID, 10, "fam_gain_special"));
                                }
                                else MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Sorry, but you dont have hight level for using expball");
                            }
                            else
                                MyClient.LocalMessage(2000, System.Drawing.Color.Green, "You can only use 10 Exp Balls in one day.");
                            break;
                        }
                    #endregion
                    #region PowerExpBall
                    case 723744:
                        {
                            if (Job >= 11 && Job <= 150 && PEBUsedToday < 10)
                            {
                                if (Level < 137)
                                {
                                    IncreaseExp(PowerEXPBall, false);
                                    RemoveItemIDAmount(I.ID,  1);
                                    PEBUsedToday++;
                                    //MyClient.SendPacket(Packets.String(EntityID, 10, "fam_gain_special "));
                                }
                            }
                            else
                                MyClient.LocalMessage(2000, System.Drawing.Color.Green, "You can only use 10 PowerEXPBalls in one day.");
                            break;
                        }
                    #endregion
                    #region Class1MoneyBag
                    case 723713:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 300000;
                            break;
                        }
                    #endregion
                    #region Class2MoneyBag
                    case 723714:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 800000;
                            break;
                        }
                    #endregion
                    #region Class3MoneyBag
                    case 723715:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 1200000;
                            break;
                        }
                    #endregion
                    #region Class4MoneyBag
                    case 723716:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 1800000;
                            break;
                        }
                    #endregion
                    #region Class5MoneyBag
                    case 723717:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 5000000;
                            break;
                        }
                    #endregion
                    #region Class6MoneyBag
                    case 723718:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 20000000;
                            break;
                        }
                    #endregion
                    #region Class7MoneyBag
                    case 723719:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 25000000;
                            break;
                        }
                    #endregion
                    #region Class8MoneyBag
                    case 723720:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 80000000;
                            break;
                        }
                    #endregion
                    #region Class9MoneyBag
                    case 723721:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 100000000;
                            break;
                        }
                    #endregion
                    #region Class10MoneyBag
                    case 723722:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 300000000;
                            break;
                        }
                    #endregion
                    #region Class11MoneyBag
                    case 7237223:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 600000000;
                            break;
                        }
                    #endregion
                    #region TopMoneyBag
                    case 723723:
                        {
                            RemoveItemIDAmount(I.ID,  1);
                            Silvers += 500000000;
                            break;
                        }
                    #endregion

                    #region DBScroll
                    case 720028:
                        {
                            if (Inventory.Count < 30)
                            {
                                RemoveItemIDAmount(720028,  1);
                                for (int i = 0; i < 10; i++)
                                    CreateItemIDAmount(1088000, 1);
                            }
                            break;
                        }
                    #endregion
                    #region MeteorScroll
                    case 720027:
                        {
                            if (Inventory.Count < 30)
                            {
                                RemoveItemIDAmount(720027,  1);
                                for (int i = 0; i < 10; i++)
                                    CreateItemIDAmount(1088001, 1);
                            }
                            break;
                        }
                    #endregion
                    
                    #region SkillBooks
                    case 725000:
                        {
                            if (Reborns >= 1)
                            {
                                NewSkill(new Skill() { ID = 1000, Lvl = 4 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, You Must Be First Reborn .");
                            break;
                        }
                    case 725001:
                        {
                            if (Job >= 132 && Job <= 135 || Job >= 142 && Job <= 145|| Job == 100 || Job == 101 && Spi == 80 && Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1001, Lvl = 3 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for WaterTaoist or FireTaoist or you dont have level 40+");
                            break;
                        }
                    case 725002:
                        {
                            if (Job >= 142 && Job <= 145 && Spi == 160)
                            {
                                NewSkill(new Skill() { ID = 1002, Lvl = 3 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for FireTaoist or you dont have level 90+");
                            break;
                        }
                    case 725003:
                        {
                            
                            //if (Job >= 132 && Job <= 135 || Job >= 142 && Job <= 145 || Job == 100 || Job == 101)
                            if (Spi >= 30)
                            {
                                NewSkill(new Skill() { ID = 1005, Lvl = 4 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but You Dont Have 30 Spirit");
                            break;
                        }
                    case 725004:
                        {
                            if (Job >= 132 && Job <= 135 || Job >= 142 && Job <= 145 && Level >= 15 || Job == 100 || Job == 101)
                            {
                                NewSkill(new Skill() { ID = 1010 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for WaterTaoist or FireTaoist or you dont have level 15+");
                            break;
                        }
                    case 725005:
                        {
                            if (Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1045 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but need level 40 for using the skill");
                            break;
                        }
                    case 725010:
                        {
                            if (Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1046 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but need level 40 for using the skill");
                            break;
                        }
                    case 725011:
                        {
                            NewSkill(new Skill() { ID = 1250 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725012:
                        {
                            NewSkill(new Skill() { ID = 1260 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725013:
                        {
                            NewSkill(new Skill() { ID = 1290 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725014:
                        {
                            NewSkill(new Skill() { ID = 1300 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725015:
                        {
                            if (Job >= 132 && Job <= 135)
                            {
                                NewSkill(new Skill() { ID = 1350 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for WaterTaoist");
                            break;
                        }
                    case 725019:
                        {
                            NewSkill(new Skill() { ID = 1385 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725020:
                        {
                            NewSkill(new Skill() { ID = 1390 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725021:
                        {
                            NewSkill(new Skill() { ID = 1395 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725022:
                        {
                            NewSkill(new Skill() { ID = 1400 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725023:
                        {
                            NewSkill(new Skill() { ID = 1405 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725024:
                        {
                            NewSkill(new Skill() { ID = 1410 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725025:
                        {
                            if (Job >= 20 && Job <= 25 && Level >= 40)
                            {
                                NewSkill(new Skill() { ID = 1320 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for Warriors and need level 40");
                            break;
                        }
                    case 725026:
                        {
                            NewSkill(new Skill() { ID = 5010 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725027:
                        {
                            NewSkill(new Skill() { ID = 5020 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725028:
                        {
                            if (Job >= 132 && Job <= 145 && Level >= 70)
                            {
                                NewSkill(new Skill() { ID = 5001 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for Fire & WaterTaoist and need level 70");
                            break;
                        }
                    case 725029:
                        {
                            NewSkill(new Skill() { ID = 5030 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725030:
                        {
                            NewSkill(new Skill() { ID = 5040 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725031:
                        {
                            NewSkill(new Skill() { ID = 5050 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725040:
                        {
                            NewSkill(new Skill() { ID = 7000 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725041:
                        {
                            NewSkill(new Skill() { ID = 7010 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725042:
                        {
                            NewSkill(new Skill() { ID = 7020 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725043:
                        {
                            NewSkill(new Skill() { ID = 7030 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 725044:
                        {
                            NewSkill(new Skill() { ID = 7040 });
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060100:
                        {
                            if (Job >= 140 && Job <= 145 && Level >= 82)
                            {
                                NewSkill(new Skill() { ID = 1160 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for FireTaoist and need level 82");
                            break;
                        }
                    case 1060101:
                        {
                            if (Job >= 140 && Job <= 145 && Level >= 84)
                            {
                                NewSkill(new Skill() { ID = 1165 });
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else MyClient.LocalMessage(2005, System.Drawing.Color.Brown, "Sorry, but the skill is for FireTaoist and need level 84");
                            break;
                        }
                    #endregion
                    #region Teleport Scrolls
                    case 1060020:
                        {
                            if (Job >= 11 && Job <= 150 && Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1615 && Loc.Map != 1707 && Loc.Map != 1068)
                            {
                                Teleport(1002, 429, 378);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use teleport scrolls in jail , AvatarWorld and ForbbedenMaps.");
                            break;
                        }
                    case 1060021:
                        {
                            if (Job >= 11 && Job <= 150 && Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1615 && Loc.Map != 1707 && Loc.Map != 1068)
                            {
                                Teleport(1000, 500, 650);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use teleport scrolls in jail , AvatarWorld and ForbbedenMaps.");
                            break;
                        }
                    case 1060022:
                        {
                            if (Job >= 11 && Job <= 150 && Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1615 && Loc.Map != 1707 && Loc.Map != 1068)
                            {
                                Teleport(1020, 565, 562);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use teleport scrolls in jail , AvatarWorld and ForbbedenMaps.");
                            break;
                        }
                    case 1060023:
                        {
                            if (Job >= 11 && Job <= 150 && Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1615 && Loc.Map != 1707 && Loc.Map != 1068)
                            {
                                Teleport(1011, 188, 264);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use teleport scrolls in jail , AvatarWorld and ForbbedenMaps.");
                            break;
                        }
                    case 1060024:
                        {
                            if (Job >= 11 && Job <= 150 && Loc.Map != 6000 && Loc.Map != 6001 && Loc.Map != 1615 && Loc.Map != 1707 && Loc.Map != 1068)
                            {
                                Teleport(1015, 717, 571);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            else
                                MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Cannot use teleport scrolls in jail , AvatarWorld and ForbbedenMaps.");
                            break;
                        }
                    #endregion
                    #region HairDye Pots
                    case 1060030:
                        {
                            Hair = ushort.Parse("3" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060040:
                        {
                            Hair = ushort.Parse("9" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060050:
                        {
                            Hair = ushort.Parse("8" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060060:
                        {
                            Hair = ushort.Parse("7" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060070:
                        {
                            Hair = ushort.Parse("6" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060080:
                        {
                            Hair = ushort.Parse("5" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    case 1060090:
                        {
                            Hair = ushort.Parse("4" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region Fireworks
                    case 720030: //fireworks
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            World.Action(this, Packets.ItemPacket(EntityID, 255, 26));
                            RemoveItemIDAmount(I.ID,  1);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Plum, "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    case 720031: //fireworks EndlessLove
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "firework-2love"));
                            RemoveItemIDAmount(I.ID,  1);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Plum, "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    case 720032: //fireworks MyWish
                        if (Stamina >= 100)
                        {
                            Stamina -= 100;
                            MyClient.SendPacket(Packets.String(EntityID, 10, "firework-like"));
                            RemoveItemIDAmount(I.ID,  1);
                        }
                        else
                            MyClient.LocalMessage(2005, System.Drawing.Color.Plum, "Sorry, you cannot use firework until you have full stamina.");
                        break;
                    #endregion

                    #region DemonBox
                    case 721620:
                        {


                            foreach (DictionaryEntry DE in Game.World.H_Chars)
                            {
                                Game.Character Chaar = (Game.Character)DE.Value;
                                Mob Demon = new Mob("106 Demon 1 731 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                                Location Loca = new Location();
                                Loca.Map = Loc.Map;
                                Loca.X = Loc.Y;
                                Loca.Y = Loc.X;
                                Demon.EntityID = 450;
                                Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loca.Map];
                                DMap D = (DMap)DMaps.H_Maps[Loca.Map];
                                Game.Mob _Mob = new Server.Game.Mob(Demon);
                                _Mob.Loc = new Server.Game.Location();
                                _Mob.Loc.Map = Loca.Map;
                                _Mob.Loc.X = Loca.Y;
                                _Mob.Loc.Y = Loca.X;


                                _Mob.StartLoc = _Mob.Loc;
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                                while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                    _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                                MapMobs.Add(_Mob.EntityID, _Mob);

                                RemoveItemIDAmount(I.ID,  1);

                            }
                            break;
                        }


                    #endregion
                    #region AncientDemonBox
                    case 721955:
                        {
                            foreach (DictionaryEntry DE in Game.World.H_Chars)
                            {
                                Game.Character Chaar = (Game.Character)DE.Value;
                                Mob Demon = new Mob("101 AncientDemon 1 556 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                                Location Loca = new Location();
                                Loca.Map = Loc.Map;
                                Loca.X = Loc.Y;
                                Loca.Y = Loc.X;
                                Demon.EntityID = 450;
                                Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                                DMap D = (DMap)DMaps.H_Maps[Loc.Map];
                                Game.Mob _Mob = new Server.Game.Mob(Demon);
                                _Mob.Loc = new Server.Game.Location();
                                _Mob.Loc.Map = Loc.Map;
                                _Mob.Loc.X = Loc.X;
                                _Mob.Loc.Y = Loc.Y;


                                _Mob.StartLoc = _Mob.Loc;
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                                while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                    _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                                MapMobs.Add(_Mob.EntityID, _Mob);
                                RemoveItemIDAmount(I.ID,  1);
                            }
                            break;
                        }

                    #endregion
                    #region FloodDemonBox
                    case 721851:
                        {

                            Mob Demon = new Mob("102 FloodDemon 1 217 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                            Location Loca = new Location();
                            Loca.Map = Loc.Map;
                            Loca.X = Loc.Y;
                            Loca.Y = Loc.X;
                            Demon.EntityID = 450;
                            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                            DMap D = (DMap)DMaps.H_Maps[Loc.Map];
                            Game.Mob _Mob = new Server.Game.Mob(Demon);
                            _Mob.Loc = new Server.Game.Location();
                            _Mob.Loc.Map = Loc.Map;
                            _Mob.Loc.X = Loc.X;
                            _Mob.Loc.Y = Loc.Y;


                            _Mob.StartLoc = _Mob.Loc;
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                            while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                            MapMobs.Add(_Mob.EntityID, _Mob);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region HeavenDemonBox
                    case 721858:
                        {

                            Mob Demon = new Mob("103 HeavenDemon 1 252 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                            Location Loca = new Location();
                            Loca.Map = Loc.Map;
                            Loca.X = Loc.Y;
                            Loca.Y = Loc.X;
                            Demon.EntityID = 450;
                            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                            DMap D = (DMap)DMaps.H_Maps[Loc.Map];
                            Game.Mob _Mob = new Server.Game.Mob(Demon);
                            _Mob.Loc = new Server.Game.Location();
                            _Mob.Loc.Map = Loc.Map;
                            _Mob.Loc.X = Loc.X;
                            _Mob.Loc.Y = Loc.Y;


                            _Mob.StartLoc = _Mob.Loc;
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                            while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                            MapMobs.Add(_Mob.EntityID, _Mob);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region ChaosDemonBox
                    case 722041:
                        {

                            Mob Demon = new Mob("104 ChaosDemon 1 225 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                            Location Loca = new Location();
                            Loca.Map = Loc.Map;
                            Loca.X = Loc.Y;
                            Loca.Y = Loc.X;
                            Demon.EntityID = 450;
                            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                            DMap D = (DMap)DMaps.H_Maps[Loc.Map];
                            Game.Mob _Mob = new Server.Game.Mob(Demon);
                            _Mob.Loc = new Server.Game.Location();
                            _Mob.Loc.Map = Loc.Map;
                            _Mob.Loc.X = Loc.X;
                            _Mob.Loc.Y = Loc.Y;


                            _Mob.StartLoc = _Mob.Loc;
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                            while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                            MapMobs.Add(_Mob.EntityID, _Mob);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    #region SacredDemonBox
                    case 722042:
                        {

                            Mob Demon = new Mob("105 SacredDemon 1 950 1 33 0 0 0 5 6 1 36 2 True 18 100 500 500 3 True");
                            Location Loca = new Location();
                            Loca.Map = Loc.Map;
                            Loca.X = Loc.Y;
                            Loca.Y = Loc.X;
                            Demon.EntityID = 450;
                            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                            DMap D = (DMap)DMaps.H_Maps[Loc.Map];
                            Game.Mob _Mob = new Server.Game.Mob(Demon);
                            _Mob.Loc = new Server.Game.Location();
                            _Mob.Loc.Map = Loc.Map;
                            _Mob.Loc.X = Loc.X;
                            _Mob.Loc.Y = Loc.Y;


                            _Mob.StartLoc = _Mob.Loc;
                            _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);
                            while (Game.World.H_Chars.Contains(_Mob.EntityID) || MapMobs.Contains(_Mob.EntityID))
                                _Mob.EntityID = (uint)Program.Rnd.Next(400000, 500000);

                            MapMobs.Add(_Mob.EntityID, _Mob);
                            RemoveItemIDAmount(I.ID,  1);
                            break;
                        }
                    #endregion
                    default:
                        {
                            MyClient.LocalMessage(2005, System.Drawing.Color.Red , "Unable to use the item.");
                            break;
                        }
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
    }
}
