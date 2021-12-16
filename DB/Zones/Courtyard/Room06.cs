using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room06 : Room
    {
        public Room06()
        {
            RoomTileStartX = 94;
            RoomTileStartY = 85;
            Width = 22;
            Height = 19;

            AddConnector(94, 95, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(115, 97, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);

            AddTreasurePosition(105, 100);
        }
    }
}
