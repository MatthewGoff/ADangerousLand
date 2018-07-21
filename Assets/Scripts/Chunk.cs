using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public bool RiversInitialized { get; private set; } = false;
    public bool Initialized { get; private set; } = false;
    public bool LocalityInitialized { get; set; } = false;
    public bool Awake { get; set; } = false;

    private int ResidentEnemies;
    private List<WorldLocation> UnocupiedTiles;

    private readonly WorldController MyWorld;
    private readonly ChunkIndex ChunkIndex;
    private readonly RiverNode[,] RiverNodes;
    private readonly List<RiverPackage> ImportedRivers;
    private readonly Tile[,] Tiles;

    public Chunk(WorldController myWorld, ChunkIndex chunkIndex)
    {
        MyWorld = myWorld;
        ChunkIndex = chunkIndex;
        ImportedRivers = new List<RiverPackage>();
        RiverNodes = CreateRiverNodes();
        Tiles = CreateTiles();
        ResidentEnemies = 0;
        UnocupiedTiles = new List<WorldLocation>();
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
                //if (euclidianDistance < Configuration.FOG_OUTER_RADIUS + Configuration.FOG_UPDATE_MARGIN)
                {
                    tile.UpdateFog(euclidianDistance);
                }
            }
        }
    }

    public void SpawnEnemies()
    {
        /*
         * while (ResidentEnemies < Configuration.EnemiesPerChunk)
        {
            SpawnEnemy();
        }
        */
    }

    private void SpawnEnemy()
    {

    }

    private Tile[,] CreateTiles()
    {
        Tile[,] tiles = new Tile[MyWorld.ChunkSize, MyWorld.ChunkSize];
        for (int chunkX = 0; chunkX < MyWorld.ChunkSize; chunkX++)
        {
            for (int chunkY = 0; chunkY < MyWorld.ChunkSize; chunkY++)
            {
                WorldLocation worldLocation = ChunkToWorldLocation(new ChunkLocation(chunkX, chunkY));
                tiles[chunkX, chunkY] = new Tile(worldLocation);
            }
        }
        return tiles;
    }

    private RiverNode[,] CreateRiverNodes()
    {
        RiverNode[,] riverNodes = new RiverNode[MyWorld.ChunkSize, MyWorld.ChunkSize];
        for (int chunkX = 0; chunkX < MyWorld.ChunkSize; chunkX++)
        {
            for (int chunkY = 0; chunkY < MyWorld.ChunkSize; chunkY++)
            {
                riverNodes[chunkX, chunkY] = new RiverNode();
            }
        }
        return riverNodes;
    }

    public void InitializeInternalRivers()
    {
        for (int worldX = ChunkIndex.X*MyWorld.ChunkSize; worldX < (ChunkIndex.X+1)*MyWorld.ChunkSize; worldX++)
        {
            for (int worldY = ChunkIndex.Y*MyWorld.ChunkSize; worldY < (ChunkIndex.Y+1)*MyWorld.ChunkSize; worldY++)
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
        return Configuration.MOVEMENT_MULTIPLIERS[terrain];
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
    }

    public void DecideTerrain()
    {
        for (int chunkX = 0; chunkX < MyWorld.ChunkSize; chunkX++)
        {
            for (int chunkY = 0; chunkY < MyWorld.ChunkSize; chunkY++)
            {
                ChunkLocation chunkLocation = new ChunkLocation(chunkX, chunkY);
                DecideGround(chunkLocation);
                DecideTrees(chunkLocation);
                DecideRivers(chunkLocation);
            }
        }
    }

    private void DecideGround(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, worldLocation.Tuple);

        if (altitude < MyWorld.GenerationParameters.OceanAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.Ocean;
        }
        else if (altitude < MyWorld.GenerationParameters.SandAltitude)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.Sand;
        }
        else if (altitude < MyWorld.GenerationParameters.MountainAltitude)
        {
            DecideGrass(chunkLocation);
        }
        else
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.Mountain;
        }
    }

    private void DecideGrass(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float mediumGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassMediumSeed, MyWorld.GenerationParameters.GrassMediumPeriods, worldLocation.Tuple);
        if (mediumGrassSample < MyWorld.GenerationParameters.GrassMediumDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.GrassMedium;
        }
        else
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.GrassShort;
        }

        float tallGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassTallSeed, MyWorld.GenerationParameters.GrassTallPeriods, worldLocation.Tuple);
        if (tallGrassSample < MyWorld.GenerationParameters.GrassTallDensity)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.GrassTall;
        }
    }

    private void DecideTrees(ChunkLocation chunkLocation)
    {
        WorldLocation worldLocation = ChunkToWorldLocation(chunkLocation);
        float treeSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.TreeRandomSeed, MyWorld.GenerationParameters.TreePeriods, worldLocation.Tuple);
        if (treeSample < MyWorld.GenerationParameters.TreeDensity
            && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType == TerrainType.GrassMedium
            && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType == TerrainType.GrassTall
            && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType == TerrainType.GrassShort)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.Tree;
        }
    }

    private void DecideRivers(ChunkLocation chunkLocation)
    {
        if (RiverNodes[chunkLocation.X, chunkLocation.Y].IsRiver && Tiles[chunkLocation.X, chunkLocation.Y].TerrainType != TerrainType.Ocean)
        {
            Tiles[chunkLocation.X, chunkLocation.Y].TerrainType = TerrainType.River;
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
        return (chunkLocation.X >= 0 && chunkLocation.X < MyWorld.ChunkSize && chunkLocation.Y >= 0 && chunkLocation.Y < MyWorld.ChunkSize);
    }

    public bool WithinInitializationRadius(WorldLocation worldLocation)
    {
        ChunkLocation chunkLocation = WorldToChunkLocation(worldLocation);
        return (chunkLocation.X >= -MyWorld.ChunkSize
            && chunkLocation.X < MyWorld.ChunkSize * 2
            && chunkLocation.Y >= -MyWorld.ChunkSize
            && chunkLocation.Y < MyWorld.ChunkSize * 2);
    }

    public ChunkLocation WorldToChunkLocation(WorldLocation worldLocation)
    {
        int x = worldLocation.X - (ChunkIndex.X * MyWorld.ChunkSize);
        int y = worldLocation.Y - (ChunkIndex.Y * MyWorld.ChunkSize);
        return new ChunkLocation(x, y);
    }

    public WorldLocation ChunkToWorldLocation(ChunkLocation chunkLocation)
    {
        int x = chunkLocation.X + ChunkIndex.X * MyWorld.ChunkSize;
        int y = chunkLocation.Y + ChunkIndex.Y * MyWorld.ChunkSize;
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