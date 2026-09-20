using UnityEngine;

/// <summary>
/// Per-instance, per-session mutable ammo state for a single WeaponController.
/// Deliberately NOT stored on the WeaponConfig ScriptableObject asset (WeaponData) —
/// each weapon instance owns its own copy, seeded once from WeaponData's design-time
/// defaults, so ammo can never leak between two holders of the same weapon config or
/// survive a scene reload. See DECISIONS.md, "Патроны оружия вынесены из ScriptableObject...".
/// </summary>
public class WeaponRuntimeData
{
    public int CurrentAmmo { get; private set; }

    public WeaponRuntimeData(int initialCurrentAmmo)
    {
        CurrentAmmo = Mathf.Max(0, initialCurrentAmmo);
    }

    public void Decrement()
    {
        CurrentAmmo = Mathf.Max(0, CurrentAmmo - 1);
    }

    public void Add(int amount, int magazineSize)
    {
        CurrentAmmo = Mathf.Clamp(CurrentAmmo + amount, 0, magazineSize);
    }
}
