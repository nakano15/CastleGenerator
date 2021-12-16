using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Corridor06 : Room
    {
        public Corridor06()
        {
            RoomTileStartX = 175;
            RoomTileStartY = 93;
            Width = 25;
            Height = 11;

            AddConnector(175, 96, 5, ConnectorPosition.Left, ConnectorRequirement.Water, TileID.StoneSlab);
            AddConnector(199, 96, 5, ConnectorPosition.Right, ConnectorRequirement.Water, TileID.StoneSlab);

            AddMonsterPosition(187, 98, DifficultyLevel.Hard, 5);
        }
    }
}
