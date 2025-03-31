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
    public float chargeSpeed = 10f;
    public float patrolSpeed = 2f;
    public float chargeDuration = 2f;

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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chasing;
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
            agent.SetDestination(transform.position + transform.right * chargeSpeed);
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
}
