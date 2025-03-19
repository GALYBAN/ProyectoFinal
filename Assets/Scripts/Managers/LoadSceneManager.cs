using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar; // Barra de carga opcional

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        // Obtener el nombre de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("NextScene", "MainMenu");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // Espera hasta que la barra esté llena

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress; // Actualizar la barra de carga

            // Si la carga está completa, activar la escena
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); // Pequeña pausa para efecto visual
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
