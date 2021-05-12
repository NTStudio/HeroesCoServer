using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.PacketHandling
{
    public class MarketShops
    {
        public struct ItemValue
        {
            public byte MoneyType;
            public uint Value;
        }
        public class Shop
        {
            public Character Owner;
            public string Hawk = "";
            public Hashtable Items;
            public NPC NPCInfo;
            public uint UID;

            public Shop(Character C, uint Time)
            {
                Owner = C;
                Items = new Hashtable();

                NPCInfo = new NPC();
                NPCInfo.EntityID = (uint)Program.Rnd.Next(102000, 106000);
                while (World.H_NPCs.Contains(NPCInfo.EntityID) || World.H_PShops.Contains(NPCInfo.EntityID) || World.H_Chars.Contains(NPCInfo.EntityID))
                    NPCInfo.EntityID = (uint)Program.Rnd.Next(102000, 106000);
                NPCInfo.Type = 400;
                NPCInfo.Flags = 14;
                NPCInfo.Loc = C.Loc;
                NPCInfo.Loc.X++;
                NPCInfo.Direction = 6;
                NPCInfo.Avatar = 0;

                UID = NPCInfo.EntityID;

                ArrayList ePackets = new ArrayList();

                C.MyClient.SendPacket(Packets.SpawnNamedNPC2(NPCInfo, Name));
                C.MyClient.SendPacket(Packets.GeneralData(C.EntityID, UID, NPCInfo.Loc.X, NPCInfo.Loc.Y, 111, 6));
                

                World.H_PShops.Add(UID, this);

                World.Spawn(this);
            }
            public void Close()
            {
                World.H_PShops.Remove(UID);
                World.Action(Owner, Packets.GeneralData(UID, 0, 0, 0, 135));
                Owner.MyShop = null;
            }
            public bool AddItem(uint UID, uint Value, byte MoneyType)
            {
                Item I = new Item();


                foreach (Game.Item Ii in Owner.Inventory.Values)
                {
                    if (Ii.UID == UID)
                    {
                        I = Ii;
                    }
                }





                if (!I.FreeItem && I.UID == UID && !Items.Contains(UID))
                {
                    Items.Add(UID, new ItemValue() { Value = Value, MoneyType = MoneyType });
                    return true;
                }
                return false;
            }
            public void Buy(uint UID, Character C)
            {
                if (Owner != C && Items.Contains(UID) && C.Inventory.Count < 40)
                {
                    Item I = new Item();
                    foreach (Game.Item Ii in Owner.Inventory.Values)
                    {
                        if (Ii.UID == UID)
                        {
                            I = Ii;
                        }
                    }
                    if (I.ID != 0)
                    {
                        ItemValue Val = (ItemValue)Items[UID];
                        uint Costs = Val.Value;

                        if (Val.MoneyType == 1 && C.Silvers >= Costs)
                        {
                            C.Silvers -= Costs;
                            Owner.Silvers += Costs;
                            Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Red , C.Name + " has bought " + I.ItemDBInfo.Name + " for " + Costs + " from you.");
                            Owner.TradeItem(I, C);
                            RemoveItem(UID, Packets.ItemPacket(UID, this.UID, 23));
                            //Owner.RemoveItemI(I.UID, 1);
                        }
                        else if (Val.MoneyType == 3 && C.CPs >= Costs)
                        {
                            C.CPs -= Costs;
                            Owner.CPs += Costs;
                            Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Red , C.Name + " has bought " + I.ItemDBInfo.Name + " for " + Costs + " from you.");
                            Owner.TradeItem(I, C);
                            RemoveItem(UID, Packets.ItemPacket(UID, this.UID, 23));
                            //Owner.RemoveItemI(I.UID, 1);
                        }
                    }
                }
            }
            public void RemoveItem(uint UID, byte[] Data)
            {
                if (Items.Contains(UID))
                    Items.Remove(UID);
                foreach (Character C in World.H_Chars.Values)
                    if (C.Loc.Map == Owner.Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y, 18))
                        C.MyClient.SendPacket2(Data);
            }
            public void SendItems(GameClient C)
            {
                foreach (DictionaryEntry DE in Items)
                {
                   // Item I = Owner.FindInvItem((uint)DE.Key);
                    Item I = new Item();


                    foreach (Game.Item Ii in Owner.Inventory.Values)
                    {
                        if (Ii.UID == (uint)DE.Key)
                        {
                            I = Ii;
                        }
                    }



                    if (I.ID != 0)
                        C.SendPacket(Packets.AddStallItem(I, (ItemValue)DE.Value, UID));
                }
            }
            public string Name
            {
                get
                {
                    return Owner.Name;
                }
            }
        }
    }
}
