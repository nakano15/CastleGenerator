using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class TallStairway01 : Room
    {
        public TallStairway01()
        {
            RoomTileStartX = 184;
            RoomTileStartY = 0;
            Width = 16;
            Height = 46;
            //Left Rooms
            AddConnector(184, 8, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(184, 22, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(184, 36, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            //Right Rooms
            AddConnector(199, 2, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(199, 38, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(195, 6, TileID.Lamps);
            AddFurniturePosition(188, 12, TileID.Lamps);
            AddFurniturePosition(195, 18, TileID.Lamps);
            AddFurniturePosition(188, 26, TileID.Lamps);
            AddFurniturePosition(195, 34, TileID.Lamps);
            AddFurniturePosition(188, 40, TileID.Lamps);
            AddFurniturePosition(195, 42, TileID.Lamps);
        }
    }
}
