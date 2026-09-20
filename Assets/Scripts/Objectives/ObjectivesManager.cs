using System;

using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    private const string LOG_PREFIX = "ObjectivesManager";

    public enum ObjectiveState
    {
        Pending,
        Active,
        Completed
    }

    [SerializeField] private LevelObjectivesConfig objectivesConfig;

    public event Action OnChanged;

    private ObjectiveState[] states = new ObjectiveState[0];
    private int[] currentCounts = new int[0];
    private int[] totalCounts = new int[0];

    public int ObjectiveCount => objectivesConfig != null && objectivesConfig.objectives != null
        ? objectivesConfig.objectives.Count
        : 0;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void Start()
    {
        if (ObjectiveCount == 0)
        {
            return;
        }

        SetActive(0);
        OnChanged?.Invoke();
    }

    public ObjectiveData GetObjectiveData(int index)
    {
        return IsValidIndex(index) ? objectivesConfig.objectives[index] : null;
    }

    public ObjectiveState GetState(int index)
    {
        EnsureInitialized();
        return IsValidIndex(index) ? states[index] : ObjectiveState.Pending;
    }

    public (int current, int total) GetProgress(int index)
    {
        EnsureInitialized();
        return IsValidIndex(index) ? (currentCounts[index], totalCounts[index]) : (0, 0);
    }

    public void SetProgress(int index, int current, int total)
    {
        EnsureInitialized();

        if (!IsValidIndex(index))
        {
            Debug.LogWarning($"{LOG_PREFIX}: SetProgress() invalid index {index}.");
            return;
        }

        currentCounts[index] = current;
        totalCounts[index] = total;
        OnChanged?.Invoke();
    }

    public void CompleteObjective(int index)
    {
        EnsureInitialized();

        if (!IsValidIndex(index))
        {
            Debug.LogWarning($"{LOG_PREFIX}: CompleteObjective() invalid index {index}.");
            return;
        }

        if (states[index] == ObjectiveState.Completed)
        {
            return;
        }

        states[index] = ObjectiveState.Completed;

        int nextIndex = index + 1;
        if (nextIndex < ObjectiveCount)
        {
            SetActive(nextIndex);
        }

        OnChanged?.Invoke();
    }

    private void SetActive(int index)
    {
        EnsureInitialized();
        states[index] = ObjectiveState.Active;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < ObjectiveCount;
    }

    // Called both from Awake() and defensively from every public accessor: Unity does not
    // guarantee Awake() on this object runs before OnEnable()/Start() on some OTHER object
    // (e.g. ObjectivesUI, which reads state as soon as it enables) — so the state arrays
    // must be ready no matter which script asks first, not just after this object's own
    // Awake() has had a chance to run.
    private void EnsureInitialized()
    {
        if (states != null && states.Length == ObjectiveCount)
        {
            return;
        }

        if (ObjectiveCount == 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: No objectives configured.");
            states = new ObjectiveState[0];
            currentCounts = new int[0];
            totalCounts = new int[0];
            return;
        }

        states = new ObjectiveState[ObjectiveCount];
        currentCounts = new int[ObjectiveCount];
        totalCounts = new int[ObjectiveCount];
    }
}
