using ProtoBuf;

[ProtoContract]
public class RiverNode
{
    [ProtoMember(1)] public int WaterLevel { get; private set; }
    [ProtoMember(2)] public bool IsRiver;

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