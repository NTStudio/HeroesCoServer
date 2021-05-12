using System;
using System.Collections.Generic;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class Durability
    {
        public static void DefenceDurability(GameClient GC)
        {
            if (GC.MyChar.Loc.Map != 1039 || GC.MyChar.Loc.Map != 1038 || GC.MyChar.Loc.Map != 6000 || GC.MyChar.Loc.Map != 6001)
            {
                for (byte i = 1; i < 12; i++)
                {
                    if (i == 1 || i == 2 || i == 3 || i == 8 || i == 11)
                    {
                        Game.Item I = GC.MyChar.Equips.Get(i);
                        bool Arrow = false;
                        if (I.ID == 1050000 || I.ID == 1050001 || I.ID == 1050002 || I.ID == 1050020 || I.ID == 1050030
                            || I.ID == 1050021 || I.ID == 1051000 || I.ID == 1050050 || I.ID == 1050031 || I.ID == 1050040
                            || I.ID == 1050031 || I.ID == 1050022 || I.ID == 1050051 || I.ID == 1050032 || I.ID == 1050041
                            || I.ID == 1050041 || I.ID == 1050052 || I.ID == 1050042 || I.ID == 1050023 || I.ID == 1050033 || I.ID == 1050043)
                        { Arrow = true; }
                        if (I.ID != 0 && I.CurDur > 0 && Arrow == false)
                        {
                            if (MyMath.ChanceSuccess(((100 - GC.MyChar.EqStats.TotalDodge) / 100)))
                            {
                                I.CurDur -= 1;
                                if (I.CurDur <= (ushort)I.MaxDur / 10)
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Be carefull, your item named " + I.ItemDBInfo.Name + " current dura is too low. Go repair it now!");
                                else if (I.CurDur <= 0)
                                    I.CurDur = 0;

                                GC.SendPacket(Packets.UpdateItem(I, i));
                                //Console.WriteLine("Durability removed from item position " + i);
                                //Console.WriteLine("Durability removed from item named " + I.DBInfo.Name);
                            }
                        }
                        else if (I.ID != 0 && I.CurDur == 0 && I.MaxDur > 0 && Arrow == false)
                        {
                            GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Be carefull, your item named " + I.ItemDBInfo.Name + " current dura is broken, repair it now or the item will desappear!");
                        }
                    }
                }
            }
        }

        public static void AttackDurability(GameClient GC)
        {
            for (byte i = 1; i < 12; i++)
            {
                if (i != 1 || i != 2 || i != 3 || i != 7 || i != 8 || i != 11)
                {
                    Game.Item I = GC.MyChar.Equips.Get(i);
                    bool Arrow = false;
                    if (I.ID == 1050000 || I.ID == 1050001 || I.ID == 1050002 || I.ID == 1050020 || I.ID == 1050030
                        || I.ID == 1050021 || I.ID == 1051000 || I.ID == 1050050 || I.ID == 1050031 || I.ID == 1050040
                        || I.ID == 1050031 || I.ID == 1050022 || I.ID == 1050051 || I.ID == 1050032 || I.ID == 1050041
                        || I.ID == 1050041 || I.ID == 1050052 || I.ID == 1050042 || I.ID == 1050023 || I.ID == 1050033 || I.ID == 1050043)
                    { Arrow = true; }
                    if (I.ID != 0 && I.CurDur > 0 && Arrow == false)
                    {
                        if (MyMath.ChanceSuccess(((100 - GC.MyChar.EqStats.TotalDodge) / 90)))
                        {
                            I.CurDur -= 2;
                            if (I.CurDur <= (ushort)I.MaxDur/10)
                                GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Be carefull, your item named " + I.ItemDBInfo.Name + " current dura is too low. Go repair it now!");
                            else if (I.CurDur <= 0)
                                I.CurDur = 0;

                            GC.SendPacket(Packets.UpdateItem(I, i));
                            //Console.WriteLine("Durability removed from item position " + i);
                            //Console.WriteLine("Durability removed from item named " + I.DBInfo.Name);
                        }
                    }
                    else if (I.ID != 0 && I.CurDur == 0 && I.MaxDur > 0 && Arrow == false)
                    {
                        GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Be carefull, your item named " + I.ItemDBInfo.Name + " current dura is broken, repair it now or the item will desappear!");
                    }
                }
            }
        }
    }
}