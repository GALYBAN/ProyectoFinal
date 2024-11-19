using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3.0f;
    private Vector3 playerGravity;

    private GroundSensor groundSensor;

    void Start()
    {
        groundSensor = GetComponent<GroundSensor>();
    }

    public void ApplyGravity(CharacterController controller)
    {
        if (!groundSensor.IsGrounded())
        {
            playerGravity.y += gravity * Time.deltaTime;
        }
        else if (groundSensor.IsGrounded() && playerGravity.y < 0)
        {
            playerGravity.y = -1;
        }

        controller.Move(playerGravity * Time.deltaTime);
    }

    public void Jump()
    {
        if (groundSensor.IsGrounded())
        {
            playerGravity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
}
