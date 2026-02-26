using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Ammo/AmmoSpawnRules", menuName = "AmmoSpawnRules")]
public class AmmoSpawnRules : ScriptableObject
{
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
                continue;
            }

            lookup.Add(entry.gunsType, entry);
        }
    }

    public bool IsAmmoEnabled(GunsType gunsType)
    {
        if (lookup == null)
        {
            RebuildLookup();
        }

        if (lookup == null || lookup.TryGetValue(gunsType, out var entry))
        {
            return true;
        }

        return entry.isEnabled;
    }
}
