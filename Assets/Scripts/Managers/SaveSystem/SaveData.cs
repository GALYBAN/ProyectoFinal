using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float playerX, playerY, playerZ; // Posición del jugador
    public int playerHealth; // Vida del jugador
    public int playerMana;
    public string lastCheckpoint; // Último punto de guardado

    public SaveData
        (
        Vector3 position,
        int health, 
        int mana, 
        string checkpoint
        )
            {
                playerX = position.x;
                playerY = position.y;
                playerZ = position.z;
                playerHealth = health;
                lastCheckpoint = checkpoint;
                playerMana = mana;
            }
}
