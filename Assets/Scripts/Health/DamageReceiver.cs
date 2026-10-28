using UnityEngine;

// Sits on the collider GameObject (e.g. AttackZone) and caches a reference to the
// HealthController found on a parent. Lets BaseBulletsController avoid GetComponentInParent
// on every hit — O(1) GetComponent instead of an upward hierarchy traversal.
public class DamageReceiver : MonoBehaviour
{
    private HealthController healthController;

    private void Awake()
    {
        healthController = GetComponentInParent<HealthController>();

        if (healthController == null)
        {
            Debug.LogWarning($"DamageReceiver: No HealthController found in parent of {gameObject.name}.");
        }
    }

    public void TakeDamage(int damage)
    {
        healthController?.TakeDamage(damage);
    }
}
