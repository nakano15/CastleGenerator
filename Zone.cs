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
        private Texture2D _ZoneTiles = null;
        public List<ZoneMobDefinition> ZoneMobs = new List<ZoneMobDefinition>();
        public List<TileInfo> ZoneTileInfoCodes = new List<TileInfo>();

        public void LoadTexture()
        {
            Type myType = this.GetType();
            string TextureDirectory = myType.Namespace.Replace(".", "/") + myType.Name;
            //throw new Exception(TextureDirectory);
            if (MainMod.mod.HasAsset(TextureDirectory))
            {
                _ZoneTiles = ModContent.Request<Texture2D>(TextureDirectory).Value;
            }
            else
            {
                _ZoneTiles = Terraria.GameContent.TextureAssets.BlackTile.Value;
            }
        }

        private Texture2D GetTexture()
        {
            /*if(_ZoneTiles == null)
            {
                Type myType = this.GetType();
                string TextureDirectory = myType.Namespace.Replace(".", "/") + myType.Name;
                //throw new Exception(TextureDirectory);
                if (MainMod.mod.HasAsset(TextureDirectory))
                {
                    _ZoneTiles = ModContent.Request<Texture2D>(TextureDirectory).Value;
                }
                else
                {
                    _ZoneTiles = Terraria.GameContent.TextureAssets.BlackTile.Value;
                }
            }*/
            return _ZoneTiles;
        }

        public Color[,] GetColorMap()
        {
            Color[,] colorMap = new Color[0, 0];
            Texture2D mapTexture = GetTexture();
            {
                Color[] color1D = new Color[mapTexture.Width * mapTexture.Height];
                mapTexture.GetData(color1D);
                colorMap = new Color[mapTexture.Width, mapTexture.Height];
                for (int x = 0; x < mapTexture.Width; x++)
                {
                    for (int y = 0; y < mapTexture.Height; y++)
                    {
                        colorMap[x, y] = color1D[x + y * mapTexture.Width];
                    }
                }
            }
            return colorMap;
        }

        public static Zone CreateInvalidZone()
        {
            Zone z = new Zone(){ InvalidZone = true, ID = -1 };
            z.LoadTexture();
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
