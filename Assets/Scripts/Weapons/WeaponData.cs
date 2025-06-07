using System;

using UnityEngine;

[Serializable]
public class WeaponData
{
    public string Name;

    public int Damage;
    public int TotalAmmo;
    public int CurrentAmmo;
    public int MagazineSize;

    public float Range;
    public float FireRate;

    public GunsType GunsType;
    public BulletsType BulletsType;

    public GameObject GunPrefab;
    public GameObject MuzzleFlashPrefab;
    //public Transform BulletSpawnPoint;
}
