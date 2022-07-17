using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Graphics;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace CastleGenerator
{
	public class MainMod : Mod
	{
        public static Mod mod;
        public const int ModVersion = 2;
        public static List<Zone> ZoneTypes = new List<Zone>();
        public static Zone InvalidZone;
        public static LegacyGameInterfaceLayer MapBordersInterfaceLayer;
        public static float Zoom = 2;
        private static bool DaytimeBackup = false;
        public static byte ScreenBlackoutTime = 0;
        public const string IsCastleString = "IsCastle";
        public int PortalBlinkCounter = 0;
        public static float PortalBlinkValue = 1;

        public override object Call(params object[] args)
        {
            if(args.Length > 0 && args[0] is string)
            {
                switch ((string)args[0])
                {
                    case IsCastleString:
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
            }
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (WorldMod.IsCastle)
            {
                Transform.Zoom *= Zoom;
            }
        }

        public override void MidUpdatePlayerNPC()
        {
            if (WorldMod.IsCastle)
            {
                DaytimeBackup = Main.dayTime;
                Main.dayTime = false;
            }
            PortalBlinkCounter++;
            PortalBlinkValue = 1f - (float)System.Math.Sin(PortalBlinkCounter * 0.2f) * 0.2f;
        }

        public override void MidUpdateNPCGore()
        {
            if (WorldMod.IsCastle)
                Main.dayTime = DaytimeBackup;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if(WorldMod.IsCastle)
                layers.Insert(0, MapBordersInterfaceLayer);
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
                Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
                return true;
            }
            int LeftEdgeWidth = (int)(System.Math.Floor(ri.RoomX * 16 - Main.screenPosition.X));
            int RightEdgeWidth = (int)(Main.screenPosition.X + Main.screenWidth - (ri.RoomX + ri.GetRoom.Width) * 16);
            int TopEdgeHeight = (int)(System.Math.Floor(ri.RoomY * 16 - Main.screenPosition.Y));
            int BottomEdgeHeight = (int)(Main.screenPosition.Y + Main.screenHeight - (ri.RoomY + ri.GetRoom.Height) * 16);
            if (LeftEdgeWidth > 0)
            {
                Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, LeftEdgeWidth, Main.screenHeight), Color.Black);
            }
            if (TopEdgeHeight > 0)
            {
                Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, TopEdgeHeight), Color.Black);
            }
            if (RightEdgeWidth > 0)
            {
                Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(Main.screenWidth - RightEdgeWidth, 0, RightEdgeWidth, Main.screenHeight), Color.Black);
            }
            if (BottomEdgeHeight > 0)
            {
                Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, Main.screenHeight - BottomEdgeHeight, Main.screenWidth, BottomEdgeHeight), Color.Black);
            }
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "Top Edge: " + TopEdgeHeight + "\nBottom Edge: " + BottomEdgeHeight + "\nLeft Edge: " + LeftEdgeWidth + "\nRight Edge: " + RightEdgeWidth, Main.screenPosition.X, Main.screenPosition.Y, Color.White, Color.Black, Vector2.Zero);
            return true;
        }
    }
}