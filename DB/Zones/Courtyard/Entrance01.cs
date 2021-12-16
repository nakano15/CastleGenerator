using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Entrance01 : Room
    {
        public Entrance01()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 0;
            roomType = RoomType.Spawn;
            Width = 31;
            Height = 20;
            SurfaceRoom = true;
            AddConnector(30, 12, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            AddFurniturePosition(10, 16, TileID.Lamps);
            AddFurniturePosition(22, 16, TileID.Lamps);
            AddFurniturePosition(3, 12, TileID.Torches);
            AddFurniturePosition(28, 11, TileID.Torches);
            AddFurniturePosition(3, 3, TileID.Torches);
            AddFurniturePosition(7, 5, TileID.Tables);
            AddFurniturePosition(9, 5, TileID.Chairs, FacingLeft: true);

            AddTreasurePosition(4, 5, ConnectorRequirement.Flight);
            AddTreasurePosition(6, 16, ConnectorRequirement.None);

            AddMonsterPosition(15, 16, DifficultyLevel.VeryEasy);

            PlayerSpawnX = 6;
            PlayerSpawnY = 12;

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(170, 120, 84).SetTile(TileID.WoodBlock);
            AddTileInfo(121, 119, 101).SetTile(TileID.Cog);
            AddTileInfo(77, 77, 77).SetWall(WallID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
        }
    }
}
