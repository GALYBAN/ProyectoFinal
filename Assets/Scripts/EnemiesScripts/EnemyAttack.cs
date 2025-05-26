using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Saltador saltador;
    private Horse horse;

    void Awake()
    {
        // Buscar el script Saltador en los padres
        saltador = GetComponentInParent<Saltador>();
        horse = GetComponentInParent<Horse>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && saltador != null && saltador.CanDealDamage())
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage();
                saltador.DisableDamage();
                Debug.Log("Player damaged by Espantaocells attack!");
            }
        }

        if (other.CompareTag("Player") && horse != null && horse.CanDealDamage())
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage();
                horse.DisableDamage();
                Debug.Log("Player damaged by Cavall attack!");
            }
        }
    }
}
