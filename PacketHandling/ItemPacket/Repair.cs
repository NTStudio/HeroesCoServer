using System;
using System.Collections.Generic;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class Repair
    {
        public static void Handle(byte[] Data, GameClient GC)
        {
            uint id = BitConverter.ToUInt32(Data, 4);
          //  Game.Item Repairing = GC.MyChar.FindInvItem(id);

            string P = ""; string Phex = "";
            for (byte bit = 0; bit < Data.Length - 8; bit++)
            {
                int Pi = Data[bit];
                P += Data[bit] + " ";
                Phex += Pi.ToString("X") + " ";
            }

            Game.Item Repairing = new Game.Item();

            foreach (Game.Item Item in GC.MyChar.Inventory.Values)
            {
                if (Item.UID == id)
                {
                    Repairing = Item;
                }
            }


            if (Repairing.CurDur == 0 && Repairing.MaxDur == 0)
            {
                if (GC.MyChar.FindInventoryItemIDAmount(1088001, 5))
                {
                   for (int i = 0; i < 5; i++)
                       GC.MyChar.RemoveItemUIDAmount(1088001, 1);
                   //     GC.MyChar.RemoveItem(GC.MyChar.NextItem(1088001));

                    Repairing.MaxDur = Repairing.ItemDBInfo.Durability;
                    Repairing.CurDur = Repairing.MaxDur;
                    GC.SendPacket(Packets.UpdateItem(Repairing, 0));
                    Console.WriteLine("Repairing item");
                    Database.UpdateItem(Repairing);
                }
                else
                {
                    GC.LocalMessage(2005, System.Drawing.Color.Red , "You don't have 5 meteors to repair this item!");
                }
            }
            else
            {
                int nRecoverDurability = Math.Max(0, (Repairing.MaxDur - Repairing.CurDur));

                if (nRecoverDurability == 0)
                    return;

                int nRepairCost = (int)(Math.Max(1, (Repairing.ItemDBInfo.Worth * nRecoverDurability / Repairing.MaxDur / 2)));
                if (GC.MyChar.Silvers >= (nRepairCost / 2))
                {
                    GC.MyChar.Silvers -= (uint)(nRepairCost / 2);
                    Repairing.CurDur = Repairing.MaxDur;
                    GC.SendPacket(Packets.UpdateItem(Repairing, 0));
                    Database.UpdateItem(Repairing);
                }
                else
                    GC.LocalMessage(2005, System.Drawing.Color.Red , "You don`t have enough money. Come back after you have more!");
            }
        }
        public static void HandleVipRepair(GameClient GC)
        {
            for (byte i = 1; i < 12; i++)
            {
                if (i != 7)
                {
                    Game.Item Repairing = GC.MyChar.Equips.Get(i);
                    if (Repairing.ID != 0)
                    {
                        int nRecoverDurability = Math.Max(0, (Repairing.MaxDur - Repairing.CurDur));

                        if (nRecoverDurability == 0)
                            return;

                        int nRepairCost = (int)(Math.Max(1, (Repairing.ItemDBInfo.Worth * nRecoverDurability / Repairing.MaxDur / 2)));
                        if (GC.MyChar.Silvers >= (nRepairCost / 2))
                        {
                            GC.MyChar.Silvers -= (uint)(nRepairCost / 2);
                            Repairing.CurDur = Repairing.MaxDur;
                            GC.SendPacket(Packets.UpdateItem(Repairing, i));
                            Database.UpdateItem(Repairing);
                        }
                        else
                            GC.LocalMessage(2005, System.Drawing.Color.Red , "You don`t have enough money. Come back after you have more!");
                    }
                }
            }
        }
    }
}