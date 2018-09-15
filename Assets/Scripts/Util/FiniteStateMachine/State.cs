using System.Collections.Generic;

public class State<S, I>
{
    public OnEnterStateDelegate<S, I> OnEnter;
    public OnExitStateDelegate<S, I> OnExit;
    public readonly S StateIdentifier;
    public readonly Dictionary<I, Transition<S, I>> Transitions;
    public State<S, I> PreviousState;

    public State(S stateIdentifier)
    {
        StateIdentifier = stateIdentifier;
        Transitions = new Dictionary<I, Transition<S, I>>();
    }

    public void AddTransition(I input, State<S, I> state, bool cachePrevious)
    {
        Transitions.Add(input, new Transition<S, I>(state, cachePrevious));
    }

    public void AddTransitionToPrevious(I input, bool cachePrevious)
    {
        Transitions.Add(input, new Transition<S, I>(cachePrevious));
    }
}
