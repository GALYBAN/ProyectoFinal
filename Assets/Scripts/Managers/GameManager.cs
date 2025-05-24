using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isPaused = false;
    
    // Referencia al menú de pausa
    public GameObject pauseMenu; // Asegúrate de asignar esto en el Inspector
    private PlayerInputs playerInput;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // Cargar la partida guardada al inicio
        LoadPlayerData();
        
        if (GameObject.Find("CleoArmature") != null)
        {
            playerInput = GameObject.Find("CleoArmature").GetComponent<PlayerInputs>();
        }
        else
        {
            playerInput = GameObject.Find("CleoTArmature").GetComponent<PlayerInputs>();
        }
        
    }

    private void Update()
    {
        // Detectar si se presiona la tecla ESC
        if (playerInput.PauseInput)
        {
            Debug.Log("Pausa: Tecla ESC presionada");
            TogglePause();
            if (isPaused)
            {
                ShowPauseMenu(); // Mostrar el menú de pausa
            }
            else
            {
                HidePauseMenu(); // Ocultar el menú de pausa
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void LoadGameFromMenu()
    {
        SceneManager.LoadScene("Juego");
    }

    public void LoadPlayerData()
    {
        SaveData data = SaveSystem.Instance.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("No save data found");
            return;
        }

        Debug.Log($"Loading save data - IsTransformed: {data.isTransformed}");
        
        // Intentar encontrar el CharacterTransitionManager
        CharacterTransitionManager transitionManager = FindObjectOfType<CharacterTransitionManager>();
        
        // Restaurar el estado del PlatformManager independientemente de si existe el TransitionManager
        PlatformManager platformManager = FindObjectOfType<PlatformManager>();
        if (platformManager != null)
        {
            platformManager.gameObject.SetActive(data.isPlatformPowerActive);
            Debug.Log($"PlatformManager state restored: {data.isPlatformPowerActive}");
        }

        // Si existe el TransitionManager, usarlo para cargar el estado del personaje
        if (transitionManager != null)
        {
            Debug.Log("Found CharacterTransitionManager, loading character state");
            
            // Usar el método LoadCharacterState del manager directamente
            transitionManager.LoadCharacterState(data);
        }
        else
        {
            Debug.Log("CharacterTransitionManager not found - looking for initial character");
            
            // Buscar solo el personaje inicial (sin transformación) si no hay TransitionManager
            GameObject initialCharacter = GameObject.FindGameObjectWithTag("Player");
            if (initialCharacter != null)
            {
                Debug.Log($"Found initial character: {initialCharacter.name}");
                
                // Cargar posición del personaje
                initialCharacter.transform.position = data.unpoweredPosition.ToVector3();
                Debug.Log($"Updated initial character position to: {initialCharacter.transform.position}");
                
                // Cargar stats del personaje inicial si existen
                PlayerStats stats = initialCharacter.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.maxHealthSlots = data.unpoweredMaxHealth;
                    stats.currentHealthSlots = data.unpoweredCurrentHealth;
                    stats.maxManaSlots = data.unpoweredMaxMana;
                    stats.currentManaSlots = data.unpoweredCurrentMana;
                    stats.UpdateUI();
                    Debug.Log($"Updated initial character stats - Health: {stats.currentHealthSlots}/{stats.maxHealthSlots}, Mana: {stats.currentManaSlots}/{stats.maxManaSlots}");
                }
            }
            else
            {
                Debug.LogWarning("No initial character found with Player tag");
            }
        }

        // Configure camera to follow active character si existe el TransitionManager
        if (transitionManager != null)
        {
            CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();

            // Find and configure the saved camera
            CinemachineVirtualCamera savedCamera = null;
            foreach (var camera in cameras)
            {
                if (camera.gameObject.name == data.activeCameraName)
                {
                    savedCamera = camera;
                    break;
                }
            }

            if (savedCamera != null)
            {
                // Obtener el personaje activo del TransitionManager
                GameObject activePlayer = data.isTransformed ? 
                    transitionManager.GetPoweredCharacter() : 
                    transitionManager.GetUnpoweredCharacter();
                
                if (activePlayer != null)
                {
                    // Configure the saved camera
                    savedCamera.Follow = activePlayer.transform;
                    savedCamera.LookAt = activePlayer.transform;
                    Debug.Log($"Configured saved camera {savedCamera.gameObject.name} to follow {activePlayer.name}");
                }
            }
        }
        else
        {
            // Si no hay TransitionManager, configurar la cámara para seguir al personaje inicial
            GameObject initialCharacter = GameObject.FindGameObjectWithTag("Player");
            if (initialCharacter != null)
            {
                CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
                foreach (var camera in cameras)
                {
                    camera.Follow = initialCharacter.transform;
                    camera.LookAt = initialCharacter.transform;
                    Debug.Log($"Configured camera {camera.gameObject.name} to follow initial character");
                }
            }
        }

        // Finalizar la carga de datos
        Debug.Log("Player data loaded successfully");
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true); // Activar el menú de pausa
            Debug.Log("Pausa: Menú de pausa mostrado");
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // Desactivar el menú de pausa
            Debug.Log("Pausa: Menú de pausa ocultado");
        }
    }
}
