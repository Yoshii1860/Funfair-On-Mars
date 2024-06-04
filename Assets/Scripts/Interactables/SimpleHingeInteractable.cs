using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHingeInteractable : XRSimpleInteractable
{
    public UnityEvent<SimpleHingeInteractable> OnHingeSelected;

    [SerializeField] private Vector3 _positionLimits;
    [SerializeField] private bool _isLocked = true;

    [SerializeField] private AudioClip _hingeMoveClip;

    private Transform _grabHand;
    private Collider _hingeCollider;
    private Vector3 _hingePosition;

    private const string DEFAULT_LAYER = "Default";
    private const string GRAB_LAYER = "Grab"; 

    public AudioClip GetHingeMoveClip() => _hingeMoveClip;

    protected virtual void Start()
    {
        _hingeCollider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        if (_grabHand != null)
        {
            TrackHand();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!_isLocked)
        {
            base.OnSelectEntered(args);
            _grabHand = args.interactorObject.transform;
            OnHingeSelected?.Invoke(this);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _grabHand = null;
        ChangeLayerMask(GRAB_LAYER);
        ResetHinge();
    }

    private void TrackHand()
    {
        transform.LookAt(_grabHand, transform.forward);
        _hingePosition = _hingeCollider.bounds.center;

        if (_grabHand.position.z >= _hingePosition.z + _positionLimits.z
            || _grabHand.position.z <= _hingePosition.z - _positionLimits.z
            || _grabHand.position.y >= _hingePosition.y + _positionLimits.y
            || _grabHand.position.y <= _hingePosition.y - _positionLimits.y
            || _grabHand.position.x >= _hingePosition.x + _positionLimits.x
            || _grabHand.position.x <= _hingePosition.x - _positionLimits.x)
        {
            Release();
        }
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    public void Release()
    {
        ChangeLayerMask(DEFAULT_LAYER);
    }

    protected abstract void ResetHinge();

    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
