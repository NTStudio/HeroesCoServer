using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.PacketHandling.ItemPacket
{
    public class Compose
    {
        //public static void Handle(Main.GameClient GC, byte[] Data)
        //{
        //    uint T = BitConverter.ToUInt32(Data, 4);
        //    uint MainUID = BitConverter.ToUInt32(Data, 8);
        //    uint MinorUID = BitConverter.ToUInt32(Data, 12);
        //    Console.WriteLine("MainItem " + MainUID + "");
        //    Console.WriteLine("MinorItem " + MinorUID + "");

        //    Item MainItem = new Item();
        //    foreach (Game.Item I in GC.MyChar.Inventory.Values)
        //    {
        //        if (I.UID == MainUID)
        //        {
        //            MainItem = I;
        //            Console.WriteLine("MainItem " + MainItem + "");

        //        }
        //    }
        //    Item MinorItem = new Item();
        //    foreach (Game.Item I in GC.MyChar.Inventory.Values)
        //    {
        //        if (I.UID == MinorUID)
        //        {
        //            MinorItem = I;

        //        }
        //    }

        //    int OldPlus = 0;

        //    byte PacketType = Data[4];
        //    Console.WriteLine("Compose PacketType Is " + Data[4] + " " + T + "");
        //    switch (PacketType)
        //    {
        //        case 0:
        //            {
        //                if (MainItem.ID != 0 && MinorItem.ID != 0)
        //                {
        //                    uint Progress = MainItem.Progress;
        //                    Progress += Database.StonePts[MinorItem.Plus];
        //                    OldPlus = MainItem.Plus;
        //                    if (MainItem.Plus <= 11)
        //                    {
        //                        while (Progress >= Database.ComposePts[MainItem.Plus] && MainItem.Plus <= 12)
        //                        {
        //                            Progress -= Database.ComposePts[MainItem.Plus];
        //                            if (MainItem.Plus == 12)
        //                            {
        //                                return;
        //                            }
        //                            else
        //                                MainItem.Plus++;

        //                        }
        //                    }
        //                    if (MainItem.Plus == 12)
        //                        Progress = 0;

        //                    MainItem.Progress = Progress;
        //                    GC.MyChar.UpdateItem(MainItem);
        //                    if (MainItem.Plus != OldPlus && MainItem.Plus >= 10)
        //                    {
        //                        Console.WriteLine("Congratulations 0");
        //                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations " + GC.MyChar.Name + " Has Upgraded Plus Of [" + MainItem.ItemDBInfo.Name + "] To +" + MainItem.Plus + ".");
        //                    }
        //                }
        //                break;
        //            }
        //        case 2:
        //        case 3:
        //            {
        //                if (MainItem.ID == 300000 && MinorItem.ID == 300000)
        //                {
        //                    uint Progress = MainItem.Progress;
        //                    Progress += Database.StonePts[MinorItem.Plus];
        //                    OldPlus = MainItem.Plus;

        //                    if (MainItem.Plus <= 11)
        //                    {
        //                        while (Progress >= Database.ComposePts[MainItem.Plus] && MainItem.Plus <= 12)
        //                        {
        //                            Progress -= Database.ComposePts[MainItem.Plus];
        //                            if (MainItem.Plus == 12)
        //                            {
        //                                return;
        //                            }
        //                            else
        //                                MainItem.Plus++;
        //                        }
        //                    }
        //                    if (MainItem.Plus == 12)
        //                        Progress = 0;

        //                    MainItem.TalismanProgress = Progress;
        //                    MainItem.Progress = Progress;

        //                    GC.MyChar.UpdateItem(MainItem);

        //                    if (MainItem.Plus != OldPlus && MainItem.Plus >= 10)
        //                    {
        //                        Console.WriteLine("Congratulations 2 3");
        //                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations " + GC.MyChar.Name + " Has Upgraded Plus Of [" + MainItem.ItemDBInfo.Name + "] To +" + MainItem.Plus + ".");
        //                    }
        //                }
        //                break;
        //            }
        //    }
        //}
    }
}
