public struct ChunkIndex
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

    public ChunkIndex(int x, int y)
    {
        X = x;
        Y = y;
    }

    public ChunkIndex((int X, int Y) tuple)
    {
        X = tuple.X;
        Y = tuple.Y;
    }

    public ChunkIndex Add((int X, int Y) tuple)
    {
        return new ChunkIndex(X+tuple.X, Y+tuple.Y);
    }

    public ChunkIndex Add(int x, int y)
    {
        return new ChunkIndex(X + x, Y + y);
    }
}