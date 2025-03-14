using System.Collections;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboMoveSpeedMultiplier = 0.3f;

    public int comboStep = 0;
    private float lastAttackTime;
    private bool canCombo = true; // Controla si se puede continuar el combo

    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        // Reset the combo if the time between attacks exceeds the reset time
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void HandleCombo(Vector3 attackDirection)
    {
        if (!canCombo) return;  // Si no se puede ejecutar el combo, salimos.

        comboStep++;
        if (comboStep > 3)
        {
            ResetCombo();
            return;
        }

        movementController.SetAttackState(true, comboMoveSpeedMultiplier, attackDirection);
        movementController.Attack(comboStep);
        lastAttackTime = Time.time;

        canCombo = false; // Evita que se ejecute un nuevo combo inmediatamente
    }

    // Método que se llama desde el evento de animación para continuar el combo
    public void OnAttackAnimationEnd()
    {
        canCombo = true;  // Permite continuar el combo cuando la animación haya terminado
    }

    // Este evento es para finalizar todo el combo
    public void ComboEnd()
    {
        ResetCombo();
    }

    public void ResetCombo()
    {
        comboStep = 0;
        movementController.SetAttackState(false, 1f, Vector3.zero);
        canCombo = true;  // Vuelve a habilitar el combo después de resetear
    }

    // Método adicional para verificar si el combo está listo desde otro script
    public bool IsComboReady()
    {
        return canCombo;
    }
}