using UnityEngine;

public class HPAmmoVisibilityController : UIVisibilityBase
{
    [SerializeField] private UIFader hpFader;
    [SerializeField] private UIFader ammoFader;
    [SerializeField] private UIFader waveFader;
    [SerializeField] private PortalMessageUI portalMessageUI;

    public bool HasHpFader => hpFader != null;
    public bool HasAmmoFader => ammoFader != null;
    public bool HasWaveFader => waveFader != null;

    public void HideImmediate()
    {
        if (hpFader != null)
        {
            hpFader.SetAlpha(0f);
        }

        if (ammoFader != null)
        {
            ammoFader.SetAlpha(0f);
        }

        if (waveFader != null)
        {
            waveFader.SetAlpha(0f);
        }

        if (portalMessageUI != null)
        {
            portalMessageUI.PauseHideImmediate();
        }
    }

    public void ShowImmediate()
    {
        if (hpFader != null)
        {
            hpFader.SetAlpha(1f);
        }

        if (ammoFader != null)
        {
            ammoFader.SetAlpha(1f);
        }

        if (waveFader != null)
        {
            waveFader.SetAlpha(1f);
        }

        if (portalMessageUI != null)
        {
            portalMessageUI.PauseShowImmediate();
        }
    }
}
