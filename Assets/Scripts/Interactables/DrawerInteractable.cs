using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class DrawerInteractable : XRGrabInteractable
{
    #region Variables

    [Space(10)]
    [Header("Drawer Settings")]
    [SerializeField] private Transform _drawerTransform;
    [SerializeField] private Vector3 _drawerLimitDistance = new Vector3(0.02f, 0.02f, 0f);
    [SerializeField] private float _drawerLimitZ = 0.92f;
    [SerializeField] private XRSocketInteractor _keySocket;
    [SerializeField] private XRPhysicsButtonInteractable _physicsButton;
    [SerializeField] private GameObject _keyLight;
    [SerializeField] private bool _isLoacked = true;
    [SerializeField] private bool _isDetachable = false;
    [SerializeField] private bool _isDetached = false;
    [Space(10)]
    [SerializeField] private AudioClip _drawerMoveClip;
    [SerializeField] private AudioClip _socketedClip;

    private Transform _parentTransform;
    private Vector3 _limitPosition;
    private Rigidbody _rb;

    private const string DEFAULT_LAYER = "Default";
    private const string GRAB_LAYER = "Grab";

    private bool _isGrabbed = false;

    public XRSocketInteractor GetKeySocket() => _keySocket;
    public XRPhysicsButtonInteractable GetPhysicsButton() => _physicsButton;
    public AudioClip GetDrawerMoveClip() => _drawerMoveClip;
    public AudioClip GetSocketedClip() => _socketedClip;

    public UnityEvent OnDrawerDetach;

    #endregion




    #region Unity Methods

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_keySocket != null)
        {
            _keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            _keySocket.selectExited.AddListener(OnDrawerLocked);
        }

        _parentTransform = transform.parent.transform;
        _limitPosition = _drawerTransform.localPosition;

        if (_physicsButton != null)
        {
            _physicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            _physicsButton.OnBaseExit.AddListener(OnIsNotDetachable);
        }
    }

    private void Update()
    {
        if (!_isDetached)
        {
            if (_isGrabbed && _drawerTransform != null)
            {
                _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, transform.localPosition.z);

                CheckLimits();
            }
        }
    }

    #endregion




    #region XRGrabInteractable

    private void OnDrawerUnlocked(SelectEnterEventArgs args)
    {
        _isLoacked = false;
        _keyLight.SetActive(false);
        Debug.Log("Drawer Unlocked!");
    }

    private void OnDrawerLocked(SelectExitEventArgs args)
    {
        _isLoacked = true;
        Debug.Log("Drawer Locked!");
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        
        if (!_isLoacked)
        {
            transform.SetParent(_parentTransform);

            _isGrabbed = true;
        }
        else
        {
            ChangeLayerMask(DEFAULT_LAYER);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    
        if (!_isDetached)
        {
            ChangeLayerMask(GRAB_LAYER);
            _isGrabbed = false;
            transform.localPosition = _drawerTransform.localPosition;
        }
        else
        {
            _rb.isKinematic = false;
        }
    }

    #endregion




    #region Detached Events

    private void OnIsDetachable()
    {
        _isDetachable = true;
    }

    private void OnIsNotDetachable()
    {
        _isDetachable = false;
    }

    #endregion




    #region Helpers

    private void CheckLimits()
    {
        if (transform.localPosition.x > _limitPosition.x + _drawerLimitDistance.x
            || transform.localPosition.x < _limitPosition.x - _drawerLimitDistance.x)
        {
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (transform.localPosition.y > _limitPosition.y + _drawerLimitDistance.y
            || transform.localPosition.y < _limitPosition.y - _drawerLimitDistance.y)
        {
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (transform.localPosition.z <= _limitPosition.z - _drawerLimitDistance.z)
        {
            _isGrabbed = false;
            _drawerTransform.localPosition = _limitPosition;
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (transform.localPosition.z >= _drawerLimitZ + _drawerLimitDistance.z)
        {
            if (!_isDetachable)
            {
                _isGrabbed = false;
                _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, _drawerLimitZ);
                ChangeLayerMask(DEFAULT_LAYER);
            }
            else
            {
                DetachDrawer();
            }
        }
    }

    private void DetachDrawer()
    {
        Debug.Log("Drawer Detached!");
        _isDetached = true;
        _drawerTransform.SetParent(this.transform);
        OnDrawerDetach?.Invoke();
    }

    private void ChangeLayerMask(string layerName)
    {
        interactionLayers = InteractionLayerMask.GetMask(layerName);
    }

    #endregion
}
