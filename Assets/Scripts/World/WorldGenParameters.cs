using ProtoBuf;

[ProtoContract]
public class WorldGenParameters
{
    [ProtoMember(1)] public readonly int MasterSeed;

    [ProtoMember(2)] public readonly float GrassMediumSeed;
    [ProtoMember(3)] public readonly float[] GrassMediumPeriods;
    [ProtoMember(4)] public readonly float GrassMediumDensity;

    [ProtoMember(5)] public readonly float GrassTallSeed;
    [ProtoMember(6)] public readonly float[] GrassTallPeriods;
    [ProtoMember(7)] public readonly float GrassTallDensity;

    [ProtoMember(8)] public readonly float TreeRandomSeed;
    [ProtoMember(9)] public readonly float[] TreePeriods;
    [ProtoMember(10)] public readonly float TreeDensity;

    [ProtoMember(11)] public readonly float Topography;
    [ProtoMember(12)] public readonly float[] TopographyPeriods;
    [ProtoMember(13)] public readonly float MountainAltitude;
    [ProtoMember(14)] public readonly float SandAltitude;
    [ProtoMember(15)] public readonly float OceanAltitude;

    [ProtoMember(16)] public readonly float RiverDensity;
    [ProtoMember(17)] public readonly float MaxRiverJumpDistance;
    [ProtoMember(18)] public readonly float[] RiverJumpPeriods;
    
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
        Topography = UnityEngine.Random.value;
    }
}
