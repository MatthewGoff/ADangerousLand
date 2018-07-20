public class PathNode<T> : IPrioritizable
{
    public PathNode<T> Previous;
    public float ActualDistance;
    public float HeuristicDistance;
    public T Data;

    public PathNode(T data)
    {
        Data = data;
    }

    public float GetPriority()
    {
        return ActualDistance + HeuristicDistance;
    }
}