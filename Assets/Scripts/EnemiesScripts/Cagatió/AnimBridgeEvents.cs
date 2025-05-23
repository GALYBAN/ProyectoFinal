using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBridgeEvents : MonoBehaviour
{
    public Animator introAnimator;

    public delegate void ActivarJefe();
    public static event ActivarJefe OnActivarJefe;
    [SerializeField] private IntroCagatió cagatio;
    
    void Start()
    {
        // Asegurarse de que tenemos la referencia al Cagatió
        if (cagatio == null)
        {
            cagatio = FindObjectOfType<IntroCagatió>();
        }
        
        if (introAnimator == null)
        {
            introAnimator = GetComponent<Animator>();
        }
    }

    public void CagatioLights()
    {
        if (cagatio != null)
        {
            cagatio.ActivateCagatioLights();
        }
        else
        {
            Debug.LogError("No se encontró la referencia a IntroCagatió");
        }
    }

    public void SceneLights()
    {
        if (cagatio != null)
        {
            cagatio.ActivateSceneLights();
        }
    }

    public void Cristals()
    {
        if (cagatio != null)
        {
            cagatio.ActivateCristals();
        }
    }
    
    // Este método será llamado por un Animation Event
    public void FinalizarIntro()
    {
        Debug.Log("Finalizando intro del jefe...");
        OnActivarJefe?.Invoke();
    }

    public void Intro(float waitTime)
    {
        Debug.Log("Iniciando intro del jefe...");
        StartCoroutine(StartBossMusicAfterDelay(waitTime)); // Start the coroutine
    }

    private IEnumerator StartBossMusicAfterDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Wait for the specified time

        // Cambiar a música de jefe al activarlo
        if (SOUNDManager.Instance != null)
        {
            SOUNDManager.Instance.PlayMusic("BossFight");
        }
        else
        {
            Debug.LogWarning("SOUNDManager no encontrado");
        }
    }
    
    public void ActivateBossHealthBar()
    {
        if (cagatio != null)
        {
            cagatio.ActivateBossHealthBar();
        }
    }
}
