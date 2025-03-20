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
            }
        }
        saveMenu.SetActive(false);
    }

    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
    }
}
