using UnityEngine;
using UnityEngine.UI;

public class WeaponIconManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image targetImage;

    [Header("Provider")]
    [SerializeField] private MonoBehaviour providerComponent;

    [Header("Visibility controller")]
    [SerializeField] private MonoBehaviour visibilityControllerComponent;

    private IWeaponIconProvider provider;
    private UIVisibilityBase visibilityController;

    private const string LOG_PREFIX = "WeaponIconManager";

    private string currentKey;

    private void Awake()
    {
        if (targetImage == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: targetImage is not assigned.");
        }

        if (providerComponent == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: No provider component assigned. " +
                $"Please assign ResourcesWeaponIconProvider or AddressablesWeaponIconProvider.");
        }
        else
        {
            provider = providerComponent as IWeaponIconProvider;
            if (provider == null)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Assigned providerComponent does not implement IWeaponIconProvider.");
            }
        }

        if (visibilityControllerComponent != null)
        {
            visibilityController = visibilityControllerComponent as UIVisibilityBase;
            if (visibilityController == null)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Assigned visibilityControllerComponent does not inherit UIVisibilityBase.");
            }
        }

        if (targetImage != null)
        {
            targetImage.sprite = null;
            targetImage.enabled = false;
        }
    }

    public void ShowIconPrefab(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            ClearIcon();
            return;
        }

        string iconKey = weaponPrefab.name.Replace("(Clone)", "").Trim();
        ShowIconForKey(iconKey);
    }

    public void ShowIconForKey(string iconKey)
    {
        if (string.IsNullOrEmpty(iconKey))
        {
            ClearIcon();
            return;
        }

        if (!string.IsNullOrEmpty(currentKey) && currentKey != iconKey && provider != null)
        {
            provider.ReleaseIcon(currentKey);
            currentKey = null;
        }

        currentKey = iconKey;

        if (provider == null)
        {
            string path = $"WeaponIcons/{iconKey}";
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
            {
                CheckSprite(sprite);
                Debug.Log($"{LOG_PREFIX}: Set icon for '{iconKey}' via Resources.");
            }
            else
            {
                ClearIcon();
                Debug.LogWarning($"{LOG_PREFIX}: provider == null. Using direct Resources fallback for key '" +
                    $"{iconKey}'.");
            }
            return;
        }

        provider.GetIcon(iconKey, (sprite) =>
        {
            if (sprite != null)
            {
                CheckSprite(sprite);
                Debug.Log($"{LOG_PREFIX}: Set icon for '{iconKey}'.");
            }
            else
            {
                ClearIcon();
                Debug.LogWarning($"{LOG_PREFIX}: provider returned null for key '{iconKey}'. Clearing icon.");
            }
        });
    }

    private void CheckSprite(Sprite sprite)
    {
        ApplySprite(sprite);
        visibilityController?.Show();
    }

    private void ApplySprite(Sprite sprite)
    {
        if (targetImage == null)
        {
            return;
        }

        targetImage.sprite = sprite;
        targetImage.enabled = sprite != null;
    }

    public void ClearIcon()
    {
        if (!string.IsNullOrEmpty(currentKey) && provider != null)
        {
            provider.ReleaseIcon(currentKey);
            currentKey = null;
        }
        ApplySprite(null);

        if (visibilityController != null)
        {
            visibilityController.Hide();
        }
        else
        {
            ApplySprite(null);
        }
    }

    public void SetVisibleImmediate(bool visible)
    {
        if (visibilityController != null)
        {
            visibilityController.SetVisibleImmediate(visible);
            if (!visible)
            {
                ApplySprite(null);
            }
        }
        else
        {
            ApplySprite(visible ? targetImage.sprite : null);
        }
    }
}
