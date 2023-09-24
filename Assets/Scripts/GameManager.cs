using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Running, Paused, Interrupted }
    public enum PauseState { MainMenu, PauseMenu }

    public static GameState gameState { get; private set; }
    private GameState previousGameState;
    public static PauseState pauseState { get; private set; }
    private PauseState previousPauseState;

    public static bool isPaused { get { return gameState == GameState.Paused; } }

    private static int startCounter = 0, pauseCounter = 0, resumeCounter = 0, mainMenuCounter = 0;

    [Header("UI")]
    [SerializeField] private GameObject[] mainMenuElements, inGameElements, pauseMenuElements;

    void Awake()
    {
        gameState = GameState.Paused;
        pauseState = PauseState.MainMenu;
    }

    //private void Start()
    //{
    //    SceneManager.LoadSceneAsync("Game 1", LoadSceneMode.Additive);   
    //}

    // Update is called once per frame
    void Update()
    {
        // Reset the counters so an object can call one of these
        startCounter = pauseCounter = resumeCounter = mainMenuCounter = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (pauseState != PauseState.MainMenu)
                {
                    ResumeGame();
                    Debug.Log("hh");
                }
                else
                {
                    Debug.Log("vv");
                }
            }

            else
            {
                PauseGame(PauseState.PauseMenu);
            }
        }

        OnGameStateChange();

        if (gameState == GameState.Paused)
        {
            OnPauseStateChange();
        }

        SetUI(gameState, pauseState);

        Debug.Log("CURRENT GAME STATE: " + gameState);
        Debug.Log("CURRENT PAUSE STATE: " + pauseState);
        Debug.Log("TIMESCALE: " + Time.timeScale);
    }

    // GAME STATE // 

    private void OnGameStateChange()
    {
        //Debug.Log("1");

        if (gameState != previousGameState)
        {
            //Debug.Log("2");

            switch (gameState)
            {
                case GameState.Running:
                    OnResume();

                    //Debug.Log("3");

                    break;

                case GameState.Paused:
                    OnPause();

                    //Debug.Log("4");

                    break;

                case GameState.Interrupted:
                    OnInterrupt();

                    //Debug.Log("5");

                    break;

                default:

                    //Debug.Log("6");

                    break;
            }

            previousGameState = gameState;
        }
    }

    private void OnResume()
    {
        SetCursor(false);
        Time.timeScale = 1f;
        Debug.Log("ANNA");
    }

    private void OnPause()
    {
        SetCursor(true);
        Time.timeScale = 0.00001f;
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

                break;

            case GameState.Paused:
                foreach (var element in inGameElements) element.SetActive(false);

                switch (pauseState)
                {
                    case PauseState.MainMenu:
                        foreach (var element in mainMenuElements) element.SetActive(true);
                        foreach (var element in pauseMenuElements) element.SetActive(false);

                        break;

                    case PauseState.PauseMenu:
                        foreach (var element in pauseMenuElements) element.SetActive(true);
                        foreach (var element in mainMenuElements) element.SetActive(false);

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

            startCounter++;
        }
    }

    // Can't call this from Unity events on the UI due to the parameter being an enum
    // Refer to the GoToMainMenu() for this
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

    public void GoToMainMenu()
    {
        if (mainMenuCounter == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //PauseGame(PauseState.MainMenu);

            //mainMenuCounter++;
        }
    }
}