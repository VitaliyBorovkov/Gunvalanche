using UnityEngine;

public class WaveUIHandler : MonoBehaviour
{
    [SerializeField] private WaveUI waveUI;
    [SerializeField] private WaveManager waveManager;

    private void OnEnable()
    {
        if (waveManager == null)
        {
            Debug.LogWarning("WaveUIHandler: waveManager is not assigned.");
            return;
        }

        waveManager.OnWaveStarted += HandleWaveStarted;
        waveManager.OnEnemyCountChanged += HandleEnemyCountChanged;
        waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnWaveStarted -= HandleWaveStarted;
        waveManager.OnEnemyCountChanged -= HandleEnemyCountChanged;
        waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    private void HandleWaveStarted(int currentWave, int totalWaves, int totalEnemiesInWave)
    {
        waveUI.SetWave(currentWave, totalWaves);
        waveUI.SetEnemies(0, totalEnemiesInWave);
    }

    private void HandleEnemyCountChanged(int aliveEnemies, int totalEnemiesInWave)
    {
        waveUI.SetEnemies(aliveEnemies, totalEnemiesInWave);
    }

    private void HandleAllWavesCompleted()
    {
        waveUI.Hide();
    }
}
