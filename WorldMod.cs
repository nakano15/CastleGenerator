using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;

namespace CastleGenerator
{
    public class WorldMod : ModWorld
    {
        public static bool GenerateCastle = false;
        public static bool IsCastle = false;
        public static List<RoomInfo> Rooms = new List<RoomInfo>();

        public override void Initialize()
        {
            IsCastle = false;
            Rooms.Clear();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            if (GenerateCastle)
            {
                while (tasks.Count > 2)
                    tasks.RemoveAt(2);
                tasks.Add(new Generator.GenerateCastle());
                GenerateCastle = false;
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag.Add("ModVersion", MainMod.ModVersion);
            tag.Add("IsCastle", IsCastle); //Room Infos needs to be saved too. Their zones must be saved by name.
            if (IsCastle)
                RoomInfo.Save(tag);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int Version = tag.GetInt("ModVersion");
            IsCastle = tag.GetBool("IsCastle");
            if (IsCastle)
            {
                RoomInfo.Load(tag, Version);
            }
        }
    }
}
