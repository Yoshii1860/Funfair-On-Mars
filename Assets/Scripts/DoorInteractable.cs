using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : SimpleHingeInteractable
{

    [SerializeField] private Transform _doorObject;
    [SerializeField] private CombinationLock _combinationLock;
    [SerializeField] private Vector3 _rotationLimits;
    [SerializeField] private Collider _closedCollider;
    [SerializeField] private Collider _openCollider;

    private Vector3 _startRotation;
    private Vector3 _endRotation;

    private float _startAngleX;

    private bool _isClosed = true;
    private bool _isOpen = false;

    protected override void Start()
    {
        base.Start();

        if (_combinationLock != null)
        {
            _combinationLock.UnlockAction += Unlock;
        }

        _startRotation = transform.localEulerAngles;
        _startAngleX = _startRotation.x;

        _endRotation = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + _rotationLimits.y,
            transform.localEulerAngles.z
        );

        if (_startAngleX >= 180)
        {
            _startAngleX -= 360;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_doorObject != null)
        {
            _doorObject.localEulerAngles = new Vector3(
                _doorObject.localEulerAngles.x,
                transform.localEulerAngles.y,
                _doorObject.localEulerAngles.z
            );
        }

        if (isSelected)
        {
            CheckLimits();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == _closedCollider)
        {
            _isClosed = true;
            Release();
        }
        else if (other == _openCollider)
        {
            _isOpen = true;
            Release();
        }
    }

    private void CheckLimits()
    {
        _isClosed = false;
        _isOpen = false;

        float localAngleX = transform.localEulerAngles.x;
        if (localAngleX > 180)
        {
            localAngleX -= 360;
        }

        if (localAngleX >= _startAngleX + _rotationLimits.x
        || localAngleX <= _startAngleX - _rotationLimits.x)
        {
            Release();
        }
    }

    protected override void ResetHinge()
    {
        if (_isClosed)
        {
            transform.localEulerAngles = _startRotation;
        }
        else if (_isOpen)
        {
            transform.localEulerAngles = _endRotation;
            _isOpen = false;
        }
        else
        {
            transform.localEulerAngles = new Vector3
            (
                _startAngleX,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z
            );
        }
    }
}
