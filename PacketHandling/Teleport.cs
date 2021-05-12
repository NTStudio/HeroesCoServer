using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    public class Teleport
    {
        public static void Handle(GameClient GC, byte[] Data)
        {
            try
            {
                if (!GC.LoginDataSent)
                {
                    GC.SendPacket(Packets.Packet1012Time(GC.MyChar.EntityID));
                    GC.SendPacket(Packets.GeneralData(GC.MyChar.Loc.Map, GC.MyChar.Loc.Map, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0x4a));
                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0xffffffff, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0x68));
                    if (GC.MyChar.Loc.Map == 1036)
                        GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 30));
                    else
                        GC.SendPacket(Packets.MapStatus(GC.MyChar.Loc.Map, 32));
                    try
                    {
                        GC.MyChar.Equips.Send(GC, true);
                    }
                    catch
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.SystemMessage(2000, "Erorr!!Try again to join"));
                        GC.Disconnect();
                        Game.World.H_Chars.Remove(GC.MyChar.EntityID);
                        Game.World.Action(GC.MyChar, Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 135));
                        GC.MyChar.MyClient = null;
                    }
                    foreach (Game.Prof P in GC.MyChar.Profs.Values)
                        GC.SendPacket(Packets.Prof(P));
                    foreach (Game.Skill S in GC.MyChar.Skills.Values)
                        GC.SendPacket(Packets.Skill(S));
                    if (GC.MyChar.MyGuild != null)
                    {
                        GC.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                        GC.SendPacket(Packets.String(GC.MyChar.MyGuild.GuildID, (byte)Game.StringType.GuildName, GC.MyChar.MyGuild.GuildName));
                    }
                    foreach (Features.Guild G in Features.Guilds.AllTheGuilds.Values)
                        GC.SendPacket(Packets.String(G.GuildID, (byte)Game.StringType.GuildName, G.GuildName));
                    try
                    {
                        GC.MyChar.PKPoints = GC.MyChar.PKPoints;
                    }
                    catch { }
                    Database.Clain(GC.MyChar);//confiscator
                    Database.Reward(GC.MyChar);//confiscator

                    if (GC.MyChar.Loc.Map == 601)
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.OffLineTgScreen(GC.MyChar.Level, 0));
                    }
                    //GC.MyChar.Nobility.Rank = GC.MyChar.Nobility.Rank;
                    if ((Game.World.H_Chars.Count >= Game.World.Playersmax)/* && (!Game.World.PlayersPool.ContainsKey(GC.MyChar.Name))*/)
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.SystemMessage(2000, "Sorry!Server is full (Players: " + Game.World.H_Chars.Count + "),you need slot"));
                        GC.Disconnect();
                        Game.World.H_Chars.Remove(GC.MyChar.EntityID);
                        Game.World.Action(GC.MyChar, Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 135));
                        GC.MyChar.MyClient = null;
                    }
                    if (GC.MyChar.banned == 1)
                    {
                        GC.MyChar.MyClient.SendPacket(Packets.SystemMessage(2000, "You Acc has Banned by " + GC.MyChar.BanBy + " "));
                        Program.WriteMessage("User " + GC.MyChar.Name + " disconnected. He is banned by " + GC.MyChar.BanBy + "");
                        Game.World.H_Chars.Remove(GC.MyChar.EntityID);
                        Game.World.Action(GC.MyChar, Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 135));
                        GC.MyChar.MyClient = null;
                    }
                    if (GC.MyChar.Loc.Map >= 10000)
                    { GC.MyChar.Teleport(1002, 429, 378); }
                    try
                    {
                        Database.LoadCharacterItems(GC.MyChar);
                    }
                    catch { Console.WriteLine("Try,again!!!"); }
                    try
                    {
                        GC.MyChar.MyClient = GC;
                    }
                    catch { GC.Soc.Disconnect(false); return; }
                    if (GC.MyChar.Loaded)
                    { }
                    else
                    { Console.WriteLine("Buggerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" + GC.MyChar.Name); GC.Disconnect(); return; }

                    #region Loging GuildEnemy
                    try
                    {
                        foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                        {
                            Features.Guild TGuild = Guilds.Value;
                            if (GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                            {
                                GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                            }
                        }
                    }
                    catch { }
                    #endregion
                    #region Logingn GuildAllis
                    try
                    {
                        foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                        {
                            Features.Guild TGuild = Guilds.Value;
                            if (GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                            {
                                GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 7));
                            }
                        }
                    }
                    catch { }
                    #endregion
                    #region Start Messages
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Welcome to " + Game.World.ServerInfo.ServerName + " .");
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "The official WebSit is " + Game.World.ServerInfo.WebSite + " .");
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "The server is still being fixed. Anyway, We want you to enjoy with US.");
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "You need help? Contact With (GameMaster[GM] Or PlayerMaster[PM]).");
                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Enjoy.");
                    #endregion
                    #region KOBoard Message
                    if (Game.World.KOBoard[0].Name == GC.MyChar.Name)
                    {
                        GC.LocalMessage(2005, System.Drawing.Color.Yellow, "You are the 1st place on the KO board, you gain EXPBall as a reward.");
                    }
                    #endregion
                    #region Current BroadCastMessage
                    if (Game.World.CurrentBC.Message != null)
                    {
                        GC.SendPacket(Packets.ChatMessage(GC.MessageID, Game.World.CurrentBC.Name, "ALL", Game.World.CurrentBC.Message, 2500, 0, System.Drawing.Color.White));
                    }
                    #endregion

                    if (GC.MyChar.BlessingLasts > 0)
                    {
                        GC.MyChar.StatEff.Add(Game.StatusEffectEn.Blessing);
                    }
                    //if (GC.MyChar.DoubleExp)
                    //    GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.DoubleExpTime, (ulong)GC.MyChar.DoubleExpLeft));

                    GC.MyChar.Stamina = 100;
                    Database.LoadPKStatus(GC.MyChar);
                    GC.MyChar.Nobility.Rank = GC.MyChar.Nobility.Rank;
                    GC.MyChar.SendScreen(Packets.Packet(PacketType.NobilityPacket, GC.MyChar));
                    GC.MyChar.CPSDonate = 215;
                    Game.World.Spawns(GC.MyChar, true);
                    //GC.MyChar.MyClient.SendPacket(Packets.AttackPacket(0, GC.MyChar.EntityID, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 1, 37));
                    GC.MyChar.MyClient.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, Game.World.ScreenColor, 0, 0, 104));
                }
            }
            catch (Exception e) { Program.WriteMessage(e); }
        }
    }
}