using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Extra
{
    public partial class Exterminator
    {
        public string m_name = "None";
        public uint char_uid;
        public int need_ko = 200;
        public int stage = 1;
        public int char_ko = 0;
        public bool can_continue = true;
    }
    public partial class Exterminat
    {
        public static string SelectMonster()
        {
            string new_mob = "None";
        again:
            int rand = Game.World.Rnd.Next(1, 4);

            switch (rand)
            {
                case 1: new_mob = "S-Dragon"; break;
                case 2: new_mob = "M-Dragon"; break;
                case 3: new_mob = "L-Dragon"; break;
                case 4: new_mob = "Poltergaist"; break;
                default: break;
            }
            if (new_mob == "None")
                goto again;

            return new_mob;
        }

        public static string Continue(int stage)
        {
            string n_mob = "None";
        again:
            int rand = Game.World.Rnd.Next(1, 4);

            switch (stage)
            {
                case 1:
                    {
                        switch (rand)
                        {
                            case 1: n_mob = "WingedSnake"; break;
                            case 2: n_mob = "Bandit"; break;
                            case 3: n_mob = "FireRat"; break;
                            case 4: n_mob = "FireSpirit"; break;
                            default: goto again;
                        }
                        break;
                    }
                case 2:
                    {
                        switch (rand)
                        {
                            case 1: n_mob = "Macaque"; break;
                            case 2: n_mob = "GiantApe"; break;
                            case 3: n_mob = "ThunderApe"; break;
                            case 4: n_mob = "Snakeman"; break;
                            default: goto again;
                        }
                        break;
                    }
                case 3:
                    {
                        switch (rand)
                        {
                            case 1: n_mob = "Hero-CPs"; break;
                            case 2: n_mob = "Hero-DB"; break;
                            case 3: n_mob = "BullMonster"; break;
                            case 4: n_mob = "TombBat"; break;
                            default: goto again;
                        }
                        break;
                    }
                default: goto again;
            }

            return n_mob;
        }
    }
}