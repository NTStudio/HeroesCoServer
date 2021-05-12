using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class DropAnItem
    {
         public static void Handle(GameClient GC, byte[] Data)
        {
            uint ItemUID = BitConverter.ToUInt32(Data, 4);
           // Game.Item I = GC.MyChar.FindInvItem(ItemUID);

            Game.Item I = new Game.Item();
            foreach (Game.Item Item in GC.MyChar.Inventory.Values)
            {
                if (Item.UID == ItemUID)
                {
                    I = Item;
                }
            }
            if (I.ID != 0)
            {
                if (GC.MyChar.MyShop == null)
                {
                    /* if (I.ID == 300000)
                    {
                        GC.LocalMessage(2005, System.Drawing.Color.Yellow , "Unambled Drop Steed");
                        return;
                    }*/
                     if (!I.FreeItem)
                    {
                        Game.DroppedItem DI = new Server.Game.DroppedItem();
                        DI.Info = I;
                        DI.Owner = GC.MyChar.EntityID;
                        DI.DropTime = DateTime.Now;
                        DI.Loc.MapDimention = GC.MyChar.Loc.MapDimention;
                        DI.Loc = GC.MyChar.Loc;
                        DI.someBodyDropedIt = true;
                        DI.UID = (uint)Program.Rnd.Next(10000000);
                        if (!DI.FindPlace((Hashtable)Game.World.H_Items[GC.MyChar.Loc.Map]))
                            return;
                        DI.Drop();

                        GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                        Database.KeepItemAwyFromHand(I.UID, GC.MyChar);
                        GC.MyChar.Inventory.Remove(I.UID);
                    }
                    else
                        GC.LocalMessage(2005, System.Drawing.Color.Red , "Cannot drop Free items.");
                }
            }
            else
                GC.SendPacket(Packets.ItemPacket(ItemUID, 0, 3));
        }
    }
}
