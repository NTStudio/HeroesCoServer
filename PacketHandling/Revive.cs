using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    class Revive
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            if (!GC.MyChar.Alive)
            {
                if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(20))
                {
                    
                    GC.MyChar.Ghost = false;
                    GC.MyChar.BlueName = false;
                    GC.MyChar.CurHP = (ushort)GC.MyChar.MaxHP;
                    GC.MyChar.Alive = true;
                    GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.Dead);
                    GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.BlueName);
                    GC.MyChar.Body = GC.MyChar.Body;
                    GC.MyChar.Hair = GC.MyChar.Hair;
                    GC.MyChar.Equips.Send(GC, false);
                    GC.MyChar.Protection = true;
                    GC.MyChar.LastProtection = DateTime.Now;
                    if (GC.MyChar.Loc.Map == 1090)
                    {
                        if (GC.MyChar.MyTDmTeam.TeamID == 10)//black
                        {
                            GC.MyChar.Teleport(1090, 121, 70);
                        }
                        if (GC.MyChar.MyTDmTeam.TeamID == 30)//blue
                        {
                            GC.MyChar.Teleport(1090, 144, 152);
                        }
                        if (GC.MyChar.MyTDmTeam.TeamID == 20)//red
                        {
                            GC.MyChar.Teleport(1090, 43, 67);
                        }
                        if (GC.MyChar.MyTDmTeam.TeamID == 40)//white
                        {
                            GC.MyChar.Teleport(1090, 85, 108);
                        }
                    }
                    if (GC.MyChar.Loc.Map == 1038 && Extra.GuildWars.War)
                        GC.MyChar.Teleport(6001, 32, 72);
                    else if (GC.MyChar.Loc.Map == 1730)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1731)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1732)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1733)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1734)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1735)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1736)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else if (GC.MyChar.Loc.Map == 1737)
                        GC.MyChar.Teleport(1002, 429, 378);
                    else
                    {
                        if (!DMaps.FreePKMaps.Contains(GC.MyChar.Loc.Map))
                        if (GC.MyChar.PKPoints >= 100 && GC.MyChar.Loc.Map != 6000 && GC.MyChar.Loc.Map != 6001 && GC.MyChar.Loc.Map != 1090 && GC.MyChar.Loc.Map != 5000 && GC.MyChar.Loc.Map != 1068 && GC.MyChar.Loc.Map != 1707 && GC.MyChar.Loc.Map != 1005 && GC.MyChar.Loc.Map != 1038 && GC.MyChar.Loc.Map != 5000)
                            GC.MyChar.Teleport(6000, 32, 72);
                        else
                        {
                            foreach (ushort[] Point in Database.RevPoints)
                                if (Point[0] == GC.MyChar.Loc.Map)
                                {
                                    GC.MyChar.Teleport(Point[1], Point[2], Point[3]);
                                    break;
                                }
                        }
                    }
                    GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                }
            }
        }
    }
    class ReviveHere
    {
        public static void Handle(byte[] Data, GameClient GC)//, byte[] Data)
        {
            if (!GC.MyChar.Alive)
            {
                if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(20))
                {
                    if (GC.MyChar.Loc.Map == 1038 && Extra.GuildWars.War)
                    {

                    }
                    else
                    {
                        GC.MyChar.Ghost = false;
                        GC.MyChar.BlueName = false;
                        GC.MyChar.CurHP = (ushort)GC.MyChar.MaxHP;
                        GC.MyChar.Alive = true;
                        GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.Dead);
                        GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.BlueName);
                        GC.MyChar.Body = GC.MyChar.Body;
                        GC.MyChar.Hair = GC.MyChar.Hair;
                        GC.MyChar.Equips.Send(GC, false);
                        GC.MyChar.Protection = true;
                        GC.MyChar.LastProtection = DateTime.Now;
                        if (GC.MyChar.Loc.Map == 1090)
                        {
                            if (GC.MyChar.MyTDmTeam.TeamID == 10)//black
                            {
                                GC.MyChar.Teleport(1090, 121, 70);
                            }
                            if (GC.MyChar.MyTDmTeam.TeamID == 30)//blue
                            {
                                GC.MyChar.Teleport(1090, 144, 152);
                            }
                            if (GC.MyChar.MyTDmTeam.TeamID == 20)//red
                            {
                                GC.MyChar.Teleport(1090, 43, 67);
                            }
                            if (GC.MyChar.MyTDmTeam.TeamID == 40)//white
                            {
                                GC.MyChar.Teleport(1090, 85, 108);
                            }
                        }
                        {
                            if (GC.MyChar.Loc.Map == 1730)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1731)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1732)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1733)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1734)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1735)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1736)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else if (GC.MyChar.Loc.Map == 1737)
                                GC.MyChar.Teleport(1002, 429, 378);
                            else
                            {
                            }
                            //GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                        }
                        
                        GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                    }
                }
            }
        }
    }
}