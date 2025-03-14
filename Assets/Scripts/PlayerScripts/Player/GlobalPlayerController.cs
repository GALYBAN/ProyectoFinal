using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{
    private Animator anim;
    private PlayerInputs inputs;
    private MovementController movementController;
    private PlayerGravity gravityController;

    private ComboController combos;
    [SerializeField] private CharacterController characterController;

    private ComboController comboController;
    //[SerializeField] private PhysicsMaterial physicsMaterial;

    void Awake()
    {
        combos = GetComponent<ComboController>();
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

        if (inputs.CrouchInput)
        {
            anim.SetBool("Crouch", true);
        }
        else if (!inputs.CrouchInput)
        {
            anim.SetBool("Crouch", false);
        }

        if (inputs.AttackInput && combos.OnAttackAnimationEnd() == true)
        {
            comboController.HandleCombo(new Vector3(inputs.HorizontalInput, 0, 0));
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
