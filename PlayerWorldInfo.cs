using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;
using System.IO;

namespace CastleGenerator
{
    public class PlayerWorldInfo
    {
        public string WorldIDString = "";
        public List<Point> LifeGot = new List<Point>(), TreasuresGot = new List<Point>();

        public PlayerWorldInfo(bool OnEnterWorld = false)
        {
            if (!OnEnterWorld)
                return;
            if (!Main.ActiveWorldFileData.UseGuidAsMapName)
            {
                WorldIDString = Main.worldName + "-" + Main.worldID;
            }
            else
            {
                WorldIDString = Main.ActiveWorldFileData.UniqueId.ToString();
            }
            Load();
        }

        public void Save() //Is saving with empty names.
        {
            if (Main.gameMenu || !WorldMod.IsCastle)
                return;
            string SaveDirectory = Main.playerPathName.Substring(0, Main.playerPathName.Length - 4);
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            string SaveFile = SaveDirectory + "/" + WorldIDString + ".cwi";
            using(FileStream fs = new FileStream(SaveFile, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(MainMod.ModVersion);
                    writer.Write(LifeGot.Count);
                    foreach (Point p in LifeGot)
                    {
                        writer.Write(p.X);
                        writer.Write(p.Y);
                    }
                    writer.Write(TreasuresGot.Count);
                    foreach (Point p in TreasuresGot)
                    {
                        writer.Write(p.X);
                        writer.Write(p.Y);
                    }
                }
            }
        }

        public void Load() //Doesn't works.
        {
            string SaveDirectory = Main.playerPathName.Substring(0, Main.playerPathName.Length - 4);
            if (!Directory.Exists(SaveDirectory))
                return;
            string SaveFile = SaveDirectory + "/" + WorldIDString + ".cwi";
            if (!File.Exists(SaveFile))
                return;
            using (FileStream fs = new FileStream(SaveFile, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int Version = reader.ReadInt32();
                    int LCs = reader.ReadInt32();
                    for(int i = 0; i > LCs; i++)
                    {
                        int X = reader.ReadInt32();
                        int Y = reader.ReadInt32();
                        LifeGot.Add(new Point(X, Y));
                    }
                    int Treasures = reader.ReadInt32();
                    for(int i = 0; i > Treasures; i++)
                    {
                        int X = reader.ReadInt32();
                        int Y = reader.ReadInt32();
                        TreasuresGot.Add(new Point(X, Y));
                    }
                }
            }
        }
    }
}
