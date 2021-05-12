using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server.PacketHandling.Effects
{
    class CoolEffect
    {
        public static void ActiveCool(GameClient MyClient)
        {
            byte counter = 0;

            for (byte i = 1; i < 9; i++)
            {
                if (i == 7) i++;
                Game.Item I = MyClient.MyChar.Equips.Get(i);
                if (I.ID != 0)
                {
                    Game.ItemIDManipulation Q = new Server.Game.ItemIDManipulation(I.ID);
                    if (Q.Quality == Game.Item.ItemQuality.Super)
                        counter += 1;
                }
            }

            if (MyClient.MyChar.Job >= 100)
                if (counter == 6)
                    counter = 7;
            if (MyClient.MyChar.Job >= 40 && MyClient.MyChar.Job <= 45)
                if (counter == 6)
                {
                    Game.Item I = MyClient.MyChar.Equips.Get(5);
                    I.ID = MyClient.MyChar.Equips.LeftHand.ID;
                    if (I.ID == 0)
                        counter = 7;
                }
            if (counter == 7)
            {
                if (MyClient.MyChar.Job >= 10 && MyClient.MyChar.Job <= 15)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "warrior"));
                else if (MyClient.MyChar.Job >= 20 && MyClient.MyChar.Job <= 25)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "fighter"));
                else if (MyClient.MyChar.Job >= 100)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "taoist"));
                else if (MyClient.MyChar.Job >= 39 && MyClient.MyChar.Job <= 46)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "archer"));
                else if (MyClient.MyChar.Job >= 50 && MyClient.MyChar.Job <= 55)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "Ninja120"));
            }
            else
            {
                if (MyClient.MyChar.Job >= 10 && MyClient.MyChar.Job <= 15)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "warrior-s"));
                else if (MyClient.MyChar.Job >= 20 && MyClient.MyChar.Job <= 25)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "fighter-s"));
                else if (MyClient.MyChar.Job >= 100)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "taoist-s"));
                else if (MyClient.MyChar.Job >= 39 && MyClient.MyChar.Job <= 46)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "archer-s"));
                else if (MyClient.MyChar.Job >= 50 && MyClient.MyChar.Job <= 55)
                    MyClient.SendPacket(Packets.String(MyClient.MyChar.EntityID, 10, "Ninja120"));
            }
            MyClient.MyChar.Action = 100;

        }
    }
}