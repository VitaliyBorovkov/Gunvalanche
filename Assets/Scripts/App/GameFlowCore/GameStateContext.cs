using UnityEngine;

public sealed class GameStateContext
{
    public InputManager InputManager;

    public readonly InputHandler InputHandler;
    public readonly GameOverUI GameOverUI;
    public readonly PauseUI PauseUI;

    public GameStateContext(InputHandler inputHandler, InputManager inputManager, GameOverUI gameOverUI,
        PauseUI pauseUI)
    {
        InputHandler = inputHandler;
        InputManager = inputManager;
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
