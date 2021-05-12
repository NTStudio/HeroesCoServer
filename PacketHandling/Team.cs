using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Game;
using Server.Features;

namespace Server.PacketHandling
{
    public class TeamHandle
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            byte Type = Data[4];
            switch (Type)
            {
                case 0:
                    {
                        GC.MyChar.MyTeam = new Team(GC.MyChar);
                        break;
                    }
                case 1: // Request to join
                    {
                        uint WhoUID = BitConverter.ToUInt32(Data, 8);
                        Character Who = (Character)World.H_Chars[WhoUID];
                        if (Who == null) return;

                        if (Who.TeamLeader)
                        {
                            if (Who.MyTeam.Members.Contains(GC.MyChar)) return;

                            if (!Who.MyTeam.Forbid)
                            {
                                if (Who.MyTeam.Members.Count < 5)
                                {
                                    Who.MyClient.SendPacket(Packets.TeamPacket(GC.MyChar.EntityID, 1));
                                    GC.LocalMessage(2005, System.Drawing.Color.Red , "[Team]Request to join team has been sent out.");
                                }
                                else
                                    GC.LocalMessage(2005, System.Drawing.Color.Red , "[Team]The team is full.");
                            }
                            else
                                GC.LocalMessage(2005, System.Drawing.Color.Red , "The team doesn't accept new members.");
                        }
                        else
                            GC.LocalMessage(2005, System.Drawing.Color.Red , "[Team]The target has not created a team.");

                        break;
                    }
                case 2:
                    {
                        if (GC.MyChar.MyTeam == null) return;
                        if (GC.MyChar.MyTeam.Members.Contains(GC.MyChar))
                            GC.MyChar.MyTeam.Leaves(GC.MyChar);
                        break;
                    }
                case 3:
                    {
                        uint WhoUID = BitConverter.ToUInt32(Data, 8);
                        Character Who = (Character)World.H_Chars[WhoUID];
                        if (Who == null) return;
                        if (!Who.TeamLeader) return;
                        if (GC.MyChar.TeamLeader) return;
                        if (Who.MyTeam == null) return;

                        Who.MyTeam.Joins(GC.MyChar);
                        break;
                    }
                case 4:
                    {
                        uint WhoUID = BitConverter.ToUInt32(Data, 8);
                        Character Who = (Character)World.H_Chars[WhoUID];
                        if (GC.MyChar.MyTeam == null) return;
                        if (!GC.MyChar.TeamLeader) return;

                        if (!Who.TeamLeader && !GC.MyChar.MyTeam.Members.Contains(Who))
                            Who.MyClient.SendPacket(Packets.TeamPacket(GC.MyChar.EntityID, 4));


                        break;
                    }
                case 5:
                    {
                        uint WhoUID = BitConverter.ToUInt32(Data, 8);
                        Character Who = (Character)World.H_Chars[WhoUID];
                        if (Who == null) return;
                        if (Who.TeamLeader) return;
                        if (GC.MyChar.MyTeam == null) return;
                        if (!GC.MyChar.TeamLeader) return;

                        GC.MyChar.MyTeam.Joins(Who);

                        break;
                    }
                case 6:// dismiss
                    {
                        if (!GC.MyChar.TeamLeader) return;
                        if (GC.MyChar.MyTeam == null) return;
                        GC.MyChar.MyTeam.Dismiss(GC.MyChar);

                        break;
                    }
                case 7:
                    {
                        uint WhoUID = BitConverter.ToUInt32(Data, 8);
                        Character Who = (Character)World.H_Chars[WhoUID];
                        if (!GC.MyChar.MyTeam.Members.Contains(Who)) return;
                        if (Who == GC.MyChar) return;
                        if (GC.MyChar.MyTeam == null) return;

                        GC.MyChar.MyTeam.Leaves(Who);

                        break;
                    }
                case 8:
                    {
                        GC.MyChar.MyTeam.Forbid = true;
                        break;
                    }
                case 9:
                    {
                        GC.MyChar.MyTeam.Forbid = false;
                        break;
                    }
            }
        }
    }
}
