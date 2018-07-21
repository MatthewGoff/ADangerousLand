using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public bool RiversInitialized { get; private set; } = false;
    public bool Initialized { get; private set; } = false;
    public bool LocalityInitialized { get; set; } = false;
    public bool Awake { get; set; } = false;

    private readonly World MyWorld;
    private readonly (int X, int Y) ChunkIndex;

    private readonly RiverNode[,] RiverNodes;
    private readonly List<RiverPackage> ImportedRivers;
    private readonly Tile[,] Tiles;

    public Chunk(World myWorld, (int X, int Y) chunkIndex)
    {
        MyWorld = myWorld;
        ChunkIndex = chunkIndex;
        ImportedRivers = new List<RiverPackage>();
        RiverNodes = CreateRiverNodes();
        Tiles = CreateTiles();
    }

    public void Update(Vector2 playerPosition)
    {
        if (!Initialized)
        {
            return;
        }
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tile tile = Tiles[x, y];
                float distanceToPlayer = tile.DistanceToPlayer(playerPosition);
                if (distanceToPlayer <= Configuration.TREADMILL_RADIUS && !tile.Awake)
                {
                    tile.WakeUp();
                }
                else if (distanceToPlayer > Configuration.TREADMILL_RADIUS && tile.Awake)
                {
                    tile.Sleep();
                }
                if (distanceToPlayer < Configuration.FOG_OUTER_RADIUS + Configuration.FOG_UPDATE_MARGIN)
                {
                    tile.UpdateFog(distanceToPlayer);
                }
            }
        }
    }

    private Tile[,] CreateTiles()
    {
        Tile[,] tiles = new Tile[MyWorld.ChunkSize, MyWorld.ChunkSize];
        for (int x = 0; x < MyWorld.ChunkSize; x++)
        {
            for (int y = 0; y < MyWorld.ChunkSize; y++)
            {
                (int X, int Y) worldLocation = ChunkToWorldLocation((x, y));
                tiles[x, y] = new Tile(worldLocation);
            }
        }
        return tiles;
    }

    private RiverNode[,] CreateRiverNodes()
    {
        RiverNode[,] riverNodes = new RiverNode[MyWorld.ChunkSize, MyWorld.ChunkSize];
        for (int x = 0; x < MyWorld.ChunkSize; x++)
        {
            for (int y = 0; y < MyWorld.ChunkSize; y++)
            {
                riverNodes[x, y] = new RiverNode();
            }
        }
        return riverNodes;
    }

    public void InitializeInternalRivers()
    {
        for (int x = 0; x < MyWorld.ChunkSize; x++)
        {
            for (int y = 0; y < MyWorld.ChunkSize; y++)
            {
                (int X, int Y) worldLocation = ChunkToWorldLocation((x, y));
                if (IsRiverSource(worldLocation))
                {
                    EstablishRiverSource(worldLocation);
                }
            }
        }
    }

    public void EstablishRiverSource((int X, int Y) worldLocation)
    {
        EstablishRiverCenter(worldLocation);
        List<RiverPackage> outlets = GetSourceOutlets(worldLocation);
        foreach (RiverPackage outlet in outlets)
        {
            AttemptToEstablishRiver(outlet);
        }
    }

    public void EstablishRiverCenter((int X, int Y) worldLocation)
    {
        EstablishRiverFloodplain(worldLocation);
        (int X, int Y) chunkLocation = WorldToChunkLocation(worldLocation);
        RiverNode riverCenter = RiverNodes[chunkLocation.X, chunkLocation.Y];
        //riverCenter.IncrementWaterLevel();
        List<RiverPackage> floodplains = GetFloodplains(worldLocation, riverCenter.WaterLevel);
        foreach (RiverPackage floodplain in floodplains)
        {
            AttemptToEstablishRiver(floodplain);
        }
    }

    public void EstablishRiverFloodplain((int X, int Y) worldLocation)
    {
        (int X, int Y) chunkLocation = WorldToChunkLocation(worldLocation);
        RiverNodes[chunkLocation.X, chunkLocation.Y].IsRiver = true;
    }

    public float MovementMultiplierAt((int X, int Y) worldLocation)
    {
        (int X, int Y) chunkLocation = WorldToChunkLocation(worldLocation);
        TerrainType terrain = Tiles[chunkLocation.X, chunkLocation.Y].TerrainType;
        return Configuration.MOVEMENT_MULTIPLIERS[terrain];
    }

    public void AttemptToEstablishRiver(RiverPackage river)
    {
        if (WithinChunk(river.WorldLocation))
        {
            EstablishRiver(river);
        }
        else
        {
            Chunk chunk = MyWorld.Chunks.GetChunk(MyWorld.GetChunkIndex(river.WorldLocation));
            chunk.ImportRiver(river);
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

    public List<RiverPackage> GetFloodplains((int X, int Y) worldLocation, int depth)
    {
        int radius = depth - 1;
        int lowerRadius = (int) Math.Ceiling(radius / 2.0f);
        int upperRadius = (int) Math.Floor(radius / 2.0f);
        List<RiverPackage> floodplains = new List<RiverPackage>();
        for (int x = worldLocation.X - lowerRadius; x<worldLocation.X + upperRadius; x++)
        {
            for (int y = worldLocation.Y - lowerRadius; y<worldLocation.Y + upperRadius; y++)
            {
                floodplains.Add(new RiverPackage((x, y), RiverType.Floodplain));
            }
        }
        return floodplains;
    }

    private List<RiverPackage> GetSourceOutlets((int X, int Y) sourceWorldLocation)
    {
        float sourceAltitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, sourceWorldLocation);
        List<RiverPackage> outlets = new List<RiverPackage>();

        if (sourceAltitude < MyWorld.GenerationParameters.OceanAltitude)
        {
            return outlets;
        }

        int searchRadius = 1;
        bool outletFound = false;
        while (!outletFound && searchRadius < MyWorld.GenerationParameters.MaxRiverJumpDistance)
        {
            (int X, int Y) outletWorldLocation = LowestLocationInRadius(sourceWorldLocation, searchRadius);
            float outletAltitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, outletWorldLocation);
            if (outletAltitude < sourceAltitude)
            {
                outlets.Add(new RiverPackage(outletWorldLocation, RiverType.Source));
                List<(int X, int Y)> betweenLocations = GetRiverTunnel(sourceWorldLocation, outletWorldLocation);
                foreach ((int X, int Y) betweenLocation in betweenLocations)
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
                (int X, int Y) location = (chunkX, chunkY);
                DecideGround(location);
                DecideTrees(location);
                DecideRivers(location);
            }
        }
    }

    private void DecideGround((int X, int Y) location)
    {
        (int X, int Y) worldLocation = ChunkToWorldLocation(location);
        float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, worldLocation);

        if (altitude < MyWorld.GenerationParameters.OceanAltitude)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.Ocean;
        }
        else if (altitude < MyWorld.GenerationParameters.SandAltitude)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.Sand;
        }
        else if (altitude < MyWorld.GenerationParameters.MountainAltitude)
        {
            DecideGrass(location);
        }
        else
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.Mountain;
        }
    }

    private void DecideGrass((int X, int Y) location)
    {
        (int X, int Y) worldLocation = ChunkToWorldLocation(location);
        float mediumGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassMediumSeed, MyWorld.GenerationParameters.GrassMediumPeriods, worldLocation);
        if (mediumGrassSample < MyWorld.GenerationParameters.GrassMediumDensity)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.GrassMedium;
        }
        else
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.GrassShort;
        }

        float tallGrassSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.GrassTallSeed, MyWorld.GenerationParameters.GrassTallPeriods, worldLocation);
        if (tallGrassSample < MyWorld.GenerationParameters.GrassTallDensity)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.GrassTall;
        }
    }

    private void DecideTrees((int X, int Y) location)
    {
        (int X, int Y) worldLocation = ChunkToWorldLocation(location);
        float treeSample = Util.GetPerlinNoise(MyWorld.GenerationParameters.TreeRandomSeed, MyWorld.GenerationParameters.TreePeriods, worldLocation);
        if (treeSample < MyWorld.GenerationParameters.TreeDensity && Tiles[location.X, location.Y].TerrainType == TerrainType.GrassMedium)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.Tree;
        }
    }

    private void DecideRivers((int X, int Y) location)
    {
        if (RiverNodes[location.X, location.Y].IsRiver && Tiles[location.X, location.Y].TerrainType != TerrainType.Ocean)
        {
            Tiles[location.X, location.Y].TerrainType = TerrainType.River;
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

    public bool WithinChunk((int X, int Y) worldLocation)
    {
        (int X, int Y) chunkLocation = WorldToChunkLocation(worldLocation);
        return (chunkLocation.X >= 0 && chunkLocation.X < MyWorld.ChunkSize && chunkLocation.Y >= 0 && chunkLocation.Y < MyWorld.ChunkSize);
    }

    public bool WithinInitializationRadius((int X, int Y) worldLocation)
    {
        (int X, int Y) chunkLocation = WorldToChunkLocation(worldLocation);
        return (chunkLocation.X >= -MyWorld.ChunkSize
            && chunkLocation.X < MyWorld.ChunkSize * 2
            && chunkLocation.Y >= -MyWorld.ChunkSize
            && chunkLocation.Y < MyWorld.ChunkSize * 2);
    }

    public (int X, int Y) WorldToChunkLocation((int X, int Y) worldLocation)
    {
        int x = worldLocation.X - (ChunkIndex.X * MyWorld.ChunkSize);
        int y = worldLocation.Y - (ChunkIndex.Y * MyWorld.ChunkSize);
        return (x, y);
    }

    public (int X, int Y) ChunkToWorldLocation((int X, int Y) chunkLocation)
    {
        int x = chunkLocation.X + ChunkIndex.X * MyWorld.ChunkSize;
        int y = chunkLocation.Y + ChunkIndex.Y * MyWorld.ChunkSize;
        return (x, y);
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

    private List<(int X, int Y)> GetRiverTunnel((int X, int Y) A, (int X, int Y) B)
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
        List<(int X, int Y)> tunnel = AStarPathfinder<(int X, int Y)>.FindPath(Neighbors, Weight, Heuristic, Equal, A, B);
        tunnel.Remove(A);
        tunnel.Remove(B);
        return tunnel;
    }

    private (int X, int Y) LowestLocationInRadius((int X, int Y) worldLocation, int radius)
    {
        ((int X, int Y) Location, float Altitude) lowestLocation = ((0, 0), 1);
        for (int x = worldLocation.X - radius; x <= worldLocation.X + radius; x++)
        {
            for (int y = worldLocation.Y - radius; y <= worldLocation.Y + radius; y++)
            {
                (int X, int Y) candidate = (x, y);
                float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, candidate);
                float distance = Util.EuclidianDistance(worldLocation, candidate);
                if (altitude < lowestLocation.Altitude && distance <= radius)
                {
                    lowestLocation = (candidate, altitude);
                }
            }
        }
        return lowestLocation.Location;
    }

    private bool IsRiverSource((int X, int Y) worldLocation)
    {
        float altitude = Util.GetPerlinNoise(MyWorld.GenerationParameters.TopologyRandomSeed, MyWorld.GenerationParameters.TopologyPeriods, worldLocation);
        //return ((altitude * 1000) % 1) < MyWorld.GenerationParameters.RiverDensity && altitude > MyWorld.GenerationParameters.MountainAltitude;
        return ((altitude * 1000) % 1) < MyWorld.GenerationParameters.RiverDensity * Math.Pow(altitude, 5);
    }
}