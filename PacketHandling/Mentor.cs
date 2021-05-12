using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    class Mentor
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
                /*string P = ""; string Phex = "";
                for (byte bit = 0; bit < Data.Length - 8; bit++)
                {
                    int Pi = Data[bit];
                    P += Data[bit] + " ";
                    Phex += Pi.ToString("X") + " ";
                }
                Console.WriteLine("packet: {0} ", P);
                Console.WriteLine("hex Packet: " + Phex);
                */
            // PacketHandling.mentors.Handle(GC, Data);
            uint UID = BitConverter.ToUInt32(Data, 8);//8=???????
            uint UIDtosend = BitConverter.ToUInt32(Data, 12);

            //Console.WriteLine(UIDtosend);
           // Console.WriteLine(UID);
            uint Type = Data[8];
            uint Type2 = Data[4];
           // Console.WriteLine("UID{0}", UID);

           // Console.WriteLine(UID);
            Game.Character Request = (Game.Character)Game.World.H_Chars[UIDtosend];
            switch (Type)
            {
                case 161:
                    {
                            switch (Type2)//RequestApprentice
                            {
                                case 2:
                                    //  Game.Character Who = (Game.Character)World.H_Chars[UID];
                                    //{
                                    //15 = AddFriend, 19 = AddEnemy, 14 = Remove
                                    {

                                        Request.MyClient.SendPacket(Packets.Mentor(GC.MyChar.Name, 11, GC.MyChar.EntityID, Request.EntityID, true));
                                        //GC.SendPacket(Packets.MentorApprenticePacket(GC.MyChar.EntityID, UID, Request.Name, 11, 1));
                                        break;
                                    }
                                case 1:
                                    //  Game.Character Who = (Game.Character)World.H_Chars[UID];
                                    //{
                                    //15 = AddFriend, 19 = AddEnemy, 14 = Remove
                                    {
                                        Request.MyClient.SendPacket(Packets.MentorApprenticePacket(UID, GC.MyChar.EntityID, GC.MyChar.Name, GC.MyChar.mtype, 1));                                        
                                        //GC.SendPacket(Packets.MentorApprenticePacket(GC.MyChar.EntityID, UID, Request.Name, 5, 1));
                                        break;
                                    }
                                // }
                            }

                            //Types 7=??, 11=Ask for a mentor, 14=Leave Mentor, 15=AddFriend, 18= Remove Enemy, 19 AddEnemy

                            // case 223:
                            //  else if (Type == 2)//RequestMentor
                            {
                                //   Request.EntityID = UID;
                                //   GC.SendPacket(Packets.Mentor(Request.Name, 11, GC.MyChar.EntityID, UID, 1));
                                /*   byte cont = 10;
                                   while (true)
                                   {
                                     System.Threading.Thread.Sleep(5000);
                                       Request.MyClient.SendPacket(Packets.Mentor(GC.MyChar.Name, cont, GC.MyChar.EntityID, Request.EntityID,true));
                                       cont++;
                                       Console.WriteLine("count ==== "+ cont );
                                   }
                                    GC.SendPacket(Packets.MentorApprenticePacket(GC.MyChar.EntityID, UID, Request.Name, 1, 1));
                                   break;*/
                            }
                            break;
                    }
                default:
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] Unknown 2065 Sub type : " + Type.ToString());
                        break;
                    }
                     
            }
        }
    }
}
