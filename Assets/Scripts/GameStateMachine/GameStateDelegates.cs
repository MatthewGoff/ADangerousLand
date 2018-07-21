public delegate void OnEnterDelegate(GameStateType previousState, InputType inputType);
public delegate void OnExitDelegate(InputType inputType, GameStateType nextState);