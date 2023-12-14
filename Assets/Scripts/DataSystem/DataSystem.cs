public class DataSystem : IDataSystem
{
    public EntityData[] EntityData { get; }

    public DataSystem(EntityDataStorage entityDataStorage)
    {
        EntityData = entityDataStorage?.entityData ?? new EntityData[0];
    }
}
