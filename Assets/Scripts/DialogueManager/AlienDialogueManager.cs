using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class AlienDialogueManager : MonoBehaviour
{
    public Dictionary<string, Action> alienActions;

    public static AlienDialogueManager Instance { get; private set; }

    [Header("Alien Actions References")]
    [SerializeField] private FerrisWheel _ferrisWheel;

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

        PopulateActionDictionary();
    }

    private void PopulateActionDictionary()
    {
        alienActions = new Dictionary<string, Action>();
        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            if (method.GetParameters().Length == 0 && method.ReturnType == typeof(void))
            {
                Action action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                alienActions[method.Name] = action;
            }
        }
    }

    public void PlayButtonAction(string actionKey)
    {
        if (!alienActions.ContainsKey(actionKey))
        {
            Debug.LogWarning($"Action '{actionKey}' not found in AlienDialogueManager.");
        }
        else
        {
            alienActions[actionKey].Invoke();
        }
    }

    private void GetOnFerriesWheel()
    {
        if (_ferrisWheel != null)
        {
            if (_ferrisWheel.IsRotating)
            {
                _ferrisWheel.StopFerrisWheel();

            }
            
            CloseDialogue();
        }
    }

    private void OpenWayToCannons()
    {
        // Open the way to the cannons
    }

    private void ContinueConversation()
    {
        DialogueManager.Instance.DisplayNextLine();
    }

    private void FinishDialogue()
    {
        DialogueManager.Instance.FinishDialogue();
    }

    private void CloseDialogue()
    {
        DialogueManager.Instance.CloseDialogue();
    }
}