using UnityEngine;

[RequireComponent(typeof(ActionMapRequester))]
[RequireComponent(typeof(PlayerDeathObserver))]
public class GameStateMachine : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;
    [SerializeField] private HPAmmoVisibilityController hpAmmoVisibilityController;

    private GameStateContext gameStateContext;
    private GameStateController gameStateController;

    private ActionMapRequester actionMapRequester;

    private void Awake()
    {
        actionMapRequester = GetComponent<ActionMapRequester>();

        gameStateContext = new GameStateContext(inputManager, inputHandler, gameOverUI, pauseUI, hpAmmoVisibilityController);

        gameStateContext.RequestGameplayMap = actionMapRequester.RequestGameplayMap;
        gameStateContext.RequestUIMap = actionMapRequester.RequestUIMap;
        //Debug.Log("GameStateMachine: wired GameStateContext -> ActionMapRequester.");

        gameStateController = new GameStateController(gameStateContext);
    }

    private void Start()
    {
        if (inputManager != null)
        {
            actionMapRequester.SetInputManager(inputManager);
        }

        gameStateController.ToGameplay();
    }

    private void Update()
    {
        gameStateController.Update();
    }

    public void ToGameplay()
    {
        gameStateController.ToGameplay();
    }

    public void ToGameOver()
    {
        gameStateController.ToGameOver();
    }

    public void ToPause()
    {
        gameStateController.ToPause();
    }

    public void SetCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}