using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Features;
using Server.Extra;
using Server.Game;

namespace Server.Extra
{
    public class GuildWars
    {
        public class GWScore
        {
            public Guild TheGuild;
            public uint Score;
        }
        public struct Pole
        {
            public Location Loc;
            public uint MaxHP;
            public uint CurHP;
            public uint Mesh;
            public uint EntityID;

            public void Spawn(Character C, bool Check)
            {
                if (C.Loc.Map == Loc.Map /*&& MyMath.InBox(C.Loc.X, C.Loc.Y, Loc.X, Loc.Y, 16)*/ && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, Loc.X, Loc.Y, 16) || !Check))
                {
                    if (LastWinner == null)
                        C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 10, Loc, true, "HeroesPole", CurHP, MaxHP));
                    else
                        C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 10, Loc, true, LastWinner.GuildName, CurHP, MaxHP));
                }
            }
            public void ReSpawn()
            {
                foreach (Character C in World.H_Chars.Values)
                    if (C.Loc.Map == Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, Loc.X, Loc.Y, 16))
                    {
                        if (LastWinner == null)
                            C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 10, Loc, true, "Pole", CurHP, MaxHP));
                        else
                            C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 10, Loc, true, LastWinner.GuildName, CurHP, MaxHP));
                    }
            }
            public void TakeAttack(Character C, uint Damage, byte AtkType)
            {
                if (War && C.MyGuild != null && C.MyGuild != LastWinner)
                {
                    if (AtkType != 21)
                        World.Action(C, Packets.AttackPacket(C.EntityID, EntityID, Loc.X, Loc.Y, Damage, AtkType));
                    if (Damage >= CurHP)
                    {

                        AddScore(C.MyGuild, CurHP);
                        C.AtkMem.Attacking = false;
                        C.AtkMem.Target = 0;
                        World.Action(C, Packets.AttackPacket(C.EntityID, EntityID, Loc.X, Loc.Y, 0, 14));
                        CurHP = MaxHP;
                        EndPole();
                    }
                    else
                    {
                        CurHP -= Damage;
                        AddScore(C.MyGuild, Damage);
                        if (GuildWarScores.Contains(C.MyGuild.GuildID))
                        {
                            GuildWarScores[C.MyGuild.GuildID] = (uint)GuildWarScores[C.MyGuild.GuildID] + Damage;
                        }
                        else
                            GuildWarScores.Add(C.MyGuild.GuildID, Damage);
                    }
                    Game.World.Spawns(C, true);
                   }
            }
        }
        public struct Gate
        {
            public Location Loc;
            public uint MaxHP;
            public uint CurHP;
            public uint EntityID;
            public uint Mesh;

            public bool Opened
            {
                set
                {
                    if (EntityID == 6701)//Left Gate
                    {
                        if (value) Mesh = 250;
                        else Mesh = 240;
                    }
                    else if (EntityID == 6702)//Right Gate
                    {
                        if (value) Mesh = 280;
                        else Mesh = 270;
                    }
                }
                get
                {
                    if (EntityID == 6701)//Left Gate
                    {
                        if (Mesh == 250) return true;
                        else return false;
                    }
                    else if (EntityID == 6702)//Right Gate
                    {
                        if (Mesh == 280) return true;
                        else return false;
                    }
                    return false;
                }
            }
            public void Spawn(Character C, bool Check)
            {
                if (C.Loc.Map == Loc.Map /*&& MyMath.InBox(C.Loc.X, C.Loc.Y, Loc.X, Loc.Y, 16)*/ && (!MyMath.InBox(C.Loc.PreviousX, C.Loc.PreviousY, Loc.X, Loc.Y, 16) || !Check))
                    C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 26, Loc, true, "Gate", CurHP, MaxHP));
            }
            public void ReSpawn()
            {
                foreach (Character C in World.H_Chars.Values)
                    if (C.Loc.Map == Loc.Map && MyMath.InBox(C.Loc.X, C.Loc.Y, Loc.X, Loc.Y, 16))
                        C.MyClient.SendPacket(Packets.SpawnNPCWithHP(EntityID, (ushort)Mesh, 26, Loc, true, "Gate", CurHP, MaxHP));
            }
            public void TakeAttack(Character C, uint Damage, byte AtkType)
            {
                if (AtkType != 21)
                    World.Action(C, Packets.AttackPacket(C.EntityID, EntityID, Loc.X, Loc.Y, Damage, AtkType));
                if (Damage >= CurHP)
                {
                    C.AtkMem.Attacking = false;
                    C.AtkMem.Target = 0;
                    World.Action(C, Packets.AttackPacket(C.EntityID, EntityID, Loc.X, Loc.Y, 0, 14));
                    Opened = true;
                    ReSpawn();
                }
                else
                    CurHP -= Damage;
                //Game.World.Spawns(C, true);
                //World.Action(C, Packets.AttackPacket(C.EntityID, EntityID, Loc.X, Loc.Y, Damage, AtkType));
            }
        }

        public static Pole ThePole;
        public static Gate TheLeftGate;
        public static Gate TheRightGate;

        public static bool War;
        public static Hashtable Scores;
        public static Hashtable GuildWarScores;
        public static DateTime LastScores;
        public static Guild LastWinner;

        public static void Init()
        {
            War = false;
            Scores = new Hashtable();
            GuildWarScores = new Hashtable();
            LastScores = DateTime.Now;

            ThePole = new Pole();
            ThePole.EntityID = 6700;
            ThePole.Mesh = 1137;//1137
            ThePole.CurHP = 20000000;
            ThePole.MaxHP = 20000000;
            ThePole.Loc = new Location();
            ThePole.Loc.Map = 1038;
            ThePole.Loc.X = 84;
            ThePole.Loc.Y = 99;

            TheLeftGate = new Gate();
            TheLeftGate.EntityID = 6701;
            TheLeftGate.Opened = false;
            TheLeftGate.MaxHP = 10000000;
            TheLeftGate.CurHP = 10000000;
            TheLeftGate.Loc = new Location();
            TheLeftGate.Loc.Map = 1038;
            TheLeftGate.Loc.X = 163;
            TheLeftGate.Loc.Y = 210;
            TheLeftGate.ReSpawn();

            TheRightGate = new Gate();
            TheRightGate.EntityID = 6702;
            TheRightGate.Opened = false;
            TheRightGate.MaxHP = 10000000;
            TheRightGate.CurHP = 10000000;
            TheRightGate.Loc = new Location();
            TheRightGate.Loc.Map = 1038;
            TheRightGate.Loc.X = 222;
            TheRightGate.Loc.Y = 177;
            TheRightGate.ReSpawn();
        }
        public static void AddScore(Guild G, uint Points)
        {
            if (!Scores.Contains(G.GuildID))
            {
                GWScore S = new GWScore();
                S.Score = Points;
                S.TheGuild = G;
                Scores.Add(G.GuildID, S);
            }
            else
            {
                GWScore S = (GWScore)Scores[G.GuildID];
                S.Score += Points;
            }
        }
        public static string[] ShuffleGuildScores()
        {
            try
            {
                string[] ret = new string[5];
                DictionaryEntry[] Vals = new DictionaryEntry[5];

                for (sbyte i = 0; i < 5; i++)
                {
                    Vals[i] = new DictionaryEntry();
                    Vals[i].Key = (ushort)0;
                    Vals[i].Value = (uint)0;
                }

                foreach (DictionaryEntry Score in GuildWarScores)
                {
                    sbyte Pos = -1;
                    for (sbyte i = 0; i < 5; i++)
                    {
                        //if (((GWScore)Score.Value).Score > (uint)Vals[i].Value)
                        if ((uint)Score.Value > (uint)Vals[i].Value)
                        {
                            Pos = i;
                            break;
                        }
                    }
                    if (Pos == -1)
                        continue;

                    for (sbyte i = 4; i > Pos; i--)
                        Vals[i] = Vals[i - 1];

                    Vals[Pos] = Score;
                }

                for (sbyte i = 0; i < 5; i++)
                {
                    if ((ushort)Vals[i].Key == 0)
                    {
                        ret[i] = "";
                        continue;
                    }
                    Features.Guild eGuild = (Features.Guild)Features.Guilds.AllTheGuilds[(ushort)Vals[i].Key];
                    ret[i] = "No  " + (i + 1).ToString() + ": " + eGuild.GuildName + "(" + Vals[i].Value + ")";
                }

                return ret;
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); return null; }
        }
        public static void SendScores()
        {
            LastScores = DateTime.Now;
            string[] ShuffledScores = ShuffleGuildScores();

            foreach (Character C in World.H_Chars.Values)
            {
               if (C.Loc.Map == 1038)
                {
                    byte c = 0;
                    foreach (string t in ShuffledScores)
                    {
                        if (t != "")
                        {
                            if (c == 0)
                                C.MyClient.SendPacket(Packets.ChatMessage(0, "SYSTEM", "ALLUSERS", t, 0x83c, 0, System.Drawing.Color.White));
                            else
                                C.MyClient.SendPacket(Packets.ChatMessage(0, "SYSTEM", "ALLUSERS", t, 0x83d, 0, System.Drawing.Color.White));
                        }
                        c++;
                    }
                }
            }
        }

        public static void StartWar()
        {
            Init();
            World.WorldMessage("SYSTEM", "Guild War has begun!", 2011, 0, System.Drawing.Color.Red);
            War = true;
        }
        public static void FinishWar()
        {
            Database.ResetTopGuild();
            War = false;
            TheLeftGate.Opened = false;
            TheRightGate.Opened = false;

            TheLeftGate.CurHP = TheLeftGate.MaxHP;
            TheRightGate.CurHP = TheRightGate.MaxHP;
            TheLeftGate.ReSpawn();
            TheRightGate.ReSpawn();

            GWScore Highest = new GWScore();
            Highest.Score = 0;


            if (Highest.TheGuild != null)
            {
                // LastWinner = Highest.TheGuild;
                LastWinner.Wins++;
                ThePole.ReSpawn();
                World.WorldMessage("SYSTEM", LastWinner + " have won!", 2011, 0, System.Drawing.Color.Red);
            }
            Features.Guilds.SaveGuilds();
            SendScores();
            Scores = new Hashtable();
        }
        public static void EndPole()
        {
            //War = false;
            TheLeftGate.Opened = false;
            TheRightGate.Opened = false;

            TheLeftGate.CurHP = TheLeftGate.MaxHP;
            TheRightGate.CurHP = TheRightGate.MaxHP;
            TheLeftGate.ReSpawn();
            TheRightGate.ReSpawn();

            GWScore Highest = new GWScore();
            Highest.Score = 0;

            foreach (GWScore Score in Scores.Values)
            {
                if (Score.Score > Highest.Score)
                    Highest = Score;
            }

            if (Highest.TheGuild != null)
            {
                LastWinner = Highest.TheGuild;
                //LastWinner.Wins++;
                ThePole.ReSpawn();
                World.WorldMessage("SYSTEM", LastWinner.GuildName + " have won!", 2011, 0, System.Drawing.Color.Red);
            }
            Features.Guilds.SaveGuilds();
            SendScores();
            Scores = new Hashtable();
            GuildWarScores.Clear();
            //StartWarAgain();
        }
    }
}
