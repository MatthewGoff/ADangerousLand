using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private delegate void PostInitAction();

    public bool RiversInitialized { get; private set; } = false;
    public bool Initialized { get; private set; } = false;
    public bool LocalityInitialized { get; set; } = false;
    public bool Awake { get; set; } = false;

    private readonly List<EnemyManager> ResidentEnemies;
    private readonly List<WorldLocation> UnocupiedTiles;

    private readonly World MyWorld;
    private readonly ChunkIndex ChunkIndex;
    private readonly RiverNode[,] RiverNodes;
    private readonly List<RiverPackage> ImportedRivers;
    private readonly Tile[,] Tiles;
    private readonly Queue<PostInitAction> PostInitActions;

    public Chunk(World myWorld, ChunkIndex chunkIndex)
    {
        MyWorld = myWorld;
        ChunkIndex = chunkIndex;
        ImportedRivers = new List<RiverPackage>();
        RiverNodes = CreateRiverNodes();
        Tiles = CreateTiles();
        ResidentEnemies = new List<EnemyManager>();
        UnocupiedTiles = new List<WorldLocation>();
        PostInitActions = new Queue<PostInitAction>();
    }

    public void Update(Vector2 playerPosition)
    {
        if (!Initialized)
        {
            return;
        }
        for (int chunkX = 0; chunkX < Tiles.GetLength(0); chunkX++)
        {
            for (int chunkY = 0; chunkY < Tiles.GetLength(1); chunkY++)
            {
                Tile tile = Tiles[chunkX, chunkY];
                float euclidianDistance = tile.EuclidianDistanceToPlayer(playerPosition);
                float squareDistance = tile.SquareDistanceToPlayer(playerPosition);
                if (squareDistance <= Configuration.TREADMILL_RADIUS && !tile.Awake)
                {
                    tile.WakeUp();
                }
                else if (squareDistance > Configuration.TREADMILL_RADIUS && tile.Awake)
                {
                    tile.Sleep();
                }
                tile.UpdateFog(euclidianDistance);
            }
        }
    }

    public void SpawnEnemies()
    {
        if (!Initialized)
        {
            PostInitActions.Enqueue(SpawnEnemies);
        }
        else
        {
            while (ResidentEnemies.Count < Configuration.EnemiesPerChunk && UnocupiedTiles.Count != 0)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        WorldLocation spawnLocation = UnocupiedTiles[Util.RandomInt(0, UnocupiedTiles.Count)];
        UnocupiedTiles.Remove(spawnLocation);

        ResidentEnemies.Add(new EnemyManager(spawnLocation));
        GameObject Enemy = GameObject.Instantiate(Prefabs.ENEMY_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
    }

    private Tile[,] CreateTiles()
    {
        Tile[,] tiles = new Tile[Configuration.CHUNK_SIZE, Configuration.CHUNK_SIZE];
        for (int chunkX = 0; chunkX < Configuration.CHUNK_SIZE; chunkX++)
        {
            for (int chunkY = 0; chunkY < Configuration.CHUNK_SIZE; chunkY++)
            {
                WorldLocation worldLocation = ChunkToWorldLocation(new ChunkLocation(chunkX, chunkY));
                tiles[chunkX, chunkY] = new Tile(worldLocation);
            }
        }
        return tiles;
    }

    private RiverNode[,] CreateRiverNodes()
    {
        RiverNode[,] riverNodes = new RiverNode[Configuration.CHUNK_SIZE, Configuration.CHUNK_SIZE];
        for (int chunkX = 0; chunkX < Configuration.CHUNK_SIZE; chunkX++)
        {
            for (int chunkY = 0; chunkY < Configuration.CHUNK_SIZE; chunkY++)
            {
                riverNodes[chunkX, chunkY] = new RiverNode();
            }
        }
        return riverNodes;
    }

    public void InitializeInternalRivers()
    {
        for (int worldX = ChunkIndex.X* Configuration.CHUNK_SIZE; worldX < (ChunkIndex.X+1)* Configuration.CHUNK_SIZE; worldX++)
        {
            for (int worldY = ChunkIndex.Y* Configuration.CHUNK_SIZE; worldY < (ChunkIndex.Y+1)* Configuration.CHUNK_SIZE; worldY++)
            {
                WorldLocation worldLocation = new WorldLocation(worldX, worldY);
                if (IsRiverSource(worldLocation))
                {
                    EstablishRiverSource(worldLocation);
                }
            }
        }
    }

    public void EstablishRiverSource(WorldLocation worldLocation)
    {
        EstablishRiverCenter(worldLocation);
        List<RiverPackage> outlets = GetSourceOutlets(worldLocation);
        foreach (RiverPackage outlet in outlets)
        {
            AttemptToEstablishRiver(outlet);
        }
    }

    public void EstablishRiverCenter(WorldLocation worldLocation)
    {
        EstablishRiverFloodplain(worldLocation);
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        RiverNode riverCenter = RiverNodes[chunkLocation.X, chunkLocation.Y];
        //riverCenter.IncrementWaterLevel();
        List<RiverPackage> floodplains = GetFloodplains(worldLocation, riverCenter.WaterLevel);
        foreach (RiverPackage floodplain in floodplains)
        {
            AttemptToEstablishRiver(floodplain);
        }
    }

    public void EstablishRiverFloodplain(WorldLocation worldLocation)
    {
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        RiverNodes[chunkLocation.X, chunkLocation.Y].IsRiver = true;
    }

    public float MovementMultiplierAt(WorldLocation worldLocation)
    {
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        TerrainType terrain = Tiles[chunkLocation.X, chunkLocation.Y].TerrainType;
        return Configuration.MOVEMENT_MULTIPLIERS[terrain.Type];
    }

    public void AttemptToEstablishRiver(RiverPackage riverPackage)
    {
        if (WithinChunk(riverPackage.WorldLocation))
        {
            EstablishRiver(riverPackage);
        }
        else
        {
            Chunk chunk = MyWorld.Chunks.GetChunk(MyWorld.GetChunkIndex(riverPackage.WorldLocation));
            chunk.ImportRiver(riverPackage);
        }
    }

    public void EstablishRiver(RiverPackage river)
    {
        if (river.Type == RiverType.Source)
        {
            EstablishRiverSource(river.WorldLocation);
        }
        else if (river.Type == RiverType.Center)
        {
            EstablishRiverCenter(river.WorldLocation);
        }
        else if (river.Type == RiverType.Floodplain)
        {
            EstablishRiverFloodplain(river.WorldLocation);
        }
    }

    public List<RiverPackage> GetFloodplains(WorldLocation worldLocation, int depth)
    {
        int radius = depth - 1;
        int lowerRadius = (int) Math.Ceiling(radius / 2.0f);
        int upperRadius = (int) Math.Floor(radius / 2.0f);
        List<RiverPackage> floodplains = new List<RiverPackage>();
        for (int worldX = worldLocation.X - lowerRadius; worldX<worldLocation.X + upperRadius; worldX++)
        {
            for (int worldY = worldLocation.Y - lowerRadius; worldY<worldLocation.Y + upperRadius; worldY++)
            {
                floodplains.Add(new RiverPackage(new WorldLocation(worldX, worldY), RiverType.Floodplain));
            }
        }
        return floodplains;
    }

    private List<RiverPackage> GetSourceOutlets(WorldLocation sourceWorldLocation)
    {
        float sourceAltitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, sourceWorldLocation.Tuple);
        List<RiverPackage> outlets = new List<RiverPackage>();

        if (sourceAltitude < MyWorld.GenerationParameters.OceanAltitude)
        {
            return outlets;
        }

        int searchRadius = 1;
        bool outletFound = false;
        while (!outletFound && searchRadius < MyWorld.GenerationParameters.MaxRiverJumpDistance)
        {
            WorldLocation outletWorldLocation = LowestLocationInRadius(sourceWorldLocation, searchRadius);
            float outletAltitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, outletWorldLocation.Tuple);
            if (outletAltitude < sourceAltitude)
            {
                outlets.Add(new RiverPackage(outletWorldLocation, RiverType.Source));
                List<WorldLocation> betweenLocations = GetRiverTunnel(sourceWorldLocation, outletWorldLocation);
                foreach (WorldLocation betweenLocation in betweenLocations)
                {
                    outlets.Add(new RiverPackage(betweenLocation, RiverType.Center));
                }
                outletFound = true;
            }
            else
            {
                searchRadius++;
            }
        }

        return outlets;
    }

    public void FinalInitialization()
    {
        DecideTerrain();
        Initialized = true;
        
        foreach (PostInitAction postInitAction in PostInitActions)
        {
            postInitAction();
        }
    }

    public void DecideTerrain()
    {
        ChunkLocation chunkLocation;
        WorldLocation worldLocation;

        for (int chunkX = 0; chunkX < Configuration.CHUNK_SIZE; chunkX++)
        {
            for (int chunkY = 0; chunkY < Configuration.CHUNK_SIZE; chunkY++)
            {
                chunkLocation = new ChunkLocation(chunkX, chunkY);
                DecideGround(chunkLocation);
                DecideTrees(chunkLocation);
                DecideRivers(chunkLocation);

                worldLocation = ChunkToWorldLocation(chunkLocation);
                if (Tiles[chunkLocation.X, chunkLocation.Y].TerrainType.Type == TerrainTypeEnum.Grass)
                {
                    UnocupiedTiles.Add(worldLocation);
                }
            }
        }
    }

    private void DecideGround(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, worldLocation.Tuple);

        if (altitude < MyWorld.GenerationParameters.OceanAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Ocean, TerrainSubtypeEnum.Ocean);
        }
        else if (altitude < MyWorld.GenerationParameters.SandAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Sand, TerrainSubtypeEnum.Sand);
        }
        else if (altitude < MyWorld.GenerationParameters.MountainAltitude)
        {
            DecideGrass(chunkLocation);
        }
        else
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Mountain, TerrainSubtypeEnum.Mountain);
        }
    }

    private void DecideGrass(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float mediumGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassMediumSeed, MyWorld.GenerationParameters.GrassMediumPeriods, worldLocation.Tuple);
        if (mediumGrassSample < MyWorld.GenerationParameters.GrassMediumDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Medium);
        }
        else
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Short);
        }

        float tallGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassTallSeed, MyWorld.GenerationParameters.GrassTallPeriods, worldLocation.Tuple);
        if (tallGrassSample < MyWorld.GenerationParameters.GrassTallDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Tall);
        }
    }

    private void DecideTrees(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float treeSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.TreeRandomSeed, MyWorld.GenerationParameters.TreePeriods, worldLocation.Tuple);
        if (treeSample < MyWorld.GenerationParameters.TreeDensity && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType.Type == TerrainTypeEnum.Grass)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Tree, TerrainSubtypeEnum.Tree);
        }
    }

    private void DecideRivers(ChunkLocation chunkLocation)
    {
        if (RiverNodes[chunkLocation.X, chunkLocation.Y].IsRiver && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType.Type != TerrainTypeEnum.Ocean)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.River, TerrainSubtypeEnum.River);
        }
    }

    public void InitializeRivers()
    {
        RiversInitialized = true;
        InitializeInternalRivers();
        InitializeImportedRivers();
    }

    private void InitializeImportedRivers()
    {
        foreach (RiverPackage river in ImportedRivers)
        {
            EstablishRiver(river);
        }
    }

    public bool WithinChunk(WorldLocation worldLocation)
    {
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        return (chunkLocation.X >= 0 && chunkLocation.X < Configuration.CHUNK_SIZE && chunkLocation.Y >= 0 && chunkLocation.Y < Configuration.CHUNK_SIZE);
    }

    public ChunkLocation WorldToChunkLocation(WorldLocation worldLocation)
    {
        int x = worldLocation.X - (ChunkIndex.X * Configuration.CHUNK_SIZE);
        int y = worldLocation.Y - (ChunkIndex.Y * Configuration.CHUNK_SIZE);
        return new ChunkLocation(x, y);
    }

    public WorldLocation ChunkToWorldLocation(ChunkLocation chunkLocation)
    {
        int x = chunkLocation.X + ChunkIndex.X * Configuration.CHUNK_SIZE;
        int y = chunkLocation.Y + ChunkIndex.Y * Configuration.CHUNK_SIZE;
        return new WorldLocation(x, y);
    }

    public void ImportRiver(RiverPackage river)
    {
        if (RiversInitialized)
        {
            EstablishRiver(river);
        }
        else
        {
            ImportedRivers.Add(river);
        }
    }

    private List<WorldLocation> GetRiverTunnel(WorldLocation A, WorldLocation B)
    {
        float Weight((int X, int Y) C, (int X, int Y) D)
        {
            (float X, float Y) midpoint = ((C.X + D.X) / 2.0f, (C.Y + D.Y) / 2.0f);
            float weight = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, new float[] {7}, midpoint);
            weight *= Util.EuclidianDistance(C, D);
            weight *= 10.0f;
            return weight;
        }
        float Heuristic((int X, int Y) C, (int X, int Y) D)
        {
            return Util.EuclidianDistance(C, D)*0.01f;
        }
        List<(int X, int Y)> Neighbors((int X, int Y) C)
        {
            List<(int X, int Y)> neighbors = new List<(int X, int Y)>
            {
                (C.X + 1, C.Y),
                (C.X - 1, C.Y),
                (C.X, C.Y + 1),
                (C.X, C.Y - 1)
            };
            return neighbors;
        }
        bool Equal((int X, int Y) C, (int X, int Y) D)
        {
            return (C.X == D.X && C.Y == D.Y);
        }
        List<(int X, int Y)> tunnel = AStarPathfinder<(int X, int Y)>.FindPath(Neighbors, Weight, Heuristic, Equal, A.Tuple, B.Tuple);
        tunnel.Remove(A.Tuple);
        tunnel.Remove(B.Tuple);
        List<WorldLocation> returnTunnel = new List<WorldLocation>();
        foreach ((int X, int Y) tunnelEntry in tunnel)
        {
            returnTunnel.Add(new WorldLocation(tunnelEntry));
        }
        return returnTunnel;
    }

    private WorldLocation LowestLocationInRadius(WorldLocation worldLocation, int radius)
    {
        (WorldLocation WorldLocation, float Altitude) lowestLocation = (new WorldLocation(0, 0), 1);
        for (int x = worldLocation.X - radius; x <= worldLocation.X + radius; x++)
        {
            for (int y = worldLocation.Y - radius; y <= worldLocation.Y + radius; y++)
            {
                WorldLocation candidate = new WorldLocation(x, y);
                float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, candidate.Tuple);
                float distance = Util.EuclidianDistance(worldLocation.Tuple, candidate.Tuple);
                if (altitude < lowestLocation.Altitude && distance <= radius)
                {
                    lowestLocation = (candidate, altitude);
                }
            }
        }
        return lowestLocation.WorldLocation;
    }

    private bool IsRiverSource(WorldLocation worldLocation)
    {
        float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, worldLocation.Tuple);
        //return ((altitude * 1000) % 1) < MyWorld.GenerationParameters.RiverDensity && altitude > MyWorld.GenerationParameters.MountainAltitude;
        return ((altitude * 1000) % 1) < MyWorld.GenerationParameters.RiverDensity * Math.Pow(altitude, 5);
    }
}