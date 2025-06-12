public interface IWeapon
{
    bool CanShoot();
    float GetFireRate();
    void Shoot();
    WeaponData GetWeaponData();
}