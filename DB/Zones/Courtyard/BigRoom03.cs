using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace CastleGenerator.DB.Zones.Courtyard
{
    public class BigRoom03 : Room
    {
        public BigRoom03()
        {
            RoomTileStartX = 117;
            RoomTileStartY = 91;
            Width = 40;
            Height = 25;

            AddConnector(117, 97, 5, ConnectorPosition.Left, ConnectorRequirement.None, TileID.StoneSlab, 2);
            AddConnector(117, 109, 5, ConnectorPosition.Left, ConnectorRequirement.Water, TileID.StoneSlab, 2);
            AddConnector(156, 109, 5, ConnectorPosition.Right, ConnectorRequirement.Water, TileID.StoneSlab, 2);

            AddMonsterPosition(141, 99, DifficultyLevel.Easy, 3);
            AddMonsterPosition(105, 103, DifficultyLevel.Normal, 3);
            AddMonsterPosition(128, 111, DifficultyLevel.Easy, 3);
            AddMonsterPosition(150, 111, DifficultyLevel.Easy, 3);
            AddMonsterPosition(137, 111, DifficultyLevel.Hard, 3);

            AddTreasurePosition(146, 97, ConnectorRequirement.Water);
        }
    }
}
