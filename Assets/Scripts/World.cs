using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public WorldGenParameters GenerationParameters { get; private set; }

    public PlayerManager PlayerManager { get; private set; }
    public readonly ChunkStorage Chunks;

    private bool Initialized = false;
    private readonly Treadmill Treadmill;
    private readonly Queue<WorldInitializer> WorldInitializers;

    private ChunkIndex CurrentChunk;
    private readonly List<Chunk> AwakeChunks;

    public World()
    {
        GenerationParameters = new WorldGenParameters();
        WorldInitializers = new Queue<WorldInitializer>();
        Treadmill = new Treadmill(Configuration.TREADMILL_WIDTH, Configuration.TREADMIL_HEIGHT);
        Chunks = new ChunkStorage(this);
        PlayerManager = new PlayerManager(this);
    }

    public void Start()
    {
        SpawnPlayer();
        PlayerChangedChunks();
        Initialized = true;
    }

    public void Update()
    {
        if (!Initialized)
        {
            return;
        }
        Treadmill.Center = PlayerManager.GetPlayerPosition();
        UpdateCurrentChunk();
        UpdateWorldInitializers();
        UpdateChunks();
    }

    private void UpdateCurrentChunk()
    {
        ChunkIndex currentChunk = GetPlayerChunk();
        if (!currentChunk.Equals(CurrentChunk))
        {
            CurrentChunk = currentChunk;
            PlayerChangedChunks();
        }
    }

    private void UpdateWorldInitializers()
    {
        if (WorldInitializers.Count != 0)
        {
            WorldInitializer currentInitializer = WorldInitializers.Peek();
            currentInitializer.Update();
            if (currentInitializer.IsDone)
            {
                WorldInitializers.Dequeue();
            }
            else if (!currentInitializer.IsRunning)
            {
                currentInitializer.Start();
            }
        }
    }

    private void UpdateChunks()
    {
        int updateRadius = 2;
        for (int indexX = CurrentChunk.X - updateRadius; indexX <= CurrentChunk.X + updateRadius; indexX++)
        {
            for (int indexY = CurrentChunk.Y - updateRadius; indexY <= CurrentChunk.Y + updateRadius; indexY++)
            {
                Chunks.GetChunk(new ChunkIndex(indexX, indexY)).Update(Treadmill);
            }
        }
    }

    public void PlayerChangedChunks()
    {
        if (!Chunks.GetChunk(CurrentChunk).LocalityInitialized)
        {
            WorldInitializer newInitializer = new WorldInitializer
            {
                MyWorld = this,
                ChunkIndex = CurrentChunk
            };
            WorldInitializers.Enqueue(newInitializer);
        }
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                int distance = Math.Max(Math.Abs(x), Math.Abs(y));
                Chunk chunk = Chunks.GetChunk(CurrentChunk.Add(x, y));
                if (distance == 2)
                {
                    chunk.Sleep();
                    chunk.SpawnEnemies();
                }
            }
        }
    }

    public void Sleep()
    {
        Initialized = false;
        PlayerManager.Sleep();
        int updateRadius = 2;
        for (int indexX = CurrentChunk.X - updateRadius; indexX <= CurrentChunk.X + updateRadius; indexX++)
        {
            for (int indexY = CurrentChunk.Y - updateRadius; indexY <= CurrentChunk.Y + updateRadius; indexY++)
            {
                Chunks.GetChunk(new ChunkIndex(indexX, indexY)).Sleep();
            }
        }
    }

    public void InitializeRiverLocality(ChunkIndex chunkIndex)
    {
        float radius = Util.ArrayMaximum(GenerationParameters.TopologyPeriods);
        int chunkRadius = (int)(radius / Configuration.CHUNK_SIZE) + 2;

        for (int indexX = chunkIndex.X - chunkRadius; indexX <= chunkIndex.X + chunkRadius; indexX++)
        {
            for (int indexY = chunkIndex.Y - chunkRadius; indexY <= chunkIndex.Y + chunkRadius; indexY++)
            {
                Chunk chunk = Chunks.GetChunk(new ChunkIndex(indexX, indexY));
                if (!chunk.RiversInitialized)
                {
                    chunk.InitializeRivers();
                }
            }
        }
    }

    public void FinalLocalityInitialization(ChunkIndex chunkIndex)
    {
        int radius = 2;
        for (int indexX = chunkIndex.X - radius; indexX <= chunkIndex.X + radius; indexX++)
        {
            for (int indexY = chunkIndex.Y - radius; indexY <= chunkIndex.Y + radius; indexY++)
            {
                Chunk chunk = Chunks.GetChunk(new ChunkIndex(indexX, indexY));
                if (!chunk.Initialized)
                {
                    chunk.FinalInitialization();
                }
            }
        }
        Chunks.GetChunk(chunkIndex).LocalityInitialized = true;
        GameManager.Singleton.Input(GameInputType.FinishedLoading);
    }

    private ChunkIndex GetPlayerChunk()
    {
        return GetChunkIndex(GetPlayerLocation());
    }

    public WorldLocation GetPlayerLocation()
    {
        return new WorldLocation(Util.RoundVector2(PlayerManager.GetPlayerPosition()));
    }

    public float GetVisibilityLevel(Vector2 position)
    {
        float distance = (PlayerManager.GetPlayerPosition() - position).magnitude;
        float alpha = (distance - PlayerManager.GetSightRadiusNear()) / (PlayerManager.GetSightRadiusFar() - PlayerManager.GetSightRadiusNear());
        return 1 - Mathf.Clamp(alpha, 0f, 1f);
    }

    public ChunkIndex GetChunkIndex(WorldLocation worldLocation)
    {
        int x = (int)Mathf.Floor(worldLocation.X / (float)Configuration.CHUNK_SIZE);
        int y = (int)Mathf.Floor(worldLocation.Y / (float)Configuration.CHUNK_SIZE);
        return new ChunkIndex(x, y);
    }

    public ChunkIndex GetChunkIndex(Vector2 position)
    {
        int x = (int)Mathf.Floor(position.x / Configuration.CHUNK_SIZE);
        int y = (int)Mathf.Floor(position.y / Configuration.CHUNK_SIZE);
        return new ChunkIndex(x, y);
    }

    private void SpawnPlayer()
    {
        WorldLocation spawnLocation = DecideSpawnPoint();
        PlayerManager.Spawn(spawnLocation);
    }
    
    public float MovementMultiplier(WorldLocation worldLocation)
    {
        return Chunks.GetChunk(GetChunkIndex(worldLocation)).MovementMultiplierAt(worldLocation);
    }

    private WorldLocation DecideSpawnPoint()
    {
        int y = 0;
        int x = 0;
        float altitude = Util.GetPerlinNoise(GenerationParameters.TopologyRandomSeed, GenerationParameters.TopologyPeriods, (x, y));
        while (altitude > GenerationParameters.MountainAltitude || altitude < GenerationParameters.OceanAltitude)
        {
            x++;
            altitude = Util.GetPerlinNoise(GenerationParameters.TopologyRandomSeed, GenerationParameters.TopologyPeriods, (x, y));
        }
        return new WorldLocation(x, y);
    }

    public int GetRandomSeed()
    {
        return GenerationParameters.MasterSeed;
    }
}
