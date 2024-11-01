using UnityEngine;

public class HealthPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private int healthAmount = 20;

    public void ApplyEffect(GameObject player)
    {
        var healthController = player.GetComponent<HealthController>();

        if (healthController != null)
        {
            healthController.AddHealth(healthAmount);
        }
    }
}
