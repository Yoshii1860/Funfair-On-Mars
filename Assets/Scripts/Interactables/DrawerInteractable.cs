using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    #region Variables

    [Space(10)]
    [Header("Drawer Settings")]
    [SerializeField] private Transform _drawerTransform;
    [SerializeField] private Vector3 _drawerLimitDistance = new Vector3(0.02f, 0.02f, 0f);
    [SerializeField] private float _drawerLimitZ = 0.92f;
    [SerializeField] private XRSocketInteractor _keySocket;
    [SerializeField] private GameObject _keyLight;
    [SerializeField] private bool _isLoacked = true;
    [Space(10)]
    [SerializeField] private AudioClip _drawerMoveClip;
    [SerializeField] private AudioClip _socketedClip;

    private Transform _parentTransform;
    private Vector3 _limitPosition;

    private const string DEFAULT_LAYER = "Default";
    private const string GRAB_LAYER = "Grab";

    private bool _isGrabbed = false;

    public XRSocketInteractor GetKeySocket() => _keySocket;
    public AudioClip GetDrawerMoveClip() => _drawerMoveClip;
    public AudioClip GetSocketedClip() => _socketedClip;

    #endregion




    #region Unity Methods

    void Start()
    {
        if (_keySocket != null)
        {
            _keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            _keySocket.selectExited.AddListener(OnDrawerLocked);
        }

        _parentTransform = transform.parent.transform;
        _limitPosition = _drawerTransform.localPosition;
    }

    private void Update()
    {
        if (_isGrabbed && _drawerTransform != null)
        {
            _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, transform.localPosition.z);

            CheckLimits();
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

        ChangeLayerMask(GRAB_LAYER);
        _isGrabbed = false;
        transform.localPosition = _drawerTransform.localPosition;
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
            _isGrabbed = false;
            _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, _drawerLimitZ);
            ChangeLayerMask(DEFAULT_LAYER);
        }
    }

    private void ChangeLayerMask(string layerName)
    {
        interactionLayers = InteractionLayerMask.GetMask(layerName);
    }

    #endregion
}
