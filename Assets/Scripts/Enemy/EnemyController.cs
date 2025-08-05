using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float attackRange = 2f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private bool hasLoggedMissingPlayer = false;

    private void OnEnable()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.Log("EnemyController: Player with tag 'Player' not found.");
            }
        }
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerTransform == null)
        {
            Debug.LogError("EnemyController: PlayerTransform is not set.");
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            if (!hasLoggedMissingPlayer)
            {
                Debug.Log($"EnemyController:{gameObject.name}does not have a reference to the player!");
                hasLoggedMissingPlayer = true;
            }
            return;
        }
        hasLoggedMissingPlayer = false;

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
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }

    private void SetAttacking(bool isAttacking)
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", isAttacking);
        }
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}

