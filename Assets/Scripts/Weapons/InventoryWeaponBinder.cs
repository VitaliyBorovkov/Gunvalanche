using System.Collections.Generic;

using UnityEngine;

[DisallowMultipleComponent]
public class InventoryWeaponBinder : MonoBehaviour
{
    private const string LOG_PREFIX = "InventoryWeaponBinder";

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory;
    [Header("Switcher")]
    [SerializeField] private PlayerSwitchWeapon playerSwitchWeapon;

    private readonly List<GunsType> ownedOrder = new List<GunsType>();
    private readonly Dictionary<GunsType, int> indexLookup = new Dictionary<GunsType, int>();

    private void Awake()
    {
        if (playerInventory == null)
        {
            playerInventory = GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
            if (playerInventory != null)
            {
                Debug.Log($"{LOG_PREFIX}: PlayerInventory auto-found.");
            }
        }

        if (playerSwitchWeapon == null)
        {
            playerSwitchWeapon = GetComponent<PlayerSwitchWeapon>() ?? FindObjectOfType<PlayerSwitchWeapon>();
            if (playerSwitchWeapon != null)
            {
                Debug.Log($"{LOG_PREFIX}: PlayerSwitchWeapon auto-found.");
            }
        }
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnWeaponAdded -= HandleWeaponAdded;
            playerInventory.OnWeaponAdded += HandleWeaponAdded;
        }

        SyncFromInventory();
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnWeaponAdded -= HandleWeaponAdded;
        }
    }

    private void HandleWeaponAdded(GunsType gunsType, WeaponController weaponController, bool autoEquip)
    {
        if (indexLookup.ContainsKey(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: HandleWeaponAdded -> {gunsType} already tracked.");
            return;
        }

        indexLookup[gunsType] = ownedOrder.Count;
        ownedOrder.Add(gunsType);
        Debug.Log($"{LOG_PREFIX}: HandleWeaponAdded -> {gunsType} at index {indexLookup[gunsType]}.");
    }

    private void SyncFromInventory()
    {
        ownedOrder.Clear();
        indexLookup.Clear();

        if (playerInventory == null)
        {
            return;
        }

        var owned = playerInventory.GetOwnedWeapons();
        for (int i = 0; i < owned.Count; i++)
        {
            ownedOrder.Add(owned[i]);
            indexLookup[owned[i]] = i;
        }

        Debug.Log($"{LOG_PREFIX}: SyncOwnedFromInventory -> found {ownedOrder.Count} owned weapons.");
    }

    public bool EquipWeapon(GunsType gunsType)
    {
        if (playerSwitchWeapon == null)
        {
            return false;
        }

        if (!indexLookup.TryGetValue(gunsType, out var index))
        {
            SyncFromInventory();
            if (!indexLookup.TryGetValue(gunsType, out index))
            {
                Debug.LogWarning($"{LOG_PREFIX}: EquipWeapon({gunsType}) failed — weapon not owned.");
                return false;
            }
        }

        playerSwitchWeapon.SwitchWeaponByIndex(index);
        return true;
    }

    public bool EquipNext()
    {
        if (playerSwitchWeapon == null || ownedOrder.Count == 0)
        {
            return false;
        }

        int next = (playerSwitchWeapon.GetCurrentWeaponIndex() + 1) % ownedOrder.Count;
        playerSwitchWeapon.SwitchWeaponByIndex(next);
        return true;
    }

    public bool EquipPrevious()
    {
        if (playerSwitchWeapon == null || ownedOrder.Count == 0)
        {
            return false;
        }
        int previous = (playerSwitchWeapon.GetCurrentWeaponIndex() - 1 + ownedOrder.Count) % ownedOrder.Count;
        playerSwitchWeapon.SwitchWeaponByIndex(previous);
        return true;
    }

    public IReadOnlyList<GunsType> GetOwnedOrder() => ownedOrder;
}
