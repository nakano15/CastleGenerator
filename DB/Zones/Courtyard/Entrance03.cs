using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Entrance03 : Room
    {
        public Entrance03()
        {
            RoomTileStartX = 45;
            RoomTileStartY = 65;
            roomType = RoomType.Spawn;
            Width = 44;
            Height = 19;
            AddConnector(45, 75, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);

            PlayerSpawnX = 83;
            PlayerSpawnY = 80;

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);
            AddTileInfo(141, 121, 111).SetTile(TileID.Iron);

            AddFurniturePosition(45 + 8, 80, TileID.Lamps);
            AddFurniturePosition(45 + 16, 80, TileID.Lamps);
            AddFurniturePosition(45 + 24, 80, TileID.Lamps);
            AddFurniturePosition(45 + 36, 80, TileID.Lamps);

            AddFurniturePosition(45 + 12, 80, TileID.Benches);

            int TableX = 45 + 29;
            AddFurniturePosition(TableX, 80, TileID.Tables);
            AddFurniturePosition(TableX - 2, 80, TileID.Chairs);
            AddFurniturePosition(TableX + 2, 80, TileID.Chairs, FacingLeft: true);

            AddMonsterPosition(45 + 20, 80, DifficultyLevel.Normal);
            AddMonsterPosition(45 + 12, 80, DifficultyLevel.VeryEasy);
            AddMonsterPosition(45 + 28, 80, DifficultyLevel.VeryEasy);
        }
    }
}
