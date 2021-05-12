using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class MeteorUpgrade
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint EquipUID = BitConverter.ToUInt32(Data, 4);
            uint MeteorUID = BitConverter.ToUInt32(Data, 8);

            Game.Item Equip = new Game.Item();//GC.MyChar.FindInvItem(EquipUID);
            Game.Item Meteor = new Game.Item();// GC.MyChar.FindInvItem(MeteorUID);



            foreach (Game.Item II in GC.MyChar.Inventory.Values)
            {
                if (II.UID == EquipUID)
                {
                    Equip = II;
                }
            }
            foreach (Game.Item Ia in GC.MyChar.Inventory.Values)
            {
                if (Ia.UID == MeteorUID)
                {
                    Meteor = Ia;
                }
            }
            if (Meteor.ID == 1088001)
            {
                Game.ItemIDManipulation E = new Game.ItemIDManipulation(Equip.ID);

                sbyte Chance = (sbyte)(100 - (Equip.ItemDBInfo.Level / 1.3));
                byte Quality = (byte)E.Quality;

                if (Quality < 6)
                    Chance += 10;

                E.IncreaseLevel();
                DatabaseItem Di = (DatabaseItem)Database.DatabaseItems[E.ToID()];

                byte NewLevel = Di.Level;
                if (NewLevel > Equip.ItemDBInfo.Level)
                {
                    if (MyMath.ChanceSuccess(Chance))
                    {
                        Equip.ID = E.ToID();
                        GC.MyChar.UpdateItem(Equip);
                    }
                    if (!Chance.ToString().Contains('-'))
                        GC.MyChar.RemoveItemUIDAmount(Meteor.ID, 1);
                }
            }
            if (Meteor.ID == 1088002)
            {
                Game.ItemIDManipulation E = new Game.ItemIDManipulation(Equip.ID);

                sbyte Chance = (sbyte)(100 - (Equip.ItemDBInfo.Level / 1.3));
                byte Quality = (byte)E.Quality;

                if (Quality < 6)
                    Chance += 10;
                if (Quality == 7)
                    Chance -= 5;
                if (Quality == 8)
                    Chance -= 20;
                if (Quality == 9)
                    Chance -= 30;

                E.IncreaseLevel();
                DatabaseItem Di = (DatabaseItem)Database.DatabaseItems[E.ToID()];
                byte NewLevel = Di.Level;
                if (NewLevel > Equip.ItemDBInfo.Level)
                {
                    if (MyMath.ChanceSuccess(Chance))
                    {

                        Equip.ID = E.ToID();
                        GC.MyChar.UpdateItem(Equip);
                    }
                    if (!Chance.ToString().Contains('-'))
                        GC.MyChar.RemoveItemUIDAmount(Meteor.UID, 1);
                }
            }
        }
    }
}
