public struct ChunkLocation
{
    public int X { get; set; }
    public int Y { get; set; }
    public (int X, int Y) Tuple
    {
        get
        {
            return (X, Y);
        }
    }

    public ChunkLocation(int x, int y)
    {
        X = x;
        Y = y;
    }

    public ChunkLocation((int x, int y) tuple)
    {
        X = tuple.x;
        Y = tuple.y;
    }
}