using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Extra;
using Server.Game;
using Server.Skills;

namespace Server.Skills
{
    public unsafe class SkillsClass
    {
        public static Hashtable SkillInfos = new Hashtable();
        public static Hashtable WepSkillIDs = new Hashtable();
            
        public enum ExtraEffect : byte
        {
            None,
            Stigma,
            MagicShield,
            Accuracy,
            Superman,
            Cyclone,
            Invisibility,
            Revive,
            Poison,
            Fly,
            Transform,
            Summon,
            FatalStrike,
            ShurikenVortex,
            RemoveFly,
            FlashStep,
            Ride,
            UnMount,
            NoPots,
            Scapegoat,
            BlessPray
        }
        public enum TargetType : byte
        {
            Single,
            FromSingle,
            FromPoint,
            Range,
            Sector,
            Linear
        }
        public enum DamageType : byte
        {
            Magic,
            Ranged,
            Melee,
            HealHP,
            HealMP,
            Percent
        }
        public struct SkillInfo
        {
            public ushort ID;
            public byte Level;
            public ushort ManaCost;
            public byte StaminaCost;
            public byte ArrowsCost;
            public bool EndsXPWait;
            public byte UpgReqLvl;
            public uint UpgReqExp;
            public uint Damage;
            public TargetType Targetting;
            public DamageType Damageing;
            public ExtraEffect ExtraEff;
            public ushort EffectLasts;
            public float EffectValue;
            public byte ActivationChance;
            public byte MaxDist;
            public byte SectorSize;

            public void LoadThis(BinaryReader BR)
            {
                ID = BR.ReadUInt16();
                Level = BR.ReadByte();
                ManaCost = BR.ReadUInt16();
                StaminaCost = BR.ReadByte();
                ArrowsCost = BR.ReadByte();
                EndsXPWait = BR.ReadBoolean();
                UpgReqLvl = BR.ReadByte();
                UpgReqExp = BR.ReadUInt32();
                Damage = BR.ReadUInt32();
                Targetting = (TargetType)BR.ReadByte();
                Damageing = (DamageType)BR.ReadByte();
                ExtraEff = (ExtraEffect)BR.ReadByte();
                EffectLasts = BR.ReadUInt16();
                EffectValue = BR.ReadSingle();
                ActivationChance = BR.ReadByte();
                MaxDist = BR.ReadByte();
                SectorSize = BR.ReadByte();
            }
            public void SaveThis(BinaryWriter BW)
            {
                BW.Write(ID);
                BW.Write(Level);
                BW.Write(ManaCost);
                BW.Write(StaminaCost);
                BW.Write(ArrowsCost);
                BW.Write(EndsXPWait);
                BW.Write(UpgReqLvl);
                BW.Write(UpgReqExp);
                BW.Write(Damage);
                BW.Write((byte)Targetting);
                BW.Write((byte)Damageing);
                BW.Write((byte)ExtraEff);
                BW.Write(EffectLasts);
                BW.Write(EffectValue);
                BW.Write(ActivationChance);
                BW.Write(MaxDist);
                BW.Write(SectorSize);
            }
        }
        public struct SkillUse
        {
            public SkillInfo Info;
            public Hashtable MobTargets;
            public Hashtable CompanionTargets;
            public Hashtable PlayerTargets;

            public Hashtable NPCTargets;
            public Hashtable MiscTargets;
            public Game.Character User;
            public Game.Mob MUser;
            public ushort AimX;
            public ushort AimY;

            public void Init(Game.Character C, ushort SkillID, byte SkillLvl, ushort AimX, ushort AimY)
            {
                try
                {
                    User = C;
                    Info = (SkillInfo)SkillsClass.SkillInfos[SkillID + " " + SkillLvl];
                    this.AimX = AimX;
                    this.AimY = AimY;
                    CompanionTargets = new Hashtable();
                    MobTargets = new Hashtable();
                    PlayerTargets = new Hashtable();
                    NPCTargets = new Hashtable();
                    MiscTargets = new Hashtable();
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            public void Init(Game.Mob C, ushort SkillID, byte SkillLvl, ushort AimX, ushort AimY)
            {
                try
                {
                    MUser = C;
                    Info = (SkillInfo)SkillsClass.SkillInfos[SkillID + " " + SkillLvl];
                    this.AimX = AimX;
                    this.AimY = AimY;
                    CompanionTargets = new Hashtable();
                    MobTargets = new Hashtable();
                    PlayerTargets = new Hashtable();

                    NPCTargets = new Hashtable();
                    MiscTargets = new Hashtable();
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            public bool InSector(ushort X, ushort Y)
            {
                if (User != null)
                {
                double Aim = MyMath.PointDirecton2(User.Loc.X, User.Loc.Y, AimX, AimY);
                double MobAngle = MyMath.PointDirecton2(User.Loc.X, User.Loc.Y, X, Y);

                if (Aim - Info.SectorSize / 2 < MobAngle)
                    if (Aim + Info.SectorSize / 2 > MobAngle)
                        return true;
                }
                else
                {
                double Aim = MyMath.PointDirecton2(MUser.Loc.X, MUser.Loc.Y, AimX, AimY);
                double MobAngle = MyMath.PointDirecton2(MUser.Loc.X, MUser.Loc.Y, X, Y);

                if (Aim - Info.SectorSize / 2 < MobAngle)
                    if (Aim + Info.SectorSize / 2 > MobAngle)
                        return true;
                }
                
                return false;
            }

            public void GetTargets(uint Single)
            {
                try
                {
                    GetMobTargets(Single);
                    GetPlayerTargets(Single);
                    GetNPCTargets(Single);
                    GetMiscTargets(Single);
                    GetCompanionTargets(Single);
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            void GetPlayerTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.FlashStep || Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.Scapegoat)
                {
                    PlayerTargets.Add(User, (uint)0);
                    return;
                }
                if (Info.Targetting == TargetType.Single)
                {
                    Character C = (Character)World.H_Chars[Single];
                    if (C != null && C.Alive)
                        PlayerTargets.Add(C, GetDamage(C));
                }
                else
                {
                    bool RangeFromChar = true;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        Character C = (Character)World.H_Chars[Single];
                        if (C != null && C.Alive)
                        {
                            PlayerTargets.Add(C, GetDamage(C));
                            AimX = C.Loc.X;
                            AimY = C.Loc.Y;
                            RangeFromChar = false;
                        }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = User.Loc.X;
                        AimY = User.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = false;
                    List<coords> Line = new List<coords>(5);
                    List<coords> Line2 = new List<coords>(5);
                    List<coords> Line3 = new List<coords>(5);
                    List<coords> Line4 = new List<coords>(5);
                    List<coords> Line5 = new List<coords>(5);

                    if (Info.Targetting == TargetType.Linear)
                    {

                        Line = MyMath.LineCoords(User.Loc.X, User.Loc.Y, (ushort)(AimX + 1), AimY, 10);
                        Line2 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, (ushort)(AimX - 1), AimY, 10);
                        Line3 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, AimY, 10);
                        Line4 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX,(ushort)(AimY + 1) , 10);
                        Line5 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX,(ushort)(AimY - 1) , 10);
                    }
                    foreach (Character C in World.H_Chars.Values)
                    {
                        if (C.EntityID != User.EntityID && C.Alive && (C != User || Info.Damageing == DamageType.HealHP || Info.Damageing == DamageType.HealMP) && User.Loc.Map == C.Loc.Map && User.Loc.MapDimention == C.Loc.MapDimention)
                        {
                            if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, C.Loc.X, C.Loc.Y) <= Info.MaxDist) || MyMath.PointDistance(User.Loc.X, User.Loc.Y, C.Loc.X, C.Loc.Y) <= Info.MaxDist)
                                if (Info.Targetting == TargetType.Sector && InSector(C.Loc.X, C.Loc.Y) || Info.Targetting != TargetType.Sector)
                                    if (Info.Targetting == TargetType.Linear && (Line.Contains(new coords(C.Loc.X, C.Loc.Y)) || Line2.Contains(new coords(C.Loc.X, C.Loc.Y)) || Line3.Contains(new coords(C.Loc.X, C.Loc.Y)) || Line4.Contains(new coords(C.Loc.X, C.Loc.Y)) || Line5.Contains(new coords(C.Loc.X, C.Loc.Y))) || Info.Targetting != TargetType.Linear)
                                        if (C.PKAble(User.PKMode, User) && !PlayerTargets.Contains(C) && !DMaps.NoPKMaps.Contains(User.Loc.Map) || Info.ExtraEff == ExtraEffect.UnMount)
                                            if (DMaps.NoPKMaps.Contains(User.Loc.Map) && GetDamage(C) == 0 || !DMaps.NoPKMaps.Contains(User.Loc.Map) || (Info.ExtraEff == ExtraEffect.UnMount && C.StatEff.Contains(StatusEffectEn.Ride)) && C.Equips.Steed.Plus < User.Equips.Steed.Plus)
                                                if (true)
                                                    PlayerTargets.Add(C, GetDamage(C));
                        }
                    }
                }
            }
            void GetMobTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    if (World.H_Mobs.Contains(User.Loc.Map))
                    {
                        Mob M = (Mob)((Hashtable)(World.H_Mobs[User.Loc.Map]))[Single];
                        if (M != null && M.Alive)
                            MobTargets.Add(M, GetDamage(M));
                    }
                }
                else
                {
                    bool RangeFromChar = true;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        try
                        {
                            Mob M = (Mob)((Hashtable)(World.H_Mobs[User.Loc.Map]))[Single];

                            if (M != null && M.Alive)
                            {
                                MobTargets.Add(M, GetDamage(M));
                                AimX = M.Loc.X;
                                AimY = M.Loc.Y;
                                RangeFromChar = false;
                            }
                        }
                        catch { }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = User.Loc.X;
                        AimY = User.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = false;
                    Hashtable MapMobs = (Hashtable)World.H_Mobs[User.Loc.Map];
                    List<coords> Line = new List<coords>(5);
                    List<coords> Line2 = new List<coords>(5);
                    List<coords> Line3 = new List<coords>(5);
                    List<coords> Line4 = new List<coords>(5);
                    List<coords> Line5 = new List<coords>(5);

                    if (Info.Targetting == TargetType.Linear)
                    {

                        Line = MyMath.LineCoords(User.Loc.X, User.Loc.Y, (ushort)(AimX + 1), AimY, 10);
                        Line2 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, (ushort)(AimX - 1), AimY, 10);
                        Line3 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, AimY, 10);
                        Line4 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, (ushort)(AimY + 1), 10);
                        Line5 = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, (ushort)(AimY - 1), 10);
                    }
                    if (MapMobs != null)
                        foreach (Mob M in MapMobs.Values)
                        {
                            if (M.Alive && User.Loc.Map == M.Loc.Map && User.Loc.MapDimention == M.Loc.MapDimention)
                            {
                                if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, M.Loc.X, M.Loc.Y) <= Info.MaxDist) || MyMath.PointDistance(User.Loc.X, User.Loc.Y, M.Loc.X, M.Loc.Y) <= Info.MaxDist)
                                    if (Info.Targetting == TargetType.Sector && InSector(M.Loc.X, M.Loc.Y) || Info.Targetting != TargetType.Sector)
                                        if (Info.Targetting == TargetType.Linear && (Line.Contains(new coords(M.Loc.X, M.Loc.Y)) || Line2.Contains(new coords(M.Loc.X, M.Loc.Y)) || Line3.Contains(new coords(M.Loc.X, M.Loc.Y)) || Line4.Contains(new coords(M.Loc.X, M.Loc.Y)) || Line5.Contains(new coords(M.Loc.X, M.Loc.Y))) || Info.Targetting != TargetType.Linear)
                                            if (User.PKMode == PKMode.PK || !M.NeedsPKMode && !MobTargets.Contains(M))
                                                try
                                                {
                                                    MobTargets.Add(M, GetDamage(M));
                                                }
                                                catch { }
                            }
                        }
                }
            }
            void GetCompanionTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    Companion C = (Companion)World.H_Companions[Single];
                    if (C != null && C.Alive)
                        CompanionTargets.Add(C, GetDamage(C));
                }
                else
                {
                    bool RangeFromChar = true;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        try
                        {
                            Companion M = (Companion)World.H_Companions[Single];

                            if (M != null && M.Alive)
                            {
                                CompanionTargets.Add(M, GetDamage(M));
                                AimX = M.Loc.X;
                                AimY = M.Loc.Y;
                                RangeFromChar = false;
                            }
                        }
                        catch { }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = User.Loc.X;
                        AimY = User.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = false;
                    //Hashtable MapMobs = (Hashtable)World.H_Mobs[User.Loc.Map];
                    List<coords> Line = new List<coords>(5);
                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, AimY, 10);
                    // if (MapMobs != null)
                    foreach (Companion M in Game.World.H_Companions.Values)
                    {
                        if (M.Alive && User.Loc.Map == M.Loc.Map && User.Loc.MapDimention == M.Loc.MapDimention)
                        {
                            if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, M.Loc.X, M.Loc.Y) <= Info.MaxDist) || MyMath.PointDistance(User.Loc.X, User.Loc.Y, M.Loc.X, M.Loc.Y) <= Info.MaxDist)
                                if (Info.Targetting == TargetType.Sector && InSector(M.Loc.X, M.Loc.Y) || Info.Targetting != TargetType.Sector)
                                    if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(M.Loc.X, M.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                        if (M.Owner.PKAble(User.PKMode, User) && !CompanionTargets.Contains(M))
                                            try
                                            {
                                                CompanionTargets.Add(M, GetDamage(M));
                                            }
                                            catch { }
                        }
                    }
                }
            }
            void GetNPCTargets(uint Single)
            {
                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    NPC C = (NPC)World.H_NPCs[Single];
                    if (C != null && (C.Loc.Map == 1039 || (C.EntityID >= 6700 && C.EntityID <= 6702)))
                        NPCTargets.Add(C, GetDamage(C));
                }
                else
                {
                    bool RangeFromChar = true;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        NPC C = (NPC)World.H_NPCs[Single];
                        if (C != null && (C.Loc.Map == 1039 || (C.EntityID >= 6700 && C.EntityID <= 6702)))
                        {
                            NPCTargets.Add(C, GetDamage(C));
                            AimX = C.Loc.X;
                            AimY = C.Loc.Y;
                            RangeFromChar = false;
                        }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = User.Loc.X;
                        AimY = User.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = false;
                    List<coords> Line = new List<coords>(5);
                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, AimY, 10);
                    foreach (NPC C in World.H_NPCs.Values)
                    {
                        if ((C.Flags == 21 || C.Flags == 22) && User.Level >= C.Level)
                        {
                            if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, C.Loc.X, C.Loc.Y) <= Info.MaxDist) || MyMath.PointDistance(User.Loc.X, User.Loc.Y, C.Loc.X, C.Loc.Y) <= Info.MaxDist)
                                if (Info.Targetting == TargetType.Sector && InSector(C.Loc.X, C.Loc.Y) || Info.Targetting != TargetType.Sector)
                                    if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(C.Loc.X, C.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                        if (!NPCTargets.Contains(C))
                                            NPCTargets.Add(C, GetDamage(C));
                        }
                    }
                }
            }
            void GetMiscTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    if (Single == 6700 && GuildWars.War && User.MyGuild != null && User.MyGuild!= GuildWars.LastWinner)
                        MiscTargets.Add(Single, GetDamage(GuildWars.ThePole.CurHP));
                    else if (Single == 6701 && !GuildWars.TheLeftGate.Opened && GuildWars.War)
                        MiscTargets.Add(Single, GetDamage(GuildWars.TheLeftGate.CurHP));
                    else if (Single == 6702 && !GuildWars.TheRightGate.Opened && GuildWars.War)
                        MiscTargets.Add(Single, GetDamage(GuildWars.TheRightGate.CurHP));
                }
                else
                {
                    bool RangeFromChar = true;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        if (Single == 6700 && GuildWars.War && User.MyGuild != null && User.MyGuild != GuildWars.LastWinner)
                        {
                            MiscTargets.Add(Single, GetDamage(GuildWars.ThePole.CurHP));
                            AimX = GuildWars.ThePole.Loc.X;
                            AimY = GuildWars.ThePole.Loc.Y;
                        }
                        else if (Single == 6701 && !GuildWars.TheLeftGate.Opened && GuildWars.War)
                        {
                            MiscTargets.Add(Single, GetDamage(GuildWars.TheLeftGate.CurHP));
                            AimX = GuildWars.TheLeftGate.Loc.X;
                            AimY = GuildWars.TheLeftGate.Loc.Y;
                        }
                        else if (Single == 6702 && !GuildWars.TheRightGate.Opened && GuildWars.War)
                        {
                            MiscTargets.Add(Single, GetDamage(GuildWars.TheRightGate.CurHP));
                            AimX = GuildWars.TheRightGate.Loc.X;
                            AimY = GuildWars.TheRightGate.Loc.Y;
                        }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = User.Loc.X;
                        AimY = User.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = false;

                    List<coords> Line = new List<coords>(5);
                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(User.Loc.X, User.Loc.Y, AimX, AimY, 10);
                    if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Info.MaxDist || MyMath.PointDistance(User.Loc.X, User.Loc.Y, GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) <= Info.MaxDist) && GuildWars.War && User.MyGuild != GuildWars.LastWinner)
                        if (Info.Targetting == TargetType.Sector && InSector(GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y) || Info.Targetting != TargetType.Sector)
                            if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(GuildWars.ThePole.Loc.X, GuildWars.ThePole.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                MiscTargets.Add(GuildWars.ThePole.EntityID, GetDamage(GuildWars.ThePole.CurHP));

                    if (GuildWars.War && !GuildWars.TheRightGate.Opened && (!RangeFromChar && MyMath.PointDistance(AimX, AimY, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Info.MaxDist || MyMath.PointDistance(User.Loc.X, User.Loc.Y, GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) <= Info.MaxDist))
                        if (Info.Targetting == TargetType.Sector && InSector(GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y) || Info.Targetting != TargetType.Sector)
                            if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(GuildWars.TheRightGate.Loc.X, GuildWars.TheRightGate.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                MiscTargets.Add(GuildWars.TheRightGate.EntityID, GetDamage(GuildWars.TheRightGate.CurHP));

                    if (GuildWars.War && !GuildWars.TheLeftGate.Opened && (!RangeFromChar && MyMath.PointDistance(AimX, AimY, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Info.MaxDist || MyMath.PointDistance(User.Loc.X, User.Loc.Y, GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) <= Info.MaxDist))
                        if (Info.Targetting == TargetType.Sector && InSector(GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y) || Info.Targetting != TargetType.Sector)
                            if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(GuildWars.TheLeftGate.Loc.X, GuildWars.TheLeftGate.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                MiscTargets.Add(GuildWars.TheLeftGate.EntityID, GetDamage(GuildWars.TheLeftGate.CurHP));
                }
            }

            public void M_GetTargets(uint Single)
            {
                try
                {
                    M_GetMobTargets(Single);
                    M_GetPlayerTargets(Single);
                    M_GetCompanionTargets(Single);
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            public void M_GetPlayerTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.FlashStep || Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.Scapegoat)
                {
                    PlayerTargets.Add(MUser, (uint)0);
                    return;
                }
                if (Info.Targetting == TargetType.Single)
                {
                    Character C = (Character)World.H_Chars[Single];
                    if (C != null)
                        PlayerTargets.Add(C, M_GetDamage(C));
                }
                else
                {
                    bool RangeFromChar = false;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        Character C = (Character)World.H_Chars[Single];
                        if (C != null && C.Alive)
                        {
                            PlayerTargets.Add(C, M_GetDamage(C));
                            AimX = C.Loc.X;
                            AimY = C.Loc.Y;
                            RangeFromChar = false;
                        }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = MUser.Loc.X;
                        AimY = MUser.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = true;
                    List<coords> Line = new List<coords>(5);
                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(MUser.Loc.X, MUser.Loc.Y, AimX, AimY, 10);
                    foreach (Character C in World.H_Chars.Values)
                    {
                        if (C.Alive && MUser.Loc.Map == C.Loc.Map && MUser.Loc.MapDimention == C.Loc.MapDimention && ((MUser.Type == MobType.HuntMobsAndBlue && C.BlueNameLasts > 0) || MUser.Type != MobType.HuntMobsAndBlue))
                        {
                            if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, C.Loc.X, C.Loc.Y) <= Info.MaxDist) || (RangeFromChar && MyMath.PointDistance(MUser.Loc.X, MUser.Loc.Y, C.Loc.X, C.Loc.Y) <= Info.MaxDist))
                                if (Info.Targetting == TargetType.Sector && InSector(C.Loc.X, C.Loc.Y) || Info.Targetting != TargetType.Sector)
                                    if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(C.Loc.X, C.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                        if (!PlayerTargets.ContainsKey(C))
                                            PlayerTargets.Add(C, M_GetDamage(C));
                        }
                    }
                }
            }
            public void M_GetMobTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    if (World.H_Mobs.Contains(MUser.Loc.Map))
                    {
                        Mob M = (Mob)((Hashtable)(World.H_Mobs[MUser.Loc.Map]))[Single];
                        if (M != null && M.Alive)
                            MobTargets.Add(M, M_GetDamage(M));
                    }
                }
                else
                {
                    bool RangeFromChar = false;

                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        try
                        {
                            Mob M = (Mob)((Hashtable)(World.H_Mobs[MUser.Loc.Map]))[Single];

                            if (M != null && M.Alive)
                            {
                                MobTargets.Add(M, M_GetDamage(M));
                                AimX = M.Loc.X;
                                AimY = M.Loc.Y;
                                RangeFromChar = false;
                            }
                        }
                        catch { }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = MUser.Loc.X;
                        AimY = MUser.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = true;
                    if (Info.ID == 10310)
                        RangeFromChar = false;
                    Hashtable MapMobs = (Hashtable)World.H_Mobs[MUser.Loc.Map];
                    List<coords> Line = new List<coords>(5);


                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(MUser.Loc.X, MUser.Loc.Y, AimX, AimY, 10);
                    if (MapMobs != null)

                        foreach (Mob M in MapMobs.Values)
                        {

                            if (M.Alive && MUser.Loc.Map == M.Loc.Map && MUser.Loc.MapDimention == M.Loc.MapDimention)
                            {
                                if ((M.EntityID != MUser.EntityID || (Info.Damageing == DamageType.HealHP)) && ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, M.Loc.X, M.Loc.Y) <= Info.MaxDist) || (RangeFromChar && MyMath.PointDistance(MUser.Loc.X, MUser.Loc.Y, M.Loc.X, M.Loc.Y) <= Info.MaxDist)))
                                    if (Info.Targetting == TargetType.Sector && InSector(M.Loc.X, M.Loc.Y) || Info.Targetting != TargetType.Sector)
                                        if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(M.Loc.X, M.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                            if (!MobTargets.Contains(M))
                                                try
                                                {
                                                    MobTargets.Add(M, M_GetDamage(M));
                                                }
                                                catch { }
                            }
                        }
                }
            }
            public void M_GetCompanionTargets(uint Single)
            {

                if (Info.ExtraEff == ExtraEffect.Ride || Info.ExtraEff == ExtraEffect.UnMount) return;
                if (Info.Targetting == TargetType.Single)
                {
                    Companion C = (Companion)World.H_Companions[Single];
                    if (C != null && C.Alive)
                        CompanionTargets.Add(C, M_GetDamage(C));
                }
                else
                {
                    bool RangeFromChar = false;
                    if (Info.Targetting == TargetType.FromSingle)
                    {
                        try
                        {
                            Companion M = (Companion)World.H_Companions[Single];

                            if (M != null && M.Alive)
                            {
                                CompanionTargets.Add(M, M_GetDamage(M));
                                AimX = M.Loc.X;
                                AimY = M.Loc.Y;
                                RangeFromChar = false;
                            }
                        }
                        catch { }
                    }
                    else if (Info.Targetting != TargetType.Sector && Info.Targetting != TargetType.Linear && Info.Targetting != TargetType.Range && Info.Targetting != TargetType.FromPoint)
                    {
                        AimX = MUser.Loc.X;
                        AimY = MUser.Loc.Y;
                        RangeFromChar = true;
                    }
                    else
                        RangeFromChar = true;
                    //Hashtable MapMobs = (Hashtable)World.H_Mobs[User.Loc.Map];
                    List<coords> Line = new List<coords>(5);
                    if (Info.Targetting == TargetType.Linear)
                        Line = MyMath.LineCoords(MUser.Loc.X, MUser.Loc.Y, AimX, AimY, 10);
                    // if (MapMobs != null)
                    foreach (Companion M in Game.World.H_Companions.Values)
                    {
                        if (M.Alive && MUser.Loc.Map == M.Loc.Map && MUser.Loc.MapDimention == M.Loc.MapDimention)
                        {
                            if ((!RangeFromChar && MyMath.PointDistance(AimX, AimY, M.Loc.X, M.Loc.Y) <= Info.MaxDist) || MyMath.PointDistance(MUser.Loc.X, MUser.Loc.Y, M.Loc.X, M.Loc.Y) <= Info.MaxDist)
                                if (Info.Targetting == TargetType.Sector && InSector(M.Loc.X, M.Loc.Y) || Info.Targetting != TargetType.Sector)
                                    if (Info.Targetting == TargetType.Linear && Line.Contains(new coords(M.Loc.X, M.Loc.Y)) || Info.Targetting != TargetType.Linear)
                                        if (!CompanionTargets.Contains(M))
                                            try
                                            {
                                                CompanionTargets.Add(M, M_GetDamage(M));
                                            }
                                            catch { }
                        }
                    }
                }
            }

            public uint M_GetDamage(Character C)
            {
                if (Info.ID == 5030 || Info.ID == 1290 || Info.ID ==
                              5040 || Info.ID == 7000 || Info.ID ==
                              7010 || Info.ID == 7030)
                {
                    Info.ExtraEff = ExtraEffect.None;
                    Console.WriteLine("Info.ExtraEff = ExtraEffect.None");
                }
                if (MUser.Loc.Map == C.Loc.Map && MUser.Loc.MapDimention == C.Loc.MapDimention && MyMath.ChanceSuccess(50))
                    if (Info.ID == 6004 || Info.ID == 6002)
                    {
                        uint Damage2 = 0;
                        Damage2 = (uint)(C.CurHP * 10 / 100);
                        if (Info.ID == 6004)
                        {
                            C.StatEff.Remove(StatusEffectEn.Fly);
                            // C.StatEff.Add(StatusEffectEn.attack35);
                        }
                        if (Info.ID == 6002)
                        {
                            C.UnableToUseDrugs = DateTime.Now;
                            C.UnableToUseDrugsFor = 10;
                            //
                        }
                        //Game.World.Action(MUser, Packets2.SingleMagic(MUser.EntityID, new Packets2.Target(C.EntityID, Damage2, true), this));

                        return Damage2;
                    }
                if (MUser.Loc.Map != C.Loc.Map || MUser.Loc.MapDimention != C.Loc.MapDimention)
                    return 0;
                if (Info.ExtraEff != ExtraEffect.None)
                    return 0;
                uint Damage = 1;
                if (C.Protection)
                    return 0;
                //if (Info.ExtraEff == ExtraEffect.NoPots || Info.ExtraEff == ExtraEffect.RemoveFly)
                //    return 1;

                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            if (DMaps.NoPKMaps.Contains(MUser.Loc.Map))
                                if (Info.ExtraEff == ExtraEffect.None)
                                    return 0;
                            /*if (DateTime.Now > C.InvencibleTime.AddSeconds(C.secondeimunity))
                            {
                                C.secondeimunity = 0;
                                User.InvencibleTime = DateTime.Now;
                            }
                            else
                                return 0;*/
                            Damage = (uint)(C.CurHP * Info.EffectValue);
                            Damage = (uint)((double)Damage * (100 - C.EqStats.TotalBless) / 100);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                            {

                                if (C.BuffOf(ExtraEffect.Fly).Eff == ExtraEffect.Fly)
                                    return 0;
                                Damage = MUser.PrepareAttack(MobAttackType.Melee);
                                //Damage = 2000;
                                //Console.WriteLine("damge= " + Damage);

                                if (Info.ID != 6000)
                                { Damage = (uint)(Damage * Info.EffectValue); }
                                else
                                { Damage = (uint)(Damage * 0.90); }
                                Damage += Info.Damage;


                            }
                            break;
                        }
                    case DamageType.Ranged:
                        {

                            if (C.BuffOf(ExtraEffect.Fly).Eff == ExtraEffect.Fly)
                                return 0;
                            Damage = MUser.PrepareAttack(MobAttackType.Ranged);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;



                            break;
                        }
                    case DamageType.Magic:
                        {

                            Damage = MUser.PrepareAttack(MobAttackType.Magic);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            //Damage = (uint)(Damage * 2);


                            break;
                        }
                    case DamageType.HealHP:
                        {

                            Damage = Info.Damage;
                            C.CurHP += (ushort)Info.Damage;
                            if (C.CurHP > C.MaxHP)
                                C.CurHP = C.MaxHP;
                            break;
                        }
                    case DamageType.HealMP:
                        {
                            Damage = Info.Damage;
                            C.CurMP += (ushort)Info.Damage;
                            if (C.CurMP > C.MaxMP)
                                C.CurMP = C.MaxMP;
                            break;
                        }
                }
                return Damage;
            }
            public uint M_GetDamage(Mob M)
            {

                uint Damage = 1;
                if (MUser.Loc.Map != M.Loc.Map || MUser.Loc.MapDimention != M.Loc.MapDimention)
                    return 0;
                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(M.CurrentHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Melee);
                            Damage = (uint)(Damage * Info.EffectValue);
                            if (Info.ID == 10310)
                                Damage = (uint)(Damage * 1.5);
                            Damage += Info.Damage;
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Ranged);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Magic);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;


                            break;
                        }
                    case DamageType.HealHP:
                        {
                            //Console.WriteLine("DamageType.HealHP:.........");
                            Damage = Info.Damage;
                            if (Info.ID == 1005)
                                Damage = (uint)(5000 * Info.Level);
                          
                            break;
                        }

                }
                // Damage = (uint)(Damage / M.DmgReduceTimes);
                return Damage;
            }
            public uint M_GetDamage(Companion M)
            {

                uint Damage = 1;
                if (MUser.Loc.Map != M.Loc.Map || MUser.Loc.MapDimention != M.Loc.MapDimention)
                    return 0;

                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(M.CurHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Melee);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Ranged);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = MUser.PrepareAttack(MobAttackType.Magic);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            break;
                        }
                    case DamageType.HealHP:
                        {
                            Damage = Info.Damage;
                            M.CurHP += Info.Damage;
                            if (M.CurHP > M.MaxHP)
                                M.CurHP = M.MaxHP;
                            World.Spawn(M, false);

                            break;
                        }
                }
                //Damage = (uint)(Damage / M.DmgReduceTimes);
                return Damage;
            }

            public uint GetDamage(Character C)
            {
                if (Info.ID == 5030 || Info.ID == 1290 || Info.ID ==
                              5040 || Info.ID == 7000 || Info.ID ==
                              7010 || Info.ID == 7030 || Info.ID == 6002 || Info.ID == 6004)
                { Info.ExtraEff = ExtraEffect.None; }
                if (!DMaps.NoPKMaps.Contains(User.Loc.Map) && C.PKAble(User.PKMode, User) && User.Loc.Map == C.Loc.Map && User.Loc.MapDimention == C.Loc.MapDimention)
                    if (Info.ID == 6004 || Info.ID == 6002)
                        if (C.Potency <= User.Potency || User.Name == "E.V.E" || (C.Potency - 3 <= User.Potency && MyMath.ChanceSuccess(25)) || (C.Potency - 2 <= User.Potency && MyMath.ChanceSuccess(40)))
                        {
                            uint Damage2 = 0;
                            Damage2 = (uint)(C.CurHP * 10 / 100);
                            if (Info.ID == 6004)
                            {
                                C.StatEff.Remove(StatusEffectEn.Fly);
                                C.StatEff.Add(StatusEffectEn.attack35);
                            }
                            if (Info.ID == 6002)
                            {
                                C.UnableToUseDrugs = DateTime.Now;
                                C.UnableToUseDrugsFor = 10;
                                //
                            }
                            return Damage2;
                        }
                if (User.Loc.Map != C.Loc.Map || User.Loc.MapDimention != C.Loc.MapDimention)
                    return 0;
                if (Info.ExtraEff != ExtraEffect.None)
                    return 0;
                uint Damage = 1;
                if (C.Protection)
                    return 0;
                if (Info.ExtraEff == ExtraEffect.NoPots || Info.ExtraEff == ExtraEffect.RemoveFly)
                    return 1;

                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            if (DMaps.NoPKMaps.Contains(User.Loc.Map))
                                if (Info.ExtraEff == ExtraEffect.None)
                                    return 0;
                            /*if (DateTime.Now > C.InvencibleTime.AddSeconds(C.secondeimunity))
                            {
                                C.secondeimunity = 0;
                                User.InvencibleTime = DateTime.Now;
                            }
                            else
                                return 0;*/
                            Damage = (uint)(C.CurHP * Info.EffectValue);
                            Damage = (uint)((double)Damage * (100 - C.EqStats.TotalBless) / 100);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            if ((C.StatEff.Contains(StatusEffectEn.Fly) && C.StatEff.Contains(StatusEffectEn.Ride) || !C.StatEff.Contains(StatusEffectEn.Fly)) && !C.StatEff.Contains(StatusEffectEn.Invisible))
                            {
                                if (DMaps.NoPKMaps.Contains(User.Loc.Map))
                                    if (Info.ExtraEff == ExtraEffect.None)
                                        return 0;
                                if (DMaps.NoPKMaps.Contains(User.Loc.Map) || !C.PKAble(User.PKMode, User) || User.Loc.Map != C.Loc.Map)
                                    return 0;
                                /*if (DateTime.Now > C.InvencibleTime.AddSeconds(C.secondeimunity))
                                {
                                    C.secondeimunity = 0;
                                    User.InvencibleTime = DateTime.Now;
                                }
                                else
                                    return 0;*/
                                if (C.BuffOf(ExtraEffect.Fly).Eff == ExtraEffect.Fly)
                                    return 0;
                                ushort Def = C.EqStats.TotalDefense;
                                Buff Shield = C.BuffOf(SkillsClass.ExtraEffect.MagicShield);
                                if (Shield.Eff == SkillsClass.ExtraEffect.MagicShield)
                                    Def = (ushort)(Def * Shield.Value);

                                Damage = User.PrepareAttack(2, false);
                                Damage = (uint)(Damage * MyMath.PotencyDifference(User.Potency, C.Potency));
                                Damage = (uint)(Damage * MyMath.PotencyDifference(User.KPotency, C.KPotency));
                                #region potencey effect
                                //if (User.KPotency - 2 == C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.05);
                                //}
                                //if (User.KPotency - 4 == C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.10);
                                //}
                                //if (User.KPotency - 5 == C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.13);
                                //}
                                //if (User.KPotency - 6 == C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.17);
                                //}
                                //if (User.KPotency - 7 >= C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.20);
                                //}
                                //if (C.KPotency - 2 == User.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 0.95);
                                //}
                                //if (C.KPotency - 4 == User.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 0.90);
                                //}
                                //if (C.KPotency - 5 == User.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 0.87);
                                //}
                                //if (C.KPotency - 6 == User.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 0.85);
                                //}
                                //if (C.KPotency - 7 >= User.KPotency)
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
                                //if (User.KPotency - 3 == C.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 1.10);
                                //}
                                //if (C.KPotency - 3 == User.KPotency)
                                //{
                                //    Damage = (uint)(Damage * 0.90);
                                //}
                                //// normal potencey effect:::::::::>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<</////\\\\\////\\\\////
                                //if (User.Potency - 2 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.13);
                                //}
                                //if (User.Potency - 3 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.17);
                                //}
                                //if (User.Potency - 4 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.21);
                                //}
                                //if (User.Potency - 5 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.25);
                                //}
                                //if (User.Potency - 6 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.28);
                                //}
                                //if (User.Potency - 7 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.32);
                                //}
                                //if (User.Potency - 8 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.35);
                                //}
                                //if (User.Potency - 9 >= C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.38);
                                //}
                                //if (C.Potency - 2 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.88);
                                //}
                                //if (C.Potency - 3 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.85);
                                //}
                                //if (C.Potency - 4 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.83);
                                //}
                                //if (C.Potency - 5 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.80);
                                //}
                                //if (C.Potency - 6 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.77);
                                //}
                                //if (C.Potency - 7 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.75);
                                //}
                                //if (C.Potency - 8 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.73);
                                //}
                                //if (C.Potency - 9 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.71);
                                //}
                                //if (C.Potency - 10 >= User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.69);
                                //}
                                //if (User.Potency - 1 == C.Potency)
                                //{
                                //    Damage = (uint)(Damage * 1.05);
                                //}
                                //if (C.Potency - 1 == User.Potency)
                                //{
                                //    Damage = (uint)(Damage * 0.95);
                                //}
                                #endregion
                                if (Info.ID != 6000)
                                { Damage = (uint)(Damage * Info.EffectValue); }
                                else
                                { Damage = (uint)(Damage * 0.90); }
                                Damage += Info.Damage;
                                if (Def >= Damage)
                                    Damage = 1;
                                else
                                    Damage -= Def;
                                

                                Damage = (uint)((double)Damage * (100 - C.EqStats.TotalBless) / 100);

                                Damage += User.EqStats.TotalDamageIncrease;
                                if (C.EqStats.TotalDamageDecrease >= Damage)
                                    Damage = 1;
                                else
                                    Damage -= C.EqStats.TotalDamageDecrease;
                            
                            }
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            if (DMaps.NoPKMaps.Contains(User.Loc.Map))
                                if (Info.ExtraEff == ExtraEffect.None)
                                    return 0;
                            /*if (DateTime.Now > C.InvencibleTime.AddSeconds(C.secondeimunity))
                            {
                                C.secondeimunity = 0;
                                User.InvencibleTime = DateTime.Now;
                            }
                            else
                                return 0;*/
                            if (C.BuffOf(ExtraEffect.Fly).Eff == ExtraEffect.Fly)
                                return 0;
                            Damage = User.PrepareAttack(25, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            if (Info.ID == 8000 && User.Loc.Map != 1090)
                            {
                                Damage = (uint)(Damage * 0.33);
                                
                                if (User.Potency - 2 >= C.Potency && User.Potency >= 310)
                                {
                                    Damage = (uint)(Damage * 1.14);
                                }
                                if (User.Potency - 4 >= C.Potency)
                                {
                                    Damage = (uint)(Damage * 1.13);
                                }
                                if (User.Potency - 6 >= C.Potency)
                                {
                                    Damage = (uint)(Damage * 1.12);
                                }
                                if (C.Potency - 2 >= User.Potency && C.Potency >= 310)
                                {
                                    Damage = (uint)(Damage * 0.74);
                                }
                                if (C.Potency - 4 >= User.Potency)
                                {
                                    Damage = (uint)(Damage * 0.72);
                                }
                                if (C.Potency - 6 >= User.Potency)
                                {
                                    Damage = (uint)(Damage * 0.80);
                                }
                                if (User.Potency - 1 >= C.Potency && User.Potency >= 305)
                                {
                                    Damage = (uint)(Damage * 1.10);
                                }
                                if (C.Potency - 1 >= User.Potency && C.Potency >= 305)
                                {
                                    Damage = (uint)(Damage * 0.90);
                                }
                            }
                            else
                            {
                                Damage = (uint)((double)Damage * 0.10);
                            }
                            Damage = (uint)((Damage * (((double)(110 - C.EqStats.TotalDodge)) / 100)) / 8);


                            Damage = (uint)((double)Damage * (100 - C.EqStats.TotalBless) / 100);

                            Damage += User.EqStats.TotalDamageIncrease;
                            if (C.EqStats.TotalDamageDecrease >= Damage)
                                Damage = 1;
                            else
                                Damage -= C.EqStats.TotalDamageDecrease;

                            break;
                        }
                    case DamageType.Magic:
                        {
                            if (DMaps.NoPKMaps.Contains(User.Loc.Map))
                                if (Info.ExtraEff == ExtraEffect.None)
                                    return 0;
                           /* if (DateTime.Now > C.InvencibleTime.AddSeconds(C.secondeimunity))
                            {
                                C.secondeimunity = 0;
                                User.InvencibleTime = DateTime.Now;
                            }
                            else
                                return 0;*/
                            Damage = User.PrepareAttack(21, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage = (uint)(Damage * 2);
                            #region potencey effect
                            if (User.KPotency - 2 == C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.10);
                            }
                            if (User.KPotency - 4 == C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.10);
                            }
                            if (User.KPotency - 5 == C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.23);
                            }
                            if (User.KPotency - 6 == C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.25);
                            }
                            if (User.KPotency - 7 >= C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.28);
                            }
                            if (C.KPotency - 2 == User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.91);
                            }
                            if (C.KPotency - 4 == User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.82);
                            }
                            if (C.KPotency - 5 == User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.80);
                            }
                            if (C.KPotency - 6 == User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.75);
                            }
                            if (C.KPotency - 7 >= User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.72);
                            }
                            
                            if (User.KPotency - 3 == C.KPotency)
                            {
                                Damage = (uint)(Damage * 1.13);
                            }
                            if (C.KPotency - 3 == User.KPotency)
                            {
                                Damage = (uint)(Damage * 0.88);
                            }
                            // normal potencey effect:::::::::>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<</////\\\\\////\\\\////
                            if (User.Potency - 2 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.15);
                            }
                            if (User.Potency - 3 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.17);
                            }
                            if (User.Potency - 4 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.20);
                            }
                            if (User.Potency - 5 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.23);
                            }
                            if (User.Potency - 6 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.26);
                            }
                            if (User.Potency - 7 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.29);
                            }
                            if (User.Potency - 8 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.33);
                            }
                            if (User.Potency - 9 >= C.Potency)
                            {
                                Damage = (uint)(Damage * 1.35);
                            }
                            if (C.Potency - 2 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.90);
                            }
                            if (C.Potency - 3 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.87);
                            }
                            if (C.Potency - 4 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.85);
                            }
                            if (C.Potency - 5 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.82);
                            }
                            if (C.Potency - 6 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.79);
                            }
                            if (C.Potency - 7 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.77);
                            }
                            if (C.Potency - 8 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.75);
                            }
                            if (C.Potency - 9 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.73);
                            }
                            if (C.Potency - 10 >= User.Potency)
                            {
                                Damage = (uint)(Damage * 0.71);
                            }
                            if (User.Potency - 1 == C.Potency)
                            {
                                Damage = (uint)(Damage * 1.07);
                            }
                            if (C.Potency - 1 == User.Potency)
                            {
                                Damage = (uint)(Damage * 0.93);
                            }
                            #endregion
                            Damage = (uint)((double)Damage * (((double)(100 - C.EqStats.TotalMDef1) / 100)));
                            if (C.EqStats.TotalMDef2 >= Damage)
                                Damage = 1;
                            else
                                Damage -= (uint)C.EqStats.TotalMDef2;

                            Damage = (uint)((double)Damage * (100 - C.EqStats.TotalBless) / 95);///here tao atak

                            Damage += User.EqStats.TotalMagicDamageIncrease;
                            if (C.EqStats.TotalDamageIncrease >= Damage)
                                Damage = 1;
                            else
                                Damage -= C.EqStats.TotalDamageIncrease;

                            break;
                        }
                    case DamageType.HealHP:
                        {
                            Damage = Info.Damage;
                            C.CurHP += (ushort)Info.Damage;
                            if (C.CurHP > C.MaxHP)
                                C.CurHP = C.MaxHP;
                            break;
                        }
                    case DamageType.HealMP:
                        {
                            Damage = Info.Damage;
                            C.CurMP += (ushort)Info.Damage;
                            if (C.CurMP > C.MaxMP)
                                C.CurMP = C.MaxMP;
                            break;
                        }
                }
                return Damage;
            }
            public uint GetDamage(NPC C)
            {
                //if (C.Loc.Map != 1039)
                 //   return 0;
                uint Damage = 1;
                
                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(C.CurHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = User.PrepareAttack(2, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = User.PrepareAttack(25, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = User.PrepareAttack(21, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage += User.EqStats.TotalMagicDamageIncrease;
                            break;
                        }
                    case DamageType.HealHP:
                        {
                            Damage = Info.Damage;
                            C.CurHP += (ushort)Info.Damage;
                            if (C.CurHP > C.MaxHP)
                                C.CurHP = C.MaxHP;
                            break;
                        }
                }
                if (C.Flags == 21) Damage = (uint)((double)Damage * 0.75);
                return Damage;
            }
            public uint GetDamage(Mob M)
            {

                uint Damage = 1;
                if (User.Loc.Map != M.Loc.Map || User.Loc.MapDimention != M.Loc.MapDimention)
                    return 0;
                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(M.CurrentHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = User.PrepareAttack(2, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            if (M.MeleeDefense >= Damage)
                                Damage = 1;
                            else
                                Damage -= M.MeleeDefense;
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));

                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = User.PrepareAttack(28, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage = (uint)((Damage * (((double)(200 - M.Dodge)) / 100)));
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));
                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = User.PrepareAttack(21, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            if (M.MagicDefense >= Damage)
                                Damage = 1;
                            else
                                Damage -= M.MagicDefense;
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));
                            Damage += User.EqStats.TotalMagicDamageIncrease;
                            break;
                        }
                    case DamageType.HealHP:
                        {
                            Damage = Info.Damage;
                            M.CurrentHP += Info.Damage;
                            if (M.CurrentHP > M.MaxHP)
                                M.CurrentHP = M.MaxHP;
                            World.Spawn(M, false);
                            //return 0 ;
                            break;
                        }
                }
                Damage = (uint)(Damage / M.DmgReduceTimes);
                return Damage;
            }
            public uint GetDamage(Companion M)
            {

                uint Damage = 1;
                if (User.Loc.Map != M.Loc.Map || User.Loc.MapDimention != M.Loc.MapDimention)
                    return 0;
                
                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(M.CurHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = User.PrepareAttack(2, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage = (uint)(Damage * M.Defense / 100);
                            if (M.Defense >= Damage)
                                Damage = 1;
                            else
                                Damage -= M.Defense;
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));

                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = User.PrepareAttack(28, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            Damage = (uint)((Damage * (((double)(200 - M.Dodge)) / 100)));
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));
                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = User.PrepareAttack(21, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;

                            if (M.MDef >= Damage)
                                Damage = 1;
                            else
                                Damage -= M.MDef;
                            Damage = (uint)(Damage * MyMath.LevelDifference(User.Level, M.Level));
                            Damage += User.EqStats.TotalMagicDamageIncrease;
                            break;
                        }
                    case DamageType.HealHP:
                        {
                            Damage = Info.Damage;
                            M.CurHP += Info.Damage;
                            if (M.CurHP > M.MaxHP)
                                M.CurHP = M.MaxHP;
                            World.Spawn(M, false);
                            //return 0 ;
                            break;
                        }
                }
                //Damage = (uint)(Damage / M.DmgReduceTimes);
                return Damage;
            }
            public uint GetDamage(uint CurHP)
            {
                uint Damage = 1;
                switch (Info.Damageing)
                {
                    case DamageType.Percent:
                        {
                            Damage = (uint)(CurHP * Info.EffectValue);
                            break;
                        }
                    case DamageType.Melee:
                        {
                            Damage = User.PrepareAttack(2, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            Damage += User.EqStats.TotalDamageIncrease;
                            break;
                        }
                    case DamageType.Ranged:
                        {
                            Damage = User.PrepareAttack(25, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            //Damage += User.EqStats.FanMagicMeleeDamageIncrease;
                            break;
                        }
                    case DamageType.Magic:
                        {
                            Damage = User.PrepareAttack(21, false);
                            Damage = (uint)(Damage * Info.EffectValue);
                            Damage += Info.Damage;
                            //Damage += User.EqStats.FanMagicDamageIncrease;
                            break;
                        }
                }
                return Damage;
            }
            public void Use()
            {
                try
                {


                    uint Exp = 0;
                    foreach (DictionaryEntry DE in CompanionTargets)
                    {
                        Companion M = (Companion)DE.Key;
                        if (User.Loc.Map != M.Loc.Map || User.Loc.MapDimention != M.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {

                                if (Info.Damageing == DamageType.Ranged)
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Melee, true);
                                else
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Magic, true);
                            }

                        }
                        else
                        {
                            Exp += Damage;
                           
                        }
                    }
                    foreach (DictionaryEntry DE in MiscTargets)
                    {
                        uint EntityID = (uint)DE.Key;
                        uint Damage = (uint)DE.Value;

                        if (EntityID == 6700)
                            GuildWars.ThePole.TakeAttack(User, Damage, 21);
                        if (EntityID == 6701)
                            GuildWars.TheLeftGate.TakeAttack(User, Damage, 21);
                        if (EntityID == 6702)
                            GuildWars.TheRightGate.TakeAttack(User, Damage, 21);
                    }
                    Hashtable TempHash = new Hashtable();
                    foreach (DictionaryEntry DE in MobTargets)
                    {
                        Mob M = (Mob)DE.Key;
                        if (User.Loc.Map != M.Loc.Map || User.Loc.MapDimention != M.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {

                                if (Info.Damageing == DamageType.Ranged)
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Melee, true);
                                else
                                    Exp += M.TakeAttack(User, ref Damage, MobAttackType.Magic, true);
                            }
                            switch (Info.ExtraEff)
                            {
                                case ExtraEffect.FlashStep:
                                    {
                                        M.Shift(AimX, AimY);
                                        break;
                                    }
                                case ExtraEffect.Stigma:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Stigma;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.MagicShield:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Shield;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Invisibility:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Invisible;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Accuracy:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Accuracy;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Cyclone:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Cyclone;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Superman:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.SuperMan;


                                        M.AddBuff(B);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            Exp += Damage;
                            //M.CurrentHP += Damage;
                            //if (M.CurrentHP > M.MaxHP) M.CurrentHP = M.MaxHP;
                        }
                        TempHash.Add(M, Damage);
                    }
                    MobTargets = TempHash;
                    foreach (DictionaryEntry DE in NPCTargets)
                    {
                        NPC N = (NPC)DE.Key;
                        uint Damage = (uint)DE.Value;
                        if (Info.Damageing != DamageType.HealHP)
                        {
                            if (Info.Damageing == DamageType.Ranged)
                                Exp += N.TakeAttack(User, Damage, MobAttackType.Ranged, true);
                            else if (Info.Damageing == DamageType.Melee)
                                Exp += N.TakeAttack(User, Damage, MobAttackType.Melee, true);
                            else
                                Exp += N.TakeAttack(User, Damage, MobAttackType.Magic, true);
                        }
                        else
                        {
                            N.CurHP += Damage;
                            if (N.CurHP > N.MaxHP) N.CurHP = N.MaxHP;
                            Exp += Damage / 10;
                        }
                    }
                    foreach (DictionaryEntry DE in PlayerTargets)
                    {
                        Character C = (Character)DE.Key;
                        if (User.Loc.Map != C.Loc.Map || User.Loc.MapDimention != C.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;

                        if ((User.Loc.Map == 1707 || User.Loc.Map == 1068) && Info.ID == 1045 || User.Loc.Map == 1707 && Info.ID == 1046)
                        {
                            if (User.Luchando == true)
                            {

                                Game.Character J = Game.World.CharacterFromName(User.Enemigo);

                                if (C.FreeBattel == false && J.FreeBattel == false)
                                {
                                    User.PkPuntos++;
                                    User.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, User.Name + ": " + User.PkPuntos + " points. " + J.Name + ": " + J.PkPuntos + " points");
                                    J.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, User.Name + ": " + User.PkPuntos + " points. " + J.Name + ": " + J.PkPuntos + " points");
                                    {
                                        if (User.PkPuntos == User.VSpets)
                                        {
                                            Game.World.WorldMessage(User.Name, User.Name + " (" + User.PkPuntos + ")" + " has won " + User.Enemigo + " (" + J.PkPuntos + ")" + " in a battle of 1 v 1! The winner has won: " + User.Apuesta + " CPs", 2011, 0, System.Drawing.Color.Red);
                                            J.Teleport(1002, 406, 400);
                                            User.Teleport(1002, 399, 400);
                                            User.Protection = false;
                                            J.Protection = false;
                                            User.Luchando = false;
                                            J.Luchando = false;
                                            User.PkPuntos = 0;
                                            J.PkPuntos = 0;
                                            User.Enemigo = "";
                                            J.Enemigo = "";
                                            User.CPs += (uint)User.Apuesta;
                                            User.CPs += (uint)User.Apuesta;
                                            World.pvpclear(User);
                                            World.pvpclear(J);

                                        }
                                    }
                                }
                            }
                        }
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {
                                if (Info.Damageing == DamageType.Ranged)
                                    C.TakeAttack(User, Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    C.TakeAttack(User, Damage, MobAttackType.Melee, true);
                                else
                                    C.TakeAttack(User, Damage, MobAttackType.Magic, true);
                            }
                        }
                        switch (Info.ExtraEff)
                        {
                            case ExtraEffect.BlessPray:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Pray;
                                    C.AddBuff(B);
                                    C.Prayer = true;
                                    C.PrayDT = DateTime.Now;
                                    C.GettingLuckyTime = true;
                                    break;
                                }
                            case ExtraEffect.UnMount:
                                {
                                    //Console.WriteLine(C.Name);
                                    if (C.Equips.Steed.Plus < User.Equips.Steed.Plus)
                                    {
                                        C.StatEff.Remove(StatusEffectEn.Ride);
                                        C.StatEff.Remove(StatusEffectEn.DragonCyclone);
                                        C.StatEff.Remove(StatusEffectEn.Fly);
                                    }
                                    break;
                                }
                            case ExtraEffect.Scapegoat:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Normal;
                                    
                                    C.AddBuff(B);
                                    C.MyClient.LocalMessage(2011, System.Drawing.Color.Brown,"CounterKill switch is on!");
                                    break;
                                }
                            case ExtraEffect.NoPots:
                                {
                                    C.UnableToUseDrugsFor = Info.EffectLasts;
                                    C.UnableToUseDrugs = DateTime.Now;
                                    break;
                                }
                            case ExtraEffect.Ride:
                                {
                                    if (User.MyClient.AuthInfo.Status == "[PM]" || User.MyClient.AuthInfo.Status == "[GM]" || (User.Nobility.Rank == Game.Ranks.King) || (User.Nobility.Rank == Game.Ranks.Prince) || (User.Nobility.Rank == Game.Ranks.Duke))
                                    {
                                        if (!User.StatEff.Contains(StatusEffectEn.Ride))
                                        {
                                            User.StatEff.Add(StatusEffectEn.Ride);
                                            User.StatEff.Add(StatusEffectEn.DragonCyclone);
                                            //User.StatEff.Add(StatusEffectEn.Fly);
                                        }
                                        else
                                        {
                                            User.StatEff.Remove(StatusEffectEn.Ride);
                                            User.StatEff.Remove(StatusEffectEn.DragonCyclone);
                                            User.StatEff.Remove(StatusEffectEn.Fly);
                                        }
                                        User.Vigor = User.MaxVigor;
                                    }
                                    break;
                                }
                            case ExtraEffect.Summon:
                                {
                                    if (User.guardsArmy)
                                    {
                                        if (User.MyCompanion != null)
                                        {
                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion1.Dissappear();
                                            User.MyCompanion2.Dissappear();
                                            User.MyCompanion3.Dissappear();
                                            User.MyCompanion4.Dissappear();
                                            User.MyCompanion5.Dissappear();
                                            User.MyCompanion6.Dissappear();
                                            User.MyCompanion7.Dissappear();
                                            User.MyCompanion8.Dissappear();
                                            User.MyCompanion9.Dissappear();
                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion4 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion5 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion6 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion7 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion8 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion9 = new Game.Companion(User, Info.Damage);

                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion4 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion5 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion6 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion7 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion8 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion9 = new Game.Companion(User, Info.Damage);
                                        }
                                    }
                                    if (User.MyGuild == Extra.GuildWars.LastWinner && (User.GuildRank == Features.GuildRank.GuildLeader) && User.Nobility.Rank == Game.Ranks.King)
                                    {
                                        if (User.MyCompanion != null)
                                        {

                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion1.Dissappear();
                                            User.MyCompanion2.Dissappear();
                                            User.MyCompanion3.Dissappear();
                                            User.MyCompanion4.Dissappear();

                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion4 = new Game.Companion(User, Info.Damage);
                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion4 = new Game.Companion(User, Info.Damage);
                                        }
                                        break;
                                    }
                                    if (User.Nobility.Rank == Game.Ranks.King || User.MyGuild == Extra.GuildWars.LastWinner && (User.GuildRank == Features.GuildRank.GuildLeader))
                                    {
                                        if (User.MyCompanion != null)
                                        {

                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion1.Dissappear();
                                            User.MyCompanion2.Dissappear();
                                            User.MyCompanion3.Dissappear();
                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion3 = new Game.Companion(User, Info.Damage);
                                        }
                                        break;
                                    }
                                    if (User.MyGuild == Extra.GuildWars.LastWinner && (User.GuildRank == Features.GuildRank.DeputyMgr))
                                    {
                                        if (User.MyCompanion != null)
                                        {

                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion1.Dissappear();
                                            User.MyCompanion2.Dissappear();
                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);

                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);

                                        }
                                        break;
                                    }
                                    if (User.Nobility.Rank == Game.Ranks.Prince)
                                    {
                                        if (User.MyCompanion != null)
                                        {

                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion1.Dissappear();
                                            User.MyCompanion2.Dissappear();

                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);

                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion1 = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);

                                        }
                                        break;
                                    }

                                    if (User.Reborns == 2)
                                        if (User.MyCompanion != null)
                                        {
                                            //if(User.MyCompanion != null)
                                            User.MyCompanion.Dissappear();
                                            User.MyCompanion2.Dissappear();
                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            //User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                            User.MyCompanion2 = new Game.Companion(User, Info.Damage);
                                        }
                                    if (User.Reborns == 1)
                                    {

                                        if (User.MyCompanion != null)
                                        {
                                            //if(User.MyCompanion != null)
                                            User.MyCompanion.Dissappear();
                                            //User.MyCompanion = new Game.Companion(User, Info.Damage);
                                        }
                                        else
                                        {
                                            User.MyCompanion = new Game.Companion(User, Info.Damage);
                                        }

                                    }
                                    break;
                                }
                            case ExtraEffect.RemoveFly:
                                {
                                    Buff B = C.BuffOf(ExtraEffect.Fly);
                                    if (B.Eff == ExtraEffect.Fly && C.Potency < User.Potency)
                                        C.RemoveBuff(B);
                                    break;
                                }
                            case ExtraEffect.Transform:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Transform = Info.Damage;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Normal;
                                    C.AddBuff(B);
                                    break;
                                }
                            case ExtraEffect.Fly:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Fly;
                                    C.AddBuff(B);
                                    break;
                                }
                            case ExtraEffect.Revive:
                                {
                                    C.Ghost = false;
                                    C.BlueName = false;
                                    C.CurHP = (ushort)C.MaxHP;
                                    C.Alive = true;
                                    C.StatEff.Clear();
                                    C.PKPoints = C.PKPoints;
                                    C.Body = C.Body;
                                    C.Hair = C.Hair;
                                    C.Equips.Send(C.MyClient, false);
                                    World.Spawn(C, false);
                                    break;
                                }
                            case ExtraEffect.FatalStrike:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.FatalStrike;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.ShurikenVortex:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.ShurikenVortex;
                                    C.AddBuff(B);
                                    C.VortexOn = true;
                                    //C.LastVortexAttk = DateTime.Now;
                                    break;
                                }
                            case ExtraEffect.Stigma:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Stigma;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.MagicShield:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Shield;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Invisibility:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Invisible;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Accuracy:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Accuracy;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Cyclone:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Cyclone;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Superman:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.SuperMan;


                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.FlashStep:
                                {
                                    C.Shift(AimX, AimY);
                                    break;
                                }
                        }
                    }

                    if (User.Loc.Map != 1039)
                    {
                        User.IncreaseExp(Exp, false);
                        User.AddSkillExp(Info.ID, Exp);
                        User.AddSkillExp(Info.ID, 1000);
                    }
                    else
                    {
                        User.IncreaseExp(Exp / 10, false);
                        User.AddSkillExp(Info.ID, Exp / 10);
                        User.AddSkillExp(Info.ID, 100);
                    }
                    Game.World.Action(User, Packets.SkillUse(this));
                }

                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            public void MUse()
            {
                try
                {

                    
                    Hashtable TempHash = new Hashtable();
                    foreach (DictionaryEntry DE in MobTargets)
                    {
                        Mob M = (Mob)DE.Key;
                        if (MUser.Loc.Map != M.Loc.Map || MUser.Loc.MapDimention != M.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {

                                if (Info.Damageing == DamageType.Ranged)
                                   Damage = M.TakeAttack(MUser, Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    Damage = M.TakeAttack(MUser, Damage, MobAttackType.Melee, true);
                                else
                                    Damage = M.TakeAttack(MUser, Damage, MobAttackType.Magic, true);
                               
                       
                            }
                            switch (Info.ExtraEff)
                            {
                                case ExtraEffect.FlashStep:
                                    {
                                        MUser.Shift(AimX, AimY);
                                        break;
                                    }
                                case ExtraEffect.Stigma:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Stigma;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.MagicShield:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Shield;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Invisibility:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Invisible;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Accuracy:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Accuracy;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Cyclone:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.Cyclone;
                                        M.AddBuff(B);

                                        break;
                                    }
                                case ExtraEffect.Superman:
                                    {
                                        Buff B = new Buff();
                                        B.Eff = Info.ExtraEff;
                                        B.Lasts = Info.EffectLasts;
                                        B.Value = Info.EffectValue;
                                        B.Started = DateTime.Now;
                                        B.StEff = StatusEffectEn.SuperMan;


                                        M.AddBuff(B);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                           
                            M.CurrentHP += Damage;
                            if (M.CurrentHP > M.MaxHP) M.CurrentHP = M.MaxHP;
                            //World.Spawn(M, false);

                        }
                        TempHash.Add(M, Damage);
                    }
                    MobTargets = TempHash;
                    TempHash = new Hashtable();

                    foreach (DictionaryEntry DE in CompanionTargets)
                    {
                        Companion M = (Companion)DE.Key;
                        if (MUser.Loc.Map != M.Loc.Map || MUser.Loc.MapDimention != M.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {

                                if (Info.Damageing == DamageType.Ranged)
                                   Damage = M.TakeAttack(MUser, Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    Damage = M.TakeAttack(MUser, Damage, MobAttackType.Melee, true);
                                else
                                    Damage = M.TakeAttack(MUser, Damage, MobAttackType.Magic, true);
                               
                            }
                            switch (Info.ExtraEff)
                            {

                                case ExtraEffect.Poison:
                                    {
                                        if (MyMath.ChanceSuccess(50))
                                        {
                                            M.StatEff.Add(StatusEffectEn.Poisoned);
                                            M.PoisonedInfo = new Server.Game.PoisonType(Info.Level);
                                            World.Action(MUser, Packets.Status(M.EntityID, Server.Game.Status.Effect, (ulong)Game.StatusEffectEn.Poisoned));
                                            M.TakeAttack(MUser, Damage, Server.Game.MobAttackType.Magic, true);
                                        }
                                        else
                                            Damage = 0;
                                        break;
                                    }
                            }
                            
                        }
                        else
                        {
                           
                            //M.CurrentHP += Damage;
                            //if (M.CurrentHP > M.MaxHP) M.CurrentHP = M.MaxHP;
                        }
                        TempHash.Add(M, Damage);
                    }
                    CompanionTargets = TempHash;
                    TempHash = new Hashtable();
                    
                    foreach (DictionaryEntry DE in PlayerTargets)
                    {
                        Character C = (Character)DE.Key;
                        if (MUser.Loc.Map != C.Loc.Map || MUser.Loc.MapDimention != C.Loc.MapDimention)
                            continue;
                        uint Damage = (uint)DE.Value;

                       
                        if (Info.Damageing != DamageType.HealHP && Info.Damageing != DamageType.HealMP)
                        {
                            if (Info.ExtraEff == ExtraEffect.None || Info.ExtraEff == ExtraEffect.RemoveFly)
                            {
                                if (Info.Damageing == DamageType.Ranged)
                                   Damage = C.TakeAttack(MUser, Damage, MobAttackType.Ranged, true);
                                else if (Info.Damageing == DamageType.Melee)
                                    Damage = C.TakeAttack(MUser, Damage, MobAttackType.Melee, true);
                                else
                                    Damage = C.TakeAttack(MUser, Damage, MobAttackType.Magic, true);
                            }
                          
                           
                        }
                        Console.WriteLine("ExtraEff ..");

                        switch (Info.ExtraEff)
                        {
                            case ExtraEffect.Poison:
                                {
                                    if (MyMath.ChanceSuccess(70))
                                    {
                                        Console.WriteLine("poison ..");
                                        Damage = 1;
                                        C.StatEff.Add(StatusEffectEn.Poisoned);
                                        C.PoisonedInfo = new Server.Game.PoisonType(Info.Level);
                                        World.Action(MUser, Packets.Status(C.EntityID, Server.Game.Status.Effect, (ulong)Game.StatusEffectEn.Poisoned));
                                        C.TakeAttack(MUser, Damage, Server.Game.MobAttackType.Magic, true);
                                        if (C.PoisonedInfo != null)
                                            Console.WriteLine("poison success");
                                        else
                                            Console.WriteLine("poison failer");

                                    }
                                    else
                                        Console.WriteLine("poison missed");

                                    break;
                                }
                            case ExtraEffect.BlessPray:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Pray;
                                    C.AddBuff(B);
                                    C.Prayer = true;
                                    C.PrayDT = DateTime.Now;
                                    C.GettingLuckyTime = true;
                                    break;
                                }
                            case ExtraEffect.UnMount:
                                {
                                    //Console.WriteLine(C.Name);
                                    //if (C.Equips.Steed.Plus < User.Equips.Steed.Plus)
                                    {
                                        C.StatEff.Remove(StatusEffectEn.Ride);
                                        C.StatEff.Remove(StatusEffectEn.DragonCyclone);
                                        C.StatEff.Remove(StatusEffectEn.Fly);
                                    }
                                    break;
                                }
                            case ExtraEffect.Scapegoat:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Normal;

                                    C.AddBuff(B);
                                    C.MyClient.LocalMessage(2011, System.Drawing.Color.Brown, "CounterKill switch is on!");
                                    break;
                                }
                            case ExtraEffect.NoPots:
                                {
                                    C.UnableToUseDrugsFor = Info.EffectLasts;
                                    C.UnableToUseDrugs = DateTime.Now;
                                    break;
                                }
                            case ExtraEffect.RemoveFly:
                                {
                                    Buff B = C.BuffOf(ExtraEffect.Fly);
                                    //if (B.Eff == ExtraEffect.Fly && C.Potency < User.Potency)
                                        C.RemoveBuff(B);
                                    break;
                                }
                            case ExtraEffect.Transform:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Transform = Info.Damage;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Normal;
                                    C.AddBuff(B);
                                    break;
                                }
                            case ExtraEffect.Fly:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Fly;
                                    C.AddBuff(B);
                                    break;
                                }
                            case ExtraEffect.Revive:
                                {
                                    C.Ghost = false;
                                    C.BlueName = false;
                                    C.CurHP = (ushort)C.MaxHP;
                                    C.Alive = true;
                                    C.StatEff.Clear();
                                    C.PKPoints = C.PKPoints;
                                    C.Body = C.Body;
                                    C.Hair = C.Hair;
                                    C.Equips.Send(C.MyClient, false);
                                    World.Spawn(C, false);
                                    break;
                                }
                            case ExtraEffect.FatalStrike:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.FatalStrike;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.ShurikenVortex:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.ShurikenVortex;
                                    C.AddBuff(B);
                                    C.VortexOn = true;
                                    //C.LastVortexAttk = DateTime.Now;
                                    break;
                                }
                            case ExtraEffect.Stigma:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Stigma;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.MagicShield:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Shield;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Invisibility:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Invisible;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Accuracy:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Accuracy;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Cyclone:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.Cyclone;
                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.Superman:
                                {
                                    Buff B = new Buff();
                                    B.Eff = Info.ExtraEff;
                                    B.Lasts = Info.EffectLasts;
                                    B.Value = Info.EffectValue;
                                    B.Started = DateTime.Now;
                                    B.StEff = StatusEffectEn.SuperMan;


                                    C.AddBuff(B);

                                    break;
                                }
                            case ExtraEffect.FlashStep:
                                {
                                    C.Shift(AimX, AimY);
                                    break;
                                }
                        }
                        TempHash.Add(C, Damage);
                    }
                    PlayerTargets = TempHash;
                    TempHash = null;
                    Game.World.Action(MUser, Packets.SkillUse(this));
                }

                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
        }

        public static void Load()
        {
            if (File.Exists(Program.ConquerPath + @"Skills.dat"))
            {
                FileStream FS = new FileStream(Program.ConquerPath + @"Skills.dat", FileMode.Open);
                BinaryReader BR = new BinaryReader(FS);
                int SkillCount = BR.ReadInt32();
                for (int i = 0; i < SkillCount; i++)
                {
                    SkillInfo S = new SkillInfo();
                    S.LoadThis(BR);
                    SkillInfos.Add(S.ID + " " + S.Level, S);
                }
                BR.Close();
                FS.Close();
            }
            WepSkillIDs.Add((ushort)480, (ushort)7020);
            WepSkillIDs.Add((ushort)420, (ushort)5030);
            WepSkillIDs.Add((ushort)421, (ushort)5030);
            WepSkillIDs.Add((ushort)510, (ushort)1250);
            WepSkillIDs.Add((ushort)530, (ushort)5050);
            WepSkillIDs.Add((ushort)561, (ushort)5010);
            WepSkillIDs.Add((ushort)560, (ushort)1260);
            WepSkillIDs.Add((ushort)721, (ushort)1290);
            WepSkillIDs.Add((ushort)460, (ushort)5040);
            WepSkillIDs.Add((ushort)540, (ushort)1300);
            WepSkillIDs.Add((ushort)430, (ushort)7000);
            WepSkillIDs.Add((ushort)450, (ushort)7010);
            WepSkillIDs.Add((ushort)481, (ushort)7030);
            WepSkillIDs.Add((ushort)440, (ushort)7040);
            WepSkillIDs.Add((ushort)580, (ushort)5020);

            Console.WriteLine("Skill Loaded");
        }
        public static void Save()
        {
            FileStream FS = new FileStream(Program.ConquerPath + @"Skills.dat", FileMode.OpenOrCreate);
            BinaryWriter BW = new BinaryWriter(FS);
            BW.Write(SkillInfos.Count);
            foreach (SkillInfo S in SkillInfos.Values)
                S.SaveThis(BW);
            BW.Close();
            FS.Close();
        }
    }
}
