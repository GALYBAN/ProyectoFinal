using System.IO;
using UnityEditor.SearchService;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string savePath => Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(Vector3 playerPosition, int playerHealth, int playerMana, string checkpoint)
    {
        SaveData data = new SaveData(playerPosition, playerHealth, playerMana, checkpoint);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Juego guardado en: " + savePath);
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Juego cargado con éxito.");
            return data;
        }
        Debug.LogWarning("No se encontró archivo de guardado.");
        return null;
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Archivo de guardado eliminado.");
        }
    }
}
