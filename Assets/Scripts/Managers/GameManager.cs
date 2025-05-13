using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isPaused = false;

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
        
        // Find CharacterTransitionManager first
        CharacterTransitionManager transitionManager = FindObjectOfType<CharacterTransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogError("CharacterTransitionManager not found in the scene");
            return;
        }

        // Get character references from the manager
        GameObject cleoUnpowered = transitionManager.GetUnpoweredCharacter();
        GameObject cleoPowered = transitionManager.GetPoweredCharacter();

        if (cleoUnpowered == null || cleoPowered == null)
        {
            Debug.LogError("One or both characters not assigned in CharacterTransitionManager");
            return;
        }

        Debug.Log($"Found both characters - Unpowered: {cleoUnpowered.name}, Powered: {cleoPowered.name}");
        
        // Set active states based on transformation state
        cleoUnpowered.SetActive(!data.isTransformed);  // Active when NOT transformed
        cleoPowered.SetActive(data.isTransformed);     // Active when transformed
        
        Debug.Log($"Set character states - Unpowered active: {!data.isTransformed}, Powered active: {data.isTransformed}");

        // Always update positions for both characters
        cleoUnpowered.transform.position = data.unpoweredPosition.ToVector3();
        cleoPowered.transform.position = data.poweredPosition.ToVector3();
        
        Debug.Log($"Updated positions - Unpowered: {cleoUnpowered.transform.position}, Powered: {cleoPowered.transform.position}");

        // Update stats for both characters
        PlayerStats unpoweredStats = cleoUnpowered.GetComponent<PlayerStats>();
        if (unpoweredStats != null)
        {
            unpoweredStats.maxHealthSlots = data.unpoweredMaxHealth;
            unpoweredStats.currentHealthSlots = data.unpoweredCurrentHealth;
            unpoweredStats.maxManaSlots = data.unpoweredMaxMana;
            unpoweredStats.currentManaSlots = data.unpoweredCurrentMana;
            if (!data.isTransformed) unpoweredStats.UpdateUI();
            Debug.Log($"Updated unpowered character stats - Health: {unpoweredStats.currentHealthSlots}/{unpoweredStats.maxHealthSlots}, " +
                     $"Mana: {unpoweredStats.currentManaSlots}/{unpoweredStats.maxManaSlots}");
        }

        PlayerStats poweredStats = cleoPowered.GetComponent<PlayerStats>();
        if (poweredStats != null)
        {
            poweredStats.maxHealthSlots = data.poweredMaxHealth;
            poweredStats.currentHealthSlots = data.poweredCurrentHealth;
            poweredStats.maxManaSlots = data.poweredMaxMana;
            poweredStats.currentManaSlots = data.poweredCurrentMana;
            if (data.isTransformed) poweredStats.UpdateUI();
            Debug.Log($"Updated powered character stats - Health: {poweredStats.currentHealthSlots}/{poweredStats.maxHealthSlots}, " +
                     $"Mana: {poweredStats.currentManaSlots}/{poweredStats.maxManaSlots}");
        }

        // Configure camera to follow active character
        GameObject activePlayer = data.isTransformed ? cleoPowered : cleoUnpowered;
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
            // Configure the saved camera
            savedCamera.Follow = activePlayer.transform;
            savedCamera.LookAt = activePlayer.transform;
            Debug.Log($"Configured saved camera {savedCamera.gameObject.name} to follow {activePlayer.name}");
        }
        else
        {
            // If we can't find the saved camera, use the main game camera
            CinemachineVirtualCamera mainCamera = cameras.FirstOrDefault(c => c.gameObject.name == "CM vcam1");
            if (mainCamera != null)
            {
                mainCamera.Follow = activePlayer.transform;
                mainCamera.LookAt = activePlayer.transform;
                Debug.Log($"Configured main camera to follow {activePlayer.name} as fallback");
            }
            else
            {
                Debug.LogError("Could not find any suitable camera to follow the player!");
            }
        }

        // Update the CharacterTransitionManager with the loaded state
        transitionManager.SetTransformed(data.isTransformed, 
            data.isTransformed ? data.poweredPosition.ToVector3() : data.unpoweredPosition.ToVector3());
        Debug.Log($"Updated CharacterTransitionManager state to: {data.isTransformed}");
    }
}
