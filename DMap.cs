using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using DMapLoader;

namespace Server
{
    public class DMaps
    {
        public static ArrayList MapsNeeded = new ArrayList() { 1026, 1025, 1013, 1700, 600, 700, 1005, 1038, 1000, 1001, 1010, 1002, 1036, 1950, 1038, 1039, 1037, 1039, 1011, 1015, 1020, 2021, 2022, 2023, 2024, 1028, 1785, 1786, 1787, 1351, 1352, 1353, 1354, 1010, 6000, 6001, 6002, 5000, 1707, 1068, 9995, 9996, 9997, 1099, 1098, 9998, 1996 };
        public static ArrayList DengunMaps1 = new ArrayList() { (ushort)1996, (ushort)1785, (ushort)0, (ushort)1, (ushort)2 };
        public static ArrayList DimMaps = new ArrayList() { (ushort)1996, (ushort)1707, (ushort)1068, (ushort)0, (ushort)1, (ushort)2 };
        public static ArrayList NoPKMaps = new ArrayList() { (ushort)1036, (ushort)1950, (ushort)1039, (ushort)1002, (ushort)700, (ushort)1004 };
        public static ArrayList FreePKMaps = new ArrayList() { (ushort)6000, (ushort)1081, (ushort)6001, (ushort)1038, (ushort)1005, (ushort)1068, (ushort)1090, (ushort)1707, (ushort)5000 };

        public static Hashtable DynamicMapsOwner = new Hashtable();
        public static Hashtable H_Maps = new Hashtable();
        public static Hashtable H_DynamicMaps = new Hashtable();
        public static bool Loaded = false;
        
        public static void LoadMaps()
        {
            if (Directory.Exists(Program.ConquerPath))
            {
                FileStream FS = new FileStream(Program.ConquerPath + @"GameMap.dat", FileMode.Open);
                BinaryReader BR = new BinaryReader(FS);

                int count = 0;
                uint MapCount = BR.ReadUInt32();
                for (uint i = 0; i < MapCount; i++)
                {
                    ushort MapID = (ushort)BR.ReadUInt32();
                    string Path = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                    count++;
                    if (MapsNeeded.Contains((int)MapID) || DengunMaps1.Contains((int)MapID))
                    if (!H_Maps.ContainsKey(MapID))
                    {
                        DMap D = new DMap(MapID, Path);
                        H_Maps.Add(MapID, D);
                    }
                    BR.ReadInt32();
                }
                BR.Close();
                FS.Close();
                Loaded = true;
                Console.WriteLine("Maps Loaded " + count);
            }
            else
                Program.WriteMessage("The specified Conquer Online folder doesn't exist. DMaps couldn't be loaded.");
        }
        public static bool CreateDynamicMap(ushort mapadd, ushort mapneed, uint ownerid)
        {
            bool addedmap = false;
            if (DMaps.DynamicMapsOwner.Contains(Convert.ToInt32(ownerid)))
                return false;
            while (DMaps.H_Maps.Contains(mapadd))
            {
                mapadd++;
            }
            FileStream FS = new FileStream(Program.ConquerPath + @"GameMap.dat", FileMode.Open);
            BinaryReader BR = new BinaryReader(FS);
            uint MapCount = BR.ReadUInt32();
            for (uint i = 0; i < MapCount; i++)
            {
                ushort MapID = (ushort)BR.ReadUInt32();
                
                string Path = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                if (mapneed == MapID)
                {
                    ushort NewMapID = mapadd;
                    DMap D = new DMap(NewMapID, Path);
                    H_Maps.Add(NewMapID, D);
                    H_DynamicMaps.Add(NewMapID, mapneed);
                    DynamicMapsOwner.Add(Convert.ToInt32(ownerid), NewMapID);
                    addedmap = true;
                    break;
                }
                BR.ReadInt32();
            }
            BR.Close();
            FS.Close();
            return addedmap;
        }
        public static bool DeleteDynamicMap(ushort mapadd, uint ownerid)
        {
            bool deletedmap = false;
            if (!DMaps.DynamicMapsOwner.Contains(Convert.ToInt32(ownerid)))
                return false;

            ushort NewMapID = mapadd;
            H_Maps.Remove(NewMapID);
            H_DynamicMaps.Remove(NewMapID);
            DynamicMapsOwner.Remove(Convert.ToInt32(ownerid));
            deletedmap = true;

            return deletedmap;
        }
    }
    public struct DMapCell
    {
        public Boolean High;
        private Boolean _noAccess;
        public Boolean NoAccess
        {
            get
            {
                return _noAccess;
            }
            internal set
            {
                _noAccess = value;
            }
        }
        public DMapCell(Boolean noAccess)
        {
            _noAccess = noAccess;
            High = false;
        }
    }
    public class DMap
    {
        private Int32 Width;
        private Int32 Height;
        private DMapCell[,] Cells;

        public DMap(ushort MapID, string Path)
        {
             
            if (File.Exists(Program.ConquerPath + Path))
            {
                FileStream FS = new FileStream(Program.ConquerPath + Path, FileMode.Open);
                BinaryReader BR = new BinaryReader(FS);
                BR.ReadBytes(268);
                Width = BR.ReadInt32();
                Height = BR.ReadInt32();
                Cells = new DMapCell[Width, Height];

                byte[] cell_data = BR.ReadBytes(((6 * Width) + 4) * Height);
                int offset = 0;

                for (int y = 0; y < Width; y++)
                {
                    for (int x = 0; x < Height; x++)
                    {
                        Boolean noAccess = BitConverter.ToBoolean(cell_data, offset) != false;

                        //Console.WriteLine(y);
                        if (MapID == 5000)
                        {
                            if (x >= 24 && x <= 29)
                                if (y >= 67 && y <= 72)
                                    noAccess = true;
                        }
                        if (MapID == 1002)
                        {
                            if (x >= 606 && x <= 641)
                                if (y >= 674 && y <= 680)
                                    noAccess = false;
                            if (x >= 148 && x <= 194)
                                if (y >= 541 && y <= 546)
                                    noAccess = false;
                        }
                        Cells[x, y] = new DMapCell(noAccess);
                        if (MapID == 1038)
                        {
                            if (x <= 119)
                                Cells[x, y].High = true;
                            if (x >= 120 && x <= 216 && y <= 210)
                                Cells[x, y].High = true;
                        }
                        offset += 6;

                    }
                    offset += 4;
                }
                BR.Close();
                FS.Close();
                
            }
        }

        public DMapCell GetCell(ushort X, ushort Y)
        {
            return Cells[X, Y];
        }
    }
}