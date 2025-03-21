using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCagatió : MonoBehaviour
{
    [SerializeField] private GameObject[] lightsCagatio;
    [SerializeField] private GameObject[] lightsScene;
    [SerializeField] private GameObject cristal;
    [SerializeField] private Material cristalMaterial;
    [SerializeField] private GameObject cagatio;
    [SerializeField] private Animator animator;

    void Awake()
    {
        cagatio = GameObject.Find("CagatióUnity");
        animator = cagatio.GetComponent<Animator>();
        cristalMaterial = cristal.GetComponent<Renderer>().materials[0];
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
        float emissionIntensity = cristalMaterial.GetFloat("_EmissionIntensity");
        cristalMaterial.SetFloat("_EmissionIntensity", 17.3f);
    }
}
