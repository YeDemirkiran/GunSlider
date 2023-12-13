using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Running, Paused, Interrupted }
    public enum PauseState { MainMenu = 0, PauseMenu = 1, DeathMenu = 2 }

    public static GameState gameState { get; private set; }
    private GameState previousGameState;
    public static PauseState pauseState { get; private set; }
    private PauseState previousPauseState;

    public static bool isPaused { get { return gameState == GameState.Paused; } }

    private static int startCounter = 0, pauseCounter = 0, resumeCounter = 0, mainMenuCounter = 0;

    [Header("Scene Management")]
    [SerializeField] private int menuSceneIndex = 0;
    [SerializeField] private int gameSceneIndex = 1;

    public int currentLevel { get; private set; } = 44;

    [Header("UI")]
    [SerializeField] private GameObject[] mainMenuElements;
    [SerializeField] private GameObject[] inGameElements, pauseMenuElements, deathMenuElements;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            gameState = GameState.Paused;
            pauseState = PauseState.MainMenu;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }     
    }

    void Update()
    {
        // Reset the counters so an object can call one of these
        startCounter = pauseCounter = resumeCounter = mainMenuCounter = 0;

        OnGameStateChange();

        if (gameState == GameState.Paused)
        {
            OnPauseStateChange();
        }

        SetUI(gameState, pauseState);
    }

    // GAME STATE // 

    private void OnGameStateChange()
    {
        if (gameState != previousGameState)
        {
            switch (gameState)
            {
                case GameState.Running:
                    OnResume();
                    break;

                case GameState.Paused:
                    OnPause();
                    break;

                case GameState.Interrupted:
                    OnInterrupt();
                    break;

                default:
                    break;
            }

            previousGameState = gameState;
        }
    }

    private void OnResume()
    {
        SetCursor(false);
        Time.timeScale = 1f;
    }

    private void OnPause()
    {
        SetCursor(true);

        switch (pauseState)
        {
            case PauseState.MainMenu:
                Time.timeScale = 1f;
                break;
            case PauseState.PauseMenu:
                Time.timeScale = 0.00001f;
                break;
            case PauseState.DeathMenu:
                Time.timeScale = 0.00001f;
                break;
        }
        
    }

    private void OnInterrupt()
    {
        SetCursor(true);

        Time.timeScale = 0.00001f;
    }


    // PAUSE STATE //

    private void OnPauseStateChange()
    {
        if (pauseState != previousPauseState)
        {
            switch (pauseState)
            {
                case PauseState.MainMenu:

                    break;

                case PauseState.PauseMenu:

                    break;

                default:
                    break;
            }
        }

        previousPauseState = pauseState;
    }

    private void SetCursor(bool isActive)
    {
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }

    private void SetUI(GameState gameState, PauseState pauseState)
    {
        switch (gameState)
        {
            case GameState.Running:
                foreach (var element in inGameElements) element.SetActive(true);

                foreach (var element in pauseMenuElements) element.SetActive(false);
                foreach (var element in mainMenuElements) element.SetActive(false);
                foreach (var element in deathMenuElements) element.SetActive(false);

                break;

            case GameState.Paused:
                foreach (var element in inGameElements) element.SetActive(false);

                switch (pauseState)
                {
                    case PauseState.MainMenu:
                        foreach (var element in mainMenuElements) element.SetActive(true);

                        foreach (var element in pauseMenuElements) element.SetActive(false);
                        foreach (var element in deathMenuElements) element.SetActive(false);

                        break;

                    case PauseState.PauseMenu:
                        foreach (var element in pauseMenuElements) element.SetActive(true);

                        foreach (var element in mainMenuElements) element.SetActive(false);
                        foreach (var element in deathMenuElements) element.SetActive(false);

                        break;

                    case PauseState.DeathMenu:
                        foreach (var element in deathMenuElements) element.SetActive(true);

                        foreach (var element in mainMenuElements) element.SetActive(false);
                        foreach (var element in pauseMenuElements) element.SetActive(false);

                        break;
                }

                break;
        }
    }

    // PUBLIC FUNCTIONS //
    public void StartGame()
    {
        if (startCounter == 0)
        {
            gameState = GameState.Running;

            SceneManager.LoadScene(gameSceneIndex);

            startCounter++;
        }
    }

    // Can't call this from Unity events on the UI due to the parameter being an enum
    // Refer to the GoToMainMenu() if you want to go to the main menu
    public void PauseGame()
    {
        if (isPaused)
        {
            if (pauseState != PauseState.MainMenu)
            {
                ResumeGame();
            }
        }

        else
        {
            PauseGame(PauseState.PauseMenu);
        }
    }

    public void PauseGame(int pausedState)
    {
        if (pauseCounter == 0)
        {
            gameState = GameState.Paused;
            pauseState = (PauseState)pausedState;

            pauseCounter++;
        }
    }

    public void PauseGame(PauseState pausedState)
    {
        if (pauseCounter == 0)
        {
            gameState = GameState.Paused;
            pauseState = pausedState;

            pauseCounter++;
        }
    }

    public void ResumeGame()
    {
        if (resumeCounter == 0)
        {
            gameState = GameState.Running;

            resumeCounter++;
        }
    }

    public void ReplayLevel()
    {
        Debug.LogWarning("Not Implemented");
    }

    public void GoToMainMenu()
    {
        if (mainMenuCounter == 0)
        {
            SceneManager.LoadScene(menuSceneIndex);
            PauseGame(PauseState.MainMenu);

            //mainMenuCounter++;
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}