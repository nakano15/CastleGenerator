using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Graphics;
using Terraria.UI;

namespace CastleGenerator
{
    public class WorldMod : ModSystem
    {
        public static bool GenerateCastle = false;
        public static bool IsCastle = false;
        public static List<RoomInfo> Rooms = new List<RoomInfo>();
        private static bool DaytimeBackup = false;
        private static int PortalBlinkCounter = 0;
        public static float PortalBlinkValue = 1;

        public override void OnWorldUnload()
        {
            IsCastle = false;
            Rooms.Clear();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            if (GenerateCastle)
            {
                while (tasks.Count > 2)
                    tasks.RemoveAt(2);
                tasks.Add(new Generator.GenerateCastle());
                GenerateCastle = false;
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("ModVersion", MainMod.ModVersion);
            tag.Add("IsCastle", IsCastle); //Room Infos needs to be saved too. Their zones must be saved by name.
            if (IsCastle)
                RoomInfo.Save(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            int Version = tag.GetInt("ModVersion");
            IsCastle = tag.GetBool("IsCastle");
            if (IsCastle)
            {
                RoomInfo.Load(tag, Version);
            }
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (IsCastle)
            {
                Transform.Zoom *= MainMod.Zoom;
            }
        }

        public override void PostUpdatePlayers()
        {
            if (IsCastle)
            {
                DaytimeBackup = Main.dayTime;
                Main.dayTime = false;
            }
            PortalBlinkCounter++;
            PortalBlinkValue = 1f - (float)System.Math.Sin(PortalBlinkCounter * 0.2f) * 0.2f;
        }

        public override void PostUpdateNPCs()
        {
            if (IsCastle)
                Main.dayTime = DaytimeBackup;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if(IsCastle)
                layers.Insert(0, MainMod.MapBordersInterfaceLayer);
        }

        public override void PreWorldGen()
        {
            
        }
    }
}
