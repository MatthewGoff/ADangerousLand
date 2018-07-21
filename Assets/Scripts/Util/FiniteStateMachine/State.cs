using System.Collections.Generic;

public class State<S, I>
{
    public OnEnterDelegate<S, I> OnEnter;
    public OnExitDelegate<S, I> OnExit;
    public readonly S StateIdentifier;
    public readonly Dictionary<I, State<S, I>> Transitions;
    public readonly bool PlayerInputEnabled;

    public State(S stateIdentifier, bool playerInputEnabled)
    {
        StateIdentifier = stateIdentifier;
        Transitions = new Dictionary<I, State<S, I>>();
        PlayerInputEnabled = playerInputEnabled;
    }

    public void AddTransition(I input, State<S, I> state)
    {
        Transitions.Add(input, state);
    }
}
