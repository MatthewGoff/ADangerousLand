using System.Collections.Generic;

public class AStarPathfinder<T>
{
    public delegate List<T> Neighbors(T A);
    public delegate float Heuristic(T A, T B);
    public delegate float Weight(T A, T B);
    public delegate bool Equal(T A, T B);

    public static List<T> FindPath(Neighbors neighborFunction, Heuristic heuristicFunction, Weight weightFunction, Equal equal, T start, T finish)
    {
        List<PathNode<T>> visited = new List<PathNode<T>>();
        DequeuePriorityQueue<PathNode<T>> toVisit = new DequeuePriorityQueue<PathNode<T>>();
        List<PathNode<T>> knownPlaces = new List<PathNode<T>>();

        PathNode<T> visiting = GetPathNode(start);
        visiting.ActualDistance = 0.0f;
        visiting.HeuristicDistance = heuristicFunction(start, finish);

        while (!equal(visiting.Data, finish))
        {
            List<T> neighbors = neighborFunction(visiting.Data);
            foreach (T neighborData in neighbors)
            {
                PathNode<T> neighborNode = GetPathNode(neighborData);
                if (toVisit.Contains(neighborNode))
                {
                    float distance = visiting.ActualDistance + weightFunction(neighborData, visiting.Data);
                    if (distance < neighborNode.ActualDistance)
                    {
                        neighborNode.ActualDistance = distance;
                        neighborNode.Previous = visiting;
                    }
                }
                else if (!visited.Contains(neighborNode))
                {
                    neighborNode.ActualDistance = visiting.ActualDistance + weightFunction(neighborData, visiting.Data);
                    neighborNode.HeuristicDistance = heuristicFunction(neighborData, finish);
                    neighborNode.Previous = visiting;
                    toVisit.Enqueue(neighborNode);
                }
            }

            visited.Add(visiting);
            visiting = toVisit.Dequeue();
        }

        return ConstructPath();

        PathNode<T> GetPathNode(T data)
        {
            foreach (PathNode<T> node in knownPlaces)
            {
                if (equal(node.Data, data))
                {
                    return node;
                }
            }
            PathNode<T> newNode = new PathNode<T>(data);
            knownPlaces.Add(newNode);
            return newNode;
        }

        List<T> ConstructPath()
        {
            List<T> path = new List<T>();
            path.Add(visiting.Data);
            while (visiting.Previous != null)
            {
                visiting = visiting.Previous;
                path.Add(visiting.Data);
            }
            path.Reverse();
            return path;
        }
    }
}