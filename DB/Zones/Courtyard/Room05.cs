using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Room05 : Room
    {
        public Room05()
        {
            RoomTileStartX = 0;
            RoomTileStartY = 85;
            roomType = RoomType.Corridor;
            Width = 44;
            Height = 19;

            AddConnector(43, 95, 5, ConnectorPosition.Right, ConnectorRequirement.None, TileID.StoneSlab);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(28, 216, 94).SetTile(TileID.Grass);
            AddTileInfo(151, 107, 75).SetTile(TileID.Dirt);

            AddTileInfo(170, 120, 84).SetTile(TileID.WoodBlock).SetWall(WallID.Wood);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(141, 121, 111).SetTile(TileID.Iron);

            AddFurniturePosition(6, 96, TileID.Torches);
            AddFurniturePosition(18, 96, TileID.Torches);

            AddFurniturePosition(28, 96, TileID.Lamps);

            AddFurniturePosition(41, 97, TileID.Torches);
            AddFurniturePosition(2, 95, TileID.Torches);

            AddMonsterPosition(32, 100);
            AddMonsterPosition(24, 100, DifficultyLevel.Hard);
            AddMonsterPosition(4, 100);
        }
    }
}
