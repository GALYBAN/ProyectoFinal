using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    public int maxManaSlots = 2;
    private int currentManaSlots;
    public Image[] manaSlotsUI;

    void Start()
    {
        currentManaSlots = maxManaSlots;
        UpdateManaUI();
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
}