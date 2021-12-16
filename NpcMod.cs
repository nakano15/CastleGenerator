using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CastleGenerator
{
    public class NpcMod : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => false;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!WorldMod.IsCastle)
                return;
            pool.Clear();
        }

        public override bool PreNPCLoot(NPC npc)
        {
            if (!WorldMod.IsCastle)
                return true;
            if(Main.rand.Next(5) == 0)
            {
                int Value = 1;
                float RandomValue = Main.rand.NextFloat() * npc.lifeMax;
                if (RandomValue >= 1000)
                    Value = 1000;
                else if (RandomValue >= 500)
                    Value = 500;
                else if (RandomValue >= 250)
                    Value = 250;
                else if (RandomValue >= 100)
                    Value = 100;
                else if (RandomValue >= 50)
                    Value = 50;
                else if (RandomValue >= 25)
                    Value = 25;
                else if (RandomValue >= 10)
                    Value = 10;
                int c = Value, s = 0;
                if(c >= 100)
                {
                    s += c / 100;
                    c -= s * 100;
                }
                if (s > 0)
                {
                    Item.NewItem(npc.getRect(), Terraria.ID.ItemID.SilverCoin, s);
                }
                if (c > 0)
                {
                    Item.NewItem(npc.getRect(), Terraria.ID.ItemID.CopperCoin, c);
                }
            }
            return false;
        }
    }
}
