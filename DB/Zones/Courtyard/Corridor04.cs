using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Corridor04 : Room
    {
        public Corridor04()
        {
            RoomTileStartX = 113;
            RoomTileStartY = 57;
            roomType = RoomType.Corridor;
            Width = 44;
            Height = 19;

            AddConnector(113, 67, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(156, 67, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);

            AddFurniturePosition(115, 66, TileID.Torches);
            AddFurniturePosition(154, 66, TileID.Torches);

            AddFurniturePosition(127, 59, TileID.Chandeliers, 2);
            AddFurniturePosition(140, 59, TileID.Chandeliers, 2);

            AddFurniturePosition(124, 72, TileID.Benches);

            AddFurniturePosition(121, 72, TileID.Lampposts);
            AddFurniturePosition(134, 72, TileID.Lampposts);
            AddFurniturePosition(148, 72, TileID.Lampposts);

            AddMonsterPosition(128, 72, DifficultyLevel.VeryEasy);
            AddMonsterPosition(140, 72, DifficultyLevel.VeryEasy);
            AddMonsterPosition(134, 72, DifficultyLevel.Normal);
        }
    }
}
