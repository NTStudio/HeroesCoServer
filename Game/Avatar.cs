using System;
using System.Collections;

namespace Server.Game
{
    public enum XPskill
    {
        None,
        Superman = 1025,
        Cyclone = 1110,
        fatalstrike = 6011,
    }
   public class Avatar:Character
    {

        byte AvaLevel;
        public DateTime InRange;
        public DateTime Near = DateTime.Now;
        bool catched = false;
        
        //public bool TrainingRevive = false;

        public int jumpSpeed = 700;
        public int WalkSpeed = 330;
        public int RunSpeed = 120;
        public int reviveSpeed = 20;
        public DateTime LastJump = DateTime.Now;
        public DateTime LastWalk = DateTime.Now;
        public bool training = false;

        public ushort X;
        public ushort Y;
        
        Mob PosMob;

        public XPskill XPused = XPskill.None;

        public void Initaliz(Character owner)
        {
            this.Owner = owner;
            owner.MyAvatar = this;
            AvaLevel = owner.AvatarLevel;
            OwnerSigned = true;
            World.Spawns(this, false);
        }
        
        // new Thread(new ThreadStart(delegate()
        //  {
        //    while (true)
        void GetDirection(Location Target, Character C)
        {
            //TX = (ushort)Target.X + 3;
            byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(C.Loc.X, C.Loc.Y, Target.X, Target.Y) / 45 % 8)) - 1 % 8);
            C.Direction = (byte)((int)ToDir % 8);
        }
        bool FreeToGo(Avatar C)
        {
            Location eLoc = C.Loc;
            eLoc.Walk(C.Direction);
            if (!DMaps.H_Maps.Contains(C.Loc.Map))
                return true;
            if (((DMap)DMaps.H_Maps[C.Loc.Map]).GetCell(eLoc.X, eLoc.Y).NoAccess)
                return false;
            return true;
        }
        void Jump(Avatar A, ushort X, ushort Y)
        {
            A.LastJump = DateTime.Now;
            World.Action(A, Packets.GeneralData(A.EntityID, X, Y, 0, 0, A.Direction, 137));
            A.Loc.Jump(X, Y);
            World.Spawn(A, true);
            A.Protection = false;
        }
        void WalkAround(Avatar A, Character Owner, uint WalkOrRun)
        {
            A.LastWalk = DateTime.Now;
            GetDirection(Owner.Loc, A);
            A.Direction = (byte)((A.Direction + 1) % 8);
            A.Loc.Walk(A.Direction);
            World.Action(A, Packets.Movement(A.EntityID, A.Direction, WalkOrRun));
            World.Spawn(A, true);
            A.Protection = false;
        }
        void Walk(Avatar A, Location Loc, uint WalkOrRun)
        {
                A.LastWalk = DateTime.Now;
            GetDirection(Loc, A);
            byte eDir = A.Direction;
            bool Success = true;
            ushort X;
            ushort Y;
            if (!FreeToGo(A))
            {
                try
                {
                    A.Direction = (byte)((A.Direction + 1) % 8);
                    if (A.Direction == eDir)
                    {
                        Success = false;
                        //break;
                    }
                    Success = true;
                }
                catch { }
            }
            if (!Success)
            {
                if (DateTime.Now > A.LastJump.AddMilliseconds(A.jumpSpeed))
                {
                    X = (ushort)(Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                    Y = (ushort)(Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                    Jump(A, X, Y);
                }
            }
            else
            {

                A.Loc.Walk(A.Direction);
                World.Action(A, Packets.Movement(A.EntityID, A.Direction, WalkOrRun));
                World.Spawn(A, true);
                A.Protection = false;
            }
        }
        void UseSkill(ushort SkillId,byte SkillAvaLevel, ushort X, ushort Y, uint Target)
        {
            Game.Skill S = (Game.Skill)Skills[SkillId];
            
            if (Server.Skills.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Lvl))
            {
                Skills.SkillsClass.SkillUse SU = new Server.Skills.SkillsClass.SkillUse();
                SU.Init(this, S.ID, S.Lvl, X, Y);
                if (SU.Info.ID == 0)
                    return;
                bool EnoughArrows = true;
                if (SU.Info.ArrowsCost > 0)
                {
                    /*if (C.Loc.Map != 1039)
                    {
                        if (C.Equips.LeftHand.ID != 0 && Game.Item.IsArrow(C.Equips.LeftHand.ID))
                        {
                            if (C.Equips.LeftHand.CurDur >= SU.Info.ArrowsCost)
                                C.Equips.LeftHand.CurDur -= SU.Info.ArrowsCost;
                            else
                                C.Equips.LeftHand.CurDur = 0;
                            if (C.Equips.LeftHand.CurDur == 0)
                                C.Equips.LeftHand = new Game.Item();
                        }
                        else
                        {
                            C.AtkMem.Attacking = false;
                            EnoughArrows = false;
                        }
                    }*/
                }
                if (CurMP >= SU.Info.ManaCost && Stamina >= SU.Info.StaminaCost && EnoughArrows || Loc.Map == 1039)
                {
                    if (SU.Info.EndsXPWait)
                    {
                        if (StatEff.Contains(Server.Game.StatusEffectEn.XPStart))
                        {
                            StatEff.Remove(Server.Game.StatusEffectEn.XPStart);
                            XPKO = 0;
                        }
                        else
                            return;
                    }
                    if (Loc.Map != 1039)
                    {
                        CurMP -= SU.Info.ManaCost;
                        Stamina -= SU.Info.StaminaCost;
                    }
                    else
                    {
                        AtkMem.AtkType = 21;
                        AtkMem.Skill = SU.Info.ID;
                        AtkMem.Attacking = true;
                        AtkMem.Target = Target;
                        AtkMem.LastAttack = DateTime.Now;
                        AtkMem.SX = X;
                        AtkMem.SY = Y;
                    }
                    SU.GetTargets(Target);
                    SU.Use();
                    AtkMem.LastAttack = DateTime.Now;
                }
            }
        }
        bool speed(Avatar A)
        {
            if(A.StatEff.Contains(Game.StatusEffectEn.Cyclone))
            {
                return true;
            }
            return false;
        }
        void Revive(Avatar A)
        {
            if (!A.Alive)
            {
                //if (DateTime.Now > GC.MyChar.DeathHit.AddSeconds(0))
                {
                    A.Ghost = false;
                    A.BlueName = false;
                    A.CurHP = (ushort)A.MaxHP;
                    A.Alive = true;
                    A.StatEff.Remove(Server.Game.StatusEffectEn.Dead);
                    A.StatEff.Remove(Server.Game.StatusEffectEn.BlueName);
                    A.Body = A.Body;
                    A.Hair = A.Hair;
                    //C.Equips.Send(C.MyClient, false);
                    A.Protection = true;
                    A.LastProtection = DateTime.Now;
                    Game.World.Spawn(A, false);
                    //C.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 0));
                }
                reviveSpeed = 20;
                //TrainingRevive = false;
            }
        }
        Mob NearestMob
        {
            get
            {
               Hashtable H = (Hashtable)World.H_Mobs[Loc.Map];
                if (H != null)
                {
                    Mob Nearest = null;
                    byte NDist = (byte)(15 + AvaLevel * 2);//AvaLevel
                    foreach (Mob M in H.Values)
                        if (M.Type == MobType.HuntPlayers && M.Loc.MapDimention == Loc.MapDimention && M.Alive && MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) < NDist && MyMath.PointDistance(Owner.Loc.X, Owner.Loc.Y, M.Loc.X, M.Loc.Y) < 41)
                        {
                            Nearest = M;
                            NDist = (byte)MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y);
                        }
                    return Nearest;
                }
                return null;
            }
        }
        bool CanGo(Vector2 Pos, out int JumpLen)
        {
            int Distance = (int)MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y);
            DMap DM = (DMap)DMaps.H_Maps[Loc.Map];
            bool f = false;
            JumpLen = 14;

            for (int i = 0; i <= Math.Min(64, Distance); i++)
            {
                Vector2 V = DestPart(Pos, i, 0);
                if (!DM.GetCell(V.X, V.Y).NoAccess)
                {
                    f = true;
                    if (i < 16)
                    JumpLen = i;
                }
            }
            return f;
        }
        Vector2 DestPart(Vector2 Target, int Length, short DirAdd)
        {
            double Dir = MyMath.PointDirectonRad(Loc.X, Loc.Y, Target.X, Target.Y);
            Dir += DirAdd * Math.PI / 180;
            Vector2 V = new Vector2();
            V.X = Loc.X;
            V.Y = Loc.Y;
            V.X = (ushort)(Loc.X + Math.Cos(Dir) * Length);
            V.Y = (ushort)(Loc.Y + Math.Sin(Dir) * Length);
            return V;
        }
        ushort MPamount()
        {
            ushort MP = (ushort)(MaxMP*0.15);
            return MP;
        }
        ushort HPamount()
        {
            ushort HP = 1000;
            if (AvaLevel == 1)
                HP = 3000;
           else if (AvaLevel == 2)
                HP = 3300;
            else if (AvaLevel == 3)
                HP = 3700;
            else if (AvaLevel == 4)
                HP = 4100;
            else if (AvaLevel >= 5)
                HP = 4500;
            return HP;
        }
        void Travel(Vector2 To)
        {
            ArrayList Chances = new ArrayList();
            Vector2 JumpTo = DestPart(To, Math.Min(11, MyMath.PointDistance(Loc.X, Loc.Y, To.X, To.Y)), Direction);

            DMap DM = (DMap)DMaps.H_Maps[Loc.Map];
            bool Add = true;
            if (MyMath.PointDirecton(Loc.X, Loc.Y, To.X, To.Y) >= 90 && MyMath.PointDirecton(Loc.X, Loc.Y, To.X, To.Y) < 270)
                Add = false;
            int JumpLen = 14;
            byte trys = 0;
            bool g = false;
            while ((Chances.Contains(JumpTo) || DM.GetCell(JumpTo.X, JumpTo.Y).NoAccess || !CanGo(JumpTo, out JumpLen)) && trys < 11)
            {
                g = true;
                if (Add)
                    Direction += 10;
                else
                    Direction -= 10;
                JumpTo = DestPart(To, JumpLen, Direction);
                trys++;
                if (trys > 10)
                    Jump(this,To.X,To.Y);
            }
            if (!g)
                Direction = 0;

            if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed))
                Jump(this,JumpTo.X, JumpTo.Y);
        }



        public void step()
        {


            try
            {
                if (Loc.Map == 1707 || Loc.Map == 1090)
                {
                    Owner.MyAvatar = null;
                    MyClient.Disconnect();
                }
                if (this == null)
                {
                    Owner.MyAvatar = null;
                    //break;
                }
                if (Owner == null && World.H_Chars.ContainsKey(EntityID))
                { MyClient.Disconnect(); }
                if (!Alive || !Owner.Alive)
                { Process = ""; }
                if (Owner.Alive && !Alive && DateTime.Now >= DeathHit.AddSeconds(reviveSpeed))
                    Revive(this);
                if (this != null && DateTime.Now >= Owner.AvaSummoned.AddSeconds(Owner.avaLasts))
                {
                    MyClient.LocalMessage(2011, System.Drawing.Color.Red, "Your Avatar Has Finish his Time.");
                    Owner.MyAvatar = null;
                    MyClient.Disconnect();
                    Owner.AvaRunning = false;
                    //Console.WriteLine("Owner.AvaRunning = false ava");
                }
                if (Alive && Owner.Alive && DateTime.Now > LoggedOn.AddMilliseconds(2000))
                {

                    if (StatEff.Contains(StatusEffectEn.XPStart) && AvaLevel >= 4)//AvaLevel 4
                    {
                        if (XPused != XPskill.None && Skills.Contains((ushort)XPused))
                        {
                            UseSkill((ushort)XPused, 0, Loc.X, Loc.Y, EntityID);

                        }
                        else
                        {
                            if (Skills.Contains((ushort)1025))
                                UseSkill(1025, 0, Loc.X, Loc.Y, EntityID);
                            else
                            {
                                if (Skills.Contains((ushort)6011))
                                    UseSkill(6011, 0, Loc.X, Loc.Y, EntityID);
                                else if (Skills.Contains((ushort)1110))
                                    UseSkill(1110, 0, Loc.X, Loc.Y, EntityID);
                                else if (Skills.Contains((ushort)1025))
                                    UseSkill(1025, 0, Loc.X, Loc.Y, EntityID);
                            }
                        }
                    }
                    PKMode = Owner.PKMode;
                    if (CurHP <= MaxHP - 100 && DateTime.Now > lastHPUse.AddMilliseconds(2500) && Stamina > 9)
                    {
                        Stamina -= 10;
                        lastHPUse = DateTime.Now;
                        CurHP += HPamount();
                        if (CurHP > MaxHP)
                            CurHP = MaxHP;
                    }
                    if (CurMP <= MaxMP / 2 && DateTime.Now > lastMPUse.AddMilliseconds(2500))
                    {
                        lastMPUse = DateTime.Now;
                        CurMP += MPamount();
                        if (CurHP > MaxHP)
                            CurHP = MaxHP;
                    }
                    if (training == true)
                    {
                        Owner.ownerpk = true;
                    }
                    if (Owner.StatEff.Contains(StatusEffectEn.BlueName) && Owner.PKMode == PKMode.Team && Owner.PKMode == PKMode.PK)
                        Process = "";
                    if (Loc.Map == Owner.Loc.Map && Loc.MapDimention == Owner.Loc.MapDimention)
                    {
                        if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 20)
                        {
                            Near = DateTime.Now;
                        }
                        if (Process == "")
                        {
                            if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 15)
                            {
                                if ((training || CurHP < MaxHP / 1.34) && Loc.Map != 1002 && Loc.Map != 1036 && Loc.Map != 1039 && Loc.Map != 700)
                                {
                                    if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed))
                                    {
                                        X = (ushort)(Owner.Loc.X + Program.Rnd.Next(11) - Program.Rnd.Next(7));
                                        Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(11) - Program.Rnd.Next(7));
                                        Jump(this, X, Y);
                                    }
                                }
                                else
                                {
                                    if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 6 && DateTime.Now >= LastWalk.AddMilliseconds(RunSpeed) && DateTime.Now >= LastJump.AddMilliseconds(600))
                                    {
                                        Walk(this, Owner.Loc, 1);

                                    }
                                    else if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 4 && DateTime.Now >= LastWalk.AddMilliseconds(WalkSpeed) && DateTime.Now >= LastJump.AddMilliseconds(600))
                                    {
                                        WalkAround(this, Owner, 0);
                                    }

                                }
                                if (AtkMem.Target != 0 || Owner.AtkMem.Target != 0)
                                {
                                    // if (World.H_Chars.ContainsKey(C.AtkMem.Target) || World.H_Chars.ContainsKey(Owner.AtkMem.Target) || World.H_Mobs.ContainsKey(Owner.AtkMem.Target))
                                    Process = "attack";
                                }
                                else
                                {

                                    if (Owner.CurHP < Owner.MaxHP + 200 && DateTime.Now > AtkMem.LastAttack.AddMilliseconds(500))
                                    {
                                        if (Skills.Contains((ushort)1005))
                                        {
                                            Skill s = (Skill)Skills[(ushort)1005];
                                            UseSkill(1005, s.Lvl, Owner.Loc.X, Owner.Loc.Y, Owner.EntityID);
                                        }
                                    }
                                }


                            }
                            else
                            {
                                if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 14)
                                {
                                    //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                    X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                    Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                    try
                                    {
                                        Travel(new Vector2(X, Y));
                                    }
                                    catch { Jump(this, X, Y); }
                                }
                            }
                            if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 65 || DateTime.Now >= Near.AddSeconds(30))
                            {
                                X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                Jump(this, X, Y);
                            }

                        }
                        else if (Process == "Hunting" && AvaLevel >= 3)//AvaLevel 3
                        {
                            if (Owner.StatEff.Contains(StatusEffectEn.BlueName) && (Owner.PKMode == PKMode.PK || Owner.PKMode == PKMode.Team))
                                Process = "";
                            if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 40)
                            {
                                #region hunting

                                if (AtkMem.Target == 0)
                                {
                                    Mob M = NearestMob;
                                    if (M != null)
                                        AtkMem.Target = M.EntityID;
                                    else
                                    {

                                        //if (MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 7 && DateTime.Now >= C.LastWalk.AddMilliseconds(C.RunSpeed) && DateTime.Now >= C.LastJump.AddMilliseconds(300))
                                        //{
                                        //    Walk(C, Owner.Loc, 1);

                                        //}
                                        //else 
                                        if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 9 && DateTime.Now >= LastWalk.AddMilliseconds(WalkSpeed) && DateTime.Now >= LastJump.AddMilliseconds(300))
                                        {
                                            WalkAround(this, Owner, 0);
                                        }
                                        //else
                                        //{
                                        //    if (DateTime.Now > C.LastJump.AddMilliseconds(C.jumpSpeed) && MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 14)
                                        //    {
                                        //        
                                        //        X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                        //        Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                        //        Jump(C, X, Y);
                                        //    }
                                        //}
                                    }
                                }
                                else
                                {
                                    if (((Hashtable)World.H_Mobs[Loc.Map]).Contains(AtkMem.Target))
                                    {
                                        Mob M = (Mob)((Hashtable)World.H_Mobs[Loc.Map])[AtkMem.Target];

                                        if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= 2)
                                            AtkMem.Attacking = true;
                                        else if (StatEff.Contains(StatusEffectEn.FatalStrike) && MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= 10)
                                            AtkMem.Attacking = true;
                                        else
                                        {
                                            AtkMem.Attacking = false;
                                            if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) < 40)
                                            {
                                                if (CurHP > MaxHP / 4)
                                                {
                                                    if (MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y) <= 6 && (DateTime.Now > LastWalk.AddMilliseconds(RunSpeed) || (speed(this) && DateTime.Now >= LastWalk.AddMilliseconds(100)) && DateTime.Now >= LastJump.AddMilliseconds(300)))
                                                    {
                                                        GetDirection(M.Loc, this);
                                                        Walk(this, M.Loc, 1);
                                                    }
                                                    else if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed + AtkFrequence) /*&& DateTime.Now > C.AtkMem.LastAttack.AddMilliseconds(C.AtkFrequence)*/)
                                                    {
                                                        Vector2 V = DestPart(new Vector2(M.Loc.X, M.Loc.Y), (byte)(Math.Min(8, MyMath.PointDistance(Loc.X, Loc.Y, M.Loc.X, M.Loc.Y)) + 1), 0);
                                                        Jump(this, V.X, V.Y);
                                                    }
                                                }
                                                else
                                                {
                                                    AtkMem.Target = 0;
                                                    if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 7)
                                                    {
                                                        //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                                        //    Thread.Sleep(390);
                                                        X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                                        Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                                        //Jump(C, X, Y);
                                                        try
                                                        {
                                                            Travel(new Vector2(X, Y));
                                                        }
                                                        catch { Jump(this, X, Y); }
                                                    }
                                                }
                                            }
                                            else
                                                AtkMem.Target = 0;
                                        }
                                    }
                                    else
                                        AtkMem.Target = 0;
                                }
                            }
                            else
                                if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 14)
                                {
                                    //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                    //    Thread.Sleep(390);
                                    X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                    Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                    //Jump(C, X, Y);
                                    try
                                    {
                                        Travel(new Vector2(X, Y));
                                    }
                                    catch { Jump(this, X, Y); }
                                }
                                #endregion
                        }
                        else if (Process == "attack")
                        {

                            if (World.H_Chars.Contains(Owner.AtkMem.Target) || (World.H_Chars.Contains(AtkMem.Target) && (AvaLevel > 4)))//AvaLevel 5
                            {
                                if (AtkMem.Target != Owner.EntityID && Owner.AtkMem.Target != 0 && World.H_Chars.Contains(Owner.AtkMem.Target))
                                    AtkMem.Target = Owner.AtkMem.Target;
                                Character pos = (Character)World.H_Chars[Owner.AtkMem.Target];
                                if (pos == null || !pos.Alive || Loc.Map != pos.Loc.Map || Loc.MapDimention != pos.Loc.MapDimention || pos.EntityID == EntityID)
                                    pos = (Character)World.H_Chars[AtkMem.Target];
                                if (pos != null && pos.Alive && Loc.Map == pos.Loc.Map && Loc.MapDimention == pos.Loc.MapDimention && pos.EntityID != EntityID && pos.PKAble(PKMode, this) && MyMath.PointDistance(Loc.X, Loc.Y, pos.Loc.X, pos.Loc.Y) <= 40)
                                {

                                    if (MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 30 && MyMath.PointDistance(Loc.X, Loc.Y, pos.Loc.X, pos.Loc.Y) < 20 && MyMath.PointDistance(pos.Loc.X, pos.Loc.Y, Owner.Loc.X, Owner.Loc.Y) < 35)
                                    {
                                        if (MyMath.PointDistance(Loc.X, Loc.Y, pos.Loc.X, pos.Loc.Y) <= 7)
                                        {

                                            if (MyMath.PointDistance(Loc.X, Loc.Y, pos.Loc.X, pos.Loc.Y) <= 2)
                                            {

                                                if (catched == false)
                                                {
                                                    InRange = DateTime.Now;
                                                    catched = true;
                                                }
                                                if (DateTime.Now > InRange.AddMilliseconds(1000))
                                                    AtkMem.Attacking = true;
                                            }
                                            else
                                            {
                                                catched = false;
                                                AtkMem.Attacking = false;
                                                if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed))
                                                {
                                                    //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))

                                                    X = (ushort)(pos.Loc.X + Program.Rnd.Next(11 - AvaLevel) - Program.Rnd.Next(10 - AvaLevel));
                                                    Y = (ushort)(pos.Loc.Y + Program.Rnd.Next(11 - AvaLevel) - Program.Rnd.Next(10 - AvaLevel));
                                                    Jump(this, X, Y);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed))//&& MyMath.PointDistance(C.Loc.X, C.Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 7 && MyMath.PointDistance(pos.Loc.X, pos.Loc.Y, Owner.Loc.X, Owner.Loc.Y) < 30
                                            {
                                                //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                                //    Thread.Sleep(390);
                                                X = (ushort)(pos.Loc.X + Program.Rnd.Next(11 - AvaLevel) - Program.Rnd.Next(10 - AvaLevel));
                                                Y = (ushort)(pos.Loc.Y + Program.Rnd.Next(11 - AvaLevel) - Program.Rnd.Next(10 - AvaLevel));
                                                try
                                                {
                                                    Travel(new Vector2(X, Y));
                                                }
                                                catch { Jump(this, X, Y); }
                                            }
                                        }
                                        if (AvaLevel >= 1 && (MyMath.PointDistance(Loc.X, Loc.Y, pos.Loc.X, pos.Loc.Y) <= 10))
                                        {
                                            if (catched == false)
                                            {
                                                InRange = DateTime.Now;
                                                catched = true;
                                            }
                                            if (DateTime.Now > InRange.AddMilliseconds(890))
                                            {
                                                catched = false;
                                                if (Skills.Contains((ushort)1045))
                                                {
                                                    Skill s = (Skill)Skills[(ushort)1045];
                                                    UseSkill(1045, s.Lvl, pos.Loc.X, pos.Loc.Y, pos.EntityID);
                                                }
                                                else
                                                {
                                                    if (Skills.Contains((ushort)1046))
                                                    {
                                                        Skill s = (Skill)Skills[(ushort)1046];
                                                        UseSkill(1046, s.Lvl, pos.Loc.X, pos.Loc.Y, pos.EntityID);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 7)
                                        {
                                            //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                            //    Thread.Sleep(390);
                                            X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                            Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                            try
                                            {
                                                Travel(new Vector2(X, Y));
                                            }
                                            catch { Jump(this, X, Y); }
                                        }
                                    }

                                }
                                else
                                {
                                    AtkMem.Target = 0;
                                    Owner.AtkMem.Target = 0;
                                    if (AtkMem.Target.Equals(Owner.AtkMem.Target))
                                        Owner.AtkMem.Attacking = false;
                                    Process = "";
                                }
                            }
                            else
                            {
                                if (Game.World.H_Mobs.Contains(Loc.Map))
                                {
                                    Hashtable MapMobs = (Hashtable)Game.World.H_Mobs[Loc.Map];
                                    if (MapMobs.Contains(Owner.AtkMem.Target) || MapMobs.Contains(AtkMem.Target))
                                    {
                                        PosMob = (Game.Mob)MapMobs[Owner.AtkMem.Target];
                                        if (PosMob == null || !PosMob.Alive)
                                        {
                                            if (AvaLevel >= 2)//AvaLevel 2 def
                                                PosMob = (Game.Mob)MapMobs[AtkMem.Target];
                                        }
                                        else
                                            AtkMem.Target = Owner.AtkMem.Target;

                                        if (PosMob != null && PosMob.Alive)
                                        {

                                            if (MyMath.PointDistance(Loc.X, Loc.Y, PosMob.Loc.X, PosMob.Loc.Y) <= 2)
                                                AtkMem.Attacking = true;
                                            else
                                            {
                                                AtkMem.Attacking = false;
                                                if (MyMath.PointDistance(Loc.X, Loc.Y, PosMob.Loc.X, PosMob.Loc.Y) < 40)
                                                {
                                                    if (CurHP > MaxHP / 4)
                                                    {
                                                        if (MyMath.PointDistance(Loc.X, Loc.Y, PosMob.Loc.X, PosMob.Loc.Y) <= 6 && (DateTime.Now > LastWalk.AddMilliseconds(RunSpeed) || (speed(this) && DateTime.Now >= LastWalk.AddMilliseconds(100)) && DateTime.Now >= LastJump.AddMilliseconds(300)))
                                                        {
                                                            GetDirection(PosMob.Loc, this);
                                                            Walk(this, PosMob.Loc, 1);
                                                        }
                                                        else if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed + AtkFrequence) /*&& DateTime.Now > C.AtkMem.LastAttack.AddMilliseconds(C.AtkFrequence)*/)
                                                        {
                                                            Vector2 V = DestPart(new Vector2(PosMob.Loc.X, PosMob.Loc.Y), (byte)(Math.Min(8, MyMath.PointDistance(Loc.X, Loc.Y, PosMob.Loc.X, PosMob.Loc.Y)) + 1), 0);
                                                            Jump(this, V.X, V.Y);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AtkMem.Target = 0;
                                                        if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) > 7)
                                                        {
                                                            //if (DateTime.Now > C.LastJump.AddMilliseconds(2000))
                                                            //    Thread.Sleep(390);
                                                            X = (ushort)(Owner.Loc.X + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                                            Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(4) - Program.Rnd.Next(4));
                                                            //Jump(C, X, Y);
                                                            try
                                                            {
                                                                Travel(new Vector2(X, Y));
                                                            }
                                                            catch { Jump(this, X, Y); }
                                                        }
                                                        else if (DateTime.Now > LastJump.AddMilliseconds(jumpSpeed) && MyMath.PointDistance(Loc.X, Loc.Y, Owner.Loc.X, Owner.Loc.Y) <= 7)
                                                        {
                                                            X = (ushort)(Owner.Loc.X + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                                            Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(7) - Program.Rnd.Next(7));
                                                            Jump(this, X, Y);
                                                        }
                                                    }
                                                }
                                                else
                                                    AtkMem.Target = 0;
                                            }
                                        }
                                        else
                                        {
                                            AtkMem.Target = 0;
                                            Owner.AtkMem.Target = 0;
                                            if (AtkMem.Target.Equals(Owner.AtkMem.Target))
                                                Owner.AtkMem.Attacking = false;
                                            Process = "";
                                        }

                                    }
                                    else
                                    {
                                        AtkMem.Target = 0;
                                        Owner.AtkMem.Target = 0;
                                        if (AtkMem.Target.Equals(Owner.AtkMem.Target))
                                            Owner.AtkMem.Attacking = false;
                                        Process = "";
                                    }
                                }
                                //else
                                //{
                                //    C.AtkMem.Target = 0;
                                //    Owner.AtkMem.Target = 0;
                                //    if (C.AtkMem.Target.Equals(Owner.AtkMem.Target))
                                //        Owner.AtkMem.Attacking = false;
                                //    C.Process = "";
                                //}
                            }
                        }
                    }
                    if (Loc.MapDimention != Owner.Loc.MapDimention && DateTime.Now > Owner.LastTele.AddMilliseconds(2000))
                    {
                        //Thread.Sleep(1000);
                        Game.World.Action(this, Packets.GeneralData(EntityID, 0, 0, 0, 135));
                        Loc.MapDimention = Owner.Loc.MapDimention;
                        Game.World.Spawns(this, false);
                    }
                    if (Loc.Map != Owner.Loc.Map && DateTime.Now > Owner.LastTele.AddMilliseconds(2000))
                    {
                        //Thread.Sleep(2000);
                        X = (ushort)(Owner.Loc.X + Program.Rnd.Next(9) - Program.Rnd.Next(9));
                        Y = (ushort)(Owner.Loc.Y + Program.Rnd.Next(9) - Program.Rnd.Next(9));

                        Teleport(Owner.Loc.Map, X, Y);
                        Process = "";
                    }
                }
                else
                {
                    AtkMem.Target = 0;
                }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }

    }
}