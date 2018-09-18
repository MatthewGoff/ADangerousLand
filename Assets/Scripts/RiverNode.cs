using System;

[Serializable]
public class RiverNode
{
    public int WaterLevel { get; private set; }
    public bool IsRiver;

    public RiverNode()
    {
        WaterLevel = 0;
        IsRiver = false;
    }

    public void IncrementWaterLevel()
    {
        WaterLevel++;
    }
}