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
    public GameObject InfoMenu;
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
    public GameObject PassiveMenu;

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
        stateMachine.AddState(GameStateType.InfoMenu, false);
        stateMachine.AddState(GameStateType.PlayerDead, false);
        stateMachine.AddState(GameStateType.PassiveMenu, false);

        // Loading
        stateMachine.AddTransition(GameStateType.Loading, GameStateType.Playing, GameInputType.FinishedLoading);
        stateMachine.OnExit(GameStateType.Loading, OnFinishedLoading);

        // Playing
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PlayerDead, GameInputType.PlayerDeath);
        stateMachine.AddTransition(GameStateType.Playing, GameStateType.PassiveMenu, GameInputType.PassiveMenu);

        // Paused Menu
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.InfoMenu, GameInputType.OpenInfoMenu);
        stateMachine.OnEnter(GameStateType.PausedMenu, OnPause);
        stateMachine.OnExit(GameStateType.PausedMenu, OnResume);

        // Info Menu
        stateMachine.AddTransition(GameStateType.InfoMenu, GameStateType.Playing, GameInputType.Pause);
        stateMachine.AddTransition(GameStateType.InfoMenu, GameStateType.PausedMenu, GameInputType.CloseInfoMenu);
        stateMachine.OnEnter(GameStateType.InfoMenu, OnOpenInfoMenu);
        stateMachine.OnExit(GameStateType.InfoMenu, OnCloseInfoMenu);

        // Player Dead
        stateMachine.AddTransition(GameStateType.PlayerDead, GameStateType.Playing, GameInputType.PlayerRespawn);
        stateMachine.OnEnter(GameStateType.PlayerDead, OnPlayerDeath);

        // Passive Menu
        stateMachine.AddTransition(GameStateType.PassiveMenu, GameStateType.Playing, GameInputType.PassiveMenu);
        stateMachine.OnEnter(GameStateType.PassiveMenu, OnOpenPassives);
        stateMachine.OnExit(GameStateType.PassiveMenu, OnClosePassives);

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
        if (UnityEngine.Input.GetKeyUp(KeyCode.P))
        {
            Input(GameInputType.PassiveMenu);
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.C))
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
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
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
        Time.timeScale = 0;
        PausedMenu.SetActive(true);
    }

    public void OnResume(GameInputType intputType, GameStateType nextState)
    {
        Time.timeScale = 1;
        PausedMenu.SetActive(false);
    }

    public void OnOpenInfoMenu(GameStateType previousState, GameInputType inputType)
    {
        InfoMenu.SetActive(true);
        RandomSeedText.GetComponent<Text>().text = "Random Seed: "+World.GetRandomSeed().ToString();
        PlayerLocationText.GetComponent<Text>().text = "Player Location: "+World.GetPlayerLocation().ToString();
    }

    public void OnCloseInfoMenu(GameInputType inputType, GameStateType nextState)
    {
        InfoMenu.SetActive(false);
    }

    public void OnOpenPassives(GameStateType previousState, GameInputType inputType)
    {
        PassiveMenu.SetActive(true);
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
        Time.timeScale = 0;
    }

    public void OnClosePassives(GameInputType intputType, GameStateType nextState)
    {
        PassiveMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ButtonPressed_PauseMenu_Resume()
    {
        Input(GameInputType.Pause);
    }

    public void ButtonPressed_PauseMenu_Options()
    {

    }

    public void ButtonPressed_PauseMenu_InfoMenu()
    {
        Input(GameInputType.OpenInfoMenu);
    }

    public void ButtonPressed_PauseMenu_Quit()
    {

    }

    public void ButtonPressed_InfoMenu_Back()
    {
        Input(GameInputType.CloseInfoMenu);
    }

    public void ButtonPressed_MainMenu_Play()
    {

    }

    public void ButtonPressed_MainMenu_Options()
    {

    }

    public void ButtonPressed_MainMenu_Credits()
    {

    }

    public void ButtonPressed_MainMenu_Quit()
    {

    }

    public void UpgradeAttackDamage()
    {
        World.PlayerManager.UpgradeAttackDamage();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeAttackSpeed()
    {
        World.PlayerManager.UpgradeAttackSpeed();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeMoveSpeed()
    {
        World.PlayerManager.UpgradeMoveSpeed();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeSightRadius()
    {
        World.PlayerManager.UpgradeSightRadius();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeProjectileDamage()
    {
        World.PlayerManager.UpgradeProjectileDamage();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeMeleeAoe()
    {
        World.PlayerManager.UpgradeMeleeAoe();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeHealthRegen()
    {
        World.PlayerManager.UpgradeHealthRegen();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
    }

    public void UpgradeStaminaRegen()
    {
        World.PlayerManager.UpgradeStaminaRegen();
        PassiveMenu.GetComponent<PassiveMenuController>().UpdateUI();
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
