using UnityEngine;

public class WallTransparencyController : MonoBehaviour
{
    [Header("Wall Settings")]
    [SerializeField] private Renderer wallRenderer;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float targetAlpha = 0.3f;
    
    private Material wallMaterial;
    private Color originalColor;
    private float currentFadeTime = 0f;
    private bool isFading = false;
    private bool isFadingIn = false;
    private bool playerInTrigger = false;

    private void Start()
    {
        if (wallRenderer != null)
        {
            // Crear una instancia del material para no modificar el original
            wallMaterial = new Material(wallRenderer.material);
            wallRenderer.material = wallMaterial;
            
            // Guardar el color original
            originalColor = wallMaterial.color;
            
            // Asegurarse de que el material soporte transparencia
            wallMaterial.SetFloat("_Mode", 2); // Modo Fade
            wallMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            wallMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            wallMaterial.EnableKeyword("_ALPHABLEND_ON");
            wallMaterial.renderQueue = 3000;
        }
        else
        {
            Debug.LogError("Wall Renderer not assigned!");
        }
    }

    private void Update()
    {
        if (isFading)
        {
            HandleFade();
        }
        else if (playerInTrigger)
        {
            // Verificar si el jugador sigue en el trigger
            CheckPlayerInTrigger();
        }
    }

    private void CheckPlayerInTrigger()
    {
        // Buscar todos los objetos con tag Player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool playerFound = false;

        foreach (GameObject player in players)
        {
            if (IsPlayerInTrigger(player))
            {
                playerFound = true;
                break;
            }
        }

        if (!playerFound)
        {
            // Si no encontramos al jugador en el trigger, asumimos que se teletransportó
            playerInTrigger = false;
            StartFade(false);
        }
    }

    private bool IsPlayerInTrigger(GameObject player)
    {
        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null) return false;

        // Verificar si el jugador está dentro del trigger
        return GetComponent<Collider>().bounds.Intersects(playerCollider.bounds);
    }

    private void HandleFade()
    {
        currentFadeTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentFadeTime / fadeDuration);

        Color currentColor = wallMaterial.color;
        
        if (isFadingIn)
        {
            currentColor.a = Mathf.Lerp(1f, targetAlpha, progress);
        }
        else
        {
            currentColor.a = Mathf.Lerp(targetAlpha, 1f, progress);
        }
        
        wallMaterial.color = currentColor;

        if (progress >= 1f)
        {
            isFading = false;
            currentFadeTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            StartFade(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            StartFade(false);
        }
    }

    private void StartFade(bool fadeIn)
    {
        isFading = true;
        isFadingIn = fadeIn;
        currentFadeTime = 0f;
    }

    private void OnDestroy()
    {
        // Restaurar el color original al destruir el objeto
        if (wallMaterial != null)
        {
            wallMaterial.color = originalColor;
        }
    }
} 