using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Server.PacketHandling
{
    public partial class Struct1
    {
        public class Portal
        {
            public int ID;
            public int StartX;
            public int StartY;
            public int StartMap;
            public int EndX;
            public int EndY;
            public int EndMap;
        }
        public static void Handle(GameClient GC, byte[] Data)
        {
            ushort EndMap = GC.MyChar.Loc.Map;
            ushort EndX = (ushort)(GC.MyChar.Loc.X + 3);
            ushort EndY = (ushort)(GC.MyChar.Loc.Y - 2);

            //foreach (ushort[] Port in Database.Portals)
            try
            {
                foreach (KeyValuePair<string, PacketHandling.Struct1.Portal> Portale in Game.World.Portals)
                {
                    PacketHandling.Struct1.Portal APortals = Portale.Value;

                    if (GC.MyChar.Loc.Map >= 10000)
                    {

                        if (GC.MyChar.StatEff.Contains(Server.Game.StatusEffectEn.Ride))
                        {
                            if (APortals.StartMap == 1098 /*&& !DMaps.HouseUpgrade(GC.MyChar.Loc.Map)*/)
                            {
                                if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 6)
                                {
                                    EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                                    EndX = (ushort)APortals.EndX;
                                    EndY = (ushort)APortals.EndY;
                                    GC.MyChar.Teleport(EndMap, EndX, EndY);
                                    break;
                                }
                            }
                            else if (APortals.StartMap == 1099 /*&& DMaps.HouseUpgrade(GC.MyChar.Loc.Map)*/)
                            {
                                if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 6)
                                {
                                    EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                                    EndX = (ushort)APortals.EndX;
                                    EndY = (ushort)APortals.EndY;
                                    GC.MyChar.Teleport(EndMap, EndX, EndY);
                                    break;
                                }
                            }

                        }
                        else
                        {
                            if (APortals.StartMap == 1098 /*&& !DMaps.HouseUpgrade(GC.MyChar.Loc.Map)*/)
                            {
                                if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 2)
                                {
                                    EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                                    EndX = (ushort)APortals.EndX;
                                    EndY = (ushort)APortals.EndY;
                                    GC.MyChar.Teleport(EndMap, EndX, EndY);
                                    break;
                                }
                            }
                            else if (APortals.StartMap == 1099/* && DMaps.HouseUpgrade(GC.MyChar.Loc.Map)*/)
                            {
                                if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 2)
                                {
                                    EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                                    EndX = (ushort)APortals.EndX;
                                    EndY = (ushort)APortals.EndY;
                                    GC.MyChar.Teleport(EndMap, EndX, EndY);
                                    break;
                                }
                            }
                        }
                    }
                    else if (APortals.StartMap == GC.MyChar.Loc.Map)
                    {
                        if (GC.MyChar.StatEff.Contains(Server.Game.StatusEffectEn.Ride))
                        {
                            if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 6)
                            {
                                EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                                EndX = (ushort)APortals.EndX;
                                EndY = (ushort)APortals.EndY;
                                GC.MyChar.Teleport(EndMap, EndX, EndY);
                                break;
                            }
                        }
                        else if (MyMath.PointDistance(GC.MyChar.Loc.X, GC.MyChar.Loc.Y, APortals.StartX, APortals.StartY) <= 2)
                        {
                            EndMap = (ushort)APortals.EndMap;//Port[EndMap];
                            EndX = (ushort)APortals.EndX;
                            EndY = (ushort)APortals.EndY;
                            GC.MyChar.Teleport(EndMap, EndX, EndY);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                GC.MyChar.Teleport(1002, 429, 378);
                Program.WriteMessage(e.ToString());
            }
        }
    }
}
