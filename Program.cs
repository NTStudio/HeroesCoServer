using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Net;
//using CountryLookupProj;
using Server.Game;
using Server.Features;

namespace Server
{
    public struct ProgramMessage
    {
        public string message;
        public bool sent;
        public ProgramMessage(string message_, bool sent_)
        {
            message = message_;
            sent = sent_;
        }
        public void Sent()
        {
            sent = true;
        }
    }
    public class Program
    {
        public static string ConquerPath = "Database/";
        public static DateTime ServerStartDate = new DateTime();
        public static DateTime SystemMsgTime = new DateTime();
        public static Random Rnd = new Random();

        public static int ThreaderCount = 0;
        public static MyThread PlayersThead;
        public static MyThread CompanionThread;
        public static MyThread EventsThread;
        public static MyThread MobThread;
        public static MyThread MessageWriter;
        static ArrayList messages;

        static void Main(string[] args)
        {
            messages = new ArrayList();
            ServerStartDate = DateTime.Now;

            Console.Title = "Server 5165. Start time: " + ServerStartDate.ToString("[ (hh:mm).(dd/MM/yyyy) ]");
            if (Directory.Exists(Program.ConquerPath))
            {
                Database.CreateConnection(database: "heroesconline", user: "root", password: "root");
                try
                {
                    Database.MySqlConnection.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }

                #region Server IP
                //IPHostEntry LocalHE;
                //LocalHE = Dns.GetHostEntry(Dns.GetHostName());
                //foreach (IPAddress ipCurrent in LocalHE.AddressList)
                //{
                //    string LocalServerIp = ipCurrent.ToString();
                //    if (LocalServerIp != "::1")
                //    {
                //        Game.World.ServerInfo.ServerIP = LocalServerIp;
                //        Connection.SetAdresIp(LocalServerIp);
                //    }
                //}
                #endregion

                #region Load Server
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--------------------- [Configuration Loaded Successfuly] ----------------------");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("_______________________________________________________________________________");// Loading Characters
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading Characters...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Database.LoadCharacterStats();
                Database.LoadLevelExp();
                Database.LoadEmpire();
                Database.LoadPromotedSkills();
                Skills.SkillsClass.Load();
                Database.LoadKOs();
                Database.ResetExpBall();

                Console.WriteLine("______________________________________________");// Loading Guilds
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading Guilds...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Features.Guilds.LoadGuilds();
                #region GuildWar Check
                Extra.GuildWars.Init();
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour >= 7
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour <= 20 && DateTime.Now.Minute < 59)
                    Extra.GuildWars.StartWar();
                #endregion

                Console.WriteLine("______________________________________________");// Loading NPCs
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading NPCs...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Database.LoadNPCs();

                Console.WriteLine("______________________________________________");// Loading Monesters
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading Monesters...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Database.LoadMonsters();
                Database.LoadCompanions();

                Console.WriteLine("______________________________________________");// Loading Items
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading Items...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Database.LoadItems();
                Database.LoadItemsPlus();
                Database.LoadShops();
                Database.LoadLottoItems();
                Database.LoadProfExp();
                Database.Fixinventory(0);

                Console.WriteLine("______________________________________________");// Loading Maps
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - Loading Maps...!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                DMaps.LoadMaps();
                Database.LoadPortals();
                Database.LoadRevivePoints();
                Database.LoadDefaultCoords();
                DropRates.Load();
                Console.WriteLine("_______________________________________________________________________________");
                //Finishing Loading
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Server Using IP [" + World.ServerInfo.ServerIP + "] Server Using Port [" + World.ServerInfo.AuthPort + "]");
                Console.WriteLine("-------------------- [Heroes-C-Server Loaded Successfuly] --------------------");

                Console.ForegroundColor = ConsoleColor.White;
                MessageWriter = new MyThread();
                MessageWriter.Execute += new Execute(MessageWriter_Execute);
                MessageWriter.Start(1);
                EventsThread = new MyThread();
                EventsThread.Execute += new Execute(EventsThread_Execute);
                EventsThread.Start(4000);
                CompanionThread = new MyThread();
                CompanionThread.Execute += new Execute(CompanionThread_Execute);
                CompanionThread.Start(300);
                MobThread = new MyThread();
                MobThread.Execute += new Execute(MobThread_Execute);
                MobThread.Start(300);
                PlayersThead = new MyThread();
                PlayersThead.Execute += new Execute(PlayersThead_Execute);
                PlayersThead.Start(300);

                Server.AuthWorker.StartServer(Game.World.ServerInfo.ServerIP);
                Server.GameWorker.StartServer();

                //MainFormStart();
                Application.EnableVisualStyles();
                #endregion

                #region ConsoleCheats
                while (true)
                {
                    string[] data = Console.ReadLine().Split(' ');
                    switch (data[0])
                    {
                        case "/s":
                            {
                                try
                                {
                                    Server.Skills.SkillAdder S = new Server.Skills.SkillAdder();
                                    S.ShowDialog();
                                }
                                catch { };
                                break;
                            }
                        case "/ss":
                            {
                                Console.WriteLine("teeeest");
                                break;
                            }
                        case "/f":
                            {
                                try
                                {
                                    onlinePlayers f = new onlinePlayers();
                                    f.ShowDialog();
                                }
                                catch { };
                                break;
                            }
                        case "/m":
                            {
                                try
                                {
                                    MainFormStart();
                                    //Thread.CurrentThread.Abort();
                                }
                                catch { };
                                break;
                            }
                        case "/tele":
                            {
                                try
                                {
                                    Game.Character C = World.CharacterFromName(data[1]);
                                    C.Teleport(ushort.Parse(data[2]), ushort.Parse(data[3]), ushort.Parse(data[4]));
                                }
                                catch { };
                                break;
                            }
                        case "/do":
                            {
                                try
                                {
                                    string[] FileCaseSensitive = Directory.GetFiles(Program.ConquerPath + @"map\map\");
                                    for (int i = 0; i < FileCaseSensitive.Length; i++)
                                    {
                                        if (Path.GetExtension(FileCaseSensitive[i]) == ".7z")
                                            continue;
                                        Path.ChangeExtension(FileCaseSensitive[i], ".DM");
                                        Console.WriteLine("count done= " + i);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Program.WriteMessage(e);
                                }
                                break;
                            }
                        default:
                            Console.WriteLine("Command Does`t Exist", data[0]);
                            break;
                    }
                    Thread.Sleep(250);
                }
                #endregion
            }
            else
            {
                Program.WriteMessage("Server Database Not Found.");
                Console.ReadLine();
            }
        }

        public static void MainFormStart()
        {
            new Thread(new ThreadStart(delegate () { new MainForm().ShowDialog(); })).Start();
        }
        public static void WriteMessage(object message)
        {
            ProgramMessage s = new ProgramMessage(message.ToString(), false);

            if (messages.Count < 1000)
                messages.Add(s);
        }

        static void MessageWriter_Execute()
        {
            if (messages.Count > 0)
            {
                ArrayList removeList = new ArrayList();
                foreach (ProgramMessage S in messages)
                {
                    if (S.sent == false)
                    {
                        Console.WriteLine(S.message);
                        removeList.Add(S);
                        S.Sent();
                    }
                }
                foreach (ProgramMessage S in removeList)
                {
                    messages.Remove(S);
                }
                removeList = null;
            }
        }
        static void EventsThread_Execute()
        {
            #region Broadcaster Time
            try
            {
                if (World.BroadCastCount > 0 && DateTime.Now > World.LastBroadCast.AddSeconds(10))
                {
                    BroadCastMessage B = World.BroadCasts[0];

                    for (int i = 0; i < World.BroadCastCount; i++)
                        World.BroadCasts[i] = World.BroadCasts[i + 1];

                    World.BroadCastCount--;

                    World.WorldMessage(B.Name, B.Message, 2500, 0, System.Drawing.Color.Red);
                    World.LastBroadCast = DateTime.Now;
                    World.CurrentBC = B;
                }
            }
            catch { }
            #endregion
            #region DroppedItem Time
            try
            {
                foreach (Hashtable H in World.H_Items.Values)
                {
                    ArrayList Deleted = new ArrayList();
                    try
                    {
                        foreach (DroppedItem I in H.Values)
                            if (DateTime.Now > I.DropTime.AddSeconds(60))
                                Deleted.Add(I);
                        foreach (DroppedItem I in Deleted)
                            I.Dissappear();
                    }
                    catch { }
                }
            }
            catch { }
            #endregion
            #region ClearExpBalls Time
            try
            {
                if (DateTime.Now.Hour == 0)
                {
                    Database.ResetExpBall();
                }
            }
            catch { }
            #endregion
            #region Dragons
            try
            {
                if (DateTime.Now.Minute == 30 && DateTime.Now.Second < 4)
                    if (DateTime.Now.Hour == 0 || DateTime.Now.Hour == 2 || DateTime.Now.Hour == 4 || DateTime.Now.Hour == 6 || DateTime.Now.Hour == 8 || DateTime.Now.Hour == 10 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 14 || DateTime.Now.Hour == 16 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 20 || DateTime.Now.Hour == 22)
                        if (World.dragonsALIVE <= 0)
                        {
                            Database.LoadDragons();
                            World.dragonsALIVE = 2;
                            World.WorldMessage("Server", "Most Wanted Monsters Is Apeared In The Game Called [TeratoDragon,MillenniumDragon,Zombe and AxeOrc Search For It Is Location And Kill Them To Get Some Rewards!", 2011, 0, System.Drawing.Color.Red);
                        }
            }
            catch { }
            #endregion

            #region Weather
            /*try
            {
                TimeSpan Span = Features.Weather.NextChange - DateTime.Now;
                int Change = Rnd.Next(10);

                while (Change == 6 || Change == 7 || Change == 8 || Change == 4)
                {
                    Change = Rnd.Next(10);
                }

                if (Span.Minutes == 0)
                {
                    Features.Weather.Intensity = (uint)Rnd.Next(50, 220);
                    Features.Weather.Direction = (uint)Rnd.Next(100, 200);
                    if (Change == 5)
                        Features.Weather.Appearence = (uint)Rnd.Next(7);
                    else
                        Features.Weather.Appearence = 0;
                    Features.Weather.NextChange = DateTime.Now.AddMinutes(Rnd.Next(2, 4));
                    Features.Weather.CurrentWeather = (Features.WeatherType)Change;
                }
            }
            catch { }*/
            #endregion
            #region ScreenColor
            /*try
            {
                if (DateTime.Now.Minute == 0)
                {
                    Game.World.ScreenColor = 0;
                    foreach (Game.Character C in Game.World.H_Chars.Values)
                        try
                        {
                            C.MyClient.SendPacket(Packets.GeneralData(C.EntityID, (ushort)Game.World.ScreenColor, 0, 0, 104));
                            //Game.World.SendMsgToAll("[SERVER]", "Hayy Good Day Guy's.What's UP?",2011,0 ,System.Drawing.Color.Yellow);
                            
                        }
                        catch { }

                }
                //if (DateTime.Now.Hour >= 17)
                //{
                //    Game.World.ScreenColor = 5855577;
                //    foreach (Game.Character C in Game.World.H_Chars.Values)
                //        try
                //        {
                //            C.MyClient.SendPacket(Packets.GeneralData(C.EntityID, Game.World.ScreenColor, 0, 0, 104));
                //            //Game.World.SendMsgToAll("[SERVER]", "Good Night Server Players.", 2011, 0, System.Drawing.Color.Yellow);
                //        }
                //        catch { }
                //}
            }
            catch { }*/
            #endregion

            #region VS.Arena TimeUp
            try
            {
                foreach (Character chaar in World.H_Chars.Values)
                {
                    if (chaar.Loc.Map == 1707 || chaar.Loc.Map == 1068)
                    {
                        if (chaar != null)
                            if (chaar.Luchando)
                            {
                                if (DateTime.Now > chaar.PVPCuonter.AddMinutes(chaar.VsLimetTimeLastes))
                                {
                                    foreach (Character C in World.H_Chars.Values)
                                    {
                                        if (C != null)
                                            if (C.Name == chaar.Enemigo)
                                            {
                                                if (chaar.PkPuntos > C.PkPuntos)
                                                {
                                                    chaar.CPs += (uint)C.Apuesta;
                                                    chaar.CPs += (uint)C.Apuesta;
                                                    chaar.MyClient.LocalMessage(2011, System.Drawing.Color.White, " |TimeUP|  You Win The Battele " + (chaar.Apuesta) * 2 + " CPs Gretz");
                                                    C.MyClient.LocalMessage(2011, System.Drawing.Color.White, " |TimeUP|  You Lose The Battele HardLuck ");
                                                    World.pvpclear(C);
                                                    World.pvpclear(chaar);
                                                    C.Teleport(1002, 430, 371);
                                                    chaar.Teleport(1002, 430, 383);
                                                }
                                            }
                                    }
                                }
                            }
                    }
                }
            }
            catch { }
            #endregion

            #region SystemMessages Start
            try
            {
                if (DateTime.Now > SystemMsgTime.AddMinutes(5))
                {
                    Events.SystemMessages();
                    SystemMsgTime = DateTime.Now;
                }
            }
            catch { }
            #endregion
            #region DeathMatch Score
            try
            {
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1038)
                            {
                                if (Extra.GuildWars.War && DateTime.Now > Extra.GuildWars.LastScores.AddSeconds(3))
                                    Extra.GuildWars.SendScores();
                            }
                            if (Game.World.DeathMatch.DeathMatchScore == true)
                            {
                                Extra.DMTornament.SendScores();
                            }
                            else if (Chaar.Loc.Map != 1950 && Chaar.Loc.Map != 1038 && Chaar.Loc.Map == 1090)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "", "", "", 0x83c, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            catch { }

            #endregion
            #region DeathMatch Start
            try
            {//Start Every Hour
                if (World.H_Chars.Count > 3 && DateTime.Now.Minute == 00 && Game.World.DeathMatch.DeathMatchStart == false)
                {
                    Game.World.DeathMatch.DeathMatchStart = true;
                    Events.StartDeathMatchWar();
                }
            }
            catch { }
            #endregion
            #region DisCity Start
            try
            {//Start In 20:00/8:00 PM
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour == 19 && DateTime.Now.Minute == 55)
                    || (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour == 19 && DateTime.Now.Minute == 55)
                    && Game.World.DisCity.DisCityStart == false)
                {
                    Events.StartDisCityWar();
                }
            }
            catch { }
            #endregion
            #region ClassPK War Start
            try
            {//Start In 18:00/6:00 PM
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && DateTime.Now.Hour == 18 && DateTime.Now.Minute == 00) && (Game.World.ClassPkWar.AddMap.InTimeRange == false))
                {
                    Database.ResetTopTrojan();
                    Database.ResetTopWar();
                    Database.ResetTopArcher();
                    Database.ResetTopNinja();
                    //Database.ResetTopMonk();
                    Database.ResetTopWater();
                    Database.ResetTopFire();
                    Game.World.ClassPkWar.ClassPKTime = true;

                    Events.StartClassPKWar();
                    Game.World.ClassPkWar.AddMap.InTimeRange = true;
                }
            }
            catch { }
            #endregion
            #region WeeklyPK War Start
            try
            {//Start In 20:00/8:00 PM
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00) && (Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange == false))
                {
                    Database.ResetTopWeeklyPK();
                    Game.World.WeeklyAndMonthlyPkWar.WeeklyWar = true; Events.StartWeeklyPKWar();
                    Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange = true;
                }
            }
            catch { }
            #endregion
            #region MonthlyPK War Start
            try
            {//Start In 22:00/10:00 PM
                if ((DateTime.Now.Month == 01 && DateTime.Now.Hour == 22 && DateTime.Now.Minute == 00) && (Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange == false))
                {
                    Database.ResetTopMonthlyPK();
                    Game.World.WeeklyAndMonthlyPkWar.MonthlyWar = true;
                    Events.StartMonthlyPKWar();
                    Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange = true;
                }
            }
            catch { }
            #endregion
        }
        static void CompanionThread_Execute()
        {
            try
            {
                if (World.H_Chars.Count > 0)
                {
                    foreach (Companion C in World.H_Companions.Values)
                        try
                        {
                            C.Step();
                        }
                        catch { }
                }
            }
            catch { }
        }
        static void MobThread_Execute()
        {
            try
            {
                if (World.H_Chars.Count > 0)
                {
                    try
                    {
                        foreach (Hashtable H in Game.World.H_Mobs.Values)
                        {
                            foreach (Mob M in H.Values)
                            {
                                M.Step();

                                if (M.PoisonedInfo != null)
                                {
                                    if (DateTime.Now > M.PoisonedInfo.LastAttack.AddSeconds(3))
                                    {
                                        if (M.CurrentHP == 0)
                                        {
                                            M.PoisonedInfo = null;
                                            foreach (Game.Character C in Game.World.H_Chars.Values)
                                                if (M.Loc.Map == C.Loc.Map)
                                                    if (MyMath.PointDistance(M.Loc.X, M.Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                                                        C.MyClient.SendPacket(Packets.Status(M.EntityID, Server.Game.Status.Effect, 0));
                                            continue;
                                        }
                                        M.PoisonedInfo.Times--;
                                        M.PoisonedInfo.LastAttack = DateTime.Now;
                                        uint Dmg = (uint)(M.CurrentHP * (10 + M.PoisonedInfo.SpellLevel * 10) / 100);
                                        if (Dmg == 1)
                                            continue;
                                        M.GetPoisoned(M, Dmg);
                                        if (M.PoisonedInfo.Times == 0)
                                        {
                                            M.PoisonedInfo = null;
                                            foreach (Game.Character C in Game.World.H_Chars.Values)
                                                if (M.Loc.Map == C.Loc.Map)
                                                    if (MyMath.PointDistance(M.Loc.X, M.Loc.Y, C.Loc.X, C.Loc.Y) <= 20)
                                                        C.MyClient.SendPacket(Packets.Status(M.EntityID, Server.Game.Status.Effect, 0));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception Exc) { Program.WriteMessage(Exc); }
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
        }
        static void PlayersThead_Execute()
        {
            lock (World.H_Chars)
            {
                if (World.H_Chars.Count > 0)
                {
                    try
                    {
                        Game.Character[] BaseCharacters = new Character[World.H_Chars.Count];
                        World.H_Chars.Values.CopyTo(BaseCharacters, 0);
                        foreach (Game.Character Char in BaseCharacters)
                        {
                            Char.Step();
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
