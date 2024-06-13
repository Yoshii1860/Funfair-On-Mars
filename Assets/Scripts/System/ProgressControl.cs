using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string, string, bool> OnChallangeComplete;

    public class ChallengeData
    {
        public string ChallengeString;
        public string ButtonString;
    }

    private Dictionary<int, ChallengeData> _challenges;

    [Header("Start Interactables")]
    [SerializeField] private XRButtonInteractable _startButton;
    [SerializeField] private GameObject[] _keyLights;
    [SerializeField] private AudioClip _startGameClip;
    private bool _challangesCompleted = false;
    [SerializeField] private int _challangeNumber;

    [Space(10)]
    [Header("Challenge Interactables")]
    [SerializeField] string[] _challengeStrings;
    [SerializeField] string[] _challengeBtnStrings;
    [SerializeField] AudioClip _challengeClip;

    [Space(10)]
    [Header("Challenge 1 & 2: Drawer")]
    private int _drawerSocketChallenge = 1;
    private int _drawerDetachChallenge = 2;
    [SerializeField] private DrawerInteractable _drawer;
    private XRSocketInteractor _drawerKeySocket;

    [Space(10)]
    [Header("Challenge 3: Combo Lock")]
    private int _comboLockChallenge = 3;
    [SerializeField] private CombinationLock _comboLock;

    [Space(10)]
    [Header("Challenge 4 & 5: Wall Interactables")]
    private int _wallSocketChallenge = 4;
    private int _wallDestroyChallenge = 5;
    [SerializeField] private WallSystem _wall;
    [SerializeField] private XRSocketInteractor _wallSocket;
    [SerializeField] private GameObject[] _wallTeleportAreas;

    [Space(10)]
    [Header("Challenge 6: Light Switch")]
    private int _lightSwitchChallenge = 6;
    [SerializeField] private LightSwitch _lightSwitch;

    [Space(10)]
    [Header("Challenge 7: Robot")]
    private int _robotChallenge = 7;
    [SerializeField] private NavMeshRobot _robot;
    [SerializeField] private int _wallCubesToDestroy;
    private int _wallCubesDestroyed = 0;

    private bool _isPressed = false;
    private int _currentChallenge = 0;

    public AudioClip GetStartGameClip() => _startGameClip;
    public AudioClip GetChallengeClip() => _challengeClip;

    private void Start()
    {
        _challenges = new Dictionary<int, ChallengeData>();

        foreach (string challenge in _challengeStrings)
        {
            ChallengeData data = new ChallengeData();
            data.ChallengeString = challenge;
            data.ButtonString = _challengeBtnStrings[_challenges.Count];
            _challenges.Add(_challenges.Count, data);
        }

        if (_startButton != null)
        {
            _startButton.selectEntered.AddListener(OnStartButtonPressed);
        }

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

        if (_robot != null)
        {
            _robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
        }
    }

    private void CompleteChallenge()
    {
        _currentChallenge++;
        if (_currentChallenge < _challengeStrings.Length)
        {
            Debug.Log("Challenge Completed");
            OnChallangeComplete?.Invoke(_challengeStrings[_currentChallenge], _challengeBtnStrings[_currentChallenge], true);
        }
        else if (_currentChallenge == _challengeStrings.Length)
        {
            Debug.Log("All Challenges Completed");
            OnChallangeComplete?.Invoke(_challengeStrings[_currentChallenge], _challengeBtnStrings[_currentChallenge], true);
        }
    }

    ////////// CHALLENGE 0 //////////
    private void OnStartButtonPressed(SelectEnterEventArgs args)
    {
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

            if (_currentChallenge < _challengeStrings.Length && _currentChallenge == 0)
            {
                Debug.Log("Challenge Started");
                CompleteChallenge();
            }
        }
    }

    ////////// CHALLENGE 1 //////////
    private void OnDrawerSocketed(SelectEnterEventArgs args)
    {
        if (_drawerSocketChallenge == _currentChallenge)
        {
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 2 //////////
    private void OnDrawerDetach()
    {
        if (_drawerDetachChallenge == _currentChallenge)
        {
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 3 //////////
    private void OnComboUnlock()
    {
        if (_comboLockChallenge == _currentChallenge)
        {
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 4 //////////
    private void OnWallSocketed(SelectEnterEventArgs args)
    {
        if (_wallSocketChallenge == _currentChallenge)
        {
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 5 //////////
    private void OnDestroyWall()
    {
        if (_wallDestroyChallenge == _currentChallenge)
        {
            for (int i = 0; i < _wallTeleportAreas.Length; i++)
            {
                _wallTeleportAreas[i].SetActive(true);
            }
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 6 //////////
    private void LibraryLightOn()
    {
        if (_lightSwitchChallenge == _currentChallenge)
        {
            CompleteChallenge();
        }
    }

    ////////// CHALLENGE 7 //////////
    private void OnDestroyWallCube()
    {
        _wallCubesDestroyed++;

        if (_wallCubesDestroyed >= _wallCubesToDestroy)
        {
            if (_robotChallenge == _currentChallenge)
            {
                CompleteChallenge();
            }
        }
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
}
