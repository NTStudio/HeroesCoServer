using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenSSL.Core;

namespace Server
{
    class GameWorker
    {
        public static Connection Listener;
        static System.Random Rnd = new System.Random();

        public static void StartServer()
        {
            try
            {
                Listener = new Connection();
                Listener.SetConnHandler(new ConnectionArrived(ConnectionHandler));
                Listener.SetDataHandler(new DataArrived(DataHandler));
                Listener.SetDCHandler(new Disconnection(DCHandler));
                Listener.StartServer(5816);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        public static void Close()
        {
            try
            {
                Listener.Close();
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        static void ConnectionHandler(StateObj S)
        {
            try
            {
                GameClient C = new GameClient(false, S);
                C.Soc = S.Sock;
                S.Wrapper = C;
                C.SendPacket(Packets.DHKeyPacket(C.KeyExchance.PublicKey.ToHexString(), C.NewServerIV, C.NewClientIV));
                //C.StartReceiveing();
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        public static void DataHandler(StateObj StO, Byte[] Data)
        {
            //data2[0] = 0;
            try
            {

                GameClient GC = StO.Wrapper as GameClient;
                GC.Crypto.Decrypt(Data);
                if (GC != null)
                {
                    if (GC.SetBF)
                    {
                        try
                        {

                            GC.SetBF = false;
                            ushort position = 7;
                            uint PacketLen = BitConverter.ToUInt32(Data, position);
                            position += 4;
                            int JunkLen = BitConverter.ToInt32(Data, position);
                            position += 4;
                            position += (ushort)JunkLen;
                            int Len = BitConverter.ToInt32(Data, position);
                            position += 4;
                            byte[] pubKey = new byte[Len];
                            for (int x = 0; x < Len; x++)
                                pubKey[x] = Data[x + position];
                            string PubKey = System.Text.ASCIIEncoding.ASCII.GetString(pubKey);

                            GC.Crypto = new GameCrypto(GC.KeyExchance.ComputeKey(BigNumber.FromHexString(PubKey)));

                            GC.Crypto.Blowfish.DecryptIV = GC.NewClientIV;
                            GC.Crypto.Blowfish.EncryptIV = GC.NewServerIV;
                        }
                        catch
                        {
                            Console.WriteLine("game worker [DC]");
                            GC.Disconnect();
                        }
                    }
                    else //if (Data[0] == 0x1c && Data[1] == 0x00 && Data[2] == 0x1c && Data[3] == 0x04)
                    {
                        uint packet = BitConverter.ToUInt16(Data, 2);
                        if (packet == 1052)
                        {
                            try
                            {

                                ulong CryptoKey = BitConverter.ToUInt64(Data, 4);

                                AuthWorker.AuthInfo Info = (AuthWorker.AuthInfo)AuthWorker.KeyedClients[CryptoKey];
                                GC.AuthInfo = Info;
                                GC.MessageID = (uint)Rnd.Next(50000);
                                GC.Soc = StO.Sock;
                                if (GC.AuthInfo.LogonType == 3)
                                {
                                    GC.SendPacket(Packets.SystemMessage(GC.MessageID, "You have been banned!"));
                                }
                                if (GC.AuthInfo.LogonType == 2)
                                {
                                    GC.SendPacket(Packets.SystemMessage(GC.MessageID, "NEW_ROLE"));

                                    System.Threading.Thread.Sleep(100);
                                    //GC.Soc.Disconnect(false);
                                    return;
                                }
                                else if (GC.AuthInfo.LogonType == 1)
                                {
                                    string Acc = "";

                                    foreach (Game.Character Character in Game.World.H_Chars.Values)
                                    {
                                        if (Character.MyClient.AuthInfo.Character == GC.AuthInfo.Character)
                                        {
                                            GC.SendPacket(Packets.SystemMessage(GC.MessageID, "Account in use.!Try again now"));

                                            System.Threading.Thread.Sleep(100);
                                            Character.MyClient.Disconnect();
                                            System.Threading.Thread.Sleep(100);
                                            GC.Soc.Disconnect(false);
                                            return;
                                        }
                                    }
                                    GC.MyChar = Database.LoadCharacter(GC.AuthInfo.Character, ref Acc, false);
                                    try
                                    {
                                        GC.MyChar.MyClient = GC;
                                    }
                                    catch { GC.Soc.Disconnect(false); Console.WriteLine("oba7 ");return;  }
                                    GC.SendPacket(Packets.SystemMessage(GC.MessageID, "ANSWER_OK"));
                                    
                                    GC.SendPacket(Packets.CharacterInfo(GC.MyChar));
                                    GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.VIPLevel, GC.MyChar.VipLevel));
                                    GC.SendPacket(Packets.Time());
                                    GC.SendPacket(Packets.Packet(PacketType.NobilityPacket, GC.MyChar));
                                    //GC.SendPacket(Packets.Donators(GC.MyChar));
                                    GC.SendPacket(Packets.Packet1012(GC.MyChar.EntityID));
                                    GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Effect, 0));
                                    GC.MyChar.Stamina = 100;
                                    Program.WriteMessage("Character [" + GC.MyChar.Name + "] Connected Sucsessfuly");
                                    //Program.WriteMessage("[" + DateTime.Now.ToLongTimeString() + "] [" + GC.Soc.RemoteEndPoint.ToString().Split(':')[0].ToString() + "]" + GC.MyChar.Name + " from (" + Program.Lookup.lookupCountryName(GC.IpAdres) + ") has logged on.");
                                    GC.WorldMessage(2001, "" + GC.MyChar.Name + " Has LoggedOn.", System.Drawing.Color.White);
                                }
                            }
                            catch { GC.Soc.Disconnect(false); }
                        }
                        else
                            PacketHandler.HandleBuffer(Data, GC);
                        //PacketHandler.Handle(GC, Data);
                    }
                }
                else
                {
                    GC.Crypto.Decrypt(Data);
                    PacketHandler.Handle(GC, Data);
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        static void DCHandler(StateObj StO)
        {
            try
            {
                Program.WriteMessage("[DC]Handler");
                GameClient GC = (GameClient)StO.Wrapper;
                //if (GC != null && GC.MyChar != null)
                //{
                    GC.LogOff(true);
                //}
                   
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
    }
}