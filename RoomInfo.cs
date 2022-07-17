using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CastleGenerator
{
    public class RoomInfo
    {
        public string ZoneName = "";
        public int ZoneID = 0;
        public int RoomID = 0;
        public int RoomX = 0, RoomY = 0;
        public float Difficulty = 0;
        private Zone _Zone = null;
        public Zone GetZone { get { if (_Zone == null) { FindZone(); } return _Zone; } }
        public Room GetRoom { get { return GetZone.RoomTypes[RoomID]; } }
        public List<MobSlot> Mobs = new List<MobSlot>();
        public List<TreasureSlot> Treasures = new List<TreasureSlot>();
        public bool Visited = false;

        private void FindZone()
        {
            foreach(Zone z in MainMod.ZoneTypes)
            {
                if(z.Name == ZoneName)
                {
                    _Zone = z;
                    return;
                }
            }
            _Zone = MainMod.InvalidZone;
        }

        public static void Save(TagCompound tag)
        {
            tag.Add("RoomCount", WorldMod.Rooms.Count);
            for(int i = 0; i < WorldMod.Rooms.Count; i++)
            {
                RoomInfo ri = WorldMod.Rooms[i];
                tag.Add("RoomZoneName_" + i, ri.ZoneName);
                tag.Add("RoomZoneID_" + i, ri.ZoneID);
                tag.Add("RoomID_" + i, ri.RoomID);
                tag.Add("RoomDifficulty_" + i, ri.Difficulty);
                tag.Add("RoomX_" + i, ri.RoomX);
                tag.Add("RoomY_" + i, ri.RoomY);
                tag.Add("RoomMobCount_" + i, ri.Mobs.Count);
                for (int j = 0; j < ri.Mobs.Count; j++)
                {
                    tag.Add("RoomMobID_" + i + "|" + j, ri.Mobs[j].MobID);
                    tag.Add("RoomMobSlot_" + i + "|" + j, ri.Mobs[j].Slot);
                }
                tag.Add("RoomTreasureCount_" + i, ri.Treasures.Count);
                for (int j = 0; j < ri.Treasures.Count; j++)
                {
                    tag.Add("RoomTreasureID_" + i + "|" + j, ri.Treasures[j].ItemID);
                    tag.Add("RoomTreasureSlot_" + i + "|" + j, ri.Treasures[j].Slot);
                }
            }
        }

        public static void Load(TagCompound tag, int ModVersion)
        {
            WorldMod.Rooms.Clear();
            int Rooms = tag.GetInt("RoomCount");
            for(int i = 0; i < Rooms; i++)
            {
                RoomInfo ri = new RoomInfo();
                ri.ZoneName = tag.GetString("RoomZoneName_" + i);
                ri.ZoneID = tag.GetInt("RoomZoneID_" + i);
                ri.RoomID = tag.GetInt("RoomID_" + i);
                ri.Difficulty = tag.GetFloat("RoomDifficulty_" + i);
                ri.RoomX = tag.GetInt("RoomX_" + i);
                ri.RoomY = tag.GetInt("RoomY_" + i);
                if (ModVersion >= 1)
                {
                    int MobCount = tag.GetInt("RoomMobCount_" + i);
                    for (int j = 0; j < MobCount; j++)
                    {
                        int MobID = tag.GetInt("RoomMobID_" + i + "|" + j);
                        int MobSlot = tag.GetInt("RoomMobSlot_" + i + "|" + j);
                        ri.Mobs.Add(new MobSlot() { MobID = MobID, Slot = MobSlot });
                    }
                }
                if (ModVersion >= 2)
                {
                    int TreasureCount = tag.GetInt("RoomTreasureCount_" + i);
                    for (int j = 0; j < TreasureCount; j++)
                    {
                        int ItemID = tag.GetInt("RoomTreasureID_" + i + "|" + j);
                        int ItemSlot = tag.GetInt("RoomTreasurelot_" + i + "|" + j);
                        ri.Treasures.Add(new TreasureSlot() { ItemID = ItemID, Slot = ItemSlot });
                    }
                }
                WorldMod.Rooms.Add(ri);
            }
        }

        public struct MobSlot
        {
            public int MobID, Slot;
        }

        public struct TreasureSlot
        {
            public int ItemID, Slot;
        }
    }
}
