using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CastleGenerator
{
    public class ItemMod : GlobalItem
    {
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (WorldMod.IsCastle)
                grabRange = 0;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if(WorldMod.IsCastle && PlayerMod.ItemsPickupCreated.Contains(item.whoAmI))
            {
                gravity = 0;
                maxFallSpeed = 0;
                item.velocity = Vector2.Zero;
            }
        }

        public override bool CanPickup(Item item, Player player)
        {
            if (WorldMod.IsCastle)
            {
                if(player.whoAmI == Main.myPlayer || !PlayerMod.ItemsPickupCreated.Contains(item.whoAmI))
                {
                    return true;
                }
                return false;
            }
            return base.CanPickup(item, player);
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (WorldMod.IsCastle && player.whoAmI == Main.myPlayer)
            {
                for(int i = 0; i < PlayerMod.ItemsPickupCreated.Count; i++)
                {
                    if(PlayerMod.ItemsPickupCreated[i] == item.whoAmI)
                    {
                        PlayerMod pl = Main.LocalPlayer.GetModPlayer<PlayerMod>();
                        RoomInfo ri = pl.MyRoom;
                        RoomInfo.TreasureSlot slot = ri.Treasures[i];
                        pl.pwi.TreasuresGot.Add(new Point(ri.RoomX + pl.MyRoom.GetRoom.TreasureSpawnPosition[slot.Slot].PositionX - ri.GetRoom.RoomTileStartX, 
                            ri.RoomY + pl.MyRoom.GetRoom.TreasureSpawnPosition[slot.Slot].PositionY - ri.GetRoom.RoomTileStartY));
                    }
                }
            }
            return base.OnPickup(item, player);
        }
    }
}
