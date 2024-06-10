using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string, string, bool> OnStartGame;
    public UnityEvent<string, string, bool> OnChallangeComplete;

    [Header("Start Interactables")]
    [SerializeField] XRButtonInteractable _startButton;
    [SerializeField] GameObject[] _keyLights;
    [SerializeField] string _startGameString;
    [SerializeField] string _startBtnString;
    [SerializeField] AudioClip _startGameClip;

    [Space(10)]
    [Header("Drawer Interactables")]
    [SerializeField] private DrawerInteractable _drawer;
    private XRSocketInteractor _drawerKeySocket;

    [Space(10)]
    [Header("Combo Lock Interactables")]
    [SerializeField] private CombinationLock _comboLock;

    [Space(10)]
    [Header("Wall Interactables")]
    [SerializeField] private WallSystem _wall;
    [SerializeField] private XRSocketInteractor _wallSocket;
    [SerializeField] private GameObject[] _wallTeleportAreas;

    [Space(10)]
    [Header("Challenge Interactables")]
    [SerializeField] string[] _challengeStrings;
    [SerializeField] string[] _challengeBtnStrings;
    [SerializeField] AudioClip _challengeClip;

    [Space(10)]
    [Header("Light Switch Interactables")]
    [SerializeField] private LightSwitch _lightSwitch;


    private bool _isPressed = false;
    private int _currentChallenge = 0;

    public AudioClip GetStartGameClip() => _startGameClip;
    public AudioClip GetChallengeClip() => _challengeClip;

    private void Start()
    {
        if (_startButton != null)
        {
            _startButton.selectEntered.AddListener(OnStartButtonPressed);
        }

        OnStartGame?.Invoke(_startGameString, _startBtnString, true);
        SetDrawerInteractable();

        if (_comboLock != null)
        {
            _comboLock.UnlockAction += OnComboUnlock;
        }

        if (_wall != null)
        {
            SetWall();
        }

        if (_lightSwitch != null)
        {
            _lightSwitch.OnLightSwitchedOn.AddListener(LibraryLightOn);
        }
    }

    private void CompleteChallenge()
    {
        _currentChallenge++;
        if (_currentChallenge < _challengeStrings.Length)
        {
            Debug.Log("Challenge Completed");
            OnChallangeComplete?.Invoke(_challengeStrings[_currentChallenge], _challengeBtnStrings[_currentChallenge], false);
        }
        else if (_currentChallenge == _challengeStrings.Length)
        {
            Debug.Log("All Challenges Completed");
        }
    }

    private void LibraryLightOn()
    {
        CompleteChallenge();
    }

    private void OnStartButtonPressed(SelectEnterEventArgs args)
    {
        Debug.Log("Start Button Pressed");
        if (!_isPressed)
        {
            if (_keyLights != null)
            {
                for (int i = 0; i < _keyLights.Length; i++)
                {
                    _keyLights[i].SetActive(true);
                }
            }
            _isPressed = true;

            if (_currentChallenge < _challengeStrings.Length)
            {
                Debug.Log("Challenge Started");
                OnStartGame?.Invoke(_challengeStrings[_currentChallenge], _challengeBtnStrings[_currentChallenge], false);
            }
        }
    }

    private void SetDrawerInteractable()
    {
        if (_drawer != null)
        {
            _drawer.OnDrawerDetach.AddListener(OnDrawerDetach);
            _drawerKeySocket = _drawer.GetKeySocket();
            if (_drawerKeySocket != null)
            {
                _drawer.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs args)
    {
        Debug.Log("Drawer Socketed");
        CompleteChallenge();
    }

    private void OnDrawerDetach()
    {
        CompleteChallenge();
    }

    private void OnComboUnlock()
    {
        CompleteChallenge();
    }

    private void SetWall()
    {
        if (_wallSocket != null)
        {
            _wall.OnDestroy.AddListener(OnDestroyWall);
            _wallSocket = _wall.GetSocketInteractor();

            if (_wallSocket != null)
            {
                _wallSocket.selectEntered.AddListener(OnWallSocketed);
            }
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs args)
    {
        Debug.Log("Wall Socketed");
        CompleteChallenge();
    }

    private void OnDestroyWall()
    {
        for (int i = 0; i < _wallTeleportAreas.Length; i++)
        {
            _wallTeleportAreas[i].SetActive(true);
        }
        CompleteChallenge();
    }
}
