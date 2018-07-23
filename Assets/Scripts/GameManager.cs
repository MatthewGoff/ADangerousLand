using UnityEngine;
using UnityEngine.UI;

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
    public bool PlayerInputEnabled
    {
        get
        {
            return StateMachine.PlayerInputEnabled;
        }
    }

    // UI stuff
    public GameObject SplashScreen;
    public GameObject PausedMenu;
    public GameObject GameInfo;
    public GameObject RandomSeedText;
    public GameObject PlayerLocationText;

    private static bool MaxMode = false;
    private GameObject PlayerCamera;
    private World World;
    private FiniteStateMachine<GameStateType, GameInputType> StateMachine;

    void Awake()
    {
        Singleton = this;
        StateMachine = CreateStateMachine();
        StateMachine.Enter();

        Prefabs.LoadPrefabs();
        World = new World();
        PlayerCamera = Instantiate(Prefabs.CAMERA_PREFAB, new Vector3(0, 0, -10), Quaternion.identity) as GameObject;
        InitGame();
    }

    private FiniteStateMachine<GameStateType, GameInputType> CreateStateMachine()
    {
        FiniteStateMachine<GameStateType, GameInputType> stateMachine = new FiniteStateMachine<GameStateType, GameInputType>();

        stateMachine.AddEntryState(GameStateType.Loading, false);
        stateMachine.AddState(GameStateType.Playing, true);
        stateMachine.AddState(GameStateType.PausedMenu, false);
        stateMachine.AddState(GameStateType.GameInfoMenu, false);

        //Loading
        stateMachine.AddTransition(GameStateType.Loading, GameStateType.Playing, GameInputType.FinishedLoading);
        stateMachine.OnEnter(GameStateType.Loading, OnStartLoading);
        stateMachine.OnExit(GameStateType.Loading, OnFinishedLoading);

        //Playing
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, GameInputType.Pause);

        //Paused Menu
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.GameInfoMenu, GameInputType.OpenGameInfoMenu);
        stateMachine.OnEnter(GameStateType.PausedMenu, OnPause);
        stateMachine.OnExit(GameStateType.PausedMenu, OnResume);

        //GameInfo Menu
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.PausedMenu, GameInputType.CloseGameInfoMenu);
        stateMachine.OnEnter(GameStateType.GameInfoMenu, OnOpenGameInfo);
        stateMachine.OnExit(GameStateType.GameInfoMenu, OnCloseGameInfo);

        return stateMachine;
    }

    private void InitGame()
    {
        World.Start();
        PlayerCamera.GetComponent<CameraController>().AssignPlayer(World.PlayerMonoBehaviour);
        World.PlayerMonoBehaviour.AssignCamera(PlayerCamera);
    }

    public void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.M) && PlayerInputEnabled)
        {
            MaxMode = !MaxMode;
            if (MaxMode)
            {
                Configuration.FOG_OUTER_RADIUS = 80;
                Configuration.FOG_INNER_RADIUS = 75;
                PlayerMonoBehaviour.MoveSpeed = 50;
            }
            else
            {
                Configuration.FOG_OUTER_RADIUS = 11;
                Configuration.FOG_INNER_RADIUS = 7;
                PlayerMonoBehaviour.MoveSpeed = 5;
            }
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
        {
            Input(GameInputType.Pause);
        }
        //print(GameObject.FindGameObjectsWithTag("Terrain").GetLength(0););
        print(1/Time.deltaTime);
    }

    public void FixedUpdate()
    {
        World.FixedUpdate();
    }

    public void Input(GameInputType inputType)
    {
        StateMachine.GiveInput(inputType);
    }

    public void OnStartLoading(GameStateType previousState, GameInputType intputType)
    {
        SplashScreen.SetActive(true);
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

    public void Print(string s)
    {
        print(s);
    }
}
