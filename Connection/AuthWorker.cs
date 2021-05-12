using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Game;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    public class AuthWorker
    {

        public struct AuthInfo
        {
            public enum AccountState : byte
            { ProjectManager = 4, GameMaster = 3, Player = 2, Banned = 1, DoesntExist = 0 }
            public string Account;
            public string Character;
            public byte LogonType;
            public string Status;
        }
        public class AuthClient
        {
            public Socket Soc;
            public Cryption Crypto;
            public unsafe void Send(COPacket P)
            {
                try
                {
                    if (Soc.Connected)
                    {
                        try
                        {
                            byte[] data = P.Get;
                            P.Get.CopyTo(data, 0);
                            //foreach (byte D in data)
                            //{
                            //  Console.Write((Convert.ToString(D, 16)).PadLeft(2, '0') + " ");
                            //}
                            //Console.WriteLine();
                            //Console.WriteLine();
                            byte[] Data = new byte[data.Length];
                            data.CopyTo(Data, 0);
                            Crypto.Encrypt(Data);
                            Soc.Send(Data, Data.Length, SocketFlags.None);
                        }
                        catch { }
                    }
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            public void Disconnect()
            {
                try
                {
                    if (Soc.Connected)
                    {
                        Soc.Shutdown(SocketShutdown.Both);
                        Soc.Close();
                    }
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
        }
        static string GameIP;
        public static Connection Listener;

        static Random Rnd = new Random();
        public static Hashtable KeyedClients = new Hashtable();

        public static void StartServer(string GameIP)
        {
            AuthWorker.GameIP = GameIP;
            try
            {
                Listener = new Connection();
                Listener.SetConnHandler(new ConnectionArrived(ConnectionHandler));
                Listener.SetDataHandler(new DataArrived(DataHandler));
                Listener.SetDCHandler(new Disconnection(DCHandler));
                Listener.StartServer(World.ServerInfo.AuthPort);
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
        static void ConnectionHandler(StateObj StO)
        {
            try
            {
                AuthClient AC = new AuthClient();
                AC.Crypto = new Cryption();
                AC.Soc = StO.Sock;
                StO.Wrapper = AC;
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        static void DataHandler(StateObj StO, byte[] Data)
        {
            try
            {
                AuthClient AC = (AuthClient)StO.Wrapper;
                if (Data.Length == 276)
                {
                    if (AC != null)
                    {
                        AC.Crypto.Decrypt(Data);
                        if (Data[0] == 0x14 && Data[1] == 0x01 && Data[2] == 0x3e && Data[3] == 0x04)
                        {
                            MemoryStream MS = new MemoryStream(Data);
                            BinaryReader BR = new BinaryReader(MS);
                            ushort PacketLength = BR.ReadUInt16();
                            ushort PacketID = BR.ReadUInt16();

                            if (PacketID == 1086)
                            {
                                string Account = Encoding.ASCII.GetString(BR.ReadBytes(16));
                                Account = Account.Replace("\0", "");
                                BR.ReadBytes(112);
                                string Password = Encoding.ASCII.GetString(BR.ReadBytes(16));
                                BR.ReadBytes(112);
                                string Server = Encoding.ASCII.GetString(BR.ReadBytes(16));
                                Server = Server.Replace("\0", "");
                                if (Server == Game.World.ServerInfo.ServerName)
                                {
                                    AuthInfo Info = Database.Authenticate(Account, Password);

                                    if (Info.LogonType != 255)
                                    {

                                        byte[] IV = new byte[8];
                                        for (int i = 0; i < 8; i++)
                                            IV[i] = (byte)Rnd.Next(255);

                                        if (!KeyedClients.Contains(BitConverter.ToUInt64(IV, 0)))
                                            KeyedClients.Add(BitConverter.ToUInt64(IV, 0), Info);


                                        AC.Send(Packets.SendAuthentication(GameIP, IV));
                                    }
                                    else
                                    {
                                        AC.Send(Packets.WrongAuth());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Server Name");
                                    AC.Soc.Disconnect(false);
                                }
                            }


                            BR.Close();
                            MS.Close();

                        }
                        else
                        {
                            Console.WriteLine("Unknown packet on Auth server");
                            foreach (byte D in Data)
                            {
                                Console.Write((Convert.ToString(D, 16)).PadLeft(2, '0') + " ");
                            }
                            Console.WriteLine();
                            Console.WriteLine();
#if DEBUG
                            Console.WriteLine("Unknown packet on Auth server");
#endif
                            AC.Soc.Disconnect(false);

                            Data = null;
                        }
                    }
                    else
                        AC.Soc.Disconnect(false);
                }
                else
                    AC.Soc.Disconnect(false);

            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        static void DCHandler(StateObj StO)
        {
            try
            {
                StO.Sock.Disconnect(true);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
    }
    unsafe public class PassCrypto
    {
        static UInt32 LeftRotate(UInt32 var, UInt32 offset)
        {
            UInt32 tmp1, tmp2;
            offset &= 0x1f;
            tmp1 = var >> (int)(32 - offset);
            tmp2 = var << (int)offset;
            tmp2 |= tmp1;
            return tmp2;
        }
        static UInt32 RightRotate(UInt32 var, UInt32 offset)
        {
            UInt32 tmp1, tmp2;
            offset &= 0x1f;
            tmp1 = var << (int)(32 - offset);
            tmp2 = var >> (int)offset;
            tmp2 |= tmp1;
            return tmp2;
        }
        static uint[] key = new uint[] {
                0xEBE854BC, 0xB04998F7, 0xFFFAA88C, 
                0x96E854BB, 0xA9915556, 0x48E44110, 
                0x9F32308F, 0x27F41D3E, 0xCF4F3523, 
                0xEAC3C6B4, 0xE9EA5E03, 0xE5974BBA, 
                0x334D7692, 0x2C6BCF2E, 0xDC53B74,  
                0x995C92A6, 0x7E4F6D77, 0x1EB2B79F, 
                0x1D348D89, 0xED641354, 0x15E04A9D,
                0x488DA159, 0x647817D3, 0x8CA0BC20, 
                0x9264F7FE, 0x91E78C6C, 0x5C9A07FB, 
                0xABD4DCCE, 0x6416F98D, 0x6642AB5B
        };
        public static string EncryptPassword(string password)
        {
            UInt32 tmp1, tmp2, tmp3, tmp4, A, B, chiperOffset, chiperContent;

            byte[] plain = new byte[16];
            Encoding.ASCII.GetBytes(password, 0, password.Length, plain, 0);

            MemoryStream mStream = new MemoryStream(plain);
            BinaryReader bReader = new BinaryReader(mStream);
            UInt32[] pSeeds = new UInt32[4];
            for (int i = 0; i < 4; i++) pSeeds[i] = bReader.ReadUInt32();
            bReader.Close();

            chiperOffset = 7;

            byte[] encrypted = new byte[plain.Length];
            MemoryStream eStream = new MemoryStream(encrypted);
            BinaryWriter bWriter = new BinaryWriter(eStream);

            for (int j = 0; j < 2; j++)
            {
                tmp1 = tmp2 = tmp3 = tmp4 = 0;
                tmp1 = key[5];
                tmp2 = pSeeds[j * 2];
                tmp3 = key[4];
                tmp4 = pSeeds[j * 2 + 1];

                tmp2 += tmp3;
                tmp1 += tmp4;

                A = B = 0;

                for (int i = 0; i < 12; i++)
                {
                    chiperContent = 0;
                    A = LeftRotate(tmp1 ^ tmp2, tmp1);
                    chiperContent = key[chiperOffset + i * 2 - 1];
                    tmp2 = A + chiperContent;

                    B = LeftRotate(tmp1 ^ tmp2, tmp2);
                    chiperContent = key[chiperOffset + i * 2];
                    tmp1 = B + chiperContent;
                }

                bWriter.Write(tmp2);
                bWriter.Write(tmp1);
            }
            bWriter.Close();

            return ASCIIEncoding.ASCII.GetString(encrypted);

        }
    }
}
