using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSettings", menuName = "Weapons/ExplosionSettings")]
public class ExplosionSettings : ScriptableObject
{
    public GameObject explosionEffectPrefub;
    public float explosionRadius = 5f;
    public int explosionDamage = 100;
    public LayerMask explosionLayerMask;
}
