using UnityEngine;

public class Pauser : MonoBehaviour
{
    [SerializeField] private GameStateMachine gameStateMachine;

    private void Awake()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = FindObjectOfType<GameStateMachine>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0.5f)
            {
                gameStateMachine?.ToPause();
                Debug.Log("Pauser: Paused.");
            }
            else
            {
                gameStateMachine?.ToGameplay();
            }
        }
    }
}
