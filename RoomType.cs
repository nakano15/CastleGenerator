using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleGenerator
{
    public enum RoomType : byte
    {
        Normal,
        Spawn,
        SaveRoom,
        BossRoom,
        Corridor
    }
}
