using UnityEngine;

public class RocketChamberPresenter : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private GameObject loadedRocketVisual;

    private int lastAmmo = -1;

    private void Reset()
    {
        weaponController = GetComponentInChildren<WeaponController>();

        if (loadedRocketVisual == null)
        {
            var trans = transform.Find("Design/LoadedRocketVisual");
            if (trans != null)
            {
                loadedRocketVisual = trans.gameObject;
            }
        }
    }

    private void Awake()
    {
        if (weaponController == null)
        {
            weaponController = GetComponentInChildren<WeaponController>();
        }
    }

    private void OnEnable()
    {
        if (weaponController != null)
        {
            weaponController.OnAmmoChanged += HandleAmmoChanged;
        }

        SuncVisualImmediate();
    }

    private void OnDisable()
    {
        if (weaponController != null)
        {
            weaponController.OnAmmoChanged -= HandleAmmoChanged;
        }
    }

    private void HandleAmmoChanged()
    {
        var data = weaponController != null ? weaponController.GetWeaponData() : null;
        int currentAmmo = data != null ? data.CurrentAmmo : 0;

        if (currentAmmo == lastAmmo)
        {
            return;
        }

        lastAmmo = currentAmmo;

        bool shouldShow = currentAmmo > 0;
        if (loadedRocketVisual != null && loadedRocketVisual.activeSelf != shouldShow)
        {
            loadedRocketVisual.SetActive(shouldShow);
        }
    }

    private void SuncVisualImmediate()
    {
        var data = weaponController != null ? weaponController.GetWeaponData() : null;
        lastAmmo = data != null ? data.CurrentAmmo : 0;

        if (loadedRocketVisual != null)
        {
            loadedRocketVisual.SetActive(lastAmmo > 0);
        }
    }
}
