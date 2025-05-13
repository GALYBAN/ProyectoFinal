using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;
using Cinemachine;

[Serializable]
public class SaveData
{
    // General save data
    public string checkpointName;
    public bool isTransformed;  // Indicates if we're in powered form
    public string activeCameraName;

    // Unpowered character data
    public string unpoweredCharacterName;
    public SerializableVector3 unpoweredPosition;
    public bool isUnpoweredActive;
    public int unpoweredMaxHealth;
    public int unpoweredCurrentHealth;
    public int unpoweredMaxMana;
    public int unpoweredCurrentMana;

    // Powered character data
    public string poweredCharacterName;
    public SerializableVector3 poweredPosition;
    public bool isPoweredActive;
    public int poweredMaxHealth;
    public int poweredCurrentHealth;
    public int poweredMaxMana;
    public int poweredCurrentMana;
}

[Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    private string savePath;

    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SaveSystem");
                instance = obj.AddComponent<SaveSystem>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log($"Save path: {savePath}");
    }

    public void SaveGame(string checkpointName)
    {
        Debug.Log($"Starting save game process at checkpoint: {checkpointName}");

        // Find the CharacterTransitionManager
        CharacterTransitionManager transitionManager = GameObject.FindObjectOfType<CharacterTransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogError("CharacterTransitionManager not found in the scene");
            return;
        }

        // Get character references from the manager
        GameObject cleoUnpowered = transitionManager.GetUnpoweredCharacter();
        GameObject cleoPowered = transitionManager.GetPoweredCharacter();

        if (cleoUnpowered == null || cleoPowered == null)
        {
            Debug.LogError("One or both characters not assigned in CharacterTransitionManager");
            return;
        }

        SaveData saveData = new SaveData();
        saveData.checkpointName = checkpointName;
        saveData.isTransformed = transitionManager.IsTransformed();

        Debug.Log($"Found characters - Unpowered: {cleoUnpowered.name} (Active: {cleoUnpowered.activeSelf}), " +
                  $"Powered: {cleoPowered.name} (Active: {cleoPowered.activeSelf})");

        // Save unpowered character data
        saveData.unpoweredCharacterName = cleoUnpowered.name;
        saveData.unpoweredPosition = new SerializableVector3(cleoUnpowered.transform.position);
        saveData.isUnpoweredActive = cleoUnpowered.activeSelf;

        PlayerStats unpoweredStats = cleoUnpowered.GetComponent<PlayerStats>();
        if (unpoweredStats != null)
        {
            saveData.unpoweredMaxHealth = unpoweredStats.maxHealthSlots;
            saveData.unpoweredCurrentHealth = unpoweredStats.currentHealthSlots;
            saveData.unpoweredMaxMana = unpoweredStats.maxManaSlots;
            saveData.unpoweredCurrentMana = unpoweredStats.currentManaSlots;
            Debug.Log($"Saving unpowered character stats - Health: {unpoweredStats.currentHealthSlots}/{unpoweredStats.maxHealthSlots}, " +
                     $"Mana: {unpoweredStats.currentManaSlots}/{unpoweredStats.maxManaSlots}");
        }

        // Save powered character data
        saveData.poweredCharacterName = cleoPowered.name;
        saveData.poweredPosition = new SerializableVector3(cleoPowered.transform.position);
        saveData.isPoweredActive = cleoPowered.activeSelf;

        PlayerStats poweredStats = cleoPowered.GetComponent<PlayerStats>();
        if (poweredStats != null)
        {
            saveData.poweredMaxHealth = poweredStats.maxHealthSlots;
            saveData.poweredCurrentHealth = poweredStats.currentHealthSlots;
            saveData.poweredMaxMana = poweredStats.maxManaSlots;
            saveData.poweredCurrentMana = poweredStats.currentManaSlots;
            Debug.Log($"Saving powered character stats - Health: {poweredStats.currentHealthSlots}/{poweredStats.maxHealthSlots}, " +
                     $"Mana: {poweredStats.currentManaSlots}/{poweredStats.maxManaSlots}");
        }

        // Save camera data
        CinemachineVirtualCamera activeCamera = FindObjectsOfType<CinemachineVirtualCamera>()
            .OrderByDescending(c => c.Priority)
            .FirstOrDefault();

        if (activeCamera != null)
        {
            saveData.activeCameraName = activeCamera.gameObject.name;
            Debug.Log($"Saved active camera name: {saveData.activeCameraName}");
        }

        // Save the data to file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Game saved successfully at checkpoint: {checkpointName}\n" +
                 $"Transformation state: {(saveData.isTransformed ? "Powered" : "Unpowered")}\n" +
                 $"Unpowered character: {saveData.unpoweredCharacterName} (Active: {saveData.isUnpoweredActive})\n" +
                 $"Powered character: {saveData.poweredCharacterName} (Active: {saveData.isPoweredActive})");
    }

    public SaveData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                
                Debug.Log($"Loaded game data at checkpoint: {data.checkpointName}, IsTransformed: {data.isTransformed}");
                Debug.Log($"Unpowered Character: {data.unpoweredCharacterName}, Active: {data.isUnpoweredActive}, Position: {data.unpoweredPosition.ToVector3()}");
                Debug.Log($"Powered Character: {data.poweredCharacterName}, Active: {data.isPoweredActive}, Position: {data.poweredPosition.ToVector3()}");
                
                return data;
            }
            else
            {
                Debug.LogWarning($"No save file found at path: {savePath}");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading game from {savePath}: {e.Message}");
            return null;
        }
    }

    public bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log($"Save file deleted successfully from {savePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting save file from {savePath}: {e.Message}");
        }
    }
}
