using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager instance;
    public static CinematicManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CinematicManager");
                instance = obj.AddComponent<CinematicManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject cinematicCanvas; // Reference to the canvas containing the video
    private bool hasPlayed = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SetupVideoPlayer();
    }

    private void SetupVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        if (renderTexture != null)
        {
            videoPlayer.targetTexture = renderTexture;
            if (videoDisplay != null)
            {
                videoDisplay.texture = renderTexture;
            }
        }

        // Ensure the canvas is initially disabled
        if (cinematicCanvas != null)
        {
            cinematicCanvas.SetActive(false);
        }
    }

    public void PlayCinematic(string sceneToLoad)
    {
        if (hasPlayed) return;

        nextSceneName = sceneToLoad;
        hasPlayed = true;
        
        if (videoPlayer != null)
        {
            // Activate the canvas and video display
            if (cinematicCanvas != null)
            {
                cinematicCanvas.SetActive(true);
            }
            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(true);
            }
            videoPlayer.Play();
        }
        else
        {
            LoadNextScene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Deactivate the canvas and video display
        if (cinematicCanvas != null)
        {
            cinematicCanvas.SetActive(false);
        }
        if (videoDisplay != null)
        {
            videoDisplay.gameObject.SetActive(false);
        }
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        PlayerPrefs.SetString("NextScene", nextSceneName);
        SceneManager.LoadScene("Cargando");
    }

    public void SkipCinematic()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            // Deactivate the canvas and video display
            if (cinematicCanvas != null)
            {
                cinematicCanvas.SetActive(false);
            }
            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(false);
            }
            LoadNextScene();
        }
    }
} 