using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.Features
{
    public class Mining
    {
        public static void Swing(Game.Character MyChar)
        {
            if (!MyChar.Alive)
            { MyChar.Mining = false; return; }
            if (MyChar.Equips.RightHand.ID == 0)
            { MyChar.Mining = false; return; }
            if (MyChar.Equips.LeftHand.ID != 0)
            { MyChar.Mining = false; return; }
            if (MyChar.Equips.RightHand.ItemDBInfo.Name != "Hoe" && MyChar.Equips.RightHand.ItemDBInfo.Name != "PickAxe")
            { MyChar.Mining = false; return; }
            Game.World.Action(MyChar, Packets.GeneralData(MyChar.EntityID, 0, MyChar.Loc.X, MyChar.Loc.Y, 99));
            MyChar.Action = 100;

            switch (MyChar.Loc.Map)
            {
                case 1025://meteor zone mine
                case 6001://jail war mine
                case 6000://jails
                    {
                        Mine(700001, 700011, 700031, 700041, 1072010, 1072020, 1072050, 0, MyChar);
                        break;
                    }
                case 1028://twincity minecave
                    {
                        Mine(700011, 700001, 700021, 700071, 1072010, 1072050, 1072031, 0, MyChar);
                        break;
                    }
                case 1027://DesertMine
                case 1026://ApeMine
                    {
                        Mine(700051, 700061, 0, 0, 1072010, 1072020, 1072040, 1072050, MyChar);
                        break;
                    }
                default:
                    {
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You cannot mine here go else-where.");
                        MyChar.Mining = false;
                        break;
                    }
            }
        }

        static void Mine(uint GemID, uint GemID2, uint GemID3, uint GemID4, uint Ore1, uint Ore2, uint Ore3, uint Ore4, Game.Character MyChar)
        {
            if (MyChar.Inventory.Count <= 39)
            {
                Random Rnd = new Random();

                if (Ore1 != 0 && MyMath.ChanceSuccess(30))//ores type 1
                {
                    if (Ore1 != 1072031) { Random rnd = new Random(); Ore1 += (uint)rnd.Next(0, 9); }

                    if (Database.DatabaseItems.Contains(Ore1))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = Ore1;
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (Ore2 != 0 && MyMath.ChanceSuccess(0.29))//ores type 2
                {
                    if (Ore2 != 1072031) { Random rnd = new Random(); Ore2 += (uint)rnd.Next(0, 9); }

                    if (Database.DatabaseItems.Contains(Ore2))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = Ore2;
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (Ore3 != 0 && MyMath.ChanceSuccess(10))//ores type 3
                {
                    if (Ore3 != 1072031) { Random rnd = new Random(); Ore3 += (uint)rnd.Next(0, 9); }

                    if (Database.DatabaseItems.Contains(Ore3))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = Ore3;
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (Ore4 != 0 && MyMath.ChanceSuccess(0.27))//ores type 4
                {
                    if (Ore4 != 1072031) { Random rnd = new Random(); Ore4 += (uint)rnd.Next(0, 9); }

                    if (Database.DatabaseItems.Contains(Ore4))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = Ore4;
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (GemID != 0 && MyMath.ChanceSuccess(0.09))
                {
                    int add = 0;
                    if (MyMath.ChanceSuccess(DropRates.RefinedGem)) { add = 1; }
                    else if (MyMath.ChanceSuccess(DropRates.SuperGem)) { add = 2; }
                    else { add = 0; }

                    if (Database.DatabaseItems.Contains((uint)(GemID + add)))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = (uint)(GemID + add);
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);
                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (GemID2 != 0 && MyMath.ChanceSuccess(0.007))
                {
                    int add = 0;
                    if (MyMath.ChanceSuccess(DropRates.RefinedGem)) { add = 1; }
                    else if (MyMath.ChanceSuccess(DropRates.SuperGem)) { add = 2; }
                    else { add = 0; }

                    if (Database.DatabaseItems.Contains((uint)(GemID2 + add)))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = (uint)(GemID2 + add);
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);

                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (GemID3 != 0 && MyMath.ChanceSuccess(0.005))
                {
                    int add = 0;
                    if (MyMath.ChanceSuccess(DropRates.RefinedGem)) { add = 1; }
                    else if (MyMath.ChanceSuccess(DropRates.SuperGem)) { add = 2; }
                    else { add = 0; }

                    if (Database.DatabaseItems.Contains((uint)(GemID3 + add)))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = (uint)(GemID3 + add);
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);

                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have found a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
                if (GemID4 != 0 && MyMath.ChanceSuccess(0.003))
                {
                    int add = 0;
                    if (MyMath.ChanceSuccess(DropRates.RefinedGem)) { add = 1; }
                    else if (MyMath.ChanceSuccess(DropRates.SuperGem)) { add = 2; }
                    else { add = 0; }

                    if (Database.DatabaseItems.Contains((uint)(GemID4 + add)))
                    {
                        Item I = new Item();
                        I.UID = (uint)Rnd.Next(10000000);
                        I.ID = (uint)(GemID4 + add);
                        I.MaxDur = 1;
                        I.CurDur = 1;
                        MyChar.CreateReadyItem(I);

                        MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red , "You have gained a " + I.ItemDBInfo.Name + ".");
                        return;
                    }
                }
            }
        }
    }
}
