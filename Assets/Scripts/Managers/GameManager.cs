using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Mana Settings")]
    public int maxManaSlots = 2;
    public int currentManaSlots;
    public Image[] manaSlotsUI;

    [Header("Health Settings")]
    public int maxHealthSlots = 3;
    public int currentHealthSlots;
    public Image[] healthSlotsUI;

    private Vector3 lastSavedPosition;
    private string lastCheckpoint;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        currentManaSlots = maxManaSlots;
        currentHealthSlots = maxHealthSlots;

        UpdateManaUI();
        UpdateHealthUI();
    }

    void Start()
    {
        LoadPlayerData();
    }

    public bool ConsumeManaSlot()
    {
        if (currentManaSlots > 0)
        {
            currentManaSlots--;
            UpdateManaUI();
            return true;
        }
        return false;
    }

    public void RegenerateManaSlot()
    {
        if (currentManaSlots < maxManaSlots)
        {
            currentManaSlots++;
            UpdateManaUI();
        }
    }

    private void UpdateManaUI()
    {
        for (int i = 0; i < manaSlotsUI.Length; i++)
        {
            manaSlotsUI[i].enabled = i < currentManaSlots;
        }
    }

    public void TakeDamage()
    {
        if (currentHealthSlots > 0)
        {
            currentHealthSlots--;
            UpdateHealthUI();
        }

        if (currentHealthSlots <= 0)
        {
            ScenesManager.Instance.DeathScene();
        }
    }

    public void Heal()
    {
        if (currentHealthSlots < maxHealthSlots)
        {
            currentHealthSlots++;
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthSlotsUI.Length; i++)
        {
            healthSlotsUI[i].enabled = i < currentHealthSlots;
        }
    }

    public void SavePlayerData(string checkpointName, Vector3 playerPosition)
    {
        lastCheckpoint = checkpointName;
        lastSavedPosition = playerPosition;
        SaveManager.SaveGame(lastSavedPosition, currentHealthSlots, currentManaSlots, lastCheckpoint);
        Debug.Log("Partida guardada en: " + checkpointName);
    }

    public void LoadPlayerData()
    {
        SaveData data = SaveManager.LoadGame();
        if (data != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false; // Desactiva el CharacterController
                }

                player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ); // Mueve al jugador
                
                if (controller != null)
                {
                    controller.enabled = true; // Reactiva el CharacterController
                }
            }
            currentHealthSlots = data.playerHealth;
            currentManaSlots = data.playerMana;
            Debug.Log("Jugador cargado en: " + data.lastCheckpoint);
        }
    }

}
