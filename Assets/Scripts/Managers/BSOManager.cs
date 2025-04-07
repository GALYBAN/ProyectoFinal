using UnityEngine;
using System.Collections;

public class BSOManager : MonoBehaviour
{
    public static BSOManager Instance { get; private set; }

    [Header("Configuración de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Pistas Musicales")]
    public AudioClip introMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;

    void Awake()
    {
        // Implementación del patrón Singleton
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

    // Método para reproducir cualquier pista
    public void PlayTrack(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (musicSource == null || clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        musicSource.Play();
    }

    // Métodos específicos para cada pista
    public void PlayIntroMusic() => PlayTrack(introMusic);
    public void PlayBossMusic() => PlayTrack(bossMusic, true, 0.8f);
    public void PlayVictoryMusic() => PlayTrack(victoryMusic, false);

    // Método para efectos de sonido
    public void PlaySFX(AudioClip sfx, float volume = 1f)
    {
        if (sfxSource == null || sfx == null) return;

        sfxSource.PlayOneShot(sfx, volume);
    }
}