using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Stairway01 : Room
    {
        public Stairway01()
        {
            RoomTileStartX = 123;
            RoomTileStartY = 37;
            Width = 41;
            Height = 19;
            roomType = RoomType.Corridor;

            AddConnector(163, 48, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(123, 39, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(126, 43, TileID.Lamps);
            AddFurniturePosition(134, 45, TileID.Lamps);
            AddFurniturePosition(142, 47, TileID.Lamps);
            AddFurniturePosition(150, 49, TileID.Lamps);
            AddFurniturePosition(158, 51, TileID.Lamps);

            AddMonsterPosition(154, 50);
            AddMonsterPosition(142, 47, DifficultyLevel.Normal);
            AddMonsterPosition(134, 45);
        }
    }
}
