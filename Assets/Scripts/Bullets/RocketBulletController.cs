using UnityEngine;

public class RocketBulletController : BaseBulletsController
{
    [SerializeField] ExplosionSettings explosionSettings;

    private bool hasExploded = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasExploded = false;
    }

    protected override void DespawnEffect()
    {
        if (explosionSettings != null && explosionSettings.explosionEffectPrefub != null)
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
            if (hitCollider.attachedRigidbody == rigidBody)
            {
                continue;
            }

            HealthController healthController = hitCollider.GetComponentInParent<HealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(explosionSettings.explosionDamage);
                //Debug.Log($"RocketBulletController: {gameObject.name} damaged {hitCollider.name} " +
                //    $"with {explosionSettings.explosionDamage} damage.");
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
        {
            return;
        }

        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == environmentLayer)
        {
            hasExploded = true;
            Explode();
            DespawnBullet();
        }
    }
}
