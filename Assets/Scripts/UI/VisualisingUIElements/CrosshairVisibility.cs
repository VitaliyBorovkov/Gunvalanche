using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class CrosshairVisibility : MonoBehaviour
{
    private UIFader fader;

    private void Awake()
    {
        fader = GetComponent<UIFader>();
    }

    public void HideImmediate()
    {
        fader.SetAlpha(0f);
    }

    public void ShowImmediate()
    {
        fader.SetAlpha(1f);
    }
}
