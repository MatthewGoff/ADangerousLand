using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
public class World
{
    public PlayerManager PlayerManager;
    private bool Active;
    private Treadmill Treadmill;
    private Queue<WorldInitializer> WorldInitializers;
    private ChunkIndex CurrentChunk;

    [ProtoMember(1)] public int WorldIdentifier;
    [ProtoMember(2)] public WorldGenParameters GenerationParameters { get; private set; }
    [ProtoMember(3)] public readonly ChunkStorage Chunks;

    public World(int worldIdentifier, int seed)
    {
        WorldIdentifier = worldIdentifier;
        GenerationParameters = new WorldGenParameters(seed);
        Chunks = new ChunkStorage();
    }

    public void Setup(PlayerManager playerManager)
    {
        PlayerManager = playerManager;
        Active = false;

        WorldInitializers = new Queue<WorldInitializer>();
        Treadmill = new Treadmill(Configuration.TREADMILL_WIDTH, Configuration.TREADMIL_HEIGHT);
    }

    public void Start()
    {
        SpawnPlayer();
        PlayerChangedChunks();
        Active = true;
    }

    public void Update()
    {
        if (!Active)
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
                int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
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
        Active = false;
        int updateRadius = 2;
        for (int indexX = CurrentChunk.X - updateRadius; indexX <= CurrentChunk.X + updateRadius; indexX++)
        {
            for (int indexY = CurrentChunk.Y - updateRadius; indexY <= CurrentChunk.Y + updateRadius; indexY++)
            {
                Chunks.GetChunk(new ChunkIndex(indexX, indexY)).Sleep();
            }
        }
        PlayerManager.Sleep();
    }

    public void InitializeRiverLocality(ChunkIndex chunkIndex)
    {
        float radius = Util.ArrayMaximum(GenerationParameters.TopographyPeriods);
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
        if (!Active)
        {
            return 0f;
        }
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

    public ChunkIndex GetChunkIndex((float X, float Y) position)
    {
        int x = (int)Mathf.Floor(position.X / Configuration.CHUNK_SIZE);
        int y = (int)Mathf.Floor(position.Y / Configuration.CHUNK_SIZE);
        return new ChunkIndex(x, y);
    }

    public Chunk GetChunk(ChunkIndex chunkIndex)
    {
        return Chunks.GetChunk(chunkIndex);
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
        float altitude = Util.GetPerlinNoise(GenerationParameters.Topography, GenerationParameters.TopographyPeriods, (x, y));
        while (altitude > GenerationParameters.MountainAltitude || altitude < GenerationParameters.OceanAltitude)
        {
            x++;
            altitude = Util.GetPerlinNoise(GenerationParameters.Topography, GenerationParameters.TopographyPeriods, (x, y));
        }
        return new WorldLocation(x, y);
    }

    public int GetRandomSeed()
    {
        return GenerationParameters.MasterSeed;
    }
}
