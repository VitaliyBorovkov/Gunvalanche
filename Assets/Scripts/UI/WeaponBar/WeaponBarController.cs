using System.Collections.Generic;

using UnityEngine;

public class WeaponBarController : MonoBehaviour
{
    private const string LOG_PREFIX = "WeaponBarController";

    [Header("References")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private MonoBehaviour iconProviderComponent;

    [Header("Slot order (top = leftmost slot)")]
    [SerializeField] private GunsType[] slotOrder = { GunsType.Pistol, GunsType.Riffle, GunsType.Shotgun, GunsType.RocketLauncher };

    private IWeaponIconProvider iconProvider;
    private PlayerInventory playerInventory;
    private PlayerShoot playerShoot;

    private readonly Dictionary<GunsType, WeaponBarSlot> slots = new Dictionary<GunsType, WeaponBarSlot>();
    private WeaponController activeWeaponController;

    private void Awake()
    {
        iconProvider = iconProviderComponent as IWeaponIconProvider;
        if (iconProviderComponent != null && iconProvider == null)
            Debug.LogWarning($"{LOG_PREFIX}: iconProviderComponent does not implement IWeaponIconProvider.");
    }

    private void OnEnable()
    {
        PlayerSpawner.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        PlayerSpawner.OnPlayerSpawned -= HandlePlayerSpawned;

        if (playerShoot != null)
            playerShoot.OnWeaponChanged -= HandleWeaponChanged;

        if (playerInventory != null)
            playerInventory.OnWeaponAdded -= HandleWeaponAdded;
    }

    private void HandlePlayerSpawned(PlayerShoot ps)
    {
        playerShoot = ps;
        playerShoot.OnWeaponChanged += HandleWeaponChanged;

        CreateAllSlots();

        playerInventory = ps.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.OnWeaponAdded += HandleWeaponAdded;
            SyncExistingWeapons();
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: PlayerInventory not found on player.");
        }

        var currentWeapon = ps.GetCurrentWeapon();
        if (currentWeapon != null)
            HandleWeaponChanged(currentWeapon);
    }

    private void CreateAllSlots()
    {
        if (slotPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: slotPrefab is not assigned.");
            return;
        }

        // Clear any leftover slots from a previous spawn
        foreach (var slot in slots.Values)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        slots.Clear();

        foreach (var gunsType in slotOrder)
        {
            if (slots.ContainsKey(gunsType))
            {
                Debug.LogWarning($"{LOG_PREFIX}: Duplicate GunsType '{gunsType}' in slotOrder — skipped.");
                continue;
            }

            var go = Instantiate(slotPrefab, slotsContainer);
            var slot = go.GetComponent<WeaponBarSlot>();
            if (slot == null)
            {
                Debug.LogError($"{LOG_PREFIX}: slotPrefab does not have WeaponBarSlot component.");
                Destroy(go);
                continue;
            }

            slot.SetEmpty();
            slots[gunsType] = slot;
        }
    }

    private void SyncExistingWeapons()
    {
        foreach (var gunsType in playerInventory.GetOwnedWeapons())
        {
            if (playerInventory.TryGetWeaponController(gunsType, out var wc))
                PopulateSlot(gunsType, wc);
        }
    }

    private void HandleWeaponAdded(GunsType gunsType, WeaponController wc, bool _)
    {
        PopulateSlot(gunsType, wc);
    }

    private void HandleWeaponChanged(IWeapon weapon)
    {
        activeWeaponController = weapon as WeaponController;
        RefreshHighlights();
    }

    private void PopulateSlot(GunsType gunsType, WeaponController wc)
    {
        if (!slots.TryGetValue(gunsType, out var slot))
        {
            Debug.LogWarning($"{LOG_PREFIX}: No slot found for GunsType '{gunsType}'. Check slotOrder.");
            return;
        }

        if (slot.WeaponController == wc)
            return;

        string iconKey = null;
        var identity = wc.GetComponent<WeaponIdentity>();
        if (identity != null)
            iconKey = identity.iconKey;

        slot.Populate(wc, iconKey, iconProvider);
        RefreshHighlights();
    }

    private void RefreshHighlights()
    {
        foreach (var slot in slots.Values)
            slot.SetHighlighted(slot.WeaponController == activeWeaponController);
    }
}
