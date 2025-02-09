using UnityEngine;

public class PistolAmmoBox : AmmoBox
{
    private void Awake()
    {
        gunsType = GunsType.Pistol;
        ammoInBox = 14;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/PistolAmmoPickup");
    }

    protected override bool AddAmmoToPlayer(GameObject player)
    {
        bool ammoAdded = base.AddAmmoToPlayer(player);
        //Debug.Log($"PistolAmmoBox: Игрок получил {ammoInBox} патронов для пистолета.");
        return ammoAdded;
    }
}
