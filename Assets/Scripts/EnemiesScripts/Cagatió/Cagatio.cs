using System.Collections;
using UnityEngine;

public class Cagatió : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public GameObject manoPrefab;

    public Transform puntoInvocacion1;
    public Transform puntoInvocacion2;
    public Transform[] puntosAparicionManos;

    [Header("Puntos de Patrulla para Caballos")]
    public Transform[] puntosPatrullaCaballo1; // Puntos para el primer caballo
    public Transform[] puntosPatrullaCaballo2; // Puntos para el segundo caballo

    public float tiempoEntreAtaques = 4f;
    public int maxManos = 1;
    public float duracionCansancio = 8f;

    public GameObject jugador;

    private int enemigosVivos = 0;
    private int manosLanzadas = 0;

    private Animator animator;
    private EstadoJefe estadoActual = EstadoJefe.Inactivo;

    private GameObject manoActual; // Referencia a la mano actual

    private enum EstadoJefe { Inactivo, Invocando, Atacando, Cansado }

    void Start()
    {
        animator = GetComponent<Animator>();
        AnimBridgeEvents.OnActivarJefe += Activar;

        // Verificar que tenemos los puntos de patrulla necesarios
        if (puntosPatrullaCaballo1 == null || puntosPatrullaCaballo1.Length < 2)
        {
            Debug.LogError("No hay suficientes puntos de patrulla asignados para el caballo 1!");
        }
        if (puntosPatrullaCaballo2 == null || puntosPatrullaCaballo2.Length < 2)
        {
            Debug.LogError("No hay suficientes puntos de patrulla asignados para el caballo 2!");
        }
    }

    void Update()
    {
        //Debug.Log($"Estado jefe: {estadoActual} | Enemigos vivos: {enemigosVivos} | Manos lanzadas: {manosLanzadas}");
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
        manosLanzadas = 0; // Resetear el contador de manos
        
        // Asegurarse de que no quede ninguna mano
        if (manoActual != null)
        {
            Destroy(manoActual);
            manoActual = null;
        }
        
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

        // Asignar puntos de patrulla según el punto de invocación
        Horse horseScript = enemigo.GetComponentInChildren<Horse>();
        if (horseScript != null)
        {
            Transform[] puntosPatrulla = (punto == puntoInvocacion1) ? puntosPatrullaCaballo1 : puntosPatrullaCaballo2;
            if (puntosPatrulla != null && puntosPatrulla.Length >= 2)
            {
                Debug.Log($"Asignando puntos de patrulla al caballo en {punto.name}");
                horseScript.SetPatrolPoints(puntosPatrulla[0], puntosPatrulla[1]);
            }
            else
            {
                Debug.LogError($"No hay suficientes puntos de patrulla asignados para el caballo en {punto.name}");
            }
        }
        else
        {
            Debug.LogError("No se encontró el componente Horse en el enemigo instanciado ni en sus hijos");
        }

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
                manoActual = mano; // Guardar referencia a la mano actual
                ConfigurarMano(mano, puntoElegido);
                manosLanzadas++;
                Debug.Log($"Mano lanzada: {manosLanzadas}/{maxManos}");
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

        Vector3 spawnPosition = new Vector3(
            punto.position.x,
            punto.position.y + 15f,
            punto.position.z
        );
        
        GameObject mano = Instantiate(manoPrefab, spawnPosition, manoPrefab.transform.rotation);
        
        Mano manoScript = mano.GetComponent<Mano>();
        if (manoScript != null)
        {
            manoScript.player = jugador;
            manoScript.Activate();
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
    }

    void CambiarACansado()
    {
        Debug.Log("Cambiando a estado Cansado");
        CancelInvoke(nameof(AtacarConMano));
        estadoActual = EstadoJefe.Cansado;
        
        // Destruir la mano actual si existe
        if (manoActual != null)
        {
            Destroy(manoActual);
            manoActual = null;
        }
        
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