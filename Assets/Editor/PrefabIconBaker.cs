// PrefabIconBaker.cs
// Put this file into Assets/Editor/PrefabIconBaker.cs

using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrefabIconBaker
{
    private const string LOG_PREFIX = "PrefabIconBaker";
    private const string OUTPUT_FOLDER = "Assets/Resources/WeaponIcons";
    private const string LAYER_NAME = "IconPreview"; // layer used for preview rendering

    [MenuItem("Assets/Tools/Bake Selected Prefab Icons (PNG, transparent)", false, 200)]
    private static void BakeSelected()
    {
        var selections = Selection.objects;
        if (selections == null || selections.Length == 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: No objects selected.");
            return;
        }

        if (!Directory.Exists(OUTPUT_FOLDER)) Directory.CreateDirectory(OUTPUT_FOLDER);

        int processed = 0;

        // ensure layer exists
        int layerIndex = EnsureLayerExists(LAYER_NAME);
        if (layerIndex < 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Could not ensure layer '{LAYER_NAME}'. Using Default layer and camera culling may include scene objects.");
        }

        foreach (var obj in selections)
        {
            var prefab = obj as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Skipping non-GameObject selection: {obj.name}");
                continue;
            }

            try
            {
                string outPath = Path.Combine(OUTPUT_FOLDER, prefab.name + ".png");
                BakePrefabToPNG(prefab, outPath, 512, layerIndex); // size 512 — change if needed
                Debug.Log($"{LOG_PREFIX}: Baked icon for {prefab.name} -> {outPath}");
                processed++;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}: Failed to bake {prefab.name}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"{LOG_PREFIX}: Done. Processed {processed} prefabs.");
    }

    private static void BakePrefabToPNG(GameObject prefab, string outputPath, int size, int previewLayer)
    {
        // 1) Create empty additive scene
        Scene tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        // 2) Instantiate prefab in scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null) throw new System.Exception("InstantiatePrefab returned null");
        SceneManager.MoveGameObjectToScene(instance, tmpScene);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        // assign preview layer to instance (recursively)
        if (previewLayer >= 0) SetLayerRecursively(instance, previewLayer);

        // 3) Add simple directional light for consistent lighting
        GameObject lightGO = new GameObject("IconLight");
        SceneManager.MoveGameObjectToScene(lightGO, tmpScene);
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.0f;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // 4) Create camera
        GameObject camGO = new GameObject("IconCamera");
        SceneManager.MoveGameObjectToScene(camGO, tmpScene);
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0f, 0f, 0f, 0f); // transparent background
        cam.allowHDR = false;
        cam.allowMSAA = false;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;
        cam.fieldOfView = 30f; // tweak if needed

        // IMPORTANT: make camera render only preview layer to avoid drawing scene background
        if (previewLayer >= 0)
        {
            cam.cullingMask = (1 << previewLayer);
        }
        else
        {
            // fallback: render nothing but we still need the prefab layer visible,
            // keep default culling mask (may include scene objects) and log warning
            Debug.LogWarning($"{LOG_PREFIX}: previewLayer == -1, camera.cullingMask left default.");
        }

        // 5) Calculate bounds
        Bounds b = CalculateBounds(instance);
        Vector3 center = b.center;
        float radius = b.extents.magnitude;
        if (radius <= 0.001f) radius = 0.5f;

        // 6) Position camera (nice 3/4 view)
        Vector3 dir = (Vector3.back + Vector3.up * 0.25f).normalized;
        float distance = ComputeCameraDistanceForBounds(cam, radius);
        cam.transform.position = center + dir * distance;
        cam.transform.LookAt(center);
        cam.transform.RotateAround(center, cam.transform.right, -10f);

        // 7) Auto-correct orientation if object is facing away from camera
        Vector3 toCamera = (cam.transform.position - center).normalized;
        // use local forward to approximate muzzle direction
        Vector3 localForward = instance.transform.forward;
        float dot = Vector3.Dot(localForward, toCamera);
        // if dot < 0 => forward points roughly opposite to camera => rotate 90 around up
        if (dot < 0f)
        {
            instance.transform.Rotate(Vector3.up, 90f, Space.Self);
            Debug.Log($"{LOG_PREFIX}: Auto-rotated instance {instance.name} by 90 degrees to face camera (dot={dot:F2}).");
            // recalc bounds/position if rotation changes extents significantly
            b = CalculateBounds(instance);
            center = b.center;
            radius = b.extents.magnitude;
            distance = ComputeCameraDistanceForBounds(cam, radius);
            cam.transform.position = center + dir * distance;
            cam.transform.LookAt(center);
            cam.transform.RotateAround(center, cam.transform.right, -10f);
        }

        // 8) Render to RenderTexture
        RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        cam.targetTexture = rt;

        // 9) Force render and read
        RenderTexture current = RenderTexture.active;
        RenderTexture.active = rt;
        cam.Render();

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();

        RenderTexture.active = current;

        // 10) Encode to PNG and save
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(outputPath, bytes);

        // 11) Import asset and set to Sprite (2D and UI) preserving alpha
        AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(outputPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: TextureImporter not found for {outputPath}");
        }

        // 12) Cleanup
        Object.DestroyImmediate(tex);
        cam.targetTexture = null;
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGO);
        Object.DestroyImmediate(lightGO);
        Object.DestroyImmediate(instance);

        // Close temporary scene
        EditorSceneManager.CloseScene(tmpScene, true);
    }

    // Ensure layer exists in TagManager. Returns layer index or -1 if failed.
    private static int EnsureLayerExists(string layerName)
    {
        if (string.IsNullOrEmpty(layerName)) return -1;
        for (int i = 0; i < 32; i++)
        {
            string name = LayerMask.LayerToName(i);
            if (name == layerName) return i;
        }

        // add layer in TagManager (only in Editor)
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (layersProp == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Cannot find layers in TagManager.");
            return -1;
        }

        // find empty slot in user layers (8..31)
        for (int i = 8; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"{LOG_PREFIX}: Added layer '{layerName}' at index {i}.");
                return i;
            }
        }

        Debug.LogWarning($"{LOG_PREFIX}: No free layer slot found to add '{layerName}'. Please add layer manually and re-run.");
        return -1;
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        if (go == null) return;
        go.layer = layer;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    private static Bounds CalculateBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one * 0.5f);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
        return b;
    }

    private static float ComputeCameraDistanceForBounds(Camera cam, float radius)
    {
        float halfFovRad = cam.fieldOfView * Mathf.Deg2Rad * 0.5f;
        if (halfFovRad <= 0.001f) halfFovRad = 0.5f;
        float distance = radius / Mathf.Sin(halfFovRad);
        distance *= 1.25f; // padding
        if (distance < 0.5f) distance = 0.5f;
        return distance;
    }
}
