using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class NPC
    {
        public Location Loc;
        public ushort Type;
        public uint EntityID;
        public byte Direction;
        public byte Avatar;
        public byte Flags;//Shop = 1, Dialog = 2, FaceChange = 5, TCUpgrade = 6, GemSocket = 7, ShopCarpet = 14, AskVending = 16, Gamble = 19, Stake = 21, Scarecrow = 22


        public byte Level;

        public uint CurHP = 0;
        public uint MaxHP = 0;

        public NPC()
        {
        }
        public NPC(string Line)
        {
            string[] Info = Line.Split(' ');
            EntityID = uint.Parse(Info[0]);
            Type = ushort.Parse(Info[1]);
            Flags = byte.Parse(Info[2]);
            Avatar = byte.Parse(Info[3]);
            Loc = new Location();
            Loc.Map = ushort.Parse(Info[4]);
            Loc.X = ushort.Parse(Info[5]);
            Loc.Y = ushort.Parse(Info[6]);
            
            if (Flags == 21)
                Level = (byte)((Type - 427) / 6 + 20);
            if (Flags == 22)
                Level = (byte)((Type - 437) / 6 + 20);
            if (Type == 1500)
                Level = 125;
            if (Type == 1520)
                Level = 125;

            if (Flags == 21 || Flags == 22)
            {
                CurHP = 10000;
                MaxHP = 10000;
            }
        }
        public uint TakeAttack(Character Attacker, uint Damage, MobAttackType AT, bool IsSkill)
        {
            try
            {
                if (Level > Attacker.Level)
                {
                    Attacker.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "This dummy is too high level for you.");
                    return 0;
                }
                double e = 0.1;
                if (Level + 4 < Attacker.Level)
                    e = 0.01;
                if (Level + 4 >= Attacker.Level)
                    e = 0.1;

                if (AT == MobAttackType.Melee || AT == MobAttackType.Ranged || AT == MobAttackType.FatalStrike)
                    Damage += Attacker.EqStats.TotalDamageIncrease;
                else if (AT == MobAttackType.Magic)
                    Damage += Attacker.EqStats.TotalMagicDamageIncrease;

                uint Exp = 0;
                if (Damage < CurHP)
                {
                    CurHP -= Damage;
                    if (AT != MobAttackType.Magic && !IsSkill)
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));

                    Exp = (uint)(Damage * e);
                    if (!IsSkill)
                    {
                        if (AT == MobAttackType.Ranged || AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                        {
                            if (Attacker.Equips.RightHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.RightHand.ID, 0, 3), Damage / 100);
                            if (Attacker.Equips.LeftHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.LeftHand.ID, 0, 3), Damage / 300 * 2);
                        }
                    }
                    if (!IsSkill)
                        Attacker.IncreaseExp(Exp, false);
                }
                else
                {
                    uint Benefit = CurHP;
                    CurHP = MaxHP;
                    World.Action(this, Packets.Status(EntityID, Status.HP, CurHP));
                    //World.Spawn(this);
                    if (AT != MobAttackType.Magic && !IsSkill)
                        World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, Damage, (byte)AT));

                    World.Action(this, Packets.AttackPacket(Attacker.EntityID, EntityID, Loc.X, Loc.Y, 0, (byte)MobAttackType.Kill));

                    Exp = (uint)(Benefit * e); if (!IsSkill)
                    {
                        if (AT == MobAttackType.Ranged || AT == MobAttackType.Melee || AT == MobAttackType.FatalStrike)
                        {
                            if (Attacker.Equips.RightHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.RightHand.ID, 0, 3), Benefit / 100);
                            if (Attacker.Equips.LeftHand.ID != 0)
                                Attacker.AddProfExp((ushort)Game.ItemIDManipulation.Part(Attacker.Equips.LeftHand.ID, 0, 3), Benefit / 300 * 2);
                        }
                    }
                    if (!IsSkill)
                        Attacker.IncreaseExp(Exp, false);
                }
                return Exp;
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); return 0; }
        }
    }
}
