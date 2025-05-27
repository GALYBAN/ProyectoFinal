using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Horse : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Charging, Recovering, Idle }
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
    private bool hasPatrolPoints = false;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float visionAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    private bool canSeePlayer;

    [Header("Combat Settings")]
    public float chargeSpeed = 10f;
    public float chargeDuration = 2f;
    public float chargeDistance = 5f;
    public float recoveryTime = 1f;
    private float recoveryTimer;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    private Vector3 chargeTarget;
    [SerializeField] private Collider attackCollider; // Collider para el ataque

    [Header("References")]
    [SerializeField] private Transform[] player;
    private NavMeshAgent agent;
    private Animator animator;
    private float currentDirection = 1f; // 1 para derecha, -1 para izquierda

    private bool canTakeDamage = true; // Nuevo estado para controlar el daño

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Guardar posición Z inicial
        initialZ = transform.position.z;
        
        // Verificar que los puntos de patrulla estén asignados
        CheckPatrolPoints();

        // Desactivar el collider de ataque al inicio
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        
    }
    
    void Start()
    {
        // Acceder al CharacterTransitionManager
        CharacterTransitionManager characterManager = FindObjectOfType<CharacterTransitionManager>();
        if (characterManager != null)
        {
            player = new Transform[2];
            player[0] = characterManager.GetUnpoweredCharacter().transform; // Asignar el personaje no transformado
            player[1] = characterManager.GetPoweredCharacter().transform; // Asignar el personaje transformado
        }
        else
        {
            Debug.LogError("No se encontró el CharacterTransitionManager en la escena!");
            return;
        }

        if (player == null || player.Length == 0)
        {
            Debug.LogError("No se encontraron jugadores en la escena!");
            return;
        }

        if (hasPatrolPoints)
        {
            currentState = EnemyState.Patrolling;
            targetPoint = pointA;
            agent.speed = patrolSpeed;
            SetDestination2D(targetPoint.position);
        }
        else
        {
            currentState = EnemyState.Idle;
            Debug.LogWarning($"El caballo en {gameObject.name} no tiene puntos de patrulla asignados. Permanecerá en estado Idle.");
        }
    }

    void CheckPatrolPoints()
    {
        hasPatrolPoints = (pointA != null && pointB != null);
        if (!hasPatrolPoints)
        {
            Debug.LogWarning($"Los puntos de patrulla no están asignados para el caballo en {gameObject.name}! Por favor, asigna los puntos en el Inspector o usa SetPatrolPoints.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Mantener solo la posición Z fija
        Vector3 currentPos = transform.position;
        currentPos.z = initialZ;
        transform.position = currentPos;

        // Actualizar la dirección basada en el movimiento
        if (agent.velocity.magnitude > 0.1f)
        {
            float direction = Mathf.Sign(agent.velocity.x);
            if (direction != 0 && direction != currentDirection)
            {
                currentDirection = direction;
                UpdateRotation();
            }
        }

        UpdateState();
        UpdateAnimation();
    }

    void UpdateRotation()
    {
        // Girar el modelo 180 grados si cambia la dirección
        transform.rotation = Quaternion.Euler(0, currentDirection > 0 ? 0 : 180, 0);
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
                if (hasPatrolPoints)
                {
                    Patrol();
                }
                else
                {
                    currentState = EnemyState.Idle;
                }
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Charging:
                Charge();
                break;
            case EnemyState.Recovering:
                Recover();
                break;
            case EnemyState.Idle:
                // En estado Idle, solo detectar al jugador
                DetectPlayer();
                break;
        }
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(agent.velocity.x));
            animator.SetBool("IsCharging", isCharging);
        }
    }

    void Patrol()
    {
        if (!hasPatrolPoints) return;

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
        // Calcular distancia solo en el eje X para ambos jugadores
        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var p in player)
        {
            float distanceToPlayerX = Mathf.Abs(p.position.x - transform.position.x);
            if (distanceToPlayerX < closestDistance)
            {
                closestDistance = distanceToPlayerX;
                closestPlayer = p;
            }
        }

        if (closestPlayer != null && closestDistance < detectionRange)
        {
            float directionX = Mathf.Sign(closestPlayer.position.x - transform.position.x);
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
        Transform closestPlayer = null;
        float closestDistance = float.MaxValue;

        foreach (var p in player)
        {
            float distanceToPlayerX = Mathf.Abs(p.position.x - transform.position.x);
            if (distanceToPlayerX < closestDistance)
            {
                closestDistance = distanceToPlayerX;
                closestPlayer = p;
            }
        }

        if (closestPlayer != null)
        {
            SetDestination2D(closestPlayer.position);
            // Si el jugador se aleja demasiado en X, volver a patrullar
            if (closestDistance > detectionRange * 1.5f)
            {
                currentState = EnemyState.Patrolling;
                return;
            }

            if (closestDistance < 3f)
            {
                StartCharge();
            }
        }
    }

    void StartCharge()
    {
        currentState = EnemyState.Charging;
        isCharging = true;
        chargeTimer = chargeDuration;
        agent.speed = chargeSpeed;

        // Activar el collider de ataque
        StartCoroutine(ChargeHitboxCoroutine());

        // Calcular dirección de carga solo en X
        Transform closestPlayer = null;
        float closestDistance = float.MaxValue;

        foreach (var p in player)
        {
            float distanceToPlayerX = Mathf.Abs(p.position.x - transform.position.x);
            if (distanceToPlayerX < closestDistance)
            {
                closestDistance = distanceToPlayerX;
                closestPlayer = p;
            }
        }

        if (closestPlayer != null)
        {
            float directionX = Mathf.Sign(closestPlayer.position.x - transform.position.x);
            chargeTarget = new Vector3(
                transform.position.x + directionX * chargeDistance,
                transform.position.y,
                initialZ
            );
            SetDestination2D(chargeTarget);
        }
    }

    private IEnumerator ChargeHitboxCoroutine()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
        yield return new WaitForSeconds(1f);
        attackCollider.enabled = false;
    }

    void Charge()
    {
        if (isCharging)
        {
            chargeTimer -= Time.deltaTime;
            
            if (chargeTimer <= 0 || (!agent.pathPending && agent.remainingDistance < 0.5f))
            {
                EndCharge();
            }
        }
    }

    void EndCharge()
    {
        isCharging = false;
        currentState = EnemyState.Recovering;
        recoveryTimer = recoveryTime;
        agent.speed = patrolSpeed;
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

        CanDealDamage();
    }

    void OnDrawGizmos()
    {
        // Dibujar rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dibujar ángulo de visión
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * detectionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Dibujar línea al jugador más cercano si está en rango
        if (player != null && player.Length > 0)
        {
            Transform closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var p in player)
            {
                float distanceToPlayerX = Mathf.Abs(p.position.x - transform.position.x);
                if (distanceToPlayerX < closestDistance)
                {
                    closestDistance = distanceToPlayerX;
                    closestPlayer = p;
                }
            }

            if (closestPlayer != null)
            {
                float directionX = Mathf.Sign(closestPlayer.position.x - transform.position.x);
                Vector3 directionToPlayer = new Vector3(directionX, 0, 0);
                Gizmos.color = canSeePlayer ? Color.green : Color.red;
                Gizmos.DrawRay(transform.position + Vector3.up * 1f, directionToPlayer * detectionRange);
            }
        }
    }

    // Método público para asignar puntos de patrulla desde el jefe
    public void SetPatrolPoints(Transform newPointA, Transform newPointB)
    {
        pointA = newPointA;
        pointB = newPointB;
        CheckPatrolPoints();
        
        if (hasPatrolPoints && currentState == EnemyState.Patrolling)
        {
            targetPoint = pointA;
            SetDestination2D(targetPoint.position);
        }
        else if (hasPatrolPoints && currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Patrolling;
            targetPoint = pointA;
            SetDestination2D(targetPoint.position);
        }
    }

    public bool CanDealDamage()
    {
        return canTakeDamage;
    }

    public void DisableDamage()
    {
        canTakeDamage = false;
    }
}