using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleGenerator
{
    public class ZoneMobDefinition
    {
        public int MobID = 0;
        public string Name = "";
        public int Health = 100, Damage = 5, Defense = 0;
        public float KBRes = 0.05f;
        public DifficultyLevel Difficulty = DifficultyLevel.VeryEasy;
        public Microsoft.Xna.Framework.Color? Color = null;
        public bool Aquatic = false, Nocturnal = false;
    }
}
