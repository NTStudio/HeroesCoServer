using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroesOnline.PacketHandling
{
    public class Attack
    {
        public static DateTime Attakat;

        public static void Handle(Main.GameClient GC, byte[] Data)
        {
            if ((DateTime.Now > GC.MyChar.AtackTime.AddMilliseconds(600)) && (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55))
            {
            Jumps:
                if (DateTime.Now >= GC.MyChar.lastJumpTime.AddMilliseconds(280))
                {
                    GC.MyChar.lastJumpTime = DateTime.Now;
                }
                else
                    goto Jumps;

                    GC.MyChar.AtackTime = DateTime.Now;
                    try
                    {
                        Extra.Durability.AttackDurability(GC);
                        uint AttackType = BitConverter.ToUInt32(Data, 20);
                        if (AttackType != 24)
                            GC.MyChar.AtkMem.AtkType = (byte)AttackType;
                        GC.MyChar.Action = 100;

                        if (!GC.MyChar.Alive) return;

                        if (AttackType == 2 || AttackType == 28)
                        {
                            
                            uint TargetUID = BitConverter.ToUInt32(Data, 12);
                            try
                            {
                                Game.Character Request = (Game.Character)Game.World.H_Chars[TargetUID];
                                if (DateTime.Now > Request.InvencibleTime.AddMilliseconds(Request.secondeimunity))
                                {
                                    Request.secondeimunity = 200;
                                    Request.InvencibleTime = DateTime.Now;
                                }
                                else
                                    return;
                            }
                            catch { }
                            Game.Mob PossMob = null;
                            Game.Character PossChar = null;


                            if (Game.World.H_Mobs.Contains(GC.MyChar.Loc.Map))
                            {
                                Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[GC.MyChar.Loc.Map];
                                if (MapMobs.Contains(TargetUID))
                                    PossMob = (Game.Mob)MapMobs[TargetUID];
                                else if (Game.World.H_Chars.Contains(TargetUID))
                                    PossChar = (Game.Character)Game.World.H_Chars[TargetUID];
                            }
                            else if (Game.World.H_Chars.Contains(TargetUID))
                                PossChar = (Game.Character)Game.World.H_Chars[TargetUID];

                            if (PossMob != null || (PossChar != null && !Game.World.NoPKMaps.Contains(GC.MyChar.Loc.Map) && PossChar.PKAble(GC.MyChar.PKMode, GC.MyChar)))
                            {
                                GC.MyChar.AtkMem.Target = TargetUID;
                                GC.MyChar.AtkMem.Attacking = true;

                                if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                                {
                                    uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                    if (PossMob != null && PossMob.Alive && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 3 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 15 || GC.MyChar.StatEff.Contains(Game.StatusEffectEn.FatalStrike)))
                                    {
                                        if (!GC.MyChar.WeaponSkill(PossMob.Loc.X, PossMob.Loc.Y, PossMob.EntityID))
                                            PossMob.TakeAttack(GC.MyChar, ref Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                    }
                                    else if (PossChar != null && ((PossChar.StatEff.Contains(Game.StatusEffectEn.Fly) && PossChar.StatEff.Contains(Game.StatusEffectEn.Ride) || !PossChar.StatEff.Contains(Game.StatusEffectEn.Fly)) && !PossChar.StatEff.Contains(Game.StatusEffectEn.Invisible) || GC.MyChar.AtkMem.AtkType != 2) && PossChar.Alive && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 2 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 15))
                                    {
                                        if (!GC.MyChar.WeaponSkill(PossChar.Loc.X, PossChar.Loc.Y, PossChar.EntityID))
                                            PossChar.TakeAttack(GC.MyChar, Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                    }
                                    else
                                    {
                                        GC.MyChar.AtkMem.Target = 0;
                                        GC.MyChar.AtkMem.Attacking = false;
                                    }
                                }
                            }
                            else if (TargetUID >= 6700 && TargetUID <= 6702)
                            {
                                GC.MyChar.AtkMem.Target = TargetUID;
                                GC.MyChar.AtkMem.Attacking = true;

                                if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                                {
                                    uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                    if (TargetUID == 6700)
                                    {
                                        if (Features.GuildWars.War)
                                        {
                                            if (!GC.MyChar.WeaponSkill(Features.GuildWars.ThePole.Loc.X, Features.GuildWars.ThePole.Loc.Y, Features.GuildWars.ThePole.EntityID))
                                                Features.GuildWars.ThePole.TakeAttack(GC.MyChar, Damage, (byte)AttackType);
                                        }
                                        else
                                        {
                                            GC.MyChar.AtkMem.Target = 0;
                                            GC.MyChar.AtkMem.Attacking = false;
                                        }
                                    }
                                    else if (TargetUID == 6701)
                                    {
                                        if (!GC.MyChar.WeaponSkill(Features.GuildWars.TheLeftGate.Loc.X, Features.GuildWars.TheLeftGate.Loc.Y, Features.GuildWars.TheLeftGate.EntityID))
                                            Features.GuildWars.TheLeftGate.TakeAttack(GC.MyChar, Damage, (byte)AttackType);

                                    }
                                    else
                                    {
                                        if (!GC.MyChar.WeaponSkill(Features.GuildWars.TheRightGate.Loc.X, Features.GuildWars.TheRightGate.Loc.Y, Features.GuildWars.TheRightGate.EntityID))
                                            Features.GuildWars.TheRightGate.TakeAttack(GC.MyChar, Damage, (byte)AttackType);
                                    }
                                }
                                return;
                            }
                            else
                                GC.MyChar.AtkMem.Attacking = false;

                            if (PossChar == null && PossMob == null)
                            {
                                Game.NPC PossNPC = (Game.NPC)Game.World.H_NPCs[TargetUID];
                                if ((PossNPC != null && (PossNPC.Loc.Map == 1039)) && PossNPC.Flags == 21 && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 3 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 15))
                                {
                                    GC.MyChar.AtkMem.Target = TargetUID;
                                    GC.MyChar.AtkMem.Attacking = true;

                                    if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                                    {
                                        uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                        if (!GC.MyChar.WeaponSkill(PossNPC.Loc.X, PossNPC.Loc.Y, PossNPC.EntityID))
                                            PossNPC.TakeAttack(GC.MyChar, Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                    }
                                }
                            }
                        }
                        else if (AttackType == 44)
                        {
                            
                            if (GC.MyChar.Stamina >= 50)
                            {
                                ushort SkillId = 6003;
                                ushort x = GC.MyChar.Loc.X;
                                ushort y = GC.MyChar.Loc.Y;
                                uint Target = GC.MyChar.EntityID;
                                foreach (DictionaryEntry DE in Game.World.H_Chars)
                                {
                                    Game.Character Chaar = (Game.Character)DE.Value;
                                    if (Chaar.Name != GC.MyChar.Name)
                                    {
                                        GC.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, Chaar.EntityID, Chaar.Loc.X, Chaar.Loc.Y, 1, 44));
                                    }
                                }
                                GC.MyChar.Stamina -= 50;
                                GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "CounterKill activated");
                                if (SkillId != 0 && GC.MyChar.Skills.Contains(SkillId))
                                {
                                    Game.Skill S = (Game.Skill)GC.MyChar.Skills[SkillId];
                                    if (Features.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                                    {
                                        Features.SkillsClass.SkillUse SU = new HeroesOnline.Features.SkillsClass.SkillUse();
                                        SU.Init(GC.MyChar, S.ID, S.Lvl, (ushort)x, (ushort)y);
                                        if (SU.Info.ID == 0)
                                            return;
                                        SU.GetTargets(Target);
                                        SU.Use();
                                    }
                                }
                                return;
                            }
                            else
                                GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You need stamina");
                        }
                        else if (AttackType == 24)
                        {


                            ushort SkillId = 0, x = 0, y = 0;
                            uint Target;
                            #region SKils
                            SkillId = Convert.ToUInt16(((long)Data[24] & 0xFF) | (((long)Data[25] & 0xFF) << 8));
                            SkillId ^= (ushort)0x915d;
                            SkillId ^= (ushort)GC.MyChar.EntityID;// client.Entity.UID;
                            SkillId = (ushort)(SkillId << 0x3 | SkillId >> 0xd);
                            SkillId -= 0xeb42;
                            #endregion
                            #region GetCoords
                            x = (ushort)((Data[16] & 0xFF) | ((Data[17] & 0xFF) << 8));
                            x = (ushort)(x ^ (uint)(GC.MyChar.EntityID & 0xffff) ^ 0x2ed6);
                            x = (ushort)(((x << 1) | ((x & 0x8000) >> 15)) & 0xffff);
                            x = (ushort)((x | 0xffff0000) - 0xffff22ee);

                            y = (ushort)((Data[18] & 0xFF) | ((Data[19] & 0xFF) << 8));
                            y = (ushort)(y ^ (uint)(GC.MyChar.EntityID & 0xffff) ^ 0xb99b);
                            y = (ushort)(((y << 5) | ((y & 0xF800) >> 11)) & 0xffff);
                            y = (ushort)((y | 0xffff0000) - 0xffff8922);
                            #endregion
                            #region GetTarget
                            Target = ((uint)Data[12] & 0xFF) | (((uint)Data[13] & 0xFF) << 8) | (((uint)Data[14] & 0xFF) << 16) | (((uint)Data[15] & 0xFF) << 24);
                            Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ GC.MyChar.EntityID) - 0x746F4AE6;
                            #endregion

                            #region GemuriEfecte
                            {
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperDragonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperRainbowGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperVioletGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperMoonGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperKylinGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperFuryGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                    }
                                }
                                if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                                if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                                {
                                    if (MyMath.ChanceSuccess(0.3))
                                    {
                                        foreach (DictionaryEntry DE in Game.World.H_Chars)
                                        {
                                            Game.Character Chaar = (Game.Character)DE.Value;
                                            if (Chaar.Name != GC.MyChar.Name)
                                            {
                                                Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                            }
                                        }
                                        GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                    }
                                }
                            }
                            #endregion
                            if (GC.MyChar.Equips.RightHand.Effect == HeroesOnline.Game.Item.RebornEffect.HP)//HpBS
                            {
                                //  MyMath.ChanceSuccess(1000000);
                                GC.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "magicfull"));
                                GC.MyChar.CurHP += 310;
                                if (GC.MyChar.CurMP > GC.MyChar.MaxMP)
                                    GC.MyChar.CurMP = GC.MyChar.MaxMP;
                            }
                            else if (GC.MyChar.Equips.RightHand.Effect == HeroesOnline.Game.Item.RebornEffect.MP)//ManaBS
                            {
                                // MyMath.ChanceSuccess(1000000);
                                GC.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "magicfull"));
                                GC.MyChar.CurMP += 310;
                                if (GC.MyChar.CurMP > GC.MyChar.MaxMP)
                                    GC.MyChar.CurMP = GC.MyChar.MaxMP;
                            }
                            if (SkillId != 0 && GC.MyChar.Skills.Contains(SkillId))
                            {
                                Game.Skill S = (Game.Skill)GC.MyChar.Skills[SkillId];
                                if (Features.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                                {
                                    Features.SkillsClass.SkillUse SU = new HeroesOnline.Features.SkillsClass.SkillUse();
                                    SU.Init(GC.MyChar, S.ID, S.Lvl, (ushort)x, (ushort)y);
                                    if (SU.Info.ID == 0)
                                        return;
                                    bool EnoughArrows = true;
                                    if (SU.Info.ArrowsCost > 0)
                                    {
                                        if (GC.MyChar.Loc.Map != 1039)
                                        {
                                            if (GC.MyChar.Equips.LeftHand.ID != 0 && Game.Item.IsArrow(GC.MyChar.Equips.LeftHand.ID))
                                            {
                                                if (GC.MyChar.Equips.LeftHand.CurDur >= SU.Info.ArrowsCost)
                                                    GC.MyChar.Equips.LeftHand.CurDur -= SU.Info.ArrowsCost;
                                                else
                                                    GC.MyChar.Equips.LeftHand.CurDur = 0;
                                                if (GC.MyChar.Equips.LeftHand.CurDur == 0)
                                                {
                                                    GC.SendPacket(Packets.ItemPacket(GC.MyChar.Equips.LeftHand.UID, 5, 6));
                                                    GC.SendPacket(Packets.ItemPacket(GC.MyChar.Equips.LeftHand.UID, 0, 3));
                                                    GC.MyChar.Equips.LeftHand = new Game.Item();
                                                }
                                                else
                                                    GC.SendPacket(Packets.AddItem(GC.MyChar.Equips.LeftHand, 5));
                                            }
                                            else
                                            {
                                                GC.MyChar.AtkMem.Attacking = false;
                                                EnoughArrows = false;
                                            }
                                        }
                                    }
                                    if (GC.MyChar.CurMP >= SU.Info.ManaCost && GC.MyChar.Stamina >= SU.Info.StaminaCost && EnoughArrows || GC.MyChar.Loc.Map == 1039)
                                    {
                                        if (SU.Info.EndsXPWait)
                                        {
                                            if (GC.MyChar.StatEff.Contains(HeroesOnline.Game.StatusEffectEn.XPStart))
                                            {
                                                GC.MyChar.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.XPStart);
                                                GC.MyChar.XPKO = 0;
                                            }
                                            else
                                                return;
                                        }
                                        try
                                        {
                                            if (GC.MyChar.Loc.Map != 1039)
                                            {
                                                GC.MyChar.CurMP -= SU.Info.ManaCost;
                                                if (SU.Info.ID == 1045 || SU.Info.ID == 1046)
                                                    GC.MyChar.Stamina -= 14;
                                                else
                                                    GC.MyChar.Stamina -= SU.Info.StaminaCost;
                                                if (SkillId >= 1000 && SkillId <= 1002)
                                                {
                                                    GC.MyChar.AtkMem.AtkType = 21;
                                                    GC.MyChar.AtkMem.Skill = SU.Info.ID;
                                                    GC.MyChar.AtkMem.Attacking = true;
                                                    GC.MyChar.AtkMem.Target = Target;
                                                    GC.MyChar.AtkMem.LastAttack = DateTime.Now;
                                                    GC.MyChar.AtkMem.SX = (ushort)x;
                                                    GC.MyChar.AtkMem.SY = (ushort)y;
                                                }
                                            }
                                            else
                                            {
                                                GC.MyChar.AtkMem.AtkType = 21;
                                                GC.MyChar.AtkMem.Skill = SU.Info.ID;
                                                GC.MyChar.AtkMem.Attacking = true;
                                                GC.MyChar.AtkMem.Target = Target;
                                                GC.MyChar.AtkMem.LastAttack = DateTime.Now;
                                                GC.MyChar.AtkMem.SX = (ushort)x;
                                                GC.MyChar.AtkMem.SY = (ushort)y;
                                            }
                                        }
                                        catch (Exception c) { Program.WriteLine(c.ToString()); }

                                        if (SU.Info.ID == 7003)
                                            return;
                                        
                                       else if (SU.Info.ID == 6000)
                                        {
                                            #region Players
                                            try
                                            {
                                                if (GC.MyChar.EntityID != Target)
                                                {
                                                    if (Game.World.H_Chars.ContainsKey(Target))
                                                    {
                                                        Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                        //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                        if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player))
                                                        {
                                                            Console.WriteLine("lol");
                                                            if (Player != GC.MyChar)
                                                            {
                                                                //int Dst = 0;
                                                                if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) == 1)
                                                                //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                                {
                                                                    if (!Game.World.H_NPCs.Contains(Target))
                                                                    {
                                                                        SU.GetTargets(Target);
                                                                        SU.Use();
                                                                    }
                                                                }
                                                                // if ((Player.Potency + 5) <= GC.MyChar.Potency)
                                                                //  {
                                                                //Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                                // Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                                //Player.TakeAttack(GC.MyChar, 0, HeroesOnline.Game.AttackType.Magic, true);
                                                                //SU.PlayerTargets.Add(Player, (uint)1);
                                                            }
                                                            //else
                                                            // {
                                                            //     SU.PlayerTargets.Add(Player, (uint)0);
                                                            // }
                                                        }
                                                    }
                                                    else if (!Game.World.H_NPCs.Contains(Target))
                                                    {
                                                        SU.GetTargets(Target);
                                                        SU.Use();
                                                        GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                    }
                                                  
                                                }
                                            }



                                            catch { }
                                            #endregion
                                        }
                                        else if (SU.Info.ID == 1115)
                                        {
                                            #region Players
                                            try
                                            {
                                                if (GC.MyChar.EntityID != Target)
                                                {
                                                    if (Game.World.H_Chars.ContainsKey(Target))
                                                    {
                                                        Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                        //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                        if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player))
                                                        {
                                                            if (Player != GC.MyChar)
                                                            {
                                                                //int Dst = 0;

                                                                if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) <= 4)
                                                                {    //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)



                                                                    if (!Game.World.H_NPCs.Contains(Target))
                                                                    {
                                                                        SU.GetTargets(Target);
                                                                        SU.Use();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (!Game.World.H_NPCs.Contains(Target))
                                                    {
                                                        SU.GetTargets(Target);
                                                        SU.Use();
                                                        GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                    }

                                                }
                                            }



                                            catch { }
                                            #endregion
                                        }
                                        else if (SU.Info.ID == 6002)
                                        {
                                            #region Players
                                            try
                                            {
                                                if (GC.MyChar.EntityID != Target)
                                                {
                                                    if (Game.World.H_Chars.ContainsKey(Target))
                                                    {
                                                        Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                        //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                        if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player) && GC.MyChar.Loc.MapCopy == Player.Loc.MapCopy)
                                                        {
                                                            Console.WriteLine("lol");
                                                            if (Player != GC.MyChar)
                                                            {
                                                                //int Dst = 0;
                                                                //if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) == 1)
                                                                //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                                {
                                                                    if (!Game.World.H_NPCs.Contains(Target))
                                                                    {
                                                                        SU.GetTargets(Target);
                                                                        SU.Use();
                                                                    }
                                                                }
                                                                // if ((Player.Potency + 5) <= GC.MyChar.Potency)
                                                                //  {
                                                                //Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                                // Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                                //Player.TakeAttack(GC.MyChar, 0, HeroesOnline.Game.AttackType.Magic, true);
                                                                SU.PlayerTargets.Add(Player, (uint)1);
                                                            }
                                                            else
                                                             {
                                                                 SU.PlayerTargets.Add(Player, (uint)0);
                                                             }
                                                        }
                                                    }
                                                    else if (!Game.World.H_NPCs.Contains(Target))
                                                    {
                                                        SU.GetTargets(Target);
                                                        SU.Use();
                                                        GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                    }

                                                }
                                            }



                                            catch { }
                                            #endregion
                                        }
                                        else if (SU.Info.ID == 6001)//toxic fog
                                        {
                                            #region ToxicFog
                                            if (GC.MyChar.Loc.Map != 1039)
                                            {
                                                #region Mobs
                                                try
                                                {
                                                    foreach (Game.Mob Mob in (Game.World.H_Mobs[GC.MyChar.Loc.Map] as Hashtable).Values)
                                                    {
                                                        if (Mob.Alive)
                                                        {
                                                            int Dst = 6;
                                                            if (MyMath.PointDistance(Mob.Loc.X, Mob.Loc.Y, x, y) <= Dst && GC.MyChar.Loc.MapCopy == Mob.Loc.MapCopy)
                                                            {
                                                               // if (Mob.Name.Contains("TeratoDragon"))
                                                                //{ return; }
                                                                if (Mob.Name.Contains("Guard") || Mob.Name.Contains("TeratoDragon") || Mob.Name.Contains("Millennium.Dragon"))
                                                                {
                                                                    continue;
                                                                }
                                                                Mob.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                                foreach (Game.Character C in Game.World.H_Chars.Values)
                                                                    if (Mob.Loc.Map == C.Loc.Map)
                                                                        if (MyMath.PointDistance(Mob.Loc.X, Mob.Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                                                                            C.MyClient.SendPacket(Packets.Status(Mob.EntityID, HeroesOnline.Game.Status.Effect, (ulong)Game.StatusEffectEn.Poisoned));
                                                                uint Damage = 0;
                                                                Mob.TakeAttack(GC.MyChar, ref Damage, HeroesOnline.Game.AttackType.Magic, true);
                                                                GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                            }
                                                        }
                                                    }
                                                }
                                                catch { }
                                                #endregion
                                                #region Players
                                                try
                                                {
                                                    if (!Game.World.NoPKMaps.Contains(GC.MyChar.Loc.Map))
                                                    {
                                                        foreach (Game.Character Player in Game.World.H_Chars.Values)
                                                        {
                                                            if (Player.Alive)
                                                            {
                                                                int Dst = 6;
                                                                if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                                {
                                                                    if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player) && GC.MyChar.Loc.MapCopy == Player.Loc.MapCopy)
                                                                    {
                                                                        if (Player != GC.MyChar)
                                                                        {
                                                                            if (Player.Potency <= GC.MyChar.Potency || GC.MyChar.Name == "E.V.E" || (Player.Potency - 3 <= GC.MyChar.Potency && MyMath.ChanceSuccess(25)) || (Player.Potency - 2 <= GC.MyChar.Potency && MyMath.ChanceSuccess(40)))
                                                                            {
                                                                                Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                                                Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                                                Player.TakeAttack(GC.MyChar, 100, HeroesOnline.Game.AttackType.Magic, true);

                                                                                SU.PlayerTargets.Add(Player, (uint)1);
                                                                                //  int DamagePercent = 10 + Player.PoisonedInfo.SpellLevel * 10;
                                                                                //   Player.CurHP -= (ushort)(Player.CurHP * (DamagePercent) / 50);
                                                                            }
                                                                            else
                                                                            {
                                                                                SU.PlayerTargets.Add(Player, (uint)0);
                                                                            }
                                                                        }
                                                                    }
                                                                    GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch { }
                                                #endregion
                                            }
                                            else
                                                GC.MyChar.AddSkillExp(SU.Info.ID, 10);
                                            //Game.World.Action(this, Packets.GeneralData(EntityID, X, Y, 0, 0, Direction, 137));
                                            Game.World.Action(GC.MyChar, Packets.SkillUse(SU));
                                            #endregion
                                        }
                                        else if (SU.Info.ID == 1100 || SU.Info.ID == 1050)
                                        {
                                            #region Pray
                                            if (GC.MyChar.EntityID != Target)
                                            {
                                                if (Game.World.H_Chars.ContainsKey(Target))
                                                {
                                                    Game.Character Char = (Game.Character)Game.World.H_Chars[Target];
                                                    if (!Char.Alive)
                                                    {
                                                        SU.GetTargets(Target);
                                                        SU.PlayerTargets[Char] = (uint)1;
                                                        Game.World.Action(Char, Packets.SkillUse(SU));
                                                        Char.Ghost = false;
                                                        Char.BlueName = false;
                                                        Char.CurHP = (ushort)Char.MaxHP;
                                                        Char.Alive = true;
                                                        Char.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.Dead);
                                                        Char.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.BlueName);
                                                        Char.Body = Char.Body;
                                                        Char.Hair = Char.Hair;
                                                        Char.Equips.Send(Char.MyClient, false);
                                                        Char.secondeimunity = 10;
                                                        Char.InvencibleTime = DateTime.Now;
                                                       Char.MyClient.SendPacket(Packets.MapStatus(Char.Loc.Map, 0));
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            if (!Game.World.H_NPCs.Contains(Target))
                                            {
                                                SU.GetTargets(Target);
                                                SU.Use();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception e) { Program.WriteLine(e); }
                
            }
            else
            {
            Jumps:
                if (DateTime.Now >= GC.MyChar.lastJumpTime.AddMilliseconds(280))
                {
                    GC.MyChar.lastJumpTime = DateTime.Now;
                }
                else
                    goto Jumps;

                GC.MyChar.AtackTime = DateTime.Now;
                try
                {
                    Extra.Durability.AttackDurability(GC);
                    uint AttackType = BitConverter.ToUInt32(Data, 20);
                    if (AttackType != 24)
                        GC.MyChar.AtkMem.AtkType = (byte)AttackType;
                    GC.MyChar.Action = 100;

                    if (!GC.MyChar.Alive) return;

                    if (AttackType == 2 || AttackType == 28)
                    {
                        
                        uint TargetUID = BitConverter.ToUInt32(Data, 12);
                        try
                        {
                            Game.Character Request = (Game.Character)Game.World.H_Chars[TargetUID];
                            if (DateTime.Now > Request.InvencibleTime.AddSeconds(Request.secondeimunity))
                            {
                                Request.secondeimunity = 0;
                                Request.InvencibleTime = DateTime.Now;
                            }
                            else
                                return;
                        }
                        catch { }
                        Game.Mob PossMob = null;
                        Game.Character PossChar = null;


                        if (Game.World.H_Mobs.Contains(GC.MyChar.Loc.Map))
                        {
                            Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[GC.MyChar.Loc.Map];
                            if (MapMobs.Contains(TargetUID))
                                PossMob = (Game.Mob)MapMobs[TargetUID];
                            else if (Game.World.H_Chars.Contains(TargetUID))
                                PossChar = (Game.Character)Game.World.H_Chars[TargetUID];
                        }
                        else if (Game.World.H_Chars.Contains(TargetUID))
                            PossChar = (Game.Character)Game.World.H_Chars[TargetUID];

                        if (PossMob != null || PossChar != null)
                        {
                            GC.MyChar.AtkMem.Target = TargetUID;
                            GC.MyChar.AtkMem.Attacking = true;

                            if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                            {
                                uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                if (PossMob != null && PossMob.Alive && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 3 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossMob.Loc.X, PossMob.Loc.Y) <= 15 || GC.MyChar.StatEff.Contains(Game.StatusEffectEn.FatalStrike)))
                                {
                                    if (!GC.MyChar.WeaponSkill(PossMob.Loc.X, PossMob.Loc.Y, PossMob.EntityID))
                                        PossMob.TakeAttack(GC.MyChar, ref Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                }
                                else if (PossChar != null && ((PossChar.StatEff.Contains(Game.StatusEffectEn.Fly) && PossChar.StatEff.Contains(Game.StatusEffectEn.Ride) || !PossChar.StatEff.Contains(Game.StatusEffectEn.Fly)) && !PossChar.StatEff.Contains(Game.StatusEffectEn.Invisible) || GC.MyChar.AtkMem.AtkType != 2) && PossChar.Alive && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 2 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossChar.Loc.X, PossChar.Loc.Y) <= 15))
                                {
                                    if (!GC.MyChar.WeaponSkill(PossChar.Loc.X, PossChar.Loc.Y, PossChar.EntityID))
                                        PossChar.TakeAttack(GC.MyChar, Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                }
                                else
                                {
                                    GC.MyChar.AtkMem.Target = 0;
                                    GC.MyChar.AtkMem.Attacking = false;
                                }
                            }
                        }
                        else if (TargetUID >= 6700 && TargetUID <= 6702)
                        {
                            GC.MyChar.AtkMem.Target = TargetUID;
                            GC.MyChar.AtkMem.Attacking = true;

                            if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                            {
                                uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                if (TargetUID == 6700)
                                {
                                    if (Features.GuildWars.War)
                                    {
                                        if (!GC.MyChar.WeaponSkill(Features.GuildWars.ThePole.Loc.X, Features.GuildWars.ThePole.Loc.Y, Features.GuildWars.ThePole.EntityID))
                                            Features.GuildWars.ThePole.TakeAttack(GC.MyChar, Damage, (byte)AttackType);
                                    }
                                    else
                                    {
                                        GC.MyChar.AtkMem.Target = 0;
                                        GC.MyChar.AtkMem.Attacking = false;
                                    }
                                }
                                else if (TargetUID == 6701)
                                {
                                    if (!GC.MyChar.WeaponSkill(Features.GuildWars.TheLeftGate.Loc.X, Features.GuildWars.TheLeftGate.Loc.Y, Features.GuildWars.TheLeftGate.EntityID))
                                        Features.GuildWars.TheLeftGate.TakeAttack(GC.MyChar, Damage, (byte)AttackType);

                                }
                                else
                                {
                                    if (!GC.MyChar.WeaponSkill(Features.GuildWars.TheRightGate.Loc.X, Features.GuildWars.TheRightGate.Loc.Y, Features.GuildWars.TheRightGate.EntityID))
                                        Features.GuildWars.TheRightGate.TakeAttack(GC.MyChar, Damage, (byte)AttackType);
                                }
                            }
                            return;
                        }
                        else
                            GC.MyChar.AtkMem.Attacking = false;

                        if (PossChar == null && PossMob == null)
                        {
                            Game.NPC PossNPC = (Game.NPC)Game.World.H_NPCs[TargetUID];
                            if (PossNPC != null && PossNPC.Flags == 21 && (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 3 || AttackType == 28 && MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, PossNPC.Loc.X, PossNPC.Loc.Y) <= 15))
                            {
                                GC.MyChar.AtkMem.Target = TargetUID;
                                GC.MyChar.AtkMem.Attacking = true;

                                if (DateTime.Now > GC.MyChar.AtkMem.LastAttack.AddMilliseconds(GC.MyChar.AtkFrequence))
                                {
                                    uint Damage = GC.MyChar.PrepareAttack((byte)AttackType, true);
                                    if (!GC.MyChar.WeaponSkill(PossNPC.Loc.X, PossNPC.Loc.Y, PossNPC.EntityID))
                                        PossNPC.TakeAttack(GC.MyChar, Damage, (HeroesOnline.Game.AttackType)AttackType, false);
                                }
                            }
                        }
                    }
                    else if (AttackType == 44)
                    {
                        if (GC.MyChar.Stamina >= 50)
                        {
                            ushort SkillId = 6003;
                            ushort x = GC.MyChar.Loc.X;
                            ushort y = GC.MyChar.Loc.Y;
                            uint Target = GC.MyChar.EntityID;
                            foreach (DictionaryEntry DE in Game.World.H_Chars)
                            {
                                Game.Character Chaar = (Game.Character)DE.Value;
                                if (Chaar.Name != GC.MyChar.Name)
                                {
                                    GC.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, Chaar.EntityID, Chaar.Loc.X, Chaar.Loc.Y, 1, 44));
                                }
                            }
                            GC.MyChar.Stamina -= 50;
                            GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "CounterKill activated");
                            if (SkillId != 0 && GC.MyChar.Skills.Contains(SkillId))
                            {
                                Game.Skill S = (Game.Skill)GC.MyChar.Skills[SkillId];
                                if (Features.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                                {
                                    Features.SkillsClass.SkillUse SU = new HeroesOnline.Features.SkillsClass.SkillUse();
                                    SU.Init(GC.MyChar, S.ID, S.Lvl, (ushort)x, (ushort)y);
                                    if (SU.Info.ID == 0)
                                        return;
                                    SU.GetTargets(Target);
                                    SU.Use();
                                }
                            }
                            return;
                        }
                        else
                            GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You need stamina");
                    }
                    else if (AttackType == 24)
                    {


                        ushort SkillId = 0, x = 0, y = 0;
                        uint Target;
                        #region SKils
                        SkillId = Convert.ToUInt16(((long)Data[24] & 0xFF) | (((long)Data[25] & 0xFF) << 8));
                        SkillId ^= (ushort)0x915d;
                        SkillId ^= (ushort)GC.MyChar.EntityID;// client.Entity.UID;
                        SkillId = (ushort)(SkillId << 0x3 | SkillId >> 0xd);
                        SkillId -= 0xeb42;
                        #endregion
                        #region GetCoords
                        x = (ushort)((Data[16] & 0xFF) | ((Data[17] & 0xFF) << 8));
                        x = (ushort)(x ^ (uint)(GC.MyChar.EntityID & 0xffff) ^ 0x2ed6);
                        x = (ushort)(((x << 1) | ((x & 0x8000) >> 15)) & 0xffff);
                        x = (ushort)((x | 0xffff0000) - 0xffff22ee);

                        y = (ushort)((Data[18] & 0xFF) | ((Data[19] & 0xFF) << 8));
                        y = (ushort)(y ^ (uint)(GC.MyChar.EntityID & 0xffff) ^ 0xb99b);
                        y = (ushort)(((y << 5) | ((y & 0xF800) >> 11)) & 0xffff);
                        y = (ushort)((y | 0xffff0000) - 0xffff8922);
                        #endregion
                        #region GetTarget
                        Target = ((uint)Data[12] & 0xFF) | (((uint)Data[13] & 0xFF) << 8) | (((uint)Data[14] & 0xFF) << 16) | (((uint)Data[15] & 0xFF) << 24);
                        Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ GC.MyChar.EntityID) - 0x746F4AE6;
                        #endregion

                        #region GemuriEfecte
                        {
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperDragonGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperDragonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldendragon"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperPhoenixGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperPhoenixGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "phoenix"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperRainbowGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperRainbowGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "rainbow"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperVioletGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperVioletGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "purpleray"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperMoonGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperMoonGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "moon"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperKylinGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperKylinGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "goldenkylin"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperFuryGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperFuryGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "fastflash"));
                                }
                            }
                            if (GC.MyChar.Equips.HeadGear.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.HeadGear.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.Necklace.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Necklace.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.Ring.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Ring.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.LeftHand.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.LeftHand.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.Armor.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Armor.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.RightHand.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.RightHand.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                            if (GC.MyChar.Equips.Boots.Soc1 == Game.Item.Gem.SuperTortoiseGem || GC.MyChar.Equips.Boots.Soc2 == Game.Item.Gem.SuperTortoiseGem)
                            {
                                if (MyMath.ChanceSuccess(0.3))
                                {
                                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                                    {
                                        Game.Character Chaar = (Game.Character)DE.Value;
                                        if (Chaar.Name != GC.MyChar.Name)
                                        {
                                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));

                                        }
                                    }
                                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "recovery"));
                                }
                            }
                        }
                        #endregion
                        if (GC.MyChar.Equips.RightHand.Effect == HeroesOnline.Game.Item.RebornEffect.HP)//HpBS
                        {
                            //  MyMath.ChanceSuccess(1000000);
                            GC.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "magicfull"));
                            GC.MyChar.CurHP += 310;
                            if (GC.MyChar.CurMP > GC.MyChar.MaxMP)
                                GC.MyChar.CurMP = GC.MyChar.MaxMP;
                        }
                        else if (GC.MyChar.Equips.RightHand.Effect == HeroesOnline.Game.Item.RebornEffect.MP)//ManaBS
                        {
                            // MyMath.ChanceSuccess(1000000);
                            GC.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "magicfull"));
                            GC.MyChar.CurMP += 310;
                            if (GC.MyChar.CurMP > GC.MyChar.MaxMP)
                                GC.MyChar.CurMP = GC.MyChar.MaxMP;
                        }
                        if (SkillId != 0 && GC.MyChar.Skills.Contains(SkillId))
                        {
                            Game.Skill S = (Game.Skill)GC.MyChar.Skills[SkillId];
                            if (Features.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
                            {
                                Features.SkillsClass.SkillUse SU = new HeroesOnline.Features.SkillsClass.SkillUse();
                                SU.Init(GC.MyChar, S.ID, S.Lvl, (ushort)x, (ushort)y);
                                if (SU.Info.ID == 0)
                                    return;
                                bool EnoughArrows = true;
                                if (SU.Info.ArrowsCost > 0)
                                {
                                    if (GC.MyChar.Loc.Map != 1039)
                                    {
                                        if (GC.MyChar.Equips.LeftHand.ID != 0 && Game.Item.IsArrow(GC.MyChar.Equips.LeftHand.ID))
                                        {
                                            if (GC.MyChar.Equips.LeftHand.CurDur >= SU.Info.ArrowsCost)
                                                GC.MyChar.Equips.LeftHand.CurDur -= SU.Info.ArrowsCost;
                                            else
                                                GC.MyChar.Equips.LeftHand.CurDur = 0;
                                            if (GC.MyChar.Equips.LeftHand.CurDur == 0)
                                            {
                                                GC.SendPacket(Packets.ItemPacket(GC.MyChar.Equips.LeftHand.UID, 5, 6));
                                                GC.SendPacket(Packets.ItemPacket(GC.MyChar.Equips.LeftHand.UID, 0, 3));
                                                GC.MyChar.Equips.LeftHand = new Game.Item();
                                            }
                                            else
                                                GC.SendPacket(Packets.AddItem(GC.MyChar.Equips.LeftHand, 5));
                                        }
                                        else
                                        {
                                            GC.MyChar.AtkMem.Attacking = false;
                                            EnoughArrows = false;
                                        }
                                    }
                                }
                                if (GC.MyChar.CurMP >= SU.Info.ManaCost && GC.MyChar.Stamina >= SU.Info.StaminaCost && EnoughArrows || GC.MyChar.Loc.Map == 1039)
                                {
                                    if (SU.Info.EndsXPWait)
                                    {
                                        if (GC.MyChar.StatEff.Contains(HeroesOnline.Game.StatusEffectEn.XPStart))
                                        {
                                            GC.MyChar.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.XPStart);
                                            GC.MyChar.XPKO = 0;
                                        }
                                        else
                                            return;
                                    }
                                    try
                                    {
                                        if (GC.MyChar.Loc.Map != 1039)
                                        {
                                            if (SU.Info.ID == 1045 || SU.Info.ID == 1046)
                                                GC.MyChar.Stamina -= 14;
                                            else
                                                GC.MyChar.Stamina -= SU.Info.StaminaCost;
                                            if (SkillId >= 1000 && SkillId <= 1002)
                                            {
                                                GC.MyChar.AtkMem.AtkType = 21;
                                                GC.MyChar.AtkMem.Skill = SU.Info.ID;
                                                GC.MyChar.AtkMem.Attacking = true;
                                                GC.MyChar.AtkMem.Target = Target;
                                                GC.MyChar.AtkMem.LastAttack = DateTime.Now;
                                                GC.MyChar.AtkMem.SX = (ushort)x;
                                                GC.MyChar.AtkMem.SY = (ushort)y;
                                            }
                                        }
                                        else
                                        {
                                            GC.MyChar.AtkMem.AtkType = 21;
                                            GC.MyChar.AtkMem.Skill = SU.Info.ID;
                                            GC.MyChar.AtkMem.Attacking = true;
                                            GC.MyChar.AtkMem.Target = Target;
                                            GC.MyChar.AtkMem.LastAttack = DateTime.Now;
                                            GC.MyChar.AtkMem.SX = (ushort)x;
                                            GC.MyChar.AtkMem.SY = (ushort)y;
                                        }
                                    }
                                    catch (Exception c) { Program.WriteLine(c.ToString()); }

                                    if (SU.Info.ID == 7003)
                                        return;
                                    
                                    else if (SU.Info.ID == 6000)
                                    {
                                        #region Players
                                        try
                                        {
                                            if (GC.MyChar.EntityID != Target)
                                            {
                                                if (Game.World.H_Chars.ContainsKey(Target))
                                                {
                                                    Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                    //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                    if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player))
                                                    {
                                                        Console.WriteLine("lol");
                                                        if (Player != GC.MyChar)
                                                        {
                                                            //int Dst = 0;
                                                            if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) == 1)
                                                            //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                            {
                                                                if (!Game.World.H_NPCs.Contains(Target))
                                                                {
                                                                    SU.GetTargets(Target);
                                                                    SU.Use();
                                                                }
                                                            }
                                                            // if ((Player.Potency + 5) <= GC.MyChar.Potency)
                                                            //  {
                                                            //Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                            // Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                            //Player.TakeAttack(GC.MyChar, 0, HeroesOnline.Game.AttackType.Magic, true);
                                                            //SU.PlayerTargets.Add(Player, (uint)1);
                                                        }
                                                        //else
                                                        // {
                                                        //     SU.PlayerTargets.Add(Player, (uint)0);
                                                        // }
                                                    }
                                                }
                                                else if (!Game.World.H_NPCs.Contains(Target))
                                                {
                                                    SU.GetTargets(Target);
                                                    SU.Use();
                                                    GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                }

                                            }
                                        }



                                        catch { }
                                        #endregion
                                    }
                                    else if (SU.Info.ID == 1115)
                                    {
                                        #region Players
                                        try
                                        {
                                            if (GC.MyChar.EntityID != Target)
                                            {
                                                if (Game.World.H_Chars.ContainsKey(Target))
                                                {
                                                    Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                    //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                    if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player))
                                                    {
                                                        if (Player != GC.MyChar)
                                                        {
                                                            //int Dst = 0;

                                                            if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) <= 4)
                                                            {    //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)



                                                                if (!Game.World.H_NPCs.Contains(Target))
                                                                {
                                                                    SU.GetTargets(Target);
                                                                    SU.Use();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (!Game.World.H_NPCs.Contains(Target))
                                                {
                                                    SU.GetTargets(Target);
                                                    SU.Use();
                                                    GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                }

                                            }
                                        }



                                        catch { }
                                        #endregion
                                    }
                                    else if (SU.Info.ID == 6002)
                                    {
                                        #region Players
                                        try
                                        {
                                            if (GC.MyChar.EntityID != Target)
                                            {
                                                if (Game.World.H_Chars.ContainsKey(Target))
                                                {
                                                    Game.Character Player = (Game.Character)Game.World.H_Chars[Target];
                                                    //     if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                    if (GC.MyChar.PKAble(GC.MyChar.PKMode, Player))
                                                    {
                                                        Console.WriteLine("lol");
                                                        if (Player != GC.MyChar)
                                                        {
                                                            //int Dst = 0;
                                                            if ((Player.Loc.X - GC.MyChar.Loc.X) == 1 || (Player.Loc.Y - GC.MyChar.Loc.Y) == 1 || (GC.MyChar.Loc.X - Player.Loc.X) == 1 || (GC.MyChar.Loc.Y - Player.Loc.Y) == 1)
                                                            //if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                            {
                                                                if (!Game.World.H_NPCs.Contains(Target))
                                                                {
                                                                    SU.GetTargets(Target);
                                                                    SU.Use();
                                                                }
                                                            }
                                                            // if ((Player.Potency + 5) <= GC.MyChar.Potency)
                                                            //  {
                                                            //Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                            // Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                            //Player.TakeAttack(GC.MyChar, 0, HeroesOnline.Game.AttackType.Magic, true);
                                                            //SU.PlayerTargets.Add(Player, (uint)1);
                                                        }
                                                        //else
                                                        // {
                                                        //     SU.PlayerTargets.Add(Player, (uint)0);
                                                        // }
                                                    }
                                                }
                                                else if (!Game.World.H_NPCs.Contains(Target))
                                                {
                                                    SU.GetTargets(Target);
                                                    SU.Use();
                                                    GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                }

                                            }
                                        }



                                        catch { }
                                        #endregion
                                    }
                                    else if (SU.Info.ID == 6001)//toxic fog
                                    {
                                        #region ToxicFog
                                        if (GC.MyChar.Loc.Map != 1039)
                                        {
                                            #region Mobs
                                            try
                                            {
                                                foreach (Game.Mob Mob in (Game.World.H_Mobs[GC.MyChar.Loc.Map] as Hashtable).Values)
                                                {
                                                    if (Mob.Alive)
                                                    {
                                                        int Dst = 6;
                                                        if (MyMath.PointDistance(Mob.Loc.X, Mob.Loc.Y, x, y) <= Dst)
                                                        {
                                                            if (Mob.Name.Contains("TeratoDragon"))
                                                            { return; }
                                                            if (Mob.Name.Contains("Guard"))
                                                            {
                                                                continue;
                                                            }
                                                            Mob.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                            foreach (Game.Character C in Game.World.H_Chars.Values)
                                                                if (Mob.Loc.Map == C.Loc.Map)
                                                                    if (MyMath.PointDistance(Mob.Loc.X, Mob.Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                                                                        C.MyClient.SendPacket(Packets.Status(Mob.EntityID, HeroesOnline.Game.Status.Effect, (ulong)Game.StatusEffectEn.Poisoned));
                                                            uint Damage = 0;
                                                            Mob.TakeAttack(GC.MyChar, ref Damage, HeroesOnline.Game.AttackType.Magic, true);
                                                            GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                            #endregion
                                            #region Players
                                            try
                                            {
                                                if (!Game.World.NoPKMaps.Contains(GC.MyChar.Loc.Map))
                                                {
                                                    foreach (Game.Character Player in Game.World.H_Chars.Values)
                                                    {
                                                        if (Player.Alive)
                                                        {
                                                            int Dst = 6;
                                                            if (MyMath.PointDistance(Player.Loc.X, Player.Loc.Y, x, y) <= Dst)
                                                            {
                                                                if (Player.PKAble(GC.MyChar.PKMode, GC.MyChar))
                                                                {
                                                                    //if (GC.MyChar.PKMode == HeroesOnline.Game.PKMode.PK)
                                                                    //{
                                                                        if (Player != GC.MyChar)
                                                                        {
                                                                            if (Player.Potency <= GC.MyChar.Potency || (Player.Potency - 3 <= GC.MyChar.Potency && MyMath.ChanceSuccess(25)) || (Player.Potency - 2 <= GC.MyChar.Potency && MyMath.ChanceSuccess(40)))
                                                                            {
                                                                                Player.PoisonedInfo = new HeroesOnline.Game.PoisonType(SU.Info.Level);
                                                                                Player.StatEff.Add(HeroesOnline.Game.StatusEffectEn.Poisoned);
                                                                                Player.TakeAttack(GC.MyChar, 0, HeroesOnline.Game.AttackType.Magic, true);

                                                                                SU.PlayerTargets.Add(Player, (uint)1);
                                                                                //  int DamagePercent = 10 + Player.PoisonedInfo.SpellLevel * 10;
                                                                                //   Player.CurHP -= (ushort)(Player.CurHP * (DamagePercent) / 50);
                                                                            }
                                                                            else
                                                                            {
                                                                                SU.PlayerTargets.Add(Player, (uint)0);
                                                                            }
                                                                        }
                                                                    //}
                                                                    GC.MyChar.AddSkillExp(SU.Info.ID, 100);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                            #endregion
                                        }
                                        else
                                            GC.MyChar.AddSkillExp(SU.Info.ID, 10);
                                        //Game.World.Action(this, Packets.GeneralData(EntityID, X, Y, 0, 0, Direction, 137));
                                        Game.World.Action(GC.MyChar, Packets.SkillUse(SU));
                                        #endregion
                                    }
                                    else if (SU.Info.ID == 1100 || SU.Info.ID == 1050)
                                    {
                                        #region Pray
                                        if (GC.MyChar.EntityID != Target && GC.MyChar.Loc.Map != 1090)
                                        {
                                            if (Game.World.H_Chars.ContainsKey(Target))
                                            {
                                                Game.Character Char = (Game.Character)Game.World.H_Chars[Target];
                                                if (!Char.Alive)
                                                {
                                                    SU.GetTargets(Target);
                                                    SU.PlayerTargets[Char] = (uint)1;
                                                    Game.World.Action(Char, Packets.SkillUse(SU));
                                                    Char.Ghost = false;
                                                    Char.BlueName = false;
                                                    Char.CurHP = (ushort)Char.MaxHP;
                                                    Char.Alive = true;
                                                    Char.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.Dead);
                                                    Char.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.BlueName);
                                                    Char.Body = Char.Body;
                                                    Char.Hair = Char.Hair;
                                                    Char.Equips.Send(Char.MyClient, false);
                                                    Char.secondeimunity = 10;
                                                    Char.InvencibleTime = DateTime.Now;
                                                    Char.MyClient.SendPacket(Packets.MapStatus(Char.Loc.Map, 0));
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (!Game.World.H_NPCs.Contains(Target))
                                        {
                                            SU.GetTargets(Target);
                                            SU.Use();
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception e) { Program.WriteLine(e); }

            }
        }
    }
}
