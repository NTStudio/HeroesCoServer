using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class DropMoney
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint HowMuch = BitConverter.ToUInt32(Data, 4);
            if (HowMuch > 100)
            {
                if (HowMuch <= GC.MyChar.Silvers)
                {
                    if (GC.MyChar.MyShop == null)
                    {
                        Game.DroppedItem DI = new Server.Game.DroppedItem();
                        DI.Info = new Game.Item();
                        DI.Silvers = HowMuch;
                        if (DI.Silvers < 10)
                            DI.Info.ID = 1090000;
                        else if (DI.Silvers < 100)
                            DI.Info.ID = 1090010;
                        else if (DI.Silvers < 1000)
                            DI.Info.ID = 1090020;
                        else if (DI.Silvers < 3000)
                            DI.Info.ID = 1091000;
                        else if (DI.Silvers < 10000)
                            DI.Info.ID = 1091010;
                        else
                            DI.Info.ID = 1091020;

                        DI.Info.UID = (uint)new Random().Next(10000000);
                        DI.DropTime = DateTime.Now;
                        DI.Loc = GC.MyChar.Loc;
                        DI.Loc.MapDimention = GC.MyChar.Loc.MapDimention;
                        DI.UID = (uint)Program.Rnd.Next(10000000);
                        if (!DI.FindPlace((Hashtable)Game.World.H_Items[GC.MyChar.Loc.Map])) return;
                        DI.Drop();
                        GC.MyChar.Silvers -= HowMuch;
                    }
                }
            }
           /*
            if (I.ID != 0)
            {
                if (GC.MyChar.MyShop == null)
                {
                    if (!I.FreeItem)
                    {
                        Game.DroppedItem DI = new Server.Game.DroppedItem();
                        DI.Info = I;
                        DI.DropTime = DateTime.Now;
                        DI.Loc = GC.MyChar.Loc;
                        DI.UID = (uint)Program.Rnd.Next(10000000);
                        if (!DI.FindPlace((Hashtable)Game.World.H_Items[GC.MyChar.Loc.Map])) return;
                        DI.Drop();
                        GC.MyChar.RemoveItem(I);
                    }
                    else
                        GC.LocalMessage(2005, System.Drawing.Color.Red , "Cannot drop Free items.");
                }
            }
            else
                GC.SendPacket(Packets.ItemPacket(ItemUID, 0, 3));
        */}
    }
}
