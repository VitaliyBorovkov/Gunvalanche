using UnityEngine;

public class WeaponPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private WeaponData weaponData;

    public void ApplyEffect(GameObject player)
    {
        var playerInventory = player.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.AddWeapon(weaponData);
        }
    }
}
