using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject platformPrefab; // Prefab de la plataforma
    [SerializeField] private LayerMask collisionLayers; // Capas que bloquearán la creación

    private GameObject currentPlatform; // Plataforma actualmente activa
    private bool isPlacingPlatform = false; // Indica si estás colocando la plataforma
    private GlobalPlayerController playerController; // Referencia al controlador del jugador
    private PlayerInputs playerInputs;


    void Awake()
    {
        playerController = FindObjectOfType<GlobalPlayerController>();
        playerInputs = FindObjectOfType<PlayerInputs>();    
    }

    void Update()
    {
        if (playerInputs.TogglePlatformInput) // Botón para crear o alternar la plataforma
        {
            TogglePlatform();
        }

        if (isPlacingPlatform && currentPlatform != null)
        {
            MovePlatform();
            if (playerInputs.ConfirmPlatformPlacement) // Confirma la posición con Enter
            {
                PlacePlatform();
            }
        }
    }

    void TogglePlatform()
    {
        if (currentPlatform != null)
        {
            Destroy(currentPlatform); // Destruye la plataforma existente
            currentPlatform = null;
            isPlacingPlatform = false;
            playerController.enabled = true; // Reactiva el control del jugador
        }
        else
        {
            CreatePlatform();
        }
    }

    void CreatePlatform()
    {
        // Desactiva el movimiento del jugador
        playerController.enabled = false;

        // Instancia la plataforma en la posición inicial (frente al jugador)
        Vector3 spawnPosition = playerController.transform.position + new Vector3(2, 0, 0);
        currentPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

        // Ignorar colisión con el jugador
        Collider platformCollider = currentPlatform.GetComponent<Collider>();
        Collider playerCollider = playerController.GetComponent<Collider>();
        Physics.IgnoreCollision(platformCollider, playerCollider, true);

        isPlacingPlatform = true;
    }

    void MovePlatform()
    {
        // Obtén la posición del mouse o las teclas para mover la plataforma
        Vector3 newPosition = currentPlatform.transform.position;

        // Mueve la plataforma según los inputs de WASD o flechas
        newPosition.x += playerInputs.PlatformMovementInput.x * 5f * Time.deltaTime;
        newPosition.y += playerInputs.PlatformMovementInput.y * 5f * Time.deltaTime;

        // Comprueba si la nueva posición está libre de colisiones
        Collider[] hits = Physics.OverlapBox(newPosition, currentPlatform.transform.localScale / 2, Quaternion.identity, collisionLayers);

        if (hits.Length == 0) // Solo mueve la plataforma si no hay colisiones
        {
            currentPlatform.transform.position = newPosition;
        }
    }

    void PlacePlatform()
    {
        Collider platformCollider = currentPlatform.GetComponent<Collider>();
        Collider playerCollider = playerController.GetComponent<Collider>();
        Physics.IgnoreCollision(platformCollider, playerCollider, false);

        isPlacingPlatform = false;
        playerController.enabled = true; // Reactiva el movimiento del jugador
    }
}
