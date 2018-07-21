using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int ChunkSize;
    private Queue<WorldInitializer> WorldInitializers;

    public WorldGenParameters GenerationParameters { get; private set; }

    public PlayerController PlayerController { get; private set; }
    public ChunkStorage Chunks { get; private set; }

    private ChunkIndex CurrentChunk;
    private bool Initialized = false;

    public void CreateWorld()
    {
        GenerationParameters = new WorldGenParameters();


        WorldInitializers = new Queue<WorldInitializer>();
        Chunks = new ChunkStorage(this);

        SpawnPlayer();
        PlayerChangedChunks();
        Initialized = true;
    }

    public void FixedUpdate()
    {
        if (!Initialized)
        {
            return;
        }

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
        int updateRadius = (int)Math.Ceiling((Configuration.TREADMILL_RADIUS + Configuration.TREADMILL_UPDATE_MARGIN) / (float)ChunkSize);
        for (int indexX = CurrentChunk.X - updateRadius; indexX <= CurrentChunk.X + updateRadius; indexX++)
        {
            for (int indexY = CurrentChunk.Y - updateRadius; indexY <= CurrentChunk.Y + updateRadius; indexY++)
            {
                Chunks.GetChunk(new ChunkIndex(indexX, indexY)).Update(PlayerController.GetPlayerPosition());
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
        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                int distance = Math.Max(Math.Abs(x), Math.Abs(y));
                Chunk chunk = Chunks.GetChunk(CurrentChunk.Add(x, y));
                if (distance == 3)
                {
                    chunk.Awake = false;
                    chunk.SpawnEnemies();
                }
                else
                {
                    chunk.Awake = true;
                }
            }
        }
    }

    public void InitializeRiverLocality(ChunkIndex chunkIndex)
    {
        float radius = Util.ArrayMaximum(GenerationParameters.TopologyPeriods);
        int chunkRadius = (int)(radius / ChunkSize) + 3;

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
        int radius = 3;
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
        return new WorldLocation(Util.RoundVector2(PlayerController.GetPlayerPosition()));
    }

    public ChunkIndex GetChunkIndex(WorldLocation worldLocation)
    {
        int x = (int)Mathf.Floor(worldLocation.X / (float)ChunkSize);
        int y = (int)Mathf.Floor(worldLocation.Y / (float)ChunkSize);
        return new ChunkIndex(x, y);
    }

    private void SpawnPlayer()
    {
        WorldLocation spawnLocation = DecideSpawnPoint();
        GameObject player = Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity) as GameObject;
        PlayerController = player.GetComponent<PlayerController>();
        PlayerController.AssignMovementMultiplier(MovementMultiplier);
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
