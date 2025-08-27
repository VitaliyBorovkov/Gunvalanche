using UnityEngine;

public sealed class PausedState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public PausedState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;
        GameStateContext.InputHandler.SetEnabled(false);
        GameStateContext.PauseUI.ShowPausePanel();
        GameStateContext.SetCursor(true);
        Debug.Log("PausedState: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.PauseUI?.Hide();
        GameStateContext.SetCursor(false);
        Time.timeScale = 1f;
        Debug.Log("PausedState: Exited.");
    }

    public void UpdateState()
    {

    }
}
