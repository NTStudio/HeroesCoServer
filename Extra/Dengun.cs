using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.Extra
{
    public enum DengunStage
    {
        None,
        StageOneFirstQuest,
        StageOneSecondQuest,
        StageOneFinished,
        Over
    }
    public class Dengun
    {
        public static DengunStage Stage = DengunStage.None;
        public Character Hero;
        ushort starMap = 1996;
        public ArrayList DengunMaps = new ArrayList() {(ushort) 1996 };
        uint DGdimntion = 0;
        public bool firstPossKilled;
        public bool firstStageClear = false;

        //public bool FiretQuestDone = false;
        public Hashtable DengunMembers;
        public DateTime DengunStarted;

        public byte TheFourPossMobsKilled = 3;
        public byte TheFourPossKilled = 0;

        ushort requeredNuber_1 = 15;
        public ushort UndeadSoldier, UndeadSpearman, HellTroll, Centicore;
        public Dengun(Character Hero)
        {
            this.Hero = Hero;
            Hero.TakeDimention();
            DGdimntion = Hero.Loc.MapDimention;
            Database.LoadDengun(Hero.Loc.MapDimention, "Dengun1/DengunMonsters");
            Console.WriteLine("Dengun Loaded For : " + Hero.Name);
            Hero.Teleport(starMap, 29, 120);
            firstPossKilled = false;
            DengunMembers = new Hashtable();
            Stage = DengunStage.StageOneFirstQuest;
            DengunMembers.Add(Hero.EntityID, Hero);
            DengunStarted = DateTime.Now;
        }

        public bool joinNewMem(Character C,out byte ErorrType)
        {
           
            //if (Hero2 != null)
            //    Hero2 = C;
            //else if (Hero3 != null)
            //    Hero3 = C;
            //else if (Hero4 != null)
            //    Hero4 = C;
            //else if (Hero5 != null)
            //    Hero5 = C;
            if(DengunMembers.Count < 5)
            {
                if (C.FindInventoryItemIDAmount(12300/*put item id here*/, 1))
                {
                    DengunMembers.Add(C.EntityID, C);
                    C.RemoveItemIDAmount(1230/*put item id here*/, 1);
                    C.Loc.MapDimention = DGdimntion;
                    C.Teleport(1996, 29, 121);
                }
                else
                {
                    ErorrType = 1;
                    return false;
                }
            }
            else
            {
                C.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, "There is no Room Left.");
                ErorrType = 2;
                return false;
            }
            ErorrType = 0;
            return true;
        }
        // transfeer leader randomly
        public void TransfeerLeader()
        {
            foreach (Character C in DengunMembers.Values)
            {
                if (DengunMaps.Contains(C.Loc.Map))
                {
                    Hero = C;
                    break;
                }
            }
            //Abandon(1);
        }
        public void Abandon(byte type)
        {
            try
            {
                foreach (Character C in DengunMembers.Values)
                {
                    C.myDengun1 = null;
                    if (DengunMaps.Contains(C.Loc.Map))
                        C.Teleport(1700, 311, 325);
                    if (type == 1)
                        DengunMembersMessage("TimeUp You Have been teleported out [you have to finsh the dengun in 60 minuts].");
                    else if (type == 2)
                        DengunMembersMessage("Dengun Closed no body left.");

                }

               
                foreach(ushort map in DengunMaps)
                {
                    ClearMobs(map);
                }
                
            }
            catch (Exception e) { Program.WriteMessage(e); }

        }
        public void ClearMobs(ushort map)
        {
                Hashtable RemoveList = new Hashtable();
                Hashtable mapmobs = (Hashtable)World.H_Mobs[map];
                foreach (Mob mob in mapmobs.Values)
                {
                    if (mob.Loc.MapDimention == DGdimntion)
                    {
                        RemoveList.Add(mob.EntityID, mob);
                    }
                }
                foreach (Mob M in RemoveList.Values)
                {
                    mapmobs.Remove(M.EntityID);
                    World.Action(M, Packets.AttackPacket(12345, M.EntityID, M.Loc.X, M.Loc.Y, 1, (byte)MobAttackType.Kill));
                    World.Action(M, Packets.Status(M.EntityID, Status.Effect, 2080));

                }
            
        }
        public bool KillsTester(string MobName,ushort MobKilled, ushort RequeredN)
        {
            if (MobKilled < RequeredN)
            {
                foreach (Character C in DengunMembers.Values)
                {
                    C.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, MobName + " Killed : " + MobKilled + " , there " + (RequeredN - MobKilled) + " Left ..");
                }
            }
            else if (MobKilled == RequeredN)
            {
                return true;
                //string mobinfo = "6100 Wraith 1 327 255 3934 30000 100 0 10030 23280 15 130 2 True 2 70 700 1500 900 True";

                //World.Load_Single_Monster(mobinfo,starMap,
               
            }
            else
                return false;
            return false;

        }
        public void DMKilledCounter(string MobName)
        {

            if (!firstStageClear)
                Firststage(MobName);
        }
        void Firststage(string MobName)
        {
            if (Stage == DengunStage.StageOneFirstQuest)
            {
                if (MobName == "UndeadSoldier")
                {
                    UndeadSoldier++;
                    if (KillsTester(MobName, UndeadSoldier, requeredNuber_1))
                    {
                        if (TheFourPossMobsKilled < 4)
                            TheFourPossMobsKilled++;
                        DengunMembersMessage(MobName + " quest finished " + (4 - TheFourPossMobsKilled) + " quests left..");

                    }
                }
                if (MobName == "UndeadSpearman")
                {
                    UndeadSpearman++;
                    if (KillsTester(MobName, UndeadSpearman, requeredNuber_1))
                    {
                        if (TheFourPossMobsKilled < 4)
                            TheFourPossMobsKilled++;
                        DengunMembersMessage(MobName + " quest finished " + (4 - TheFourPossMobsKilled) + " quests left..");

                    }
                }
                if (MobName == "HellTroll")
                {
                    HellTroll++;
                    if (KillsTester(MobName, HellTroll, requeredNuber_1))
                    {
                        if (TheFourPossMobsKilled < 4)
                            TheFourPossMobsKilled++;
                        DengunMembersMessage(MobName + " quest finished " + (4 - TheFourPossMobsKilled) + " quests left..");

                    }
                }
                if (MobName == "Centicore")
                {
                    Centicore++;
                    if (KillsTester(MobName, Centicore, requeredNuber_1))
                    {
                        if (TheFourPossMobsKilled < 4)
                            TheFourPossMobsKilled++;
                        DengunMembersMessage(MobName + " quest finished " + (4 - TheFourPossMobsKilled) + " quests left..");

                    }
                }

                if (TheFourPossMobsKilled == 4)
                {
                    Console.WriteLine("first dengun quest done");
                    ClearMobs(starMap);
                    Database.LoadDengun(DGdimntion, "Dengun1/Dengun4poss");
                    Stage = DengunStage.StageOneSecondQuest;
                    DengunMembersMessage(MobName + "the first quest finished go to kill the 4 monsters in the end if the pass..");

                }
            }
            if (Stage == DengunStage.StageOneSecondQuest)
            {
                if (MobName == "UndeadSoldierPoss")
                {
                    if (TheFourPossKilled < 4)
                        TheFourPossKilled++;

                }
                if (MobName == "UndeadSpearmanPoss")
                {
                    if (TheFourPossKilled < 4)
                        TheFourPossKilled++;

                }
                if (MobName == "HellTrollPoss")
                {
                    if (TheFourPossKilled < 4)
                        TheFourPossKilled++;

                }
                if (MobName == "CenticorePoss")
                {
                    if (TheFourPossKilled < 4)
                        TheFourPossKilled++;
                }

                if (TheFourPossKilled == 4)
                {
                    Mob M;
                    string info = "6105 LeaderOfDeads 1 179 255 25794 38000 100 0 10000 13000 15 130 2 True 6 70 700 1500 900 True 1260 9 2500";
                    World.Load_Single_Monster(info, (ushort)starMap, (ushort)136, (ushort)76, (ushort)DGdimntion, out  M);
                    M.promoud(1, 6001, 4, 12000);
                    DengunMembersMessage(" The second Quest have been finished one quest remaining.");
                    World.Spawn(M, false);
                }
                if (MobName == "LeaderOfDeads")
                {
                    Stage = DengunStage.StageOneFinished;
                    firstPossKilled = true;
                    // temporary Dengune clear here
                    {
                        foreach (Character C in DengunMembers.Values)
                        {

                            C.IncreaseExp(C.EXPBall * 15, false);
                        }
                        DengunMembersMessage("You Finished The Dengun and got 15 * EXP Ball ");
                        Abandon(1);
                    }
                }
            }

        }
        public void MonstersDrope(Mob Mob , Character Owner)
        {
            Owner.HonorPoints += 15;
            Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Aqua, "you claimed " + 15 + " HonorPoints");
            if (MyMath.ChanceSuccess(80))
            {
                uint amm = (uint)Program.Rnd.Next(100);
                Owner.CPs += amm;
                Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Yellow, "you claimed " + amm + " CPs");
            }


            if (Mob.MobID == 6105)
            {
                int HonorPointAmount = 15;
                Owner.HonorPoints += HonorPointAmount;

                Owner.MyClient.LocalMessage(2005, System.Drawing.Color.Aqua, "you claimed " + HonorPointAmount + " HonorPoints");



            }

        }
        public void step()
        {
            if (DengunMembers.Count == 0)
                Abandon(2);
            if (DateTime.Now >= DengunStarted.AddMinutes(60))
            {
                Abandon(1);
            }

            {
                ///remove out sided members.
                Hashtable RemoveLest = new Hashtable();
                foreach (Character member in DengunMembers.Values)
                {
                    if (!DengunMaps.Contains(member.Loc.Map) || member.MyClient == null)
                    {
                        if (Hero == member)
                            TransfeerLeader();
                        RemoveLest.Add(member.EntityID, member);
                    }
                }
                foreach (Character C in RemoveLest.Values)
                {
                    DengunMembers.Remove(C.EntityID);
                }
                RemoveLest = null;
            }
            
        }
        void DengunMembersMessage(string line)
        {
            foreach (Character C in DengunMembers.Values)
            {
                C.MyClient.LocalMessage(2011, System.Drawing.Color.Yellow, line);
            }
        }
        
    }
}
