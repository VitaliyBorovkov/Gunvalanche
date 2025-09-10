using System;

[Serializable]
public class WeaponRuntimeData
{
    public int InitialCurrentAmmo { get; private set; }
    public int InitialTotalAmmo { get; private set; }

    public WeaponRuntimeData(WeaponData weaponData)
    {
        InitialCurrentAmmo = weaponData.CurrentAmmo;
        InitialTotalAmmo = weaponData.TotalAmmo;
    }
}
