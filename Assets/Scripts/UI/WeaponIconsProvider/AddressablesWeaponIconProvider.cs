using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesWeaponIconProvider : MonoBehaviour, IWeaponIconProvider
{
    private const string LOG_PREFIX = "AddressablesWeaponIconProvider";

    private Dictionary<string, Sprite> loadedIcons = new Dictionary<string, Sprite>();
    private Dictionary<string, AsyncOperationHandle<Sprite>> handles = new Dictionary<string, AsyncOperationHandle<Sprite>>();

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

        var handle = Addressables.LoadAssetAsync<Sprite>(iconKey);
        handle.Completed += (asyncOperation) =>
        {
            if (asyncOperation.Status == AsyncOperationStatus.Succeeded && asyncOperation.Result != null)
            {
                loadedIcons[iconKey] = asyncOperation.Result;
                handles[iconKey] = asyncOperation;
                Debug.Log($"{LOG_PREFIX}: Loaded sprite '{iconKey}' from Addressables.");
                onLoaded?.Invoke(asyncOperation.Result);
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}: Failed to load sprite '{iconKey}' from Addressables.");
                onLoaded?.Invoke(null);
            }
        };
    }

    public void ReleaseIcon(string iconKey)
    {
        if (handles.TryGetValue(iconKey, out var handle))
        {
            Addressables.Release(handle);
            handles.Remove(iconKey);
            loadedIcons.Remove(iconKey);
            Debug.Log($"{LOG_PREFIX}: Released Addressables asset '{iconKey}'.");
        }
        else if (loadedIcons.ContainsKey(iconKey))
        {
            loadedIcons.Remove(iconKey);
            Debug.Log($"{LOG_PREFIX}: Removed '{iconKey}' from cache (no handle).");
        }
    }
}
