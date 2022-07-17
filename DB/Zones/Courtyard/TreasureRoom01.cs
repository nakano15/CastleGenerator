using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class TreasureRoom01 : Room
    {
        public TreasureRoom01()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 105;
            Width = 19;
            Height = 12;
            roomType = RoomType.Treasure;

            AddConnector(18, 110, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(0, 110, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.Stone);
            AddTileInfo(107, 91, 34).SetWall(WallID.GoldBrick);

            AddTreasurePosition(8, 112);

            AddFurniturePosition(4, 107, TileID.Chandeliers, 2);
            AddFurniturePosition(14, 107, TileID.Chandeliers, 2);
        }
    }
}
