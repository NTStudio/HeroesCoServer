using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;

namespace Server.PacketHandling
{
    public class Friends
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint UID = BitConverter.ToUInt32(Data, 4);
            byte Type = Data[8];
            Console.WriteLine(UID);
            switch (Type)
            {
                case 10://Add
                    {
                        if (!GC.MyChar.Friends.Contains(UID))
                        {
                            Character C = (Character)World.H_Chars[UID];
                            if (C != null)
                            {
                                if (C.RequestFriends != GC.MyChar.EntityID)
                                {
                                    GC.MyChar.RequestFriends = UID;
                                    GC.LocalMessage(2005, System.Drawing.Color.Red , "Request to make friends has been sent out.");
                                    C.MyClient.LocalMessage(2005, System.Drawing.Color.Red , GC.MyChar.Name + " wants to make friends with you.");
                                }
                                else
                                {
                                    Friend F = new Friend();
                                    F.Name = C.Name;
                                    F.UID = C.EntityID;
                                    Friend F2 = new Friend();
                                    F2.Name = GC.MyChar.Name;
                                    F2.UID = GC.MyChar.EntityID;

                                    GC.MyChar.Friends.Add(F.UID, F);
                                    C.Friends.Add(F2.UID, F2);

                                    //15 = AddFriend, 19 = AddEnemy, 14 = Remove
                                  //  World.Chat(GC.MyChar, 2005, "SYSTEM", "ALL", GC.MyChar.Name + " and " + C.Name + " are friends from now on.");
                                    GC.SendPacket(Packets.FriendEnemyPacket(F.UID, F.Name, 15, 1));
                                    C.MyClient.SendPacket(Packets.FriendEnemyPacket(F2.UID, F2.Name, 15, 1));//15
                                    Database.SaveFriends(F, GC.MyChar);
                                    Database.SaveFriends(F2, C);
                                }
                            }
                        }
                        break;
                    }
                case 14://Remove
                    {
                        if (GC.MyChar.Friends.Contains(UID))
                        {
                            Friend F = (Friend)GC.MyChar.Friends[UID];
                            if (F.Online)
                            {
                                Character C = F.Info;
                                if (C.Friends.Contains(GC.MyChar.EntityID))
                                {
                                    C.Friends.Remove(GC.MyChar.EntityID);
                                    C.MyClient.SendPacket(Packets.FriendEnemyPacket(GC.MyChar.EntityID, "", 14, 0));
                                }
                                GC.MyChar.Friends.Remove(C.EntityID);
                                //Database.DelFrends(F,C);
                            }
                            else
                            {
                                try
                                {
                                    string Acc = "";
                                    Character C = Database.LoadCharacter(F.Name, ref Acc,false);
                                    C.Friends.Remove(GC.MyChar.EntityID);
                                }
                                catch
                                {
                                    GC.MyChar.Friends.Remove(F.UID);
                                    GC.SendPacket(Packets.FriendEnemyPacket(F.UID, "", 14, 0));
                                }
                            }
                            GC.MyChar.Friends.Remove(F.UID);
                            GC.SendPacket(Packets.FriendEnemyPacket(F.UID, "", 14, 0));
                            Database.DeleteFriends(F, GC.MyChar);

                            GC.LocalMessage(2005, System.Drawing.Color.Red , GC.MyChar.Name + " has broken the friendship with " + F.Name + ".");
                  
                            //   World.Chat(GC.MyChar, 2005, "SYSTEM", "ALL", GC.MyChar.Name + " has broken the friendship with " + F.Name + ".");
                        }                        
                        break;
                    }
                case 18:
                    {
                        if (GC.MyChar.Enemies.Contains(UID))
                        {
                            GC.MyChar.Enemies.Remove(UID);
                            GC.SendPacket(Packets.FriendEnemyPacket(UID, "", 14, 0));
                            Database.DeleteEnemys(UID, GC.MyChar);
                        }
                        break;
                    }
            }
        }
    }
}
