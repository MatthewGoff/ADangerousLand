using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using ADL.World;
using ADL.Util;
using ADL.Persistence;
using ADL.UI;
using ADL.Combat.Player;

namespace ADL.Core
{
    /// <summary>
    /// Singleton class which controls the application at the highest level.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance of the GameManager
        /// </summary>
        public static GameManager Singleton;
        /// <summary>
        /// Member encapsulates get type call to the state machine
        /// </summary>
        public GameStateType GameState
        {
            get
            {
                return StateMachine.GetCurrentState();
            }
        }

        /// <summary>
        /// Splash screen GameObject
        /// </summary>
        public GameObject SplashScreen;
        /// <summary>
        /// Paused menu GameObject
        /// </summary>
        public GameObject PausedMenu;
        /// <summary>
        /// Info menu GameObject
        /// </summary>
        public GameObject InfoMenu;
        /// <summary>
        /// HUD GameObject
        /// </summary>
        public GameObject HUD;
        /// <summary>
        /// Death screen GameObject
        /// </summary>
        public GameObject DeathScreen;
        /// <summary>
        /// Death background GameObject
        /// </summary>
        public GameObject DeathBackground;
        /// <summary>
        /// GameStatesCanvas GameObject
        /// </summary>
        public GameObject GameStatsCanvas;
        /// <summary>
        /// FPS text GameObject
        /// </summary>
        public GameObject FPSText;
        /// <summary>
        /// UPS text GameObject
        /// </summary>
        public GameObject UPSText;
        /// <summary>
        /// Level up text GameObject
        /// </summary>
        public GameObject LevelUpText;
        /// <summary>
        /// Passive menu GameObject
        /// </summary>
        public GameObject PassivesMenu;
        /// <summary>
        /// Main menu GameObject
        /// </summary>
        public GameObject MainMenu;
        /// <summary>
        /// Player menu GameObject
        /// </summary>
        public GameObject PlayerMenu;
        /// <summary>
        /// New player menu GameObject
        /// </summary>
        public GameObject NewPlayerMenu;
        /// <summary>
        /// World menu GameObject
        /// </summary>
        public GameObject WorldMenu;
        /// <summary>
        /// New world menu GameObject
        /// </summary>
        public GameObject NewWorldMenu;
        /// <summary>
        /// Loading screen GameObject
        /// </summary>
        public GameObject LoadingScreen;
        /// <summary>
        /// Controls menu GameObject
        /// </summary>
        public GameObject ControlsMenu;

        /// <summary>
        /// Audio source GameObject
        /// </summary>
        public GameObject AudioSource;

        /// <summary>
        /// Queue for recent FPS measurements used for averaging
        /// </summary>
        private Queue<float> FPSQueue;
        /// <summary>
        /// Queue for recent UPS measruements used for averaging
        /// </summary>
        private Queue<float> UPSQueue;

        /// <summary>
        /// The main camera used to display gameplay
        /// </summary>
        public GameObject PlayerCamera;
        /// <summary>
        /// The world
        /// </summary>
        public WorldManager World;
        /// <summary>
        /// The FSM used to manage game state
        /// </summary>
        private FiniteStateMachine<GameStateType, GameInputType> StateMachine;

        /// <summary>
        /// Gets called when the GameObject is instantiated
        /// </summary>
        void Awake()
        {
            Singleton = this;
            StateMachine = CreateStateMachine();
            StateMachine.Enter();

            FPSQueue = new Queue<float>();
            UPSQueue = new Queue<float>();

            StartCoroutine("PrintFPS");
            StartCoroutine("PrintUPS");

            Prefabs.LoadPrefabs();
            PlayerPersistenceManager.Initialize();
            WorldPersistenceManager.Initialize();

            PlayerCamera = Instantiate(Prefabs.CAMERA_PREFAB, new Vector3(0, 0, -1), Quaternion.identity);
            PlayerCamera.SetActive(false);
            Time.timeScale = 0f;
            StartCoroutine("Load");
        }

        /// <summary>
        /// Coroutine waits for a period before sending intput to the state machine
        /// </summary>
        /// <returns>
        /// Mandatory return type for a coroutine
        /// </returns>
        private IEnumerator Load()
        {
            yield return new WaitForSecondsRealtime(3f);
            TakeInput(GameInputType.FinishedLoading);
        }

        /// <summary>
        /// Coroutine continuously displays the recent FPS
        /// </summary>
        /// <returns>
        /// Mandatory return type for a coroutine
        /// </returns>
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

        /// <summary>
        /// Coroutine continuously displays the recent UPS
        /// </summary>
        /// <returns>
        /// Mandatory return type for a coroutine
        /// </returns>
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

        /// <summary>
        /// Initialize the FSM for the game
        /// </summary>
        /// <returns>
        /// The FSM for the game
        /// </returns>
        private FiniteStateMachine<GameStateType, GameInputType> CreateStateMachine()
        {
            FiniteStateMachine<GameStateType, GameInputType> stateMachine = new FiniteStateMachine<GameStateType, GameInputType>();

            stateMachine.AddEntryState(GameStateType.Startup);
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
            stateMachine.AddState(GameStateType.LoadingIn);
            stateMachine.AddState(GameStateType.LoadingOut);
            stateMachine.AddState(GameStateType.ControlsMenu);

            // Startup
            stateMachine.AddTransition(GameStateType.Startup, GameStateType.MainMenu, GameInputType.FinishedLoading, false);
            stateMachine.OnEnterState(GameStateType.Startup, delegate (GameStateType state, GameInputType input) { SplashScreen.SetActive(true); });
            stateMachine.OnExitState(GameStateType.Startup, delegate (GameInputType input, GameStateType state) { SplashScreen.SetActive(false); });

            // Playing
            stateMachine.AddTransition(GameStateType.Playing, GameStateType.PausedMenu, GameInputType.Escape, true);
            stateMachine.AddTransition(GameStateType.Playing, GameStateType.PlayerDead, GameInputType.PlayerDeath, false);
            stateMachine.AddTransition(GameStateType.Playing, GameStateType.PassivesMenu, GameInputType.OpenPassivesMenu, false);
            stateMachine.AddTransition(GameStateType.Playing, GameStateType.ControlsMenu, GameInputType.OpenControlsMenu, false);
            stateMachine.OnEnterState(GameStateType.Playing, delegate (GameStateType state, GameInputType input) { Time.timeScale = 1; });
            stateMachine.OnExitState(GameStateType.Playing, delegate (GameInputType input, GameStateType state) { Time.timeScale = 0; });

            // Paused Menu
            stateMachine.AddTransitionToPrevious(GameStateType.PausedMenu, GameInputType.Escape, false);
            stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.InfoMenu, GameInputType.OpenInfoMenu, false);
            stateMachine.AddTransition(GameStateType.PausedMenu, GameStateType.LoadingOut, GameInputType.SaveAndExit, false);
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
            stateMachine.AddTransition(GameStateType.PassivesMenu, GameStateType.Playing, GameInputType.OpenPassivesMenu, false);
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
            stateMachine.AddTransition(GameStateType.WorldMenu, GameStateType.LoadingIn, GameInputType.StartPlay, false);
            stateMachine.OnEnterState(GameStateType.WorldMenu, delegate (GameStateType state, GameInputType input) { WorldMenu.SetActive(true); });
            stateMachine.OnExitState(GameStateType.WorldMenu, delegate (GameInputType input, GameStateType state) { WorldMenu.SetActive(false); });

            // New World Menu
            stateMachine.AddTransition(GameStateType.NewWorldMenu, GameStateType.WorldMenu, GameInputType.Escape, false);
            stateMachine.OnEnterState(GameStateType.NewWorldMenu, delegate (GameStateType state, GameInputType input) { NewWorldMenu.SetActive(true); });
            stateMachine.OnExitState(GameStateType.NewWorldMenu, delegate (GameInputType input, GameStateType state) { NewWorldMenu.SetActive(false); });

            // Loading In 
            stateMachine.AddTransition(GameStateType.LoadingIn, GameStateType.ControlsMenu, GameInputType.WorldLoaded, false);
            stateMachine.OnEnterState(GameStateType.LoadingIn, LoadWorld);
            stateMachine.OnExitState(GameStateType.LoadingIn, delegate (GameInputType input, GameStateType state) { LoadingScreen.SetActive(false); Time.timeScale = 0; });

            // Loading Out
            stateMachine.AddTransition(GameStateType.LoadingOut, GameStateType.MainMenu, GameInputType.WorldUnloaded, false);
            stateMachine.OnEnterState(GameStateType.LoadingOut, SaveAndExit);
            stateMachine.OnExitState(GameStateType.LoadingOut, delegate (GameInputType input, GameStateType state) { LoadingScreen.SetActive(false); });

            // Controls Menu
            stateMachine.AddTransition(GameStateType.ControlsMenu, GameStateType.Playing, GameInputType.OpenControlsMenu, false);
            stateMachine.AddTransition(GameStateType.ControlsMenu, GameStateType.Playing, GameInputType.Escape, false);
            stateMachine.OnEnterState(GameStateType.ControlsMenu, delegate (GameStateType state, GameInputType input) { ControlsMenu.SetActive(true); });
            stateMachine.OnExitState(GameStateType.ControlsMenu, delegate (GameInputType input, GameStateType state) { ControlsMenu.SetActive(false); });

            // Exit
            stateMachine.OnEnterState(GameStateType.Exit, delegate (GameStateType state, GameInputType input) { Application.Quit(); });

            return stateMachine;
        }

        /// <summary>
        /// Gets called once per frame
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TakeInput(GameInputType.Escape);
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                GameStatsCanvas.SetActive(!GameStatsCanvas.activeInHierarchy);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                TakeInput(GameInputType.OpenPassivesMenu);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                TakeInput(GameInputType.OpenControlsMenu);
            }
            if (Input.GetKeyDown(KeyCode.M) && NoFocus())
            {
                AudioSource.GetComponent<AudioSource>().mute = !AudioSource.GetComponent<AudioSource>().mute;
            }
            if (Input.GetKeyDown(KeyCode.C) && GameState == GameStateType.Playing)
            {
                //World.PlayerManager.RecieveExp(190);
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

        /// <summary>
        /// Check whether any UI element has focus for text input
        /// </summary>
        /// <returns>
        /// True if there is no UI element with focus for text input
        /// </returns>
        private bool NoFocus()
        {
            return NewPlayerMenu.GetComponent<NewPlayerMenuController>().NoFocus() && NewWorldMenu.GetComponent<NewWorldMenuController>().NoFocus();
        }

        /// <summary>
        /// Gets called once per physics update
        /// </summary>
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

        /// <summary>
        /// Start preparing the world for play
        /// </summary>
        /// <param name="previousState">
        /// Standard paramater of a state machine delegate
        /// </param>
        /// <param name="input">
        /// Standard paramater of a state machine delegate
        /// </param>
        public void LoadWorld(GameStateType previousState, GameInputType input)
        {
            HUD.SetActive(true);
            LoadingScreen.SetActive(true);
            PlayerManager playerManager = PlayerMenu.GetComponent<PlayerMenuController>().SelectedPlayer;
            World = WorldMenu.GetComponent<WorldMenuController>().SelectedWorld;
            World.Setup(playerManager);
            World.SpawnPlayer();
            PlayerCamera.SetActive(true);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Signal the the player has leveled up
        /// </summary>
        public void LevelUp()
        {
            StartCoroutine("ShowLevelUp");
        }

        /// <summary>
        /// Coroutine which shows and fades the level up text
        /// </summary>
        /// <returns>
        /// Mandatory return type of a coroutine
        /// </returns>
        private IEnumerator ShowLevelUp()
        {
            LevelUpText.GetComponent<Text>().text = "Level " + World.PlayerManager.Level.ToString() + "!";
            LevelUpText.GetComponent<Text>().color = new Color(1, 1, 1, 1);
            LevelUpText.SetActive(true);
            float duration = 2f;
            for (float i = 0f; i < duration; i += Time.deltaTime)
            {
                yield return null;
            }
            duration = 3f;
            Text text = LevelUpText.GetComponent<Text>();
            for (float i = 0f; i < duration; i += Time.deltaTime)
            {
                text.color = new Color(1, 1, 1, 1 - (i / duration));
                yield return null;
            }
            LevelUpText.SetActive(false);
        }

        /// <summary>
        /// Take input to the game's state machine
        /// </summary>
        /// <param name="inputType">
        /// The GameInputType to be input
        /// </param>
        public void TakeInput(GameInputType inputType)
        {
            StateMachine.TakeInput(inputType);
        }

        /// <summary>
        /// Call when the player has died
        /// </summary>
        /// <param name="previousState">
        /// Standard paramater of a state machine delegate
        /// </param>
        /// <param name="inputType">
        /// Standard paramater of a state machine delegate
        /// </param>
        public void OnPlayerDeath(GameStateType previousState, GameInputType inputType)
        {
            Time.timeScale = 1f;
            DeathScreen.SetActive(true);
            StartCoroutine("FadeToBlack");
        }

        /// <summary>
        /// Coroutine which fades the screen to black (after player death) and then enters the appropriate GameInputType
        /// </summary>
        /// <returns>
        /// Mandatory return type of a coroutine
        /// </returns>
        private IEnumerator FadeToBlack()
        {
            DeathBackground.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            for (float i = 0f; i < Configuration.DEATH_DURATION; i += Time.deltaTime)
            {
                DeathBackground.GetComponent<Image>().color = new Color(0, 0, 0, (i / Configuration.DEATH_DURATION));
                yield return null;
            }

            PlayerCamera.SetActive(false);
            World.Sleep();

            DeathPenaltyType deathPenalty = World.PlayerManager.DeathPenalty;
            if (deathPenalty == DeathPenaltyType.Softcore)
            {
                World.SpawnPlayer();
                DeathScreen.SetActive(false);
                PlayerCamera.SetActive(true);
                TakeInput(GameInputType.PlayerRespawn);
            }
            else
            {
                PlayerPersistenceManager.DeletePlayer(World.PlayerManager.PlayerIdentifier);
                DeathScreen.SetActive(false);
                TakeInput(GameInputType.PlayerDeath);
            }
        }

        /// <summary>
        /// Save player and world data and exit to the main menu
        /// </summary>
        /// <param name="state">
        /// Standard paramater for a state machine delegate
        /// </param>
        /// <param name="input">
        /// Standard paramater for a state machine delegate
        /// </param>
        public void SaveAndExit(GameStateType state, GameInputType input)
        {
            LoadingScreen.SetActive(true);
            PlayerCamera.SetActive(false);
            PlayerPersistenceManager.SavePlayer(World.PlayerManager);
            WorldPersistenceManager.SaveWorld(World);
            World.Sleep();
            TakeInput(GameInputType.WorldUnloaded);
        }
    }
}