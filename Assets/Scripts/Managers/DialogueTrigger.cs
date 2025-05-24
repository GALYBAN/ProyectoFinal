using UnityEngine;
using System;

[System.Serializable]
public class DialogueZone
{
    public Transform triggerZone;  // Usamos Transform en lugar de BoxCollider para más flexibilidad
    public DialogueData dialogueData;
    public float triggerRadius = 2f;  // Radio de activación
    public bool hasBeenTriggered = false;
    public bool canBeTriggeredMultipleTimes = false;
}

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueZone[] dialogueZones;
    public bool showDebugGizmos = true;

    private bool isDialogueActive = false;

    private void Start()
    {
        // Verificar que todas las zonas tienen los componentes necesarios
        foreach (var zone in dialogueZones)
        {
            if (zone.triggerZone == null || zone.dialogueData == null) continue;

            // Verificar que la zona tiene un collider
            Collider zoneCollider = zone.triggerZone.GetComponent<Collider>();
            if (zoneCollider != null)
            {
                zoneCollider.isTrigger = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto o alguno de sus padres tiene el tag "Player"
        Transform current = other.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                if (!isDialogueActive)
                {
                    CheckDialogueZones(other.transform);
                }
                return;
            }
            current = current.parent;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Verificar si el objeto o alguno de sus padres tiene el tag "Player"
        Transform current = other.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                if (!isDialogueActive)
                {
                    CheckDialogueZones(other.transform);
                }
                return;
            }
            current = current.parent;
        }
    }

    private void CheckDialogueZones(Transform playerTransform)
    {
        foreach (DialogueZone zone in dialogueZones)
        {
            if (zone.triggerZone == null || zone.dialogueData == null) continue;

            float distance = Vector3.Distance(playerTransform.position, zone.triggerZone.position);

            if (distance <= zone.triggerRadius && (!isDialogueActive || zone.canBeTriggeredMultipleTimes))
            {
                if (DialogueSystem.Instance != null)
                {
                    isDialogueActive = true;
                    DialogueSystem.Instance.StartDialogue(zone.dialogueData.dialogueLines);
                    
                    // Suscribirse al evento de finalización del diálogo
                    DialogueSystem.Instance.OnDialogueEnd += () => {
                        isDialogueActive = false; // Restablecer el estado al finalizar el diálogo
                    };

                    if (!zone.canBeTriggeredMultipleTimes)
                    {
                        zone.hasBeenTriggered = true;
                    }
                }
                else
                {
                    Debug.LogError("DialogueSystem not found!");
                }
                break;
            }
        }
    }

    public void ResetDialogueZones()
    {
        Debug.Log("Resetting all dialogue zones");
        foreach (DialogueZone zone in dialogueZones)
        {
            zone.hasBeenTriggered = false;
        }
        isDialogueActive = false;
    }

    // Método para debug visual de las zonas
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        if (dialogueZones != null)
        {
            foreach (DialogueZone zone in dialogueZones)
            {
                if (zone.triggerZone != null)
                {
                    // Dibujar el radio de activación
                    Gizmos.color = zone.hasBeenTriggered ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(zone.triggerZone.position, zone.triggerRadius);

                    // Dibujar una línea al centro de la zona
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(zone.triggerZone.position, zone.triggerZone.position + Vector3.up * 0.5f);
                }
            }
        }
    }

    // Método auxiliar para obtener la ruta completa de un objeto
    private string GetFullPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
} 