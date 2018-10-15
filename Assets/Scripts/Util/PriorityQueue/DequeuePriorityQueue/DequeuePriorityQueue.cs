using System.Collections.Generic;

namespace ADL.Util
{
    public class DequeuePriorityQueue<T> where T : IPrioritizable
    {
        public List<T> Nodes;

        public DequeuePriorityQueue()
        {
            Nodes = new List<T>();
        }

        public void Enqueue(T data)
        {
            Nodes.Add(data);
        }

        public T Dequeue()
        {
            T minimumData = default;
            float minimumPriority = float.MaxValue;

            foreach (T data in Nodes)
            {
                float priority = data.GetPriority();
                if (priority < minimumPriority)
                {
                    minimumPriority = priority;
                    minimumData = data;
                }
            }

            Nodes.Remove(minimumData);
            return minimumData;
        }

        public bool Contains(T node)
        {
            if (Nodes == null)
            {
                return false;
            }
            else
            {
                return Nodes.Contains(node);
            }
        }
    }
}