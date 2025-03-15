using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager stats = other.GetComponent<GameManager>();
            GlobalPlayerController player = other.GetComponent<GlobalPlayerController>();
            if (player != null)
            {
                SaveManager.SaveGame(player.transform.position, stats.currentHealthSlots, stats.currentManaSlots, checkpointName);
                Debug.Log("Partida guardada en: " + checkpointName);
            }
        }
    }
}
