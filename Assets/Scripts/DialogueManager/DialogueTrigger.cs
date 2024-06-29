using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DialogueTrigger : XRSimpleInteractable
{
    public Dialogue _dialogue;
    [SerializeField] Dialogue[] _otherDialogues;
    private int _dialogueIndex = 0;
    [SerializeField] private float _gazeTime = 1f;
    [SerializeField] private float _resetDistance = 4f;

    private Animator _animator;
    private Transform _canvasTransform;

    private float _gazeTimer = 0f;
    private bool _isGazing = false;
    private bool _isSpeaking = false;

    private bool _isResetting = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("UI"))
            {
                _canvasTransform = child;
                break;
            }
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        _isGazing = true;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        _isGazing = false;
    }

    private void Update()
    {
        if (_isGazing && DialogueManager.Instance.IsDialogueActive == false)
        {
            _gazeTimer += Time.deltaTime;
            if (_gazeTimer >= _gazeTime && !_isResetting)
            {
                _animator.SetTrigger("Wave");
                DialogueManager.Instance.StartDialogue(_dialogue, _canvasTransform, this);
                _isGazing = false;
                _isSpeaking = true;
                _gazeTimer = 0f;
                StartCoroutine(ResetTimer());
            }
        }
        else if (_isSpeaking)
        {
            if (Vector3.Distance(GameManager.Instance.XRRig.position, transform.position) > _resetDistance)
            {
                DialogueManager.Instance.CloseDialogue();
                _isSpeaking = false;
            }
        }
    }

    private IEnumerator ResetTimer()
    {
        _isResetting = true;
        yield return new WaitUntil(() => DialogueManager.Instance.IsDialogueActive == false);
        _animator.SetTrigger("Goodbye");
        _isSpeaking = false;
        yield return new WaitForSeconds(5f);
        _gazeTimer = 0f;
        _isResetting = false;
    }

    public void SetNewDialogue()
    {
        if (_otherDialogues.Length > 0 && _dialogueIndex < _otherDialogues.Length)
        {
            _dialogue = _otherDialogues[_dialogueIndex];
            _dialogueIndex++;
        }
    }
}