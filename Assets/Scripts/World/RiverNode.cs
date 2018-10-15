using MessagePack;

namespace ADL.World
{
    [MessagePackObject]
    public struct RiverNode
    {
        [Key(0)] public int WaterLevel { get; private set; }
        [Key(1)] public bool IsRiver;

        [SerializationConstructor]
        public RiverNode(int waterLevel, bool isRiver)
        {
            WaterLevel = waterLevel;
            IsRiver = isRiver;
        }

        public void IncrementWaterLevel()
        {
            WaterLevel++;
        }
    }
}