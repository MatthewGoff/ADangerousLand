using MessagePack;

[MessagePackObject]
public class WorldGenParameters
{
    [Key(0)] public readonly int MasterSeed;

    [Key(1)] public readonly float GrassMediumSeed;
    [Key(2)] public readonly float[] GrassMediumPeriods;
    [Key(3)] public readonly float GrassMediumDensity;

    [Key(4)] public readonly float GrassTallSeed;
    [Key(5)] public readonly float[] GrassTallPeriods;
    [Key(6)] public readonly float GrassTallDensity;

    [Key(7)] public readonly float TreeRandomSeed;
    [Key(8)] public readonly float[] TreePeriods;
    [Key(9)] public readonly float TreeDensity;

    [Key(10)] public readonly float TopographySeed;
    [Key(11)] public readonly float[] TopographyPeriods;
    [Key(12)] public readonly float MountainAltitude;
    [Key(13)] public readonly float SandAltitude;
    [Key(14)] public readonly float OceanAltitude;

    [Key(15)] public readonly float RiverDensity;
    [Key(16)] public readonly float MaxRiverJumpDistance;
    [Key(17)] public readonly float[] RiverJumpPeriods;

    public WorldGenParameters(int seed)
    {
        GrassMediumDensity = 0.7f;
        GrassTallDensity = 0.4f;
        TreeDensity = 0.37f;
        RiverDensity = 0.003f;

        MountainAltitude = 0.65f;
        SandAltitude = 0.37f;
        OceanAltitude = 0.35f;

        GrassMediumPeriods = new float[] { 1.0f, 5.0f };
        GrassTallPeriods = new float[] { 5.0f, 20.0f };
        TreePeriods = new float[] { 1.0f, 10.0f, 20.0f, 40.0f, 50.0f, 100.0f };
        TopographyPeriods = new float[] { 25.0f, 50.0f, 100.0f, 200.0f, 200.0f, 200.0f, 200.0f};
        RiverJumpPeriods = new float[] { 2.0f, 5.0f, 10.0f };

        MaxRiverJumpDistance = 75f;

        //System.Random rng = new System.Random();
        //MasterSeed = rng.Next(-int.MaxValue, int.MaxValue);
        MasterSeed = seed;
        UnityEngine.Random.InitState(MasterSeed);
        GrassMediumSeed = UnityEngine.Random.value;
        GrassTallSeed = UnityEngine.Random.value;
        TreeRandomSeed = UnityEngine.Random.value;
        TopographySeed = UnityEngine.Random.value;
    }

    [SerializationConstructor]
    public WorldGenParameters(
        int masterSeed,
        float grassMediumSeed,
        float[] grassMediumPeriods,
        float grassMediumDensity,
        float grassTallSeed,
        float[] grassTallPeriods,
        float grassTallDensity,
        float treeRandomSeed,
        float[] treePeriods,
        float treeDensity,
        float topographySeed,
        float[] topographyPeriods,
        float mountainAltitude,
        float sandAltitude,
        float oceanAltitude,
        float riverDensity,
        float maxRiverJumpDistance,
        float[] riverJumpPeriods)
    {
        MasterSeed = masterSeed;
        GrassMediumSeed = grassMediumSeed;
        GrassMediumPeriods = grassMediumPeriods;
        GrassMediumDensity = grassMediumDensity;
        GrassTallSeed = grassTallSeed;
        GrassTallPeriods = grassTallPeriods;
        GrassTallDensity = grassTallDensity;
        TreeRandomSeed = treeRandomSeed;
        TreePeriods = treePeriods;
        TreeDensity = treeDensity;
        TopographySeed = topographySeed;
        TopographyPeriods = topographyPeriods;
        MountainAltitude = mountainAltitude;
        SandAltitude = sandAltitude;
        OceanAltitude = oceanAltitude;
        RiverDensity = riverDensity;
        MaxRiverJumpDistance = maxRiverJumpDistance;
        RiverJumpPeriods = riverJumpPeriods;
    }
}
