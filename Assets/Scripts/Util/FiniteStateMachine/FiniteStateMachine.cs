using System.Collections.Generic;

public class FiniteStateMachine<S, I>
{
    private Dictionary<S, State<S, I>> GameStates;
    private State<S, I> EntryState;
    public State<S, I> CurrentState;

    public FiniteStateMachine()
    {
        GameStates = new Dictionary<S, State<S, I>>();
    }

    public void AddState(S gameStateType)
    {
        GameStates.Add(gameStateType, new State<S, I>(gameStateType));
    }

    public void AddEntryState(S gameStateType)
    {
        State<S, I> state = new State<S, I>(gameStateType);
        GameStates.Add(gameStateType, state);
        EntryState = state;
    }

    public void Enter()
    {
        CurrentState = EntryState;
        CurrentState.OnEnter?.Invoke(default, default);
    }

    public void AssignEntryState(S entryState)
    {
        EntryState = GameStates[entryState];
    }

    public void AddTransition(S state1, S state2, I intput, bool cachePrevious)
    {
        GameStates[state1].AddTransition(intput, GameStates[state2], cachePrevious);
    }

    public void AddTransitionToPrevious(S state, I input, bool cachePrevious)
    {
        GameStates[state].AddTransitionToPrevious(input, cachePrevious);
    }

    public void TakeInput(I input)
    {
        if (CurrentState.Transitions.ContainsKey(input))
        {
            Transition<S, I> transition = CurrentState.Transitions[input];
            State<S, I> nextState;
            if (transition.IsTransitionToPrevious)
            {
                nextState = CurrentState.PreviousState;
            }
            else
            {
                nextState = CurrentState.Transitions[input].Destination;
            }

            if (transition.CachePrevious)
            {
                nextState.PreviousState = CurrentState;
            }

            CurrentState.OnExit?.Invoke(input, nextState.StateIdentifier);
            State<S, I> PreviousState = CurrentState;
            CurrentState = nextState;
            CurrentState.OnEnter?.Invoke(PreviousState.StateIdentifier, input);
        }
    }

    public void OnEnterState(S state, OnEnterStateDelegate<S, I> onEnter)
    {
        GameStates[state].OnEnter = onEnter;
    }

    public void OnExitState(S state, OnExitStateDelegate<S, I> onExit)
    {
        GameStates[state].OnExit = onExit;
    }

    public S GetCurrentState()
    {
        return CurrentState.StateIdentifier;
    }
}