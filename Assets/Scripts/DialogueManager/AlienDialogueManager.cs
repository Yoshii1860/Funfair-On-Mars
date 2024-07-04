using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class AlienDialogueManager : MonoBehaviour
{
    public Dictionary<string, Action> alienActions;

    public static AlienDialogueManager Instance { get; private set; }

    [Header("Ferris Wheel References")]
    [SerializeField] private FerrisWheel _ferrisWheel;
    [Space(10)]

    [Header("Cannon Alien References")]
    [SerializeField] private GameObject _cannonAlien;
    [SerializeField] private XRGrabInteractable _pistol;
    [SerializeField] private Transform _rightGateDoor;
    [SerializeField] private Transform _leftGateDoor;
    [SerializeField] private float _gateOpenYRotation = 90f;
    [SerializeField] private float _gateOpenSpeed = 2f;
    private Vector3 _cannonAlienPosition;
    private Vector3 _cannonAlienMovePosition;
    private Animator _cannonAlienAnimator;
    private UnityEngine.AI.NavMeshAgent _cannonAlienNavMeshAgent;
    private bool _hasGameStarted = false;


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

    private void Start()
    {
        InitializeCannonAlien();
    }




    #region Initialization

    private void InitializeCannonAlien()
    {
        if (_cannonAlien != null)
        {
            _cannonAlienPosition = _cannonAlien.transform.position;
            _cannonAlienAnimator = _cannonAlien.GetComponent<Animator>();
            _cannonAlienNavMeshAgent = _cannonAlien.GetComponent<UnityEngine.AI.NavMeshAgent>();
            for (int i = 0; i < _cannonAlien.transform.childCount; i++)
            {
                if (_cannonAlien.transform.GetChild(i).gameObject.tag == "Waypoint")
                {
                    _cannonAlienMovePosition = _cannonAlien.transform.GetChild(i).position;
                    break;
                }
            }
        }
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

    #endregion



    #region Alien Actions

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
        StartCoroutine(MoveCannonAlien(_cannonAlienMovePosition));
        CloseDialogue();
        CannonGame();
    }

    private void CannonGame()
    {
        _pistol.selectEntered.AddListener((args) => OnPistolSelectEntered(args));
    }

    private void OnPistolSelectEntered(SelectEnterEventArgs args)
    {
        if (!_hasGameStarted)
        {
            _hasGameStarted = true;
            StartCoroutine(MoveCannonAlien(_cannonAlienPosition));
            StartCoroutine(OpenGate());
        }
    }

    private IEnumerator OpenGate()
    {
        float t = 0;
        while (t < _gateOpenSpeed)
        {
            t += Time.deltaTime;
            _rightGateDoor.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, -_gateOpenYRotation, 0), t / _gateOpenSpeed);
            _leftGateDoor.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, _gateOpenYRotation, 0), t / _gateOpenSpeed);
            yield return null;
        }
    }

    private IEnumerator MoveCannonAlien(Vector3 position)
    {
        _cannonAlienNavMeshAgent.SetDestination(position);
        _cannonAlienAnimator.SetBool("Walk", true);
        yield return new WaitUntil(() => Vector3.Distance(_cannonAlien.transform.position, position) < 0.5f);
        _cannonAlienAnimator.SetBool("Walk", false);
    }

    #endregion




    #region Dialogue Actions

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

    #endregion
}