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
        waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnWaveStarted -= HandleWaveStarted;
        waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    private void HandleWaveStarted(int currentWave, int totalWaves, int totalEnemiesInWave)
    {
        waveUI.SetWave(currentWave, totalWaves);
    }

    private void HandleAllWavesCompleted()
    {
        waveUI.Hide();
    }
}
