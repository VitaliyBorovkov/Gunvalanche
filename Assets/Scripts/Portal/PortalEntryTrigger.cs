using UnityEngine;

public class PortalEntryTrigger : MonoBehaviour
{
    private const string LOG_PREFIX = "PortalEntryTrigger";

    [SerializeField] private GameStateMachine gameStateMachine;
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private int findPortalObjectiveIndex = 1;

    private bool hasEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasEntered || !other.CompareTag("Player"))
        {
            return;
        }

        hasEntered = true;

        if (objectivesManager != null)
        {
            objectivesManager.CompleteObjective(findPortalObjectiveIndex);
        }

        if (gameStateMachine != null)
        {
            gameStateMachine.ToLevelComplete();
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: GameStateMachine is not assigned.");
        }
    }
}
