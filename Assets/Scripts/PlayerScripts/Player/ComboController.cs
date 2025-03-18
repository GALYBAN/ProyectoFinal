using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboMoveSpeedMultiplier = 0.3f;
    [SerializeField] private PlayerStats playerStats; // Referencia a PlayerStats para regenerar maná
    [SerializeField] private float baseDamage = 10f; // Daño base del primer ataque
    [SerializeField] private float damageMultiplier = 1.2f; // Factor de incremento de daño

    public int comboStep = 0;
    private float lastAttackTime;
    private bool canCombo = true;
    private int successfulHits = 0; // Contador de golpes acertados

    void Start()
    {
        movementController = GetComponent<MovementController>();
        playerStats = GetComponent<PlayerStats>(); // Obtener PlayerStats
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

        movementController.SetAttackState(true, comboMoveSpeedMultiplier, attackDirection);
        movementController.Attack(comboStep);
        lastAttackTime = Time.time;

        canCombo = false; // Bloquea el combo hasta que termine la animación
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log("Animación de ataque finalizada, combo disponible nuevamente");
        canCombo = true;
    }

    public void ApplyDamage(EnemyStats enemy)
    {
        if (enemy == null) return;

        float damage = baseDamage * Mathf.Pow(damageMultiplier, comboStep - 1); // Escala el daño con el comboStep
        enemy.TakeDamage(damage);

        Debug.Log($"Golpe {comboStep}: Aplicado {damage} de daño al enemigo");

        successfulHits++;
        if (successfulHits >= 3)
        {
            playerStats.RegenerateManaSlot(); // Recuperar maná cada 3 golpes acertados
            Debug.Log("¡Maná regenerado!");
            successfulHits = 0;
        }
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
}
