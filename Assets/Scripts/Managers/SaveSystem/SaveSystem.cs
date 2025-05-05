using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

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
        savePath = Application.persistentDataPath + "/save.dat";
        Debug.Log($"Save path: {savePath}");
    }

    public void SaveGame(PlayerStats stats, Vector3 position, string checkpointName, string playerName, bool isVisible, int cameraPriority, string cameraName, bool isTransformed)
    {
        try
        {
            SaveData data = new SaveData
            {
                playerName = playerName,
                checkpointName = checkpointName,
                playerPosition = new SerializableVector3(position),
                maxHealthSlots = stats.maxHealthSlots,
                currentHealthSlots = stats.currentHealthSlots,
                maxManaSlots = stats.maxManaSlots,
                currentManaSlots = stats.currentManaSlots,
                isPlayerVisible = isVisible,
                cameraPriority = cameraPriority,
                cameraName = cameraName,
                isTransformed = isTransformed
            };

            Debug.Log($"Saving game data: Player={data.playerName}, Checkpoint={data.checkpointName}, " +
                     $"Position={data.playerPosition.ToVector3()}, " +
                     $"Health={data.currentHealthSlots}/{data.maxHealthSlots}, " +
                     $"Mana={data.currentManaSlots}/{data.maxManaSlots}, " +
                     $"Visible={data.isPlayerVisible}, CameraPriority={data.cameraPriority}, " +
                     $"CameraName={data.cameraName}, IsTransformed={data.isTransformed}");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }

            Debug.Log($"Game saved successfully at {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public SaveData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                using (FileStream stream = new FileStream(savePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    SaveData data = (SaveData)formatter.Deserialize(stream);
                    
                    Debug.Log($"Loaded game data: Player={data.playerName}, Checkpoint={data.checkpointName}, " +
                             $"Position={data.playerPosition.ToVector3()}, " +
                             $"Health={data.currentHealthSlots}/{data.maxHealthSlots}, " +
                             $"Mana={data.currentManaSlots}/{data.maxManaSlots}, " +
                             $"Visible={data.isPlayerVisible}, CameraPriority={data.cameraPriority}, " +
                             $"CameraName={data.cameraName}, IsTransformed={data.isTransformed}");
                    
                    return data;
                }
            }
            else
            {
                Debug.LogWarning("No save file found");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
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
                Debug.Log("Save file deleted successfully");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting save file: {e.Message}");
        }
    }
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

[Serializable]
public class SaveData
{
    public string playerName;
    public string checkpointName;
    public SerializableVector3 playerPosition;
    public int maxHealthSlots;
    public int currentHealthSlots;
    public int maxManaSlots;
    public int currentManaSlots;
    public bool isPlayerVisible;
    public int cameraPriority;
    public string cameraName;
    public bool isTransformed;
}
