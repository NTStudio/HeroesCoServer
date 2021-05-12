using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Features
{
    public class Marriage
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            uint AttackType = BitConverter.ToUInt32(Data, 20);

            if (AttackType == 8)
            {
                uint TargetUID = BitConverter.ToUInt32(Data, 12);
                Game.Character TargetMarry = null;

                if (Game.World.H_Chars.Contains(TargetUID))
                    TargetMarry = (Game.Character)Game.World.H_Chars[TargetUID];

                if (GC.MyChar.Spouse != "None")
                { GC.LocalMessage(2005, System.Drawing.Color.Red , "You are married."); return; }
                if (TargetMarry.Spouse != "None")
                { GC.LocalMessage(2005, System.Drawing.Color.Red , "The target is married."); return; }
                if ((TargetMarry.Body == 1003 || TargetMarry.Body == 1004) && (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004))
                { GC.LocalMessage(2005, System.Drawing.Color.Red , "No male and male marriage is permited!"); return; }
                if ((TargetMarry.Body == 2001 || TargetMarry.Body == 2002) && (GC.MyChar.Body == 2001 || GC.MyChar.Body == 2002))
                { GC.LocalMessage(2005, System.Drawing.Color.Red , "No female and female marriage is permited!"); return; }

                TargetMarry.MyClient.SendPacket(Packets.AttackPacket(GC.MyChar.EntityID, TargetMarry.EntityID, TargetMarry.Loc.X, TargetMarry.Loc.Y, 0, 8));
            }
            else if (AttackType == 9)
            {
                uint TargetUID = BitConverter.ToUInt32(Data, 12);
                Game.Character TargetMarry = null;

                if (Game.World.H_Chars.Contains(TargetUID))
                    TargetMarry = (Game.Character)Game.World.H_Chars[TargetUID];

                TargetMarry.Spouse = GC.MyChar.Name;
                GC.MyChar.Spouse = TargetMarry.Name;

                TargetMarry.MyClient.SendPacket(Packets.String(TargetMarry.EntityID, 6, TargetMarry.Spouse));
                GC.SendPacket(Packets.String(GC.MyChar.EntityID, 6, GC.MyChar.Spouse));

                Database.SaveCharacter(TargetMarry, TargetMarry.MyClient.AuthInfo.Account);
                Database.SaveCharacter(GC.MyChar, GC.AuthInfo.Account);
                if (GC.MyChar.Body == 1003 || GC.MyChar.Body == 1004)
                    Game.World.WorldMessage("SYSTEM", GC.MyChar.Name + " and " + TargetMarry.Name + " are married now.", 2011, 0, System.Drawing.Color.Red);
                else
                    Game.World.WorldMessage("SYSTEM", TargetMarry.Name + " and " + GC.MyChar.Name + " are married now.", 2011, 0, System.Drawing.Color.Red);
            }
        }
    }
}
