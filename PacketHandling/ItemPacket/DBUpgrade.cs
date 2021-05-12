using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling.ItemPacket
{
    public class DBUpgrade
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint EquipUID = BitConverter.ToUInt32(Data, 4);
            uint MeteorUID = BitConverter.ToUInt32(Data, 8);

            //Game.Item Equip = GC.MyChar.FindInvItem(EquipUID);
            //Game.Item DragonBall = GC.MyChar.FindInvItem(MeteorUID);
            Game.Item Equip = new Game.Item();
            Game.Item DragonBall = new Game.Item();

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
                    DragonBall = Ia;
                }
            }



            if (DragonBall.ID == 1088000)
            {
                Game.ItemIDManipulation E = new Game.ItemIDManipulation(Equip.ID);

                sbyte Chance = (sbyte)(100 - (Equip.ItemDBInfo.Level / 3));
                byte Quality = (byte)E.Quality;

                if (Quality == 6)
                    Chance -= 25;
                else if (Quality == 7)
                    Chance -= 40;
                else if (Quality == 8)
                    Chance -= 70;

                if (Quality < 9)
                {
                    if (Quality < 5) Quality = 5;
                    E.QualityChange((Game.Item.ItemQuality)(Quality + 1));
                    Chance += 16;
                    if (MyMath.ChanceSuccess(Chance))
                    {
                        //GC.MyChar.RemoveItemI(Equip.UID, 1);
                        Equip.ID = E.ToID();
                        GC.MyChar.UpdateItem(Equip); 
                        //GC.MyChar.AddFullItem(Equip.ID, Equip.Bless, Equip.Plus, Equip.Enchant, Equip.Soc1, Equip.Soc2, Equip.Color, Equip.Progress, Equip.TalismanProgress, Equip.Effect, Equip.FreeItem, Equip.CurDur, Equip.MaxDur, Equip.Suspicious, Equip.Locked,Equip.LockedDays,Equip.RBG[0],Equip.RBG[1],Equip.RBG[2],Equip.RBG[3]);
                    }
                    GC.MyChar.RemoveItemUIDAmount(DragonBall.UID, 1);
                }
            }
        }
    }
}
