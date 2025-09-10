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
        GameStateContext.RequestUIMap?.Invoke();

        GameStateContext.InputHandler.SetEnabled(false);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var shoot = player.GetComponent<PlayerShoot>();
            shoot?.StopFiring();
            var reload = player.GetComponent<PlayerReload>();
            reload?.CancelReload();
        }

        var hpAmmoController = GameStateContext.GetOrResolveHPAmmoVisibilityController();
        if (hpAmmoController != null)
        {
            hpAmmoController.FadeOut();
        }

        Time.timeScale = 0f;

        GameStateContext.PauseUI.ShowScreen();
        GameStateContext.SetCursor(true);
        //Debug.Log("PausedState: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.PauseUI?.HideScreen();
        GameStateContext.InputHandler.SetEnabled(true);
        GameStateContext.RequestGameplayMap?.Invoke();
        Time.timeScale = 1f;

        var hpAmmoController = GameStateContext.GetOrResolveHPAmmoVisibilityController();
        if (hpAmmoController != null)
        {
            hpAmmoController.FadeIn();
        }

        GameStateContext.SetCursor(false);
        //Debug.Log("PausedState: Exited.");
    }

    public void UpdateState()
    {

    }
}
