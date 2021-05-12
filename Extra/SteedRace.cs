using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.Game;

namespace Server.Extra
{
    class SteedRace
    {
        public static void CpsFowin(GameClient GC)
        {
            // Character GC = new Character();
            if (World.SteedTornament.cps == 1)
            {
                GC.MyChar.CPs += 20000;
                GC.MyChar.cp = 20000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "", "", "", 0x83c, 0, System.Drawing.Color.White));
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", " Rank         Name         Score", 2109, 0, System.Drawing.Color.White));
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.1          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 2)
            {
                GC.MyChar.CPs += 17000;
                GC.MyChar.cp = 17000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.2          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 3)
            {
                GC.MyChar.CPs += 15000;
                GC.MyChar.cp = 15000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                           Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.3          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 4)
            {
                GC.MyChar.CPs += 14000;
                GC.MyChar.cp = 14000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.4          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 5)
            {
                GC.MyChar.CPs += 12000;
                GC.MyChar.cp = 12000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.5          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 6)
            {
                GC.MyChar.CPs += 10000;
                GC.MyChar.cp = 10000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.6          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 7)
            {
                GC.MyChar.CPs += 8000;
                GC.MyChar.cp = 8000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.7          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }

            }
            else if (World.SteedTornament.cps == 8)
            {
                GC.MyChar.CPs += 5000;
                GC.MyChar.cp = 5000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.8          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 9)
            {
                GC.MyChar.CPs += 4000;
                GC.MyChar.cp = 4000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {
                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.9          " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps == 10)
            {
                GC.MyChar.CPs += 3000;
                GC.MyChar.cp = 3000;
                foreach (DictionaryEntry DE in World.H_Chars)
                {
                    Character Chaar = (Game.Character)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Loc.Map == 1950)
                            {

                                Chaar.MyClient.SendPacket(Packets.ChatMessage(Chaar.MyClient.MessageID, "SYSTEM", "ALL", "No.10         " + GC.MyChar.Name + "      " + (DateTime.Now - GC.MyChar.SteedRaceTime) + "", 2109, 0, System.Drawing.Color.White));
                            }
                        }
                    }
                }
            }
            else if (World.SteedTornament.cps > 10)
            {
                GC.MyChar.CPs += 1000;
                GC.MyChar.cp = 1000;
            }
        }
        public static void WaiForWin(bool mod, GameClient GC)
        {
            while (mod)
            {
                //  Game.Character GC = new Game.Character();
                try
                {
                    foreach (DictionaryEntry DE in World.H_Chars)
                    {
                        Character Chaar = (Game.Character)DE.Value;
                        if (Chaar.Loc.Map == 1950)
                        {
                            if (Chaar.Loc.Map == 1950 && (Chaar.Loc.X >= 494 && Chaar.Loc.X <= 498) && Chaar.Loc.Y == 373)
                            {
                                World.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                Game.World.WorldMessage("Server", "" + Chaar.Name + " has finished the steed race no " + World.SteedTornament.cps + " and won " + GC.MyChar.cp + " Cps", 2011, 0, System.Drawing.Color.Red);

                            }
                            else if (Chaar.Loc.Map == 1950 && (Chaar.Loc.X >= 494 && Chaar.Loc.X <= 498) && Chaar.Loc.Y == 372)
                            {
                                World.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                Game.World.WorldMessage("Server", "" + Chaar.Name + " has finished the steed race no " + World.SteedTornament.cps + " and won " + Chaar.cp + " Cps", 2011, 0, System.Drawing.Color.Red);

                            }
                            else if (Chaar.Loc.Map == 1950 && (Chaar.Loc.X >= 494 && Chaar.Loc.X <= 498) && Chaar.Loc.Y == 371)
                            {
                                World.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                Game.World.WorldMessage("Server", "" + Chaar.Name + " has finished the steed race no " + World.SteedTornament.cps + " and won " + Chaar.cp + " Cps", 2011, 0, System.Drawing.Color.Red);

                            }
                            else if (Chaar.Loc.Map == 1950 && (Chaar.Loc.X >= 478 && Chaar.Loc.X <= 510) && (Chaar.Loc.Y >= 346 && Chaar.Loc.Y <= 370))
                            {
                                World.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                Game.World.WorldMessage("Server", "" + Chaar.Name + " has finished the steed race no " + World.SteedTornament.cps + " and won " + Chaar.cp + " Cps", 2011, 0, System.Drawing.Color.Red);

                            }

                        }
                    }
                }
                catch { }
            }
        }
    }
}

