using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private GameObject canvasPortada;
    [SerializeField] private GameObject canvasMenu;
    [SerializeField] private GameObject canvasOpciones;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private Animator[] anim;

    void Awake()
    {
        canvasPortada = GameObject.Find("CanvasPortada");
        canvasMenu = GameObject.Find("CanvasMenu");
        canvasOpciones = GameObject.Find("CanvasOpciones");
        canvasGroup = GameObject.Find("CanvasOpciones").GetComponent<CanvasGroup>();

    }

    void Start()
    {
        anim = GetComponentsInChildren<Animator>();

        canvasPortada.SetActive(true);
        canvasMenu.SetActive(false);
        canvasGroup.alpha = 0f;

    }

    private void SetBoolInAllAnimators(string paramName, bool value)
    {
        foreach (Animator animator in anim)
        {
            animator.SetBool(paramName, value);
        }
    }

    public void AMenuPrincipal()
    {
        SetBoolInAllAnimators("PortadaCamera", true);
        SetBoolInAllAnimators("Portada", true);
    }

    public void AMenuOpciones()
    {
        SetBoolInAllAnimators("Page1", true);
    }

    public void Regresa()
    {
        SetBoolInAllAnimators("Page1", false);
    }

    public void Animation1Start()
    {
        canvasPortada.SetActive(false);
    }

    public void Animation1Complete()
    {
        canvasMenu.SetActive(true);
    }

    public void Animation2Start()
    {
        canvasMenu.SetActive(false);
    }

    public void Animation2Complete()
    {
        StartCoroutine(IncrementAlpha());
    }

    IEnumerator IncrementAlpha()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            float alpha = Mathf.SmoothStep(0f, 1f, elapsedTime / 1);
            canvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // Asegurarse de que el alpha llegue a 1
    }

    public void Animation3Start()
    {
        StartCoroutine(DecreaseAlpha());
    }

    IEnumerator DecreaseAlpha()
    {
        float t = 0f;
        while (t < 1f)
        {
            canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, t);
            t += 0.01f; // incrementa t en pequeños pasos
            yield return null;
        }
        canvasGroup.alpha = 0f; // asegurarse de que el alpha llegue a 0
    }

    public void Animation3Complete()
    {
        canvasMenu.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void NovaPartida()
    {
        SceneManager.LoadScene(1);
    }
}

