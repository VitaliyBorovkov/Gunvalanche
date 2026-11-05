using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public static class WeaponBarSlotPrefabCreator
{
    private const string PREFAB_PATH = "Assets/Prefabs/UI/WeaponBarSlot.prefab";

    [MenuItem("Tools/UI/Create WeaponBarSlot Prefab")]
    public static void CreatePrefab()
    {
        // ---- root ------------------------------------------------
        var root = new GameObject("WeaponBarSlot");
        var rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(155f, 150f);
        root.AddComponent<WeaponBarSlot>();

        // ---- background ------------------------------------------
        var bg = CreateChild(root, "Background", new Vector2(155f, 150f));
        var bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.45f);
        bgImage.raycastTarget = false;

        // ---- selection border ------------------------------------
        var border = CreateChild(root, "SelectionBorder", new Vector2(155f, 150f));
        var borderImage = border.AddComponent<Image>();
        borderImage.color = Color.white;
        borderImage.raycastTarget = false;

        // Use a built-in Unity UI sprite so the border looks good without extra assets.
        // After import you can replace this with any custom sprite.
        var builtInSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        if (builtInSprite != null)
        {
            borderImage.sprite   = builtInSprite;
            borderImage.type     = Image.Type.Sliced;
            borderImage.fillCenter = false;
        }

        borderImage.enabled = false; // hidden by default

        // ---- weapon icon -----------------------------------------
        var icon = CreateChild(root, "WeaponIcon", new Vector2(140f, 105f));
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchoredPosition = new Vector2(0f, 18f);
        var iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;
        iconImage.enabled = false; // will be enabled once sprite loads
        iconImage.raycastTarget = false;

        // ---- ammo text -------------------------------------------
        var ammoGO = CreateChild(root, "AmmoText", new Vector2(155f, 42f));
        var ammoRect = ammoGO.GetComponent<RectTransform>();
        ammoRect.anchoredPosition = new Vector2(0f, -57f);
        var ammoTMP = ammoGO.AddComponent<TextMeshProUGUI>();
        ammoTMP.text = "0/0";
        ammoTMP.fontSize = 36f;
        ammoTMP.alignment = TextAlignmentOptions.Center;
        ammoTMP.color = Color.white;
        ammoTMP.raycastTarget = false;

        // ---- wire up WeaponBarSlot references --------------------
        var slot = root.GetComponent<WeaponBarSlot>();
        var so = new SerializedObject(slot);
        so.FindProperty("iconImage").objectReferenceValue      = iconImage;
        so.FindProperty("ammoText").objectReferenceValue       = ammoTMP;
        so.FindProperty("selectionBorder").objectReferenceValue = borderImage;
        so.ApplyModifiedPropertiesWithoutUndo();

        // ---- save prefab -----------------------------------------
        EnsureDirectory("Assets/Prefabs/UI");
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, PREFAB_PATH);
        Object.DestroyImmediate(root);

        if (prefab != null)
        {
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log($"[WeaponBarSlotPrefabCreator] Prefab created at {PREFAB_PATH}");
        }
        else
        {
            Debug.LogError("[WeaponBarSlotPrefabCreator] Failed to save prefab.");
        }
    }

    private static GameObject CreateChild(GameObject parent, string name, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot     = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        return go;
    }

    private static void EnsureDirectory(string path)
    {
        var parts = path.Split('/');
        var current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
