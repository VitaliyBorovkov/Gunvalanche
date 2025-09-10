using UnityEngine;
using UnityEngine.UI;

public class WeaponIconManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite fallbackSprite;

    [Header("Provider")]
    [SerializeField] private MonoBehaviour providerComponent;

    private IWeaponIconProvider provider;

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
    }

    public void ShowIconPrefab(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            SetSprite(null);
            return;
        }

        string iconKey = weaponPrefab.name.Replace("(Clone)", "").Trim();
        ShowIconForKey(iconKey);
    }

    public void ShowIconForKey(string iconKey)
    {
        if (string.IsNullOrEmpty(iconKey))
        {
            SetSprite(null);
            return;
        }

        if (provider == null)
        {
            string path = $"WeaponIcons/{iconKey}";
            var fallback = Resources.Load<Sprite>(path);
            SetSprite(fallback ?? fallbackSprite);
            Debug.LogWarning($"{LOG_PREFIX}: provider == null. Using direct Resources fallback for key '" +
                $"{iconKey}'.");
            currentKey = iconKey;
            return;
        }

        if (!string.IsNullOrEmpty(currentKey) && currentKey != iconKey)
        {
            provider.ReleaseIcon(currentKey);
            currentKey = null;
        }

        provider.GetIcon(iconKey, (sprite) =>
        {
            SetSprite(sprite ?? fallbackSprite);
            currentKey = iconKey;
            //Debug.Log($"{LOG_PREFIX}: Set icon for '{iconKey}'.");
        });
    }

    private void SetSprite(Sprite sprite)
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
        SetSprite(null);
    }
}
