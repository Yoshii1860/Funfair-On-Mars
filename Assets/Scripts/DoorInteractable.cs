using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : SimpleHingeInteractable
{

    [SerializeField] private Transform _doorObject;
    [SerializeField] private CombinationLock _combinationLock;
    [SerializeField] private Vector3 _rotationLimits;

    private Transform _startTransform;
    private float _startAngleX;

    private void Start()
    {
        if (_combinationLock != null)
        {
            _combinationLock.UnlockAction += Unlock;
        }

        _startTransform = transform;
        _startAngleX = _startTransform.localEulerAngles.x;

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

    private void CheckLimits()
    {
        float localAngleX = transform.localEulerAngles.x;
        if (localAngleX > 180)
        {
            localAngleX -= 360;
        }

        if (localAngleX >= _startAngleX + _rotationLimits.x
        || localAngleX <= _startAngleX - _rotationLimits.x)
        {
            Release();
            transform.localEulerAngles = new Vector3(
                _startAngleX,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z
            );
        }
    }
}
