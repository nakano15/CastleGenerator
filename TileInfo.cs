using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CastleGenerator
{
    public class TileInfo
    {
        public Color Code = Color.White;
        public bool Active = false;
        public ushort TileID = 0, WallID = 0;
        public short TileX = 0, TileY = 0;
        public byte LiquidID = 0, LiquidValue = 0;

        public TileInfo(Color Code)
        {
            this.Code = Code;
        }

        public TileInfo(byte CodeR, byte CodeG, byte CodeB, byte CodeA = 255)
        {
            this.Code = new Color(CodeR, CodeG, CodeB, CodeA);
        }

        public bool IsSameColor(Color color)
        {
            return Code.R == color.R && Code.G == color.G && Code.B == color.B && Code.A == color.A;
        }

        public TileInfo SetTile(ushort TileID, short TileX = 0, short TileY = 0)
        {
            this.TileID = TileID;
            this.TileX = TileX;
            this.TileY = TileY;
            Active = true;
            return this;
        }

        public TileInfo SetWall(ushort WallID)
        {
            this.WallID = WallID;
            return this;
        }

        public TileInfo SetLiquid(byte LiquidType, byte LiquidValue)
        {
            this.LiquidID = LiquidType;
            if (LiquidValue > 100) LiquidValue = 100;
            this.LiquidValue = LiquidValue;
            return this;
        }

        public const int Type_Solid = 0;
        public const int Type_Halfbrick = 1;
        public const int Type_SlopeDownRight = 2;
        public const int Type_SlopeDownLeft = 3;
        public const int Type_SlopeUpRight = 4;
        public const int Type_SlopeUpLeft = 5;
        public const int Liquid_Water = 0;
        public const int Liquid_Lava = 1;
        public const int Liquid_Honey = 2;
    }
}
