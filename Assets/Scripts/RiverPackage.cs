using System;

[Serializable]
public struct RiverPackage
{
    public WorldLocation WorldLocation;
    public RiverType Type;
   
    public RiverPackage(WorldLocation worldLocation,  RiverType type)
    {
        WorldLocation = worldLocation;
        Type = type;
    }
}