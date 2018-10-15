using ADL.Util;

namespace ADL.World
{
    public class WorldInitializer : ThreadedJob
    {
        public WorldManager World;
        public ChunkIndex ChunkIndex;

        protected override void ThreadFunction()
        {
            World.InitializeRiverLocality(ChunkIndex);
        }

        protected override void OnFinished()
        {
            World.FinalLocalityInitialization(ChunkIndex);
        }
    }
}