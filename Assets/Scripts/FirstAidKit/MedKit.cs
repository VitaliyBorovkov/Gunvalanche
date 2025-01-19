using UnityEngine;

public class MedKit : CollectibleItems
{
    [SerializeField] private int healAmount = 50;

    protected override void Collect(GameObject player)
    {
        PlayerHealthController playerHealthController = player.GetComponent<PlayerHealthController>();

        if (playerHealthController != null)
        {
            if (playerHealthController.GetCurrentHealth() < playerHealthController.GetMaxHealth())
            {
                playerHealthController.Heal(healAmount);
                Debug.Log($"MedKit: {gameObject.name}: ����� {player.name} ������ �� {healAmount} HP.");
                base.Collect(player);
            }
            else
            {
                Debug.Log($"MedKit: {gameObject.name}: � ������ {player.name} ������ �������� � ������� �� ���� ���������.");
            }
        }
        else
        {
            Debug.LogWarning($"MedKit: {player.name} �� ����� PlayerHealthController!", this);
        }
    }
}
