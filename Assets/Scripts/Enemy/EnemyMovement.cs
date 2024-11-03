using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null )
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.Log("Player with tag ‘Player’ not found.");
            }
        }
    }

    private void Update()
    {
        if(playerTransform != null)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
    }
}
