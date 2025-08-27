using UnityEngine;

public sealed class GameStateContext
{
    public readonly InputHandler InputHandler;
    public readonly GameOverUI GameOverUI;
    //public readonly PauseUI PauseUI;

    public GameStateContext(InputHandler inputHandler, GameOverUI gameOverUI/*, PauseUI pauseUI*/)
    {
        InputHandler = inputHandler;
        GameOverUI = gameOverUI;
        //PauseUI = pauseUI;
    }

    public void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
