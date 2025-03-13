using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool TogglePlatformInput { get; private set; } // Activar/desactivar plataforma
    public Vector2 PlatformMovementInput { get; private set; } // Movimiento de la plataforma
    public bool ConfirmPlatformPlacement { get; private set; } // Confirmar ubicación
    public bool DashInput { get; private set; }
    public bool CrouchInput { get; private set; }
    public bool SlashInput { get; private set; }
    public bool AttackInput { get; private set; }
    
    private bool isPressed = false;
    void Update()
    {
        float rightPad = Input.GetAxisRaw("Place");

        HorizontalInput = Input.GetAxisRaw("Horizontal");
        JumpPressed = Input.GetButtonDown("Jump");
        DashInput = Input.GetButtonDown("Dash");
        CrouchInput = Input.GetButton("Crouch");
        SlashInput = Input.GetButtonDown("Slash");
        AttackInput = Input.GetButtonDown("Attack");



        TogglePlatformInput = Input.GetButtonDown("Platform");
        if(rightPad > 0.5f && !isPressed) 
        {
            TogglePlatformInput = true;
            isPressed = true;
        }
        else if(rightPad <= 0.5f)
        {
            isPressed = false;
            TogglePlatformInput = false;
        }
        PlatformMovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // WASD o flechas
        ConfirmPlatformPlacement = Input.GetButtonDown("Jump");
    }
}
