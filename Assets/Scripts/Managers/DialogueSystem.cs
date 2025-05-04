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
    public float autoAdvanceDelay = 2f;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private Coroutine typingCoroutine;

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

        Debug.Log($"Starting dialogue with {lines.Length} lines");
        dialogueQueue.Clear();
        foreach (DialogueLine line in lines)
        {
            dialogueQueue.Enqueue(line);
        }
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        Debug.Log("DisplayNextLine called");
        
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            dialogueText.text = dialogueQueue.Peek().text;
            return;
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();
        Debug.Log($"Displaying line: {currentLine.text}");
        
        if (characterNameText != null)
        {
            characterNameText.text = currentLine.characterName;
        }
        else
        {
            Debug.LogError("CharacterNameText is not assigned!");
        }

        if (dialogueText != null)
        {
            dialogueText.color = currentLine.textColor;
        }
        else
        {
            Debug.LogError("DialogueText is not assigned!");
        }

        if (currentLine.voiceClip != null)
        {
            if (SOUNDManager.Instance != null)
            {
                SOUNDManager.Instance.PlayVoiceClip(currentLine.voiceClip);
            }
            else
            {
                Debug.LogError("SOUNDManager.Instance is null!");
            }
        }

        typingCoroutine = StartCoroutine(TypeText(currentLine.text));
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

        if (dialogueQueue.Count > 0)
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            DisplayNextLine();
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Ending dialogue");
        dialoguePanel.SetActive(false);
    }
} 