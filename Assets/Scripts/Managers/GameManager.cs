using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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
        if (data != null)
        {
            // Buscar ambos personajes
            GameObject cleoNormal = GameObject.Find("CleoArmature"); // CleoArmature es la base
            GameObject cleoTransformed = GameObject.Find("CleoTArmature"); // CleoTArmature es la transformada

            if (cleoNormal != null && cleoTransformed != null)
            {
                // Activar/desactivar personajes según el estado guardado
                cleoNormal.SetActive(!data.isTransformed);
                cleoTransformed.SetActive(data.isTransformed);

                // Usar el personaje activo
                GameObject activePlayer = data.isTransformed ? cleoTransformed : cleoNormal;
                Debug.Log($"Loading data for player: {activePlayer.name}, IsTransformed={data.isTransformed}");

                // Aplicar posición y rotación
                activePlayer.transform.position = data.playerPosition.ToVector3();
                
                // Aplicar stats
                PlayerStats stats = activePlayer.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.maxHealthSlots = data.maxHealthSlots;
                    stats.currentHealthSlots = data.currentHealthSlots;
                    stats.maxManaSlots = data.maxManaSlots;
                    stats.currentManaSlots = data.currentManaSlots;
                    stats.UpdateUI();
                    Debug.Log($"Stats loaded: Health={stats.currentHealthSlots}/{stats.maxHealthSlots}, Mana={stats.currentManaSlots}/{stats.maxManaSlots}");
                }
                
                // Configurar la cámara
                CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
                foreach (var camera in cameras)
                {
                    if (camera.gameObject.name == data.cameraName)
                    {
                        camera.Follow = activePlayer.transform;
                        camera.LookAt = activePlayer.transform;
                        camera.Priority = data.cameraPriority;
                        Debug.Log($"Camera {camera.gameObject.name} configured to follow {activePlayer.name}");
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Could not find one or both character GameObjects in the scene");
            }
        }
        else
        {
            Debug.LogWarning("No save data found");
        }
    }
}
