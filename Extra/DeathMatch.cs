using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.Extra
{
    public class Teams
    {
        public ushort TeamID = 0;
        public string TeamName = "";
    }
    public class DMTornament
    {
        public static DateTime LastScores;
        public static string WinerName1 = " ";
        public static string WinerName2 = " ";
        public static string WinerName3 = " ";
        public static string WinerName4 = " ";

        public static string[] ShuffleTDMscore()
        {
            string[] ret = new string[5];
            DictionaryEntry[] Vals = new DictionaryEntry[5];
            for (sbyte i = 0; i < 5; i++)
            {
                Vals[i] = new DictionaryEntry();
                Vals[i].Key = (ushort)0;
                Vals[i].Value = (uint)0;
            }
            foreach (DictionaryEntry Score in Game.World.DeathMatch.TDMScore)
            {
                sbyte Pos = -1;
                for (sbyte i = 0; i < 5; i++)
                {
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
                Extra.Teams eTems = (Extra.Teams)Game.World.AllTeams[(ushort)Vals[i].Key];
                ret[i] = "No  " + (i + 1).ToString() + ": Team " + eTems.TeamName + " with Score: [ " + Vals[i].Value + " ]";
                if ((i + 1).ToString() == "1")
                {
                    WinerName1 = eTems.TeamName;
                    Console.WriteLine("1" + eTems.TeamName);
                }
                else if ((i + 1).ToString() == "2")
                {
                    WinerName2 = eTems.TeamName;
                    Console.WriteLine("2" + eTems.TeamName);
                }
                else if ((i + 1).ToString() == "3")
                {
                    WinerName3 = eTems.TeamName;
                    Console.WriteLine("3" + eTems.TeamName);
                }
                else if ((i + 1).ToString() == "4")
                {
                    WinerName4 = eTems.TeamName;
                    Console.WriteLine("4" + eTems.TeamName);
                }
            }
            return ret;
        }
        public static void SendScores()
        {
            LastScores = DateTime.Now;
            string[] ShuffledScores = ShuffleTDMscore();
            foreach (Character C in World.H_Chars.Values)
            {
                if (C.Loc.Map == 1090)
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
    }
}

