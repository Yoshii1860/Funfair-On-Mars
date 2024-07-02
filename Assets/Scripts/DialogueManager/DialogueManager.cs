using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsDialogueActive = false;

    public Transform eventSystem;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public XRButtonInteractable[] dialogueButtons;

    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private string firstActionKey;
    private string secondActionKey;

    private Transform resetTransform;
    private DialogueTrigger currentDialogueTrigger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        resetTransform = new GameObject().transform;
        resetTransform.position = new Vector3(0, -1000, 0);
    }

    public void StartDialogue(Dialogue dialogue, Transform canvasTransform, DialogueTrigger dialogueTrigger)
    {
        IsDialogueActive = true;
        currentDialogueTrigger = dialogueTrigger;
        MoveCanvas(canvasTransform);
        currentDialogue = dialogue;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    private void MoveCanvas(Transform canvasTransform)
    {
        eventSystem.position = canvasTransform.position;
        eventSystem.rotation = canvasTransform.rotation;
    }

    private void ResetCanvas()
    {
        eventSystem.position = resetTransform.position;
        eventSystem.rotation = resetTransform.rotation;
        IsDialogueActive = false;
        speakerNameText.text = "";
        dialogueText.text = "";
        ClearButtons();
    }

    private void DisplayCurrentLine()
    {
        ClearButtons();

        if (currentLineIndex <= currentDialogue.DialogueLines.Length)
        {
            DialogueLine line = currentDialogue.DialogueLines[currentLineIndex];
            speakerNameText.text = currentDialogue.SpeakerName;
            dialogueText.text = line.DialogueText;

            for (int i = 0; i < dialogueButtons.Length; i++)
            {
                if (i < line.DialogueButtons.Length)
                {
                    dialogueButtons[i].gameObject.SetActive(true);
                    dialogueButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = line.DialogueButtons[i].ButtonText;

                    int capturedIndex = i;
                    if (i == 0)
                    {
                        firstActionKey = line.DialogueButtons[i].ActionKey;
                    }
                    else if (i == 1)
                    {
                        secondActionKey = line.DialogueButtons[i].ActionKey;
                    }

                    dialogueButtons[capturedIndex].selectEntered.AddListener((SelectEnterEventArgs args) =>
                    {
                        AlienDialogueManager.Instance.PlayButtonAction(capturedIndex == 0 ? firstActionKey : secondActionKey);
                    });
                }
                else
                {
                    dialogueButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("Current line index is out of bounds. " + currentLineIndex + " / " + currentDialogue.DialogueLines.Length);
        }
    }

    private void ClearButtons()
    {
        foreach (var button in dialogueButtons)
        {
            button.selectEntered.RemoveAllListeners();
        }
    }


    private void SetNewDialogue()
    {
        currentDialogueTrigger.SetNewDialogue();
    }

    public void DisplayNextLine()
    {
        currentLineIndex++;
        DisplayCurrentLine();
    }

    public void FinishDialogue()
    {
        currentLineIndex = currentDialogue.DialogueLines.Length - 1;
        DisplayCurrentLine();
    }

    public void CloseDialogue()
    {
        ResetCanvas();
    }
}