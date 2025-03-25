using UnityEngine;
using Cinemachine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;
    [SerializeField] private GameObject saveButtonUI;
    [SerializeField] private GameObject saveMenu;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int newPriority = 15;

    private PlayerInputs inputs;
    private bool playerInRange = false;

    private void Awake()
    {
        inputs = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputs>();

        // Generar un nombre único si no tiene uno asignado
        if (string.IsNullOrEmpty(checkpointName))
        {
            checkpointName = gameObject.name + "_" + transform.position.x + "_" + transform.position.y;
        }
    }

    private void Start()
    {
        saveButtonUI.SetActive(false);
        saveMenu.SetActive(false);
    }

    private void Update()
    {
        if (inputs.InteractInput && playerInRange)
        {
            virtualCamera.Priority = newPriority;
            OpenSaveMenu();
        }
        else if (inputs.PauseInput && playerInRange)
        {
            CloseSaveMenu();
            virtualCamera.Priority = 5;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            saveButtonUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            saveButtonUI.SetActive(false);
            saveMenu.SetActive(false);
            virtualCamera.Priority = 5;
        }
    }

    public void OpenSaveMenu()
    {
        if (playerInRange)
        {
            saveMenu.SetActive(true);
        }
    }

    public void SaveGame()
    {
        if (playerInRange)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerStats stats = player.GetComponent<PlayerStats>();

            if (player != null && stats != null)
            {
                SaveSystem.SaveGame(stats, player.transform.position, checkpointName);
                Debug.Log($"Partida guardada en: {checkpointName}");
            }
        }
        saveMenu.SetActive(false);
    }

    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
    }
}