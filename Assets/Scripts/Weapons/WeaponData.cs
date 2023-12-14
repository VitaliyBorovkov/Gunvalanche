using System;
using UnityEngine;

[Serializable]
public struct WeaponData
{
    public string Name;
    
    public int Damage;
    public int MagazineSize;
    public int CurrentAmmo;

    public float FireRate;
    public float Range;
    public float ReloadTime;

    [HideInInspector]public bool isReloading;

    public GunsType GunsType;

    public Transform BulletSpawnPoint;
    public GameObject GunPrefab;
}
