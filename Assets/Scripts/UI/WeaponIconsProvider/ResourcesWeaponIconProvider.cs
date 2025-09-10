using System;
using System.Collections.Generic;

using UnityEngine;

public class ResourcesWeaponIconProvider : MonoBehaviour, IWeaponIconProvider
{
    [SerializeField] private string resourcesSubFolder = "WeaponIcons";

    private const string LOG_PREFIX = "ResourcesWeaponIconProvider";

    private Dictionary<string, Sprite> loadedIcons = new Dictionary<string, Sprite>();

    public void GetIcon(string iconKey, Action<Sprite> onLoaded)
    {
        if (string.IsNullOrEmpty(iconKey))
        {
            Debug.LogWarning($"{LOG_PREFIX}: GetIcon called with null/empty key.");
            onLoaded?.Invoke(null);
            return;
        }

        if (loadedIcons.TryGetValue(iconKey, out var cachedIcon))
        {
            onLoaded?.Invoke(cachedIcon);
            return;
        }

        string path = $"{resourcesSubFolder}/{iconKey}";
        Sprite icon = Resources.Load<Sprite>(path);
        if (icon != null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Sprite not found at Resources/{path}");
            onLoaded?.Invoke(null);
            return;
        }

        loadedIcons[iconKey] = icon;
        Debug.Log($"{LOG_PREFIX}: Loaded sprite '{iconKey}' from Resources.");
        onLoaded?.Invoke(icon);
    }

    public void ReleaseIcon(string iconKey)
    {
        if (loadedIcons.ContainsKey(iconKey))
        {
            loadedIcons.Remove(iconKey);
            Debug.Log($"{LOG_PREFIX}: Removed '{iconKey}' from cache (no Addressables release).");
        }
    }
}
