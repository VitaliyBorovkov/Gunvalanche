using UnityEngine;

public class PortalMessageUIHandler : MonoBehaviour
{
    private const string PORTAL_OPENED_MESSAGE = "PORTAL OPENED - FIND IT";

    [SerializeField] private PortalMessageUI portalMessageUI;
    [SerializeField] private WaveManager waveManager;

    private void OnEnable()
    {
        if (waveManager == null)
        {
            Debug.LogWarning("PortalMessageUIHandler: waveManager is not assigned.");
            return;
        }

        waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;

        if (waveManager.AreAllWavesCompleted)
        {
            HandleAllWavesCompleted();
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }
    }

    private void HandleAllWavesCompleted()
    {
        portalMessageUI.Show(PORTAL_OPENED_MESSAGE);
    }
}
