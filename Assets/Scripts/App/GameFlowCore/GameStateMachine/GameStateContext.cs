using System;

using UnityEngine;

public sealed class GameStateContext
{
    public InputManager InputManager;

    public readonly InputHandler InputHandler;
    public readonly GameOverUI GameOverUI;
    public readonly PauseUI PauseUI;

    public Action RequestGameplayMap;
    public Action RequestUIMap;

    public GameStateContext(InputManager inputManager, InputHandler inputHandler, GameOverUI gameOverUI, PauseUI pauseUI)
    {
        InputManager = inputManager;
        InputHandler = inputHandler;
        GameOverUI = gameOverUI;
        PauseUI = pauseUI;
    }

    public void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    public void SetInputManager(InputManager input)
    {
        InputManager = input;
    }
}
