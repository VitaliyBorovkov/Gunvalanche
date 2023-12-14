using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EntityInitializer : MonoBehaviour
{
    [SerializeField] private string entityKey;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HealthController health;
    //[SerializeField] private NavMeshAgent navMeshAgent;
    
    private void Start()
    {
        if (Context.Instance == null)
        {
            Debug.LogError("Context.Instance is null. Make sure it's initialized.");
            return;
        }

        if (Context.Instance.DataSystem == null)
        {
            Debug.LogError("Context.Instance.DataSystem is null. Make sure it's initialized.");
            return;
        }

        if (Context.Instance.DataSystem.EntityData == null)
        {
            Debug.LogError("Context.Instance.DataSystem.EntityData is null. Make sure it's initialized.");
            return;
        }

        var data = Context.Instance.DataSystem.EntityData.FirstOrDefault(e => e.Name == entityKey);

        if (data.Equals(default(EntityData)))
        {
            Debug.LogError($"No entity data found with name {entityKey}.");
            return;
        }

        Initialize(data);
    }

    public void Initialize(EntityData data)
    {
        if (playerMovement != null)
            playerMovement.walkSpeed = data.WalkSpeed;

        //if (navMeshAgent != null)
        //{
        //    navMeshAgent.speed = data.Speed;
        //}

        //health.SetHealth(data.Health);
    }
}
