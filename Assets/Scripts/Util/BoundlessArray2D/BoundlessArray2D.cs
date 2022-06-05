using MessagePack;

namespace ADL.Util
{
    [MessagePackObject]
    public class BoundlessArray2D<T> {

        [IgnoreMember] private static readonly int DEFAULT_QUAD_SIZE = 10;

        [Key(0)] public BoundlessArray2DQuadrant<T> QuadOne;
        [Key(1)] public BoundlessArray2DQuadrant<T> QuadTwo;
        [Key(2)] public BoundlessArray2DQuadrant<T> QuadThree;
        [Key(3)] public BoundlessArray2DQuadrant<T> QuadFour;

        public BoundlessArray2D()
        {
            QuadOne = new BoundlessArray2DQuadrant<T>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
            QuadTwo = new BoundlessArray2DQuadrant<T>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
            QuadThree = new BoundlessArray2DQuadrant<T>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
            QuadFour = new BoundlessArray2DQuadrant<T>(DEFAULT_QUAD_SIZE, DEFAULT_QUAD_SIZE);
        }

        [SerializationConstructor]
        public BoundlessArray2D(
            BoundlessArray2DQuadrant<T> quadOne,
            BoundlessArray2DQuadrant<T> quadTwo,
            BoundlessArray2DQuadrant<T> quadThree,
            BoundlessArray2DQuadrant<T> quadFour)
        {
            QuadOne = quadOne;
            QuadTwo = quadTwo;
            QuadThree = quadThree;
            QuadFour = quadFour;
        }

        public T Get((int X, int Y) tuple)
        {
            return Get(tuple.X, tuple.Y);
        }

        public T Get(int x, int y)
        {
            if (x >= 0 && y >= 0)
            {
                return QuadOne.Get(x, y);
            }
            else if (x < 0 && y >= 0)
            {
                return QuadTwo.Get(-x, y);
            }
            else if (x < 0 && y < 0)
            {
                return QuadThree.Get(-x, -y);
            }
            else
            {
                return QuadFour.Get(x, -y);
            }
        }

        public void Set(int x, int y, T data)
        {
            if (x >= 0 && y >= 0)
            {
                QuadOne.Set(x, y, data);
            }
            else if (x < 0 && y >= 0)
            {
                QuadTwo.Set(-x, y, data);
            }
            else if (x < 0 && y < 0)
            {
                QuadThree.Set(-x, -y, data);
            }
            else
            {
                QuadFour.Set(x, -y, data);
            }
        }
    }
}