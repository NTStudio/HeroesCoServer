using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    class KingsReviveHere
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            if (!GC.MyChar.Alive)
            {
                if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(20))
                {
                    if (GC.MyChar.BlessingLasts > 0)
                    {
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
                        }
                    }
                    
                }
            }
        }
    }
}
