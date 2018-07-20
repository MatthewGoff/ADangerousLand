public struct RiverPackage
{
    public (int X, int Y) WorldLocation;
    public RiverType Type;
   
    public RiverPackage((int X, int Y) worldLocation,  RiverType type)
    {
        WorldLocation = worldLocation;
        Type = type;
    }
}