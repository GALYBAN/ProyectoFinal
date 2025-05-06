using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SOUNDManager : MonoBehaviour
{
    public static SOUNDManager Instance { get; private set; }

    [Header("Audio Mixers")]
    public AudioMixer mainMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    private MusicManager musicManager;

    private void Awake()
    {
        Debug.Log("SOUNDManager Awake called");
        
        if (Instance == null)
        {
            Debug.Log("Setting up SOUNDManager Instance");
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize MusicManager
            musicManager = GetComponent<MusicManager>();
            // NO añadir por código, solo obtenerlo
            // if (musicManager == null)
            // {
            //     musicManager = gameObject.AddComponent<MusicManager>();
            // }
            // Ensure all audio sources are properly set up
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
            }
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple SOUNDManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVoiceVolume(float volume)
    {
        mainMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
    }

    public void PlayVoiceClip(AudioClip clip)
    {
        if (voiceSource != null && clip != null)
        {
            Debug.Log($"Playing voice clip: {clip.name}");
            voiceSource.Stop(); // Detener cualquier audio actual
            voiceSource.clip = clip;
            voiceSource.Play();
        }
        else
        {
            if (voiceSource == null)
            {
                Debug.LogError("Voice source is not assigned in SOUNDManager!");
            }
            if (clip == null)
            {
                Debug.LogError("Audio clip is null!");
            }
        }
    }

    public void PlayMusic(string trackName)
    {
        Debug.Log("[SOUNDManager] PlayMusic llamado con: " + trackName);
        if (musicManager != null)
        {
            musicManager.PlayTrack(trackName);
        }
        else
        {
            Debug.LogError("[SOUNDManager] musicManager es null");
        }
    }

    public void StopMusic()
    {
        if (musicManager != null)
        {
            musicManager.StopMusic();
        }
    }

    public void PauseMusic()
    {
        if (musicManager != null)
        {
            musicManager.PauseMusic();
        }
    }

    public void ResumeMusic()
    {
        if (musicManager != null)
        {
            musicManager.ResumeMusic();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SOUNDManager Start called");
        if (voiceSource == null)
        {
            Debug.LogError("Voice source is not assigned in SOUNDManager!");
        }
        else
        {
            Debug.Log("Voice source is properly assigned");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Debug.LogWarning("SOUNDManager se está destruyendo", this);
    }
}
