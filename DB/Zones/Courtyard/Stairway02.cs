using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Stairway02 : Room
    {
        public Stairway02()
        {
            RoomTileStartX = 81;
            RoomTileStartY = 37;
            Width = 41;
            Height = 19;
            roomType = RoomType.Corridor;

            AddConnector(121, 39, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(81, 48, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(118, 43, TileID.Lamps);
            AddFurniturePosition(110, 45, TileID.Lamps);
            AddFurniturePosition(102, 47, TileID.Lamps);
            AddFurniturePosition(94, 49, TileID.Lamps);
            AddFurniturePosition(86, 51, TileID.Lamps);

            AddMonsterPosition(110, 45, DifficultyLevel.Easy);
            AddMonsterPosition(102, 47, DifficultyLevel.Normal);
            AddMonsterPosition(94, 49, DifficultyLevel.VeryEasy);
        }
    }
}
