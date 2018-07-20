public class ChunkStorage
{

    private static readonly int DEFAULT_QUAD_SIZE = 10;

    private World MyWorld;
    private ExtensibleArray2D<Chunk> QuadOne;
    private ExtensibleArray2D<Chunk> QuadTwo;
    private ExtensibleArray2D<Chunk> QuadThree;
    private ExtensibleArray2D<Chunk> QuadFour;

    public ChunkStorage(World world)
    {
        MyWorld = world;
        QuadOne = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadTwo = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadThree = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadFour = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
    }

    public Chunk GetChunk((int X, int Y) index)
    {
        Chunk chunk;
        if (index.X >= 0 && index.Y >= 0)
        {
            chunk = QuadOne.Get(index.X, index.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, index);
                QuadOne.Set(index.X, index.Y, chunk);
            }
        }
        else if (index.X < 0 && index.Y >= 0)
        {
            chunk = QuadTwo.Get(-index.X, index.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, index);
                QuadTwo.Set(-index.X, index.Y, chunk);
            }
        }
        else if (index.X < 0 && index.Y < 0)
        {
            chunk = QuadThree.Get(-index.X, -index.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, index);
                QuadThree.Set(-index.X, -index.Y, chunk);
            }
        }
        else
        {
            chunk = QuadFour.Get(index.X, -index.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, index);
                QuadFour.Set(index.X, -index.Y, chunk);
            }
        }
        return chunk;
    }

    public void SetChunk(int x, int y, Chunk c)
    {
        if (x >= 0 && y >= 0)
        {
            QuadOne.Set(x, y, c);
        }
        else if (x < 0 && y >= 0)
        {
            QuadTwo.Set(-x, y, c);
        }
        else if (x < 0 && y < 0)
        {
            QuadThree.Set(-x, -y, c);
        }
        else
        {
            QuadFour.Set(x, -y, c);
        }
    }

    public bool HasChunk((int X, int Y) index)
    {
        return GetChunk(index) != null;
    }
}
