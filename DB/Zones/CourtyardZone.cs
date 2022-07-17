using CastleGenerator.DB.Zones.Courtyard;
using Terraria.ID;

namespace CastleGenerator.DB.Zones
{
    public class CourtyardZone : Zone
    {
        public CourtyardZone()
        {
            Name = "Courtyard";
            AddRoom(new Entrance01());
            AddRoom(new Corridor01());
            AddRoom(new BigRoom01());
            AddRoom(new TallStairway01());
            AddRoom(new Corridor02());
            AddRoom(new TallRoom01());
            AddRoom(new BossRoom01());
            AddRoom(new Room01());
            AddRoom(new Room02());
            AddRoom(new Stairway01());
            AddRoom(new Stairway02());
            AddRoom(new Corridor03());
            AddRoom(new Room03());
            AddRoom(new Room04());
            AddRoom(new Entrance02());
            AddRoom(new Entrance03());
            AddRoom(new TallRoom02());
            AddRoom(new Corridor04());
            AddRoom(new BigRoom02());
            AddRoom(new Corridor05());
            AddRoom(new Room05());
            AddRoom(new SaveRoom01());
            AddRoom(new SaveRoom02());
            AddRoom(new Room06());
            AddRoom(new BigRoom03());
            AddRoom(new Room07());
            AddRoom(new Corridor06());
            AddRoom(new Corridor07());
            AddRoom(new Room08());
            AddRoom(new TreasureRoom01());

            AddZoneMob(NPCID.Zombie, DifficultyLevel.VeryEasy, 30, 10, 0, 0.15f);
            AddZoneMob(NPCID.Skeleton, DifficultyLevel.Easy, 40, 25, 5, 0.2f);
            AddZoneMob(NPCID.Werewolf, DifficultyLevel.Normal, 80, 45, 10, 0.3f);
            AddZoneMob(NPCID.DemonEye, DifficultyLevel.Normal, 15, 7, 3, 0.2f);
            AddZoneMob(NPCID.CaveBat, DifficultyLevel.Easy, 10, 5, 0, 0.05f, "Bat");
            AddZoneMob(NPCID.CorruptBunny, DifficultyLevel.Easy, 50, 30, 6, 0.35f);
            AddZoneMob(NPCID.Mimic, DifficultyLevel.VeryHard, 200, 20, 10, 0.85f);
            AddZoneMob(NPCID.BoneThrowingSkeleton, DifficultyLevel.Hard, 35, 20, 3, 0.16f);
            AddZoneMob(NPCID.Piranha, DifficultyLevel.VeryEasy, 25, 7, 0, 0.2f, Aquatic: true);
            AddZoneMob(NPCID.PinkJellyfish, DifficultyLevel.Normal, 80, 23, 0, 0.35f, Name: "Jellyfish", Aquatic: true);
            AddZoneMob(NPCID.IcyMerman, DifficultyLevel.Hard, 200, 40, 0, 0.65f, Name: "Merman", Color: Microsoft.Xna.Framework.Color.DarkBlue, Aquatic: true);

            AddTileInfo(128, 128, 128).SetTile(TileID.StoneSlab);
            AddTileInfo(53, 53, 53).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(54, 59, 82).SetWall(WallID.BorealWood);
            AddTileInfo(69, 50, 37).SetWall(WallID.Wood);
            AddTileInfo(54, 89, 98).SetWall(WallID.Glass);
            AddTileInfo(114, 120, 45).SetTile(TileID.Platforms, 0, 43 * 18).SetWall(WallID.EbonstoneBrick);
            AddTileInfo(78, 80, 114).SetWall(WallID.EbonstoneBrick).SetLiquid(0, 255);
        }
    }
}
