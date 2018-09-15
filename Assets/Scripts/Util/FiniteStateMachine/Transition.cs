public struct Transition<S, I> {

    public readonly State<S, I> Destination;
    public readonly bool IsTransitionToPrevious;
    public readonly bool CachePrevious;

    public Transition(State<S, I> destination, bool cachePrevious)
    {
        Destination = destination;
        IsTransitionToPrevious = false;
        CachePrevious = cachePrevious;
    }

    public Transition(bool cachePrevious)
    {
        Destination = default;
        IsTransitionToPrevious = true;
        CachePrevious = cachePrevious;
    }
}
