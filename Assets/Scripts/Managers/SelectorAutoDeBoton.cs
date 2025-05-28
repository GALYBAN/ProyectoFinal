using UnityEngine;
using UnityEngine.EventSystems;

public class SelectorAutoDeBoton : MonoBehaviour
{
    public GameObject botonInicial;

    private void OnEnable()
    {
        if (botonInicial != null && botonInicial.activeInHierarchy)
        {
            StartCoroutine(SeleccionarBoton());
        }
    }

    private System.Collections.IEnumerator SeleccionarBoton()
    {
        yield return null; // Espera un frame para que el menú esté activo completamente

        EventSystem.current.SetSelectedGameObject(null); // Limpia la selección anterior
        EventSystem.current.SetSelectedGameObject(botonInicial); // Selecciona el botón nuevo
    }
}