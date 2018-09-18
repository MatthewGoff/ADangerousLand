using System.Collections.Generic;
using UnityEngine;
using MessagePack;

[MessagePackObject]
public class Chunk
{
    private delegate void PostInitAction();

    [IgnoreMember] private Queue<PostInitAction> PostInitActions;

    [Key(0)] public bool RiversInitialized { get; private set; } = false;
    [Key(1)] public bool Initialized { get; private set; } = false;
    [Key(2)] public bool LocalityInitialized { get; set; } = false;
    [Key(3)] public readonly List<EnemyManager> ResidentEnemies;
    [Key(4)] public readonly List<WorldLocation> UnocupiedTiles;
    [Key(5)] public readonly ChunkIndex ChunkIndex;
    [Key(6)] public readonly RiverNode[,] RiverNodes;
    [Key(7)] public readonly List<RiverPackage> ImportedRivers;
    [Key(8)] public readonly Tile[,] Tiles;
    [Key(9)] public readonly int DangerRating;
    [Key(10)] public readonly int MaxEnemies;

    public Chunk(ChunkIndex chunkIndex)
    {
        ChunkIndex = chunkIndex;
        DangerRating = DecideDangerRating();
        MaxEnemies = DangerRating;
        ImportedRivers = new List<RiverPackage>();
        RiverNodes = CreateRiverNodes();
        Tiles = CreateTiles();
        ResidentEnemies = new List<EnemyManager>();
        UnocupiedTiles = new List<WorldLocation>();
    }

    [SerializationConstructor]
    public Chunk(
        bool riversInitialized,
        bool initialized,
        bool localityInitialized,
        List<EnemyManager> residentEnemies,
        List<WorldLocation> unocupiedTiles,
        ChunkIndex chunkIndex,
        RiverNode[,] riverNodes,
        List<RiverPackage> importedRivers,
        Tile[,] tiles)
    {
        RiversInitialized = riversInitialized;
        Initialized = initialized;
        LocalityInitialized = localityInitialized;
        ResidentEnemies = residentEnemies;
        UnocupiedTiles = unocupiedTiles;
        ChunkIndex = chunkIndex;
        RiverNodes = riverNodes;
        ImportedRivers = importedRivers;
        Tiles = tiles;
    }

    private int DecideDangerRating()
    {
        return Mathf.Clamp(Mathf.FloorToInt((new Vector2(ChunkIndex.X, ChunkIndex.Y)).magnitude/3), 0, 9);
    }

    public void Update(Treadmill treadmill)
    {
        if (!Initialized)
        {
            return;
        }
        for (int chunkX = 0; chunkX < Tiles.GetLength(0); chunkX++)
        {
            for (int chunkY = 0; chunkY < Tiles.GetLength(1); chunkY++)
            {
                Tiles[chunkX, chunkY].Update(treadmill);
            }
        }
        List<EnemyManager> emigrantEnemies = new List<EnemyManager>();
        foreach (EnemyManager enemy in ResidentEnemies)
        {
            enemy.CheckTreadmill(treadmill);
            if (!WithinChunk((enemy.XPosition,enemy.YPosition)))
            {
                emigrantEnemies.Add(enemy);
                ChunkIndex newHome = GameManager.Singleton.World.GetChunkIndex((enemy.XPosition, enemy.YPosition));
                GameManager.Singleton.World.GetChunk(newHome).RecieveImmigrantEnemy(enemy);
                enemy.Immigrate(newHome);
            }
        }
        foreach (EnemyManager enemy in emigrantEnemies)
        {
            ResidentEnemies.Remove(enemy);
        }
    }

    public void Sleep()
    {
        for (int chunkX = 0; chunkX < Tiles.GetLength(0); chunkX++)
        {
            for (int chunkY = 0; chunkY < Tiles.GetLength(1); chunkY++)
            {
                Tiles[chunkX, chunkY].Sleep();
            }
        }
        foreach (EnemyManager enemy in ResidentEnemies)
        {
            enemy.Sleep();
        }
    }

    private void RecieveImmigrantEnemy(EnemyManager enemy)
    {
        ResidentEnemies.Add(enemy);
    }

    public void SpawnEnemies()
    {
        if (!Initialized)
        {
            if (PostInitActions == null)
            {
                PostInitActions = new Queue<PostInitAction>();
            }
            PostInitActions.Enqueue(SpawnEnemies);
        }
        else
        {
            while ((ResidentEnemies.Count < MaxEnemies) && (UnocupiedTiles.Count != 0))
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        WorldLocation spawnLocation = UnocupiedTiles[Util.RandomInt(0, UnocupiedTiles.Count-1)];
        UnocupiedTiles.Remove(spawnLocation);

        EnemyType enemyType = DecideEnemyType();
        ResidentEnemies.Add(new EnemyManager(spawnLocation, enemyType));
    }

    private EnemyType DecideEnemyType()
    {
        (float prob, EnemyType enemyType)[] enemyTypes = Configuration.SPAWN_PROBABILITIES[DangerRating];

        float random = UnityEngine.Random.Range(0f, 1f);
        float accumulator = 0f;
        for (int x = 0; x < enemyTypes.GetLength(0); x++)
        {
            accumulator += enemyTypes[x].prob;
            if (random <= accumulator)
            {
                return enemyTypes[x].enemyType;
            }
        }
        return enemyTypes[0].enemyType;
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
                riverNodes[chunkX, chunkY] = new RiverNode(0, false);
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
            Chunk chunk = GameManager.Singleton.World.Chunks.GetChunk(GameManager.Singleton.World.GetChunkIndex(riverPackage.WorldLocation));
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
        int lowerRadius = (int) Mathf.Ceil(radius / 2.0f);
        int upperRadius = (int) Mathf.Floor(radius / 2.0f);
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
        float sourceAltitude = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, GameManager.Singleton.World.GenerationParameters.TopographyPeriods, sourceWorldLocation.Tuple);
        List<RiverPackage> outlets = new List<RiverPackage>();

        if (sourceAltitude < GameManager.Singleton.World.GenerationParameters.OceanAltitude)
        {
            return outlets;
        }

        int searchRadius = 1;
        bool outletFound = false;
        while (!outletFound && searchRadius < GameManager.Singleton.World.GenerationParameters.MaxRiverJumpDistance)
        {
            WorldLocation outletWorldLocation = LowestLocationInRadius(sourceWorldLocation, searchRadius);
            float outletAltitude = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, GameManager.Singleton.World.GenerationParameters.TopographyPeriods, outletWorldLocation.Tuple);
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
        
        if (PostInitActions != null)
        {
            foreach (PostInitAction postInitAction in PostInitActions)
            {
                postInitAction();
            }
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
        float altitude = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, GameManager.Singleton.World.GenerationParameters.TopographyPeriods, worldLocation.Tuple);

        if (altitude < GameManager.Singleton.World.GenerationParameters.OceanAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Ocean, TerrainSubtypeEnum.Ocean);
        }
        else if (altitude < GameManager.Singleton.World.GenerationParameters.SandAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Sand, TerrainSubtypeEnum.Sand);
        }
        else if (altitude < GameManager.Singleton.World.GenerationParameters.MountainAltitude)
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
        float mediumGrassSample = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.GrassMediumSeed, GameManager.Singleton.World.GenerationParameters.GrassMediumPeriods, worldLocation.Tuple);
        if (mediumGrassSample < GameManager.Singleton.World.GenerationParameters.GrassMediumDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Medium);
        }
        else
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Short);
        }

        float tallGrassSample = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.GrassTallSeed, GameManager.Singleton.World.GenerationParameters.GrassTallPeriods, worldLocation.Tuple);
        if (tallGrassSample < GameManager.Singleton.World.GenerationParameters.GrassTallDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = new TerrainType(TerrainTypeEnum.Grass, TerrainSubtypeEnum.Grass_Tall);
        }
    }

    private void DecideTrees(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float treeSample = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TreeRandomSeed, GameManager.Singleton.World.GenerationParameters.TreePeriods, worldLocation.Tuple);
        if (treeSample < GameManager.Singleton.World.GenerationParameters.TreeDensity && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType.Type == TerrainTypeEnum.Grass)
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

    public bool WithinChunk((float X, float Y) position)
    {
        return ((position.X >= ChunkIndex.X * Configuration.CHUNK_SIZE)
            && (position.X < (ChunkIndex.X + 1) * Configuration.CHUNK_SIZE)
            && (position.Y >= ChunkIndex.Y * Configuration.CHUNK_SIZE)
            && (position.Y < (ChunkIndex.Y + 1) * Configuration.CHUNK_SIZE));
    }

    public bool WithinChunk(WorldLocation worldLocation)
    {
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        return (chunkLocation.X >= 0 && chunkLocation.X < Configuration.CHUNK_SIZE && chunkLocation.Y >= 0 && chunkLocation.Y < Configuration.CHUNK_SIZE);
    }

    public bool WithinChunk(Vector2 position)
    {
        return ((position.x >= ChunkIndex.X * Configuration.CHUNK_SIZE)
            && (position.x < (ChunkIndex.X + 1) * Configuration.CHUNK_SIZE)
            && (position.y >= ChunkIndex.Y * Configuration.CHUNK_SIZE)
            && (position.y < (ChunkIndex.Y + 1) * Configuration.CHUNK_SIZE));
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
            float weight = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, new float[] {7}, midpoint);
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
                float altitude = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, GameManager.Singleton.World.GenerationParameters.TopographyPeriods, candidate.Tuple);
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
        float altitude = Util.GetPerlinNoise(GameManager.Singleton.World.GenerationParameters.TopographySeed, GameManager.Singleton.World.GenerationParameters.TopographyPeriods, worldLocation.Tuple);
        //return ((altitude * 1000) % 1) < MyWorld.GenerationParameters.RiverDensity && altitude > MyWorld.GenerationParameters.MountainAltitude;
        return ((altitude * 1000) % 1) < GameManager.Singleton.World.GenerationParameters.RiverDensity * Mathf.Pow(altitude, 5);
    }

    public void EnemyHasDied(EnemyManager enemyManager)
    {
        ResidentEnemies.Remove(enemyManager);
    }
}