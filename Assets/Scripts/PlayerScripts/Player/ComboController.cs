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

    [Header("Detección de enemigos")]
    [SerializeField] private float attackRange = 1.5f; // Distancia máxima de ataque
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

        // Si hay un enemigo cerca, desactiva Root Motion
        if (IsEnemyInRange())
        {
            anim.applyRootMotion = false;
            Debug.Log("Enemigo cerca, Root Motion desactivado");
        }
        else
        {
            anim.applyRootMotion = true;
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

    private bool IsEnemyInRange()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = -transform.right;

        if (Physics.Raycast(origin, direction, out hit, attackRange, enemyLayer))
        {
            Debug.DrawRay(origin, direction * attackRange, Color.green, 0.2f);
            return true;
        }

        Debug.DrawRay(origin, direction * attackRange, Color.red, 0.2f);
        return false;
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
    }

    public bool IsComboReady()
    {
        return canCombo;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Color de la línea del Raycast
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = -transform.right * attackRange;

        // Dibuja una línea para ver el rango de ataque
        Gizmos.DrawRay(origin, direction);

        // Dibuja una esfera en el extremo para visualizar el alcance máximo
        Gizmos.color = IsEnemyInRange() ? Color.green : Color.red;
        Gizmos.DrawSphere(origin + direction, 0.2f);
    }
}
