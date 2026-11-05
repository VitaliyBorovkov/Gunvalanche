using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public static class WeaponBarSetup
{
    [MenuItem("Tools/UI/Setup WeaponBar SlotsContainer")]
    public static void SetupSlotsContainer()
    {
        var barGO = GameObject.Find("WeaponBar");
        if (barGO == null)
        {
            Debug.LogError("[WeaponBarSetup] 'WeaponBar' not found in scene.");
            return;
        }

        // Remove existing SlotsContainer if any
        var existing = barGO.transform.Find("SlotsContainer");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        // Create SlotsContainer
        var container = new GameObject("SlotsContainer");
        container.transform.SetParent(barGO.transform, false);

        var rect = container.AddComponent<RectTransform>();
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta        = Vector2.zero;

        var hlg = container.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment         = TextAnchor.MiddleCenter;
        hlg.spacing                = 8f;
        hlg.childForceExpandWidth  = false;
        hlg.childForceExpandHeight = false;
        hlg.childControlWidth      = false;
        hlg.childControlHeight     = false;

        // Wire SlotsContainer into WeaponBarController
        var controller = barGO.GetComponent<WeaponBarController>();
        if (controller != null)
        {
            var so = new SerializedObject(controller);
            so.FindProperty("slotsContainer").objectReferenceValue = container.transform;
            so.ApplyModifiedProperties();
            Debug.Log("[WeaponBarSetup] SlotsContainer wired into WeaponBarController.");
        }
        else
        {
            Debug.LogWarning("[WeaponBarSetup] WeaponBarController not found on WeaponBar.");
        }

        EditorUtility.SetDirty(barGO);
        Selection.activeGameObject = container;
        EditorGUIUtility.PingObject(container);

        Debug.Log("[WeaponBarSetup] SlotsContainer created successfully.");
    }
}
