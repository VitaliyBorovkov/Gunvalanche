using UnityEngine;

public class WeaponIdentity : MonoBehaviour
{
    [Tooltip("Key used to find the icon (Resources/WeaponIcons/{iconKey}.png or Addressables address)")]
    public string iconKey;
}
