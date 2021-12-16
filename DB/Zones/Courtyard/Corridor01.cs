using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    class Corridor01 : Room
    {
        public Corridor01()
        {
            RoomTileStartX = 32;
            RoomTileStartY = 0;
            Width = 90;
            Height = 20;
            roomType = RoomType.Corridor;
            AddConnector(32, 12, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 1);
            AddConnector(121, 12, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 1);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(73, 51, 36).SetTile(TileID.WoodenBeam);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(107, 91, 34).SetWall(WallID.GoldBrick);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
            AddTileInfo(73, 51, 36).SetTile(TileID.Torches);

            //Lights
            AddFurniturePosition(45, 10, TileID.Torches);
            AddFurniturePosition(48, 10, TileID.Torches);

            AddFurniturePosition(60, 10, TileID.Torches);
            AddFurniturePosition(63, 10, TileID.Torches);

            AddFurniturePosition(75, 10, TileID.Torches);
            AddFurniturePosition(78, 10, TileID.Torches);

            AddFurniturePosition(90, 10, TileID.Torches);
            AddFurniturePosition(93, 10, TileID.Torches);

            AddFurniturePosition(105, 10, TileID.Torches);
            AddFurniturePosition(108, 10, TileID.Torches);

            AddFurniturePosition(33, 11, TileID.Torches);
            AddFurniturePosition(120, 11, TileID.Torches);

            AddFurniturePosition(39, 2, TileID.Chandeliers, 2);
            AddFurniturePosition(54, 2, TileID.Chandeliers, 2);
            AddFurniturePosition(69, 2, TileID.Chandeliers, 2);
            AddFurniturePosition(84, 2, TileID.Chandeliers, 2);
            AddFurniturePosition(99, 2, TileID.Chandeliers, 2);
            AddFurniturePosition(114, 2, TileID.Chandeliers, 2);

            //
            AddMonsterPosition(47, 16, DifficultyLevel.VeryEasy);
            AddMonsterPosition(62, 16, DifficultyLevel.Normal);
            AddMonsterPosition(77, 16, DifficultyLevel.Hard);
            AddMonsterPosition(92, 16, DifficultyLevel.Normal);
            AddMonsterPosition(107, 16, DifficultyLevel.VeryEasy);
        }
    }
}
