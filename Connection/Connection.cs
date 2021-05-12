﻿using Server.Game;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{

    public delegate void ConnectionArrived(StateObj StO);
    public delegate void DataArrived(StateObj StO, byte[] Buffer);
    public delegate void Disconnection(StateObj StO);

    public class StateObj
    {
        public byte[] Data;
        public Socket Sock;
        public object Wrapper;

        public Socket Socket2
        { get { return Sock; } }
    }
    public class Connection
    {
        private Socket Listener;
        private ConnectionArrived ConnHandler = null;
        private DataArrived DataHandler = null;
        private Disconnection DCHandler = null;
        private const int m_nListenBacklog = 32;
        private const int m_nServerPause = 2;
        private int m_nPort = 0;
        //private int BufferSize = 1024;

        public void SetConnHandler(ConnectionArrived Method)
        {
            Monitor.Enter(this);
            if (ConnHandler == null) ConnHandler = Method;
            Monitor.Exit(this);
        }
        public void SetDataHandler(DataArrived Method)
        {
            Monitor.Enter(this);
            if (DataHandler == null) DataHandler = Method;
            Monitor.Exit(this);
        }
        public void SetDCHandler(Disconnection Method)
        {
            Monitor.Enter(this);
            if (DCHandler == null) DCHandler = Method;
            Monitor.Exit(this);
        }
        public void Close()
        {
            Listener.Close();
        }

        public void StartServer(int port)
        {
            m_nPort = port;
            Listen();
        }
        private void Listen()
        {
            try
            {
                XYNetCommon.SetSocketPermission();
                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                IPEndPoint myEnd = (World.ServerInfo.ServerIP == "") ? (new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], m_nPort)) : (new IPEndPoint(IPAddress.Parse(World.ServerInfo.ServerIP), m_nPort));
                Listener.Bind(myEnd);
                Listener.Listen(m_nListenBacklog);
                //this.BufferSize = 1024;
                Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
#if DEBUG
                Console.WriteLine("Server successfully started");
#endif
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        void WaitConnections(IAsyncResult Res)
        {
            try
            {
                StateObj S = null;
                try
                {
                    S = (StateObj)Res.AsyncState;
                    S.Sock = Listener.EndAccept(Res);
                    S.Data = new byte[1024];
                }
                catch
                {
                    Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                    return;
                }
                if (ConnHandler != null)
                    ConnHandler.Invoke(S);

                Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                S.Sock.BeginReceive(S.Data, 0, 1024, SocketFlags.None, new AsyncCallback(WaitData), S);
            }
            catch { }
        }
        unsafe void WaitData(IAsyncResult Res)
        {
            try
            {
                StateObj S = (StateObj)Res.AsyncState;
                SocketError SE;
                try
                {
                    if (S.Sock.Connected)
                    {
                        uint DataLen = (uint)S.Sock.EndReceive(Res, out SE);
                        // Console.WriteLine("data size = " + DataLen);
                        if (SE == SocketError.Success && DataLen != 0)
                        {
                            byte[] RData = new byte[DataLen];
                            fixed (byte* p = RData, p2 = S.Data)
                                Native.memcpy(p, p2, DataLen);
                            //Console.WriteLine("data AR ");
                            //byte[] dataSet = new byte[] { 1 };
                            if (DataHandler != null)
                                DataHandler.Invoke(S, RData);
                            S.Sock.BeginReceive(S.Data, 0, 1024, SocketFlags.None, new AsyncCallback(WaitData), S);
                        }
                        else if (DCHandler != null)
                            DCHandler.Invoke(S);
                    }
                    else if (DCHandler != null)
                        DCHandler.Invoke(S);
                }
                catch
                {
                    if (DCHandler != null)
                        DCHandler.Invoke(S);
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        /*void WaitConnections(IAsyncResult Res)
        {
            Monitor.Enter(this);
            try
            {
                StateObj S = Res.AsyncState as StateObj;
                try
                {
                    S.Sock = Listener.EndAccept(Res);
                    S.Data = new byte[BufferSize];
                }
                catch
                {
                    Monitor.Exit(this); 
                    Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                    return;
                }
                S.Sock.BeginReceive(S.Data, 0, this.BufferSize, SocketFlags.None, new AsyncCallback(WaitData), S);
                if (ConnHandler != null)
                {
                    ConnHandler.Invoke(S);
                }
                Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                //Thread.Sleep(m_nServerPause);
                //Monitor.Exit(this);
            }
            catch { Monitor.Exit(this); }
        }
        unsafe void WaitData(IAsyncResult Res)
        {
            Monitor.Enter(this);
            try
            {
                StateObj S = (StateObj)Res.AsyncState;
                SocketError SE;
                try
                {
                    if (S.Sock.Connected)
                    {
                        int DataLen = S.Sock.EndReceive(Res, out SE);
                        if (DataLen < 0)
                        {
                            Console.WriteLine("Return 1");
                            Monitor.Exit(this);
                            return;
                        }
                        Console.WriteLine(" aa  "+DataLen);
                        if (SE != SocketError.Success)
                        {
                            Console.WriteLine("Return 2");
                            Monitor.Exit(this);
                            return;
                        }
                        if (SE == SocketError.Success && DataLen != 0)
                        {
                            byte[] RData = new byte[DataLen];
                            Buffer.BlockCopy(S.Data, 0, RData, 0, DataLen);
                            S.Data = new byte[BufferSize];
                            byte[] dataSet = new byte[] { 1 };
                            if (DataHandler != null)
                            { DataHandler.Invoke(S, RData, dataSet); }
                            //if (S.Sock.Connected && dataSet[0] == 1)
                                S.Sock.BeginReceive(S.Data, 0, this.BufferSize, SocketFlags.None, WaitData, S);
                            //Monitor.Exit(this);
                        }
                        else if (DCHandler != null)
                        {
                            DCHandler.Invoke(S);
                            //Monitor.Exit(this);
                        }
                    }
                    else if (DCHandler != null)
                    {
                        DCHandler.Invoke(S);
                        //Monitor.Exit(this);
                    }
                }
                catch
                {
                    if (DCHandler != null)
                    {
                        DCHandler.Invoke(S);
                        Monitor.Exit(this);
                    }
                }
            }
            catch (Exception Exc) { Monitor.Exit(this); Program.WriteMessage(Exc); }
        }*/
        public class XYNetCommon
        {
            public static SocketPermission m_permissionSocket = null;
            private static bool m_bPermissionSet = false;

            public static void SetSocketPermission()
            {
                lock (typeof(XYNetCommon))
                {
                    if (m_bPermissionSet == false)
                    {
                        if (m_permissionSocket != null)
                        {
                            m_permissionSocket.Demand();
                        }
                        m_bPermissionSet = true;
                    }
                }
            }
            public static String BinaryToString(Byte[] pData)
            {
                if ((pData.Length % 2) != 0) throw new Exception("Invalid string data size");
                Char[] pChar = new Char[pData.Length / 2];
                for (int i = 0; i < pChar.Length; i++)
                {
                    pChar[i] = (Char)(pData[2 * i] + pData[2 * i + 1] * 256);
                }
                return new String(pChar);
            }

            public static Byte[] StringToBinary(String sData)
            {
                Byte[] pData = new Byte[sData.Length * 2];
                for (int i = 0; i < sData.Length; i++)
                {
                    pData[2 * i] = (Byte)(sData[i] % 256);
                    pData[2 * i + 1] = (Byte)(sData[i] / 256);
                }
                return pData;
            }
        }
    }
}
