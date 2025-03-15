using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{
    private Animator anim;
    private PlayerInputs inputs;
    private MovementController movementController;
    private PlayerGravity gravityController;
    private GroundSensor groundSensor;
    private ComboController comboController;
    [SerializeField] private CharacterController characterController;

    private bool attackProcessed = false; // Track if attack input has been processed

    void Awake()
    {
        groundSensor = GetComponent<GroundSensor>();
        comboController = GetComponent<ComboController>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        inputs = GetComponent<PlayerInputs>();
        movementController = GetComponent<MovementController>();
        gravityController = GetComponent<PlayerGravity>();
    }

    void Update()
    {
        if (inputs.JumpPressed)
        {
            gravityController.Jump();

            if (inputs.HorizontalInput < 0)
            {
                movementController.SetLastKey("left");
            }
            else if (inputs.HorizontalInput > 0)
            {
                movementController.SetLastKey("right");
            }
        }

        if (inputs.DashInput)
        {
            gravityController.Dash(isDashing: true);
        }

        if (inputs.CrouchInput && groundSensor.IsGrounded())
        {
            movementController.Crouch(true);
            anim.SetBool("Crouch", true);
        }
        else if (!inputs.CrouchInput)
        {
            movementController.Crouch(false);
            anim.SetBool("Crouch", false);
        }

        // Handle attack input with a flag to prevent spamming
        if (inputs.AttackInput && comboController.IsComboReady() && !attackProcessed)
        {
            comboController.HandleCombo(new Vector3(inputs.HorizontalInput, 0, 0));
            attackProcessed = true;
        }
        else if (!inputs.AttackInput)
        {
            attackProcessed = false; // Reset the flag when the attack input is released
        }

        movementController.Move();
        gravityController.ApplyGravity(GetComponent<CharacterController>());
    }
}