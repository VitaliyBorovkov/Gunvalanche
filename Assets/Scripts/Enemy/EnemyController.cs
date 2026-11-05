using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float attackRange = 2f;

    [Header("Navigation")]
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float wanderInterval = 2.5f;
    [SerializeField] private float wanderRadius = 5f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private bool hasLoggedMissingPlayer = false;
    private bool isPlayerDead = false;

    private float pathUpdateTimer = 0f;
    private float wanderTimer = 0f;
    private Vector3 lastFallbackDestination = Vector3.positiveInfinity;

    private void OnEnable()
    {
        PlayerDeathHandler.PlayerDied += OnPlayerDied;

        // Сбрасываем таймеры — после respawn даём агенту кадр на инициализацию
        pathUpdateTimer = 0f;
        wanderTimer = 0f;
        lastFallbackDestination = Vector3.positiveInfinity;

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

    private void OnDisable()
    {
        PlayerDeathHandler.PlayerDied -= OnPlayerDied;
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
        if (isPlayerDead)
        {
            if (navMeshAgent != null && !navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = true;
            }

            SetWalking(false);
            SetAttacking(false);
            return;
        }

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

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0f;
            UpdateDestination();
        }

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

    private void UpdateDestination()
    {
        // Пробуем дойти прямо до игрока
        if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            lastFallbackDestination = Vector3.positiveInfinity;
            navMeshAgent.SetDestination(playerTransform.position);
            return;
        }

        // Игрок недоступен (на камне/горе) — бродим рядом, чтобы не стоять болванчиком
        wanderTimer += pathUpdateInterval;
        if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0f;
            TrySetWanderDestination();
        }
    }

    private void TrySetWanderDestination()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        if (NavMesh.SamplePosition(candidate, out NavMeshHit wanderHit, wanderRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(wanderHit.position);
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

    private void OnPlayerDied()
    {
        isPlayerDead = true;
    }
}
