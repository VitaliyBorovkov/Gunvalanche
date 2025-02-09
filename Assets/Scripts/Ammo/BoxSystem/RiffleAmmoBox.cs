using UnityEngine;

public class RiffleAmmoBox : AmmoBox
{
    private void Awake()
    {
        gunsType = GunsType.Riffle;
        ammoInBox = 30;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/RiffleAmmoPickup");
    }

    protected override bool AddAmmoToPlayer(GameObject player)
    {
        bool ammoAdded = base.AddAmmoToPlayer(player);
        //Debug.Log($"RiffleAmmoBox: Игрок получил {ammoInBox} патронов для автомата.");
        return ammoAdded;
    }
}
