using System.Collections.Generic;

/*
 * 
 * 
 */
public class FiniteStateMachine<S, I>
{
    private Dictionary<S, State<S, I>> GameStates;
    private State<S, I> EntryState;
    public State<S, I> CurrentState;
    public bool PlayerInputEnabled
    {
        get
        {
            return CurrentState.PlayerInputEnabled;
        }
    }

    public FiniteStateMachine()
    {
        GameStates = new Dictionary<S, State<S, I>>();
    }

    public void AddState(S gameStateType, bool playerInputEnabled)
    {
        GameStates.Add(gameStateType, new State<S, I>(gameStateType, playerInputEnabled));
    }

    public void AddEntryState(S gameStateType, bool playerInputEnabled)
    {
        State<S, I> state = new State<S, I>(gameStateType, playerInputEnabled);
        GameStates.Add(gameStateType, state);
        EntryState = state;
    }

    public void Enter()
    {
        CurrentState = EntryState;
        CurrentState.OnEnter?.Invoke(default(S), default(I));
    }

    public void AssignEntryState(S entryState)
    {
        EntryState = GameStates[entryState];
    }

    public void AddTransition(S state1, S state2, I intput)
    {
        GameStates[state1].AddTransition(intput, GameStates[state2]);
    }

    public void GiveInput(I input)
    {
        if (CurrentState.Transitions.ContainsKey(input))
        {
            State<S, I> nextState = CurrentState.Transitions[input];
            CurrentState.OnExit?.Invoke(input, nextState.StateIdentifier);
            CurrentState = nextState;
            CurrentState.OnEnter?.Invoke(CurrentState.StateIdentifier, input);
        }
    }

    public void OnEnter(S state, OnEnterDelegate<S, I> onEnter)
    {
        GameStates[state].OnEnter = onEnter;
    }

    public void OnExit(S state, OnExitDelegate<S, I> onExit)
    {
        GameStates[state].OnExit = onExit;
    }

    public S GetCurrentState()
    {
        return CurrentState.StateIdentifier;
    }
}