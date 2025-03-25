using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void LoadGameFromMenu()
    {
        SceneManager.LoadScene("Nivel1"); // Asegúrate de usar el nombre correcto de la escena
    }

    public void LoadPlayerData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerStats stats = player.GetComponent<PlayerStats>();

        if (player != null && stats != null)
        {
            SaveSystem.LoadGame(stats, player);
        }
    }


}
