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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        currentManaSlots = maxManaSlots;
        currentHealthSlots = maxHealthSlots;
        UpdateManaUI();
        UpdateHealthUI();
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
}
