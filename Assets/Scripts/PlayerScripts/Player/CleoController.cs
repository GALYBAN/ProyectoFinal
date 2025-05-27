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

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Referencia al AudioSource
    [SerializeField] private AudioClip walkSound; // Clip de sonido de caminar
    private bool isWalking = false; // Para controlar el estado de caminar

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInputs = GetComponentInParent<PlayerInputs>();

        // Asegúrate de que el AudioSource esté asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
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
        // Invertir el movimiento en el eje X
        Vector3 move3D = new Vector3(move, 0, 0);
        controller.Move(move3D * Time.deltaTime * playerSpeed);

        // Rotar el personaje según la dirección del movimiento
        if (move != 0)
        {
            // Calcular la rotación objetivo (180 grados si se mueve a la izquierda, 0 si se mueve a la derecha)
            float targetRotation = move > 0 ? 180f : 0f;
            // Rotación instantánea
            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        }

        // Estados de animación
        if (move != 0)
        {
            animator.SetBool("IsWalking", true);
            PlayWalkSound(); // Reproducir sonido de caminar
        }
        else
        {
            animator.SetBool("IsWalking", false);
            StopWalkSound(); // Detener sonido de caminar
        }

        // Cambiar la altura del jugador
        if (playerInputs.JumpPressed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void PlayWalkSound()
    {
        if (!audioSource.isPlaying && walkSound != null)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
    }

    private void StopWalkSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
