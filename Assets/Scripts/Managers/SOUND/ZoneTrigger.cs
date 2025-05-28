using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra en la zona es el jugador
        if (other.CompareTag("Player") && !hasTriggered) // Asegúrate de que el jugador tenga la etiqueta "Player"
        {
            SOUNDManager.Instance.PlayMusic("Inicio"); // Reproducir la canción "Inicio"
            Debug.Log("Reproduciendo música: Inicio");
            hasTriggered = true;
        }
    }
}
