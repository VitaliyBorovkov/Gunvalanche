using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Weapons/Config", order = 0)]
public class WeaponConfig : ScriptableObject
{
    public WeaponData[] weaponData;

    private WeaponRuntimeData[] runtimeData;

    private void OnEnable()
    {
        InitializeRuntimeData();
    
#if UNITY_EDITOR
    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

    private void InitializeRuntimeData()
    {
        if (runtimeData == null)
        {
            runtimeData = new WeaponRuntimeData[weaponData.Length];
        }

        for (int i = 0; i < weaponData.Length; i++)
        {
            runtimeData[i] = new WeaponRuntimeData(weaponData[i]);
        }
    }

    private void ResetToDefaultRuntimeData()
    {
        if (runtimeData == null)
        {
            return;
        }

        for (int i = 0; i < weaponData.Length; i++)
        {
            weaponData[i].CurrentAmmo = runtimeData[i].InitialCurrentAmmo;
            weaponData[i].TotalAmmo = runtimeData[i].InitialTotalAmmo;
        }
    }
#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode)
        {
            ResetToDefaultRuntimeData();
        }
    }
#endif
}
