using System;
using UnityEngine;

[Serializable]
public struct WeaponData
{
    public string Name;
    
    public int Damage;
    public int MagazineSize;
    public int CurrentAmmo;
    public int TotalAmmo;

    public float FireRate;
    public float Range;

    public GunsType GunsType;

    public Transform BulletSpawnPoint;
    public GameObject GunPrefab;
}
