using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room01 : Room
    {
        public Room01()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 21;
            Width = 18;
            Height = 27;

            AddConnector(17, 41, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(4, 47, 6, ConnectorPosition.Down, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.Stone);
            AddTileInfo(255, 63, 63).SetWall(WallID.RedBrick);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddTreasurePosition(4, 30, ConnectorRequirement.Flight);
            AddTreasurePosition(14, 30, ConnectorRequirement.Flight);

            AddFurniturePosition(15, 40, TileID.Torches);
            AddFurniturePosition(2, 40, TileID.Torches);
            AddFurniturePosition(15, 28, TileID.Torches);
            AddFurniturePosition(2, 28, TileID.Torches);
        }
    }
}
