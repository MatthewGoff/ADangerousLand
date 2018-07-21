using System.Collections.Generic;

public class GameStateMachine
{
    private Dictionary<GameStateType, GameState> GameStates;
    private GameState EntryState;
    public GameState CurrentState;
    public bool PlayerInputEnabled
    {
        get
        {
            return CurrentState.PlayerInputEnabled;
        }
    }

    public GameStateMachine()
    {
        GameStates = new Dictionary<GameStateType, GameState>();
    }

    public void AddState(GameStateType gameStateType, bool playerInputEnabled)
    {
        GameStates.Add(gameStateType, new GameState(gameStateType, playerInputEnabled));
    }

    public void AddEntryState(GameStateType gameStateType, bool playerInputEnabled)
    {
        GameState state = new GameState(gameStateType, playerInputEnabled);
        GameStates.Add(gameStateType, state);
        EntryState = state;
    }

    public void Enter()
    {
        CurrentState = EntryState;
        CurrentState.OnEnter?.Invoke(GameStateType.BeforeEntry, InputType.Entry);
    }

    public void AssignEntryState(GameStateType entryState)
    {
        EntryState = GameStates[entryState];
    }

    public void AddTransition(GameStateType state1, GameStateType state2, InputType inputType)
    {
        GameStates[state1].AddTransition(inputType, GameStates[state2]);
    }

    public void GiveInput(InputType inputType)
    {
        if (CurrentState.Transitions.ContainsKey(inputType))
        {
            GameState nextState = CurrentState.Transitions[inputType];
            CurrentState.OnExit?.Invoke(inputType, nextState.MyGameState);
            CurrentState = nextState;
            CurrentState.OnEnter?.Invoke(CurrentState.MyGameState, inputType);
        }
    }

    public void OnEnter(GameStateType state, OnEnterDelegate onEnter)
    {
        GameStates[state].OnEnter = onEnter;
    }

    public void OnExit(GameStateType state, OnExitDelegate onExit)
    {
        GameStates[state].OnExit = onExit;
    }

    public GameStateType GetCurrentState()
    {
        return CurrentState.MyGameState;
    }
}