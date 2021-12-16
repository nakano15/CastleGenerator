using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class TallRoom01 : Room
    {
        public TallRoom01()
        {
            RoomTileStartX = 56;
            RoomTileStartY = 21;
            Width = 24;
            Height = 43;
            AddConnector(56, 26, 5, ConnectorPosition.Left, ConnectorRequirement.Flight, TileID.StoneSlab, 2);
            AddConnector(79, 26, 5, ConnectorPosition.Right, ConnectorRequirement.Flight, TileID.StoneSlab, 2);
            //
            AddConnector(56, 56, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(79, 56, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);

            AddFurniturePosition(62, 30, TileID.Lamps);
            AddFurniturePosition(73, 30, TileID.Lamps);

            AddTreasurePosition(60, 43, ConnectorRequirement.Flight);

            AddMonsterPosition(68, 60, DifficultyLevel.VeryHard);
        }
    }
}
