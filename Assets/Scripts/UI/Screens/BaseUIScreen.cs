using UnityEngine;

public abstract class BaseUIScreen : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected GameObject screenRoot;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (screenRoot == null)
        {
            screenRoot = gameObject;
        }

        HideScreen();
    }

    public virtual void ShowScreen()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    public virtual void HideScreen()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        gameObject.SetActive(false);
    }
}
