using System;
using System.Collections;
using System.IO;
using Server.Game;
using System.Threading;

namespace Server.PacketHandling
{
    public enum ChatTypes : ushort
    {
        Talk = 2000,
        Whisper = 2001,
        Action = 2002,
        Team = 2003,
        Guild = 2004,
        Top = 2005,
        Spouse = 2006,
        SYSTem = 2007,
        clan = 2004,
        tip = 2015,
        Yell = 2008,
        Friend = 2009,
        Broadcast = 2010,
        Center = 2011,
        Ghost = 2013,
        Service = 2014,
        World = 2021,
        Dialog = 2100,
        LoginInformation = 2101,
        VendorHawk = 2104,
        TopRight = 2109,
        ClearTopRight = 2108,
        FriendsOfflineMessage = 2110,
        GuildBulletin = 2111,
        TradeBoard = 2201,
        FriendBoard = 2202,
        NewBroadcast = 2500,
        TeamBoard = 2203,
        GuildBoard = 2204,
        OthersBoard = 2205
    }

    public class Chat
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            ChatTypes ChatType = (ChatTypes)((Data[9] << 8) + Data[8]);
            int Positions = 26;
            int Len = 0;
            string From = "";
            string To = "";
            string Message = "";
            for (int C = 0; C < Data[25]; C++)
            {
                From += Convert.ToChar(Data[Positions]);
                Positions++;
            }
            Len = Data[Positions];
            Positions++;
            for (int C = 0; C < Len; C++)
            {
                To += Convert.ToChar(Data[Positions]);
                Positions++;
            }
            Positions++;
            Len = Data[Positions];
            Positions++;
            for (int C = 0; C < Len; C++)
            {
                Message += Convert.ToChar(Data[Positions]);
                Positions++;
            }
            #region BadWords
            /*Message = Message.Replace("damn", "****");
                                Message = Message.Replace("fuck", "****");
                                Message = Message.Replace("shit", "****");
                                Message = Message.Replace("stupid", "******");
                                Message = Message.Replace("fucker", "******");*/
            #endregion
            if (ChatType == ChatTypes.VendorHawk && GC.MyChar.MyShop != null)
                GC.MyChar.MyShop.Hawk = Message;
            try
            {
                if (Message[0] == '/')
                {
                    string[] Cmd = Message.Split(' ');
                    {
                        #region Avatar
                        if (Cmd[0] == "//ava")
                        {
                            string Name = Cmd[1];
                            string Account = "";
                            Program.WriteMessage("requested");
                            if (Game.World.CharacterFromName(Name) == null)
                            {
                                Game.Avatar R = (Avatar)Database.LoadAvatar(Name, ref Account, true);
                                if (R != null)
                                {
                                    Program.WriteMessage("in1");
                                    R.Initalize(Account);
                                    Program.WriteMessage("in2 > ");
                                    R.Initaliz(GC.MyChar);
                                   
                                    // Database.SaveCharacter(R, Account);
                                }
                            }
                        }
                        if (Cmd[0] == "//npcava")
                        {
                            GC.DialogNPC = 1020300;
                        }
                        if (Cmd[0] == "//play")
                            GC.MyChar.MyAvatar.training = !GC.MyChar.MyAvatar.training;
                        if (Cmd[0] == "//off")
                        {
                            GC.MyChar.MyAvatar.MyClient.Disconnect();
                            GC.MyChar.MyAvatar = null;
                        }
                        if (Cmd[0] == "//hunt")
                            GC.MyChar.MyAvatar.Process = "Hunting";

                        if (Cmd[0] == "//come")
                            GC.MyChar.MyAvatar.Process = "";

                        if (Cmd[0] == "//pk")
                        {
                            GC.MyChar.ownerpk = !GC.MyChar.ownerpk;
                        }
                        if (Cmd[0] == "//speed")
                        {
                            GC.MyChar.MyAvatar.jumpSpeed = int.Parse(Cmd[1]);
                            if (GC.MyChar.MyAvatar.jumpSpeed < 400)
                                GC.MyChar.MyAvatar.jumpSpeed = 400;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }
    }
}
