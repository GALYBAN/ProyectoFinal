using UnityEngine;

public class SaveSystem : MonoBehaviour
{


    
    public static void SaveGame(PlayerStats playerStats, Vector3 playerPosition, string checkpointName)
    {
        SaveManager.SaveGame(playerPosition, playerStats.currentHealthSlots, playerStats.currentManaSlots, checkpointName);
        Debug.Log("Juego guardado en: " + checkpointName);
    }

    public static void LoadGame(PlayerStats playerStats, GameObject player)
    {
        SaveData data = SaveManager.LoadGame();
        if (data != null)
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
            playerStats.currentHealthSlots = data.playerHealth;
            playerStats.currentManaSlots = data.playerMana;
            Debug.Log("Jugador cargado en: " + data.lastCheckpoint);
        }
    }
}
