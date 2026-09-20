using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [SerializeField] private float hideFadeDuration = 0.5f;

    private UIFader fader;

    private int currentWave;
    private int totalWaves;
    private int aliveEnemies;
    private int totalEnemiesInWave;

    private void Awake()
    {
        fader = GetComponent<UIFader>();
    }

    public void SetWave(int currentWave, int totalWaves)
    {
        this.currentWave = currentWave;
        this.totalWaves = totalWaves;
        Refresh();
    }

    public void SetEnemies(int aliveEnemies, int totalEnemiesInWave)
    {
        this.aliveEnemies = aliveEnemies;
        this.totalEnemiesInWave = totalEnemiesInWave;
        Refresh();
    }

    public void Hide()
    {
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        fader.PlayFadeOut(hideFadeDuration);
        yield return new WaitForSecondsRealtime(hideFadeDuration);
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        waveInfoText.text = $"WAVE {currentWave}/{totalWaves}\nENEMIES {aliveEnemies}/{totalEnemiesInWave}";
    }
}
