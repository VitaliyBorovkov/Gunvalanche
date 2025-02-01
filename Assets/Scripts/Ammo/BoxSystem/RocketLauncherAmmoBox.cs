using UnityEngine;

public class RocketLauncherAmmoBox : AmmoBox
{
    private void Awake()
    {
        ammoType = GunsType.RocketLauncher;
        ammoInBox = 2;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/PistolPickup");
    }

    protected override void AddAmmoToPlayer(GameObject player)
    {
        Debug.Log($"RocketLauncherAmmoBox: Игрок получил {ammoInBox} ракеты для ракетницы.");
    }
}
