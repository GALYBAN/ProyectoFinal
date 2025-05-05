using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DoorInteractionController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform teleportPoint; // Punto al que se teletransportará el jugador
    [SerializeField] private float interactionRadius = 2f; // Radio de interacción
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt; // UI que muestra "Presiona E"
    [SerializeField] private float promptHeight = 2f; // Altura del prompt sobre el trigger
    
    [Header("Events")]
    public UnityEvent onPlayerEnter; // Evento cuando el jugador entra en el trigger
    public UnityEvent onPlayerExit; // Evento cuando el jugador sale del trigger
    public UnityEvent onTeleport; // Evento cuando se realiza la teletransportación
    
    private bool isPlayerInRange = false;
    private GameObject currentPlayer;
    private PlayerInputs playerInputs;
    private bool isTeleporting = false; // Nueva variable para controlar el estado de teletransporte

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
            // Posicionar el prompt sobre el trigger
            interactionPrompt.transform.position = transform.position + Vector3.up * promptHeight;
        }
        
        if (teleportPoint == null)
        {
            Debug.LogError("Teleport Point not assigned!");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && playerInputs != null && playerInputs.InteractInput && !isTeleporting)
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayer = other.gameObject;
            playerInputs = currentPlayer.GetComponent<PlayerInputs>();
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            currentPlayer = null;
            playerInputs = null;
            isTeleporting = false; // Resetear el estado de teletransporte
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            onPlayerExit?.Invoke();
        }
    }

    private void TeleportPlayer()
    {
        if (currentPlayer != null && teleportPoint != null && !isTeleporting)
        {
            isTeleporting = true; // Marcar que estamos en proceso de teletransporte
            
            // Desactivar el CharacterController temporalmente para permitir la teletransportación
            CharacterController characterController = currentPlayer.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }
            
            // Teletransportar al jugador
            currentPlayer.transform.position = teleportPoint.position;
            currentPlayer.transform.rotation = teleportPoint.rotation;
            
            // Reactivar el CharacterController
            if (characterController != null)
            {
                characterController.enabled = true;
            }
            
            onTeleport?.Invoke();
            
            // Limpiar el estado después de un breve retraso
            StartCoroutine(ResetTeleportState());
        }
    }

    private IEnumerator ResetTeleportState()
    {
        yield return new WaitForSeconds(0.5f); // Esperar un momento antes de resetear
        isTeleporting = false;
        isPlayerInRange = false;
        currentPlayer = null;
        playerInputs = null;
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar el radio de interacción
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        
        // Dibujar el punto de teletransporte
        if (teleportPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(teleportPoint.position, 0.5f);
            Gizmos.DrawLine(transform.position, teleportPoint.position);
        }
    }
} 