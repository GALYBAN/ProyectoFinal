using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject platformManager; // Referencia al GameObject que contiene el PlatformManager
    [SerializeField] private string playerTag = "PoweredCharacter"; // Tag del jugador que puede interactuar
    [SerializeField] private GameObject scrollObject; // Referencia al pergamino
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject interactionPrompt; // Opcional: UI que muestra cuando se puede interactuar
    [SerializeField] private GameObject controlCanvas;

    private bool playerInRange = false;
    private PlayerInputs playerInputs;

    // Variables para la animación del pergamino
    [Header("Scroll Animation Settings")]
    [SerializeField] private float moveDistance = 0.1f; // Distancia de movimiento
    [SerializeField] private float moveSpeed = 2f; // Velocidad de movimiento
    [SerializeField] private float rotationSpeed = 50f; // Velocidad de rotación

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Si hay un prompt de interacción, lo desactivamos al inicio
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Guardar la posición original del pergamino
        if (scrollObject != null)
        {
            originalPosition = scrollObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && playerInputs != null)
        {
            // Verificar si el jugador presiona el botón de interacción
            if (playerInputs.InteractInput)
            {
                ActivatePlatformPower();
            }
        }

        // Animar el pergamino
        AnimateScroll();
    }

    private void AnimateScroll()
    {
        if (scrollObject != null)
        {
            // Movimiento de subir y bajar
            float newY = originalPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            scrollObject.transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

            // Rotación aleatoria
            scrollObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerInputs = GameObject.Find("PlayerReference").GetComponent<PlayerInputs>();
            
            // Mostrar el prompt de interacción si existe
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerInputs = null;
            
            // Ocultar el prompt de interacción si existe
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void ActivatePlatformPower()
    {
        if (platformManager != null)
        {
            platformManager.SetActive(true);
            
            // Activar el canvas de control
            ActivateControlCanvas();
            
            // Opcional: Desactivar este objeto después de obtener el poder
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Platform Manager reference is missing!");
        }
    }

    private void ActivateControlCanvas()
    {
        controlCanvas.SetActive(true);
    }
}
