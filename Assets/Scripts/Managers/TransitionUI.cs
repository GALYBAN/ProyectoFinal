using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    
    private Color flashColor = new Color(1f, 1f, 1f, 0f);
    private Color textColor = new Color(0f, 0f, 0f, 0f);
    private int currentTextIndex = 0;

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

    public IEnumerator PlayTransitionSequence()
    {
        // Flash in
        yield return StartCoroutine(FadeFlash(0f, 1f, flashDuration));
        
        // Show all texts in sequence
        for (int i = 0; i < transitionDialogue.dialogueLines.Length; i++)
        {
            currentTextIndex = i;
            DialogueLine currentLine = transitionDialogue.dialogueLines[i];
            transitionText.text = currentLine.text;
            transitionText.color = currentLine.textColor;
            
            // Play voice clip if available
            if (currentLine.voiceClip != null)
            {
                SOUNDManager.Instance.PlayVoiceClip(currentLine.voiceClip);
            }
            
            // Fade in text
            yield return StartCoroutine(FadeText(0f, 1f, textFadeInDuration));
            
            // Hold text for the specified display time
            yield return new WaitForSeconds(currentLine.displayTime);
            
            // Fade out text
            yield return StartCoroutine(FadeText(1f, 0f, textFadeOutDuration));
        }
        
        // Flash out
        yield return StartCoroutine(FadeFlash(1f, 0f, flashDuration));
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
} 