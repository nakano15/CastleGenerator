using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room04 : Room
    {
        public Room04()
        {
            RoomTileStartX = 184;
            RoomTileStartY = 47;
            Width = 16;
            Height = 13;

            AddConnector(189, 59, 6, ConnectorPosition.Down, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);
            //Wood
            AddTileInfo(170, 120, 84).SetTile(TileID.WoodBlock);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            //Glass
            AddTileInfo(200, 246, 254).SetTile(TileID.Glass);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);

            AddFurniturePosition(188, 57, TileID.Lamps);
            AddFurniturePosition(195, 57, TileID.Lamps);

            AddFurniturePosition(192, 57, TileID.Tables);
            AddFurniturePosition(194, 57, TileID.Chairs,FacingLeft: true);
            AddFurniturePosition(191, 55, TileID.Bottles, 3);

            AddTreasurePosition(187, 57);
        }
    }
}
