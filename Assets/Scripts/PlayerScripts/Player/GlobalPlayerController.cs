using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{
    private PlayerInputs inputs;
    private MovementController movementController;
    private PlayerGravity gravityController;
    [SerializeField] private CharacterController characterController;
    //[SerializeField] private PhysicsMaterial physicsMaterial;

    void Awake()
    {
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

        if (inputs.CrouchInput)
        {
            movementController.Crouch(crouching: true);
        }
        else if (!inputs.CrouchInput)
        {
            movementController.Crouch(crouching: false);
        }

        movementController.Move();
        gravityController.ApplyGravity(GetComponent<CharacterController>());
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Cambiar la fricción según la superficie o el material
        if (hit.collider.CompareTag("SlipperySurface"))
        {
            // Ajusta el comportamiento de fricción si estás en una superficie resbaladiza
            characterController.material.dynamicFriction = 0.2f;
            characterController.material.staticFriction = 0.2f;
        }
        else
        {
            // Físicas por defecto
            characterController.material.dynamicFriction = 0.6f;
            characterController.material.staticFriction = 0.6f;
        }
    }*/

}
