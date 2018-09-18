using System;

[Serializable]
public struct WorldLocation
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

    public WorldLocation(int x, int y)
    {
        X = x;
        Y = y;
    }

    public WorldLocation((int x, int y) tuple)
    {
        X = tuple.x;
        Y = tuple.y;
    }

    public override string ToString()
    {
        return "(" + X.ToString() + "," + Y.ToString() + ")";
    }
}