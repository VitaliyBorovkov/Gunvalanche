using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[DisallowMultipleComponent]
public class WeaponUnlockManager : MonoBehaviour
{
    private const string LOG_PREFIX = "WeaponUnlockManager: ";

    [Header("Level settings")]
    [SerializeField] private int currentLevel = 0;

    [Header("Unlock rules")]
    [SerializeField] private List<WeaponUnlockEntry> unlockRules = new List<WeaponUnlockEntry>();

    private Dictionary<GunsType, int> unlockLevelLookup;
    private Dictionary<GunsType, bool> spawnAllowedLookup;

    private HashSet<GunsType> unlockedWeapons = new HashSet<GunsType>();

    public event Action<int> OnLevelChanged;

    private void Awake()
    {
        RebuildLookup();
    }

    private void OnValidate()
    {
        RebuildLookup();
    }

    private void RebuildLookup()
    {
        unlockLevelLookup = new Dictionary<GunsType, int>();
        spawnAllowedLookup = new Dictionary<GunsType, bool>();

        if (unlockRules == null || unlockRules.Count == 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: unlockRules is null in inspector.");
            return;
        }

        foreach (var entry in unlockRules)
        {
            if (unlockLevelLookup.ContainsKey(entry.gunsType))
            {
                Debug.LogWarning($"{LOG_PREFIX}: Duplicate unlock rule for {entry.gunsType} found in inspector. Using first occurrence.");
                continue;
            }

            unlockLevelLookup[entry.gunsType] = Math.Max(0, entry.levelToUnlock);
            spawnAllowedLookup[entry.gunsType] = entry.spawnAllowed;
        }
    }

    public bool IsUnlocked(GunsType gunsType)
    {
        bool unlockByConfig = unlockLevelLookup != null &&
            unlockLevelLookup.TryGetValue(gunsType, out var level) && currentLevel >= level;

        bool unlock = unlockedWeapons.Contains(gunsType);

        bool allowed = unlockByConfig || unlock;

        string configLevel = unlockLevelLookup != null &&
            unlockLevelLookup.TryGetValue(gunsType, out var lvl) ? lvl.ToString() : "N/A";
        Debug.Log($"{LOG_PREFIX}: IsUnlocked({gunsType}) -> {allowed} for level {currentLevel} (configLevel={configLevel}).");
        return allowed;
    }

    public bool IsAllowedToSpawn(GunsType gunsType)
    {
        bool spawnAllowed = spawnAllowedLookup != null &&
            spawnAllowedLookup.TryGetValue(gunsType, out var canSpawn) && canSpawn;

        bool unlocked = IsUnlocked(gunsType);

        bool allowed = spawnAllowed && unlocked;
        Debug.Log($"{LOG_PREFIX}: IsAllowedToSpawn({gunsType}) -> {allowed} (spawnAllowed={spawnAllowed}, unlocked={unlocked}).");
        return allowed;
    }

    public void UnlockNow(GunsType gunsType)
    {
        if (unlockedWeapons.Add(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: UnlockNow -> {gunsType} (force unlocked).");
        }
        else
        {
            Debug.Log($"{LOG_PREFIX}: UnlockNow -> {gunsType} was already unlocked.");
        }
    }

    public void SetLevel(int newLevel)
    {
        if (newLevel < 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: SetLevel({newLevel}) ignored (must be >= 0).");
            return;
        }

        if (newLevel == currentLevel)
        {
            Debug.Log($"{LOG_PREFIX}: SetLevel -> {newLevel} (no change).");
            return;
        }

        currentLevel = newLevel;
        Debug.Log($"{LOG_PREFIX}: SetLevel -> {currentLevel}.");

        OnLevelChanged?.Invoke(currentLevel);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetUnlockLevel(GunsType gunsType)
    {
        if (unlockLevelLookup != null && unlockLevelLookup.TryGetValue(gunsType, out var level))
        {
            return level;
        }

        return -1;
    }

    public List<GunsType> GetAllUnlockedWeapons()
    {
        var result = new List<GunsType>();

        if (unlockLevelLookup != null)
        {
            foreach (var kvp in unlockLevelLookup)
            {
                if (IsUnlocked(kvp.Key))
                {
                    result.Add(kvp.Key);
                }
            }
        }

        result.AddRange(unlockedWeapons.Where(x => !result.Contains(x)));

        return result;
    }
}
