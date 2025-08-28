using UnityEngine;

public class GameOverState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public GameOverState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;
        GameStateContext.InputHandler.SetEnabled(false);
        GameStateContext.GameOverUI.ShowGameOverScreen(0.8f);
        GameStateContext.SetCursor(true);
        Debug.Log("GameOverState: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.GameOverUI.HideScreen();
        Debug.Log("GameOverState: Exited.");
    }

    public void UpdateState()
    {

    }
}
