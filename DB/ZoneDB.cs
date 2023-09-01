using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CastleGenerator.DB.Zones;

namespace CastleGenerator.DB
{
    public class ZoneDB
    {
        public static void CreateZoneList()
        {
            AddZone(new CourtyardZone());
        }

        public static void AddZone(Zone zone)
        {
            zone.ID = MainMod.ZoneTypes.Count;
            zone.LoadZoneTiles();
            MainMod.ZoneTypes.Add(zone);
        }
    }
}
