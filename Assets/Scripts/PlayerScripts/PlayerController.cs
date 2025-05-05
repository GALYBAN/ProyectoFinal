using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"PlayerController Start - Tag: {gameObject.tag}");
        Debug.Log($"PlayerController Start - Position: {transform.position}");
        
        // Verificar que el jugador tiene un collider
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogError("Player is missing a Collider component!");
        }
        else
        {
            Debug.Log($"Player has collider: {playerCollider.GetType().Name}");
        }

        // Verificar que el jugador tiene un Rigidbody
        Rigidbody playerRb = GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogError("Player is missing a Rigidbody component!");
        }
        else
        {
            Debug.Log($"Player has Rigidbody - IsKinematic: {playerRb.isKinematic}, UseGravity: {playerRb.useGravity}");
        }
    }
} 