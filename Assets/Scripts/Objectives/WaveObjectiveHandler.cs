using UnityEngine;

public class WaveObjectiveHandler : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private int objectiveIndex = 0;

    private void OnEnable()
    {
        if (waveManager == null || objectivesManager == null)
        {
            Debug.LogWarning("WaveObjectiveHandler: waveManager or objectivesManager is not assigned.");
            return;
        }

        waveManager.OnEnemyCountChanged += HandleEnemyCountChanged;
        waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnEnemyCountChanged -= HandleEnemyCountChanged;
        waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    private void HandleEnemyCountChanged(int aliveEnemies, int totalEnemiesInWave)
    {
        objectivesManager.SetProgress(objectiveIndex, aliveEnemies, totalEnemiesInWave);
    }

    private void HandleAllWavesCompleted()
    {
        objectivesManager.CompleteObjective(objectiveIndex);
    }
}
