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
        public static bool GenerateCastle = true;
        public static bool IsCastle = false;
        public static List<RoomInfo> Rooms = new List<RoomInfo>();

        public override void OnWorldLoad()/* tModPorter Suggestion: Also override OnWorldUnload, and mirror your worldgen-sensitive data initialization in PreWorldGen */
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

        public override void SaveWorldData(TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
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
            if (WorldMod.IsCastle)
            {
                Transform.Zoom *= MainMod.Zoom;
            }
        }

        public override void PostUpdatePlayers()
        {
            MainMod.PostUpdatePlayerScripts();
        }

        public override void PostUpdateNPCs()
        {
            MainMod.PostUpdateNpcScript();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if(WorldMod.IsCastle)
                layers.Insert(0, MainMod.MapBordersInterfaceLayer);
        }

        public override void PreWorldGen()
        {
            /*foreach (Zone z in MainMod.ZoneTypes)
            {
                z.LoadTexture();
            }*/
        }
    }
}
