using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private WeaponConfig weaponConfig;

    public List<WeaponData> weapons = new List<WeaponData>();

    private Dictionary<GunsType, int> ammoCount = new Dictionary<GunsType, int>();

    private WeaponData? currentWeapon;

    private void Start()
    {
        if (weaponConfig != null)
        {
            foreach (var weapon in weaponConfig.weaponData)
            {
                weapons.Add(weapon);
                ammoCount[weapon.GunsType] = weapon.TotalAmmo;
            }
        }

        foreach (GunsType type in System.Enum.GetValues(typeof(GunsType)))
        {
            if (!ammoCount.ContainsKey(type))
            {
                ammoCount[type] = 0;
            }

        }

        if (weapons.Count > 0)
        {
            currentWeapon = weapons[0];
            Debug.Log(" PlayerInventory. Current weapon set to: " + currentWeapon?.Name);
        }
        else
        {
            Debug.LogError(" PlayerInventory. No weapons found in weaponConfig!");
        }
    }

    public void AddAmmo(GunsType type, int amount)
    {
        if (ammoCount.ContainsKey(type))
        {
            ammoCount[type] += amount;
            Debug.Log($" PlayerInventory. {type} ammo added. Current ammo: {ammoCount[type]}");
        }
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            ammoCount[newWeapon.GunsType] = newWeapon.TotalAmmo;
            Debug.Log(" PlayerInventory. New weapon added: " + newWeapon.Name);
        }

        if (!currentWeapon.HasValue)
        {
            currentWeapon = newWeapon;
        }
    }

    public WeaponData? GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponData? GetWeaponByType(GunsType type)
    {
        return weapons.Find(w => w.GunsType == type);
    }

    public void SetCurrentWeapon(GunsType type)
    {
        var weapon = GetWeaponByType(type);
        if (weapon.HasValue)
        {
            currentWeapon = weapon.Value;
            Debug.Log(" PlayerInventory. Current weapon set to: " + weapon.Value.Name);
        }
        else
        {
            Debug.Log($" PlayerInventory. Weapon of type {type} not found in inventory.");
        }
    }

    public int GetAmmoCount(GunsType type)
    {
        return ammoCount.ContainsKey(type) ? ammoCount[type] : 0; 
    }

    public void UseAmmo(GunsType type, int amount)
    {
        if (ammoCount.ContainsKey(type))
        {
            ammoCount[type] = Mathf.Max(0, ammoCount[type] - amount);
        }
    }
}
