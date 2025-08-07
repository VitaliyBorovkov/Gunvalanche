using UnityEngine;

public class RocketBulletController : BaseBulletsController
{
    [SerializeField] ExplosionSettings explosionSettings;

    protected override void DespawnEffect()
    {
        if (explosionSettings.explosionEffectPrefub != null)
        {
            Instantiate(explosionSettings.explosionEffectPrefub, transform.position, Quaternion.identity);
        }
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,
            explosionSettings.explosionRadius, explosionSettings.explosionLayerMask);

        foreach (Collider hitCollider in hitColliders)
        {
            HealthController healthController = hitCollider.GetComponentInParent<HealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(explosionSettings.explosionDamage);
                Debug.Log($"RocketBulletController: {gameObject.name} damaged {hitCollider.name} " +
                    $"with {explosionSettings.explosionDamage} damage.");
            }
        }
        DespawnEffect();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == environmentLayer)
        {
            Explode();
        }
    }
}
