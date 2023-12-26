using System;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    [SerializeField] private WeaponData[] weaponData;
    [SerializeField] private float reloadTime = 2f;

    private bool IsReloading = false;

    public void Reload()
    {
        if (!IsReloading)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        if (weaponData[0].CurrentAmmo < weaponData[0].MagazineSize)
        {
            IsReloading = true;
            Debug.Log("Reloading...");

            Invoke("FinishReload", reloadTime);
        }
    }

    private void FinishReload()
    {
        if (weaponData != null)
        {
            weaponData[0].CurrentAmmo = weaponData[0].MagazineSize;
            IsReloading = false;
            Debug.Log("Reload complete!");
        }
    }
}
