using Terraria;
using Terraria.ModLoader;
using System;
using System.IO;

namespace CastleGenerator
{
    public class ExportWorldCommand : ModCommand
    {
        public override string Command => "exporttileinfos";
        public override string Name => "Export World Tile Infos";
        public override string Description => "";
        public override CommandType Type => CommandType.Console;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/exportedtiles.txt", FileMode.OpenOrCreate))
            {
                TilesetPacker p = TilesetPacker.PackWorld();
                p.Save(fs);
            }
        }
    }
}