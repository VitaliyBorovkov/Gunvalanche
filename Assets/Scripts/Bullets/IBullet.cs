using UnityEngine;

public interface IBullet
{
    void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon, BulletsData bullets);
    void DespawnBullet();
}