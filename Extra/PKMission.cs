using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Server.Game;

namespace Server.Extra
{
    public enum PKTournamentStage
    {
        None,
        Inviting,
        Countdown,
        Fighting,
        Fighting2,
        Over
    }
    public enum BroadCastLoc
    {
        World,
        Map
    }
    public static class PKTournament
    {
        public static ushort Map;
        public static ushort Map2;
        public static ushort X, Y;
        public static ushort X2, Y2;
        public static PKTournamentStage Stage = PKTournamentStage.None;
        public static Dictionary<uint, GameClient> PKTHash;
        public static int CountDown;
        private static Thread PkThread;

        private static void Broadcast(string msg, BroadCastLoc loc)
        {
            Console.WriteLine(msg);
            if (loc == BroadCastLoc.World)
            {
                foreach (Character Char in World.H_Chars.Values)
                {
                    Char.MyClient.SendPacket(Packets.ChatMessage(0, "[Server]", "All", msg, 2011, 0, System.Drawing.Color.DarkRed));
                }
            }
            else if (loc == BroadCastLoc.Map)
            {
                foreach (GameClient Char in PKTHash.Values)
                {
                    Char.SendPacket(Packets.ChatMessage(0, "[Server]", "All", msg, 2011, 0, System.Drawing.Color.DarkRed));
                }
            }
        }
        public static void StartTournament()
        {
            PKTHash = new Dictionary<uint, GameClient>();
            CountDown = 10;
            Stage = PKTournamentStage.Inviting;
            Map = 1018;
            Map2 = 1082;
            X = 100;
            Y = 80;
            X2 = 210;
            Y2 = 210;
            BeginTournament();
            PkThread = new Thread(new ThreadStart(BeginTournament));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void BeginTournament()
        {
            Broadcast(CountDown + " Minutes Until Start PKMission", BroadCastLoc.World);
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 5)
                    Broadcast("5 Minutes Until Start PKMission", BroadCastLoc.World);
                else if (CountDown == 2)
                {
                    Stage = PKTournamentStage.Countdown;
                    if (PKTHash.Count < 1)
                    {
                        Broadcast("The PKMision Requires At Least 2 People To Start, PKMision Cancelled.", BroadCastLoc.World);
                        Stage = PKTournamentStage.None;
                        PKTHash = null;
                        return;
                    }
                    Broadcast("1 Minutes Until Start PKMission", BroadCastLoc.World);
                }
                else if (CountDown < 5)
                    Broadcast(CountDown + " Minutes Until Start PKMission", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(60000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("PKMission Started. Fight Begine!", BroadCastLoc.World);
            WaitForTheNextStage();
        }
        public static void WaitForTheNextStage()
        {
            ushort xx;
            ushort yy;
            ushort xx2;
            ushort yy2;
            foreach (GameClient _GC in PKTHash.Values)
            {
                xx = (ushort)(X + Program.Rnd.Next(31) - Program.Rnd.Next(30));
                yy = (ushort)(Y + Program.Rnd.Next(16) - Program.Rnd.Next(15));
                if (_GC.MyChar.InPKT)
                    if (_GC.MyChar.Alive)
                    {
                        _GC.MyChar.Teleport(Map, xx, yy);
                    }
            }
            Stage = PKTournamentStage.Fighting;
            uint Tick = (uint)Environment.TickCount;
            int InMapAlive = PKTHash.Count;
            int InMap1 = PKTHash.Count;
            while (true)
            {
                try
                {
                    foreach (GameClient _GC in PKTHash.Values)
                    {
                        if (_GC.MyChar.InPKT)
                            if (!_GC.MyChar.Alive)
                            {
                                InMapAlive--;
                                _GC.MyChar.InPKT = false;
                                PKTHash.Remove(_GC.MyChar.EntityID);

                            }
                            else if (!World.H_Chars.ContainsKey(_GC.MyChar.EntityID))
                            {
                                InMapAlive--;
                                _GC.MyChar.InPKT = false;
                                PKTHash.Remove(_GC.MyChar.EntityID);

                            }
                    }
                }
                catch { }
                try
                {
                    foreach (Game.Character chaar in World.H_Chars.Values)
                        if (!chaar.isAvatar)
                        {
                            if ((chaar.Loc.Map == Map || chaar.Loc.Map == Map2) && chaar.CurHP == 0 && DateTime.Now >= chaar.DeathHit.AddSeconds(5))
                            {
                                chaar.Teleport(1002, 363, 330);
                            }
                        }
                }
                catch { }
                try
                {
                    foreach (GameClient _GC in PKTHash.Values)
                    {
                        if (_GC.MyChar.InPKT)
                            if (_GC.MyChar.Alive && InMapAlive <= (uint)(InMap1 / 2))
                                if (_GC.MyChar.Loc.Map == Map)
                                {
                                    xx2 = (ushort)(X2 + Program.Rnd.Next(20) - Program.Rnd.Next(20));
                                    yy2 = (ushort)(Y2 + Program.Rnd.Next(20) - Program.Rnd.Next(20));
                                    _GC.MyChar.Teleport(Map2, xx2, yy2);
                                    AwardFirstWinners(_GC.MyChar);
                                }
                    }
                    if (InMapAlive <= (uint)(InMap1 / 2))
                    {
                        //Stage = PKTournamentStage.Fighting2;
                        WaitForWinner();
                        return;
                    }
                }
                catch { }

                //if (InMapAlive > PKTHash.Count / 2)
                Broadcast("There are " + InMapAlive + " people left for the next stage", BroadCastLoc.Map);
                Thread.Sleep(3000);
            }
        }
        public static void WaitForWinner()
        {
            PKTournament.Stage = PKTournamentStage.Fighting2;
            uint Tick = (uint)Environment.TickCount;
            int InMapAlive = PKTHash.Count;
            //ushort xx2;
            //ushort yy2;
            int players = 0;
            Broadcast("the secound stage has started", BroadCastLoc.Map);
            while (true)
            {
                try
                {
                    foreach (GameClient _GC in PKTHash.Values)
                    {
                        if (_GC.MyChar.InPKT)
                        {
                            if (!_GC.MyChar.Alive)
                            {
                                InMapAlive--;
                                _GC.MyChar.InPKT = false;

                            }
                            else if (!World.H_Chars.ContainsKey(_GC.MyChar.EntityID))
                            {
                                InMapAlive--;
                                _GC.MyChar.InPKT = false;

                            }
                        }
                    }
                }
                catch { }
                try
                {
                    foreach (Game.Character chaar in World.H_Chars.Values)
                    {
                        if (!chaar.isAvatar)
                            if ((chaar.Loc.Map == Map || chaar.Loc.Map == Map2) && chaar.CurHP == 0 && DateTime.Now >= chaar.DeathHit.AddSeconds(5))
                            {
                                chaar.Teleport(1002, 363, 330);
                            }
                    }
                }
                catch { }
                try
                {
                    players = 0;
                    foreach (Game.Character chaar in World.H_Chars.Values)
                    {
                        if (!chaar.isAvatar)
                            if (chaar.Loc.Map == Map2 && chaar.Alive && chaar.InPKT)
                            {
                                players++;
                            }
                    }
                }
                catch { }
                try
                {
                    foreach (GameClient _GC in PKTHash.Values)
                    {
                        if (players == 1)
                        {
                            _GC.MyChar.Teleport(1002, 438, 382);
                            AwardWinner(_GC.MyChar);
                            Stage = PKTournamentStage.Over;
                            return;
                        }
                    }
                    if (InMapAlive != 1)
                    {
                        Broadcast("There are " + players + " people left", BroadCastLoc.Map);
                    }
                }
                catch { }

                Thread.Sleep(2000);
            }
        }
        private static void AwardFirstWinners(Character Winner)
        {
            Broadcast(" players in PKmision has pased the first stage and awarded 200kCPs", BroadCastLoc.World);

            Winner.CPs += 200000;

            PKTournament.Stage = PKTournamentStage.Fighting2;
            return;
        }
        private static void AwardWinner(Character Winner)
        {
            Broadcast(Winner.Name + " Is The Winner In PKMision And He Win [500.000 CPs & Top PKmision.", BroadCastLoc.World);
            Winner.StatEff.Add(StatusEffectEn.Flashy);
            Winner.CPs += 500000;
            try
            {
                StreamWriter sw = new StreamWriter(Program.ConquerPath + @"Tops/toppkmission.txt");
                sw.WriteLine("" + Winner.EntityID + "#1111");
                sw.Close();
            }
            catch (Exception e)
            {
                Program.WriteMessage(e.Message);
            }
            PKTournament.Stage = PKTournamentStage.None;
            PkThread.Abort();
            return;
        }
    }
}