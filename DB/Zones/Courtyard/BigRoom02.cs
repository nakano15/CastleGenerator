using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class BigRoom02 : Room
    {
        public BigRoom02()
        {
            RoomTileStartX = 158;
            RoomTileStartY = 61;
            Width = 42;
            Height = 31;

            AddConnector(158, 67, 5, ConnectorPosition.Left, ConnectorRequirement.Flight, TileID.StoneSlab);
            AddConnector(158, 82, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);

            AddFurniturePosition(167, 81, TileID.Lamps);
            AddFurniturePosition(175, 81, TileID.Lamps);
            AddFurniturePosition(183, 81, TileID.Lamps);
            AddFurniturePosition(191, 81, TileID.Lamps);
            AddFurniturePosition(166, 71, TileID.Lamps);

            AddFurniturePosition(171, 81, TileID.Saplings);
            AddFurniturePosition(179, 81, TileID.Saplings);
            AddFurniturePosition(188, 81, TileID.Saplings);
            AddFurniturePosition(195, 81, TileID.Saplings);

            AddTreasurePosition(164, 71, ConnectorRequirement.Flight);

            AddMonsterPosition(171, 87, DifficultyLevel.VeryEasy);
            AddMonsterPosition(188, 87, DifficultyLevel.VeryEasy);
            AddMonsterPosition(180, 87, DifficultyLevel.Hard);
        }
    }
}
