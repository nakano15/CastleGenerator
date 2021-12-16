using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class TallRoom02 : Room
    {
        public TallRoom02()
        {
            RoomTileStartX = 90;
            RoomTileStartY = 57;
            Width = 22;
            Height = 27;
            //Upper
            AddConnector(90, 60, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(111, 60, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);
            //Lower
            AddConnector(90, 74, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab);
            AddConnector(111, 74, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddMonsterPosition(101, 80);
            AddMonsterPosition(101, 70);

            AddFurniturePosition(94, 64, TileID.Lamps);
            AddFurniturePosition(107, 64, TileID.Lamps);
            AddFurniturePosition(99, 70, TileID.Lamps);
            AddFurniturePosition(102, 70, TileID.Lamps);
            AddFurniturePosition(96, 80, TileID.Lamps);
            AddFurniturePosition(105, 80, TileID.Lamps);
        }
    }
}
