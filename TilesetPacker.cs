using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace CastleGenerator
{
    public class TilesetPacker
    {
        const int PackerVersion = 0;
        public int Width = 0, Height = 0;
        public List<TileStep> TileSteps = new List<TileStep>();

        public static TilesetPacker PackWorld()
        {
            TilesetPacker pack = new TilesetPacker();
            pack.Width = Main.maxTilesX;
            pack.Height = Main.maxTilesY;
            TileStep LastStep = new TileStep();
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!LastStep.IsSameTile(tile, true))
                    {
                        pack.TileSteps.Add(LastStep);
                        LastStep = new TileStep();
                        LastStep.StoreTileInfos(tile);
                    }
                }
            }
            pack.TileSteps.Add(LastStep);
            /*string ExportLogInfos = "Packed world: Width " + pack.Width + "  Height: " + pack.Height + "\nTile Steps: " + pack.TileSteps.Count;
            for (int i = 0; i < pack.TileSteps.Count; i++)
            {
                ExportLogInfos += "\n" + i + "= Has Tile? " + pack.TileSteps[i].HasTile + "  Tile ID: " + pack.TileSteps[i].TileType;
            }
            Console.WriteLine(ExportLogInfos);*/
            //throw new Exception(ExportLogInfos);
            return pack;
        }

        public static void UnpackWorld(TilesetPacker Package, out TilesetPacker.TileStep[,] Tiles)
        {
            int Width = Package.Width;
            int Height = Package.Height;
            Tiles = new TilesetPacker.TileStep[Width, Height];
            ushort Count = 0;
            int CurrentStep = 0;
            TileStep Blank = new TileStep();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    TilesetPacker.TileStep tile;
                    if (CurrentStep < Package.TileSteps.Count)
                    {
                        tile = Package.TileSteps[CurrentStep];
                        Count++;
                        if (Count >= Package.TileSteps[CurrentStep].RepeatTimes)
                        {
                            Count = 0;
                            CurrentStep++;
                        }
                    }
                    else
                    {
                        tile = Blank;
                    }
                    Tiles[x, y] = tile;
                }
            }
        }

        public void Save(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(PackerVersion);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(TileSteps.Count);
                for (int i = 0; i < TileSteps.Count; i++)
                    TileSteps[i].Save(writer);
            }
        }

        public void Load(Stream stream)
        {
            TileSteps.Clear();
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int LastVersion = reader.ReadInt32();
                Width = reader.ReadInt32();
                Height = reader.ReadInt32();
                int Total = reader.ReadInt32();
                for (int i = 0; i < Total; i++)
                {
                    TileStep step = new TileStep();
                    step.Load(reader, LastVersion);
                    TileSteps.Add(step);
                }
            }
        }

        public class TileStep
        {
            public ushort RepeatTimes = 0;
            public ushort TileType = 0;
            public ushort WallType = 0;
            public bool HasTile = false;
            public bool IsActuated = false;
            public bool HasActuator = false;
            public bool HasUnactuatedTile = false;
            public byte SlopeType = 0, BlockType = 0;
            public short TileFrameX = 0, TileFrameY = 0;
            public int WallFrameX = 0, WallFrameY = 0;
            public int TileFrameNum = 0, WallFrameNum = 0;
            public byte TileColor = 0, WallColor = 0;
            public byte LiquidAmount = 0;
            public int LiquidType = 0;
            public ExtraTileFlags ExtraFlags = 0;
            public bool RedWire {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.RedWire);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.RedWire;
                }
            }
            public bool GreenWire {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.GreenWire);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.GreenWire;
                }
            }
            public bool BlueWire {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.BlueWire);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.BlueWire;
                }
            }
            public bool YellowWire {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.YellowWire);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.YellowWire;
                }
            }
            public bool TileInvisible {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.TileInvisible);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.TileInvisible;
                }
            }
            public bool WallInvisible {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.WallInvisible);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.WallInvisible;
                }
            }
            public bool TileExtraBright {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.TileExtraBright);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.TileExtraBright;
                }
            }
            public bool WallExtraBright {
                get
                {
                    return ExtraFlags.HasFlag(ExtraTileFlags.WallExtraBright);
                }
                set
                {
                    ExtraFlags |= ExtraTileFlags.WallExtraBright;
                }
            }
            
            public bool IsSameTile(Tile tile, bool Increment = false)
            {
                if (HasTile == tile.HasTile && 
                    TileType == tile.TileType && 
                    WallType == tile.WallType && 
                    IsActuated == tile.IsActuated && 
                    HasActuator == tile.HasActuator && 
                    SlopeType == (byte)tile.Slope && 
                    BlockType == (byte)tile.BlockType && 
                    TileFrameX == tile.TileFrameX && 
                    TileFrameY == tile.TileFrameY && 
                    WallFrameX == tile.WallFrameX && 
                    WallFrameY == tile.WallFrameY && 
                    TileFrameNum == tile.TileFrameNumber && 
                    WallFrameNum == tile.WallFrameNumber && 
                    TileColor == tile.TileColor && 
                    WallColor == tile.WallColor && 
                    LiquidType == tile.LiquidType &&
                    LiquidAmount == tile.LiquidAmount && 
                    RedWire == tile.RedWire && 
                    GreenWire == tile.GreenWire && 
                    BlueWire == tile.BlueWire && 
                    YellowWire == tile.YellowWire && 
                    TileInvisible == tile.IsTileInvisible && 
                    WallInvisible == tile.IsWallInvisible && 
                    TileExtraBright == tile.IsTileFullbright && 
                    WallExtraBright == tile.IsWallFullbright)
                {
                    if (Increment) RepeatTimes++;
                    return true;
                }
                return false;
            }

            public void StoreTileInfos(Tile tile)
            {
                HasTile = tile.HasTile;
                TileType = tile.TileType; 
                WallType = tile.WallType; 
                IsActuated = tile.IsActuated; 
                HasActuator = tile.HasActuator; 
                SlopeType = (byte)tile.Slope; 
                BlockType = (byte)tile.BlockType; 
                TileFrameX = tile.TileFrameX; 
                TileFrameY = tile.TileFrameY; 
                WallFrameX = tile.WallFrameX; 
                WallFrameY = tile.WallFrameY; 
                TileFrameNum = tile.TileFrameNumber; 
                WallFrameNum = tile.WallFrameNumber; 
                TileColor = tile.TileColor; 
                WallColor = tile.WallColor; 
                LiquidType = tile.LiquidType;
                LiquidAmount = tile.LiquidAmount;
                RedWire = tile.RedWire;
                GreenWire = tile.GreenWire;
                BlueWire = tile.BlueWire;
                YellowWire = tile.YellowWire;
                TileInvisible = tile.IsTileInvisible;
                WallInvisible = tile.IsWallInvisible;
                TileExtraBright = tile.IsTileFullbright;
                WallExtraBright = tile.IsWallFullbright;
            }

            public Tile ExtractTileInfos()
            {
                Tile t = new Tile();
                t.HasTile = this.HasTile;
                t.TileType = this.TileType;
                t.WallType = this.WallType;
                t.IsActuated = this.IsActuated;
                t.HasActuator = this.HasActuator;
                t.Slope = (SlopeType)this.SlopeType;
                t.BlockType = (BlockType)this.BlockType;
                t.TileFrameX = this.TileFrameX;
                t.TileFrameY = this.TileFrameY;
                t.WallFrameX = this.WallFrameX;
                t.WallFrameY = this.WallFrameY;
                t.TileFrameNumber = this.TileFrameNum;
                t.WallFrameNumber = this.WallFrameNum;
                t.TileColor = this.TileColor; 
                t.WallColor = this.WallColor;
                t.LiquidType = this.LiquidType;
                t.LiquidAmount = this.LiquidAmount;
                t.RedWire = this.RedWire;
                t.GreenWire = this.GreenWire;
                t.BlueWire = this.BlueWire;
                t.YellowWire = this.YellowWire;
                t.IsTileInvisible = this.TileInvisible;
                t.IsWallInvisible = this.WallInvisible;
                t.IsTileFullbright = this.TileExtraBright;
                t.IsWallFullbright = this.WallExtraBright;
                return t;
                /*return new Tile()
                {
                    HasTile = this.HasTile, 
                    TileType = this.TileType,
                    WallType = this.WallType,
                    IsActuated = this.IsActuated, 
                    HasActuator = this.HasActuator, 
                    Slope = (SlopeType)this.SlopeType, 
                    BlockType = (BlockType)this.BlockType , 
                    TileFrameX = this.TileFrameX , 
                    TileFrameY = this.TileFrameY , 
                    WallFrameX = this.WallFrameX , 
                    WallFrameY = this.WallFrameY , 
                    TileFrameNumber = this.TileFrameNum , 
                    WallFrameNumber = this.WallFrameNum , 
                    TileColor = this.TileColor , 
                    WallColor = this.WallColor , 
                    LiquidType = this.LiquidType ,
                    LiquidAmount = this.LiquidAmount,
                    RedWire = this.RedWire,
                    GreenWire = this.GreenWire,
                    BlueWire = this.BlueWire,
                    YellowWire = this.YellowWire,
                    IsTileInvisible = this.TileInvisible,
                    IsWallInvisible = this.WallInvisible,
                    IsTileFullbright = this.TileExtraBright,
                    IsWallFullbright = this.WallExtraBright
                };*/
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(RepeatTimes);
                writer.Write(TileType);
                writer.Write(WallType);
                writer.Write(HasTile);
                writer.Write(IsActuated);
                writer.Write(HasActuator);
                writer.Write(HasUnactuatedTile);
                writer.Write(SlopeType);
                writer.Write(BlockType);
                writer.Write(TileFrameX);
                writer.Write(TileFrameY);
                writer.Write(WallFrameX);
                writer.Write(WallFrameY);
                writer.Write(TileFrameNum);
                writer.Write(WallFrameNum);
                writer.Write(TileColor);
                writer.Write(WallColor);
                writer.Write(LiquidAmount);
                writer.Write(LiquidType);
                writer.Write((byte)ExtraFlags);
            }

            public void Load(BinaryReader reader, int LastVersion)
            {
                RepeatTimes = reader.ReadUInt16();
                TileType = reader.ReadUInt16();
                WallType = reader.ReadUInt16();
                HasTile = reader.ReadBoolean();
                IsActuated = reader.ReadBoolean();
                HasActuator = reader.ReadBoolean();
                HasUnactuatedTile = reader.ReadBoolean();
                SlopeType = reader.ReadByte();
                BlockType = reader.ReadByte();
                TileFrameX = reader.ReadInt16();
                TileFrameY = reader.ReadInt16();
                WallFrameX = reader.ReadInt32();
                WallFrameY = reader.ReadInt32();
                TileFrameNum = reader.ReadInt32();
                WallFrameNum = reader.ReadInt32();
                TileColor = reader.ReadByte();
                WallColor = reader.ReadByte();
                LiquidAmount = reader.ReadByte();
                LiquidType = reader.ReadInt32();
                ExtraFlags = (ExtraTileFlags)reader.ReadByte();
            }
            
            [Flags]
            public enum ExtraTileFlags : byte
            {
                None = 0,
                RedWire = 1,
                GreenWire = 2,
                BlueWire = 4,
                YellowWire = 8,
                TileInvisible = 16,
                WallInvisible = 32,
                TileExtraBright = 64,
                WallExtraBright = 128
            }
        }
    }
}