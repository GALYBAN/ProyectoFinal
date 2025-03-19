using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator anim;
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboMoveSpeedMultiplier = 0.3f;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float damageMultiplier = 1.2f;

    [Header("Detección de estados")]
    [SerializeField] private GroundSensor groundSensor;
    [SerializeField] private PlayerInputs inputs;

    [Header("Detección de enemigos")]
    [SerializeField] private float attackRange = 1.5f; // Rango del ataque
    [SerializeField] private LayerMask enemyLayer; // Capa para detectar enemigos

    public int comboStep = 0;
    private float lastAttackTime;
    private bool canCombo = true;
    private int successfulHits = 0;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        playerStats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        groundSensor = GetComponent<GroundSensor>();
        inputs = GetComponent<PlayerInputs>();
    }

    void Update()
    {
        if (comboStep > 0 && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void HandleCombo(Vector3 attackDirection)
    {
        if (!canCombo) return;

        comboStep++;
        if (comboStep > 3)
        {
            ResetCombo();
            return;
        }

        Debug.Log($"Ejecutando combo step {comboStep}");

        // Si hay un enemigo en rango, desactivar Root Motion
        if (IsEnemyInFront())
        {
            anim.applyRootMotion = false;
            Debug.Log("Enemigo en rango, Root Motion desactivado.");
        }

        movementController.SetAttackState(true, comboMoveSpeedMultiplier, attackDirection);
        movementController.Attack(comboStep);
        lastAttackTime = Time.time;
        canCombo = false;
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log("Animación de ataque finalizada, combo disponible nuevamente");

        canCombo = true;
        anim.applyRootMotion = true; // Reactivar Root Motion después del ataque
    }

    public void ApplyDamage(EnemyStats enemy)
    {
        if (enemy == null) return;

        float damage = baseDamage * Mathf.Pow(damageMultiplier, comboStep - 1);
        enemy.TakeDamage(damage);

        Debug.Log($"Golpe {comboStep}: Aplicado {damage} de daño al enemigo");

        successfulHits++;
        if (successfulHits >= 3)
        {
            playerStats.RegenerateManaSlot();
            Debug.Log("¡Maná regenerado!");
            successfulHits = 0;
        }
    }

    private bool IsEnemyInFront()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1f; // Ray desde el pecho
        Vector3 direction = -transform.right; // Mirando hacia adelante

        bool enemyDetected = Physics.Raycast(origin, direction, out hit, attackRange, enemyLayer);

        Debug.DrawRay(origin, direction * attackRange, enemyDetected ? Color.green : Color.red, 0.2f);

        if (enemyDetected)
        {
            Debug.Log($"Enemigo detectado en rango: {hit.collider.gameObject.name}");
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = -transform.right * attackRange;

        Gizmos.DrawRay(origin, direction);
        Gizmos.color = IsEnemyInFront() ? Color.green : Color.red;
        Gizmos.DrawSphere(origin + direction, 0.2f);
    }

    public void ComboEnd()
    {
        ResetCombo();
    }

    public void ResetCombo()
    {
        if (comboStep == 0) return;

        Debug.Log("Reseteando combo");
        comboStep = 0;
        movementController.SetAttackState(false, 1f, Vector3.zero);
        canCombo = true;

        anim.applyRootMotion = true; // Asegurar que Root Motion vuelva a activarse al resetear el combo
    }

    public bool IsComboReady()
    {
        return canCombo;
    }
}
