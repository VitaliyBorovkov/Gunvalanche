using UnityEngine;

public class GameOverUI : BaseUIScreen
{
    [SerializeField] private UIFader fader;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowGameOverScreen(float fadeDuration)
    {
        gameObject.SetActive(true);
        if (fader != null)
        {
            fader.PlayFadeIn(fadeDuration);
        }
    }
}
