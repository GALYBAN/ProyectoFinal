using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    public Cagatió jefe;
    [SerializeField] private Slider slider;

    void Start()
    {
        slider.maxValue = maxHealth;
        currentHealth = maxHealth;
    }

    void Update()
    {
        slider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemigo recibió {damage} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemigo derrotado");
        Destroy(transform.parent.gameObject); // Elimina al enemigo
    }
    // Cuando el enemigo muere:
    public void Morir()
    {
        jefe?.NotificarMuerte(); // Notifica al jefe si está presente
        Destroy(gameObject);
    }
}
