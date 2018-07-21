using System.Collections.Generic;

public class GameState
{
    public OnEnterDelegate OnEnter;
    public OnExitDelegate OnExit;
    public readonly GameStateType MyGameState;
    public readonly Dictionary<InputType, GameState> Transitions;
    public readonly bool PlayerInputEnabled;

    public GameState(GameStateType gameState, bool playerInputEnabled)
    {
        MyGameState = gameState;
        Transitions = new Dictionary<InputType, GameState>();
        PlayerInputEnabled = playerInputEnabled;
    }

    public void AddTransition(InputType inputType, GameState state)
    {
        Transitions.Add(inputType, state);
    }
}
