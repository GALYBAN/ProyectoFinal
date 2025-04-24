using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleoController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    private Animator animator;
    private PlayerInputs playerInputs;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInputs = GetComponent<PlayerInputs>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Use PlayerInputs instead of direct Input
        float move = playerInputs.HorizontalInput;
        // Invert X axis movement
        Vector3 move3D = new Vector3(move, 0, 0);
        controller.Move(move3D * Time.deltaTime * playerSpeed);

        // Rotate character based on movement direction
        if (move != 0)
        {
            // Calculate target rotation (180 degrees if moving left, 0 if moving right)
            float targetRotation = move > 0 ? 180f : 0f;
            // Instant rotation
            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        }

        // Animation states
        if (move != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // Changes the height position of the player
        if (playerInputs.JumpPressed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Set grounded animation parameter
        animator.SetBool("IsGrounded", groundedPlayer);
    }
}
