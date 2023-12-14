public class Context
{
    private static Context instance;
    public static Context Instance => instance ?? (instance = new Context());

    public bool IsInitialized { get; private set; }
    public IDataSystem DataSystem { get; private set; }
    //public IScoreSystem ScoreSystem { get; private set; }

    private Context()
    {
    }

    public static void Initialize(EntityDataStorage entityData)
    {
        Instance.DataSystem = new DataSystem(entityData);
        //Instance.ScoreSystem = new ScoreSystem();
        Instance.IsInitialized = true;
    }
}
