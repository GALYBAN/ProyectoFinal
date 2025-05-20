using UnityEngine;

/// <summary>
/// Define un punto de reaparición donde el jugador volverá si cae al vacío.
/// Se debe adjuntar a un objeto con collider configurado como trigger.
/// </summary>
public class RespawnPoint : MonoBehaviour
{
    [Tooltip("ID único para este punto de reaparición")]
    [SerializeField] private string respawnPointId;
    
    [Tooltip("Si está activo, al entrar en este trigger se registrará como último punto de reaparición")]
    [SerializeField] private bool activateOnTriggerEnter = true;
    
    [Tooltip("Transform que define la posición exacta donde reaparecerá el jugador. Si es null, usará la posición de este objeto")]
    [SerializeField] private Transform spawnPosition;
    
    private void Start()
    {
        // Asegurar que tiene un collider configurado como trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"RespawnPoint '{respawnPointId}' - El collider debe estar configurado como trigger");
            col.isTrigger = true;
        }
        else if (col == null)
        {
            Debug.LogError($"RespawnPoint '{respawnPointId}' - No tiene un collider adjunto. Añade un BoxCollider, SphereCollider, etc.");
        }
        
        // Si no tiene ID, generar uno automáticamente
        if (string.IsNullOrEmpty(respawnPointId))
        {
            respawnPointId = System.Guid.NewGuid().ToString();
            Debug.Log($"RespawnPoint generó ID automática: {respawnPointId}");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by {other.name}, tiene tag Player? {other.CompareTag("Player")}");
        
        if (activateOnTriggerEnter && other.CompareTag("Player"))
        {
            Debug.Log($"Intentando registrar punto de respawn: {respawnPointId}");
            
            // Verificar que el RespawnManager existe
            if (RespawnManager.Instance != null)
            {
                // Registrar este punto como el último punto de reaparición
                RespawnManager.Instance.SetLastRespawnPoint(this);
                Debug.Log($"Punto de reaparición activado exitosamente: {respawnPointId}");
            }
            else
            {
                Debug.LogError("No se pudo encontrar RespawnManager.Instance. ¿Está inicializado el Singleton?");
            }
        }
    }
    
    /// <summary>
    /// Activa manualmente este punto de reaparición
    /// </summary>
    public void ActivateRespawnPoint()
    {
        RespawnManager.Instance.SetLastRespawnPoint(this);
        Debug.Log($"Punto de reaparición activado manualmente: {respawnPointId}");
    }
    
    /// <summary>
    /// Obtiene la posición donde debería reaparecer el jugador
    /// </summary>
    public Vector3 GetRespawnPosition()
    {
        // Si hay un Transform hijo definido para la posición de respawn, usarlo
        if (spawnPosition != null)
        {
            return spawnPosition.position;
        }
        
        // Si no, usar la posición del objeto principal
        return transform.position;
    }
    
    /// <summary>
    /// Obtiene el ID de este punto de reaparición
    /// </summary>
    public string GetRespawnPointId()
    {
        return respawnPointId;
    }
}
