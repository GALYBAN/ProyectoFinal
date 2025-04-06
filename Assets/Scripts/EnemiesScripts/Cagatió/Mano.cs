using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mano : MonoBehaviour
{
    // Velocidad a la que se mueve la mano
    public float velocidadMovimiento = 5f;

    // Velocidad a la que cae la mano
    public float velocidadCaída = 10f;

    // Distancia mínima para caer hacia el jugador
    public float distanciaCaida = 5f;

    // Referencia al jugador
    public GameObject jugador;

    // Estado de la mano
    bool isActivada = false;

    void Update()
    {
        if (isActivada)
        {
            // Mover la mano hacia el jugador desde el cielo
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z), velocidadMovimiento * Time.deltaTime);

            // Caer hacia el jugador cuando esté cerca
            if (Vector3.Distance(transform.position, jugador.transform.position) < distanciaCaida)
            {
                transform.position -= new Vector3(0f, velocidadCaída * Time.deltaTime, 0f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si la mano choca con el jugador, infligir daño
        if (collision.gameObject.CompareTag("Jugador"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage();
        }

        // Destruir la mano
        Destroy(gameObject);
    }

    public void Activar()
    {
        isActivada = true;
    }
}
