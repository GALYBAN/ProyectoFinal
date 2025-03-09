using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;

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

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
