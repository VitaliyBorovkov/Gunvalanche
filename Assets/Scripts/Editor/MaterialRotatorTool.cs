using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor tool: assigns materials to MeshRenderers in round-robin order,
/// grouped by GameObject name filter.
/// Open via Tools > Material Rotator.
/// Config is saved to Assets/Editor/MaterialRotatorConfig.asset — persists between sessions.
/// </summary>
public class MaterialRotatorTool : EditorWindow
{
    const string ConfigPath = "Assets/Scripts/Editor/MaterialRotatorConfig.asset";

    MaterialRotatorConfig _config;
    Vector2 _scroll;

    [MenuItem("Tools/Material Rotator")]
    static void Open() => GetWindow<MaterialRotatorTool>("Material Rotator");

    // ── Config load / create ──────────────────────────────────────────────────

    void OnEnable()
    {
        LoadOrCreateConfig();
    }

    void LoadOrCreateConfig()
    {
        _config = AssetDatabase.LoadAssetAtPath<MaterialRotatorConfig>(ConfigPath);
        if (_config != null) return;

        _config = CreateInstance<MaterialRotatorConfig>();
        _config.groups.Add(new MaterialGroup { label = "Rocks",           nameFilter = "Rock"   });
        _config.groups.Add(new MaterialGroup { label = "Stones",          nameFilter = "Stone"  });
        _config.groups.Add(new MaterialGroup { label = "Trees (Leaves)",  nameFilter = "Leaves" });
        _config.groups.Add(new MaterialGroup { label = "Trees (Trunks)",  nameFilter = "Tree_3" });
        _config.groups.Add(new MaterialGroup { label = "Grass",           nameFilter = "Grass"  });
        _config.groups.Add(new MaterialGroup { label = "Plants / Bushes", nameFilter = "Plant"  });

        AssetDatabase.CreateAsset(_config, ConfigPath);
        AssetDatabase.SaveAssets();
    }

    void SaveConfig()
    {
        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
    }

    // ── GUI ───────────────────────────────────────────────────────────────────

    void OnGUI()
    {
        if (_config == null) { LoadOrCreateConfig(); return; }

        EditorGUILayout.LabelField("Material Rotator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Finds MeshRenderers whose name contains the filter and assigns materials in round-robin order.\n" +
            "Open Scene.prefab in Prefab Mode before applying.",
            MessageType.Info);

        EditorGUILayout.Space(4);
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        for (int i = _config.groups.Count - 1; i >= 0; i--)
            if (!DrawGroup(_config.groups[i]))
            {
                _config.groups.RemoveAt(i);
                SaveConfig();
            }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space(4);

        if (GUILayout.Button("+ Add Group"))
        {
            _config.groups.Add(new MaterialGroup());
            SaveConfig();
        }

        EditorGUILayout.Space(8);

        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("▶  Apply All Groups", GUILayout.Height(32)))
            ApplyAll();
        GUI.backgroundColor = Color.white;
    }

    /// <returns>false if the group was deleted</returns>
    bool DrawGroup(MaterialGroup g)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        g.foldout = EditorGUILayout.Foldout(g.foldout, g.label, true, EditorStyles.foldoutHeader);
        if (GUILayout.Button("✕", GUILayout.Width(22)))
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck()) SaveConfig();
            return false;
        }
        EditorGUILayout.EndHorizontal();

        if (g.foldout)
        {
            g.label      = EditorGUILayout.TextField("Label",         g.label);
            g.nameFilter = EditorGUILayout.TextField("Name contains", g.nameFilter);

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Materials (rotation order):", EditorStyles.miniBoldLabel);

            for (int i = g.materials.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                g.materials[i] = (Material)EditorGUILayout.ObjectField(
                    g.materials[i], typeof(Material), false);
                if (GUILayout.Button("−", GUILayout.Width(22)))
                    g.materials.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Material Slot"))
                g.materials.Add(null);

            EditorGUILayout.Space(2);

            if (GUILayout.Button($"Preview: count '{g.nameFilter}' objects"))
            {
                int count = FindRenderers(g.nameFilter).Length;
                Debug.Log($"[MaterialRotator] '{g.nameFilter}': {count} MeshRenderers found.");
                EditorUtility.DisplayDialog("Preview",
                    $"'{g.nameFilter}': {count} objects found.", "OK");
            }
        }

        EditorGUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck()) SaveConfig();
        return true;
    }

    // ── Logic ─────────────────────────────────────────────────────────────────

    void ApplyAll()
    {
        int totalObjects = 0;

        foreach (var g in _config.groups)
        {
            if (string.IsNullOrEmpty(g.nameFilter)) continue;

            var mats = g.materials.Where(m => m != null).ToArray();
            if (mats.Length == 0)
            {
                Debug.LogWarning($"[MaterialRotator] '{g.label}' — no materials assigned, skipped.");
                continue;
            }

            var renderers = FindRenderers(g.nameFilter);
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"[MaterialRotator] '{g.label}' — no objects found for filter '{g.nameFilter}'.");
                continue;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Undo.RecordObject(renderers[i], "Material Rotator");
                renderers[i].sharedMaterial = mats[i % mats.Length];
                EditorUtility.SetDirty(renderers[i]);
            }

            Debug.Log($"[MaterialRotator] '{g.label}': {renderers.Length} objects → {mats.Length} materials");
            totalObjects += renderers.Length;
        }

        MarkDirty();
        Debug.Log($"[MaterialRotator] Done — {totalObjects} objects updated. Save the prefab (Ctrl+S).");
    }

    static MeshRenderer[] FindRenderers(string nameFilter)
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null)
        {
            return stage.prefabContentsRoot
                .GetComponentsInChildren<MeshRenderer>(includeInactive: true)
                .Where(r => r.gameObject.name.Contains(nameFilter))
                .ToArray();
        }

        return Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => r.gameObject.name.Contains(nameFilter))
            .ToArray();
    }

    static void MarkDirty()
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null)
            EditorSceneManager.MarkSceneDirty(stage.scene);
        else
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
