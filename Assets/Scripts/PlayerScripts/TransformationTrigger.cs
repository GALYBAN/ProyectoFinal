using UnityEngine;

public class TransformationTrigger : MonoBehaviour
{
    private CharacterTransitionManager transitionManager;
    private bool hasTriggered = false;

    private void Start()
    {
        transitionManager = FindObjectOfType<CharacterTransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogError("CharacterTransitionManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            transitionManager.StartCharacterSwitch();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
} 