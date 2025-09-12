using System.Collections;

using UnityEngine;

[DisallowMultipleComponent]
public class UIVisibilityBase : MonoBehaviour
{
    [SerializeField] protected UIFader[] uiFaders = new UIFader[0];

    [SerializeField] protected float defaultFadeDuration = 0.5f;
    [SerializeField] protected float defaultAutoHideDelay = 0.5f;

    private Coroutine autoHideRoutine;
    private bool autoHidePaused;

    public virtual void Show(float fadeDuration = -1f, float autoHideDelay = -1f)
    {
        float duration = (fadeDuration < 0f) ? defaultFadeDuration : fadeDuration;
        float hideDelay = (defaultAutoHideDelay < 0f) ? defaultAutoHideDelay : autoHideDelay;

        StopAutoHideRoutine();

        foreach (var fader in uiFaders)
        {
            if (fader != null)
            {
                fader.PlayFadeIn(duration);
            }
        }

        if (hideDelay > 0f)
        {
            autoHideRoutine = StartCoroutine(AutoHideAfterDelay(hideDelay, duration));
        }
    }

    public virtual void Hide(float fadeDuration = -1f)
    {
        float duration = (fadeDuration < 0f) ? defaultFadeDuration : fadeDuration;
        StopAutoHideRoutine();

        Fader(fadeDuration);
    }

    public virtual void SetVisibleImmediate(bool visible)
    {
        StopAutoHideRoutine();
        float alpha = visible ? 1f : 0f;
        foreach (var fader in uiFaders)
        {
            if (fader != null)
            {
                fader.SetAlpha(alpha);
            }
        }
    }

    public virtual void PauseAutoHide()
    {
        autoHidePaused = true;
    }

    public virtual void ResumeAutoHide()
    {
        autoHidePaused = false;
    }

    protected IEnumerator AutoHideAfterDelay(float delay, float fadeDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            if (!autoHidePaused)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        Fader(fadeDuration);
    }

    protected void StopAutoHideRoutine()
    {
        if (autoHideRoutine != null)
        {
            StopCoroutine(autoHideRoutine);
            autoHideRoutine = null;
        }
    }

    public void SetFaders(params UIFader[] newFader)
    {
        uiFaders = newFader ?? new UIFader[0];
    }

    private void Fader(float fadeDuration)
    {
        foreach (var fader in uiFaders)
        {
            if (fader != null)
            {
                fader.PlayFadeOut(fadeDuration);
            }
        }
    }
}
