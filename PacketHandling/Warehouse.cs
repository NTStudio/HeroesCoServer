using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    public class Warehouse
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            if (!GC.MyChar.WHOpen && GC.MyChar.WHPassword != "0")
                return;

            uint NPC = BitConverter.ToUInt32(Data, 4);
            uint IUID = BitConverter.ToUInt32(Data, 12);
            byte Type = Data[8];

            ArrayList Warehouse = null;

            switch (NPC)
            {
                case 8: { Warehouse = GC.MyChar.Warehouses.TCWarehouse; break; }
                case 10012: { Warehouse = GC.MyChar.Warehouses.PCWarehouse; break; }
                case 10028: { Warehouse = GC.MyChar.Warehouses.ACWarehouse; break; }
                case 10011: { Warehouse = GC.MyChar.Warehouses.DCWarehouse; break; }
                case 10027: { Warehouse = GC.MyChar.Warehouses.BIWarehouse; break; }
                case 44: { Warehouse = GC.MyChar.Warehouses.MAWarehouse; break; }
                case 4101: { Warehouse = GC.MyChar.Warehouses.SCWarehouse; break; }

                case 2511: { Warehouse = GC.MyChar.Warehouses.TCWarehouse; break; }
                case 2512: { Warehouse = GC.MyChar.Warehouses.PCWarehouse; break; }
                case 2513: { Warehouse = GC.MyChar.Warehouses.ACWarehouse; break; }
                case 2514: { Warehouse = GC.MyChar.Warehouses.DCWarehouse; break; }
                case 2515: { Warehouse = GC.MyChar.Warehouses.BIWarehouse; break; }
                case 2510: { Warehouse = GC.MyChar.Warehouses.MAWarehouse; break; }
                case 2516: { Warehouse = GC.MyChar.Warehouses.SCWarehouse; break; }
            }

            if (Type == 1)
            {
                try
                {
                    if (Warehouse.Count < Warehouse.Capacity)
                    {
                        Game.Item I = new Game.Item();
                        foreach (Game.Item II in GC.MyChar.Inventory.Values)
                        {
                            if (II.UID == IUID)
                            {
                                I = II;
                            }
                        }
                        if (I.ID != 0)
                        {
                            Database.KeepItemAwyFromHand(I.UID, GC.MyChar);
                            GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                            GC.MyChar.Inventory.Remove(I.UID);
                            Warehouse.Add(I);
                                             
                            GC.SendPacket(Packets.AddWHItem(GC.MyChar, (ushort)NPC, I));
                            Database.SaveEqupsAndWH(GC.MyChar);
                            return;
                        }
                        else
                            GC.SendPacket(Packets.ItemPacket(IUID, 0, 3));
                    }
                }
                catch { }
                return;
            }
            else if (Type == 2)
            {
                try
                {
                    if (GC.MyChar.Inventory.Count < 40)
                    {
                        Game.Item I = new Game.Item();
                        foreach (Game.Item II in Warehouse)
                            if (II.UID == IUID)
                                I = II;

                        if (I.ID != 0 && I.UID != 0)
                        {
                            Database.ReputItemInHand(I, GC.MyChar);
                            GC.SendPacket(Packets.RemoveWHItem(GC.MyChar, (ushort)NPC, I));
                            Warehouse.Remove(I);
                            GC.MyChar.Inventory.Add(I.UID, I);
                            GC.SendPacket(Packets.AddItem(I, 0));
                            Database.SaveEqupsAndWH(GC.MyChar);
                            return;
                        }
                    }
                }
                catch { return; }
                return;
            }

            switch (NPC)
            {
                case 8: { GC.MyChar.Warehouses.TCWarehouse = Warehouse; break; }
                case 10012: { GC.MyChar.Warehouses.PCWarehouse = Warehouse; break; }
                case 10028: { GC.MyChar.Warehouses.ACWarehouse = Warehouse; break; }
                case 10011: { GC.MyChar.Warehouses.DCWarehouse = Warehouse; break; }
                case 10027: { GC.MyChar.Warehouses.BIWarehouse = Warehouse; break; }
                case 44: { GC.MyChar.Warehouses.MAWarehouse = Warehouse; break; }
                case 4101: { Warehouse = GC.MyChar.Warehouses.SCWarehouse; break; }

                case 2511: { Warehouse = GC.MyChar.Warehouses.TCWarehouse; break; }
                case 2512: { Warehouse = GC.MyChar.Warehouses.PCWarehouse; break; }
                case 2513: { Warehouse = GC.MyChar.Warehouses.ACWarehouse; break; }
                case 2514: { Warehouse = GC.MyChar.Warehouses.DCWarehouse; break; }
                case 2515: { Warehouse = GC.MyChar.Warehouses.BIWarehouse; break; }
                case 2510: { Warehouse = GC.MyChar.Warehouses.MAWarehouse; break; }
                case 2516: { Warehouse = GC.MyChar.Warehouses.SCWarehouse; break; }
            }
            //GC.SendPacket(Packets.SendWarehouse(GC.MyChar, (ushort)NPC));
            if (Warehouse == GC.MyChar.Warehouses.MAWarehouse)
            {
                if (Warehouse.Count < 41)
                    for (int x = 0; x < Warehouse.Count; x++)
                    {
                        Console.WriteLine("market item display");
                        GC.SendPacket(Packets.AddWHItem(GC.MyChar, (ushort)NPC, (Game.Item)Warehouse[x]));
                    }
            }
            else if (Warehouse.Count < 21)
            {
                for (int x = 0; x < Warehouse.Count; x++)
                {
                    Console.WriteLine("others item display");
                    GC.SendPacket(Packets.AddWHItem(GC.MyChar, (ushort)NPC, (Game.Item)Warehouse[x]));
                }
            }
        }
    }
}
