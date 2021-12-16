using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class Corridor07 : Room
    {
        public Corridor07()
        {
            RoomTileStartX = 175;
            RoomTileStartY = 105;
            Width = 25;
            Height = 11;

            AddConnector(175, 109, 5, ConnectorPosition.Left, ConnectorRequirement.Water, TileID.StoneSlab);
            AddConnector(199, 109, 5, ConnectorPosition.Right, ConnectorRequirement.Water, TileID.StoneSlab);

            AddTreasurePosition(187, 113, ConnectorRequirement.Water);

            AddMonsterPosition(187, 110, DifficultyLevel.Normal, 5);
        }
    }
}
