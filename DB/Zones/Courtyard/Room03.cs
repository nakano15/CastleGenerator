using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room03 : Room
    {
        public Room03()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 49;
            Width = 44;
            Height = 15;

            AddConnector(31, 49, 6, ConnectorPosition.Up, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(0, 57, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.BorealWood);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(35, 57, TileID.Lamps);
            AddFurniturePosition(32, 57, TileID.Lamps);

            AddFurniturePosition(22, 51, TileID.Chandeliers, 2);
            AddFurniturePosition(11, 51, TileID.Chandeliers, 2);

            AddFurniturePosition(22, 61, TileID.Chairs, FacingLeft: true);
            AddFurniturePosition(20, 61, TileID.Tables);
            AddFurniturePosition(19, 59, TileID.Bowls);
            AddFurniturePosition(8, 61, TileID.Benches, 1);
            AddFurniturePosition(12, 61, TileID.Bookcases, 1);

            AddTreasurePosition(34, 57);

            AddMonsterPosition(14, 61, DifficultyLevel.VeryEasy);
            AddMonsterPosition(34, 57, DifficultyLevel.VeryEasy);
            AddMonsterPosition(24, 61, DifficultyLevel.Normal);
        }
    }
}
