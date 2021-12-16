﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CastleGenerator.Generator
{
    public class GenerateCastle : GenPass
    {
        const float LoadWeight = 10;
        private List<RoomPosition> rooms = new List<RoomPosition>();
        private GenerationProgress progress;
        private bool InvalidWorld = false;
        private int RoomsToGenerate = 250;
        public static Terraria.Utilities.UnifiedRandom rand { get { return WorldGen.genRand; } }
        private byte LifeCrystals = 0;
        private List<Loot> loot = new List<Loot>();
        private bool PlayerGotDoubleJump = false, PlayerGotFlight = false, PlayerGotRod = false, PlayerGotSwiming = false, PlayerGotLavaImmunity = false, PlayerGotRunningBoots = false;

        public GenerateCastle() : base("CastleGenerator: Generate Castle", LoadWeight)
        {
        }

        public override void Apply(GenerationProgress progress)
        {
            this.progress = progress;
            WorldMod.IsCastle = true;
            NPC.downedBoss3 = true;
            Main.hardMode = true;
            CreateSpawnPoint();
            if (InvalidWorld)
                return;
            TestCreateCastleRooms();
            if (InvalidWorld)
                return;
            PlaceTreasures();
            PlaceMonsters();
            SpawnRoomTiles();
            CheckForDisconnectedConnectors();
            FinishGeneration();
        }

        private void CreateSpawnPoint()
        {
            progress.Message = "Placing Castle Entrance.";
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
                    if (!Main.tile[SpawnRefPosX, RoomPositionY].active())
                        RoomPositionY++;
                    else
                        break;
                }
            }
            RoomPositionY += -StartingRoom.Height + PickedConnector.PositionY - StartingRoom.RoomTileStartY;
            if (!PickedConnector.Horizontal)
                RoomPositionY += PickedConnector.Distance;
            if (!PlaceRoom(StartingRoom, RoomPositionX, RoomPositionY, 1, true))
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
            float DifficultyLevel = 1;
            bool CanCreateDeadEnds = false;
            bool CreateBoss = false;
            byte ForkedRoad = (byte)rand.Next(2, 4);
            List<RoomPosition> RoomsWithOpenConnectors = new List<RoomPosition>();
            RoomsWithOpenConnectors.Add(rooms[0]);
            Zone PickedZone = rooms[rooms.Count - 1].room.ParentZone;
            bool GenerationFinished = false;
            while (true)
            {
                if(RoomsWithOpenConnectors.Count == 0)
                {
                    GenerateInvalidWorld("Somehow the mod found a deadend.");
                    return;
                }
                //Checking Progress...
                int CurrentCastleDimension = 0;
                foreach (RoomPosition room in rooms)
                {
                    CurrentCastleDimension += room.Position.Width * room.Position.Height;
                }
                {
                    float Percentage = (int)((float)CurrentCastleDimension * 100 / MaxSize);
                    progress.Message = "Generating Castle: Filled " + Percentage + "% of the map.";
                    if (Percentage >= 70)
                    {
                        GenerationFinished = true;
                        break;
                    }
                }
                RoomPosition ParentRoom = RoomsWithOpenConnectors[RoomsWithOpenConnectors.Count - 1];
                RoomType NewRoomType = RoomType.Normal;
                bool MakeForkedRoad = false;
                if (ForkedRoad == 0)
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
                foreach (Room room in PickedZone.RoomTypes)
                {
                    if(room.roomType == NewRoomType)
                    {
                        byte OpenConnectors = 0;
                        foreach(Connector ParentConnector in ParentRoom.room.RoomConnectors)
                        {
                            if(!IsConnectorConnected(ParentRoom, ParentConnector) && !IsConnectorBlocked(ParentRoom, ParentConnector) && CanUseConnector(ParentConnector))
                            {
                                OpenConnectors++;
                            }
                        }
                        if ((CanCreateDeadEnds && ((!MakeForkedRoad && OpenConnectors > 1) || OpenConnectors > 2)) || OpenConnectors > 1)
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
                            goto retryRoomPicking;
                        }
                        else
                        {
                            //Something should happen in case this exception happens.
                        }
                    }
                }
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
                    while (PossibleConnectors.Count > 0 && !Success)
                    {
                        int PickedParentConnector = rand.Next(PossibleConnectors.Count);
                        Connector ParentConnector = PossibleConnectors[PickedParentConnector];
                        PossibleConnectors.RemoveAt(PickedParentConnector);
                        List<Connector> NewRoomPossibleConnectors = new List<Connector>();
                        foreach (Connector NewRoomConnector in ParentRoom.room.RoomConnectors)
                        {
                            if(CanUseConnector(NewRoomConnector) && CanConnectToConnector(ParentConnector, NewRoomConnector))
                            {
                                NewRoomPossibleConnectors.Add(NewRoomConnector);
                            }
                        }
                        while (NewRoomPossibleConnectors.Count > 0 && !Success)
                        {
                            int PickedNewConnector = rand.Next(NewRoomPossibleConnectors.Count);
                            Connector NewConnector = NewRoomPossibleConnectors[PickedNewConnector];
                            NewRoomPossibleConnectors.RemoveAt(PickedNewConnector);
                            int RoomX = ParentRoom.Position.X, RoomY = ParentRoom.Position.Y;
                            switch (ParentConnector.Position)
                            {
                                case ConnectorPosition.Up:
                                    RoomY -= NewRoom.Height;
                                    RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                                    break;
                                case ConnectorPosition.Right:
                                    RoomX += ParentRoom.Position.Width;
                                    RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                                    break;
                                case ConnectorPosition.Down:
                                    RoomX -= (NewConnector.PositionX - NewRoom.RoomTileStartX) - (ParentConnector.PositionX - ParentRoom.room.RoomTileStartX);
                                    RoomY += ParentRoom.Position.Height;
                                    break;
                                case ConnectorPosition.Left:
                                    RoomX -= NewRoom.Width;
                                    RoomY -= (NewConnector.PositionY - NewRoom.RoomTileStartY) - (ParentConnector.PositionY - ParentRoom.room.RoomTileStartY);
                                    break;
                            }
                            if (!HasRoomHere(RoomX, RoomY, NewRoom.Width, NewRoom.Height) && PlaceRoom(NewRoom, RoomX, RoomY, DifficultyLevel))
                            {
                                Success = true;
                            }
                        }
                    }
                }
                if (Success)
                {
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
                        CorridorRoom = (byte)rand.Next(12, 17);
                    foreach(Connector c in ParentRoom.room.RoomConnectors)
                    {
                        if(IsConnectorBlocked(ParentRoom, c) || IsConnectorConnected(ParentRoom, c))
                        {
                            RoomsWithOpenConnectors.Remove(ParentRoom);
                        }
                    }
                }
            }
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
                float Difficulty = 1f + (float)rooms.Count * 10 / RoomsToGenerate;
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
                                NewRoomPos.Y -= NewRoom.Height;
                                NewRoomPos.X -= (otherconnector.PositionX - NewRoom.RoomTileStartX) - (connector.PositionX - Room.room.RoomTileStartX);
                                break;
                            case ConnectorPosition.Down:
                                NewRoomPos.X -= (otherconnector.PositionX - NewRoom.RoomTileStartX) - (connector.PositionX - Room.room.RoomTileStartX);
                                NewRoomPos.Y += Room.room.Height;
                                break;
                            case ConnectorPosition.Left:
                                NewRoomPos.Y -= (otherconnector.PositionY - NewRoom.RoomTileStartY) - (connector.PositionY - Room.room.RoomTileStartY);
                                NewRoomPos.X -= NewRoom.Width;
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
                float DifficultyGrade = room.Difficulty + 0.5f * room.Treasures.Count;
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
            CreateLootTable();
            progress.Message = "Adding Treasures to the Castle";
            float LifeCrystalSpawnChance = 0.5f;
            float TreasureSpawnChance = 0.2f;
            List<int> PickedItemIDs = new List<int>();
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
                    if (rand.NextFloat() < LifeCrystalSpawnChance && LifeCrystals < 20)
                    {
                        ItemToSpawn = Terraria.ID.ItemID.LifeCrystal;
                        if (LifeCrystalSpawnChance > 1)
                            LifeCrystalSpawnChance -= 1;
                        else
                            LifeCrystalSpawnChance = 0;
                        LifeCrystals++;
                    }
                    else if(rand.NextFloat() < TreasureSpawnChance)
                    {
                        while (ItemToSpawn == 0 || PickedItemIDs.Contains(ItemToSpawn))
                        {
                            ItemToSpawn = Main.rand.Next(1, Main.maxItemTypes);
                        }
                        PickedItemIDs.Add(ItemToSpawn);
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
            progress.Message = "Placing Generated Rooms";
            foreach(RoomPosition rp in rooms)
            {
                SpawnRoom(rp);
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
            if (Connector1.Requirement == Connector2.Requirement)
                return true;
            return CanUseConnector(Connector2);
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
                    return true;
            }
            return false;
        }

        private void CheckForDisconnectedConnectors()
        {
            progress.Message = "Blocking Doors with Disconnected Connectors.";
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
                                Tile tile = Main.tile[PosX, PosY];
                                tile.active(true);
                                tile.slope(0);
                                tile.type = c.BlockTile;
                                WorldGen.TileFrame(PosX, PosY);
                            }
                        }
                    }
                }
            }
        }

        private void FinishGeneration()
        {
            progress.Message = "Finishing up.";
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

        private bool PlaceRoom(Room room, int X, int Y, float DifficultyLevel, bool Starter = false)
        {
            Rectangle rect = new Rectangle(X, Y, room.Width, room.Height);
            if (!Starter && (X < 50|| X + room.Width >= Main.maxTilesX - 50 || Y < 50 || Y + room.Height >= Main.maxTilesY - 50))
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
            Color[,] ColorMap = rp.room.ParentZone.GetColorMap();
            Zone zone = rp.room.ParentZone;
            for (int y = 0; y < rp.room.Height; y++)
            {
                for (int x = 0; x < rp.room.Width; x++)
                {
                    int TilePosX = rp.Position.X + x, TilePosY = rp.Position.Y + y;
                    Color color = ColorMap[rp.room.RoomTileStartX + x, rp.room.RoomTileStartY + y];
                    Tile tile = Main.tile[TilePosX, TilePosY];
                    tile.active(false);
                    tile.wall = 0;
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
                    }
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
                            if (!Main.tile[FurnitureX, FurnitureY].active() || Main.tile[FurnitureX, FurnitureY].type != Terraria.ID.TileID.Chairs)
                                break;
                            Main.tile[FurnitureX, FurnitureY].frameX += 18;
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
            Tile tile = Main.tile[X, Y];
            tile.slope(0);
            if (Ti.Active)
            {
                tile.active(true);
                tile.type = Ti.TileID;
                tile.frameX = Ti.TileX;
                tile.frameY = Ti.TileY;
            }
            else
            {
                tile.active(false);
            }
            tile.wall = Ti.WallID;
            tile.liquidType(Ti.LiquidID);
            tile.liquid = Ti.LiquidValue;
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
                    Main.tile[x, y].active(true);
                    Main.tile[x, y].type = Terraria.ID.TileID.StoneSlab;
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
                        Main.tile[TileX, TileY].active(false);
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

        public struct Loot
        {
            public int ItemID;
            public LootType ltype;
            public DifficultyLevel Difficulty;

            public Loot(int ItemID, LootType type = LootType.Normal, DifficultyLevel Difficulty = DifficultyLevel.VeryEasy)
            {
                this.ItemID = ItemID;
                ltype = type;
                this.Difficulty = Difficulty;
            }

            public enum LootType : byte
            {
                Normal,
                DoubleJump,
                Flight,
                Teleport,
                Waterbreathing,
                Lavaswimming
            }
        }
    }
}
