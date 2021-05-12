using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public struct coords
    {
        public int X;
        public int Y;

        public coords(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    public static class MyMath
    {
        static Random Rnd = new Random();

        public static int[][] fbCoords(int userx, int usery, int shotx, int shoty, int length)
        {
            int[][] fbcr = new int[length][];
            int nx = shotx - userx;
            int ny = shoty - usery;
            double dir = Math.Atan2(ny, nx);
            for (int i = 1; i <= length; i++)
            {
                double ax = i * Math.Cos(dir);
                double ay = i * Math.Sin(dir);
                int fbx = (int)Math.Round(userx + ax);
                int fby = (int)Math.Round(usery + ay);
                fbcr[i - 1] = new int[2] { fbx, fby };
            }
            return fbcr;
        }
        public static bool Contains(this coords[] Coords, coords Check)
        {
            foreach (coords Coord in Coords)
                if (Coord.X == Check.X && Check.Y == Coord.Y)
                    return true;
            return false;
        }
        public static List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty, byte length)
        {
            double dir = Math.Atan2(shoty - usery, shotx - userx);
            double f_x = (Math.Cos(dir) * length) + userx;
            double f_y = (Math.Sin(dir) * length) + usery;

            return bresenham(userx, usery, (int)f_x, (int)f_y);
        }
        public static void Add(this List<coords> Coords, int x, int y)
        {
            coords add = new coords((ushort)x, (ushort)y);
            if (!Coords.Contains(add))
                Coords.Add(add);
        }
        public static List<coords> bresenham(int x0, int y0, int x1, int y1)
        {
            List<coords> ThisLine = new List<coords>();

            int dy = y1 - y0;
            int dx = x1 - x0;
            int stepx, stepy;

            if (dy < 0) { dy = -dy; stepy = -1; } else { stepy = 1; }
            if (dx < 0) { dx = -dx; stepx = -1; } else { stepx = 1; }
            dy <<= 1;
            dx <<= 1;

            ThisLine.Add(x0, y0);
            if (dx > dy)
            {
                int fraction = dy - (dx >> 1);
                while (x0 != x1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    ThisLine.Add(x0, y0);
                }
            }
            else
            {
                int fraction = dx - (dy >> 1);
                while (y0 != y1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    ThisLine.Add(x0, y0);
                }
            }
            return ThisLine;
        }
        public static double LevelDifference(byte Lev1, byte Lev2)
        {
            if (Lev1 > Lev2)
            {
                double Rt = (Lev1 - Lev2 + 7) / 5;
                return Rt = ((Rt - 1) * 0.8) + 1;
            }
            return 1;
        }
        public static double PotencyDifference(ushort pot1, ushort pot2)
        {
            if (pot1 > pot2)
            {
                double Rt = ((double)(pot1 - pot2 + 15) / 15);
               
                //Console.WriteLine(Rt);
                return Rt ;
            }
            if (pot1 < pot2)
            {
                double Rt = (20 / (double)(pot2 - pot1 + 20));
                
                //Console.WriteLine(Rt);
                return Rt;
            }
            return 1;
        }
        public static bool ChanceSuccess(double Chance)
        {
            int e = Rnd.Next(10000000);
            double a = ((double)e / (double)10000000) * 100;
            return Chance >= a;
        }
        public static double PointDirecton(double x1, double y1, double x2, double y2)
        {
            double direction = 0;

            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double PointDirectonRad(double x1, double y1, double x2, double y2)
        {
            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            return r;
        }
        public static double PointDirecton2(double x1, double y1, double x2, double y2)
        {
            double direction = 0;

            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            direction = (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double RadianToDegree(double r)
        {
            if (r < 0) r += (double)Math.PI * 2;

            double direction = 360 - (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double DegreeToRadian(double degr)
        {
            return degr * Math.PI / 180;
        }
        public static int PointDistance(double x1, double y1, double x2, double y2)
        {
            return (int)Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }
        public static bool InBox(double x1, double y1, double x2, double y2, byte Range)
        {
            return (Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)) <= Range);
        }
    }
}
