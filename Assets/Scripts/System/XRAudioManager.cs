using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Local Audio Settings")]
    [SerializeField] private AudioSource _backgroundSource;
    [SerializeField] private AudioClip _backgroundClip;
    [SerializeField] private AudioClip _fallBackClip;
    
    [Header("Grab Interactables")]
    [SerializeField] private AudioSource _grabSource;
    [SerializeField] private AudioSource _activateSource;
    [Space(10)]
    [SerializeField] private AudioClip _grabClip;
    [SerializeField] private AudioClip _keyClip;
    [SerializeField] private AudioClip _activateGrabClip;
    [SerializeField] private AudioClip _activateWandClip;
    private XRGrabInteractable[] _grabInteractables;

    [Header("Progress Control")]
    [SerializeField] private ProgressControl _progressControl;
    private AudioSource _progressSource;
    private AudioClip _startGameClip;
    private AudioClip _challengeCompleteClip;
    private bool _hasStarted = false;   

    [Header("Drawer Interactables")]
    [SerializeField] private DrawerInteractable _drawer;
    private XRSocketInteractor _drawerKeySocket;
    private XRPhysicsButtonInteractable _drawerPyhsicsButton;
    private AudioSource _drawerSocketSource;
    private AudioSource _drawerSource;
    private AudioClip _drawerSocketClip;
    private AudioClip _drawerMoveClip;
    private bool _isDetached = false;
    
    [Header("Hinge Interactables")]
    [SerializeField] private SimpleHingeInteractable[] _cabinetDoors = new SimpleHingeInteractable[2];
    private AudioSource[] _cabinetDoorSources;
    private AudioClip _cabinetDoorMoveClip;

    [Header("Combo Lock")]
    [SerializeField] private CombinationLock _comboLock;
    private AudioSource _comboLockSource;
    private AudioClip _unlockClip;
    private AudioClip _comboButtonClip;
    private AudioClip _incorrectClip;

    [Header("Wall")]
    [SerializeField] private WallSystem _wall;
     private XRSocketInteractor _wallSocket;
    private AudioSource _wallSource;
    private AudioSource _wallSocketSource;
    private AudioClip _wallExplosionClip;
    private AudioClip _wallSocketClip;

    [Header("Joystick")]
    [SerializeField] private SimpleHingeInteractable _joystick;
    private AudioSource _joystickSource;
    private AudioClip _joystickClip;

    [Header("Robot")]
    [SerializeField] private NavMeshRobot _robot;
    private AudioSource _robotSource;
    private AudioClip _destroyClip;

    private const string FALLBACKCLIP_NAME = "fallBackClip";

    private void OnEnable()
    {
        if (_fallBackClip == null)
        {
            _fallBackClip = AudioClip.Create(FALLBACKCLIP_NAME, 1, 1, 1000, true);
        }

        SetGrabInteractables();

        if (_progressControl != null)
        {
            SetProgressControl();
            _progressControl.OnChallangeComplete.AddListener(ChallangeCompleted);
        }

        if (_drawer != null)
        {
            SetDrawerInteractable();
        }

        _cabinetDoorSources = new AudioSource[_cabinetDoors.Length];
        for (int i = 0; i < _cabinetDoors.Length; i++)
        {
            if (_cabinetDoors[i] != null)
            {
                SetCabinetDoors(i);
            }
        }

        if (_comboLock != null)
        {
            SetComboLock();
        }

        if (_wall != null)
        {
            SetWall();
        }

        if (_joystick != null)
        {
            SetJoystick();
        }

        if (_robot != null)
        {
            SetRobot();
        }
    }

    private void OnDisable()
    {
        if (_progressControl != null)
        {
            _progressControl.OnChallangeComplete.RemoveListener(ChallangeCompleted);
        }

        for (int i = 0; i < _grabInteractables.Length; i++)
        {
            _grabInteractables[i].selectEntered.RemoveListener(OnSelectEnterGrab);
            _grabInteractables[i].selectExited.RemoveListener(OnSelectExitGrab);
            _grabInteractables[i].activated.RemoveListener(OnActivatedGrab);
        }

        if (_drawer != null)
        {
            _drawer.selectEntered.RemoveListener(OnDrawerMove);
            _drawer.selectExited.RemoveListener(OnDrawerStop);
            _drawer.OnDrawerDetach.RemoveListener(OnDrawerDetach);

            if (_drawerKeySocket != null)
            {
                _drawerKeySocket.selectEntered.RemoveListener(OnDrawerSocketed);
            }
        }

        for (int i = 0; i < _cabinetDoors.Length; i++)
        {
            _cabinetDoors[i].OnHingeSelected.RemoveListener(OnCabinetDoorMove);
            _cabinetDoors[i].selectExited.RemoveListener(OnCabinetDoorStop);
        }

        if (_comboLock != null)
        {
            _comboLock.UnlockAction -= OnComboUnlock;
            _comboLock.IncorrectAction -= OnIncorrectCombo;
            _comboLock.ComboButtonPressed -= OnComboButtonPress;
        }

        if (_wall != null)
        {
            _wall.OnDestroy.RemoveListener(OnDestroyWall);

            if (_wallSocket != null)
            {
                _wallSocket.selectEntered.RemoveListener(OnWallSocketed);
            }
        }
    }

    private void StartGame(string msg, string index, bool isButton)
    {
        Debug.Log("Start Game Called on AudioManager");
        if (!_hasStarted)
        {
            _hasStarted = true;
            if (_backgroundSource != null && _backgroundClip != null)
            {
                _backgroundSource.clip = _backgroundClip;
                _backgroundSource.Play();
            }
        }
        else
        {
            if (_progressSource != null && _startGameClip != null)
            {
                _progressSource.clip = _startGameClip;
                _progressSource.Play();
            }
        }
    }

    private void ChallangeCompleted(string msg, string index, bool isButton)
    {
        if (_progressSource != null && _challengeCompleteClip != null)
        {
            _progressSource.clip = _challengeCompleteClip;
            _progressSource.Play();
        }
    }

    private void OnSelectEnterGrab(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Key"))
        {
            PlayGrabSound(_keyClip);
        }
        else
        {
            PlayGrabSound();
        }
    }

    private void OnSelectExitGrab(SelectExitEventArgs args)
    {
        PlayGrabSound();
    }

    private void OnActivatedGrab(ActivateEventArgs args)
    {
        GameObject tempGameObject = args.interactableObject.transform.gameObject;
        if (tempGameObject.GetComponent<WandControl>() != null)
        {
            _activateSource.clip = _activateWandClip;
        }
        else
        {
            _activateSource.clip = _activateGrabClip;
        }
        _activateSource.Play();
    }

    private void OnDestroyWall()
    {
        _wallSource.Play();
    }

    private void PlayGrabSound(AudioClip clip = null)
    {
        if (clip == null)
        {
            _grabSource.clip = _grabClip;
        }
        else
        {
            _grabSource.clip = clip;
        }
        _grabSource.Play();
    }

    private void OnDrawerMove(SelectEnterEventArgs args)
    {
        if (_isDetached)
        {
            PlayGrabSound();
        }
        else
        {
            _drawerSource.Play();
        }
    }

    private void OnDrawerStop(SelectExitEventArgs args)
    {
        _drawerSource.Stop();
    }

    private void OnDrawerSocketed(SelectEnterEventArgs args)
    {
        _drawerSocketSource.Play();
    }

    private void OnWallSocketed(SelectEnterEventArgs args)
    {
        _wallSocketSource.Play();
    }

    private void OnCabinetDoorMove(SimpleHingeInteractable args)
    {
        for (int i = 0; i < _cabinetDoors.Length; i++)
        {
            if (args == _cabinetDoors[i])
            {
                _cabinetDoorSources[i].Play();
            }
        }
    }

    private void OnCabinetDoorStop(SelectExitEventArgs args)
    {
        for (int i = 0; i < _cabinetDoors.Length; i++)
        {
            if (args.interactableObject == _cabinetDoors[i])
            {
                _cabinetDoorSources[i].Stop();
            }
        }
    }

    private void OnComboUnlock()
    {
        _comboLockSource.clip = _unlockClip;
        _comboLockSource.Play();
    }

    private void OnIncorrectCombo()
    {
        _comboLockSource.clip = _incorrectClip;
        _comboLockSource.Play();
    }

    private void OnComboButtonPress()
    {
        _comboLockSource.clip = _comboButtonClip;
        _comboLockSource.Play();
    }

    private void OnDrawerDetach()
    {
        _isDetached = true;
        _drawerSource.Stop();
    }

    private void OnPhysicsButtonEnter()
    {
        PlayGrabSound(_keyClip);
    }

    private void OnPhysicsButtonExit()
    {
        PlayGrabSound(_keyClip);
    }

    private void JoystickMove(SimpleHingeInteractable args)
    {
        _joystickSource.Play();
    }

    private void JoystickExited(SelectExitEventArgs args)
    {
        _joystickSource.Stop();
    }

    private void OnDestroyWallCube()
    {
        _robotSource.Play();
    }

    private void CheckClip(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = _fallBackClip;
        }
    }

    private void SetGrabInteractables()
    {
        _grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        for (int i = 0; i < _grabInteractables.Length; i++)
        {
            _grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrab);
            _grabInteractables[i].selectExited.AddListener(OnSelectExitGrab);
            _grabInteractables[i].activated.AddListener(OnActivatedGrab);
        }
    }

    private void SetProgressControl()
    {
        _progressSource = _progressControl.gameObject.AddComponent<AudioSource>();
        _startGameClip = _progressControl.GetStartGameClip();
        _challengeCompleteClip = _progressControl.GetChallengeClip();
        CheckClip(ref _startGameClip);
        CheckClip(ref _challengeCompleteClip);
    }  

    private void SetDrawerInteractable()
    {
        _drawerSource = _drawer.transform.gameObject.AddComponent<AudioSource>();
        _drawerMoveClip = _drawer.GetDrawerMoveClip();
        CheckClip(ref _drawerMoveClip);
        _drawerSource.clip = _drawerMoveClip;
        _drawerSource.loop = true;

        _drawer.selectEntered.AddListener(OnDrawerMove);
        _drawer.selectExited.AddListener(OnDrawerStop);
        _drawer.OnDrawerDetach.AddListener(OnDrawerDetach);

        _drawerKeySocket = _drawer.GetKeySocket();
        if (_drawerKeySocket != null)
        {
            _drawerSocketSource = _drawerKeySocket.gameObject.AddComponent<AudioSource>();
            _drawerSocketClip = _drawer.GetSocketedClip();
            CheckClip(ref _drawerSocketClip);
            _drawerSocketSource.clip = _drawerSocketClip;
            _drawerKeySocket.selectEntered.AddListener(OnDrawerSocketed);
        }

        _drawerPyhsicsButton = _drawer.GetPhysicsButton();
        if (_drawerPyhsicsButton != null)
        {
            _drawerPyhsicsButton.OnBaseEnter.AddListener(OnPhysicsButtonEnter);
            _drawerPyhsicsButton.OnBaseExit.AddListener(OnPhysicsButtonExit);
        }
    }

    private void SetCabinetDoors(int index)
    {
        _cabinetDoorSources[index] = _cabinetDoors[index].gameObject.AddComponent<AudioSource>();
        _cabinetDoorMoveClip = _cabinetDoors[index].GetHingeMoveClip();
        CheckClip(ref _cabinetDoorMoveClip);
        _cabinetDoorSources[index].clip = _cabinetDoorMoveClip;
        _cabinetDoors[index].OnHingeSelected.AddListener(OnCabinetDoorMove);
        _cabinetDoors[index].selectExited.AddListener(OnCabinetDoorStop);
    }

    private void SetComboLock()
    {
        _comboLockSource = _comboLock.gameObject.AddComponent<AudioSource>();
        _unlockClip = _comboLock.GetUnlockClip();
        _comboButtonClip = _comboLock.GetComboButtonClip();
        _incorrectClip = _comboLock.GetIncorrectClip();
        CheckClip(ref _unlockClip);
        CheckClip(ref _comboButtonClip);
        CheckClip(ref _incorrectClip);

        _comboLock.UnlockAction += OnComboUnlock;
        _comboLock.IncorrectAction += OnIncorrectCombo;
        _comboLock.ComboButtonPressed += OnComboButtonPress;
    }

    private void SetWall()
    {
        _wallSource = _wall.gameObject.AddComponent<AudioSource>();
        _wallExplosionClip = _wall.GetExplodeWallClip();
        CheckClip(ref _wallExplosionClip);
        _wallSource.clip = _wallExplosionClip;
        _wall.OnDestroy.AddListener(OnDestroyWall);
        _wallSocket = _wall.GetSocketInteractor();

        if (_wallSocket != null)
        {
            _wallSocketSource = _wallSocket.gameObject.AddComponent<AudioSource>();
            _wallSocketClip = _wall.GetSocketedClip();
            CheckClip(ref _wallSocketClip);
            _wallSocketSource.clip = _wallSocketClip;
            _wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void SetJoystick()
    {
        _joystickClip = _joystick.GetHingeMoveClip();
        CheckClip(ref _joystickClip);
        _joystickSource = _joystick.gameObject.AddComponent<AudioSource>();
        _joystickSource.clip = _joystickClip;
        _joystickSource.loop = true;
        _joystick.OnHingeSelected.AddListener(JoystickMove);
        _joystick.selectExited.AddListener(JoystickExited);
    }

    private void SetRobot()
    {
        _robotSource = _robot.gameObject.AddComponent<AudioSource>();
        _destroyClip = _robot.GetCollisionClip();
        CheckClip(ref _destroyClip);
        _robotSource.clip = _destroyClip;
        _robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
    }
}
