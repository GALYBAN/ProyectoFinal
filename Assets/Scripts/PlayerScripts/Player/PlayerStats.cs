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
        SaveSystem.LoadGame(this, gameObject);
    }
    private void Start()
    {
        currentManaSlots = maxManaSlots;
        currentHealthSlots = maxHealthSlots;
        UpdateUI();
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
        if (currentHealthSlots < maxHealthSlots)
        {
            currentHealthSlots++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
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
