using UnityEngine;

public sealed class GameplayState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public GameplayState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 1f;
        GameStateContext.InputHandler.SetEnabled(true);
        //GameStateContext.InputManager?.SwitchToGameplayActionMap();
        GameStateContext.RequestGameplayMap?.Invoke();
        GameStateContext.PauseUI.HideScreen();
        GameStateContext.GameOverUI.HideScreen();
        GameStateContext.SetCursor(false);
        Debug.Log("GameplayState: Entered.");
    }

    public void ExitState()
    {
        Debug.Log("GameplayState: Exited.");
    }

    public void UpdateState()
    {

    }
}
