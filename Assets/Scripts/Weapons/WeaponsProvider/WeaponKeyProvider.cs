using UnityEngine;

[DisallowMultipleComponent]
public class WeaponKeyProvider : MonoBehaviour, IWeaponKeyProvider
{
    public string GetKey(GameObject weaponGameObject)
    {
        if (weaponGameObject == null)
        {
            return null;
        }

        var idenity = weaponGameObject.GetComponent<WeaponIdentity>();
        if (idenity != null && !string.IsNullOrEmpty(idenity.iconKey))
        {
            return idenity.iconKey.Trim();
        }

        return weaponGameObject.name.Replace("(Clone)", "").Trim();
    }
}
