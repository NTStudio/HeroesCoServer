using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using OpenSSL;
using System.Threading;
using Server.Game;
using OpenSSL.Crypto;
using OpenSSL.Core;

namespace Server
{
    public struct PacketPag
    {
       public byte[] Packet;
       public byte packetType;
    }
    public class GameClient
    {
        public Character MyChar;
        public AuthWorker.AuthInfo AuthInfo;
        public GameCrypto Crypto;

        public string IpAdres
        {
            get
            {
                return Soc.RemoteEndPoint.ToString().Split(':')[0];
            }
        }
        public Socket Socket2
        { get { return Soc; } }

        public DH KeyExchance;
        public byte[] NewServerIV;
        public byte[] NewClientIV;
        public bool SetBF = true;
        public bool Robot = false;

        public byte wil = 0;
        public byte _FixedStart = 0;
        public Socket Soc;
        public Socket _socket;
        public uint MessageID = 0;
        public bool DoneLoading = false;
        public uint DialogNPC = 0;
        public bool Paid = false;
        public bool Agreed = false;

        public bool SpawnOnHold = false;
        public ushort SpawnXStart = 0;
        public ushort SpawnYStart = 0;

        public bool LoginDataSent = false;
        public ArrayList PacketGroup;

        public void LocalMessage(ushort ChatType, System.Drawing.Color Color, string Message)
        {
            SendPacket(Packets.ChatMessage(MessageID, "Server", MyChar.Name, Message, ChatType, MyChar.Mesh, Color));
        }
        public void WorldMessage(ushort ChatType, string Message, System.Drawing.Color Color)
        {
            try
            {
                foreach (Game.Character G in Game.World.H_Chars.Values)
                {
                    if (!G.isAvatar)
                    {
                        G.MyClient.SendPacket(Packets.ChatMessage(MessageID, "Server", MyChar.Name, Message, ChatType, MyChar.Mesh, Color));
                    }
                }
            }
            catch { }
        }
        
        public Server.StateObj Receive;
       /* public void intern_receive()
        {
            while (Continue)
            {
                if (Socket2.Connected)
                {
                    byte[] Data = null;
                    int size;
                    try
                    {
                        try
                        {
                            size = Socket2.Receive(Receive.Data, SocketFlags.None);
                        }
                        catch
                        {
                            Disconnect();
                            Continue = false;
                            return;
                        }
                        if (size <= 0)
                        {
                            Disconnect();
                            Continue = false;
                            return;
                        }
                        if (size < 1024)
                        {
                            Data = new byte[size];
                            Array.Copy(Receive.Data, Data, size);
                        }
                        else
                        {
                            Console.WriteLine("[" + this.IpAdres + "][" + this.AuthInfo.Account + "][" + this.MyChar.Name + "] ->  Packet too large for client to send.");
                            Disconnect();
                            Continue = false;
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[" + this.IpAdres + "][" + this.AuthInfo.Account + "][" + this.MyChar.Name + "] ->  hay.");
                        Program.WriteMessage(e.ToString());
                        Disconnect();
                        Continue = false;
                        break;
                    }
                    if (Data != null && Data.Length > 3)
                    {
                        try
                        {
                            GameWorker.DataHandler(Receive, Data);
                        }
                        catch (Exception e)
                        {
                            Program.WriteMessage(e);
                        }
                    }
                    Thread.Sleep(20);
                }
                else
                {
                    Continue = false;
                    Console.WriteLine("socket2 is not connected [DC]");
                    Disconnect();
                    return;
                }
            }
        }*/
        private System.Timers.Timer Timer;
        public GameClient(bool Robot,StateObj Sto)
        {
            if (!Robot)
            Soc = Sto.Sock;
            if (!Robot)
            {
                try
                {
                    PacketGroup = new ArrayList();
                    Timer = new System.Timers.Timer();
                    Timer.Interval = 5;
                    Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                    Timer.Start();

                    Crypto = new GameCrypto(ASCIIEncoding.ASCII.GetBytes("DR654dt34trg4UI6"));//2316dgbsu123djdj
                    KeyExchance = new DH();
                    KeyExchance.G = 05;
                    KeyExchance.P = BigNumber.FromHexString("A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB\0");//A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB\0
                    KeyExchance.GenerateKeys();
                    
                    NewServerIV = GenerateIV();
                    NewClientIV = GenerateIV();
                }
                catch (Exception Exc) { Program.WriteMessage(Exc); }
            }
            else
            {
                MessageID = (ushort)Program.Rnd.Next(50000);
            }
            //Continue = true;
            Receive = new StateObj();
            Receive.Data = new byte[1096];
            Receive.Sock = Soc;
           
            Receive.Wrapper = this;
        }
        public GameClient()
        {
            {
                MessageID = (ushort)Program.Rnd.Next(50000);
            }
        }
        byte[] GenerateIV()
        {
            System.Random Rnd = new System.Random();
            byte[] iv = new byte[8];
            for (byte i = 0; i < 8; i++)
                iv[i] = (byte)Rnd.Next(byte.MaxValue);
            return iv;
        }
        public void SendPacket(byte[] data)
        {
            while (coping)
            {
            }
            Backing = true;
            if (!Robot)
            {
                PacketPag P = new PacketPag();
                P.packetType = 1;
                P.Packet = data;
                PacketGroup.Add(P);
            }
            Backing = false;
        }
        public void SendPacket2(byte[] packet)
        {
            while (coping)
            {
            }
            Backing = true;
            if (!Robot)
            {
                PacketPag P = new PacketPag();
                P.packetType = 2;
                P.Packet = packet;
                PacketGroup.Add(P);
            }
            Backing = false;

        }
       void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           try
           {
               //Program.WriteMessage("Tek");

               //if (!Program.EndSession)
               PacketSenderStep();
           }
           catch (Exception E) { Program.WriteMessage(E); }
       }
       ArrayList Ps;
       bool coping = false;
       bool Backing = false;
       void PacketSenderStep()
       {
           if (!Backing)
               if (PacketGroup.Count > 0)
               {
                   // Program.WriteMessage(">sending " + PacketGroup.Count);

                   {
                       coping = true;
                       Ps = new ArrayList(PacketGroup);
                       PacketGroup.Clear();
                       coping = false;
                   }
                   //Program.WriteMessage("sending " + PacketGroup.Count + "\n");
                   try
                   {
                       //PacketGroup.CopyTo(Ps, 0);
                      
                       if (Ps != null)
                       {
                           foreach (PacketPag p in Ps)
                           {
                               if (!p.Equals(null))
                               {
                                   //Console.WriteLine("sending by console " + p.packetType);
                                   if (p.packetType == 1)
                                       Send(p.Packet);
                                   else if (p.packetType == 2)
                                       Send2(p.Packet);
                               }
                           }
                       }
                   }
                   catch (Exception E) { Program.WriteMessage(E); }
                   Ps = null;
               }
       }

       public int Test;
       byte[] _packet = new byte[1024];
       public void Send(byte[] data)
       {
          // Program.WriteMessage("Tek");
           if (!Robot)
               if (Monitor.TryEnter(this, new TimeSpan(0, 0, 0, 30, 0)))
               {
                   if (MyChar != null)
                   {
                       //Program.WriteMessage("Monitor Entered " + MyChar.Name);
                   }

                   try
                   {
                       _packet = data;
                       run();
                       //System.Threading.Thread.Sleep(Test);
                       Monitor.Exit(this);
                   }
                   catch (Exception e)
                   {
                       Monitor.Exit(this);
                       Program.WriteMessage(e.ToString());
                   }
               }
               else
               {
                   Program.WriteMessage("Can not enter Monitor "+ MyChar.Name);
                   Disconnect();
               }
           else
               return;
       }
       public void run()
       {
           while (wil == 1)
               Thread.Sleep(10);
           if (Soc.Connected)
           {
               wil = 1;
               try
               {
                 //  Monitor.TryEnter(this, new TimeSpan(0, 0, 0, 20, 0));
                   byte[] data = new byte[_packet.Length];
                   _packet.CopyTo(data, 0);
                   try
                   {
                       //  byte[] getbyte = new byte[256];

                       if (data.Length <= 1024)
                       {

                           //foreach (byte D in data)
                           //{
                           //    Console.Write((Convert.ToString(D, 16)).PadLeft(2, '0') + " ");
                           //}
                           //Console.WriteLine();
                           //Console.WriteLine();

                           Crypto.Encrypt(data);
                           Soc.Send(data);
                       }
                       else
                       {
                           Console.WriteLine(data.Length);

                       }
                   }
                   catch { }
                  // Monitor.Exit(this);
                   wil = 0;
               }
               catch { 
                   //Monitor.Exit(this); 
                   wil = 0; }

           }
           else
           {
               Console.WriteLine("soc run is not connected [DC]");
               Disconnect();
               
           }
       }

       byte[] _packet2 = new byte[1024];
       public void Send2(byte[] packet)
       {
           if (!Robot)
               if (Monitor.TryEnter(this, new TimeSpan(0, 0, 0, 30, 0)))
               {
                   try
                   {
                       _packet2 = packet;

                       run2();
                       //System.Threading.Thread.Sleep(Test);

                       Monitor.Exit(this);

                   }
                   catch (Exception e)
                   {
                       Monitor.Exit(this);
                       Program.WriteMessage(e.ToString());
                   }
               }
           else
               {
                   Program.WriteMessage("Can not enter Monitor " + MyChar.Name);
                   Disconnect();
                   
               }
       }

       public void run2()
       {
           while (wil == 1)
               Thread.Sleep(10);
           if (Soc.Connected)
           {
               wil = 1;
               try
               {
                   byte[] data = new byte[_packet2.Length];//new byte[Packet.Length];
                   _packet2.CopyTo(data, 0);
                   string s = "TQServer";
                   ASCIIEncoding encoding = new ASCIIEncoding();
                   encoding.GetBytes(s).CopyTo(data, (int)(data.Length - 8));
                   try
                   {
                       //byte[] getbyte = new byte[256];
                       if (data.Length <= 1024)
                       {
                           Crypto.Encrypt(data);
                           Soc.Send(data);//, 0);
                       }
                       else
                       {
                           Console.WriteLine(data.Length);
                       }
                   }
                   catch { }
                   wil = 0;
               }
               catch { wil = 0; }

           }
           else
           {
               Console.WriteLine("soc run2 is not connected [DC]");
               Disconnect();
           }
       }
        public void LogOff(bool Remove)
        {
            try
            {
                if (!Robot)
                {
                    if (MyChar.MyTeam != null)
                    {
                        if (MyChar.TeamLeader)
                            MyChar.MyTeam.Dismiss(MyChar);
                        else
                            MyChar.MyTeam.Leaves(MyChar);
                    }
                    if (MyChar.Loc.Map == 1039 || MyChar.Loc.Map == 1036)
                    {
                        Game.Vector2 V = (Game.Vector2)Database.DefaultCoords[MyChar.Loc.PreviousMap];
                        MyChar.Teleport(MyChar.Loc.PreviousMap, V.X, V.Y);
                    }
                    if (MyChar.MyShop != null)
                        MyChar.MyShop.Close();
                    foreach (Game.Friend F in MyChar.Friends.Values)
                    {
                        SendPacket(Packets.FriendEnemyPacket(F.UID, F.Name, 15, Convert.ToByte(F.Online)));
                        if (F.Online)
                        {
                            F.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(MyChar.EntityID, MyChar.Name, 14, 1));
                            F.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(MyChar.EntityID, MyChar.Name, 15, 0));
                            F.Info.MyClient.LocalMessage(2005, System.Drawing.Color.Green, "Your Friend " + MyChar.Name + " Has Logged Off.");
                        }
                    }
                    foreach (Game.Enemy E in MyChar.Enemies.Values)
                    {
                        SendPacket(Packets.FriendEnemyPacket(E.UID, E.Name, 19, Convert.ToByte(E.Online)));
                        if (E.Online == true)
                        {
                            E.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(MyChar.EntityID, MyChar.Name, 18, 1));
                            E.Info.MyClient.SendPacket(Packets.FriendEnemyPacket(MyChar.EntityID, MyChar.Name, 19, 0));
                            E.Info.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "Your Enemy " + MyChar.Name + " Has Logged Off.");
                        }
                    }
                    if (!MyChar.Alive)
                    {
                        foreach (ushort[] Point in Database.RevPoints)
                            if (Point[0] == MyChar.Loc.Map)
                            {
                                MyChar.Loc.PreviousMap = MyChar.Loc.Map;
                                MyChar.Loc.Map = Point[1];
                                MyChar.Loc.X = Point[2];
                                MyChar.Loc.Y = Point[3];
                                break;
                            }
                        MyChar.CurHP = 2;
                    }
                }
            }
            catch { }
            try
            {
                if (Remove /*&& Game.World.H_Chars.Contains(MyChar.EntityID)*/)
                {
                    if (this.MyChar.Loc.Map >= 10000 || MyChar.Loc.Map == 1950)
                    { MyChar.Teleport(1002, 429, 378); }
                    PacketHandling.TradePartener.Status(this, false);
                    if (!Robot)
                    {
                        Game.World.ClassPkWar.RemovePlayersFromWar(MyChar);

                        Program.WriteMessage("Character [" + MyChar.Name + "] Disconnected Sucsessfuly");
                        try
                        {
                            if (MyChar != null)
                                Database.SaveCharacter(MyChar, AuthInfo.Account);
                        }
                        catch { Program.WriteMessage("broblem During saving Character :" + MyChar.Name); }
                    }
                    else if (MyChar != null && MyChar.isAvatar)
                    {
                        try
                        {
                                Database.SaveAva(MyChar);
                                Program.WriteMessage("Avatar [" + MyChar.Name + "] Disconnected Sucsessfuly");

                        }
                        catch { Program.WriteMessage("broblem During saving Avatar :" + MyChar.Name); }
                    }
                    if (MyChar.myDengun1 != null)
                    {
                        if (MyChar.myDengun1.Hero == MyChar && MyChar.MyTeam != null)
                        {
                            MyChar.myDengun1.DengunMembers.Remove(MyChar.EntityID);
                            MyChar.myDengun1.TransfeerLeader();
                        }
                        //MyChar.myDengun1 = null;
                    }
                   
                    //Database.teste();
                    //Database.LoadEmpire();
                   // Database.UpgradeServerStatus();
                    if (MyChar.MyCompanion != null)
                        MyChar.MyCompanion.Dissappear();
                    if (MyChar.MyCompanion1 != null)
                        MyChar.MyCompanion1.Dissappear();
                    if (MyChar.MyCompanion2 != null)
                        MyChar.MyCompanion2.Dissappear();
                    if (MyChar.MyCompanion3 != null)
                        MyChar.MyCompanion3.Dissappear();
                    if (MyChar.MyCompanion4 != null)
                        MyChar.MyCompanion4.Dissappear();
                    if (MyChar.MyCompanion5 != null)
                        MyChar.MyCompanion5.Dissappear();
                    if (MyChar.MyCompanion6 != null)
                        MyChar.MyCompanion6.Dissappear();
                    if (MyChar.MyCompanion7 != null)
                        MyChar.MyCompanion7.Dissappear();
                    if (MyChar.MyCompanion8 != null)
                        MyChar.MyCompanion8.Dissappear();
                    if (MyChar.MyCompanion9 != null)
                        MyChar.MyCompanion9.Dissappear();
                    if (MyChar.MyCompanion10 != null)
                        MyChar.MyCompanion10.Dissappear();
                    if (MyChar.MyCompanion11 != null)
                        MyChar.MyCompanion11.Dissappear();
                    if (MyChar.Loc.Map == 1730)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 1731)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 1732)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 1733)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 1734)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 1735)
                        MyChar.Teleport(1002, 429, 378);

                    if (MyChar.Loc.Map == 1737)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 2024)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 2023)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 2022)
                        MyChar.Teleport(1002, 429, 378);
                    if (MyChar.Loc.Map == 2021)
                        MyChar.Teleport(1002, 429, 378);
                    if ((MyChar.Loc.Map == 1707 || MyChar.Loc.Map == 1068) && MyChar.Luchando == true)
                    {
                        foreach (Character C in World.H_Chars.Values)
                        {
                            if (C != null)
                                if (C.Name == MyChar.Enemigo)
                                {
                                    C.CPs += (uint)C.Apuesta;
                                    C.CPs += (uint)C.Apuesta;
                                    World.pvpclear(C);
                                    C.Teleport(1002, 429, 378);
                                    C.MyClient.LocalMessage(2011,System.Drawing.Color.Yellow, "The opprent has Loged off  u WON THE BATTELE congratulations");
                                }
                        }
                    }
                    if (MyChar.Owner != null)
                    {
                        if (MyChar.Owner.AvaRunning)
                        {

                            MyChar.Owner.avaLasts -= (int)(DateTime.Now - MyChar.Owner.AvaSummoned).TotalSeconds;
                            
                        }
                        MyChar.Owner.MyClient.LocalMessage(2000, System.Drawing.Color.Red, "Your Avatar Has Been Disconnected.");

                    }
                    if (MyChar.MyAvatar != null)
                    {
                        if (MyChar.AvaRunning)
                        {

                            MyChar.avaLasts -= (int)(DateTime.Now - MyChar.AvaSummoned).TotalSeconds;
                            MyChar.AvaRunning = false;
                           // Console.WriteLine("Owner.AvaRunning = false GameClint");
                        }
                        MyChar.MyAvatar.MyClient.Disconnect();
                    }
                    if (MyChar.dmblack == 1)
                        World.DeathMatch.teamblack -= 1;
                    if (MyChar.dmred == 1)
                        World.DeathMatch.teamred -= 1;
                   
                    if(Game.World.H_Chars.Contains(MyChar.EntityID))
                    Game.World.H_Chars.Remove(MyChar.EntityID);
                    MyChar.Loaded = false;
                    Game.World.Action(MyChar, Packets.GeneralData(MyChar.EntityID, 0, 0, 0, 135));
                    MyChar.MyClient.Timer.Close();
                    MyChar.MyClient = null;
                    MyChar = null;
                    
                }
            }
            catch { }
        }
        public void Disconnect()
        {
            try
            {
                if (!Robot)
                {
                    if (Soc.Connected)
                    {

                        Soc.Shutdown(SocketShutdown.Both);
                        LogOff(true);
                        Soc.Close();
                    }
                }
                else
                    LogOff(true);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }

        public bool ValidName(string Name)
        {
            if (Name.IndexOfAny(new char[15] { ' ', '~', '[', ']', '#', '*', '\\', '/', '<', '>', ':', '"', '|', '?', '='}) > -1) //this is all windows folder invalids characters
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidWHPass(string Name)
        {
            bool result = true;
            for (int i = 0; i < Name.Length; i++)
            {
                if (char.IsDigit(Name.ElementAt(i)) == false)
                {
                    result = false;
                    break;
                }
            }
            return (result);
        }
    }
}
