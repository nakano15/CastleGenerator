using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria;

namespace CastleGenerator
{
    public class Zone
    {
        public string Name = "";
        public int ID;
        private bool InvalidZone = false;
        public List<Room> RoomTypes = new List<Room>();
        public bool IsInvalid { get { return InvalidZone; } }
        public List<ZoneMobDefinition> ZoneMobs = new List<ZoneMobDefinition>();
        public List<TileInfo> ZoneTileInfoCodes = new List<TileInfo>();
        public TilesetPacker.TileStep[,] ZoneTileMap = new TilesetPacker.TileStep[0, 0];

        public void LoadZoneTiles()
        {
            Type myType = this.GetType(); //How to load the tile infos?
            string TilesetDirectory = myType.Namespace.Replace(".", "/") + "/" + myType.Name + ".tiles";
            string ModName = myType.Namespace.Split('.')[0];
            TilesetDirectory = TilesetDirectory.Replace(ModName + "/", "");
            Mod mod = ModLoader.GetMod(ModName);
            try
            {
                using (Stream s = mod.GetFileStream(TilesetDirectory))
                {
                    TilesetPacker p = new TilesetPacker();
                    p.Load(s); //At least the tile infos doesn't seems to be loading as intended.
                    TilesetPacker.UnpackWorld(p, out ZoneTileMap);
                }
            }
            catch
            {
            }
            /*if ((Found = ModContent.HasAsset(TilesetDirectory)))
            {
                using (Stream stream = ModContent.OpenRead(TilesetDirectory))
                {
                    TilesetPacker p = new TilesetPacker();
                    p.Load(stream);
                    TilesetPacker.UnpackWorld(p, out ZoneTileMap);
                }
            }
            throw new Exception("Found ? " + Found + "  Directory: " + TilesetDirectory);*/
        }

        public void LoadTexture()
        {
            /*Type myType = this.GetType();
            string TextureDirectory = myType.Namespace.Replace(".", "/") + "/" + myType.Name;
            //throw new Exception(TextureDirectory);
            Texture2D ZoneMapTexture = null;
            if (ModContent.HasAsset(TextureDirectory))
            {
                ZoneMapTexture = ModContent.Request<Texture2D>(TextureDirectory).Value;
            }
            else
            {
                if (myType.Name != "Zone")throw new Exception("Couldn't find " + TextureDirectory);
                ZoneMapTexture = Terraria.GameContent.TextureAssets.BlackTile.Value;
            }
            ColorMap = new Color[ZoneMapTexture.Width, ZoneMapTexture.Height];
            Color[] Color1D = new Color[ZoneMapTexture.Width * ZoneMapTexture.Height];
            ZoneMapTexture.GetData<Color>(Color1D);
            for (int y = 0; y < ZoneMapTexture.Height; y++)
            {
                for (int x = 0; x < ZoneMapTexture.Width; x++)
                {
                    ColorMap[x, y] = Color1D[x + (y * ZoneMapTexture.Width)];
                }
            }
            ColorMap = null;
            ZoneMapTexture = null;*/
        }

        public void UnloadZone()
        {
            ZoneTileMap = null;
            ZoneTileInfoCodes.Clear();
            ZoneMobs.Clear();
            ZoneTileInfoCodes = null;
            ZoneMobs = null;
        }

        public TilesetPacker.TileStep[,] GetTilesetMap()
        {
            return ZoneTileMap;
        }

        public static Zone CreateInvalidZone()
        {
            Zone z = new Zone(){ InvalidZone = true, ID = -1 };
            return z;
        }

        public void AddRoom(Room r)
        {
            r.ParentZone = this;
            r.ID = RoomTypes.Count;
            RoomTypes.Add(r);
        }

        public void AddZoneMob(int MobID, DifficultyLevel difficulty, int MobHealth, int MobDamage, int MobDefense, float KnockbackRes, string Name = "", Color? Color = null, bool Aquatic = false, bool Nocturnal = false)
        {
            ZoneMobs.Add(new ZoneMobDefinition() { MobID = MobID, Difficulty = difficulty, Health = MobHealth, Damage = MobDamage, Defense = MobDefense, KBRes = KnockbackRes, Name = Name, Color = Color, Aquatic = Aquatic, Nocturnal = Nocturnal });
        }

        public TileInfo AddTileInfo(byte CodeR, byte CodeG, byte CodeB, byte CodeA = 255)
        {
            TileInfo Ti = new TileInfo(CodeR, CodeG, CodeB, CodeA);
            ZoneTileInfoCodes.Add(Ti);
            return Ti;
        }
    }
}
