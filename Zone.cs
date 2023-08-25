using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using ReLogic.Content;

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
        public Color[,] ColorMap = new Color[0, 0];

        public void LoadTexture()
        {
            Type myType = this.GetType();
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
            ZoneMapTexture = null;
        }

        public Color[,] GetColorMap()
        {
            return ColorMap;
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
