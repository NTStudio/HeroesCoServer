using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using Server;

namespace Server
{
    class Events
    {
        #region SystemMessages
        public static void SystemMessages()
        {
            switch (DateTime.Now.Minute)
            {
                case 5:
                    Game.World.WorldMessage("GM", "TOP Characters Starting In Thursday Hour 18:45 / End In Hour 19:00.", 2011, 0, System.Drawing.Color.Wheat);
                    break;
                case 10:
                    Game.World.WorldMessage("GM", "TOP PKChampion Starting In Friday Hour 18:45 / End In Hour 19:00.", 2011, 0, System.Drawing.Color.Wheat);
                    break;
                case 15:
                    Game.World.WorldMessage("GM", "UnknownMan In Market Help You To Get 135 Easily And Quikly.", 2011, 0, System.Drawing.Color.Wheat);
                    break;
                case 20:
                    Game.World.WorldMessage("GM", "All Informations about the game like ( how to get cps , featurs time )at the meddle of TwinCity", 2011, 0, System.Drawing.Color.Wheat);
                    break;
                case 25:
                    Game.World.WorldMessage("GM", "Visit Us In WWW.HeroesDragon.Webs.COM This Is Only Official Site For Server.", 2011, 0, System.Drawing.Color.Wheat);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region ClassPK War
        public static void StartClassPKWar()
        {
            new Thread(new ThreadStart(delegate()
            {
                Game.World.WorldMessage("Server", "ClassPK War has Begun Go ClassPKEnvoy In TwinCity at [437 247] To Join.", 2011, 0, System.Drawing.Color.LightGreen);
                System.Threading.Thread.Sleep(900000);
                Game.World.ClassPkWar.AddMap.InTimeRange = false;
                Game.World.ClassPkWar.AddMap.timer = true;
                Game.World.WorldMessage("Server", "ClassPK War Started.", 2011, 0, System.Drawing.Color.Yellow);
                System.Threading.Thread.Sleep(2700000);
                Game.World.WorldMessage("Server", "ClassPK War Finished.", 2011, 0, System.Drawing.Color.Red);
                Game.World.ClassPkWar.ClassPKTime = false;

                Game.World.ClassPkWar.AddMap.timer = false;
            }
                )).Start();
        }
        #endregion
        #region WeeklyPK War
        public static void StartWeeklyPKWar()
        {
            new Thread(new ThreadStart(delegate()
            {
                Game.World.WorldMessage("Server", "WeeklyPK War has Begun Go To GeneralBravery In TwinCity at [437 247] To Join.", 2011, 0, System.Drawing.Color.LightGreen);
                System.Threading.Thread.Sleep(900000);
                Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange = false;
                Game.World.WeeklyAndMonthlyPkWar.AddMap.timer = true;
                Game.World.WorldMessage("Server", "WeeklyPK War Started.", 2011, 0, System.Drawing.Color.Yellow);
                System.Threading.Thread.Sleep(2700000);
                Game.World.WorldMessage("Server", "WeeklyPK War Finished.", 2011, 0, System.Drawing.Color.Red);
                Game.World.WeeklyAndMonthlyPkWar.WeeklyWar = false;

                Game.World.WeeklyAndMonthlyPkWar.AddMap.timer = false;
            }
                )).Start();
        }
        #endregion
        #region MonthlyPK War
        public static void StartMonthlyPKWar()
        {
            new Thread(new ThreadStart(delegate()
            {
                Game.World.WorldMessage("Server", "MonthlyPK War has Begun Go To GeneralBravery In TwinCity at [437 247] To Join.", 2011, 0, System.Drawing.Color.LightGreen);
                System.Threading.Thread.Sleep(900000);
                Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange = false;
                Game.World.WeeklyAndMonthlyPkWar.AddMap.timer = true;
                Game.World.WorldMessage("Server", "MonthlyPK War Started.", 2011, 0, System.Drawing.Color.Yellow);
                System.Threading.Thread.Sleep(2700000);
                Game.World.WorldMessage("Server", "MonthlyPK War Finished.", 2011, 0, System.Drawing.Color.Red);
                Game.World.WeeklyAndMonthlyPkWar.MonthlyWar = false;

                Game.World.WeeklyAndMonthlyPkWar.AddMap.timer = false;
            }
                )).Start();
        }
        #endregion

        #region DeathMatch War
        public static void StartDeathMatchWar()
        {
            new Thread(new ThreadStart(delegate()
            {
                Game.World.WorldMessage("Server", "DeathMatch Will Start In 5 Minutes Hurry Go TwinCity To Join.", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Start In 4 Minutes Hurry Go TwinCity To Join.", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Start In 3 Minutes Hurry Go TwinCity To Join.", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Start In 2 Minutes Hurry Go TwinCity To Join.", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Start In 1 Minutes Hurry Go TwinCity To Join.", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Started Good Luck Every Body.", 2011, 0, System.Drawing.Color.Red);
                
                foreach (DictionaryEntry DE in Game.World.H_Chars)
                {
                    Game.Character Chaar = (Game.Character)DE.Value;
                    if (Chaar.MyTDmTeam.TeamName != "")
                    {
                        if (Chaar.MyTDmTeam.TeamID == 10)//black
                        {
                            Game.Item I = Chaar.Equips.Get(9);
                            if (I.ID >= 181305 && I.ID <= 191905)
                            {
                            }
                            else
                                Chaar.EquipGermant(181525);
                            Chaar.Teleport(1090, 121, 70);
                        }
                        if (Chaar.MyTDmTeam.TeamID == 20)//red
                        {
                            Game.Item I = Chaar.Equips.Get(9);
                            if (I.ID >= 181305 && I.ID <= 191905)
                            {
                            }
                            else
                                Chaar.EquipGermant(181625);
                            Chaar.Teleport(1090, 43, 67);
                        }
                        if (!Game.World.AllTeams.ContainsKey(Chaar.MyTDmTeam.TeamID))
                            Game.World.AllTeams.Add(Chaar.MyTDmTeam.TeamID, Chaar.MyTDmTeam);
                    }
                }
                Game.World.DeathMatch.DeathMatchStart = false;
                Game.World.DeathMatch.DeathMatchScore = true;
                Thread.Sleep(1200000);
                Game.World.WorldMessage("Server", "DeathMatch Will Finish In 5 Minutes, Hurry Show Others What You Got.", 2011, 0, System.Drawing.Color.White);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Finish In 4 Minutes, Hurry Show Others What You Got.", 2011, 0, System.Drawing.Color.White);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Finish In 3 Minutes, Hurry Show Others What You Got.", 2011, 0, System.Drawing.Color.White);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Finish In 2 Minutes, Hurry Show Others What You Got.", 2011, 0, System.Drawing.Color.White);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Will Finish In 1 Minutes, Hurry Show Others What You Got.", 2011, 0, System.Drawing.Color.White);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DeathMatch Finished.", 2011, 0, System.Drawing.Color.White);
                
                foreach (DictionaryEntry DE in Game.World.H_Chars)
                {
                    Game.Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar.Loc.Map == 1090)
                        {
                            if (Chaar.MyTDmTeam.TeamName == Extra.DMTornament.WinerName1)
                            {
                                Chaar.CPs += 200000;
                                Chaar.Teleport(1002, 363, 345);
                                Chaar.MyClient.LocalMessage(2011, System.Drawing.Color.White, "Congratulation! Your Team Is The Winner In DeathMatch, And All Members Recive [200.000 CPs].");
                            }
                            else if (Chaar.MyTDmTeam.TeamName == Extra.DMTornament.WinerName2)
                            {
                                Chaar.CPs += 10000;
                                Chaar.Teleport(1002, 362, 322);
                                Chaar.MyClient.LocalMessage(2011, System.Drawing.Color.White, "HardLuck! Your Team Is The Losser In DeathMatch, But All Members Recive [100.000 CPs].");
                            }
                            Chaar.EquipStats(9, false);
                            uint GarmentID = Chaar.Equips.Garment.UID;
                            Chaar.Equips.Dissappear(9, Chaar);
                            Chaar.dmjoin = 0;
                            Chaar.dmred = 0;
                            Chaar.dmblack = 0;
                            Chaar.MyTDmTeam.TeamID = 0;
                            Chaar.MyTDmTeam.TeamName = "";
                            Game.World.DeathMatch.teamred = 0;
                            Game.World.DeathMatch.teamblack = 0;
                            Game.World.DeathMatch.DeathMatchScore = false;
                        }
                    }
                }
                foreach (DictionaryEntry DE in Game.World.H_Chars)
                {
                    Game.Character Chaar = (Game.Character)DE.Value;
                    if (Chaar.Loc.Map == 1090)
                    {
                        Chaar.dmjoin = 0;
                        Chaar.dmred = 0;
                        Chaar.dmblack = 0;
                        Chaar.MyTDmTeam.TeamID = 0;
                        Chaar.MyTDmTeam.TeamName = "";
                        Game.World.DeathMatch.teamred = 0;
                        Game.World.DeathMatch.teamblack = 0;
                        Chaar.Equips.Dissappear(9, Chaar);
                        Game.World.DeathMatch.DeathMatchScore = false;
                        Chaar.Teleport(1002, 439, 366);
                    }
                } return;
            }
            )).Start();
        }
        #endregion

        #region DisCity War
        public static void StartDisCityWar()
        {
            new Thread(new ThreadStart(delegate()
            {
                Game.World.WorldMessage("Server", "DisCity Will Start In 5 Minutes! Hurry up And Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DisCity Will Start In 4 Minutes! Hurry up And Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DisCity Will Start In 3 Minutes! Hurry up And Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DisCity Will Start In 2 Minutes! Hurry up And Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DisCity Will Start In 1 Minutes! Hurry up And Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);
                Thread.Sleep(60000);
                Game.World.WorldMessage("Server", "DisCity Started Find SolarSaint To Claim ExpBall And Join!", 2011, 0, System.Drawing.Color.Red);

                Game.World.DisCity.DisCityStart = true;
                Game.World.DisCity.PlayerInStage2 = 0;
                Game.World.DisCity.PlayerInStage3 = 0;

                Thread.Sleep(300000);
                Game.World.DisCity.DisCityStart = false;
            }
                )).Start();
        }
        #endregion
    }
}
