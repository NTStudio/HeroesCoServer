using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Features
{

    public class Confiscator
    {
        public static int CpsItem(Game.Item I)
        {
            return I.Plus * 15000;
        }
        public static void RewardConfiscator(GameClient GC, byte[] Data)
        {
            string P = ""; string Phex = "";
            for (byte bit = 0; bit < Data.Length - 8; bit++)
            {
                int Pi = Data[bit];
                P += Data[bit] + " ";
                Phex += Pi.ToString("X") + " ";
            }
            Console.WriteLine("packet: {0} ", P);


            uint Uid = BitConverter.ToUInt32(Data, 4);
            uint NPC1 = BitConverter.ToUInt32(Data, 6);
            uint NPC = BitConverter.ToUInt32(Data, 12);
            Console.WriteLine("Npc {0} ID {1}", NPC, Uid);
            Console.WriteLine("this 1");

            if (NPC == 33)
            {
                GC.SendPacket(Packets.ItemPacket(Uid, 100, 33));
                foreach (Game.Item I in GC.MyChar.ConfiscatorClain.Values)
                {
                    if (I.UID == Uid)
                        if (I.Position == 1)
                        {
                            GC.MyChar.CPs += (uint)Features.Confiscator.CpsItem(I);
                            GC.MyChar.ConfiscatorClain.Remove(I.UID);
                            Database.DeleteClain(I.UID, GC.MyChar);
                        }
                        else if (I.Position == 0)
                        {
                            uint myTIme = Convert.ToUInt32(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                            if (myTIme > I.Time)
                            {
                                GC.MyChar.ReturnItemToInventory(I);
                                Database.DeleteReward(I.UID, GC.MyChar);
                                GC.MyChar.ConfiscatorClain.Remove(I.UID);
                                GC.SendPacket(Packets.ItemPacket(Uid, 1, 33));
                                GC.SendPacket2(Data);
                            }
                            foreach (Game.Character C in Game.World.H_Chars.Values)
                            {
                                if (C != null && C.Name == I.NameClain)
                                    if (C.EntityID != GC.MyChar.EntityID)
                                    {
                                        C.ConfiscatorReward.Remove(I.UID);
                                    }
                            }
                        }
                }
            }
        }
        public static void RetriveItem(GameClient GC, byte[] Data)
        {
            uint Uid = BitConverter.ToUInt32(Data, 4);
            uint NPC = BitConverter.ToUInt32(Data, 12);

            if (Data[12] == 32)
            {
                Console.WriteLine("this 32 passed");
                foreach (Game.Item Item in GC.MyChar.ConfiscatorReward.Values)
                {
                    if (Item.UID == Uid)
                    {

                        //if (Item.NameClain == GC.MyChar.Name)
                        {
                            uint myTIme = Convert.ToUInt32(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));

                            if (myTIme <= Item.Time && Item.Position == 0)
                            {
                                if (GC.MyChar.CPs >= (uint)Features.Confiscator.CpsItem(Item))
                                {

                                    GC.MyChar.CPs -= (uint)Features.Confiscator.CpsItem(Item);
                                    GC.MyChar.ReturnItemToInventory(Item);
                                    Item.Position = 1;
                                    GC.MyChar.ConfiscatorReward.Remove(Item.UID);
                                    Database.update_CI_position(Item.UID, GC.MyChar);
                                    Game.Character C = Game.World.CharacterFromName(Item.NameReward);
                                    if (C != null)
                                    {
                                        {
                                            C.ConfiscatorClain.Remove(Uid);
                                            C.MyClient.SendPacket(Packets.RemoveItemClain2(Item, 0, 32));
                                            C.MyClient.SendPacket(Packets.Sac(Item, C, (ushort)Features.Confiscator.CpsItem(Item), GC.MyChar.Name));
                                            //if (!C.ConfiscatorClain.ContainsKey(Uid))
                                            C.ConfiscatorClain.Add(Item.UID, Item);
                                            // C.MyClient.SendPacket(Packets.ConfiscatorClain(Item, C, (ushort)PacketHandling.Confiscator.CpsItem(Item), Item.NameClain));
                                        }
                                    }                                            //GC.SendPacket(Packets.ItemPacket(Uid, 2, 32));
                                    GC.SendPacket(Packets.RemoveItemClain2(Item, 0, 3));
                                    GC.LocalMessage(2000, System.Drawing.Color.Red, "Congratulation " + GC.MyChar.Name + "  has detaine the items " + Item.ItemDBInfo.ID + " whit " + (ushort)Features.Confiscator.CpsItem(Item) + " for player " + Item.NameReward + " ");
                                }
                                else
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry, you need " + (ushort)Features.Confiscator.CpsItem(Item) + " CPs to claim");
                            }
                        }
                    }
                }
            }
        }
    }
}
