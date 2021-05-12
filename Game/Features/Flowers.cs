using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.Game;

namespace Server.Features
{
    public class Flowers
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            string P = ""; string Phex = "";
            for (byte bit = 0; bit < Data.Length - 8; bit++)
            {
                int Pi = Data[bit];
                P += Data[bit] + " ";
                Phex += Pi.ToString("X") + " ";
            }
            Console.WriteLine("packet: {0} ", P);
            Console.WriteLine("hex Packet: " + Phex);

            #region Target
            string ID = "";
            for (byte x = 18; x < 25; x++)
            {
                ID += Convert.ToChar(Data[x]).ToString();
            }

            uint TargetID = uint.Parse(ID);
            Character SaveChar = (Character)World.H_Chars[TargetID];
            GC.LocalMessage(2005, System.Drawing.Color.Red , "Sending to target id: " + TargetID);//test : D
            #endregion
            #region Type
            string Typing = "";
            for (byte x = 25; x < 32; x++)
            {
                Typing += Convert.ToChar(Data[x]).ToString();
            }
            string[] SplitValue = Typing.Split(' ');

            int Flowers = int.Parse(SplitValue[1]);
            int Type = int.Parse(SplitValue[2]);
            #endregion

            if (GC.MyChar.Body == 2001 || GC.MyChar.Body == 2002 || GC.MyChar.Body == 1002 || GC.MyChar.Body == 1001)
            { GC.LocalMessage(2005, System.Drawing.Color.Red , "You can't send flowers"); return; }

            if (World.H_Chars.Contains(TargetID))
            {
                if (World.AllFlowers.ContainsKey(SaveChar.EntityID))
                {
                    Game.Character Who = (Game.Character)World.H_Chars[TargetID];
                    Who.EntityID = TargetID;
                    Struct.Flowers F = World.AllFlowers[SaveChar.EntityID];
                    switch (Type)
                    {
                        #region Red Roses
                        case 0:
                            {
                                #region Item
                                string It = "751";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.RedRoses += Flowers;
                                F.RedRoses2day += Flowers;
                                SaveChar.Flowers = World.AllFlowers[SaveChar.EntityID];
                               // SaveChar.FlowerName = "Red Roses";
                                Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                                Who.MyClient.SendPacket2(Data);
                                //Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 0, 0, 0x74));
                                break;
                            }
                        #endregion
                        #region Lilies
                        case 1:
                            {
                                #region Item
                                string It = "752";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                               
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1);
                                GC.SendPacket2(Data);
                                F.Lilies += Flowers;
                                F.Lilies2day += Flowers;
                                SaveChar.Flowers = World.AllFlowers[SaveChar.EntityID];
                                //SaveChar.FlowerName = "Lilies"; 
                              Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                              Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        #region Orchids
                        case 2:
                            {
                                #region Item
                                string It = "753";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                #endregion
                               
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); 
                                GC.SendPacket2(Data);
                                F.Orchads += Flowers;
                                F.Orchads2day += Flowers;
                                SaveChar.Flowers = World.AllFlowers[SaveChar.EntityID];
                             //   SaveChar.FlowerName = "Orchids";
                              Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                              Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        #region Tulips
                        case 3:
                            {
                                #region Item
                                string It = "754";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999"; 
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.Tulips += Flowers;
                                F.Tulips2day += Flowers;
                                SaveChar.Flowers = World.AllFlowers[SaveChar.EntityID];
                            //    SaveChar.FlowerName = "Tulips";
                               Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                               Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                    }
                 //   Packets.RankFlowerPacket("Tulips", 1,GC);
                    //Database.SaveFlowerRank(SaveChar);
                    
                    GC.WorldMessage(2011, "What a love " + GC.MyChar.Name + " has sent " + Flowers.ToString() + " " + SaveChar.FlowerName + " To Precious " + SaveChar.Name + "",System.Drawing.Color.White);
                }
                else
                {
                    Game.Character Who = (Game.Character)World.H_Chars[TargetID];
                    Who.EntityID = TargetID;
                    Struct.Flowers F = new Struct.Flowers();
                    switch (Type)
                    {
                        #region Red Roses
                        case 0:
                            {
                                #region Item
                                string It = "751";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.RedRoses += Flowers;
                                F.RedRoses2day += Flowers;
                               // SaveChar.FlowerName = "Red Roses";
                               Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                               Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        #region Lilies
                        case 1:
                            {
                                #region Item
                                string It = "752";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999"; 
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.Lilies += Flowers;
                                F.Lilies2day += Flowers;
                               // SaveChar.FlowerName = "Lilies";
                                Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                                Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        #region Orchids
                        case 2:
                            {
                                #region Item
                                string It = "753";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.Orchads += Flowers;
                                F.Orchads2day += Flowers;
                                ///SaveChar.FlowerName = "Orchids";
                              Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                              Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        #region Tulips
                        case 3:
                            {
                                #region Item
                                string It = "754";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                #endregion
                                //GC.LocalMessage(2005, System.Drawing.Color.Red , "Item id : " + It);
                                GC.MyChar.RemoveItemUIDAmount(uint.Parse(It), 1); GC.SendPacket2(Data);
                                F.Tulips += Flowers;
                                F.Tulips2day += Flowers;
                            //    SaveChar.FlowerName = "Tulips";
                              Who.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x4e0, 000, 000, 0x74));
                              Who.MyClient.SendPacket2(Data);
                                break;
                            }
                        #endregion
                        default:
                            Console.WriteLine("Unknown Flower type " + Type);
                            break;
                    }
                   // Packets.RankFlowerPacket("Tulips", 1,GC);
                    World.AllFlowers.Add(SaveChar.EntityID, F);
                    SaveChar.Flowers = World.AllFlowers[SaveChar.EntityID];
                    GC.WorldMessage(2011, "What a love! " + GC.MyChar.Name + " has sent " + Flowers.ToString() + " " + SaveChar.FlowerName + " to your loved " + SaveChar.Name + "...",System.Drawing.Color.White);
                } 
                //Database.SaveFlowerRank(SaveChar);

                // Database.SaveFlowerRank(SaveChar);
            }
            else
            {
                GC.LocalMessage(2005, System.Drawing.Color.Red , "The target player isn't online right now.");
            }
        }
    }
    public partial class Struct
    {
        public class Flowers
        {
            public int RedRoses;
            public int RedRoses2day;
            public int Lilies;
            public int Lilies2day;
            public int Orchads;
            public int Orchads2day;
            public int Tulips;
            public int Tulips2day;
            public int Ammount;
        }
    }
}