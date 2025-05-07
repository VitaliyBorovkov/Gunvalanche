using UnityEngine;

public class RocketLauncherAmmoBox : AmmoBox
{
    private void Awake()
    {
        gunsType = GunsType.RocketLauncher;
        ammoInBox = 2;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/RocketsPickup");
    }

    protected override bool AddAmmoToPlayer(GameObject player)
    {
        bool ammoAdded = base.AddAmmoToPlayer(player);
        //Debug.Log($"RocketLauncherAmmoBox: Игрок получил {ammoInBox} ракеты для ракетницы.");
        return ammoAdded;
    }
}
