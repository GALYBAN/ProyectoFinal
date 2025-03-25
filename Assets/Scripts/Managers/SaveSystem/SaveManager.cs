using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string savePath => Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(Vector3 playerPosition, int playerHealth, int playerMana, string checkpoint)
    {
        // Siempre sobrescribimos el archivo de guardado con los datos más recientes
        SaveData data = new SaveData(playerPosition, playerHealth, playerMana, checkpoint);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Juego guardado en: {checkpoint}");
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Juego cargado desde: {data.lastCheckpoint}");
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
