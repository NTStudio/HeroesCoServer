using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Server.PacketHandling
{
    public class Jump
    {
        public static bool InRange(int rX, int rY, int MyX, int MyY, int Distance)
        {
            return (Math.Max(Math.Abs(rX - MyX), Math.Abs(rY - MyY)) <= Distance);
        }
        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            return (short)Math.Sqrt((X - X2) * (X - X2) + (Y - Y2) * (Y - Y2));
        }
        public static uint maxJumpTime(short distance)
        {
            uint x = 0;
            if (distance == 1)
            {
                x = 2000;
            }
            if (distance == 2)
            {
                x = 2000;
            }
            if (distance == 3)
            {
                x = 4500 * (uint)distance / 10;
            }
            if (distance == 4)
            {
                x = 2000 * (uint)distance / 10;
            }
            if (distance == 5)
            {
                x = 1500 * (uint)distance / 10;
            }
            else
                x = 800 * (uint)distance / 10;
            return x;
        }
        public static Game.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Game.ConquerAngle)(byte)((int)Dir % 8);
        }
        public static bool CanSee(int SeeX, int SeeY, int MyX, int MyY)
        {
            return (Math.Max(Math.Abs(SeeX - MyX), Math.Abs(SeeY - MyY)) <= 18);
        }
        public static void Handle(GameClient GC, byte[] Data)
        {
            /*string P = ""; string Phex = "";
            for (byte bit = 0; bit < Data.Length - 8; bit++)
            {
                int Pi = Data[bit];
                P += Data[bit] + " ";
                Phex += Pi.ToString("X") + " ";
            }
            Console.WriteLine("packet: {0} ", P);*/
            ushort Previos_X = BitConverter.ToUInt16(Data, 20);
            ushort Previos_y = BitConverter.ToUInt16(Data, 22);
            ushort new_X = (ushort)(BitConverter.ToUInt32(Data, 8) & 0xFFFF);
            ushort new_Y = (ushort)(BitConverter.ToUInt32(Data, 10) & 0xFFFF);//(ushort)(BitConverter.ToUInt32(Data, 8) >> 16);
            //Console.WriteLine("X {0} and Y{1}", new_X, new_Y);
            byte[] Jump = Packets.GeneralData(GC.MyChar.EntityID, (ushort)new_X, (ushort)new_Y, (ushort)Previos_X, (ushort)Previos_y, 0, 137);
            GC.SendPacket(Jump);
            if (!CanSee(Previos_X, Previos_y, new_X, new_Y))
            {
                GC.Disconnect();
                return;
            }
            foreach (DictionaryEntry DE in Game.World.H_Chars)
            {
                Game.Character C = (Game.Character)DE.Value;
                {
                    if (C != GC.MyChar)
                        if (GC.MyChar.Loc.Map == C.Loc.Map && GC.MyChar.Loc.MapDimention == C.Loc.MapDimention)
                            if (CanSee(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, C.Loc.X, C.Loc.Y))
                                C.MyClient.SendPacket2(Data);
                }
            }
            GC.MyChar.lastJumpDistance = GetDistance((ushort)new_X, (ushort)new_Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y);
            GC.MyChar.lastJumpTime = DateTime.Now;
            if (GetDistance((ushort)new_X, (ushort)new_Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y) == 1)
            {
                GC.MyChar.lastJumpTime = DateTime.Now.AddMilliseconds(1000);
            }
            if (GetDistance((ushort)new_X, (ushort)new_Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y) == 2)
            {
                GC.MyChar.lastJumpTime = DateTime.Now.AddMilliseconds(800);
            }
            else
                GC.MyChar.lastJumpTime = DateTime.Now.AddMilliseconds(maxJumpTime(GetDistance((ushort)new_X, (ushort)new_Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y)));
            //Console.WriteLine("distance: " + GetDistance((ushort)new_X, (ushort)new_Y, GC.MyChar.Loc.X, GC.MyChar.Loc.Y));
            GC.MyChar.Mining = false;
            GC.MyChar.AtkMem.Attacking = false;
            GC.MyChar.Protection = false;
            
            GC.MyChar.Action = (byte)Game.ConquerAction.Jump;
            GC.MyChar.Loc.PreviousX = (ushort)Previos_X;
            GC.MyChar.Loc.PreviousY = (ushort)Previos_y;
            GC.MyChar.Loc.X = (ushort)new_X;
            GC.MyChar.Loc.Y = (ushort)new_Y;
            Game.World.Spawns(GC.MyChar, true);
            if (GC.MyChar.BuffOf(Server.Skills.SkillsClass.ExtraEffect.BlessPray).Eff == Server.Skills.SkillsClass.ExtraEffect.BlessPray)
                GC.MyChar.RemoveBuff(GC.MyChar.BuffOf(Skills.SkillsClass.ExtraEffect.BlessPray));
            if (GC.MyChar.StatEff.Contains(Server.Game.StatusEffectEn.Ride))
            {
                if (GC.MyChar.Vigor >= 10)
                    GC.MyChar.Vigor -= 10;
                else return;
            }
            if (GC.MyChar.InteractionInProgress && GC.MyChar.InteractionSet)
            {
                if (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004)
                {
                    if (Game.World.H_Chars.ContainsKey(GC.MyChar.InteractionWith))
                    {

                        Game.Character character = Game.World.H_Chars[GC.MyChar.InteractionWith] as Game.Character;
                        character.MyClient.SendPacket(Packets.GeneralData(character.EntityID, 0, BitConverter.ToUInt16(Data, 8), BitConverter.ToUInt16(Data, 10), 0x9c));
                        //  character.Loc.Jump(BitConverter.ToUInt16(Data, 8), BitConverter.ToUInt16(Data, 10));
                        character.Action = (byte)Game.ConquerAction.Jump;
                        character.Mining = false;
                        character.AtkMem.Attacking = false;
                        
                        character.Loc.X = (ushort)new_X;
                        character.Loc.Y = (ushort)new_Y;
                        character.Direction = (byte)GetAngle(BitConverter.ToUInt16(Data, 20), BitConverter.ToUInt16(Data, 22), (ushort)new_X, (ushort)new_Y);
                        Game.World.Action(character, Data);
                        Game.World.Spawns(character, true);

                    }
                }
            }
          //GC.SendPacket2(Packets.TheA4(GC.MyChar));//its qounqer 
          //GC.SendPacket2(Packets.The5604(GC.MyChar));
        }
    }
}
