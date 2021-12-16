using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class SaveRoom01 : Room
    {
        public SaveRoom01()
        {
            RoomTileStartX = 45;
            RoomTileStartY = 85;
            Width = 27;
            Height = 19;
            roomType = RoomType.SaveRoom;

            AddConnector(45, 95, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(71, 95, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);

            AddFurniturePosition(58, 99, TileID.Beds, 0);
            AddFurniturePosition(54, 99, TileID.Lamps, 0);
            AddFurniturePosition(62, 99, TileID.Lamps, 0);
            AddFurniturePosition(58, 87, TileID.Chandeliers, 2);
        }
    }
}
