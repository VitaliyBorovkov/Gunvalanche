using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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

            float distanceToPlayer = Vector3.Distance(transform.position, 
                playerTransform.position);

            if (distanceToPlayer > navMeshAgent.stoppingDistance)
            {
                //if it's far away from the player, go.
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
            else
            {
                //if close to the player, attack.
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }
        }
        else
        {   //if player not found, stay where we are.
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }
}
