using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameManager.Signal DoneLoading;

    public static readonly int TREADMILL_RADIUS = 59;
    public static readonly int TREADMILL_UPDATE_MARGIN = 5;

    public int ChunkSize;
    private Queue<WorldInitializer> WorldInitializers;

    public WorldGenParameters GenerationParameters { get; private set; }

    public GameObject Player { get; private set; }
    public ChunkStorage Chunks { get; private set; }

    private (int X, int Y) CurrentChunk;
    private bool Initialized = false;

    public void CreateWorld()
    {
        GenerationParameters = new WorldGenParameters();
        print(GenerationParameters.MasterSeed);

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
        (int X, int Y) currentChunk = GetPlayerChunk();
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
        int updateRadius = (int)Math.Ceiling((TREADMILL_RADIUS + TREADMILL_UPDATE_MARGIN) / (float)ChunkSize);
        for (int x = CurrentChunk.X - updateRadius; x <= CurrentChunk.X + updateRadius; x++)
        {
            for (int y = CurrentChunk.Y - updateRadius; y <= CurrentChunk.Y + updateRadius; y++)
            {
                Chunks.GetChunk((x, y)).Update(Player.transform.position);
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
    }

    public void InitializeRiverLocality((int X, int Y) chunkIndex)
    {
        float radius = Util.ArrayMaximum(GenerationParameters.TopologyPeriods);
        int chunkRadius = (int)(radius / ChunkSize);

        for (int x = chunkIndex.X - chunkRadius; x <= chunkIndex.X + chunkRadius; x++)
        {
            for (int y = chunkIndex.Y - chunkRadius; y <= chunkIndex.Y + chunkRadius; y++)
            {
                Chunk chunk = Chunks.GetChunk((x, y));
                if (!chunk.RiversInitialized)
                {
                    chunk.InitializeRivers();
                }
            }
        }
    }

    public void FinalLocalityInitialization((int X, int Y) chunkIndex)
    {
        int radius = 2;
        for (int x = chunkIndex.X - radius; x <= chunkIndex.X + radius; x++)
        {
            for (int y = chunkIndex.Y - radius; y <= chunkIndex.Y + radius; y++)
            {
                Chunk chunk = Chunks.GetChunk((x, y));
                if (!chunk.Initialized)
                {
                    chunk.FinalInitialization();
                }
            }
        }
        Chunks.GetChunk(chunkIndex).LocalityInitialized = true;
        DoneLoading();
    }

    private (int, int) GetPlayerChunk()
    {
        return GetChunkIndex(Util.RoundVector2(Player.transform.position));
    }

    public (int, int) GetChunkIndex((int X, int Y) location)
    {
        int x = (int)Mathf.Floor(location.X / (float)ChunkSize);
        int y = (int)Mathf.Floor(location.Y / (float)ChunkSize);
        return (x, y);
    }

    private void SpawnPlayer()
    {
        (int X, int Y) spawnLocation = DecideSpawnPoint();
        Player = Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity) as GameObject;
    }

    private (int X, int Y) DecideSpawnPoint()
    {
        int y = 0;
        int x = 0;
        float altitude = Util.GetPerlinNoise(GenerationParameters.TopologyRandomSeed, GenerationParameters.TopologyPeriods, (x, y));
        while (altitude > GenerationParameters.MountainAltitude || altitude < GenerationParameters.OceanAltitude)
        {
            x++;
            altitude = Util.GetPerlinNoise(GenerationParameters.TopologyRandomSeed, GenerationParameters.TopologyPeriods, (x, y));
        }
        return (x, y);
    }

    public void DestoryWorld()
    {
        Initialized = false;
        if (Player != null)
        {
            UnityEngine.Object.Destroy(Player);
        }
    }
}
