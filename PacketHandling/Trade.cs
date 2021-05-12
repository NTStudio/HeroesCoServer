using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Server.Game;

namespace Server.PacketHandling
{
    public class Trade
    {
        static void CancelTrade(GameClient C)
        {
            if (C.MyChar.Trading)
            {
                Character Who = (Character)World.H_Chars[C.MyChar.TradingWith];
                if (Who != null)
                {
                    Who.MyClient.SendPacket(Packets.TradePacket(C.MyChar.TradingWith, 5));
                    Who.Trading = false;
                    Who.TradingWith = 0;
                    Who.TradeSide.Clear();
                    Who.TradingCPs = 0;
                    Who.TradingSilvers = 0;
                    Who.ClickedOK = false;
                    Who.Silvers = Who.Silvers;//update the silvers
                    Who.CPs = Who.CPs;//update the cps
                    Who.MyClient.SendPacket(Packets.ChatMessage(Who.MyClient.MessageID, "SYSTEM", Who.Name, "Trading failed!", 2005, 0, System.Drawing.Color.White));
                }
                C.SendPacket(Packets.TradePacket(C.MyChar.TradingWith, 5));
                C.MyChar.Trading = false;
                C.MyChar.TradingWith = 0;
                C.MyChar.TradeSide = new System.Collections.ArrayList(20);
                C.MyChar.TradingCPs = 0;
                C.MyChar.TradingSilvers = 0;
                C.MyChar.ClickedOK = false;
                C.MyChar.CPs = C.MyChar.CPs;//update the cps
                C.MyChar.Silvers = C.MyChar.Silvers;//update the silvers
                C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "Trading failed!", 2005, 0, System.Drawing.Color.White));
            }
        }
        public static void Handle(GameClient C, byte[] Data)
        {
            uint UID = BitConverter.ToUInt32(Data, 4);
            byte Type = Data[8];

            switch (Type)
            {
                case 1:
                    {
                        Character Who = (Character)World.H_Chars[UID];
                        if (Who != null && !Who.Trading)
                        {
                            if (!C.MyChar.Trading)
                            {
                                if (Who.EntityID != C.MyChar.TradingWith)
                                {
                                    C.MyChar.TradingWith = UID;
                                    if (Who.EntityID == C.MyChar.TradingWith && Who.TradingWith == C.MyChar.EntityID)
                                    {
                                        Who.MyClient.SendPacket(Packets.TradePacket(C.MyChar.EntityID, 3));
                                        C.SendPacket(Packets.TradePacket(Who.EntityID, 3));
                                        C.MyChar.Trading = true;
                                        Who.Trading = true;
                                        break;
                                    }
                                    else
                                    {
                                        C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]Request for trading has been sent out.", 2005, 0, System.Drawing.Color.White));
                                        Who.MyClient.SendPacket(Packets.TradePacket(C.MyChar.EntityID, 1));
                                    }
                                }
                                if (Who.EntityID == C.MyChar.TradingWith && Who.TradingWith == C.MyChar.EntityID)
                                {
                                    Who.MyClient.SendPacket(Packets.TradePacket(C.MyChar.EntityID, 3));
                                    C.SendPacket(Packets.TradePacket(Who.EntityID, 3));
                                    C.MyChar.Trading = true;
                                    Who.Trading = true;
                                }
                            }
                            else
                                C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]Close the current trade before you take another one.", 2005, 0, System.Drawing.Color.White));
                        }
                        else
                            C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]The target is trading with someone else.", 2005, 0, System.Drawing.Color.White));

                        break;
                    }
                case 2:
                    {
                        CancelTrade(C);
                        break;
                    }
                case 6:
                    {
                        Character Who = (Character)World.H_Chars[C.MyChar.TradingWith];
                        if (Who != null)
                        {
                            if (C.MyChar.TradeSide.Count < 20)
                            {
                                if (Who.Inventory.Count + C.MyChar.TradeSide.Count < 40)
                                {
                                   // Game.Item I = C.MyChar.FindInvItem(UID);
                                    Game.Item I = new Game.Item();
                                    foreach (Game.Item Is in C.MyChar.Inventory.Values)
                                    {
                                        if (Is.UID == UID)
                                        {
                                            I = Is;
                                        }
                                    }
                                    if (I.ID != 729960 && I.ID != 729961 && I.ID != 729962 &&
                    I.ID != 729963 && I.ID != 729964 && I.ID != 729965 &&
                    I.ID != 729966 && I.ID != 729967 && I.ID != 729968 &&
                    I.ID != 729969 && I.ID != 729970)
                                    {
                                        if (!I.FreeItem)
                                        {
                                            Who.MyClient.SendPacket(Packets.TradeItem(I));
                                            C.MyChar.TradeSide.Add(I.UID);
                                        }
                                        else
                                        {
                                            C.SendPacket(Packets.TradePacket(UID, 11));
                                            C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]Cannot trade items made with gm commands.", 2005, 0, System.Drawing.Color.White));
                                        }
                                    }
                                    else
                                    {
                                        C.SendPacket(Packets.TradePacket(UID, 11));
                                        C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]Cannot trade this items.", 2005, 0, System.Drawing.Color.White));
                                    }
                                }
                                else
                                {
                                    C.SendPacket(Packets.TradePacket(UID, 11));
                                    C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]Your trade partner can't hold any more items.", 2005, 0, System.Drawing.Color.White));
                                    Who.MyClient.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "[Trade]The one your trading with cant add anymore items on the table because you have no room in your inventory.", 2005, 0, System.Drawing.Color.White));
                                }
                            }

                        }
                        break;
                    }
                case 7:
                    {
                        C.MyChar.TradingSilvers = UID;
                        Character Who = (Character)World.H_Chars[C.MyChar.TradingWith];
                        Who.MyClient.SendPacket(Packets.TradePacket(UID, 8));

                        break;
                    }
                case 13:
                    {
                        C.MyChar.TradingCPs = UID;
                        Character Who = (Character)World.H_Chars[C.MyChar.TradingWith];
                        Who.MyClient.SendPacket(Packets.TradePacket(UID, 12));

                        break;
                    }
                case 10:
                    {
                        Character Who = (Character)World.H_Chars[C.MyChar.TradingWith];

                        if (Who != null && Who.ClickedOK)
                        {
                            if (C.MyChar.Silvers >= C.MyChar.TradingSilvers && C.MyChar.CPs >= C.MyChar.TradingCPs && Who.Silvers >= Who.TradingSilvers && Who.CPs >= Who.TradingCPs)
                            {
                                Who.MyClient.SendPacket(Packets.TradePacket(C.MyChar.TradingWith, 5));
                                C.SendPacket(Packets.TradePacket(C.MyChar.EntityID, 5));

                                Who.Silvers += C.MyChar.TradingSilvers;
                                Who.Silvers -= Who.TradingSilvers;
                                C.MyChar.Silvers += Who.TradingSilvers;
                                C.MyChar.Silvers -= C.MyChar.TradingSilvers;

                                Who.CPs += C.MyChar.TradingCPs;
                                Who.CPs -= Who.TradingCPs;
                                C.MyChar.CPs += Who.TradingCPs;
                                C.MyChar.CPs -= C.MyChar.TradingCPs;
                                StreamWriter sw;
                                if (!File.Exists(Program.ConquerPath + @"Trades/" + C.MyChar.Name + ".txt"))
                                    Creat(C.MyChar.Name);
                                    
                                sw = File.AppendText(Program.ConquerPath + @"Trades/" + C.MyChar.Name + ".txt"); sw.WriteLine("TradeWith " + Who.Name + " AT " + DateTime.Now.Month + "/ " + DateTime.Now.Day + " [" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + "] * " + Who.Name + " ( takeCPs [ " + C.MyChar.TradingCPs + " ] TakeSilvers [ " + C.MyChar.TradingSilvers + " ]) ** " + C.MyChar.Name + " (TakeCPs[ " + Who.TradingCPs + " ]TakeSilvers[ " + Who.TradingSilvers + " ])");
                                sw.Close();
                                   if (!File.Exists(Program.ConquerPath + @"Trades/" + Who.Name + ".txt"))
                                       Creat(Who.Name);
                                    
                                    sw = File.AppendText(Program.ConquerPath + @"Trades/" + Who.Name + ".txt"); sw.WriteLine("TradeWith " + C.MyChar.Name + " AT " + DateTime.Now.Month + "/ " + DateTime.Now.Day + " [" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + "] * " + C.MyChar.Name + " ( takeCPs [ " + Who.TradingCPs + " ] TakeSilvers [ " + Who.TradingSilvers + " ]) ** " + Who.Name + " (TakeCPs[ " + C.MyChar.TradingCPs + " ]TakeSilvers[ " + C.MyChar.TradingSilvers + " ])");
                                    sw.Close();

                                foreach (uint Id in C.MyChar.TradeSide)
                                {
                                   
                                    Game.Item I = new Game.Item();
                                    foreach (Game.Item Is in C.MyChar.Inventory.Values)
                                    {
                                        if (Is.UID == Id)
                                        {
                                            I = Is;
                                            break;
                                        }
                                    }

                                    C.MyChar.TradeItem(I, Who);
                                    //Who.AddFullItem(I.ID, I.Bless, I.Plus, I.Enchant, I.Soc1, I.Soc2, I.Color, I.Progress, I.TalismanProgress, I.Effect, I.FreeItem, I.CurDur, I.MaxDur, I.Suspicious, I.Locked, I.LockedDays, I.RBG[0], I.RBG[1], I.RBG[2], I.RBG[3]);
                                    //Database.DeleteItem(I.UID, C.MyChar.EntityID);
                                    //C.MyChar.Inventory.Remove(I.UID);
                                    //C.MyChar.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));
                                  
                                }
                                foreach (uint Id in Who.TradeSide)
                                {
                                   
                                    Game.Item I = new Game.Item();
                                    foreach (Game.Item Is in Who.Inventory.Values)
                                    {
                                        if (Is.UID == Id)
                                        {
                                            I = Is;
                                            break;
                                        }
                                    }
                                    Who.TradeItem(I, C.MyChar);
                                    //C.MyChar.AddFullItem(I.ID, I.Bless, I.Plus, I.Enchant, I.Soc1, I.Soc2, I.Color, I.Progress, I.TalismanProgress, I.Effect, I.FreeItem, I.CurDur, I.MaxDur, I.Suspicious, I.Locked, I.LockedDays, I.RBG[0], I.RBG[1], I.RBG[2], I.RBG[3]);
                                    //Database.DeleteItem(I.UID, Who.EntityID);
                                    //Who.Inventory.Remove(I.UID);
                                    //Who.MyClient.SendPacket(Packets.ItemPacket(I.UID, 0, 3));

                                    
                                }

                                Who.Trading = false;
                                Who.TradingWith = 0;
                                Who.TradeSide = new System.Collections.ArrayList(20);
                                Who.TradingCPs = 0;
                                Who.TradingSilvers = 0;
                                Who.ClickedOK = false;
                                Who.MyClient.SendPacket(Packets.ChatMessage(Who.MyClient.MessageID, "SYSTEM", Who.Name, "Trading succeeded!", 2005, 0, System.Drawing.Color.White));
                                C.MyChar.Trading = false;
                                C.MyChar.TradingWith = 0;
                                C.MyChar.TradeSide = new System.Collections.ArrayList(20);
                                C.MyChar.TradingCPs = 0;
                                C.MyChar.TradingSilvers = 0;
                                C.MyChar.ClickedOK = false;
                                C.SendPacket(Packets.ChatMessage(C.MessageID, "SYSTEM", C.MyChar.Name, "Trading succeeded!", 2005, 0, System.Drawing.Color.White));
                            }
                            else
                                CancelTrade(C);
                        }
                        else
                        {
                            C.MyChar.ClickedOK = true;
                            Who.MyClient.SendPacket(Packets.TradePacket(0, 10));
                        }

                        break;
                    }
            }
        }
        static void Creat(string Name)
        {
            FileStream fs = new FileStream(Program.ConquerPath + @"Trades\" + Name + ".txt", FileMode.Create);
            fs.Close();
        }
    }
}


