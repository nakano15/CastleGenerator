using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubworldLibrary;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using Terraria;

namespace CastleGenerator
{
    public class CastleSubworld : Subworld
    {
        private static int FinalityItem = 0, RoomsToGenerateMin = -1, RoomsToGenerateMax = -1, DifficultyChangeMin = -1, DifficultyChangeMax = -1;

        private static List<Loot> loots = new List<Loot>();

        public override int width => 1400;

        public override int height => 1400;

        private Generator.GenerateCastle Castle = new Generator.GenerateCastle();

        public override List<GenPass> tasks
        {
            get
            {
                List<GenPass> gen = new List<GenPass>();
                gen.Add(new PassLegacy("Initializing Castle World", InitializeCastleWorld));
                gen.Add(new PassLegacy("Terrain", GenerateTerrain));
                gen.Add(Castle);
                return gen;
            }
        }

        public static void ChangeFinalityItem(int ItemID)
        {
            FinalityItem = ItemID;
        }

        public static void ChangeRoomsToGenerate(int Min = 15, int Max = 30)
        {
            RoomsToGenerateMin = Min;
            RoomsToGenerateMax = Max;
        }

        public static void ChangeDifficulty(int Min = 1, int Max = 10)
        {
            DifficultyChangeMin = Min;
            DifficultyChangeMax = Max;
        }

        public static void AddItem(int ItemID, Loot.LootType type = Loot.LootType.Normal, DifficultyLevel difficulty = DifficultyLevel.VeryEasy)
        {
            loots.Add(new Loot(ItemID, type, difficulty));
        }

        public override bool noWorldUpdate => true;

        public override bool saveSubworld => false;

        public void InitializeCastleWorld(GenerationProgress progress)
        {
            Castle.InitializeCastle();
            Castle.Finality = FinalityItem;
            Castle.CreateLifeCrystals = false;
            //Castle.GenerateLootTable = false;
            if (loots.Count > 0)
            {
                Castle.loot = loots;
                Castle.GenerateLootTable = false;
            }
            if (DifficultyChangeMax > -1 && DifficultyChangeMin > -1)
            {
                Castle.MinDifficultyLevel = DifficultyChangeMin;
                Castle.MaxDifficultyLevel = DifficultyChangeMax;
            }
            if (RoomsToGenerateMin > -1 && RoomsToGenerateMax > -1)
                Castle.RoomsToGenerate = Main.rand.Next(RoomsToGenerateMin, RoomsToGenerateMax + 1);
            FinalityItem = 0;
            RoomsToGenerateMin = -1;
            RoomsToGenerateMax = -1;
            DifficultyChangeMin = -1;
            DifficultyChangeMax = -1;
            loots = new List<Loot>();
        }

        public void GenerateTerrain(GenerationProgress progress)
        {
            Terraria.Utilities.UnifiedRandom genRand = Castle.rand;
            progress.Message = Lang.gen[0].Value;
            int num700 = 0;
            int num701 = 0;
            Main.worldSurface = (double)Main.maxTilesY * 0.3;
            Main.worldSurface *= (double)genRand.Next(90, 110) * 0.005;
            Main.rockLayer = Main.worldSurface + (double)Main.maxTilesY * 0.2;
            Main.rockLayer *= (double)genRand.Next(90, 110) * 0.01;
            double worldSurfaceLow = Main.worldSurface;
            double worldSurfaceHigh = Main.worldSurface;
            double rockLayerLow = Main.rockLayer;
            double rockLayerHigh = Main.rockLayer;
            for (int num702 = 0; num702 < Main.maxTilesX; num702++)
            {
                float value21 = (float)num702 / (float)Main.maxTilesX;
                progress.Set(value21);
                if (Main.worldSurface < worldSurfaceLow)
                {
                    worldSurfaceLow = Main.worldSurface;
                }
                if (Main.worldSurface > worldSurfaceHigh)
                {
                    worldSurfaceHigh = Main.worldSurface;
                }
                if (Main.rockLayer < rockLayerLow)
                {
                    rockLayerLow = Main.rockLayer;
                }
                if (Main.rockLayer > rockLayerHigh)
                {
                    rockLayerHigh = Main.rockLayer;
                }
                if (num701 <= 0)
                {
                    num700 = genRand.Next(0, 5);
                    num701 = genRand.Next(5, 40);
                    if (num700 == 0)
                    {
                        num701 *= (int)((double)genRand.Next(5, 30) * 0.2);
                    }
                }
                num701--;
                if ((double)num702 > (double)Main.maxTilesX * 0.43 && (double)num702 < (double)Main.maxTilesX * 0.57 && num700 >= 3)
                {
                    num700 = genRand.Next(3);
                }
                if ((double)num702 > (double)Main.maxTilesX * 0.47 && (double)num702 < (double)Main.maxTilesX * 0.53)
                {
                    num700 = 0;
                }
                if (num700 == 0)
                {
                    while (genRand.Next(0, 7) == 0)
                    {
                        Main.worldSurface += genRand.Next(-1, 2);
                    }
                }
                else if (num700 == 1)
                {
                    while (genRand.Next(0, 4) == 0)
                    {
                        Main.worldSurface -= 1.0;
                    }
                    while (genRand.Next(0, 10) == 0)
                    {
                        Main.worldSurface += 1.0;
                    }
                }
                else if (num700 == 2)
                {
                    while (genRand.Next(0, 4) == 0)
                    {
                        Main.worldSurface += 1.0;
                    }
                    while (genRand.Next(0, 10) == 0)
                    {
                        Main.worldSurface -= 1.0;
                    }
                }
                else if (num700 == 3)
                {
                    while (genRand.Next(0, 2) == 0)
                    {
                        Main.worldSurface -= 1.0;
                    }
                    while (genRand.Next(0, 6) == 0)
                    {
                        Main.worldSurface += 1.0;
                    }
                }
                else if (num700 == 4)
                {
                    while (genRand.Next(0, 2) == 0)
                    {
                        Main.worldSurface += 1.0;
                    }
                    while (genRand.Next(0, 5) == 0)
                    {
                        Main.worldSurface -= 1.0;
                    }
                }
                if (Main.worldSurface < (double)Main.maxTilesY * 0.17)
                {
                    Main.worldSurface = (double)Main.maxTilesY * 0.17;
                    num701 = 0;
                }
                else if (Main.worldSurface > (double)Main.maxTilesY * 0.3)
                {
                    Main.worldSurface = (double)Main.maxTilesY * 0.3;
                    num701 = 0;
                }
                if ((num702 < 275 || num702 > Main.maxTilesX - 275) && Main.worldSurface > (double)Main.maxTilesY * 0.25)
                {
                    Main.worldSurface = (double)Main.maxTilesY * 0.25;
                    num701 = 1;
                }
                while (genRand.Next(0, 3) == 0)
                {
                    Main.rockLayer += genRand.Next(-2, 3);
                }
                if (Main.rockLayer < Main.worldSurface + (double)Main.maxTilesY * 0.05)
                {
                    Main.rockLayer += 1.0;
                }
                if (Main.rockLayer > Main.worldSurface + (double)Main.maxTilesY * 0.35)
                {
                    Main.rockLayer -= 1.0;
                }
                for (int num703 = 0; (double)num703 < Main.worldSurface; num703++)
                {
                    Main.tile[num702, num703].active(active: false);
                    Main.tile[num702, num703].frameX = -1;
                    Main.tile[num702, num703].frameY = -1;
                }
                for (int num704 = (int)Main.worldSurface; num704 < Main.maxTilesY; num704++)
                {
                    if ((double)num704 < Main.rockLayer)
                    {
                        Main.tile[num702, num704].active(active: true);
                        Main.tile[num702, num704].type = 0;
                        Main.tile[num702, num704].frameX = -1;
                        Main.tile[num702, num704].frameY = -1;
                    }
                    else
                    {
                        Main.tile[num702, num704].active(active: true);
                        Main.tile[num702, num704].type = 1;
                        Main.tile[num702, num704].frameX = -1;
                        Main.tile[num702, num704].frameY = -1;
                    }
                }
            }
            Main.worldSurface = worldSurfaceHigh + 25.0;
            Main.rockLayer = rockLayerHigh;
            double num705 = (int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6;
            Main.rockLayer = Main.worldSurface + num705;
            float waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
            waterLine += genRand.Next(-100, 20);
            float lavaLine = waterLine + genRand.Next(50, 80);
        }
    }
}
