public class PauseUI : BaseUIScreen
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
    }

    private void StartFade(float from, float to, float duration, Action onComplete = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration, onComplete));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Action onComplete = null)
    {
        float elapsedTime = 0f;
        pauseMenuCanvasGroup.alpha = from;

        if (Mathf.Approximately(duration, 0f))
        {
            pauseMenuCanvasGroup.alpha = to;
            onComplete?.Invoke();
            yield break;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }
        pauseMenuCanvasGroup.alpha = to;
        onComplete?.Invoke();
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
