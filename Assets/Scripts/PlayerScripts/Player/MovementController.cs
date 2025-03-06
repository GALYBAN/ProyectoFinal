using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float groundedSpeed = 5f;
    [SerializeField] private float airSpeed = 2.5f;

    private float speed;
    private string lastKey;

    private CharacterController controller;
    private PlayerInputs inputs;
    private GroundSensor groundSensor;
    private Animator anim;
    
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<PlayerInputs>();
        groundSensor = GetComponent<GroundSensor>();
    }

    public void Move()
    {
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
        }
    }

    private void RotateToDirection(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
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

    public void Crouch(bool crouching)
    {
        anim.SetBool("Agachado", crouching);
    }
}
