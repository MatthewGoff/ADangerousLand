using MessagePack;

namespace ADL.World
{
    [MessagePackObject]
    public struct RiverPackage
    {
        [Key(0)] public WorldLocation WorldLocation;
        [Key(1)] public RiverType Type;

        [SerializationConstructor]
        public RiverPackage(WorldLocation worldLocation, RiverType type)
        {
            WorldLocation = worldLocation;
            Type = type;
        }
    }
}