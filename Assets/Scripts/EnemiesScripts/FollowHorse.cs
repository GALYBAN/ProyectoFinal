using UnityEngine;

public class FollowHorse : MonoBehaviour
{
    [SerializeField] private Transform horse; // Asigna el caballo en el Inspector

    void Update()
    {
        if (horse != null)
        {
            transform.position = horse.position + Vector3.up * 1.1f; // Ajusta la altura si es necesario
        }
    }
}