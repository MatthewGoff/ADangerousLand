public delegate void OnEnterDelegate<S, I>(S previousState, I input);
public delegate void OnExitDelegate<S, I>(I input, S nextState);