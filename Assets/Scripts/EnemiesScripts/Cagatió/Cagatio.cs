using System.Collections;
using UnityEngine;

public class Cagatió : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public GameObject manoPrefab;

    public Transform puntoInvocacion1;
    public Transform puntoInvocacion2;
    public Transform[] puntosAparicionManos;

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

    void Update()
    {
        Debug.Log($"Estado jefe: {estadoActual} | Enemigos vivos: {enemigosVivos} | Manos lanzadas: {manosLanzadas}");
    }

    void OnDestroy()
    {
        AnimBridgeEvents.OnActivarJefe -= Activar;
    }

    void Activar()
    {
        Debug.Log("Jefe Activado");
        enabled = true;
        animator.enabled = true;
        estadoActual = EstadoJefe.Invocando;
        InvocarEnemigosIniciales();
    }

    void InvocarEnemigosIniciales()
    {
        Debug.Log("Invocando enemigos iniciales...");
        enemigosVivos = 0;
        InvocarEnemigo(puntoInvocacion1);
        InvocarEnemigo(puntoInvocacion2);
        estadoActual = EstadoJefe.Invocando;
    }

    void InvocarEnemigo(Transform punto)
    {
        if (punto == null || enemigoPrefab == null)
        {
            Debug.LogError("Punto de invocación o prefab no asignado");
            return;
        }

        GameObject enemigo = Instantiate(enemigoPrefab, punto.position, punto.rotation);
        enemigo.transform.LookAt(jugador.transform);

        EnemyStats stats = enemigo.GetComponentInChildren<EnemyStats>();
        if (stats != null)
        {
            stats.jefe = this;
            Debug.Log($"Referencia asignada a enemigo en {punto.name}");
        }
        else
        {
            Debug.LogError("No se encontró EnemyStats en el enemigo instanciado");
        }

        enemigosVivos++;
        Debug.Log($"Enemigo invocado. Total: {enemigosVivos}");
    }

    public void NotificarMuerte()
    {
        enemigosVivos--;
        Debug.Log($"¡Muerte notificada! Enemigos restantes: {enemigosVivos}");

        if (enemigosVivos <= 0)
        {
            Debug.Log("¡Activando fase de ataque!");
            estadoActual = EstadoJefe.Atacando;
            manosLanzadas = 0;
            CancelInvoke(nameof(AtacarConMano));
            InvokeRepeating(nameof(AtacarConMano), 1f, tiempoEntreAtaques);
        }
    }

    void AtacarConMano()
    {
        try
        {
            if (manosLanzadas >= maxManos)
            {
                CambiarACansado();
                return;
            }

            if (!VerificarPuntosAparicion()) return;

            Transform puntoElegido = SeleccionarPuntoAleatorio();
            GameObject mano = CrearManoEnPunto(puntoElegido);

            if (mano != null)
            {
                ConfigurarMano(mano, puntoElegido);
                manosLanzadas++;
                Debug.Log($"Manos lanzadas: {manosLanzadas}/{maxManos}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error en AtacarConMano: {e.Message}");
        }
    }

    bool VerificarPuntosAparicion()
    {
        if (puntosAparicionManos == null || puntosAparicionManos.Length == 0)
        {
            Debug.LogError("No hay puntos de aparición asignados");
            return false;
        }
        return true;
    }

    Transform SeleccionarPuntoAleatorio()
    {
        return puntosAparicionManos[Random.Range(0, puntosAparicionManos.Length)];
    }

    GameObject CrearManoEnPunto(Transform punto)
    {
        if (manoPrefab == null || punto == null)
        {
            Debug.LogError("Prefab de mano o punto de spawn no asignado");
            return null;
        }

        GameObject mano = Instantiate(manoPrefab, punto.position, punto.rotation);
        Mano manoScript = mano.GetComponent<Mano>();
        if (manoScript != null)
        {
            manoScript.player = jugador;
        }
        else
        {
            Debug.LogError("Componente Mano no encontrado en el prefab");
        }

        return mano;
    }

    void ConfigurarMano(GameObject mano, Transform puntoOrigen)
    {
        Mano manoScript = mano.GetComponent<Mano>();
        if (manoScript == null)
        {
            Debug.LogError("Componente Mano no encontrado");
            return;
        }

        manoScript.Activate();
    }

    void CambiarACansado()
    {
        Debug.Log("Cambiando a estado Cansado");
        CancelInvoke(nameof(AtacarConMano));
        estadoActual = EstadoJefe.Cansado;
        StartCoroutine(FaseCansancio());
    }

    IEnumerator FaseCansancio()
    {
        Debug.Log("Entrando en fase de cansancio...");
        estadoActual = EstadoJefe.Cansado;
        yield return new WaitForSeconds(duracionCansancio);
        
        if (estadoActual == EstadoJefe.Cansado)
        {
            estadoActual = EstadoJefe.Invocando;
            Debug.Log("Volviendo a estado Invocando.");
            InvocarEnemigosIniciales();
        }
    }

    public bool PuedeRecibirDaño()
    {
        return estadoActual == EstadoJefe.Cansado;
    }
}