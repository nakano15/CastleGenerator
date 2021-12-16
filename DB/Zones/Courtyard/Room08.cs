using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room08 : Room
    {
        public Room08()
        {
            RoomTileStartX = 102;
            RoomTileStartY = 105;
            Width = 14;
            Height = 11;

            AddConnector(102, 109, 5, ConnectorPosition.Left, ConnectorRequirement.Water, TileID.StoneSlab);
            AddConnector(115, 109, 5, ConnectorPosition.Right, ConnectorRequirement.Water, TileID.StoneSlab);

            AddTreasurePosition(109, 113, ConnectorRequirement.Water);
        }
    }
}
