using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Running, Paused, Interrupted }
    public enum PauseState { MainMenu = 0, PauseMenu = 1, DeathMenu = 2 }

    public static GameState gameState { get; private set; }
    public static PauseState pauseState { get; private set; }

    public static bool isPaused { get { return gameState == GameState.Paused; } }

    public UnityAction onPause, onResume;

    [Header("Scene Management")]
    [SerializeField] private int menuSceneIndex = 0;
    [SerializeField] private int gameSceneIndex = 1;

    public int currentLevel { get { return PlayerData.lastPlayedLevel; } }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            gameState = GameState.Paused;
            pauseState = PauseState.MainMenu;

            onPause += () => SetCursor(true);
            onResume += () => SetCursor(false);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }     
    }

    private void SetCursor(bool isActive)
    {
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }

    // PUBLIC FUNCTIONS //
    public void StartGame()
    {
        gameState = GameState.Running;

        SceneManager.LoadScene(gameSceneIndex);
    }

    public void Pause()
    {
        PauseGame(PauseState.PauseMenu);
        onPause.Invoke();
        Time.timeScale = 0f;
    }

    void PauseGame(int pausedState)
    {
        gameState = GameState.Paused;
        pauseState = (PauseState)pausedState;
    }
    void PauseGame(PauseState pausedState)
    {
        gameState = GameState.Paused;
        pauseState = pausedState; 
    }

    public void Resume()
    {
        gameState = GameState.Running;

        onResume.Invoke();

        Time.timeScale = 1f;
    }

    public void ReplayLevel()
    {
        Debug.LogWarning("Not Implemented");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
        PauseGame(PauseState.MainMenu);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}

public static class PlayerData
{
    public static int lastUnlockedLevel { get; set; }
    public static int lastPlayedLevel { get; set; }

    public static int money { get; set; }

    public static int[] unlockedStoreItems { get; set; }

    public static void InitializeData()
    {
        // Implement
    }

    public static void SaveData()
    {
        // Implement
    }
}