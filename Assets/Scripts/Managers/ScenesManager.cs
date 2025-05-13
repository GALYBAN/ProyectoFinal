using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;
    private Animator animator;
    private bool shouldLoadSaveData = false;

    public static ScenesManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ScenesManager");
                instance = obj.AddComponent<ScenesManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FindDeathCanvas();
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneWithLoadingScreen(string sceneName, bool playCinematic = false)
    {
        shouldLoadSaveData = !playCinematic && sceneName == "Juego";
        
        if (playCinematic)
        {
            CinematicManager.Instance.PlayCinematic(sceneName);
        }
        else
        {
            PlayerPrefs.SetString("NextScene", sceneName);
            SceneManager.LoadScene("Cargando");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DeathScene()
    {
        if (animator != null)
        {
            animator.SetBool("HasMort", true);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDeathCanvas();

        // If we're loading the game scene and should load save data
        if (scene.name == "Juego" && shouldLoadSaveData)
        {
            Debug.Log("Loading save data after scene load");
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.LoadPlayerData();
            }
            else
            {
                Debug.LogError("GameManager not found in scene!");
            }
            shouldLoadSaveData = false;
        }
    }

    private void FindDeathCanvas()
    {
        GameObject deathCanvas = GameObject.Find("DEATHCanvas");
        if (deathCanvas != null)
        {
            animator = deathCanvas.GetComponent<Animator>();
            animator.SetBool("HasMort", false);
        }
        else
        {
            animator = null;
            Debug.LogWarning("DEATHCanvas no encontrado en la escena actual.");
        }
    }
}
