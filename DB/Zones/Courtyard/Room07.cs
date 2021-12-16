using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room07 : Room
    {
        public Room07()
        {
            RoomTileStartX = 158;
            RoomTileStartY = 93;
            Width = 16;
            Height = 23;

            AddConnector(163, 93, 6, ConnectorPosition.Up, ConnectorRequirement.Water, TileID.StoneSlab);
            AddConnector(158, 109, 5, ConnectorPosition.Left, ConnectorRequirement.Water, TileID.StoneSlab);
            AddConnector(173, 109, 5, ConnectorPosition.Right, ConnectorRequirement.Water, TileID.StoneSlab);

            AddTreasurePosition(166, 113, ConnectorRequirement.Water);
        }
    }
}
