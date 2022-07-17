using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CastleGenerator
{
    public class PlayerMod : ModPlayer
    {
        public override bool CloneNewInstances => false;
        public RoomInfo MyRoom = null;
        private bool JustSpawned = false;
        public PlayerWorldInfo pwi = new PlayerWorldInfo();

        public override void PreUpdateBuffs()
        {
            if (WorldMod.IsCastle)
                player.AddBuff(Terraria.ID.BuffID.NoBuilding, 5);
        }

        public override void ResetEffects()
        {
            if (!WorldMod.IsCastle)
                return;
            player.statLifeMax2 = 100 + 20 * pwi.LifeGot.Count;
        }

        public override void OnRespawn(Player player)
        {
            JustSpawned = true;
        }

        public override void OnEnterWorld(Player player)
        {
            pwi = new PlayerWorldInfo(true);
            JustSpawned = true;
            if(player.whoAmI == Main.myPlayer && SubworldLibrary.SLWorld.subworld && SubworldLibrary.SLWorld.currentSubworld is CastleSubworld)
            {
                Main.Map.Clear();
            }
        }

        public override void PostSavePlayer()
        {
            pwi.Save();
        }

        public override void PostUpdate()
        {
            int TileX = (int)(player.Center.X * (1f / 16));
            int TileY = (int)(player.Center.Y * (1f / 16));
            RoomInfo NewRoom = MyRoom;
            foreach(RoomInfo ri in WorldMod.Rooms)
            {
                Room r = ri.GetRoom;
                if(TileX >= ri.RoomX && TileX < ri.RoomX + r.Width && 
                    TileY >= ri.RoomY && TileY < ri.RoomY + r.Height)
                {
                    NewRoom = ri;
                    break;
                }
            }
            if (player.whoAmI == Main.myPlayer && NewRoom != MyRoom)
            {
                //Clean Up stuff from last room, and create stuff for new room.
                ChangeRoom(NewRoom);
            }
            MyRoom = NewRoom;
            if (player.whoAmI == Main.myPlayer)
            {
                const float DivisionBy16 = 1f / 16;
                int MinCheckX = (int)(player.position.X * DivisionBy16), MaxCheckX = (int)((player.position.X + player.width) * DivisionBy16);
                int MinCheckY = (int)(player.position.Y * DivisionBy16), MaxCheckY = (int)((player.position.Y + player.height) * DivisionBy16);
                for (int y = MinCheckY; y < MaxCheckY; y++)
                {
                    for (int x = MinCheckX; x < MaxCheckX; x++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.active())
                            continue;
                        if (tile.type == Terraria.ID.TileID.Heart)
                        {
                            Point TileBottom = new Point(x, y);
                            if (tile.frameX == 0)
                                TileBottom.X++;
                            if (tile.frameY * (1f / 18) % 2 == 0)
                                TileBottom.Y++;
                            if (!pwi.LifeGot.Contains(TileBottom))
                            {
                                pwi.LifeGot.Add(TileBottom);
                                CombatText.NewText(player.getRect(), Color.Green, "Life Max Up", true);
                                player.statLife = player.statLifeMax2 + 20;
                                WorldGen.KillTile(x, y, false, false, true);
                            }
                        }
                    }
                }
            }
        }

        private void ChangeRoom(RoomInfo NewRoom)
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].townNPC)
                {
                    Main.npc[i].active = false;
                }
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].hostile)
                    Main.projectile[i].active = false;
            }
            for (int i = 0; i < Main.maxItems; i++)
            {
                Main.item[i].active = false;
            }
            ItemsPickupCreated.Clear();
            MainMod.ScreenBlackoutTime = 8;
            //Main.NewText("Mobs in this room: " + NewRoom.Mobs.Count);
            if (JustSpawned)
            {
                JustSpawned = false;
            }
            else
            {
                foreach (RoomInfo.MobSlot mob in NewRoom.Mobs)
                {
                    ZoneMobDefinition mobdefinition = NewRoom.GetZone.ZoneMobs[mob.MobID];
                    MobSpawnPos spawnpos = NewRoom.GetRoom.MobSpawnPosition[mob.Slot];
                    int SpawnX = NewRoom.RoomX + spawnpos.PositionX - NewRoom.GetRoom.RoomTileStartX,
                        SpawnY = NewRoom.RoomY + spawnpos.PositionY - NewRoom.GetRoom.RoomTileStartY;
                    int NpcSpawnPos = NPC.NewNPC(SpawnX * 16 + 8, SpawnY * 16 + 16, mobdefinition.MobID);
                    if (NpcSpawnPos > -1)
                    {
                        NPC npc = Main.npc[NpcSpawnPos];
                        npc.life = npc.lifeMax = mobdefinition.Health;
                        npc.damage = npc.defDamage = mobdefinition.Damage;
                        npc.defense = npc.defDefense = mobdefinition.Defense;
                        npc.knockBackResist = 1f - mobdefinition.KBRes;
                    }
                }
            }
            foreach (RoomInfo.TreasureSlot treasure in NewRoom.Treasures)
            {
                int ItemID = treasure.ItemID;
                TreasureSpawnPos spawnpos = NewRoom.GetRoom.TreasureSpawnPosition[treasure.Slot];
                Point treasurePosition = new Point(NewRoom.RoomX + spawnpos.PositionX - NewRoom.GetRoom.RoomTileStartX,
                    NewRoom.RoomY + spawnpos.PositionY - NewRoom.GetRoom.RoomTileStartY);
                if (ItemID == Terraria.ID.ItemID.LifeCrystal)
                {
                    if (pwi.LifeGot.Contains(treasurePosition))
                    {
                        for (int y = -1; y < 1; y++)
                        {
                            for (int x = -1; x < 1; x++)
                            {
                                Main.tile[x + treasurePosition.X, treasurePosition.Y + y].active(false);
                            }
                        }
                    }
                    else
                    {
                        WorldGen.Place2x2(treasurePosition.X, treasurePosition.Y, Terraria.ID.TileID.Heart, 0);
                    }
                }
                else
                {
                    if (!pwi.TreasuresGot.Contains(treasurePosition))
                    {
                        ItemsPickupCreated.Add(Item.NewItem(new Vector2(treasurePosition.X * 16, treasurePosition.Y * 16), ItemID, noBroadcast: true, noGrabDelay: true));
                    }
                    /*for (int y = -1; y < 1; y++)
                    {
                        for (int x = -1; x < 1; x++)
                        {
                            Main.tile[treasurePosition.X + x, treasurePosition.Y + y].active(false);
                        }
                    }
                    WorldGen.PlaceTile(treasurePosition.X, treasurePosition.Y, Terraria.ID.TileID.Containers);
                    if (pwi.TreasuresGot.Contains(treasurePosition))
                    {
                        Main.NewText("  Got!");
                        for (int y = -1; y < 1; y++)
                        {
                            for (int x = -1; x < 1; x++)
                            {
                                Main.tile[treasurePosition.X + x, treasurePosition.Y + y].frameY += 36 * 2;
                            }
                        }
                    }*/
                }
            }
        }

        public static List<int> ItemsPickupCreated = new List<int>();

        public override void ModifyScreenPosition()
        {
            if (!WorldMod.IsCastle || MyRoom == null)
                return;
            float Scale = 1f / MainMod.Zoom;
            float ScreenWidth = Main.screenWidth, ScreenHeight = Main.screenHeight;
            float LeftEdge = MyRoom.RoomX * 16;
            float RightEdge = (MyRoom.RoomX + MyRoom.GetRoom.Width) * 16;
            float TopEdge = MyRoom.RoomY * 16;
            float BottomEdge = (MyRoom.RoomY + MyRoom.GetRoom.Height) * 16;
            //Horizontal
            float WidthDiscount = (ScreenWidth * Scale) * 0.5f;
            if (RightEdge - LeftEdge < ScreenWidth * Scale)
            {
                Main.screenPosition.X = LeftEdge + (RightEdge - LeftEdge - ScreenWidth) * 0.5f;
            }
            else
            {
                if (Main.screenPosition.X < LeftEdge - WidthDiscount)
                    Main.screenPosition.X = LeftEdge - WidthDiscount;
                if (Main.screenPosition.X + ScreenWidth > RightEdge + WidthDiscount)
                    Main.screenPosition.X = RightEdge + WidthDiscount - ScreenWidth;
            }
            //Vertical
            if (BottomEdge - TopEdge < ScreenHeight * Scale)
            {
                Main.screenPosition.Y = TopEdge + (BottomEdge - TopEdge - ScreenHeight) * 0.5f;
            }
            else
            {
                float HeightDiscount = (ScreenHeight * Scale) * 0.5f;
                if (Main.screenPosition.Y < TopEdge - HeightDiscount)
                    Main.screenPosition.Y = TopEdge - HeightDiscount;
                if (Main.screenPosition.Y + ScreenHeight > BottomEdge + HeightDiscount)
                    Main.screenPosition.Y = BottomEdge + HeightDiscount - ScreenHeight;
            }
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            if (WorldMod.IsCastle)
                return false;
            return base.ConsumeAmmo(weapon, ammo);
        }

        public override bool PreItemCheck()
        {
            if (!WorldMod.IsCastle)
                return true;
            Item item = player.inventory[player.selectedItem];
            switch (item.type)
            {
                case Terraria.ID.ItemID.Dynamite:
                case Terraria.ID.ItemID.Bomb:
                case Terraria.ID.ItemID.HoneyBucket:
                case Terraria.ID.ItemID.LavaBucket:
                case Terraria.ID.ItemID.WaterBucket:
                case Terraria.ID.ItemID.BottomlessBucket:
                case Terraria.ID.ItemID.ActuationRod:
                case Terraria.ID.ItemID.WireCutter:
                case Terraria.ID.ItemID.BlueWrench:
                case Terraria.ID.ItemID.GreenWrench:
                case Terraria.ID.ItemID.MulticolorWrench:
                case Terraria.ID.ItemID.YellowWrench:
                case Terraria.ID.ItemID.Actuator:
                    return false;
            }
            return item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.createTile == -1 && item.createWall == -1;
        }
    }
}
