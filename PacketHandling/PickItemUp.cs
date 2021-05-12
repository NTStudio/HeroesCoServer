using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    public class PickItemUp
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint IUID = BitConverter.ToUInt32(Data, 4);

            if (((Hashtable)Game.World.H_Items[GC.MyChar.Loc.Map]).Contains(IUID) && GC.MyChar.MyShop == null)
            {
                Game.DroppedItem DI = (Game.DroppedItem)((Hashtable)Game.World.H_Items[GC.MyChar.Loc.Map])[IUID];

                if (DI.Info.ID != 0 && DI.Owner == GC.MyChar.EntityID || (GC.MyChar.MyAvatar != null && DI.Owner == GC.MyChar.MyAvatar.EntityID) || DI.Owner == 0 || DateTime.Now > DI.DropTime.AddSeconds(15))
                {                    
                    if (DI.Silvers > 0)
                    {
                        GC.MyChar.Silvers += DI.Silvers;
                        DI.Dissappear();
                        GC.LocalMessage(2005, System.Drawing.Color.Red , "You have picked up " + DI.Silvers + " Silvers.");
                    }
                    else
                    {
                        if (GC.MyChar.Inventory.Count < 40)
                        {
                            if (DI.someBodyDropedIt)
                                GC.MyChar.ReturnItemToInventory(DI.Info);
                            else
                                GC.MyChar.CreateReadyItem(DI.Info);
                            DI.someBodyDropedIt = false;
                            DI.Dissappear();
                            GC.LocalMessage(2005, System.Drawing.Color.Red , "You have picked up a(n) " + DI.Info.ItemDBInfo.Name + ".");
                        }
                        else
                            GC.LocalMessage(2005, System.Drawing.Color.Red , "Your inventory is full.");
                    }
                }
                else
                    GC.LocalMessage(2005, System.Drawing.Color.Red , "You have to wait a while before picking up items dropped by monsters killed by other players.");
            }
        }
    }
}
