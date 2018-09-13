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
    public bool GameIsLive
    {
        get
        {
            return StateMachine.GameIsLive;
        }
    }

    // UI stuff
    public GameObject SplashScreen;
    public GameObject PausedMenu;
    public GameObject GameInfo;
    public GameObject RandomSeedText;
    public GameObject PlayerLocationText;
    public GameObject HUD;
    public GameObject DeathScreen;
    public GameObject DeathBackground;
    public GameObject GameStatsCanvas;
    public GameObject FPSText;
    public GameObject UPSText;
    public GameObject GameObjectText;
    public GameObject LevelUpText;

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
        World = new World();
        PlayerCamera = Instantiate(Prefabs.CAMERA_PREFAB, new Vector3(0, 0, -1), Quaternion.identity);
        World.Start();
    }

    IEnumerator PrintFPS()
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
    IEnumerator PrintUPS()
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
    IEnumerator PrintGameObjects()
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

        stateMachine.AddEntryState(GameStateType.Loading, false);
        stateMachine.AddState(GameStateType.Playing, true);
        stateMachine.AddState(GameStateType.PausedMenu, false);
        stateMachine.AddState(GameStateType.GameInfoMenu, false);
        stateMachine.AddState(GameStateType.PlayerDead, false);

        // Loading
        stateMachine.AddTransition(GameStateType.Loading, GameStateType.Playing, GameInputType.FinishedLoading);
        stateMachine.OnExit(GameStateType.Loading, OnFinishedLoading);

        // Playing
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PlayerDead, GameInputType.PlayerDeath);

        // Paused Menu
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.GameInfoMenu, GameInputType.OpenGameInfoMenu);
        stateMachine.OnEnter(GameStateType.PausedMenu, OnPause);
        stateMachine.OnExit(GameStateType.PausedMenu, OnResume);

        // GameInfo Menu
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.PausedMenu, GameInputType.CloseGameInfoMenu);
        stateMachine.OnEnter(GameStateType.GameInfoMenu, OnOpenGameInfo);
        stateMachine.OnExit(GameStateType.GameInfoMenu, OnCloseGameInfo);

        // Player Dead
        stateMachine.AddTransition(GameStateType.PlayerDead, GameStateType.Playing, GameInputType.PlayerRespawn);
        stateMachine.OnEnter(GameStateType.PlayerDead, OnPlayerDeath);

        return stateMachine;
    }

    public void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
        {
            Input(GameInputType.Pause);
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.BackQuote))
        {
            GameStatsCanvas.SetActive(!GameStatsCanvas.activeInHierarchy);
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
        World.Update();
    }

    public void FixedUpdate()
    {
        UPSQueue.Enqueue(1 / Time.deltaTime);
        if (UPSQueue.Count > 10)
        {
            UPSQueue.Dequeue();
        }
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

    public void Input(GameInputType inputType)
    {
        StateMachine.GiveInput(inputType);
    }

    public void OnFinishedLoading(GameInputType intputType, GameStateType nextState)
    {
        SplashScreen.SetActive(false);
    }

    public void OnPause(GameStateType previousState, GameInputType inputType)
    {
        PausedMenu.SetActive(true);
    }

    public void OnResume(GameInputType intputType, GameStateType nextState)
    {
        PausedMenu.SetActive(false);
    }

    public void OnOpenGameInfo(GameStateType previousState, GameInputType inputType)
    {
        GameInfo.SetActive(true);
        RandomSeedText.GetComponent<Text>().text = "Random Seed: "+World.GetRandomSeed().ToString();
        PlayerLocationText.GetComponent<Text>().text = "Player Location: "+World.GetPlayerLocation().ToString();
    }

    public void OnCloseGameInfo(GameInputType inputType, GameStateType nextState)
    {
        GameInfo.SetActive(false);
    }

    public void ResumePressed()
    {
        Input(GameInputType.Pause);
    }

    public void GameInfoPressed()
    {
        Input(GameInputType.OpenGameInfoMenu);
    }

    public void BackPressed()
    {
        Input(GameInputType.CloseGameInfoMenu);
    }

    public void QuitPressed()
    {
        Application.Quit();
    }

    public void OnPlayerDeath(GameStateType previousState, GameInputType inputType)
    {
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
        World.Start();
        DeathScreen.SetActive(false);
        Singleton.Input(GameInputType.PlayerRespawn);
    }

    public void Print(string s)
    {
        print(s);
    }
}
