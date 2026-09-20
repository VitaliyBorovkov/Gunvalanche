using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Weapons/Config", order = 0)]
public class WeaponConfig : ScriptableObject
{
    // Pure design-time data. Nothing here is mutated at runtime — see WeaponRuntimeData
    // for the per-instance ammo state that used to live (unsafely) on this asset.
    public WeaponData[] weaponData;
}
