using UnityEngine;

public interface IBullet
{
    void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon, BulletsData bullets);
    void ResolveImmediately(ObjectPool pool, WeaponData weapon, Vector3 position, Collider hitCollider);
    void DespawnBullet();
}