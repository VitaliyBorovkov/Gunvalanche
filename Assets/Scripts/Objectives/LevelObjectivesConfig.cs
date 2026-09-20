using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Level Objectives Config", fileName = "LevelObjectivesConfig")]
public class LevelObjectivesConfig : ScriptableObject
{
    public List<ObjectiveData> objectives = new();
}
