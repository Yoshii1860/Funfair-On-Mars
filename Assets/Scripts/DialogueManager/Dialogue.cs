using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string SpeakerName;
    public DialogueLine[] DialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string DialogueText;
    public DialogueButton[] DialogueButtons;
}

[System.Serializable]
public class DialogueButton
{
    public string ButtonText;
    public string ActionKey;    
}