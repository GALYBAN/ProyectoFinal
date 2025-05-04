using UnityEngine;

[System.Serializable]
public class DialogueZone
{
    public BoxCollider triggerZone;
    public DialogueData dialogueData;
    public bool hasBeenTriggered = false;
    public bool canBeTriggeredMultipleTimes = false;
}

public class DialogueTrigger : MonoBehaviour
{
    public DialogueZone[] dialogueZones;

    private void Start()
    {
        Debug.Log($"DialogueTrigger initialized with {dialogueZones.Length} zones");
        
        // Verificar que el objeto tiene un Collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("DialogueTrigger needs a Collider component!");
            gameObject.AddComponent<BoxCollider>();
        }
        
        // Asegurarse de que el Collider es un trigger
        GetComponent<Collider>().isTrigger = true;

        foreach (var zone in dialogueZones)
        {
            if (zone.triggerZone == null)
            {
                Debug.LogError("A trigger zone is missing its BoxCollider!");
            }
            else
            {
                Debug.Log($"Trigger zone found: {zone.triggerZone.gameObject.name}");
                // Asegurarse de que el trigger zone está configurado correctamente
                zone.triggerZone.isTrigger = true;
            }
            
            if (zone.dialogueData == null)
            {
                Debug.LogError("A trigger zone is missing its DialogueData!");
            }
            else
            {
                Debug.Log($"Dialogue data found with {zone.dialogueData.dialogueLines.Length} lines");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");
        Debug.Log($"Other collider is trigger: {other.isTrigger}");
        Debug.Log($"Other gameObject tag: {other.gameObject.tag}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            
            // Encontrar la zona que fue activada
            foreach (DialogueZone zone in dialogueZones)
            {
                if (zone.triggerZone != null)
                {
                    Debug.Log($"Checking zone: {zone.triggerZone.gameObject.name}");
                    
                    // Verificar si el collider que entró está dentro de la zona de trigger
                    if (zone.triggerZone.bounds.Contains(other.transform.position) && 
                        (!zone.hasBeenTriggered || zone.canBeTriggeredMultipleTimes))
                    {
                        Debug.Log($"Starting dialogue from zone with {zone.dialogueData.dialogueLines.Length} lines");
                        if (DialogueSystem.Instance != null)
                        {
                            DialogueSystem.Instance.StartDialogue(zone.dialogueData.dialogueLines);
                            zone.hasBeenTriggered = true;
                        }
                        else
                        {
                            Debug.LogError("DialogueSystem.Instance is null!");
                        }
                        break; // Solo activamos un diálogo a la vez
                    }
                }
            }
        }
    }

    public void ResetDialogueZones()
    {
        Debug.Log("Resetting dialogue zones");
        foreach (DialogueZone zone in dialogueZones)
        {
            zone.hasBeenTriggered = false;
        }
    }

    // Método para debug visual de las zonas
    private void OnDrawGizmos()
    {
        if (dialogueZones != null)
        {
            foreach (DialogueZone zone in dialogueZones)
            {
                if (zone.triggerZone != null)
                {
                    Gizmos.color = zone.hasBeenTriggered ? Color.red : Color.green;
                    Gizmos.matrix = zone.triggerZone.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(Vector3.zero, zone.triggerZone.size);
                }
            }
        }
    }
} 