using UnityEngine;

public class AmmoPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private GunsType ammoType;
    [SerializeField] private int ammoAmount = 20;

    public void ApplyEffect(GameObject player)
    {
        var playerInventory = player.GetComponent<PlayerInventory>();

        if (playerInventory != null) 
        {
            playerInventory.AddAmmo(ammoType, ammoAmount);
        }
    }
}
