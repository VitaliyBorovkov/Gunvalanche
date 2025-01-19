using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EntityInitializer : MonoBehaviour
{
    [SerializeField] private string entityKey;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HealthController health;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private void Start()
    {
        if (Context.Instance == null || Context.Instance.DataSystem == null || Context.Instance.DataSystem.EntityData == null)
        {
            //Debug.LogError(" EntityInitializer. Context.Instance or its systems have not been initialized.");
            return;
        }

        var data = Context.Instance.DataSystem.EntityData.FirstOrDefault(e => e.Name == entityKey);

        if (data.Equals(default(EntityData)))
        {
            //Debug.LogError($" EntityInitializer. No entity data found with name {entityKey}.");
            return;
        }

        Initialize(data);
    }

    public void Initialize(EntityData data)
    {
        if (playerMovement != null)
            playerMovement.walkSpeed = data.WalkSpeed;

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = data.WalkSpeed;
        }

        if (health != null)
        {
            health.SetHealth(data.Health);
        }
    }
}
