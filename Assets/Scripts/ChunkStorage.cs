public class ChunkStorage
{

    private static readonly int DEFAULT_QUAD_SIZE = 10;

    private WorldController MyWorld;
    private ExtensibleArray2D<Chunk> QuadOne;
    private ExtensibleArray2D<Chunk> QuadTwo;
    private ExtensibleArray2D<Chunk> QuadThree;
    private ExtensibleArray2D<Chunk> QuadFour;

    public ChunkStorage(WorldController world)
    {
        MyWorld = world;
        QuadOne = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadTwo = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadThree = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        QuadFour = new ExtensibleArray2D<Chunk>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
    }

    public Chunk GetChunk(ChunkIndex chunkIndex)
    {
        Chunk chunk;
        if (chunkIndex.X >= 0 && chunkIndex.Y >= 0)
        {
            chunk = QuadOne.Get(chunkIndex.X, chunkIndex.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, chunkIndex);
                QuadOne.Set(chunkIndex.X, chunkIndex.Y, chunk);
            }
        }
        else if (chunkIndex.X < 0 && chunkIndex.Y >= 0)
        {
            chunk = QuadTwo.Get(-chunkIndex.X, chunkIndex.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, chunkIndex);
                QuadTwo.Set(-chunkIndex.X, chunkIndex.Y, chunk);
            }
        }
        else if (chunkIndex.X < 0 && chunkIndex.Y < 0)
        {
            chunk = QuadThree.Get(-chunkIndex.X, -chunkIndex.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, chunkIndex);
                QuadThree.Set(-chunkIndex.X, -chunkIndex.Y, chunk);
            }
        }
        else
        {
            chunk = QuadFour.Get(chunkIndex.X, -chunkIndex.Y);
            if (chunk == null)
            {
                chunk = new Chunk(MyWorld, chunkIndex);
                QuadFour.Set(chunkIndex.X, -chunkIndex.Y, chunk);
            }
        }
        return chunk;
    }

    public void SetChunk(int x, int y, Chunk chunk)
    {
        if (x >= 0 && y >= 0)
        {
            QuadOne.Set(x, y, chunk);
        }
        else if (x < 0 && y >= 0)
        {
            QuadTwo.Set(-x, y, chunk);
        }
        else if (x < 0 && y < 0)
        {
            QuadThree.Set(-x, -y, chunk);
        }
        else
        {
            QuadFour.Set(x, -y, chunk);
        }
    }
}
