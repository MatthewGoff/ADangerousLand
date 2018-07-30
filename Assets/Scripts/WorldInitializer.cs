public class WorldInitializer : ThreadedJob
{
    public World MyWorld;
    public ChunkIndex ChunkIndex;

    protected override void ThreadFunction()
    {
        MyWorld.InitializeRiverLocality(ChunkIndex);
    }

    protected override void OnFinished()
    {
        MyWorld.FinalLocalityInitialization(ChunkIndex);
    }
}