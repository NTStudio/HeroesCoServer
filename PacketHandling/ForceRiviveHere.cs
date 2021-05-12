using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    class ForceReviveHere
    {
        public static void Handle(GameClient GC)
        {
            if (!GC.MyChar.Alive)
            {
                //if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(0))
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
                    GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                }
            }
        }
    }
}
