using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float groundedSpeed = 5f;
    [SerializeField] private float airSpeed = 2.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f; // Reducirá la velocidad al 50% al agacharse

    private float speed;
    private float speedMultiplier = 1f;
    private string lastKey;
    private bool isAttacking;
    private bool isCrouching; // Nueva variable para detectar si está agachado
    private Vector3 attackDirection;

    private ComboController comboController;
    private Animator anim;
    private CharacterController controller;
    private PlayerInputs inputs;
    private GroundSensor groundSensor;
    
    void Start()
    {
        comboController = GetComponent<ComboController>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<PlayerInputs>();
        groundSensor = GetComponent<GroundSensor>();
    }

    public bool Move()
    {
        float horizontal = inputs.HorizontalInput;
        Vector3 direction = new Vector3(horizontal, 0, 0);

        if (isAttacking && Vector3.Dot(direction, attackDirection) < 0)
        {
            direction = Vector3.zero;
        }

        if (direction != Vector3.zero)
        {
            if (groundSensor.IsGrounded())
            {
                speed = groundedSpeed * speedMultiplier;
                RotateToDirection(direction);
            }
            else
            {
                speed = airSpeed * speedMultiplier;
                AdjustAirSpeed(horizontal);
            }

            // Aplicar reducción de velocidad si está agachado
            if (isCrouching)
            {
                speed *= crouchSpeedMultiplier;
            }

            controller.Move(direction * speed * Time.deltaTime);
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        return direction != Vector3.zero;
    }

    private void RotateToDirection(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }

    private void AdjustAirSpeed(float horizontal)
    {
        if ((lastKey == "left" && horizontal > 0) || (lastKey == "right" && horizontal < 0))
        {
            speed = airSpeed;
        }
        else
        {
            speed = groundedSpeed;
        }
    }

    public void SetLastKey(string key)
    {
        lastKey = key;
    }

    public void EndAttackAfterAnimation()
    {
        isAttacking = false;
        speedMultiplier = 1f;
        attackDirection = Vector3.zero;

        if (comboController.comboStep == 3)
        {
            comboController.ComboEnd();
        }
    }

    public void Attack(int comboStep)
    {
        isAttacking = true;
        anim.SetTrigger("Attack" + comboStep);
    }

    public void SetAttackState(bool attacking, float multiplier, Vector3 direction)
    {
        isAttacking = attacking;
        speedMultiplier = multiplier;
        attackDirection = direction.normalized;
    }

    // Método para activar o desactivar el estado de agachado
    public void Crouch(bool crouching)
    {
        isCrouching = crouching;
        anim.SetBool("Crouch", crouching);
    }
}
