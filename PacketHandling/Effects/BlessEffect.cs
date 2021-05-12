using System;
using Server.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.Effects
{
    public class BlessEffect
    {
        public static void Handler(GameClient GC)
        {
            byte bless1 = 0;
            byte bless3 = 0;
            byte bless5 = 0;
            byte bless7 = 0;
            for (byte i = 1; i < 12; i++)
            {
                if (i != 7)
                {
                    Game.Item I = (Game.Item)GC.MyChar.Equips.Get(i);
                    if (I.Bless == 1)
                        bless1++;
                    if (I.Bless == 3)
                        bless3++;
                    if (I.Bless == 5)
                        bless5++;
                    if (I.Bless == 7)
                        bless7++;
                }
            }
            #region Check for bless 7
            if (bless7 > bless5 && bless7 > bless3 && bless7 > bless1)
            {
                if (MyMath.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                    {
                        Game.Character Chaar = (Game.Character)DE.Value;
                        if (Chaar.Name != GC.MyChar.Name)
                        {
                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis4"));

                        }
                    }
                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis4"));
                }
            }
            #endregion
            #region Check for bless 5
            else if (bless5 > bless7 && bless5 > bless3 && bless5 > bless1)
            {
                if (MyMath.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                    {
                        Game.Character Chaar = (Game.Character)DE.Value;
                        if (Chaar.Name != GC.MyChar.Name)
                        {
                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis3"));

                        }
                    }
                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis3"));
                }
                goto end;
            }
            #endregion
            #region Check for bless 3
            else if (bless3 > bless7 && bless3 > bless5 && bless3 > bless1)
            {
                if (MyMath.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                    {
                        Game.Character Chaar = (Game.Character)DE.Value;
                        if (Chaar.Name != GC.MyChar.Name)
                        {
                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis2"));

                        }
                    }
                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis2"));
                }
                goto end;
            }
            #endregion
            #region Check for bless 1
            else if (bless1 > bless7 && bless1 > bless5 && bless1 > bless3)
            {
                if (MyMath.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in Game.World.H_Chars)
                    {
                        Game.Character Chaar = (Game.Character)DE.Value;
                        if (Chaar.Name != GC.MyChar.Name)
                        {
                            Chaar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis1"));

                        }
                    }
                    GC.MyChar.MyClient.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "Aegis1"));
                }
                goto end;
            }//try it now.... lets see : D kk
            #endregion
        end:
            return;
        }
    }
}