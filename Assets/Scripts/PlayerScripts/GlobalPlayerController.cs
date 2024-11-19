using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{
    private PlayerInputs inputs;
    private MovementController movementController;
    private PlayerGravity gravityController;

    void Awake()
    {
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

        movementController.Move();
        gravityController.ApplyGravity(GetComponent<CharacterController>());
    }
}
