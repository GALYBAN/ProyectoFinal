using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlatformOnTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject platformManager; // Referencia al GameObject que contiene el PlatformManager
    [SerializeField] private string playerTag = "Player"; // Tag del jugador que puede activar el PlatformManager

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra en el trigger tiene la etiqueta correcta
        if (other.CompareTag(playerTag))
        {
            // Verificar si el PlatformManager está inactivo
            if (platformManager != null && !platformManager.activeSelf)
            {
                platformManager.SetActive(true); // Activar el PlatformManager
                Debug.Log("PlatformManager has been activated by trigger.");
            }
            else if (platformManager == null)
            {
                Debug.LogWarning("Platform Manager reference is missing!");
            }
            else
            {
                Debug.Log("PlatformManager is already active.");
            }
        }
    }
}
