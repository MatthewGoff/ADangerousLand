using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace ADL
{
    [MessagePackObject]
    public class World
    {
        [IgnoreMember] public PlayerManager PlayerManager;
        [IgnoreMember] private bool Active;
        [IgnoreMember] private Queue<WorldInitializer> WorldInitializers;
        [IgnoreMember] private List<Chunk> LiveChunks;

        [Key(0)] public int WorldIdentifier;
        [Key(1)] public WorldGenParameters GenerationParameters { get; private set; }
        [Key(2)] public readonly ChunkStorage Chunks;

        public World(int worldIdentifier, int seed)
        {
            WorldIdentifier = worldIdentifier;
            GenerationParameters = new WorldGenParameters(seed);
            Chunks = new ChunkStorage();
        }

        [SerializationConstructor]
        public World(int worldIdentifier, WorldGenParameters generationParameters, ChunkStorage chunks)
        {
            WorldIdentifier = worldIdentifier;
            GenerationParameters = generationParameters;
            Chunks = chunks;
        }

        public void Setup(PlayerManager playerManager)
        {
            PlayerManager = playerManager;
            Active = false;

            WorldInitializers = new Queue<WorldInitializer>();
            LiveChunks = new List<Chunk>();
        }

        public void Update()
        {
            if (!Active)
            {
                return;
            }

            foreach (Chunk chunk in LiveChunks)
            {
                chunk.Update();
            }
        }

        public void FixedUpdate()
        {
            if (!Active)
            {
                return;
            }

            UpdateWorldInitializers();

            ChunkIndex currentChunk = GetChunkIndex(PlayerManager.GetPosition());

            QueueNextStateForChunks(currentChunk);

            LiveChunks.Clear();
            bool allLiveChunksLoaded = true;
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    Chunk chunk = Chunks.GetChunk(currentChunk.Add(x, y));

                    TransitionChunkState(chunk);

                    if (chunk.State == ChunkState.Occupied || chunk.State == ChunkState.Live)
                    {
                        LiveChunks.Add(chunk);
                        allLiveChunksLoaded &= chunk.FixedUpdate();
                    }
                }
            }
            if (allLiveChunksLoaded)
            {
                GameManager.Singleton.TakeInput(GameInputType.WorldLoaded);
            }
        }

        /*
         * Inform chunks arround the current chunk what their state in the next fixed update is to be.
         * (State is purely a function of proximity to the current occuppied chunk(s))
         */
        private void QueueNextStateForChunks(ChunkIndex currentChunk)
        {
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    Chunk chunk = Chunks.GetChunk(currentChunk.Add(x, y));
                    int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    if (distance == 0)
                    {
                        chunk.NextState = ChunkState.Occupied;
                    }
                    else if (distance == 1)
                    {
                        chunk.NextState = ChunkState.Live;
                    }
                    else if (distance == 2)
                    {
                        chunk.NextState = ChunkState.SpawningGrounds;
                    }
                    else if (distance == 3)
                    {
                        chunk.NextState = ChunkState.Inactive;
                    }
                }
            }
        }

        /*
         * Transition a chunk from its state in the last fixed update to its state
         * in this fixed update. If there has been a change of state execute the
         * appropriate logic.
         */
        private void TransitionChunkState(Chunk chunk)
        {
            if (chunk.NextState != chunk.State)
            {
                if (chunk.NextState == ChunkState.Occupied)
                {
                    if (!chunk.InitializingLocality && !chunk.LocalityInitialized)
                    {
                        chunk.InitializingLocality = true;
                        WorldInitializer newInitializer = new WorldInitializer
                        {
                            World = this,
                            ChunkIndex = chunk.ChunkIndex,
                        };
                        WorldInitializers.Enqueue(newInitializer);
                    }
                }
                if (chunk.NextState == ChunkState.Inactive)
                {
                    chunk.Sleep();
                }
                if (chunk.NextState == ChunkState.SpawningGrounds)
                {
                    chunk.SpawnEnemies();
                }

                chunk.State = chunk.NextState;
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

        public void Sleep()
        {
            Active = false;
            ChunkIndex currentchunk = GetChunkIndex(PlayerManager.GetPosition());
            for (int indexX = currentchunk.X - 5; indexX <= currentchunk.X + 5; indexX++)
            {
                for (int indexY = currentchunk.Y - 5; indexY <= currentchunk.Y + 5; indexY++)
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
            Chunks.GetChunk(chunkIndex).InitializingLocality = false;
        }

        private ChunkIndex GetPlayerChunk()
        {
            return GetChunkIndex(GetPlayerLocation());
        }

        public WorldLocation GetPlayerLocation()
        {
            return new WorldLocation(Util.RoundVector2(PlayerManager.GetPosition()));
        }

        public float GetVisibilityLevel(Vector2 position)
        {
            if (!Active)
            {
                return 0f;
            }
            PlayerManager playerManager = NearestPlayer(position);
            float distance = (playerManager.GetPosition() - position).magnitude;
            float alpha = (distance - playerManager.GetSightRadiusNear()) / (playerManager.GetSightRadiusFar() - playerManager.GetSightRadiusNear());
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

        public void SpawnPlayer()
        {
            WorldLocation spawnLocation = DecideSpawnPoint();
            PlayerManager.Spawn(spawnLocation);
            Active = true;
        }

        public float MovementMultiplier(WorldLocation worldLocation)
        {
            return Chunks.GetChunk(GetChunkIndex(worldLocation)).MovementMultiplierAt(worldLocation);
        }

        private WorldLocation DecideSpawnPoint()
        {
            int y = 0;
            int x = 0;
            float altitude = Util.GetPerlinNoise(GenerationParameters.TopographySeed, GenerationParameters.TopographyPeriods, (x, y));
            while (altitude > GenerationParameters.MountainAltitude || altitude < GenerationParameters.OceanAltitude)
            {
                x++;
                altitude = Util.GetPerlinNoise(GenerationParameters.TopographySeed, GenerationParameters.TopographyPeriods, (x, y));
            }
            return new WorldLocation(x, y);
        }

        public PlayerManager NearestPlayer(WorldLocation worldLocation)
        {
            return PlayerManager;
        }

        public PlayerManager NearestPlayer(Vector2 position)
        {
            return PlayerManager;
        }

        public bool OnTreadmill(WorldLocation worldLocation)
        {
            Vector2 center = NearestPlayer(worldLocation).GetPosition();
            int radius = Configuration.TREADMILL_RADIUS;
            bool withinX = (worldLocation.X > center.x - radius) && (worldLocation.X < center.x + radius);
            bool withinY = (worldLocation.Y > center.y - radius) && (worldLocation.Y < center.y + radius);
            return withinX && withinY;
        }

        public bool OnTreadmill(Vector2 position)
        {
            Vector2 center = NearestPlayer(position).GetPosition();
            int radius = Configuration.TREADMILL_RADIUS;
            bool withinX = (position.x > center.x - radius) && (position.x < center.x + radius);
            bool withinY = (position.y > center.y - radius) && (position.y < center.y + radius);
            return withinX && withinY;
        }
    }
}
