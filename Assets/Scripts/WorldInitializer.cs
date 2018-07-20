public class WorldInitializer : ThreadedJob
{
    public World MyWorld;
    public (int X, int Y) ChunkIndex;

    protected override void ThreadFunction()
    {
        MyWorld.InitializeRiverLocality(ChunkIndex);
    }

    protected override void OnFinished()
    {
        MyWorld.FinalLocalityInitialization(ChunkIndex);
    }
}