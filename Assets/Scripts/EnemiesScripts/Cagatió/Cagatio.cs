using System.Collections;
using UnityEngine;

public class Cagatió : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public GameObject manoPrefab;

    public Transform puntoInvocacion1;
    public Transform puntoInvocacion2;

    public float tiempoEntreAtaques = 2f;
    public int maxManos = 5;
    public float duracionCansancio = 5f;

    public GameObject jugador;

    private int enemigosVivos = 0;
    private int manosLanzadas = 0;

    private Animator animator;
    private EstadoJefe estadoActual = EstadoJefe.Inactivo;

    private enum EstadoJefe { Inactivo, Invocando, Atacando, Cansado }

    void Start()
    {
        animator = GetComponent<Animator>();
        AnimBridgeEvents.OnActivarJefe += Activar;
    }

    void OnDestroy()
    {
        AnimBridgeEvents.OnActivarJefe -= Activar;
    }

    void Activar()
    {
        enabled = true;
        animator.enabled = true;
        estadoActual = EstadoJefe.Invocando;
        InvocarEnemigosIniciales();
    }

    void Desactivar()
    {
      enabled = false;
    }

    void InvocarEnemigosIniciales()
    {
        InvocarEnemigo(puntoInvocacion1);
        InvocarEnemigo(puntoInvocacion2);
    }

    void InvocarEnemigo(Transform punto)
    {
        GameObject enemigo = Instantiate(enemigoPrefab, punto.position, punto.rotation);
        enemigo.transform.LookAt(jugador.transform);

        enemigosVivos++;

        if (enemigo.TryGetComponent<EnemyStats>(out var enemigoScript))
        {
            enemigoScript.jefe = this;
        }
    }

    public void NotificarMuerte()
    {
        enemigosVivos--;

        if (estadoActual == EstadoJefe.Invocando && enemigosVivos <= 0)
        {
            estadoActual = EstadoJefe.Atacando;
            manosLanzadas = 0;
            InvokeRepeating(nameof(AtacarConMano), 1f, tiempoEntreAtaques);
        }
    }

    void AtacarConMano()
    {
        if (manosLanzadas >= maxManos)
        {
            CancelInvoke(nameof(AtacarConMano));
            estadoActual = EstadoJefe.Cansado;
            StartCoroutine(FaseCansancio());
            return;
        }

        Vector3 posicionMano = new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f));
        GameObject mano = Instantiate(manoPrefab, posicionMano, Quaternion.identity);
        mano.GetComponent<Mano>().jugador = jugador;
        mano.transform.LookAt(jugador.transform);
        mano.GetComponent<Mano>().Activar();

        manosLanzadas++;
    }

    IEnumerator FaseCansancio()
    {
        yield return new WaitForSeconds(duracionCansancio);
        estadoActual = EstadoJefe.Invocando;
        InvocarEnemigosIniciales();
    }

    public bool PuedeRecibirDaño()
    {
        return estadoActual == EstadoJefe.Cansado;
    }
}