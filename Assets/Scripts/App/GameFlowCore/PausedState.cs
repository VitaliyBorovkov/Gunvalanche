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

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var shoot = player.GetComponent<PlayerShoot>();
            shoot?.StopFiring();
            var reload = player.GetComponent<PlayerReload>();
            reload?.CancelReload();
        }

        GameStateContext.RequestUIMap?.Invoke();
        GameStateContext.PauseUI.ShowScreen();
        GameStateContext.SetCursor(true);
        Debug.Log("PausedState: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.PauseUI?.HideScreen();
        GameStateContext.InputHandler.SetEnabled(true);
        GameStateContext.RequestGameplayMap?.Invoke();
        //GameStateContext.InputManager?.SwitchToGameplayActionMap();
        GameStateContext.SetCursor(false);
        Time.timeScale = 1f;
        Debug.Log("PausedState: Exited.");
    }

    public void UpdateState()
    {

    }
}
