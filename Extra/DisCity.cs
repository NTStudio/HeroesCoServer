using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.Game;
using System.Threading;

namespace Server.Extra
{
    public static class discity
    {
        private static Thread nightdevil_thread;

        public static void Stage4()
        {
            nightdevil_thread = new Thread(new ThreadStart(Stage4run));
            nightdevil_thread.IsBackground = true;
            nightdevil_thread.Start();
        }
        public static void Stage4run()
        {
            while (true)
            {
                if (World.DisCity.PlayerInStage3 >= 50)
                {
                    foreach (DictionaryEntry DE in World.H_Chars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Loc.Map == 2023)
                        {
                            Chaar.DisKO = 0;
                            Chaar.CPs += 50000;
                            Chaar.IncreaseExp(Chaar.EXPBall, false);
                            Chaar.StatEff.Add(StatusEffectEn.Shield);
                            Chaar.Teleport(2024, 150, 290);
                            Chaar.StatEff.Add(StatusEffectEn.XPStart);
                            Chaar.Buffs.Add(new Buff() { StEff = StatusEffectEn.XPStart, Lasts = 20, Started = DateTime.Now, Eff = Skills.SkillsClass.ExtraEffect.Cyclone });
                            World.WorldMessage("Server", "All Players On HellCloister Teleport To Next Stage [BattleFormation]", 2011, 0, System.Drawing.Color.Red);
                            Thread.Sleep(30000);
                        }
                        else if (Chaar.Loc.Map == 2022)
                        {
                            Chaar.Teleport(1020, 533, 483);
                            Game.World.WorldMessage("Server", "All Players On HellHall Out Because Final Stage Started", 2011, 0, System.Drawing.Color.Red);
                        }
                    }
                }
                if (DateTime.Now.Hour == 20 && DateTime.Now.Minute >= 59)
                {
                    foreach (DictionaryEntry DE in World.H_Chars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Loc.Map == 2021 || Chaar.Loc.Map == 2022 || Chaar.Loc.Map == 2023 || Chaar.Loc.Map == 2024)
                        {
                            Chaar.Teleport(1020, 533, 483);
                            Game.World.WorldMessage("Server", "DisCity Has Finished", 2011, 0, System.Drawing.Color.Red);
                        }
                    }
                }
                Thread.Sleep(50);
            }
        }
    }
}
