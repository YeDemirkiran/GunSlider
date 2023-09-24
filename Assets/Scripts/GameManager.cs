using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Running, Paused, Interrupted }
    public enum PauseState { MainMenu, PauseMenu }

    public static GameState gameState { get; private set; }
    private GameState previousGameState;
    public static PauseState pauseState { get; private set; }
    private PauseState previousPauseState;

    public static bool isPaused { get { return gameState == GameState.Paused; } }

    private static int pauseCounter = 0, resumeCounter = 0;

    void Awake()
    {
        gameState = GameState.Paused;
        pauseState = PauseState.MainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
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
    }

    private void SetCursor(bool isActive)
    {
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }

    // PUBLIC FUNCTIONS //
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
}