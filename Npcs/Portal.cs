using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace CastleGenerator.Npcs
{
    public class Portal : ModNPC
    {
        private Vector3 color = Vector3.One;
        private int LootReward = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal");
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 48;
            npc.damage = npc.defense = 0;
            npc.lifeMax = 100;
            npc.dontTakeDamage = npc.dontTakeDamageFromHostiles = true;
            npc.friendly = true;
            npc.lavaImmune = true;
            {
                Color newcolor = Color.White;
                switch (Main.rand.Next(3))
                {
                    default:
                        newcolor = Color.Blue;
                        break;
                    case 1:
                        newcolor = Color.Red;
                        break;
                    case 2:
                        newcolor = Color.Green;
                        break;
                }
                color.X = (float)newcolor.R / 255;
                color.Y = (float)newcolor.G / 255;
                color.Z = (float)newcolor.B / 255;
            }
            npc.scale = 2;
            PickFinalityLoot();
        }

        private void PickFinalityLoot()
        {
            switch (Main.rand.Next(5))
            {
                default:
                    LootReward = ItemID.BandofRegeneration;
                    break;
                case 1:
                    LootReward = ItemID.BandofStarpower;
                    break;
                case 2:
                    LootReward = ItemID.WeatherRadio;
                    break;
                case 3:
                    LootReward = ItemID.ZombieArm;
                    break;
                case 4:
                    LootReward = ItemID.NeptunesShell;
                    break;
            }
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            return "This portal appears to take you somewhere.";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Enter";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                CastleSubworld.ChangeFinalityItem(LootReward);
                CastleSubworld.ChangeRoomsToGenerate(40, 60);
                CastleSubworld.ChangeDifficulty(1, 2);
                PlaceLoots(Main.LocalPlayer);
                Subworld.Enter<CastleSubworld>();
            }
        }

        private void PlaceLoots(Player player)
        {
            for (int i = 0; i < 25; i++)
            {
                if (NPC.downedMechBossAny)
                {
                    AddLoot(ItemID.GreaterHealingPotion);
                    AddLoot(ItemID.GreaterManaPotion);
                }
                else if (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee || NPC.downedSlimeKing)
                {
                    AddLoot(ItemID.HealingPotion);
                    AddLoot(ItemID.ManaPotion);
                }
                else
                {
                    AddLoot(ItemID.LesserHealingPotion);
                    AddLoot(ItemID.LesserManaPotion);
                }
            }
            AddLoot(ItemID.Aglet, difficulty: DifficultyLevel.VeryEasy);
            AddLoot(ItemID.FrogLeg, difficulty: DifficultyLevel.Easy);
            AddLoot(ItemID.AnkletoftheWind, difficulty: DifficultyLevel.Normal);
        }

        private void AddLoot(int ItemID, Loot.LootType type = Loot.LootType.Normal, DifficultyLevel difficulty = DifficultyLevel.Trivial)
        {
            CastleSubworld.AddItem(ItemID, type, difficulty);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.water && !SLWorld.subworld && !NPC.AnyNPCs(ModContent.NPCType<Portal>()))
                return 1f / 250;
            return 0;
        }

        public override void DrawEffects(ref Color drawColor)
        {
            drawColor = new Color(color * MainMod.PortalBlinkValue);
            Lighting.AddLight(npc.Center, color * 1.5f * npc.scale);
        }
    }
}
