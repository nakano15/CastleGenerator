using Terraria.GameContent;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Graphics;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using System;

namespace CastleGenerator
{
	public class MainMod : Mod
	{
        public static Mod mod;
        public const int ModVersion = 2;
        public static List<Zone> ZoneTypes = new List<Zone>();
        public static Zone InvalidZone;
        public static LegacyGameInterfaceLayer MapBordersInterfaceLayer, DebugInfoLayer;
        public static float Zoom = 2;
        public static byte ScreenBlackoutTime = 0;
        public const string IsCastleCallString = "IsCastle";
        private static Mod TerraGuardiansMod;
        private static Func<Player, bool> TgCheckIsCompanion = null;

        public override object Call(params object[] args)
        {
            if(args.Length > 0 && args[0] is string)
            {
                switch ((string)args[0])
                {
                    case IsCastleCallString:
                        return WorldMod.IsCastle;
                }
            }
            return base.Call(args);
        }

        public override void Load()
        {
            mod = this;
            InvalidZone = Zone.CreateInvalidZone();
            DB.ZoneDB.CreateZoneList();
            if (!Main.dedServ)
            {
                MapBordersInterfaceLayer = new LegacyGameInterfaceLayer("Castle Generator: Map Borders", DrawMapBorders, InterfaceScaleType.Game);
                DebugInfoLayer = new LegacyGameInterfaceLayer("Castle Generator: Debug Info", DebugDisplay, InterfaceScaleType.UI);
            }
        }

        public override void Unload()
        {
            foreach(Zone z in ZoneTypes) z.UnloadZone();
            ZoneTypes.Clear();
            ZoneTypes = null;
            InvalidZone.UnloadZone();
            InvalidZone = null;
            TerraGuardiansMod = null;
            TgCheckIsCompanion = null;
        }

        public static bool IsPC(Player player)
        {
            if (TerraGuardiansMod != null)
                return !TgCheckIsCompanion(player);
            return true;
        }

        public override void PostSetupContent()
        {
            if (ModLoader.HasMod("terraguardians"))
            {
                TerraGuardiansMod = ModLoader.GetMod("terraguardians");
                TgCheckIsCompanion = (Func<Player, bool>)TerraGuardiansMod.Call("IsCompanionDelegate");
            }
        }

        private static bool DrawMapBorders()
        {
            if (WorldMod.Rooms.Count == 0)
                return true;
            RoomInfo ri = Main.LocalPlayer.GetModPlayer<PlayerMod>().MyRoom;
            if(ri == null || ScreenBlackoutTime > 0)
            {
                if (ScreenBlackoutTime > 0)
                    ScreenBlackoutTime--;
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
                return true;
            }
            int LeftEdgeWidth = (int)(System.Math.Floor(ri.RoomX * 16 - Main.screenPosition.X));
            int RightEdgeWidth = (int)(Main.screenPosition.X + Main.screenWidth - (ri.RoomX + ri.GetRoom.Width) * 16);
            int TopEdgeHeight = (int)(System.Math.Floor(ri.RoomY * 16 - Main.screenPosition.Y));
            int BottomEdgeHeight = (int)(Main.screenPosition.Y + Main.screenHeight - (ri.RoomY + ri.GetRoom.Height) * 16);
            if (LeftEdgeWidth > 0)
            {
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, LeftEdgeWidth, Main.screenHeight), Color.Black);
            }
            if (TopEdgeHeight > 0)
            {
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, TopEdgeHeight), Color.Black);
            }
            if (RightEdgeWidth > 0)
            {
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(Main.screenWidth - RightEdgeWidth, 0, RightEdgeWidth, Main.screenHeight), Color.Black);
            }
            if (BottomEdgeHeight > 0)
            {
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, Main.screenHeight - BottomEdgeHeight, Main.screenWidth, BottomEdgeHeight), Color.Black);
            }
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "Top Edge: " + TopEdgeHeight + "\nBottom Edge: " + BottomEdgeHeight + "\nLeft Edge: " + LeftEdgeWidth + "\nRight Edge: " + RightEdgeWidth, Main.screenPosition.X, Main.screenPosition.Y, Color.White, Color.Black, Vector2.Zero);
            return true;
        }

        private static bool DebugDisplay()
        {
            Vector2 Position = Vector2.Zero;
            Zone z = ZoneTypes[0];
            string s = "Tiles Width: " + z.ZoneTileMap.GetLength(0) + " Tiles Height: " + z.ZoneTileMap.GetLength(1) + "\n";
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    s += "{";
                    TilesetPacker.TileStep tile = z.ZoneTileMap[x, y];
                    s += tile.HasTile.ToString();
                    if (tile.HasTile)
                    {
                        s += ":" + tile.TileType;
                    }
                    s += "|"+ tile.WallType + "} ";
                }
                s+= "\n";
            }
            Utils.DrawBorderString(Main.spriteBatch, s, Position, Color.White, .7f);
            return true;
        }
    }
}