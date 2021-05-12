using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HeroesOnline.Game
{
    public class Trainer
    {
        /*DateTime LastJump = DateTime.Now;
        DateTime LastWalk = DateTime.Now;
        DateTime LastMotionChange = DateTime.Now;
        DateTime LastFoundSuitableMob = DateTime.Now;
        int WalkSpeed = 250;
        int JumpSpeed = 750;
        Location Destination = new Location();
        Location PortalLocation = new Location();
        bool GoToPortal = false;
        bool GoToDestination = false;*/

        //bool SearchingLevPlace = false;
        //bool FoundLevelingPlace = false;


        void GetDirection(Location Target, Character C)
        {
            //TX = (ushort)Target.X + 3;
            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(C.Loc.X, C.Loc.Y, Target.X, Target.Y) / 45 % 8)) - 1 % 8);
            C.Direction = (byte)((int)ToDir % 8);
        }
        bool FreeToGo(Character C)
        {
            Location eLoc = C.Loc;
            eLoc.Walk(C.Direction);
            if (!DMaps.H_DMaps.Contains(C.Loc.Map))
                return true;
            if (((DMap)DMaps.H_DMaps[C.Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess)
                return false;
            return true;
        }
        void Jump(Character C, ushort X, ushort Y)
        {
            C.LastJump = DateTime.Now;
            World.Action(C, Packets.GeneralData(C.EntityID, X, Y, 0, 0, C.Direction, 137));
            C.Loc.Jump(X, Y);
            World.Spawn(C, true);
            C.Protection = false;
        }
        void WalkAround(Character C, Character Owner, uint WalkOrRun)
        {
            C.LastWalk = DateTime.Now;
            GetDirection(Owner.Loc, C);
            C.Direction = (byte)((C.Direction + 1) % 8);
            C.Loc.Walk(C.Direction);
            World.Action(C, Packets.Movement(C.EntityID, C.Direction, WalkOrRun));
            World.Spawn(C, true);
            C.Protection = false;
        }
        void Walk(Character C, Character Owner, uint WalkOrRun)
        {

            GetDirection(Owner.Loc, C);
            byte eDir = C.Direction;
            bool Success = true;
            ushort X;
            ushort Y;
            while (!FreeToGo(C))
            {
                try
                {
                    C.Direction = (byte)((C.Direction + 1) % 8);
                    if (C.Direction == eDir)
                    {
                        Success = false;
                        break;
                    }
                    Success = true;
                }
                catch { }
            }
            if (!Success)
            {
                X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                Jump(C, X, Y);
            }
            else
            {

                C.Loc.Walk(C.Direction);
                World.Action(C, Packets.Movement(C.EntityID, C.Direction, WalkOrRun));
                World.Spawn(C, true);
                C.Protection = false;
            }
        }
        void Revive(Character C)
        {
            if (!C.Alive)
            {
                //if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(0))
                {
                    C.Ghost = false;
                    C.BlueName = false;
                    C.CurHP = (ushort)C.MaxHP;
                    C.Alive = true;
                    C.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.Dead);
                    C.StatEff.Remove(HeroesOnline.Game.StatusEffectEn.BlueName);
                    C.Body = C.Body;
                    C.Hair = C.Hair;
                    //C.Equips.Send(C.MyClient, false);
                    C.Protection = true;
                    C.LastProtection = DateTime.Now;
                    Game.World.Spawn(C, false);
                    //C.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                }
            }
        }
        public ushort X;
        public ushort Y;
        public Character C;
        public Character Owner;
        public Trainer(Character C, Character Owner)
        {
            this.C = C;
            this.Owner = Owner;

            Owner.MyTrainer = C;
            C.Owner = Owner;

        }
        // new Thread(new ThreadStart(delegate()
        // {
        //   while (true)
        public void Step()
        {
            try
            {
                if (!C.Alive && DateTime.Now >= C.DeathHit.AddSeconds(C.reviveSpeed))
                    Revive(C);
                if (C.Alive && Owner.Alive && DateTime.Now > C.LoggedOn.AddMilliseconds(2000))
                {

                    if (C == null)
                    {
                        Owner.MyTrainer = null;

                    }
                    if (Owner == null)
                        Thread.Sleep(5000);
                    { C.MyClient.Disconnect(); }
                    if (C.CurHP <= C.MaxHP - 100 && DateTime.Now > C.lastHPUse.AddMilliseconds(2500))
                    {
                        C.lastHPUse = DateTime.Now;
                        C.CurHP += 4000;
                        if (C.CurHP > C.MaxHP)
                            C.CurHP = C.MaxHP;
                    }
                    
                    if (C.Loc.Map == Owner.Loc.Map && C.Loc.MapCopy == Owner.Loc.MapCopy)
                    {
                        if (MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 14)
                        {
                            if ((C.training || C.CurHP < C.MaxHP / 1.34) && C.Loc.Map != 1002 && C.Loc.Map != 1036 && C.Loc.Map != 1039 && C.Loc.Map != 700)
                            {
                                if (DateTime.Now > C.LastJump.AddMilliseconds(C.jumpSpeed))
                                {
                                    if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                        Thread.Sleep(290);
                                    X = (ushort)(Owner.Loc.X + Program.Rnd.Next(11) - Program.Rnd.Next(7));
                                    Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(11) - Program.Rnd.Next(7));
                                    Jump(C, X, Y);
                                }
                            }
                            else
                            {
                                if (MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 8 && DateTime.Now >= C.LastWalk.AddMilliseconds(C.RunSpeed) && DateTime.Now >= C.LastJump.AddMilliseconds(300))
                                {
                                    Walk(C, Owner, 1);

                                }
                                else if (MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 6 && DateTime.Now >= C.LastWalk.AddMilliseconds(C.WalkSpeed) && DateTime.Now >= C.LastJump.AddMilliseconds(300))
                                {
                                    WalkAround(C, Owner, 0);
                                }
                            }
                        }
                        else
                        {
                            if (DateTime.Now > C.LastJump.AddMilliseconds(C.jumpSpeed))
                            {
                                if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                    Thread.Sleep(390);
                                X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                Jump(C, X, Y);
                            }
                        }
                    }
                    if (C.Loc.MapCopy != Owner.Loc.MapCopy)
                    {
                        Thread.Sleep(1000);
                        Game.World.Action(C, Packets.GeneralData(C.EntityID, 0, 0, 0, 135));
                        C.Loc.MapCopy = Owner.Loc.MapCopy;
                        Game.World.Spawns(C, false);
                    }
                    if (C.Loc.Map != Owner.Loc.Map)
                    {
                        Thread.Sleep(2000);
                        X = (ushort)(Owner.Loc.X + Program.Rnd.Next(9) - Program.Rnd.Next(9));
                        Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(9) - Program.Rnd.Next(9));

                        C.Teleport(Owner.Loc.Map, X, Y);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            Thread.Sleep(130);
        }
        //}
        // )).Start();

    }
}
