using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class Equip
    {
        public static void HandleUnEquip(GameClient GC, byte[] Data)
        {
            if (GC.MyChar.Loc.Map == 1090)
            { return; }
            uint ItemUID = BitConverter.ToUInt32(Data, 4);

            if (GC.MyChar.Inventory.Count < 40)
            {
                byte Slot = GC.MyChar.Equips.GetSlot(ItemUID);
                if (Slot != 0)
                {
                   try
                    {
                        Game.Item i = GC.MyChar.Equips.Get(Slot);
                        if (i.ID == 300000 && GC.MyChar.StatEff.Contains(Server.Game.StatusEffectEn.Ride))
                        {
                            GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.Ride);
                            GC.MyChar.StatEff.Remove(Server.Game.StatusEffectEn.Cyclone);
                        }
                    }
                    catch { }
                    Console.WriteLine(ItemUID);
                    if (GC.MyChar.Equips.Get(Slot).UID == 0)
                    {
                    Console.WriteLine("a7eh 3al uid ****************************");
                    return;
                    }
                    try
                    {
                        if (!GC.MyChar.Inventory.ContainsKey(GC.MyChar.Equips.Get(Slot).UID))
                        {

                           
                            GC.MyChar.EquipStats(Slot, false);
                            GC.MyChar.Equips.UnEquip(Slot, GC.MyChar);
                            Game.World.Spawn(GC.MyChar, false);
                            Database.SaveEqupsAndWH(GC.MyChar);
                        }
                        else
                            Console.WriteLine("same key is exists ***");
                    }
                    catch { Console.WriteLine("UnEquep problem created **"); }
                    
                }
            }
        }
        public static void HandleEquip(GameClient GC, byte[] Data)
        {
            if (GC.MyChar.Loc.Map == 1090)
            { return; }
            uint ItemUID = BitConverter.ToUInt32(Data, 4);
            byte Pos = Data[8];
            if (Pos >= 21)
                return;
            Game.Item I = GC.MyChar.FindInventoryItemUID(ItemUID);
            if (I.ID == 0)
            {
                GC.SendPacket(Packets.ItemPacket(ItemUID, 0, 3));
                return;
            }
            if (I.ID == 182375 || I.ID == 182335 || I.ID == 182365 || I.ID == 182345 || I.ID == 182385 || I.ID == 182355)
            {
                if (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004)
                {
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "This Germant For Females Only.Are You Gay!");
                    return;
                }
            }
            /*if (I.ID >= 117059 && I.ID <= 117119)
            {
                if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 55)
                    || (GC.MyChar.Job >= 150 && GC.MyChar.Job <= 165))
                {
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Sorry but this Earring for just Taoists!");
                    return;
                }
            }*/
            /*if ((I.ID >= 117003 && I.ID <= 117049) || (I.ID >= 134003 && I.ID <= 134059))
            {
                if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 155)
                {
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Sorry but this AvatarHeadGear for just Avatars!");
                    return;
                }
            }*/
            /*if ((I.ID >= 123000 && I.ID <= 123109) || (I.ID >= 135060 && I.ID <= 135109))
            {
                if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 45)
                    || (GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                    || (GC.MyChar.Job >= 132 && GC.MyChar.Job <= 135)
                    || (GC.MyChar.Job >= 142 && GC.MyChar.Job <= 145))
                {
                    //GC.LocalMessage(2005, System.Drawing.Color.Yellow, "This Germant For Females Only.Are You Gay!");
                    return;
                }
            }*/
            /*if ((I.ID >= 123120 && I.ID <= 123129) || (I.ID >= 135040 && I.ID <= 135049) || (I.ID >= 460139 && I.ID <= 460339))
            {
                if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 55)
                    || (GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                    || (GC.MyChar.Job >= 132 && GC.MyChar.Job <= 135)
                    || (GC.MyChar.Job >= 142 && GC.MyChar.Job <= 145))
                {
                    //GC.LocalMessage(2005, System.Drawing.Color.Yellow, "This Germant For Females Only.Are You Gay!");
                    return;
                }
            }*/
            /*if (I.ID >= 430139 && I.ID <= 430339)
            {
                if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 55)
                    || (GC.MyChar.Job >= 150 && GC.MyChar.Job <= 155)
                    || (GC.MyChar.Job >= 132 && GC.MyChar.Job <= 135)
                    || (GC.MyChar.Job >= 142 && GC.MyChar.Job <= 145))
                {
                    //GC.LocalMessage(2005, System.Drawing.Color.Yellow, "This Germant For Females Only.Are You Gay!");
                    return;
                }
            }*/
            bool Arrow = false;
            if (I.ID == 1050000 || I.ID == 1050001 || I.ID == 1050002 || I.ID == 1050020 || I.ID == 1050030
                || I.ID == 1050021 || I.ID == 1051000 || I.ID == 1050050 || I.ID == 1050031 || I.ID == 1050040
                || I.ID == 1050031 || I.ID == 1050022 || I.ID == 1050051 || I.ID == 1050032 || I.ID == 1050041
                || I.ID == 1050041 || I.ID == 1050052 || I.ID == 1050042 || I.ID == 1050023 || I.ID == 1050033 || I.ID == 1050043)
            { Pos = 5; Arrow = true; }
            if (Arrow)
            {
                if (GC.MyChar.Equips.Get(4).ID == 0)
                    return;
                else if (GC.MyChar.Equips.Get(4).ID / 10000 != 50)
                    return;
            }
            if (Pos == 0)
            {
                 GC.MyChar.UseItem(I);
            }
            else
                if (I.CanEquip(GC.MyChar))
                {
                    Game.Item Current = GC.MyChar.Equips.Get(Pos);
                 
                    if (Current.UID == 0 && (Pos != 4 || GC.MyChar.Equips.Get(5).UID == 0 || Game.ItemIDManipulation.Digit(I.ID, 1) != 5))
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                        Database.KeepItemAwyFromHand(I.UID, GC.MyChar);
                        GC.MyChar.Inventory.Remove(I.UID);
                        GC.MyChar.Equips.Replace(Pos, I, GC.MyChar);
                        GC.MyChar.EquipStats(Pos, true);
                    }
                    else if (Current.UID != 0 && (Pos != 4 || GC.MyChar.Equips.Get(5).UID == 0 || Game.ItemIDManipulation.Digit(I.ID, 1) != 5))
                    {
                        GC.MyChar.EquipStats(Pos, false);
                        GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                        Database.KeepItemAwyFromHand(I.UID, GC.MyChar);
                        GC.MyChar.Inventory.Remove(I.UID);
                        if (Current.UID == 0)
                        {
                            Current.UID = (uint)GC.MyChar.Rnd.Next(10000000);
                        }
                        GC.MyChar.Inventory.Add(Current.UID, Current);
                        GC.MyChar.MyClient.SendPacket(Packets.AddItem(Current, 0));
                        Database.ReputItemInHand(Current, GC.MyChar);
                        GC.MyChar.Equips.Replace(Pos, I, GC.MyChar);
                        GC.MyChar.EquipStats(Pos, true);
                    }
                    else if (GC.MyChar.Inventory.Count < 40)
                    {
                        if (GC.MyChar.Equips.Get(4).UID != 0)
                        {
                            GC.MyChar.EquipStats(4, false);
                            GC.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                            Database.KeepItemAwyFromHand(I.UID, GC.MyChar);
                            GC.MyChar.Inventory.Remove(I.UID);
                            if (Current.UID == 0)
                            {
                                GC.MyChar.Equips.Get(4).UID = (uint)GC.MyChar.Rnd.Next(10000000);
                            }
                            GC.MyChar.Inventory.Add(GC.MyChar.Equips.Get(4).UID, GC.MyChar.Equips.Get(4));
                            Database.ReputItemInHand(GC.MyChar.Equips.Get(4), GC.MyChar);
                            GC.MyChar.MyClient.SendPacket(Packets.AddItem(GC.MyChar.Equips.Get(4), 0));

                            GC.MyChar.Equips.Replace(4, I, GC.MyChar);
                            GC.MyChar.EquipStats(4, true);
                        }
                        GC.MyChar.EquipStats(5, false);
                        if (Current.UID == 0)
                        {
                            GC.MyChar.Equips.Get(5).UID = (uint)GC.MyChar.Rnd.Next(10000000);
                        }
                        GC.MyChar.MyClient.SendPacket(Packets.AddItem(GC.MyChar.Equips.Get(5), 0));
                        GC.MyChar.Equips.UnEquip(5, GC.MyChar);
                    }
                    Game.World.Spawn(GC.MyChar, false);
                    Database.SaveEqupsAndWH(GC.MyChar);
                }
        }
       
    }
}

