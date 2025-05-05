using UnityEngine;

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
        Debug.Log($"DialogueTrigger initialized with {dialogueZones.Length} zones");
        
        // Verificar que todas las zonas tienen los componentes necesarios
        foreach (var zone in dialogueZones)
        {
            if (zone.triggerZone == null)
            {
                Debug.LogError($"Dialogue zone is missing its trigger zone transform!");
                continue;
            }
            
            if (zone.dialogueData == null)
            {
                Debug.LogError($"Dialogue zone is missing its DialogueData!");
                continue;
            }

            // Verificar que la zona tiene un collider
            Collider zoneCollider = zone.triggerZone.GetComponent<Collider>();
            if (zoneCollider == null)
            {
                Debug.LogError($"Dialogue zone {zone.triggerZone.name} is missing a Collider component!");
            }
            else
            {
                zoneCollider.isTrigger = true;
                Debug.Log($"Zone {zone.triggerZone.name} has collider: {zoneCollider.GetType().Name}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");
        Debug.Log($"Trigger position: {other.transform.position}");
        
        // Verificar si el objeto o alguno de sus padres tiene el tag "Player"
        Transform current = other.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                Debug.Log($"Player entered dialogue trigger: {other.gameObject.name}");
                if (!isDialogueActive)
                {
                    CheckDialogueZones(other.transform);
                }
                return;
            }
            current = current.parent;
        }

        Debug.Log($"Object {other.gameObject.name} is not tagged as Player. Full path: {GetFullPath(other.transform)}");
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
        Debug.Log($"Checking dialogue zones for player at position: {playerTransform.position}");
        foreach (DialogueZone zone in dialogueZones)
        {
            if (zone.triggerZone == null || zone.dialogueData == null)
            {
                Debug.LogWarning($"Skipping zone - triggerZone or dialogueData is null");
                continue;
            }

            float distance = Vector3.Distance(playerTransform.position, zone.triggerZone.position);
            Debug.Log($"Distance to zone {zone.triggerZone.name}: {distance} (radius: {zone.triggerRadius})");

            // Modificamos la condición para que funcione correctamente con canBeTriggeredMultipleTimes
            if (distance <= zone.triggerRadius && (!isDialogueActive || zone.canBeTriggeredMultipleTimes))
            {
                Debug.Log($"Starting dialogue from zone: {zone.triggerZone.name}");
                if (DialogueSystem.Instance != null)
                {
                    Debug.Log("DialogueSystem.Instance found, starting dialogue");
                    Debug.Log($"DialogueData has {zone.dialogueData.dialogueLines.Length} lines");
                    isDialogueActive = true;
                    DialogueSystem.Instance.StartDialogue(zone.dialogueData.dialogueLines);
                    // Solo marcamos como triggered si no permite múltiples activaciones
                    if (!zone.canBeTriggeredMultipleTimes)
                    {
                        zone.hasBeenTriggered = true;
                    }
                }
                else
                {
                    Debug.LogError("DialogueSystem.Instance is null! Make sure the DialogueSystem is in the scene.");
                }
                break;
            }
            else
            {
                Debug.Log($"Zone {zone.triggerZone.name} not triggered - Distance: {distance}, Triggered: {zone.hasBeenTriggered}, CanTriggerMultiple: {zone.canBeTriggeredMultipleTimes}");
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