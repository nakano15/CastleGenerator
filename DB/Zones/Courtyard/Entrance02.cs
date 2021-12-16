using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Entrance02 : Room
    {
        public Entrance02()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 65;
            roomType = RoomType.Spawn;
            Width = 44;
            Height = 19;
            SurfaceRoom = true;
            AddConnector(43, 75, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            PlayerSpawnX = 5;
            PlayerSpawnY = 80;

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);
            AddTileInfo(141, 121, 111).SetTile(TileID.WoodBlock);

            AddFurniturePosition(8, 80, TileID.Lamps);
            AddFurniturePosition(16, 80, TileID.Lamps);
            AddFurniturePosition(24, 80, TileID.Lamps);
            AddFurniturePosition(36, 80, TileID.Lamps);

            AddFurniturePosition(12, 80, TileID.Benches);

            int TableX = 29;
            AddFurniturePosition(TableX, 80, TileID.Tables);
            AddFurniturePosition(TableX - 2, 80, TileID.Chairs);
            AddFurniturePosition(TableX + 2, 80, TileID.Chairs, FacingLeft:true);

            AddMonsterPosition(20, 80, DifficultyLevel.Normal);
            AddMonsterPosition(12, 80, DifficultyLevel.VeryEasy);
            AddMonsterPosition(28, 80, DifficultyLevel.VeryEasy);
        }
    }
}
