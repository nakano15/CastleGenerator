using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class BigRoom01 : Room
    {
        public BigRoom01()
        {
            RoomTileStartX = 123;
            RoomTileStartY = 0;
            Width = 60;
            Height = 36;
            //Left Rooms
            AddConnector(123, 12, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(123, 26, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            //Right Rooms
            AddConnector(182, 12, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(182, 26, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);
            //Upper Room
            AddConnector(134, 0, 6, ConnectorPosition.Up, ConnectorRequirement.Flight, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.Stone);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.Stone);
            AddTileInfo(73, 51, 36).SetTile(TileID.WoodenBeam).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.EbonstoneBrick);

            //Left
            AddFurniturePosition(125, 11, TileID.Torches);
            AddFurniturePosition(133, 16, TileID.Lamps);
            AddFurniturePosition(140, 19, TileID.Lamps);
            AddFurniturePosition(147, 26, TileID.Lamps);
            AddFurniturePosition(131, 30, TileID.Lamps);
            AddFurniturePosition(125, 25, TileID.Torches);
            //Right
            AddFurniturePosition(180, 11, TileID.Torches);
            AddFurniturePosition(172, 16, TileID.Lamps);
            AddFurniturePosition(165, 19, TileID.Lamps);
            AddFurniturePosition(158, 26, TileID.Lamps);
            AddFurniturePosition(174, 30, TileID.Lamps);
            AddFurniturePosition(180, 25, TileID.Torches);

            AddFurniturePosition(145, 7, TileID.Lamps);
            AddFurniturePosition(162, 7, TileID.Lamps);

            AddTreasurePosition(154, 7, ConnectorRequirement.DoubleJump);
            AddTreasurePosition(154, 26, ConnectorRequirement.None);

            AddMonsterPosition(139, 9, DifficultyLevel.VeryEasy);
            AddMonsterPosition(169, 9, DifficultyLevel.VeryEasy);
            AddMonsterPosition(130, 30, DifficultyLevel.VeryEasy);
            AddMonsterPosition(175, 30, DifficultyLevel.VeryEasy);
        }
    }
}
