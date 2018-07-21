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
    private GameStateMachine StateMachine;

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

    private GameStateMachine CreateStateMachine()
    {
        GameStateMachine stateMachine = new GameStateMachine();

        stateMachine.AddEntryState(GameStateType.Loading, false);
        stateMachine.AddState(GameStateType.Playing, true);
        stateMachine.AddState(GameStateType.PausedMenu, false);
        stateMachine.AddState(GameStateType.GameInfoMenu, false);

        //Loading
        stateMachine.AddTransition(GameStateType.Loading, GameStateType.Playing, InputType.FinishedLoading);
        stateMachine.OnEnter(GameStateType.Loading, OnStartLoading);
        stateMachine.OnExit(GameStateType.Loading, OnFinishedLoading);

        //Playing
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, InputType.Pause);

        //Paused Menu
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.Playing, InputType.Pause);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.GameInfoMenu, InputType.OpenGameInfoMenu);
        stateMachine.OnEnter(GameStateType.PausedMenu, OnPause);
        stateMachine.OnExit(GameStateType.PausedMenu, OnResume);

        //GameInfo Menu
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.Playing, InputType.Pause);
        stateMachine.AddTransition(GameStateType.GameInfoMenu, GameStateType.PausedMenu, InputType.CloseGameInfoMenu);
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
            Input(InputType.Pause);
        }

    }

    public void Input(InputType inputType)
    {
        StateMachine.GiveInput(inputType);
    }

    public void OnStartLoading(GameStateType previousState, InputType intputType)
    {
        SplashScreen.SetActive(true);
    }

    public void OnFinishedLoading(InputType intputType, GameStateType nextState)
    {
        SplashScreen.SetActive(false);
    }

    public void OnPause(GameStateType previousState, InputType inputType)
    {
        PausedMenu.SetActive(true);
    }

    public void OnResume(InputType intputType, GameStateType nextState)
    {
        PausedMenu.SetActive(false);
    }

    public void OnOpenGameInfo(GameStateType previousState, InputType inputType)
    {
        GameInfoMenu.SetActive(true);
        RandomSeedText.GetComponent<Text>().text = "Random Seed: "+MyWorldScript.GetRandomSeed().ToString();
        PlayerLocationText.GetComponent<Text>().text = "Player Location: "+MyWorldScript.GetPlayerLocation().ToString();
    }

    public void OnCloseGameInfo(InputType inputType, GameStateType nextState)
    {
        GameInfoMenu.SetActive(false);
    }

    public void ResumePressed()
    {
        Input(InputType.Pause);
    }

    public void GameInfoPressed()
    {
        Input(InputType.OpenGameInfoMenu);
    }

    public void BackPressed()
    {
        Input(InputType.CloseGameInfoMenu);
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
