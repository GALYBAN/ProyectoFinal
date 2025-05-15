using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject platformManager; // Referencia al GameObject que contiene el PlatformManager
    [SerializeField] private string playerTag = "PoweredCharacter"; // Tag del jugador que puede interactuar
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject interactionPrompt; // Opcional: UI que muestra cuando se puede interactuar

    private bool playerInRange = false;
    private PlayerInputs playerInputs;

    // Start is called before the first frame update
    void Start()
    {
        // Asegurarse de que el PlatformManager esté desactivado al inicio
        if (platformManager != null)
        {
            platformManager.SetActive(false);
        }

        // Si hay un prompt de interacción, lo desactivamos al inicio
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerInputs = other.GetComponent<PlayerInputs>();
            
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
            
            // Opcional: Desactivar este objeto después de obtener el poder
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Platform Manager reference is missing!");
        }
    }
}
