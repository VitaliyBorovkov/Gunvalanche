using UnityEngine;

public class AmmoUIHandler : MonoBehaviour
{
    [SerializeField] private AmmoUI ammoUI;

    private PlayerShoot playerShoot;
    private IWeapon currentWeapon;

    private void OnEnable()
    {
        PlayerSpawner.OnPlayerSpawned += SetPlayerShoot;
    }

    private void OnDisable()
    {
        PlayerSpawner.OnPlayerSpawned -= SetPlayerShoot;

        CheckerPlayerShootToNullThenUnsubscribe();

        if (currentWeapon is WeaponController weaponController)
        {
            weaponController.OnAmmoChanged -= UpdateAmmoUI;
        }
    }

    public void SetPlayerShoot(PlayerShoot playerShoot)
    {
        CheckerPlayerShootToNullThenUnsubscribe();

        this.playerShoot = playerShoot;
        playerShoot.OnWeaponChanged += HandleWeaponChanged;

        HandleWeaponChanged(playerShoot.GetCurrentWeapon());

        CheckerCurrentWeaponToNullThenUpdateUI();
    }

    private void HandleWeaponChanged(IWeapon weapon)
    {
        if (currentWeapon is WeaponController oldWeaponController)
        {
            oldWeaponController.OnAmmoChanged -= UpdateAmmoUI;
        }

        currentWeapon = weapon;

        if (currentWeapon is WeaponController newWeaponController)
        {
            newWeaponController.OnAmmoChanged += UpdateAmmoUI;
            //Debug.Log("AmmoUIHandler: Subscribed to OnAmmoChanged");
        }
        else
        {
            Debug.LogWarning("AmmoUIHandler: currentWeapon is not WeaponController");
        }

        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("AmmoUIHandler: currentWeapon is null");
            return;
        }

        int currentInClip = currentWeapon.GetCurrentAmmoInClip();
        int totalAmmo = currentWeapon.GetTotalAmmo();

        //Debug.Log($"AmmoUIHandler: UpdateAmmoUI() → {currentInClip} / {totalAmmo}");
        ammoUI.UpdateAmmo(currentInClip, totalAmmo);
    }

    private void CheckerCurrentWeaponToNullThenUpdateUI()
    {
        if (currentWeapon != null)
        {
            UpdateAmmoUI();
        }
    }

    private void CheckerPlayerShootToNullThenUnsubscribe()
    {
        if (playerShoot != null)
        {
            playerShoot.OnWeaponChanged -= HandleWeaponChanged;
        }
    }
}
