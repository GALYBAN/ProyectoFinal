using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
            
            // Add video event handlers
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.started += (source) => Debug.Log("Video started playing");
            
            // Set video to play once and not loop
            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
        }

        // Add prepared event handler
        videoPlayer.prepareCompleted += (source) => {
            Debug.Log("Video prepared successfully");
        };
        
        // Add error event handler
        videoPlayer.errorReceived += (source, message) => {
            Debug.LogError($"Video player error: {message}");
            LoadNextScene(); // Fallback to loading next scene if video fails
        };

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

    private bool isVideoNearEnd()
    {
        if (videoPlayer == null || !videoPlayer.isPrepared) return false;
        
        // Check if we're at or very near the end of the video
        long currentFrame = videoPlayer.frame;
        long totalFrames = (long)videoPlayer.frameCount;
        
        Debug.Log($"Video progress - Current frame: {currentFrame}, Total frames: {totalFrames}");
        
        // Consider the video complete if we're within the last few frames
        return currentFrame > 0 && totalFrames > 0 && currentFrame >= totalFrames - 2;
    }

    public void PlayCinematic(string sceneToLoad)
    {
        if (hasPlayed) return;

        Debug.Log($"Starting cinematic with next scene: {sceneToLoad}");
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
            try
            {
                videoPlayer.Play();
                Debug.Log("Video playback started");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error playing video: {e.Message}");
                LoadNextScene(); // Fallback to loading next scene if video fails
            }
        }
        else
        {
            Debug.LogWarning("No video player found, loading next scene directly");
            LoadNextScene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Prevent multiple calls
        if (hasPlayed)
        {
            Debug.Log("Video finished playing, preparing to load next scene");
            
            // Ensure we stop the video player
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            // Deactivate the canvas and video display
            if (cinematicCanvas != null)
            {
                cinematicCanvas.SetActive(false);
            }
            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(false);
            }

            hasPlayed = false;  // Reset the flag

            // Use invoke to ensure we're on the main thread and give a small delay
            Invoke("LoadNextScene", 0.1f);
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next scene name is null or empty!");
            return;
        }

        Debug.Log($"Loading next scene: {nextSceneName}");
        PlayerPrefs.SetString("NextScene", nextSceneName);
        SceneManager.LoadScene("Cargando");
    }

    public void SkipCinematic()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            Debug.Log("Skipping cinematic");
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
            hasPlayed = false;  // Reset the flag
            LoadNextScene();
        }
    }

    private void Update()
    {
        // Check for skip input (Escape key or Space bar) when video is playing
        if (videoPlayer != null && videoPlayer.isPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            SkipCinematic();
        }

        // Check if video has finished playing
        if (videoPlayer != null && hasPlayed && !videoPlayer.isPlaying && isVideoNearEnd())
        {
            Debug.Log("Video completion detected in Update");
            OnVideoFinished(videoPlayer);
        }
    }
} 