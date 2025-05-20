using System.Collections;
using UnityEngine;

/// <summary>
/// Gestor central del sistema de reaparición
/// Implementa el patrón Singleton para acceso global
/// </summary>
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }
    
    [Header("Configuración")]
    [Tooltip("Punto de reaparición por defecto si no se ha activado ninguno")]
    [SerializeField] private RespawnPoint defaultRespawnPoint;
    
    [Tooltip("Posición de respawn temporal (usar esto como último recurso)")]
    [SerializeField] private Transform temporalRespawnPosition;
    
    [Tooltip("Tiempo de fade a negro al reaparecer")]
    [SerializeField] private float respawnFadeTime = 0.5f;
    
    [Tooltip("Tiempo que permanece la pantalla en negro")]
    [SerializeField] private float blackScreenDuration = 0.5f;
    
    [Header("Referencias")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    
    // Último punto de reaparición activado
    private RespawnPoint lastRespawnPoint;
    
    // Controla si estamos en proceso de reaparición
    private bool isRespawning = false;
    
    // Referencia al CharacterTransitionManager para manejar personajes
    private CharacterTransitionManager characterManager;
    
    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Inicializar el punto de reaparición por defecto
        lastRespawnPoint = defaultRespawnPoint;
    }
    
    private void Start()
    {
        // Encontrar el CharacterTransitionManager
        characterManager = FindObjectOfType<CharacterTransitionManager>();
        
        // Inicializar el canvas de fade si no está asignado
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("RespawnManager: No se ha asignado el CanvasGroup para el fade. Los respawns no tendrán efecto visual.");
            
            // Intentar encontrar uno en la escena
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            if (fadeCanvasGroup != null)
            {
                Debug.Log("RespawnManager: Se encontró automáticamente un CanvasGroup en la escena.");
            }
        }
        
        // Ocultar el canvas de fade al inicio
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
    
    /// <summary>
    /// Establece el último punto de reaparición activo
    /// </summary>
    public void SetLastRespawnPoint(RespawnPoint respawnPoint)
    {
        if (respawnPoint != null)
        {
            lastRespawnPoint = respawnPoint;
            Debug.Log($"Punto de reaparición actualizado: {respawnPoint.GetRespawnPointId()}");
        }
    }
    
    /// <summary>
    /// Inicia el proceso de reaparición del jugador
    /// </summary>
    public void RespawnPlayer()
    {
        if (isRespawning)
        {
            Debug.Log("Ya hay un proceso de reaparición en curso");
            return;
        }
        
        if (lastRespawnPoint == null)
        {
            Debug.LogError("No hay un punto de reaparición configurado");
            return;
        }
        
        StartCoroutine(RespawnPlayerCoroutine());
    }
    
    // Método para resetear la gravedad del personaje
    private void ResetPlayerGravity(GameObject player)
    {
        if (player == null) return;
        
        // Buscar y resetear el componente PlayerGravity
        PlayerGravity gravityController = player.GetComponent<PlayerGravity>();
        if (gravityController != null)
        {
            Debug.Log($"Reseteando gravedad para {player.name}");
            // Resetear la gravedad
            gravityController.playerGravity = new Vector3(0, -1, 0);
        }
        
        // También resetear cualquier Rigidbody por si acaso
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
        }
    }
    
    private IEnumerator RespawnPlayerCoroutine()
    {
        isRespawning = true;
        Debug.Log("Iniciando proceso de reaparición...");
        
        // Fade a negro
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsedTime = 0;
            while (elapsedTime < respawnFadeTime)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / respawnFadeTime);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1;
        }
        
        // Esperar un momento con la pantalla en negro
        yield return new WaitForSeconds(blackScreenDuration);
        
        // Obtener la posición de respawn con verificación de seguridad
        Vector3 respawnPosition;
        
        // VERIFICAR SI TENEMOS UN PUNTO DE RESPAWN VÁLIDO
        if (lastRespawnPoint != null)
        {
            respawnPosition = lastRespawnPoint.GetRespawnPosition();
            Debug.Log($"Usando punto de respawn registrado: {lastRespawnPoint.GetRespawnPointId()}");
        }
        else if (defaultRespawnPoint != null)
        {
            respawnPosition = defaultRespawnPoint.GetRespawnPosition();
            Debug.Log("Usando punto de respawn por defecto");
        }
        else if (temporalRespawnPosition != null)
        {
            respawnPosition = temporalRespawnPosition.position;
            Debug.Log("Usando posición de respawn temporal");
        }
        else
        {
            // Último recurso: posición elevada (para evitar caer de nuevo al vacío)
            respawnPosition = new Vector3(0, 10, 0); // Altura segura
            Debug.LogWarning("No se encontró ningún punto de respawn. Usando posición de emergencia.");
        }
        
        Debug.Log($"Posición de respawn final: {respawnPosition}");
        
        // MÉTODO EXTREMADAMENTE DIRECTO
        // Reposicionar directamente a todos los jugadores y personajes posibles
        
        // 1. Reposicionar personajes usando CharacterTransitionManager
        if (characterManager != null)
        {
            Debug.Log("Usando CharacterTransitionManager");
            GameObject cleo = characterManager.GetUnpoweredCharacter();
            GameObject powered = characterManager.GetPoweredCharacter();
            bool isTransformed = characterManager.IsTransformed();
            
            // Resetear la gravedad en ambos personajes
            ResetPlayerGravity(cleo);
            ResetPlayerGravity(powered);
            
            // Verificar cuál está activo y reposicionar ambos de todos modos
            if (cleo != null)
            {
                cleo.transform.position = respawnPosition;
                Debug.Log("Cleo reposicionado");
            }
            
            if (powered != null)
            {
                powered.transform.position = respawnPosition;
                Debug.Log("Personaje powered reposicionado");
            }
            
            // Usar el método SetTransformed para asegurar la posición correcta y estado
            characterManager.SetTransformed(isTransformed, respawnPosition);
            Debug.Log($"SetTransformed({isTransformed}, {respawnPosition})");
        }
        
        // 2. Reposicionar todos los objetos con tag Player por seguridad
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            // Resetear gravedad primero
            ResetPlayerGravity(player);
            
            // Reposicionar
            player.transform.position = respawnPosition;
            Debug.Log($"Reposicionado Player: {player.name}");
            
            // Desactivar y reactivar cualquier controlador que pueda interferir
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
            }
            
            // Resetear scripts relacionados con movimiento
            MovementController moveController = player.GetComponent<MovementController>();
            if (moveController != null)
            {
                Debug.Log($"Reseteando MovementController para {player.name}");
            }
        }
        
        // 3. Llamada directa a teleport para CharacterController
        CharacterController[] allControllers = FindObjectsOfType<CharacterController>();
        foreach (CharacterController controller in allControllers)
        {
            // Resetear la gravedad si tiene PlayerGravity
            ResetPlayerGravity(controller.gameObject);
            
            // Teleport
            if (controller.enabled)
            {
                controller.enabled = false;
                controller.transform.position = respawnPosition;
                controller.enabled = true;
                Debug.Log($"Teleportado controller: {controller.gameObject.name}");
            }
            else
            {
                // Incluso si está deshabilitado, reposicionarlo
                controller.transform.position = respawnPosition;
            }
            
            // Asegurarse de que no tenga una velocidad residual
            PlayerGravity gravityController = controller.GetComponent<PlayerGravity>();
            if (gravityController != null)
            {
                // Asegurarse absolutamente de que la gravedad se resetea
                gravityController.playerGravity = new Vector3(0, -1, 0);
                Debug.Log($"Gravedad reseteada para {controller.gameObject.name}");
            }
        }
        
        // Fade out (volver a transparente)
        if (fadeCanvasGroup != null)
        {
            float elapsedTime = 0;
            while (elapsedTime < respawnFadeTime)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / respawnFadeTime);
                yield return null;
            }
            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.blocksRaycasts = false;
        }
        
        isRespawning = false;
        Debug.Log("Proceso de reaparición completado");
    }
    
    /// <summary>
    /// Activa un punto de reaparición por su ID
    /// </summary>
    public void ActivateRespawnPointById(string respawnPointId)
    {
        if (string.IsNullOrEmpty(respawnPointId))
        {
            Debug.LogError("Se intentó activar un punto de reaparición con ID nulo o vacío");
            return;
        }
        
        RespawnPoint[] allPoints = FindObjectsOfType<RespawnPoint>();
        foreach (RespawnPoint point in allPoints)
        {
            if (point.GetRespawnPointId() == respawnPointId)
            {
                SetLastRespawnPoint(point);
                return;
            }
        }
        
        Debug.LogWarning($"No se encontró ningún punto de reaparición con ID: {respawnPointId}");
    }
}
