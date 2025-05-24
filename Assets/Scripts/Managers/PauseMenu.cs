using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("UI References")]
    /*public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;*/
    public Button resumeButton;
    public Button optionsButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Awake()
    {
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

    private void Start()
    {
        //InitializeVolumeSliders();
        InitializeButtons();
    }



    /*private void InitializeVolumeSliders()
    {
        // Set initial values and add listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);
    }*/

    private void InitializeButtons()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }


    public void SetMasterVolume(float volume)
    {
        SOUNDManager.Instance.SetMasterVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        SOUNDManager.Instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SOUNDManager.Instance.SetSFXVolume(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        SOUNDManager.Instance.SetVoiceVolume(volume);
    }

    private void OnResumeButtonClicked()
    {
        // Llama a TogglePause y oculta el menú de pausa
        GameManager.Instance.TogglePause();
        GameManager.Instance.HidePauseMenu();
    }

    private void OnOptionsButtonClicked()
    {
        // Aquí puedes activar el GameObject del menú de opciones
        // Por ejemplo, si tienes un GameObject llamado optionsMenu
        GameObject optionsMenu = GameObject.Find("OptionsMenu");
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(true);
        }
    }

    private void OnMainMenuButtonClicked()
    {
        GameManager.Instance.TogglePause();
        ScenesManager.Instance.LoadSceneWithLoadingScreen("MenuPrincipal", false); // Asumiendo que el índice 0 es el menú principal
    }

    private void OnQuitButtonClicked()
    {
        // Llama a la función de ScenesManager para salir del juego
        GameManager.Instance.TogglePause();
        ScenesManager.Instance.QuitGame();
    }
} 