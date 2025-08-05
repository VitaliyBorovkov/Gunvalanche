using UnityEngine;

public class RocketBulletController : BaseBulletsController
{
    protected override void DespawnEffect()
    {
        if (bulletsData.ExplosionEffectPrefab != null)
        {
            Instantiate(bulletsData.ExplosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
