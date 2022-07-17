using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleGenerator
{
    public struct Loot
    {
        public int ItemID;
        public LootType ltype;
        public DifficultyLevel Difficulty;

        public Loot(int ItemID, LootType type = LootType.Normal, DifficultyLevel Difficulty = DifficultyLevel.VeryEasy)
        {
            this.ItemID = ItemID;
            ltype = type;
            this.Difficulty = Difficulty;
        }

        public enum LootType : byte
        {
            Normal,
            DoubleJump,
            Flight,
            Teleport,
            Waterbreathing,
            Lavaswimming
        }
    }
}
