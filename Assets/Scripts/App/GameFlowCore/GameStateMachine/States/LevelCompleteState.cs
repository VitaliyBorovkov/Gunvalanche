using UnityEngine;

// Temporary: reuses GameOverUI's screen (background + buttons) with a different title,
// instead of a dedicated level-complete screen — see DECISIONS.md. Entering the portal
// currently just counts as a win; a real "next level" flow will replace this state's
// EnterState() later without needing to touch anything else in the state machine.
public class LevelCompleteState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public LevelCompleteState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        GameStateContext.RequestUIMap?.Invoke();

        GameStateContext.InputHandler.SetEnabled(false);

        var hpAmmoController = GameStateContext.GetOrResolveHPAmmoVisibilityController();
        if (hpAmmoController != null)
        {
            hpAmmoController.HideImmediate();
        }

        var weaponIconController = GameStateContext.GetOrResolveWeaponIconVisibilityController();
        if (weaponIconController != null)
        {
            weaponIconController.SetVisibleImmediate(false);
        }

        var crosshair = GameStateContext.GetOrResolveCrosshairVisibility();
        if (crosshair != null)
        {
            crosshair.HideImmediate();
        }

        Time.timeScale = 0f;

        GameStateContext.GameOverUI?.ShowWithTitle(0.8f, "LEVEL COMPLETE", Color.white);
        GameStateContext.SetCursor(true);
    }

    public void ExitState()
    {
        GameStateContext.GameOverUI?.HideScreen();
    }

    public void UpdateState()
    {
    }
}
