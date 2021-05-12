using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using Server.Features;
using Server.Skills;
using Server.Game;

namespace Server.Game
{
    public enum MobAttackType : byte
    {
        Melee = 2,
        Ranged = 28,
        Magic = 21,
        Kill = 14,
        FatalStrike = 45,
        Scapegoat = 43,
        HealHP = 5
    }
    public enum MobType : byte
    {
        HuntPlayers = 1,
        HuntMobs = 2,
        HuntMobsAndPlayers = 3,
        HuntBlueNames = 4,
        HuntMobsAndBlue = 6
    }

    public class Mob
    {
        public int MobID;
        public string Name;
        public ushort Mesh;
        public MobType Type;
        public byte Level;
        public uint CurrentHP;
        public uint MaxHP;

        public ushort MinMeleeAttack;
        public ushort MaxMeleeAttack;
        public ushort MeleeDefense;
        public ushort MagicAttack;
        public ushort MagicDefense;
        public byte Dodge;

        public MobAttackType AttackType;
        public byte AttackDist;
        public byte DmgReduceTimes;
        bool LevDifDmg = true;
        public ushort attackSpeed = 1000;

        uint ReSpawnSpeed = 0;

        public bool Drops;
        public int MinSilvers;
        public int MaxSilvers;
        ushort MoveSpeed = 0;

        public PoisonType PoisonedInfo = null;
        public StatusEffect StatusEff;

        public ushort Skill = 0;
        public byte SkillLvl = 0;
        public ushort SkillSpeed;
        public DateTime SkillLastttack;
        public ushort S_Skill = 0;
        public byte S_SkillLvl = 0;
        public ushort S_SkillSpeed;
        public DateTime S_SkillLastttack;

        byte MultyAttackLevel = 0;
        public Buff Buffer;
        public ArrayList Buffers;
        DateTime Died;
        DateTime LastMove;
        DateTime Lastwalk = DateTime.Now;
        DateTime LastAttack = DateTime.Now;
        DateTime LastBuffRemove = DateTime.Now;
        public static DateTime LastTarget = DateTime.Now;
        public byte Direction = 0;
        public byte Action = 0;
        public uint EntityID;
        public Location Loc;
        public Location StartLoc;
        public bool Alive = true;
        public bool Dissappeared = false;

        public uint RandomTime = 30;
        public static Random Rnd = new Random();
        Companion CompanionTarget;
        Character PlayerTarget;
        Mob MobTarget;

        public Mob(string Line)
        {
            LastMove = DateTime.Now;
            string[] Info = Line.Split(' ');
            MobID = int.Parse(Info[0]);
            Name = Info[1];
            Mesh = ushort.Parse(Info[2]);
            Type = (MobType)byte.Parse(Info[3]);
            Level = byte.Parse(Info[4]);
            CurrentHP = uint.Parse(Info[5]);
            MaxHP = uint.Parse(Info[6]);

            MinMeleeAttack = ushort.Parse(Info[7]);
            MaxMeleeAttack = ushort.Parse(Info[8]);
            MeleeDefense = ushort.Parse(Info[9]);
            MagicAttack = ushort.Parse(Info[10]);
            MagicDefense = ushort.Parse(Info[11]);
            Dodge = byte.Parse(Info[12]);

            AttackType = (MobAttackType)byte.Parse(Info[13]);
            Skill = ushort.Parse(Info[14]);
            SkillLvl = byte.Parse(Info[15]);
            SkillSpeed = ushort.Parse(Info[16]);
            AttackDist = byte.Parse(Info[17]);
            DmgReduceTimes = byte.Parse(Info[18]);
            LevDifDmg = bool.Parse(Info[19]);

            ReSpawnSpeed = uint.Parse(Info[20]);

            Drops = bool.Parse(Info[21]);
            MinSilvers = int.Parse(Info[22]);
            MaxSilvers = int.Parse(Info[23]);
        }

        public void Mob2(string Line)
        {
            StatusEff = new StatusEffect(this);
            Buffers = new ArrayList();
            LastMove = DateTime.Now;
            string[] Info = Line.Split(' ');
            MobID = int.Parse(Info[0]);
            Name = Info[1];
            Type = (MobType)byte.Parse(Info[2]);
            Mesh = ushort.Parse(Info[3]);
            Level = byte.Parse(Info[4]);
            MaxHP = uint.Parse(Info[5]);
            MeleeDefense = ushort.Parse(Info[6]);
            MagicDefense = ushort.Parse(Info[7]);
            MagicAttack = ushort.Parse(Info[8]);
            MinMeleeAttack = ushort.Parse(Info[9]);
            MaxMeleeAttack = ushort.Parse(Info[10]);
            DmgReduceTimes = byte.Parse(Info[11]);
            Dodge = byte.Parse(Info[12]);
            AttackType = (MobAttackType)byte.Parse(Info[13]);
            if (AttackType == MobAttackType.Magic)
            {//8036
                Skill = ushort.Parse(Info[14]);
                SkillLvl = byte.Parse(Info[15]);
                Drops = bool.Parse(Info[16]);
                AttackDist = byte.Parse(Info[17]);
                MinSilvers = int.Parse(Info[18]);
                MaxSilvers = int.Parse(Info[19]);
                MoveSpeed = ushort.Parse(Info[20]);//97 Guard1 6 900 250 50000 2500 70 60000 0 0 100 70 14 1002 3 False 18 100000 500000 500 3 False
                ReSpawnSpeed = uint.Parse(Info[21]);
                LevDifDmg = bool.Parse(Info[22]);
            }
            else
            {
                Drops = bool.Parse(Info[14]);
                AttackDist = byte.Parse(Info[15]);
                MinSilvers = int.Parse(Info[16]);
                MaxSilvers = int.Parse(Info[17]);
                MoveSpeed = ushort.Parse(Info[18]);
                ReSpawnSpeed = uint.Parse(Info[19]);
               LevDifDmg = bool.Parse(Info[20]);
            }
            if (AttackType == MobAttackType.Melee)
            {
                if (Info.Length == 24)
                {
                    Skill = ushort.Parse(Info[21]);
                    SkillLvl = byte.Parse(Info[22]);
                    SkillSpeed = ushort.Parse(Info[23]);
                    //Console.WriteLine("attakSpeed " + attackSpeed);
                }
            }
            if (AttackType == MobAttackType.Magic)
            {
                if (Info.Length == 26)
                {
                    Skill = ushort.Parse(Info[23]);
                    SkillLvl = byte.Parse(Info[24]);
                    SkillSpeed = ushort.Parse(Info[25]);

                }
            }
            CurrentHP = MaxHP;
        }
        public Mob(Mob M)
        {
            LastMove = DateTime.Now;
            MobID = M.MobID;
            Mesh = M.Mesh;
            Level = M.Level;
            MaxHP = M.MaxHP;
            CurrentHP = M.CurrentHP;
            MeleeDefense = M.MeleeDefense;
            MagicDefense = M.MagicDefense;
            MagicAttack = M.MagicAttack;
            MinMeleeAttack = M.MinMeleeAttack;
            MaxMeleeAttack = M.MaxMeleeAttack;
            Name = M.Name;
            Type = M.Type;
            DmgReduceTimes = M.DmgReduceTimes;
            Dodge = M.Dodge;
            AttackType = M.AttackType;
            Drops = M.Drops;
            AttackDist = M.AttackDist;
            Skill = M.Skill;
            SkillLvl = M.SkillLvl;
            MinSilvers = M.MinSilvers;
            MaxSilvers = M.MaxSilvers;
            MoveSpeed = M.MoveSpeed;
            ReSpawnSpeed = M.ReSpawnSpeed;
            LevDifDmg = M.LevDifDmg;
            attackSpeed = M.attackSpeed;
            Skill = M.Skill;
            SkillLvl = M.SkillLvl;
            SkillSpeed = M.SkillSpeed;
            StatusEff = new StatusEffect(this);
            Buffers = new ArrayList();
            if (Name.Contains("Guard"))
                StatusEff.Add(StatusEffectEn.Shield);
        }

        public void Respawn()
        {
            try
            {
                Loc = StartLoc;
                Alive = true;
                CurrentHP = MaxHP;
                Action = 100;
                World.Spawn(this, false);
                World.Action(this, Packets.String(EntityID, 10, "MBStandard"));
                Dissappeared = false;
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }

        public void AddBuff(Buff B)
        {

            Buff ExBuff = BuffOf(B.Eff);
            if (ExBuff.Eff == B.Eff)
                Buffers.Remove(ExBuff);

            Buffers.Add(B);
            StatusEff.Add(B.StEff);
        }
        public void RemoveBuff(Buff B)
        {
            Buffers.Remove(B);
            StatusEff.Remove(B.StEff);

        }
        public Buff BuffOf(SkillsClass.ExtraEffect E)
        {
            if(Buffers != null)
            if (Buffers.Count > 0)
            {
                Buff[] Bufffs = new Buff[Buffers.Count];
                if (Buffers.Count > 0)
                {
                    Buffers.CopyTo(Bufffs, 0);
                }
                foreach (Buff B in Bufffs)
                    if (B.Eff == E)
                        return B;
                Bufffs = null;

            }
            return new Buff();
        }

        public uint PrepareAttack()
        {
            uint dmg = (uint)Rnd.Next(MinMeleeAttack, MaxMeleeAttack);
            if (AttackType == MobAttackType.Melee || AttackType == MobAttackType.Ranged)
            {
                Buff Stig = BuffOf(SkillsClass.ExtraEffect.Stigma);
                if (Stig.Eff == SkillsClass.ExtraEffect.Stigma)
                    dmg = (uint)(dmg * 1.20);
                return dmg;
            }
            else
                return MagicAttack;
        }
        public uint PrepareAttack(MobAttackType Atktype)
        {
            uint dmg = 0;
            if (MobID == 97)
                dmg = (uint)Rnd.Next(30000, 35000);
            else
                dmg = (uint)Rnd.Next(MinMeleeAttack, MaxMeleeAttack);

            if (Atktype == MobAttackType.Melee || Atktype == MobAttackType.Ranged)
            {
                Buff Stig = BuffOf(SkillsClass.ExtraEffect.Stigma);
                if (Stig.Eff == SkillsClass.ExtraEffect.Stigma)
                    dmg = (uint)(dmg * Stig.Value);
                return dmg;
            }
            else
                return MagicAttack;
        }
        public bool NeedsPKMode
        {
            get
            {
                if (Type == MobType.HuntBlueNames || Type == MobType.HuntMobsAndBlue)
                    return true;
                return false;
            }
        }

        public uint TakeAttack(Character Attacker,ref uint Damage, MobAttackType AT, bool IsSkill)
        {
            if (AT != MobAttackType.Magic && Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.Superman).Eff == Skills.SkillsClass.ExtraEffect.Superman)
                Damage *= 4;
            if (!IsSkill && Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.FatalStrike).Eff == Skills.SkillsClass.ExtraEffect.FatalStrike)
            {
                AT = MobAttackType.FatalStrike;
                Damage *= 3;
            }
            double e = 1;
            if (Level + 4 < Attacker.Level)
                e = 0.1;
            if (Level + 4 >= Attacker.Level)
                e = 1;
            if (Level >= Attacker.Level)
                e = 1.1;
            if (Level - 4 > Attacker.Level)
                e = 1.3;

            if (Type == MobType.HuntBlueNames || Type == MobType.HuntMobsAndBlue)
            {
                Attacker.BlueName = true;
                if (Attacker.BlueNameLasts < 60)
                    Attacker.BlueNameLasts = 15;
            }
            if (AT != MobAttackType.Magic && !IsSkill)
            {
                short _Agi = (short)(Attacker.Agi + Attacker.EqStats.TotalAgility);

                Buff Accuracy = Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.Accuracy);
                if (Accuracy.Eff == Skills.SkillsClass.ExtraEffect.Accuracy)
                    _Agi = (short)(_Agi * Accuracy.Value);
                Buff SM = Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.Superman);
                if (SM.StEff == StatusEffectEn.SuperMan)
                    _Agi *= 2;
                double MissValue = Rnd.Next(15 + _Agi, _Agi + Dodge + 15);
                if (MissValue <= Dodge && AT != MobAttackType.FatalStrike)
                    Damage = 0;
                else
                {
                    if (LevDifDmg) Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));                    
                }
            }
            if (!IsSkill && Damage != 0)
            {
                if (AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                {
                    if (MeleeDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MeleeDefense;
                    Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));
                    Damage += Attacker.EqStats.TotalDamageIncrease;
                    PlayerTarget = Attacker;
                }
                else if (AT == MobAttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * ((double)(200 - Dodge) / 100));
                    Damage += Attacker.EqStats.TotalDamageIncrease;
                }
                else
                {
                    if (MagicDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MagicDefense;

                    Damage += Attacker.EqStats.TotalMagicDamageIncrease;
                }
                Damage = (uint)(Damage / DmgReduceTimes);
            }            
            uint Exp = 0;
            if (Damage < CurrentHP)
            {
                if (MobID == 5564)
                {
                    if (World.dragon1MSG <= 10)
                        if (CurrentHP < 100000)
                        {
                            World.dragon1MSG++;
                            Game.World.WorldMessage("DRAGON", "TERATODRAGON HP IS lower than" + CurrentHP + " if his max hp 7kk.", 2011, 0, System.Drawing.Color.Snow);
                        }
                }
                if (MobID == 985)
                {
                    if (World.dragon2MSG <= 10)
                        if (CurrentHP < 100000)
                        {
                            World.dragon2MSG++;
                            Game.World.WorldMessage("DRAGON", "Millennium.Dragon HP IS lower than" + CurrentHP + " if his max hp 7kk.", 2011, 0, System.Drawing.Color.Snow);
                        }
                }
                CurrentHP -= Damage;
                if (!IsSkill)
                {
                    if (AT == MobAttackType.FatalStrike)
                    {
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, (ushort)(Loc.X + 1), (ushort)(Loc.Y + 1), Damage, (byte)AT));
                        Attacker.Shift((ushort)(Loc.X), (ushort)(Loc.Y));
                    }
                    else if (AT == MobAttackType.Scapegoat)
                    {
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 43));
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                    }
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                }
                if (Drops)
                {
                    Exp = (uint)(Damage * e);
                    if (!IsSkill)
                    {
                        if (AT == MobAttackType.Ranged || AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                        {
                            if (Attacker.Equips.RightHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.RightHand.ID, 0, 3), Damage);
                            if (Attacker.Equips.LeftHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.LeftHand.ID, 0, 3), Damage / 3 * 2);
                        }
                    }
                }
            }
            else
            {
                if (!Attacker.isAvatar)
                {
                    
                    if (MobID == 5564)
                    {
                        Attacker.CPs += 1000000;
                        Game.World.WorldMessage("GM", Attacker.Name + "has hilled TeratoDragon and got 1kk CPs.", 2011, 0, System.Drawing.Color.Snow);
                    }
                    else if (Name == "Millennium.Dragon")
                    {
                        Attacker.CPs += 500000;
                        Game.World.WorldMessage("GM", Attacker.Name + "has hilled Millennium.Dragon and got 500kCPs.", 2011, 0, System.Drawing.Color.Snow);
                    }
                    Attacker.XPKO++;
                    if (Attacker.Superman || Attacker.Cyclone)
                        Attacker.TotalKO++;
                    if (Attacker.e_quest != null)
                    {
                        if (Name.Contains(Attacker.e_quest.m_name))
                        {
                            Attacker.e_quest.char_ko++;
                            if (Attacker.e_quest.char_ko >= Attacker.e_quest.need_ko)
                                Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You already killed enough demons, find CloudSaint at Market!");
                        }
                    }
                    if (this.EntityID == Attacker.AtkMem.Target)
                    {
                        Attacker.AtkMem.Attacking = false;
                        Attacker.AtkMem.Target = 0;
                    }
                    PlayerTarget = null;
                    Alive = false;
                    uint Benefit = CurrentHP;
                    CurrentHP = 0;
                    PoisonedInfo = null;
                    Died = DateTime.Now;
                    if (!IsSkill)
                    {
                        if (AT == MobAttackType.FatalStrike)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, (ushort)(Loc.X + 1), (ushort)(Loc.Y + 1), Damage, (byte)AT));
                            Attacker.Shift((ushort)(Loc.X), (ushort)(Loc.Y));
                        }
                        else if (AT == MobAttackType.Scapegoat)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 43));
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                        }
                        else
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    }
                    if (Attacker.Superman || Attacker.Cyclone)
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, (uint)(65536 * Attacker.TotalKO), (byte)MobAttackType.Kill));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, (uint)(1), (byte)MobAttackType.Kill));

                    World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));

                    if (Drops)
                    {
                        Exp = (uint)(Benefit * e);
                        if (Attacker.MyTeam != null)
                        {
                            foreach (Character C in Attacker.MyTeam.Members)
                            {
                                if (C != Attacker && C.Alive && MyMath.InBox(C.Loc.X, C.Loc.Y, Attacker.Loc.X, Attacker.Loc.Y, 18))
                                {
                                    if (C.Level + 20 > Level || C.Level >= 70)
                                    {
                                        C.IncreaseExp(MaxHP / 10, true);
                                    }
                                    else
                                    {
                                        uint Amount = (uint)(356 + (C.Level * 27));
                                        byte Lev = C.Level;
                                        C.IncreaseExp(Amount, true);
                                        for (; Lev < C.Level; Lev++)
                                        {
                                            uint VPAmount = (uint)Math.Max(1, Lev * 12 - 17);
                                            Attacker.MyTeam.Leader.VP += VPAmount;
                                            Attacker.MyTeam.Message(Packets.ChatMessage(45216, "SYSTEM", "ALL", Attacker.MyTeam.Leader.Name + " gained " + VPAmount + " virtue points.", 2003, 0, System.Drawing.Color.White));
                                        }
                                    }
                                }
                            }
                        }
                        Attacker.IncreaseExp(MaxHP / 10, false);
                        if (!IsSkill)
                        {
                            if (AT == MobAttackType.Ranged || AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                            {
                                if (Attacker.Equips.RightHand.ID != 0)
                                    Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.RightHand.ID, 0, 3), Benefit);
                                if (Attacker.Equips.LeftHand.ID != 0)
                                    Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.LeftHand.ID, 0, 3), Benefit / 3 * 2);
                            }
                        }
                        DropAnItem(Attacker, Attacker.Level);
                    }
                }
                else if (Attacker.Owner != null)
                {
                    if (MobID == 5564)
                    {
                        Attacker.Owner.CPs += 1000000;
                        Game.World.WorldMessage("GM", Attacker.Owner.Name + "has hilled TeratoDragon and got 1kk CPs.", 2011, 0, System.Drawing.Color.Snow);
                    }
                    else if (Name == "Millennium.Dragon")
                    {
                        Attacker.Owner.CPs += 500000;
                        Game.World.WorldMessage("GM", Attacker.Owner.Name + "has hilled Millennium.Dragon and got 500kCPs.", 2011, 0, System.Drawing.Color.Snow);
                    }
                    Attacker.XPKO++;
                    if (Attacker.Superman || Attacker.Cyclone)
                        Attacker.TotalKO++;
                    if (Attacker.Owner.e_quest != null)
                    {
                        if (Name.Contains(Attacker.Owner.e_quest.m_name))
                        {
                            Attacker.Owner.e_quest.char_ko++;
                            if (Attacker.Owner.e_quest.char_ko >= Attacker.Owner.e_quest.need_ko)
                                Attacker.Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You already killed enough demons, find CloudSaint at Market!");
                        }
                    }
                    if (this.EntityID == Attacker.AtkMem.Target)
                    {
                        Attacker.AtkMem.Attacking = false;
                        Attacker.AtkMem.Target = 0;

                    }
                    PlayerTarget = null;
                    Alive = false;
                    uint Benefit = CurrentHP;
                    CurrentHP = 0;
                    PoisonedInfo = null;
                    Died = DateTime.Now;
                    if (!IsSkill)
                    {
                        if (AT == MobAttackType.FatalStrike)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, (ushort)(Loc.X + 1), (ushort)(Loc.Y + 1), Damage, (byte)AT));
                            Attacker.Shift((ushort)(Loc.X), (ushort)(Loc.Y));
                        }
                        else if (AT == MobAttackType.Scapegoat)
                        {
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 43));
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                        }
                        else
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    }
                    if (Attacker.Superman || Attacker.Cyclone)
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, (uint)(65536 * Attacker.TotalKO), (byte)MobAttackType.Kill));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, (uint)(1), (byte)MobAttackType.Kill));

                    World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));

                    Attacker.AtkMem.Attacking = false;
                    Attacker.AtkMem.Target = 0;

                    if (Drops)
                    {
                        Exp = (uint)(Benefit * e);
                        if (Attacker.MyTeam != null)
                        {
                            foreach (Character C in Attacker.MyTeam.Members)
                            {
                                if (C != Attacker && C.Alive && MyMath.InBox(C.Loc.X, C.Loc.Y, Attacker.Loc.X, Attacker.Loc.Y, 18))
                                {
                                    if (C.Level + 20 > Level || C.Level >= 70)
                                    {
                                        C.IncreaseExp(MaxHP / 10, true);
                                    }
                                    else
                                    {
                                        uint Amount = (uint)(356 + (C.Level * 27));
                                        byte Lev = C.Level;
                                        C.IncreaseExp(Amount, true);
                                        for (; Lev < C.Level; Lev++)
                                        {
                                            uint VPAmount = (uint)Math.Max(1, Lev * 12 - 17);
                                            Attacker.MyTeam.Leader.VP += VPAmount;
                                            Attacker.MyTeam.Message(Packets.ChatMessage(45216, "SYSTEM", "ALL", Attacker.MyTeam.Leader.Name + " gained " + VPAmount + " virtue points.", 2003, 0, System.Drawing.Color.White));
                                        }
                                    }
                                }
                            }
                        }
                        Attacker.IncreaseExp(MaxHP / 10, false);
                        if (Attacker.Owner != null)
                            Attacker.Owner.IncreaseExp(MaxHP, false);
                        if (!IsSkill)
                        {
                            if (AT == MobAttackType.Ranged || AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                            {
                                if (Attacker.Equips.RightHand.ID != 0)
                                    Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.RightHand.ID, 0, 3), Benefit);
                                if (Attacker.Equips.LeftHand.ID != 0)
                                    Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.LeftHand.ID, 0, 3), Benefit / 3 * 2);
                            }
                        }
                        if (!Attacker.MyClient.Robot)
                            DropAnItem(Attacker, Attacker.Level); 
                        else if (Attacker.Owner != null)
                            DropAnItem(Attacker.Owner, Attacker.Owner.Level); 

                    }
                }
            }
            if (!IsSkill)
                Attacker.IncreaseExp(Exp, false);
            return Exp;
        }
        public uint TakeAttack(Mob Attacker, uint Damage, MobAttackType AT, bool IsSkill)
        {
           
                if (LevDifDmg) Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));
                Damage = (uint)(Damage / DmgReduceTimes);
                if (AT == MobAttackType.Melee)
                {
                    if (MeleeDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MeleeDefense;
                }
                else if (AT == MobAttackType.Ranged)
                    Damage = (uint)((double)Damage * ((double)Dodge / 100));
                else if (AT == MobAttackType.Magic)
                {
                    if (MagicDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MagicDefense;
                }

                if (Damage < CurrentHP)
                {
                    CurrentHP -= Damage;
                    if (!IsSkill)
                        if (AT == MobAttackType.Magic)
                            World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.Skill, Attacker.SkillLvl, Loc.X, Loc.Y));
                        else
                            if (AT != MobAttackType.HealHP)
                                World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                }
                else
                {
                    PoisonedInfo = null;
                    PlayerTarget = null;
                    Alive = false;
                    uint Benefit = CurrentHP;
                    CurrentHP = 0;
                    Died = DateTime.Now;
                    if (!IsSkill)
                        if (AT == MobAttackType.Magic)
                            World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.Skill, Attacker.SkillLvl, Loc.X, Loc.Y));
                        else
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
                    World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));

                    if (Drops)
                        DropAnItem(null, Attacker.Level);
                }
            return Damage;
        }
        public void TakeAttack(Companion Attacker, uint Damage, MobAttackType AT)
        {
            try
            {
                if (LevDifDmg) Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));
                Damage = (uint)(Damage / DmgReduceTimes);
                if (AT == MobAttackType.Melee)
                {
                    if (MeleeDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MeleeDefense;
                }
                else if (AT == MobAttackType.Ranged)
                    Damage = (uint)((double)Damage * ((double)Dodge / 100));
                else if (AT == MobAttackType.Magic)
                {
                    if (MagicDefense >= Damage)
                        Damage = 1;
                    else
                        Damage -= MagicDefense;
                }

                if (Damage < CurrentHP)
                {
                    CurrentHP -= Damage;
                    if (AT == MobAttackType.Magic)
                        World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));

                    if (Drops)
                        Attacker.Owner.IncreaseExp(Damage, false);
                }
                else
                {
                    Attacker.Owner.AtkMem.Target = 0;
                    Attacker.Owner.AtkMem.Attacking = false;
                    PoisonedInfo = null;
                    PlayerTarget = null;
                    Alive = false;
                    uint Benefit = CurrentHP;
                    CurrentHP = 0;
                    Died = DateTime.Now;
                    if (Name == Attacker.Owner.MonsterName && Attacker.Owner.AbleToHunt)
                        Attacker.Owner.MonsterHunted++;
                    if (AT == MobAttackType.Magic)
                        World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, (ushort)Attacker.SkillUses, 0, Loc.X, Loc.Y));
                    else
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
                    World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));

                    if (Drops)
                    {
                        DropAnItem(Attacker.Owner, Attacker.Owner.Level);
                        Attacker.Owner.IncreaseExp(Benefit, false);
                    }
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        
        public void GetReflect(ref uint Damage, MobAttackType AT)
        {
            if (Damage < CurrentHP)
            {
                CurrentHP -= Damage;
                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
            }
            else
            {
                Alive = false;
                uint Benefit = CurrentHP;
                CurrentHP = 0;
                PoisonedInfo = null;
                Died = DateTime.Now;

                World.Action(this, Packets.AttackPacket(EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));

                World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));
            }
        }
        public void GetPoisoned(Mob Attacker, uint Damage)
        {
            CurrentHP -= Damage;
            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, 2));
                
                    
        }

        public void DropAnItem(Character Owner, byte OwnerLevel)
        {
            try
            {
                ArrayList Arr = (ArrayList)DropRates.Specifics[MobID];

                #region DropRates
                if (Arr != null)
                {
                    foreach (DropRates.RateItemInfo R in Arr)
                    {
                        if (MyMath.ChanceSuccess(R.DropChance))
                        {
                            DroppedItem DI = new DroppedItem();
                            DI.DropTime = DateTime.Now;
                            DI.UID = (uint)Rnd.Next(10000000);
                            DI.Loc = new Location();
                            DI.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                            DI.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                            DI.Loc.Map = Loc.Map;
                            if (Owner != null)
                            {
                                DI.Loc.MapDimention = Owner.Loc.MapDimention;
                                DI.Owner = Owner.EntityID;
                            }
                            else
                                DI.Owner = 0;
                            DI.Info = new Item();
                            DI.Info.ID = R.ItemID;
                            DI.Info.UID = (uint)Rnd.Next(10000000);
                            DI.Info.Plus = R.Plus;
                            DI.Info.Bless = R.Bless;
                            if (R.Sockets >= 1)
                                DI.Info.Soc1 = Item.Gem.EmptySocket;
                            if (R.Sockets >= 2)
                                DI.Info.Soc2 = Item.Gem.EmptySocket;
                            try
                            {
                                DI.Info.MaxDur = DI.Info.ItemDBInfo.Durability;
                                DI.Info.CurDur = DI.Info.MaxDur;
                            }
                            catch (Exception Exc) { Program.WriteMessage(Exc); }

                            if (!DI.FindPlace((Hashtable)Game.World.H_Items[Loc.Map]))
                            {
                                DI.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                                DI.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                            }
                            if (!DI.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) continue;
                            DI.Drop();
                        }
                    }
                }
                #endregion

                DroppedItem DI2 = new DroppedItem();
                DI2.DropTime = DateTime.Now;
                DI2.UID = (uint)Rnd.Next(10000000);
                DI2.Loc = new Location();
                DI2.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                DI2.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                DI2.Loc.Map = Loc.Map;
                if (Owner != null)
                {
                    DI2.Loc.MapDimention = Owner.Loc.MapDimention;
                    DI2.Owner = Owner.EntityID;
                }
                else
                    DI2.Owner = 0;
                DI2.Info = new Item();
                DI2.Info.UID = (uint)Rnd.Next(10000000);

                #region BeginnarQuest DC
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                    {
                        if (Owner.AbleToHunt == true && Owner.Loc.Map == 1000)
                        {
                            if ((Owner.Job >= 10 && Owner.Job <= 15) || (Owner.Job >= 20 && Owner.Job <= 25))
                            {
                                if (MobID == 100 && Owner.Level <= 70)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 110 && Owner.Level <= 100)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 120 && Owner.Level <= 110)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 130 && Owner.Level <= 120)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region BeginnarQuest AC
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                    {
                        if (Owner.AbleToHunt == true && Owner.Loc.Map == 1020)
                        {
                            if ((Owner.Job >= 50 && Owner.Job <= 55) || (Owner.Job >= 60 && Owner.Job <= 65))
                            {
                                if (MobID == 200 && Owner.Level <= 70)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 210 && Owner.Level <= 100)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 220 && Owner.Level <= 110)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 230 && Owner.Level <= 120)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region BeginnarQuest PC
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                    {
                        if (Owner.AbleToHunt == true && Owner.Loc.Map == 1011)
                        {
                            if (Owner.Job >= 100 && Owner.Job <= 145)
                            {
                                if (MobID == 300 && Owner.Level <= 70)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 310 && Owner.Level <= 100)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 320 && Owner.Level <= 110)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 330 && Owner.Level <= 120)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region BeginnarQuest BC
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                    {
                        if (Owner.AbleToHunt == true && Owner.Loc.Map == 1015)
                        {
                            if (Owner.Job >= 40 && Owner.Job <= 45)
                            {
                                if (MobID == 400 && Owner.Level <= 70)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 410 && Owner.Level <= 100)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 420 && Owner.Level <= 110)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                                else if (MobID == 430 && Owner.Level <= 120)
                                {
                                    if (Owner.MonsterHunted < 100)
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] From " + Owner.MonsterName + " .");
                                        return;
                                    }
                                    else
                                    {
                                        Owner.MonsterHunted += 1;
                                        Owner.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "You Have Killed [" + Owner.MonsterHunted + "/100] You Finished Your Quest.");
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region DisCity KO
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                        if (MobID == 5053)
                        {
                            if (Owner.Job >= 10 && Owner.Job <= 15) { if (Owner.DisKO <= 802) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 800 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 20 && Owner.Job <= 25) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 40 && Owner.Job <= 45) { if (Owner.DisKO <= 1302) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1300 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 50 && Owner.Job <= 55) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 132 && Owner.Job <= 135) { if (Owner.DisKO <= 602) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 600 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 142 && Owner.Job <= 145) { if (Owner.DisKO <= 1002) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1000 monster go back at npc!he teleport you left/right."); }
                        }
                }
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                        if (MobID == 5055)
                        {
                            if (Owner.Job >= 10 && Owner.Job <= 15) { if (Owner.DisKO <= 802) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 800 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 20 && Owner.Job <= 25) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 40 && Owner.Job <= 45) { if (Owner.DisKO <= 1302) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1300 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 50 && Owner.Job <= 55) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 132 && Owner.Job <= 135) { if (Owner.DisKO <= 602) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 600 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 142 && Owner.Job <= 145) { if (Owner.DisKO <= 1002) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1000 monster go back at npc!he teleport you left/right."); }
                        }
                }
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                        if (MobID == 5054)
                        {
                            if (Owner.Job >= 10 && Owner.Job <= 15) { if (Owner.DisKO <= 802) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 800 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 20 && Owner.Job <= 25) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 40 && Owner.Job <= 45) { if (Owner.DisKO <= 1302) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1300 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 50 && Owner.Job <= 55) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 132 && Owner.Job <= 135) { if (Owner.DisKO <= 602) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 600 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 142 && Owner.Job <= 145) { if (Owner.DisKO <= 1002) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1000 monster go back at npc!he teleport you left/right."); }
                        }
                }
                if (MyMath.ChanceSuccess(90))
                {
                    if (Owner != null)
                        if (MobID == 5056)
                        {
                            if (Owner.Job >= 10 && Owner.Job <= 15) { if (Owner.DisKO <= 802) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 800 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 20 && Owner.Job <= 25) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 40 && Owner.Job <= 45) { if (Owner.DisKO <= 1302) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1300 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 50 && Owner.Job <= 55) { if (Owner.DisKO <= 902) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 900 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 132 && Owner.Job <= 135) { if (Owner.DisKO <= 602) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 600 monster go back at npc!he teleport you left/right."); }
                            else if (Owner.Job >= 142 && Owner.Job <= 145) { if (Owner.DisKO <= 1002) { Owner.DisKO++; return; } else Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You finish kill 1000 monster go back at npc!he teleport you left/right."); }
                        }
                }
                #endregion
                #region DisCity Drops
                if (Name == "HeroKing")
                {
                    if (MyMath.ChanceSuccess(50))
                    {
                        if (Owner != null)
                        {
                            Owner.CPs += 20;
                        }
                    }
                }
                if (Name == "Wraith")
                {
                    if (MyMath.ChanceSuccess(1))
                    {
                        DI2.Info.ID = 725016; ;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                if (Name == "Syren")
                {
                    if (MyMath.ChanceSuccess(100))
                    {
                        DI2.Info.ID = 720028; ;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                if (Name == "UndeadSpearman" || Name == "UndeadSoldier")
                {
                    if (MyMath.ChanceSuccess(80))
                    {
                        if (Owner != null)
                        {
                            Owner.CPs += 100;
                        }
                    }
                }
                if (Name == "UndeadSpearman" && Loc.Map == 2021)
                {
                    if (MyMath.ChanceSuccess(4))
                    {
                        DI2.Info.ID = 1088000; ;
                        DI2.Info.ID = 723085; ;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                if (Name == "UndeadSoldier" && Loc.Map == 2021)
                {
                    if (MyMath.ChanceSuccess(4))
                    {
                        DI2.Info.ID = 723085; ;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                #endregion

                #region Denguns Drops
                //Dark Dengun
                if (MobID == 852 || MobID == 853 || MobID == 854 || MobID == 855 || MobID == 856 || MobID == 857)
                {
                    if (MyMath.ChanceSuccess(4))
                    {
                        DI2.Info.ID = 710034; ;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                //MainDengun Quest
                if (Owner !=null && Owner.myDengun1 != null)
                {
                    if (Owner.myDengun1.DengunMaps.Contains(Owner.Loc.Map))
                    {
                        Owner.myDengun1.MonstersDrope(this,Owner);
                        

                    }
                // dengun 
                    if (Owner.myDengun1.DengunMaps.Contains(Loc.Map))
                    {
                        Owner.myDengun1.DMKilledCounter(Name);
                    }
                }
               
                #endregion

                #region Spicial Monesters
                if (Name == "Dragon.Son")
                {
                    if (MyMath.ChanceSuccess(5))
                    {
                        DI2.Info.ID = 1200000;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                if (Name == "S-Dragon")
                {
                    if (MyMath.ChanceSuccess(45))
                    {
                        if (Owner != null)
                        {
                            Owner.CPs += 40;
                        }
                    }
                }
                if (Name == "Hero-CPs" || Name == "Hero-DB")
                {
                    if (MyMath.ChanceSuccess(30))
                    {
                        if (Owner != null)
                        {
                            Owner.CPs += 20;
                        }
                    }
                }
                if (Name == "Hero-DB")
                {
                    if (MyMath.ChanceSuccess(2))
                    {
                        DI2.Info.ID = 1088000;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                if (Name == "HillMonster")
                {

                    if (MyMath.ChanceSuccess(20))
                    {
                        DI2.Info.ID = 1080001;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                        DI2.Info.CurDur = DI2.Info.MaxDur;
                    }
                }
                #endregion


                #region Drop Silver
                if (MyMath.ChanceSuccess(DropRates.Silver))
                {
                    DI2.Silvers = (uint)(Rnd.Next(MinSilvers, MaxSilvers) * 6);
                    if (Owner != null)
                        DI2.Loc.MapDimention = Owner.Loc.MapDimention;
                    if (Owner != null)
                        if (Owner.VipLevel >= 7)
                        {
                            Owner.Silvers += DI2.Silvers;
                            return;
                        }
                    DI2.UID = (uint)Rnd.Next(10000000);
                    DI2.Loc.X = (ushort)(Loc.X + Rnd.Next(4) - Rnd.Next(4));
                    DI2.Loc.Y = (ushort)(Loc.Y + Rnd.Next(4) - Rnd.Next(4));
                    if (DI2.Silvers < 10)
                        DI2.Info.ID = 1090000;
                    else if (DI2.Silvers < 1000)
                        DI2.Info.ID = 1090010;
                    else if (DI2.Silvers < 10000)
                        DI2.Info.ID = 1090020;
                    else if (DI2.Silvers < 30000)
                        DI2.Info.ID = 1091000;
                    else if (DI2.Silvers < 100000)
                        DI2.Info.ID = 1091010;
                    else
                        DI2.Info.ID = 1091020;

                    if (!DI2.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) return;
                    DI2.Drop();
                }
                #endregion
                #region Drop DragonBall
                if (MyMath.ChanceSuccess(DropRates.DragonBall))
                {
                    DI2.Info.ID = 1088000;
                    DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;
                    DI2.Info.CurDur = DI2.Info.MaxDur;
                }
                #endregion
                #region Drop Items
                else if (MyMath.ChanceSuccess(DropRates.Item))
                {
                    Item.ItemQuality Q = Item.ItemQuality.Simple;
                    if (MyMath.ChanceSuccess(DropRates.Refined))
                        Q = Item.ItemQuality.Refined;
                    if (MyMath.ChanceSuccess(DropRates.Unique))
                        Q = Item.ItemQuality.Unique;
                    if (MyMath.ChanceSuccess(DropRates.Elite))
                        Q = Item.ItemQuality.Elite;
                    if (MyMath.ChanceSuccess(DropRates.Super))
                        Q = Item.ItemQuality.Super;
                    uint ItemID = 0;

                    ArrayList From = new ArrayList();
                    foreach (DatabaseItem D in Database.DatabaseItems.Values)
                    {
                        if (D.Level + 5 > Level && D.Level - 5 <= Level)
                        {
                            if (D.Level != 0)
                                From.Add(D.ID);
                        }
                    }
                    if (From != null)
                    {
                        byte Tries = (byte)Rnd.Next(0, From.Count);
                        ItemID = (uint)From[Tries];
                    }
                    if (ItemID != 0)
                    {
                        DI2.Info.ID = ItemID;
                        if (DI2.Info.ItemDBInfo.Level != 1)
                        {
                            ItemIDManipulation E = new ItemIDManipulation(ItemID);
                            E.QualityChange(Q);
                            DI2.Info.ID = E.ToID();
                        }
                        DI2.Info.Color = Item.ArmorColor.Orange;
                        if (ItemIDManipulation.Digit(DI2.Info.ID, 1) == 4 || ItemIDManipulation.Digit(DI2.Info.ID, 1) == 5)
                        {
                            if (MyMath.ChanceSuccess(DropRates.OneSoc))
                                DI2.Info.Soc1 = Item.Gem.EmptySocket;
                            if (MyMath.ChanceSuccess(DropRates.TwoSoc))
                            {
                                DI2.Info.Soc1 = Item.Gem.EmptySocket;
                                DI2.Info.Soc2 = Item.Gem.EmptySocket;
                            }
                        }

                        if (MyMath.ChanceSuccess(DropRates.PlusOne))
                            DI2.Info.Plus = 1;
                        DI2.Info.MaxDur = DI2.Info.ItemDBInfo.Durability;

                        DI2.Info.CurDur = DI2.Info.MaxDur;
                        if (Q == Item.ItemQuality.Super || Q == Item.ItemQuality.Elite)
                            if (Owner != null)
                            {
                                if (Owner.Inventory.Count <= 39)
                                {
                                    Owner.CreateItemIDAmount(DI2.Info.ID, 1);
                                    return;
                                }
                            }
                    }
                }
                if (DI2.Info.ID != 0)
                {
                    if (!DI2.FindPlace((Hashtable)Game.World.H_Items[Loc.Map])) return;
                    DI2.Drop();
                }
                #endregion
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
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

                //World.Spawns(, true);
            }
        }
        
        public void promoud(byte level ,ushort Skill, byte SkillLevel, ushort cooldown)
        {
            MultyAttackLevel = Level;
            S_Skill = Skill;
            S_SkillLvl = SkillLevel;
            S_SkillSpeed = cooldown;

        }
        public void Step()
        {
            
            try
            {
                if (!Alive)
                {
                    if (!Dissappeared && DateTime.Now > Died.AddSeconds(1))
                    {
                        World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
                        Dissappeared = true;
                        RandomTime = (uint)(Rnd.Next(15, 25));
                    }
                    if (DateTime.Now > Died.AddSeconds(ReSpawnSpeed + RandomTime))//changed back to 30 by Ricardo
                        Respawn();
                }
                if (Alive && DateTime.Now > LastMove.AddMilliseconds(750))
                {
                    #region buffers
                    try
                    {
                        if (DateTime.Now > LastBuffRemove.AddMilliseconds(500))
                        {
                            LastBuffRemove = DateTime.Now;
                            ArrayList BDelete = new ArrayList();
                            if (Buffers != null)
                            foreach (Buff B in Buffers)
                            {

                                ushort Time = B.Lasts;
                                if (B.Eff == Skills.SkillsClass.ExtraEffect.Cyclone || B.Eff == Skills.SkillsClass.ExtraEffect.Superman)
                                {
                                    Time = (ushort)(B.Lasts);
                                    if (Time > 60)
                                        Time = 60;
                                }
                                if (DateTime.Now > B.Started.AddSeconds(Time))
                                {
                                    //if (B.Eff == Skills.SkillsClass.ExtraEffect.ShurikenVortex)
                                    //VortexOn = false;
                                    BDelete.Add(B);
                                }
                            }

                            foreach (Buff B in BDelete)
                            {
                                RemoveBuff(B);

                            }
                        }
                    }
                    catch (Exception e) { Program.WriteMessage(e); }
                    #endregion
                    LastMove = DateTime.Now;
                    if ((Type == MobType.HuntPlayers || Type == MobType.HuntMobsAndBlue) && PlayerTarget == null && MobTarget == null)
                    {
                        byte NDist = 13;
                        byte MaxDist = 12;
                        if (PlayerTarget == null && CompanionTarget == null)
                        {
                            foreach (Character C in World.H_Chars.Values)
                            {
                                if (C.Loc.Map == Loc.Map && C.Loc.MapDimention == Loc.MapDimention)
                                {
                                    if (C.Alive)
                                    {
                                        if (C.CanBeMeleed || AttackType != MobAttackType.Melee)
                                        {
                                            byte Dst = (byte)MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y);
                                            if (Dst <= MaxDist && Dst < NDist)
                                            {
                                                if ((Type != MobType.HuntMobsAndBlue || C.BlueName) && Type != MobType.HuntMobs)
                                                {
                                                    NDist = (byte)MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y);
                                                   
                                                        PlayerTarget = C;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (PlayerTarget == null && CompanionTarget == null)

                        foreach (Companion C in World.H_Companions.Values)
                        {
                            if (C.Loc.Map == Loc.Map && C.Loc.MapDimention == Loc.MapDimention)
                            {
                                if (C.Alive)
                                {

                                    {
                                        byte Dst = (byte)MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y);
                                        if (Dst <= MaxDist && Dst < NDist)
                                        {
                                            if (Type != MobType.HuntMobsAndBlue && Type != MobType.HuntMobs)
                                            {
                                                NDist = (byte)MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y);
                                                CompanionTarget = C;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Type == MobType.HuntMobs || Type == MobType.HuntMobsAndBlue && MobTarget == null && PlayerTarget == null)
                    {
                        byte NDist = 15;
                        byte MaxDist = 8;
                        foreach (Mob M in ((Hashtable)World.H_Mobs[Loc.Map]).Values)
                            if ((M != this || Skill == 1005) && M.Alive && MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= MaxDist && MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) < NDist)
                            {
                                if ((Skill == 1005 && CurrentHP < MaxHP) || Skill != 1005)
                                {
                                    NDist = (byte)MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y);
                                    MobTarget = M;
                                }
                            }
                    }
                    #region Companion Target
                        if (CompanionTarget != null && CompanionTarget.Alive && MyMath.PointDistance(Loc.X, Loc.Y, CompanionTarget.Loc.X, CompanionTarget.Loc.Y) < Math.Max(15, (int)AttackDist) && ((Type == MobType.HuntMobsAndBlue && PlayerTarget.BlueName) || Type == MobType.HuntPlayers))
                        {
                            if (MyMath.PointDistance(Loc.X, Loc.Y, CompanionTarget.Loc.X, CompanionTarget.Loc.Y) >= AttackDist)
                            {
                                byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(Loc.X, Loc.Y, CompanionTarget.Loc.X, CompanionTarget.Loc.Y) / 45 % 8)) - 1 % 8);
                                Direction = (byte)((int)ToDir % 8);

                                Location eLoc = Loc;
                                eLoc.Walk(Direction);
                                System.Collections.Hashtable H = (System.Collections.Hashtable)World.H_Mobs[Loc.Map];
                                bool PlaceFree = true;
                                if (DMaps.Loaded)
                                {
                                    if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;
                                }
                                foreach (Mob M in H.Values)
                                    if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                    {
                                        PlaceFree = false;
                                        break;
                                    }
                                if (PlaceFree)
                                {
                                    World.Action(this, Packets.Movement(EntityID, Direction, 0));
                                    World.Spawn(this, true);
                                    Loc.Walk(Direction);
                                }
                                else
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        PlaceFree = true;
                                        eLoc = Loc;
                                        Direction = (byte)((Direction + 1) % 8);
                                        eLoc.Walk(Direction);

                                        if (DMaps.Loaded)
                                            if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;

                                        foreach (Mob M in H.Values)
                                            if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                            {
                                                PlaceFree = false;
                                                break;
                                            }
                                        if (PlaceFree)
                                        {
                                            World.Action(this, Packets.Movement(EntityID, Direction, 0));
                                            World.Spawn(this, true);
                                            Loc.Walk(Direction);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((DateTime.Now >= LastAttack.AddMilliseconds(4000) || (DateTime.Now >= LastAttack.AddMilliseconds(1200) && MobID != 5564 && MobID != 985)))
                                {
                                    if (MyMath.ChanceSuccess(75) || Type == MobType.HuntMobsAndBlue)
                                    {
                                        if (Skill > 0)
                                        {

                                            SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                            SU.Init(this, Skill, SkillLvl, CompanionTarget.Loc.X, CompanionTarget.Loc.Y);
                                            SU.M_GetCompanionTargets(CompanionTarget.EntityID);
                                            SU.MUse();

                                        }
                                        else
                                            CompanionTarget.TakeAttack(this, PrepareAttack(), AttackType, false);
                                    }
                                    else
                                        CompanionTarget.TakeAttack(this, 0, AttackType, false);
                                    LastAttack = DateTime.Now;
                                }
                            }

                        }
                        else { CompanionTarget = null; }
                        #endregion
                    
                    #region Player Target
                    if (PlayerTarget != null && (PlayerTarget.CanBeMeleed || AttackType != MobAttackType.Melee) && PlayerTarget.Alive && PlayerTarget.MyClient != null && MyMath.PointDistance(Loc.X, Loc.Y, PlayerTarget.Loc.X, PlayerTarget.Loc.Y) < Math.Max(15, (int)AttackDist) && ((Type == MobType.HuntMobsAndBlue && PlayerTarget.BlueName) || Type == MobType.HuntPlayers) )
                    {
                        if (MyMath.PointDistance(Loc.X, Loc.Y, PlayerTarget.Loc.X, PlayerTarget.Loc.Y) >= AttackDist)
                        {
                            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(Loc.X, Loc.Y, PlayerTarget.Loc.X, PlayerTarget.Loc.Y) / 45 % 8)) - 1 % 8);
                            Direction = (byte)((int)ToDir % 8);

                            Location eLoc = Loc;
                            eLoc.Walk(Direction);
                            System.Collections.Hashtable H = (System.Collections.Hashtable)World.H_Mobs[Loc.Map];
                            bool PlaceFree = true;
                            if (DMaps.Loaded)
                            {
                                if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;
                            }
                            foreach (Mob M in H.Values)
                                if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                {
                                    PlaceFree = false;
                                    break;
                                }
                            if (PlaceFree)
                            {
                                World.Action(this, Packets.Movement(EntityID, Direction, 0));
                                World.Spawn(this, true);
                                Loc.Walk(Direction);
                            }
                            else
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    PlaceFree = true;
                                    eLoc = Loc;
                                    Direction = (byte)((Direction + 1) % 8);
                                    eLoc.Walk(Direction);

                                    if (DMaps.Loaded)
                                        if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;

                                    foreach (Mob M in H.Values)
                                        if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                        {
                                            PlaceFree = false;
                                            break;
                                        }
                                    if (PlaceFree)
                                    {
                                        World.Action(this, Packets.Movement(EntityID, Direction, 0));
                                        World.Spawn(this, true);
                                        Loc.Walk(Direction);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MultyAttackLevel == 0 && (DateTime.Now >= LastAttack.AddMilliseconds(4000) || (DateTime.Now >= LastAttack.AddMilliseconds(attackSpeed) && MobID != 5564 && MobID != 985)) && PlayerTarget.Protection == false)
                            {
                                //Console.WriteLine("1");
                                if (MyMath.ChanceSuccess(80) || Type == MobType.HuntMobsAndBlue)
                                {
                                    if (Type == MobType.HuntMobsAndBlue)
                                    {
                                        SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                        SU.Init(this, 1000, 4, PlayerTarget.Loc.X, PlayerTarget.Loc.Y);
                                        SU.M_GetTargets(PlayerTarget.EntityID);
                                        SU.MUse();
                                    }
                                    else if (Skill > 0)
                                    {
                                        if (DateTime.Now >= SkillLastttack.AddMilliseconds(SkillSpeed))
                                        {
                                            SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                            SU.Init(this, Skill, SkillLvl, PlayerTarget.Loc.X, PlayerTarget.Loc.Y);
                                            SU.M_GetPlayerTargets(PlayerTarget.EntityID);
                                            SU.MUse();

                                            SkillLastttack = DateTime.Now;
                                        }
                                    }
                                    else
                                    {

                                        PlayerTarget.TakeAttack(this, PrepareAttack(), AttackType, false);
                                    }
                                }
                                else
                                    PlayerTarget.TakeAttack(this, 0, AttackType, false);
                                LastAttack = DateTime.Now;
                            }
                            if (MultyAttackLevel > 0 && (DateTime.Now >= LastAttack.AddMilliseconds(attackSpeed)) && PlayerTarget.Protection == false)
                            {
                                
                                    if (Skill > 0 || S_Skill > 0)
                                    {
                                        if (MyMath.ChanceSuccess(80))
                                        {
                                            if (Skill > 0 && DateTime.Now >= SkillLastttack.AddMilliseconds(SkillSpeed))
                                            {
                                                SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                                SU.Init(this, Skill, SkillLvl, PlayerTarget.Loc.X, PlayerTarget.Loc.Y);
                                                SU.M_GetPlayerTargets(PlayerTarget.EntityID);
                                                SU.MUse();

                                                SkillLastttack = DateTime.Now;
                                            }
                                            if (S_Skill > 0 && DateTime.Now >= S_SkillLastttack.AddMilliseconds(S_SkillSpeed) && DateTime.Now >= SkillLastttack.AddMilliseconds(500))
                                            {

                                                SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                                SU.Init(this, S_Skill, S_SkillLvl, PlayerTarget.Loc.X, PlayerTarget.Loc.Y);
                                                SU.M_GetPlayerTargets(PlayerTarget.EntityID);
                                                SU.MUse();

                                                S_SkillLastttack = DateTime.Now;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (MyMath.ChanceSuccess(80))

                                            PlayerTarget.TakeAttack(this, PrepareAttack(), AttackType, false);
                                        else
                                            PlayerTarget.TakeAttack(this, 0, AttackType, false);
                                    }

                                LastAttack = DateTime.Now;
                            }
                        }                       
                    }
                    else { PlayerTarget = null; }
                    #endregion
                    #region Mob Target
                    if (MobTarget != null && MobTarget.Alive && MobTarget.Type != MobType.HuntMobsAndBlue && MyMath.PointDistance(Loc.X, Loc.Y, MobTarget.Loc.X, MobTarget.Loc.Y) < 13)
                    {
                        if (MyMath.PointDistance(Loc.X, Loc.Y, MobTarget.Loc.X, MobTarget.Loc.Y) >= AttackDist)
                        {
                            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(Loc.X, Loc.Y, MobTarget.Loc.X, MobTarget.Loc.Y) / 45 % 8)) - 1 % 8);
                            Direction = (byte)((int)ToDir % 8);

                            Location eLoc = Loc;
                            eLoc.Walk(Direction);
                            System.Collections.Hashtable H = (System.Collections.Hashtable)World.H_Mobs[Loc.Map];
                            bool PlaceFree = true;

                            if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;

                            foreach (Mob M in H.Values)
                                if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                {
                                    PlaceFree = false;
                                    break;
                                }
                            if (PlaceFree)
                            {
                                World.Action(this, Packets.Movement(EntityID, Direction , 0));
                                World.Spawn(this, true);
                                Loc.Walk(Direction);
                            }
                            else
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    PlaceFree = true;
                                    eLoc = Loc;
                                    Direction = (byte)((Direction + 1) % 8);
                                    eLoc.Walk(Direction);

                                    if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess) PlaceFree = false;

                                    foreach (Mob M in H.Values)
                                        if (M != this && M.Loc.X == eLoc.X && M.Loc.Y == eLoc.Y && M.Alive)
                                        {
                                            PlaceFree = false;
                                            break;
                                        }
                                    if (PlaceFree)
                                    {
                                        World.Action(this, Packets.Movement(EntityID, Direction , 0));
                                        World.Spawn(this, true);
                                        Loc.Walk(Direction);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (DateTime.Now >= LastAttack.AddMilliseconds(attackSpeed))
                            {
                                if (Skill > 0)
                                {
                                    SkillsClass.SkillUse SU = new SkillsClass.SkillUse();
                                    SU.Init(this, Skill, SkillLvl, MobTarget.Loc.X, MobTarget.Loc.Y);
                                    SU.M_GetMobTargets(MobTarget.EntityID);
                                    SU.MUse();
                                }
                                else
                                {
                                    MobTarget.TakeAttack(this, PrepareAttack(), AttackType, false);
                                }
                                LastAttack = DateTime.Now;

                            }
                        }
                    }
                    else if (MobTarget != null) MobTarget = null;
                    #endregion
                }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }
    }
}