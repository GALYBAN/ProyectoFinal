using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class IntroCinematic : MonoBehaviour
{
    [SerializeField] private VideoClip introVideo;
    [SerializeField] private string nextSceneName = "Cargando";
    
    private VideoPlayer videoPlayer;
    private RawImage videoDisplay;
    private Canvas canvas;
    private RenderTexture renderTexture;
    private bool isPlaying = false;

    private void Awake()
    {
        // Crear el Canvas
        GameObject canvasObj = new GameObject("CinematicCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.SetActive(false);

        // Crear el RawImage
        GameObject rawImageObj = new GameObject("VideoDisplay");
        rawImageObj.transform.SetParent(canvas.transform, false);
        videoDisplay = rawImageObj.AddComponent<RawImage>();
        videoDisplay.rectTransform.anchorMin = Vector2.zero;
        videoDisplay.rectTransform.anchorMax = Vector2.one;
        videoDisplay.rectTransform.sizeDelta = Vector2.zero;

        // Crear el RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        videoDisplay.texture = renderTexture;

        // Configurar el VideoPlayer
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.clip = introVideo;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, gameObject.AddComponent<AudioSource>());
        videoPlayer.loopPointReached += OnVideoEnd;
        
        // Desactivar el VideoPlayer inicialmente
        videoPlayer.enabled = false;
    }

    public void PlayCinematic()
    {
        if (isPlaying) return;
        
        isPlaying = true;
        canvas.gameObject.SetActive(true);
        videoPlayer.enabled = true;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        isPlaying = false;
        videoPlayer.enabled = false;
        canvas.gameObject.SetActive(false);
        ScenesManager.Instance.LoadSceneWithLoadingScreen(nextSceneName);
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
} 