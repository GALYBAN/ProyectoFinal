using System.Collections;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboMoveSpeedMultiplier = 0.3f; // Velocidad reducida durante el combo

    private int comboStep = 0;
    private float lastAttackTime;
    private bool canCombo = true; // Controla si se puede continuar el combo

    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void HandleCombo(Vector3 attackDirection)
    {
        comboStep++;
        if (comboStep > 3)
        {
            ResetCombo();
            return;
        }

        movementController.SetAttackState(true, comboMoveSpeedMultiplier, attackDirection);
        movementController.Attack(comboStep);
        lastAttackTime = Time.time;

        // Bloquea el combo hasta que la animación termine
        canCombo = false;
    }

    // Este método será llamado desde otro script para saber si la animación ha terminado
    public bool OnAttackAnimationEnd()
    {
        Debug.Log("OnAttackAnimationEnd()");
        canCombo = true;
        return canCombo;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        movementController.SetAttackState(false, 1f, Vector3.zero);
    }
}

