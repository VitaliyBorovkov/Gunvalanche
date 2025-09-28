using System.Collections.Generic;

using UnityEngine;

[DisallowMultipleComponent]
public class InventoryWeaponBinder : MonoBehaviour
{
    private const string LOG_PREFIX = "InventoryWeaponBinder";

    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private bool autoSubscribe = true;
    [SerializeField] private bool enableKeyboardShortcuts = false;

    private readonly List<GunsType> ownedOrder = new List<GunsType>();
    private readonly Dictionary<GunsType, int> indexLookup = new Dictionary<GunsType, int>();

    private int currentIndex = -1;

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

        if (playerShoot == null)
        {
            playerShoot = GetComponent<PlayerShoot>() ?? FindObjectOfType<PlayerShoot>();
            if (playerShoot != null)
            {
                Debug.Log($"{LOG_PREFIX}: PlayerShoot auto-found.");
            }
        }
    }

    private void Start()
    {
        if (autoSubscribe)
        {
            SubscribeToInventory();
        }

        SyncOwnedFromInventory();
    }

    private void Update()
    {
        if (!enableKeyboardShortcuts || ownedOrder.Count == 0)
        {
            return;
        }

        for (int i = 0; i < Mathf.Min(9, ownedOrder.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipByIndex(i);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromInventory();
    }

    public void SubscribeToInventory()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: SubscribeToInventory failed — playerInventory is null.");
            return;
        }

        playerInventory.OnWeaponAdded -= HandleWeaponAdded;
        playerInventory.OnWeaponEquipped -= HandleWeaponEquipped;

        playerInventory.OnWeaponAdded += HandleWeaponAdded;
        playerInventory.OnWeaponEquipped += HandleWeaponEquipped;

        Debug.Log($"{LOG_PREFIX}: Subscribed to PlayerInventory events.");
    }

    public void UnsubscribeFromInventory()
    {
        if (playerInventory == null)
        {
            return;
        }

        playerInventory.OnWeaponAdded -= HandleWeaponAdded;
        playerInventory.OnWeaponEquipped -= HandleWeaponEquipped;
        Debug.Log($"{LOG_PREFIX}: Unsubscribed from PlayerInventory events.");
    }

    public bool EquipWeapon(GunsType gunsType)
    {
        if (playerInventory == null || playerShoot == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: EquipWeapon({gunsType}) failed — dependencies not set.");
            return false;
        }

        if (!playerInventory.TryGetWeaponController(gunsType, out var weaponController))
        {
            Debug.LogWarning($"{LOG_PREFIX}: EquipWeapon({gunsType}) failed — weapon not owned.");
            return false;
        }

        playerShoot.SetCurrentWeapon(weaponController as IWeapon);
        UpdateCurrentIndex(gunsType);
        Debug.Log($"{LOG_PREFIX}: EquipWeapon -> {gunsType}.");
        return true;
    }

    public bool EquipNext()
    {
        if (ownedOrder.Count == 0)
        {
            return false;
        }

        int next = (currentIndex + 1) % ownedOrder.Count;
        EquipByIndex(next);
        return true;
    }

    public bool EquipPrevious()
    {
        if (ownedOrder.Count == 0)
        {
            return false;
        }
        int previous = (currentIndex - 1 + ownedOrder.Count) % ownedOrder.Count;
        EquipByIndex(previous);
        return true;
    }

    private void EquipByIndex(int index)
    {
        if (index < 0 || index >= ownedOrder.Count)
        {
            Debug.LogWarning($"{LOG_PREFIX}: EquipByIndex({index}) failed — index out of range.");
            return;
        }

        var weaponType = ownedOrder[index];
        EquipWeapon(weaponType);
    }

    private void HandleWeaponAdded(GunsType gunsType)
    {
        if (!indexLookup.ContainsKey(gunsType))
        {
            indexLookup[gunsType] = ownedOrder.Count;
            ownedOrder.Add(gunsType);
            Debug.Log($"{LOG_PREFIX}: HandleWeaponAdded -> {gunsType}.");

            if (ownedOrder.Count == 1 && currentIndex == -1)
            {
                EquipByIndex(0);
            }
        }
        else
        {
            Debug.Log($"{LOG_PREFIX}: HandleWeaponAdded -> {gunsType} already in ownedOrder.");
        }
    }

    private void HandleWeaponEquipped(IWeapon iweapon)
    {
        if (iweapon is WeaponController weaponController)
        {
            foreach (var pair in indexLookup)
            {
                if (playerInventory.TryGetWeaponController(pair.Key, out var wc) && wc == weaponController)
                {
                    currentIndex = pair.Value;
                    Debug.Log($"{LOG_PREFIX}: HandleWeaponEquiped -> {pair.Key}.");
                    return;
                }
            }

            SyncOwnedFromInventory();

            for (int i = 0; i < ownedOrder.Count; i++)
            {
                var gunsType = ownedOrder[i];
                if (playerInventory.TryGetWeaponController(gunsType, out var wc) && wc == weaponController)
                {
                    currentIndex = i;
                    Debug.Log($"{LOG_PREFIX}: HandleWeaponEquiped after sync -> {gunsType}.");
                    return;
                }
            }
        }

        Debug.Log($"{LOG_PREFIX}: HandleWeaponEquipped -> could not match equipped IWeapon to known owned entries.");
    }

    private void SyncOwnedFromInventory()
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

        if (playerShoot != null)
        {
            var current = playerShoot.GetCurrentWeapon();
            if (current is WeaponController weaponController)
            {
                for (int i = 0; i < ownedOrder.Count; i++)
                {
                    var gunsType = ownedOrder[i];
                    if (playerInventory.TryGetWeaponController(gunsType, out var wc) && wc == weaponController)
                    {
                        currentIndex = i;
                        Debug.Log($"{LOG_PREFIX}: SyncOwnedFromInventory -> current weapon is {gunsType} at index {i}.");
                        return;
                    }
                }
            }
        }

        Debug.Log($"{LOG_PREFIX}: SyncOwnedFromInventory -> found {ownedOrder.Count} owned weapons.");
    }

    private void UpdateCurrentIndex(GunsType gunsType)
    {
        if (indexLookup.TryGetValue(gunsType, out var index))
        {
            currentIndex = index;
        }
    }
}
