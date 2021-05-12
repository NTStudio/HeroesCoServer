using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.PacketHandling
{
    public class TradePartener
    {
        public static void Status(GameClient GC,bool Loging)
        {
            if (Loging)
            {
                Game.Character[] BaseCharacters = new Character[World.H_Chars.Count];
                World.H_Chars.Values.CopyTo(BaseCharacters, 0);
                try
                {
                    foreach (TradePartner tp in GC.MyChar.Partners.Values)
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.TradePartner("", tp.UID, false, DateTime.Now, 4));
                        GC.MyChar.MyClient.SendPacket(Packets.TradePartner(tp.Name, tp.UID, false, DateTime.FromBinary(GC.MyChar.TimePartner), 5));
                    }
                }
                catch { }
                try
                {
                    foreach (Game.Character C in BaseCharacters)
                    {
                        try
                        {
                            if (GC.MyChar.Partners.ContainsKey(C.EntityID))
                            {
                                C.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You Partener " + GC.MyChar.Name + " has coming game");
                                C.MyClient.SendPacket(Packets.TradePartner("", GC.MyChar.EntityID, false, DateTime.Now, 4));
                                C.MyClient.SendPacket(Packets.TradePartner(GC.MyChar.Name,GC.MyChar.EntityID,true,DateTime.FromBinary(GC.MyChar.TimePartner),5));
                                GC.MyChar.MyClient.SendPacket(Packets.TradePartner("", C.EntityID, false, DateTime.Now, 4));
                                GC.MyChar.MyClient.SendPacket(Packets.TradePartner(C.Name, C.EntityID, true, DateTime.FromBinary(GC.MyChar.TimePartner), 5));
                            }

                        }
                        catch { continue; }
                    }
                }
                catch { }
            }
            else
            {
                Game.Character[] BaseCharacters = new Character[World.H_Chars.Count];
                World.H_Chars.Values.CopyTo(BaseCharacters, 0);

                try
                {
                    foreach (Game.Character C in BaseCharacters)
                    {
                        try
                        {
                            if (C.Partners.ContainsKey(GC.MyChar.EntityID))
                            {
                                C.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You Partener " + GC.MyChar.Name + " has left game");
                                C.MyClient.SendPacket(Packets.TradePartner("", GC.MyChar.EntityID, false, DateTime.Now, 4));
                                C.MyClient.SendPacket(Packets.TradePartner(GC.MyChar.Name, GC.MyChar.EntityID, false, DateTime.FromBinary(GC.MyChar.TimePartner), 5));
      
                            }

                        }
                        catch { continue; }
                    }
                }
                catch { }
            }
        }
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint UID = BitConverter.ToUInt32(Data, 4);
            byte Type = Data[8];
            Character C = (Character)World.H_Chars[UID];
            switch (Type)
            {

                case 0://Add
                    {
                        if (!GC.MyChar.Partners.ContainsKey(UID))
                        {
                            if (C != null)
                            {
                                if (C.TradePartnerWith != GC.MyChar.EntityID)
                                {
                                    GC.MyChar.TradePartnerWith = UID;
                                    C.MyClient.SendPacket(Packets.TradePartner(GC.MyChar.Name, GC.MyChar.EntityID, true, DateTime.Now.AddHours(0), 0));
                                }
                                else
                                {
                                    TradePartner TP = new TradePartner();
                                    TP.Name = C.Name;
                                    TP.UID = C.EntityID;
                                    TP.ProbationStartedOn = DateTime.Now;
                                    TradePartner TP2 = new TradePartner();
                                    TP2.Name = GC.MyChar.Name;
                                    TP2.UID = GC.MyChar.EntityID;
                                    TP2.ProbationStartedOn = DateTime.Now;
                                    GC.MyChar.Partners.Add(TP.UID, TP);
                                    C.Partners.Add(TP2.UID, TP2);
                                    GC.SendPacket(Packets.TradePartner(TP.Name, UID, true, DateTime.Now, 5));
                                    C.MyClient.SendPacket(Packets.TradePartner(TP2.Name, TP2.UID, true, DateTime.Now, 5));
                                    Database.SavePartner(TP.UID, TP.Name, (long)DateTime.Now.Ticks, GC.MyChar);
                                    Database.SavePartner(TP2.UID, TP2.Name, (long)DateTime.Now.Ticks, C);
                                }
                            }
                        }
                        break;
                    }
                case 1: //reject
                    {
                        if (C.RequestPartnerWith == GC.MyChar.EntityID)
                        {
                            C.MyClient.SendPacket(Packets.TradePartner(GC.MyChar.Name, GC.MyChar.EntityID, true, DateTime.Now, 1));
                        }
                        break;
                    }
                case 4: //delete partner
                    {
                        try
                        {
                            if (GC.MyChar.Partners.ContainsKey(C.EntityID))
                            {
                                Database.DeletePartner(GC.MyChar.EntityID, UID);
                                Database.DeletePartner(UID, GC.MyChar.EntityID);
                                GC.MyChar.Partners.Remove(C.EntityID);
                                if (Game.World.H_Chars.Contains(UID))
                                {
                                    C.Partners.Remove(GC.MyChar.EntityID);
                                    C.MyClient.SendPacket(Packets.TradePartner("", GC.MyChar.EntityID, false, DateTime.Now, 4));
                                }
                            }
                        }
                        catch
                        {
                            Database.DeletePartner(GC.MyChar.EntityID, UID);
                            Database.DeletePartner(UID, GC.MyChar.EntityID);
                            GC.MyChar.Partners.Remove(UID);
                  
                        }
                        GC.MyChar.MyClient.SendPacket(Packets.TradePartner("", UID, false, DateTime.Now, 4));
                        break;
                    }
                default:
                    Console.WriteLine(Type);
                    break;
            }
        }
    }
}

