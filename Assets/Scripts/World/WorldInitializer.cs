namespace ADL
{
    public class WorldInitializer : ThreadedJob
    {
        public World World;
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