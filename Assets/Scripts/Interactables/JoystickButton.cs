using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class JoystickButton : MonoBehaviour
{
    [SerializeField] private Transform _visualTarget;
    [SerializeField] private Vector3 _localAxis;
    [SerializeField] private float _resetSpeed = 5.0f;

    private float _followAngle = 45f; 

    private Vector3 _initialPosition;

    private Vector3 _offset;
    private Transform _pokeAttachTransform;

    private XRBaseInteractable _interactable;

    private bool _isFollowing = false;
    private bool _isFrozen = false;

    private void Start()
    {
        _interactable = GetComponent<XRBaseInteractable>();
        _initialPosition = _visualTarget.localPosition;
        _interactable.hoverEntered.AddListener(Follow);
        _interactable.hoverExited.AddListener(Reset);
        _interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactor is XRPokeInteractor)
        {
            XRPokeInteractor pokeInteractor = (XRPokeInteractor)hover.interactor;

            _pokeAttachTransform = pokeInteractor.attachTransform;
            _offset = transform.position - _pokeAttachTransform.position;

            float pokeAngle = Vector3.Angle(_offset, _visualTarget.TransformDirection(_localAxis));

            if (pokeAngle < _followAngle)
            {
                _isFollowing = true;
                _isFrozen = false;
            }
        }
    }

    private void Update()
    {
        if (_isFrozen)
        {
            return;
        }

        if (_isFollowing)
        {
            Vector3 localTargetPosition = _visualTarget.InverseTransformPoint(_pokeAttachTransform.position + _offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, _localAxis);

            _visualTarget.position = _visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
        else
        {
            _visualTarget.localPosition = Vector3.Lerp(_visualTarget.localPosition, _initialPosition, Time.deltaTime * _resetSpeed);
        }
    }

    private void Reset(BaseInteractionEventArgs hover)
    {
        if (hover.interactor is XRPokeInteractor)
        {
            _isFollowing = false;
            _isFrozen = false;
        }
    }

    private void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactor is XRPokeInteractor)
        {
            _isFrozen = true;
        }
    }
}
