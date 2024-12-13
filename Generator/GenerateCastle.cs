using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.WorldBuilding;
using Terraria.IO;

namespace CastleGenerator.Generator
{
    public class GenerateCastle : GenPass
    {
        const float LoadWeight = 10;
        private List<RoomPosition> rooms = new List<RoomPosition>();
        private GenerationProgress progress;
        private bool InvalidWorld = false;
        public int RoomsToGenerate = -1;
        public Terraria.Utilities.UnifiedRandom rand
        {
            get
            {
                if (randomizer != null)
                    return randomizer;
                return WorldGen.genRand;
            }
        }
        public Terraria.Utilities.UnifiedRandom randomizer = null;
        private byte LifeCrystals = 0;
        public List<Loot> loot = new List<Loot>();
        private bool PlayerGotDoubleJump = false, PlayerGotFlight = false, PlayerGotRod = false, PlayerGotSwiming = false, PlayerGotLavaImmunity = false, PlayerGotRunningBoots = false, 
            PlayerGotWaterwalking = false, PlayerGotExtraJumpHeight = false;
        private int FinalityRoomPosition = -1;
        public int Finality = 0;
        public float MinDifficultyLevel = 1;
        public float MaxDifficultyLevel = 10;
        public bool CreateLifeCrystals = true, GenerateLootTable = true;

        public GenerateCastle() : base("CastleGenerator: Generate Castle", LoadWeight)
        {
            randomizer = new Terraria.Utilities.UnifiedRandom();
        }

        public void InitializeCastle()
        {
            GenerateLootTable = true;
            CreateLifeCrystals = true;
            randomizer = null;
            LifeCrystals = 0;
            InvalidWorld = false;
            RoomsToGenerate = -1;
            loot.Clear();
            PlayerGotDoubleJump = false;
            PlayerGotFlight = false;
            PlayerGotRod = false;
            PlayerGotSwiming = false;
            PlayerGotLavaImmunity = false;
            PlayerGotRunningBoots = false;
            PlayerGotWaterwalking = false;
            PlayerGotExtraJumpHeight = false;
            FinalityRoomPosition = -1;
            Finality = 0;
            MinDifficultyLevel = 1;
            MaxDifficultyLevel = 10;
        }

        public void ChangeSeed(int NewSeed)
        {
            randomizer = new Terraria.Utilities.UnifiedRandom(NewSeed);
        }

        public void ApplyFlagsBasedOnPlayer(Player player)
        {
            PlayerGotDoubleJump = HasItemEquipped(player, new int[] { ItemID.CloudinaBottle, ItemID.CloudinaBalloon, ItemID.BlizzardinaBottle, ItemID.BlizzardinaBalloon,
                ItemID.SandstorminaBottle, ItemID.SandstorminaBalloon, ItemID.FartinaJar, ItemID.FartInABalloon, ItemID.BalloonHorseshoeFart, ItemID.BundleofBalloons,
                ItemID.RocketBoots, ItemID.SpectreBoots});
            PlayerGotFlight = player.armor.Skip(3).Take(6).Any(x => x.wingSlot > 0);
            PlayerGotRod = player.inventory.Any(x => x.type == ItemID.RodofDiscord);
            PlayerGotSwiming = HasItemEquipped(player, new int[] { ItemID.Flipper, ItemID.DivingGear, ItemID.ArcticDivingGear, ItemID.NeptunesShell, ItemID.MoonShell, ItemID.CelestialShell });
            PlayerGotLavaImmunity = HasItemEquipped(player, new int[] { ItemID.LavaWaders, ItemID.LavaCharm });
            PlayerGotRunningBoots = HasItemEquipped(player, new int[] { ItemID.HermesBoots, ItemID.FlurryBoots, ItemID.FrostsparkBoots, ItemID.LightningBoots, ItemID.SailfishBoots,
                ItemID.SpectreBoots});
            PlayerGotWaterwalking = HasItemEquipped(player, new int[] { ItemID.WaterWalkingBoots, ItemID.ObsidianWaterWalkingBoots, ItemID.LavaWaders });
            PlayerGotExtraJumpHeight = HasItemEquipped(player, new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeHoney, ItemID.BalloonHorseshoeSharkron, ItemID.BalloonPufferfish,
                ItemID.BlizzardinaBalloon, ItemID.BlizzardinaBalloon, ItemID.BlueHorseshoeBalloon, ItemID.BundleofBalloons, ItemID.CloudinaBalloon, ItemID.FartInABalloon,
                ItemID.HoneyBalloon, ItemID.SandstorminaBalloon, ItemID.SharkronBalloon, ItemID.ShinyRedBalloon, ItemID.WhiteHorseshoeBalloon, ItemID.YellowHorseshoeBalloon});
        }

        private bool HasItemEquipped(Player player, int[] EquipmentIDs)
        {
            for(int i = 3; i < 9; i++)
            {
                if (EquipmentIDs.Contains(player.armor[i].type))
                    return true;
            }
            return false;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            this.progress = progress;
            WorldMod.IsCastle = true;
            NPC.downedBoss3 = true;
            Main.hardMode = true;
            CreateSpawnPoint();
            if (InvalidWorld)
                return;
            CreateCastleRooms();
            //TestCreateCastleRooms();
            if (InvalidWorld)
                return;
            CreateExtraCastleRooms();
            PlaceTreasures();
            PlaceMonsters();
            SpawnRoomTiles();
            CheckForDisconnectedConnectors();
            BlockOpenSpaces();
            FinishGeneration();
        }

        private void CreateSpawnPoint()
        {
            progress.Message = "Placing Castle Entrance.";
            Main.statusText = progress.Message;
            Zone PickedZone = null;
            {
                List<Zone> BegginingZone = new List<Zone>();
                foreach (Zone z in MainMod.ZoneTypes)
                {
                    if (z.RoomTypes.Any(x => x.roomType == RoomType.Spawn && x.RoomConnectors.Any(y => y.Requirement == ConnectorRequirement.None)))
                    {
                        BegginingZone.Add(z);
                        break;
                    }
                }
                if (BegginingZone.Count == 0)
                {
                    //Create Invalid World
                    GenerateInvalidWorld("No zones with spawn areas.");
                    return;
                }
                PickedZone = BegginingZone[rand.Next(BegginingZone.Count)];
            }
            Room StartingRoom = null;
            {
                List<Room> BegginingRooms = new List<Room>();
                foreach(Room r in PickedZone.RoomTypes)
                {
                    if (r.roomType == RoomType.Spawn)
                        BegginingRooms.Add(r);
                }
                StartingRoom = BegginingRooms[rand.Next(BegginingRooms.Count)];
            }
            Connector PickedConnector = null;
            {
                List<Connector> StartConnector = new List<Connector>();
                foreach (Connector c in
                StartingRoom.RoomConnectors)
                {
                    if (c.Requirement == ConnectorRequirement.None)
                    {
                        StartConnector.Add(c);
                    }
                }
                if(StartConnector.Count == 0)
                {
                    GenerateInvalidWorld(PickedZone.Name + "'s room {" + StartingRoom.ToString() + "} has no possible spawn point for players.");
                    return;
                }
                PickedConnector = StartConnector[rand.Next(StartConnector.Count)];
            }
            bool SpawnOnLeft = false;
            if (PickedConnector.Position == ConnectorPosition.Right)
                SpawnOnLeft = true;
            else
            {
                if(PickedConnector.Position != ConnectorPosition.Left)
                {
                    SpawnOnLeft = rand.NextDouble() < 0.5;
                }
            }
            int RoomPositionX = 0, RoomPositionY = 50;
            if (SpawnOnLeft)
                RoomPositionX = 50 + 5;
            else
                RoomPositionX = Main.maxTilesX - 50 - 6 - StartingRoom.Width;
            {
                int SpawnRefPosX = RoomPositionX;
                if (SpawnOnLeft)
                    SpawnRefPosX += StartingRoom.Width;
                while (true)
                {
                    if (!Main.tile[SpawnRefPosX, RoomPositionY].HasTile)
                        RoomPositionY++;
                    else
                        break;
                }
            }
            RoomPositionY += -StartingRoom.Height + PickedConnector.PositionY - StartingRoom.RoomTileStartY;
            if (!PickedConnector.Horizontal)
                RoomPositionY += PickedConnector.Distance;
            if (!PlaceRoom(StartingRoom, RoomPositionX, RoomPositionY, 0, true))
            {
                GenerateInvalidWorld("Failed to create starting room.");
                return;
            }
            Main.spawnTileX = RoomPositionX + StartingRoom.PlayerSpawnX - StartingRoom.RoomTileStartX;
            Main.spawnTileY = RoomPositionY + StartingRoom.PlayerSpawnY - StartingRoom.RoomTileStartY;
        }

        private void CreateCastleRooms()
        {
            int MaxSize = (Main.maxTilesX - 100) * (Main.maxTilesY - 100);
            byte CorridorRoom = (byte)rand.Next(2);
            byte SaveRoom = (byte)rand.Next(5, 8);
            bool CanCreateDeadEnds = false;
            bool CreateBoss = false;
            byte ForkedRoad = (byte)rand.Next(2, 4);
            List<RoomPosition> RoomsWithOpenConnectors = new List<RoomPosition>
            {
                rooms[0]
            };
            Zone PickedZone = rooms[rooms.Count - 1].room.ParentZone;
            int StuckCheck = RoomsWithOpenConnectors.Count;
            double GenerationPercentage = 0;
            float HighestRoomDifficulty = 0;
            while (RoomsToGenerate < 0 || RoomsToGenerate > 0)
            {
                int LastPossibleRooms = 0, LastPossibleConnectors = 0, LastNewRoomPossibleConnectors = 0;
                bool CreateFinalityRoom = Finality > 0 && FinalityRoomPosition == -1 && RoomsToGenerate == 1;
                //Checking Progress...
                int CurrentCastleDimension = 0;
                foreach (RoomPosition room in rooms)
                {
                    CurrentCastleDimension += room.Position.Width * room.Position.Height;
                }
                {
                    double Percentage = System.Math.Round((double)CurrentCastleDimension * 100 / MaxSize, 1);
                    GenerationPercentage = Percentage;
                    progress.Message = "Generating Castle: Rooms Created: " + rooms.Count + " - Filled " + Percentage + "% of the map.";
                    Main.statusText = progress.Message;
                    if (StuckCheck == 0)
                    {
                        if (Percentage >= 60)
                            break;
                        GenerateInvalidWorld("World generator got stuck.\nRooms created: " + rooms.Count + "\nProgress: " + Percentage + "%");
                        return;
                    }
                }
                StuckCheck--;
                if(RoomsWithOpenConnectors.Count == 0)
                {
                    GenerateInvalidWorld("Somehow the mod found a deadend.");
                    return;
                }
                RoomPosition ParentRoom = RoomsWithOpenConnectors[RoomsWithOpenConnectors.Count - 1];
                RoomType NewRoomType = RoomType.Normal;
                bool MakeForkedRoad = false;
                if (CreateFinalityRoom)
                {
                    NewRoomType = RoomType.Treasure;
                    FinalityRoomPosition = rooms.Count;
                }
                else if (ForkedRoad == 0)
                {
                    MakeForkedRoad = true;
                }
                else if (CorridorRoom == 0)
                {
                    NewRoomType = RoomType.Corridor;
                }
                else if (SaveRoom == 0)
                {
                    NewRoomType = RoomType.SaveRoom;
                }
                else if (CreateBoss)
                {
                    NewRoomType = RoomType.BossRoom;
                }
                List<Room> PossibleRooms = new List<Room>();
            retryRoomPicking:
                foreach (Room room in PickedZone.RoomTypes) //Game is not finding possible rooms
                {
                    if(room.roomType == NewRoomType)
                    {
                        byte OpenConnectors = 0;
                        foreach(Connector connector in room.RoomConnectors)
                        {
                            if(CanUseConnector(connector))
                            {
                                foreach (Connector ParentConnector in ParentRoom.room.RoomConnectors)
                                {
                                    if (CanUseConnector(ParentConnector) && CanConnectToConnector(ParentConnector, connector) && CanPlaceRoomHere(ParentConnector, ParentRoom, room))
                                    {
                                        OpenConnectors++;
                                        break;
                                    }
                                }
                            }
                        }
                        if((MakeForkedRoad && OpenConnectors > 2) || (CanCreateDeadEnds && OpenConnectors > 0) || OpenConnectors > 1)
                        //if ((CanCreateDeadEnds && ((!MakeForkedRoad && OpenConnectors > 0) || OpenConnectors > 2)) || OpenConnectors > 1)
                        {
                            PossibleRooms.Add(room);
                        }
                    }
                }
                if(PossibleRooms.Count == 0)
                {
                    if(NewRoomType != RoomType.Normal)
                    {
                        NewRoomType = RoomType.Normal;
                        goto retryRoomPicking;
                    }
                    else
                    {
                        if (MakeForkedRoad)
                        {
                            MakeForkedRoad = false;
                            CreateFinalityRoom = false;
                            goto retryRoomPicking;
                        }
                        else
                        {

                        }
                    }
                }
                LastPossibleRooms = PossibleRooms.Count;
                bool Success = false;
                while(PossibleRooms.Count > 0 && !Success)
                {
                    int PickedRoomStyle = rand.Next(PossibleRooms.Count);
                    Room NewRoom = PossibleRooms[PickedRoomStyle];
                    PossibleRooms.RemoveAt(PickedRoomStyle);
                    List<Connector> PossibleConnectors = new List<Connector>();
                    foreach(Connector ParentConnector in ParentRoom.room.RoomConnectors)
                    {
                        if (CanUseConnector(ParentConnector) && !IsConnectorBlocked(ParentRoom, ParentConnector) && !IsConnectorConnected(ParentRoom, ParentConnector))
                        {
                            PossibleConnectors.Add(ParentConnector);
                        }
                    }
                    LastPossibleConnectors = PossibleConnectors.Count;
                    while (PossibleConnectors.Count > 0 && !Success)
                    {
                        int PickedParentConnector = rand.Next(PossibleConnectors.Count);
                        Connector ParentConnector = PossibleConnectors[PickedParentConnector];
                        PossibleConnectors.RemoveAt(PickedParentConnector);
                        List<Connector> NewRoomPossibleConnectors = new List<Connector>();
                        foreach (Connector NewRoomConnector in NewRoom.RoomConnectors)
                        {
                            if(CanUseConnector(NewRoomConnector) && CanConnectToConnector(ParentConnector, NewRoomConnector))
                            {
                                NewRoomPossibleConnectors.Add(NewRoomConnector);
                            }
                        }
                        LastNewRoomPossibleConnectors = NewRoomPossibleConnectors.Count;
                        while (NewRoomPossibleConnectors.Count > 0 && !Success)
                        {
                            int PickedNewConnector = rand.Next(NewRoomPossibleConnectors.Count);
                            Connector NewConnector = NewRoomPossibleConnectors[PickedNewConnector];
                            NewRoomPossibleConnectors.RemoveAt(PickedNewConnector);
                            int RoomX = ParentRoom.Position.X, RoomY = ParentRoom.Position.Y;
                            switch (ParentConnector.Position) //Placement horizontally seems off. Vertically might be off too.
                            {
                                case ConnectorPosition.Up:
                                    RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                                    RoomY -= NewRoom.Height;
                                    break;
                                case ConnectorPosition.Down:
                                    RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                                    RoomY += ParentRoom.Position.Height;
                                    break;
                                case ConnectorPosition.Left:
                                    RoomX -= NewRoom.Width;
                                    RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                                    break;
                                case ConnectorPosition.Right:
                                    RoomX += ParentRoom.Position.Width;
                                    RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                                    break;
                            }
                            if (!HasRoomHere(RoomX, RoomY, NewRoom.Width, NewRoom.Height) && PlaceRoom(NewRoom, RoomX, RoomY, ParentRoom.Difficulty + (float)NewRoom.Width / NewRoom.Height * 0.1f))
                            {
                                RoomPosition NewRoomPos = rooms[rooms.Count - 1];
                                if (NewRoomPos.Difficulty > HighestRoomDifficulty)
                                    HighestRoomDifficulty = NewRoomPos.Difficulty;
                                byte ConnectorCount = 0;
                                foreach(Connector connector in NewRoomPos.room.RoomConnectors)
                                {
                                    if (!IsConnectorBlocked(NewRoomPos, connector))
                                        ConnectorCount++;
                                }
                                if (ConnectorCount > 0)
                                    RoomsWithOpenConnectors.Add(NewRoomPos);
                                Success = true;
                                PickedZone = NewRoom.ParentZone;
                            }
                        }
                    }
                }
                if (Success)
                {
                    RoomPosition NewRoomPos = rooms[rooms.Count - 1];
                    if (ForkedRoad > 0)
                        ForkedRoad--;
                    else if (MakeForkedRoad)
                        ForkedRoad = (byte)rand.Next(5, 9);
                    if (CorridorRoom > 0)
                        CorridorRoom--;
                    else if (NewRoomType == RoomType.Corridor)
                        CorridorRoom = (byte)rand.Next(3, 6);
                    if (SaveRoom > 0)
                        SaveRoom--;
                    else if (NewRoomType == RoomType.SaveRoom)
                        SaveRoom = (byte)rand.Next(12, 17);
                    if(RoomsToGenerate > 0) RoomsToGenerate--;
                    if(NewRoomType == RoomType.BossRoom)
                    {
                        CreateBoss = false;
                    }
                    else
                    {
                        CreateBoss = rooms.Count % 250 == 0; //Change this in the future.
                    }
                    bool HasConnectorOpened = false;
                    foreach(Connector c in ParentRoom.room.RoomConnectors)
                    {
                        if(!IsConnectorBlocked(ParentRoom, c) && !IsConnectorConnected(ParentRoom, c))
                        {
                            HasConnectorOpened = true;
                            break;
                        }
                    }
                    if (!HasConnectorOpened)
                    {
                        RoomsWithOpenConnectors.Remove(ParentRoom);
                    }
                    StuckCheck = RoomsWithOpenConnectors.Count;
                    if (StuckCheck > 1000)
                        StuckCheck = 1000;
                }
                else
                {
                    //progress.Message = "Attempt#" + StuckCheck + " - Rooms: " + LastPossibleRooms + "  Connectors: " + LastPossibleConnectors + "  New Connectors: " + LastNewRoomPossibleConnectors;
                    //System.Threading.Thread.Sleep(1000);
                    RoomsWithOpenConnectors.Remove(ParentRoom);
                    RoomsWithOpenConnectors.Insert(0, ParentRoom);
                }
            }
            //Normalize Difficulty Levels;
            if (HighestRoomDifficulty > 0)
            {
                float Normalized = 1f / HighestRoomDifficulty;
                for (int r = 0; r < rooms.Count; r++)
                {
                    RoomPosition room = rooms[r];
                    room.Difficulty = MinDifficultyLevel + room.Difficulty * Normalized * (MaxDifficultyLevel - MinDifficultyLevel);
                }
            }
        }

        private void CreateExtraCastleRooms()
        {
            const byte MaxRoomsTillSavePoint = 8; //15
            byte RoomsUntilSavePoint = MaxRoomsTillSavePoint;
            int LastMap = rooms.Count;
            for (int i = 0; i < LastMap; i++)
            {
                progress.Message = "Generating Castle: Adding extra rooms [" + i + "/" + LastMap + "].";
                Main.statusText = progress.Message;
                RoomPosition roomPos = rooms[i];
                Room room = roomPos.room;
                Zone zone = room.ParentZone;
                foreach (Connector c in room.RoomConnectors)
                {
                    if (CanUseConnector(c) && !IsConnectorConnected(roomPos, c))
                    {
                        //Try Adding Room
                        if (RoomsUntilSavePoint == 0)
                        {
                            List<Room> PossibleRooms = new List<Room>();
                            foreach(Room r in zone.RoomTypes)
                            {
                                if(r.roomType == RoomType.SaveRoom && CanPlaceRoomHere(c, roomPos, r))
                                {
                                    PossibleRooms.Add(r);
                                }
                            }
                            while(PossibleRooms.Count > 0)
                            {
                                int p = rand.Next(PossibleRooms.Count);
                                Room NewRoom = PossibleRooms[p];
                                PossibleRooms.RemoveAt(p);
                                if(PlaceRoom(NewRoom, roomPos, c, roomPos.Difficulty))
                                {
                                    RoomsUntilSavePoint = MaxRoomsTillSavePoint;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            byte RoomsToCreate = (byte)rand.Next(1, 4);
                            if (rand.Next(3) == 0)
                                RoomsToCreate = (byte)rand.Next(3, 7);
                            RoomType LastType = RoomType.Normal;
                            while (RoomsToCreate > 0)
                            {
                                progress.Message = "Creating room " + RoomsToCreate;
                                RoomsToCreate--;
                                List<Room> PossibleRooms = new List<Room>();
                                foreach (Room r in zone.RoomTypes)
                                {
                                    if (r.roomType != LastType && (RoomsToCreate > 0 ? (r.roomType == RoomType.Normal || r.roomType == RoomType.Corridor || r.roomType == RoomType.Treasure) : (r.roomType == RoomType.Treasure || r.roomType == RoomType.SaveRoom || r.roomType == RoomType.Normal)))
                                    {
                                        if (CanPlaceRoomHere(c, roomPos, r))
                                            PossibleRooms.Add(r);
                                    }
                                }
                                while (PossibleRooms.Count > 0)
                                {
                                    progress.Message = "Trying to place rooms: " + PossibleRooms.Count;
                                    int p = rand.Next(PossibleRooms.Count);
                                    Room PickedRoom = PossibleRooms[p];
                                    PossibleRooms.RemoveAt(p);
                                    if (PlaceRoom(PickedRoom, roomPos, c, roomPos.Difficulty))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (room.roomType == RoomType.SaveRoom)
                {
                    RoomsUntilSavePoint = MaxRoomsTillSavePoint;
                }
                else if(RoomsUntilSavePoint > 0)
                {
                    RoomsUntilSavePoint--;
                }
            }
        }

        private bool CanPlaceRoomHere(Connector ReferenceConnector, RoomPosition ReferenceRoom, Room NewRoom)
        {
            return CanPlaceRoomHere(ReferenceConnector, ReferenceRoom, NewRoom.Width, NewRoom.Height);
        }

        private bool CanPlaceRoomHere(Connector ReferenceConnector, RoomPosition ReferenceRoom, int Width, int Height)
        {
            switch (ReferenceConnector.Position)
            {
                case ConnectorPosition.Up:
                    return ReferenceRoom.Position.Y + ReferenceRoom.Position.Y - ReferenceRoom.room.RoomTileStartY - Height >= 50;
                case ConnectorPosition.Down:
                    return ReferenceRoom.Position.Y + ReferenceRoom.Position.Y - ReferenceRoom.room.RoomTileStartY + ReferenceRoom.Position.Height + Height < Main.maxTilesY - 50;
                case ConnectorPosition.Left:
                    return ReferenceRoom.Position.X + ReferenceRoom.Position.X - ReferenceRoom.room.RoomTileStartX - Width >= 50;
                case ConnectorPosition.Right:
                    return ReferenceRoom.Position.X + ReferenceRoom.Position.X - ReferenceRoom.room.RoomTileStartX + ReferenceRoom.Position.Width + Width < Main.maxTilesX - 50;
            }
            return false;
        }

        private void TestCreateCastleRooms()
        {
            //Progressivelly gets slower as rooms are added. Also has the chance of softlocking.
            byte StuckCounter = 0;
            int TotalRoomsToCreate = RoomsToGenerate;
            while (RoomsToGenerate > 0)
            {
                StuckCounter++;
                if(StuckCounter == 255)
                {
                    if (rooms.Count == 1)
                    {
                        bool SpawnRoomConnectorToLeft = rooms[0].Position.X < Main.maxTilesX * 0.5f;
                        GenerateInvalidWorld("Rooms generation got stuck at " + RoomsToGenerate + " rooms to create.\nSpawn Room created to the " + (SpawnRoomConnectorToLeft ? "left" : "right") + ".");
                        return;
                    }
                    else
                    {
                        RoomsToGenerate++;
                        rooms.RemoveAt(rooms.Count - 1);
                    }
                }
                float Difficulty = MinDifficultyLevel + (float)rooms.Count * MaxDifficultyLevel / RoomsToGenerate;
                progress.Message = "Generating Castle Rooms. (" + RoomsToGenerate + ")";
                int DisconnectedConnectors = GetDisconnectedConnectors();
                if(rooms.Count > 1 && DisconnectedConnectors == 0)
                {
                    byte RoomsToRemove = (byte)rand.Next(3, 6);
                    while(RoomsToRemove-- > 0 && rooms.Count > 1)
                        rooms.RemoveAt(rooms.Count - 1);
                    RoomsToGenerate++;
                    continue;
                }
                RoomPosition Room;
                {
                    RoomPosition? r = GetRandomPossibleRoom();
                    if(!r.HasValue)
                    {
                        GenerateInvalidWorld("No room to proceed from here.");
                        return;
                    }
                    Room = r.Value;
                }
                Connector connector;
                {
                    Connector[] PossibleConnectors = GetPossibleConnectors(Room);
                    if (PossibleConnectors.Length == 0)
                    {
                        continue;
                    }
                    int ConnectorIndex = rand.Next(PossibleConnectors.Length);
                    connector = PossibleConnectors[ConnectorIndex];
                }
                Zone RefZone = Room.room.ParentZone;
                List<Room> PossibleRooms = new List<Room>();
                foreach(Room r in RefZone.RoomTypes)
                {
                    int PossibleConnectorsCount = 0;
                    foreach (Connector other in r.RoomConnectors)
                    {
                        if (CanConnectToConnector(connector, other) && CanUseConnector(other) && !IsConnectorOnDangerousPosition(other, Room) && connector.IsOpposite(other))
                        {
                            PossibleConnectorsCount++;
                        }
                        //break;
                    }
                    if(PossibleConnectorsCount > 1 || DisconnectedConnectors > 4)
                    {
                        PossibleRooms.Add(r);
                    }
                }
                if (PossibleRooms.Count == 0)
                    continue;
                int PickedRoom = rand.Next(PossibleRooms.Count);
                Room NewRoom = PossibleRooms[PickedRoom];
                int RoomPositionX = Room.Position.X, RoomPositionY = Room.Position.Y;
                {
                    List<Connector> PossibleConnectors = GetRoomPossibleConnectors(NewRoom, connector);
                    if (PossibleConnectors.Count == 0) //Nani?!
                        continue;
                    bool Success = false;
                    do
                    {
                        int PickedConnector = rand.Next(PossibleConnectors.Count);
                        Connector otherconnector = PossibleConnectors[PickedConnector];
                        Rectangle NewRoomPos = new Rectangle(Room.Position.X, Room.Position.Y, NewRoom.Width, NewRoom.Height);
                        switch (connector.Position)
                        {
                            case ConnectorPosition.Up:
                                NewRoomPos.X -= (otherconnector.PositionX - NewRoom.RoomTileStartX) - (connector.PositionX - Room.room.RoomTileStartX);
                                NewRoomPos.Y -= NewRoom.Height;
                                break;
                            case ConnectorPosition.Down:
                                NewRoomPos.X -= (otherconnector.PositionX - NewRoom.RoomTileStartX) - (connector.PositionX - Room.room.RoomTileStartX);
                                NewRoomPos.Y += Room.room.Height;
                                break;
                            case ConnectorPosition.Left:
                                NewRoomPos.X -= NewRoom.Width;
                                NewRoomPos.Y -= (otherconnector.PositionY - NewRoom.RoomTileStartY) - (connector.PositionY - Room.room.RoomTileStartY);
                                break;
                            case ConnectorPosition.Right:
                                NewRoomPos.X += Room.room.Width;
                                NewRoomPos.Y -= (otherconnector.PositionY - NewRoom.RoomTileStartY) - (connector.PositionY - Room.room.RoomTileStartY);
                                break;
                        }
                        if (!HasRoomHere(NewRoomPos) && PlaceRoom(NewRoom, NewRoomPos.X, NewRoomPos.Y, Difficulty))
                        {
                            Success = true;
                        }
                        else
                        {
                            PossibleConnectors.RemoveAt(PickedConnector);
                            if (PossibleConnectors.Count == 0)
                                break;
                        }

                    }
                    while (!Success);
                    if (Success)
                    {
                        RoomsToGenerate--;
                        StuckCounter = 0;
                    }
                }
            }
        }

        private void PlaceMonsters()
        {
            progress.Message = "Adding Monsters to Rooms";
            Main.statusText = progress.Message;
            bool FirstRoom = true;
            foreach(RoomPosition room in rooms)
            {
                if(FirstRoom)
                {
                    FirstRoom = false;
                    continue;
                }
                List<int> SpawnSlots = new List<int>();
                for(int i = 0; i < room.room.MobSpawnPosition.Count; i++)
                {
                    SpawnSlots.Add(i);
                }
                float DifficultyGrade = 0.5f + room.Difficulty + 0.2f * room.Treasures.Count;
                while(SpawnSlots.Count > 0)
                {
                    int Slot = rand.Next(SpawnSlots.Count);
                    int PickedSlot = SpawnSlots[Slot];
                    MobSpawnPos spawn = room.room.MobSpawnPosition[PickedSlot];
                    if (DifficultyGrade >= (int)spawn.Difficulty)
                    {
                        Zone zone = room.room.ParentZone;
                        List<int> PossibleMobs = new List<int>();
                        for (int i = 0; i < zone.ZoneMobs.Count; i++)
                        {
                            if (DifficultyGrade >= (int)zone.ZoneMobs[i].Difficulty)
                            {
                                PossibleMobs.Add(i);
                            }
                        }
                        if (PossibleMobs.Count > 0)
                        {
                            int PickedMob = PossibleMobs[rand.Next(PossibleMobs.Count)];
                            room.AddMob(PickedMob, PickedSlot);
                            DifficultyGrade -= (int)spawn.Difficulty * 0.5f;
                        }
                    }
                    SpawnSlots.RemoveAt(Slot);
                }
            }
        }

        private void PlaceTreasures()
        {
            if (GenerateLootTable) CreateLootTable();
            progress.Message = "Adding Treasures to the Castle";
            Main.statusText = progress.Message;
            float LifeCrystalSpawnChance = 0.5f;
            float TreasureSpawnChance = 0.2f;
            bool SpawnedFinalityLoot = false;
            for(int r = 0; r < rooms.Count; r++)
            {
                RoomPosition roompos = rooms[r];
                Room room = roompos.room;
                if(room.TreasureSpawnPosition.Count > 0)
                {
                    List<int> PossiblePositions = new List<int>();
                    for(int i = 0; i < room.TreasureSpawnPosition.Count; i++)
                    {
                        PossiblePositions.Add(i);
                    }
                    int ItemToSpawn = 0;
                    int Picked = rand.Next(PossiblePositions.Count);
                    PossiblePositions.RemoveAt(Picked);
                    if(r == FinalityRoomPosition && !SpawnedFinalityLoot)
                    {
                        ItemToSpawn = Finality;
                        SpawnedFinalityLoot = true;
                    }
                    else if (CreateLifeCrystals && rand.NextFloat() < LifeCrystalSpawnChance && LifeCrystals < 20)
                    {
                        ItemToSpawn = ItemID.LifeCrystal;
                        if (LifeCrystalSpawnChance > 1)
                            LifeCrystalSpawnChance -= 1;
                        else
                            LifeCrystalSpawnChance = 0;
                        LifeCrystals++;
                    }
                    else if(rand.NextFloat() < TreasureSpawnChance)
                    {
                        List<int> PossibleLoots = new List<int>();
                        int LastLootIndex = 0;
                        while(LastLootIndex < loot.Count)
                        {
                            if(roompos.Difficulty >= (int)loot[LastLootIndex].Difficulty)
                            {
                                PossibleLoots.Add(LastLootIndex);
                            }
                            LastLootIndex++;
                        }
                        if(PossibleLoots.Count > 0)
                        {
                            int PickedLoot = rand.Next(PossibleLoots.Count);
                            ItemToSpawn = loot[PossibleLoots[PickedLoot]].ItemID;
                            loot.RemoveAt(PossibleLoots[PickedLoot]);
                        }
                        TreasureSpawnChance--;
                    }
                    if (ItemToSpawn > 0)
                    {
                        TreasurePosition tp = new TreasurePosition(ItemToSpawn, Picked);
                        roompos.Treasures.Add(tp);
                    }
                }
                TreasureSpawnChance += 0.1f;
                LifeCrystalSpawnChance += 0.05f;
            }
        }

        private void CreateLootTable()
        {
            for (int i = 0; i < 5; i++)
            {
                AddLoot(ItemID.GreaterHealingPotion, DifficultyLevel.Hard);
                AddLoot(ItemID.GreaterManaPotion, DifficultyLevel.Hard);
                AddLoot(ItemID.HealingPotion, DifficultyLevel.Easy);
                AddLoot(ItemID.ManaPotion, DifficultyLevel.Easy);
                AddLoot(ItemID.LesserHealingPotion);
                AddLoot(ItemID.LesserManaPotion);
            }
            //Swords
            AddLoot(ItemID.IronBroadsword);
            AddLoot(ItemID.BoneSword);
            if(Main.xMas)
                AddLoot(ItemID.CandyCaneSword);
            AddLoot(ItemID.Katana);
            AddLoot(ItemID.IceBlade);
            AddLoot(ItemID.Muramasa);
            AddLoot(ItemID.DyeTradersScimitar);
            AddLoot(ItemID.Starfury);
            AddLoot(ItemID.BladedGlove);
            AddLoot(ItemID.EnchantedSword);
            AddLoot(ItemID.BeeKeeper);
            AddLoot(ItemID.BladeofGrass);
            AddLoot(ItemID.FieryGreatsword);
            AddLoot(ItemID.NightsEdge);
            AddLoot(ItemID.SlapHand);
            AddLoot(ItemID.IceSickle);
            AddLoot(ItemID.BreakerBlade);
            AddLoot(ItemID.Cutlass);
            AddLoot(ItemID.Frostbrand);
            AddLoot(ItemID.BeamSword);
            AddLoot(ItemID.Bladetongue);
            AddLoot(ItemID.FetidBaghnakhs);
            AddLoot(ItemID.Excalibur);
            AddLoot(ItemID.TrueExcalibur);
            AddLoot(ItemID.ChlorophyteSaber);
            AddLoot(ItemID.DeathSickle);
            AddLoot(ItemID.PsychoKnife);
            AddLoot(ItemID.ChlorophyteClaymore);
            AddLoot(ItemID.TheHorsemansBlade);
            AddLoot(ItemID.ChristmasTreeSword);
            AddLoot(ItemID.TrueNightsEdge);
            AddLoot(ItemID.Seedler);
            AddLoot(ItemID.TerraBlade);
            AddLoot(ItemID.InfluxWaver);
            AddLoot(ItemID.StarWrath);
            AddLoot(ItemID.Meowmere);
            //Spears
            AddLoot(ItemID.Spear);
            AddLoot(ItemID.Trident);
            AddLoot(ItemID.TheRottedFork);
            AddLoot(ItemID.DarkLance);
            AddLoot(ItemID.CobaltNaginata);
            AddLoot(ItemID.MythrilHalberd);
            AddLoot(ItemID.AdamantiteGlaive);
            AddLoot(ItemID.Gungnir);
            AddLoot(ItemID.ChlorophytePartisan);
            AddLoot(ItemID.MushroomSpear);
            AddLoot(ItemID.NorthPole);
            //Boomerangs
            AddLoot(ItemID.WoodenBoomerang);
            AddLoot(ItemID.EnchantedBoomerang);
            if(Main.halloween)
                AddLoot(ItemID.BloodyMachete);
            AddLoot(ItemID.IceBoomerang);
            AddLoot(ItemID.ThornChakram);
            AddLoot(ItemID.Flamarang);
            AddLoot(ItemID.FlyingKnife);
            AddLoot(ItemID.LightDisc);
            AddLoot(ItemID.PossessedHatchet);
            AddLoot(ItemID.PaladinsShield);
            //Flails
            AddLoot(ItemID.ChainKnife);
            AddLoot(ItemID.BallOHurt);
            AddLoot(ItemID.TheMeatball);
            AddLoot(ItemID.BlueMoon);
            AddLoot(ItemID.Sunfury);
            AddLoot(ItemID.KOCannon);
            AddLoot(ItemID.ChainGuillotines);
            AddLoot(ItemID.DaoofPow);
            AddLoot(ItemID.FlowerPow);
            AddLoot(ItemID.GolemFist);
            AddLoot(ItemID.Flairon);
        }

        private void AddLoot(int ItemID, DifficultyLevel Difficulty = DifficultyLevel.VeryEasy, Loot.LootType type = Loot.LootType.Normal)
        {
            loot.Add(new Loot(ItemID, type, Difficulty));
        }

        public int AvailRoomConnectors(Room NewRoom, int NewRoomX, int NewRoomY, RoomPosition ConnectedRoom)
        {
            int DisconnectedConnectors = 0;
            foreach(Connector c in ConnectedRoom.room.RoomConnectors)
            {
                if(!IsConnectorBlocked(ConnectedRoom, c))
                {
                    int ConnX, ConnY;
                    GetConnectorPosition(ConnectedRoom, c, out ConnX, out ConnY);
                    switch (c.Position)
                    {
                        case ConnectorPosition.Up:
                            ConnY--;
                            break;
                        case ConnectorPosition.Down:
                            ConnY++;
                            break;
                        case ConnectorPosition.Left:
                            ConnX--;
                            break;
                        case ConnectorPosition.Right:
                            ConnX++;
                            break;
                    }
                    bool ConnectedOrBlocked = ConnX >= NewRoomX && ConnX < NewRoomX + NewRoom.Width &&
                        ConnY >= NewRoomY && ConnY < NewRoomY + NewRoom.Height;
                    if (!ConnectedOrBlocked)
                    {
                        foreach (Connector newc in NewRoom.RoomConnectors)
                        {
                            int NewConX = NewRoomX + newc.PositionX - NewRoom.RoomTileStartX, NewConY = NewRoomY + newc.PositionY - NewRoom.RoomTileStartY;
                            if(NewConX == ConnX && NewConY == ConnY)
                            {
                                ConnectedOrBlocked = true;
                                break;
                            }
                        }
                    }
                    if (!ConnectedOrBlocked)
                        DisconnectedConnectors++;
                    else
                        DisconnectedConnectors--;
                }
            }
            foreach(Connector c in NewRoom.RoomConnectors)
            {
                int ConnX = NewRoomX + c.PositionX - NewRoom.RoomTileStartX,
                    ConnY = NewRoomY + c.PositionY - NewRoom.RoomTileStartY;
                switch (c.Position)
                {
                    case ConnectorPosition.Up:
                        ConnY--;
                        break;
                    case ConnectorPosition.Down:
                        ConnY++;
                        break;
                    case ConnectorPosition.Left:
                        ConnX--;
                        break;
                    case ConnectorPosition.Right:
                        ConnX++;
                        break;
                }
                bool Blocked = false;
                foreach (RoomPosition r in rooms)
                {
                    if(ConnX >= r.Position.X && ConnX < r.Position.X + r.Position.Width && 
                        ConnY >= r.Position.Y && ConnY < r.Position.Y + r.Position.Height)
                    {
                        Blocked = true;
                        break;
                    }
                }
                if(Blocked)
                    DisconnectedConnectors++;
            }
            return DisconnectedConnectors;
        }

        private void SpawnRoomTiles()
        {
            int Total = rooms.Count, Current = 0;
            float RoomPercentageChange = 1f / Total;
            foreach(RoomPosition rp in rooms)
            {
                progress.Message = "Placing Generated Rooms: " + Math.Round((float)Current * RoomPercentageChange) + "%";
                Main.statusText = progress.Message;
                SpawnRoom(rp);
                Current++;
            }
        }

        public void GetConnectorPosition(RoomPosition Room, Connector connector, out int PositionX, out int PositionY)
        {
            PositionX = Room.Position.X + connector.PositionX - Room.room.RoomTileStartX;
            PositionY = Room.Position.Y + connector.PositionY - Room.room.RoomTileStartY;
        }

        public void GetMobSpawnerPosition(RoomPosition Room, MobSpawnPos mob, out int PositionX, out int PositionY)
        {
            PositionX = Room.Position.X + mob.PositionX - Room.room.RoomTileStartX;
            PositionY = Room.Position.Y + mob.PositionY - Room.room.RoomTileStartY;
        }

        public int GetDisconnectedConnectors()
        {
            int DisconnectedConnectors = 0;
            foreach (RoomPosition room in rooms)
            {
                foreach (Connector c in room.room.RoomConnectors)
                {
                    if (!IsConnectorConnected(room, c) && CanUseConnector(c))
                    {
                        DisconnectedConnectors++;
                    }
                }
            }
            return DisconnectedConnectors;
        }

        public List<Connector> GetRoomPossibleConnectors(Room room, Connector connectortoconnect)
        {
            List<Connector> PossibleConnectors = new List<Connector>();
            foreach (Connector other in room.RoomConnectors)
            {
                if (CanConnectToConnector(connectortoconnect, other) && other.Distance == connectortoconnect.Distance && connectortoconnect.IsOpposite(other))
                    PossibleConnectors.Add(other);
            }
            return PossibleConnectors;
        }

        public Connector[] GetPossibleConnectors(RoomPosition Room)
        {
            List<Connector> PossibleConnectors = new List<Connector>();
            foreach (Connector c in Room.room.RoomConnectors)
            {
                if (!IsConnectorConnected(Room, c) && CanUseConnector(c) && !IsConnectorBlocked(Room, c))
                {
                    PossibleConnectors.Add(c);
                }
            }
            return PossibleConnectors.ToArray();
        }

        public RoomPosition? GetRandomPossibleRoom()
        {
            RoomPosition[] PossibleRooms = GetRoomsWithConnectors();
            if (PossibleRooms.Length == 0)
                return null;
            return PossibleRooms[rand.Next(PossibleRooms.Length)];
        }

        public RoomPosition[] GetRoomsWithConnectors()
        {
            List<RoomPosition> PossibleRooms = new List<RoomPosition>();
            foreach(RoomPosition room in rooms)
            {
                foreach(Connector c in room.room.RoomConnectors)
                {
                    if(!IsConnectorConnected(room, c) && CanUseConnector(c) && !IsConnectorBlocked(room, c))
                    {
                        PossibleRooms.Add(room);
                        break;
                    }
                }
            }
            return PossibleRooms.ToArray();
        }

        public bool IsConnectorOnDangerousPosition(Connector connector, RoomPosition room)
        {
            int ConnectorX = room.Position.X + connector.PositionX - room.room.RoomTileStartX,
                ConnectorY = room.Position.Y + connector.PositionY - room.room.RoomTileStartY;
            if (connector.Position == ConnectorPosition.Right)
                ConnectorX += room.Position.Width;
            if (connector.Position == ConnectorPosition.Down)
                ConnectorY += room.Position.Height;
            switch (connector.Position)
            {
                case ConnectorPosition.Up:
                    return ConnectorY < 100;
                case ConnectorPosition.Right:
                    return ConnectorX >= Main.maxTilesX - 100;
                case ConnectorPosition.Down:
                    return ConnectorY >= Main.maxTilesY - 100;
                case ConnectorPosition.Left:
                    return ConnectorX < 100;
            }
            return ConnectorX < 50 || ConnectorX >= Main.maxTilesX - 50 || ConnectorY < 50 || ConnectorY >= Main.maxTilesY - 50;
        }

        public bool CanConnectToConnector(Connector Connector1, Connector Connector2)
        {
            if (Connector1.Distance != Connector2.Distance)
                return false;
            if (Connector1.Requirement == Connector2.Requirement && Connector1.IsOpposite(Connector2))
                return CanUseConnector(Connector2);
            return false;
        }

        public bool IsConnectorBlocked(RoomPosition Room, Connector connector)
        {
            int ConnectorPosX = Room.Position.X + connector.PositionX - Room.room.RoomTileStartX,
                ConnectorPosY = Room.Position.Y + connector.PositionY - Room.room.RoomTileStartY;
            switch (connector.Position)
            {
                case ConnectorPosition.Up:
                    ConnectorPosY--;
                    break;
                case ConnectorPosition.Down:
                    ConnectorPosY++;
                    break;
                case ConnectorPosition.Left:
                    ConnectorPosX--;
                    break;
                case ConnectorPosition.Right:
                    ConnectorPosX++;
                    break;
            }
            foreach(RoomPosition other in rooms)
            {
                if (ConnectorPosX >= other.Position.X && ConnectorPosX < other.Position.X + other.Position.Width &&
                    ConnectorPosY >= other.Position.Y && ConnectorPosY < other.Position.Y + other.Position.Height)
                    return true;
            }
            return false;
        }

        public bool CanUseConnector(Connector connector)
        {
            switch (connector.Requirement)
            {
                case ConnectorRequirement.DoubleJump:
                    return PlayerGotDoubleJump;
                case ConnectorRequirement.Flight:
                    return PlayerGotFlight;
                case ConnectorRequirement.RodOfDiscord:
                    return PlayerGotRod;
                case ConnectorRequirement.Water:
                    return PlayerGotSwiming;
                case ConnectorRequirement.Lava:
                    return PlayerGotLavaImmunity;
                case ConnectorRequirement.None:
                    return true;
                case ConnectorRequirement.Honey:
                    return PlayerGotSwiming;
                case ConnectorRequirement.Waterwalking:
                    return PlayerGotWaterwalking;
            }
            return false;
        }

        private void CheckForDisconnectedConnectors()
        {
            progress.Message = "Blocking Doors with Disconnected Connectors.";
            Main.statusText = progress.Message;
            foreach(RoomPosition room in rooms)
            {
                foreach(Connector c in room.room.RoomConnectors)
                {
                    if(!IsConnectorConnected(room, c))
                    {
                        int StartX = room.Position.X + c.PositionX - room.room.RoomTileStartX,
                            StartY = room.Position.Y + c.PositionY - room.room.RoomTileStartY;
                        bool NegativeBlockDim = c.Position == ConnectorPosition.Right || c.Position == ConnectorPosition.Down;
                        for (byte BlockDim = 0; BlockDim < c.BlockDimension; BlockDim++)
                        {
                            for (int dist = 0; dist < c.Distance; dist++)
                            {
                                int PosX = StartX, PosY = StartY;
                                if (c.Horizontal)
                                {
                                    PosX += dist;
                                    PosY += BlockDim * (NegativeBlockDim ? -1 : 1);
                                }
                                else
                                {
                                    PosX += BlockDim * (NegativeBlockDim ? -1 : 1);
                                    PosY += dist;
                                }
                                WorldGen.PlaceTile(PosX, PosY, c.BlockTile);
                                /*Main.tile[PosX, PosY].HasTile = true;
                                Main.tile[PosX, PosY].Slope = 0;
                                Main.tile[PosX, PosY].TileType = c.BlockTile;*/
                                WorldGen.TileFrame(PosX, PosY);
                            }
                        }
                    }
                }
            }
        }

        private void BlockOpenSpaces()
        {
            /*for(int x = 49; x < Main.maxTilesX - 49; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 49; y++)
                {

                }
            }*/
        }

        private void FinishGeneration()
        {
            progress.Message = "Finishing up.";
            Main.statusText = progress.Message;
            WorldMod.Rooms.Clear();
            //Turn the RoomPosition stuff into RoomInfo, and place them on WorldMod.Rooms
            while (rooms.Count > 0)
            {
                RoomPosition rp = rooms[0];
                rooms.RemoveAt(0);
                RoomInfo ri = new RoomInfo() { RoomID = rp.room.ID, RoomX = rp.Position.X, RoomY = rp.Position.Y, ZoneID = rp.room.ParentZone.ID, ZoneName = rp.room.ParentZone.Name,Difficulty = rp.Difficulty };
                foreach(MobPosition m in rp.Mobs)
                {
                    ri.Mobs.Add(new RoomInfo.MobSlot() { MobID = m.MobID, Slot = m.SpawnSlot });
                }
                foreach(TreasurePosition p in rp.Treasures)
                {
                    ri.Treasures.Add(new RoomInfo.TreasureSlot() { ItemID = p.ItemID, Slot = p.SpawnSlot });
                }
                WorldMod.Rooms.Add(ri);
            }
            randomizer = null;
        }

        private bool IsConnectorConnected(RoomPosition room, Connector connector)
        {
            int CheckPosX = room.Position.X + connector.PositionX - room.room.RoomTileStartX,
                CheckPosY = room.Position.Y + connector.PositionY - room.room.RoomTileStartY;
            switch (connector.Position)
            {
                case ConnectorPosition.Up:
                    CheckPosY--;
                    break;
                case ConnectorPosition.Down:
                    CheckPosY++;
                    break;
                case ConnectorPosition.Left:
                    CheckPosX--;
                    break;
                case ConnectorPosition.Right:
                    CheckPosX++;
                    break;
            }
            foreach (RoomPosition OtherRoom in rooms)
            {
                if (CheckPosX >= OtherRoom.Position.X && CheckPosX < OtherRoom.Position.X + OtherRoom.Position.Width &&
                    CheckPosY >= OtherRoom.Position.Y && CheckPosY < OtherRoom.Position.Y + OtherRoom.Position.Height)
                {
                    foreach (Connector otherconnector in OtherRoom.room.RoomConnectors)
                    {
                        if (connector.Horizontal == otherconnector.Horizontal)
                        {
                            if (CheckPosX == OtherRoom.Position.X + otherconnector.PositionX - OtherRoom.room.RoomTileStartX &&
                                CheckPosY == OtherRoom.Position.Y + otherconnector.PositionY - OtherRoom.room.RoomTileStartY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool HasRoomHere(Rectangle rect)
        {
            foreach (RoomPosition room in rooms)
            {
                if (room.Position.Intersects(rect))
                    return true;
            }
            return false;
        }

        public bool HasRoomHere(int PosX, int PosY, byte RoomWidth, byte RoomHeight)
        {
            Rectangle rect = new Rectangle(PosX, PosY, RoomWidth, RoomHeight);
            foreach(RoomPosition room in rooms)
            {
                if (room.Position.Intersects(rect))
                    return true;
            }
            return false;
        }

        private bool PlaceRoom(Room NewRoom, RoomPosition ParentRoom, Connector ParentConnector, float DifficultyLevel, Connector PickedConnector = null)
        {
            List<Connector> PossibleConnectors = new List<Connector>();
            if (PickedConnector == null)
            {
                foreach (Connector nc in NewRoom.RoomConnectors)
                {
                    if (CanUseConnector(nc) && !IsConnectorBlocked(ParentRoom, ParentConnector) && CanConnectToConnector(nc, ParentConnector) && CanPlaceRoomHere(ParentConnector, ParentRoom, NewRoom))
                    {
                        PossibleConnectors.Add(nc);
                    }
                }
            }
            else
            {
                PossibleConnectors.Add(PickedConnector);
            }
            while (PossibleConnectors.Count > 0)
            {
                int p = rand.Next(PossibleConnectors.Count);
                Connector NewConnector = PossibleConnectors[p];
                PossibleConnectors.RemoveAt(p);
                int RoomX = ParentRoom.Position.X, RoomY = ParentRoom.Position.Y;
                switch (ParentConnector.Position) //Placement horizontally seems off. Vertically might be off too.
                {
                    case ConnectorPosition.Up:
                        RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                        RoomY -= NewRoom.Height;
                        break;
                    case ConnectorPosition.Down:
                        RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                        RoomY += ParentRoom.Position.Height;
                        break;
                    case ConnectorPosition.Left:
                        RoomX -= NewRoom.Width;
                        RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                        break;
                    case ConnectorPosition.Right:
                        RoomX += ParentRoom.Position.Width;
                        RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                        break;
                }
                if (PlaceRoom(NewRoom, RoomX, RoomY, DifficultyLevel, false))
                    return true;
            }
            return false;
        }

        private bool PlaceRoom(Room room, int X, int Y, float DifficultyLevel, bool Starter = false)
        {
            Rectangle rect = new Rectangle(X, Y, room.Width, room.Height);
            if (!Starter && (X < 50|| X + room.Width >= Main.maxTilesX - 50 || Y < Main.worldSurface * 0.5f || Y + room.Height >= Main.maxTilesY - 50))
            {
                return false;
            }
            for (int r = 0; r < rooms.Count; r++)
            {
                RoomPosition otherroom = rooms[r];
                if (otherroom.Position.Intersects(rect))
                {
                    return false;
                }
            }
            RoomPosition rp = new RoomPosition(room, rect, DifficultyLevel);
            rooms.Add(rp);
            return true;
        }

        private void SpawnRoom(RoomPosition rp)
        {
            TilesetPacker.TileStep[,] TileMap = rp.room.ParentZone.GetTilesetMap();
            Zone zone = rp.room.ParentZone;
            //throw new Exception("Tile map size: " + TileMap.GetLength(0) + " ~ " + TileMap.GetLength(1));
            for (int y = 0; y < rp.room.Height; y++)
            {
                for (int x = 0; x < rp.room.Width; x++)
                {
                    int TilePosX = rp.Position.X + x, TilePosY = rp.Position.Y + y;
                    TilesetPacker.TileStep t = TileMap[rp.room.RoomTileStartX + x, rp.room.RoomTileStartY + y];
                    bool HasTile = t.HasTile;
                    if (HasTile)
                        WorldGen.PlaceTile(TilePosX, TilePosY, t.TileType, mute: true, forced: true, style: t.TileFrameNum);
                    else
                        WorldGen.KillTile(TilePosX, TilePosY, noItem: true);
                    WorldGen.KillWall(TilePosX, TilePosY);
                    if (t.WallType > 0)
                    {
                        WorldGen.PlaceWall(TilePosX, TilePosY, t.WallType, mute: true);
                    }
                    WorldGen.paintTile(TilePosX, TilePosY, t.TileColor);
                    WorldGen.paintWall(TilePosX, TilePosY, t.WallColor);
                    if (t.RedWire)
                        WorldGen.PlaceWire(TilePosX, TilePosY);
                    if (t.GreenWire)
                        WorldGen.PlaceWire2(TilePosX, TilePosY);
                    if (t.BlueWire)
                        WorldGen.PlaceWire3(TilePosX, TilePosY);
                    if (t.YellowWire)
                        WorldGen.PlaceWire4(TilePosX, TilePosY);
                    if (t.HasActuator)
                        WorldGen.PlaceActuator(TilePosX, TilePosY);
                    if (t.LiquidAmount == 0)
                        WorldGen.EmptyLiquid(TilePosX, TilePosY);
                    else
                        WorldGen.PlaceLiquid(TilePosX, TilePosY, (byte)t.LiquidType, t.LiquidAmount);
                    //if (t.IsActuated) //How?
                    //    WorldGen.Tile
                    //Main.tile[TilePosX, TilePosY].CopyFrom(t);
                    //Color color = ColorMap[rp.room.RoomTileStartX + x, rp.room.RoomTileStartY + y];
                    //Main.tile[TilePosX, TilePosY].HasTile = false;
                    /*WorldGen.KillTile(TilePosX, TilePosY, noItem: true);
                    Main.tile[TilePosX, TilePosY].WallType = 0;
                    bool FoundTileInfo = false;
                    foreach (TileInfo Ti in rp.room.RoomMapCodes)
                    {
                        if (Ti.IsSameColor(color))
                        {
                            FoundTileInfo = true;
                            PlaceTile(TilePosX, TilePosY, Ti);
                            break;
                        }
                    }
                    if (!FoundTileInfo)
                    {
                        foreach (TileInfo Ti in zone.ZoneTileInfoCodes)
                        {
                            if (Ti.IsSameColor(color))
                            {
                                FoundTileInfo = true;
                                PlaceTile(TilePosX, TilePosY, Ti);
                                break;
                            }
                        }
                    }*/
                }
            }
            foreach (FurnitureSpawnPos furniture in rp.room.FurnitureSpawnPosition)
            {
                int FurnitureX = rp.Position.X + furniture.PositionX - rp.room.RoomTileStartX,
                    FurnitureY = rp.Position.Y + furniture.PositionY - rp.room.RoomTileStartY;
                if (furniture.FurnitureID == Terraria.ID.TileID.Saplings)
                {
                    WorldGen.GrowTree(FurnitureX, FurnitureY);
                }
                if (furniture.FurnitureID == 240)
                {
                    int PortraitToSpawn = 0;
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            PortraitToSpawn = Main.rand.Next(13, 17);
                            break;
                        case 1:
                            PortraitToSpawn = Main.rand.Next(19, 37);
                            break;
                        case 2:
                            PortraitToSpawn = Main.rand.Next(64, 72);
                            break;
                    }
                    WorldGen.PlaceTile(FurnitureX, FurnitureY, furniture.FurnitureID, style: PortraitToSpawn);
                }
                else if (furniture.FurnitureID == 242)
                {
                    int PortraitToSpawn = 0;
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            PortraitToSpawn = Main.rand.Next(0, 18);
                            break;
                        case 1:
                            PortraitToSpawn = Main.rand.Next(27, 32);
                            break;
                    }
                    if (Main.xMas && Main.rand.NextDouble() < 0.4)
                    {
                        PortraitToSpawn = Main.rand.Next(32, 37);
                    }
                    if (Main.halloween && Main.rand.NextDouble() < 0.4)
                    {
                        PortraitToSpawn = Main.rand.Next(18, 23);
                    }
                    WorldGen.PlaceTile(FurnitureX, FurnitureY, furniture.FurnitureID, style: PortraitToSpawn);
                }
                else if (furniture.FurnitureID == 245)
                {
                    int PortraitToSpawn = Main.rand.Next(0, 14);
                    WorldGen.PlaceTile(FurnitureX, FurnitureY, furniture.FurnitureID, style: PortraitToSpawn);
                }
                else if (furniture.FurnitureID == 246)
                {
                    int PortraitToSpawn = Main.rand.Next(0, 19);
                    WorldGen.PlaceTile(FurnitureX, FurnitureY, furniture.FurnitureID, style: PortraitToSpawn);
                }
                else
                {
                    WorldGen.PlaceTile(FurnitureX, FurnitureY, furniture.FurnitureID, true, true, -1, furniture.Style);
                    if (furniture.FurnitureID == Terraria.ID.TileID.Chairs && !furniture.FacingLeft)
                    {
                        while (true)
                        {
                            if (!Main.tile[FurnitureX, FurnitureY].HasTile || Main.tile[FurnitureX, FurnitureY].TileType != Terraria.ID.TileID.Chairs)
                                break;
                            Main.tile[FurnitureX, FurnitureY].TileFrameX += 18;
                            FurnitureY--;
                        }
                    }
                }
            }
            for (int y = -1; y <= rp.room.Height; y++)
            {
                for (int x = -1; x <= rp.room.Width; x++)
                {
                    int TilePosX = rp.Position.X + x, TilePosY = rp.Position.Y + y;
                    if (TilePosX >= 0 && TilePosX < Main.maxTilesX && TilePosY >= 0 && TilePosY <= Main.maxTilesY)
                    {
                        WorldGen.TileFrame(TilePosX, TilePosY);
                    }
                }
            }
        }

        private void PlaceTile(int X, int Y, TileInfo Ti)
        {
            //Main.tile[X, Y].Slope = 0;
            if (Ti.Active)
            {
                //Main.tile[X, Y].HasTile = true;
                WorldGen.PlaceTile(X, Y, Ti.TileID);
                //Main.tile[X, Y].TileType = Ti.TileID;
                Main.tile[X, Y].TileFrameX = Ti.TileX;
                Main.tile[X, Y].TileFrameY = Ti.TileY;
            }
            else
            {
                WorldGen.KillTile(X, Y, noItem: true);
                //Main.tile[X, Y].HasTile = false;
            }
            Main.tile[X, Y].WallType = Ti.WallID;
            WorldGen.PlaceLiquid(X, Y, Ti.LiquidID, Ti.LiquidValue);
            //Main.tile[X, Y].LiquidType = Ti.LiquidID;
            //Main.tile[X, Y].LiquidAmount = Ti.LiquidValue;
            //WorldGen.TileFrame(X, Y);
        }

        private void GenerateInvalidWorld(string ErrorMessage)
        {
            InvalidWorld = true;
            progress.Message = "Problem found. Creating invalid world.:\n" + ErrorMessage;
            progress.CurrentPassWeight = 1;
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    WorldGen.PlaceTile(x, y, Terraria.ID.TileID.StoneSlab);
                    //Main.tile[x, y].HasTile = true;
                    //Main.tile[x, y].TileType = Terraria.ID.TileID.StoneSlab;
                    WorldGen.TileFrame(x, y);
                }
            }
            progress.CurrentPassWeight = 5;
            int SpawnX = Main.maxTilesX / 2;
            int SpawnY = Main.maxTilesY / 2;
            Main.spawnTileX = SpawnX;
            Main.spawnTileY = SpawnY;
            for(int x = 0; x < 15; x++)
            {
                for(int dir = -1; dir < 2; dir += 2)
                {
                    for(int y = 0; y < 10; y++)
                    {
                        int TileX = SpawnX + x * dir, TileY = SpawnY - y;
                        if (dir == 1 && x == 0)
                            continue;
                        WorldGen.KillTile(TileX, TileY, noItem: true);
                        //Main.tile[TileX, TileY].HasTile = false;
                        WorldGen.TileFrame(TileX, TileY);
                    }
                }
            }
            for(int i = -1; i < 2; i++)
            {
                int TileX = SpawnX + i * 10 + i * 2, TileY = SpawnY;
                WorldGen.Place1xX(TileX, TileY, Terraria.ID.TileID.Lampposts);
            }
            progress.CurrentPassWeight = 9;
            WorldGen.Place2x2(SpawnX + 3, SpawnY, Terraria.ID.TileID.Signs, 0);
            int SignID = Sign.ReadSign(SpawnX + 3, SpawnY);
            if(SignID > -1)
            {
                Main.sign[SignID].text = ErrorMessage + "\nPlease report to the mod developer.";
            }
        }

        /// <summary>
        /// Upon placing a room, It should check the connectors, and see if they're connected to another room.
        /// If the connector doesn't connects to another room, then the connector should be added to the OrphanConnectors list.
        /// In case it finds a room, It must check if there's any connector connected to it. If there is, try removing it from OrphanConnectors list.
        /// </summary>

        public struct RoomPosition
        {
            public Room room;
            public Rectangle Position;
            public float Difficulty;
            public List<MobPosition> Mobs;
            public List<TreasurePosition> Treasures;

            public RoomPosition(Room r, Rectangle pos, float DifficultyLevel)
            {
                this.room = r;
                Position = pos;
                Difficulty = DifficultyLevel;
                Mobs = new List<MobPosition>();
                Treasures = new List<TreasurePosition>();
            }

            public void AddMob(int MobID, int SpawnSlot)
            {
                Mobs.Add(new MobPosition(MobID, SpawnSlot));
            }

            public void AddTreasure(int ItemID, int SpawnSlot)
            {
                Treasures.Add(new TreasurePosition(ItemID, SpawnSlot));
            }
        }

        public struct MobPosition
        {
            public int MobID, SpawnSlot;

            public MobPosition(int MobID, int Slot)
            {
                this.MobID = MobID;
                SpawnSlot = Slot;
            }
        }

        public struct TreasurePosition
        {
            public int ItemID, SpawnSlot;

            public TreasurePosition(int ItemID, int SpawnSlot)
            {
                this.ItemID = ItemID;
                this.SpawnSlot = SpawnSlot;
            }
        }
    }
}
