using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persistent config asset for MaterialRotatorTool.
/// Saved at Assets/Editor/MaterialRotatorConfig.asset
/// </summary>
[CreateAssetMenu(fileName = "MaterialRotatorConfig", menuName = "Tools/Material Rotator Config")]
public class MaterialRotatorConfig : ScriptableObject
{
    public List<MaterialGroup> groups = new List<MaterialGroup>();
}

[System.Serializable]
public class MaterialGroup
{
    public string label      = "Group";
    public string nameFilter = "";
    public bool   foldout    = true;
    public List<Material> materials = new List<Material>();
}
