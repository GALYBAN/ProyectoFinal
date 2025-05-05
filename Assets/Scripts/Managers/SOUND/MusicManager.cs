using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [System.Serializable]
    public class MusicTrack
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = true;
    }

    [Header("Music Settings")]
    [SerializeField] private MusicTrack[] musicTracks;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    private AudioSource musicSource;
    private Coroutine fadeCoroutine;
    private MusicTrack currentTrack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Initialize()
    {
        // Create and set up the music source if it doesn't exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicMixerGroup;
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        // Ensure the music source is properly configured
        if (musicSource != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }

    public void PlayTrack(string trackName)
    {
        MusicTrack track = System.Array.Find(musicTracks, t => t.name == trackName);
        if (track != null)
        {
            if (currentTrack != track)
            {
                currentTrack = track;
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeToNewTrack(track));
            }
        }
        else
        {
            Debug.LogWarning($"Track '{trackName}' not found in MusicManager!");
        }
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        StartCoroutine(FadeOut());
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    private IEnumerator FadeToNewTrack(MusicTrack track)
    {
        // Fade out current track if playing
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Set up and play new track
        musicSource.clip = track.clip;
        musicSource.volume = 0f;
        musicSource.loop = track.loop;
        musicSource.Play();

        // Fade in new track
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(0f, track.volume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = track.volume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = 0f;
    }

    public void SetVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public bool IsPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    public string GetCurrentTrackName()
    {
        return currentTrack != null ? currentTrack.name : "None";
    }
} 