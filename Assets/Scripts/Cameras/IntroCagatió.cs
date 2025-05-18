using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCagatió : MonoBehaviour
{
    [SerializeField] private GameObject[] lightsCagatio;
    [SerializeField] private GameObject[] lightsScene;
    [SerializeField] private GameObject cagatio;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer cristalMaterial;
    [SerializeField] private Color emissionColor;
    [SerializeField] private float emissionIntensity = 17.5f;
    [SerializeField] private GameObject bossHealthBarCanvas;
    [SerializeField] private BoxCollider triggerCollider;

    private MaterialPropertyBlock propBlock;
    private bool introTriggered = false;

    void Awake()
    {
        // Buscar el Cagatió si no está asignado
        if (cagatio == null)
        {
            cagatio = GameObject.Find("CagatióUnity");
            if (cagatio == null)
            {
                Debug.LogError("No se encontró el objeto CagatióUnity!");
                return;
            }
        }

        // Obtener el Animator si no está asignado
        if (animator == null)
        {
            animator = cagatio.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No se encontró el Animator en CagatióUnity!");
                return;
            }
        }

        // Configurar el material del cristal
        if (cristalMaterial != null)
        {
            propBlock = new MaterialPropertyBlock();
            cristalMaterial.GetPropertyBlock(propBlock);
            cristalMaterial.material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("No se asignó el material del cristal!");
        }

        // Desactivar la barra de vida del jefe
        if (bossHealthBarCanvas != null)
        {
            bossHealthBarCanvas.SetActive(false);
        }

        // Asegurarse de que el collider está configurado
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<BoxCollider>();
            if (triggerCollider == null)
            {
                Debug.LogWarning("No se encontró el BoxCollider para el trigger de la intro!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !introTriggered)
        {
            Debug.Log("Trigger de intro activado por el jugador");
            introTriggered = true;
            animator.SetTrigger("Intro");
        }
    }

    public void ActivateCagatioLights()
    {
        if (lightsCagatio != null && lightsCagatio.Length > 0)
        {
            foreach (GameObject light in lightsCagatio)
            {
                if (light != null)
                {
                    light.SetActive(true);
                }
            }
        }
    }

    public void ActivateSceneLights()
    {
        if (lightsScene != null && lightsScene.Length > 0)
        {
            foreach (GameObject light in lightsScene)
            {
                if (light != null)
                {
                    light.SetActive(true);
                }
            }
        }
    }

    public void ActivateCristals()
    {
        if (cristalMaterial != null)
        {
            Color finalEmission = emissionColor * Mathf.LinearToGammaSpace(emissionIntensity);
            propBlock.SetColor("_EmissionColor", finalEmission);
            cristalMaterial.SetPropertyBlock(propBlock);
        }
    }

    public void ActivateBossHealthBar()
    {
        if (bossHealthBarCanvas != null)
        {
            bossHealthBarCanvas.SetActive(true);
        }
    }
}
