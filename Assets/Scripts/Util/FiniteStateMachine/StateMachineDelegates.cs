namespace ADL.Util
{
    public delegate void OnEnterStateDelegate<S, I>(S previousState, I input);
    public delegate void OnExitStateDelegate<S, I>(I input, S nextState);
}