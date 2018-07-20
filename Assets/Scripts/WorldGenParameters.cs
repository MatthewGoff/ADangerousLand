﻿using UnityEngine;

public class WorldGenParameters
{
    public readonly float GrassMediumSeed;
    public readonly float GrassMediumDensity;
    public readonly float[] GrassMediumPeriods;

    public readonly float GrassTallSeed;
    public readonly float GrassTallDensity;
    public readonly float[] GrassTallPeriods;

    public readonly float TreeRandomSeed;
    public readonly float TreeDensity;
    public readonly float[] TreePeriods;

    public readonly float MountainAltitude;
    public readonly float SandAltitude;
    public readonly float OceanAltitude;
    public readonly float RiverDensity;
    public readonly float[] TopologyPeriods;
    public readonly int MaxRiverJumpDistance;
    public readonly float[] RiverJumpPeriods;
    public readonly int MasterSeed;
    
    public readonly float TopologyRandomSeed;

    public WorldGenParameters()
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
        TopologyPeriods = new float[] { 25.0f, 50.0f, 100.0f, 200.0f, 200.0f, 200.0f, 200.0f};
        RiverJumpPeriods = new float[] { 2.0f, 5.0f, 10.0f };

        MaxRiverJumpDistance = 75;

        System.Random rng = new System.Random();
        //MasterSeed = rng.Next(-int.MaxValue, int.MaxValue);
        //MasterSeed = -1481425401;
        MasterSeed = 214699974;
        UnityEngine.Random.InitState(MasterSeed);
        GrassMediumSeed = UnityEngine.Random.value;
        GrassTallSeed = UnityEngine.Random.value;
        TreeRandomSeed = UnityEngine.Random.value;
        TopologyRandomSeed = UnityEngine.Random.value;
        //TopologyRandomSeed = 0.2864451f;
    }
}
