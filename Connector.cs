using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleGenerator
{
    public class Connector
    {
        public byte PositionX = 0, PositionY = 0;
        public byte Distance = 1;
        public ConnectorPosition Position = ConnectorPosition.Right;
        public ushort BlockTile = 0;
        public bool Horizontal { get { return Position == ConnectorPosition.Up || Position == ConnectorPosition.Down; } }
        public ConnectorRequirement Requirement = ConnectorRequirement.None;
        public byte BlockDimension = 1;

        public Connector(byte PosX, byte PosY, byte TileDistance, ConnectorPosition ExtremityPos, ConnectorRequirement GearRequirement, ushort BlockTileID = Terraria.ID.TileID.Stone, byte BlockDimension = 2)
        {
            PositionX = PosX;
            PositionY = PosY;
            Distance = TileDistance;
            Position = ExtremityPos;
            Requirement = GearRequirement;
            BlockTile = BlockTileID;
            this.BlockDimension = BlockDimension;
        }

        public bool IsOpposite(Connector other)
        {
            return IsOpposite(other.Position);
        }

        public bool IsOpposite(ConnectorPosition other)
        {
            switch (Position)
            {
                case ConnectorPosition.Up:
                    return other == ConnectorPosition.Down;
                case ConnectorPosition.Down:
                    return other == ConnectorPosition.Up;
                case ConnectorPosition.Left:
                    return other == ConnectorPosition.Right;
                case ConnectorPosition.Right:
                    return other == ConnectorPosition.Left;
            }
            return false;
        }
    }
}
