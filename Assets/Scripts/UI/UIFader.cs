using System.Collections;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Coroutine currentRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void PlayFadeIn(float duration)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(FadeIn(duration));
    }

    public void PlayFadeOut(float duration)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(FadeOut(duration));
    }

    public void SetAlpha(float value)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        canvasGroup.alpha = Mathf.Clamp01(value);
        canvasGroup.blocksRaycasts = value > 0f;
        canvasGroup.interactable = value >= 1f;
    }

    public IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        currentRoutine = null;
    }

    public IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        currentRoutine = null;
    }
}
