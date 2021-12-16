using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class BossRoom01 : Room
    {
        public BossRoom01()
        {
            RoomTileStartX = 19;
            RoomTileStartY = 21;
            Width = 36;
            Height = 27;
            roomType = RoomType.BossRoom;

            AddConnector(19, 41, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(54, 26, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab, 2);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.BlueStainedGlass);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms).SetWall(WallID.EbonstoneBrick);

            //1st Row
            AddFurniturePosition(27, 40, TileID.Lamps);
            AddFurniturePosition(30, 40, TileID.Lamps);

            AddFurniturePosition(35, 40, TileID.Lamps);
            AddFurniturePosition(38, 40, TileID.Lamps);

            AddFurniturePosition(43, 40, TileID.Lamps);
            AddFurniturePosition(46, 40, TileID.Lamps);

            //2nd Row
            AddFurniturePosition(31, 35, TileID.Lamps);
            AddFurniturePosition(34, 35, TileID.Lamps);

            AddFurniturePosition(39, 35, TileID.Lamps);
            AddFurniturePosition(42, 35, TileID.Lamps);

            //3rd Row
            AddFurniturePosition(27, 29, TileID.Lamps);
            AddFurniturePosition(30, 29, TileID.Lamps);

            AddFurniturePosition(35, 29, TileID.Lamps);
            AddFurniturePosition(38, 29, TileID.Lamps);

            AddFurniturePosition(43, 29, TileID.Lamps);
            AddFurniturePosition(46, 29, TileID.Lamps);

            AddMonsterPosition(36, 45, DifficultyLevel.Normal);
        }
    }
}
