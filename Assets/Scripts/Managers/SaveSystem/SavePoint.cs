using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;
    [SerializeField] private GameObject saveButtonUI;
    [SerializeField] private GameObject saveMenu;

    private PlayerInputs inputs;
    private bool playerInRange = false;

    private void Start()
    {
        inputs = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputs>();
        saveButtonUI.SetActive(false);
        saveMenu.SetActive(false);
    }

    private void Update()
    {
        if (inputs.InteractInput && playerInRange)
        {
            OpenSaveMenu();
            saveButtonUI.SetActive(false);
        }
        else if (inputs.PauseInput)
        {
            CloseSaveMenu();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            saveButtonUI.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inputs.AttackInput = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            saveButtonUI.SetActive(false);
            saveMenu.SetActive(false);
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
            if (player != null)
            {
                GameManager.Instance.SavePlayerData(checkpointName, player.transform.position);
            }
        }
        saveMenu.SetActive(false);
    }

    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
    }
}
