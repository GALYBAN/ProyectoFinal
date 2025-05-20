using UnityEngine;

/// <summary>
/// Detector de caídas al vacío que activa el sistema de reaparición.
/// Debe colocarse como un collider trigger por debajo del nivel para detectar caídas.
/// </summary>
public class VoidDetector : MonoBehaviour
{
    [Tooltip("Segundos de espera antes de activar el respawn")]
    [SerializeField] private float respawnDelay = 0.5f;
    
    private void Start()
    {
        // Asegurar que tiene un collider configurado como trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("VoidDetector - El collider debe estar configurado como trigger");
            col.isTrigger = true;
        }
        else if (col == null)
        {
            Debug.LogError("VoidDetector - No tiene un collider adjunto. Añade un BoxCollider, SphereCollider, etc.");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el jugador quien cae
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado cayendo al vacío");
            
            // Esperar un momento antes de activar respawn
            StartCoroutine(RespawnWithDelay());
        }
    }
    
    private System.Collections.IEnumerator RespawnWithDelay()
    {
        // Esperar el tiempo configurado
        if (respawnDelay > 0)
        {
            yield return new WaitForSeconds(respawnDelay);
        }
        
        // Activar sistema de reaparición
        if (RespawnManager.Instance != null)
        {
            Debug.Log("Activando reaparición...");
            RespawnManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("No se encuentra RespawnManager en la escena");
        }
    }
}
