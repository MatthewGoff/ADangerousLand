public class EnqueuePriorityQueueNode<T>
{
    public int Priority { get; set; }
    public T Data { get; set; }
    public EnqueuePriorityQueueNode<T> Next { get; set; }

    public EnqueuePriorityQueueNode(T data, int priority)
    {
        Data = data;
        Priority = priority;
    }
}