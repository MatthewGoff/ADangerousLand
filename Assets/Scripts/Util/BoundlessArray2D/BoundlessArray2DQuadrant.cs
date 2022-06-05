using MessagePack;

namespace ADL.Util
{
    [MessagePackObject]
    public class BoundlessArray2DQuadrant<T>
    {

        [Key(0)] public T[,] Array;

        public BoundlessArray2DQuadrant(int width, int height)
        {
            Array = new T[width, height];
        }

        [SerializationConstructor]
        public BoundlessArray2DQuadrant(T[,] array)
        {
            Array = array;
        }

        public T Get(int x, int y)
        {
            if (Helpers.WithinArrayBounds2D<T>(ref Array, x, y))
            {
                return Array[x, y];
            }
            else
            {
                return default;
            }
        }

        public void Set(int x, int y, T data)
        {
            if (Helpers.WithinArrayBounds2D<T>(ref Array, x, y))
            {
                Array[x, y] = data;
            }
            else
            {
                T[,] newArray = new T[Array.GetLength(0) * 2, Array.GetLength(1) * 2];
                for (int i = 0; i < Array.GetLength(0); i++)
                {
                    for (int j = 0; j < Array.GetLength(1); j++)
                    {
                        newArray[i, j] = Array[i, j];
                    }
                }
                Array = newArray;
                Set(x, y, data);
            }
        }
    }
}