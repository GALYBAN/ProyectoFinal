using UnityEngine;
using Cinemachine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;
    [SerializeField] private GameObject saveButtonUI;
    [SerializeField] private GameObject saveMenu;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int newPriority = 15;
    private string cameraName;

    private PlayerInputs inputs;
    private bool playerInRange = false;
    private GameObject currentPlayer;

    private void Awake()
    {
        // Generar un nombre único si no tiene uno asignado
        if (string.IsNullOrEmpty(checkpointName))
        {
            checkpointName = gameObject.name + "_" + transform.position.x + "_" + transform.position.y;
        }

        // Guardar el nombre de la cámara si no está asignado
        if (virtualCamera != null && string.IsNullOrEmpty(cameraName))
        {
            cameraName = virtualCamera.gameObject.name;
        }
    }

    private void Start()
    {
        saveButtonUI.SetActive(false);
        saveMenu.SetActive(false);
    }

    private void Update()
    {
        if (currentPlayer != null)
        {
            inputs = GameObject.Find("PlayerReference").GetComponent<PlayerInputs>();

            if (inputs != null && inputs.InteractInput && playerInRange)
            {
                virtualCamera.Priority = newPriority;
                OpenSaveMenu();
            }
            else if (inputs != null && inputs.PauseInput && playerInRange)
            {
                CloseSaveMenu();
                virtualCamera.Priority = 5;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
            saveButtonUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayer = null;
            inputs = null;
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
        if (playerInRange && currentPlayer != null)
        {
            SaveSystem.Instance.SaveGame(checkpointName);
            Debug.Log($"Game saved at {checkpointName}");
        }
        saveMenu.SetActive(false);
    }

    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
    }
}