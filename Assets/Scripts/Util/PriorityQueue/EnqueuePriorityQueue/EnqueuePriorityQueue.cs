namespace ADL.Util
{
    public class EnqueuePriorityQueue<T>
    {
        public EnqueuePriorityQueueNode<T> First;

        public void Enqueue(T data, int priority)
        {
            EnqueuePriorityQueueNode<T> newNode = new EnqueuePriorityQueueNode<T>(data, priority);
            if (First == null || newNode.Priority <= First.Priority)
            {
                newNode.Next = First;
                First = newNode;
                return;
            }

            EnqueuePriorityQueueNode<T> runner = First;
            while (runner.Next != null && newNode.Priority > runner.Next.Priority)
            {
                runner = runner.Next;
            }
            newNode.Next = runner.Next;
            runner.Next = newNode;
        }

        public T Dequeue()
        {
            if (First == null)
            {
                return default;
            }
            else
            {
                T returnData = First.Data;
                First = First.Next;
                return returnData;
            }
        }
    }
}