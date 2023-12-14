using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataStorage")]
public class EntityDataStorage : ScriptableObject
{
    public EntityData[] entityData;

    public EntityData GetEntityData(string key)
    {
        return entityData.FirstOrDefault(entity => entity.Name == key);
    }
}
