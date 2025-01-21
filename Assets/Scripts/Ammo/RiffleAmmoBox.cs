using UnityEngine;

public class RiffleAmmoBox : AmmoBox
{
    private void Awake()
    {
        ammoType = GunsType.Riffle;
        ammoInBox = 30;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/RifflePickup");
    }

    protected override void AddAmmoToPlayer(GameObject player)
    {
        Debug.Log($"RiffleAmmoBox: Игрок получил {ammoInBox} патронов для автомата.");
    }
}
