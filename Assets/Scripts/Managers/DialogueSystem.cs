using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public string text;
    public AudioClip voiceClip;
    public float displayTime = 3f;
    public Color textColor = Color.white;
}

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float textSpeed = 0.05f;
    public float autoAdvanceDelay = 0.5f;  // Tiempo entre líneas

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Coroutine dialogueCoroutine;
    

    private void Awake()
    {
        Debug.Log("DialogueSystem Awake called");
        
        if (Instance == null)
        {
            Debug.Log("Setting up DialogueSystem Instance");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple DialogueSystem instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("DialogueSystem Start called");
        
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is not assigned in DialogueSystem!");
            return;
        }

        if (characterNameText == null)
        {
            Debug.LogError("CharacterNameText is not assigned in DialogueSystem!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("DialogueText is not assigned in DialogueSystem!");
            return;
        }

        dialoguePanel.SetActive(false);
        Debug.Log("DialogueSystem initialized successfully");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            SkipToNextLine();
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        Debug.Log("StartDialogue called");
        
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("No dialogue lines provided!");
            return;
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is not assigned!");
            return;
        }

        Debug.Log($"Number of lines to display: {lines.Length}");
        Debug.Log($"First line character: {lines[0].characterName}");
        Debug.Log($"First line text: {lines[0].text}");
        Debug.Log($"First line display time: {lines[0].displayTime}");

        // Detener cualquier diálogo actual
        if (dialogueCoroutine != null)
        {
            Debug.Log("Stopping existing dialogue coroutine");
            StopCoroutine(dialogueCoroutine);
        }

        Debug.Log($"Starting dialogue with {lines.Length} lines");
        dialogueQueue.Clear();
        foreach (DialogueLine line in lines)
        {
            dialogueQueue.Enqueue(line);
        }
        dialoguePanel.SetActive(true);
        
        // Iniciar la secuencia de diálogo
        dialogueCoroutine = StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        Debug.Log("PlayDialogueSequence started");
        while (dialogueQueue.Count > 0)
        {
            DialogueLine currentLine = dialogueQueue.Dequeue();
            Debug.Log($"Displaying line: {currentLine.text}");

            // Configurar el nombre del personaje
            if (characterNameText != null)
            {
                characterNameText.text = currentLine.characterName;
                Debug.Log($"Setting character name to: {currentLine.characterName}");
            }
            else
            {
                Debug.LogError("characterNameText is null!");
            }

            // Configurar el color del texto
            if (dialogueText != null)
            {
                dialogueText.color = currentLine.textColor;
                Debug.Log($"Setting text color to: {currentLine.textColor}");
            }
            else
            {
                Debug.LogError("dialogueText is null!");
            }

            // Reproducir el clip de voz si existe
            if (currentLine.voiceClip != null)
            {
                if (SOUNDManager.Instance != null)
                {
                    Debug.Log($"Playing voice clip: {currentLine.voiceClip.name}");
                    SOUNDManager.Instance.PlayVoiceClip(currentLine.voiceClip);
                    // Esperar un pequeño delay para asegurar que el audio comienza
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    Debug.LogError("SOUNDManager.Instance is null!");
                }
            }
            else
            {
                Debug.Log("No voice clip for this line");
            }

            // Mostrar el texto con efecto de escritura
            typingCoroutine = StartCoroutine(TypeText(currentLine.text));
            yield return typingCoroutine;

            Debug.Log($"Waiting for display time: {currentLine.displayTime} seconds");
            // Esperar el tiempo de visualización especificado en el DialogueLine
            yield return new WaitForSeconds(currentLine.displayTime);

            Debug.Log($"Waiting for auto advance delay: {autoAdvanceDelay} seconds");
            // Esperar un pequeño delay entre líneas
            yield return new WaitForSeconds(autoAdvanceDelay);
        }

        // Cuando se acaba el diálogo
        EndDialogue();
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
    }

    private void EndDialogue()
    {
        Debug.Log("Ending dialogue");
        dialoguePanel.SetActive(false);
    }

    // Método para saltar al siguiente diálogo
    public void SkipToNextLine()
    {
        Debug.Log("SkipToNextLine called");
        
        // Si está escribiendo, completar el texto actual de inmediato
        if (isTyping)
        {
            Debug.Log("Skipping typing animation");
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                // Aseguramos que el texto se muestre completo
                if (dialogueText != null)
                {
                    string currentDialogueText = dialogueText.text;
                    // Actualizar solo si hay texto actual para mostrar
                    if (!string.IsNullOrEmpty(currentDialogueText))
                    {
                        dialogueText.text = currentDialogueText; // Mantener el texto actual
                    }
                }
                isTyping = false;
            }
            return; // Importante: Salir del método aquí para dar control al jugador
        }
        // Si no está escribiendo pero hay una secuencia de diálogo en curso
        else if (dialogueCoroutine != null)
        {
            Debug.Log("Skipping to next dialogue line");
            try
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null; // Evitar referencias nulas
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al detener la coroutina de diálogo: " + e.Message);
            }
            
            // Solo avanzamos al siguiente diálogo si hay más en la cola
            if (dialogueQueue.Count > 0)
            {
                dialogueCoroutine = StartCoroutine(PlayDialogueSequence());
            }
            else
            {
                // Si no hay más diálogos, terminamos
                EndDialogue();
            }
        }
        else
        {
            // Si no hay ninguna coroutina activa pero el panel sigue visible
            if (dialoguePanel != null && dialoguePanel.activeSelf)
            {
                Debug.Log("No dialogue coroutine active but panel is visible, ending dialogue");
                EndDialogue();
            }
        }
    }
} 