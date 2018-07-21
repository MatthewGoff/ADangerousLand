using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

    public GameObject SplashScreen;
    public GameObject PausedMenu;
    public GameObject GameInfoMenu;
    public Text RandomSeedText;
    public Text PlayerLocationText;
    
    public static bool MaxMode = false;
    public GameObject PlayerCamera;
    public GameObject MyWorld;
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

    private WorldController MyWorldScript;
    private FiniteStateMachine<GameStateType, GameInputType> StateMachine;

    void Awake()
    {
        Singleton = this;
        StateMachine = CreateStateMachine();
        StateMachine.Enter();

        Prefabs.LoadPrefabs();
        MyWorld = Instantiate(MyWorld, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        MyWorldScript = MyWorld.GetComponent<WorldController>();
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
        MyWorldScript.CreateWorld();
        PlayerCamera.GetComponent<CameraController>().AssignPlayer(MyWorldScript.PlayerController);
        MyWorldScript.PlayerController.AssignCamera(PlayerCamera);
    }

    public void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.M) && PlayerInputEnabled)
        {
            MaxMode = !MaxMode;
            if (MaxMode)
            {
                Configuration.FOG_OUTER_RADIUS = 54;
                Configuration.FOG_INNER_RADIUS = 45;
                PlayerController.MoveSpeed = 50;
            }
            else
            {
                Configuration.FOG_OUTER_RADIUS = 11;
                Configuration.FOG_INNER_RADIUS = 7;
                PlayerController.MoveSpeed = 5;
            }
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
        {
            Input(GameInputType.Pause);
        }

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
        GameInfoMenu.SetActive(true);
        RandomSeedText.GetComponent<Text>().text = "Random Seed: "+MyWorldScript.GetRandomSeed().ToString();
        PlayerLocationText.GetComponent<Text>().text = "Player Location: "+MyWorldScript.GetPlayerLocation().ToString();
    }

    public void OnCloseGameInfo(GameInputType inputType, GameStateType nextState)
    {
        GameInfoMenu.SetActive(false);
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
}
