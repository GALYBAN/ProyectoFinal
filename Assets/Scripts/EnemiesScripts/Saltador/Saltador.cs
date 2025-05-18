using UnityEngine;
using UnityEngine.AI;

public class Saltador : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Jumping, Recovering }
    private EnemyState currentState;

    [Header("Patrol Settings")]
    [Tooltip("Punto A de patrulla - Asignar en el Inspector")]
    public Transform pointA;
    [Tooltip("Punto B de patrulla - Asignar en el Inspector")]
    public Transform pointB;
    private Transform targetPoint;
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 1f;
    private float patrolWaitTimer;
    private float initialZ;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float visionAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    private bool canSeePlayer;

    [Header("Jump Attack Settings")]
    public float jumpForce = 10f;
    public float jumpHeight = 5f;
    public float jumpDuration = 1f;
    public float recoveryTime = 1f;
    private float recoveryTimer;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private Vector3 jumpTarget;
    private Vector3 jumpStartPosition;
    private float jumpProgress = 0f;

    [Header("References")]
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Guardar posición Z inicial
        initialZ = transform.position.z;
        
        // Verificar que los puntos de patrulla estén asignados
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"Los puntos de patrulla no están asignados para el saltador en {gameObject.name}! Por favor, asigna los puntos en el Inspector o usa SetPatrolPoints.");
        }
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("No se encontró el jugador!");
            return;
        }

        currentState = EnemyState.Patrolling;
        targetPoint = pointA;
        agent.speed = patrolSpeed;
        SetDestination2D(targetPoint.position);
    }

    void Update()
    {
        if (player == null) return;

        // Mantener solo la posición Z fija
        Vector3 currentPos = transform.position;
        currentPos.z = initialZ;
        transform.position = currentPos;

        UpdateState();
        UpdateAnimation();
    }

    void SetDestination2D(Vector3 target)
    {
        // Mantener solo el movimiento en X y Y
        Vector3 targetPos = new Vector3(target.x, target.y, initialZ);
        agent.SetDestination(targetPos);
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Jumping:
                JumpAttack();
                break;
            case EnemyState.Recovering:
                Recover();
                break;
        }
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(agent.velocity.x));
            animator.SetBool("IsJumping", isJumping);
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (patrolWaitTimer <= 0)
            {
                targetPoint = targetPoint == pointA ? pointB : pointA;
                SetDestination2D(targetPoint.position);
                patrolWaitTimer = patrolWaitTime;
            }
            else
            {
                patrolWaitTimer -= Time.deltaTime;
            }
        }
        DetectPlayer();
    }

    void DetectPlayer()
    {
        // Calcular distancia solo en el eje X
        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        
        if (distanceToPlayerX < detectionRange)
        {
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            float angle = Vector3.Angle(transform.forward, new Vector3(directionX, 0, 0));

            if (angle < visionAngle / 2f)
            {
                RaycastHit hit;
                Vector3 rayDirection = new Vector3(directionX, 0, 0);
                if (Physics.Raycast(transform.position + Vector3.up * 1f, rayDirection, out hit, detectionRange, playerLayer | obstacleLayer))
                {
                    canSeePlayer = hit.collider.CompareTag("Player");
                    if (canSeePlayer)
                    {
                        currentState = EnemyState.Chasing;
                    }
                }
            }
        }
    }

    void ChasePlayer()
    {
        SetDestination2D(player.position);
        
        // Si el jugador se aleja demasiado en X, volver a patrullar
        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        if (distanceToPlayerX > detectionRange * 1.5f)
        {
            currentState = EnemyState.Patrolling;
            return;
        }

        if (distanceToPlayerX < 3f)
        {
            StartJumpAttack();
        }
    }

    void StartJumpAttack()
    {
        currentState = EnemyState.Jumping;
        isJumping = true;
        jumpTimer = jumpDuration;
        agent.enabled = false; // Desactivar el NavMeshAgent durante el salto
        
        // Guardar posición inicial del salto
        jumpStartPosition = transform.position;
        
        // Calcular objetivo del salto (posición del jugador)
        jumpTarget = new Vector3(
            player.position.x,
            transform.position.y,
            initialZ
        );

        // Aplicar fuerza de salto
        if (rb != null)
        {
            rb.velocity = new Vector3(
                (jumpTarget.x - transform.position.x) / jumpDuration,
                jumpForce,
                0
            );
        }
    }

    void JumpAttack()
    {
        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            
            if (jumpTimer <= 0 || IsGrounded())
            {
                EndJumpAttack();
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    void EndJumpAttack()
    {
        isJumping = false;
        currentState = EnemyState.Recovering;
        recoveryTimer = recoveryTime;
        
        // Reactivar el NavMeshAgent
        agent.enabled = true;
        agent.speed = patrolSpeed;
        
        // Detener el movimiento
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void Recover()
    {
        recoveryTimer -= Time.deltaTime;
        
        if (recoveryTimer <= 0)
        {
            currentState = EnemyState.Patrolling;
            targetPoint = Mathf.Abs(transform.position.x - pointA.position.x) < Mathf.Abs(transform.position.x - pointB.position.x) ? pointB : pointA;
            SetDestination2D(targetPoint.position);
        }
    }

    // Método público para asignar puntos de patrulla desde el jefe
    public void SetPatrolPoints(Transform newPointA, Transform newPointB)
    {
        pointA = newPointA;
        pointB = newPointB;
        
        if (currentState == EnemyState.Patrolling)
        {
            targetPoint = pointA;
            SetDestination2D(targetPoint.position);
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dibujar ángulo de visión
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * Vector3.right * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * Vector3.right * detectionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Dibujar línea al jugador si está en rango
        if (player != null)
        {
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            Vector3 directionToPlayer = new Vector3(directionX, 0, 0);
            Gizmos.color = canSeePlayer ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 1f, directionToPlayer * detectionRange);
        }
    }
} 