using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CastleGenerator
{
    public class TileMod : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (WorldMod.IsCastle)
                return false;
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (WorldMod.IsCastle)
                noItem = true;
        }

        public override void RightClick(int i, int j, int type)
        {
            if (!WorldMod.IsCastle)
                return;
            if(type == Terraria.ID.TileID.Containers || type == Terraria.ID.TileID.Containers2)
            {
                Tile tile = Main.tile[i, j];
                if(tile.TileFrameY < 36)
                {
                    PlayerMod player = Main.LocalPlayer.GetModPlayer<PlayerMod>();
                    Point TileBottom = new Point(i, j);
                    if (tile.TileFrameX % (1f / 18) == 0)
                        TileBottom.X++;
                    if (tile.TileFrameY % (1f / 18) == 0)
                        TileBottom.Y++;
                    if (!player.pwi.TreasuresGot.Contains(TileBottom))
                    {
                        bool HasInventorySpace = false;
                        for(int e = 0; e < 50; e++)
                        {
                            if(player.Player.inventory[e].type == 0)
                            {
                                HasInventorySpace = true;
                                break;
                            }
                        }
                        if (!HasInventorySpace)
                        {
                            CombatText.NewText(player.Player.getRect(), Color.Red, "Inventory is full!");
                        }
                        else
                        {
                            RoomInfo room = player.MyRoom;
                            if (room == null) return;
                            Room baseRoom = room.GetRoom;
                            foreach (RoomInfo.TreasureSlot treasure in room.Treasures)
                            {
                                int TreasureX = room.RoomX + baseRoom.TreasureSpawnPosition[treasure.Slot].PositionX - baseRoom.RoomTileStartX;
                                int TreasureY = room.RoomY + baseRoom.TreasureSpawnPosition[treasure.Slot].PositionY - baseRoom.RoomTileStartY;
                                if (TreasureX == TileBottom.X && TreasureY == TileBottom.Y)
                                {
                                    Item.NewItem(Item.GetSource_NaturalSpawn(), player.Player.Center, treasure.ItemID);
                                    break;
                                }
                            }
                            player.pwi.TreasuresGot.Add(TileBottom);
                        }
                    }
                }
            }
        }
    }
}
