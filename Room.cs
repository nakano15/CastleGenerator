using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CastleGenerator
{
    public class Room
    {
        public Zone ParentZone;
        public int ID;
        public byte Width = 20, Height = 15;
        public RoomType roomType = RoomType.Normal;
        public List<Connector> RoomConnectors = new List<Connector>();
        public virtual string GetRoomMap { get; }
        public List<TileInfo> RoomMapCodes = new List<TileInfo>();
        public byte PlayerSpawnX = 0, PlayerSpawnY = 0;
        public int RoomTileStartX = 0, RoomTileStartY = 0;
        public List<MobSpawnPos> MobSpawnPosition = new List<MobSpawnPos>();
        public List<TreasureSpawnPos> TreasureSpawnPosition = new List<TreasureSpawnPos>();
        public List<FurnitureSpawnPos> FurnitureSpawnPosition = new List<FurnitureSpawnPos>();
        public bool SurfaceRoom = false;

        public override string ToString()
        {
            return GetType().Name;
        }

        public void AddConnector(byte PosX, byte PosY, byte TileDistance, ConnectorPosition ExtremityPos, ConnectorRequirement GearRequirement, ushort BlockTileID = Terraria.ID.TileID.Stone, byte BlockDimension = 2)
        {
            this.RoomConnectors.Add(new Connector(PosX, PosY, TileDistance, ExtremityPos, GearRequirement, BlockTileID, BlockDimension));
        }

        public TileInfo AddTileInfo(byte CodeR, byte CodeG, byte CodeB, byte CodeA = 255)
        {
            TileInfo Ti = new TileInfo(CodeR, CodeG, CodeB, CodeA);
            RoomMapCodes.Add(Ti);
            return Ti;
        }

        public void AddMonsterPosition(int PositionX, int PositionY, DifficultyLevel difficulty = DifficultyLevel.Easy, byte Height = 9)
        {
            MobSpawnPosition.Add(new MobSpawnPos() { PositionX = PositionX, PositionY = PositionY, Difficulty = difficulty, Height = Height });
        }

        public void AddTreasurePosition(int PositionX, int PositionY, ConnectorRequirement Requirement = ConnectorRequirement.None)
        {
            TreasureSpawnPosition.Add(new TreasureSpawnPos() { PositionX = PositionX, PositionY = PositionY, Requirement = Requirement });
        }

        public void AddFurniturePosition(int PositionX, int PositionY, ushort FurnitureType, byte Style = 0, bool FacingLeft = false)
        {
            FurnitureSpawnPosition.Add(new FurnitureSpawnPos() { PositionX = PositionX, PositionY = PositionY, FurnitureID = FurnitureType, Style = Style, FacingLeft = FacingLeft });
        }
    }
}
