using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private Vector3 dashSpeed = new Vector3 (0.0f, 0.0f, 0.0f);
    [SerializeField] private float dashForce = 10.0f;
    [SerializeField] private float dashDuration = 2.5f;
    [SerializeField] private int dashCount = 1;
    public Vector3 playerGravity;
    public float gravity = -9.81f;

    private GroundSensor groundSensor;

    private PlayerInputs inputs;    

    void Start()
    {
        inputs = GetComponent<PlayerInputs>();
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

    public void Dash(bool isDashing)
    {
        float horizontal = inputs.HorizontalInput;
        Vector3 direction = new Vector3(horizontal, 0, 0);

        if (isDashing && dashCount > 0)
        {
            dashSpeed = new Vector3(dashForce * transform.right.x, playerGravity.y, playerGravity.z);
            playerGravity =  dashSpeed;
            dashCount--;
            StartCoroutine(WaitForDash());
        }
    }

    IEnumerator WaitForDash()
    {
        yield return new WaitForSeconds(0.5f);
        playerGravity = new Vector3(0.0f, playerGravity.y, 0.0f);
        yield return new WaitForSeconds(dashDuration);
        dashCount = 1;
        Dash(isDashing: false);

    }
}
