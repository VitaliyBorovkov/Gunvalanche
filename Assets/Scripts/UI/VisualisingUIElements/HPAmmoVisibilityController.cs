using UnityEngine;

public class HPAmmoVisibilityController : UIVisibilityBase
{
    [SerializeField] private UIFader hpFader;
    [SerializeField] private UIFader ammoFader;

    [SerializeField] private float fadeDuration = 0.5f;

    public bool HasHpFader => hpFader != null;
    public bool HasAmmoFader => ammoFader != null;

    public void FadeOut()
    {
        FadeOut(fadeDuration);
    }

    public void FadeOut(float duration)
    {
        if (hpFader != null)
        {
            hpFader.PlayFadeOut(duration);
        }

        if (ammoFader != null)
        {
            ammoFader.PlayFadeOut(duration);
        }
    }

    public void FadeIn()
    {
        FadeIn(fadeDuration);
    }

    public void FadeIn(float duration)
    {
        if (hpFader != null)
        {
            hpFader.PlayFadeIn(duration);
        }

        if (ammoFader != null)
        {
            ammoFader.PlayFadeIn(duration);
        }
    }

    //public void SetVisibleImmediate(bool visible)
    //{
    //    float alpha = visible ? 1f : 0f;
    //    if (hpFader != null)
    //    {
    //        hpFader.SetAlpha(alpha);
    //    }

    //    if (ammoFader != null)
    //    {
    //        ammoFader.SetAlpha(alpha);
    //    }
    //}
}
