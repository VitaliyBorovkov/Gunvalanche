using UnityEngine;

public class PortalOpener : MonoBehaviour
{
    private const string LOG_PREFIX = "PortalOpener";

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject portalObject;

    private void Awake()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (waveManager == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: WaveManager is NULL. Portal will not be opened.");
            return;
        }

        waveManager.OnAllWavesCompleted += OpenPortal;

        if (waveManager.AreAllWavesCompleted)
        {
            OpenPortal();
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -= OpenPortal;
        }
    }

    private void OpenPortal()
    {
        if (portalObject == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: PortalObject is NULL. Nothing to open.");
            return;
        }

        portalObject.SetActive(true);
        Debug.Log($"{LOG_PREFIX}: Portal opened.");
    }
}
