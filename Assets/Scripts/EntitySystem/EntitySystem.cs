using System.Collections.Generic;
using System.Linq;

public interface IEntitySystem
{
    EntityData GetEntityData(string key);
}

public class EntitySystem : IEntitySystem
{
    public List<EntityData> entities;

    public EntitySystem()
    {
    }

    public EntityData GetEntityData(string key)
    {
        return entities.FirstOrDefault(entityKey => entityKey.Name == key);
    }
}
