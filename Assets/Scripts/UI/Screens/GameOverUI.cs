using TMPro;

using UnityEngine;

public class GameOverUI : BaseUIScreen
{
    private static readonly Color GameOverTitleColor = new(0.9019608f, 0f, 0f, 1f);

    [SerializeField] private UIFader fader;
    [SerializeField] private TextMeshProUGUI titleText;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowGameOverScreen(float fadeDuration)
    {
        ShowWithTitle(fadeDuration, "GAME OVER", GameOverTitleColor);
    }

    public void ShowWithTitle(float fadeDuration, string title, Color titleColor)
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.color = titleColor;
        }

        gameObject.SetActive(true);
        if (fader != null)
        {
            fader.PlayFadeIn(fadeDuration);
        }
    }
}
