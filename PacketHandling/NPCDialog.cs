using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Server.Game;

namespace Server.PacketHandling
{
    public class NPCDialog
    {
        static string ReadString(byte[] Data)
        {
            string Name = "";
            for (int i = 14; i < 14 + Data[13]; i++)
                Name += Convert.ToChar(Data[i]);
            return Name;
        }

        public static void NPCText(string Text, GameClient GC)
        {
            GC.SendPacket(Packets.NPCText(Text));
        }
        public static void NPCLink(string Text, byte Option, GameClient GC)
        {
            GC.SendPacket(Packets.NPCLink(Text, Option));
        }
        public static void NPCLink2(string Text, byte Option, GameClient GC)
        {
            GC.SendPacket(Packets.NPCLink2(Text, Option));
        }
        public static void NPCFace(ushort ID, GameClient GC)
        {
            GC.SendPacket(Packets.NPCFace(ID));
        }
        public static void NPCFinish(GameClient GC)
        {
            GC.SendPacket(Packets.NPCFinish());
        }

        public static void Handles(GameClient GC, byte[] Data, uint NPC, byte option)
        {
            Random Rnd = new Random();
            try
            {
                Game.NPC N = (Game.NPC)Game.World.H_NPCs[NPC];
                #region Check Warehouse Pass
                if (NPC == 8 || NPC == 12 || NPC == 10012 || NPC == 10028 || NPC == 10011 || NPC == 10027 || NPC == 44)
                {
                    if (!GC.MyChar.WHOpen && GC.MyChar.WHPassword != "0")
                    {
                        if (GC.MyChar.WHPassword == ReadString(Data))
                        { GC.MyChar.WHOpen = true; }
                        else
                        {
                            if (GC.MyChar.WHOpenChance >= 1)
                                GC.MyChar.WHOpenChance++;
                        }

                        if (GC.MyChar.WHOpenChance < 4 && !GC.MyChar.WHOpen)
                        {
                            if (GC.MyChar.WHOpenChance == 0)
                            {
                                GC.MyChar.WHOpenChance = 1;
                                NPCText("Please put your warehouse password to open it.", GC);
                            }
                            else
                            {
                                NPCText("Wrong! You have more " + GC.MyChar.WHOpenChance + " times to try.", GC);
                            }
                            NPCLink2("Password.", 1, GC);
                            NPCLink("Let me think.", 255, GC);
                            NPCFace(N.Avatar, GC);
                            NPCFinish(GC);
                            return;
                        }
                        else if (GC.MyChar.WHOpenChance == 4)
                        {
                            NPCText("You have to logoff and login to try it again.", GC);
                            NPCLink("I See.", 255, GC);
                            NPCFace(N.Avatar, GC);
                            NPCFinish(GC);
                        }
                    }
                    else if (GC.MyChar.WHPassword == "")
                        GC.LocalMessage(2005, System.Drawing.Color.Red, "To protect your items, please put a password in the WHSGuardian in TwinCity.");
                }
                #endregion

                if (option != 255 && GC.MyChar.Alive)
                {
                    switch (NPC)
                    {
                        #region Birth Vilage
                        #region RecruitMaster
                        case 5000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hellow, Welcome To HeroesCOnline, Are You Ready To Enter The New World ?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah Iam Ready.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    GC.MyChar.Teleport(1010, 090, 075);
                                }
                                break;
                            }
                        #endregion
                        #region VillageGateman
                        case 5010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Wellcome, I am agateman You must choose what you want make about your Account."));
                                    GC.SendPacket(Packets.NPCText("You Have Just Two Choise It Is [NormalCharacetr OR Avatar]."));
                                    GC.SendPacket(Packets.NPCLink("NormalCharacter.", 10));
                                    GC.SendPacket(Packets.NPCLink("Avatar.", 20));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("Well, You Choose To Make A NormalCharacetr, Are You Sure From That?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah Do It.", 11));
                                    GC.SendPacket(Packets.NPCLink("Just Passin By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 11)
                                {
                                    GC.SendPacket(Packets.NPCText("Now You Will Choosing The CharacterClass, You Can Choose One Class Between This Classes."));
                                    //GC.SendPacket(Packets.NPCSay("But There Is Note Here Every Class Of This Classes Have His Own EmpireCity, "
                                    //    + " Means That [(Warrior In DesertCity) , (Wizard In PhoenixCity) , (Archer In BirdIsland) And (Ninja In ApeCity)"));
                                    GC.SendPacket(Packets.NPCLink("Warrior.", 12));
                                    GC.SendPacket(Packets.NPCLink("Wizard.", 13));
                                    GC.SendPacket(Packets.NPCLink("Archer.", 14));
                                    GC.SendPacket(Packets.NPCLink("Ninja.", 15));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 12 && option <= 15)
                                {
                                    //if (option == 12)
                                    //{ GC.MyChar.Job = 10; GC.MyChar.Teleport(1000, 485, 634); }
                                    //else if (option == 13)
                                    //{ GC.MyChar.Job = 100; GC.MyChar.Teleport(1011, 210, 260); }
                                    //else if (option == 14)
                                    //{ GC.MyChar.Job = 40; GC.MyChar.Teleport(1015, 737, 547); }
                                    //else if (option == 15)
                                    //{ GC.MyChar.Job = 50; GC.MyChar.Teleport(1020, 560, 543); }

                                    if (option == 12)
                                    {
                                        GC.MyChar.Job = 20;
                                    }
                                    else if (option == 13)
                                    {
                                        GC.MyChar.Job = 100;
                                    }
                                    else if (option == 14)
                                    {
                                        GC.MyChar.Job = 40;
                                    }
                                    else if (option == 15)
                                    {
                                        GC.MyChar.Job = 50;
                                    }
                                    GC.MyChar.Teleport(1002, 439, 388);

                                    GC.SendPacket(Packets.NPCText("Welcome To HeroesCOnline."));
                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 20)
                                {
                                    GC.SendPacket(Packets.NPCText("Well, You Choose To Make A Avatar, Are You Sure From That?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah Do It.", 21));
                                    GC.SendPacket(Packets.NPCLink("Just Passin By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 21)
                                {
                                    GC.MyChar.Job = 160;
                                    GC.MyChar.Teleport(1037, 226, 230);
                                    GC.SendPacket(Packets.NPCText("Welcome To HeroesCOnline."));
                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region BeginnerQuest TC
                        case 5520:
                            {
                                if (option == 0)
                                {
                                    if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15) || (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25))
                                    {
                                        if (GC.MyChar.AbleToHunt == false)
                                        {
                                            if (GC.MyChar.Level >= 1 && GC.MyChar.Level < 120)
                                            {
                                                GC.SendPacket(Packets.NPCText("Hello, [" + GC.MyChar.Name + "] I am Your Captain, Your Level Is [" + GC.MyChar.Level + "] So I Can Help You To Get Some EXP For Your Levels (1 To 120), All You Must To Do Kill Some Monsters For Me And I`ll Give You A Rewards, You Will ?"));
                                                GC.SendPacket(Packets.NPCLink("Yeah, I`ll.", 1));
                                                GC.SendPacket(Packets.NPCLink("No, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Level Higher Than 120, And You Have Made The Land Peaceful! There Is Nothing Left For You To Do."));
                                                GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.AbleToHunt == true && GC.MyChar.MonsterHunted >= 100)
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Finished Your Quest, Do You Want Claim Your Reward."));
                                            GC.SendPacket(Packets.NPCLink("Yeah, I Want Claim My Reward", 5));
                                            GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Have Quest, Go Continue Your Quest."));
                                            GC.SendPacket(Packets.NPCLink("Okay", 255));
                                            GC.SendPacket(Packets.NPCLink("Quit My Quest", 10));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay You, It Is Not You City Get Out."));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Here We Are You Must Kill The Monsters To Made The Land More Peaceful."));
                                    GC.SendPacket(Packets.NPCLink("I Accept The Quest.", 2));
                                    GC.SendPacket(Packets.NPCLink("No, I am A Fraid.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2 && GC.MyChar.Level <= 70)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 100)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 110)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 120)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 5)
                                {
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.MyChar.CPs += 1000;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You Got 1K CPs And 2 EXPBalls");
                                }
                                if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("You Are Sure You Want Quit Your Quest, You Killd [" + GC.MyChar.MonsterHunted + "/100 From (" + GC.MyChar.MonsterName + ") ] Go Continue Your Quest."));
                                    GC.SendPacket(Packets.NPCLink("Yes, Quit The Quest.", 15));
                                    GC.SendPacket(Packets.NPCLink("No, Thanks.", 15));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 15)
                                {
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Your Quest Has Quit");
                                }
                                break;
                            }
                        #endregion
                        #region BeginnerQuest PC
                        case 5521:
                            {
                                if (option == 0)
                                {
                                    if (GC.MyChar.Job >= 100 && GC.MyChar.Job <= 145)
                                    {
                                        if (GC.MyChar.AbleToHunt == false)
                                        {
                                            if (GC.MyChar.Level >= 1 && GC.MyChar.Level < 120)
                                            {
                                                GC.SendPacket(Packets.NPCText("Hello, [" + GC.MyChar.Name + "] I am Your Captain, Your Level Is [" + GC.MyChar.Level + "] So I Can Help You To Get Some EXP For Your Levels (1 To 120), All You Must To Do Kill Some Monsters For Me And I`ll Give You A Rewards, You Will ?"));
                                                GC.SendPacket(Packets.NPCLink("Yeah, I`ll.", 1));
                                                GC.SendPacket(Packets.NPCLink("No, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Level Higher Than 120, And You Have Made The Land Peaceful! There Is Nothing Left For You To Do."));
                                                GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.AbleToHunt == true && GC.MyChar.MonsterHunted >= 100)
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Finished Your Quest, Do You Want Claim Your Reward."));
                                            GC.SendPacket(Packets.NPCLink("Yeah, I Want Claim My Reward", 5));
                                            GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Have Quest, Go Continue Your Quest."));
                                            GC.SendPacket(Packets.NPCLink("Okay", 255));
                                            GC.SendPacket(Packets.NPCLink("Quit My Quest", 10));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay You, It Is Not You City Get Out."));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Here We Are You Must Kill The Monsters To Made The Land More Peaceful."));
                                    GC.SendPacket(Packets.NPCLink("I Accept The Quest.", 2));
                                    GC.SendPacket(Packets.NPCLink("No, I am A Fraid.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2 && GC.MyChar.Level <= 70)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 100)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 110)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 120)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 5)
                                {
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.MyChar.CPs += 1000;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You Got 1K CPs And 2 EXPBalls");
                                }
                                if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("You Are Sure You Want Quit Your Quest, You Killd [" + GC.MyChar.MonsterHunted + "/100 From (" + GC.MyChar.MonsterName + ") ] Go Continue Your Quest."));
                                    GC.SendPacket(Packets.NPCLink("Yes, Quit The Quest.", 15));
                                    GC.SendPacket(Packets.NPCLink("No, Thanks.", 15));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 15)
                                {
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Your Quest Has Quit");
                                }
                                break;
                            }
                        #endregion
                        #region BeginnerQuest AC
                        case 5522:
                            {
                                if (option == 0)
                                {
                                    if ((GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55) || (GC.MyChar.Job >= 60 && GC.MyChar.Job <= 65))
                                    {
                                        if (GC.MyChar.AbleToHunt == false)
                                        {
                                            if (GC.MyChar.Level >= 1 && GC.MyChar.Level < 120)
                                            {
                                                GC.SendPacket(Packets.NPCText("Hello, [" + GC.MyChar.Name + "] I am Your Captain, Your Level Is [" + GC.MyChar.Level + "] So I Can Help You To Get Some EXP For Your Levels (1 To 120), All You Must To Do Kill Some Monsters For Me And I`ll Give You A Rewards, You Will ?"));
                                                GC.SendPacket(Packets.NPCLink("Yeah, I`ll.", 1));
                                                GC.SendPacket(Packets.NPCLink("No, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Level Higher Than 120, And You Have Made The Land Peaceful! There Is Nothing Left For You To Do."));
                                                GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.AbleToHunt == true && GC.MyChar.MonsterHunted >= 100)
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Finished Your Quest, Do You Want Claim Your Reward."));
                                            GC.SendPacket(Packets.NPCLink("Yeah, I Want Claim My Reward", 5));
                                            GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Have Quest, Go Continue Your Quest."));
                                            GC.SendPacket(Packets.NPCLink("Okay", 255));
                                            GC.SendPacket(Packets.NPCLink("Quit My Quest", 10));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay You, It Is Not You City Get Out."));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Here We Are You Must Kill The Monsters To Made The Land More Peaceful."));
                                    GC.SendPacket(Packets.NPCLink("I Accept The Quest.", 2));
                                    GC.SendPacket(Packets.NPCLink("No, I am A Fraid.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2 && GC.MyChar.Level <= 70)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 100)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 110)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 120)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 5)
                                {
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.MyChar.CPs += 1000;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You Got 1K CPs And 2 EXPBalls");
                                }
                                if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("You Are Sure You Want Quit Your Quest, You Killd [" + GC.MyChar.MonsterHunted + "/100 From (" + GC.MyChar.MonsterName + ") ] Go Continue Your Quest."));
                                    GC.SendPacket(Packets.NPCLink("Yes, Quit The Quest.", 15));
                                    GC.SendPacket(Packets.NPCLink("No, Thanks.", 15));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 15)
                                {
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Your Quest Has Quit");
                                }
                                break;
                            }
                        #endregion
                        #region BeginnerQuest DC
                        case 5523:
                            {
                                if (option == 0)
                                {
                                    if ((GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15) || (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25))
                                    {
                                        if (GC.MyChar.AbleToHunt == false)
                                        {
                                            if (GC.MyChar.Level >= 1 && GC.MyChar.Level < 120)
                                            {
                                                GC.SendPacket(Packets.NPCText("Hello, [" + GC.MyChar.Name + "] I am Your Captain, Your Level Is [" + GC.MyChar.Level + "] So I Can Help You To Get Some EXP For Your Levels (1 To 120), All You Must To Do Kill Some Monsters For Me And I`ll Give You A Rewards, You Will ?"));
                                                GC.SendPacket(Packets.NPCLink("Yeah, I`ll.", 1));
                                                GC.SendPacket(Packets.NPCLink("No, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Level Higher Than 120, And You Have Made The Land Peaceful! There Is Nothing Left For You To Do."));
                                                GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.AbleToHunt == true && GC.MyChar.MonsterHunted >= 100)
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Finished Your Quest, Do You Want Claim Your Reward."));
                                            GC.SendPacket(Packets.NPCLink("Yeah, I Want Claim My Reward", 5));
                                            GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Have Quest, Go Continue Your Quest."));
                                            GC.SendPacket(Packets.NPCLink("Okay", 255));
                                            GC.SendPacket(Packets.NPCLink("Quit My Quest", 10));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay You, It Is Not You City Get Out."));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Here We Are You Must Kill The Monsters To Made The Land More Peaceful."));
                                    GC.SendPacket(Packets.NPCLink("I Accept The Quest.", 2));
                                    GC.SendPacket(Packets.NPCLink("No, I am A Fraid.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2 && GC.MyChar.Level <= 70)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "SandMonster 1-70";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 100)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "SandMonster 70-100";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 110)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "SandMonster 100-110";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 120)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "SandMonster 110-120";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 5)
                                {
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.MyChar.CPs += 1000;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You Got 1K CPs And 2 EXPBalls");
                                }
                                if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("You Are Sure You Want Quit Your Quest, You Killd [" + GC.MyChar.MonsterHunted + "/100 From (" + GC.MyChar.MonsterName + ") ] Go Continue Your Quest."));
                                    GC.SendPacket(Packets.NPCLink("Yes, Quit The Quest.", 15));
                                    GC.SendPacket(Packets.NPCLink("No, Thanks.", 15));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 15)
                                {
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Your Quest Has Quit");
                                }
                                break;
                            }
                        #endregion
                        #region BeginnerQuest BC
                        case 5524:
                            {
                                if (option == 0)
                                {
                                    if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                    {
                                        if (GC.MyChar.AbleToHunt == false)
                                        {
                                            if (GC.MyChar.Level >= 1 && GC.MyChar.Level < 120)
                                            {
                                                GC.SendPacket(Packets.NPCText("Hello, [" + GC.MyChar.Name + "] I am Your Captain, Your Level Is [" + GC.MyChar.Level + "] So I Can Help You To Get Some EXP For Your Levels (1 To 120), All You Must To Do Kill Some Monsters For Me And I`ll Give You A Rewards, You Will ?"));
                                                GC.SendPacket(Packets.NPCLink("Yeah, I`ll.", 1));
                                                GC.SendPacket(Packets.NPCLink("No, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Level Higher Than 120, And You Have Made The Land Peaceful! There Is Nothing Left For You To Do."));
                                                GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.AbleToHunt == true && GC.MyChar.MonsterHunted >= 100)
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Finished Your Quest, Do You Want Claim Your Reward."));
                                            GC.SendPacket(Packets.NPCLink("Yeah, I Want Claim My Reward", 5));
                                            GC.SendPacket(Packets.NPCLink("Ok, Thanks", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Have Quest, Go Continue Your Quest."));
                                            GC.SendPacket(Packets.NPCLink("Okay", 255));
                                            GC.SendPacket(Packets.NPCLink("Quit My Quest", 10));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay You, It Is Not You City Get Out."));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Here We Are You Must Kill The Monsters To Made The Land More Peaceful."));
                                    GC.SendPacket(Packets.NPCLink("I Accept The Quest.", 2));
                                    GC.SendPacket(Packets.NPCLink("No, I am A Fraid.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2 && GC.MyChar.Level <= 70)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 100)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 110)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2 && GC.MyChar.Level <= 120)
                                {
                                    GC.MyChar.AbleToHunt = true;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "M-Dragon";
                                    GC.SendPacket(Packets.NPCText("You Must Kill 100 Monster From [" + GC.MyChar.MonsterName + "] Okay?"));
                                    GC.SendPacket(Packets.NPCLink("Okay.", 225));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 5)
                                {
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.MyChar.CPs += 1000;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You Got 1K CPs And 2 EXPBalls");
                                }
                                if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("You Are Sure You Want Quit Your Quest, You Killd [" + GC.MyChar.MonsterHunted + "/100 From (" + GC.MyChar.MonsterName + ") ] Go Continue Your Quest."));
                                    GC.SendPacket(Packets.NPCLink("Yes, Quit The Quest.", 15));
                                    GC.SendPacket(Packets.NPCLink("No, Thanks.", 15));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 15)
                                {
                                    GC.MyChar.AbleToHunt = false;
                                    GC.MyChar.MonsterHunted = 0;
                                    GC.MyChar.MonsterName = "";
                                    GC.LocalMessage(2005, System.Drawing.Color.Yellow, "Your Quest Has Quit");
                                }
                                break;
                            }
                        #endregion
                        #region CloudSaint
                        case 5510:
                            {
                                GC.Agreed = false;
                                if (option == 0)
                                {
                                    if (GC.MyChar.Level >= 120)
                                    {
                                        if (GC.MyChar.Level < 140)
                                        {
                                            if (World.DemonExterm.ContainsKey(GC.MyChar.EntityID))
                                            {
                                                Extra.Exterminator Demon = World.DemonExterm[GC.MyChar.EntityID];
                                                if (!Demon.can_continue)
                                                {
                                                    NPCText("You already helped me out today my warrior, please come back to me tomorrow!", GC);
                                                    NPCLink("I see.", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    return;
                                                }
                                                else if (Demon.char_ko < Demon.need_ko && !GC.Agreed)
                                                {
                                                    NPCText("You have to kill the monsters yet, but you can give up if you like. ", GC);
                                                    NPCText("What will you do?", GC);
                                                    NPCLink("what is my mission", 150, GC);
                                                    NPCLink("I'd like to give up", 15, GC);
                                                    NPCLink("Let me think it over", option, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.Agreed = true;
                                                    return;
                                                }
                                                else if (Demon.char_ko >= Demon.need_ko)
                                                {
                                                    if (Demon.stage < 4)
                                                    {
                                                        NPCText("You have passed to the next stage now, you need to kill ", GC);
                                                        NPCText("some devils for me yet. Let's continue?", GC);
                                                        NPCLink("Yes, please", 50, GC);
                                                        NPCLink("Let me think it over", 255, GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        NPCText("You have passed the four stages of the extermination, you can recive", GC);
                                                        NPCText("your reward now, and my gratefully thanks!", GC);
                                                        NPCLink("Wow, thanks and i wish to help again!", 100, GC);
                                                        NPCLink("Let me think it over", 255, GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                        return;
                                                    }
                                                }
                                            }
                                            NPCText("Stay calm and listem, the Demons are taking over the option of ", GC);
                                            NPCText("the world. My family is the only one that can make a contract with ", GC);
                                            NPCText("the humans to purify the world from the devils. I can give you five ", GC);
                                            NPCText("expballs as reward for doing it. But you can do it once one time per ", GC);
                                            NPCText("day, what will you do?", GC);
                                            if (!GC.Agreed)
                                                NPCLink("I want to make it to get the 25 EXP bals and the cps reward", 1, GC);
                                            NPCLink("I'll buy it i jst need 5 exp balls", 10, GC);
                                            NPCLink("Just passing by", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("You have reached the higest level. You can't help me anymore!", GC);
                                            NPCLink("I see, thanks!", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You need to be at last level 120 to help me, go help the city capitains!", GC);
                                        NPCLink("Okay, i will!", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 1)
                                {
                                    #region start the quest
                                    Extra.Exterminator e = new Extra.Exterminator();
                                    e.char_uid = GC.MyChar.EntityID;
                                    e.m_name = Extra.Exterminat.SelectMonster();
                                    if ((GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45) || (GC.MyChar.Job >= 140 && GC.MyChar.Job <= 145))
                                        e.need_ko = 200;
                                    else
                                        e.need_ko = 50;
                                    e.stage = 1;
                                    e.can_continue = true;
                                    GC.MyChar.e_quest = e;
                                    if (!World.DemonExterm.ContainsKey(e.char_uid))
                                        World.DemonExterm.Add(e.char_uid, e);
                                    else
                                    {
                                        World.DemonExterm.Remove(GC.MyChar.EntityID);
                                        Extra.Exterminator e2 = new Extra.Exterminator();
                                        e2.char_uid = GC.MyChar.EntityID;
                                        e2.m_name = Extra.Exterminat.SelectMonster();
                                        e2.need_ko = 200;
                                        e2.stage = 1;
                                        e2.can_continue = true;
                                        World.DemonExterm.Add(e2.char_uid, e2);
                                        GC.MyChar.e_quest = e2;
                                    }
                                    GC.MyChar.CreateItemIDAmount(750000, 1);
                                    NPCText("monsternam: " + e.m_name + " you have to kill " + e.need_ko + " ofMonsters (stage one)", GC);
                                    NPCLink("Okay thanx!", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    #endregion
                                }
                                else if (option == 15)
                                {
                                    #region give up
                                    Extra.Exterminator e = World.DemonExterm[GC.MyChar.EntityID];
                                    e.can_continue = false;
                                    e.m_name = "None";
                                    e.need_ko = 0;
                                    e.stage = 5;
                                    e.can_continue = false;
                                    World.DemonExterm[GC.MyChar.EntityID] = e;
                                    GC.MyChar.e_quest = e;
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You gived up, you will be abble to take another quest tomorrow only!");
                                    #endregion
                                }
                                else if (option == 50)
                                {
                                    #region continue the quest
                                    Extra.Exterminator e = World.DemonExterm[GC.MyChar.EntityID];
                                    e.stage++;
                                    if ((GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45) || (GC.MyChar.Job >= 140 && GC.MyChar.Job <= 145))
                                        switch (e.stage)
                                        {
                                            case 2: e.need_ko = 400; break;
                                            case 3: e.need_ko = 700; break;
                                            case 4: e.need_ko = 900; break;
                                        }
                                    else
                                        switch (e.stage)
                                        {
                                            case 2: e.need_ko = 100; break;
                                            case 3: e.need_ko = 175; break;
                                            case 4: e.need_ko = 250; break;
                                        }
                                    e.m_name = Extra.Exterminat.Continue(e.stage);
                                    e.can_continue = true;
                                    World.DemonExterm[GC.MyChar.EntityID] = e;
                                    GC.MyChar.e_quest = e;
                                    NPCText("monsternam: " + e.m_name + " you have to kill " + e.need_ko + " ofMonsters (stage " + e.stage + ")", GC);
                                    NPCLink("Okay thanx!", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    #endregion
                                }
                                else if (option == 100)
                                {
                                    #region over the quest
                                    if (!World.DemonExterm.ContainsKey(GC.MyChar.EntityID))
                                    {
                                        Extra.Exterminator e = new Extra.Exterminator();
                                        e.char_uid = GC.MyChar.EntityID;
                                        e.m_name = "None";
                                        e.need_ko = 0;
                                        e.stage = 5;
                                        e.can_continue = false;
                                        World.DemonExterm.Add(e.char_uid, e);
                                        GC.MyChar.e_quest = e;
                                    }
                                    else
                                    {
                                        Extra.Exterminator e = World.DemonExterm[GC.MyChar.EntityID];
                                        e.can_continue = false;
                                        e.m_name = "None";
                                        e.need_ko = 0;
                                        e.stage = 5;
                                        e.can_continue = false;
                                        World.DemonExterm[GC.MyChar.EntityID] = e;
                                        GC.MyChar.e_quest = e;
                                    }
                                    GC.LocalMessage(2011, System.Drawing.Color.Yellow, "Congratulations! You have over the extermination you got 200K cps, here is your prize and come back tomorrow!"); ;
                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 25, false); //<< modify!
                                    GC.MyChar.CPs += 200000;
                                    #endregion
                                }
                                else if (option == 150)
                                {
                                    Extra.Exterminator e = GC.MyChar.e_quest;
                                    NPCText("monsternam: " + e.m_name + " you have to kill " + e.need_ko + " ofMonsters (stage one)", GC);
                                    NPCLink("Okay thanx!", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 10)
                                {
                                    NPCText("You need to give me 2150 cps, and i will give to you the reward, ", GC);
                                    NPCText("BUT it will give You just 5 exp balls if you made it Your silf it will give you 25 exp ball!", GC);
                                    NPCLink("I wish to buy it to gain 5 exp balls", 11, GC);
                                    NPCLink("Let me think it over", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 11)
                                {
                                    if (GC.MyChar.CPs >= 2150)
                                    {
                                        #region buy the quest
                                        if (!World.DemonExterm.ContainsKey(GC.MyChar.EntityID))
                                        {
                                            Extra.Exterminator e = new Extra.Exterminator();
                                            e.char_uid = GC.MyChar.EntityID;
                                            e.m_name = "None";
                                            e.need_ko = 0;
                                            e.stage = 5;
                                            e.can_continue = false;
                                            World.DemonExterm.Add(e.char_uid, e);
                                            GC.MyChar.e_quest = e;
                                        }
                                        else
                                        {
                                            Extra.Exterminator e = World.DemonExterm[GC.MyChar.EntityID];
                                            e.can_continue = false;
                                            e.m_name = "None";
                                            e.need_ko = 0;
                                            e.stage = 5;
                                            e.can_continue = false;
                                            World.DemonExterm[GC.MyChar.EntityID] = e;
                                            GC.MyChar.e_quest = e;
                                        }
                                        GC.MyChar.CPs -= 2150;
                                        GC.LocalMessage(2011, System.Drawing.Color.Yellow, "You have bought the quest today, come back tomorrow!");
                                        GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 5, false);
                                        #endregion
                                    }
                                    else
                                    {
                                        NPCText("You don't have enough cps to buy the quest, please come back later!", GC);
                                        NPCLink("I'll be right back!", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                GC.Agreed = false;
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant HelpThe Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion

                        #region Auto Invite Events
                        #region Guild War Start
                        case 91122:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi " + GC.MyChar.Name + " Guild War Is Started Now.Do You Want To Join."));
                                    GC.SendPacket(Packets.NPCLink("I Want To Join Guild War.", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1038, 318, 304);
                                }
                                break;
                            }
                        #endregion
                        #region PK Tournament Start
                        case 92222:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi " + GC.MyChar.Name + " Charcter PK Tournament Is Started.Do You Want To Join."));
                                    GC.SendPacket(Packets.NPCLink("I Want To Join PK Tournament.", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1002, 437, 252);
                                }
                                break;
                            }
                        #endregion
                        #region Pk Champion Start
                        case 93322:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi " + GC.MyChar.Name + " PK Champion Is Started Now.Do You Want To Join."));
                                    GC.SendPacket(Packets.NPCLink("I Want To Join PK Champion.", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1002, 424, 252);
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region Teleport NPCs
                        case 10000:// In TC --> To All
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to somewhere? It will only cost you 1000 Silvers."));
                                    GC.SendPacket(Packets.NPCLink("Phoenix Castle", 1));
                                    GC.SendPacket(Packets.NPCLink("Ape City", 2));
                                    GC.SendPacket(Packets.NPCLink("Desert City", 3));
                                    GC.SendPacket(Packets.NPCLink("Bird Island", 4));
                                    GC.SendPacket(Packets.NPCLink("Mine Cave", 5));
                                    GC.SendPacket(Packets.NPCLink("Market", 6));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (GC.MyChar.Silvers >= 1000)
                                {
                                    GC.MyChar.Silvers -= 1000;
                                    if (option == 1) GC.MyChar.Teleport(1002, 958, 555);
                                    if (option == 2) GC.MyChar.Teleport(1002, 555, 957);
                                    if (option == 3) GC.MyChar.Teleport(1002, 61, 460);
                                    if (option == 4) GC.MyChar.Teleport(1002, 232, 190);
                                    if (option == 5) GC.MyChar.Teleport(1002, 50, 404);
                                    if (option == 6) GC.MyChar.Teleport(1036, 211, 196);
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("I Said 1000 Silvers! If you don't have that, don't bother me."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        case 10010:// In Market --> To PreviousMap
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to leave market? It's free!"));
                                    GC.SendPacket(Packets.NPCLink("Yes, i do.", 1));
                                    GC.SendPacket(Packets.NPCLink("No, i dont.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    try
                                    {
                                        Game.Vector2 V = (Game.Vector2)Database.DefaultCoords[GC.MyChar.Loc.PreviousMap];
                                        GC.MyChar.Teleport(GC.MyChar.Loc.PreviousMap, V.X, V.Y);
                                    }
                                    catch
                                    {
                                        if (GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                        {
                                            GC.MyChar.Teleport(1037, 234, 238);
                                        }
                                        else
                                        {
                                            GC.MyChar.Teleport(1002, 432, 378);
                                        }
                                    }
                                }
                                break;
                            }
                        case 10020:// In PC --> To TC
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to somewhere? It will only cost you 100 silvers."));
                                    GC.SendPacket(Packets.NPCLink("Twin City", 1));
                                    GC.SendPacket(Packets.NPCLink("Market", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (GC.MyChar.Silvers >= 100)
                                {
                                    GC.MyChar.Silvers -= 100;
                                    if (option == 1) GC.MyChar.Teleport(1011, 10, 377);
                                    if (option == 2) GC.MyChar.Teleport(1036, 211, 196);
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("I said 100 silvers! If you don't have that, don't bother me."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        case 10021:// In AC --> To TC
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to somewhere? It will only cost you 100 silvers."));
                                    GC.SendPacket(Packets.NPCLink("Twin City", 1));
                                    GC.SendPacket(Packets.NPCLink("Market", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (GC.MyChar.Silvers >= 100)
                                {
                                    GC.MyChar.Silvers -= 100;
                                    if (option == 1) GC.MyChar.Teleport(1020, 378, 13);
                                    if (option == 2) GC.MyChar.Teleport(1036, 211, 196);
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("I said 100 silvers! If you don't have that, don't bother me."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }

                        case 10022://// In DC --> To TC
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to somewhere? It will only cost you 100 silvers."));
                                    GC.SendPacket(Packets.NPCLink("Twin City", 1));
                                    GC.SendPacket(Packets.NPCLink("Mystic Castle", 2));
                                    GC.SendPacket(Packets.NPCLink("Market", 3));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (GC.MyChar.Silvers >= 100)
                                {
                                    GC.MyChar.Silvers -= 100;
                                    if (option == 1) GC.MyChar.Teleport(1000, 973, 668);
                                    if (option == 2) GC.MyChar.Teleport(1000, 80, 320);
                                    if (option == 3) GC.MyChar.Teleport(1036, 211, 196);
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("I said 100 silvers! If you don't have that, don't bother me."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        case 10023://// In BC --> To TC
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to somewhere? It will only cost you 100 silvers."));
                                    GC.SendPacket(Packets.NPCLink("Twin City", 1));
                                    GC.SendPacket(Packets.NPCLink("Market", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (GC.MyChar.Silvers >= 100)
                                {
                                    GC.MyChar.Silvers -= 100;
                                    if (option == 1) GC.MyChar.Teleport(1015, 1015, 710);
                                    if (option == 2) GC.MyChar.Teleport(1036, 211, 196);
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("I said 100 silvers! If you don't have that, don't bother me."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        case 10030:// In TC --> To DC
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("This is the way to desert city. You wan't to continue?"));
                                    GC.SendPacket(Packets.NPCLink("Yes Teleport Me.", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1000, 973, 667);
                                }

                                break;
                            }
                        #endregion
                        #region Simon
                        case 10050:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hey want me to teleport you to the lab? It will only cost you 100 silvers."));
                                    GC.SendPacket(Packets.NPCLink("Lab 1", 1));
                                    GC.SendPacket(Packets.NPCLink("Lab 2", 2));
                                    GC.SendPacket(Packets.NPCLink("Lab 3", 3));
                                    GC.SendPacket(Packets.NPCLink("Lab 4", 4));
                                    // GC.SendPacket(Packets.NPCLink("Lab 5", 5));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.CPs >= 215)
                                    {
                                        GC.MyChar.CPs -= 215;
                                        GC.MyChar.Teleport(1351, 015, 127);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 215 CPs."));
                                        GC.SendPacket(Packets.NPCLink("I See.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.CPs >= 430)
                                    {
                                        GC.MyChar.CPs -= 430;
                                        GC.MyChar.Teleport(1352, 029, 230);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 430 CPs."));
                                        GC.SendPacket(Packets.NPCLink("I See.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 3)
                                {
                                    if (GC.MyChar.CPs >= 645)
                                    {
                                        GC.MyChar.CPs -= 645;
                                        GC.MyChar.Teleport(1353, 028, 270);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 645 CPs."));
                                        GC.SendPacket(Packets.NPCLink("I See.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 4)
                                {
                                    if (GC.MyChar.CPs >= 860)
                                    {
                                        GC.MyChar.CPs -= 860;
                                        GC.MyChar.Teleport(1354, 009, 290);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 860 CPs."));
                                        GC.SendPacket(Packets.NPCLink("I See.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 5)
                                {
                                    if (GC.MyChar.CPs >= 1075)
                                    {
                                        GC.MyChar.CPs -= 1075;
                                        GC.MyChar.Teleport(1762, 061, 255);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 1075 CPs."));
                                        GC.SendPacket(Packets.NPCLink("I See.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region Warehouses
                        case 2510:
                        case 2511:
                        case 2512:
                        case 2513:
                        case 2514:
                        case 2515:
                        case 2516:
                            {
                                if (!GC.MyChar.WHOpen && GC.MyChar.WHPassword != "0")
                                {
                                    if (GC.MyChar.WHPassword == ReadString(Data))
                                    {
                                        GC.MyChar.WHOpen = true;
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 4, N.Loc.X, N.Loc.Y, 0x7e));
                                    }
                                    else
                                    {
                                        if (GC.MyChar.WHOpenChance >= 1)
                                            GC.MyChar.WHOpenChance++;
                                    }
                                    if (GC.MyChar.WHOpenChance <= 3 && !GC.MyChar.WHOpen)
                                    {
                                        if (GC.MyChar.WHOpenChance == 0)
                                        {
                                            GC.MyChar.WHOpenChance = 1;
                                            NPCText("Please put your warehouse password to open it.", GC);
                                        }
                                        else
                                        {
                                            NPCText("Wrong! You have more " + GC.MyChar.WHOpenChance + " times to try.", GC);
                                        }
                                        NPCLink2("Password.", 1, GC);
                                        NPCLink("Let me think.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if (GC.MyChar.WHOpenChance == 4)
                                    {
                                        NPCText("You have to logoff and login againe to try it again.", GC);
                                        NPCLink("I see.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (GC.MyChar.WHPassword == "" || GC.MyChar.WHPassword == "0")
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "To protect your items, please put a password in the WHSGuardian in TwinCity.");
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 4, N.Loc.X, N.Loc.Y, 0x7e));
                                }
                                else if (GC.MyChar.WHOpen == true)
                                {
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 4, N.Loc.X, N.Loc.Y, 0x7e));
                                }
                                break;
                            }
                        #endregion
                        #region VIP WarehouseNPC
                        case 12:
                            {
                                if (!GC.MyChar.WHOpen && GC.MyChar.WHPassword != "0")
                                {
                                    if (GC.MyChar.WHPassword == ReadString(Data))
                                    {
                                        GC.MyChar.WHOpen = true;
                                        GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 341, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0x7e));
                                    }
                                    else
                                    {
                                        if (GC.MyChar.WHOpenChance >= 1)
                                            GC.MyChar.WHOpenChance++;
                                    }
                                    if (GC.MyChar.WHOpenChance <= 3 && !GC.MyChar.WHOpen)
                                    {
                                        if (GC.MyChar.WHOpenChance == 0)
                                        {
                                            GC.MyChar.WHOpenChance = 1;
                                            NPCText("Please put your warehouse password to open it.", GC);
                                        }
                                        else
                                        {
                                            NPCText("Wrong! You have more " + GC.MyChar.WHOpenChance + " times to try.", GC);
                                        }
                                        NPCLink2("Password.", 1, GC);
                                        NPCLink("Let me think.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if (GC.MyChar.WHOpenChance == 4)
                                    {
                                        NPCText("You have to logoff and login againe to try it again.", GC);
                                        NPCLink("I see.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (GC.MyChar.WHPassword == "" || GC.MyChar.WHPassword == "0")
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "To protect your items, please put a password in the WHSGuardian in TwinCity.");
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 341, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0x7e));
                                }
                                else if (GC.MyChar.WHOpen == true)
                                {
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 341, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 0x7e));
                                }
                                break;
                            }
                        #endregion
                        #region WHPassGuardian
                        case 2520:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello! What Do I can for you?"));
                                    if (GC.MyChar.WHPassword == "")
                                    {
                                        GC.SendPacket(Packets.NPCLink("Put a password in my warehouse.", 1));
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCLink("Remove Password.", 4));
                                        GC.SendPacket(Packets.NPCLink("Change Password.", 6));
                                    }
                                    GC.SendPacket(Packets.NPCLink("Let me think.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.TempPass = "";
                                    GC.SendPacket(Packets.NPCText("Please put your password. Min characters 4 and Max 10 characters. Just numbers is permited"));
                                    GC.SendPacket(Packets.NPCLink2("Password", 2));
                                    GC.SendPacket(Packets.NPCLink("Let me think.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    GC.MyChar.TempPass = ReadString(Data);
                                    GC.SendPacket(Packets.NPCText("Please put again your password."));
                                    GC.SendPacket(Packets.NPCLink2("Retype Password", 3));
                                    GC.SendPacket(Packets.NPCLink("Cancel it.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.TempPass == ReadString(Data))
                                    {
                                        if (GC.MyChar.TempPass.Length >= 4 && GC.MyChar.TempPass.Length <= 10)
                                        {
                                            if (GC.ValidWHPass(GC.MyChar.TempPass))
                                            {
                                                GC.MyChar.WHPassword = GC.MyChar.TempPass;
                                                GC.MyChar.WHOpen = false;
                                                GC.SendPacket(Packets.NPCText("Done! Now you is protected."));
                                                GC.SendPacket(Packets.NPCLink("Thanks!", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Just numbers is permited!"));
                                                GC.SendPacket(Packets.NPCLink("Sorry!", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Min characters 4 and Max 10 characters!"));
                                            GC.SendPacket(Packets.NPCLink("Sorry!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("The passwords is not same. Try again"));
                                        GC.SendPacket(Packets.NPCLink2("The Password", 3));
                                        GC.SendPacket(Packets.NPCLink("Cancel it.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 4)
                                {
                                    GC.SendPacket(Packets.NPCText("To remove the password, please put it here for you security."));
                                    GC.SendPacket(Packets.NPCLink2("Current Password!", 5));
                                    GC.SendPacket(Packets.NPCLink("Let me think", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 5)
                                {
                                    if (GC.MyChar.WHPassword == ReadString(Data))
                                    {
                                        GC.MyChar.WHPassword = "0";
                                        GC.MyChar.WHOpen = false;
                                        GC.SendPacket(Packets.NPCText("Done! Now you are without password!"));
                                        GC.SendPacket(Packets.NPCLink("Thanks!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Wrong! are you tryin to hack me?"));
                                        GC.SendPacket(Packets.NPCLink("No, Sorry!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 6)
                                {
                                    GC.SendPacket(Packets.NPCText("To change your password please put the old first. Remeber just numbers is permited!"));
                                    GC.SendPacket(Packets.NPCLink2("Old Password", 7));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 7)
                                {
                                    if (GC.MyChar.WHPassword == ReadString(Data))
                                    {
                                        GC.SendPacket(Packets.NPCText("Now is the new password."));
                                        GC.SendPacket(Packets.NPCLink2("New Password", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Wrong! are you tryin to hack me?"));
                                        GC.SendPacket(Packets.NPCLink("No, Sorry!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        

                        #region NPC In TwinCity
                        #region TC Arena
                        case 3000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter the PK Arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1005, 51, 71);
                                break;
                            }
                        #endregion
                        #region Enter TC Jail
                        case 3010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to visit jail?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(6000, 32, 72);
                                break;
                            }
                        #endregion
                        #region Leave TC Jail
                        case 3011:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Only if your PK < 100 you can leave here, if your PK > 100 you must Pay some CPSs."));
                                    GC.SendPacket(Packets.NPCLink("Let me out of here.", 1));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.PKPoints < 100)
                                    {
                                        GC.MyChar.Teleport(1002, 512, 355);
                                    }
                                    else if (GC.MyChar.PKPoints >= 100 && GC.MyChar.PKPoints < 200)
                                    {
                                        GC.SendPacket(Packets.NPCText("Your PKPoints is between 100 : 200 so you will Pay 70K CPS to set you free."));
                                        GC.SendPacket(Packets.NPCLink("So i have to Pay.", 2));
                                        // GC.SendPacket(Packets.NPCLink("I have 10 kk silvers.", 3));
                                        GC.SendPacket(Packets.NPCLink("I see", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.PKPoints >= 200 && GC.MyChar.PKPoints < 400)
                                    {
                                        GC.SendPacket(Packets.NPCText("Your PKPoints is between 200 : 400 so you will Pay 140K CPS to set you free."));
                                        GC.SendPacket(Packets.NPCLink("So i have to Pay.", 3));
                                        // GC.SendPacket(Packets.NPCLink("I have 10 kk silvers.", 3));
                                        GC.SendPacket(Packets.NPCLink("I see", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.PKPoints >= 400)
                                    {
                                        GC.SendPacket(Packets.NPCText("your PKPoints is 400 or more so you will Pay 1KK CPS to set you free."));
                                        GC.SendPacket(Packets.NPCLink("So i have to Pay.", 4));
                                        // GC.SendPacket(Packets.NPCLink("I have 10 kk silvers.", 3));
                                        GC.SendPacket(Packets.NPCLink("I see", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.CPs >= 70000)
                                    {
                                        GC.MyChar.CPs -= 70000;
                                        GC.MyChar.Teleport(1002, 512, 355);
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.CPs >= 140000)
                                    {
                                        GC.MyChar.CPs -= 140000;
                                        GC.MyChar.Teleport(1002, 512, 355);
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have Cps ."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 4)
                                {
                                    if (GC.MyChar.CPs >= 1000000)
                                    {
                                        GC.MyChar.CPs -= 1000000;
                                        GC.MyChar.Teleport(1002, 512, 355);
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have Cps ."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Enter TG
                        case 3021:
                        case 3022:
                        case 3023:
                        case 3024:
                        case 3025:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Training Center Not Available Now, Welcome To HeroesCOnline."));
                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                    //GC.SendPacket(Packets.NPCLink("Yeah.", 1));
                                    //GC.SendPacket(Packets.NPCLink("Just passing by", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.Silvers >= 1000)
                                    {
                                        GC.MyChar.Silvers -= 1000;
                                        GC.MyChar.Teleport(1039, 228, 226);
                                        GC.SendPacket(Packets.NPCText("Ok. Here you are. You can only hit dummies that are of your level or lower."));
                                        GC.SendPacket(Packets.NPCLink("Ok then.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("As i said... you need 1000 silvers. Why do i need to repeat myself?"));
                                        GC.SendPacket(Packets.NPCLink("Alright alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Leave TG
                        case 3020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to leave this place?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah.", 1));
                                    GC.SendPacket(Packets.NPCLink("No, I'll stay here.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    Game.Vector2 V = (Game.Vector2)Database.DefaultCoords[GC.MyChar.Loc.PreviousMap];
                                    GC.MyChar.Teleport(GC.MyChar.Loc.PreviousMap, V.X, V.Y);
                                }
                                break;
                            }
                        #endregion
                        #region HairStyles
                        case 3030:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Would you like to change your HairStyle? I can offer you a change for 500 silvers. You can choose from the styles below."));
                                    GC.SendPacket(Packets.NPCLink("StylesType 1", 1));
                                    GC.SendPacket(Packets.NPCLink("StylesType 2", 2));
                                    GC.SendPacket(Packets.NPCLink("StylesType 3", 3));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Paid = false;
                                }
                                #region StylesType 1
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("Style 1", 10));
                                        GC.SendPacket(Packets.NPCLink("Style 2", 11));
                                        GC.SendPacket(Packets.NPCLink("Style 3", 12));
                                        GC.SendPacket(Packets.NPCLink("Style 4", 13));
                                        GC.SendPacket(Packets.NPCLink("Style 5", 14));
                                        GC.SendPacket(Packets.NPCLink("Style 6", 15));
                                        GC.SendPacket(Packets.NPCLink("Style 7", 16));
                                        GC.SendPacket(Packets.NPCLink("Next", 100));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 100 && GC.Paid)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                    GC.SendPacket(Packets.NPCLink("Style 8", 17));
                                    GC.SendPacket(Packets.NPCLink("Style 9", 18));
                                    GC.SendPacket(Packets.NPCLink("Style 10", 19));
                                    GC.SendPacket(Packets.NPCLink("Style 11", 20));
                                    GC.SendPacket(Packets.NPCLink("Style 12", 21));
                                    GC.SendPacket(Packets.NPCLink("Previous", 1));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 10 && option <= 21 && GC.Paid)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 1));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (20 + option).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (20 + option).ToString());
                                }
                                #endregion
                                #region StylesType 2
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("Style 1", 30));
                                        GC.SendPacket(Packets.NPCLink("Style 2", 31));
                                        GC.SendPacket(Packets.NPCLink("Style 3", 32));
                                        GC.SendPacket(Packets.NPCLink("Style 4", 33));
                                        GC.SendPacket(Packets.NPCLink("Style 5", 34));
                                        GC.SendPacket(Packets.NPCLink("Style 6", 35));
                                        GC.SendPacket(Packets.NPCLink("Style 7", 36));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option >= 30 && option <= 36)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 20).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 20).ToString());
                                }
                                #endregion
                                #region StylesType 3
                                else if (option == 3)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("Style 1", 40));
                                        GC.SendPacket(Packets.NPCLink("Style 2", 41));
                                        GC.SendPacket(Packets.NPCLink("Style 3", 42));
                                        GC.SendPacket(Packets.NPCLink("Style 4", 43));
                                        GC.SendPacket(Packets.NPCLink("Style 5", 44));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option >= 40 && option <= 44)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 3));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 19).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 19).ToString());
                                }
                                #endregion
                                break;
                            }
                        #endregion
                        #region Armor/Headgear/Shield Dyeing
                        case 3040:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Are you not satisfied with your current armor, headgear or shield color? If not, then come in to our shop, that man inside will give you the service. Just give me one meteor."));
                                    GC.SendPacket(Packets.NPCLink("Yeah, i want to dye my equipment.", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088001, 1))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(1088001, 1);
                                        GC.MyChar.Teleport(1008, 22, 26);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("No meteor, no entry."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        case 3041:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Great! A customer, so do you want to dye your equipment?"));
                                    GC.SendPacket(Packets.NPCLink("Dye My Armor.", 1));
                                    GC.SendPacket(Packets.NPCLink("Dye My Headgear.", 2));
                                    if (GC.MyChar.Equips.LeftHand.ID != 0 && Game.ItemIDManipulation.Part(GC.MyChar.Equips.LeftHand.ID, 0, 3) == 900)
                                        GC.SendPacket(Packets.NPCLink("Dye My Shield.", 3));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1 || option == 2 || option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose the color."));
                                    GC.SendPacket(Packets.NPCLink("Orange", (byte)(option * 10 + 3)));
                                    GC.SendPacket(Packets.NPCLink("Light Blue", (byte)(option * 10 + 4)));
                                    GC.SendPacket(Packets.NPCLink("Red", (byte)(option * 10 + 5)));
                                    GC.SendPacket(Packets.NPCLink("Blue", (byte)(option * 10 + 6)));
                                    GC.SendPacket(Packets.NPCLink("Yellow", (byte)(option * 10 + 7)));
                                    GC.SendPacket(Packets.NPCLink("Purple", (byte)(option * 10 + 8)));
                                    GC.SendPacket(Packets.NPCLink("White", (byte)(option * 10 + 9)));
                                    GC.SendPacket(Packets.NPCLink("I've changed my mind.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else
                                {
                                    if (option >= 13 && option <= 19 && GC.MyChar.Equips.Armor.ID == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have an armor equipped. What am i gonna dye, your body?"));
                                        GC.SendPacket(Packets.NPCLink("No, don't do that!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if (option >= 13 && option <= 19)
                                    {
                                        GC.MyChar.Equips.Armor.Color = (Game.Item.ArmorColor)(option - 10);
                                        GC.SendPacket(Packets.AddItem(GC.MyChar.Equips.Armor, 3));
                                    }
                                    else if (option >= 23 && option <= 29 && GC.MyChar.Equips.HeadGear.ID == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have any headgear equipped. I'm no hair dyer, so put something on your head."));
                                        GC.SendPacket(Packets.NPCLink("No, don't do that!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if (option >= 23 && option <= 29)
                                    {
                                        GC.MyChar.Equips.HeadGear.Color = (Game.Item.ArmorColor)(option - 20);
                                        GC.SendPacket(Packets.AddItem(GC.MyChar.Equips.HeadGear, 1));
                                    }
                                    else if (option >= 33 && option <= 39 && (GC.MyChar.Equips.LeftHand.ID == 0 || Game.ItemIDManipulation.Part(GC.MyChar.Equips.LeftHand.ID, 0, 3) != 900))
                                    {
                                        GC.SendPacket(Packets.NPCText("Where did you put your shield to? You just had one equiped."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if (option >= 33 && option <= 39)
                                    {
                                        GC.MyChar.Equips.LeftHand.Color = (Game.Item.ArmorColor)(option - 30);
                                        GC.SendPacket(Packets.AddItem(GC.MyChar.Equips.LeftHand, 5));
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region VipNpc
                        case 3050:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Welcome " + GC.MyChar.Name + "Iam Selling VIP-Service So Would You Like To Buy?"));
                                    GC.SendPacket(Packets.NPCLink("I Want Buy VIP-Service", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Pick on your choice."));
                                    GC.SendPacket(Packets.NPCLink("VIP-1 for 100K CPs", 2));
                                    GC.SendPacket(Packets.NPCLink("VIP-2 for 200K CPs", 3));
                                    GC.SendPacket(Packets.NPCLink("VIP-3 for 300K CPs", 4));
                                    GC.SendPacket(Packets.NPCLink("VIP-4 for 500K CPs", 5));
                                    GC.SendPacket(Packets.NPCLink("VIP-5 for 700K CPs", 6));
                                    GC.SendPacket(Packets.NPCLink("VIP-6 for 1KK CPs", 7));
                                    GC.SendPacket(Packets.NPCLink("No, thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.CPs >= 100000)
                                    {
                                        if (GC.MyChar.VipLevel == 0)
                                        {

                                            GC.MyChar.CPs -= 100000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for playing Heroes online.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip1", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 3)
                                {
                                    if (GC.MyChar.CPs >= 200000)
                                    {
                                        if (GC.MyChar.VipLevel == 1)
                                        {
                                            GC.MyChar.CPs -= 200000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for buying it.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip 2", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you aren't Vip 1 you must buy Vip 1 frist."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 4)
                                {
                                    if (GC.MyChar.CPs >= 300000)
                                    {
                                        if (GC.MyChar.VipLevel == 2)
                                        {
                                            GC.MyChar.CPs -= 300000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for buying it.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip 3", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you aren't Vip 2 you must buy Vip 2 frist."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 5)
                                {
                                    if (GC.MyChar.CPs >= 500000)
                                    {
                                        if (GC.MyChar.VipLevel == 3)
                                        {
                                            GC.MyChar.CPs -= 500000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for buying it.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip 4", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you aren't Vip 3 you must buy Vip 3 frist."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 6)
                                {
                                    if (GC.MyChar.CPs >= 700000)
                                    {
                                        if (GC.MyChar.VipLevel == 4)
                                        {
                                            GC.MyChar.CPs -= 700000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for buying it.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip 5", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you aren't Vip 4 you must buy Vip 4 frist."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 7)
                                {
                                    if (GC.MyChar.CPs >= 1000000)
                                    {
                                        if (GC.MyChar.VipLevel == 5)
                                        {
                                            GC.MyChar.CPs -= 1000000;
                                            GC.MyChar.VipLevel += 1;
                                            //GC.SendPacket(Packets.ChatMessage(0, "SYSTEM", GC.MyChar.Name, "Congratulations! Your vip level is " + GC.MyChar.VipLevel + ". Thank you for buying it.", 2001, 0));
                                            GC.WorldMessage(2005, GC.MyChar.Name + "Congratulations you succeeded to buy Vip 6", System.Drawing.Color.Black);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you aren't Vip 5 you must buy Vip 5 frist."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region NPC In Market
                        #region LadyLuck
                        case 4000:// EnterLucky
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText(("Why hello young warrior, Do you wish to try your luck in my lottery? Well it will only cost you 27 cps, You may have a chance to win 500,000,000 in gold and super 2 socket gears, So what do you say wanna try it?")));
                                    //GC.SendPacket(Packets.NPCLink("I sure do wanna try!", 1));
                                    //GC.SendPacket(Packets.NPCLink("May I know the rules first?", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Lottery)
                                    {
                                        GC.MyChar.Teleport(700, 40, 50);
                                        return;
                                    }
                                    if (GC.MyChar.Level >= 70)
                                    {
                                        if (GC.MyChar.LotteryUsed < 8)
                                        {
                                            if (GC.MyChar.Inventory.Count < 40)
                                            {

                                                if (GC.MyChar.CPs >= 27)
                                                {
                                                    GC.MyChar.Teleport(700, 40, 50);
                                                    GC.MyChar.CPs -= 27;
                                                    GC.MyChar.Lottery = true;
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("I'm sorry you do not have the required CPs."));
                                                    GC.SendPacket(Packets.NPCLink("Okay, I will be back when I have 27 CPs.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Please make some room in your item box first."));
                                                GC.SendPacket(Packets.NPCLink("Okay...", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Come Towmorrow Cuz You Finished Your 8 Chances ToDay."));
                                            GC.SendPacket(Packets.NPCLink("Okay.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("I'm sorry you do not have the required Level 70."));
                                        GC.SendPacket(Packets.NPCLink("Okay...", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("You will be teleported to Lottery Center where there are many LuckyBoxes after you pay me 27 CPs. You may choose one box to try your luck at your will."));
                                    GC.SendPacket(Packets.NPCText("Remeber you have only one chance to open a LuckyBox every time. If you want to open another box, you have to leave the room to re-enrol in Market."));
                                    GC.SendPacket(Packets.NPCLink("Okay...", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant HelpThe Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        case 4001: // LeaveLucky 
                            {
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Do you want to leave this place?"));
                                        GC.SendPacket(Packets.NPCLink("Yeah.", 1));
                                        GC.SendPacket(Packets.NPCLink("No, I'll stay here.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option == 1)
                                    {
                                        try
                                        {
                                            GC.MyChar.Teleport(1036, 213, 206);
                                        }
                                        catch
                                        {
                                            Game.Vector2 V = (Game.Vector2)Database.DefaultCoords[GC.MyChar.Loc.PreviousMap];
                                            GC.MyChar.Teleport(GC.MyChar.Loc.PreviousMap, V.X, V.Y);
                                        }
                                    }
                                    break;
                                }
                            }

                        case 925:
                        case 926:
                        case 927:
                        case 928:
                        case 929:
                        case 930:
                        case 931:
                        case 932:
                        case 933:
                        case 934:
                        case 935:
                        case 936:
                        case 937:
                        case 938:
                        case 939:
                        case 940:
                        case 942:
                        case 943:
                        case 944:
                        case 945:
                            {

                                if (GC.MyChar.Lottery)
                                {
                                TryAgain:
                                    bool Found = false; short Chance = 0; string ItemName = "";
                                    Random Rd = new Random();
                                    while (!Found)
                                    {
                                        Chance = (short)Rd.Next(Game.World.H_LottoItems.Count);
                                        if (Chance > 999) Chance = (short)Rd.Next(Game.World.H_LottoItems.Count);
                                        if (Game.World.H_LottoItems.ContainsKey(Chance)) Found = true;
                                    }
                                    if (Found)
                                    {

                                        Game.Item Item = (Game.Item)Game.World.H_LottoItems[Chance];


                                        if (Item.ID.ToString().EndsWith("8"))
                                        {
                                            if ((byte)Item.Soc1 > 0 || Item.Plus == 8)
                                            {
                                                ItemName = "an Elite ";
                                                if ((byte)Item.Soc2 > 0) ItemName += "2 socket ";
                                                else if ((byte)Item.Soc1 > 0) ItemName += "one socket ";
                                                else if (Item.Plus == 8) ItemName += "+1 ";
                                            }
                                        }
                                        else if (Item.ID.ToString().EndsWith("9") && Item.ID != 723719) ItemName = "A Super ";
                                        DatabaseItem I = (DatabaseItem)Database.DatabaseItems[Item.ID];
                                        ItemName += I.Name;
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulation, you have won " + ItemName + ".");
                                        if (Chance > 510)
                                        {
                                            if (MyMath.ChanceSuccess(80))
                                                goto TryAgain;
                                            GC.WorldMessage(2000, GC.MyChar.Name + " was so lucky to won " + ItemName + " from lady luck in Market.", System.Drawing.Color.Orange);
                                        }
                                        GC.MyChar.Lottery = false;
                                        GC.MyChar.CreateReadyItem(Item);
                                        GC.MyChar.LotteryUsed++;
                                        World.Action(GC.MyChar, Packets.String(N.EntityID, 10, "MBStandard"));
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region RelotAttributePoint
                        case 7020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi, Iam The RelotAttributePoint Man Like They Sayes ^_^ Yeah I guess So , Now Iam Asking You About You Want Relot Your AttributePoints ?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah Do It.", 200));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 200)
                                {
                                    GC.SendPacket(Packets.NPCText("Alright I'll need an DragonBall do you have ?"));
                                    GC.SendPacket(Packets.NPCLink("Okay here it is.", 201));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 201)
                                {
                                    if (GC.MyChar.Reborns > 0)
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(1088000, 1))
                                        {
                                            //Game.Item DB = null;
                                            //foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                                //if (I.ID == 1088000)
                                                //{ DB = I; break; }
                                            //if (DB != null)
                                            {
                                                int AllAtributes = GC.MyChar.Str + GC.MyChar.Agi + GC.MyChar.Spi + GC.MyChar.Vit + GC.MyChar.StatusPoints;
                                                GC.MyChar.Str = 0;
                                                GC.MyChar.Agi = 0;
                                                GC.MyChar.Spi = 0;
                                                GC.MyChar.Vit = 0;
                                                GC.MyChar.StatusPoints = (ushort)(AllAtributes);
                                                GC.MyChar.CurHP = 1;
                                                GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                GC.SendPacket(Packets.NPCText("Okay, Done Your AttributePoints Now Reloted."));
                                                GC.SendPacket(Packets.NPCLink("Ok Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You dont have the DragonBall"));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Are Not Reborn Yet."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region JewelerLau
                        case 4020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Wanna mix some gems together? Give me 15 normal gems and i'll mix them into refined one. Same goes for refineds to super."));
                                    GC.SendPacket(Packets.NPCLink("I wanna mix normal gems into refined ones.", 1));
                                    GC.SendPacket(Packets.NPCLink("I wanna mix refined gems into super ones.", 2));
                                    GC.SendPacket(Packets.NPCLink("Got any other offers for me?", 3));
                                    GC.SendPacket(Packets.NPCLink("No.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("Yeah sure. I want you to bring me all 10 normal gems and a MagicBox to me. I will pay 2500 CPs for them."));
                                    GC.SendPacket(Packets.NPCLink("I have them.", 4));
                                    // GC.SendPacket(Packets.NPCLink("How do i get a MagicBox?", 5));
                                    GC.SendPacket(Packets.NPCLink("No thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 4)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(700001, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700011, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700021, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700031, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700041, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700051, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700061, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700071, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700101, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(700121, 1) &&
                                        GC.MyChar.FindInventoryItemIDAmount(721285, 1))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(700001, 1);
                                        GC.MyChar.RemoveItemIDAmount(700011, 1);
                                        GC.MyChar.RemoveItemIDAmount(700021, 1);
                                        GC.MyChar.RemoveItemIDAmount(700031, 1);
                                        GC.MyChar.RemoveItemIDAmount(700041, 1);
                                        GC.MyChar.RemoveItemIDAmount(700051, 1);
                                        GC.MyChar.RemoveItemIDAmount(700061, 1);
                                        GC.MyChar.RemoveItemIDAmount(700071, 1);
                                        GC.MyChar.RemoveItemIDAmount(700101, 1);
                                        GC.MyChar.RemoveItemIDAmount(700121, 1);
                                        GC.MyChar.RemoveItemIDAmount(721285, 1);

                                        GC.MyChar.CPs += 2500;

                                        GC.SendPacket(Packets.NPCText("Nice doing business with you."));
                                        GC.SendPacket(Packets.NPCLink("Yeah, sure.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have all of them. So the deal is off."));
                                        GC.SendPacket(Packets.NPCLink("Alright. I'll bring them.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                /*  else if (option == 5)
                                  {
                                      GC.SendPacket(Packets.NPCSay("Well. There is a map on which are midgets and mutated bears. Mutated bears are the ones that drop MagicBoxes."));
                                      GC.SendPacket(Packets.NPCSay("Wanna go there now?"));
                                      GC.SendPacket(Packets.NPCLink("Yes.", 6));
                                      GC.SendPacket(Packets.NPCLink("No.", 255));
                                      GC.SendPacket(Packets.NPCSetFace(N.Avatar));
                                      NPCFinish(GC);
                                  }
                                  else if (option == 6)
                                      GC.MyChar.Teleport(1785, 35, 90);*/
                                else if (option == 1 || option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose the gem you want to mix."));
                                    GC.SendPacket(Packets.NPCLink("Phoenix Gem", (byte)(option * 10 + 0)));
                                    GC.SendPacket(Packets.NPCLink("Dragon Gem", (byte)(option * 10 + 1)));
                                    GC.SendPacket(Packets.NPCLink("Fury Gem", (byte)(option * 10 + 2)));
                                    GC.SendPacket(Packets.NPCLink("Rainbow Gem", (byte)(option * 10 + 3)));
                                    GC.SendPacket(Packets.NPCLink("Kylin Gem", (byte)(option * 10 + 4)));
                                    GC.SendPacket(Packets.NPCLink("Violet Gem", (byte)(option * 10 + 5)));
                                    GC.SendPacket(Packets.NPCLink("Moon Gem", (byte)(option * 10 + 6)));
                                    GC.SendPacket(Packets.NPCLink("Next.", (byte)(100 * option)));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 100 || option == 200)
                                {
                                    option = (byte)(option / 100);

                                    GC.SendPacket(Packets.NPCText("Choose the gem you want to mix."));
                                    GC.SendPacket(Packets.NPCLink("Tortoise Gem", (byte)(option * 10 + 7)));
                                    GC.SendPacket(Packets.NPCLink("Thunder Gem", (byte)(option * 10 + 8)));
                                    GC.SendPacket(Packets.NPCLink("Glory Gem", (byte)(option * 10 + 9)));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 10 && option <= 19)
                                {
                                    uint ItemID = 0;
                                    if (option <= 17)
                                        ItemID = (uint)((option - 10) * 10 + 700001);
                                    else if (option == 18)
                                        ItemID = 700101;
                                    else
                                        ItemID = 700121;

                                    if (GC.MyChar.FindInventoryItemIDAmount(ItemID, 15))
                                    {
                                        for (byte i = 0; i < 15; i++)
                                            GC.MyChar.RemoveItemIDAmount(ItemID, 1);
                                        GC.MyChar.CreateItemIDAmount((uint)(ItemID + 1), 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough gems."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option >= 20 && option <= 29)
                                {
                                    uint ItemID = 0;
                                    if (option <= 27)
                                        ItemID = (uint)((option - 20) * 10 + 700002);
                                    else if (option == 28)
                                        ItemID = 700102;
                                    else
                                        ItemID = 700122;

                                    if (GC.MyChar.FindInventoryItemIDAmount(ItemID, 15))
                                    {
                                        for (byte i = 0; i < 15; i++)
                                            GC.MyChar.RemoveItemIDAmount(ItemID, 1);
                                        GC.MyChar.CreateItemIDAmount((uint)(ItemID + 1), 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough gems."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region LoveStone
                        case 4030:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello, We Are PeaceMakers Here, What Do you Want Now?"));
                                    GC.SendPacket(Packets.NPCLink("I Want Get Married.", 1));
                                    GC.SendPacket(Packets.NPCLink("I Want Divorce.", 5));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Are you sure, This Is Great Responsbility ?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah.", 2));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("Ok, You Now Get The Flower Click In Your Love To Marry."));
                                    GC.SendPacket(Packets.NPCLink("Thanks Alot", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 1067, 0, 0, 116));
                                }
                                break;
                            }
                        #endregion
                        #region Starlit
                        case 500000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Are you unloved? Want divorce? I can help you."));
                                    GC.SendPacket(Packets.NPCLink("No, I am loved.", 255));
                                    GC.SendPacket(Packets.NPCLink("Yes, I want Divorce", 1));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("I Can get you Divorced for Just 215 CPs."));
                                    GC.SendPacket(Packets.NPCLink("Okay, here is your 215 CPs.", 2));
                                    GC.SendPacket(Packets.NPCLink("I will not give it to you!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.CPs >= 215)
                                    {
                                        Game.Character Love = Game.World.CharacterFromName(GC.MyChar.Spouse);
                                        if (Love != null)
                                        {
                                            if (Love.Inventory.Count <= 39)
                                            {
                                                Love.CreateItemIDAmount(1088002, 1);
                                                Love.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "You have gain a MeteorTear from your unloved.");
                                            }
                                            World.WorldMessage("SYSTEM", GC.MyChar.Spouse + " and " + Love.Spouse + " are dirvocied now.", 2011, 0, System.Drawing.Color.Red);
                                            Love.Spouse = "None";
                                            GC.MyChar.Spouse = "None";
                                            Love.MyClient.SendPacket(Packets.String(Love.EntityID, 6, Love.Spouse));
                                            GC.SendPacket(Packets.String(GC.MyChar.EntityID, 6, GC.MyChar.Spouse));

                                            //Database.SaveCharacter(Love, Love.MyClient.AuthInfo.Account);
                                            // Database.SaveCharacter(GC.MyChar, GC.AuthInfo.Account);
                                        }
                                        else
                                        {
                                            GC.MyChar.RemoveItemIDAmount(1088002, 1);
                                            GC.MyChar.RemoveItemIDAmount(1088001, 1);

                                            World.WorldMessage("SYSTEM", GC.MyChar.Spouse + " and " + GC.MyChar.Name + " are dirvocied now.", 2011, 0, System.Drawing.Color.Red);

                                            string SpouseName = GC.MyChar.Spouse;

                                            if (GC.MyChar.Spouse.Contains("[PM]"))
                                                SpouseName = GC.MyChar.Spouse.Replace("[PM]", "");

                                            if (GC.MyChar.Spouse.Contains("[GM]"))
                                                SpouseName = GC.MyChar.Spouse.Replace("[GM]", "");

                                            string TempAccount = "";
                                            Game.Character TempChar = Database.LoadCharacter(SpouseName, ref TempAccount, false);

                                            if (TempChar.Inventory.Count <= 39)
                                            {
                                                Item I = new Item();
                                                I.UID = (uint)Rnd.Next(10000000);
                                                I.ID = 1088002;
                                                I.MaxDur = 1;
                                                I.CurDur = 1;
                                                TempChar.Inventory.Add(I.UID, I);
                                            }

                                            TempChar.Spouse = "None";
                                            GC.MyChar.Spouse = "None";

                                            GC.SendPacket(Packets.String(GC.MyChar.EntityID, 6, GC.MyChar.Spouse));

                                            //Database.SaveCharacter(GC.MyChar, GC.AuthInfo.Account);
                                            // Database.SaveCharacter(TempChar, TempAccount);
                                            TempChar = null;
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You do not have the neededs."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Shelby
                        case 4040:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello my friend! What can I do for you?"));
                                    GC.SendPacket(Packets.NPCLink("Check my Virtue Points.", 1));
                                    GC.SendPacket(Packets.NPCLink("Give me Prizes for my Virtues.", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("You have " + GC.MyChar.VP + " of virtue points. Enjoy it."));
                                    GC.SendPacket(Packets.NPCLink("Okay, Thanks!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("I can give a Meteor for 1k and a DragonBall for 50k of virtue points."));
                                    GC.SendPacket(Packets.NPCLink("Give me a Meteor for 1k", 4));
                                    GC.SendPacket(Packets.NPCLink("Give me a DragonBall for 50k", 3));
                                    GC.SendPacket(Packets.NPCLink("Let me think.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3 || option == 4)
                                {
                                    if (GC.MyChar.Inventory.Count <= 39)
                                    {
                                        ulong Price = 1000;
                                        if (option == 3)
                                            Price = 50000;

                                        if (GC.MyChar.VP >= Price)
                                        {
                                            GC.MyChar.VP -= Price;
                                            uint ItemID = (uint)(1087997 + option);

                                            GC.MyChar.CreateItemIDAmount(ItemID, 1);
                                            GC.SendPacket(Packets.NPCText("Here is your Prize."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You do not have the right price."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You do not have one free slot in your inventory."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region MillionaireLee
                        case 4050:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi i can make scrolls for your Meteors Or DragonBalls"));
                                    GC.SendPacket(Packets.NPCLink("Compose My Meteors.", 1));
                                    GC.SendPacket(Packets.NPCLink("Compose My DragonBalls.", 2));
                                    GC.SendPacket(Packets.NPCLink("Exchange My DragonBall For Meteors Scroll.", 3));
                                    GC.SendPacket(Packets.NPCLink("No Thanks!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088001, 10))
                                    {
                                        for (int i = 0; i < 10; i++)
                                            GC.MyChar.RemoveItemIDAmount(1088001, 1);
                                        GC.MyChar.CreateItemIDAmount(720027, 1);
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "You successfully composed 10 meteors into a meteor scroll.");
                                    }
                                    else
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry you dont have 10 Meteors!For compuse in MeteorScroll.");
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, 10))
                                    {
                                        for (int i = 0; i < 10; i++)
                                            GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                        GC.MyChar.CreateItemIDAmount(720028, 1);
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "You successfully composed 10 dragon balls into a dragon ball scroll.");
                                    }
                                    else
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry you dont have 10 DragonBalls !For compuse in DbScroll.");
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("Are You Sure For You Choise Cuz I Will Give You Just 15 Meteor Scrolls."));
                                    GC.SendPacket(Packets.NPCLink("I'm Sure.", 4));
                                    GC.SendPacket(Packets.NPCLink("No way!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 4)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, 1) && GC.MyChar.Inventory.Count <= 26)
                                    {
                                        GC.MyChar.RemoveItemIDAmount(1088001, 1);
                                        for (int i = 0; i < 15; i++)
                                            GC.MyChar.CreateItemIDAmount(720027, 1);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region MerchantClerk
                        case 4060:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Want to become a merchant? If you get scammed while you are a merchant or any other time we will not help you."));
                                    GC.SendPacket(Packets.NPCLink("Become Merchant!", 1));
                                    GC.SendPacket(Packets.NPCLink("Remove Merchant!", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Merchant = MerchantTypes.Yes;
                                    GC.SendPacket(Packets.NPCText("You Becomes Merchant Now!"));
                                    GC.SendPacket(Packets.NPCLink("Thanks!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    GC.MyChar.Merchant = MerchantTypes.Not;
                                    GC.SendPacket(Packets.NPCText("You Remove Merchant Now!"));
                                    GC.SendPacket(Packets.NPCLink("Thanks!", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region CPAdmin
                        case 4070:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello, I can trade with you 15CPs for a Meteor, 215CPs for a DragonBall and 2150CPs For DBScroll."));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my Meteor!", 1));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my DragonBall!", 2));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my DBScroll!", 3));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 1088000)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 215;
                                            GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                        }
                                    }
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088001, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 1088001)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 15;
                                            GC.MyChar.RemoveItemIDAmount(1088001, 1);
                                        }
                                    }
                                }
                                if (option == 3)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(720028, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 720028)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 2150;
                                            GC.MyChar.RemoveItemIDAmount(720028, 1);
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region CollectorZhao
                        case 500001: // CollectorZhao
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("I am fond of collecting all kinds of treasure. If you happen to have any"));
                                    GC.SendPacket(Packets.NPCText("\nDisguiseAmuets, LfeFruitBasket, or PenitenceAmulets, you may sell them"));
                                    GC.SendPacket(Packets.NPCText("\nTo me. I will pay you ten cps for each one."));
                                    GC.SendPacket(Packets.NPCLink("Here are my DisguisAmulets (No\nmore than 15).", 1));
                                    GC.SendPacket(Packets.NPCLink("Here are my LifeFruitBaskets (No\nmore than 15).", 2));
                                    GC.SendPacket(Packets.NPCLink("Here are my PenitenceAmulets (No\nmore than 15.", 3));
                                    GC.SendPacket(Packets.NPCLink("I have none of them.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }

                                if (option == 1)//DisguiseAmulets
                                {

                                    if (GC.MyChar.FindInventoryItemIDAmount(723724, 1))
                                    {
                                        GC.MyChar.CPs += 100000;
                                        GC.WorldMessage(2005, GC.MyChar.Name + "You got now 100 CPs more.", System.Drawing.Color.Azure);
                                        GC.MyChar.RemoveItemIDAmount(723724, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You do not have any DisguiseAmulets. Whenever you have one of DisguiseAmulets,"));
                                        GC.SendPacket(Packets.NPCText("\nLifeFruitBaskets or PenitenceAmulets, just come here."));
                                        GC.SendPacket(Packets.NPCLink("See you again.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)//LifeFruitBaskets.
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(723725, 1))
                                    {
                                        GC.MyChar.CPs += 100000;
                                        GC.WorldMessage(2005, GC.MyChar.Name + "You got now 100 CPs more.", System.Drawing.Color.Azure);
                                        GC.MyChar.RemoveItemIDAmount(723725, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You do not have any LifeFruitBaskets. "));
                                        GC.SendPacket(Packets.NPCText("\n just come back whenever y have any."));
                                        GC.SendPacket(Packets.NPCLink("See you again.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)//PenitenceAmulet
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(720128, 1))
                                    {
                                        GC.MyChar.CPs += 100000;
                                        GC.WorldMessage(2005, GC.MyChar.Name + "You got now 100 CPs more.", System.Drawing.Color.Azure);
                                        GC.MyChar.RemoveItemIDAmount(720128, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You do not have any PenitenceAmulets. "));
                                        GC.SendPacket(Packets.NPCText("\n just come back whenever y have any."));
                                        GC.SendPacket(Packets.NPCLink("See you again.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }

                                break;
                            }
                        #endregion
                        #region BodySize
                        case 4010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello There, I Can Change You're Body Size, I Charge 1 DragonBall, Would You Like To Change? "));
                                    GC.SendPacket(Packets.NPCLink("Yes Please", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {

                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, 1))
                                    {
                                        for (byte i = 0; i < 1; i++)
                                            GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                        {
                                            if (GC.MyChar.Body == 1004)
                                                GC.MyChar.Body -= 1;
                                            else if (GC.MyChar.Body == 1003)
                                                GC.MyChar.Body += 1;
                                            if (GC.MyChar.Body == 2002)
                                                GC.MyChar.Body -= 1;
                                            else if (GC.MyChar.Body == 2001)
                                                GC.MyChar.Body += 1;

                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry, You Don't Have A DragonBall"));
                                        GC.SendPacket(Packets.NPCLink("Ok, Sorry About That.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region PK Down
                        case 4080:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hay Iam Who Down Your PKPoints So Select You Mode?"));
                                    GC.SendPacket(Packets.NPCLink("Down My 100 PKPoints For 15 K CPs ", 1));
                                    GC.SendPacket(Packets.NPCLink("Down All My PKPoints For 220 K CPs ", 2));
                                    GC.SendPacket(Packets.NPCLink("No I Dont Want", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }

                                if (option == 1)
                                {
                                    if (GC.MyChar.PKPoints >= 100 && GC.MyChar.CPs >= 15000)
                                    {
                                        GC.MyChar.CPs -= 15000;
                                        GC.MyChar.PKPoints -= 100;
                                        if (GC.MyChar.PKPoints < 100)
                                        {
                                            GC.MyChar.StatEff.Remove(StatusEffectEn.RedName);
                                        }
                                        else if (GC.MyChar.PKPoints < 200)
                                        {
                                            GC.MyChar.StatEff.Remove(StatusEffectEn.BlackName);
                                        }
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);

                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 15 K CPs Or Your PKPoints Less Than 100 Points."));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }

                                if (option == 2)
                                {
                                    if (GC.MyChar.PKPoints >= 200 && GC.MyChar.CPs >= 200000)
                                    {
                                        GC.MyChar.CPs -= 220000;
                                        GC.MyChar.PKPoints = 0;
                                        if (GC.MyChar.PKPoints < 100)
                                        {
                                            GC.MyChar.StatEff.Remove(StatusEffectEn.RedName);
                                        }
                                        else if (GC.MyChar.PKPoints < 200)
                                        {
                                            GC.MyChar.StatEff.Remove(StatusEffectEn.BlackName);
                                        }
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);

                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 220K CPs Or Your PKPoints Less Than 200 Points."));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }

                                break;
                            }
                        #endregion
                        #region Confiscator
                        case 4090:
                            {
                                if (option == 0)
                                {
                                    NPCText("The Red/Black name can redeem their detained equipment within 7 day.The CPs they pay for their", GC);
                                    NPCText(" guilt will be rewarded you the whi killed them! If they do not play, the equipment detained will be given to the one who killed them as rewards", GC);
                                    NPCLink("I wana to redeem my equipment", 1, GC);
                                    NPCLink("I wana to claim my equipment", 2, GC);
                                    NPCLink("Just passing by.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    NPCText("You can pay some CPs to redeem you equipment", GC);
                                    NPCLink("Yes,i wana my equiment", 3, GC);
                                    NPCLink("Let me Think", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 3)
                                {
                                    NPCText("You do not have any equipment detained", GC);
                                    NPCLink("Go it", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 336, 0, 0, 126));
                                }

                                if (option == 2)
                                {
                                    NPCText("The red/black name you killed can redeem equipment within 7 days. You will be reward the CPs that he/she pay for the guilt. If not, you will be get the detained equipment as rewards ", GC);
                                    NPCLink("i wana to claim here", 4, GC);
                                    NPCLink("Not yet", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 4)
                                {
                                    NPCText("Sorry, you do not have anything to claim now", GC);
                                    NPCLink("I see", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 337, 0, 0, 126));
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region TrainingCenter
                        #region WarriorTrainer
                        case 6000:
                            {
                                if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Hi, I am WarriorTrainer And I Teach Only Warriors, I Help You To Get Promoted And To Learn Warrior Skills It Make You Powerful."));
                                        GC.SendPacket(Packets.NPCLink("I Want Get Promoted.", 1));
                                        GC.SendPacket(Packets.NPCLink("Learn Skills.", 2));
                                        GC.SendPacket(Packets.NPCLink("Learn PureSkills.", 3));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #region Promote
                                    if (option == 1)
                                    {
                                        if (GC.MyChar.Job <= 24)
                                        {
                                            GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString()));
                                            if (GC.MyChar.Job == 20)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an Meteor."));
                                            else if (GC.MyChar.Job == 21)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an MeteorTear."));
                                            else if (GC.MyChar.Job == 22)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an DragonBall."));
                                            else if (GC.MyChar.Job == 23)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a Emerald."));
                                            else if (GC.MyChar.Job == 24)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a MoonBox."));
                                            GC.SendPacket(Packets.NPCLink("Promote me.", 10));
                                            GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already WarriorMaster, i cannot promote you anymore."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (option == 10)
                                    {
                                        if (GC.MyChar.Level >= GC.MyChar.PromoteLevel)
                                        {
                                            uint ID = GC.MyChar.PromoteItems;
                                            if (GC.MyChar.FindInventoryItemIDAmount(ID, 1))
                                            {
                                                GC.MyChar.RemoveItemIDAmount(ID, 1);
                                                GC.MyChar.Job++;
                                                if (GC.MyChar.Job == 21)
                                                    GC.MyChar.CreateItemIDAmount(723713, 1);// Class1MoneyBag
                                                else if (GC.MyChar.Job == 22)
                                                    GC.MyChar.CreateItemIDAmount(720027, 1);// MeteorScroll
                                                else if (GC.MyChar.Job == 23)
                                                    GC.MyChar.CreateItemIDAmount(723714, 1);// Class2MoneyBag
                                                else if (GC.MyChar.Job == 24)
                                                    GC.MyChar.CreateItemIDAmount(1088000, 5);// 5 DragonBalls
                                                else if (GC.MyChar.Job == 25)
                                                    GC.MyChar.CreateItemIDAmount(2100045, 1);// MagicalBottle

                                                GC.SendPacket(Packets.NPCText("Congratulations! You are now " + ((Game.Character.JobName)GC.MyChar.Job).ToString() + " and received a Gift."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have the Required Materials."));
                                                GC.SendPacket(Packets.NPCLink("I'll Buy Them.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Not Qualified Yet. Your Level Is Too Low."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                    #region Skills
                                    else if (option == 2)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)2];
                                        if (Skills.Count < 8)
                                        {
                                            for (byte i = 0; i < Skills.Count; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        }
                                        else
                                        {
                                            for (byte i = 0; i < 7; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                            GC.SendPacket(Packets.NPCLink("Next", 100));
                                        }
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option == 100)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)2];
                                        for (byte i = 7; i < Skills.Count; i++)
                                            GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        
                                        GC.SendPacket(Packets.NPCLink("Previous", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option >= 20 && option <= 34)
                                    {
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)2];
                                        SkillLearn S = (SkillLearn)Skills[(byte)(option - 20)];
                                        if (GC.MyChar.Level >= S.LevelReq)
                                        {
                                            GC.MyChar.NewSkill(S.ToSkill());
                                            GC.SendPacket(Packets.NPCText("Congratulation You Learned " + ((Skills.SkillIDs)(S.ID)).ToString() + " ."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not high level enough."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("You Are`t Warrior. You Want Still Me"));
                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region ArcherTrainer
                        case 6010:
                            {
                                if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Hi, I am ArcherTrainer And I Teach Only Archers, I Help You To Get Promoted And To Learn Archer Skills It Make You Powerful."));
                                        GC.SendPacket(Packets.NPCLink("I Want Get Promoted.", 1));
                                        GC.SendPacket(Packets.NPCLink("Learn Skills.", 2));
                                        GC.SendPacket(Packets.NPCLink("Learn PureSkills.", 3));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #region Promote
                                    if (option == 1)
                                    {
                                        if (GC.MyChar.Job <= 44)
                                        {
                                            GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString()));
                                            if (GC.MyChar.Job == 40)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an Meteor."));
                                            else if (GC.MyChar.Job == 41)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an MeteorTear."));
                                            else if (GC.MyChar.Job == 42)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an DragonBall."));
                                            else if (GC.MyChar.Job == 43)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a Emerald."));
                                            else if (GC.MyChar.Job == 44)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a MoonBox."));
                                            GC.SendPacket(Packets.NPCLink("Promote me.", 10));
                                            GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already ArcherMaster, i cannot promote you anymore."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (option == 10)
                                    {
                                        if (GC.MyChar.Level >= GC.MyChar.PromoteLevel)
                                        {
                                            uint ID = GC.MyChar.PromoteItems;
                                            if (GC.MyChar.FindInventoryItemIDAmount(ID, 1))
                                            {
                                                GC.MyChar.RemoveItemIDAmount(ID, 1);
                                                GC.MyChar.Job++;
                                                if (GC.MyChar.Job == 41)
                                                    GC.MyChar.CreateItemIDAmount(723713, 1);// Class1MoneyBag
                                                else if (GC.MyChar.Job == 42)
                                                    GC.MyChar.CreateItemIDAmount(720027, 1);// MeteorScroll
                                                else if (GC.MyChar.Job == 43)
                                                    GC.MyChar.CreateItemIDAmount(723714, 1);// Class2MoneyBag
                                                else if (GC.MyChar.Job == 44)
                                                    GC.MyChar.CreateItemIDAmount(1088000, 5);// 5 DragonBalls
                                                else if (GC.MyChar.Job == 45)
                                                    GC.MyChar.CreateItemIDAmount(2100045, 1);// MagicalBottle

                                                GC.SendPacket(Packets.NPCText("Congratulations! You are now " + ((Game.Character.JobName)GC.MyChar.Job).ToString() + " and received a Gift."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have the Required Materials."));
                                                GC.SendPacket(Packets.NPCLink("I'll Buy Them.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Not Qualified Yet. Your Level Is Too Low."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                    #region Skills
                                    else if (option == 2)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)4];
                                        if (Skills.Count < 8)
                                        {
                                            for (byte i = 0; i < Skills.Count; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        }
                                        else
                                        {
                                            for (byte i = 0; i < 7; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                            GC.SendPacket(Packets.NPCLink("Next", 100));
                                        }
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option == 100)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)4];
                                        for (byte i = 7; i < Skills.Count; i++)
                                            GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));

                                        GC.SendPacket(Packets.NPCLink("Previous", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option >= 20 && option <= 34)
                                    {
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)4];
                                        SkillLearn S = (SkillLearn)Skills[(byte)(option - 20)];
                                        if (GC.MyChar.Level >= S.LevelReq)
                                        {
                                            GC.MyChar.NewSkill(S.ToSkill());
                                            GC.SendPacket(Packets.NPCText("Congratulation You Learned " + ((Skills.SkillIDs)(S.ID)).ToString() + " ."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not high level enough."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("You Are`t Archer. You Want Still Me"));
                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region NinjaTrainer
                        case 6020:
                            {
                                if (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55)
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Hi, I am NinjaTrainer And I Teach Only Ninjas, I Help You To Get Promoted And To Learn Ninja Skills It Make You Powerful."));
                                        GC.SendPacket(Packets.NPCLink("I Want Get Promoted.", 1));
                                        GC.SendPacket(Packets.NPCLink("Learn Skills.", 2));
                                        GC.SendPacket(Packets.NPCLink("Learn PureSkills.", 3));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #region Promote
                                    if (option == 1)
                                    {
                                        if (GC.MyChar.Job <= 54)
                                        {
                                            GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString()));
                                            if (GC.MyChar.Job == 50)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an Meteor."));
                                            else if (GC.MyChar.Job == 51)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an MeteorTear."));
                                            else if (GC.MyChar.Job == 52)
                                                GC.SendPacket(Packets.NPCText(" Also you will need an DragonBall."));
                                            else if (GC.MyChar.Job == 53)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a Emerald."));
                                            else if (GC.MyChar.Job == 54)
                                                GC.SendPacket(Packets.NPCText(" Also you will need a MoonBox."));
                                            GC.SendPacket(Packets.NPCLink("Promote me.", 10));
                                            GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already NinjaMaster, i cannot promote you anymore."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (option == 10)
                                    {
                                        if (GC.MyChar.Level >= GC.MyChar.PromoteLevel)
                                        {
                                            uint ID = GC.MyChar.PromoteItems;
                                            if (GC.MyChar.FindInventoryItemIDAmount(ID, 1))
                                            {
                                                GC.MyChar.RemoveItemIDAmount(ID, 1);
                                                GC.MyChar.Job++;
                                                if (GC.MyChar.Job == 51)
                                                    GC.MyChar.CreateItemIDAmount(723713, 1);// Class1MoneyBag
                                                else if (GC.MyChar.Job == 52)
                                                    GC.MyChar.CreateItemIDAmount(720027, 1);// MeteorScroll
                                                else if (GC.MyChar.Job == 53)
                                                    GC.MyChar.CreateItemIDAmount(723714, 1);// Class2MoneyBag
                                                else if (GC.MyChar.Job == 54)
                                                    GC.MyChar.CreateItemIDAmount(1088000, 5);// 5 DragonBalls
                                                else if (GC.MyChar.Job == 55)
                                                    GC.MyChar.CreateItemIDAmount(2100045, 1);// MagicalBottle

                                                GC.SendPacket(Packets.NPCText("Congratulations! You are now " + ((Game.Character.JobName)GC.MyChar.Job).ToString() + " and received a Gift."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have the Required Materials."));
                                                GC.SendPacket(Packets.NPCLink("I'll Buy Them.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Not Qualified Yet. Your Level Is Too Low."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                    #region Skills
                                    else if (option == 2)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)5];
                                        if (Skills.Count < 8)
                                        {
                                            for (byte i = 0; i < Skills.Count; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        }
                                        else
                                        {
                                            for (byte i = 0; i < 7; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                            GC.SendPacket(Packets.NPCLink("Next", 100));
                                        }
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option == 100)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)5];
                                        for (byte i = 7; i < Skills.Count; i++)
                                            GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));

                                        GC.SendPacket(Packets.NPCLink("Previous", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option >= 20 && option <= 34)
                                    {
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)5];
                                        SkillLearn S = (SkillLearn)Skills[(byte)(option - 20)];
                                        if (GC.MyChar.Level >= S.LevelReq)
                                        {
                                            GC.MyChar.NewSkill(S.ToSkill());
                                            GC.SendPacket(Packets.NPCText("Congratulation You Learned " + ((Skills.SkillIDs)(S.ID)).ToString() + " ."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not high level enough."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("You Are`t Ninja. You Want Still Me"));
                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region WizardTrainer
                        case 6030:
                            {
                                if (GC.MyChar.Job >= 100 && GC.MyChar.Job <= 145)
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Hi, I am WizardTrainer And I Teach Only Wizards, I Help You To Get Promoted And To Learn Wizard Skills It Make You Powerful."));
                                        GC.SendPacket(Packets.NPCLink("I Want Get Promoted.", 1));
                                        GC.SendPacket(Packets.NPCLink("Learn Skills.", 2));
                                        GC.SendPacket(Packets.NPCLink("Learn PureSkills.", 3));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #region Promote
                                    if (option == 1)
                                    {
                                        if (!(GC.MyChar.Job == 135 || GC.MyChar.Job == 145))
                                        {
                                            if (GC.MyChar.Job == 100)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString() + "."));
                                                GC.SendPacket(Packets.NPCText(" Also you will need an Meteor."));
                                            }
                                            else if (GC.MyChar.Job == 132 || GC.MyChar.Job == 142)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString() + "."));
                                                GC.SendPacket(Packets.NPCText(" Also you will need an DragonBall."));
                                            }
                                            else if (GC.MyChar.Job == 133 || GC.MyChar.Job == 143)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString() + "."));
                                                GC.SendPacket(Packets.NPCText(" Also you will need an Emerald."));
                                            }
                                            else if (GC.MyChar.Job == 134 || GC.MyChar.Job == 144)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need to be level " + GC.MyChar.PromoteLevel + " to promote to " + ((Game.Character.JobName)(GC.MyChar.Job + 1)).ToString() + "."));
                                                GC.SendPacket(Packets.NPCText(" Also you will need a MoonBox."));
                                            }
                                            if (GC.MyChar.Job == 101)
                                            {
                                                GC.SendPacket(Packets.NPCText("You have to choose if you want to become a FireWizard or a WaterWizard."));
                                                GC.SendPacket(Packets.NPCText(" Also you will need an MeteorTear."));
                                                GC.SendPacket(Packets.NPCLink("Promote me to FireWizard", 10));
                                                GC.SendPacket(Packets.NPCLink("Promote me to WaterWizard", 11));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCLink("Promote me.", 10));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Already Wizard, i cannot promote you anymore."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (option == 10 || option == 11)
                                    {
                                        if (GC.MyChar.Level >= GC.MyChar.PromoteLevel)
                                        {
                                            uint ID = GC.MyChar.PromoteItems;
                                            if (GC.MyChar.FindInventoryItemIDAmount(ID, 1))
                                            {
                                                GC.MyChar.RemoveItemIDAmount(ID, 1);
                                                if (GC.MyChar.Job == 101)
                                                {
                                                    if (option == 10)
                                                        GC.MyChar.Job = 142;
                                                    else
                                                        GC.MyChar.Job = 132;
                                                }
                                                else
                                                {
                                                    GC.MyChar.Job++;
                                                    if (GC.MyChar.Job == 101)
                                                        GC.MyChar.CreateItemIDAmount(723713, 1);// Class1MoneyBag
                                                    else if (GC.MyChar.Job == 132 || GC.MyChar.Job == 142)
                                                        GC.MyChar.CreateItemIDAmount(720027, 1);// MeteorScroll
                                                    else if (GC.MyChar.Job == 133 || GC.MyChar.Job == 143)
                                                        GC.MyChar.CreateItemIDAmount(723714, 1);// Class2MoneyBag
                                                    else if (GC.MyChar.Job == 134 || GC.MyChar.Job == 144)
                                                        GC.MyChar.CreateItemIDAmount(1088000, 5);// 5 DragonBalls
                                                    else if (GC.MyChar.Job == 135 || GC.MyChar.Job == 145)
                                                        GC.MyChar.CreateItemIDAmount(2100045, 1);// MagicalBottle
                                                }
                                                GC.SendPacket(Packets.NPCText("Congratulations! You are now " + ((Game.Character.JobName)GC.MyChar.Job).ToString() + " and received a Gift."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have the Required Materials."));
                                                GC.SendPacket(Packets.NPCLink("I'll Buy Them.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Are Not Qualified Yet. Your Level Is Too Low."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                    #region Skills
                                    else if (option == 2)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        byte e = 10;
                                        if (GC.MyChar.Job > 130) e = 13;
                                        if (GC.MyChar.Job > 140) e = 14;
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)e];
                                        if (Skills.Count < 8)
                                        {
                                            for (byte i = 0; i < Skills.Count; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        }
                                        else
                                        {
                                            for (byte i = 0; i < 7; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                            GC.SendPacket(Packets.NPCLink("Next", 100));
                                        }
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option == 100)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose from the skills listed below."));
                                        byte e = 10;
                                        if (GC.MyChar.Job > 130) e = 13;
                                        if (GC.MyChar.Job > 140) e = 14;
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)e];
                                        {
                                            for (byte i = 7; i < Skills.Count; i++)
                                                GC.SendPacket(Packets.NPCLink(((Skills.SkillIDs)(((SkillLearn)Skills[i]).ID)).ToString() + "(Lvl " + ((SkillLearn)Skills[i]).LevelReq.ToString() + " )", (byte)(20 + i)));
                                        }
                                        GC.SendPacket(Packets.NPCLink("Previous", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (option >= 20 && option <= 34)
                                    {
                                        byte e = 10;
                                        if (GC.MyChar.Job > 130) e = 13;
                                        if (GC.MyChar.Job > 140) e = 14;
                                        ArrayList Skills = (ArrayList)Database.SkillForLearning[(byte)e];
                                        SkillLearn S = (SkillLearn)Skills[(byte)(option - 20)];
                                        if (GC.MyChar.Level >= S.LevelReq)
                                        {
                                            GC.MyChar.NewSkill(S.ToSkill());
                                            GC.SendPacket(Packets.NPCText("Congratulation You Learned " + ((Skills.SkillIDs)(S.ID)).ToString() + " ."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not high level enough."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    GC.SendPacket(Packets.NPCText("You Are`t Wizard. You Want Still Me"));
                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region Guild NPC`s
                        #region GW Enter GuildArea
                        case 8000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do You Want Enter The Guild Arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1038, 347, 338);
                                }
                                break;
                            }
                        #endregion
                        #region GW Leave GuildArea
                        case 8010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do You Want Leave The Guild Arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yeah.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1002, 439, 390);
                                break;
                            }
                        #endregion
                        #region GW Jail Warden
                        case 8020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("You Can Leave This Fuckin Place Now."));
                                    GC.SendPacket(Packets.NPCLink("Get Me Out From Here", 1));
                                    GC.SendPacket(Packets.NPCLink("I'll wait here.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.MyChar.Teleport(1002, 430, 380);
                                }
                                break;
                            }
                        #endregion
                        #region GuildOfficer
                        case 8030:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hi I am the Director of the (Super GuildWar), yeah yeah that is start every month", GC);
                                    NPCLink("Show Me.", 1, GC);
                                    NPCLink("I see.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if ((GC.MyChar.MyGuild == Extra.GuildWars.LastWinner) && (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader))
                                    {
                                        GC.SendPacket(Packets.NPCText("Hello " + GC.MyChar.Name + ", Your Great Work Makes You The One This Week, now you will get Awsome Prize but in first you must give me The LordToken, Do You Have It?."));
                                        GC.SendPacket(Packets.NPCLink("Yeah I Have It.", 2));
                                        GC.SendPacket(Packets.NPCLink("No I Don`t Have It.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("Sorry You Are`t The GuildWinner In This Week.", GC);
                                        NPCLink("Sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(723467, 3))
                                    {
                                        GC.SendPacket(Packets.NPCText("Hello " + GC.MyChar.Name + ", Here Is The Rule, if you have (One LordToken you can claim BronzePrize) Or (Two LordToken you can claim SilverPrize) OR (Three LordToken you can claim GoldPrize), Great Now What You Have In Your Inventory."));
                                        GC.SendPacket(Packets.NPCLink("Yeah I Have It.", 3));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry " + GC.MyChar.Name + ", You Don`t Have The LordToken."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 3)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(723467, 1))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(723467, 1);
                                        GC.MyChar.CreateItemIDAmount(2100055, 1);
                                    }
                                    else if (GC.MyChar.FindInventoryItemIDAmount(723467, 2))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(723467, 2);
                                        GC.MyChar.CreateItemIDAmount(2100065, 1);
                                    }
                                    else if (GC.MyChar.FindInventoryItemIDAmount(723467, 3))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(723467, 3);
                                        GC.MyChar.CreateItemIDAmount(2100075, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry " + GC.MyChar.Name + ", You Don`t Have The LordToken."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region GW Winner
                        case 8040:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello My Friends, Take Your Reward If Your Guild Is The Winner In The GuildWar"));
                                    GC.SendPacket(Packets.NPCLink("Iam GuildLeader", 1));
                                    GC.SendPacket(Packets.NPCLink("Iam DeputyLeader ", 2));
                                    GC.SendPacket(Packets.NPCLink("Iam Guild Member ", 3));
                                    GC.SendPacket(Packets.NPCLink("Thanks", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (Extra.GuildWars.War == false)
                                    {
                                        if (GC.MyChar.MyGuild == Extra.GuildWars.LastWinner && (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader))
                                        {
                                            if (GC.MyChar.TopGuildLeader < 1)
                                            {
                                                GC.MyChar.TopGuildLeader = 1;
                                                GC.MyChar.StatEff.Add(StatusEffectEn.TopGuildLeader);
                                                GC.MyChar.CPs += 2000000;
                                                GC.MyChar.CreateItemIDAmount(723467, 1);
                                                GC.WorldMessage(2011, GC.MyChar.Name + "Has won the TopGuildLeader Sucussefully.", System.Drawing.Color.Black);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("I have no prize for you."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not the TopGuildLeader."));
                                            GC.SendPacket(Packets.NPCLink("Sorry iam just kidding!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCLink("The Guild War Is Still On!", 255));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (Extra.GuildWars.War == false)
                                    {
                                        if (GC.MyChar.MyGuild == Extra.GuildWars.LastWinner && (GC.MyChar.GuildRank == Server.Features.GuildRank.DeputyLeader))
                                        {
                                            string[] AllLines = System.IO.File.ReadAllLines(Program.ConquerPath + @"Tops\TopDeputy.txt");
                                            if (AllLines.Length < 5)
                                            {
                                                string[] AllLine = System.IO.File.ReadAllLines(Program.ConquerPath + @"Tops\TopDeputy.txt");
                                                if (GC.MyChar.TopDeputyLeader < 1)
                                                {
                                                    GC.MyChar.TopDeputyLeader = 1;
                                                    GC.MyChar.StatEff.Add(StatusEffectEn.TopDeputyLeader);
                                                    GC.MyChar.CPs += 500000;
                                                    GC.WorldMessage(2011, GC.MyChar.Name + "Has won the Top deputy prize Succuseful.", System.Drawing.Color.Black);

                                                    StreamWriter sw;

                                                    sw = File.AppendText(Program.ConquerPath + @"/Tops/TopDeputy.txt"); sw.WriteLine("" + GC.MyChar.EntityID + "#34359738368"); sw.Close();
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You got your prize befor."));
                                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("I have no prize for you."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not the TopDeputyLeader."));
                                            GC.SendPacket(Packets.NPCLink("Sorry iam just kidding!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCLink("The Guild War Is Still On!", 255));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    if (Extra.GuildWars.War == false)
                                    {
                                        if (GC.MyChar.MyGuild == Extra.GuildWars.LastWinner && (GC.MyChar.GuildRank == Server.Features.GuildRank.Member))
                                        {
                                            string[] AllLines = System.IO.File.ReadAllLines(Program.ConquerPath + @"Tops\TopMember.txt");
                                            if (AllLines.Length < 5)
                                            {
                                                string[] AllLine = System.IO.File.ReadAllLines(Program.ConquerPath + @"Tops\TopMember.txt");
                                                if (AllLines.Contains("" + GC.MyChar.EntityID + "#"))
                                                {
                                                    GC.SendPacket(Packets.NPCText("u got your prize before !"));
                                                    GC.SendPacket(Packets.NPCLink("sorry!", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.MyChar.CPs += 100000;
                                                    StreamWriter sw;
                                                    sw = File.AppendText(Program.ConquerPath + @"Tops/TopMember.txt"); sw.WriteLine("" + GC.MyChar.EntityID + "#"); sw.Close();
                                                }
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("I have no prize for you."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You are not the TopMember."));
                                            GC.SendPacket(Packets.NPCLink("Sorry iam just kidding!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCLink("The Guild War Is Still On!", 255));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region GW GuildDirector
                        case 8050:
                            {
                                if (option == 0)
                                {
                                    NPCText("I am what my name says. I create and manage guilds. So what do you want to do?You Have Level 120 For Create GuildHere?", GC);
                                    NPCLink("Create Guild", 1, GC);
                                    NPCLink("Deputy Menu", 3, GC);
                                    NPCLink("Disband My Guild", 5, GC);
                                    NPCLink("Enemies List", 45, GC);
                                    NPCLink("Allies List", 40, GC);
                                    NPCLink("Just passing by.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 45)
                                {
                                    NPCText("Guild Enemyes Menue", GC);
                                    NPCLink("Add Enemies", 8, GC);
                                    NPCLink("Remove Enemies", 10, GC);
                                    NPCLink("Explore Enemys", 41, GC);
                                    NPCLink("Just passing by.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 40)
                                {
                                    NPCText("Guild allies Menue", GC);
                                    NPCLink("Add Allis", 13, GC);
                                    NPCLink("Un-Allis", 15, GC);
                                    NPCLink("Explore allais", 29, GC);
                                    NPCLink("Just passing by.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 3)
                                {
                                    NPCText("Debuty menue", GC);
                                    NPCLink("add Deputize", 20, GC);
                                    NPCLink("Remove Deputize", 21, GC);
                                    NPCLink("Just passing by.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                #region Create
                                else if (option == 1)
                                {
                                    if (GC.MyChar.MyGuild == null && GC.MyChar.Job < 160)
                                    {
                                        NPCText("I don't know why, but you have to be level 90 or higher and need 1 million silvers to create one.", GC);
                                        NPCLink2("Create", 2, GC);
                                        NPCLink("No, i changed my mind.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.MyGuild != null)
                                    {
                                        NPCText("You are already in a guild. You cannot create a guild.", GC);
                                        NPCLink("Oh, i forgot...", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("You Can not Create A Guild.", GC);
                                        NPCLink("Oh, i forgot...", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Level >= 90 && GC.MyChar.MyGuild == null)
                                    {
                                        if (GC.MyChar.Silvers >= 1000000)
                                        {
                                            string GuildName = ReadString(Data);
                                            if (Features.Guilds.ValidName(GuildName))
                                            {
                                                GC.MyChar.Silvers -= 1000000;
                                                ushort NewGuildID = (ushort)Rnd.Next(ushort.MaxValue);
                                                while (Features.Guilds.AllTheGuilds.ContainsKey(NewGuildID))
                                                    NewGuildID = (ushort)Rnd.Next(ushort.MaxValue);
                                                Features.Guilds.CreateNewGuild(GuildName, NewGuildID, GC.MyChar);
                                                Game.World.Spawn(GC.MyChar, false);
                                                Database.CreateGuild(GC.MyChar);
                                                GC.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                                                GC.SendPacket(Packets.String(GC.MyChar.MyGuild.GuildID, (byte)Game.StringType.GuildName, GC.MyChar.MyGuild.GuildName));
                                                Features.Guilds.SaveGuilds();
                                                NPCText("Congratulations! You now have your own guild.", GC);
                                                NPCLink("Thanks.", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                NPCText("Choose another name, this name is taken or has invalid length.", GC);
                                                NPCLink2("Create", 2, GC);
                                                NPCLink("I changed my mind.", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            NPCText("I said you need 1 million silvers and you don't have enough.", GC);
                                            NPCLink("Ok ok, i'll go bring the money.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("Forget it! You're too weak.", GC);
                                        NPCLink("Alright, i will get stronger.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region Deputize ADD
                                else if (option == 20)
                                {
                                    if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                    {
                                        NPCText("Insert the name of the player in your guild you want to make a deputy leader.", GC);
                                        NPCLink2("Here", 4, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("You are not a guild leader.", GC);
                                        NPCLink("Silly me.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 4)
                                {
                                    string PlayerName = ReadString(Data);
                                    Features.MemberInfo M = GC.MyChar.MyGuild.MembOfName(PlayerName);
                                    if (M != null && M.MembName == PlayerName && ((Hashtable)GC.MyChar.MyGuild.Members[(byte)90]).Count < 5)
                                    {
                                        M.Rank = Server.Features.GuildRank.DeputyLeader;
                                        ((Hashtable)GC.MyChar.MyGuild.Members[(byte)50]).Remove(M.MembID);
                                        ((Hashtable)GC.MyChar.MyGuild.Members[(byte)90]).Add(M.MembID, M);
                                        Game.Character C = M.Info;
                                        if (C != null)
                                        {
                                            C.GuildRank = Server.Features.GuildRank.DeputyLeader;
                                            Game.World.Spawn(C, false);
                                            C.MyClient.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                                            Features.Guilds.SaveGuilds();
                                        }
                                        Features.Guilds.SaveGuilds();
                                    }
                                    else
                                    {
                                        NPCText("The player is not in your guild or is not a normal member. By the way, the max number deputy leaders there can be is 5.", GC);
                                        NPCLink("Oh, sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region remove Deputize
                                else if (option == 21)
                                {
                                    if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                    {
                                        GC.SendPacket(Packets.NPCText("Insert the name of the player in your guild you want to remove his deputy leader."));
                                        GC.SendPacket(Packets.NPCLink2("Here", 50));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not a guild leader."));
                                        GC.SendPacket(Packets.NPCLink("Silly me.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 50)
                                {
                                    string PlayerName = ReadString(Data);
                                    Features.MemberInfo M = GC.MyChar.MyGuild.deputysOfName(PlayerName);
                                    if (M != null && M.MembName == PlayerName)
                                    {
                                        M.Rank = Server.Features.GuildRank.Member;
                                        ((Hashtable)GC.MyChar.MyGuild.Members[(byte)90]).Remove(M.MembID);
                                        ((Hashtable)GC.MyChar.MyGuild.Members[(byte)50]).Add(M.MembID, M);
                                        Game.Character C = M.Info;
                                        if (C != null)
                                        {
                                            C.GuildRank = Server.Features.GuildRank.Member;
                                            Game.World.Spawn(C, false);
                                            C.MyClient.SendPacket(Packets.GuildInfo(GC.MyChar.MyGuild, GC.MyChar));
                                            Features.Guilds.SaveGuilds();
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("The player is not in your guild or is not a normal member. By the way, the max number deputy leaders there can be is 5."));
                                        GC.SendPacket(Packets.NPCLink("Oh, sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                else if (option == 5)
                                {
                                    if (GC.MyChar.MyGuild == Extra.GuildWars.LastWinner)
                                    {
                                        NPCText("im sorry but unable disband you guild, You guild win the GuildWar ,and you have the top halo, come back next 7 days", GC);
                                        NPCLink("ah,sorry man i see", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                    {
                                        NPCText("Are you sure you want to disband your guild?", GC);
                                        NPCLink("Yes, i want to disbad my guild.", 6, GC);
                                        NPCLink("I've changed my mind.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("You are not a guild leader, therefore you cannot disband a guild.", GC);
                                        NPCLink("I see.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 6)
                                {
                                    if (GC.MyChar.MyGuild != null && GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                    {
                                        Database.DeleteGuild(GC.MyChar);
                                        GC.MyChar.MyGuild.Disband();
                                    }
                                }
                                #region Explore Allais
                                else if (option == 29)
                                {
                                    string allis1 = "    ";
                                    string allis2 = "    ";
                                    string allis3 = "    ";
                                    string allis4 = "    ";
                                    string allis5 = "    ";
                                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                    {
                                        Features.Guild TGuild = Guilds.Value;
                                        if (GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                        {
                                            if (allis1 == "None")
                                                allis1 = TGuild.GuildName;
                                            else if (allis2 == "None")
                                                allis2 = TGuild.GuildName;
                                            else if (allis3 == "None")
                                                allis3 = TGuild.GuildName;
                                            else if (allis4 == "None")
                                                allis4 = TGuild.GuildName;
                                            else if (allis5 == "None")
                                                allis5 = TGuild.GuildName;
                                        }
                                    }
                                    NPCText("You Allis Guild Its: 1:" + allis1 + " 2: " + allis2 + " 3: " + allis3 + " 4: " + allis4 + " 5: " + allis5 + " ", GC);
                                    NPCLink("I see.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);

                                }
                                #endregion
                                #region Explore Enemyes
                                else if (option == 41)
                                {
                                    string allis1 = "    ";
                                    string allis2 = "    ";
                                    string allis3 = "    ";
                                    string allis4 = "    ";
                                    string allis5 = "    ";
                                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                    {
                                        Features.Guild TGuild = Guilds.Value;
                                        if (GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                        {
                                            if (allis1 == "None")
                                                allis1 = TGuild.GuildName;
                                            else if (allis2 == "None")
                                                allis2 = TGuild.GuildName;
                                            else if (allis3 == "None")
                                                allis3 = TGuild.GuildName;
                                            else if (allis4 == "None")
                                                allis4 = TGuild.GuildName;
                                            else if (allis5 == "None")
                                                allis5 = TGuild.GuildName;
                                        }
                                    }
                                    NPCText("You Enemyes Guild Its: 1:" + allis1 + " 2: " + allis2 + " 3: " + allis3 + " 4: " + allis4 + " 5: " + allis5 + " ", GC);
                                    NPCLink("I see.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);

                                }
                                #endregion
                                else if (option == 15)
                                {
                                    if (GC.MyChar.MyGuild != null)
                                    {
                                        if (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                        {
                                            NPCText("Say me what guild? Yu wana Un-allis", GC);
                                            NPCLink2("GuildName", 16, GC);
                                            NPCLink("My Allis name", 29, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("You are not a guild leader, therefore you cannot disband a guild.", GC);
                                            NPCLink("Ok, i see.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You dont have a guild.", GC);
                                        NPCLink("Oh, my bad....", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }
                                else if (option == 16)
                                {
                                    string GuildAllis = "";
                                    for (int i = 14; i < 14 + Data[13]; i++)
                                    {
                                        GuildAllis += Convert.ToChar(Data[i]);
                                    }
                                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                    {
                                        Features.Guild TGuild = Guilds.Value;
                                        if (GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                        {
                                            GC.MyChar.MyGuild.Allies.Remove(TGuild.GuildID);
                                            TGuild.Allies.Remove(GC.MyChar.MyGuild.GuildID);
                                            Database.DeleteAllisFromGuild(GC.MyChar, TGuild);
                                            Database.DeleteAllisFromOtherGuild(GC.MyChar, TGuild);
                                            NPCText("All done .", GC);
                                            NPCLink("Ok man, THX", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                }

                                #region AddAllis
                                else if (option == 13)
                                {
                                    if (GC.MyChar.MyGuild != null)
                                    {
                                        if (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                        {
                                            NPCText("You wana Allis whit guild?", GC);
                                            NPCLink2("GuildName", 14, GC);
                                            NPCLink("Sorry,dont wana", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("You are not a guild leader, therefore you cannot disband a guild.", GC);
                                            NPCLink("Ok, i see.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You dont have a guild.", GC);
                                        NPCLink("Oh, my bad....", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }
                                else if (option == 14)
                                {
                                    string GuildAllis = "";
                                    for (int i = 14; i < 14 + Data[13]; i++)
                                    {
                                        GuildAllis += Convert.ToChar(Data[i]);
                                    };
                                    if (GC.MyChar.MyGuild.GuildName == GuildAllis)
                                        return;
                                    if (GC.MyChar.MyGuild.Allies.Count < 5)
                                    {
                                        foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                        {
                                            Features.Guild TGuild = Guilds.Value;
                                            {
                                                if (TGuild.GuildName == GuildAllis)
                                                {
                                                    if (!GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                                    {
                                                        if (GC.MyChar.MyTeam != null)
                                                        {
                                                            Character Who = (Character)World.H_Chars[TGuild.Creator.MembID];
                                                            if (GC.MyChar.MyTeam.Members.Contains(Who) ||
                                                                GC.MyChar.MyTeam.Leader.EntityID == Who.EntityID)
                                                            {
                                                                if (Who.GuildRank == Server.Features.GuildRank.GuildLeader)
                                                                {
                                                                    if (!GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                                                    {

                                                                        GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 7));
                                                                        GC.MyChar.MyGuild.Allies.Add(TGuild.GuildID, TGuild);
                                                                        Who.MyGuild.Allies.Add(GC.MyChar.MyGuild.GuildID, GC.MyChar.MyGuild);
                                                                        Who.MyClient.SendPacket(Packets.SendGuild(GC.MyChar.MyGuild.GuildID, 7));
                                                                        Database.AddAllisToGuild(GC.MyChar, Who.MyGuild);
                                                                        Database.AddAllisToGuild(Who, GC.MyChar.MyGuild);
                                                                        NPCText("Done", GC);
                                                                        NPCLink("Ok ,thxxx you", 255, GC);
                                                                        NPCFace(N.Avatar, GC);
                                                                        NPCFinish(GC);
                                                                        Game.World.Spawns(GC.MyChar, true);
                                                                    }
                                                                    else
                                                                    {
                                                                        NPCText("Sorry,but you have this guild at enemy list", GC);
                                                                        NPCLink("Ok,I see new", 255, GC);
                                                                        NPCFace(N.Avatar, GC);
                                                                        NPCFinish(GC);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    NPCText("Sorry,but " + Who.Name + " dont is  GuildLider at :" + GuildAllis + " ", GC);
                                                                    NPCLink("Ok,I see new", 255, GC);
                                                                    NPCFace(N.Avatar, GC);
                                                                    NPCFinish(GC);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                NPCText("Sorry," + GuildAllis + " GuildLider dont is in you team ", GC);
                                                                NPCLink("Oh, my bad....", 255, GC);
                                                                NPCFace(N.Avatar, GC);
                                                                NPCFinish(GC);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            NPCText("You dont is in team", GC);
                                                            NPCLink("Oh, my bad....", 255, GC);
                                                            NPCFace(N.Avatar, GC);
                                                            NPCFinish(GC);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NPCText("Actuali its allis", GC);
                                                        NPCLink("Oh, my bad....", 255, GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You Allis Its full", GC);
                                        NPCLink("Ok,i see new", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region Remove Enemy
                                else if (option == 10)
                                {
                                    if (GC.MyChar.MyGuild != null)
                                    {
                                        if (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                        {
                                            NPCText("Give me the name of guild where you want to remove", GC);
                                            NPCLink2("GuildName", 11, GC);
                                            NPCLink("Let me think it over..", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("You are not a guild leader, therefore you cannot disband a guild.", GC);
                                            NPCLink("Ok, i see.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You dont have a guild.", GC);
                                        NPCLink("Oh, my bad....", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 11)
                                {
                                    string GuildEnemy = "";
                                    for (int i = 14; i < 14 + Data[13]; i++)
                                    {
                                        GuildEnemy += Convert.ToChar(Data[i]);
                                    };
                                    foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                    {
                                        Features.Guild TGuild = Guilds.Value;
                                        if (TGuild.GuildName == GuildEnemy)
                                        {
                                            if (GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                            {
                                                //GC.MyChar.MyGuild.Enemies1 = "";
                                                //GC.MyChar.MyGuild.Enemiesid1 = 0;
                                                //Database.AddEnemiesGuild(GC.MyChar);
                                                Database.DeleteEnemiesFromGuild(GC.MyChar, TGuild);
                                                GC.MyChar.MyGuild.Enemies.Remove(TGuild.GuildID);
                                                Features.Guilds.SaveGuilds();
                                                NPCText("Done", GC);
                                                NPCLink("Ok ,thxxx", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                Game.World.Spawns(GC.MyChar, true);
                                            }
                                            else
                                            {
                                                NPCText("You dont have this Guild at enemies", GC);
                                                NPCLink("Ok i see.", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            NPCText("You dont have this Guild at enemies or dont exist", GC);
                                            NPCLink("Ok i see.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }

                                }
                                #endregion
                                #region ADDEnemies
                                else if (option == 8)
                                {
                                    if (GC.MyChar.MyGuild != null)
                                    {
                                        if (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader)
                                        {
                                            NPCText("Give me the name of guild where you want to be enemy.", GC);
                                            NPCLink2("GuildName", 9, GC);
                                            NPCLink("Let me think it over..", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("You are not a guild leader, therefore you cannot disband a guild.", GC);
                                            NPCLink("Ok, i see.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You dont have a guild.", GC);
                                        NPCLink("Oh, my bad....", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 9)
                                {
                                    string GuildEnemy = "";
                                    for (int i = 14; i < 14 + Data[13]; i++)
                                    {
                                        GuildEnemy += Convert.ToChar(Data[i]);
                                    };
                                    if (GC.MyChar.MyGuild.GuildName == GuildEnemy)
                                        return;
                                    if (GC.MyChar.MyGuild.Enemies.Count < 5)// == "")
                                    {
                                        foreach (KeyValuePair<uint, Features.Guild> Guilds in Features.Guilds.AllTheGuilds)
                                        {
                                            Features.Guild TGuild = Guilds.Value;

                                            if (TGuild.GuildName != GC.MyChar.MyGuild.GuildName)
                                            {
                                                if (TGuild.GuildName == GuildEnemy)
                                                {
                                                    if (!GC.MyChar.MyGuild.Enemies.ContainsKey(TGuild.GuildID))
                                                    {
                                                        if (!GC.MyChar.MyGuild.Allies.ContainsKey(TGuild.GuildID))
                                                        {

                                                            GC.MyChar.Enemies.Add(TGuild.GuildID, 9);
                                                            GC.SendPacket(Packets.SendGuild(TGuild.GuildID, 9));
                                                            Features.Guilds.SaveGuilds();
                                                            NPCText("Done", GC);
                                                            NPCLink("Ok ,thxxx", 255, GC);
                                                            NPCFace(N.Avatar, GC);
                                                            NPCFinish(GC);
                                                            Game.World.Spawns(GC.MyChar, true);
                                                            Database.AddEnemiesToGuild(GC.MyChar, TGuild);
                                                        }
                                                        else
                                                        {
                                                            NPCText("Sorry but this guild is allis.", GC);
                                                            NPCLink("Ok i see.", 255, GC);
                                                            NPCFace(N.Avatar, GC);
                                                            NPCFinish(GC);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NPCText("Sorry but this guild is in you list enemy.", GC);
                                                        NPCLink("Ok i see.", 255, GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You are full enemy.", GC);
                                        NPCLink("Ok i see.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                break;
                            }
                        #endregion
                        #region GW HealGate
                        case 6700:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("This Option Is Not Avalible Now."));
                                    //GC.SendPacket(Packets.NPCLink("Heal the pole", 1));
                                    GC.SendPacket(Packets.NPCLink("Okay NVM", 255));
                                    NPCFace(0, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.MyGuild != null && GC.MyChar.MyGuild == Extra.GuildWars.LastWinner && (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Server.Features.GuildRank.DeputyLeader))
                                    {
                                        GC.SendPacket(Packets.NPCText("How much of your guild fund are you going to waste?"));
                                        GC.SendPacket(Packets.NPCLink2("Heal", 2));
                                        GC.SendPacket(Packets.NPCLink("I changed my mind.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not authorized to do that."));
                                        GC.SendPacket(Packets.NPCLink("Ahh okay.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.MyGuild != null && GC.MyChar.MyGuild == Extra.GuildWars.LastWinner && (GC.MyChar.GuildRank == Server.Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Server.Features.GuildRank.DeputyLeader))
                                    {
                                        uint Amount = 0;
                                        try
                                        {
                                            Amount = uint.Parse(ReadString(Data));
                                        }
                                        catch { }

                                        if (Amount > 0 && Amount <= GC.MyChar.MyGuild.Fund)
                                        {
                                            uint ToHeal = Extra.GuildWars.ThePole.MaxHP - Extra.GuildWars.ThePole.CurHP;
                                            if (Amount > ToHeal * 2) Amount = ToHeal * 2;
                                            GC.MyChar.MyGuild.Fund -= Amount;
                                            Extra.GuildWars.ThePole.CurHP += Amount / 2;
                                            Extra.GuildWars.ThePole.ReSpawn();
                                        }
                                    }
                                }
                                break;
                            }
                        case 6701:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("What do you want to do?"));
                                    GC.SendPacket(Packets.NPCLink("Open/Close the gate.", 1));
                                    GC.SendPacket(Packets.NPCLink("Heal TheLeftGate.", 2));
                                    GC.SendPacket(Packets.NPCLink("Nothing", 255));
                                    NPCFace(0, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.MyGuild != null && (GC.MyChar.GuildRank == Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Features.GuildRank.DeputyLeader) && (Extra.GuildWars.LastWinner == GC.MyChar.MyGuild))
                                    {
                                        Extra.GuildWars.TheLeftGate.Opened = !Extra.GuildWars.TheLeftGate.Opened;
                                        Extra.GuildWars.TheLeftGate.ReSpawn();
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not authorized to to that."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {

                                    if (GC.MyChar.MyGuild != null && (GC.MyChar.GuildRank == Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Features.GuildRank.DeputyLeader) && (Extra.GuildWars.LastWinner == GC.MyChar.MyGuild))
                                    {
                                        GC.SendPacket(Packets.NPCText("you are  going to waste 50,000 CPs from Your own Kach?"));
                                        GC.SendPacket(Packets.NPCLink("Heal", 3));
                                        GC.SendPacket(Packets.NPCLink("I changed my mind.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not authorized to do that."));
                                        GC.SendPacket(Packets.NPCLink("Ahh okay.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.CPs >= 50000)
                                    {
                                        GC.MyChar.CPs -= 50000;
                                        Extra.GuildWars.TheLeftGate.CurHP = Extra.GuildWars.TheLeftGate.MaxHP;
                                        Extra.GuildWars.TheLeftGate.ReSpawn();
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 20,000 CPs."));
                                        GC.SendPacket(Packets.NPCLink("Ahh okay.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        case 6702:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("What do you want to do?"));
                                    GC.SendPacket(Packets.NPCLink("Open/close the gate.", 1));
                                    GC.SendPacket(Packets.NPCLink("Hell TheRightGate.", 2));
                                    GC.SendPacket(Packets.NPCLink("Nothing", 255));
                                    NPCFace(0, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.MyGuild != null && (GC.MyChar.GuildRank == Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Features.GuildRank.DeputyLeader) && (Extra.GuildWars.LastWinner == GC.MyChar.MyGuild && !Extra.GuildWars.War))
                                    {
                                        Extra.GuildWars.TheRightGate.Opened = !Extra.GuildWars.TheRightGate.Opened;
                                        Extra.GuildWars.TheRightGate.ReSpawn();
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not authorized to to that."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.MyGuild != null && (GC.MyChar.GuildRank == Features.GuildRank.GuildLeader || GC.MyChar.GuildRank == Features.GuildRank.DeputyLeader) && (Extra.GuildWars.LastWinner == GC.MyChar.MyGuild && !Extra.GuildWars.War))
                                    {
                                        GC.SendPacket(Packets.NPCText("you are  going to waste 50,000 Cps from Your own Kach!"));
                                        GC.SendPacket(Packets.NPCLink("Heal", 3));
                                        GC.SendPacket(Packets.NPCLink("I changed my mind.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are not authorized to do that."));
                                        GC.SendPacket(Packets.NPCLink("Ahh okay.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.CPs >= 50000)
                                    {
                                        GC.MyChar.CPs -= 50000;
                                        Extra.GuildWars.TheRightGate.CurHP = Extra.GuildWars.TheRightGate.MaxHP;
                                        Extra.GuildWars.TheRightGate.ReSpawn();
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have 20,000 CPs."));
                                        GC.SendPacket(Packets.NPCLink("Ahh okay.", 255));
                                        NPCFace(0, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region GW GuildCondactors 1 : 4
                        #region GCs to Get In Gw
                        #region Gc1
                        case 8070:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to go back to guild arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1038, 348, 339);
                                break;
                            }
                        #endregion
                        #region Gc2
                        case 8071:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to go back to guild arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1038, 348, 339);
                                break;
                            }
                        #endregion
                        #region Gc3
                        case 8072:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to go back to guild arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1038, 348, 339);
                                break;
                            }
                        #endregion
                        #region Gc4
                        case 8073:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to go back to guild arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1038, 348, 339);
                                break;
                            }
                        #endregion
                        #region Gc5
                        case 8074:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want to go back to guild arena?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1038, 348, 339);
                                break;
                            }
                        #endregion
                        #endregion
                        #region Gcs to Get out Gw
                        #region Gc1
                        case 8060:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter City ?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1011, 193, 269);
                                break;
                            }
                        #endregion
                        #region Gc2
                        case 8061:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter City ?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1000, 491, 646);
                                break;
                            }
                        #endregion
                        #region Gc3
                        case 8062:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter City ?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1015, 729, 573);
                                break;
                            }
                        #endregion
                        #region Gc4
                        case 8063:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter City ?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1020, 577, 564);
                                break;
                            }
                        #endregion
                        #region Gc5
                        case 8064:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Do you want enter City ?"));
                                    GC.SendPacket(Packets.NPCLink("Yes.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1001, 352, 250);
                                break;
                            }
                        #endregion
                        #endregion
                        #endregion
                        #region FlameToist
                        case 8080:
                            {
                                if (option == 0)
                                {

                                    if (World.flamestonequest == true)
                                    {
                                        NPCText("You must have heard that the Olympic Games. I've been waiting for it for my entire life.", GC);
                                        NPCText(" There are 10 flames stones that i would like you to light up...You will get some rewards, of cource.", GC);
                                        NPCText(" It really makes sense to us, ah, at least to me.", GC);
                                        NPCLink("I would like to light up the flames", 10, GC);
                                        NPCLink("Rewards? Tell me more! Come on!", 1, GC);
                                        NPCLink("Not Intrested.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("you must have heard that the Olympic Games. I've been waiting for it for my entire life.", GC);
                                        NPCText(" There are 10 flames stones that i would like you to light up...You will get some rewards, of cource.", GC);
                                        NPCText(" It really makes sense to us, ah, at least to me.", GC);
                                        NPCLink("Rewards? Tell me more! Come on!", 1, GC);
                                        NPCLink("I have the Rune. Give me the Reward", 4, GC);
                                        NPCLink("Not Intrested.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 1)
                                {
                                    NPCText("Ah, you seem very interested! Good. You are eligible to ligth up the flame! But, it's not the rigth time.", GC);
                                    NPCText(" You see, the ceremony will be held till the last two hours of the Guild War. Come and sign up then.", GC);
                                    NPCLink("would you please give me more information?", 2, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2)
                                {
                                    NPCText("Yeah. I've prepare 10 Flames Stones in the Wind Plain and Guild Arena. When the times comes,", GC);
                                    NPCText(" i will give you a jade Rune. You just take it to ligth up all the Flame Stones in sequence", GC);
                                    NPCLink("Sure.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 4)
                                {
                                    if (GC.MyChar.Level < 137)
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(729970, 1))
                                        {
                                            GC.MyChar.RemoveItemIDAmount(729970, 1);
                                            for (int x = 0; x < 12; x++)
                                                GC.MyChar.IncreaseExp((GC.MyChar.EXPBall), false);
                                        }
                                        else
                                        {
                                            NPCText("Sorry but you not have the DragonRune", GC);
                                            NPCLink("Ups my mistake", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You are level 137 sorry you cant get the reward", GC);
                                        NPCLink("OMG you rigth", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 10)
                                {
                                    if (GC.MyChar.flames == 0)
                                    {
                                        if (GC.MyChar.Inventory.Count <= 39)
                                        {
                                            GC.MyChar.flames = 1;
                                            GC.MyChar.CreateItemIDAmount(729960, 1);
                                            NPCText("Good luck, light up the flames", GC);
                                            NPCLink("Thanks", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                        }

                                    }
                                    else
                                    {
                                        NPCText("You already have the flamestone go light up the flames", GC);
                                        NPCLink("Sure", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Flames
                        case 8081: // Flame1
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 1)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729960, 1))
                                                {
                                                    NPCText("You have the GoldRune, go to the next Flame the coord is (317,270)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729960, 1);
                                                    GC.MyChar.CreateItemIDAmount(729961, 1);
                                                    GC.MyChar.flames = 2;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the JadeRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (317,270)");
                                    }
                                }
                                else
                                {
                                    if (GC.MyChar.flames == 11)
                                    {
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                    }
                                    else
                                    {
                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                    }
                                }
                                break;
                            }
                        case 8082: // Flame2
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 2)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729961, 1))
                                                {
                                                    NPCText("You have the GoldRune, go to the next Flame the coord is (236,291)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729961, 1);
                                                    GC.MyChar.CreateItemIDAmount(729962, 1);
                                                    GC.MyChar.flames = 3;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the GoldRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (236,291)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8083: // Flame3
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 3)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729962, 1))
                                                {
                                                    NPCText("You have the GoldRune, go to the next Flame the coord is (194,168)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729962, 1);
                                                    GC.MyChar.CreateItemIDAmount(729963, 1);
                                                    GC.MyChar.flames = 4;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the GoldRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (194,168)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8084: // Flame4
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 4)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729963, 1))
                                                {
                                                    NPCText("You have the SpiritRune, go to the next Flame the coord is (115,53)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729963, 1);
                                                    GC.MyChar.CreateItemIDAmount(729964, 1);
                                                    GC.MyChar.flames = 5;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the GoldRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (115,53)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8085: // Flame5
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 5)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729964, 1))
                                                {
                                                    NPCText("You have the SpiritRune, go to the next Flame the coord is (316,378)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729964, 1);
                                                    GC.MyChar.CreateItemIDAmount(729965, 1);
                                                    GC.MyChar.flames = 6;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the SpiritRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (316,378)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8086: // Flame6
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 6)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729965, 1))
                                                {
                                                    NPCText("You have the SpiritRune, go to the next Flame the coord is (136,182)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729965, 1);
                                                    GC.MyChar.CreateItemIDAmount(729966, 1);
                                                    GC.MyChar.flames = 7;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the SpiritRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (136,182)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8087: // Flame7
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 7)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729966, 1))
                                                {
                                                    NPCText("You have the HeavenRune, go to the next Flame the coord is (38,94)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729966, 1);
                                                    GC.MyChar.CreateItemIDAmount(729967, 1);
                                                    GC.MyChar.flames = 8;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the SpiritRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (38,94)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8088: // Flame8
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 8)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729967, 1))
                                                {
                                                    NPCText("You have the HeavenRune, go to the next Flame the coord is (350,321)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729967, 1);
                                                    GC.MyChar.CreateItemIDAmount(729968, 1);
                                                    GC.MyChar.flames = 9;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the HeavenRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (350,321)");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8089: // Flame9
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 9)
                                    {
                                        if (option == 0)
                                        {
                                            if (GC.MyChar.Inventory.Count <= 39)
                                            {
                                                if (GC.MyChar.FindInventoryItemIDAmount(729968, 1))
                                                {
                                                    NPCText("You have the HeavenRune, go to the next Flame the coord is (62,59)", GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.MyChar.RemoveItemUIDAmount(729968, 1);
                                                    GC.MyChar.CreateItemIDAmount(729969, 1);
                                                    GC.MyChar.flames = 10;
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You not have the HeavenRune");
                                                }
                                            }
                                            else
                                            {
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finish the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Go at next flame (62,59)");
                                        }

                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        case 8090: // Flame10
                            {
                                if (World.flamestonequest == true)
                                {
                                    if (GC.MyChar.flames == 10)
                                    {
                                        if (World.flame10 == true)
                                        {
                                            if (option == 0)
                                            {
                                                if (GC.MyChar.Inventory.Count <= 39)
                                                {
                                                    if (GC.MyChar.FindInventoryItemIDAmount(729969, 1))
                                                    {
                                                        NPCText("You have the DragonRune, go now to flametoist for your reward", GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                        GC.MyChar.RemoveItemUIDAmount(729969, 1);
                                                        GC.MyChar.CreateItemIDAmount(729970, 1);
                                                        GC.MyChar.flames = 11;//11 = finish
                                                    }
                                                    else
                                                    {
                                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "You don't have the HeavenRune");
                                                    }
                                                }
                                                else
                                                {
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "You inventory full");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry, but this is not the time for flame 10");
                                        }
                                    }
                                    else
                                    {
                                        if (GC.MyChar.flames == 11)
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "Sorry,but you finished the flame stone quest");
                                        }
                                        else
                                        {
                                            GC.LocalMessage(2005, System.Drawing.Color.Red, "You have the DragonRune, go now to flametoist for your reward");
                                        }
                                    }
                                }
                                else
                                {
                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Its not the right time to light up the flames");
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region FirstReborn
                        case 7010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello, I am The One Who Will Get You FirstReborn."));
                                    GC.SendPacket(Packets.NPCLink("Ok, I Want Get FirstReborn.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Reborns == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Alright, you must be level 120 or higher and have a celestial stone with you."));
                                        GC.SendPacket(Packets.NPCText("Oh and WaterWizard are the exception. They can reborn on level 110"));
                                        GC.SendPacket(Packets.NPCLink("Reborn Me", 2));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.Reborns == 1)
                                    {
                                        GC.SendPacket(Packets.NPCText("You Are FirstReborn, You can`t do it again."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Are SeconedReborn. You can`t do it again."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Level >= 120 || (GC.MyChar.Job == 135 && GC.MyChar.Level >= 110))
                                    {
                                        if (GC.MyChar.Job % 10 == 5)
                                        {
                                            if (GC.MyChar.FindInventoryItemIDAmount(721259, 1))
                                            {
                                                GC.SendPacket(Packets.NPCText("Choose what do you want to receive as Gift"));
                                                GC.SendPacket(Packets.NPCLink("Super Gem.", 30));
                                                GC.SendPacket(Packets.NPCLink("Bless Equipment.", 31));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You Don`t Have The CelestialStone. Come back when you have one."));
                                                GC.SendPacket(Packets.NPCLink("I see.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You've got to be a master to reborn."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are`t in the required level."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 30 || option == 31)
                                {
                                    if (option == 30)
                                    {
                                        GC.MyChar.addBless = 0;
                                        GC.MyChar.superGem = 1;
                                        GC.SendPacket(Packets.NPCText("Alright Choose The SuperGem List Below."));
                                        GC.SendPacket(Packets.NPCLink("Dragon", 43));
                                        GC.SendPacket(Packets.NPCLink("Phoenix", 33));
                                        GC.SendPacket(Packets.NPCLink("Fury", 53));
                                        GC.SendPacket(Packets.NPCLink("Moon", 93));
                                        GC.SendPacket(Packets.NPCLink("Rainbow", 63));
                                        GC.SendPacket(Packets.NPCLink("Glory", 153));
                                        GC.SendPacket(Packets.NPCLink("Thunder", 133));
                                        //GC.SendPacket(Packets.NPCLink("Next", 64));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 31)
                                    {
                                        GC.MyChar.addBless = 1;
                                        GC.MyChar.superGem = 0;
                                        GC.SendPacket(Packets.NPCText("Alright Choose Your Reborn Job From List Below."));
                                        //GC.SendPacket(Packets.NPCLink("Trojan", 3));
                                        GC.SendPacket(Packets.NPCLink("Warrior", 4));
                                        GC.SendPacket(Packets.NPCLink("Archer", 5));
                                        GC.SendPacket(Packets.NPCLink("Ninja", 6));
                                        GC.SendPacket(Packets.NPCLink("WaterWizard", 7));
                                        GC.SendPacket(Packets.NPCLink("FireWizard", 8));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                //else if (option == 64)
                                //{
                                //    GC.SendPacket(Packets.NPCLink("Kylin", 73));
                                //    GC.SendPacket(Packets.NPCLink("Violet", 83));
                                //    GC.SendPacket(Packets.NPCLink("Glory", 153));
                                //    GC.SendPacket(Packets.NPCLink("Thunder", 133));
                                //    GC.SendPacket(Packets.NPCLink("Back", 30));
                                //    GC.SendPacket(Packets.NPCSetFace(N.Avatar));
                                //    NPCFinish(GC);
                                //}
                                else if (option >= 33 && option <= 153)
                                {
                                    byte Gem = (byte)(option - 30);
                                    GC.MyChar.superGem = Gem;
                                    GC.SendPacket(Packets.NPCText("Alright Choose Your Reborn Job From List Below."));
                                    //GC.SendPacket(Packets.NPCLink("Trojan", 3));
                                    GC.SendPacket(Packets.NPCLink("Warrior", 4));
                                    GC.SendPacket(Packets.NPCLink("Archer", 5));
                                    GC.SendPacket(Packets.NPCLink("Ninja", 6));
                                    GC.SendPacket(Packets.NPCLink("WaterWizard", 7));
                                    GC.SendPacket(Packets.NPCLink("FireWizard", 8));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 3 && option <= 8)
                                {
                                    if (GC.MyChar.Level >= 120 || (GC.MyChar.Job == 135 && GC.MyChar.Level >= 110))
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(721259, 1))
                                        {
                                            GC.MyChar.RemoveItemIDAmount(721259, 1);
                                            if (GC.MyChar.superGem != 0)
                                            {
                                                GC.MyChar.CreateItemIDAmount((uint)(700000 + GC.MyChar.superGem), 1);
                                                goto Over;
                                            }
                                            else if (GC.MyChar.addBless != 0)
                                            {
                                                #region HG
                                                if (GC.MyChar.Equips.HeadGear.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.HeadGear.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.HeadGear.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Necklace
                                                if (GC.MyChar.Equips.Necklace.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Necklace.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Necklace.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Ring
                                                if (GC.MyChar.Equips.Ring.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Ring.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Ring.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region LeftHand
                                                if (GC.MyChar.Equips.LeftHand.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.LeftHand.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.LeftHand.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region RightHand
                                                if (GC.MyChar.Equips.RightHand.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.RightHand.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.RightHand.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Boots
                                                if (GC.MyChar.Equips.Boots.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Boots.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Boots.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Gourd
                                                if (GC.MyChar.Equips.Gourd.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Gourd.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Gourd.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Garment
                                                if (GC.MyChar.Equips.Garment.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Garment.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Garment.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Fan
                                                if (GC.MyChar.Equips.Fan.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Fan.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Fan.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Tower
                                                if (GC.MyChar.Equips.Tower.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Tower.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Tower.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                            }
                                        Over:
                                            //if (option == 3) GC.MyChar.RebornCharacter(11);
                                            if (option == 4) GC.MyChar.RebornCharacter(21);
                                            if (option == 5) GC.MyChar.RebornCharacter(41);
                                            if (option == 6) GC.MyChar.RebornCharacter(51);
                                            if (option == 7) GC.MyChar.RebornCharacter(132);
                                            if (option == 8) GC.MyChar.RebornCharacter(142);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Don`t Have The CelestialStone. Come back when you have one."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are`t in the required level."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Can`t Help The Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region SecondReborn
                        case 7011:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hellow I am The One Who Will Get You SecondReborn."));
                                    GC.SendPacket(Packets.NPCLink("Ok, I Want Get SecondReborn.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Reborns == 1)
                                    {
                                        GC.SendPacket(Packets.NPCText("Alright, you must be level 120 or higher and have a celestial stone with you."));
                                        GC.SendPacket(Packets.NPCText("Oh and WaterWizard are the exception. They can reborn on level 110"));
                                        GC.SendPacket(Packets.NPCLink("Reborn Me", 2));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else if (GC.MyChar.Reborns == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("You Are`t FirstReborn, You must get FirstReborn first."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Are SeconedReborn. You can`t do it again."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Level >= 120 || (GC.MyChar.Job == 135 && GC.MyChar.Level >= 110))
                                    {
                                        if (GC.MyChar.Job % 10 == 5)
                                        {
                                            if (GC.MyChar.FindInventoryItemIDAmount(723701, 1))
                                            {
                                                GC.SendPacket(Packets.NPCText("Choose what do you want to receive as Gift"));
                                                GC.SendPacket(Packets.NPCLink("Super Gem.", 30));
                                                GC.SendPacket(Packets.NPCLink("Bless Equipment.", 31));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You Don`t Have The ExcemptionToken. Come back when you have one."));
                                                GC.SendPacket(Packets.NPCLink("I see.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You've got to be a master to reborn."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are`t in the required level."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 30 || option == 31)
                                {
                                    if (option == 30)
                                    {
                                        GC.MyChar.addBless = 0;
                                        GC.MyChar.superGem = 1;
                                        GC.SendPacket(Packets.NPCText("Alright Choose The SuperGem List Below."));
                                        GC.SendPacket(Packets.NPCLink("Dragon", 43));
                                        GC.SendPacket(Packets.NPCLink("Phoenix", 33));
                                        GC.SendPacket(Packets.NPCLink("Fury", 53));
                                        GC.SendPacket(Packets.NPCLink("Moon", 93));
                                        GC.SendPacket(Packets.NPCLink("Rainbow", 63));
                                        GC.SendPacket(Packets.NPCLink("Glory", 153));
                                        GC.SendPacket(Packets.NPCLink("Thunder", 133));
                                        //GC.SendPacket(Packets.NPCLink("Next", 64));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 31)
                                    {
                                        GC.MyChar.addBless = 1;
                                        GC.MyChar.superGem = 0;
                                        GC.SendPacket(Packets.NPCText("Alright Choose Your Reborn Job From List Below."));
                                        //GC.SendPacket(Packets.NPCLink("Trojan", 3));
                                        GC.SendPacket(Packets.NPCLink("Warrior", 4));
                                        GC.SendPacket(Packets.NPCLink("Archer", 5));
                                        GC.SendPacket(Packets.NPCLink("Ninja", 6));
                                        GC.SendPacket(Packets.NPCLink("WaterWizard", 7));
                                        GC.SendPacket(Packets.NPCLink("FireWizard", 8));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                //else if (option == 64)
                                //{
                                //    GC.SendPacket(Packets.NPCLink("Kylin", 73));
                                //    GC.SendPacket(Packets.NPCLink("Violet", 83));
                                //    GC.SendPacket(Packets.NPCLink("Glory", 153));
                                //    GC.SendPacket(Packets.NPCLink("Thunder", 133));
                                //    GC.SendPacket(Packets.NPCLink("Back", 30));
                                //    GC.SendPacket(Packets.NPCSetFace(N.Avatar));
                                //    NPCFinish(GC);
                                //}
                                else if (option >= 33 && option <= 153)
                                {
                                    byte Gem = (byte)(option - 30);
                                    GC.MyChar.superGem = Gem;
                                    GC.SendPacket(Packets.NPCText("Alright Choose Your Reborn Job From List Below."));
                                    //GC.SendPacket(Packets.NPCLink("Trojan", 3));
                                    GC.SendPacket(Packets.NPCLink("Warrior", 4));
                                    GC.SendPacket(Packets.NPCLink("Archer", 5));
                                    GC.SendPacket(Packets.NPCLink("Ninja", 6));
                                    GC.SendPacket(Packets.NPCLink("WaterWizard", 7));
                                    GC.SendPacket(Packets.NPCLink("FireWizard", 8));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 3 && option <= 8)
                                {
                                    if (GC.MyChar.Level >= 120 || (GC.MyChar.Job == 135 && GC.MyChar.Level >= 110))
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(723701, 1))
                                        {
                                            GC.MyChar.RemoveItemIDAmount(723701, 1);
                                            if (GC.MyChar.superGem != 0)
                                            {
                                                GC.MyChar.CreateItemIDAmount((uint)(700000 + GC.MyChar.superGem), 1);
                                                goto Over;
                                            }
                                            else if (GC.MyChar.addBless != 0)
                                            {
                                                #region HG
                                                if (GC.MyChar.Equips.HeadGear.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.HeadGear.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.HeadGear.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Necklace
                                                if (GC.MyChar.Equips.Necklace.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Necklace.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Necklace.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Ring
                                                if (GC.MyChar.Equips.Ring.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Ring.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Ring.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region LeftHand
                                                if (GC.MyChar.Equips.LeftHand.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.LeftHand.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.LeftHand.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region RightHand
                                                if (GC.MyChar.Equips.RightHand.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.RightHand.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.RightHand.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Boots
                                                if (GC.MyChar.Equips.Boots.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Boots.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Boots.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Gourd
                                                if (GC.MyChar.Equips.Gourd.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Gourd.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Gourd.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Garment
                                                if (GC.MyChar.Equips.Garment.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Garment.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Garment.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Fan
                                                if (GC.MyChar.Equips.Fan.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Fan.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Fan.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                                #region Tower
                                                if (GC.MyChar.Equips.Tower.ID != 0)
                                                {
                                                    if (GC.MyChar.Equips.Tower.Bless == 0)
                                                    {
                                                        GC.MyChar.Equips.Tower.Bless = 1;
                                                        goto Over;
                                                    }
                                                }
                                                #endregion
                                            }
                                        Over:
                                            //if (option == 3) { GC.MyChar.RebornCharacter2(11); GC.MyChar.CreateItemID(723776); }
                                            if (option == 4) { GC.MyChar.RebornCharacter2(21); }
                                            if (option == 5) { GC.MyChar.RebornCharacter2(41); }
                                            if (option == 6) { GC.MyChar.RebornCharacter2(51); }
                                            if (option == 7) { GC.MyChar.RebornCharacter2(132); }
                                            if (option == 8) { GC.MyChar.RebornCharacter2(142); }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Don`t Have The ExcemptionToken. Come back when you have one."));
                                            GC.SendPacket(Packets.NPCLink("I see.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You are`t in the required level."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant Help The Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region Unknownman
                        case 7000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Welcome, i can increase your level from 1 to 130 By [10000 CPs for NoneReborn | 30000 CPs for FirstReborn | 50000 CPs for SecondReborn]. "));
                                    GC.SendPacket(Packets.NPCText("And i can increase you level from 130 to 140 By [2kk CPs] for each level."));
                                    GC.SendPacket(Packets.NPCLink("Yeah Up From Lvl [1 : 130].", 1));
                                    GC.SendPacket(Packets.NPCLink("Yeah Up From Lvl [130 : 140].", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.Level < 130)
                                    {
                                        uint prise = 0;
                                        if (GC.MyChar.Reborns == 0)
                                            prise = 10000;
                                        else if (GC.MyChar.Reborns == 1)
                                            prise = 30000;
                                        else if (GC.MyChar.Reborns == 2)
                                            prise = 50000;
                                        GC.SendPacket(Packets.NPCText("Your Level Is [" + GC.MyChar.Level + "], You Need To Pay " + prise + " CPs, To Reach Level [130]."));
                                        GC.SendPacket(Packets.NPCLink("Yeah I`ill Pay, Up To [130].", 10));
                                        GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Level Is " + GC.MyChar.Level + " I Can`t Help You More."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 10)
                                {
                                    if (GC.MyChar.Level < 130)
                                    {
                                        if (GC.MyChar.Reborns == 0)
                                        {
                                            if (GC.MyChar.CPs >= 10000)
                                            {
                                                GC.MyChar.CPs -= 10000;
                                                if (GC.MyChar.Level <= 120)
                                                    GC.MyChar.StatusPoints += 30;
                                                if (GC.MyChar.Level == 121)
                                                    GC.MyChar.StatusPoints += 27;
                                                if (GC.MyChar.Level == 122)
                                                    GC.MyChar.StatusPoints += 24;
                                                if (GC.MyChar.Level == 123)
                                                    GC.MyChar.StatusPoints += 21;
                                                if (GC.MyChar.Level == 124)
                                                    GC.MyChar.StatusPoints += 18;
                                                if (GC.MyChar.Level == 125)
                                                    GC.MyChar.StatusPoints += 15;
                                                if (GC.MyChar.Level == 126)
                                                    GC.MyChar.StatusPoints += 12;
                                                if (GC.MyChar.Level == 127)
                                                    GC.MyChar.StatusPoints += 9;
                                                if (GC.MyChar.Level == 128)
                                                    GC.MyChar.StatusPoints += 6;
                                                if (GC.MyChar.Level == 129)
                                                    GC.MyChar.StatusPoints += 3;
                                                GC.MyChar.Level = 130;
                                                GC.MyChar.CurHP = GC.MyChar.MaxHP;

                                                GC.SendPacket(Packets.NPCText("Your Level Now Is 130."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You Don`t Have 10000 CPs."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.Reborns == 1)
                                        {
                                            if (GC.MyChar.CPs >= 30000)
                                            {
                                                GC.MyChar.CPs -= 30000;
                                                GC.MyChar.Str = 180;
                                                GC.MyChar.Agi = 280;
                                                GC.MyChar.Spi = 0;
                                                GC.MyChar.Vit = 40;
                                                if (GC.MyChar.PreviousJob1 == 135)
                                                {
                                                    GC.MyChar.StatusPoints = 30;
                                                }
                                                else
                                                {
                                                    GC.MyChar.StatusPoints = 0;
                                                }
                                                GC.MyChar.Level = 130;
                                                GC.MyChar.CurHP = GC.MyChar.MaxHP;

                                                GC.SendPacket(Packets.NPCText("Your Level Now Is 130."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You Don`t Have 30000 CPs."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (GC.MyChar.Reborns == 2)
                                        {
                                            if (GC.MyChar.CPs >= 50000)
                                            {
                                                GC.MyChar.CPs -= 50000;
                                                GC.MyChar.Str = 180;
                                                GC.MyChar.Agi = 280;
                                                GC.MyChar.Spi = 0;
                                                GC.MyChar.Vit = 140;
                                                if (GC.MyChar.PreviousJob1 == 135 || GC.MyChar.PreviousJob2 == 135)
                                                {
                                                    GC.MyChar.StatusPoints = 30;
                                                }
                                                else if (GC.MyChar.PreviousJob1 == 135 && GC.MyChar.PreviousJob2 == 135)
                                                {
                                                    GC.MyChar.StatusPoints = 60;
                                                }
                                                else
                                                {
                                                    GC.MyChar.StatusPoints = 0;
                                                }
                                                GC.MyChar.Level = 130;
                                                GC.MyChar.CurHP = GC.MyChar.MaxHP;

                                                GC.SendPacket(Packets.NPCText("Your Level Now Is 130."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You Don`t Have 50000 CPs."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Level Is " + GC.MyChar.Level + " I Can`t Help You More."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Level >= 130 && GC.MyChar.Level < 140)
                                    {
                                        byte priselevel = (byte)(GC.MyChar.Level - 129);
                                        uint prise = (uint)(priselevel * 2000000);
                                        GC.SendPacket(Packets.NPCText("Your Level Is [" + GC.MyChar.Level + "], You Need To Pay " + prise + " CPs, To Reach Level [" + (GC.MyChar.Level + 1) + "]."));
                                        GC.SendPacket(Packets.NPCLink("Yeah I`ill Pay, Up To [" + (GC.MyChar.Level + 1) + "].", 20));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Level Is " + GC.MyChar.Level + " I Can`t Help You."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 20)
                                {
                                    byte priselevel = (byte)(GC.MyChar.Level - 129);
                                    uint prise = (uint)(priselevel * 2000000);

                                    if (GC.MyChar.Level >= 130 && GC.MyChar.Level < 140)
                                    {
                                        if (GC.MyChar.CPs >= prise)
                                        {
                                            GC.MyChar.CPs -= prise;
                                            GC.MyChar.StatusPoints += 6;
                                            GC.MyChar.Level = (byte)(GC.MyChar.Level + 1);
                                            GC.MyChar.CurHP = GC.MyChar.MaxHP;

                                            GC.SendPacket(Packets.NPCText("Your Level Now Is " + GC.MyChar.Level + "."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Don`t Have " + prise + " CPs."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Your Level Is " + GC.MyChar.Level + " I Can`t Help You."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant HelpThe Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion

                        #region Dragon Plus +1 : +12
                        case 12000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 368, 0, 0, 126));
                                }
                                break;
                            }
                        #endregion
                        #region Dragon Plus +12 : +15
                        case 98830:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("I Can Set Your Dragon Plus Here.So Do You Want Plus It?."));
                                    GC.SendPacket(Packets.NPCLink("Yeah I Want To Plus It.", 12));
                                    GC.SendPacket(Packets.NPCLink("Information about the plus coasts.", 13));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Agreed = false;
                                }
                                else if (option == 12)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option));
                                    if (I.Plus >= 7 && I.Plus < 12)
                                    {
                                        if (I.Plus != 12)
                                        {
                                            uint DBSNeeded = 0;
                                            if (I.Plus == 7)
                                                DBSNeeded = 2000000;
                                            else if (I.Plus == 8)
                                                DBSNeeded = 3000000;
                                            else if (I.Plus == 9)
                                                DBSNeeded = 6000000;
                                            else if (I.Plus == 10)
                                                DBSNeeded = 12000000;
                                            else if (I.Plus == 11)
                                                DBSNeeded = 14000000;

                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBSNeeded + " CPs to upgrade. Do you want it?"));
                                                GC.SendPacket(Packets.NPCText("Your item current Plus is " + I.Plus + "."));
                                                if (I.Plus != 0)
                                                    GC.SendPacket(Packets.NPCText("It will be " + (I.Plus + 1) + "."));
                                                GC.SendPacket(Packets.NPCLink("Yes.", option));
                                                GC.SendPacket(Packets.NPCLink("Nevermind.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.CPs >= DBSNeeded)
                                                {
                                                    GC.MyChar.EquipStats((byte)(option), false);
                                                    GC.MyChar.CPs -= DBSNeeded;
                                                    GC.LocalMessage(2005, System.Drawing.Color.Azure, "Congratulations! " + GC.MyChar.Name + " has upgraded the bonus level of " + I.ItemDBInfo.Name + " to +" + (I.Plus + 1) + "!");
                                                    GC.LocalMessage(2005, System.Drawing.Color.Azure, "Congratulations! " + GC.MyChar.Name + " has upgraded the bonus level of " + I.ItemDBInfo.Name + " to +" + (I.Plus + 1) + "!");
                                                    if (I.Plus == 0)
                                                        I.Plus = 1;
                                                    else
                                                        I.Plus += 1;
                                                    GC.MyChar.Equips.Replace((byte)(option), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option), true);

                                                    GC.SendPacket(Packets.NPCText("Here You Are. It's Done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You Don't Have Enough CPs"));
                                                    GC.SendPacket(Packets.NPCLink("I See.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Cannot Upgrade Your Dragon Plus Which Is Already At Maximum."));
                                            GC.SendPacket(Packets.NPCLink("I See", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry Your Dragon Is AllReady +12"));
                                        GC.SendPacket(Packets.NPCLink("Ok Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 13)
                                {
                                    GC.SendPacket(Packets.NPCText("+8 coast 2kk CPs __ +9 coast 3kk CPs__ +10 coast 6kk CPs __ +11 coast 12kk CPs__ +12 coast 14kk CPs"));
                                    GC.SendPacket(Packets.NPCLink("go back", 0));
                                    GC.SendPacket(Packets.NPCLink("no excuseme", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region Dragon Trainer
                        case 12010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi Guys Iam Here To Learn You Dragons Skills."));
                                    GC.SendPacket(Packets.NPCLink("Learn Riding Dragon (1KK)", 4));
                                    GC.SendPacket(Packets.NPCLink("Learn Spook (5KK)", 5));
                                    GC.SendPacket(Packets.NPCLink("Learn War Cry (10KK)", 6));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 4)
                                {
                                    if (GC.MyChar.Silvers >= 1000000)
                                    {
                                        GC.MyChar.Silvers -= 1000000;
                                        GC.MyChar.RemoveSkill(new Game.Skill() { ID = 7001, Lvl = 0, Exp = 0 });
                                        GC.SendPacket(Packets.NPCText("You have learned Riding Dragon Skill"));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 5)
                                {
                                    if (GC.MyChar.Silvers >= 5000000)
                                    {
                                        GC.MyChar.Silvers -= 5000000;
                                        GC.MyChar.RemoveSkill(new Game.Skill() { ID = 7002, Lvl = 0, Exp = 0 });
                                        GC.SendPacket(Packets.NPCText("You have learned Spook Skill"));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 6)
                                {
                                    if (GC.MyChar.Silvers >= 10000000)
                                    {
                                        GC.MyChar.Silvers -= 10000000;
                                        GC.MyChar.RemoveSkill(new Game.Skill() { ID = 7003, Lvl = 0, Exp = 0 });
                                        GC.SendPacket(Packets.NPCText("You have learned Warcry Skill"));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You don't have enough Cps."));
                                        GC.SendPacket(Packets.NPCLink("I see.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region QualityUpgrader
                        case 11000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi. I am Who Will Upgrade Your Item Quality."));
                                    GC.SendPacket(Packets.NPCLink("Upgrade Quality", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Agreed = false;
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose The Equipment You Want To Upgrade It`s Quality."));
                                    GC.SendPacket(Packets.NPCLink("Headgear", 101));
                                    GC.SendPacket(Packets.NPCLink("Necklace", 102));
                                    GC.SendPacket(Packets.NPCLink("Armor", 103));
                                    GC.SendPacket(Packets.NPCLink("Weapon", 104));
                                    GC.SendPacket(Packets.NPCLink("Shield", 105));
                                    GC.SendPacket(Packets.NPCLink("Ring", 106));
                                    GC.SendPacket(Packets.NPCLink("Boots", 108));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 101 && option <= 108)
                                {
                                    byte Pos = (byte)(option - 100);
                                    Item Eq = GC.MyChar.Equips.Get(Pos);
                                    if (Eq.ID != 0)
                                    {
                                        byte ItemLevel = Eq.ItemDBInfo.Level;
                                        ItemIDManipulation IMan = new ItemIDManipulation(Eq.ID);
                                        if (IMan.Quality != Item.ItemQuality.Super && IMan.Quality != Item.ItemQuality.NoUpgrade)
                                        {
                                            byte DBReq = 0;
                                            Item.ItemQuality Q = IMan.Quality;
                                            if ((byte)Q < 5) Q = Item.ItemQuality.Normal;
                                            if (Q == Item.ItemQuality.Normal) DBReq = 1;
                                            else if (Q == Item.ItemQuality.Refined) DBReq = 3;
                                            else if (Q == Item.ItemQuality.Unique) DBReq = 5;
                                            else if (Q == Item.ItemQuality.Elite) DBReq = 7;

                                            DBReq = (byte)(DBReq + (ItemLevel / 10));
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBReq))
                                                {
                                                    GC.MyChar.EquipStats(Pos, false);
                                                    GC.MyChar.RemoveItemIDAmount(1088000, DBReq);
                                                    IMan.QualityChange((Item.ItemQuality)(Q + 1));
                                                    Eq.ID = IMan.ToID();
                                                    GC.MyChar.Equips.Replace(Pos, Eq, GC.MyChar);
                                                    GC.MyChar.EquipStats(Pos, true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough Dragonballs."));
                                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You cannot upgrade an item's quality which is already at maximum."));
                                            GC.SendPacket(Packets.NPCLink("I see", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("There is no item to upgrade it`s quality."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant Help The Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region LevelUpgrader
                        case 11010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi. I am Who Will Upgrade Your Item Level."));
                                    GC.SendPacket(Packets.NPCLink("Upgrade Level.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Agreed = false;
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose The Equipment You Want To Upgrade It`s Level."));
                                    GC.SendPacket(Packets.NPCLink("Headgear", 101));
                                    GC.SendPacket(Packets.NPCLink("Necklace", 102));
                                    GC.SendPacket(Packets.NPCLink("Armor", 103));
                                    GC.SendPacket(Packets.NPCLink("Weapon", 104));
                                    GC.SendPacket(Packets.NPCLink("Shield", 105));
                                    GC.SendPacket(Packets.NPCLink("Ring", 106));
                                    GC.SendPacket(Packets.NPCLink("Boots", 108));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 101 && option <= 108)
                                {
                                    byte Pos = (byte)(option - 100);
                                    Game.Item Eq = GC.MyChar.Equips.Get(Pos);
                                    if (Eq.ID != 0)
                                    {
                                        byte PrevLevel = Eq.ItemDBInfo.Level;
                                        uint newID = 0;

                                        #region HeadGears
                                        #region Earring
                                        //Start For Level 10
                                        if (Eq.ID == 117059)
                                            newID = 117069; // Level 40
                                        else if (Eq.ID == 117069)
                                            newID = 117079; // Level 70 
                                        else if (Eq.ID == 117079)
                                            newID = 117089; // Level 100
                                        else if (Eq.ID == 117089)
                                            newID = 117099; // Level 110
                                        else if (Eq.ID == 117099)
                                            newID = 117109; // Level 120
                                        //else if (I.ID == 117109)
                                        //newID = 117119; // Level 135
                                        #endregion
                                        #region Coronet
                                        //Start For Level 10
                                        else if (Eq.ID == 118059)
                                            newID = 118069; // Level 40
                                        else if (Eq.ID == 118069)
                                            newID = 118079; // Level 70 
                                        else if (Eq.ID == 118079)
                                            newID = 118089; // Level 100
                                        else if (Eq.ID == 118089)
                                            newID = 118099; // Level 110
                                        else if (Eq.ID == 118099)
                                            newID = 118109; // Level 120
                                        //else if (I.ID == 117109)
                                        //newID = 117119; // Level 135
                                        #endregion
                                        #region Hood
                                        //Start For Level 10
                                        else if (Eq.ID == 123059)
                                            newID = 123069; // Level 40
                                        else if (Eq.ID == 123069)
                                            newID = 123079; // Level 70 
                                        else if (Eq.ID == 123079)
                                            newID = 123089; // Level 100
                                        else if (Eq.ID == 123089)
                                            newID = 123099; // Level 110
                                        else if (Eq.ID == 123099)
                                            newID = 123109; // Level 120
                                        //else if (I.ID == 117109)
                                        //newID = 117119; // Level 135
                                        #endregion
                                        #region HeadBand
                                        //Start For Level 10
                                        else if (Eq.ID == 141059)
                                            newID = 141069; // Level 40
                                        else if (Eq.ID == 141069)
                                            newID = 141079; // Level 70 
                                        else if (Eq.ID == 141079)
                                            newID = 141089; // Level 100
                                        else if (Eq.ID == 141089)
                                            newID = 141099; // Level 110
                                        else if (Eq.ID == 141099)
                                            newID = 141109; // Level 120
                                        //else if (I.ID == 117109)
                                        //newID = 117119; // Level 135
                                        #endregion
                                        #region Plume
                                        //Start For Level 10
                                        else if (Eq.ID == 142059)
                                            newID = 142069; // Level 40
                                        else if (Eq.ID == 142069)
                                            newID = 142079; // Level 70 
                                        else if (Eq.ID == 142079)
                                            newID = 142089; // Level 100
                                        else if (Eq.ID == 142089)
                                            newID = 142099; // Level 110
                                        else if (Eq.ID == 142099)
                                            newID = 142109; // Level 120
                                        //else if (I.ID == 117109)
                                        //newID = 117119; // Level 135
                                        #endregion
                                        #endregion
                                        #region Necklace & Bag
                                        #region Necklace
                                        //Start For Level 10
                                        if (Eq.ID == 120129)
                                            newID = 120159; // Level 40
                                        else if (Eq.ID == 120159)
                                            newID = 120189; // Level 70 
                                        else if (Eq.ID == 120189)
                                            newID = 120219; // Level 100
                                        else if (Eq.ID == 120219)
                                            newID = 120229; // Level 110
                                        else if (Eq.ID == 120229)
                                            newID = 120239; // Level 120
                                        else if (Eq.ID == 120239)
                                            newID = 120249; // Level 130
                                        else if (Eq.ID == 120249)
                                            newID = 120259; // Level 135
                                        #endregion
                                        #region Bag
                                        //Start For Level 10
                                        else if (Eq.ID == 121129)
                                            newID = 121159; // Level 40
                                        else if (Eq.ID == 121159)
                                            newID = 121189; // Level 70 
                                        else if (Eq.ID == 121189)
                                            newID = 121219; // Level 100
                                        else if (Eq.ID == 121219)
                                            newID = 121229; // Level 110
                                        else if (Eq.ID == 121229)
                                            newID = 121239; // Level 120
                                        else if (Eq.ID == 121239)
                                            newID = 121249; // Level 130
                                        else if (Eq.ID == 121249)
                                            newID = 121259; // Level 135
                                        #endregion
                                        #endregion
                                        #region Armors
                                        #region TrojanArmor
                                        //Start For Level 10
                                        if (Eq.ID == 130069)
                                            newID = 130079; // Level 40
                                        else if (Eq.ID == 130079)
                                            newID = 130089; // Level 70 
                                        else if (Eq.ID == 130089)
                                            newID = 130099; // Level 100
                                        else if (Eq.ID == 130099)
                                            newID = 130109; // Level 120
                                        #endregion
                                        #region WarriorArmor
                                        //Start For Level 10
                                        else if (Eq.ID == 131069)
                                            newID = 131079; // Level 40
                                        else if (Eq.ID == 131079)
                                            newID = 131089; // Level 70 
                                        else if (Eq.ID == 131089)
                                            newID = 131099; // Level 100
                                        else if (Eq.ID == 131099)
                                            newID = 131109; // Level 120
                                        #endregion
                                        #region ArcherCoat
                                        //Start For Level 10
                                        else if (Eq.ID == 133069)
                                            newID = 133079; // Level 40
                                        else if (Eq.ID == 133079)
                                            newID = 133089; // Level 70 
                                        else if (Eq.ID == 133089)
                                            newID = 133099; // Level 100
                                        else if (Eq.ID == 133099)
                                            newID = 133109; // Level 120
                                        #endregion
                                        #region TaoistRobe
                                        //Start For Level 10
                                        else if (Eq.ID == 134069)
                                            newID = 134079; // Level 40
                                        else if (Eq.ID == 134079)
                                            newID = 134089; // Level 70 
                                        else if (Eq.ID == 134089)
                                            newID = 134099; // Level 100
                                        else if (Eq.ID == 134099)
                                            newID = 134109; // Level 120
                                        #endregion
                                        #region NinjaVest
                                        //Start For Level 10
                                        else if (Eq.ID == 135069)
                                            newID = 135079; // Level 40
                                        else if (Eq.ID == 135079)
                                            newID = 135089; // Level 70 
                                        else if (Eq.ID == 135089)
                                            newID = 135099; // Level 100
                                        else if (Eq.ID == 135099)
                                            newID = 135109; // Level 120
                                        #endregion
                                        #endregion
                                        #region Weapons
                                        #region Blade
                                        //Start For Level 10
                                        if (Eq.ID == 410199)
                                            newID = 410209; // Level 40
                                        else if (Eq.ID == 410209)
                                            newID = 410219; // Level 70 
                                        else if (Eq.ID == 410219)
                                            newID = 410229; // Level 100
                                        else if (Eq.ID == 410229)
                                            newID = 410239; // Level 120
                                        else if (Eq.ID == 410239)
                                            newID = 410249; // Level 121
                                        else if (Eq.ID == 410249)
                                            newID = 410259; // Level 122
                                        else if (Eq.ID == 410259)
                                            newID = 410269; // Level 123
                                        else if (Eq.ID == 410269)
                                            newID = 410279; // Level 124
                                        else if (Eq.ID == 410279)
                                            newID = 410289; // Level 125
                                        else if (Eq.ID == 410289)
                                            newID = 410299; // Level 126
                                        else if (Eq.ID == 410299)
                                            newID = 410309; // Level 127
                                        else if (Eq.ID == 410309)
                                            newID = 410319; // Level 128
                                        else if (Eq.ID == 410319)
                                            newID = 410329; // Level 129
                                        else if (Eq.ID == 410329)
                                            newID = 410339; // Level 130
                                        #endregion
                                        #region Sword
                                        //Start For Level 10
                                        else if (Eq.ID == 420199)
                                            newID = 420209; // Level 40
                                        else if (Eq.ID == 420209)
                                            newID = 420219; // Level 70 
                                        else if (Eq.ID == 420219)
                                            newID = 420229; // Level 100
                                        else if (Eq.ID == 420229)
                                            newID = 420239; // Level 120
                                        else if (Eq.ID == 420239)
                                            newID = 420249; // Level 121
                                        else if (Eq.ID == 420249)
                                            newID = 420259; // Level 122
                                        else if (Eq.ID == 420259)
                                            newID = 420269; // Level 123
                                        else if (Eq.ID == 420269)
                                            newID = 420279; // Level 124
                                        else if (Eq.ID == 420279)
                                            newID = 420289; // Level 125
                                        else if (Eq.ID == 420289)
                                            newID = 420299; // Level 126
                                        else if (Eq.ID == 420299)
                                            newID = 420309; // Level 127
                                        else if (Eq.ID == 420309)
                                            newID = 420319; // Level 128
                                        else if (Eq.ID == 420319)
                                            newID = 420329; // Level 129
                                        else if (Eq.ID == 420329)
                                            newID = 420339; // Level 130
                                        #endregion
                                        #region BackSword
                                        //Start For Level 10
                                        else if (Eq.ID == 421199)
                                            newID = 421209; // Level 40
                                        else if (Eq.ID == 421209)
                                            newID = 421219; // Level 70 
                                        else if (Eq.ID == 421219)
                                            newID = 421229; // Level 100
                                        else if (Eq.ID == 421229)
                                            newID = 421239; // Level 120
                                        else if (Eq.ID == 421239)
                                            newID = 421249; // Level 121
                                        else if (Eq.ID == 421249)
                                            newID = 421259; // Level 122
                                        else if (Eq.ID == 421259)
                                            newID = 421269; // Level 123
                                        else if (Eq.ID == 421269)
                                            newID = 421279; // Level 124
                                        else if (Eq.ID == 421279)
                                            newID = 421289; // Level 125
                                        else if (Eq.ID == 421289)
                                            newID = 421299; // Level 126
                                        else if (Eq.ID == 421299)
                                            newID = 421309; // Level 127
                                        else if (Eq.ID == 421309)
                                            newID = 421319; // Level 128
                                        else if (Eq.ID == 421319)
                                            newID = 421329; // Level 129
                                        else if (Eq.ID == 421329)
                                            newID = 421339; // Level 130
                                        #endregion
                                        #region Club
                                        //Start For Level 10
                                        else if (Eq.ID == 480199)
                                            newID = 480209; // Level 40
                                        else if (Eq.ID == 480209)
                                            newID = 480219; // Level 70 
                                        else if (Eq.ID == 480219)
                                            newID = 480229; // Level 100
                                        else if (Eq.ID == 480229)
                                            newID = 480239; // Level 120
                                        else if (Eq.ID == 480239)
                                            newID = 480249; // Level 121
                                        else if (Eq.ID == 480249)
                                            newID = 480259; // Level 122
                                        else if (Eq.ID == 480259)
                                            newID = 480269; // Level 123
                                        else if (Eq.ID == 480269)
                                            newID = 480279; // Level 124
                                        else if (Eq.ID == 480279)
                                            newID = 480289; // Level 125
                                        else if (Eq.ID == 480289)
                                            newID = 480299; // Level 126
                                        else if (Eq.ID == 480299)
                                            newID = 480309; // Level 127
                                        else if (Eq.ID == 480309)
                                            newID = 480319; // Level 128
                                        else if (Eq.ID == 480319)
                                            newID = 480329; // Level 129
                                        else if (Eq.ID == 480329)
                                            newID = 480339; // Level 130
                                        #endregion
                                        #region Bow
                                        //Start For Level 10
                                        else if (Eq.ID == 500189)
                                            newID = 500199; // Level 40
                                        else if (Eq.ID == 500199)
                                            newID = 500209; // Level 70 
                                        else if (Eq.ID == 500209)
                                            newID = 500219; // Level 100
                                        else if (Eq.ID == 500219)
                                            newID = 500229; // Level 120
                                        else if (Eq.ID == 500229)
                                            newID = 500239; // Level 121
                                        else if (Eq.ID == 500239)
                                            newID = 500249; // Level 122
                                        else if (Eq.ID == 500249)
                                            newID = 500259; // Level 123
                                        else if (Eq.ID == 500259)
                                            newID = 500269; // Level 124
                                        else if (Eq.ID == 500269)
                                            newID = 500279; // Level 125
                                        else if (Eq.ID == 500279)
                                            newID = 500289; // Level 126
                                        else if (Eq.ID == 500289)
                                            newID = 500299; // Level 127
                                        else if (Eq.ID == 500299)
                                            newID = 500309; // Level 128
                                        else if (Eq.ID == 500309)
                                            newID = 500319; // Level 129
                                        else if (Eq.ID == 500319)
                                            newID = 500329; // Level 130
                                        #endregion
                                        #region Katana
                                        //Start For Level 10
                                        else if (Eq.ID == 601199)
                                            newID = 601209; // Level 40
                                        else if (Eq.ID == 601209)
                                            newID = 601219; // Level 70 
                                        else if (Eq.ID == 601219)
                                            newID = 601229; // Level 100
                                        else if (Eq.ID == 601229)
                                            newID = 601239; // Level 120
                                        else if (Eq.ID == 601239)
                                            newID = 601249; // Level 121
                                        else if (Eq.ID == 601249)
                                            newID = 601259; // Level 122
                                        else if (Eq.ID == 601259)
                                            newID = 601269; // Level 123
                                        else if (Eq.ID == 601269)
                                            newID = 601279; // Level 124
                                        else if (Eq.ID == 601279)
                                            newID = 601289; // Level 125
                                        else if (Eq.ID == 601289)
                                            newID = 601299; // Level 126
                                        else if (Eq.ID == 601299)
                                            newID = 601309; // Level 127
                                        else if (Eq.ID == 601309)
                                            newID = 601319; // Level 128
                                        else if (Eq.ID == 601319)
                                            newID = 601329; // Level 129
                                        else if (Eq.ID == 601329)
                                            newID = 601339; // Level 130
                                        #endregion
                                        #endregion
                                        #region Shield
                                        //Start For Level 10
                                        if (Eq.ID == 900049)
                                            newID = 900059; // Level 40
                                        else if (Eq.ID == 900059)
                                            newID = 900069; // Level 70 
                                        else if (Eq.ID == 900069)
                                            newID = 900079; // Level 100
                                        else if (Eq.ID == 900079)
                                            newID = 900089; // Level 110
                                        else if (Eq.ID == 900089)
                                            newID = 900099; // Level 120
                                        else if (Eq.ID == 900099)
                                            newID = 900109; // Level 130
                                        #endregion
                                        #region Ring & Braclet
                                        #region Ring
                                        //Start For Level 10
                                        if (Eq.ID == 150159)
                                            newID = 150179; // Level 40
                                        else if (Eq.ID == 150179)
                                            newID = 150199; // Level 70 
                                        else if (Eq.ID == 150199)
                                            newID = 150219; // Level 100
                                        else if (Eq.ID == 150219)
                                            newID = 150229; // Level 110
                                        else if (Eq.ID == 150229)
                                            newID = 150239; // Level 120
                                        else if (Eq.ID == 150239)
                                            newID = 150249; // Level 130
                                        else if (Eq.ID == 150249)
                                            newID = 150259; // Level 135
                                        #endregion
                                        #region Braclet
                                        //Start For Level 10
                                        else if (Eq.ID == 152169)
                                            newID = 152189; // Level 40
                                        else if (Eq.ID == 152189)
                                            newID = 152209; // Level 70 
                                        else if (Eq.ID == 152209)
                                            newID = 152229; // Level 100
                                        else if (Eq.ID == 152229)
                                            newID = 152239; // Level 110
                                        else if (Eq.ID == 152239)
                                            newID = 152249; // Level 120
                                        else if (Eq.ID == 152249)
                                            newID = 152259; // Level 130
                                        else if (Eq.ID == 152259)
                                            newID = 152269; // Level 135
                                        #endregion
                                        #endregion
                                        #region Boots
                                        //Start For Level 10
                                        if (Eq.ID == 160159)
                                            newID = 160179; // Level 40
                                        else if (Eq.ID == 160179)
                                            newID = 160199; // Level 70 
                                        else if (Eq.ID == 160199)
                                            newID = 160219; // Level 100
                                        else if (Eq.ID == 160219)
                                            newID = 160229; // Level 110
                                        else if (Eq.ID == 160229)
                                            newID = 160239; // Level 120
                                        else if (Eq.ID == 160239)
                                            newID = 160249; // Level 130
                                        else if (Eq.ID == 160249)
                                            newID = 160259; // Level 135
                                        #endregion

                                        DatabaseItem DBI = (DatabaseItem)Database.DatabaseItems[newID];
                                        byte NewLevel = DBI.Level;

                                        if (GC.MyChar.Level >= NewLevel)
                                        {
                                            if (NewLevel <= 140 && PrevLevel < 140)
                                            {
                                                if (!GC.Agreed)//false
                                                {
                                                    byte DBsPrise = 0;
                                                    if (NewLevel <= 130)
                                                        DBsPrise = 1;
                                                    else if (NewLevel >= 130 && NewLevel <= 140)
                                                        DBsPrise = 2;
                                                    GC.SendPacket(Packets.NPCText("You need " + DBsPrise + " DragonBalls to upgrade."));
                                                    GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                    GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    GC.Agreed = true;
                                                }
                                                else//true
                                                {
                                                    byte DBsPrise = 0;
                                                    if (NewLevel <= 130)
                                                        DBsPrise = 1;
                                                    else if (NewLevel >= 130 && NewLevel <= 140)
                                                        DBsPrise = 2;

                                                    GC.Agreed = false;
                                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsPrise))
                                                    {
                                                        GC.MyChar.EquipStats(Pos, false);
                                                        GC.MyChar.RemoveItemIDAmount(1088000, DBsPrise);
                                                        Eq.ID = newID;
                                                        GC.MyChar.Equips.Replace(Pos, Eq, GC.MyChar);
                                                        GC.MyChar.EquipStats(Pos, true);

                                                        GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                    else
                                                    {
                                                        GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                        GC.SendPacket(Packets.NPCLink("Sorry", 255));
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Item Level Is Higher Than 140 I Can`t Help You More."));
                                                GC.SendPacket(Packets.NPCLink("Nevermind.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("There is no item to upgrade it`s level."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 0 && GC.MyChar.Job >= 160 && GC.MyChar.Job <= 165)
                                {
                                    GC.SendPacket(Packets.NPCText("Sorry But I Cant Help The Avatars"));
                                    GC.SendPacket(Packets.NPCLink(">> Ok No Proplem.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region Compose & Bless & Enchant
                        case 11020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Compose or Enchant Your Items?"));
                                    GC.SendPacket(Packets.NPCLink("Compose.", 1));
                                    GC.SendPacket(Packets.NPCLink("Bless.", 2));
                                    GC.SendPacket(Packets.NPCLink("Enchant.", 3));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Agreed = false;
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 1, N.Loc.X, N.Loc.Y, 0x7e));
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.GeneralData(GC.MyChar.EntityID, 0x443, GC.MyChar.Loc.X, GC.MyChar.Loc.Y, 116));
                                }
                                else if (option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose The Equipment You Want To Upgrade It`s Bless."));
                                    GC.SendPacket(Packets.NPCLink("Headgear", 101));
                                    GC.SendPacket(Packets.NPCLink("Necklace", 102));
                                    GC.SendPacket(Packets.NPCLink("Armor", 103));
                                    GC.SendPacket(Packets.NPCLink("Weapon", 104));
                                    GC.SendPacket(Packets.NPCLink("Shield", 105));
                                    GC.SendPacket(Packets.NPCLink("Ring", 106));
                                    GC.SendPacket(Packets.NPCLink("Boots", 108));
                                    GC.SendPacket(Packets.NPCLink("Next.", 200));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 200)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose The Equipment You Want To Upgrade It`s Bless."));
                                    GC.SendPacket(Packets.NPCLink("HeavenFan", 110));
                                    GC.SendPacket(Packets.NPCLink("StartTower", 111));
                                    GC.SendPacket(Packets.NPCLink("Back", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 101 && option <= 111)
                                {
                                    byte Pos = (byte)(option - 100);
                                    Game.Item Eq = GC.MyChar.Equips.Get(Pos);
                                    if (Eq.ID != 0)
                                    {
                                        if (Eq.Bless != 7 && (option >= 110 && option <= 111 && Eq.Bless != 1))
                                        {
                                            byte TortoiseNeed = 0;
                                            if (option >= 101 && option <= 108)
                                            {
                                                if (Eq.Bless == 0)
                                                    TortoiseNeed = 5;
                                                else if (Eq.Bless == 1)
                                                    TortoiseNeed = 1;
                                                else if (Eq.Bless == 3)
                                                    TortoiseNeed = 3;
                                                else if (Eq.Bless == 5)
                                                    TortoiseNeed = 5;
                                            }
                                            else if (option >= 110 && option <= 111)
                                            {
                                                if (Eq.Bless == 0)
                                                    TortoiseNeed = 10;
                                            }

                                            if (GC.Agreed == false)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + TortoiseNeed + " SuperTortoises to upgrade. Do you want it?"));
                                                if (Eq.Bless != 0)
                                                    GC.SendPacket(Packets.NPCText("Your item current Bless is " + Eq.Bless + "."));
                                                if (Eq.Bless == 0)
                                                    GC.SendPacket(Packets.NPCText("And Your item will be Bless 1."));
                                                else
                                                    GC.SendPacket(Packets.NPCText("And Your item will be Bless " + (Eq.Bless + 2) + "."));
                                                GC.SendPacket(Packets.NPCLink("Yes.", option));
                                                GC.SendPacket(Packets.NPCLink("Nevermind.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(700073, TortoiseNeed))
                                                {
                                                    GC.MyChar.EquipStats(Pos, false);
                                                    GC.MyChar.RemoveItemIDAmount(700073, TortoiseNeed);
                                                    if (Eq.Bless == 0)
                                                        Eq.Bless = 1;
                                                    else
                                                        Eq.Bless += 2;
                                                    GC.MyChar.Equips.Replace(Pos, Eq, GC.MyChar);
                                                    GC.MyChar.EquipStats(Pos, true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough TortoiseGems."));
                                                    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You cannot upgrade an item's Bless which is already at maximum."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("There is no item to upgrade it`s bless."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region SocketUpgrader
                        case 11030:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi you i am here to help you get your items socket.So are you Ready!."));
                                    GC.SendPacket(Packets.NPCLink("Upgrade Socket.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose The Equipment You Want To Upgrade It`s Socket."));
                                    GC.SendPacket(Packets.NPCLink("Headgear", 101));
                                    GC.SendPacket(Packets.NPCLink("Necklace", 102));
                                    GC.SendPacket(Packets.NPCLink("Armor", 103));
                                    GC.SendPacket(Packets.NPCLink("Weapon", 104));
                                    GC.SendPacket(Packets.NPCLink("Shield", 105));
                                    GC.SendPacket(Packets.NPCLink("Ring", 106));
                                    GC.SendPacket(Packets.NPCLink("Boots", 108));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 101 && option <= 108)
                                {
                                    byte Pos = (byte)(option - 100);
                                    Game.Item Eq = GC.MyChar.Equips.Get(Pos);
                                    if (Eq.ID != 0)
                                    {
                                        if (Eq.Soc1 == Item.Gem.NoSocket)
                                        {
                                            if (GC.MyChar.FindInventoryItemIDAmount(1200006, 1))
                                            {
                                                GC.MyChar.EquipStats(Pos, false);
                                                GC.MyChar.RemoveItemIDAmount(1200006, 1);
                                                Eq.Soc1 = Item.Gem.EmptySocket;
                                                GC.MyChar.Equips.Replace(Pos, Eq, GC.MyChar);
                                                GC.MyChar.EquipStats(Pos, true);

                                                GC.SendPacket(Packets.NPCText("Congratulation your weapon now have it is FirstSocket."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have StarDrill."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else if (Eq.Soc2 == Item.Gem.NoSocket)
                                        {
                                            if (GC.MyChar.FindInventoryItemIDAmount(1200005, 1))
                                            {
                                                GC.MyChar.EquipStats(Pos, false);
                                                GC.MyChar.RemoveItemIDAmount(1200005, 1);
                                                Eq.Soc2 = Item.Gem.EmptySocket;
                                                GC.MyChar.Equips.Replace(Pos, Eq, GC.MyChar);
                                                GC.MyChar.EquipStats(Pos, true);

                                                GC.SendPacket(Packets.NPCText("Congratulation your weapon now have it is SecondSocket."));
                                                GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("You don't have ToughDrill."));
                                                GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Your item is already have it`s SecondSocket."));
                                            GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("There is no item to upgrade it`s socket."));
                                        GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion                        

                        #region PK Mission
                        case 13010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Welcome To Me This Is PKMission Champion, Do You Want Join It?"));
                                    GC.SendPacket(Packets.NPCLink("Join Me.", 5));
                                    GC.SendPacket(Packets.NPCLink("Show Me Rules.", 2));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 5)
                                {
                                    if (Extra.PKTournament.Stage == Extra.PKTournamentStage.Inviting)
                                    {
                                        NPCText("Would you like to join the PK Mission ? You have " + (Extra.PKTournament.CountDown - 10) + " seconds to join.", GC);
                                        NPCLink("Sure, I'll join!", 1, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    else if ((Extra.PKTournament.Stage != Extra.PKTournamentStage.None) && (Extra.PKTournament.Stage != Extra.PKTournamentStage.Inviting))
                                    {
                                        NPCText("You're too late, " + GC.MyChar.Name + ", the Mission has already started!", GC);
                                        NPCLink("Sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("PK Mission Is Gone Come.", GC);
                                        NPCLink("Sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 1)
                                {
                                    NPCText("Goodluck, " + GC.MyChar.Name + ".", GC);
                                    NPCLink("Thanks", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.MyChar.InPKT = true;
                                    GC.MyChar.CurHP = GC.MyChar.MaxHP;
                                    if (!Extra.PKTournament.PKTHash.ContainsKey(GC.MyChar.EntityID))
                                        Extra.PKTournament.PKTHash.Add(GC.MyChar.EntityID, GC);
                                }
                                else if (option == 2)
                                {
                                    NPCText("The PK Mission is 3 step it mean there was 3 maps you must be alive on it to Win", GC);
                                    NPCText("The last man that standing will 'Win' the PK Mission.", GC);
                                    NPCLink("I want to join!", 1, GC);
                                    NPCLink("No thanks.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                return;
                            }
                        #endregion
                        #region DeathMatch
                        case 13000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Welcome To Me This Is DeathMatch Champion, Do You Want Join It?"));
                                    NPCLink("Join Me.", 1, GC);
                                    NPCLink("Check My Team.", 3, GC);
                                    NPCLink("Just Passing By.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Level >= 130 && GC.MyChar.Reborns >= 2)
                                    {
                                        if (World.DeathMatch.DeathMatchStart == true)
                                        {
                                            Game.Item I = GC.MyChar.Equips.Get(9);
                                            if (I.ID >= 181305 && I.ID <= 191905)
                                            {
                                                GC.SendPacket(Packets.NPCText("Sorry You Wear Garment Please UnEquip (Remove) Your Germant. To Be Able To join."));
                                                GC.SendPacket(Packets.NPCLink("Ah, Sorry", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                return;
                                            }
                                            NPCText("Hello, You Know Players Will Relocated Automaticly In Two Group [First Is RedTeam - Secound Is BlackTeam], Are You Realy Want Join?", GC);
                                            NPCLink("Yeah.", 2, GC);
                                            NPCLink("No Thanks.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("Sorry, DeathMatch Starting Every Hour . You Must Take Place Before It Start.", GC);
                                            NPCLink("Sorry.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("Sorry, But You Must Be [Level 130+ & SecoundReborn] At Least To Join.", GC);
                                        NPCLink("Sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.dmjoin == 0)
                                    {
                                        if (World.DeathMatch.DeathMatchStart == true)
                                        {
                                            GC.MyChar.dmjoin = 1;
                                            if (World.DeathMatch.teamred < World.DeathMatch.teamblack)
                                            {
                                                GC.MyChar.dmred = 1;
                                                World.DeathMatch.teamred += 1;
                                                GC.MyChar.MyTDmTeam.TeamName = "RedTeam";
                                                GC.MyChar.MyTDmTeam.TeamID = 20;
                                            }
                                            else
                                            {
                                                GC.MyChar.dmblack = 1;
                                                World.DeathMatch.teamblack += 1;
                                                GC.MyChar.MyTDmTeam.TeamName = "BlackTeam";
                                                GC.MyChar.MyTDmTeam.TeamID = 10;
                                            }
                                            NPCText("Your Team Is [" + GC.MyChar.MyTDmTeam.TeamName + "], Your Team Must Get Higher Score In 25 Minute.", GC);
                                            NPCLink("Thanks.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            NPCText("Sorry, DeathMatch Starting Every Hour . You Must Take Place Before It Start.", GC);
                                            NPCLink("Sorry.", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("You Already In [" + GC.MyChar.MyTDmTeam.TeamName + "].", GC);
                                        NPCLink("Sorry.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.dmjoin > 0)
                                    {
                                        NPCText("Your Team Is [" + GC.MyChar.MyTDmTeam.TeamName + "], Your Team Must Get Higher Score In 25 Minute.", GC);
                                        NPCLink("Thanks.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("You Are Not Join Any Team Yet.", GC);
                                        NPCLink("Thanks.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region VS Arena
                        #region Sign in NPC
                        case 13020:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hello, you want to participate in the 1v1 (fastblade / scentSowrd) tournament against the opponent that you want?", GC);
                                    NPCLink("FB/SS Battle", 20, GC);
                                    NPCLink("Death Battle", 30, GC);
                                    NPCLink("Change Settings", 35, GC);
                                    //OptionLink("More information and RULES", 10, GC);
                                    NPCLink("Clear My Tornament ", 100, GC);
                                    NPCLink("No Thanks", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 100)
                                {
                                    World.pvpclear(GC.MyChar);
                                    NPCText("You Tornament Is Clear.", GC);
                                    NPCLink("Try Againe", 10, GC);
                                    NPCLink("Good bye.", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 10)
                                {
                                    NPCText("Hello, you want to participate in the 1v1 (fastblade / scentSowrd) tournament against the opponent that you want?", GC);
                                    NPCLink("FB/SS Battle", 20, GC);
                                    NPCLink("Death Battle", 30, GC);
                                    NPCLink("Change Settings", 35, GC);
                                    NPCLink("Clear My Tornament ", 100, GC);
                                    NPCLink("No Thanks", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                #region Settings

                                else if (option == 35)
                                {
                                    NPCText("Chose TheMap That you want to play in Also TheNumber off MaxScore For the wenner And THe time .", GC);
                                    NPCLink("Maps", 36, GC);
                                    NPCLink("Max Score", 37, GC);
                                    NPCLink("Time to Limite", 38, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Ok, bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 38)
                                {
                                    NPCText("Chose The TimeLimet To END the Battele .", GC);
                                    NPCLink("End In 1 Miniute", 70, GC);
                                    NPCLink("End In 2 Miniutes", 71, GC);
                                    NPCLink("End In 5 Miniutes", 72, GC);
                                    NPCLink("End In 10 Miniutes", 73, GC);
                                    NPCLink("End In 15 Miniutes", 74, GC);
                                    NPCLink("End In 20 Miniutes", 75, GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 70 && option <= 75)
                                {
                                    if (option == 70) GC.MyChar.VsLimetTimeLastes = 1;
                                    if (option == 71) GC.MyChar.VsLimetTimeLastes = 2;
                                    if (option == 72) GC.MyChar.VsLimetTimeLastes = 5;
                                    if (option == 73) GC.MyChar.VsLimetTimeLastes = 10;
                                    if (option == 74) GC.MyChar.VsLimetTimeLastes = 15;
                                    if (option == 75) GC.MyChar.VsLimetTimeLastes = 20;
                                    NPCText("Time DowenCounter is " + GC.MyChar.VsLimetTimeLastes + " Miniutes.", GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 37)
                                {
                                    NPCText("Chose The Final Score .", GC);
                                    NPCLink("play for 1 Point", 60, GC);
                                    NPCLink("play for 2 Points", 61, GC);
                                    NPCLink("play for 5 Points", 62, GC);
                                    NPCLink("play for 10 Points", 63, GC);
                                    NPCLink("play for 15 Points", 64, GC);
                                    NPCLink("play for 20 Point", 65, GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 60 && option <= 65)
                                {
                                    if (option == 60) GC.MyChar.VSpets = 1;
                                    if (option == 61) GC.MyChar.VSpets = 2;
                                    if (option == 62) GC.MyChar.VSpets = 5;
                                    if (option == 63) GC.MyChar.VSpets = 10;
                                    if (option == 64) GC.MyChar.VSpets = 15;
                                    if (option == 65) GC.MyChar.VSpets = 20;
                                    NPCText("Score is " + GC.MyChar.VSpets + " .", GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    NPCLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 36)
                                {
                                    NPCText("Chose TheMap That you want to play in Also TheNumber off MaxScore For the wenner And THe time .", GC);
                                    NPCLink("Arena 1", 50, GC);
                                    NPCLink("Arena 2", 51, GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 51)
                                {
                                    GC.MyChar.VSmaptogo = 1068;
                                    NPCText("You joind | Arena2 |.", GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 50)
                                {
                                    NPCText("You joind | Arena1 |.", GC);
                                    NPCLink("Back To Settings", 35, GC);
                                    NPCLink("Back To Battle Menue", 10, GC);
                                    //OptionLink("Iam Leaving.. Bye..", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);

                                }
                                #endregion
                                else if (option == 20)
                                {
                                    if (GC.MyChar.Enemigo != "")
                                        foreach (Character C in World.H_Chars.Values)
                                        {
                                            if (C.Name == GC.MyChar.Enemigo)
                                                if (C != null)
                                                {
                                                    Game.World.pvpclear(C);
                                                    Game.World.pvpclear(GC.MyChar);
                                                }
                                        }
                                    if ((GC.MyChar.Equips.RightHand.ID >= 410003 && GC.MyChar.Equips.RightHand.ID <= 421339) || (GC.MyChar.Equips.LeftHand.ID >= 410003 && GC.MyChar.Equips.LeftHand.ID <= 421339))
                                    {
                                        NPCText("Enter the bet you want to do with your enemy. For example, if you bet and win 50 fps, 50 fps you rob the opponent. If you lose, the opponent to stay with your CPs.", GC);
                                        NPCLink("1000 CPs :))", 21, GC);
                                        NPCLink("10k CPs", 22, GC);
                                        NPCLink("50k CPs", 23, GC);
                                        NPCLink("100k CPs", 24, GC);
                                        NPCLink("500k CPs", 25, GC);
                                        NPCLink("1kk CPs", 26, GC);
                                        NPCLink("2kk CPs", 27, GC);
                                        NPCLink("Good bye.", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        NPCText("Sorry, you need Blade, sword or backsword for fight.", GC);
                                        NPCLink("Oh.. i see..", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 30)
                                {
                                    if (GC.MyChar.Enemigo != "")
                                        foreach (Character C in World.H_Chars.Values)
                                        {
                                            if (C.Name == GC.MyChar.Enemigo)
                                                if (C != null)
                                                {
                                                    Game.World.pvpclear(C);
                                                    Game.World.pvpclear(GC.MyChar);
                                                }
                                        }
                                    GC.MyChar.FreeBattel = true;
                                    {
                                        NPCText("Enter the bet you want to do with your enemy. For example, if you bet and win 50 fps, 50 fps you rob the opponent. If you lose, the opponent to stay with your CPs.", GC);
                                        NPCLink("1000 CPs :))", 21, GC);
                                        NPCLink("10k CPs", 22, GC);
                                        NPCLink("50k CPs", 23, GC);
                                        NPCLink("100k CPs", 24, GC);
                                        NPCLink("500k CPs", 25, GC);
                                        NPCLink("1kk CPs", 26, GC);
                                        NPCLink("2kk CPs", 27, GC);
                                        NPCLink("Good bye..", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 21)
                                {
                                    GC.MyChar.Apuesta = 1000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 22)
                                {
                                    GC.MyChar.Apuesta = 10000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 23)
                                {
                                    GC.MyChar.Apuesta = 50000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 24)
                                {
                                    GC.MyChar.Apuesta = 100000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 25)
                                {
                                    GC.MyChar.Apuesta = 500000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 26)
                                {
                                    GC.MyChar.Apuesta = 1000000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 27)
                                {
                                    GC.MyChar.Apuesta = 2000000;
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    NPCText("Okay, well written enter the opponent's name in the text box. Struggling with he for " + GC.MyChar.Apuesta + " CPs", GC);
                                    NPCLink2("Name", 2, GC);
                                    NPCLink("Bye guy...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    foreach (Character C in World.H_Chars.Values)
                                    {
                                        if (C.Name == ReadString(Data))
                                            if (C != null && C != GC.MyChar && !C.MyClient.Robot)
                                            {
                                                GC.MyChar.Enemigo = ReadString(Data);
                                                if (GC.MyChar.CPs >= GC.MyChar.Apuesta)
                                                {
                                                    if (C.CPs >= GC.MyChar.Apuesta)
                                                    {
                                                        if (C.Loc.Map == 1002)
                                                        {
                                                            NPCText("Really you want to invite " + GC.MyChar.Enemigo + " ?", GC);
                                                            NPCLink("Yes!", 3, GC);
                                                            NPCLink("No thanks..", 255, GC);
                                                            NPCFace(N.Avatar, GC);
                                                            NPCFinish(GC);
                                                        }
                                                        else
                                                        {
                                                            NPCText("The character has to be in Twin City.", GC);
                                                            NPCLink("Uhh ... Let me provide another", 10, GC);
                                                            NPCLink("Ok, then, bye.", 255, GC);
                                                            NPCFace(N.Avatar, GC);
                                                            NPCFinish(GC);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NPCText("Elected opponent does not have enough CPs. Choose a better one ...", GC);
                                                        NPCLink("Uhh ... Let me provide another", 10, GC);
                                                        NPCLink("Oh, sorry.", 255, GC);
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                }
                                                else
                                                {
                                                    NPCText("You dont have " + GC.MyChar.Apuesta + "Cps ,try hack me ?lol", GC);
                                                    NPCLink("Oh, sorry.", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            else
                                            {
                                                NPCText("I feel the character is not connected or does not exist. Or did you choose yourself as a contender ..", GC);
                                                NPCLink("Uhh ... Let me provide another", 1, GC);
                                                NPCLink("Oh, sorry.", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                    }

                                }
                                else if (option == 3)
                                {
                                    foreach (Character C in World.H_Chars.Values)
                                    {
                                        if (C.Name == GC.MyChar.Enemigo)
                                        {
                                            if (GC.MyChar.FreeBattel == true || (GC.MyChar.FreeBattel == false && ((C.Equips.RightHand.ID >= 410003 && C.Equips.RightHand.ID <= 421339) || (C.Equips.LeftHand.ID >= 410003 && C.Equips.LeftHand.ID <= 421339))))
                                            {
                                                if (C.Luchando == false && C.Enemigo == "")
                                                {
                                                    C.Apuesta = GC.MyChar.Apuesta;
                                                    C.Enemigo = GC.MyChar.Name;
                                                    C.VSpets = GC.MyChar.VSpets;
                                                    C.VSmaptogo = GC.MyChar.VSmaptogo;
                                                    C.FreeBattel = GC.MyChar.FreeBattel;
                                                    C.MyClient.DialogNPC = 600566;
                                                    PacketHandling.NPCDialog.Handles(C.MyClient, null, 600566, 0);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "The request has been sent to the opprent");
                                                }
                                                else
                                                {
                                                    NPCText("The opponent is in a Battle!", GC);
                                                    World.pvpclear(GC.MyChar);
                                                    NPCLink("Uhh, let me choise another", 1, GC);
                                                    NPCLink("Oh, bye", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            else
                                            {
                                                World.pvpclear(GC.MyChar);
                                                NPCText("The opponent who has asked not to have sword or blade, you can not fight with he!", GC);
                                                NPCLink("Uhh, let me choise another", 1, GC);
                                                NPCLink("Oh, bye", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Checkout
                        case 13021:
                            {
                                Character C = World.CharacterFromName(GC.MyChar.Enemigo);
                                if (option == 0)
                                {

                                    if (C != null)
                                    {
                                        string battelTyp = "";
                                        if (C.FreeBattel == false) battelTyp = "Fast Blade Chalenge";
                                        else
                                            battelTyp = "Death Battel";
                                        NPCText(GC.MyChar.Enemigo + " has invited you to play for " + C.Apuesta + " CPs in a 1v1" + battelTyp + " for " + C.VSpets + ", ?", GC);
                                        NPCLink("Yes, i want figth.", 1, GC);
                                        NPCLink("More information of my enemy.", 3, GC);
                                        NPCLink("No thanks", 2, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 1)
                                {
                                    //Character C = World.CharacterFromName(GC.MyChar.Enemigo);
                                    if (C != null)
                                    {
                                        if (GC.MyChar.CPs >= C.Apuesta && C.CPs >= C.Apuesta
                                        && GC.MyChar.Enemigo == C.Name
                                        && C.Enemigo == GC.MyChar.Name)
                                        {
                                            #region copy data
                                            GC.MyChar.FreeBattel = C.FreeBattel;
                                            GC.MyChar.VSmaptogo = C.VSmaptogo;
                                            GC.MyChar.VsLimetTimeLastes = C.VsLimetTimeLastes;
                                            GC.MyChar.VSpets = C.VSpets;
                                            #endregion
                                            GC.MyChar.PVPCuonter = DateTime.Now;
                                            C.PVPCuonter = DateTime.Now;
                                            GC.MyChar.CPs -= (uint)C.Apuesta;
                                            World.MapDimentions++;
                                            GC.MyChar.PkPuntos = 0;
                                            World.Action(GC.MyChar, Packets.GeneralData(GC.MyChar.EntityID, 0, 0, 0, 135));
                                            GC.MyChar.Loc.MapDimention = Game.World.MapDimentions;
                                            GC.MyChar.Teleport(C.VSmaptogo, 52, 43);
                                            World.WorldMessage(GC.MyChar.Name, GC.MyChar.Name + " and " + C.Name + " going to fight a  for " + GC.MyChar.Apuesta + " CPs For " + C.VSpets + "!", 2011, 0, System.Drawing.Color.Red);
                                            World.Action(C, Packets.GeneralData(C.EntityID, 0, 0, 0, 135));
                                            C.Loc.MapDimention = World.MapDimentions;
                                            C.Teleport(C.VSmaptogo, 52, 50);
                                            if (C.CPs >= C.Apuesta)
                                                C.CPs -= (uint)C.Apuesta;
                                            C.PkPuntos = 0;
                                            GC.MyChar.Luchando = true;
                                            C.Luchando = true;

                                        }
                                        else
                                        {
                                            World.pvpclear(C);
                                            World.pvpclear(GC.MyChar);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("Sorry,but the invite has been canceld your opperent doesnt love waiting", GC);
                                        NPCLink("Ah,ok", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 2)
                                {
                                    {
                                        World.pvpclear(GC.MyChar);
                                        C.MyClient.DialogNPC = 600567;
                                        PacketHandling.NPCDialog.Handles(C.MyClient, null, 600567, 0);
                                    }
                                }
                                else if (option == 3)
                                {
                                    {
                                        NPCText("Información of: " + C.Name + " | Level: " + C.Level + " | Potency: " + C.Potency + " | ", GC);
                                        NPCText("Max HP: " + C.MaxHP + " | MaxMP: " + C.MaxMP + " | Job: " + C.Job + "he want to PVP you for " + C.Apuesta + "CPs .", GC);
                                        NPCLink("Ok, i want figth!", 1, GC);
                                        NPCLink("No guy.. bye", 2, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Rejected
                        case 13022:
                            {
                                if (option == 0)
                                {
                                    World.pvpclear(GC.MyChar);
                                    NPCText("Your enemy has rejected you", GC);
                                    NPCLink("Uhh... Ok thanks...", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                            }
                            break;

                        #endregion
                        #endregion

                        #region DisCity
                        #region Stage1 Start
                        case 9000: //Start solarsaint
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Our ancestors exerted their utmost efforts and defeathed the demons. Since"));
                                    GC.SendPacket(Packets.NPCText(" then the world has been kept in peace for hundreads of years. Now the"));
                                    GC.SendPacket(Packets.NPCText(" demons have come back and the world is getting into turbulence again."));
                                    GC.SendPacket(Packets.NPCLink("Ok Enter Me I Know What I Have To Do!", 2));
                                    GC.SendPacket(Packets.NPCLink("Could you tell me more?", 1));
                                    GC.SendPacket(Packets.NPCLink("I'd better leave.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("The decisive battle bewtween human and the demon broke out here. The"));
                                    GC.SendPacket(Packets.NPCText(" ferocious battle lasted for seven days and nights. Countless heroes lost their"));
                                    GC.SendPacket(Packets.NPCText(" lives. And the justice won. Afterwards we can live in peace for hundreads of"));
                                    GC.SendPacket(Packets.NPCText(" years."));
                                    GC.SendPacket(Packets.NPCLink("And then?", 3));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("UltimatePluto, leader of the demons sworn to take revenge in one thousand"));
                                    GC.SendPacket(Packets.NPCText(" years befote he managed to run away. To prevent the demons coming back, I"));
                                    GC.SendPacket(Packets.NPCText(" have been scouting their land cautiously. Yesterday I found something"));
                                    GC.SendPacket(Packets.NPCText(" unwanted. It seems that UltimatePluto had come round to endanger humans"));
                                    GC.SendPacket(Packets.NPCText(" again."));
                                    GC.SendPacket(Packets.NPCLink("What should we do?", 4));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 4)
                                {
                                    GC.SendPacket(Packets.NPCText("Fortunately UltimatePluto is still unfledged. He must resort to a Battle"));
                                    GC.SendPacket(Packets.NPCText(" Formation for the moments. So i am about to organize an army too infiltrate his"));
                                    GC.SendPacket(Packets.NPCText(" land and destroy his formation before he becomes stronger."));
                                    GC.SendPacket(Packets.NPCLink("I'll join you.", 2));
                                    GC.SendPacket(Packets.NPCLink("I'd like to know more.", 5));
                                    GC.SendPacket(Packets.NPCLink("Sigh, I'm helpless.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 5)
                                {
                                    GC.SendPacket(Packets.NPCText("You must kill UltimatePluto rapisly to get Darkhorn so that i can use it to"));
                                    GC.SendPacket(Packets.NPCText(" disable the formation. Before you can do that, you must break into HellGate,"));
                                    GC.SendPacket(Packets.NPCText(" enter the HellHall, and fight through the HellCloister. Countless ferocious"));
                                    GC.SendPacket(Packets.NPCText(" demons are watching those strongholds. I'll give you some strategies on"));
                                    GC.SendPacket(Packets.NPCText(" breaking through those fortresses if you like."));
                                    GC.SendPacket(Packets.NPCLink("I want to know HellGate", 6));
                                    GC.SendPacket(Packets.NPCLink("I want to know HellHall", 7));
                                    GC.SendPacket(Packets.NPCLink("I want to know HellCloister", 8));
                                    GC.SendPacket(Packets.NPCLink("I want to know BattleFormation", 9));
                                    GC.SendPacket(Packets.NPCLink("Thanks. I know what to do.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 6) //HellGate info
                                {
                                    GC.SendPacket(Packets.NPCText("HellGate is shielded from poisonous fog, so you can't approach it. But"));
                                    GC.SendPacket(Packets.NPCText(" demons do not fear the gas. They may turn into SoulStones after they die. If"));
                                    GC.SendPacket(Packets.NPCText(" you get 5 stones for me, i'll help you break through the gate. To protect"));
                                    GC.SendPacket(Packets.NPCText(" the unrealted persons, i'll send the others back as soon as the first 60"));
                                    GC.SendPacket(Packets.NPCText(" persons pass through the gate."));
                                    GC.SendPacket(Packets.NPCLink("Thank you.", 255));
                                    GC.SendPacket(Packets.NPCLink("I'd like to know more", 5));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 7) //HellHall info
                                {
                                    GC.SendPacket(Packets.NPCText("HellHall is the very spot where the demons swear their oaths of allegiance to"));
                                    GC.SendPacket(Packets.NPCText(" UltimatePluto. Everybody must do his best to make a way out. Due to limited"));
                                    GC.SendPacket(Packets.NPCText(" time, i can lead only 30 persons to hellCloister!"));
                                    GC.SendPacket(Packets.NPCLink("Thanks", 255));
                                    GC.SendPacket(Packets.NPCLink("I'd like to know more.", 5));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 8) //Hell Cloister info
                                {
                                    GC.SendPacket(Packets.NPCText("You will be divided into 2 groups to attack from the left and the right flank of"));
                                    GC.SendPacket(Packets.NPCText(" HellCloister. Kill the Wraithas many as you can, because you can't reach"));
                                    GC.SendPacket(Packets.NPCText(" BattleFormation untill the amount of wraith is decreased to a certain level."));
                                    GC.SendPacket(Packets.NPCLink("Thank you.", 255));
                                    GC.SendPacket(Packets.NPCLink("I'd like to know more.", 5));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 9) //Battle Formation info
                                {
                                    GC.SendPacket(Packets.NPCText("BattleFormation is protected by UltimatePluto. After they are killed out,"));
                                    GC.SendPacket(Packets.NPCText(" terato dragon will appear. Make the best effort to kill him, get his dragon bice"));
                                    GC.SendPacket(Packets.NPCText(" if u have the 10 pices u will got a dragon + 7 . Then i'll send you"));
                                    GC.SendPacket(Packets.NPCText(" back. But if you fail to do it, we have to retreat and wait for another opportunity"));
                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                    GC.SendPacket(Packets.NPCLink("I'd like to know more.", 5));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Level >= 110)
                                    {
                                        if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) || (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday))
                                        {
                                            if (Game.World.DisCity.DisCityStart == true)
                                            {
                                                for (int i = 0; i < 5; i++)
                                                    GC.MyChar.RemoveItemIDAmount(723085, 1);
                                                GC.MyChar.IncreaseExp(GC.MyChar.EXPBall, false);
                                                GC.MyChar.CPs += 215;
                                                GC.MyChar.Teleport(2021, 190, 340);
                                                GC.SendPacket(Packets.NPCText("Be careful young one."));
                                                GC.SendPacket(Packets.NPCLink("I will", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                            else
                                            {
                                                GC.SendPacket(Packets.NPCText("I'm glad to see that you are such a knightly person. Please come next time."));
                                                GC.SendPacket(Packets.NPCText("you can enter DisCity at [22:00 - 22:05] on Saturday And Wednessday."));
                                                GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you can enter DisCity at [22:00 - 22:05] on Saturday And Wednessday."));
                                            GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        if (DateTime.Now.Hour == 20 && DateTime.Now.Minute >= 05)
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm glad to see that you are such a knightly person. Please come next time."));
                                            GC.SendPacket(Packets.NPCText("you can enter DisCity at [22:00 - 22:05] on Saturday And Wednessday."));
                                            GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Please come back when you are level 110 or higher"));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Stage1 To Stage2
                        case 9010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello!You need 5 SoulStone for Next Stage! and I`ll Telleport you To HellHall"));
                                    GC.SendPacket(Packets.NPCText(" I`ll Give You 10.000 CPs And 2 EXPBall."));
                                    GC.SendPacket(Packets.NPCText(" Can you give me 5 soul stone? or you dont have?"));
                                    GC.SendPacket(Packets.NPCLink("Yes,here you are", 1));
                                    GC.SendPacket(Packets.NPCLink("Sorry,i'll be back", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(723085, 5))
                                    {
                                        if (World.DisCity.PlayerInStage2 < 100)
                                        {
                                            for (int i = 0; i < 5; i++)
                                                GC.MyChar.RemoveItemIDAmount(723085, 1);

                                            GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 2, false);
                                            GC.MyChar.CPs += 10000;

                                            Game.World.DisCity.PlayerInStage2++;
                                            Game.World.WorldMessage("Server", "No. " + World.DisCity.PlayerInStage2 + "From (100) . Is[" + GC.MyChar.Name + "] From Guild [" + GC.MyChar.MyGuild + "] Has Teleported In To HellHall.", 2011, 0, System.Drawing.Color.Red);
                                            GC.MyChar.Teleport(2022, 240, 340);

                                            /*if (World.DisCity.PlayerInStage2 == 100)
                                            {
                                                foreach (DictionaryEntry DE in World.H_Chars)
                                                {
                                                    Character Chaar = (Game.Character)DE.Value;
                                                    if (Chaar.Loc.Map == 2021)
                                                    {
                                                        Chaar.Teleport(1020, 533, 483);
                                                        Chaar.MyClient.LocalMessage(2000, System.Drawing.Color.Yellow, "You didn't make it to the next floor in time.");
                                                        Game.World.SendMsgToAll("Server", "All Players of Dis City Stage1 has teleport in ApeCity", 2011, 0, System.Drawing.Color.Red);
                                                    }
                                                }
                                            }*/
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Sorry, But HellHall Is Full Players"));
                                            GC.SendPacket(Packets.NPCLink("Darn!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry, you dont have 5 SoulStone "));
                                        GC.SendPacket(Packets.NPCLink("Ok,I'll be back", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Stage2 To Stage3
                        case 9011:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello, Are You Killed Big Number Of Monster Around To Teleport You To Next Stage?"));
                                    GC.SendPacket(Packets.NPCText(" To Know [Trojan 800, Warrior 1000, Archer 1300, Ninja 600, Monk 600, WaterTaoist 500, FireTaoist 1000 and DarkTaoist 1000"));
                                    GC.SendPacket(Packets.NPCLink("Yes I Kill My Monsters", 1));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    #region DisCity KO
                                    if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15)
                                    {
                                        if (GC.MyChar.DisKO >= 800)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill  800 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                    {
                                        if (GC.MyChar.DisKO >= 1000)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill  1000 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                    {
                                        if (GC.MyChar.DisKO >= 1300)
                                        {

                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill  1300 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55)
                                    {
                                        if (GC.MyChar.DisKO >= 600)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill 600 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job >= 60 && GC.MyChar.Job <= 65)
                                    {
                                        if (GC.MyChar.DisKO >= 600)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill 600 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job >= 132 && GC.MyChar.Job <= 135)
                                    {
                                        if (GC.MyChar.DisKO >= 500)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill 500 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }

                                    else if (GC.MyChar.Job >= 142 && GC.MyChar.Job <= 145)
                                    {
                                        if (GC.MyChar.DisKO >= 1000)
                                        {
                                            GC.SendPacket(Packets.NPCText("Congration for help!Which Flank are you going to attack from?"));
                                            GC.SendPacket(Packets.NPCLink("Right.", 3));
                                            GC.SendPacket(Packets.NPCLink("Left.", 3));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);

                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You need kill 1000 Monsters "));
                                            GC.SendPacket(Packets.NPCLink("ok,I go", 255));
                                            GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    #endregion
                                }
                                if (option == 3)
                                {
                                    if (World.DisCity.PlayerInStage3 < 50)
                                    {
                                        Extra.discity.Stage4();//Start Stage4
                                        GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 3, false);
                                        GC.MyChar.CPs += 20000;
                                        World.DisCity.PlayerInStage3++;
                                        Game.World.WorldMessage("Server", "No. " + World.DisCity.PlayerInStage3 + "From (50) . Is[" + GC.MyChar.Name + "] From Guild [" + GC.MyChar.MyGuild + "] Has Teleported In To HellCloister.", 2011, 0, System.Drawing.Color.Red);
                                        GC.MyChar.Teleport(2023, 301, 652);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry, But HellCloister Is Full Players"));
                                        GC.SendPacket(Packets.NPCLink("Darn!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Stage4 Final
                        case 9012:
                            {
                                {
                                    if (option == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("Hay Iam A NightDevil Man Iam Here To Get You Out From This Nightmare."));
                                        GC.SendPacket(Packets.NPCLink("Yeah I Get The DBScroll", 1));
                                        GC.SendPacket(Packets.NPCLink("Yeah I Get The DragonToken", 2));
                                        GC.SendPacket(Packets.NPCLink("Iam Scarred I Want Go Home", 3));
                                        GC.SendPacket(Packets.NPCLink("No I Will Stay Here.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 1)
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(720028, 1))
                                        {
                                            GC.MyChar.CPs += 20000;
                                            GC.MyChar.Teleport(1002, 439, 366);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            GC.LocalMessage(2005, System.Drawing.Color.Azure, "Gretz You Got 10k CPs From Joining Last Stage Keep Working.");
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Iam Sorry I Dont Have The DBScroll"));
                                            GC.SendPacket(Packets.NPCLink("Ok this is Okay....", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    if (option == 2)
                                    {
                                        if (GC.MyChar.FindInventoryItemIDAmount(721002, 1))
                                        {
                                            GC.MyChar.CPs += 100000;
                                            GC.MyChar.Teleport(1002, 439, 366);
                                            GC.SendPacket(Packets.NPCText("Here you are."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            GC.LocalMessage(2005, System.Drawing.Color.Azure, "Gretz You Got 100k CPs From Joining Last Stage Keep Working.");
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Iam Sorry I Dont Have The DragonToken"));
                                            GC.SendPacket(Packets.NPCLink("Ok this is Okay....", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    if (option == 3)
                                    {
                                        GC.MyChar.CPs += 1000;
                                        GC.MyChar.Teleport(1002, 439, 366);
                                        GC.SendPacket(Packets.NPCText("Here you are."));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.LocalMessage(2005, System.Drawing.Color.Azure, "Gretz You Got 1k CPs From Joining Last Stage Keep Working.");
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region DragonTorken
                        case 9020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi You.What ever you are. Iam Here to See are you Collect the Two DragonToken?."));
                                    GC.SendPacket(Packets.NPCLink("Yeah I Get The DragonToken", 1));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(721002, 5))
                                    {
                                        GC.MyChar.RemoveItemIDAmount(721002, 5);
                                        GC.MyChar.CreateItemIDAmount(723865, 1);
                                        GC.SendPacket(Packets.NPCText("Thanks You."));
                                        GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.LocalMessage(2005, System.Drawing.Color.Azure, "You Got The +7BlueDragon Succsecful.");
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Iam Sorry You dont have the DragonToken"));
                                        GC.SendPacket(Packets.NPCLink("Ok this is Okay....", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region Welcome Champion
                        case 14000:
                            {
                                if (option == 0)
                                {
                                    {
                                        NPCText("Welocme To Top ChampionWars Center", GC);
                                        NPCLink("Thank You!", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region ClassPK War
                        #region SignUp
                        case 14010:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hellow! We have Class PkWar for different class and the champion will get a nice reward. Will you give it go?", GC);
                                    NPCLink("Tell me the rules", 1, GC);
                                    NPCLink("Sign me up", 3, GC);
                                    NPCLink("Nope", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    NPCText("Okay, All Classes Can Join In ClassPK War Just In Tuesday,", GC);
                                    NPCText(" It`ll Begin At [18:00 - 18:59 Means 6:00 - 6:59 PM]. Talk to me you can Sign up 15 minutes before it starts,", GC);
                                    NPCText(" but after it starts, nobody can enter the arena, And Before the War Ends, the last player in each Arena will be the champion,", GC);
                                    NPCText(" and can claim ExpBalls,CPs and the TopClass.", GC);
                                    NPCLink("Sign me up", 3, GC);
                                    NPCLink("l`ll pass", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 3)
                                {
                                    if (Game.World.ClassPkWar.AddMap.InTimeRange == true)
                                    {
                                        if (Game.World.ClassPkWar.ClassPKTime == true)
                                        {
                                            #region Trojan War
                                            if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.TrojanMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.TrojanMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region Warrior War
                                            else if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.WarriorMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.WarriorMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region Archer War
                                            else if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                            {

                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.ArcherMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.ArcherMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region Ninja War
                                            else if (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.NinjaMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.NinjaMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region Monk War
                                            else if (GC.MyChar.Job >= 60 && GC.MyChar.Job <= 65)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.MonkMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.MonkMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region WaterTaoist War
                                            else if (GC.MyChar.Job >= 130 && GC.MyChar.Job <= 135)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.WaterTaoistMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.WaterTaoistMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region FireTaoist War
                                            else if (GC.MyChar.Job >= 140 && GC.MyChar.Job <= 145)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.FireTaoistMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.FireTaoistMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                            #region DarkTaoist War
                                            else if (GC.MyChar.Job >= 150 && GC.MyChar.Job <= 155)
                                            {
                                                if (GC.MyChar.Level >= 130)
                                                {
                                                    if (!Game.World.ClassPkWar.AddMap.DarkTaoistMap.Contains(GC.MyChar.EntityID))
                                                        Game.World.ClassPkWar.AddMap.DarkTaoistMap.Add(GC.MyChar.EntityID, GC.MyChar);
                                                    GC.MyChar.Teleport(1730, 34, 19);
                                                }
                                                else
                                                {
                                                    NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                    NPCLink("Damt", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            NPCText("The ClassPK War In Thuesday Only", GC);
                                            NPCLink("Okay", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("The ClassPK War Started", GC);
                                        NPCLink("Okay", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Winner
                        case 14011:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hey,You Killed All Here?", GC);
                                    NPCLink("Yes", 1, GC);
                                    NPCLink("No", 255, GC);
                                    NPCLink("Send Me To TwinCity", 4, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 4)
                                {
                                    Game.World.ClassPkWar.RemovePlayersFromWar(GC.MyChar);
                                    GC.MyChar.Teleport(1002, 429, 378);
                                }
                                if (option == 1)
                                {
                                    if (Game.World.ClassPkWar.AddMap.timer == false)
                                    {
                                        #region Trojan Reward
                                        if (GC.MyChar.Job >= 10 && GC.MyChar.Job <= 15)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.TrojanMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.TrojanMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.TrojanMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopTrojan = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopTrojan);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class Trojan War [4 ExpBalls, 500.000 CPs and TopTrojan].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region Warrior Reward
                                        if (GC.MyChar.Job >= 20 && GC.MyChar.Job <= 25)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.WarriorMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.WarriorMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.WarriorMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopWarrior = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWarrior);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class Warrior War [4 ExpBalls, 500.000 CPs and TopWarrior].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region Archer Reward
                                        if (GC.MyChar.Job >= 40 && GC.MyChar.Job <= 45)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.ArcherMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.ArcherMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.ArcherMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopArcher = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopArcher);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class Archer War [4 ExpBalls, 500.000 CPs and TopArcher].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region Ninja Reward
                                        if (GC.MyChar.Job >= 50 && GC.MyChar.Job <= 55)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.NinjaMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.NinjaMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.NinjaMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopNinja = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopNinja);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class Ninja War [4 ExpBalls, 500.000 CPs and TopNinja].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region Monk Reward
                                        /*if (GC.MyChar.Job >= 60 && GC.MyChar.Job <= 65)
                                            {
                                                if (Game.World.ClassPkWar.AddMap.MonkMap.Contains(GC.MyChar.EntityID))
                                                {
                                                    if (Game.World.ClassPkWar.AddMap.MonkMap.Count == 1)
                                                    {
                                                        Game.World.ClassPkWar.AddMap.MonkMap.Remove(GC.MyChar.EntityID);
                                                        GC.MyChar.Teleport(1002, 429, 378);
                                                        GC.MyChar.IncreaseExp(GC.MyChar.ExpBallExp * 4, false);
                                                        GC.MyChar.TopMonk = 1;
                                                        GC.MyChar.CPs += 500000;
                                                        GC.MyChar.AddEffect2(Server.Game.StatusEffectEn2.TopMonk);
                                                        GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class Monk War [4 ExpBalls, 500.000 CPs and TopMonk].");
                                                    }
                                                    else
                                                    {
                                                        OptionText("Sorry, But Its Players In Map", GC);
                                                        OptionLink("Ah,my bad", 255, GC);
                                                        GC.SendPacket(Packets.NPCSetFace(30));
                                                        NPCFinish(GC);
                                                    }
                                                }
                                            }*/
                                        #endregion
                                        #region WaterTaoist Reward
                                        if (GC.MyChar.Job >= 130 && GC.MyChar.Job <= 135)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.WaterTaoistMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.WaterTaoistMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.WaterTaoistMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopWaterTaoist = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWaterTaoist);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class WaterTaoist War [4 ExpBalls, 500.000 CPs and TopWaterTaoist].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region FireTaoist Reward
                                        if (GC.MyChar.Job >= 140 && GC.MyChar.Job <= 145)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.FireTaoistMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.FireTaoistMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.FireTaoistMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopFireTaoist = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopFireTaoist);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class FireTaoist War [4 ExpBalls, 500.000 CPs and TopFireTaoist].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region DarkTaoist Reward
                                        if (GC.MyChar.Job >= 150 && GC.MyChar.Job <= 155)
                                        {
                                            if (Game.World.ClassPkWar.AddMap.DarkTaoistMap.Contains(GC.MyChar.EntityID))
                                            {
                                                if (Game.World.ClassPkWar.AddMap.DarkTaoistMap.Count == 1)
                                                {
                                                    Game.World.ClassPkWar.AddMap.DarkTaoistMap.Remove(GC.MyChar.EntityID);
                                                    GC.MyChar.Teleport(1002, 429, 378);
                                                    GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 4, false);
                                                    GC.MyChar.TopFireTaoist = 1;
                                                    GC.MyChar.CPs += 500000;
                                                    GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopFireTaoist);
                                                    GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " Has Win Class DarkTaoist War [4 ExpBalls, 500.000 CPs and TopDarkTaoist].");
                                                }
                                                else
                                                {
                                                    NPCText("Sorry, But Its Players In Map", GC);
                                                    NPCLink("Ah,my bad", 255, GC);
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        NPCText("Sorry, But Tournment Is Already On.", GC);
                                        NPCLink("Oh, Sorry", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #endregion
                        #region Weekly/Monthly PKChampion
                        #region SignUp
                        case 14020:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hellow! We have Weekly/Monthly PKChampion for All class and the champion will get you a nice reward. Will you give it go?", GC);
                                    NPCLink("Tell me the rules", 1, GC);
                                    NPCLink("Sign me up", 3, GC);
                                    NPCLink("Nope", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    NPCText("Okay, The Weekly PKChampion Every Friday -And- The Monthly PKChampion In The First Day Every Month ", GC);
                                    NPCText(" It`ll take place for [WeeklyPK At 20:00 - 20:59 /MonthlyPK At 22:00 - 22:59]. Talk to me you sign up 15 minutes before it starts", GC);
                                    NPCLink("Sign me up", 3, GC);
                                    NPCLink("l`ll pass", 255, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 3)
                                {
                                    #region WeeklyPK War
                                    if (Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange == true)
                                    {
                                        if (Game.World.WeeklyAndMonthlyPkWar.WeeklyWar == true)
                                        {
                                            if (GC.MyChar.Level >= 130)
                                            {
                                                if (!Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Contains(GC.MyChar.EntityID))
                                                    Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Add(GC.MyChar.EntityID, GC.MyChar);
                                                GC.MyChar.Teleport(1730, 34, 19);
                                            }
                                            else
                                            {
                                                NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                NPCLink("Damt", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            NPCText("The TopWeeklyPK War War is on Friday only", GC);
                                            NPCLink("Okay", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("The TopWeeklyPK War Started", GC);
                                        NPCLink("Okay", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #endregion
                                    #region MonthlyPK War
                                    if (Game.World.WeeklyAndMonthlyPkWar.AddMap.InTimeRange == true)
                                    {
                                        if (Game.World.WeeklyAndMonthlyPkWar.MonthlyWar == true)
                                        {
                                            if (GC.MyChar.Level >= 130)
                                            {
                                                if (!Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Contains(GC.MyChar.EntityID))
                                                    Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Add(GC.MyChar.EntityID, GC.MyChar);
                                                GC.MyChar.Teleport(1730, 34, 19);
                                            }
                                            else
                                            {
                                                NPCText("O.o You Must Reach Level 130+ To join, I am Sorry", GC);
                                                NPCLink("Damt", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        else
                                        {
                                            NPCText("The TopMonthlyPK War Is In The First Day Of Every Month", GC);
                                            NPCLink("Okay", 255, GC);
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        NPCText("The TopMonthlyPK War Started", GC);
                                        NPCLink("Okay", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    #endregion
                                }
                                break;
                            }
                        #endregion
                        #region Winner
                        case 14021:
                            {
                                if (option == 0)
                                {
                                    NPCText("Hey,You Killed All Here?", GC);
                                    NPCLink("Yes", 1, GC);
                                    NPCLink("No", 255, GC);
                                    NPCLink("Send Me To TwinCity", 4, GC);
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 4)
                                {
                                    Game.World.WeeklyAndMonthlyPkWar.RemovePlayersFromWar(GC.MyChar);
                                    GC.MyChar.Teleport(1002, 429, 378);
                                }
                                if (option == 1)
                                {
                                    if (Game.World.WeeklyAndMonthlyPkWar.AddMap.timer == false)
                                    {
                                        #region WeeklyPK Reward
                                        if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Contains(GC.MyChar.EntityID))
                                        {
                                            if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Count == 1)
                                            {
                                                Game.World.WeeklyAndMonthlyPkWar.AddMap.TopWeeklyPK.Remove(GC.MyChar.EntityID);
                                                GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 8, false);
                                                GC.MyChar.TopWeekly = 1;
                                                GC.MyChar.CPs += 1000000;
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopWeekly);
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " has win Class Trojan War (Level 1-99) and win [2 ExpBalls, 100.000 CPs and TopTrojan].");
                                                GC.MyChar.Teleport(1002, 429, 378);
                                            }
                                            else
                                            {
                                                NPCText("Sorry, but its Players on map", GC);
                                                NPCLink("Ah,my bad", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        #endregion
                                        #region MonthlyPK Reward
                                        if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Contains(GC.MyChar.EntityID))
                                        {
                                            if (Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Count == 1)
                                            {
                                                Game.World.WeeklyAndMonthlyPkWar.AddMap.TopMonthlyPK.Remove(GC.MyChar.EntityID);

                                                GC.MyChar.IncreaseExp(GC.MyChar.EXPBall * 10, false);
                                                GC.MyChar.TopMonthly = 1;
                                                GC.MyChar.CPs += 5000000;
                                                GC.MyChar.StatEff.Add(Server.Game.StatusEffectEn.TopMonthly);
                                                GC.LocalMessage(2005, System.Drawing.Color.Red, "Congratulations! " + GC.MyChar.Name + " has win Class Trojan War (Level 1-99) and win [2 ExpBalls, 100.000 CPs and TopTrojan].");
                                                GC.MyChar.Teleport(1002, 429, 378);
                                            }
                                            else
                                            {
                                                NPCText("Sorry, but its Players on map", GC);
                                                NPCLink("Ah,my bad", 255, GC);
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        NPCText("Sorry, But Tournment Is Already On.", GC);
                                        NPCLink("Oh, Sorry", 255, GC);
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #endregion

                        #region Enter DarkDengun
                        case 15000:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi DarkDaingun Aplace where Melee Power is Prefared TO guther the powerfull monsters ther Teleport fee is 20K CPs"));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me", 1));
                                    GC.SendPacket(Packets.NPCLink("Change My Tokens", 2));
                                    GC.SendPacket(Packets.NPCLink("No Thanks", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Job == 65 || GC.MyChar.Job == 15 || GC.MyChar.Job == 25 || GC.MyChar.Job == 55 || GC.MyChar.Job == 135)
                                    {
                                        if (GC.MyChar.CPs >= 10000)
                                        {
                                            GC.MyChar.CPs -= 10000;
                                            GC.MyChar.Teleport(1700, 876, 450);
                                            GC.SendPacket(Packets.NPCLink("GoodLuck And Becarful", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Oh Bad You Dont Have Enough CPs Man Oh Damt"));
                                            GC.SendPacket(Packets.NPCLink("Oh Iam Poor Sorry!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job == 145 || GC.MyChar.Job == 45)
                                    {
                                        if (GC.MyChar.CPs >= 10000)
                                        {
                                            GC.MyChar.CPs -= 10000;
                                            GC.MyChar.Teleport(1700, 441, 886);
                                            GC.SendPacket(Packets.NPCLink("GoodLuck And Becarful", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("Oh Bad You Dont Have Enough CPs Man Oh Damt"));
                                            GC.SendPacket(Packets.NPCLink("Oh Iam Poor Sorry!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("this area exclosive for the full promoted Knights"));
                                        GC.SendPacket(Packets.NPCLink("Oh Iam Poor Sorry!", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 2)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi You Want Your Prize Okay Okay Sellect What You Did In The DarkDaingun!"));
                                    GC.SendPacket(Packets.NPCLink("I Have Tokens", 3));
                                    GC.SendPacket(Packets.NPCLink("I Have AxeOrc`s Heart", 4));
                                    GC.SendPacket(Packets.NPCLink("I Have Zombe`s Heart", 5));
                                    GC.SendPacket(Packets.NPCLink("Iam Poor", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("Change 1 Token For 10,000 CPs/Change 5 Token For 60,000 CPs/Change 10 Token For 140,000 CPs!"));
                                    GC.SendPacket(Packets.NPCLink("Okay Change It", 6));
                                    GC.SendPacket(Packets.NPCLink("Iam Poor", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 6)
                                {
                                    GC.SendPacket(Packets.NPCText("Change 1 Token For 10,000 CPs/Change 5 Token For 60,000 CPs/Change 10 Token For 140,000 CPs!"));

                                    if (GC.MyChar.FindInventoryItemIDAmount(710034, 10))
                                    {
                                        GC.MyChar.CPs += 140000;
                                        GC.MyChar.RemoveItemIDAmount(710034, 10);
                                    }
                                    else if (GC.MyChar.FindInventoryItemIDAmount(710034, 5))
                                    {
                                        GC.MyChar.CPs += 60000;
                                        GC.MyChar.RemoveItemIDAmount(710034, 5);
                                    }
                                    else if (GC.MyChar.FindInventoryItemIDAmount(710034, 1))
                                    {
                                        GC.MyChar.CPs += 10000;
                                        GC.MyChar.RemoveItemIDAmount(710034, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have Tokens Yet.."));
                                        GC.SendPacket(Packets.NPCLink("Okay Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                if (option == 4)
                                {
                                    GC.SendPacket(Packets.NPCText("Change  1 AxeOrc`s Heart For 200,000 CPs!"));
                                    if (GC.MyChar.FindInventoryItemIDAmount(710038, 1))
                                    {
                                        GC.MyChar.CPs += 200000;
                                        GC.MyChar.RemoveItemIDAmount(710038, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have Tokens Yet.."));
                                        GC.SendPacket(Packets.NPCLink("Okay Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 5)
                                {
                                    GC.SendPacket(Packets.NPCText("Change  1 Zombe`s Heart For 200,000 CPs!"));
                                    if (GC.MyChar.FindInventoryItemIDAmount(710037, 1))
                                    {
                                        GC.MyChar.CPs += 200000;
                                        GC.MyChar.RemoveItemIDAmount(710037, 1);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have Tokens Yet.."));
                                        GC.SendPacket(Packets.NPCLink("Okay Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region WelcomeGate
                        case 15010:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("THis is a verey big area so we are here working for you because we know that your missin is not easey i will teleport you for your dastnation but you have to pay our fees weach is 1kk sivers"));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me To Center Of This Map", 1));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me To the top of map", 10));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me To the desert city", 3));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("u will pay 1kk silvers"));
                                    GC.SendPacket(Packets.NPCLink("ok i will pay", 2));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Silvers >= 1000000)
                                    {
                                        GC.MyChar.Silvers -= 1000000;
                                        GC.MyChar.Teleport(1700, 621, 643);
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 3)
                                {
                                    GC.SendPacket(Packets.NPCText("u will pay 1k CPs"));
                                    GC.SendPacket(Packets.NPCLink("ok i will pay", 4));
                                    GC.SendPacket(Packets.NPCLink("I Will Use teleport scroll", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 4)
                                {
                                    if (GC.MyChar.CPs >= 1000)
                                    {
                                        GC.MyChar.CPs -= 1000;
                                        GC.MyChar.Teleport(1000, 479, 643);
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("u will pay 1.5kk silvers"));
                                    GC.SendPacket(Packets.NPCLink("ok i will pay", 11));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 11)
                                {
                                    if (GC.MyChar.Silvers >= 1500000)
                                    {
                                        GC.MyChar.Silvers -= 1500000;
                                        GC.MyChar.Teleport(1700, 304, 314);
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region WelcomeGate
                        case 15020:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("THis is a verey big area so we are here working for you because we know that your missin is not easey i will teleport you for your dastnation but you have to pay our fees weach is 1kk sivers"));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me Center Of This Map", 1));
                                    GC.SendPacket(Packets.NPCLink("Teleport Me To the top of map", 10));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("u will pay 1kk silvers"));
                                    GC.SendPacket(Packets.NPCLink("ok i will pay", 2));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Silvers >= 1000000)
                                    {
                                        GC.MyChar.Silvers -= 1000000;
                                        GC.MyChar.Teleport(1700, 621, 643);
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 10)
                                {
                                    GC.SendPacket(Packets.NPCText("u will pay 1.5kk silvers"));
                                    GC.SendPacket(Packets.NPCLink("ok i will pay", 11));
                                    GC.SendPacket(Packets.NPCLink("I Will Go Wallking", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 11)
                                {
                                    if (GC.MyChar.Silvers >= 1500000)
                                    {
                                        GC.MyChar.Silvers -= 1500000;
                                        GC.MyChar.Teleport(1700, 621, 643);
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Thanks For Using easy way"));
                                        GC.SendPacket(Packets.NPCLink("NP It Is Okay", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region DragonMap
                        case 15030:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi " + GC.MyChar.Name + " Iam There To Guild You To Enter The DragonMap."));
                                    GC.SendPacket(Packets.NPCLink("I Want To Enter The DragonMap.", 1));
                                    GC.SendPacket(Packets.NPCLink("Thanks Iam Not Intersted.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                    GC.MyChar.Teleport(1013, 50, 50);
                                break;
                            }
                        #endregion
                        #region DengunQuests

                        case 15500:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Here Is Main Center Of Faire The UnderWorled Dengun i dont sudgest to go it is not easy there its a risk .."));
                                    GC.SendPacket(Packets.NPCLink("Let Me Start The Jurney", 1));
                                    //GC.SendPacket(Packets.NPCLink(".", 60));
                                    GC.SendPacket(Packets.NPCLink("Iam afraid.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Do You Have The DeadStone."));
                                    GC.SendPacket(Packets.NPCLink("Yes i have it", 2));
                                    GC.SendPacket(Packets.NPCLink("Iam afraid.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {
                                    if (GC.MyChar.MyTeam != null && GC.MyChar.MyTeam.Members.Count > 1)
                                    {

                                        foreach (Game.Character Member in GC.MyChar.MyTeam.Members)
                                            if (Member.EntityID != GC.MyChar.EntityID)
                                            {
                                                if (Member != null && Member.MyClient != null && Member.Loc.Map == 1996)
                                                {
                                                    GC.SendPacket(Packets.NPCText("i have found member team in the Dengun .. do you want to join him."));
                                                    GC.SendPacket(Packets.NPCLink("yeas i will join with him", 3));
                                                    GC.SendPacket(Packets.NPCLink("i will take a privat Dengun.", 4));
                                                    GC.SendPacket(Packets.NPCLink("iam not going any where.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                    return;
                                                }
                                                GC.DialogNPC = 15500;
                                                PacketHandling.NPCDialog.Handles(GC, null, GC.DialogNPC, 4);
                                            }
                                    }
                                    else
                                    {
                                        GC.DialogNPC = 15500;
                                        PacketHandling.NPCDialog.Handles(GC, null, GC.DialogNPC, 4);
                                    }

                                }
                                else if (option == 3)
                                {
                                    if (GC.MyChar.MyTeam != null)
                                    {
                                        if (GC.MyChar.MyTeam.Members.Count > 1)
                                            foreach (Game.Character Member in GC.MyChar.MyTeam.Members)
                                            {
                                                if (Member != null && Member.MyClient != null && Member.Loc.Map == 1996)
                                                {
                                                    byte errortype = 0;
                                                    if (Member.myDengun1.joinNewMem(GC.MyChar, out errortype))
                                                    {
                                                        GC.MyChar.myDengun1 = Member.myDengun1;
                                                        GC.SendPacket(Packets.NPCText("i have joined the Dengun with your team member go ant assist each other."));
                                                        GC.SendPacket(Packets.NPCLink("lets start.", 255));
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);

                                                    }
                                                    else
                                                    {
                                                        if (errortype == 1)
                                                        {
                                                            GC.SendPacket(Packets.NPCText("you dont have the required item DeadStone."));
                                                            GC.SendPacket(Packets.NPCLink("oh sorry.", 255));
                                                        }
                                                        else if (errortype == 2)
                                                        {
                                                            GC.SendPacket(Packets.NPCText("there are no room left for you."));
                                                            GC.SendPacket(Packets.NPCLink("oh oky.", 255));
                                                        }
                                                        NPCFace(N.Avatar, GC);
                                                        NPCFinish(GC);
                                                    }
                                                    break;
                                                }
                                            }
                                    }
                                }
                                else if (option == 4)
                                {
                                    //if (true)
                                    //{
                                        //GC.MyChar.RemoveItem(id, 1);
                                        GC.MyChar.myDengun1 = new Extra.Dengun(GC.MyChar);
                                    //}
                                    //else
                                    //{
                                    //    GC.SendPacket(Packets.NPCSay("You Do Not Have The DeadStone."));
                                    //    GC.SendPacket(Packets.NPCLink("Sorry.", 255));
                                    //    GC.SendPacket(Packets.NPCSetFace(40));
                                    //    NPCFinish(GC);
                                    //}
                                }

                            }
                            break;
                        //dengun 1 passer
                        case 15510:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("the second etration of the Dengun, you need to kill [PossOfTheDead]."));
                                    GC.SendPacket(Packets.NPCLink("i have killed the poss. ", 1));
                                    GC.SendPacket(Packets.NPCLink("i dont know what i have to do.", 60));
                                    GC.SendPacket(Packets.NPCLink("pass by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.myDengun1.firstPossKilled == true)
                                    {

                                    }
                                }
                                else if (option == 2)
                                {

                                }
                                else if (option == 60)
                                {

                                    GC.SendPacket(Packets.NPCText("first you have to kill 15 monster of each type to wakeup the four Posses then if you defeated them the [LeaderOfDead] will apear u have to defeat hom to pass this stage ."));
                                    GC.SendPacket(Packets.NPCLink("what is my score for now. ", 61));
                                    // GC.SendPacket(Packets.NPCLink("i dont know what i have to do.", 60));
                                    GC.SendPacket(Packets.NPCLink("pass by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 61)
                                {

                                    GC.SendPacket(Packets.NPCText("undeadspearman killed " + GC.MyChar.myDengun1.UndeadSpearman + " ,undeadsoldier killed " + GC.MyChar.myDengun1.UndeadSoldier + " ,Hilltroll killed " + GC.MyChar.myDengun1.HellTroll + " ,Centicore killed " + GC.MyChar.myDengun1.Centicore + " ,quests Done " + GC.MyChar.myDengun1.TheFourPossMobsKilled + " ."));
                                    GC.SendPacket(Packets.NPCLink("what is my score for now. ", 61));
                                    // GC.SendPacket(Packets.NPCLink("i dont know what i have to do.", 60));
                                    GC.SendPacket(Packets.NPCLink("pass by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }

                            }
                            break;

                        #endregion

                        #region Avatar NPC`s
                        #region AvatarGod
                        case 30:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi, I am AvatarGod and i teach only Benders, the rest of you, beat it!"));
                                    GC.SendPacket(Packets.NPCLink("Get Promoted.", 1));
                                    GC.SendPacket(Packets.NPCLink("Just passing by.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Job == 160)
                                    {
                                        if (GC.MyChar.Level >= 10)
                                        {
                                            GC.MyChar.Job++;
                                            GC.SendPacket(Packets.NPCText("Congratulaion You Are Now AirBender."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Must To Rich Level [10] To Get Promoting."));
                                            GC.SendPacket(Packets.NPCLink("Ok Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job == 161)
                                    {
                                        if (GC.MyChar.Level >= 40)
                                        {
                                            GC.MyChar.Job++;
                                            GC.SendPacket(Packets.NPCText("Congratulaion You Are Now WaterBender."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Must To Rich Level [40] To Get Promoting."));
                                            GC.SendPacket(Packets.NPCLink("Ok Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job == 162)
                                    {
                                        if (GC.MyChar.Level >= 70)
                                        {
                                            GC.MyChar.Job++;
                                            GC.SendPacket(Packets.NPCText("Congratulaion You Are Now EarthBender."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Must To Rich Level [70] To Get Promoting."));
                                            GC.SendPacket(Packets.NPCLink("Ok Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job == 163)
                                    {
                                        if (GC.MyChar.Level >= 100)
                                        {
                                            GC.MyChar.Job++;
                                            GC.SendPacket(Packets.NPCText("Congratulaion You Are Now FireBender."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Must To Rich Level [100] To Get Promoting."));
                                            GC.SendPacket(Packets.NPCLink("Ok Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else if (GC.MyChar.Job == 164)
                                    {
                                        if (GC.MyChar.Level >= 120)
                                        {
                                            GC.MyChar.Job++;
                                            GC.SendPacket(Packets.NPCText("Congratulaion You Are Now The Avatar."));
                                            GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("You Must To Rich Level [120] To Get Promoting."));
                                            GC.SendPacket(Packets.NPCLink("Ok Sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("You are already Completly Promotted Now, i cannot promote you anymore."));
                                    GC.SendPacket(Packets.NPCLink("I see.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                break;
                            }
                        #endregion
                        #region TheMarket
                        case 200021:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi I am The MarketMan Be Ready For Teleporting, it is just for 1000 Silver!"));
                                    GC.SendPacket(Packets.NPCLink("To Market", 1));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 1)
                                {
                                    if (GC.MyChar.Silvers >= 1000)
                                    {
                                        GC.MyChar.Silvers -= 1000;
                                        GC.MyChar.Teleport(1036, 195, 213);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry you Dont Have Enough Silver Yet."));
                                        GC.SendPacket(Packets.NPCLink("Ok Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Pk.Area
                        case 200031:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi There is Place Here To Fight With Others Just Pay 1000 Silver to Enter The Pk.Area."));
                                    GC.SendPacket(Packets.NPCLink("Enter Me", 1));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Silvers >= 1000)
                                    {
                                        GC.MyChar.Silvers -= 1000;
                                        GC.MyChar.Teleport(1021, 50, 50);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry you Dont Have Enough Silver Yet."));
                                        GC.SendPacket(Packets.NPCLink("Ok Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Mr & DB For CPs
                        case 200032:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hello, I can trade with you 15CPs for a Meteor, 215CPs for a DragonBall and 2150CPs For DBScroll."));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my Meteor!", 1));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my DragonBall!", 2));
                                    GC.SendPacket(Packets.NPCLink("Here, Take my DBScroll!", 3));
                                    GC.SendPacket(Packets.NPCLink("Just Passing By.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088000, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 1088000)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 215;
                                            GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                        }
                                    }
                                }
                                if (option == 2)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(1088001, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 1088001)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 15;
                                            GC.MyChar.RemoveItemIDAmount(1088001, 1);
                                        }
                                    }
                                }
                                if (option == 3)
                                {
                                    if (GC.MyChar.FindInventoryItemIDAmount(720028, 1))
                                    {
                                        Game.Item DB = null;
                                        foreach (Game.Item I in GC.MyChar.Inventory.Values)
                                            if (I.ID == 720028)
                                            { DB = I; break; }
                                        if (DB != null)
                                        {
                                            GC.MyChar.CPs += 2150;
                                            GC.MyChar.RemoveItemIDAmount(720028, 1);
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Hair.Service
                        case 200028:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Would you like to change your hairstyle? I can offer you a change for 500 silvers. You can choose from the styles below."));
                                    GC.SendPacket(Packets.NPCLink("New Styles", 1));
                                    GC.SendPacket(Packets.NPCLink("Nostalgic Styles", 2));
                                    GC.SendPacket(Packets.NPCLink("Special Styles", 3));
                                    GC.SendPacket(Packets.NPCLink("No thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                    GC.Paid = false;
                                }
                                #region New Styles
                                else if (option == 1)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle01", 10));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle02", 11));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle03", 12));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle04", 13));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle05", 14));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle06", 15));
                                        GC.SendPacket(Packets.NPCLink("New HairStyle07", 16));
                                        GC.SendPacket(Packets.NPCLink("Next", 100));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 100 && GC.Paid)
                                {
                                    GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                    GC.SendPacket(Packets.NPCLink("New HairStyle08", 17));
                                    GC.SendPacket(Packets.NPCLink("New HairStyle09", 18));
                                    GC.SendPacket(Packets.NPCLink("New HairStyle10", 19));
                                    GC.SendPacket(Packets.NPCLink("New HairStyle11", 20));
                                    GC.SendPacket(Packets.NPCLink("New HairStyle12", 21));
                                    GC.SendPacket(Packets.NPCLink("Previous", 1));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 10 && option <= 21 && GC.Paid)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 1));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (20 + option).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (20 + option).ToString());
                                }
                                #endregion
                                #region Nostalgic Styles
                                else if (option == 2)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic01", 30));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic02", 31));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic03", 32));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic04", 33));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic05", 34));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic06", 35));
                                        GC.SendPacket(Packets.NPCLink("Nostalgic07", 36));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option >= 30 && option <= 36)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 2));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 20).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 20).ToString());
                                }
                                #endregion
                                #region Special Styles
                                else if (option == 3)
                                {
                                    if (GC.MyChar.Silvers >= 500 || GC.Paid)
                                    {
                                        if (!GC.Paid)
                                        {
                                            GC.Paid = true;
                                            GC.MyChar.Silvers -= 500;
                                        }
                                        GC.Agreed = false;
                                        GC.MyChar.Hair = GC.MyChar.Hair;
                                        GC.SendPacket(Packets.NPCText("Choose the style you like the best."));
                                        GC.SendPacket(Packets.NPCLink("Special01", 40));
                                        GC.SendPacket(Packets.NPCLink("Special02", 41));
                                        GC.SendPacket(Packets.NPCLink("Special03", 42));
                                        GC.SendPacket(Packets.NPCLink("Special04", 43));
                                        GC.SendPacket(Packets.NPCLink("Special05", 44));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("500 silvers isn't that expensive. Come again when you have that money with you."));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option >= 40 && option <= 44)
                                {
                                    if (!GC.Agreed)
                                    {
                                        GC.Agreed = true;
                                        GC.SendPacket(Packets.NPCText("So, do you like it? Or do you want me to change it back?"));
                                        GC.SendPacket(Packets.NPCLink("Yes, I like it.", option));
                                        GC.SendPacket(Packets.NPCLink("No, it's awful! Change it back.", 3));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        GC.SendPacket(Packets.Status(GC.MyChar.EntityID, Game.Status.Hair, ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 19).ToString())));
                                    }
                                    else
                                        GC.MyChar.Hair = ushort.Parse(Convert.ToString(GC.MyChar.Hair)[0] + (option - 19).ToString());
                                }
                                #endregion
                                break;
                            }
                        #endregion
                        #region Upgrade Gears
                        case 200022:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Hi. Iam The One Who Upgrade Avatar Gears From [Lvl 10 To Lvl 130].So Sellect Your Type?"));
                                    GC.SendPacket(Packets.NPCLink(">> Upgrade Gears", 1));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("Welcome To Me Hehehe Now If You Want Upgrade Your Gears You Must Pay [1 DragonBall]"));
                                    GC.SendPacket(Packets.NPCLink("> Headgear", (byte)(option * 100 + 1)));
                                    GC.SendPacket(Packets.NPCLink("> Necklace", (byte)(option * 100 + 2)));
                                    GC.SendPacket(Packets.NPCLink("> Armor", (byte)(option * 100 + 3)));
                                    GC.SendPacket(Packets.NPCLink("> RightWeapon", (byte)(option * 100 + 4)));
                                    GC.SendPacket(Packets.NPCLink("> LeftWeapon", (byte)(option * 100 + 5)));
                                    GC.SendPacket(Packets.NPCLink("> Ring", (byte)(option * 100 + 6)));
                                    GC.SendPacket(Packets.NPCLink("> Boots", (byte)(option * 100 + 8)));
                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                #region AvatarCoronet
                                else if (option == 101)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    //Start For Level 10
                                    if (I.ID == 117009)
                                        newID = 117019; // Level 40
                                    else if (I.ID == 117019)
                                        newID = 117029; // Level 70 
                                    else if (I.ID == 117029)
                                        newID = 117039; // Level 100
                                    else if (I.ID == 117039)
                                        newID = 117049; // Level 120
                                    else if (I.ID == 117049)
                                        newID = 117129; // Level 130

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region AvatarNecklace
                                else if (option == 102)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    //Start For Level 10
                                    if (I.ID == 120009)
                                        newID = 120029; // Level 40
                                    else if (I.ID == 120029)
                                        newID = 120049; // Level 70 
                                    else if (I.ID == 120049)
                                        newID = 120069; // Level 100
                                    else if (I.ID == 120069)
                                        newID = 120089; // Level 110
                                    else if (I.ID == 120089)
                                        newID = 120099; // Level 120
                                    else if (I.ID == 120099)
                                        newID = 120109; // Level 130

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region AvatarGown
                                else if (option == 103)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    //Start For Level 10
                                    if (I.ID == 134009)
                                        newID = 134019; // Level 40
                                    else if (I.ID == 134019)
                                        newID = 134029; // Level 70 
                                    else if (I.ID == 134029)
                                        newID = 133009; // Level 100
                                    else if (I.ID == 133009)
                                        newID = 133019; // Level 120
                                    else if (I.ID == 133019)
                                        newID = 133029; // Level 130

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region Avatar RightWeapon
                                else if (option == 104)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    #region LiteAxe
                                    //Start For Level 10
                                    if (I.ID == 430199)
                                        newID = 430209; // Level 40
                                    else if (I.ID == 430209)
                                        newID = 430219; // Level 70 
                                    else if (I.ID == 430219)
                                        newID = 430229; // Level 100
                                    else if (I.ID == 430229)
                                        newID = 430239; // Level 120
                                    else if (I.ID == 430239)
                                        newID = 430249; // Level 121
                                    else if (I.ID == 430249)
                                        newID = 430259; // Level 122
                                    else if (I.ID == 430259)
                                        newID = 430269; // Level 123
                                    else if (I.ID == 430269)
                                        newID = 430279; // Level 124
                                    else if (I.ID == 430279)
                                        newID = 430289; // Level 125
                                    else if (I.ID == 430289)
                                        newID = 430299; // Level 126
                                    else if (I.ID == 430299)
                                        newID = 430309; // Level 127
                                    else if (I.ID == 430309)
                                        newID = 430319; // Level 128
                                    else if (I.ID == 430319)
                                        newID = 430329; // Level 129
                                    else if (I.ID == 430329)
                                        newID = 430339; // Level 130
                                    #endregion

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region Avatar LeftWeapon
                                else if (option == 105)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    #region LiteAxe
                                    //Start For Level 10
                                    if (I.ID == 430199)
                                        newID = 430209; // Level 40
                                    else if (I.ID == 430209)
                                        newID = 430219; // Level 70 
                                    else if (I.ID == 430219)
                                        newID = 430229; // Level 100
                                    else if (I.ID == 430229)
                                        newID = 430239; // Level 120
                                    else if (I.ID == 430239)
                                        newID = 430249; // Level 121
                                    else if (I.ID == 430249)
                                        newID = 430259; // Level 122
                                    else if (I.ID == 430259)
                                        newID = 430269; // Level 123
                                    else if (I.ID == 430269)
                                        newID = 430279; // Level 124
                                    else if (I.ID == 430279)
                                        newID = 430289; // Level 125
                                    else if (I.ID == 430289)
                                        newID = 430299; // Level 126
                                    else if (I.ID == 430299)
                                        newID = 430309; // Level 127
                                    else if (I.ID == 430309)
                                        newID = 430319; // Level 128
                                    else if (I.ID == 430319)
                                        newID = 430329; // Level 129
                                    else if (I.ID == 430329)
                                        newID = 430339; // Level 130
                                    #endregion

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region AvatarRing
                                else if (option == 106)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    //Start For Level 10
                                    if (I.ID == 150009)
                                        newID = 150019; // Level 40
                                    else if (I.ID == 150019)
                                        newID = 150039; // Level 70 
                                    else if (I.ID == 150039)
                                        newID = 150059; // Level 100
                                    else if (I.ID == 150059)
                                        newID = 150079; // Level 110
                                    else if (I.ID == 150079)
                                        newID = 150099; // Level 120
                                    else if (I.ID == 150099)
                                        newID = 150109; // Level 130

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                #region AvatarBoots
                                else if (option == 108)
                                {
                                    Game.Item I = GC.MyChar.Equips.Get((byte)(option - 100));
                                    byte PrevLevel = I.ItemDBInfo.Level;
                                    uint newID = 0;

                                    //Start For Level 10
                                    if (I.ID == 160019)
                                        newID = 160039; // Level 40
                                    else if (I.ID == 160039)
                                        newID = 160059; // Level 70 
                                    else if (I.ID == 160059)
                                        newID = 160079; // Level 100
                                    else if (I.ID == 160079)
                                        newID = 160099; // Level 110
                                    else if (I.ID == 160099)
                                        newID = 160119; // Level 120
                                    else if (I.ID == 160119)
                                        newID = 160129; // Level 130

                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("Sorry But I Cant Upgrade This Item."));
                                        GC.SendPacket(Packets.NPCLink("AllRight.Sorry", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (!Database.DatabaseItems.ContainsKey(newID))
                                        return;
                                    DatabaseItem Ii = (DatabaseItem)Database.DatabaseItems[newID];
                                    byte NewLevel = Ii.Level;
                                    if (GC.MyChar.Level >= NewLevel)
                                    {
                                        if (NewLevel != 0 && NewLevel <= 130 && PrevLevel < 130)
                                        {
                                            byte DBsReq = 1;
                                            if (!GC.Agreed)
                                            {
                                                GC.SendPacket(Packets.NPCText("You need " + DBsReq + " DragonBalls to upgrade."));
                                                GC.SendPacket(Packets.NPCLink("Upgrade it.", option));
                                                GC.SendPacket(Packets.NPCLink("Forget it.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                GC.Agreed = true;
                                            }
                                            else
                                            {
                                                GC.Agreed = false;
                                                if (GC.MyChar.FindInventoryItemIDAmount(1088000, DBsReq))
                                                {
                                                    GC.MyChar.EquipStats((byte)(option - 100), false);
                                                    for (byte i = 0; i < DBsReq; i++)
                                                        GC.MyChar.RemoveItemIDAmount(1088000, 1);
                                                    I.ID = newID;
                                                    GC.MyChar.Equips.Replace((byte)(option - 100), I, GC.MyChar);
                                                    GC.MyChar.EquipStats((byte)(option - 100), true);

                                                    GC.SendPacket(Packets.NPCText("Here you are. It's done."));
                                                    GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                                else
                                                {
                                                    GC.SendPacket(Packets.NPCText("You don't have enough DragonBalls."));
                                                    GC.SendPacket(Packets.NPCLink("No way! Are you really sure?", 255));
                                                    NPCFace(N.Avatar, GC);
                                                    NPCFinish(GC);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("I'm afraid i can't help you with that. I am not experienced enough with equipment that high level."));
                                            GC.SendPacket(Packets.NPCLink("You old geezer!", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You aren't high level enough to wear the item after upgrading."));
                                        GC.SendPacket(Packets.NPCLink("Alright.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                #endregion
                                break;
                            }
                        #endregion
                        #region Summon Avatar
                        case 1020300:
                            {
                                if (option == 0)
                                {
                                    GC.SendPacket(Packets.NPCText("Avatar summoning And Controling. Your avatar Control Level is " + GC.MyChar.AvatarLevel));
                                    GC.SendPacket(Packets.NPCLink("put new ava account", 1));
                                    GC.SendPacket(Packets.NPCLink("Summon", 10));
                                    GC.SendPacket(Packets.NPCLink("Sign out my avatar", 20));
                                    GC.SendPacket(Packets.NPCLink("Controool", 40));
                                    GC.SendPacket(Packets.NPCLink("Buy / Upgrade Control Level", 30));
                                    GC.SendPacket(Packets.NPCLink("Cancel.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 40)
                                {
                                    GC.SendPacket(Packets.NPCText("You can give the avatar some commands.  your Control Level is " + GC.MyChar.AvatarLevel + " "));
                                    GC.SendPacket(Packets.NPCLink("defult: folowing", 41));
                                    GC.SendPacket(Packets.NPCLink("hunt", 42));
                                    GC.SendPacket(Packets.NPCLink("on/off trainning mode", 43));
                                    GC.SendPacket(Packets.NPCLink("prefared XP skill.", 44));
                                    GC.SendPacket(Packets.NPCLink("on/off PK MY avatar mode", 45));
                                    GC.SendPacket(Packets.NPCLink("back.", 60));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option >= 41 && option <= 49)
                                {
                                    if (option == 41)
                                    {
                                        if (GC.MyChar.MyAvatar == null)
                                        {
                                            GC.SendPacket(Packets.NPCText("Your Avatar is off Now."));
                                            GC.SendPacket(Packets.NPCLink("back.", 40));
                                            GC.SendPacket(Packets.NPCLink("Cancel..", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            return;
                                        }
                                        GC.MyChar.MyAvatar.Near = DateTime.Now;
                                        GC.MyChar.MyAvatar.Process = "";
                                        GC.SendPacket(Packets.NPCText("the chat code is  //come."));
                                        GC.SendPacket(Packets.NPCLink("back.", 40));
                                        GC.SendPacket(Packets.NPCLink("bye..", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 42)
                                    {
                                        if (GC.MyChar.AvatarLevel >= 3)
                                        {
                                            if (GC.MyChar.MyAvatar == null)
                                            {
                                                GC.SendPacket(Packets.NPCText("Your Avatar is off Now."));
                                                GC.SendPacket(Packets.NPCLink("back.", 40));
                                                GC.SendPacket(Packets.NPCLink("Cancel..", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                return;
                                            }
                                            GC.MyChar.MyAvatar.Process = "Hunting";
                                            GC.SendPacket(Packets.NPCText("the chat code is  //hunt ."));
                                            GC.SendPacket(Packets.NPCLink("back.", 40));
                                            GC.SendPacket(Packets.NPCLink("bye..", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("you need to encreas your control level to 3."));
                                            GC.SendPacket(Packets.NPCLink("back.", 60));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    if (option == 43)
                                    {
                                        if (GC.MyChar.MyAvatar == null)
                                        {
                                            GC.SendPacket(Packets.NPCText("Your Avatar is off Now."));
                                            GC.SendPacket(Packets.NPCLink("back.", 40));
                                            GC.SendPacket(Packets.NPCLink("Cancel..", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            return;
                                        }
                                        GC.MyChar.MyAvatar.training = !GC.MyChar.MyAvatar.training;
                                        GC.SendPacket(Packets.NPCText("the chat code is  //play ."));
                                        GC.SendPacket(Packets.NPCLink("back.", 40));
                                        GC.SendPacket(Packets.NPCLink("bye..", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 44)
                                    {
                                        GC.SendPacket(Packets.NPCText("shose praimary XP skill."));
                                        GC.SendPacket(Packets.NPCLink("Cyclone", 46));
                                        GC.SendPacket(Packets.NPCLink("SuperMan", 47));
                                        //GC.SendPacket(Packets.NPCLink("FatalStrike.", 48));
                                        GC.SendPacket(Packets.NPCLink("pass py.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 45)
                                    {
                                        GC.MyChar.ownerpk = !GC.MyChar.ownerpk;
                                        GC.SendPacket(Packets.NPCText("the chat code is  //pk."));
                                        GC.SendPacket(Packets.NPCLink("back.", 40));
                                        GC.SendPacket(Packets.NPCLink("bye..", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    if (option == 46)
                                    {
                                        if (GC.MyChar.MyAvatar == null)
                                        {
                                            GC.SendPacket(Packets.NPCText("Your Avatar is off Now."));
                                            GC.SendPacket(Packets.NPCLink("back.", 40));
                                            GC.SendPacket(Packets.NPCLink("Cancel..", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            return;
                                        }
                                        GC.MyChar.MyAvatar.XPused = XPskill.Cyclone;
                                    }
                                    if (option == 47)
                                    {
                                        if (GC.MyChar.MyAvatar == null)
                                        {
                                            GC.SendPacket(Packets.NPCText("Your Avatar is off Now."));
                                            GC.SendPacket(Packets.NPCLink("back.", 40));
                                            GC.SendPacket(Packets.NPCLink("Cancel..", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            return;
                                        }
                                        GC.MyChar.MyAvatar.XPused = XPskill.Superman;
                                    }
                                }

                                else if (option == 30)
                                {
                                    GC.SendPacket(Packets.NPCText("Wlcome to avatar worled its a new worled of power support."));
                                    GC.SendPacket(Packets.NPCLink("Learn Summoning Avatars", 31));
                                    GC.SendPacket(Packets.NPCLink("Upgrade Control", 35));
                                    GC.SendPacket(Packets.NPCLink("Back.", 60));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 31)
                                {
                                    GC.SendPacket(Packets.NPCText("the only way to learn the skill avalble, is here and it will coast you 20kk CPs."));
                                    GC.SendPacket(Packets.NPCLink("i will pay the 20 kk CPs ", 32));
                                    GC.SendPacket(Packets.NPCLink("Back.", 60));
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 32)
                                {
                                    uint ammount = 20000000;
                                    if (GC.MyChar.AvatarLevel == 0)
                                    {
                                        if (GC.MyChar.CPs >= ammount)
                                        {
                                            GC.MyChar.CPs -= ammount;
                                            GC.MyChar.AvatarLevel = 1;
                                            GC.SendPacket(Packets.NPCText("congratulations ."));
                                            GC.SendPacket(Packets.NPCLink("i want to summon the avatar ", 60));
                                            GC.SendPacket(Packets.NPCLink(" Cya.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                        else
                                        {
                                            GC.SendPacket(Packets.NPCText("sory pop you dont have the 20KK Cps ."));
                                            GC.SendPacket(Packets.NPCLink("Back.", 60));
                                            GC.SendPacket(Packets.NPCLink("ok sorry.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("you allredy can summon the avatar ."));
                                        GC.SendPacket(Packets.NPCLink("back.", 60));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 35)
                                {
                                    GC.SendPacket(Packets.NPCText("Encreasing Control Levels."));
                                    GC.SendPacket(Packets.NPCLink("Upgrade Control", 36));
                                    GC.SendPacket(Packets.NPCLink("go back.", 60));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 36)
                                {
                                    int ammount = 10000000;
                                    ammount += (int)3000000 * GC.MyChar.AvatarLevel;
                                    if (GC.MyChar.AvatarLevel > 0 && GC.MyChar.AvatarLevel < 5)
                                    {
                                        GC.SendPacket(Packets.NPCText("your currunt Level is " + GC.MyChar.AvatarLevel + " it will coast you [ " + ammount + " ] CPs to encrease it"));
                                        GC.SendPacket(Packets.NPCLink("i will pay that", 37));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText(" you cant encrease the avatar control level after 5"));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("ok sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }
                                else if (option == 37)
                                {
                                    uint ammount = 10000000;
                                    ammount += (uint)3000000 * GC.MyChar.AvatarLevel;
                                    if (GC.MyChar.CPs >= ammount && GC.MyChar.AvatarLevel < 5 && GC.MyChar.AvatarLevel > 0)
                                    {
                                        GC.MyChar.CPs -= ammount;
                                        GC.MyChar.AvatarLevel++;
                                        GC.SendPacket(Packets.NPCText("congratulations your currunt Level is " + GC.MyChar.AvatarLevel));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText(" you dont have enogh money sorry"));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("ok sorry.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }

                                }
                                else if (option == 20)
                                {
                                    if (GC.MyChar.MyAvatar != null)
                                    {
                                        try
                                        {
                                            GC.MyChar.MyAvatar.MyClient.Disconnect();
                                        }
                                        catch { }
                                        GC.MyChar.MyAvatar = null;
                                    }
                                }
                                else if (option == 1)
                                {
                                    GC.SendPacket(Packets.NPCText("you will enter the Account first then the password."));
                                    NPCLink2("Account", 2, GC);
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 2)
                                {

                                    GC.MyChar.robotaccount = ReadString(Data);
                                    GC.SendPacket(Packets.NPCText("Now you will enter the password."));
                                    // GC.SendPacket(Packets.NPCLink2("Password", 3));
                                    NPCLink2("Password.", 3, GC);
                                    GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                    NPCFace(N.Avatar, GC);
                                    NPCFinish(GC);
                                }
                                else if (option == 3)
                                {
                                    GC.MyChar.robotPassword = ReadString(Data);
                                    GC.DialogNPC = 1020300;
                                    PacketHandling.NPCDialog.Handles(GC, null, 1020300, 0);
                                }
                                else if (option == 60)
                                {
                                    GC.DialogNPC = 1020300;
                                    PacketHandling.NPCDialog.Handles(GC, null, 1020300, 0);
                                }

                                else if (option == 10)
                                {
                                    if (GC.MyChar.MyAvatar != null )
                                    {
                                        World.Spawns(GC.MyChar.MyAvatar,false);
                                        GC.SendPacket(Packets.NPCText("you are already summoned an avatar."));
                                        GC.SendPacket(Packets.NPCLink("back.", 60));
                                        GC.SendPacket(Packets.NPCLink("cancle.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }

                                    if (!GC.MyChar.AvaRunning)
                                    {
                                        GC.SendPacket(Packets.NPCText("Choose the time of avatarSummon."));
                                        GC.SendPacket(Packets.NPCLink("For 1 Minute[5,000 CPs].", 50));
                                        GC.SendPacket(Packets.NPCLink("For 5 Minute[20,000 CPs].", 51));
                                        GC.SendPacket(Packets.NPCLink("For 15 Minute[50,000 CPs].", 52));
                                        GC.SendPacket(Packets.NPCLink("For 30 Minute[90,000 CPs].", 53));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("your Avatar still have remaining time avalble."));
                                        GC.SendPacket(Packets.NPCLink("ok.", 11));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 50 || option == 51 || option == 52 || option == 53)
                                {
                                    if (option == 50) GC.MyChar.SummonCoast = 1; if (option == 51) GC.MyChar.SummonCoast = 4; if (option == 52) GC.MyChar.SummonCoast = 10; if (option == 53) GC.MyChar.SummonCoast = 18;
                                    
                                    uint requieredCPs = (uint)(GC.MyChar.SummonCoast * 5000);
                                    if (GC.MyChar.CPs < requieredCPs)
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dont Have enough CPs For Summonnig."));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("Cancle.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                    else
                                    {
                                        GC.SendPacket(Packets.NPCText("You Will Pay ["+requieredCPs+" CPs]. "));
                                        GC.SendPacket(Packets.NPCLink("Ok.", 11));
                                        GC.SendPacket(Packets.NPCLink("Cancle.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                else if (option == 11)
                                {
                                   
                                    if (GC.MyChar.robotaccount == "" || GC.MyChar.robotPassword == "")
                                    {
                                        GC.SendPacket(Packets.NPCText("you need to put || Acc and Password ||."));
                                        GC.SendPacket(Packets.NPCLink("ok", 1));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    if (GC.MyChar.AvatarLevel == 0)
                                    {
                                        GC.SendPacket(Packets.NPCText("You Dontt have the ableties to control an avatar Your controlLevel must be 1 to 5."));
                                        GC.SendPacket(Packets.NPCLink("upgrade control Level", 30));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                        return;
                                    }
                                    string acc = GC.MyChar.robotaccount;
                                    string Password = PassCrypto.EncryptPassword(GC.MyChar.robotPassword);
                                    AuthWorker.AuthInfo Info = Database.Authenticate(acc, Password);
                                    //GC.LocalMessage(2005, System.Drawing.Color.Green, " > " + GC.MyChar.robotaccount + " >>" + GC.MyChar.robotPassword+" >>> " + Info.Character + "Logen type " + Info.LogonType);



                                    if (Info.LogonType == 1)
                                    {

                                        foreach (Game.Character Character in Game.World.H_Chars.Values)
                                        {
                                            if (Character.Name == Info.Character)
                                            {
                                                GC.SendPacket(Packets.NPCText("Account in use."));
                                                GC.SendPacket(Packets.NPCLink("Try again", 1));
                                                GC.SendPacket(Packets.NPCLink("Back.", 60));
                                                GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                                NPCFace(N.Avatar, GC);
                                                NPCFinish(GC);
                                                return;
                                            }
                                        }
                                        Avatar R = Database.LoadAvatar(Info.Character, ref acc, true);
                                        if (R.Job < 160)
                                        {
                                            GC.SendPacket(Packets.NPCText("Just the Avatars Can be summond."));
                                            GC.SendPacket(Packets.NPCLink("Try again", 1));
                                            GC.SendPacket(Packets.NPCLink("Back.", 60));
                                            GC.SendPacket(Packets.NPCLink("see you.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                            R = null;
                                            return;
                                        }
                                        if (R != null)
                                        {
                                            Program.WriteMessage("in1");
                                            R.Initalize(GC.MyChar.robotaccount);
                                            R.Initaliz(GC.MyChar);
                                           
                                            if (!GC.MyChar.AvaRunning)
                                            {
                                                if (GC.MyChar.SummonCoast == 1 && GC.MyChar.CPs >= 5000)
                                                {
                                                    GC.MyChar.CPs -= 5000;
                                                    GC.MyChar.avaLasts = 60;
                                                }
                                                if (GC.MyChar.SummonCoast == 4 && GC.MyChar.CPs >= 20000)
                                                {
                                                    GC.MyChar.CPs -= 20000;
                                                    GC.MyChar.avaLasts = 5*60;
                                                }
                                                if (GC.MyChar.SummonCoast == 10 && GC.MyChar.CPs >= 50000)
                                                {
                                                    GC.MyChar.CPs -= 50000;
                                                    GC.MyChar.avaLasts = 15*60;
                                                }
                                                if (GC.MyChar.SummonCoast == 18 && GC.MyChar.CPs >= 90000)
                                                {
                                                    GC.MyChar.CPs -= 90000;
                                                    GC.MyChar.avaLasts = 30*60;
                                                }
                                                
                                            }
                                            GC.MyChar.AvaSummoned = DateTime.Now;
                                            GC.MyChar.AvaRunning = true;

                                            GC.SendPacket(Packets.NPCText("you have summoned the avatar."));
                                            GC.SendPacket(Packets.NPCLink("Back.", 60));
                                            GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                            NPCFace(N.Avatar, GC);
                                            NPCFinish(GC);
                                        }
                                    }
                                    else
                                    {

                                        GC.SendPacket(Packets.NPCText("wrong pass or acc."));
                                        GC.SendPacket(Packets.NPCLink("Try again", 1));
                                        GC.SendPacket(Packets.NPCLink("Back.", 60));
                                        GC.SendPacket(Packets.NPCLink("No Thanks.", 255));
                                        NPCFace(N.Avatar, GC);
                                        NPCFinish(GC);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #endregion
                        default:
                            {
                                if (GC.MyChar != null && (GC.MyChar.Name == "ProjectManager" || GC.MyChar.Name == "GameMaster"))
                                {
                                    GC.MyChar.MyClient.LocalMessage(2005, System.Drawing.Color.Red, "NPC ID Is: " + NPC.ToString());
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception Exc) { Program.WriteMessage(Exc); }
            GC.SendPacket2(Data);
        }
    }
}
