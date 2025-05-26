using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    public Cagatió jefe;
    public Animator animator;
    [SerializeField] private Slider slider;

    void Awake()
    {
        animator = GameObject.Find("CagatióUnity").GetComponent<Animator>();
    }
    void Start()
    {
        slider.maxValue = maxHealth;
        currentHealth = maxHealth;
    }

    void Update()
    {
        slider.value = currentHealth;
    }

    // EnemyStats.cs
    private void Die()
    {
        // Debug importante para verificar
        Debug.Log($"Enemigo muriendo - Vida: {currentHealth} - Referencia jefe: {(jefe != null ? "EXISTE" : "NULL")}");

        // Notificar PRIMERO al jefe
        if (jefe != null)
        {
            jefe.NotificarMuerte();
        }
        else
        {
            Debug.LogError("¡No hay referencia al jefe!");
        }
        
        // Destruir el objeto
        Destroy(transform.parent.gameObject); // Cambiado a destruir solo el enemigo, no el padre
}

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (gameObject.name == "CagatióUnity")
        {
            animator.SetTrigger("Damage");
        }
        
        Debug.Log($"Enemigo recibió {damage} de daño. Vida: {currentHealth}");

        if (gameObject.name == "CagatióUnity" && currentHealth <= 0)
        {
            Debug.Log("Enemigo va a morir");
            animator.SetTrigger("End");
        }
        else if (gameObject.name != "CagatióUnity" && currentHealth <= 0)
        {
            Debug.Log("Enemigo va a morir");
            Die();
        }
    }

    public void BossDie()
    {
        Destroy(transform.parent.gameObject);
    }
}
