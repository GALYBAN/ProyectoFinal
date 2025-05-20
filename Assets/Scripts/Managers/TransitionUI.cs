using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem; // Para detectar input (opcional, puedes usar Input tradicional también)
using System;  // Para usar Action

public class TransitionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image flashImage;
    [SerializeField] private TextMeshProUGUI transitionText;
    
    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private float textFadeInDuration = 1f;
    [SerializeField] private float textFadeOutDuration = 1f;
    
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData transitionDialogue;
    
    // Evento para notificar cuando se completa o salta una transición
    public event Action OnTransitionSkipped;
    public event Action OnTransitionCompleted;
    
    private Color flashColor = new Color(1f, 1f, 1f, 0f);
    private Color textColor = new Color(0f, 0f, 0f, 0f);
    private int currentTextIndex = 0;
    
    private bool isTransitioning = false;
    private bool isTextFading = false;
    private Coroutine currentTransitionCoroutine;
    private Coroutine currentTextFadeCoroutine;

    private void Start()
    {
        // Initialize UI elements
        if (flashImage != null)
        {
            flashImage.color = flashColor;
            flashImage.gameObject.SetActive(true);
        }
        
        if (transitionText != null)
        {
            transitionText.color = textColor;
            transitionText.gameObject.SetActive(true);
        }
    }
    
    private void Update()
    {
        // Detectar entrada para saltar diálogo (puedes cambiar esto a Input.GetKeyDown si prefieres)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isTransitioning)
        {
            SkipCurrentTransition();
        }
        // Alternativa con Input tradicional
        // if (Input.GetKeyDown(KeyCode.Space) && isTransitioning)
        // {
        //     SkipCurrentTransition();
        // }
    }

    public IEnumerator PlayTransitionSequence()
    {
        Debug.Log("Starting transition sequence");
        isTransitioning = true;
        
        // Flash in
        try {
            currentTransitionCoroutine = StartCoroutine(FadeFlash(0f, 1f, flashDuration));
        } catch (System.Exception e) {
            Debug.LogError("Error al iniciar el flash: " + e.Message);
            isTransitioning = false;
            yield break;
        }
        
        yield return currentTransitionCoroutine;
        
        // Show all texts in sequence
        for (int i = 0; i < transitionDialogue.dialogueLines.Length; i++)
        {
            // Si la transición se ha cancelado, salimos
            if (!isTransitioning) yield break;
            
            currentTextIndex = i;
            DialogueLine currentLine = transitionDialogue.dialogueLines[i];
            transitionText.text = currentLine.text;
            transitionText.color = new Color(currentLine.textColor.r, currentLine.textColor.g, currentLine.textColor.b, 0f);
            
            // Play voice clip if available
            if (currentLine.voiceClip != null && SOUNDManager.Instance != null)
            {
                try {
                    SOUNDManager.Instance.PlayVoiceClip(currentLine.voiceClip);
                } catch (System.Exception e) {
                    Debug.LogError("Error al reproducir audio: " + e.Message);
                }
            }
            
            // Fade in text
            isTextFading = true;
            try {
                currentTextFadeCoroutine = StartCoroutine(FadeText(0f, 1f, textFadeInDuration));
            } catch (System.Exception e) {
                Debug.LogError("Error al iniciar fade in: " + e.Message);
                isTextFading = false;
                currentTextFadeCoroutine = null;
                continue; // Intentamos seguir con el siguiente texto
            }
            
            yield return currentTextFadeCoroutine;
            isTextFading = false;
            currentTextFadeCoroutine = null;
            
            // Si la transición se ha cancelado, salimos
            if (!isTransitioning) yield break;
            
            // Hold text for the specified display time
            float elapsedDisplayTime = 0f;
            while (elapsedDisplayTime < currentLine.displayTime && isTransitioning)
            {
                elapsedDisplayTime += Time.deltaTime;
                yield return null;
            }
            
            // Si la transición se ha cancelado, salimos
            if (!isTransitioning) yield break;
            
            // Fade out text
            isTextFading = true;
            try {
                currentTextFadeCoroutine = StartCoroutine(FadeText(1f, 0f, textFadeOutDuration));
            } catch (System.Exception e) {
                Debug.LogError("Error al iniciar fade out: " + e.Message);
                isTextFading = false;
                currentTextFadeCoroutine = null;
                continue; // Intentamos seguir con el siguiente texto
            }
            
            yield return currentTextFadeCoroutine;
            isTextFading = false;
            currentTextFadeCoroutine = null;
        }
        
        // Flash out
        try {
            currentTransitionCoroutine = StartCoroutine(FadeFlash(1f, 0f, flashDuration));
        } catch (System.Exception e) {
            Debug.LogError("Error al iniciar el flash final: " + e.Message);
        }
        
        yield return currentTransitionCoroutine;
        
        // Disparar evento de completado normal
        if (OnTransitionCompleted != null)
        {
            try {
                OnTransitionCompleted.Invoke();
            } catch (System.Exception e) {
                Debug.LogError("Error al invocar OnTransitionCompleted: " + e.Message);
            }
        }
        
        isTransitioning = false;
        Debug.Log("Transition sequence complete");
        currentTransitionCoroutine = null;
    }

    private IEnumerator FadeFlash(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            flashColor.a = alpha;
            flashImage.color = flashColor;
            yield return null;
        }
        flashColor.a = endAlpha;
        flashImage.color = flashColor;
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            textColor.a = alpha;
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, alpha);
            yield return null;
        }
        textColor.a = endAlpha;
        transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, endAlpha);
    }
    
    // Método para saltar la transición actual
    public void SkipCurrentTransition()
    {
        Debug.Log("Skipping current transition");
        
        if (!isTransitioning)
        {
            Debug.LogWarning("Intentando saltar una transición que no está activa");
            return;
        }
        
        try
        {
            // Independientemente del estado actual, completar toda la transición inmediatamente
            CompleteTransitionImmediately();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al saltar la transición: " + e.Message);
            // Resetear estado en caso de error
            isTransitioning = false;
            isTextFading = false;
            
            // Detener todas las corutinas relacionadas con la transición
            if (currentTextFadeCoroutine != null)
            {
                StopCoroutine(currentTextFadeCoroutine);
                currentTextFadeCoroutine = null;
            }
            
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
                currentTransitionCoroutine = null;
            }
            
            // Ocultar UI en caso de error para no dejar elementos visibles
            if (flashImage != null) flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0f);
            if (transitionText != null) transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, 0f);
            
            // Notificar el salto para evitar bloqueos
            if (OnTransitionSkipped != null)
            {
                OnTransitionSkipped.Invoke();
            }
        }
    }
    
    // Método para completar inmediatamente toda la transición
    private void CompleteTransitionImmediately()
    {
        Debug.Log("Completing transition immediately");
        
        try
        {
            // Detener todas las coroutinas relacionadas con la transición
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
                currentTransitionCoroutine = null;
            }
            
            if (currentTextFadeCoroutine != null)
            {
                StopCoroutine(currentTextFadeCoroutine);
                currentTextFadeCoroutine = null;
            }
            
            // Escondemos todos los elementos de UI
            if (flashImage != null)
            {
                flashColor.a = 0f;
                flashImage.color = flashColor;
            }
            
            if (transitionText != null)
            {
                textColor.a = 0f;
                transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, 0f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al completar la transición: " + e.Message);
        }
        finally
        {
            // Siempre resetear el estado, aunque haya errores
            isTransitioning = false;
            isTextFading = false;
            
            // Asegurarse de que el CharacterTransitionManager sepa que la transición ha terminado
            if (OnTransitionSkipped != null)
            {
                Debug.Log("Notificando que la transición ha sido saltada");
                OnTransitionSkipped.Invoke();
            }
        }
    }
}