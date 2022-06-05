namespace ADL.Core
{
    /// <summary>
    /// Enumeration of the types of input to the game's state
    /// </summary>
    public enum GameInputType
    {
        Entry,
        Escape,
        FinishedLoading,
        StartPlay,
        PlayerDeath,
        PlayerRespawn,
        OpenPassivesMenu,
        OpenInfoMenu,
        OpenPlayerMenu,
        OpenNewPlayerMenu,
        OpenWorldMenu,
        OpenNewWorldMenu,
        WorldLoaded,
        WorldUnloaded,
        SaveAndExit,
        OpenControlsMenu
    }
}