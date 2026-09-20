using System.Text;

using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class ObjectivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ObjectivesManager objectivesManager;

    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color pendingColor = new(0.55f, 0.55f, 0.55f);
    [SerializeField] private Color completedColor = new(0.55f, 0.55f, 0.55f);

    private UIFader fader;

    private void Awake()
    {
        fader = GetComponent<UIFader>();
    }

    private void OnEnable()
    {
        if (objectivesManager == null)
        {
            Debug.LogWarning("ObjectivesUI: objectivesManager is not assigned.");
            return;
        }

        objectivesManager.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (objectivesManager != null)
        {
            objectivesManager.OnChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        var builder = new StringBuilder();

        for (int i = 0; i < objectivesManager.ObjectiveCount; i++)
        {
            ObjectiveData data = objectivesManager.GetObjectiveData(i);
            if (data == null)
            {
                continue;
            }

            ObjectivesManager.ObjectiveState state = objectivesManager.GetState(i);

            string line = data.label;
            if (data.hasCounter)
            {
                (int current, int total) = objectivesManager.GetProgress(i);
                line += $" {current}/{total}";
            }

            Color color = state switch
            {
                ObjectivesManager.ObjectiveState.Active => activeColor,
                ObjectivesManager.ObjectiveState.Completed => completedColor,
                _ => pendingColor
            };

            if (state == ObjectivesManager.ObjectiveState.Completed)
            {
                line = $"<s>{line}</s>";
            }

            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            builder.AppendLine($"<color=#{colorHex}>{line}</color>");
        }

        objectivesText.text = builder.ToString();
    }
}
