using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Corridor02 : Room
    {
        public Corridor02()
        {
            RoomTileStartX = 81;
            RoomTileStartY = 21;
            Width = 41;
            Height = 15;
            roomType = RoomType.Corridor;
            AddConnector(81, 26, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(121, 26, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);

            AddFurniturePosition(83, 25, TileID.Torches);
            AddFurniturePosition(119, 25, TileID.Torches);

            AddFurniturePosition(92, 23, TileID.Chandeliers, 2);
            AddFurniturePosition(108, 23, TileID.Chandeliers, 2);

            AddTreasurePosition(99, 32);
        }
    }
}
