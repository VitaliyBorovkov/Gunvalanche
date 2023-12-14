using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private EntityDataStorage entityDataStorage;

    public void Start()
    {
        Context.Initialize(entityDataStorage);
    }
}
