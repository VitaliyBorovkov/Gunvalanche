using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Ammo/AmmoSpawnRules", menuName = "AmmoSpawnRules")]
public class AmmoSpawnRules : ScriptableObject
{
    private const string LOG_PREFIX = "AmmoSpawnRules";

    [Serializable]
    public struct AmmoSpawnEntry
    {
        public GunsType gunsType;
        public bool isEnabled;
    }

    [SerializeField] private List<AmmoSpawnEntry> ammoSpawnEntries = new List<AmmoSpawnEntry>();

    private Dictionary<GunsType, AmmoSpawnEntry> lookup;

    private void OnEnable()
    {
        RebuildLookup();
    }

    private void OnValidate()
    {
        RebuildLookup();
    }

    private void EnsureLookup()
    {
        if (lookup == null)
        {
            RebuildLookup();
        }
    }

    private void RebuildLookup()
    {
        lookup = new Dictionary<GunsType, AmmoSpawnEntry>();

        if (ammoSpawnEntries == null)
        {
            return;
        }

        foreach (var entry in ammoSpawnEntries)
        {
            if (lookup.ContainsKey(entry.gunsType))
            {
                Debug.LogWarning($"{LOG_PREFIX}: Duplicate entry for GunsType '{entry.gunsType}'. First entry will be used.", this);
                continue;
            }

            lookup.Add(entry.gunsType, entry);
        }
    }

    public bool IsAmmoEnabled(GunsType gunsType)
    {
        EnsureLookup();

        if (lookup != null && lookup.TryGetValue(gunsType, out var entry))
        {
            return entry.isEnabled;
        }

        return true;
    }
}
