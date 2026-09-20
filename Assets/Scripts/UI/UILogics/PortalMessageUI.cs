using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class PortalMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float fadeDuration = 0.5f;

    private UIFader fader;
    private bool isShown;

    private void Awake()
    {
        fader = GetComponent<UIFader>();
        fader.SetAlpha(0f);
    }

    public void Show(string message)
    {
        messageText.text = message;
        isShown = true;
        fader.PlayFadeIn(fadeDuration);
    }

    public void PauseHideImmediate()
    {
        fader.SetAlpha(0f);
    }

    public void PauseShowImmediate()
    {
        if (!isShown)
        {
            return;
        }

        fader.SetAlpha(1f);
    }
}
