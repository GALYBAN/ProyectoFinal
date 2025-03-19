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

    [Header("Cooldowns")]
    [SerializeField] private float upAttackCooldown = 2f; // Cooldown para ataques hacia arriba
    private float lastUpAttackTime;

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

        // Detectar si el jugador está mirando hacia arriba
        bool lookingUp = inputs.LookUpInput;
        anim.SetBool("Up", lookingUp);
    }

    public void HandleCombo(Vector3 attackDirection)
    {
        if (!canCombo) return;

        // Ataque hacia arriba con cooldown
        if (anim.GetBool("Up"))
        {
            if (Time.time - lastUpAttackTime < upAttackCooldown) 
            {
                Debug.Log("UpAttack en cooldown...");
                return; // Evita ejecutar el ataque si está en cooldown
            }

            anim.SetTrigger("UpAttack");
            Debug.Log("Ataque hacia arriba ejecutado");

            lastUpAttackTime = Time.time; // Actualiza el cooldown
            return;
        }

        // Lógica normal de combo
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
