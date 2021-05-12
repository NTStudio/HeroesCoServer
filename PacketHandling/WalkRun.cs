using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    public class WalkRun
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            if (GC.MyChar.BuffOf(Server.Skills.SkillsClass.ExtraEffect.BlessPray).Eff == Server.Skills.SkillsClass.ExtraEffect.BlessPray)
            GC.MyChar.RemoveBuff(GC.MyChar.BuffOf(Skills.SkillsClass.ExtraEffect.BlessPray));
            GC.MyChar.Mining = false;
            GC.MyChar.AtkMem.Attacking = false;
            GC.MyChar.Action = 100;
            
            Game.World.Action(GC.MyChar, Data);
            GC.MyChar.Direction = (byte)(Data[4] % 8);
            if (Data[12] == 9)
            {
                if (GC.MyChar.Vigor >= 5)
                    GC.MyChar.Vigor -= 5;
                else return;
                GC.MyChar.Loc.Walk((byte)(Data[4] % 8));
                Game.World.Spawns(GC.MyChar, true);
            }
            GC.MyChar.Loc.Walk((byte)(Data[4] % 8));
            Game.World.Spawns(GC.MyChar, true);
            GC.MyChar.Protection = false;
            #region Hold Hands Check
            if (GC.MyChar.InteractionInProgress)
            {
                if (!GC.MyChar.InteractionSet)
                {
                    if (Game.World.H_Chars.ContainsKey(GC.MyChar.InteractionWith))
                    {
                        Game.Character ch = Game.World.H_Chars[GC.MyChar.InteractionWith] as Game.Character;
                        if (ch.InteractionInProgress && ch.InteractionWith == GC.MyChar.EntityID)
                        {
                            if (GC.MyChar.InteractionX == GC.MyChar.Loc.X && GC.MyChar.Loc.Y == GC.MyChar.InteractionY)
                            {
                                if (GC.MyChar.Loc.X == ch.Loc.X && GC.MyChar.Loc.Y == ch.Loc.Y)
                                {
                                    ch.MyClient.SendPacket(Packets.AttackPacket(ch.EntityID, GC.MyChar.EntityID, ch.Loc.X, ch.Loc.Y, GC.MyChar.InteractionType, 47));
                                    GC.MyChar.SendScreen(Packets.AttackPacket(GC.MyChar.EntityID, ch.EntityID, ch.Loc.X, ch.Loc.Y, GC.MyChar.InteractionType, 49));
                                    GC.MyChar.SendScreen(Packets.AttackPacket(ch.EntityID, GC.MyChar.EntityID, ch.Loc.X, ch.Loc.Y, GC.MyChar.InteractionType, 49));
                                    GC.MyChar.InteractionSet = true;
                                    ch.InteractionSet = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004)
                    {
                        if (Game.World.H_Chars.ContainsKey(GC.MyChar.InteractionWith))
                        {
                            Game.Character ch = Game.World.H_Chars[GC.MyChar.InteractionWith] as Game.Character;
                            ch.Direction = (byte)(Data[4] % 8);
                            ch.Loc.Walk((byte)(Data[4] % 8));
                            ch.MyClient.SendPacket(Packets.GeneralData(ch.EntityID, 0, ch.Loc.X, ch.Loc.Y, 0x9c));
                            Game.World.Spawns(ch, true);
                        }
                    }
                }
            }
            #endregion

        }
    }
}
