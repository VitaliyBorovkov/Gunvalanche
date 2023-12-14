using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Weapons/Config", order = 0)]
public class WeaponConfig : ScriptableObject
{
    public WeaponData[] weaponData;

    //public WeaponData GetWeaponConfig(string key)
    //{
    //    return weaponData.FirstOrDefault(weapon => weapon.Name == key);
    //}
}
