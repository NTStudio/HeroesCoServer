using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class Shops
    {
        public static void SellHandle(GameClient GC, byte[] Data)
        {
            uint NPCID = BitConverter.ToUInt32(Data, 4);
            uint ItemUID = BitConverter.ToUInt32(Data, 8);

            if (Game.World.H_NPCs.Contains(NPCID) && Database.Shops.Contains(NPCID))
            {
                Game.NPC N = (Game.NPC)Game.World.H_NPCs[NPCID];
                if (N.Loc.Map == GC.MyChar.Loc.Map && MyMath.PointDistance(N.Loc.X, N.Loc.Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y) < 18)
                {
                   Game.Item I = new Game.Item();

                    foreach (Game.Item Item in GC.MyChar.Inventory.Values)
                   {
                        if (Item.UID == ItemUID)
                       {
                           I = Item;
                        }
                    }


                  GC.MyChar.Inventory.Remove(I.UID);
                  GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                  Database.DeleteItem(I.UID, GC.MyChar.EntityID);
                    if (I.MaxDur != 0)
                        GC.MyChar.Silvers += I.ItemDBInfo.Worth * I.CurDur / I.MaxDur / 3;
                }
            }
        }
        public static void BuyHandle(GameClient GC, byte[] Data)
        {
            try
            {
                uint NPCID = BitConverter.ToUInt32(Data, 4);
                uint ItemID = BitConverter.ToUInt32(Data, 8);
                byte Amount = Data[20];
                if (Amount == 0)
                    Amount = 1;

                if (Game.World.H_NPCs.Contains(NPCID) || NPCID == 2888 && Database.Shops.Contains(NPCID))
                {
                    Game.NPC N = (Game.NPC)Game.World.H_NPCs[NPCID];
                    Shop S = (Shop)Database.Shops[NPCID];

                    if ((N != null && N.Loc.Map == GC.MyChar.Loc.Map && MyMath.PointDistance(N.Loc.X, N.Loc.Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y) < 18 || NPCID == 2888) && S.Items.Contains(ItemID))
                    {
                        DatabaseItem DBI = (DatabaseItem)Database.DatabaseItems[ItemID];
                        if (DBI.ID == 0)
                        {
                            Program.WriteMessage("Error Database item = null, item id = " + ItemID);
                            return;
                        }
                        for (byte i = 0; i < Amount; i++)
                        {
                            if (GC.MyChar.Inventory.Count < 40)
                            {
                                if (S.MoneyType == 0 && GC.MyChar.Silvers >= DBI.Worth)
                                {
                                    Game.Item I = new Server.Game.Item();
                                    I.ID = ItemID;
                                    
                                    //catch (Exception Exc) { Program.WriteMessage(Exc); }
                                    Game.ItemIDManipulation e = new Server.Game.ItemIDManipulation(I.ID);
                                    if (e.Part(0, 2) == 11 || e.Part(0, 2) == 13 || e.Part(0, 3) == 123 || e.Part(0, 3) == 141 || e.Part(0, 3) == 142)
                                        I.Color = Server.Game.Item.ArmorColor.Red;
                                    GC.MyChar.CreateItemIDAmount(I.ID, 1);
                                    GC.MyChar.Silvers -= DBI.Worth;
                                }
                                else if (S.MoneyType == 1 && GC.MyChar.CPs >= DBI.CPsWorth)
                                {
                                    Program.WriteMessage("ShopingMail");
                                    Game.Item I = new Server.Game.Item();
                                    I.ID = ItemID;
                                  
                                    if (Game.ItemIDManipulation.Part(I.ID, 0, 2) == 73)
                                        I.Plus = Game.ItemIDManipulation.Digit(I.ID, 6);
                                    GC.MyChar.CreateItemIDAmount(I.ID, 1);
                                    GC.MyChar.CPs -= DBI.CPsWorth;
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
