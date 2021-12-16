using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    class Corridor03 : Room
    {
        public Corridor03()
        {
            RoomTileStartX = 45;
            RoomTileStartY = 49;
            roomType = RoomType.Corridor;
            Width = 10;
            Height = 15;
            //Left Rooms
            AddConnector(47, 49, 6, ConnectorPosition.Up, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(47, 63, 6, ConnectorPosition.Down, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(200, 246, 254).SetTile(TileID.Glass);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(47, 55, TileID.Torches);
            AddFurniturePosition(52, 55, TileID.Torches);
        }
    }
}
