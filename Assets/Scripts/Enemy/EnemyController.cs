using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float attackRange = 2f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.Log(" Player with tag 'Player' not found.");
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        navMeshAgent.SetDestination(playerTransform.position);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > navMeshAgent.stoppingDistance)
        {
            SetWalking(navMeshAgent.velocity.sqrMagnitude > 0.1f);
            SetAttacking(false);
        }
        else if (distanceToPlayer <= attackRange)
        {
            SetWalking(false);
            SetAttacking(true);
        }
        else
        {
            SetWalking(false);
            SetAttacking(false);
        }
    }

    private void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    private void SetAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }
}
