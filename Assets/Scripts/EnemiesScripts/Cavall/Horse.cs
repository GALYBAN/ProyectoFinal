using UnityEngine;
using UnityEngine.AI;

public class Horse : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Charging }
    private EnemyState currentState;

    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;
    public float detectionRange = 5f;
    public float visionAngle = 60f;
    public float chargeSpeed = 10f;
    public float patrolSpeed = 2f;
    public float chargeDuration = 2f;
    public LayerMask playerLayer;

    private Transform player;
    private NavMeshAgent agent;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrolling;
        targetPoint = pointA;
        agent.speed = patrolSpeed;
        agent.SetDestination(targetPoint.position);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Charging:
                Charge();
                break;
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
            agent.SetDestination(targetPoint.position);
        }
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < visionAngle / 2f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, out hit, detectionRange, playerLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Jugador detectado, cambiando a Chasing!");
                        currentState = EnemyState.Chasing;
                    }
                }
            }
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) < 3f)
        {
            currentState = EnemyState.Charging;
            isCharging = true;
            chargeTimer = chargeDuration;
            agent.speed = chargeSpeed;
        }
    }

    void Charge()
    {
        if (isCharging)
        {
            Vector3 chargeDirection = (player.position - transform.position).normalized;
            agent.velocity = chargeDirection * chargeSpeed;
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                isCharging = false;
                currentState = EnemyState.Patrolling;
                agent.speed = patrolSpeed;
                targetPoint = Vector3.Distance(transform.position, pointA.position) < Vector3.Distance(transform.position, pointB.position) ? pointB : pointA;
                agent.SetDestination(targetPoint.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * detectionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + Vector3.up * 1f, directionToPlayer * detectionRange);
        }
    }
}
