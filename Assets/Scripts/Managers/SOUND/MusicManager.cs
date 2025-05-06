using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicManager : MonoBehaviour
{

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

    [Header("Audio Source")]
    public AudioSource musicSource;

    private Coroutine fadeCoroutine;
    private MusicTrack currentTrack;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (musicSource == null)
        {
            Debug.LogError("[MusicManager] No se ha asignado un AudioSource para la música. Asígnalo en el Inspector.");
            return;
        }

        musicSource.outputAudioMixerGroup = musicMixerGroup;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    public void PlayTrack(string trackName)
    {
        Debug.Log("[MusicManager] PlayTrack llamado con: " + trackName);
        if (musicTracks == null || musicTracks.Length == 0)
        {
            Debug.LogError("[MusicManager] musicTracks está vacío o null");
            return;
        }

        foreach (var t in musicTracks)
            Debug.Log("[MusicManager] Track en array: " + t.name + " (clip: " + (t.clip != null ? t.clip.name : "null") + ")");

        MusicTrack track = System.Array.Find(musicTracks, t => t.name == trackName);
        if (track != null)
        {
            Debug.Log("[MusicManager] Track encontrado: " + track.name);
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
            Debug.LogWarning("[MusicManager] Track '" + trackName + "' no encontrado en MusicManager!");
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
        Debug.Log("[MusicManager] Iniciando FadeToNewTrack para: " + track.name);
        if (track.clip == null)
        {
            Debug.LogError("[MusicManager] El AudioClip de este track es null");
            yield break;
        }
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