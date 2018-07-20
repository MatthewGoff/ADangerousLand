public class ExtensibleArray2D<T>
{

    private T[,] Array;

    public ExtensibleArray2D(int width, int height)
    {
        Array = new T[width, height];
    }

    public T Get(int x, int y)
    {
        if (Util.WithinArrayBounds2D<T>(ref Array, x, y))
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
        if (Util.WithinArrayBounds2D<T>(ref Array, x, y))
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
