using UnityEngine;

public class PlayerAutoReload : MonoBehaviour, IAutoReload
{
    private PlayerShoot playerShoot;
    private PlayerReload playerReload;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerReload = GetComponent<PlayerReload>();
    }

    public void TryAutoReload()
    {
        if (playerShoot == null || playerShoot.GetCurrentWeapon() == null)
        {
            Debug.LogWarning("PlayerAutoReload: No active weapons!");
            return;
        }

        var weapon = playerShoot.GetCurrentWeapon();
        if (weapon == null)
        {
            Debug.LogWarning("PlayerAutoReload: No current weapon found.");
            return;
        }

        WeaponData weaponData = weapon.GetWeaponData();
        if (weaponData == null)
        {
            Debug.LogWarning("PlayerAutoReload: No weapon data found.");
            return;
        }

        if (weaponData.CurrentAmmo <= 0 && AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType) > 0 &&
            !playerReload.IsReloading())
        {
            Debug.Log("PlayerAutoReload: Auto reload triggered.");
            playerReload.Reload();
        }
    }
}
