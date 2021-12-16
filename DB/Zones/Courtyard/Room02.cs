using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room02 : Room
    {
        public Room02()
        {
            RoomTileStartX = 165;
            RoomTileStartY = 37;
            Width = 18;
            Height = 19;

            AddConnector(182, 48, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(165, 48, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.Stone);
            AddTileInfo(255, 63, 63).SetWall(WallID.RedBrick);

            AddFurniturePosition(172, 52, TileID.Lamps);
            AddFurniturePosition(175, 52, TileID.Lamps);

            AddTreasurePosition(174, 52);

            AddMonsterPosition(174, 52, DifficultyLevel.Easy);
        }
    }
}
