using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float groundedSpeed = 5f;
    [SerializeField] private float airSpeed = 2.5f;

    private float speed;
    private string lastKey;
    private bool isAttacking;

    private Animator anim;
    private CharacterController controller;
    private PlayerInputs inputs;
    private GroundSensor groundSensor;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<PlayerInputs>();
        groundSensor = GetComponent<GroundSensor>();

        anim.applyRootMotion = false; // Root Motion solo en ataques
    }

    public bool Move()
    {
        if (isAttacking) return false; // Bloquear movimiento si estamos atacando

        float horizontal = inputs.HorizontalInput;
        Vector3 direction = new Vector3(horizontal, 0, 0);

        if (direction != Vector3.zero)
        {
            if (groundSensor.IsGrounded())
            {
                speed = groundedSpeed;
                RotateToDirection(direction);
            }
            else
            {
                speed = airSpeed;
                AdjustAirSpeed(horizontal);
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

    public void Attack(int comboStep)
    {
        isAttacking = true;
        anim.applyRootMotion = true; // Activar root motion solo durante el ataque
        anim.SetTrigger("Attack" + comboStep);
        StartCoroutine(EndAttackAfterAnimation());
    }

    private IEnumerator EndAttackAfterAnimation()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
        anim.applyRootMotion = false; // Desactivar root motion después del ataque
    }
}

