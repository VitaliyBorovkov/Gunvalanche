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
        GameStateContext.RequestUIMap?.Invoke();

        GameStateContext.InputHandler.SetEnabled(false);

        var hpAmmoController = GameStateContext.GetOrResolveHPAmmoVisibilityController();
        if (hpAmmoController != null)
        {
            hpAmmoController.FadeOut();
        }

        Time.timeScale = 0f;

        GameStateContext.GameOverUI.ShowGameOverScreen(0.8f);
        GameStateContext.SetCursor(true);
        //Debug.Log("GameOverState: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.GameOverUI.HideScreen();

        //Debug.Log("GameOverState: Exited.");
    }

    public void UpdateState()
    {

    }
}
