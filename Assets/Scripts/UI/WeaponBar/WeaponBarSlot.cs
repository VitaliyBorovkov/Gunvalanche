using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class WeaponBarSlot : MonoBehaviour
{
    private const string LOG_PREFIX = "WeaponBarSlot";

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image selectionBorder;

    [Header("Active state")]
    [SerializeField] private Vector3 activeScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float scaleAnimSpeed = 10f;

    public WeaponController WeaponController { get; private set; }

    private IWeaponIconProvider iconProvider;
    private string currentIconKey;
    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = Vector3.one;
        if (selectionBorder != null)
            selectionBorder.enabled = false;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleAnimSpeed);
    }

    public void SetEmpty()
    {
        WeaponController = null;
        if (iconImage != null) iconImage.enabled = false;
        if (ammoText != null) ammoText.text = string.Empty;
        if (selectionBorder != null) selectionBorder.enabled = false;
        targetScale = Vector3.one;
    }

    public void Populate(WeaponController wc, string iconKey, IWeaponIconProvider provider)
    {
        WeaponController = wc;
        iconProvider = provider;
        currentIconKey = iconKey;

        wc.OnAmmoChanged += RefreshAmmo;
        RefreshAmmo();

        if (!string.IsNullOrEmpty(iconKey))
            LoadIcon(iconKey);

        SetHighlighted(false);
    }

    public void SetHighlighted(bool active)
    {
        targetScale = active ? activeScale : Vector3.one;

        if (selectionBorder != null)
            selectionBorder.enabled = active;
    }

    private void RefreshAmmo()
    {
        if (ammoText == null || WeaponController == null)
            return;

        int clip = WeaponController.GetCurrentAmmoInClip();
        int total = WeaponController.GetTotalAmmo();
        ammoText.text = $"{clip}/{total}";
    }

    private void LoadIcon(string key)
    {
        if (iconProvider != null)
        {
            iconProvider.GetIcon(key, sprite =>
            {
                if (iconImage != null)
                {
                    iconImage.sprite = sprite;
                    iconImage.enabled = sprite != null;
                }
            });
        }
        else
        {
            var sprite = Resources.Load<Sprite>($"WeaponIcons/{key}");
            if (sprite != null)
            {
                iconImage.sprite = sprite;
                iconImage.enabled = true;
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}: Icon not found for key '{key}'.");
            }
        }
    }

    private void OnDestroy()
    {
        if (WeaponController != null)
            WeaponController.OnAmmoChanged -= RefreshAmmo;

        if (!string.IsNullOrEmpty(currentIconKey) && iconProvider != null)
            iconProvider.ReleaseIcon(currentIconKey);
    }
}
