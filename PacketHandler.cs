using System;
using System.Collections;
using Server.Game;
using System.Threading;
using System.IO;

namespace Server
{
    public class PacketHandler
    {
//        #region HandleConnection
//        public static void WriteString(string arg, int offset, byte[] buffer)
//        {
//            try
//            {
//                if (buffer.Length >= offset + arg.Length)
//                {
//                    unsafe
//                    {
//#if UNSAFE
//                    fixed (byte* Buffer = buffer)
//                    {
//                        ushort i = 0;
//                        while (i < arg.Length)
//                        {
//                            *((byte*)(Buffer + offset + i)) = (byte)arg[i];
//                            i++;
//                        }
//                    }
//#else
//                        ushort i = 0;
//                        while (i < arg.Length)
//                        {
//                            buffer[(ushort)(i + offset)] = (byte)arg[i];
//                            i = (ushort)(i + 1);
//                        }
//#endif
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Program.WriteMessage(e);
//            }
//        }
//        public static void HandleBuffer(byte[] buffer, Main.GameClient client)
//        {

//            if (buffer == null)
//                return;
//            if (client == null)
//                return;
//        roleAgain:
//            ushort Length = BitConverter.ToUInt16(buffer, 0);
//            string Sign = "";
//            for (int i = Length; i < buffer.Length; i++)
//            {
//                Sign += Convert.ToChar(buffer[i]);
//            }

//            if ((Length + 8) == buffer.Length)
//            {
//                if (Sign == "TQServer") //WriteString("TQServer", (buffer.Length - 8), buffer);
//                    Handle(client, buffer);
//                else
//                    Console.WriteLine("warrning_____________ Un Signed Packet Recived " + BitConverter.ToUInt16(buffer, 2));
//                return;
//            }
//            else if ((Length + 8) > buffer.Length)
//            {
//                return;
//            }
//            else
//            {
//                byte[] Packet = new byte[(Length + 8)];
//                Buffer.BlockCopy(buffer, 0, Packet, 0, (Length + 8));
//                byte[] _buffer = new byte[(buffer.Length - (Length + 8))];
//                Buffer.BlockCopy(buffer, (Length + 8), _buffer, 0, (buffer.Length - (Length + 8)));
//                buffer = _buffer;
//                WriteString("TQServer", (Packet.Length - 8), Packet);
//                if (Sign == "TQServer") //WriteString("TQServer", (buffer.Length - 8), buffer);
//                    Handle(client, Packet);
//                else
//                    Console.WriteLine("warrning_____________ Un Signed Packet Recived " + BitConverter.ToUInt16(buffer, 2));
//                goto roleAgain;
//            }
//        }
//        #endregion
        #region HandleConnection
        public static void WriteString(string arg, int offset, byte[] buffer)
        {
            try
            {
                if (buffer.Length >= offset + arg.Length)
                {
                    unsafe
                    {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        ushort i = 0;
                        while (i < arg.Length)
                        {
                            *((byte*)(Buffer + offset + i)) = (byte)arg[i];
                            i++;
                        }
                    }
#else
                        ushort i = 0;
                        while (i < arg.Length)
                        {
                            buffer[(ushort)(i + offset)] = (byte)arg[i];
                            i = (ushort)(i + 1);
                        }
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Program.WriteMessage(e);
            }
        }
        public static void HandleBuffer(byte[] buffer, GameClient client)
        {

            if (buffer == null)
                return;
            if (client == null)
                return;
        roleAgain:
            ushort Length = BitConverter.ToUInt16(buffer, 0);
            if ((Length + 8) == buffer.Length)
            {
                WriteString("TQServer", (buffer.Length - 8), buffer);
                Handle(client, buffer);
                return;
            }
            else if ((Length + 8) > buffer.Length)
            {
                return;
            }
            else
            {
                byte[] Packet = new byte[(Length + 8)];
                Buffer.BlockCopy(buffer, 0, Packet, 0, (Length + 8));
                byte[] _buffer = new byte[(buffer.Length - (Length + 8))];
                Buffer.BlockCopy(buffer, (Length + 8), _buffer, 0, (buffer.Length - (Length + 8)));
                buffer = _buffer;
                WriteString("TQServer", (Packet.Length - 8), Packet);
                Handle(client, Packet);
                goto roleAgain;
            }
        }
        #endregion

        public static unsafe void Handle(GameClient GC, byte[] Data)
        {
            if (Data == null)
                return;
            if (GC == null)
                return;
            ushort PacketID = BitConverter.ToUInt16(Data, 2);
            Random Rnd = new Random();

            try
            {
                if ((GC.MyChar != null && GC.MyChar.Name == "ProjectManager") || (GC.MyChar != null && GC.MyChar.Name == "GameMaster"))
                {
                    GC.LocalMessage(2005, System.Drawing.Color.YellowGreen, "PacketID Is " + PacketID + " PacketType Is " + Data[12]);
                    //GC.LocalMessage(2005, System.Drawing.Color.YellowGreen, "paketID is " + PacketID + "[1 is" + Data[1] + "[2 is" + Data[2] + "[3 is" + Data[3] + "[4 is" + Data[4] + "[5 is" + Data[5] + "[6 is" + Data[6] + "[7 is" + Data[7] + "[8 is" + Data[8] + "[9 is" + Data[9] + "[10 is" + Data[10] + "[11 is" + Data[11] + "[12 is" + Data[12]);
                }
                Console.WriteLine("PacketID Is " + PacketID + " PacketType Is " + Data[12]);
                switch (PacketID)
                {
                    #region Nobiliti Icon

                    case 2064:
                        {
                            uint Type = BitConverter.ToUInt32(Data, 4);

                            if (Type == 2)//Open
                            {

                                //  Database.LoadEmpire();
                                uint Page = BitConverter.ToUInt32(Data, 8);
                                GC.SendPacket(Packets.DonateOpen(GC.MyChar));
                                GC.SendPacket(Packets.SendTopDonaters(Page));
                                GC.SendPacket(Packets.DonateOpen2(GC.MyChar));

                            }
                            else if (Type == 4)//Open2
                            {
                                /*  uint Page = BitConverter.ToUInt32(Data, 8);
                                  if (Data[8] == 1)//Knights
                                  {
                                      for (byte I = 0; I < 4; I++)
                                      {
                                          //CSocket.Send(ConquerPacket.Nobility(CSocket.Client.ID + I));
                                      }

                                      GC.SendPacket(Packets.SendTopDonaters(Page));
                                      World.NewEmpire(GC.MyChar);
                                  }*/
                            }
                            else if (Type == 1)
                                if (GC.MyChar.Job < 160)
                                {

                                    uint Donation = BitConverter.ToUInt32(Data, 8);
                                    if (Donation <= GC.MyChar.Silvers)
                                    {
                                        if (GC.MyChar.Nobility.Rank == Ranks.Earl)
                                        {

                                            GC.MyChar.Silvers -= Donation;
                                            GC.MyChar.Nobility.Donation += Donation;
                                            Game.World.NewEmpire(GC.MyChar);
                                            //   Database.SaveEmpire(GC.MyChar,GC.MyChar.Nobility.Donation);
                                            if (GC.MyChar.Nobility.Rank == Ranks.Duke)
                                            {
                                                GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);

                                            }
                                        }
                                        else if (GC.MyChar.Nobility.Rank == Ranks.Duke)
                                        {

                                            GC.MyChar.Silvers -= Donation;
                                            GC.MyChar.Nobility.Donation += Donation;
                                            Game.World.NewEmpire(GC.MyChar);
                                            // Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                            if (GC.MyChar.Nobility.Rank == Ranks.Prince)
                                            {
                                                GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);
                                            }
                                        }
                                        else if (GC.MyChar.Nobility.Rank == Ranks.Prince)
                                        {

                                            GC.MyChar.Silvers -= Donation;
                                            GC.MyChar.Nobility.Donation += Donation;
                                            Game.World.NewEmpire(GC.MyChar);
                                            // Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                            if (GC.MyChar.Nobility.Rank == Ranks.King)
                                            {
                                                GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);
                                            }
                                        }
                                        else
                                        {

                                            GC.MyChar.Silvers -= Donation;
                                            GC.MyChar.Nobility.Donation += Donation;
                                            Game.World.NewEmpire(GC.MyChar);
                                            //  Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                        }
                                    }
                                    else
                                    {
                                        if (Donation / 500 <= GC.MyChar.CPs)
                                        {
                                            if (GC.MyChar.Nobility.Rank == Ranks.Earl)
                                            {
                                                GC.MyChar.CPs -= (uint)(Donation / 500);
                                                GC.MyChar.Nobility.Donation += Donation;
                                                Game.World.NewEmpire(GC.MyChar);
                                                //     Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                                if (GC.MyChar.Nobility.Rank == Ranks.Duke)
                                                {
                                                    GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);
                                                }
                                            }
                                            else if (GC.MyChar.Nobility.Rank == Ranks.Duke)
                                            {

                                                GC.MyChar.CPs -= (uint)(Donation / 50000);
                                                GC.MyChar.Nobility.Donation += Donation;
                                                Game.World.NewEmpire(GC.MyChar);
                                                //    Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                                if (GC.MyChar.Nobility.Rank == Ranks.Prince)
                                                {
                                                    GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);
                                                }
                                            }
                                            else if (GC.MyChar.Nobility.Rank == Ranks.Prince)
                                            {

                                                GC.MyChar.CPs -= (uint)(Donation / 50000);
                                                GC.MyChar.Nobility.Donation += Donation;
                                                Game.World.NewEmpire(GC.MyChar);
                                                //    Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                                if (GC.MyChar.Nobility.Rank == Ranks.King)
                                                {
                                                    GC.WorldMessage(2011, "Congration! " + GC.MyChar.Name + " has donating to " + GC.MyChar.Nobility.Rank + " in Nobility Rank.", System.Drawing.Color.Red);
                                                }
                                            }
                                            else
                                            {

                                                GC.MyChar.CPs -= (uint)(Donation / 50000);
                                                GC.MyChar.Nobility.Donation += Donation;
                                                Game.World.NewEmpire(GC.MyChar);
                                                //     Database.SaveEmpire(GC.MyChar, GC.MyChar.Nobility.Donation);
                                            }
                                        }
                                    }
                                    Database.SaveEmpire();
                                }
                                else
                                {
                                }
                            break;
                        }
                    #endregion
                    #region Mentor
                    case 2065:
                        {
                            PacketHandling.Mentor.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Item Lock
                    case 2048:
                        {
                            PacketHandling.ItemPacket.ItemLock.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region StatsPoints
                    case 1024:
                        {
                            byte AddStr = Data[4];
                            byte AddAgi = Data[5];
                            byte AddVit = Data[6];
                            byte AddSpi = Data[7];
                            if (AddStr != 0)
                            {
                                if (GC.MyChar.StatusPoints == 0)
                                    return;
                                GC.MyChar.StatusPoints -= 1;
                                GC.MyChar.Str += 1;
                            }
                            else if (AddAgi != 0)
                            {
                                if (GC.MyChar.StatusPoints == 0)
                                    return;
                                GC.MyChar.StatusPoints -= 1;
                                GC.MyChar.Agi += 1;
                            }
                            else if (AddVit != 0)
                            {
                                if (GC.MyChar.StatusPoints == 0)
                                    return;
                                GC.MyChar.StatusPoints -= 1;
                                GC.MyChar.Vit += 1;
                            }
                            else if (AddSpi != 0)
                            {
                                if (GC.MyChar.StatusPoints == 0)
                                    return;
                                GC.MyChar.StatusPoints -= 1;
                                GC.MyChar.Spi += 1;
                            }
                            break;
                        }
                    #endregion
                    #region OfTg
                    case 2044:
                        {
                            byte Type = Data[4];
                            if (Type == 1)
                            {
                                GC.MyChar.Loc.Map = 601;
                                GC.MyChar.Loc.X = 60;
                                GC.MyChar.Loc.Y = 54;
                                Database.SaveCharacter(GC.MyChar, GC.AuthInfo.Account);
                            }
                            ushort MinutesTrained = (ushort)((GC.MyChar.LoggedOn - GC.MyChar.LastLogin).TotalMinutes);
                            ushort HowLong = (ushort)(MinutesTrained * 10);
                            if (HowLong > 900 || HowLong < 0)
                                HowLong = 900;
                            else if (HowLong == 0)
                                HowLong = (ushort)(MinutesTrained / 100);
                            GC.SendPacket(Packets.OffLineTg((short)HowLong, Type));
                            if (Type == 4)
                            {
                                if (GC.MyChar.BlessingLasts > 0 && GC.MyChar.InOTG)
                                {
                                    GC.MyChar.InOTG = false;
                                    uint ExpAdd = (uint)(GC.MyChar.EXPBall * ((double)MinutesTrained / 900));
                                    ExpAdd *= Game.World.ServerInfo.ExperienceRate;
                                    GC.MyChar.IncreaseExp(ExpAdd, false);
                                    GC.MyChar.TrainTimeLeft -= MinutesTrained;
                                    GC.LocalMessage(2000, System.Drawing.Color.Yellow, "You gain " + ExpAdd + " experience from offline TG.");
                                }
                                try
                                {
                                    Game.Vector2 V = (Game.Vector2)Database.DefaultCoords[GC.MyChar.Loc.PreviousMap];
                                    GC.MyChar.Teleport(GC.MyChar.Loc.PreviousMap, V.X, V.Y);
                                }
                                catch { GC.MyChar.Teleport(1002, 432, 372); }
                                GC.MyChar.InOTG = false;
                            }
                            break;
                        }
                    #endregion
                    #region Broadcaster
                    case 2050:
                        {
                            if (Data[4] == 3 && GC.MyChar.CPs >= 5)
                            {
                                if (Game.World.BroadCastCount <= 100)
                                {
                                    GC.MyChar.CPs -= 5;

                                    string Message = "";
                                    for (byte i = 0; i < Data[13]; i++)
                                        Message += Convert.ToChar(Data[14 + i]);


                                    Game.BroadCastMessage B = new Server.Game.BroadCastMessage();
                                    B.Name = GC.MyChar.Name;
                                    B.Message = Message;
                                    B.Place = Game.World.BroadCastCount;
                                    Game.World.BroadCasts[Game.World.BroadCastCount] = B;
                                    Game.World.BroadCastCount++;

                                    Data[8] = B.Place;
                                    GC.SendPacket2(Data);

                                }
                            }
                            break;
                        }
                    #endregion
                    #region Pheonix City Npc Add gem to items
                    case 1027:
                        {
                            string P = ""; string Phex = "";
                            for (byte bit = 0; bit < Data.Length - 8; bit++)
                            {
                                int Pi = Data[bit];
                                P += Data[bit] + " ";
                                Phex += Pi.ToString("X") + " ";
                            }
                            Console.WriteLine("packet: {0} ", P);

                            uint MainUID = BitConverter.ToUInt32(Data, 8);
                            uint GemUID = BitConverter.ToUInt32(Data, 12);

                            Game.Item MainItem = new Game.Item();
                            Game.Item Gem = new Item(); ;


                            foreach (Game.Item II in GC.MyChar.Inventory.Values)
                            {
                                if (II.UID == MainUID)
                                {
                                    MainItem = II;
                                }
                            }
                            foreach (Game.Item Ia in GC.MyChar.Inventory.Values)
                            {
                                if (Ia.UID == GemUID)
                                {
                                    Gem = Ia;
                                }
                            }

                            byte Mode = Data[18];
                            byte Slot = Data[16];

                            if (Mode == 0)
                            {
                                //GC.MyChar.RemoveItemI(MainItem.UID, 1); 
                                GC.MyChar.RemoveItemUIDAmount(Gem.UID, 1);

                                if (Slot == 1)
                                    MainItem.Soc1 = (Game.Item.Gem)(Gem.ID - 700000);
                                if (Slot == 2)
                                    MainItem.Soc2 = (Game.Item.Gem)(Gem.ID - 700000);


                                GC.MyChar.UpdateItem(MainItem);
                            }
                            else
                            {
                                // GC.MyChar.RemoveItemI(MainItem.UID, 1);
                                if (Slot == 1)
                                    MainItem.Soc1 = Game.Item.Gem.EmptySocket;
                                if (Slot == 2)
                                    MainItem.Soc2 = Game.Item.Gem.EmptySocket;
                                GC.MyChar.UpdateItem(MainItem);

                                // GC.MyChar.AddFullItem(MainItem.ID, MainItem.Bless, MainItem.Plus, MainItem.Enchant, MainItem.Soc1, MainItem.Soc2, MainItem.Color, MainItem.Progress, MainItem.TalismanProgress, MainItem.Effect, MainItem.FreeItem, MainItem.CurDur, MainItem.MaxDur, MainItem.Suspicious, MainItem.Locked, MainItem.LockedDays, MainItem.RBG[0], MainItem.RBG[1], MainItem.RBG[2], MainItem.RBG[3]);
                            }

                            break;
                        }
                    #endregion
                    #region Compose Npc
                    case 2036:
                        {
                            //PacketHandling.ItemPacket.Compose.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region DropItems 1101
                    case 1101:
                        {
                            PacketHandling.PickItemUp.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region TradePartener 2046
                    case 2046:
                        {
                            PacketHandling.TradePartener.Handle(GC, Data);
                            // here for trade partner
                            break;
                        }

                    #endregion
                    #region Team 1023
                    case 1023:
                        {
                            PacketHandling.TeamHandle.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Attacher Pachet 0x3FE/1022
                    case 0x3FE://1022
                        {
                            /*
                             * You Obtinet 0 Conquest Points //39
                             * conquest points bara //37
                             * un mesaj reject 10 minute //52 and 53 54
                             */
                            uint dmg = BitConverter.ToUInt32(Data, 24);
                            uint AttackType = BitConverter.ToUInt32(Data, 20);
                            switch (AttackType)
                            {
                                case 8:
                                case 9: Features.Marriage.Handle(GC, Data); break;
                                case 40:
                                    GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Server.Game.Status.Merchant, 255));
                                    GC.MyChar.Merchant = Server.Game.MerchantTypes.Yes;
                                    GC.SendPacket2(Data);
                                    break;
                                case 41:
                                    GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Server.Game.Status.Merchant, 0));
                                    GC.MyChar.Merchant = Server.Game.MerchantTypes.Not;
                                    GC.SendPacket2(Data);
                                    break;
                                case 39:
                                    {

                                        //GC.MyChar.MyClient.SendPacket(Packets.AttackPacket(2, GC.MyChar.EntityID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 1000, 39));
                                        //   GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Server.Game.Status.CPs, 1000));
                                        //GC.MyChar.CPs += 1000;
                                        // GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Server.Game.Status.CPSDOnators, GC.MyChar.CpsDonate));
                                        GC.SendPacket2(Data);
                                        // GC.MyChar.MyClient.SendPacket(Packets.AttackPacket(0, GC.MyChar.EntityID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, dmg, 39));
                                        break;
                                    }
                                #region Interactiions
                                case 46:
                                    {
                                        uint TargetUID = BitConverter.ToUInt32(Data, 12);
                                        GC.MyChar.InteractionInProgress = false;
                                        GC.MyChar.InteractionWith = TargetUID;
                                        GC.MyChar.InteractionType = 0;
                                        if (Game.World.H_Chars.ContainsKey(TargetUID))
                                        {
                                            Game.Character d = Game.World.H_Chars[TargetUID] as Character;
                                            d.InteractionInProgress = false;
                                            d.InteractionWith = GC.MyChar.EntityID;
                                            d.InteractionType = 0;
                                            d.MyClient.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, TargetUID, d.Loc.X, d.Loc.Y, dmg, 65));//46
                                        }
                                        break;
                                    }
                                case 47:
                                    {
                                        //GC.MyChar.StatEff.Remove(StatusEffectEn.Ride);
                                        uint TargetUID = BitConverter.ToUInt32(Data, 12);
                                        if (GC.MyChar.InteractionWith != TargetUID)
                                            return;
                                        GC.MyChar.InteractionSet = false;
                                        if (Game.World.H_Chars.ContainsKey(TargetUID))
                                        {
                                            Game.Character d = Game.World.H_Chars[TargetUID] as Character;
                                            //d.StatEff.Remove(StatusEffectEn.Ride);
                                            d.InteractionSet = false;
                                            if (d.InteractionWith != GC.MyChar.EntityID)
                                                return;
                                            if (d.Body == 1003 || d.Body == 1004)
                                            {
                                                d.MyClient.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, TargetUID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, dmg, 47));
                                                d.InteractionInProgress = true;
                                                GC.MyChar.InteractionInProgress = true;
                                                d.InteractionType = dmg;
                                                d.InteractionX = GC.MyChar.Loc.X;//Almost done kk
                                                d.InteractionY = GC.MyChar.Loc.Y;
                                                GC.MyChar.InteractionType = dmg;
                                                GC.MyChar.InteractionX = GC.MyChar.Loc.X;
                                                GC.MyChar.InteractionY = GC.MyChar.Loc.Y;
                                                if (d.Loc.X == GC.MyChar.Loc.X && d.Loc.Y == GC.MyChar.Loc.Y)
                                                {
                                                    d.InteractionSet = true;
                                                    GC.MyChar.InteractionSet = true;
                                                    d.MyClient.SendPacket(Packets.AttackPacket(d.EntityID, GC.MyChar.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 47));
                                                    GC.MyChar.SendScreen(Packets.AttackPacket(GC.MyChar.EntityID, d.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 49));
                                                    GC.MyChar.SendScreen(Packets.AttackPacket(d.EntityID, GC.MyChar.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 49));
                                                }
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.AttackPacket(TargetUID, GC.MyChar.EntityID, d.Loc.X, d.Loc.Y, dmg, 47));
                                                d.InteractionInProgress = true;
                                                GC.MyChar.InteractionInProgress = true;
                                                d.InteractionType = dmg;
                                                d.InteractionX = d.Loc.X;
                                                d.InteractionY = d.Loc.Y;
                                                GC.MyChar.InteractionType = dmg;
                                                GC.MyChar.InteractionX = d.Loc.X;
                                                GC.MyChar.InteractionY = d.Loc.Y;
                                                if (d.Loc.X == GC.MyChar.Loc.X && d.Loc.Y == GC.MyChar.Loc.Y)
                                                {
                                                    d.InteractionSet = true;
                                                    GC.MyChar.InteractionSet = true;
                                                    d.MyClient.SendPacket(Packets.AttackPacket(d.EntityID, GC.MyChar.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 47));
                                                    GC.MyChar.SendScreen(Packets.AttackPacket(GC.MyChar.EntityID, d.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 49));
                                                    GC.MyChar.SendScreen(Packets.AttackPacket(d.EntityID, GC.MyChar.EntityID, d.Loc.X, d.Loc.Y, GC.MyChar.InteractionType, 49));
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 48:
                                    {
                                        uint TargetUID = BitConverter.ToUInt32(Data, 12);
                                        GC.MyChar.InteractionType = 0;
                                        GC.MyChar.InteractionWith = 0;
                                        GC.MyChar.InteractionInProgress = false;
                                        if (Game.World.H_Chars.ContainsKey(TargetUID))
                                        {
                                            Character d = Game.World.H_Chars[TargetUID] as Character;
                                            d.InteractionType = 0;
                                            d.InteractionWith = 0;
                                            d.InteractionInProgress = false;
                                        }
                                        break;
                                    }
                                case 50:
                                    {
                                        uint TargetUID = BitConverter.ToUInt32(Data, 12);
                                        uint Attacker = BitConverter.ToUInt32(Data, 8);
                                        GC.MyChar.InteractionType = 0;
                                        GC.MyChar.InteractionWith = 0;
                                        GC.MyChar.InteractionInProgress = false;
                                        if (Game.World.H_Chars.ContainsKey(TargetUID))
                                        {
                                            Game.Character d = Game.World.H_Chars[TargetUID] as Character;
                                            d.InteractionType = 0;
                                            d.InteractionWith = 0;
                                            d.InteractionInProgress = false;
                                        }
                                        GC.MyChar.SendScreen(Packets.AttackPacket(GC.MyChar.EntityID, TargetUID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0, 50));
                                        GC.MyChar.SendScreen(Packets.AttackPacket(TargetUID, GC.MyChar.EntityID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0, 50));
                                        break;
                                    }
                                #endregion
                                default:
                                    if (GC.MyChar.Loc.Map != 1950)
                                        if (GC.MyChar.Loc.Map != 1036)
                                        {
                                            PacketHandling.Attack.Handle(GC, Data);
                                        }
                                    //Console.WriteLine("atackpacket ID: " + AttackType);
                                    break;
                            }
                            break;
                        }
                    #endregion

                    #region Npc dialog 2031
                    case 2031:
                        {
                            GC.DialogNPC = BitConverter.ToUInt32(Data, 4);
                            PacketHandling.NPCDialog.Handles(GC, Data, GC.DialogNPC, 0);

                            break;
                        }
                    #endregion
                    #region Npc Replay Dialog 2032
                    case 2032:
                        {
                            if (Data[10] != 0)
                                PacketHandling.NPCDialog.Handles(GC, Data, GC.DialogNPC, Data[10]);
                            else
                            {
                                byte NameLength = Data[13];
                                string Name = "";
                                for (byte i = 0; i < NameLength; i++)
                                    Name += Convert.ToChar(Data[14 + i]);

                                if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                    GC.MyChar.MyGuild.MemberLeaves(Name, true);
                            }
                            GC.SendPacket2(Data);
                            break;
                        }
                    #endregion

                    #region 1009
                    /*case 1009:
                            {
                                byte PacketType = Data[12];

                                switch (PacketType)
                                {
                                    case 1:
                                        {
                                            PacketHandling.ItemPacket.Shops.BuyHandle(GC, Data);
                                            break;
                                        }
                                    case 2:
                                        {
                                            PacketHandling.ItemPacket.Shops.SellHandle(GC, Data);
                                            break;
                                        }
                                    case 37:
                                        {
                                            PacketHandling.ItemPacket.DropAnItem.Handle(GC, Data);
                                            break;
                                        }
                                    case 38:
                                        {
                                            PacketHandling.ItemPacket.DropMoney.Handle(GC, Data);
                                            break;
                                        }
                                    case 4:
                                        {
                                            PacketHandling.ItemPacket.Equip.HandleEquip(GC, Data);
                                            break;
                                        }
                                    case 6:
                                        {
                                            PacketHandling.ItemPacket.Equip.HandleUnEquip(GC, Data);
                                            break;
                                        }
                                    case 9:
                                        {
                                            uint NPC = BitConverter.ToUInt32(Data, 4);
                                            if (NPC != 0)
                                            {
                                                GC.SendPacket(Packets.OpenWarehouse((ushort)NPC, GC.MyChar.WHSilvers));
                                            }
                                            break;
                                        }
                                    case 10:
                                        {
                                            uint Amount = BitConverter.ToUInt32(Data, 8);
                                            if (GC.MyChar.Silvers >= Amount)
                                            {
                                                GC.MyChar.Silvers -= Amount;
                                                GC.MyChar.WHSilvers += Amount;
                                            }
                                            break;
                                        }
                                    case 11:
                                        {
                                            uint Amount = BitConverter.ToUInt32(Data, 8);
                                            if (GC.MyChar.WHSilvers >= Amount)
                                            {
                                                GC.MyChar.Silvers += Amount;
                                                GC.MyChar.WHSilvers -= Amount;
                                            }
                                            break;
                                        }
                                    case 14:
                                        {
                                            PacketHandling.ItemPacket.Repair.Handle(Data, GC);
                                            break;
                                        }
                                    case 15:
                                        {
                                            PacketHandling.ItemPacket.Repair.HandleVipRepair(GC);
                                            break;
                                        }
                                    case 20:
                                        {
                                            PacketHandling.ItemPacket.MeteorUpgrade.Handle(GC, Data);
                                            break;
                                        }
                                    case 19:
                                        {
                                            PacketHandling.ItemPacket.DBUpgrade.Handle(GC, Data);
                                            break;
                                        }
                                    case 21:
                                        {
                                            uint StallID = BitConverter.ToUInt32(Data, 4);
                                            if (Game.World.H_PShops.Contains(StallID))
                                            {
                                                PacketHandling.MarketShops.Shop S = (PacketHandling.MarketShops.Shop)Game.World.H_PShops[StallID];
                                                S.SendItems(GC);
                                            }
                                            break;
                                        }
                                    case 22:
                                        {
                                            if (GC.MyChar.MyShop != null)
                                            {
                                                if (!GC.GM)
                                                    if (GC.MyChar.MyShop.AddItem(BitConverter.ToUInt32(Data, 4), BitConverter.ToUInt32(Data, 8), 1))
                                                        GC.SendPacket2(Data);
                                            }
                                            break;
                                        }
                                    case 29:
                                        {
                                            if (GC.MyChar.MyShop != null)
                                            {
                                                if (!GC.GM)
                                                    if (GC.MyChar.MyShop.AddItem(BitConverter.ToUInt32(Data, 4), BitConverter.ToUInt32(Data, 8), 3))
                                                        GC.SendPacket2(Data);
                                            }
                                            break;
                                        }
                                    case 23:
                                        {
                                            if (GC.MyChar.MyShop != null)
                                                GC.MyChar.MyShop.RemoveItem(BitConverter.ToUInt32(Data, 4), Data);
                                            break;
                                        }
                                    case 24:
                                        {
                                            uint ItemUID = BitConverter.ToUInt32(Data, 4);
                                            uint StallID = BitConverter.ToUInt32(Data, 8);

                                            if (Game.World.H_PShops.Contains(StallID))
                                            {
                                                PacketHandling.MarketShops.Shop S = (PacketHandling.MarketShops.Shop)Game.World.H_PShops[StallID];
                                                S.Buy(ItemUID, GC.MyChar);
                                            }
                                            break;
                                        }
                                    case 27:
                                        {
                                            GC.SendPacket2(Data);

                                            break;
                                        }
                                    case 28:
                                        {
                                            Game.Item I = GC.MyChar.FindInvItem(BitConverter.ToUInt32(Data, 4));
                                            Game.Item Gem = GC.MyChar.FindInvItem(BitConverter.ToUInt32(Data, 8));

                                            if (I.ID != 0 && Gem.ID != 0)
                                            {
                                                byte Enchant = 0;
                                                if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 1)
                                                    Enchant = (byte)Rnd.Next(1, 59);
                                                else if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 2)
                                                {
                                                    if (Gem.ID == 700012)
                                                        Enchant = (byte)Rnd.Next(100, 159);
                                                    else if (Gem.ID == 700002 || Gem.ID == 700052 || Gem.ID == 700062)
                                                        Enchant = (byte)Rnd.Next(60, 109);
                                                    else if (Gem.ID == 700032)
                                                        Enchant = (byte)Rnd.Next(80, 129);
                                                    else
                                                        Enchant = (byte)Rnd.Next(40, 89);
                                                }
                                                else if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 3)
                                                {
                                                    if (Gem.ID == 700013)
                                                        Enchant = (byte)Rnd.Next(200, 255);
                                                    else if (Gem.ID == 700003 || Gem.ID == 700073 || Gem.ID == 700033)
                                                        Enchant = (byte)Rnd.Next(170, 229);
                                                    else if (Gem.ID == 700063 || Gem.ID == 700053)
                                                        Enchant = (byte)Rnd.Next(140, 199);
                                                    else if (Gem.ID == 700023)
                                                        Enchant = (byte)Rnd.Next(90, 149);
                                                    else
                                                        Enchant = (byte)Rnd.Next(70, 119);
                                                }
                                                GC.MyChar.RemoveItem(Gem.ID, 1);
                                                if (Enchant > I.Enchant)
                                                {
                                                    GC.MyChar.RemoveItem(I.ID, 1);
                                                    I.Enchant = Enchant;
                                                    GC.MyChar.AddFullItem(I);
                                                    GC.LocalMessage(2005,System.Drawing.Color.White, "The equip now gives " + I.Enchant + " extra HP.");
                                                }
                                                else
                                                    GC.LocalMessage(2005, System.Drawing.Color.White, Enchant + " extra HP, the current one will stay.");
                                            }
                                            break;
                                        }
                                    case 36:
                                        {
                                            uint UID1 = BitConverter.ToUInt32(Data, 4);
                                            byte Slot = GC.MyChar.Equips.GetSlot(UID1);
                                            Game.Item Talisman = GC.MyChar.Equips.Get(Slot);
                                            if (Talisman.UID == UID1)
                                            {
                                                int Price = 0;
                                                if ((byte)Talisman.Soc1 == 0)
                                                {
                                                    decimal procent = 100 - (Talisman.TalismanProgress * 256 * 100 / 2048000);
                                                    if (100 - procent < 25)
                                                        return;
                                                    double price = (double)procent * 55;
                                                    Price = Convert.ToInt32(price);
                                                }
                                                else
                                                {
                                                    decimal procent = 100 - (Talisman.TalismanProgress * 256 * 100 / 5120000);
                                                    if (100 - procent < 25)
                                                        return;
                                                    double price = (double)procent * 110;
                                                    Price = Convert.ToInt32(price);
                                                }

                                                if (GC.MyChar.CPs >= Price)
                                                {
                                                    GC.MyChar.CPs -= (uint)Price;
                                                    if (Talisman.Soc1 == Server.Game.Item.Gem.NoSocket)
                                                    {
                                                        Talisman.Soc1 = Server.Game.Item.Gem.EmptySocket;
                                                        Talisman.TalismanProgress = 0;
                                                        GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar); return;
                                                    }
                                                    if (Talisman.Soc1 != Server.Game.Item.Gem.NoSocket)
                                                    {
                                                        Talisman.Soc2 = Server.Game.Item.Gem.EmptySocket;
                                                        Talisman.TalismanProgress = 0;
                                                        GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar); return;
                                                    }

                                                }
                                            }
                                            break;
                                        }
                                    case 35:
                                        {
                                            uint UID1 = BitConverter.ToUInt32(Data, 8);
                                            uint UID2 = BitConverter.ToUInt32(Data, 4);
                                            Game.Item UsedItem = GC.MyChar.FindInvItem(UID1);
                                            byte Slot = GC.MyChar.Equips.GetSlot(UID2);
                                            Game.Item Talisman = GC.MyChar.Equips.Get(Slot);

                                            if (UsedItem.UID == UID1 && Talisman.UID == UID2 && UsedItem.ID != 0 && Talisman.ID != 0)
                                            {
                                                ushort Points = 0;
                                                Game.ItemIDManipulation I = new Server.Game.ItemIDManipulation(UsedItem.ID);
                                                if (I.Quality == Server.Game.Item.ItemQuality.Refined)
                                                    Points += 5;
                                                else if (I.Quality == Server.Game.Item.ItemQuality.Unique)
                                                    Points += 10;
                                                else if (I.Quality == Server.Game.Item.ItemQuality.Elite)
                                                    Points += 40;
                                                else if (I.Quality == Server.Game.Item.ItemQuality.Super)
                                                    Points += 1000;

                                                if (UsedItem.Plus > 0)
                                                    Points += Database.SocPlusExtra[UsedItem.Plus - 1];

                                                if (UsedItem.FreeItem)
                                                    break;
                                                if (UsedItem.ID / 1000 == Talisman.ID / 1000)
                                                    break;

                                                string Type = UsedItem.ID.ToString().Remove(2, UsedItem.ID.ToString().Length - 2);
                                                uint WeirdThing = Convert.ToUInt32(Type);

                                                if (WeirdThing <= 61 && WeirdThing >= 40)
                                                {
                                                    if (I.Quality == Server.Game.Item.ItemQuality.Elite || I.Quality == Server.Game.Item.ItemQuality.Super)
                                                    {
                                                        if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 == 0)
                                                            Points += 160;
                                                        if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 != 0)
                                                            Points += 960;
                                                    }
                                                }
                                                else
                                                {
                                                    if (I.Quality == Server.Game.Item.ItemQuality.Elite || I.Quality == Server.Game.Item.ItemQuality.Super)
                                                    {
                                                        if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 == 0)
                                                            Points += 2000;
                                                        if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 != 0)
                                                            Points += 8000;
                                                    }
                                                }

                                                Talisman.TalismanProgress += Points;
                                                if (Talisman.Soc1 == Server.Game.Item.Gem.NoSocket && Talisman.TalismanProgress >= 8000)
                                                {
                                                    Talisman.Soc1 = Server.Game.Item.Gem.EmptySocket;
                                                    Talisman.TalismanProgress -= 8000;
                                                }
                                                if (Talisman.Soc1 != Server.Game.Item.Gem.NoSocket && Talisman.Soc2 == Server.Game.Item.Gem.NoSocket && Talisman.TalismanProgress >= 20000)
                                                {
                                                    Talisman.Soc2 = Server.Game.Item.Gem.EmptySocket;
                                                    Talisman.TalismanProgress = 0;
                                                }
                                                GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar);
                                                GC.MyChar.RemoveItem(UsedItem.ID, 1);
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Unknow 1009 subtype: " + Data[12], 2001, 0 ,System.Drawing.Color.White));
                                            break;
                                        }
                                }
                                break;
                            }*/
                    #endregion
                    #region 1009 ItemsShops/Upgrad/Stalls/Unequipd
                    case 1009:
                        {
                            byte PacketType = Data[12];
                            switch (PacketType)
                            {
                                case 1:
                                    {
                                        PacketHandling.ItemPacket.Shops.BuyHandle(GC, Data);
                                        break;
                                    }
                                case 2:
                                    {
                                        PacketHandling.ItemPacket.Shops.SellHandle(GC, Data);
                                        break;
                                    }
                                
                                case 4:
                                    {
                                        PacketHandling.ItemPacket.Equip.HandleEquip(GC, Data);
                                        break;
                                    }
                                case 6:
                                    {
                                        PacketHandling.ItemPacket.Equip.HandleUnEquip(GC, Data);
                                        break;
                                    }
                                #region Warehouse
                                case 9:
                                    {
                                        uint NPC = BitConverter.ToUInt32(Data, 4);
                                        if (NPC != 0)
                                        {
                                            GC.SendPacket(Packets.OpenWarehouse((ushort)NPC, GC.MyChar.WHSilvers));
                                        }
                                        break;
                                    }
                                case 10:
                                    {
                                        uint Amount = BitConverter.ToUInt32(Data, 8);
                                        if (GC.MyChar.Silvers >= Amount)
                                        {
                                            GC.MyChar.Silvers -= Amount;
                                            GC.MyChar.WHSilvers += Amount;
                                        }
                                        break;
                                    }
                                case 11:
                                    {
                                        uint Amount = BitConverter.ToUInt32(Data, 8);
                                        if (GC.MyChar.WHSilvers >= Amount)
                                        {
                                            GC.MyChar.Silvers += Amount;
                                            GC.MyChar.WHSilvers -= Amount;
                                        }
                                        break;
                                    }
                                #endregion
                                case 14:
                                    {
                                        PacketHandling.ItemPacket.Repair.Handle(Data, GC);
                                        break;
                                    }
                                case 15:
                                    {
                                        PacketHandling.ItemPacket.Repair.HandleVipRepair(GC);
                                        break;
                                    }
                                case 19:
                                    {
                                        PacketHandling.ItemPacket.DBUpgrade.Handle(GC, Data);
                                        break;
                                    }
                                case 20:
                                    {
                                        PacketHandling.ItemPacket.MeteorUpgrade.Handle(GC, Data);
                                        break;
                                    }
                                #region MarketShop
                                case 21:
                                    {
                                        uint StallID = BitConverter.ToUInt32(Data, 4);
                                        if (Game.World.H_PShops.Contains(StallID))
                                        {
                                            PacketHandling.MarketShops.Shop S = (PacketHandling.MarketShops.Shop)Game.World.H_PShops[StallID];
                                            S.SendItems(GC);
                                        }
                                        break;
                                    }
                                case 22:
                                    {
                                        if (GC.MyChar.MyShop != null)
                                        {
                                            if (GC.MyChar.MyShop.AddItem(BitConverter.ToUInt32(Data, 4), BitConverter.ToUInt32(Data, 8), 1))
                                                GC.SendPacket2(Data);
                                        }
                                        break;
                                    }
                                case 23:
                                    {
                                        if (GC.MyChar.MyShop != null)
                                            GC.MyChar.MyShop.RemoveItem(BitConverter.ToUInt32(Data, 4), Data);
                                        break;
                                    }
                                case 24:
                                    {
                                        uint ItemUID = BitConverter.ToUInt32(Data, 4);
                                        uint StallID = BitConverter.ToUInt32(Data, 8);

                                        if (Game.World.H_PShops.Contains(StallID))
                                        {
                                            PacketHandling.MarketShops.Shop S = (PacketHandling.MarketShops.Shop)Game.World.H_PShops[StallID];
                                            S.Buy(ItemUID, GC.MyChar);
                                        }
                                        break;
                                    }
                                case 29:
                                    {
                                        if (GC.MyChar.MyShop != null)
                                        {
                                            if (GC.MyChar.MyShop.AddItem(BitConverter.ToUInt32(Data, 4), BitConverter.ToUInt32(Data, 8), 3))
                                                GC.SendPacket2(Data);
                                        }
                                        break;
                                    }
                                #endregion
                                case 27:
                                    {
                                        GC.SendPacket2(Data);
                                        break;
                                    }
                                #region Enchant
                                case 28:
                                    {
                                        // Game.Item I = GC.MyChar.FindInvItem(BitConverter.ToUInt32(Data, 4));
                                        // Game.Item Gem = GC.MyChar.FindInvItem(BitConverter.ToUInt32(Data, 8));
                                        Game.Item I = new Game.Item();
                                        Game.Item Gem = new Game.Item();
                                        foreach (Game.Item II in GC.MyChar.Inventory.Values)
                                        {
                                            if (II.UID == BitConverter.ToUInt32(Data, 4))
                                            {
                                                I = II;
                                            }
                                        }
                                        foreach (Game.Item Ia in GC.MyChar.Inventory.Values)
                                        {
                                            if (Ia.UID == BitConverter.ToUInt32(Data, 8))
                                            {
                                                Gem = Ia;
                                            }
                                        }
                                        if (I.ID != 0 && Gem.ID != 0)
                                        {
                                            byte Enchant = 0;
                                            if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 1)
                                                Enchant = (byte)Rnd.Next(1, 59);
                                            else if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 2)
                                            {
                                                if (Gem.ID == 700012)
                                                    Enchant = (byte)Rnd.Next(100, 159);
                                                else if (Gem.ID == 700002 || Gem.ID == 700052 || Gem.ID == 700062)
                                                    Enchant = (byte)Rnd.Next(60, 109);
                                                else if (Gem.ID == 700032)
                                                    Enchant = (byte)Rnd.Next(80, 129);
                                                else
                                                    Enchant = (byte)Rnd.Next(40, 89);
                                            }
                                            else if (Game.ItemIDManipulation.Digit(Gem.ID, 6) == 3)
                                            {
                                                if (Gem.ID == 700013)
                                                    Enchant = (byte)Rnd.Next(200, 255);
                                                else if (Gem.ID == 700003 || Gem.ID == 700073 || Gem.ID == 700033)
                                                    Enchant = (byte)Rnd.Next(170, 229);
                                                else if (Gem.ID == 700063 || Gem.ID == 700053)
                                                    Enchant = (byte)Rnd.Next(140, 199);
                                                else if (Gem.ID == 700023)
                                                    Enchant = (byte)Rnd.Next(90, 149);
                                                else
                                                    Enchant = (byte)Rnd.Next(70, 119);
                                            }
                                            GC.MyChar.RemoveItemUIDAmount(Gem.UID, 1);
                                            if (Enchant > I.Enchant)
                                            {

                                                I.Enchant = Enchant;
                                                GC.MyChar.UpdateItem(I);
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "The equip now gives " + I.Enchant + " extra HP.");
                                            }
                                            else
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, Enchant + " extra HP, the current one will stay.");
                                        }
                                        break;
                                    }
                                #endregion
                                #region Confiscator
                                case 32://confiscator rewardpacket
                                    {
                                        Features.Confiscator.RetriveItem(GC, Data);

                                        break;
                                    }
                                case 33://clain removeitem confiscator
                                    {

                                        Features.Confiscator.RewardConfiscator(GC, Data);

                                        break;

                                    }
                                #endregion
                                case 34:
                                    {
                                        PacketHandling.TeamHandle.Handle(GC, Data);
                                        break;
                                    }
                                #region Fan/Tower Compose
                                case 35:
                                    {//talisman Compose Button
                                        uint UID1 = BitConverter.ToUInt32(Data, 8);
                                        uint UID2 = BitConverter.ToUInt32(Data, 4);
                                        // Game.Item UsedItem = GC.MyChar.FindInvItem(UID1);

                                        Game.Item UsedItem = new Game.Item();
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                        {
                                            if (I.UID == UID1)
                                            {
                                                UsedItem = I;
                                            }
                                        }
                                        byte Slot = GC.MyChar.Equips.GetSlot(UID2);
                                        Game.Item Talisman = GC.MyChar.Equips.Get(Slot);

                                        if (UsedItem.UID == UID1 && Talisman.UID == UID2 && UsedItem.ID != 0 && Talisman.ID != 0)
                                        {
                                            ushort Points = 0;
                                            Game.ItemIDManipulation I = new Server.Game.ItemIDManipulation(UsedItem.ID);
                                            if (I.Quality == Server.Game.Item.ItemQuality.Refined)
                                                Points += 5;
                                            else if (I.Quality == Server.Game.Item.ItemQuality.Unique)
                                                Points += 10;
                                            else if (I.Quality == Server.Game.Item.ItemQuality.Elite)
                                                Points += 40;
                                            else if (I.Quality == Server.Game.Item.ItemQuality.Super)
                                                Points += 1000;

                                            if (UsedItem.Plus > 0)
                                                Points += Database.SocPlusExtra[UsedItem.Plus - 1];

                                            if (UsedItem.FreeItem)
                                                break;
                                            if (UsedItem.ID / 1000 == Talisman.ID / 1000)
                                                break;

                                            string Type = UsedItem.ID.ToString().Remove(2, UsedItem.ID.ToString().Length - 2);
                                            uint WeirdThing = Convert.ToUInt32(Type);

                                            if (WeirdThing <= 61 && WeirdThing >= 40)
                                            {
                                                if (I.Quality == Server.Game.Item.ItemQuality.Elite || I.Quality == Server.Game.Item.ItemQuality.Super)
                                                {
                                                    if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 == 0)
                                                        Points += 160;
                                                    if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 != 0)
                                                        Points += 960;
                                                }
                                            }
                                            else
                                            {
                                                if (I.Quality == Server.Game.Item.ItemQuality.Elite || I.Quality == Server.Game.Item.ItemQuality.Super)
                                                {
                                                    if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 == 0)
                                                        Points += 2000;
                                                    if ((byte)UsedItem.Soc1 != 0 && (byte)UsedItem.Soc2 != 0)
                                                        Points += 8000;
                                                }
                                            }

                                            Talisman.TalismanProgress += Points;
                                            if (Talisman.Soc1 == Server.Game.Item.Gem.NoSocket && Talisman.TalismanProgress >= 8000)
                                            {
                                                Talisman.Soc1 = Server.Game.Item.Gem.EmptySocket;
                                                Talisman.TalismanProgress -= 8000;
                                            }
                                            if (Talisman.Soc1 != Server.Game.Item.Gem.NoSocket && Talisman.Soc2 == Server.Game.Item.Gem.NoSocket && Talisman.TalismanProgress >= 20000)
                                            {
                                                Talisman.Soc2 = Server.Game.Item.Gem.EmptySocket;
                                                Talisman.TalismanProgress = 0;
                                            }
                                            GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar);
                                            GC.MyChar.RemoveItemUIDAmount(UsedItem.UID, 1);
                                        }
                                        break;
                                    }
                                case 36:
                                    {//talisman Open Button
                                        uint UID1 = BitConverter.ToUInt32(Data, 4);
                                        byte Slot = GC.MyChar.Equips.GetSlot(UID1);
                                        Game.Item Talisman = GC.MyChar.Equips.Get(Slot);
                                        if (Talisman.UID == UID1)
                                        {
                                            int Price = 0;
                                            if ((byte)Talisman.Soc1 == 0)
                                            {
                                                decimal procent = 100 - (Talisman.TalismanProgress * 256 * 100 / 2048000);
                                                if (100 - procent < 25)
                                                    return;
                                                double price = (double)procent * 55;
                                                Price = Convert.ToInt32(price);
                                            }
                                            else
                                            {
                                                decimal procent = 100 - (Talisman.TalismanProgress * 256 * 100 / 5120000);
                                                if (100 - procent < 25)
                                                    return;
                                                double price = (double)procent * 110;
                                                Price = Convert.ToInt32(price);
                                            }

                                            if (GC.MyChar.CPs >= Price)
                                            {
                                                GC.MyChar.CPs -= (uint)Price;
                                                if (Talisman.Soc1 == Server.Game.Item.Gem.NoSocket)
                                                {
                                                    Talisman.Soc1 = Server.Game.Item.Gem.EmptySocket;
                                                    Talisman.TalismanProgress = 0;
                                                    GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar); return;
                                                }
                                                if (Talisman.Soc1 != Server.Game.Item.Gem.NoSocket)
                                                {
                                                    Talisman.Soc2 = Server.Game.Item.Gem.EmptySocket;
                                                    Talisman.TalismanProgress = 0;
                                                    GC.MyChar.Equips.Replace(Slot, Talisman, GC.MyChar); return;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                case 37:
                                    {
                                        PacketHandling.ItemPacket.DropAnItem.Handle(GC, Data);
                                        break;
                                    }
                                case 38:
                                    {
                                        PacketHandling.ItemPacket.DropMoney.Handle(GC, Data);
                                        break;
                                    }
                                default:
                                    {
                                        // GC.SendPacket2(Data);
                                        Console.WriteLine("PacketID 1009 Unknown Case " + Data[12]);
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion

                    #region Warehause Item Packet
                    case 1102:
                        {
                            PacketHandling.Warehouse.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region CreateCharacter
                    case 1001:
                        {
                            PacketHandling.CharacterMaking.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Flower Send 1150
                    case 1150:
                        {
                            //   Flowers.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region AddFlower 1151
                    case 1151:
                        {
                            /*    int sub = Data[4];
                                switch (sub)
                                {
                                    case 2://View
                                        {
                                            Database.LoadFlowers(GC.MyChar);
                                            Struct.Flowers F = GC.MyChar.Flowers;
                                            string ToSend = " " + F.RedRoses.ToString() + " " + F.RedRoses2day.ToString() + " " + F.Lilies.ToString() + " " + F.Lilies2day.ToString() + " ";
                                            ToSend += F.Orchads.ToString() + " " + F.Orchads2day.ToString() + " " + F.Tulips.ToString() + " " + F.Tulips2day.ToString();
                                            GC.SendPacket(Packets.FlowerPacket(ToSend));
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] Unknown 1151 Sub type : " + sub.ToString());
                                            break;
                                        }
                                }*/
                            break;
                        }
                    #endregion
                    #region Walk 10005
                    case 10005:
                        {
                            PacketHandling.WalkRun.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Trade 1056
                    case 1056:
                        {
                            PacketHandling.Trade.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Friends 1019
                    case 1019:
                        {
                            PacketHandling.Friends.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Elighten Send 1127
                    case 1127:
                        {

                            uint usendid = BitConverter.ToUInt32(Data, 8);
                            uint Targetid = BitConverter.ToUInt32(Data, 12);
                            if (Data[4] == 0)
                            {
                                Game.Character Request = (Game.Character)Game.World.H_Chars[Targetid];
                                {
                                    if ((Request.Level + 20) < GC.MyChar.Level)
                                    {
                                        if (GC.MyChar.ElighemPoints >= 100)
                                        {
                                            if (Request.EnhligtehnRequest <= 800)
                                            {
                                                Request.MyClient.SendPacket2(Data);
                                                Request.EnhligtehnRequest += 100;
                                                Request.IncreaseExp(Request.EXPBall * 10, false);
                                                if (GC.MyChar.ElighemPoints == 100)
                                                    GC.MyChar.ElighemPoints = 0;
                                                else
                                                    GC.MyChar.ElighemPoints -= 100;
                                                GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Status.Elighten, (ulong)GC.MyChar.ElighemPoints));
                                                GC.SendPacket2(Data);

                                            }
                                            else
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry, but " + Request.Name + " recivied 9 enligten today");
                                        }
                                        else
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry, but send all elighten today");
                                    }
                                    else
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry, but " + Request.Name + "  is big level");
                                }
                            }
                            break;
                        }
                    #endregion

                    #region 10010 GeneralData
                    case 10010:
                        {
                            //Console.WriteLine("Packet type =  " + Data[16]);
                            switch (Data[16])
                            {
                                case 118:
                                    {
                                        Game.Buff B = GC.MyChar.BuffOf(Server.Skills.SkillsClass.ExtraEffect.Transform);
                                        GC.MyChar.RemoveBuff(B);
                                        break;
                                    }
                                //case 116:
                                //    {
                                //        Console.WriteLine("Enchant");
                                //        break;
                                //    }
                                case 120:
                                    {
                                        Game.Buff B = GC.MyChar.BuffOf(Server.Skills.SkillsClass.ExtraEffect.Fly);
                                        if (B.Eff == Server.Skills.SkillsClass.ExtraEffect.Fly)
                                            GC.MyChar.RemoveBuff(B);
                                        break;
                                    }
                                case 99:
                                    {
                                        GC.MyChar.Mining = true;
                                        break;
                                    }

                                case 151:
                                    {
                                        if (GC.MyChar.Silvers >= 500)
                                        {
                                            GC.MyChar.Silvers -= 500;
                                            GC.MyChar.Avatar = BitConverter.ToUInt16(Data, 8);
                                            Game.World.Spawn(GC.MyChar, false);
                                        }
                                        break;
                                    }
                                case 94:
                                    {
                                        if (Data[8] == 1)
                                        {
                                            PacketHandling.ReviveHere.Handle(Data, GC);
                                        }
                                        else
                                            PacketHandling.Revive.Handle(GC, Data);
                                        break;
                                    }
                                case 145:
                                    {
                                        if (GC.MyChar.Loc.Map != 1090 && GC.MyChar.Loc.Map != 1038 && GC.MyChar.Loc.Map != 1351 && GC.MyChar.Loc.Map != 1352 && GC.MyChar.Loc.Map != 1353 && GC.MyChar.Loc.Map != 1354 && GC.MyChar.Loc.Map != 1350)
                                        {
                                            GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 65535));
                                        }
                                        break;
                                    }
                                case 153://confiscator
                                    {
                                        string P = ""; string Phex = "";
                                        for (byte bit = 0; bit < Data.Length - 8; bit++)
                                        {
                                            int Pi = Data[bit];
                                            P += Data[bit] + " ";
                                            Phex += Pi.ToString("X") + " ";
                                        }
                                        Console.WriteLine("packet: {0} ", P);
                                        //clin general data 136
                                        //reward general data 137
                                        GC.SendPacket2(Data);
                                        if (Data[8] == 0)
                                        {
                                            Game.Item I = new Game.Item();
                                            I.UID = 3323122;
                                            GC.SendPacket(Packets.RemoveItemClain(GC.MyChar, I));
                                            Console.WriteLine("clain a");
                                        }
                                        else if (Data[8] == 1)
                                        {
                                            Console.WriteLine("clain b");
                                        }

                                        break;
                                    }
                                case 148:
                                    {
                                        uint UID = BitConverter.ToUInt32(Data, 8);
                                        if (GC.MyChar.Friends.Contains(UID))
                                        {
                                            Game.Friend F = (Game.Friend)GC.MyChar.Friends[UID];
                                            if (F.Online)
                                            {
                                                if (F.Info.MyGuild != null)
                                                    GC.SendPacket(Packets.String(F.Info.MyGuild.GuildID, (byte)Game.StringType.GuildName, F.Info.MyGuild.GuildName));
                                                GC.SendPacket(Packets.FriendEnemyInfo(F.Info, 0));
                                            }
                                        }
                                        break;
                                    }
                                case 123:
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


                                        uint UID = BitConverter.ToUInt32(Data, 8);

                                        if (GC.MyChar.Enemies.Contains(UID))
                                        {
                                            Game.Enemy E = (Game.Enemy)GC.MyChar.Enemies[UID];
                                            if (E.Online)
                                                GC.SendPacket(Packets.FriendEnemyInfo(E.Info, 1));
                                        }
                                        break;
                                    }
                                /*case 123:
                                    {
                                        uint UID = BitConverter.ToUInt32(Data, 12);

                                        if (GC.MyChar.Enemies.Contains(UID))
                                        {
                                            Game.Enemy E = (Game.Enemy)GC.MyChar.Enemies[UID];
                                            if (E.Online)
                                                GC.SendPacket(Packets.FriendEnemyInfo(E.Info, 1));
                                        }
                                        break;
                                    }*/
                                case 117:
                                    {
                                        Game.Character C = (Game.Character)Game.World.H_Chars[BitConverter.ToUInt32(Data, 8)];
                                        if (Game.World.H_Chars.ContainsKey(BitConverter.ToUInt32(Data, 8)) == false)
                                            return;
                                        C.Equips.SendView(C.EntityID, GC);
                                        GC.SendPacket(Packets.String(GC.MyChar.EntityID, 16, C.Spouse));
                                        GC.SendPacket(Packets.ViewEquip(C));
                                        C.MyClient.LocalMessage(2005, System.Drawing.Color.Red, GC.MyChar.Name + " is respectfully observing your gears.");
                                        break;
                                    }
                                case 106://See team member's location
                                    {
                                        Game.Character C = (Game.Character)Game.World.H_Chars[BitConverter.ToUInt32(Data, 8)];
                                        if (C != null && C.Loc.Map == GC.MyChar.Loc.Map)
                                        {
                                            fixed (byte* p = Data)
                                            {
                                                *((ushort*)(p + 20)) = C.Loc.X;
                                                *((ushort*)(p + 22)) = C.Loc.Y;
                                            }
                                            GC.SendPacket2(Data);
                                        }
                                        break;
                                    }
                                case 74:
                                    {
                                        Database.LoadEnemys(GC.MyChar);
                                        Database.LoadFriends(GC.MyChar);
                                        Database.LoadCharacterPartners(GC.MyChar);
                                        //Database.LoadProfs(GC.MyChar);
                                        //Database.LoadSkills(GC.MyChar);
                                        PacketHandling.Teleport.Handle(GC, Data);
                                        GC.MyChar.LoggedOn = DateTime.Now;

                                        if (GC.MyChar.avaLasts > 0)
                                        {
                                            GC.MyChar.AvaRunning = true;

                                        }
                                        else
                                        {
                                            GC.MyChar.AvaRunning = false;
                                            //Console.WriteLine("Owner.AvaRunning = false 74");
                                        }
                                        if (GC.MyChar.DoubleExp && GC.MyChar.DoubleExpLeft > 0)
                                        {

                                            GC.MyChar.ExpPotionUsed = DateTime.Now;
                                            GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.DoubleExpTime, (ulong)GC.MyChar.DoubleExpLeft));
                                        }
                                        else
                                            GC.MyChar.DoubleExp = false;
                                        break;
                                    }
                                case 75:
                                    {
                                        // GC.SendPacket(Packets.Packet2048(GC.MyChar.EntityID));
                                        GC.SendPacket(Packets.Packet1032(GC.MyChar.EntityID));
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 75));
                                        break;
                                    }
                                case 76:
                                    {
                                        Game.World.Spawns(GC.MyChar, false);
                                        if (!GC.LoginDataSent)
                                        {
                                            GC.LoginDataSent = true;
                                            foreach (Game.Friend F in GC.MyChar.Friends.Values)
                                            {
                                                GC.SendPacket(Packets.FriendEnemyPacket(F.UID, F.Name, 14, Convert.ToByte(F.Online)));
                                                GC.SendPacket(Packets.FriendEnemyPacket(F.UID, F.Name, 15, Convert.ToByte(F.Online)));
                                                if (F.Online)
                                                {
                                                    F.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(GC.MyChar.EntityID, GC.MyChar.Name, 14, 1));//14
                                                    F.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(GC.MyChar.EntityID, GC.MyChar.Name, 15, 1));//15
                                                    F.Info.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your friend " + GC.MyChar.Name + " has logged on.");
                                                }
                                            }

                                            foreach (Game.Enemy E in GC.MyChar.Enemies.Values)
                                            {
                                                GC.SendPacket(Packets.FriendEnemyPacket(E.UID, E.Name, 19, Convert.ToByte(E.Online)));
                                                if (E.Online == true)
                                                {
                                                    E.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(GC.MyChar.EntityID, GC.MyChar.Name, 18, 1));
                                                    E.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(GC.MyChar.EntityID, GC.MyChar.Name, 19, 1));
                                                    E.Info.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your enemy " + GC.MyChar.Name + " has logged on.");
                                                }
                                            }
                                            /* foreach (Game.Enemy E in GC.MyChar.Enemies.Values)
                                             {
                                                 GC.SendPacket(Packets.FriendEnemyPacket(E.UID, E.Name, 19, Convert.ToByte(E.Online)));
                                             }*/
                                            GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 76));
                                        }
                                        break;
                                    }
                                case 77:
                                    {
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 77));//profs
                                        break;
                                    }
                                case 78:
                                    {
                                        GC.SendPacket(Packets.Packet1025());
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 78));//skills
                                        break;
                                    }
                                case 96:
                                    {
                                        if (!GC.DoneLoading)
                                        {
                                            PacketHandling.TradePartener.Status(GC, true);
                                            GC.MyChar.Stamina = 100;
                                            if (GC.MyChar.Loc.Map == 1090 || GC.MyChar.Loc.Map == 1950)
                                            {
                                                GC.MyChar.Teleport(1002, 429, 378);
                                            }
                                            //     if (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004 || GC.MyChar.Body == 2003 || GC.MyChar.Body == 2004)
                                            //     {
                                            //        Struct.Flowers F = GC.MyChar.Flowers;
                                            //        string ToSend = " " + F.RedRoses.ToString() + " " + F.RedRoses2day.ToString() + " " + F.Lilies.ToString() + " " + F.Lilies2day.ToString() + " ";
                                            //        ToSend += F.Orchads.ToString() + " " + F.Orchads2day.ToString() + " " + F.Tulips.ToString() + " " + F.Tulips2day.ToString();
                                            //        GC.SendPacket(Packets.FlowerPacket(ToSend));
                                            //     }
                                            if (GC.MyChar.BlessingLasts >= 1)
                                            {
                                                GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.BlessTime, 3 * 60 * 60 * 24));
                                                GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.OnlineTraining, 0));

                                            }

                                            /*if (GC.MyChar.UniversityPoints == 0)
                                            {
                                                GC.MyChar.UniversityPoints = 1;
                                                GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Status.QuizPts, (ulong)1));
                                            }*/


                                            if (GC.MyChar.ElighemPoints >= 10)
                                            {
                                                try
                                                {
                                                    GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Status.Elighten, (ulong)GC.MyChar.ElighemPoints));
                                                }
                                                catch { }
                                            }

                                            else if (GC.MyChar.ElightenAdd == 1)
                                            {
                                                if (GC.MyChar.Level >= 90)
                                                    GC.MyChar.ElighemPoints += 100;
                                                if (GC.MyChar.Nobility.Rank == Ranks.Prince)
                                                    GC.MyChar.ElighemPoints += 300;
                                                if (GC.MyChar.Nobility.Rank == Ranks.Baron)
                                                    GC.MyChar.ElighemPoints += 100;
                                                if (GC.MyChar.Nobility.Rank == Ranks.Knight)
                                                    GC.MyChar.ElighemPoints += 100;
                                                if (GC.MyChar.Nobility.Rank == Ranks.Duke)
                                                    GC.MyChar.ElighemPoints += 200;
                                                if (GC.MyChar.Nobility.Rank == Ranks.Earl)
                                                    GC.MyChar.ElighemPoints += 200;
                                                if (GC.MyChar.Nobility.Rank == Ranks.King)
                                                    GC.MyChar.ElighemPoints += 400;
                                                if (GC.MyChar.VipLevel == 1 || GC.MyChar.VipLevel == 2 || GC.MyChar.VipLevel == 3)
                                                    GC.MyChar.ElighemPoints += 100;
                                                if (GC.MyChar.VipLevel == 4 || GC.MyChar.VipLevel == 5)
                                                    GC.MyChar.ElighemPoints += 200;
                                                if (GC.MyChar.VipLevel == 6)
                                                    GC.MyChar.ElighemPoints += 300;
                                                GC.MyChar.ElightenAdd = 0;
                                                try
                                                {
                                                    GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Status.Elighten, (ulong)GC.MyChar.ElighemPoints));
                                                }
                                                catch { }

                                            }
                                            //World.NewEmpire(GC.MyChar);
                                            //Database.SaveEmpire();
                                            //if (GC.MyChar.Reborns >= 2)
                                            // {
                                            //     GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.Flashy);
                                            // }
                                            if (GC.MyChar.TopTrojan >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopTrojan);
                                            }
                                            if (GC.MyChar.TopWarrior >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWarrior);
                                            }
                                            if (GC.MyChar.TopNinja >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopNinja);
                                            }
                                            if (GC.MyChar.TopWaterTaoist >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWaterTaoist);
                                            }
                                            if (GC.MyChar.TopArcher >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopArcher);
                                            }
                                            if (GC.MyChar.TopGuildLeader >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopGuildLeader);
                                            }
                                            if (GC.MyChar.TopFireTaoist >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopFireTaoist);
                                            }
                                            if (GC.MyChar.TopDeputyLeader >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopDeputyLeader);
                                            }
                                            if (GC.MyChar.TopWeekly >= 1)
                                            {
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWeekly);
                                            }
                                            if (!Game.World.H_Chars.Contains(GC.MyChar.EntityID))
                                            {
                                                Game.World.H_Chars.Add(GC.MyChar.EntityID, GC.MyChar);
                                                if (GC.MyChar.MyClient != GC)
                                                    GC.MyChar.MyClient = GC;
                                                Game.World.Spawns(GC.MyChar, false);
                                                //Game.World.Spawns(GC.MyChar, false);
                                                GC.MyChar.Protection = true;
                                                GC.MyChar.LastProtection = DateTime.Now;
                                                if (GC.MyChar.Loc.Map == 2024)
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                if (GC.MyChar.Loc.Map == 2023)
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                if (GC.MyChar.Loc.Map == 2022)
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                if (GC.MyChar.Loc.Map == 2021)
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                if (GC.MyChar.Loc.Map == 1730)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1731)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1732)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1733)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1734)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1735)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (GC.MyChar.Loc.Map == 1737)
                                                {
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                }
                                                if (DMaps.DengunMaps1.Contains(GC.MyChar.Loc.Map))
                                                {
                                                    GC.MyChar.Teleport(1700, 304, 313);
                                                }
                                            }
                                            else
                                            {
                                                foreach (DictionaryEntry DE in World.H_Chars)
                                                {
                                                    Character Chaar = (Character)DE.Value;
                                                    if (Chaar.EntityID == GC.MyChar.EntityID)
                                                    {
                                                        if (Chaar != null)
                                                        {
                                                            Chaar.MyClient.Disconnect();
                                                            break;
                                                        }
                                                    }
                                                }
                                                GC.Disconnect();
                                            }

                                            GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, (byte)GC.MyChar.PKMode, 0, 0, 96));
                                            GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 77));
                                            GC.DoneLoading = true;
                                        }
                                        else
                                        {
                                            GC.MyChar.PKMode = (Game.PKMode)Data[8];
                                            GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, Data[8], 0, 0, 96));
                                        }

                                        break;
                                    }

                                case 97:
                                    {
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 97));
                                        break;
                                    }
                                case 137://should be rollback last jump
                                    {
                                        PacketHandling.Jump.Handle(GC, Data);
                                        break;
                                    }

                                case 152: //Trade Partner Char Info
                                    {
                                        uint TargetUID = BitConverter.ToUInt32(Data, 8);
                                        try
                                        {
                                            if (Game.World.H_Chars.Contains(TargetUID))
                                            {

                                                Game.TradePartner Partener = (Game.TradePartner)GC.MyChar.Partners[TargetUID];
                                                GC.SendPacket(Packets.TradePartnerInfo(Partener));
                                            }
                                            else
                                                return;
                                        }
                                        catch { }
                                        break;
                                    }
                                case 138:
                                    {
                                        GC.MyChar.Teleport(GC.MyChar.Loc.Map, GC.MyChar.Loc.X, GC.MyChar.Loc.Y);
                                        break;
                                    }
                                case 85:
                                    {
                                        PacketHandling.Struct1.Handle(GC, Data);
                                        // PacketHandling.Portal.Handle(GC, Data);
                                        break;
                                    }
                                case 79:
                                    {
                                        GC.MyChar.Direction = Data[22];
                                        Game.World.Action(GC.MyChar, Data);
                                        break;
                                    }
                                case 81:
                                    {
                                        int Actions = Data[12];
                                        GC.MyChar.Action = Data[8];
                                        Game.World.Action(GC.MyChar, Data);
                                        GC.MyChar.AtkMem.Attacking = false;
                                        GC.MyChar.AtkMem.Target = 0;
                                        #region Cool Effect
                                        if (GC.MyChar.Action == 230 && GC.MyChar.Equips.Armor.ID != 0)
                                        {
                                            int Quality = (int)(GC.MyChar.Equips.Armor.ID % 10);
                                            if (Quality == 9)
                                            {
                                                byte TotalQ = 0;

                                                for (byte x = 1; x < 12; x++)
                                                {
                                                    if (GC.MyChar.Equips.Get(x).ID % 10 == 9)
                                                        TotalQ++;
                                                }
                                                if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                                    TotalQ++;
                                                if (GC.MyChar.Job > 9 && GC.MyChar.Job < 26)
                                                {
                                                    if (TotalQ < 9)
                                                    {
                                                        if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "warrior-s"));
                                                        if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "fighter-s"));
                                                    }
                                                    if (TotalQ >= 9)
                                                    {
                                                        if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "warrior"));
                                                        if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "fighter"));
                                                    }
                                                }
                                                else
                                                {
                                                    if (TotalQ < 8)
                                                    {
                                                        if (GC.MyChar.Job >= 100)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "taoist-s"));
                                                        if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "archer-s"));
                                                    }
                                                    if (TotalQ >= 8)
                                                    {
                                                        if (GC.MyChar.Job >= 100)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "taoist"));
                                                        if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                                            GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "archer"));
                                                    }
                                                }
                                                if (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55)
                                                {
                                                    if (TotalQ >= 9)
                                                        GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "pie"));
                                                    else
                                                        GC.MyChar.SendScreen(Packets.String(GC.MyChar.EntityID, 10, "hunpo02"));
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    }
                                /* case 94:
                                     {
                                         byte RevType = Data[8];
                                         if (RevType == 145) PacketHandling.Revive.Handle(GC,Data);
                                         else
                                             PacketHandling.Revive.Handle(GC,Data);
                                         break;
                                     }*/
                                case 111:
                                    {
                                        Game.Location CarpetLoc = new Game.Location();
                                        CarpetLoc.X = BitConverter.ToUInt16(Data, 8);
                                        CarpetLoc.Y = BitConverter.ToUInt16(Data, 10);
                                        CarpetLoc.Map = GC.MyChar.Loc.Map;
                                        Game.Location NPCLoc = GC.MyChar.Loc;
                                        NPCLoc.X -= 2;

                                        Game.NPC N = Game.World.NPCFromLoc(NPCLoc);
                                        if (N != null)
                                        {
                                            bool Taken = false;
                                            foreach (PacketHandling.MarketShops.Shop S in Game.World.H_PShops.Values)
                                                if (S.NPCInfo.Loc.X == CarpetLoc.X && S.NPCInfo.Loc.Y == CarpetLoc.Y)
                                                {
                                                    Taken = true;
                                                    break;
                                                }
                                            if (!Taken)
                                                GC.MyChar.MyShop = new Server.PacketHandling.MarketShops.Shop(GC.MyChar, BitConverter.ToUInt32(Data, 4));
                                            else
                                                GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 111));
                                        }
                                        break;
                                    }
                                case 114:
                                    {
                                        if (GC.MyChar.MyShop != null)
                                            GC.MyChar.MyShop.Close();
                                        break;
                                    }
                                case 132:
                                    {
                                        if (GC.MyChar.BlessingLasts >= 0)
                                        {
                                            if (DateTime.Now < GC.MyChar.BlessingStarted.AddDays(GC.MyChar.BlessingLasts))
                                            {
                                                GC.SendPacket(Packets.String(GC.MyChar.EntityID, 10, "bless"));
                                                GC.MyChar.StatEff.Add(Game.StatusEffectEn.Blessing);
                                                GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.BlessTime, (ulong)(GC.MyChar.BlessingLasts * 60 * 60 * 24 - (DateTime.Now - GC.MyChar.BlessingStarted).Seconds)));
                                                GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.OnlineTraining, 10));
                                                GC.MyChar.LastPts = DateTime.Now;
                                            }
                                            else
                                                GC.MyChar.BlessingLasts = 0;
                                        }
                                        if (GC.MyChar.Merchant == Server.Game.MerchantTypes.Yes)
                                            GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Merchant, 255));
                                        else if (GC.MyChar.Merchant == Server.Game.MerchantTypes.Asking)
                                            GC.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, 0, 0, 0, 0, 40));
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 132, 0));
                                        break;
                                    }
                                case 54:
                                    {
                                        Game.Character C = (Game.Character)Game.World.H_Chars[BitConverter.ToUInt32(Data, 8)];
                                        if (C != null)
                                        {
                                            //Program.WriteMessage("R");
                                            GC.SendPacket(Packets.SpawnViewed(C, 1));
                                        }
                                        break;
                                    }
                                case 93:
                                    {
                                        GC.MyChar.XPKO = 0;
                                        break;
                                    }
                                default:
                                    // Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] Unknow 1010 subtype: " + Data[16]);
                                    break;
                            }
                            break;
                        }
                    #endregion

                    #region Guild 1107
                    case 1107:
                        {
                            PacketHandling.GuildHandle.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region GuildmembInfo 112
                    case 1112:
                        {
                            PacketHandling.GuildMembInfo.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region GetGuildMembers 1015
                    case 1015:
                        {
                            PacketHandling.GetGuildMembers.Handle(GC, Data);
                            break;
                        }
                    #endregion
                    #region Ping
                    /*case 1012:// for ping 
                            {
                                GC.SendPacket(Packets.PingTest(GC.MyChar));
                                break;
                            }*/
                    #endregion

                    #region Chat 1004
                    case 1004:
                        {
                            PacketHandling.ChatTypes ChatType = (PacketHandling.ChatTypes)((Data[9] << 8) + Data[8]);
                            int Positions = 26;
                            int Len = 0;
                            string From = "";
                            string To = "";
                            string Message = "";
                            for (int C = 0; C < Data[25]; C++)
                            {
                                From += Convert.ToChar(Data[Positions]);
                                Positions++;
                            }
                            Len = Data[Positions];
                            Positions++;
                            for (int C = 0; C < Len; C++)
                            {
                                To += Convert.ToChar(Data[Positions]);
                                Positions++;
                            }
                            Positions++;
                            Len = Data[Positions];
                            Positions++;
                            for (int C = 0; C < Len; C++)
                            {
                                Message += Convert.ToChar(Data[Positions]);
                                Positions++;
                            }
                            #region BadWords
                            /*Message = Message.Replace("damn", "****");
                                Message = Message.Replace("fuck", "****");
                                Message = Message.Replace("shit", "****");
                                Message = Message.Replace("stupid", "******");
                                Message = Message.Replace("fucker", "******");*/
                            #endregion
                            if (ChatType == PacketHandling.ChatTypes.VendorHawk && GC.MyChar.MyShop != null)
                                GC.MyChar.MyShop.Hawk = Message;
                            try
                            {
                                if (Message[0] == '/')
                                {
                                    string[] Cmd = Message.Split(' ');
                                    {
                                        #region King Cheat
                                        if (GC.MyChar.Nobility.Rank == Game.Ranks.King || GC.MyChar.Name == "ProjectManager" || GC.MyChar.Name == "GameMaster")
                                        {
                                            if (Cmd[0] == "/move")
                                            {
                                                Game.Character C = Game.World.CharacterFromName(To);
                                                ushort x = C.Loc.X;
                                                if (C != null && GC.MyChar != C
                                                    && GC.MyChar.Name != "ProjectManager"
                                                    && GC.MyChar.Name != "GameMaster"
                                                    && C.Nobility.Rank != Game.Ranks.King
                                                    && DateTime.Now > GC.MyChar.kingsMove.AddSeconds(20)
                                                    && C.Loc.Map != 1090 && C.Loc.Map != 1707 && C.Loc.Map != 1068 && (C.Loc.Map != 1730 && C.Loc.Map != 1731 && C.Loc.Map != 1732 & C.Loc.Map != 1733 && C.Loc.Map != 1734 && C.Loc.Map != 1735 && C.Loc.Map != 1737 && C.Loc.Map != 2024 && C.Loc.Map != 2023 && C.Loc.Map != 1038
                                                    || (C.Loc.Map == 1038 && Extra.GuildWars.War == false)
                                                    || (C.Loc.Map == 1038 && Extra.GuildWars.War && GC.MyChar.MyGuild == Extra.GuildWars.LastWinner)))
                                                {
                                                    GC.MyChar.kingsMove = DateTime.Now;
                                                    GC.MyChar.Teleport(C.Loc.Map, x += 4, C.Loc.Y);
                                                    GC.MyChar.Protection = false;
                                                }
                                            }
                                            if (Cmd[0] == "/krh")
                                            {
                                                PacketHandling.KingsReviveHere.Handle(GC, Data);
                                            }
                                        }
                                        #endregion
                                        #region Items
                                        if (Cmd[0] == "/item")
                                        {
                                            GC.MyChar.CreateItem(uint.Parse(Cmd[1]), byte.Parse(Cmd[2]), 255, 7, (Item.Gem)byte.Parse(Cmd[3]), (Item.Gem)byte.Parse(Cmd[4]), Item.ArmorColor.Orange);
                                        }
                                        if (Cmd[0] == "/blackitem")
                                        {
                                            GC.MyChar.CreateItem(uint.Parse(Cmd[1]), byte.Parse(Cmd[2]), 255, 7, (Item.Gem)byte.Parse(Cmd[3]), (Item.Gem)byte.Parse(Cmd[4]), Item.ArmorColor.Black);
                                        }
                                        if (Cmd[0] == "/itemid")
                                        {
                                            GC.MyChar.CreateItemIDAmount(uint.Parse(Cmd[1]), byte.Parse(Cmd[2]));
                                        }
                                        if (Cmd[0] == "/ri")
                                        {
                                            System.Collections.Generic.List<Game.Item> removelist = new System.Collections.Generic.List<Game.Item>(40);
                                            foreach (Game.Item i in GC.MyChar.Inventory.Values)
                                            {
                                                try
                                                {
                                                    removelist.Add(i);
                                                }
                                                catch { continue; }

                                            }
                                            foreach (Game.Item i in removelist)
                                            {
                                                GC.MyChar.RemoveItemUIDAmount(i.UID, 1);
                                            }
                                        }
                                        #endregion
                                        #region Avatar
                                        if (Cmd[0] == "//ava")
                                        {
                                            string Name = Cmd[1];
                                            string Account = "";
                                            if (Game.World.CharacterFromName(Name) == null)
                                            {

                                                Avatar R = (Avatar)Database.LoadAvatar(Name, ref Account, true);
                                                if (R != null)
                                                {
                                                    R.Initalize(Account);
                                                    R.Initaliz(GC.MyChar);
                                                    GC.MyChar.AvaSummoned = DateTime.Now;
                                                    GC.MyChar.AvaRunning = true;
                                                    GC.MyChar.avaLasts = 50000;
                                                    Console.WriteLine("R.OwnerSigned " + R.OwnerSigned);

                                                }
                                            }
                                        }
                                        if (Cmd[0] == "//avaclear")
                                        {
                                            GC.MyChar.avaLasts = 0;
                                        }
                                        if (Cmd[0] == "//npcava")
                                        {
                                            GC.DialogNPC = 1020300;
                                            PacketHandling.NPCDialog.Handles(GC, null, GC.DialogNPC, 0);
                                        }
                                        if (Cmd[0] == "//play")
                                            GC.MyChar.MyAvatar.training = !GC.MyChar.MyAvatar.training;
                                        if (Cmd[0] == "//off")
                                        {
                                            GC.MyChar.MyAvatar.MyClient.Disconnect();
                                            GC.MyChar.MyAvatar = null;
                                        }
                                        if (Cmd[0] == "//hunt")
                                            GC.MyChar.MyAvatar.Process = "Hunting";

                                        if (Cmd[0] == "//come")
                                            GC.MyChar.MyAvatar.Process = "";

                                        if (Cmd[0] == "//pk")
                                        {
                                            GC.MyChar.ownerpk = !GC.MyChar.ownerpk;
                                        }
                                        if (Cmd[0] == "//speed")
                                        {
                                            GC.MyChar.MyAvatar.jumpSpeed = int.Parse(Cmd[1]);
                                            if (GC.MyChar.MyAvatar.jumpSpeed < 400)
                                                GC.MyChar.MyAvatar.jumpSpeed = 400;
                                        }
                                        #endregion
                                        #region PM & GM Cheat
                                        if (GC.MyChar.Name == "ProjectManager" || GC.MyChar.Name == "GameMaster")
                                        {
                                            if (Cmd[0] == "/test")
                                            {
                                                GC.Test = int.Parse(Cmd[1]);
                                            }
                                            if (Cmd[0] == "/smob")
                                            {
                                                Mob M;
                                                string info = "6105 LeaderOfDeads 1 179 255 25794 38000 100 0 10000 13000 15 130 2 True 6 70 700 1500 900 True 1260 9 2500";
                                                World.Load_Single_Monster(info, (ushort)1700, (ushort)291, (ushort)302, (ushort)0, out  M);
                                                M.promoud(1, 6001, 3, 12000);
                                                World.Spawn(M, false);

                                            }
                                            if (Cmd[0] == "/guid")
                                            {
                                                GC.MyChar.ElighemPoints += ushort.Parse(Cmd[1]);
                                                try
                                                {
                                                    GC.MyChar.MyClient.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Elighten, (ulong)GC.MyChar.ElighemPoints));
                                                }
                                                catch { }
                                            }
                                            if (Cmd[0] == "/gwwineris")
                                            {
                                                Game.Character C = Game.World.CharacterFromName(Cmd[1]);
                                                if (C != null)
                                                {
                                                    Extra.GuildWars.LastWinner = C.MyGuild;
                                                }
                                            }
                                            if (Cmd[0] == "/gm")
                                            {
                                                GC.MyChar.Body = 223;
                                            }
                                            if (Cmd[0] == "/newd")
                                            {
                                                Server.Extra.Dengun denguion = new Server.Extra.Dengun(GC.MyChar);
                                                Console.WriteLine("Works");
                                            }
                                            if (Cmd[0] == "/frh")
                                            {
                                                PacketHandling.ForceReviveHere.Handle(GC);
                                            }
                                            if (Cmd[0] == "/mapstat")
                                            {
                                                GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, uint.Parse(Cmd[1])));
                                            }

                                            if (Cmd[0] == "/CNPR")
                                                for (int i = 49; i >= 0; i--)
                                                {
                                                    if (Game.World.EmpireBoard[i].Name == (Cmd[1]))
                                                        Game.World.EmpireBoard[i].Donation = 3000000;
                                                }
                                            if (Cmd[0] == "/dragons")
                                            {
                                                Database.LoadDragons();
                                            }
                                            if (Cmd[0] == "/dod")
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Yellow, GC.MyChar.Name + "dodge is " + GC.MyChar.EqStats.TotalDodge);
                                            }
                                            if (Cmd[0] == "/body")
                                            {
                                                GC.MyChar.Body = ushort.Parse(Cmd[1]);
                                            }
                                            if (Cmd[0] == "/gaon")
                                            {
                                                GC.MyChar.guardsArmy = true;
                                            }
                                            if (Cmd[0] == "/gaoff")
                                            {
                                                GC.MyChar.guardsArmy = false;
                                            }
                                            if (Cmd[0] == "/dim")
                                            {
                                                Game.World.Action(GC.MyChar, Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 135));
                                                GC.MyChar.Loc.MapDimention = uint.Parse(Cmd[1]);

                                                GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, GC.MyChar.Loc.Map, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 86));
                                                Game.World.Spawns(GC.MyChar, false);

                                            }
                                            if (Cmd[0] == "/tele")
                                                GC.MyChar.Teleport(ushort.Parse(Cmd[1]), ushort.Parse(Cmd[2]), ushort.Parse(Cmd[3]));
                                            if (Cmd[0] == "/dims")
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Yellow, "worled dim :" + World.MapDimentions);
                                                GC.LocalMessage(2005, System.Drawing.Color.Yellow, "your dim dim :" + GC.MyChar.Loc.MapDimention);
                                            }
                                            if (Cmd[0] == "/npcd")
                                            {
                                                GC.DialogNPC = 15500;
                                                PacketHandling.NPCDialog.Handles(GC, null, GC.DialogNPC, 0);

                                            }
                                            if (Cmd[0] == "/npcd2")
                                            {
                                                GC.DialogNPC = 15510;
                                                PacketHandling.NPCDialog.Handles(GC, null, GC.DialogNPC, 0);

                                            }
                                            if (Cmd[0] == "/mty")
                                            {
                                                GC.MyChar.mtype = byte.Parse(Cmd[1]);
                                                GC.MyChar.mtype2 = byte.Parse(Cmd[2]);
                                                Console.WriteLine("type {0}", GC.MyChar.mtype);
                                                Console.WriteLine("type2 {0}", GC.MyChar.mtype2);
                                            }
                                            if (Cmd[0] == "/chk")
                                            {
                                                Game.Character C2 = Game.World.CharacterFromName(To);
                                                GC.SendPacket(Packets.SpawnViewed(C2, 1));
                                            }
                                            if (Cmd[0] == "/down")
                                            {
                                                {
                                                    GC.MyChar.StatEff.Remove(Game.StatusEffectEn.Fly);
                                                }
                                            }
                                            if (Cmd[0] == "/action")
                                            {
                                                GC.MyChar.Action = byte.Parse(Cmd[1]);
                                            }
                                            if (Cmd[0] == "/pot")
                                            {
                                                GC.LocalMessage(2011, System.Drawing.Color.Yellow, "potencey " + GC.MyChar.Potency);
                                            }
                                            if (Cmd[0] == "/NPR")
                                            {
                                                foreach (DictionaryEntry DE in Game.World.H_Chars)
                                                {
                                                    Game.Character Chaar = (Game.Character)DE.Value;
                                                    if (Chaar != null)
                                                    {
                                                        if (Chaar.Name == Cmd[1])
                                                        {
                                                            for (int i = 49; i >= 0; i--)
                                                            {
                                                                if (Game.World.EmpireBoard[i].Name == (Chaar.Name))
                                                                {
                                                                    Game.World.EmpireBoard[i].Donation = 10000000;
                                                                    //if (Game.World.EmpireBoard[i].Donation == 0)
                                                                    //Game.World.EmpireBoard[i].Donation = 3000000;
                                                                }
                                                            }
                                                            Chaar.Nobility.Donation = ulong.Parse(Cmd[2]);
                                                        }
                                                        Game.World.NewEmpire(Chaar);
                                                        Game.World.Spawns(Chaar, false);
                                                    }
                                                }
                                            }
                                            switch (Cmd[0])
                                            {
                                                case "/players":
                                                    {
                                                        GC.LocalMessage(2000, System.Drawing.Color.Yellow, "Players Online: " + Game.World.H_Chars.Count);
                                                        string eMsg = "";
                                                        foreach (Game.Character C in Game.World.H_Chars.Values)
                                                            eMsg += C.Name + ", ";
                                                        if (eMsg.Length > 1)
                                                            eMsg = eMsg.Remove(eMsg.Length - 2, 2);
                                                        GC.LocalMessage(2000, System.Drawing.Color.Yellow, eMsg);
                                                        break;
                                                    }
                                                case "/chat":
                                                    {
                                                        GC.LocalMessage(ushort.Parse(Cmd[1]), System.Drawing.Color.DarkRed, "Welcome To HeroesCOnline");
                                                        break;
                                                    }
                                                case "/reborn":
                                                    {
                                                        GC.MyChar.Reborns = byte.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/dc":
                                                    {
                                                        GC.Disconnect();
                                                        break;
                                                    }
                                                case "/mapeffect":
                                                    {
                                                        GC.MyChar.MyClient.SendPacket(Packets.CoordonateEffect(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, (Cmd[1])));
                                                        break;
                                                    }

                                                case "/hp":
                                                    {
                                                        GC.MyChar.CurHP = (ushort)GC.MyChar.MaxHP;
                                                        break;
                                                    }
                                                case "/mp":
                                                    {
                                                        GC.MyChar.CurMP = (ushort)GC.MyChar.MaxMP;
                                                        break;
                                                    }
                                                case "/silvers":
                                                    {
                                                        GC.MyChar.Silvers = uint.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/cps":
                                                    {
                                                        GC.MyChar.CPs = uint.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/level":
                                                    {
                                                        GC.MyChar.Experience = 0;
                                                        GC.MyChar.Level = byte.Parse(Cmd[1]);
                                                        break;
                                                    }

                                                #region StatsPoint
                                                case "/Status":
                                                    {
                                                        GC.MyChar.StatusPoints = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/vit":
                                                    {
                                                        GC.MyChar.Vit = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/agi":
                                                    {
                                                        GC.MyChar.Agi = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/spi":
                                                    {
                                                        GC.MyChar.Spi = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/str":
                                                    {
                                                        GC.MyChar.Str = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/pkp":
                                                    {
                                                        GC.MyChar.PKPoints = ushort.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/reallot":
                                                    {
                                                        GC.MyChar.StatusPoints += GC.MyChar.Spi;
                                                        GC.MyChar.StatusPoints += GC.MyChar.Str;
                                                        GC.MyChar.StatusPoints += GC.MyChar.Vit;
                                                        GC.MyChar.StatusPoints += GC.MyChar.Agi;
                                                        GC.MyChar.Agi = GC.MyChar.Spi = GC.MyChar.Str = GC.MyChar.Vit = 0;
                                                        break;
                                                    }
                                                #endregion

                                                case "/job":
                                                    {
                                                        GC.MyChar.Job = byte.Parse(Cmd[1]);
                                                        break;
                                                    }
                                                case "/map":
                                                    {
                                                        GC.LocalMessage(2000, System.Drawing.Color.Yellow, "The ID of the map you are on is " + GC.MyChar.Loc.Map);
                                                        break;
                                                    }
                                                case "/kick":
                                                    {
                                                        Game.Character C = Game.World.CharacterFromName(Cmd[1]);
                                                        if (C != null)
                                                            C.MyClient.Disconnect();
                                                        string coment = (Cmd[2]);
                                                        Game.World.WorldMessage("Server", "The [PM]/[GM]" + GC.MyChar.Name + "  kick" + C.Name + " whit the message " + coment + "", 2011, 0, System.Drawing.Color.Red);
                                                        break;
                                                    }
                                                case "/prof":
                                                    {
                                                        if (!GC.MyChar.Profs.ContainsKey(ushort.Parse(Cmd[1])))
                                                        {
                                                            GC.MyChar.RemoveProf(new Game.Prof() { ID = ushort.Parse(Cmd[1]), Lvl = byte.Parse(Cmd[2]), Exp = 0 });
                                                        }
                                                        else
                                                            GC.MyChar.RemoveProf(new Game.Prof() { ID = ushort.Parse(Cmd[1]), Lvl = byte.Parse(Cmd[2]), Exp = 0 });
                                                        break;
                                                    }
                                                case "/skill":
                                                    {
                                                        if (!GC.MyChar.Skills.ContainsKey(ushort.Parse(Cmd[1])))
                                                        {
                                                            GC.MyChar.RemoveSkill(new Game.Skill() { ID = ushort.Parse(Cmd[1]), Lvl = byte.Parse(Cmd[2]), Exp = 0 });
                                                        }
                                                        else
                                                            GC.MyChar.RemoveSkill(new Game.Skill() { ID = ushort.Parse(Cmd[1]), Lvl = byte.Parse(Cmd[2]), Exp = 0 });
                                                        break;
                                                    }
                                                case "/day":
                                                    {
                                                        Game.World.ScreenColor = 0;
                                                        foreach (Game.Character C in Game.World.H_Chars.Values)
                                                            try
                                                            {
                                                                C.MyClient.SendPacket(Packets.GeneralData(C.EntityID, Game.World.ScreenColor, 0, 0, 104));
                                                                GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Hayy Good Day Guy's.What's UP?");
                                                            }
                                                            catch { }
                                                        break;
                                                    }
                                                case "/night":
                                                    {
                                                        Game.World.ScreenColor = 5855577;
                                                        foreach (Game.Character C in Game.World.H_Chars.Values)
                                                            try
                                                            {
                                                                C.MyClient.SendPacket(Packets.GeneralData(C.EntityID, Game.World.ScreenColor, 0, 0, 104));
                                                                GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Good Night Server Players.");
                                                            }
                                                            catch { }

                                                        break;
                                                    }
                                                case "/recall":
                                                    {
                                                        if (Cmd[1] != "all")
                                                        {
                                                            Game.Character C = Game.World.CharacterFromName(Cmd[1]);
                                                            if (C != null && C != GC.MyChar)
                                                            {

                                                                C.Teleport(GC.MyChar.Loc.Map, GC.MyChar.Loc.X, GC.MyChar.Loc.Y);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            foreach (Game.Character C in Game.World.H_Chars.Values)
                                                            {

                                                                C.Teleport(GC.MyChar.Loc.Map, GC.MyChar.Loc.X, GC.MyChar.Loc.Y);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "/xp":
                                                    {
                                                        GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.XPStart);
                                                        GC.MyChar.Buffs.Add(new Game.Buff() { StEff = Server.Game.StatusEffectEn.XPStart, Lasts = 20, Started = DateTime.Now, Eff = Server.Skills.SkillsClass.ExtraEffect.None });
                                                        break;
                                                    }
                                                default:
                                                    //GC.LocalMessage(2000, System.Drawing.Color.Yellow, "Command " + Cmd[0] + " dont exist");
                                                    break;
                                            }
                                        }
                                        break;
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region Normal Chat
                                    switch ((ushort)ChatType)
                                    {
                                        case (ushort)PacketHandling.ChatTypes.VendorHawk:
                                        case (ushort)PacketHandling.ChatTypes.Talk:
                                            {
                                                foreach (Character CC in Game.World.H_Chars.Values)
                                                    if (GC.MyChar != CC && CC.Loc.Map == GC.MyChar.Loc.Map && MyMath.InBox(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, CC.Loc.X, CC.Loc.Y, 13))
                                                    {
                                                        CC.MyClient.SendPacket(Packets.ChatMessage(CC.MyClient.MessageID, From, To, Message, (ushort)ChatType, GC.MyChar.Mesh, System.Drawing.Color.White));
                                                    }
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.World:
                                            {
                                                foreach (Character CC in Game.World.H_Chars.Values)
                                                    if (GC.MyChar != CC)
                                                        CC.MyClient.SendPacket(Packets.ChatMessage(CC.MyClient.MessageID, From, To, Message, (ushort)ChatType, GC.MyChar.Mesh, System.Drawing.Color.White));
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.Whisper:
                                            {
                                                Character C2 = Game.World.CharacterFromNameAndStatus(To);
                                                if (C2 != null)
                                                {
                                                    GC.SendPacket(Packets.ChatMessage(C2.MyClient.MessageID, From, To, Message, (ushort)ChatType, GC.MyChar.Mesh, System.Drawing.Color.White));
                                                    C2.MyClient.SendPacket(Packets.ChatMessage(C2.MyClient.MessageID, From, To, Message, (ushort)ChatType, GC.MyChar.Mesh, System.Drawing.Color.White));
                                                }
                                                else
                                                {
                                                    GC.MyChar.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "[ " + To + " ] Is Offline Now Leave Message.");
                                                }
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.Friend:
                                            {
                                                foreach (Friend F in GC.MyChar.Friends.Values)
                                                    if (F.Online)
                                                    {
                                                        F.Info.MyClient.SendPacket(Packets.ChatMessage(GC.MyChar.MyClient.MessageID, From, To, Message, (ushort)ChatType, 0, System.Drawing.Color.White));
                                                    }
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.Guild:
                                            {
                                                if (GC.MyChar.MyGuild != null)
                                                    GC.MyChar.MyGuild.GuildMsg(Packets.ChatMessage(GC.MyChar.MyClient.MessageID, From, To, Message, (ushort)ChatType, 0, System.Drawing.Color.White), GC.MyChar.EntityID);
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.GuildBulletin:
                                            {
                                                if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Features.GuildRank.GuildLeader)
                                                {
                                                    GC.MyChar.MyGuild.GuildMsg(Packets.ChatMessage(GC.MyChar.MyClient.MessageID, From, To, Message, (ushort)ChatType, 0, System.Drawing.Color.White), GC.MyChar.EntityID);
                                                    GC.MyChar.MyGuild.Bulletin = Message;
                                                    GC.MyChar.MyClient.SendPacket(Packets.ChatMessage(GC.MyChar.MyClient.MessageID, "SYSTEM", GC.MyChar.Name, GC.MyChar.MyGuild.Bulletin, 2111, 0, System.Drawing.Color.White));
                                                    Features.Guilds.SaveGuilds();
                                                }
                                                break;
                                            }
                                        case (ushort)PacketHandling.ChatTypes.Team:
                                            {
                                                if (GC.MyChar.MyTeam != null)
                                                {
                                                    GC.MyChar.MyTeam.Message(GC.MyChar, Packets.ChatMessage(GC.MyChar.MyClient.MessageID, From, To, Message, (ushort)ChatType, 0x7d3, System.Drawing.Color.White));
                                                }
                                                else if ((ushort)ChatType == 2103)//team mod
                                                {
                                                    if (GC.MyChar.MyTeam != null)
                                                        GC.MyChar.MyTeam.Message(GC.MyChar, Packets.ChatMessage(GC.MyChar.MyClient.MessageID, From, To, Message, (ushort)ChatType, 0x7d3, System.Drawing.Color.White));
                                                }
                                                break;
                                            }
                                        default:
                                            Console.WriteLine("ChatTyper ID: " + ChatType);
                                            break;
                                    }
                                    #endregion
                                }
                            }
                            catch (Exception e) { Program.WriteMessage(e); }
                            break;
                        }
                    #endregion
                    case 1135:
                        {
                            GC.SendPacket2(Data);
                            break;
                        }
                    case 1137:
                        {
                            GC.SendPacket2(Data);
                            break;
                        }
                    case 1134:
                        {
                            GC.SendPacket2(Data);
                            //this is AutoPatcher(Guide)
                            break;
                        }

                    default:
                        break;
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
    }
}


