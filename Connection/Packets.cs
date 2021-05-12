using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Server
{

    public class COPacket
    {
        byte[] PData = new byte[1024];
        public ushort PType;
        int Count;
        protected byte[] TQ_SERVER = Encoding.ASCII.GetBytes("TQServer");

        public byte[] Ptr
        {
            get { return PData; }
        }
        public byte[] Get
        {
            get
            {
                return PData;
            }
        }
        public COPacket(byte[] _Data)
        {
            Count = 0;
            PData = _Data;
        }
        public void WriteByteAddPos1(byte val)
        {
            try
            {
                Ptr[Count] = (byte)(val & 0xff);
                Count += 1;
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public unsafe void WriteUshortAddPos2(ushort val)
        {
            try
            {
                Ptr[Count] = (byte)(val & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 8) & 0xff);
                Count += 1;
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public unsafe void WriteUintAddPos4(uint val)
        {
            try
            {
                Ptr[Count] = (byte)(val & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 8) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 16) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 24) & 0xff);
                Count += 1;
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }

        }
        public unsafe void WriteUlongAddPos8(ulong val)
        {
            try
            {
                Ptr[Count] = (byte)(val & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 8) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 16) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 24) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 32) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 40) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 48) & 0xff);
                Count += 1;
                Ptr[Count] = (byte)((val >> 56) & 0xff);
                Count += 1;
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public void WriteString(string val)
        {
            try
            {
                if (val != null)
                {
                    for (int i = 0; i < val.Length; i++)
                    {
                        try
                        {
                            Ptr[Count] = Convert.ToByte(val[i]); //Count += 1;
                        }
                        catch (Exception e) { Program.WriteMessage(e.ToString()); }//For weird letters that cannot be converted into byte...
                        Count += 1;// Count++;
                    }
                }
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public void WriteString(string val, int MaxLength)
        {
            if (val.Length <= MaxLength)
                for (int i = 0; i < val.Length; i++)
                {
                    Ptr[Count] = Convert.ToByte(val[i]);
                    Count += 1;
                }
            else
                for (int i = 0; i < MaxLength; i++)
                {
                    Ptr[Count] = Convert.ToByte(val[i]);
                    Count += 1;
                }
        }
        public void WriteBytes(byte[] val)
        {
            try
            {
                for (int i = 0; i < val.Length; i++)
                {
                    Ptr[Count] = val[i];
                    Count += 1;
                }
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public void Move(int count)
        {
            try
            {
                Count += count;
            }
            catch (Exception e)
            {
#if Debug
                Program.WriteMessage(e.ToString();
                                Count = 0;
                PData = new byte[1024];
                return;
#endif
                Program.WriteMessage(e.ToString());
            }
        }
        public void Addbytes()
        {
            string s = "TQServer";
            ASCIIEncoding encoding = new ASCIIEncoding();
            encoding.GetBytes(s).CopyTo(PData, (int)(PData.Length - 8));
        }
        public byte[] AddTQServer8Byte()
        {
            Addbytes();
            return PData;
        }
    }
    public enum PacketType : ushort
    {
        NobilityPacket,
        Packet2

    }

    public class Packets
    {
        public static byte[] TheA4(Game.Character Char)
        {
            byte[] PacketData = new byte[0x18];
            uint Timer = (uint)Environment.TickCount;
            PacketData[0x0] = 0x18;
            PacketData[0x1] = 0x00;
            PacketData[0x2] = 0xf2;
            PacketData[0x3] = 0x03;
            PacketData[0x4] = (byte)(Timer & 0xff);
            PacketData[0x5] = (byte)((Timer >> 8) & 0xff);
            PacketData[0x6] = (byte)((Timer >> 16) & 0xff);
            PacketData[0x7] = (byte)((Timer >> 24) & 0xff);
            PacketData[0x8] = (byte)(Char.EntityID & 0xff);
            PacketData[0x9] = (byte)((Char.EntityID >> 8) & 0xff);
            PacketData[0xa] = (byte)((Char.EntityID >> 16) & 0xff);
            PacketData[0xb] = (byte)((Char.EntityID >> 24) & 0xff);
            PacketData[0xc] = 0xff;
            PacketData[0xd] = 0xff;
            PacketData[0xe] = 0xff;
            PacketData[0xf] = 0xff;
            PacketData[0x10] = (byte)(Char.Loc.X & 0xff);
            PacketData[0x11] = (byte)((Char.Loc.X >> 8) & 0xff);
            PacketData[0x12] = (byte)(Char.Loc.Y & 0xff);
            PacketData[0x13] = (byte)((Char.Loc.Y >> 8) & 0xff);
            PacketData[0x14] = 0x00;
            PacketData[0x15] = 0x00;
            PacketData[0x16] = 0x68;
            PacketData[0x17] = 0x00;
            return PacketData;

            //OLD
            //1c 00 f2 03 4c 11 e2 04 57 1d 12 00 5d 00 57 00   ..?L.?W...].W.
            //00 00 00 00 ff ff ff ff a4 00 00 00               ....ÿÿÿÿ?..

            //NEW
            //18 00 f2 03 8e 3a e5 04 58 04 15 00 ff ff ff ff  ; .....:..X.......
            //df 01 e1 01 00 00 68 00                          ; ......h.


        }

        public static byte[] The5604(Game.Character Char)
        {
            byte[] PacketData = new byte[0x10];
            PacketData[0] = 0x10;
            PacketData[1] = 0x00;
            PacketData[2] = 0x56;
            PacketData[3] = 0x04;
            PacketData[4] = (byte)((uint)Char.Loc.Map & 0xff);
            PacketData[5] = (byte)(((uint)Char.Loc.Map >> 8) & 0xff);
            PacketData[6] = 0x00;
            PacketData[7] = 0x00;
            PacketData[8] = (byte)((uint)Char.Loc.Map & 0xff);
            PacketData[9] = (byte)(((uint)Char.Loc.Map >> 8) & 0xff);
            PacketData[10] = 0x00;
            PacketData[11] = 0x00;
            PacketData[12] = 0x00;
            PacketData[13] = 0x00;
            PacketData[14] = 0x00;
            PacketData[15] = 0x00;
            return PacketData;
            //10 00 56 04 f8 07 00 00 dc 05 00 00 02 08 00 00   ..V.?..?......
        }

        public unsafe static byte[] Packet(PacketType Type,Game.Character C)
        {
            byte[] PacketData = null;
      
            switch (Type)
            {
                case PacketType.NobilityPacket:
                    {
                        string str = C.EntityID.ToString() + " " + C.Nobility.Donation.ToString() + " " + ((byte)C.Nobility.Rank).ToString() + " " + C.Nobility.ListPlace.ToString();
                        PacketData = new byte[8 + 33 + str.Length];

                        PacketData[0] = (byte)((ushort)PacketData.Length - 8 & 0xff);
                        PacketData[1] = (byte)(((ushort)PacketData.Length - 8 >> 8) & 0xff);
                        PacketData[2] = (byte)((ushort)2064 & 0xff);
                        PacketData[3] = (byte)(((ushort)2064 >> 8) & 0xff);

                        PacketData[4] = (byte)((byte)3 & 0xff);
                        PacketData[5] = 0x00;
                        PacketData[6] = 0x00;
                        PacketData[7] = 0x00;


                        PacketData[8] = (byte)((uint)C.EntityID & 0xff);
                        PacketData[9] = (byte)(((uint)C.EntityID >> 8) & 0xff);
                        PacketData[10] = (byte)(((uint)C.EntityID >> 16) & 0xff);
                        PacketData[11] = (byte)(((uint)C.EntityID >> 24) & 0xff);

                        PacketData[12] = 0x00;
                        PacketData[13] = 0x00;
                        PacketData[14] = 0x00;
                        PacketData[15] = 0x00;
                        PacketData[16] = 0x00;
                        PacketData[17] = 0x00;
                        PacketData[18] = 0x00;
                        PacketData[19] = 0x00;

                        PacketData[20] = 0x00;
                        PacketData[21] = 0x00;
                        PacketData[22] = 0x00;
                        PacketData[23] = 0x00;
                        PacketData[24] = 0x00;
                        PacketData[25] = 0x00;
                        PacketData[26] = 0x00;
                        PacketData[27] = 0x00;

                        PacketData[28] = 0x01;

                        PacketData[29] = (byte)((byte)str.Length & 0xff);
                        for (int i = 0; i < str.Length; i++)
                        {
                            try
                            {
                               PacketData[30 + i] = Convert.ToByte(str[i]); //Count += 1;
                            }
                            catch { }
                        }
                        string s = "TQServer";
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        encoding.GetBytes(s).CopyTo(PacketData, (int)(PacketData.Length - 8));

                        return PacketData;
                    }
            }
            return PacketData;
        }
        public static byte[] OffLineTgScreen(ushort HowLong, byte Typ)
        {
            byte[] Packet = new byte[8 + 18];//12
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)2043);
            P.WriteUshortAddPos2(Typ);
            P.WriteUshortAddPos2(600);
            P.WriteUintAddPos4(HowLong);

           return P.AddTQServer8Byte();
        }
        public static byte[] OffLineTg(short HowLong, byte Typ)
        {
            byte[] Packet = new byte[8 + 12];//12
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)2044);
            P.WriteUintAddPos4(Typ);
            P.WriteUintAddPos4((uint)HowLong);
            return P.AddTQServer8Byte();
        }
        public static byte[] ItemLock(uint ItemID, byte Value1, byte Value2, uint Value3)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)2048);
            P.WriteUintAddPos4(ItemID);
            P.WriteByteAddPos1(Value1);
            P.WriteByteAddPos1(Value2);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(Value3);
           return P.AddTQServer8Byte();
        }
        public static byte[] DonateOpen(Game.Character C)
        {
            byte[] Packet = new byte[32 + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//length
            P.WriteUshortAddPos2(2064);
            P.WriteUintAddPos4(4);
            P.WriteUintAddPos4(12);
            return P.AddTQServer8Byte();
        }
        public static byte[] DonateOpen2(Game.Character C)
        {
            byte[] Packet = new byte[32 + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//length
            P.WriteUshortAddPos2(2064);
            P.WriteUintAddPos4(4);
            if (C.Nobility.Rank < Game.Ranks.Duke)
                P.WriteUintAddPos4((uint)(Game.World.EmpireBoard[49].Donation + 1));
            else
            {
                if (C.Nobility.Rank == Server.Game.Ranks.Duke)
                    P.WriteUintAddPos4((uint)(Game.World.EmpireBoard[15].Donation + 1));
                else if (C.Nobility.Rank == Server.Game.Ranks.Prince)
                    P.WriteUintAddPos4((uint)(Game.World.EmpireBoard[3].Donation + 1));
                else
                    P.WriteUintAddPos4(0);
            }
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(60);
            P.WriteUintAddPos4(uint.MaxValue);
            return P.AddTQServer8Byte();
        }
        public static byte[] SendTopDonaters(uint Page)
        {
            string Str = "";
            for (int i = (int)(Page * 10); i < Page * 10 + 10; i++)
            {
                if (Game.World.EmpireBoard[i].Donation != 0)
                {
                    int PotGet = 7;
                    if (i < 15) PotGet = 9;
                    if (i < 3) PotGet = 12;

                    string nStr = Game.World.EmpireBoard[i].ID + " 0 0 " + Game.World.EmpireBoard[i].Name + " " + Game.World.EmpireBoard[i].Donation + " " + PotGet + " " + i;
                    nStr = Convert.ToChar((byte)nStr.Length) + nStr;
                    Str += nStr;
                }
            }
            byte[] Packet = new byte[32 + Str.Length + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//length
            P.WriteUshortAddPos2(2064);
            P.WriteUintAddPos4(2);
            P.WriteUshortAddPos2((ushort)Page);
            P.WriteUintAddPos4(5);
            P.WriteUlongAddPos8(0);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            P.WriteByteAddPos1(10);
            P.WriteString(Str);
            P.WriteByteAddPos1(0);
            P.WriteUshortAddPos2(0);
           return P.AddTQServer8Byte();
        }
        public static byte[] QuizShowStart(ushort qCount)
        {
            byte[] Packet = new byte[20 + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2(20);
            P.WriteUshortAddPos2(2068);
            P.WriteUshortAddPos2(1);//quiztype
            P.WriteUshortAddPos2(31);//countdown
            P.WriteUshortAddPos2(qCount);//questioncount
            P.WriteUshortAddPos2(30);//questiontime
            P.WriteUshortAddPos2(1800);//1st prize
            P.WriteUshortAddPos2(1200);//2nd prize
            P.WriteUshortAddPos2(600);//3rdprize
            return P.AddTQServer8Byte();
        }
        public static byte[] QuizQuestion(uint currentscore, ushort timetaken, ushort prize, ushort rlq, ushort qn, string question, string answer1, string answer2, string answer3, string answer4)
        {
            byte[] packet = new byte[19 + question.Length + 1 + answer1.Length + 1 + answer2.Length + 1 + answer3.Length + 1 + answer4.Length + 1 + 8];
            COPacket Packet = new COPacket(packet);
            Packet.WriteUshortAddPos2((ushort)(packet.Length - 8));//length
            Packet.WriteUshortAddPos2(2068);//packettype
            Packet.WriteUshortAddPos2(2);//quiztype
            Packet.WriteUshortAddPos2(qn);//questionid
            Packet.WriteUshortAddPos2(0);//last question right answer
            Packet.WriteUshortAddPos2(prize);//prize so far
            Packet.WriteUshortAddPos2(timetaken);//time taken so far
            Packet.WriteUintAddPos4(currentscore);//current score
            Packet.WriteByteAddPos1(5);
            char length = (char)question.Length;
            Packet.WriteString(length.ToString());
            Packet.WriteString(question);
            Packet.WriteByteAddPos1((byte)answer1.Length);
            Packet.WriteString(answer1);
            Packet.WriteByteAddPos1((byte)answer2.Length);
            Packet.WriteString(answer2);
            Packet.WriteByteAddPos1((byte)answer3.Length);
            Packet.WriteString(answer3);
            Packet.WriteByteAddPos1((byte)answer4.Length);
            Packet.WriteString(answer4);
            return Packet.AddTQServer8Byte();
        }
        public static byte[] AddStallItem(Game.Item I, PacketHandling.MarketShops.ItemValue Val, uint StallID)
        {
            byte[] Packet = new byte[8 + 56];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x454);
            P.WriteUintAddPos4(I.UID);
            P.WriteUintAddPos4(StallID);
            P.WriteUintAddPos4(Val.Value);
            P.WriteUintAddPos4(I.ID);
            P.WriteUshortAddPos2(I.CurDur);
            P.WriteUshortAddPos2(I.MaxDur);
            P.WriteUintAddPos4(Val.MoneyType);
            P.WriteUintAddPos4(0);
            P.WriteByteAddPos1((byte)I.Soc1);
            P.WriteByteAddPos1((byte)I.Soc2);
            P.WriteUshortAddPos2((ushort)I.Effect);
            P.WriteByteAddPos1(I.Plus);
            P.WriteByteAddPos1(I.Bless);
            if (I.FreeItem)
                P.WriteByteAddPos1(1);
            else
                P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(I.Enchant);
            P.WriteUintAddPos4(I.TalismanProgress);
            //P.Move(6);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            if (I.Locked == 1)
                P.WriteUshortAddPos2(1);
            else if (I.Locked == 2)
                P.WriteUshortAddPos2(1);
            else
                P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4((uint)I.Color);
            P.WriteUintAddPos4(I.Progress);

            return P.AddTQServer8Byte();
        }
        public static byte[] PingTest(Game.Character C)
        {
            byte[] Packet = new byte[32 + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2(1012);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteUintAddPos4(0);
            for (int i = 0; i < 16; i++)
                P.WriteByteAddPos1((byte)Program.Rnd.Next(255));
            P.WriteUintAddPos4(0);
            return P.AddTQServer8Byte();
        }
        public static byte[] RemoveItemClain(Game.Character C,Game.Item I)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            Console.WriteLine("trece");
            P.WriteUshortAddPos2((ushort)1035);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(2);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteUintAddPos4(I.UID);//3323122);
            P.WriteByteAddPos1(4);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteUintAddPos4(I.UID);//3323122);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteString("TQServer");
            
            return P.AddTQServer8Byte();
            /*1C 00 0B 04 01 00 00 00 02 00 00 00 5F 33 04 00             ............_3..
            04 00 00 00 D2 65 04 00 00 00 00 00 54 51 53 65             .....e......TQSe
            72 76 65 72*/
        }
        public static byte[] RemoveItemClain2(Game.Character C, Game.Item I)
        {
            byte[] Packet = new byte[8 + 28];//20
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1035);
            P.WriteUintAddPos4(1);
            P.WriteUintAddPos4(0);//1
            P.WriteUintAddPos4(I.UID);
            P.WriteUshortAddPos2(4);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(I.UID);
            //P.Move(4);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
   
        }

       public static byte[] RemoveItemClain2(Game.Item I,uint val ,uint val2)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            P.WriteUshortAddPos2((ushort)1035);
            P.WriteUintAddPos4(val);
            P.WriteUintAddPos4(val2);
            P.WriteUintAddPos4(I.UID);
            P.Move(4);
            //P.WriteString("TQServer");

            /*
             * 14 00 0B 04 00 00 00 00 01 00 00 00 EE 65 04 00             .............e..
             * 00 00 00 00 54 51 53 65 72 76 65 72                         ....TQServer
            */
            return P.AddTQServer8Byte();
        }
        public static byte[] ConfiscatorClain(Game.Item I, Game.Character C, ushort Cps, string Name)
        {

            byte[] Packet = new byte[8 + 104];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            P.WriteUshortAddPos2((ushort)1034);//4
            P.WriteUintAddPos4(I.UID);//12
            P.WriteUintAddPos4(66);//8
            P.WriteUintAddPos4(I.ID);//16
            P.WriteUshortAddPos2(I.CurDur);//18
            P.WriteUshortAddPos2(I.MaxDur);//20
            P.WriteByteAddPos1(0x01);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(204);//204 0xCC
            P.WriteByteAddPos1(0);
            //P.WriteUshortAddPos2(0);//22
            //P.WriteUshortAddPos2(0);//24
            P.WriteUintAddPos4(I.TalismanProgress);////28
            P.WriteByteAddPos1((byte)I.Soc1);//29
            P.WriteByteAddPos1((byte)I.Soc2);//30
            P.WriteUshortAddPos2((ushort)I.Effect);//32
            P.WriteByteAddPos1(I.Plus);//31
            P.WriteByteAddPos1(I.Bless);//32
            if (I.FreeItem == true)
                P.WriteByteAddPos1(1);//33
            else P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(I.Enchant);//34
            //P.Move(6);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            if (I.Locked == 1 || I.Locked == 2)
                P.WriteUshortAddPos2(1);//42
            else
                P.WriteUshortAddPos2(0);
            if (I.Color == 0)
                I.Color = (Game.Item.ArmorColor)new Random().Next(1, 9);
            P.WriteUintAddPos4((ushort)I.Color);//46
            P.WriteUintAddPos4(I.Progress);//50
            //P.Move(20);
            //P.WriteUlongAddPos8(0);
            //P.WriteUlongAddPos8(0);
            P.WriteString(Name);//70 + name.leath
            P.Move(16 - Name.Length);

         //   P.WriteUintAddPos4(0);//C7 8C E7 00
            P.WriteByteAddPos1(0xC7);
            P.WriteByteAddPos1(0x8C);
            P.WriteByteAddPos1(0xE7);
            P.WriteByteAddPos1(0x00);

            P.WriteString(Name);//70 + name.leath
            P.Move(16 - Name.Length);
            //  P.Move(6);
            P.WriteUintAddPos4(20100511);//46495
            //P.Move(4);
            P.WriteUintAddPos4(0);//309?
            P.WriteUshortAddPos2(Cps);
            //P.Move(2);
            P.WriteUshortAddPos2(970);//970
            P.WriteUintAddPos4(8);//7-6 == 1 day...... 7-3 =4 days
            //P.WriteUlongAddPos8(2);//time ... no sure 2
            //    P.WriteString("TQServer");
           /*68 00 0A 04 2F 66 04 00 91 E0 1E 03 DB 6D 06 00             h.../f.......m..
            C7 11 F7 11 01 00 CC 00 00 00 00 00 03 03 C9 00             ................
            0C 00 00 DC 00 00 00 00 00 00 01 00 00 00 00 00             ................
            75 A0 E7 00 73 65 72 67 73 00 00 00 00 00 00 00             u...sergs.......
            00 00 00 00 C7 8C E7 00 43 72 69 73 74 79 61 6E             ........Cristyan
            00 00 00 00 00 00 00 00 9F B5 32 01 00 00 00 00             ..........2.....
            CA 03 00 00 00 00 00 00 54 51 53 65 72 76 65 72             ........TQServer
             * */
            return P.AddTQServer8Byte();
        }
        public static byte[] ConfiscatorReward(Game.Item I, Game.Character C, ushort Cps, string name)
        {

            byte[] Packet = new byte[8 + 104 + (C.Name + C.MyClient.AuthInfo.Status).Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            P.WriteUshortAddPos2((ushort)1034);//4
            P.WriteUintAddPos4(I.UID);//12
            P.WriteUintAddPos4(6);//8
            P.WriteUintAddPos4(I.ID);//16
            P.WriteUshortAddPos2(I.CurDur);//18
            P.WriteUshortAddPos2(I.MaxDur);//20
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(204);//204
            P.WriteByteAddPos1(0);
            //P.WriteUshortAddPos2(0);//22
            //P.WriteUshortAddPos2(0);//24
            P.WriteUintAddPos4(I.TalismanProgress);////28
            P.WriteByteAddPos1((byte)I.Soc1);//29
            P.WriteByteAddPos1((byte)I.Soc2);//30
            P.WriteUshortAddPos2((ushort)I.Effect);//32
            P.WriteByteAddPos1(I.Plus);//31
            P.WriteByteAddPos1(I.Bless);//32
            if (I.FreeItem == true)
                P.WriteByteAddPos1(1);//33
            else P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(I.Enchant);//34
            //P.Move(6);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            if (I.Locked == 1 || I.Locked == 2)
                P.WriteUshortAddPos2(1);//42
            else
                P.WriteUshortAddPos2(0);
            if (I.Color == 0)
                I.Color = (Game.Item.ArmorColor)new Random().Next(1, 9);
            P.WriteUintAddPos4((ushort)I.Color);//46
            P.WriteUintAddPos4(I.Progress);//50
           // P.Move(20);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);
            P.WriteString(name);//70 + name.leath
            P.Move(16 - name.Length);
            //  P.Move(6);
            P.WriteUintAddPos4(2);
            //P.Move(4);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(Cps);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(4);//7-6 == 1 day...... 7-3 =4 days
            //P.WriteUlongAddPos8(2);//time ... no sure 2
            //    P.WriteString("TQServer");
            /*68 00 0A 04 EE 65 04 00 36 FC B8 02 E3 42 06 00             h....e..6....B..
            61 10 F7 11 00 00 CC 00 00 00 00 00 0D 0D C8 00             a...............
            0C 07 00 60 00 00 00 00 00 00 00 00 00 00 00 00             ...`............
            C7 8C E7 00 43 72 69 73 74 79 61 6E 00 00 00 00             ....Cristyan....
            00 00 00 00 AC F1 E6 00 78 58 69 6D 70 61 63 74             ........xXimpact
            58 78 00 00 00 00 00 00 9F B5 32 01 00 00 00 00             Xx........2.....
            CA 03 00 00 00 00 00 00 54 51 53 65 72 76 65 72             ........TQServer
             * */
            return P.AddTQServer8Byte();

        }
        public static byte[]  Sac(Game.Item I, Game.Character C, ushort Cps, string name)
        {

            byte[] Packet = new byte[8 + 104 + (C.Name + C.MyClient.AuthInfo.Status).Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            Console.WriteLine("packet lenght is " + Packet.Length);
            P.WriteUshortAddPos2((ushort)1034);//4
            P.WriteUintAddPos4(I.UID);//12
            P.WriteUintAddPos4(3);//npcid pos to remove item
            //P.Move(8);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(2);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            //P.Move(24);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            //P.WriteUintAddPos4(0);
            //27 EF E6 00
            P.WriteByteAddPos1(0x27);
            P.WriteByteAddPos1(0xEF);
            P.WriteByteAddPos1(0xE6);
            P.WriteByteAddPos1(0x00);
            P.WriteString(name);//kill name
            P.Move(16 - name.Length);
            //0xC7 , 0x8C , 0xE7 , 0x00 ,
            //P.WriteUintAddPos4(I.UID);//15174855);
            P.WriteByteAddPos1(0xC7);
            P.WriteByteAddPos1(0x8C);
            P.WriteByteAddPos1(0xE7);
            P.WriteByteAddPos1(0x00);
            P.WriteString(C.Name + C.MyClient.AuthInfo.Status);// my name
            //P.Move(8);
            //0x9F , 0xB5 , 0x32 , 0x01 , 0xF6 , 0x04 , 0x00 , 0x00
            P.Move(16 - (C.Name + C.MyClient.AuthInfo.Status).Length);//C.Name.Length);
            //P.WriteUlongAddPos8(340933129631);//340933129631);
            //P.Move(4);
            P.WriteByteAddPos1(0x9F);
            P.WriteByteAddPos1(0xB5);
            P.WriteByteAddPos1(0x32);
            P.WriteByteAddPos1(0x01);
            P.WriteByteAddPos1(0xF6);
            P.WriteByteAddPos1(0x04);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(0);
            P.WriteUshortAddPos2(Cps);
           // P.Move(6);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            P.WriteString("TQServer");


            return P.AddTQServer8Byte();
            /*
            68 00 0A 04 5F 33 04 00 FB 0E 6D 04 00 00 00 00             h..._3....m.....
            00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00             ................
            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00             ................
            EB 83 E8 00 A5 3F 75 4C 3F A7 86 00 00 00 00 00             .....?uL?.......
            00 00 00 00 C7 8C E7 00 43 72 69 73 74 79 61 6E             ........Cristyan
            00 00 00 00 00 00 00 00 9B B5 32 01 F6 04 00 00             ..........2.....
            F6 04 00 00 04 00 00 00 54 51 53 65 72 76 65 72             ........TQServer
            */

        }
        public static byte[] Weather(Features.WeatherType Type, uint Intensity, uint Appearence, uint Direction)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f8);
            P.WriteUintAddPos4((byte)Type);
            P.WriteUintAddPos4(Intensity);
            P.WriteUintAddPos4(Direction);
            P.WriteUintAddPos4(Appearence);
            return P.AddTQServer8Byte();
        }
        public static byte[] MapStatus(ushort Map, uint Status)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x456);
            P.WriteUintAddPos4(Map);
            P.WriteUintAddPos4(Map);
            P.WriteUintAddPos4(Status);

            return P.AddTQServer8Byte();
        }
        public static byte[] FriendEnemyInfo(Game.Character C, byte Enemy)
        {
            int Leng = (16 - C.Spouse.Length);
            byte[] Packet = new byte[8 + 40];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x7f1);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteUintAddPos4(uint.Parse(C.Avatar.ToString() + C.Body.ToString()));
            P.WriteByteAddPos1(C.Level);
            P.WriteByteAddPos1(C.Job);
            P.WriteUshortAddPos2(C.PKPoints);
            if (C.MyGuild != null)
                P.WriteUshortAddPos2(C.MyGuild.GuildID);
            else
                P.WriteUshortAddPos2(0);
               // P.Move(2);

            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1((byte)(C.GuildRank));
            P.WriteString(C.Spouse);
            for (int i = 0; i < Leng; i++)
            {
                P.WriteByteAddPos1(0);
            }
            P.WriteUintAddPos4(Enemy);

            return P.AddTQServer8Byte();
        }
        public static byte[] TradePartnerInfo(Game.TradePartner Partner)
        {
            byte[] packet = new byte[48 + Partner.Info.Spouse.Length];
            COPacket p = new COPacket(packet);
            p.WriteUshortAddPos2((ushort)(40 + Partner.Info.Spouse.Length));
            p.WriteUshortAddPos2(2047);
            p.WriteUintAddPos4(Partner.UID);
            p.WriteUintAddPos4(Partner.Info.Mesh);
            p.WriteByteAddPos1(Partner.Info.Level);
            p.WriteByteAddPos1(Partner.Info.Job);
            p.WriteUshortAddPos2(Partner.Info.PKPoints);
            if (Partner.Info.MyGuild != null)
                p.WriteUshortAddPos2(Partner.Info.MyGuild.GuildID);
            else
                p.WriteUshortAddPos2(0);
            p.WriteByteAddPos1(0);
            p.WriteByteAddPos1((byte)Partner.Info.GuildRank);
            p.WriteString(Partner.Info.Spouse);

            return p.AddTQServer8Byte();

        }
        public static byte[] TradePartner(string Name, uint UID, bool Online, DateTime ProbationStartedOn, byte Type)
        {
            byte[] packet = new byte[24 + Name.Length];
            COPacket p = new COPacket(packet);
            p.WriteUshortAddPos2((ushort)(16 + Name.Length));
            p.WriteUshortAddPos2(2046);
            p.WriteUintAddPos4(UID);
            p.WriteByteAddPos1(Type);
            p.WriteByteAddPos1(Online == true ? (byte)1 : (byte)0);
            TimeSpan finishon = new TimeSpan(ProbationStartedOn.AddDays(3).Ticks);
            int hoursleft = (int)finishon.TotalHours;
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            hoursleft -= (int)now.TotalHours;
            if (hoursleft < 0)
                hoursleft = 0;
            p.WriteUintAddPos4((uint)(hoursleft * 60));
            p.WriteUshortAddPos2(0);
            p.WriteString(Name);

            return p.AddTQServer8Byte();

        }
        public static byte[] FriendEnemyPacket(uint uid, string name, byte Mode, byte Online)
        {
            //modes 15 friends to game 14 come to game
            //mode 19 enemy to game 18 come to game
            byte[] Packet = new byte[24 + name.Length + 8];//36
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8 ));
            P.WriteUshortAddPos2((ushort)0x3fb);
            P.WriteUintAddPos4(uid);
            P.WriteByteAddPos1(Mode);
            P.WriteByteAddPos1(Online);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(1);
            P.WriteString(name);
//            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] TradePacket(uint UID, byte Type)
        {
            byte[] Packet = new byte[8 + 12];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x420);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(Type);

            return P.AddTQServer8Byte();
        }
        public static byte[] TradeItem(Game.Item I)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f0);
            P.WriteUintAddPos4(I.UID);
            P.WriteUintAddPos4(I.ID);
            P.WriteUshortAddPos2(I.CurDur);
            P.WriteUshortAddPos2(I.MaxDur);
            P.WriteUshortAddPos2(2);
            P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(I.TalismanProgress);
            P.WriteByteAddPos1((byte)I.Soc1);
            P.WriteByteAddPos1((byte)I.Soc2);
            P.WriteUshortAddPos2((ushort)I.Effect);
            P.WriteByteAddPos1(I.Plus);
            P.WriteByteAddPos1(I.Bless);
            if (I.FreeItem)
                P.WriteByteAddPos1(1);
            else
                P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(I.Enchant);
            P.WriteUintAddPos4(0); P.WriteUshortAddPos2(0);//P.Move(6);
            if (I.Locked == 1)
                P.WriteUshortAddPos2(1);
            else if (I.Locked == 2)
                P.WriteUshortAddPos2(1);
            else
                P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4((uint)I.Color);
            P.WriteUintAddPos4(I.Progress);

            return P.AddTQServer8Byte();
        }
        public static byte[] AddWHItem(Game.Character C, ushort NPC, Game.Item I)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44e);
            P.WriteUintAddPos4(NPC);
            P.WriteUintAddPos4(0);// P.Move(4);
            P.WriteUintAddPos4(1);
            if (I.ID != 0)
            {
                P.WriteUintAddPos4(I.UID);
                P.WriteUintAddPos4(I.ID);
                P.WriteByteAddPos1(0);// P.Move(1);
                P.WriteByteAddPos1((byte)I.Soc1);
                P.WriteByteAddPos1((byte)I.Soc2);
                P.WriteUshortAddPos2((ushort)I.Effect);
                P.WriteByteAddPos1(I.Plus);
                P.WriteByteAddPos1(I.Bless);
                if (I.FreeItem)
                    P.WriteByteAddPos1(1);
                else
                    P.WriteByteAddPos1(0);//P.Move(1);
                P.WriteByteAddPos1(I.Enchant);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1((byte)I.Color);
                P.WriteUintAddPos4(I.TalismanProgress);
            }
            return P.AddTQServer8Byte();
        }
        public static byte[] RemoveWHItem(Game.Character C, ushort NPC, Game.Item I)
        {
            byte[] Packet = new byte[48 + 8];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44e);
            P.WriteUintAddPos4(NPC);
            P.WriteUintAddPos4(2);
            P.WriteUintAddPos4(I.UID);
            P.WriteUintAddPos4(0); P.WriteUintAddPos4(0);//8
            P.WriteUintAddPos4(0); P.WriteUintAddPos4(0);//16
            P.WriteUintAddPos4(0); P.WriteUintAddPos4(0);//24
            P.WriteUintAddPos4(0); P.WriteUintAddPos4(0);//32
            //P.Move(32);
            return P.AddTQServer8Byte();
        }
        public static byte[] SendWarehouse(Game.Character C, ushort NPC)
        {
            ArrayList Warehouse = null;
            switch (NPC)
            {
                case 8: { Warehouse = C.Warehouses.TCWarehouse; break; }
                case 10012: { Warehouse = C.Warehouses.PCWarehouse; break; }
                case 10028: { Warehouse = C.Warehouses.ACWarehouse; break; }
                case 10011: { Warehouse = C.Warehouses.DCWarehouse; break; }
                case 10027: { Warehouse = C.Warehouses.BIWarehouse; break; }
                case 44: { Warehouse = C.Warehouses.MAWarehouse; break; }
                case 4101: { Warehouse = C.Warehouses.SCWarehouse; break; }
                default: return new byte[0];////new COPacket(new byte[0]);
            }
            uint length = 0;
            try
            {
                length = (uint)(Warehouse.Count > 20 ? 20 : Warehouse.Count);
            }
            catch { }
            byte[] Packet = new byte[8 + 16 + (32 * length)];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44e);//1102
            P.WriteUintAddPos4(NPC);
            P.WriteUintAddPos4(0); //P.Move(4);
            P.WriteUintAddPos4(length);
            uint count = 0;
            foreach (Game.Item I in Warehouse)
            {
                if (I.ID != 0)
                {
                    count++;
                    if (count == length + 1)
                        return P.AddTQServer8Byte();
                    if (Warehouse == C.Warehouses.MAWarehouse)
                    {
                        if (count == 41)
                            return P.AddTQServer8Byte();
                    }
                    else if (count == 21)
                        return P.AddTQServer8Byte();
                    P.WriteUintAddPos4(I.UID);
                    P.WriteUintAddPos4(I.ID);
                    P.WriteByteAddPos1(0); // P.Move(1);
                    P.WriteByteAddPos1((byte)I.Soc1);
                    P.WriteByteAddPos1((byte)I.Soc2);
                    P.WriteUshortAddPos2((ushort)I.Effect);
                    P.WriteByteAddPos1(I.Plus);
                    P.WriteByteAddPos1(I.Bless);
                    if (I.FreeItem)
                        P.WriteByteAddPos1(1);
                    else
                        P.WriteByteAddPos1(0);//P.Move(1);
                    P.WriteByteAddPos1(I.Enchant);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1(0);
                    P.WriteByteAddPos1((byte)I.Color);
                    P.WriteUintAddPos4(I.TalismanProgress);
                    P.WriteUintAddPos4(0);// P.Move(4);
                }
            }
            return P.AddTQServer8Byte();
        }
        public static byte[] OpenWarehouse(ushort NPCID, uint Money)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f1);
            P.WriteUintAddPos4(NPCID);
            P.WriteUintAddPos4(Money);
            P.WriteUintAddPos4(0x09);
            P.WriteUintAddPos4((uint)Environment.TickCount);

            return P.AddTQServer8Byte();
        }
        public static byte[] NPCText(string Text)
        {
            byte[] PacketData = new byte[(0xf + Text.Length) + 2 + 8];
            PacketData[0x0] = (byte)(PacketData.Length - 8 & 0xff);
            PacketData[0x1] = (byte)(PacketData.Length - 8 >> 8);
            PacketData[0x2] = 0xf0;
            PacketData[0x3] = 0x07;
            PacketData[0x4] = 0x00;
            PacketData[0x5] = 0x00;
            PacketData[0x6] = 0x00;
            PacketData[0x7] = 0x00;
            PacketData[0x8] = 0x00;
            PacketData[0x9] = 0x00;
            PacketData[0xa] = 0xff;
            PacketData[0xb] = 0x01;
            PacketData[0xc] = 0x01;
            PacketData[0xd] = (byte)(Text.Length);  //Length of the name
            for (int x = 0; x < Text.Length; x++)
            {
                PacketData[0xe + x] = Convert.ToByte(Text[x]);
            }
            string s = "TQServer";
            ASCIIEncoding encoding = new ASCIIEncoding();
            encoding.GetBytes(s).CopyTo(PacketData, (int)(PacketData.Length - 8));

            return PacketData;
        }
        public static byte[] NPCLink(string Text, byte Option)
        {
            byte[] PacketData = new byte[(0xf + Text.Length) + 2 +8];
            PacketData[0x0] = (byte)(PacketData.Length-8 & 0xff);
            PacketData[0x1] = (byte)(PacketData.Length-8 >> 8);
            PacketData[0x2] = 0xf0;
            PacketData[0x3] = 0x07;
            PacketData[0x4] = 0x00;
            PacketData[0x5] = 0x00;
            PacketData[0x6] = 0x00;
            PacketData[0x7] = 0x00;
            PacketData[0x8] = 0x00;
            PacketData[0x9] = 0x00;
            PacketData[0xa] = (byte)(Option & 0xff); // Responce ID for NPC
            PacketData[0xb] = 0x02;
            PacketData[0xc] = 0x01;
            PacketData[0xd] = (byte)(Text.Length);  //Length of the NPC text to say
            for (int x = 0; x < Text.Length; x++)
            {
                PacketData[0xe + x] = Convert.ToByte(Text[x]);
            }
            string s = "TQServer";
            ASCIIEncoding encoding = new ASCIIEncoding();
            encoding.GetBytes(s).CopyTo(PacketData, (int)(PacketData.Length - 8));

            return PacketData;
        }
        public static byte[] NPCLink2(string Text, byte Option)
        {
            byte[] Packet = new byte[8 + 18 + Text.Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x7f0);
            P.WriteUintAddPos4(0); P.WriteUshortAddPos2(0);// P.Move(6);
            P.WriteByteAddPos1(Option);
            P.WriteByteAddPos1(0x03);
            P.WriteByteAddPos1(0x01);
            P.WriteByteAddPos1((byte)Text.Length);
            P.WriteString(Text);
          //  P.Move(3);
            P.WriteUshortAddPos2(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] NPCFace(ushort ID)
        {
            byte[] PacketData = new byte[0x20 + 8];

            PacketData[0x0] = (byte)(PacketData.Length - 8 & 0xff);
            PacketData[0x1] = (byte)((PacketData.Length - 8 >> 8) & 0xff);
            PacketData[0x2] = 0xf0;
            PacketData[0x3] = 0x07;
            PacketData[0x4] = 0x0a;
            PacketData[0x5] = 0x00;
            PacketData[0x6] = 0x0a;
            PacketData[0x7] = 0x00;
            PacketData[0x8] = (byte)(ID & 0xff);
            PacketData[0x9] = (byte)((ID >> 8) & 0xff);
            PacketData[0xa] = 0xff;
            PacketData[0xb] = 0x04;
            PacketData[0xc] = 0x00;
            PacketData[0xd] = 0x00;
            PacketData[0xe] = 0x00;
            PacketData[0xf] = 0x00;
            PacketData[0x10] = 0x10;
            PacketData[0x11] = 0x00;
            PacketData[0x12] = 0xf0;
            PacketData[0x13] = 0x07;
            PacketData[0x14] = 0x00;
            PacketData[0x15] = 0x00;
            PacketData[0x16] = 0x00;
            PacketData[0x17] = 0x00;
            PacketData[0x18] = 0x00;
            PacketData[0x19] = 0x00;
            PacketData[0x1a] = 0xff;
            PacketData[0x1b] = 0x64;
            PacketData[0x1c] = 0x00;
            PacketData[0x1d] = 0x00;
            PacketData[0x1e] = 0x00;
            PacketData[0x1f] = 0x00;

            string s = "TQServer";
            ASCIIEncoding encoding = new ASCIIEncoding();
            encoding.GetBytes(s).CopyTo(PacketData, (int)(PacketData.Length - 8));

            return PacketData;
        }
        public static byte[] NPCFinish()
        {
            byte[] Packet = new byte[8 + 18];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x7f0);
            P.WriteUintAddPos4(0); P.WriteUshortAddPos2(0);//P.Move(6);
            P.WriteByteAddPos1(0xff);
            P.WriteByteAddPos1(0x64);
            P.WriteUintAddPos4(0); //P.Move(4);

            return P.AddTQServer8Byte();
        }

        public static byte[] ViewEquip(Game.Character C)
        {
            byte[] Packet = new byte[8 + 11 + C.Spouse.Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f7);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteByteAddPos1(0x0a);
            P.WriteByteAddPos1(0x01);
            P.WriteByteAddPos1((byte)(C.Spouse.Length));
            P.WriteString(C.Spouse);

            return P.AddTQServer8Byte();
        }
        public static byte[] AddViewItem(uint Viewed, Game.Item I, byte Pos)
        {
            byte[] Packet = new byte[56];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f0);
            P.WriteUintAddPos4(Viewed);
            P.WriteUintAddPos4(I.ID);
            P.WriteUshortAddPos2(I.CurDur);
            P.WriteUshortAddPos2(I.MaxDur);
            P.WriteUshortAddPos2(4);
            P.WriteUshortAddPos2(Pos);
            P.WriteUintAddPos4(I.TalismanProgress);
            P.WriteByteAddPos1((byte)I.Soc1);
            P.WriteByteAddPos1((byte)I.Soc2);
            P.WriteUshortAddPos2(0);//P.Move(2);
            P.WriteByteAddPos1(I.Plus);
            P.WriteByteAddPos1(I.Bless);
            if (I.FreeItem)
                P.WriteByteAddPos1(1);
            else
                P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(I.Enchant);
            P.WriteUintAddPos4(0); P.WriteUshortAddPos2(0);// P.Move(6);
            if (I.Locked == 1)
                P.WriteUshortAddPos2(1);
            else if (I.Locked == 2)
                P.WriteUshortAddPos2(1);
            else
                P.WriteUshortAddPos2(0);
            P.WriteUintAddPos4(3);

            P.WriteUintAddPos4(I.Progress);
            return P.AddTQServer8Byte();
        }
        public static byte[] MentorApprenticePacket(uint adder, uint uid, string name, byte Mode, byte Online)
        {
            byte[] Packet = new byte[8 + 24 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)2065);
            P.WriteUintAddPos4(Mode);
            P.WriteUintAddPos4(adder);
            P.WriteUintAddPos4(uid);
            P.WriteUintAddPos4(Online);
            P.WriteUintAddPos4(1);
            P.WriteString(name);

            return P.AddTQServer8Byte();
        }
        public static byte[] Mentor(string Name,byte Mode, uint Mentor_UniqueID, uint Apprentice_UniqueID, bool Online)
        {//Types 7=??, 11=Ask for a mentor, 14=Leave Mentor, 18= ???,
            byte[] Packet = new byte[24 + Name.Length + 8];
          //  byte[] Packet = new byte[24 + Name.Length +16 + 16 + 16];//Name.Length
            COPacket P = new COPacket(Packet);
            // PacketBuilder P = new PacketBuilder(2065, 24 + Name.Length);
            //  P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
           /* P.WriteUshortAddPos2((ushort)(Packet.Length - 8 ));//0
            P.WriteUshortAddPos2((ushort)2065);//2
            P.WriteByteAddPos1(Mode);//4
            P.WriteUintAddPos4(Mentor_UniqueID);//5
            P.WriteUintAddPos4(Apprentice_UniqueID);//9
            P.WriteByteAddPos1(Online == true ? (byte)1 : (byte)0);//13
            P.WriteByteAddPos1(7);//14
            P.WriteString(Name);//15*/
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//0
            P.WriteUshortAddPos2((ushort)2065);//2
          //4
           
            P.WriteByteAddPos1(Mode);
            P.WriteUintAddPos4(Mentor_UniqueID);
            P.WriteUintAddPos4(Apprentice_UniqueID);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(Online == true ? (byte)1 : (byte)0);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(0);
            P.WriteString(Name);
            
            //P.WriteUintAddPos4(Mentor_UniqueID);//5
           // P.WriteUintAddPos4(Apprentice_UniqueID);//9
           // P.WriteByteAddPos1(Online == true ? (byte)1 : (byte)0);//13
           // P.WriteByteAddPos1(7);//14
           // P.WriteString(Name);//15


             //0 	 ushort 	 24 + Character_Name_Length
             //2 	ushort 	2065
             //4 	byte 	Mentor_type
             //5 	uint 	Mentor_ID
             //9 	uint 	Apprentice_ID
             //13 	bool 	Online
             //14 	byte 	String_Count
             //15 	string 	Character_Name


            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNamedNPC(Game.NPC NPC, string Name)
        {
            byte[] Packet = new byte[8 + 36 + Name.Length];

            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x455);
            P.WriteUintAddPos4(NPC.EntityID);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);//P.Move(8);
            P.WriteUshortAddPos2(NPC.Loc.X);
            P.WriteUshortAddPos2(NPC.Loc.Y);
            P.WriteUshortAddPos2((ushort)(NPC.Type + NPC.Direction));
            P.WriteUshortAddPos2(NPC.Flags);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)Name.Length);
            P.WriteString(Name);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNPCWithHP(uint EntityID, ushort Type, ushort Flags, Game.Location Loc, uint CurHP, uint MaxHP)
        {
            byte[] Packet = new byte[8 + 34];

            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1109);
            P.WriteUintAddPos4(EntityID);
            P.WriteUintAddPos4(MaxHP);
            P.WriteUintAddPos4(CurHP);
            P.WriteUshortAddPos2(Loc.X);
            P.WriteUshortAddPos2(Loc.Y);
            P.WriteUshortAddPos2((ushort)Type);
            P.WriteUshortAddPos2(Flags);
            P.WriteUshortAddPos2(10);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNPCWithHP(uint EntityID, ushort Type, ushort Flags, Game.Location Loc, bool Named, string Name, uint CurHP, uint MaxHP)
        {
            byte[] Packet = new byte[8 + 36 + Name.Length];

            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1109);
            P.WriteUintAddPos4(EntityID);
            P.WriteUintAddPos4(MaxHP);
            P.WriteUintAddPos4(CurHP);
            P.WriteUshortAddPos2(Loc.X);
            P.WriteUshortAddPos2(Loc.Y);
            P.WriteUshortAddPos2((ushort)Type);
            P.WriteUshortAddPos2(Flags);
            P.WriteUshortAddPos2(11);
            if (Named)
            {
                P.WriteByteAddPos1(1);
                P.WriteByteAddPos1((byte)Name.Length);
                P.WriteString(Name);
            }

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNPCWithHP(Game.NPC NPC)
        {
            byte[] Packet = new byte[8 + 34];

            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x455);
            P.WriteUintAddPos4(NPC.EntityID);
            P.WriteUintAddPos4(NPC.MaxHP);
            P.WriteUintAddPos4(NPC.CurHP);
            P.WriteUshortAddPos2(NPC.Loc.X);
            P.WriteUshortAddPos2(NPC.Loc.Y);
            P.WriteUshortAddPos2((ushort)(NPC.Type + NPC.Direction));
            P.WriteUshortAddPos2(NPC.Flags);
            P.WriteUshortAddPos2(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNamedNPC2(Game.NPC NPC, string Name)
        {
            byte[] Packet = new byte[8 + 20 + Name.Length];

            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x7ee);
            P.WriteUintAddPos4(NPC.EntityID);
            P.WriteUshortAddPos2(NPC.Loc.X);
            P.WriteUshortAddPos2(NPC.Loc.Y);
            P.WriteUshortAddPos2((ushort)(NPC.Type + NPC.Direction));
            P.WriteUintAddPos4(NPC.Flags);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)Name.Length);
            P.WriteString(Name);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnNPC(Game.NPC N)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x7ee);
            P.WriteUintAddPos4(N.EntityID);
            P.WriteUshortAddPos2(N.Loc.X);
            P.WriteUshortAddPos2(N.Loc.Y);
            P.WriteUshortAddPos2((ushort)(N.Type + N.Direction));
            P.WriteUshortAddPos2(N.Flags);
            P.WriteUintAddPos4(1);

            return P.AddTQServer8Byte();
        }
        public static byte[] ItemDrop(Game.DroppedItem I)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44d);
            P.WriteUintAddPos4(I.UID);
            P.WriteUintAddPos4(I.Info.ID);
            P.WriteUshortAddPos2(I.Loc.X);
            P.WriteUshortAddPos2(I.Loc.Y);
            P.WriteUshortAddPos2(0x03);
            P.WriteUshortAddPos2(0x01);

            return P.AddTQServer8Byte();
        }
        public static byte[] DropConfiscatorItem(Game.DroppedItemConfiscator I)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44d);
            P.WriteUintAddPos4(I.UID);
            P.WriteUintAddPos4(I.Info.ID);
            P.WriteUshortAddPos2(I.Loc.X);
            P.WriteUshortAddPos2(I.Loc.Y);
            P.WriteUshortAddPos2(0x03);
            P.WriteByteAddPos1(4);
            P.WriteByteAddPos1(0);


            return P.AddTQServer8Byte();
        }
        public static byte[] ItemDropRemove(uint ItemUID, uint ItemID, ushort X, ushort Y)
        {
            byte[] Packet = new byte[8 + 20];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44d);
            P.WriteUintAddPos4(ItemUID);
            P.WriteUintAddPos4(ItemID);
            P.WriteUshortAddPos2(X);
            P.WriteUshortAddPos2(Y);
            P.WriteUshortAddPos2(0x03);
            P.WriteUshortAddPos2(0x02);

            return P.AddTQServer8Byte();
        }
        public static byte[] TeamPacket(uint CharID, byte Mode)
        {
            byte[] Packet = new byte[8 + 12];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3ff);
            P.WriteUintAddPos4(Mode);
            P.WriteUintAddPos4(CharID);

            return P.AddTQServer8Byte();
        }
        public static byte[] PlayerJoinsTeam(Game.Character C)
        {
            byte[] Packet = new byte[8 + 36];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x402);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1(1);
            P.WriteString(C.Name + C.MyClient.AuthInfo.Status);
            P.Move(16 - (C.Name + C.MyClient.AuthInfo.Status).Length);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteUintAddPos4(uint.Parse(C.Avatar.ToString() + C.Body.ToString()));
            P.WriteUshortAddPos2((ushort)C.CurHP);
            P.WriteUshortAddPos2((ushort)C.MaxHP);

            return P.AddTQServer8Byte();
        }
        public static byte[] SkillUse(uint EntityID, uint Target, uint Damage, ushort SkillId, byte SkillLvl, ushort X, ushort Y)
        {
            byte[] Packet = new byte[8 + 32 + (1 * 12)];//28
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x451);
            
            P.WriteUintAddPos4(EntityID);
            P.WriteUshortAddPos2(X);
            P.WriteUshortAddPos2(Y);
            P.WriteUshortAddPos2(SkillId);
            P.WriteUshortAddPos2(SkillLvl);
            P.WriteUintAddPos4(1);
            P.WriteUintAddPos4(Target);
            P.WriteUintAddPos4(Damage);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] SkillUse(Skills.SkillsClass.SkillUse SU)
        {
            byte[] Packet;// = new byte[8 + 20 + (SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count) * 12];
            if (SU.MobTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else if (SU.PlayerTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else if (SU.MiscTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else if (SU.PlayerTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else if (SU.CompanionTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else if (SU.NPCTargets == null)
            {
                Packet = new byte[8 + 32];
            }
            else
                Packet = new byte[8 + 32 + (SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count + SU.CompanionTargets.Count) * 12];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x451);
            if (SU.User != null)
                P.WriteUintAddPos4(SU.User.EntityID);
            else if (SU.MUser != null)
                P.WriteUintAddPos4(SU.MUser.EntityID);
            else
                P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(SU.AimX);
            P.WriteUshortAddPos2(SU.AimY);
            P.WriteUshortAddPos2(SU.Info.ID);
            P.WriteUshortAddPos2(SU.Info.Level);
            if (SU.MobTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.PlayerTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.MiscTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.CompanionTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.NPCTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else
                P.WriteUintAddPos4((uint)(SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count + SU.CompanionTargets.Count));
            if (SU.MobTargets != null)
                foreach (DictionaryEntry DE in SU.MobTargets)
                {
                    P.WriteUintAddPos4(((Game.Mob)DE.Key).EntityID);
                    P.WriteUintAddPos4((uint)DE.Value);
                    P.WriteUintAddPos4(0);// P.Move(4);
                }
            if (SU.PlayerTargets != null)
                foreach (DictionaryEntry DE in SU.PlayerTargets)
                {
                    P.WriteUintAddPos4(((Game.Character)DE.Key).EntityID);
                    P.WriteUintAddPos4((uint)DE.Value);
                    P.WriteUintAddPos4(0);
                }
            if (SU.CompanionTargets != null)
                foreach (DictionaryEntry DE in SU.CompanionTargets)
                {
                    P.WriteUintAddPos4(((Game.Companion)DE.Key).EntityID);
                    P.WriteUintAddPos4((uint)DE.Value);
                    P.WriteUintAddPos4(0);
                }
            if (SU.NPCTargets != null)
                foreach (DictionaryEntry DE in SU.NPCTargets)
                {
                    P.WriteUintAddPos4(((Game.NPC)DE.Key).EntityID);
                    P.WriteUintAddPos4((uint)DE.Value);
                    P.WriteUintAddPos4(0);
                }
            if (SU.MiscTargets != null)
                foreach (DictionaryEntry DE in SU.MiscTargets)
                {
                    P.WriteUintAddPos4((uint)DE.Key);
                    P.WriteUintAddPos4((uint)DE.Value);
                    P.WriteUintAddPos4(0);
                }
            
            return P.AddTQServer8Byte();
        }
        public static byte[] tecticalEff(Skills.SkillsClass.SkillUse SU)
        {
            byte[] Packet;// = new byte[8 + 20 + (SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count) * 12];
           
            {
               // Packet = new byte[8 + 32];
                Packet = new byte[8 + 32 + (1) * 12];

            }
            //else
            //    Packet = new byte[8 + 32 + (SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count) * 12];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x451);
            if (SU.User != null)
                P.WriteUintAddPos4(SU.User.EntityID);
            else
                P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(SU.AimX);
            P.WriteUshortAddPos2(SU.AimY);
            P.WriteUshortAddPos2(SU.Info.ID);
            P.WriteUshortAddPos2(SU.Info.Level);
            if (SU.MobTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.PlayerTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.MiscTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            else if (SU.NPCTargets == null)
            {
                P.WriteUintAddPos4(0);
            }
            
            else
                P.WriteUintAddPos4((uint)(SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count));
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4(1);



            P.WriteUintAddPos4(1000865);
            P.WriteUintAddPos4(1000);
            P.WriteUintAddPos4(0);// P.Move(4);
            // if (SU.MobTargets != null)
            //    foreach (DictionaryEntry DE in SU.MobTargets)
            //    {
            //        P.WriteUintAddPos4(((Game.Mob)DE.Key).EntityID);
            //        P.WriteUintAddPos4((uint)DE.Value);
            //        P.WriteUintAddPos4(0);// P.Move(4);
            //    }
            //if (SU.PlayerTargets != null)
            //    foreach (DictionaryEntry DE in SU.PlayerTargets)
            //    {
            //        P.WriteUintAddPos4(((Game.Character)DE.Key).EntityID);
            //        P.WriteUintAddPos4((uint)DE.Value);
            //        P.WriteUintAddPos4(0);
            //    }
            //if (SU.NPCTargets != null)
            //    foreach (DictionaryEntry DE in SU.NPCTargets)
            //    {
            //        P.WriteUintAddPos4(((Game.NPC)DE.Key).EntityID);
            //        P.WriteUintAddPos4((uint)DE.Value);
            //        P.WriteUintAddPos4(0);
            //    }
            //if (SU.MiscTargets != null)
            //    foreach (DictionaryEntry DE in SU.MiscTargets)
            //    {
            //        P.WriteUintAddPos4((uint)DE.Key);
            //        P.WriteUintAddPos4((uint)DE.Value);
            //        P.WriteUintAddPos4(0);
            //    }

            return P.AddTQServer8Byte();
        }
       /* public static byte[] SkillUse(Skills.SkillsClass.SkillUse SU)
        {
            byte[] Packet = new byte[8 + 20 + (SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count) * 12];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x451);
            P.WriteUintAddPos4(SU.User.EntityID);
            P.WriteUshortAddPos2(SU.AimX);
            P.WriteUshortAddPos2(SU.AimY);
            P.WriteUshortAddPos2(SU.Info.ID);
            P.WriteUshortAddPos2(SU.Info.Level);
            P.WriteUintAddPos4((uint)(SU.MobTargets.Count + SU.PlayerTargets.Count + SU.NPCTargets.Count + SU.MiscTargets.Count));
            foreach (DictionaryEntry DE in SU.MobTargets)
            {
                P.WriteUintAddPos4(((Game.Mob)DE.Key).EntityID);
                P.WriteUintAddPos4((uint)DE.Value);
                P.Move(4);
            }
            foreach (DictionaryEntry DE in SU.PlayerTargets)
            {
                P.WriteUintAddPos4(((Game.Character)DE.Key).EntityID);
                P.WriteUintAddPos4((uint)DE.Value);
                P.Move(4);
            }
            foreach (DictionaryEntry DE in SU.NPCTargets)
            {
                P.WriteUintAddPos4(((Game.NPC)DE.Key).EntityID);
                P.WriteUintAddPos4((uint)DE.Value);
                P.Move(4);
            }
            foreach (DictionaryEntry DE in SU.MiscTargets)
            {
                P.WriteUintAddPos4((uint)DE.Key);
                P.WriteUintAddPos4((uint)DE.Value);
                P.Move(4);
            }

            return P.AddTQServer8Byte();
        }*/
        public static byte[] ItemPacket(uint UID, uint pos, byte type)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));//2
            P.WriteUshortAddPos2((ushort)0x3f1);//4
            P.WriteUintAddPos4(UID);//8
            P.WriteUintAddPos4(pos);//12
            P.WriteUintAddPos4(type);//16
            P.WriteUlongAddPos8(0);//24
            P.WriteUintAddPos4(0);//28
            // P.Move(12);

            return P.AddTQServer8Byte();
        }
        public static byte[] StringGuild(uint UID, byte Type, string name, byte Count)
        {
            byte[] Packet = new byte[8 + 12 + name.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f7);
            P.WriteUintAddPos4(UID);
            P.WriteByteAddPos1(Type);
            P.WriteByteAddPos1(Count);
            P.WriteString(name);
            P.WriteUshortAddPos2(0);//P.Move(2);

            return P.AddTQServer8Byte();
        }
        public static byte[] GuildInfo(Features.Guild TheGuild, Game.Character Player)
        {
            byte[] Packet = new byte[8 + 40];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x452);
            P.WriteUintAddPos4(TheGuild.GuildID);
            P.WriteUintAddPos4(Player.GuildDonation);
            P.WriteUintAddPos4(TheGuild.Fund);
            P.WriteUintAddPos4((uint)TheGuild.MembersCount);
            P.WriteByteAddPos1((byte)Player.GuildRank);
            P.WriteString(TheGuild.Creator.MembName);
            P.Move(19 - TheGuild.Creator.MembName.Length);

            return P.AddTQServer8Byte();
        }
        public static byte[] SendGuild(uint GuildID, int Type)
        {
            byte[] Packet = new byte[8 + 12];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1107);//0x453);
            P.WriteUintAddPos4((uint)Type);
            P.WriteUintAddPos4(GuildID);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnEntity(Game.Character C)
        {
            byte[] Packet;
            //Packet = new byte[8 + 138 + (C.Name + C.MyClient.AuthInfo.Status).Length];
            if (C.MyClient.Robot && C.Owner != null)
                Packet = new byte[8 + 138 + (C.Owner.Name + "`Avatar-L"+C.Owner.AvatarLevel).Length];
            else
                Packet = new byte[8 + 138 + (C.Name + C.MyClient.AuthInfo.Status).Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);
            P.WriteUintAddPos4(C.Mesh);
            P.WriteUintAddPos4(C.EntityID);

            if (C.MyGuild != null && !C.MyClient.Robot)
            {
                P.WriteUshortAddPos2(C.MyGuild.GuildID);//Guild ID
                // P.Move(1);//Guild Branch ID maybe
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1((byte)C.GuildRank);
            }
            else if (C.MyClient.Robot)
            {
                P.WriteUintAddPos4(0);
            }
            else
                P.WriteUintAddPos4(0);
            //P.Move(4);
            P.WriteUlongAddPos8((ulong)C.StatEff.Value);

            if (C.Alive && C.Body >= 1000)
            {
                P.WriteUintAddPos4(C.Equips.HeadGear.ID);
                P.WriteUintAddPos4(C.Equips.Garment.ID);
                P.WriteUintAddPos4(C.Equips.Armor.ID);
                P.WriteUintAddPos4(C.Equips.LeftHand.ID);
                P.WriteUintAddPos4(C.Equips.RightHand.ID);
                P.WriteUintAddPos4(C.Equips.Steed.ID);
            }
            else
            {
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
            }
            //   P.Move(24);

            P.WriteUintAddPos4(12);//12
            P.WriteUshortAddPos2(0);//0
            P.WriteUshortAddPos2(0);//0
            if (C.Alive && C.Body >= 1000)
                P.WriteUshortAddPos2(C.Hair);
            else
                P.WriteUshortAddPos2(0);//P.Move(2);
            P.WriteUshortAddPos2(C.Loc.X);
            P.WriteUshortAddPos2(C.Loc.Y);

            P.WriteByteAddPos1(C.Direction);
            P.WriteByteAddPos1(C.Action);
            P.WriteUintAddPos4(0);//P.Move(4);
            P.WriteByteAddPos1(C.Reborns);
            P.WriteUshortAddPos2(C.Level);
            P.WriteByteAddPos1(0);//type 0 = screen / 1 = window
            // P.Move(16);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4((byte)C.Nobility.Rank);
            if (C.Body >= 1000)
            {
                P.WriteUshortAddPos2((ushort)C.Equips.Armor.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.LeftHand.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.HeadGear.Color);
            }
            else
            {
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
            }
            //  P.Move(6);
            P.WriteUintAddPos4(C.UniversityPoints);
            P.WriteUshortAddPos2(C.Equips.Steed.Plus);
            P.WriteUintAddPos4(0);//Not sure 0
            P.WriteUintAddPos4(C.Equips.Steed.TalismanProgress);
            //P.Move(24);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(1);
            if (!C.MyClient.Robot)
            {
                P.WriteByteAddPos1((byte)(C.Name + C.MyClient.AuthInfo.Status).Length);
                P.WriteString(C.Name + C.MyClient.AuthInfo.Status);
            }
            if (C.MyClient.Robot && C.Owner != null)
            {
                P.WriteByteAddPos1((byte)(C.Owner.Name + "`Avatar-L" + C.Owner.AvatarLevel).Length);
                P.WriteString(C.Owner.Name + "`Avatar-L" + C.Owner.AvatarLevel);
            }

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnViewed2(Game.Character C, byte Type)
        {
            byte[] Packet = new byte[8 + 138 + C.Name.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);

            P.WriteUintAddPos4(C.Mesh);
            P.WriteUintAddPos4(C.EntityID);

            if (C.MyGuild != null)
            {
                P.WriteUshortAddPos2(C.MyGuild.GuildID);//Guild ID
                //P.Move(1);//Guild Branch ID maybe
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1((byte)C.GuildRank);
            }
            else
                P.WriteUintAddPos4(0);//P.Move(4);
            P.WriteUlongAddPos8((ulong)C.StatEff.Value);

            if (C.Alive && C.Body >= 1000)
            {
                P.WriteUintAddPos4(C.Equips.HeadGear.ID);
                P.WriteUintAddPos4(C.Equips.Garment.ID);
                P.WriteUintAddPos4(C.Equips.Armor.ID);
                P.WriteUintAddPos4(C.Equips.LeftHand.ID);
                P.WriteUintAddPos4(C.Equips.RightHand.ID);
                P.WriteUintAddPos4(C.Equips.Steed.ID);
            }
            else
            {
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
            }
            //P.Move(24);
            P.WriteUintAddPos4(12);
            P.WriteUshortAddPos2(0);
            P.WriteUshortAddPos2(0);
            if (C.Alive && C.Body >= 1000)
                P.WriteUshortAddPos2(C.Hair);
            else
                P.WriteUshortAddPos2(0);//P.Move(2);
            P.WriteUshortAddPos2(C.Loc.X);
            P.WriteUshortAddPos2(C.Loc.Y);

            P.WriteByteAddPos1(C.Direction);
            P.WriteByteAddPos1(C.Action);
            P.WriteUintAddPos4(0);//  P.Move(4);
            P.WriteByteAddPos1(C.Reborns);
            P.WriteUshortAddPos2(C.Level);
            P.WriteByteAddPos1(Type);//type 0 = screen / 1 = window
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);// P.Move(16);
            P.WriteUintAddPos4((byte)C.Nobility.Rank);
            //P.WriteUshortAddPos2((ushort)C.Equips.Armor.Color);
            //P.WriteUshortAddPos2((ushort)C.Equips.LeftHand.Color);
            //P.WriteUshortAddPos2((ushort)C.Equips.HeadGear.Color);
            if (C.Body >= 1000)
            {
                P.WriteUshortAddPos2((ushort)C.Equips.Armor.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.LeftHand.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.HeadGear.Color);
            }
            else
            {
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
            }
            P.WriteUintAddPos4(C.UniversityPoints);
            P.WriteUshortAddPos2(C.Equips.Steed.Plus);
            P.WriteUintAddPos4(0);//Not sure
            P.WriteUintAddPos4(C.Equips.Steed.TalismanProgress);
            //P.Move(24);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)C.Name.Length);
            P.WriteString(C.Name);
            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnViewed(Game.Character C, byte Type)
        {
            byte[] Packet;
            if (C.MyClient.Robot && C.Owner != null)
                Packet = new byte[8 + 138 + (C.Owner.Name + "`Avatar-L" + C.Owner.AvatarLevel).Length];
            else
                Packet = new byte[8 + 138 + (C.Name + C.MyClient.AuthInfo.Status).Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);
            if (Type < 3 )
            P.WriteUintAddPos4(C.Mesh);
            P.WriteUintAddPos4(C.EntityID);

            if (C.MyGuild != null)
            {
                P.WriteUshortAddPos2(C.MyGuild.GuildID);//Guild ID
               //P.Move(1);//Guild Branch ID maybe
                P.WriteByteAddPos1(0);
                P.WriteByteAddPos1((byte)C.GuildRank);
            }
            else
                P.WriteUintAddPos4(0);//P.Move(4);
            P.WriteUlongAddPos8((ulong)C.StatEff.Value);

            if (C.Alive && C.Body >= 1000)
            {
                P.WriteUintAddPos4(C.Equips.HeadGear.ID);
                P.WriteUintAddPos4(C.Equips.Garment.ID);
                P.WriteUintAddPos4(C.Equips.Armor.ID);
                P.WriteUintAddPos4(C.Equips.LeftHand.ID);
                P.WriteUintAddPos4(C.Equips.RightHand.ID);
                P.WriteUintAddPos4(C.Equips.Steed.ID);
            }
            else
            {
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
                P.WriteUlongAddPos8(0);
            }
                //P.Move(24);
            P.WriteUintAddPos4(12);
            P.WriteUshortAddPos2(0);
            P.WriteUshortAddPos2(0);
            if (C.Alive && C.Body >= 1000)
                P.WriteUshortAddPos2(C.Hair);
            else
                P.WriteUshortAddPos2(0);//P.Move(2);
            P.WriteUshortAddPos2(C.Loc.X);
            P.WriteUshortAddPos2(C.Loc.Y);

            P.WriteByteAddPos1(C.Direction);
            P.WriteByteAddPos1(C.Action);
            P.WriteUintAddPos4(0);//  P.Move(4);
            P.WriteByteAddPos1(C.Reborns);
            P.WriteUshortAddPos2(C.Level);
            P.WriteByteAddPos1(Type);//type 0 = screen / 1 = window
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);// P.Move(16);
            P.WriteUintAddPos4((byte)C.Nobility.Rank);
            //P.WriteUshortAddPos2((ushort)C.Equips.Armor.Color);
            //P.WriteUshortAddPos2((ushort)C.Equips.LeftHand.Color);
            //P.WriteUshortAddPos2((ushort)C.Equips.HeadGear.Color);
            if (C.Body >= 1000)
            {
                P.WriteUshortAddPos2((ushort)C.Equips.Armor.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.LeftHand.Color);
                P.WriteUshortAddPos2((ushort)C.Equips.HeadGear.Color);
            }
            else
            {
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
                P.WriteUshortAddPos2(0);
            }
            P.WriteUintAddPos4(C.UniversityPoints);
            P.WriteUshortAddPos2(C.Equips.Steed.Plus);
            P.WriteUintAddPos4(0);//Not sure
            P.WriteUintAddPos4(C.Equips.Steed.TalismanProgress);
            //P.Move(24);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(1);
            if (!C.MyClient.Robot)
            {
                P.WriteByteAddPos1((byte)(C.Name + C.MyClient.AuthInfo.Status).Length);
                P.WriteString(C.Name + C.MyClient.AuthInfo.Status);
            }
            if (C.MyClient.Robot && C.Owner != null)
            {
                P.WriteByteAddPos1((byte)(C.Owner.Name + "`Avatar-L" + C.Owner.AvatarLevel).Length);
                P.WriteString(C.Owner.Name + "`Avatar-L" + C.Owner.AvatarLevel);
            }
            return P.AddTQServer8Byte();
        }
        public static byte[] AttackPacket(uint Attacker, uint Attacked, ushort X, ushort Y, uint Damage, byte AttackType)
        {
            byte[] Data = new byte[8 + 32];//22 /// 32
            COPacket P = new COPacket(Data);

            P.WriteUshortAddPos2((ushort)(Data.Length - 8));
            // P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3FE);//1022 // 0x3FE

            P.WriteUintAddPos4(0);
           // P.Move(4);
            P.WriteUintAddPos4(Attacker);
         //   if (Damage != 0)
                P.WriteUintAddPos4(Attacked);
           // else
             //   P.Move(4);
            P.WriteUshortAddPos2(X);
            P.WriteUshortAddPos2(Y);
            P.WriteUintAddPos4(AttackType);
            //if (Damage != 0)
                P.WriteUintAddPos4(Damage);
           /// else
             //   P.Move(4);
            //P.Move(4);
                P.WriteUintAddPos4(0);
            return P.AddTQServer8Byte();
        }
        public static byte[] Movement(uint UID, byte Dir,uint walkType)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10005);
            P.WriteUintAddPos4((byte)(Dir + 8 * Program.Rnd.Next(7)));
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(walkType);
            P.WriteUintAddPos4((uint)Environment.TickCount);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnEntity(Game.Mob C)
        {
            byte[] Packet = new byte[8 + 138 + C.Name.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);
            P.WriteUintAddPos4(C.Mesh);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteUintAddPos4(0);
            P.WriteUlongAddPos8((ulong)C.StatusEff.Value);//Status Effect
           // P.Move(28);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2((ushort)C.CurrentHP);
            P.WriteUshortAddPos2(C.Level);
            P.WriteUshortAddPos2(0);// P.Move(2);//Hair
            P.WriteUshortAddPos2(C.Loc.X);
            P.WriteUshortAddPos2(C.Loc.Y);
            P.WriteByteAddPos1(C.Direction);
            P.WriteByteAddPos1(C.Action);
            //P.Move(72);

            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);

            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);

            P.WriteUlongAddPos8(0);

            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)C.Name.Length);
            P.WriteString(C.Name);

            return P.AddTQServer8Byte();
        }

        public static byte[] RankFlowerPacket(string flowers, uint Rank)
        {
            byte[] packet = new byte[21 + flowers.Length + 8];
            COPacket p = new COPacket(packet);
            // p.WriteUshortAddPos2((ushort)(21 + flowers.Length));
            p.WriteUshortAddPos2((ushort)(packet.Length - 8));
            p.WriteUshortAddPos2(1150);
            p.WriteUintAddPos4(2);
            p.WriteUintAddPos4(Rank);
            p.WriteUintAddPos4(0);
            p.WriteByteAddPos1(1);
            p.WriteString(flowers);
            return p.AddTQServer8Byte();
        }
        public static byte[] FlowerScreen(uint Id, uint SenderID)
        {
            //  PacketBuilder Packet = new PacketBuilder(0x3f2, 28);
            byte[] packet = new byte[28 + 8];
            COPacket p = new COPacket(packet);
            p.WriteUshortAddPos2((ushort)(packet.Length - 8));
            p.WriteUshortAddPos2(0x3f2);
            p.WriteUlongAddPos8(0);
            p.WriteUintAddPos4(Id);
            p.WriteUshortAddPos2(0x4e0);
            p.WriteUshortAddPos2(0);
            p.WriteUintAddPos4(SenderID);
            p.WriteUshortAddPos2(9);
            p.WriteUshortAddPos2(0x74);
            p.WriteUlongAddPos8(99);

            return p.AddTQServer8Byte();
            /*Packet.Long(0x3f2);
            Packet.Long(Id);
            Packet.Short(0x4e0);
            Packet.Short(0);
            Packet.Long(SenderID);
            Packet.Short(9);
            Packet.Short(0x74);
            Packet.Long(99);
            return Packet.getFinal();*/
        }
        public static byte[] ReceiveFlower(uint ID, uint Type, uint Rank, uint FlowerType)
        {
            string flowers = ID.ToString();
            byte[] packet = new byte[21 + flowers.Length + 8];
            COPacket p = new COPacket(packet);
            p.WriteUshortAddPos2((ushort)(21 + flowers.Length));
            p.WriteUshortAddPos2(1151);
            p.WriteUintAddPos4(Type);
            p.WriteUintAddPos4(Rank);
            p.WriteUintAddPos4(FlowerType);
            p.WriteByteAddPos1(1);
            p.WriteString(flowers);
            return p.AddTQServer8Byte();
        }
        public static byte[] SpawnEntity(ushort Mesh, string Name, Game.Location Loc)
        {
            byte[] Packet = new byte[8 + 97 + Name.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);
            P.WriteUintAddPos4(Mesh);
            P.WriteUintAddPos4((uint)Program.Rnd.Next(400000, 500000));
            P.WriteUlongAddPos8(0);//Status Effect
            //P.Move(28);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2((ushort)65535);
            P.WriteUshortAddPos2(130);
            P.WriteUshortAddPos2(0); // P.Move(2);//Hair
            P.WriteUshortAddPos2(Loc.X);
            P.WriteUshortAddPos2(Loc.Y);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1(100);
           // P.Move(35);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(0);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)Name.Length);
            P.WriteString(Name);

            return P.AddTQServer8Byte();
        }
        public static byte[] SpawnEntity(Game.Companion Cmp)
        {
            byte[] Packet = new byte[8 + 138 + Cmp.Name.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10014);
            P.WriteUintAddPos4(Cmp.Mesh);
            P.WriteUintAddPos4(Cmp.EntityID);
            P.WriteUintAddPos4(0);
            P.WriteUlongAddPos8((ulong)Cmp.StatEff.Value);//Status Effect
            //P.Move(28);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2((ushort)Cmp.CurHP);
            P.WriteUshortAddPos2(Cmp.Level);
            P.WriteUshortAddPos2(0);// P.Move(2);//Hair
            P.WriteUshortAddPos2(Cmp.Loc.X);
            P.WriteUshortAddPos2(Cmp.Loc.Y);
            P.WriteByteAddPos1(Cmp.Direction);
            P.WriteByteAddPos1(100);

            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);

            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);

            P.WriteUlongAddPos8(0);

            //P.Move(72);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)Cmp.Name.Length);
            P.WriteString(Cmp.Name);

            return P.AddTQServer8Byte();
        }

        /* public static byte[] SpawnEntity(Game.Companion Cmp)
         {
             byte[] Packet = new byte[8 + 138 + Cmp.Name.Length];
             COPacket P = new COPacket(Packet);

             P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
             P.WriteUshortAddPos2((ushort)10014);
             P.WriteUintAddPos4(Cmp.Mesh);
             P.WriteUintAddPos4(Cmp.EntityID);
             P.WriteUlongAddPos8(0);//Status Effect
             P.Move(28);
             P.WriteUshortAddPos2(Cmp.CurHP);
             P.WriteUshortAddPos2(Cmp.Level);
             P.Move(2);//Hair
             P.WriteUshortAddPos2(Cmp.Loc.X);
             P.WriteUshortAddPos2(Cmp.Loc.Y);
             P.WriteByteAddPos1(0);
             P.WriteByteAddPos1(100);
             P.Move(72);
             P.WriteByteAddPos1(1);
             P.WriteByteAddPos1((byte)Cmp.Name.Length);
             P.WriteString(Cmp.Name);

             return P.AddTQServer8Byte();
         }*/
        public static byte[] GeneralData(uint Identifier, uint Value1, ushort Value2, ushort Value3, ushort Type, uint Time)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10010);
            P.WriteUintAddPos4(Identifier);
            P.WriteUintAddPos4(Value1);
            P.WriteUintAddPos4((uint)Time);
            P.WriteUintAddPos4(Type);
            P.WriteUshortAddPos2(Value2);
            P.WriteUshortAddPos2(Value3);
            P.WriteUintAddPos4(0); //P.Move(4);
            return P.AddTQServer8Byte();
        }
        public static unsafe byte[] DHKeyPacket(string Key, byte[] ServerIV, byte[] ClientIV)
        {
            //Console.WriteLine("PublicKey Is ["+ Key +"]");
            byte[] Junk = new byte[Program.Rnd.Next(8, 16)];

            fixed (byte* p = Junk)
            {
                for (int i = 0; i < Junk.Length; i++)
                    *(p + i) = (byte)Program.Rnd.Next(byte.MaxValue);
            }

            byte[] Packet = new byte[321 + Junk.Length];
            COPacket P = new COPacket(Packet);
            try
            {
                for (int i = 0; i < 11; i++)
                    P.WriteByteAddPos1((byte)Program.Rnd.Next(byte.MaxValue));
                P.WriteUintAddPos4((uint)(Packet.Length - 11));
                P.WriteUintAddPos4((uint)Junk.Length);
                P.WriteBytes(Junk);
                P.WriteUintAddPos4(8);
                P.WriteBytes(ServerIV);
                P.WriteUintAddPos4(8);
                P.WriteBytes(ClientIV);
                P.WriteUintAddPos4(128);//A320A85EDD791
                P.WriteString("A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB");
                P.WriteUintAddPos4(2);
                P.WriteString("05");
                P.WriteUintAddPos4(128);
                P.WriteString(Key);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }

            return P.AddTQServer8Byte();
        }
        /*public static unsafe COPacket DHKeyPacket(byte[] ServerIV1, byte[] ServerIV2, string P, string G, string ServerPublicKey)
        {
            int PAD_LEN = 11;
            int _junk_len = 12;
            string tqs = "A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB";
            MemoryStream ms = new MemoryStream();
            byte[] pad = new byte[PAD_LEN];
            Program.Rnd.NextBytes(pad);
            byte[] junk = new byte[_junk_len];
            Program.Rnd.NextBytes(junk);
            int size = 47 + P.Length + G.Length + ServerPublicKey.Length + 12 + 8 + 8;
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(pad);
            bw.Write(size - PAD_LEN);
            bw.Write((UInt32)_junk_len);
            bw.Write(junk);
            bw.Write((UInt32)ServerIV2.Length);
            bw.Write(ServerIV2);
            bw.Write((UInt32)ServerIV1.Length);
            bw.Write(ServerIV1);
            bw.Write((UInt32)P.ToCharArray().Length);
            foreach (char fP in P.ToCharArray())
            {
                bw.BaseStream.WriteByteAddPos1((byte)fP);
            }
            bw.Write((UInt32)G.ToCharArray().Length);
            foreach (char fG in G.ToCharArray())
            {
                bw.BaseStream.WriteByteAddPos1((byte)fG);
            }
            bw.Write((UInt32)ServerPublicKey.ToCharArray().Length);
            foreach (char SPK in ServerPublicKey.ToCharArray())
            {
                bw.BaseStream.WriteByteAddPos1((byte)SPK);
            }
            foreach (char tq in tqs.ToCharArray())
            {
                bw.BaseStream.WriteByteAddPos1((byte)tq);
            }
            //byte[] Packet = new byte[ms.Length];
            //Packet = ms.ToArray();
            byte[] packet = new byte[ms.Length];
            packet = ms.ToArray();
            COPacket p = new COPacket(packet);
            ms.Close();

            return P.AddTQServer8Byte();

        }*/
        public static byte[] FlowerPacket(string flowers)
        {
            byte[] packet = new byte[21 + flowers.Length + 8];
            COPacket p = new COPacket(packet);
            p.WriteUshortAddPos2((ushort)(21 + flowers.Length));
            p.WriteUshortAddPos2(1150);
            p.WriteUintAddPos4(1);
            p.WriteUintAddPos4(0); p.WriteUintAddPos4(0);// p.Move(8);
            p.WriteByteAddPos1(1);
            p.WriteString(flowers);
            return p.AddTQServer8Byte();
        }

        /*  public static byte[] Breeder(uint UID)
          {
              byte[] Packet = new byte[8 + 36];
              COPacket P = new COPacket(Packet);
              P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
              P.WriteUshortAddPos2((ushort)0x271a);
              P.WriteUintAddPos4(UID);
              P.WriteUshortAddPos2((ushort)0x170);
              P.Move(10);
              P.WriteUshortAddPos2((ushort)0x7e);
              P.WriteUshortAddPos2((ushort)0x0);
              P.WriteUshortAddPos2((ushort)0x1d);
              P.WriteUshortAddPos2((ushort)0x1b);
              P.Move(8);

              return P.AddTQServer8Byte();
          }*/
        public static byte[] Breeding(uint UID)
        {
            return GeneralData(UID, 0x170, 0, 0, 0x7e);
        }
        public static byte[] ReviveHere(uint UID)
        {
            return GeneralData(UID, 0, 0, 0, 144);
        }
        /* floor(0.9 * X) + floor(0.1 * Y) = Z

where:
X - color value (red, blue or green) of major steed (0-255)
Y - color value of minor steed (0-255)
Z - color value of new steed (0-255)
floor() - returns the largest integer less than or equal to a number
*/
        public static byte[] Packet2048(uint CharUID)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)2048);
            P.WriteUintAddPos4(CharUID);
            P.WriteUintAddPos4(4);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] Packet1032(uint CharUID)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1032);
            P.WriteUintAddPos4(CharUID);
            P.WriteUintAddPos4(0x1f);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }

        public static byte[] GeneralData(uint Identifier, uint Value1, ushort Value2, ushort Value3, ushort Type)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10010);
            P.WriteUintAddPos4(Identifier);
            P.WriteUintAddPos4(Value1);
            P.WriteUintAddPos4((uint)Native.timeGetTime());
            P.WriteUintAddPos4(Type);
            P.WriteUshortAddPos2(Value2);
            P.WriteUshortAddPos2(Value3);
            P.WriteUintAddPos4(0); //P.Move(4);
            return P.AddTQServer8Byte();
        }
        public static byte[] GeneralData(uint Identifier, uint Value1, ushort Value2, ushort Value3, ushort Type, byte Direction)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10010);
            P.WriteUintAddPos4(Identifier);
            P.WriteUintAddPos4(Value1);
            P.WriteUintAddPos4(Native.timeGetTime());
            P.WriteUshortAddPos2(Type);
            P.WriteUshortAddPos2(Direction);
            P.WriteUshortAddPos2(Value2);
            P.WriteUshortAddPos2(Value3);
            P.WriteUintAddPos4(0); //P.Move(4);
            return P.AddTQServer8Byte();
        }
        public static byte[] GeneralData(uint Identifier, ushort Value1, ushort Value2, ushort Value3, ushort Value4, byte Direction, ushort Type)
        {
            byte[] Packet = new byte[8 + 28];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10010);
            P.WriteUintAddPos4(Identifier);
            P.WriteUshortAddPos2(Value1);
            P.WriteUshortAddPos2(Value2);
            P.WriteUintAddPos4((uint)Native.timeGetTime());
            P.WriteUintAddPos4(Type);
            P.WriteUshortAddPos2(Value3);
            P.WriteUshortAddPos2(Value4);
            P.WriteUshortAddPos2(Direction);
            return P.AddTQServer8Byte();
        }
        
        // P.WriteUintAddPos4((uint)color.ToARBG());
        public static byte[] AddItemtest2(Game.Item I, byte Pos, System.Drawing.Color color)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            try
            {
                P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
                P.WriteUshortAddPos2((ushort)0x3f0);
                P.WriteUintAddPos4(I.UID);
                P.WriteUintAddPos4(I.ID);
                P.WriteUshortAddPos2(I.CurDur);
                P.WriteUshortAddPos2(I.MaxDur);
                P.WriteUshortAddPos2(1);
                P.WriteUshortAddPos2(Pos);
                // P.WriteUintAddPos4(I.TalismanProgress);
                P.WriteUintAddPos4((uint)color.ToArgb());
                P.WriteByteAddPos1((byte)I.Soc1);
                P.WriteByteAddPos1((byte)I.Soc2);
                P.WriteUshortAddPos2((ushort)I.Effect);
                P.WriteByteAddPos1(I.Plus);
                P.WriteByteAddPos1(I.Bless);
                if (I.FreeItem)
                    P.WriteByteAddPos1(1);
                else
                    P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(I.Enchant);
                P.WriteUshortAddPos2(0);
                P.WriteUintAddPos4(0);                //   P.Move(6);
                if (I.Locked == 1)
                    P.WriteUshortAddPos2(1);
                else if (I.Locked == 2)
                    P.WriteUshortAddPos2(1);
                else
                    P.WriteUshortAddPos2(0);
                if (I.Color == 0)
                    I.Color = (Game.Item.ArmorColor)new Random().Next(1, 9);
                P.WriteUintAddPos4((uint)I.Color);
                P.WriteUintAddPos4(I.Progress);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
            return P.AddTQServer8Byte();
        }

        public static byte[] AddItem(Game.Item I, byte Pos)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            try
            {
                P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
                P.WriteUshortAddPos2((ushort)0x3f0);
                P.WriteUintAddPos4(I.UID);
                P.WriteUintAddPos4(I.ID);
                P.WriteUshortAddPos2(I.CurDur);
                P.WriteUshortAddPos2(I.MaxDur);
                P.WriteUshortAddPos2(1);
                P.WriteUshortAddPos2(Pos);
                P.WriteUintAddPos4(I.TalismanProgress);
                P.WriteByteAddPos1((byte)I.Soc1);
                P.WriteByteAddPos1((byte)I.Soc2);
                P.WriteUshortAddPos2((ushort)I.Effect);
                P.WriteByteAddPos1(I.Plus);
                P.WriteByteAddPos1(I.Bless);
                if (I.FreeItem)
                    P.WriteByteAddPos1(1);
                else
                    P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(I.Enchant);
                P.WriteUintAddPos4(0);
                P.WriteUshortAddPos2(0);
                //P.Move(6);
                if (I.Locked == 1)
                    P.WriteUshortAddPos2(1);
                else if (I.Locked == 2)
                    P.WriteUshortAddPos2(1);
                else
                    P.WriteUshortAddPos2(0);
                if (I.Color == 0)
                    I.Color = (Game.Item.ArmorColor)new Random().Next(1, 9);
                P.WriteUintAddPos4((uint)I.Color);
                P.WriteUintAddPos4(I.Progress);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
            return P.AddTQServer8Byte();
        }

        public static byte[] UpdateItem(Game.Item I, byte Pos)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            try
            {
                P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
                P.WriteUshortAddPos2((ushort)0x3f0);
                P.WriteUintAddPos4(I.UID);
                P.WriteUintAddPos4(I.ID);
                P.WriteUshortAddPos2(I.CurDur);
                P.WriteUshortAddPos2(I.MaxDur);
                P.WriteUshortAddPos2(3);
                P.WriteUshortAddPos2(Pos);
                P.WriteUintAddPos4(I.TalismanProgress);
                P.WriteByteAddPos1((byte)I.Soc1);
                P.WriteByteAddPos1((byte)I.Soc2);
                P.WriteUshortAddPos2((ushort)I.Effect);
                P.WriteByteAddPos1(I.Plus);
                P.WriteByteAddPos1(I.Bless);
                if (I.FreeItem)
                    P.WriteByteAddPos1(1);
                else
                    P.WriteByteAddPos1(0);
                P.WriteByteAddPos1(I.Enchant);
                P.WriteUintAddPos4(0);
                P.WriteUshortAddPos2(0);// P.Move(6);
                if (I.Locked == 1)
                    P.WriteUshortAddPos2(1);
                else if (I.Locked == 2)
                    P.WriteUshortAddPos2(1);
                else
                    P.WriteUshortAddPos2(0);
                if (I.Color == 0)
                    I.Color = (Game.Item.ArmorColor)new Random().Next(1, 9);
                P.WriteUintAddPos4((uint)I.Color);
                P.WriteUintAddPos4(I.Progress);
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
            return P.AddTQServer8Byte();
        }
        public static byte[] CharacterInfo(Game.Character C)
        {
            byte[] Packet = new byte[98 + (C.Spouse.Length) + (C.Name + C.MyClient.AuthInfo.Status).Length + 4];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3ee);
            P.WriteUintAddPos4(C.EntityID);
            P.WriteUintAddPos4(C.Mesh);
            P.WriteUshortAddPos2(C.Hair);
            P.WriteUintAddPos4(C.Silvers);
            P.WriteUintAddPos4(C.CPs);
            P.WriteUlongAddPos8((ulong)C.Experience);
            // P.Move(20);
            P.WriteUlongAddPos8(0);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(C.Str);
            P.WriteUshortAddPos2(C.Agi);
            P.WriteUshortAddPos2(C.Vit);
            P.WriteUshortAddPos2(C.Spi);
            P.WriteUshortAddPos2(C.StatusPoints);
            P.WriteUshortAddPos2((ushort)C.CurHP);
            P.WriteUshortAddPos2((ushort)C.CurMP);
            P.WriteUshortAddPos2(C.PKPoints);
            P.WriteByteAddPos1(C.Level);
            P.WriteByteAddPos1(C.Job);
            P.WriteByteAddPos1(0);//0
            P.WriteByteAddPos1(C.Reborns);
            P.WriteByteAddPos1(0);//0
            P.WriteUintAddPos4(C.UniversityPoints);
            P.WriteUshortAddPos2(0);
            P.WriteUshortAddPos2(C.EnhligtehnRequest);//elightenicon at recivied 100= 20 min 200 = 40 min ....
            P.WriteUintAddPos4(C.Equips.Steed.TalismanProgress);
            P.WriteUintAddPos4(C.Equips.Steed.TalismanProgress); //P.Move(8);
            P.WriteByteAddPos1(2);
            P.WriteByteAddPos1((byte)(C.Name + C.MyClient.AuthInfo.Status).Length);
            P.WriteString(C.Name + C.MyClient.AuthInfo.Status);
            P.WriteByteAddPos1((byte)C.Spouse.Length);
            P.WriteString(C.Spouse);

            return P.AddTQServer8Byte();
        }
        public static byte[] Skill(Game.Skill S)
        {
            byte[] Packet = new byte[8 + 12];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x44f);
            P.WriteUintAddPos4(S.Exp);
            P.WriteUshortAddPos2(S.ID);
            P.WriteUshortAddPos2(S.Lvl);

            return P.AddTQServer8Byte();
        }
        public static byte[] Prof(Game.Prof Prof)
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x401);
            P.WriteUintAddPos4(Prof.ID);
            P.WriteUintAddPos4(Prof.Lvl);
            P.WriteUintAddPos4(Prof.Exp);

            return P.AddTQServer8Byte();
        }
        public static byte[] Packet1012(uint UID)
        {
            byte[] Packet = new byte[8 + 32];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1012);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(0); //P.Move(4);
            byte[] bb = new byte[16];
            for (int i = 0; i < 16; i++)
                bb[i] = (byte)Program.Rnd.Next(255);
            P.WriteBytes(bb);
            P.WriteUintAddPos4(0);// P.Move(4);

            return P.AddTQServer8Byte();
        }
        public static byte[] Packet1025()
        {
            byte[] Packet = new byte[8 + 16];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1025);
            P.WriteUintAddPos4(0); //P.Move(4);
            P.WriteUintAddPos4(1);
            //P.Move(4);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] Packet1012Time(uint UID)
        {
            byte[] Packet = new byte[8 + 32];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1012);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(Native.timeGetTime());
            byte[] bb = new byte[16];
            for (int i = 0; i < 16; i++)
                bb[i] = (byte)Program.Rnd.Next(255);
            P.WriteBytes(bb);
            P.WriteUintAddPos4(0xf76d);

            return P.AddTQServer8Byte();
        }

        public static byte[] SpawnInvisibleEntity(ushort X, ushort Y, ref uint UID)
        {
            UID = (uint)(new Random().Next() % 1000000);
            COPacket packet = new COPacket(new byte[28 + 8]);
            packet.WriteUshortAddPos2(28);
            packet.WriteUshortAddPos2(1109);
            packet.WriteUintAddPos4(UID);
            packet.WriteUintAddPos4(0);
            packet.WriteUintAddPos4(0);
            packet.WriteUshortAddPos2(X);
            packet.WriteUshortAddPos2(Y);
            packet.WriteUshortAddPos2(371);
            packet.WriteUshortAddPos2(26);
            packet.WriteByteAddPos1(11);
            packet.WriteByteAddPos1(1);
            packet.WriteByteAddPos1(1);
            packet.WriteString(" ");
            packet.WriteString("TQServer");
            return packet.AddTQServer8Byte();
        }
        public static byte[] CoordonateEffect(ushort X, ushort Y, string Effect)
        {
            uint UID = 0;
            byte[] packet1 = SpawnInvisibleEntity(X, Y, ref UID);
            byte[] packet2 = String(UID, 10, Effect);
            byte[] buffer = new byte[packet1.Length + packet2.Length];
            Buffer.BlockCopy(packet1, 0, buffer, 0, packet1.Length);
            Buffer.BlockCopy(packet2, 0, buffer, packet1.Length, packet2.Length);
            COPacket packet = new COPacket(buffer);
            return packet.AddTQServer8Byte();
        }

        public static byte[] Status56(uint UID, Game.Status Type, ulong Value)
        {
            byte[] Packet = new byte[8 + 48];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10017);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(0x02);
            P.WriteUintAddPos4(10000000);
           // P.Move(8);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4((uint)Type);
            P.WriteUlongAddPos8((ulong)Value);
            //P.Move(8);
            P.WriteUlongAddPos8(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] String(uint UID, byte Type, string str)
        {
            byte[] Packet = new byte[8 + 13 + str.Length];
            COPacket P = new COPacket(Packet);

            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3f7);
            P.WriteUintAddPos4(UID);
            P.WriteByteAddPos1(Type);
            P.WriteByteAddPos1(1);
            P.WriteByteAddPos1((byte)str.Length);
            P.WriteString(str);
            P.WriteUshortAddPos2(0);
            return P.AddTQServer8Byte();
        }
        public static byte[] Status(uint UID, Game.Status Type, ulong Value)
        {
            byte[] Packet = new byte[8 + 36];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10017);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(0x01);
            P.WriteUintAddPos4((uint)Type);
            P.WriteUlongAddPos8((ulong)Value);
            // P.Move(12);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] Status2(uint UID, Game.Status Type, ulong Value, byte Type2)
        {
            byte[] Packet = new byte[8 + 36];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)10017);
            P.WriteUintAddPos4(UID);
            P.WriteUintAddPos4(Type2);
            P.WriteUintAddPos4((uint)Type);
            P.WriteUlongAddPos8((ulong)Value);
           // P.Move(12);
            P.WriteUlongAddPos8(0);
            P.WriteUintAddPos4(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] Time()
        {
            byte[] Packet = new byte[8 + 36];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1033);
            //P.Move(4);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4((uint)(DateTime.Now.Year - 1900));
            P.WriteUintAddPos4((uint)(DateTime.Now.Month - 1));
            P.WriteUintAddPos4((uint)(DateTime.Now.DayOfYear));
            P.WriteUintAddPos4((uint)(DateTime.Now.Day));
            P.WriteUintAddPos4((uint)(DateTime.Now.Hour));
            P.WriteUintAddPos4((uint)(DateTime.Now.Minute));
            P.WriteUintAddPos4((uint)(DateTime.Now.Second));

            return P.AddTQServer8Byte();
        }
        public static byte[] Vigor(uint Amount)
        {
            byte[] Packet = new byte[8 + 36];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1033);
            P.WriteUintAddPos4(2);
            P.WriteUintAddPos4(Amount);

            return P.AddTQServer8Byte();
        }

        public static byte[] ChatMessage(uint MessageID, string From, string To, string Message, ushort Type, uint Mesh, System.Drawing.Color color)
        {
            byte[] Packet = new byte[8 + 34 + Message.Length + From.Length + To.Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3ec);
            P.WriteUintAddPos4((uint)color.ToArgb());
            P.WriteUintAddPos4(Type);
            P.WriteUintAddPos4(MessageID);
            P.WriteUintAddPos4(Mesh);
            P.WriteUintAddPos4(Mesh);
            P.WriteByteAddPos1(4);
            P.WriteByteAddPos1((byte)From.Length);
            P.WriteString(From);
            P.WriteByteAddPos1((byte)To.Length);
            P.WriteString(To);
            // P.Move(1);
            P.WriteByteAddPos1(0);
            if (Message.Length < 255)
                P.WriteByteAddPos1((byte)(Message.Length));
            else
                P.WriteByteAddPos1(255);

            P.WriteString(Message, 255);
            //P.Move(6);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(0);

            return P.AddTQServer8Byte();
        }
        public static byte[] SystemMessage(uint MessageID, string Message)
        {
            byte[] Packet = new byte[8 + 50 + Message.Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)1004);
            P.WriteBytes(new byte[] { 0xff, 0xff, 0xff, 0x00 });
            P.WriteUintAddPos4(0x835);
            P.WriteUintAddPos4(MessageID);
            //P.Move(8);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(4);
            P.WriteByteAddPos1(6);
            P.WriteString("SYSTEM");
            P.WriteByteAddPos1(8);
            P.WriteString("ALLUSERS");
           // P.Move(1);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1((byte)Message.Length);
            P.WriteString(Message);
            //P.Move(7);
            P.WriteUintAddPos4(0);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(1);

            return P.AddTQServer8Byte();
        }
        public static byte[] PopUpMessage(uint MessageID, string Message)
        {
            byte[] Packet = new byte[8 + 43 + Message.Length];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length - 8));
            P.WriteUshortAddPos2((ushort)0x3ec);
            P.WriteBytes(new byte[] { 0xff, 0xff, 0xff, 0x00 });
            P.WriteUintAddPos4(0x834);
            P.WriteUintAddPos4(MessageID);
            //P.Move(8);
            P.WriteUlongAddPos8(0);
            P.WriteByteAddPos1(4);
            P.WriteByteAddPos1(6);
            P.WriteString("SYSTEM");
            P.WriteByteAddPos1(8);
            P.WriteString("ALLUSERS");
           // P.Move(1);
            P.WriteByteAddPos1(0);
            P.WriteByteAddPos1((byte)Message.Length);
            P.WriteString(Message);
            P.WriteUshortAddPos2(0);
            P.WriteByteAddPos1(0);

            return P.AddTQServer8Byte();
        }

        public static COPacket SendAuthentication(string ip, byte[] IV)
        {
            byte[] Packet = new byte[32];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length));//2
            P.WriteUshortAddPos2(0x41f);//4
            P.WriteBytes(IV);//12
            P.WriteString(ip);//
            P.Move(16 - ip.Length);//28
            P.WriteUshortAddPos2(5816);//30
            return P;
        }
        public static COPacket WrongAuth()
        {
            byte[] Packet = new byte[32];
            COPacket P = new COPacket(Packet);
            P.WriteUshortAddPos2((ushort)(Packet.Length));
            P.WriteUshortAddPos2(0x41f);
            P.WriteUintAddPos4(0);
            P.WriteUintAddPos4((uint)1);
            P.WriteByteAddPos1(0xd5);
            P.WriteByteAddPos1(0xca);
            P.WriteByteAddPos1(0xba);
            P.WriteByteAddPos1(0xc5);
            P.WriteByteAddPos1(0xc3);
            P.WriteByteAddPos1(0xfb);
            P.WriteByteAddPos1(0xbb);
            P.WriteByteAddPos1(0xf2);
            P.WriteByteAddPos1(0xbf);
            P.WriteByteAddPos1(0xda);
            P.WriteByteAddPos1(0xc1);
            P.WriteByteAddPos1(0xee);
            P.WriteByteAddPos1(0xb4);
            P.WriteByteAddPos1(0xed);

            return P;
        }
    }
}
