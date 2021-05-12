using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Features;
using Server.Game;
using Server.Skills;

namespace Server.Game
{
    public class Companion
    {
        public DateTime Died;
        public uint RandomTime = 30;
        public static Random Rnd = new Random();
        public bool Dissappeared = false;
        public bool Alive = true;
        public ushort MDef;
        public int Dodge;
        public ushort Defense;

        public uint MinAttack;
        public uint MaxAttack;
        public byte Level;
        public uint SkillUses;//0 if just melee
        public Location Loc;
        public Character Owner;
        public uint CurHP;
        public uint MaxHP;
        public uint Mesh;
        public uint EntityID;

        public string Name;
        public byte Direction;
        DateTime LastMovement = DateTime.Now;
        DateTime LastAttack = DateTime.Now;
        DateTime Lastrun = DateTime.Now;
        DateTime LastJump = DateTime.Now;
        public PoisonType PoisonedInfo = null;
        public StatusEffect StatEff;
        public ArrayList Buffers;

        public Companion(Character Owner, uint Type)
        {
            this.Owner = Owner;
            if (Database.CompanionInfos.Contains(Type))
            {
               
                CompanionInfo Cmp = (CompanionInfo)Database.CompanionInfos[Type];
                MinAttack = Cmp.MinAttack;
                MaxAttack = Cmp.MaxAttack;
                Level = Cmp.Level;
                SkillUses = Cmp.SkillUses;
                CurHP = Cmp.HP;
                MaxHP = Cmp.HP;
                Mesh = Cmp.Mesh;
                Name = Cmp.Name;
                Defense = Cmp.Defence;
                MDef = (ushort)Math.Min(Level * 70, 400); ;
                Dodge = Math.Min(Level * 20 , 100);
                EntityID = (uint)Program.Rnd.Next(400000, 500000);
                Direction = 0;
                Loc = Owner.Loc;
                Loc.X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                Loc.Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                StatEff = new StatusEffect(this);
                Buffers = new ArrayList();
                World.H_Companions.Add(EntityID, this);
                World.Spawn(this, false);
            }
            CurHP = MaxHP;
        }
        public uint TakeAttack(Character Attacker, ref uint Damage, MobAttackType AT, bool p)
        {
            if (AT != MobAttackType.Magic && Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.Superman).Eff == Skills.SkillsClass.ExtraEffect.Superman)
                Damage *= 3;
            if (!p && Attacker.BuffOf(Skills.SkillsClass.ExtraEffect.FatalStrike).Eff == Skills.SkillsClass.ExtraEffect.FatalStrike)
            {
                AT = MobAttackType.FatalStrike;
                Damage *= 2;
            }

            if (AT != MobAttackType.Magic && !p)
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
                /* else
                 {
                     if (LevDifDmg) Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));
                 }*/
            }
            if (!DMaps.FreePKMaps.Contains(Loc.Map))
            {
                Attacker.BlueName = true;
                if (Attacker.BlueNameLasts < 15)
                    Attacker.BlueNameLasts = 15;
            }
            if(!p)
            Damage = (uint)(Damage * Defense / 100);
            if (!p && Damage != 0)
            {
                
                if (AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                {
                    if (Defense >= Damage)
                        Damage = 1;
                    else
                        Damage -= Defense;

                    Damage += Attacker.EqStats.TotalDamageIncrease;
                }
                else if (AT == MobAttackType.Ranged)
                {
                    Damage = (uint)((double)Damage * ((double)(200 - Dodge) / 100));
                    Damage += Attacker.EqStats.TotalDamageIncrease;
                }
                else
                {
                    if (MDef >= Damage)
                        Damage = 1;
                    else
                        Damage -= MDef;

                    Damage += Attacker.EqStats.TotalMagicDamageIncrease;
                }
                //Damage = (uint)(Damage / 1);
            }
            uint Exp = 0;
            if (Damage < CurHP)
            {
                CurHP -= Damage;
                if (!p)
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

            }
            else
            {
                if (Attacker.AbleToHunt == true & Attacker.MonsterName == Name)
                {
                    Attacker.MonsterHunted++;
                }
                Attacker.XPKO++;
                if (Attacker.Superman || Attacker.Cyclone)
                    Attacker.TotalKO++;

                Alive = false;
                uint Benefit = CurHP;
                CurHP = 0;
                PoisonedInfo = null;
                Died = DateTime.Now;
                if (!p)
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
                

            }

            return Exp;
        }
        public uint TakeAttack(Mob Attacker, uint Damage, MobAttackType AT, bool IsSkill)
        {
          
            if (Damage != 0)
                Damage = (uint)(Damage * Defense / 100);
               // Damage = (uint)(Damage * MyMath.LevelDifference(Attacker.Level, Level));
                //Damage = (uint)(Damage / DmgReduceTimes);

            if (AT == MobAttackType.Melee && Damage != 0)
                {
                    if (Defense >= Damage)
                        Damage = 1;
                    else
                        Damage -= Defense;
                }
                else if (AT == MobAttackType.Ranged)
                    Damage = (uint)((double)Damage * ((double)Dodge / 100));
                else if (AT == MobAttackType.Magic)
                {
                    if (MDef >= Damage)
                        Damage = 1;
                    else
                        Damage -= MDef;
                }

                if (Damage < CurHP)
                {
                    CurHP -= Damage;
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
                    //PlayerTarget = null;
                    Alive = false;
                    uint Benefit = CurHP;
                    CurHP = 0;
                    Died = DateTime.Now;
                    if (!IsSkill)
                        if (AT == MobAttackType.Magic)
                            World.Action(this, Packets.SkillUse(Attacker.EntityID, EntityID, Damage, Attacker.Skill, Attacker.SkillLvl, Loc.X, Loc.Y));
                        else
                            World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));
                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)MobAttackType.Kill));
                    World.Action(this, Packets.Status(EntityID, Status.Effect, 2080));

                   // if (Gives)
                   //     DropAnItem(null, Attacker.Level);
                }
           
            return Damage;
        }

        public void AddBuff(Buff B)
        {

            Buff ExBuff = BuffOf(B.Eff);
            if (ExBuff.Eff == B.Eff)
                Buffers.Remove(ExBuff);

            Buffers.Add(B);
            StatEff.Add(B.StEff);
            
        }
        public Buff BuffOf(SkillsClass.ExtraEffect E)
        {
            if (Buffers != null)
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
        public void Dissappear()
        {
            if (World.H_Companions.Contains(EntityID))
            {
                World.H_Chars.Remove(Owner.AtkMem.Target);
                World.H_Companions.Remove(EntityID);
                if (Owner.MyCompanion == this)
                    Owner.MyCompanion = null;
                if (Owner.MyCompanion1 == this)
                    Owner.MyCompanion1 = null;
                if (Owner.MyCompanion2 == this)
                    Owner.MyCompanion2 = null;
                if (Owner.MyCompanion3 == this)
                    Owner.MyCompanion3 = null;
                if (Owner.MyCompanion4 == this)
                    Owner.MyCompanion4 = null;
                if (Owner.MyCompanion5 == this)
                    Owner.MyCompanion5 = null;
                if (Owner.MyCompanion6 == this)
                    Owner.MyCompanion6 = null;
                if (Owner.MyCompanion7 == this)
                    Owner.MyCompanion7 = null;
                if (Owner.MyCompanion8 == this)
                    Owner.MyCompanion8 = null;
                if (Owner.MyCompanion9 == this)
                    Owner.MyCompanion9 = null;
                if (Owner.MyCompanion10 == this)
                    Owner.MyCompanion10 = null;
                if (Owner.MyCompanion11 == this)
                    Owner.MyCompanion11 = null;
                //if (Owner.MyCompanion12 == this)
                //   Owner.MyCompanion12 = null;
                // Owner.MyCompanion1 = null;

                World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
            }

        }

        public void Step()
        {
            if (!Alive)
            {
                if (!Dissappeared && DateTime.Now > Died.AddSeconds(2))
                {
                    World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
                    Dissappeared = true;
                   
                }
            }
            if (Alive && DateTime.Now > LastMovement.AddMilliseconds(350))
            {
                LastMovement = DateTime.Now;
                if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 15)
                {
                    if (Owner.AtkMem.Target != 0 && Owner.AtkMem.Target != Owner.EntityID && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 28)
                    {
                        if (SkillUses != 0 && DateTime.Now > LastAttack.AddMilliseconds(1000))
                        {
                            LastAttack = DateTime.Now;
                            uint Damage = (uint)Program.Rnd.Next((int)MinAttack, (int)MaxAttack);

                            if (World.H_Chars.Contains(Owner.AtkMem.Target))
                            {
                                Character C = (Character)World.H_Chars[Owner.AtkMem.Target];
                                if (C.Alive && C.Loc.Map == Loc.Map && C.Loc.MapDimention == Loc.MapDimention && MyMath.PointDistance(Loc.X, Loc.Y, C.Loc.X, C.Loc.Y) <= 10 && !DMaps.NoPKMaps.Contains(Loc.Map) && C.PKAble(Owner.PKMode, Owner) && Owner.Loc.Map == C.Loc.Map)

                                    C.TakeAttack(this, Damage, MobAttackType.Magic);
                            }
                            else if (World.H_Mobs.Contains(Loc.Map) && ((Hashtable)World.H_Mobs[Loc.Map]).Contains(Owner.AtkMem.Target))
                            {
                                Mob M = (Mob)((Hashtable)World.H_Mobs[Loc.Map])[Owner.AtkMem.Target];
                                if (M.Alive && M.Loc.Map == Loc.Map && M.Loc.MapDimention == Loc.MapDimention && MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= 10)
                                    M.TakeAttack(this, Damage, MobAttackType.Magic);
                            }
                        }
                    }
                    else if (Owner.AtkMem.Target == 0 && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) >= 7 && DateTime.Now > LastJump.AddMilliseconds(350))
                    {
                        GetDirection(Owner.Loc);
                        byte eDir = Direction;
                        bool Success = true;

                        while (!FreeToGo())
                        {
                            try
                            {
                                Direction = (byte)((Direction + 1) % 8);
                                if (Direction == eDir)
                                {
                                    Success = false;
                                    break;
                                }
                                Success = true;
                            }
                            catch { }
                        }
                        if (!Success)
                            JumpToOwner();
                        else
                        {
                            Loc.Walk(Direction);
                            World.Action(this, Packets.Movement(EntityID, Direction, 1));
                            World.Spawn(this, true);
                        }
                    }
                    else if (Owner.AtkMem.Target == 0 && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) >= 4 && DateTime.Now > LastJump.AddMilliseconds(350))
                    {
                        GetDirection(Owner.Loc);
                        byte eDir = Direction;
                        bool Success = true;

                        while (!FreeToGo())
                        {
                            try
                            {
                                Direction = (byte)((Direction + 1) % 8);
                                if (Direction == eDir)
                                {
                                    Success = false;
                                    break;
                                }
                                Success = true;
                            }
                            catch { }
                        }
                        if (!Success)
                            JumpToOwner();
                        else
                        {
                            
                            Loc.Walk(Direction);
                            World.Action(this, Packets.Movement(EntityID, Direction, 0));
                            World.Spawn(this, true);
                        }
                    }
                }
                else
                    JumpToOwner();
            }
        }
        void JumpToOwner()
        {
            LastJump = DateTime.Now;
            ushort X = Loc.X;
            ushort Y = Loc.Y;
            Loc.X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
            Loc.Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
            World.Action(this, Packets.GeneralData(EntityID, Loc.X, Loc.Y, X, Y, Direction, 137));
            World.Spawn(this, true);
        }
        void GetDirection(Location Target)
        {
            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(Loc.X, Loc.Y, Target.X, Target.Y) / 45 % 8)) - 1 % 8);
            Direction = (byte)((int)ToDir % 8);
        }
        bool FreeToGo()
        {
            Location eLoc = Loc;
            eLoc.Walk(Direction);
            if (!DMaps.H_Maps.Contains(Loc.Map))
                return true;
            if (((DMap)DMaps.H_Maps[Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess)
                return false;
            return true;
        }
    }

}
