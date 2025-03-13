using System.Collections;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private float comboResetTime = 1f;

    private int comboStep = 0;
    private float lastAttackTime;

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

    public void HandleCombo()
    {
        comboStep++;
        if (comboStep > 3)
        {
            ResetCombo();
            return;
        }

        movementController.Attack(comboStep);
        lastAttackTime = Time.time;
    }

    private void ResetCombo()
    {
        comboStep = 0;
    }
}
