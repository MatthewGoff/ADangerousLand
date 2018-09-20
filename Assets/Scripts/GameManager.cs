using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    public GameStateType GameState
    {
        get
        {
            return StateMachine.GetCurrentState();
        }
    }

    // UI stuff
    public GameObject SplashScreen;
    public GameObject PausedMenu;
    public GameObject InfoMenu;
    public GameObject HUD;
    public GameObject DeathScreen;
    public GameObject DeathBackground;
    public GameObject GameStatsCanvas;
    public GameObject FPSText;
    public GameObject UPSText;
    public GameObject GameObjectText;
    public GameObject LevelUpText;
    public GameObject PassivesMenu;
    public GameObject MainMenu;
    public GameObject PlayerMenu;
    public GameObject NewPlayerMenu;
    public GameObject WorldMenu;
    public GameObject NewWorldMenu;

    // Audio
    public GameObject AudioSource;

    // GameStats
    public int GameObjectCount = 0;
    private Queue<float> FPSQueue;
    private Queue<float> UPSQueue;
    private Queue<float> GameObjectsQueue;

    public GameObject PlayerCamera;
    public World World;
    private FiniteStateMachine<GameStateType, GameInputType> StateMachine;

    void Awake()
    {
        Singleton = this;
        StateMachine = CreateStateMachine();
        StateMachine.Enter();

        FPSQueue = new Queue<float>();
        UPSQueue = new Queue<float>();
        GameObjectsQueue = new Queue<float>();

        StartCoroutine("PrintFPS");
        StartCoroutine("PrintUPS");
        StartCoroutine("PrintGameObjects");

        Prefabs.LoadPrefabs();
        PlayerPersistenceManager.Initialize();
        WorldPersistenceManager.Initialize();
        StartCoroutine("Load");
    }

    private IEnumerator Load()
    {
        yield return new WaitForSecondsRealtime(2f);
        TakeInput(GameInputType.FinishedLoading);
    }

    private IEnumerator PrintFPS()
    {
        while (true)
        {
            float accumulator = 0f;
            foreach (float x in FPSQueue)
            {
                accumulator += x;
            }
            accumulator /= FPSQueue.Count;
            FPSText.GetComponent<Text>().text = accumulator.ToString();
            yield return new WaitForSeconds(.1f);
        }
    }
    private IEnumerator PrintUPS()
    {
        while (true)
        {
            float accumulator = 0f;
            foreach (float x in UPSQueue)
            {
                accumulator += x;
            }
            accumulator /= UPSQueue.Count;
            UPSText.GetComponent<Text>().text = accumulator.ToString();
            yield return new WaitForSeconds(.1f);
        }
    }
    private IEnumerator PrintGameObjects()
    {
        while (true)
        {
            float accumulator = 0f;
            foreach (float x in GameObjectsQueue)
            {
                accumulator += x;
            }
            accumulator /= GameObjectsQueue.Count;
            GameObjectText.GetComponent<Text>().text = accumulator.ToString();
            yield return new WaitForSeconds(.1f);
        }
    }

    private FiniteStateMachine<GameStateType, GameInputType> CreateStateMachine()
    {
        FiniteStateMachine<GameStateType, GameInputType> stateMachine = new FiniteStateMachine<GameStateType, GameInputType>();

        stateMachine.AddEntryState(GameStateType.Loading);
        stateMachine.AddState(GameStateType.Playing);
        stateMachine.AddState(GameStateType.PausedMenu);
        stateMachine.AddState(GameStateType.InfoMenu);
        stateMachine.AddState(GameStateType.PlayerDead);
        stateMachine.AddState(GameStateType.PassivesMenu);
        stateMachine.AddState(GameStateType.PlayerMenu);
        stateMachine.AddState(GameStateType.NewPlayerMenu);
        stateMachine.AddState(GameStateType.WorldMenu);
        stateMachine.AddState(GameStateType.NewWorldMenu);
        stateMachine.AddState(GameStateType.MainMenu);
        stateMachine.AddState(GameStateType.Exit);

        // Loading
        stateMachine.AddTransition(GameStateType.Loading, GameStateType.MainMenu, GameInputType.FinishedLoading, false);
        stateMachine.OnExitState(GameStateType.Loading, delegate (GameInputType input, GameStateType state) { SplashScreen.SetActive(false); });

        // Playing
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, GameInputType.Escape, true);
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PlayerDead, GameInputType.PlayerDeath, false);
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PassivesMenu, GameInputType.TogglePassivesMenu, false);
        stateMachine.OnEnterState(GameStateType.Playing, OnStartPlaying);
        stateMachine.OnExitState(GameStateType.Playing, delegate (GameInputType input, GameStateType state) { Time.timeScale = 0; });

        // Paused Menu
        stateMachine.AddTransitionToPrevious(GameStateType.PausedMenu, GameInputType.Escape, false);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.InfoMenu, GameInputType.OpenInfoMenu, false);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.MainMenu, GameInputType.OpenMainMenu, false);
        stateMachine.OnEnterState(GameStateType.PausedMenu, delegate (GameStateType state, GameInputType input) { PausedMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.PausedMenu, delegate (GameInputType input, GameStateType state) { PausedMenu.SetActive(false); });

        // Info Menu
        stateMachine.AddTransition(GameStateType.InfoMenu, GameStateType.PausedMenu, GameInputType.Escape, false);
        stateMachine.OnEnterState(GameStateType.InfoMenu, delegate (GameStateType state, GameInputType input) { InfoMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.InfoMenu, delegate (GameInputType input, GameStateType state) { InfoMenu.SetActive(false); });

        // Player Dead
        stateMachine.AddTransition(GameStateType.PlayerDead, GameStateType.Playing, GameInputType.PlayerRespawn, false);
        stateMachine.AddTransition(GameStateType.PlayerDead, GameStateType.PausedMenu, GameInputType.Escape, true);
        stateMachine.AddTransition(GameStateType.PlayerDead, GameStateType.MainMenu, GameInputType.PlayerDeath, false);
        stateMachine.OnEnterState(GameStateType.PlayerDead, OnPlayerDeath);
        stateMachine.OnExitState(GameStateType.PlayerDead, delegate (GameInputType input, GameStateType state) { Time.timeScale = 0; });

        // Passives Menu
        stateMachine.AddTransition(GameStateType.PassivesMenu, GameStateType.Playing, GameInputType.Escape, true);
        stateMachine.AddTransition(GameStateType.PassivesMenu, GameStateType.Playing, GameInputType.TogglePassivesMenu, false);
        stateMachine.OnEnterState(GameStateType.PassivesMenu, delegate (GameStateType state, GameInputType input) { PassivesMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.PassivesMenu, delegate (GameInputType input, GameStateType state) { PassivesMenu.SetActive(false); });

        // Main Menu
        stateMachine.AddTransition(GameStateType.MainMenu, GameStateType.PlayerMenu, GameInputType.OpenPlayerMenu, false);
        stateMachine.AddTransition(GameStateType.MainMenu, GameStateType.Exit, GameInputType.Escape, false);
        stateMachine.OnEnterState(GameStateType.MainMenu, delegate (GameStateType state, GameInputType input) { MainMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.MainMenu, delegate (GameInputType input, GameStateType state) { MainMenu.SetActive(false); });

        // Player Menu
        stateMachine.AddTransition(GameStateType.PlayerMenu, GameStateType.NewPlayerMenu, GameInputType.OpenNewPlayerMenu, false);
        stateMachine.AddTransition(GameStateType.PlayerMenu, GameStateType.MainMenu, GameInputType.Escape, false);
        stateMachine.AddTransition(GameStateType.PlayerMenu, GameStateType.WorldMenu, GameInputType.OpenWorldMenu, false);
        stateMachine.OnEnterState(GameStateType.PlayerMenu, delegate (GameStateType state, GameInputType input) { PlayerMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.PlayerMenu, delegate (GameInputType input, GameStateType state) { PlayerMenu.SetActive(false); });

        // New Player Menu
        stateMachine.AddTransition(GameStateType.NewPlayerMenu, GameStateType.PlayerMenu, GameInputType.Escape, false);
        stateMachine.OnEnterState(GameStateType.NewPlayerMenu, delegate (GameStateType state, GameInputType input) { NewPlayerMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.NewPlayerMenu, delegate (GameInputType input, GameStateType state) { NewPlayerMenu.SetActive(false); });

        // World Menu
        stateMachine.AddTransition(GameStateType.WorldMenu, GameStateType.NewWorldMenu, GameInputType.OpenNewWorldMenu, false);
        stateMachine.AddTransition(GameStateType.WorldMenu, GameStateType.PlayerMenu, GameInputType.Escape, false);
        stateMachine.AddTransition(GameStateType.WorldMenu, GameStateType.Playing, GameInputType.StartPlay, false);
        stateMachine.OnEnterState(GameStateType.WorldMenu, delegate (GameStateType state, GameInputType input) { WorldMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.WorldMenu, delegate (GameInputType input, GameStateType state) { WorldMenu.SetActive(false); });

        // New World Menu
        stateMachine.AddTransition(GameStateType.NewWorldMenu, GameStateType.WorldMenu, GameInputType.Escape, false);
        stateMachine.OnEnterState(GameStateType.NewWorldMenu, delegate (GameStateType state, GameInputType input) { NewWorldMenu.SetActive(true); });
        stateMachine.OnExitState(GameStateType.NewWorldMenu, delegate (GameInputType input, GameStateType state) { NewWorldMenu.SetActive(false); });

        // Exit
        stateMachine.OnEnterState(GameStateType.Exit, delegate (GameStateType state, GameInputType input) { Application.Quit(); });


        return stateMachine;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TakeInput(GameInputType.Escape);
        }
        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            GameStatsCanvas.SetActive(!GameStatsCanvas.activeInHierarchy);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            TakeInput(GameInputType.TogglePassivesMenu);
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            AudioSource.GetComponent<AudioSource>().mute = !AudioSource.GetComponent<AudioSource>().mute;
        }
        if (Input.GetKeyUp(KeyCode.C) && GameState == GameStateType.Playing)
        {
            World.PlayerManager.RecieveExp(190);
        }

        GameObjectsQueue.Enqueue(GameObjectCount);
        if (GameObjectsQueue.Count > 10)
        {
            GameObjectsQueue.Dequeue();
        }
        FPSQueue.Enqueue(1 / Time.deltaTime);
        if (FPSQueue.Count > 10)
        {
            FPSQueue.Dequeue();
        }

        if (World != null)
        {
            World.Update();
        }
    }

    public void FixedUpdate()
    {
        UPSQueue.Enqueue(1 / Time.deltaTime);
        if (UPSQueue.Count > 10)
        {
            UPSQueue.Dequeue();
        }

        if (World != null)
        {
            World.FixedUpdate();
        }
    }

    private void OnStartPlaying(GameStateType previousState, GameInputType input)
    {
        if (previousState == GameStateType.WorldMenu)
        {
            PlayerCamera = Instantiate(Prefabs.CAMERA_PREFAB, new Vector3(0, 0, -1), Quaternion.identity);
            PlayerManager playerManager = PlayerMenu.GetComponent<PlayerMenuController>().SelectedPlayer;
            World = WorldMenu.GetComponent<WorldMenuController>().SelectedWorld;
            World.Setup(playerManager);
            World.Start();
            HUD.SetActive(true);
        }
        Time.timeScale = 1f;
    }

    public void LevelUp()
    {
        StartCoroutine("ShowLevelUp");
    }

    private IEnumerator ShowLevelUp()
    {
        LevelUpText.GetComponent<Text>().text = "Level " + World.PlayerManager.Level.ToString() + "!";
        LevelUpText.GetComponent<Text>().color = new Color(1,1,1,1);
        LevelUpText.SetActive(true);
        float duration = 2f;
        for (float i = 0f; i<duration; i+=Time.deltaTime)
        {
            yield return null;
        }
        StartCoroutine("FadeLevelUp");
    }

    private IEnumerator FadeLevelUp()
    {
        float duration = 3f;
        Text text = LevelUpText.GetComponent<Text>();
        for (float i = 0f; i < duration; i += Time.deltaTime)
        {
            text.color = new Color(1, 1, 1, 1 - (i/duration));
            yield return null;
        }
        LevelUpText.SetActive(false);
    }

    public void TakeInput(GameInputType inputType)
    {
        StateMachine.TakeInput(inputType);
    }

    public void OnPlayerDeath(GameStateType previousState, GameInputType inputType)
    {
        Time.timeScale = 1f;
        DeathScreen.SetActive(true);
        StartCoroutine("FadeToBlack");
    }

    private IEnumerator FadeToBlack()
    {
        DeathBackground.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        for (float i = 0f; i < Configuration.DEATH_DURATION; i += Time.deltaTime)
        {
            DeathBackground.GetComponent<Image>().color = new Color(0, 0, 0, (i/ Configuration.DEATH_DURATION));
            yield return null;
        }
        World.Sleep();

        DeathPenaltyType deathPenalty = World.PlayerManager.DeathPenalty;
        if (deathPenalty == DeathPenaltyType.Softcore)
        {
            World.Start();
            DeathScreen.SetActive(false);
            Singleton.TakeInput(GameInputType.PlayerRespawn);
        }
        else
        {
            PlayerPersistenceManager.DeletePlayer(World.PlayerManager.PlayerIdentifier);
            DeathScreen.SetActive(false);
            Singleton.TakeInput(GameInputType.PlayerDeath);
        }
    }

    public void SaveAndExit()
    {
        PlayerPersistenceManager.SavePlayer(World.PlayerManager);
        WorldPersistenceManager.SaveWorld(World);
        GameManager.Singleton.World.Sleep();
    }

    public static void Print(string s)
    {
        print(s);
    }

    public static float GetTime()
    {
        return Time.time;
    }
}
