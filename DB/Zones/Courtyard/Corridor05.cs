using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Corridor05 : Room
    {
        public Corridor05()
        {
            RoomTileStartX = 117;
            RoomTileStartY = 77;
            roomType = RoomType.Corridor;
            Width = 40;
            Height = 13;

            AddConnector(117, 82, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(156, 82, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(133, 77, 6, ConnectorPosition.Up, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(122, 79, TileID.Chandeliers, 2);
            AddFurniturePosition(151, 79, TileID.Chandeliers, 2);

            AddFurniturePosition(138, 82, TileID.Lamps);

            AddFurniturePosition(142, 82, TileID.Benches, 1);
            AddFurniturePosition(140, 82, TileID.GrandfatherClocks);
            AddFurniturePosition(130, 82, TileID.Bookcases);

            AddTreasurePosition(136, 82);

            AddMonsterPosition(136, 82, DifficultyLevel.Hard, 4);
        }
    }
}
