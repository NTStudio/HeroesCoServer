using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game;
using System.Text;

namespace Server.PacketHandling.Effects
{
    class CastEffect
    {
        public int countdown;
        public System.Threading.Timer timer;
        public Buff B;
        public Character C;
        public ulong transValue;
        public void executeEffect()
        {
            System.Threading.TimerCallback cb = new System.Threading.TimerCallback(ProcessTimerEvent);
            timer = new System.Threading.Timer(cb, B, 0, 100);
        }
        private void ProcessTimerEvent(object obj)
        {
            --countdown;
            Game.Buff B = (Game.Buff)obj;
            if (countdown == 0)
            {
                C.RemoveBuff(B);
                return;
            }

            if (obj is Game.Buff)
            {
                SendEffect(Packets.Status(C.EntityID, Status.Mesh, transValue), countdown);
            }
        }
        public void SendEffect(byte[] Data, int c)
        {
            Character[] Chars = new Game.Character[Game.World.H_Chars.Count];
            Game.World.H_Chars.Values.CopyTo(Chars, 0);
            if (c > 0)
                foreach (Game.Character C in Chars)
                    if (MyMath.PointDistance(C.Loc.X, C.Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                        C.MyClient.SendPacket2(Data);
            Chars = null;
        }
    }
}

