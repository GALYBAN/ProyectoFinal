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


    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        cagatio = GameObject.Find("CagatióUnity");
        animator = cagatio.GetComponent<Animator>();

        propBlock = new MaterialPropertyBlock();
        cristalMaterial.GetPropertyBlock(propBlock);

        cristalMaterial.material.EnableKeyword("_EMISSION");

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetTrigger("Intro");
        }
    }

    public void ActivateCagatioLights()
    {
        foreach (GameObject light in lightsCagatio)
        {
            light.SetActive(true);
        }
    }

    public void ActivateSceneLights()
    {
        foreach (GameObject light in lightsScene)
        {
            light.SetActive(true);
        }
    }

    public void ActivateCristals()
    {
        Color finalEmission = emissionColor * Mathf.LinearToGammaSpace(emissionIntensity);
        propBlock.SetColor("_EmissionColor", finalEmission);
        cristalMaterial.SetPropertyBlock(propBlock);
    }
}
