using System.Collections;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    private PlayerShoot playerShoot;

    private bool isReloading = false;

    public void Reload()
    {
        playerShoot = GetComponent<PlayerShoot>();
        
        if (isReloading)
        {
            Debug.Log(" PlayerReload. Already reloading");
            return;
        }
       
        if (playerShoot.weaponData[0].CurrentAmmo == playerShoot.weaponData[0].MagazineSize)
        {
            Debug.Log(" PlayerReload. Magazine is full");
            return;
        }

        if (playerShoot.weaponData[0].TotalAmmo <= 0)
        {
            Debug.Log(" PlayerReload. No ammo left to reload");
            return;
        }

        Debug.Log(" PlayerReload. Starting reload");
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log(" PlayerReload. Reloading...");

        yield return new WaitForSeconds(2f);

        int ammoNeeded = playerShoot.weaponData[0].MagazineSize - playerShoot.weaponData[0].CurrentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, playerShoot.weaponData[0].TotalAmmo);

        playerShoot.weaponData[0].CurrentAmmo += ammoToReload;
        playerShoot.weaponData[0].TotalAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log(" PlayerReload. Reloaded: Current Ammo = " + playerShoot.weaponData[0].CurrentAmmo + ", Total Ammo = " + playerShoot.weaponData[0].TotalAmmo);
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
