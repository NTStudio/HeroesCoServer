using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class ItemLock
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint Itemuid = BitConverter.ToUInt32(Data, 4);
            byte LockType = Data[8];

            Game.Item Item = new Game.Item();
            foreach (Game.Item I in GC.MyChar.Inventory.Values)
            {
                if (I.UID == Itemuid)
                {
                    Item = I;
                }
            }
            switch (LockType)
            {
                case 0:
                    {
                        if (Item.Locked == 0 || Item.Locked == 2)
                        {
                            Item.Locked = 1;
                            GC.SendPacket(Packets.ItemLock(Itemuid, LockType, 1, 0));
                            GC.SendPacket(Packets.UpdateItem(Item, 0));
                            Database.UpdateItem(Item);
                        }
                        if (Item.Locked == 2)
                        {
                            GC.SendPacket(Packets.ItemLock(Item.UID, 1, 3, (uint)Item.LockedDays));
                        }
                        return;
                    }
                case 1:
                    {
                        DateTime datetounlock = DateTime.Now.AddDays(5);
                        Item.LockedDays = (uint)Convert.ToInt32(datetounlock.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                        Item.Locked = 2;
                        GC.SendPacket(Packets.UpdateItem(Item, 0));
                        Database.UpdateItem(Item);
                        if (Item.Locked == 2)
                        {
                            GC.SendPacket(Packets.ItemLock(Item.UID, 1, 3, (uint)Item.LockedDays));
                        }
                        break;
                    }
                case 2:
                    {
                        if (Item.Locked == 2)
                        {
                            GC.SendPacket(Packets.ItemLock(Item.UID, 2, 3, (uint)Item.LockedDays));
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}