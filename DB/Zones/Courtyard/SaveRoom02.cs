using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class SaveRoom02 : Room
    {
        public SaveRoom02()
        {
            RoomTileStartX = 73;
            RoomTileStartY = 85;
            Width = 20;
            Height = 19;
            roomType = RoomType.SaveRoom;

            AddConnector(73, 95, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(92, 95, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);

            AddFurniturePosition(83, 101, TileID.Beds, 0);
            AddFurniturePosition(78, 101, TileID.Lamps, 0);
            AddFurniturePosition(87, 101, TileID.Lamps, 0);
            AddFurniturePosition(83, 87, TileID.Chandeliers, 2);
        }
    }
}
