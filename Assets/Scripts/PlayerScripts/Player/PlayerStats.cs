using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Mana Settings")]
    public int maxManaSlots = 2;
    public int currentManaSlots;
    public Image[] manaSlotsUI;

    [Header("Health Settings")]
    public int maxHealthSlots = 3;
    public int currentHealthSlots;
    public Image[] healthSlotsUI;

    private void Awake()
    {
        Debug.Log($"PlayerStats Awake - Initial values: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
        LoadGame();
    }

    private void Start()
    {
        Debug.Log($"PlayerStats Start - Current values: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
        if (currentManaSlots == 0)
        {
            currentManaSlots = maxManaSlots;
            Debug.Log($"Setting default mana: {currentManaSlots}/{maxManaSlots}");
        }
        if (currentHealthSlots == 0)
        {
            currentHealthSlots = maxHealthSlots;
            Debug.Log($"Setting default health: {currentHealthSlots}/{maxHealthSlots}");
        }
        UpdateUI();
    }

    private void LoadGame()
    {
        SaveData data = SaveSystem.Instance.LoadGame();
        if (data != null)
        {
            bool isPowered = gameObject.name.Contains("T"); // Check if this is the powered version
            
            // Get the correct position and stats based on character type
            if (isPowered && data.poweredCharacterName == gameObject.name)
            {
                Debug.Log($"Loading powered character data - Before load: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
                
                transform.position = data.poweredPosition.ToVector3();
                maxHealthSlots = data.poweredMaxHealth;
                currentHealthSlots = data.poweredCurrentHealth;
                maxManaSlots = data.poweredMaxMana;
                currentManaSlots = data.poweredCurrentMana;
                
                Debug.Log($"After load: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
                Debug.Log($"Game loaded for powered character at {data.checkpointName}");
            }
            else if (!isPowered && data.unpoweredCharacterName == gameObject.name)
            {
                Debug.Log($"Loading unpowered character data - Before load: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
                
                transform.position = data.unpoweredPosition.ToVector3();
                maxHealthSlots = data.unpoweredMaxHealth;
                currentHealthSlots = data.unpoweredCurrentHealth;
                maxManaSlots = data.unpoweredMaxMana;
                currentManaSlots = data.unpoweredCurrentMana;
                
                Debug.Log($"After load: Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
                Debug.Log($"Game loaded for unpowered character at {data.checkpointName}");
            }
            UpdateUI();
        }
        else
        {
            Debug.Log($"No saved data found for {gameObject.name}");
        }
    }

    public bool ConsumeManaSlot()
    {
        if (currentManaSlots > 0)
        {
            currentManaSlots--;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void RegenerateManaSlot()
    {
        if (currentManaSlots < maxManaSlots)
        {
            currentManaSlots++;
            UpdateUI();
        }
    }

    public void TakeDamage()
    {
        if (currentHealthSlots > 0)
        {
            currentHealthSlots--;
            UpdateUI();
        }

        if (currentHealthSlots <= 0)
        {
            ScenesManager.Instance.DeathScene();
        }
    }

    public void Heal()
    {
        currentHealthSlots = maxHealthSlots;
        UpdateUI();
        Debug.Log("Vida recuperada completamente.");
    }

    public void UpdateUI()
    {
        Debug.Log($"Updating UI - Health={currentHealthSlots}/{maxHealthSlots}, Mana={currentManaSlots}/{maxManaSlots}");
        for (int i = 0; i < manaSlotsUI.Length; i++)
        {
            manaSlotsUI[i].enabled = i < currentManaSlots;
        }

        for (int i = 0; i < healthSlotsUI.Length; i++)
        {
            healthSlotsUI[i].enabled = i < currentHealthSlots;
        }
    }
}
