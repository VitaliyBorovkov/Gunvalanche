using UnityEngine;

public class PlayerHealthController : HealthController
{
    private PlayerHpUI playerHpUI;

    protected override void Start()
    {
        base.Start();

        playerHpUI = FindObjectOfType<PlayerHpUI>();

        if (playerHpUI == null)
        {
            Debug.Log("PlayerHealthController: PlayerHpUI не найден в сцене!");
        }
        else
        {
            playerHpUI.Initialize(entityData.Health);
            UpdateHeadUI();
        }
    }

    protected override void OnDamageTaken(int damage)
    {
        UpdateHeadUI();
    }

    private void UpdateHeadUI()
    {
        if (playerHpUI != null)
        {
            playerHpUI.UpdateUI(healthData.CurrentHealth);
        }
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player has died!");
    }
}
