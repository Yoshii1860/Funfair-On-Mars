using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : SimpleHingeInteractable
{
    public UnityEvent OnOpen;

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
        _startAngleX = GetAngle(_startRotation.x);

        _endRotation = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + _rotationLimits.y,
            transform.localEulerAngles.z
        );
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

    protected override void ResetHinge()
    {
        if (_isClosed)
        {
            transform.localEulerAngles = _startRotation;
        }
        else if (_isOpen)
        {
            transform.localEulerAngles = _endRotation;
            OnOpen?.Invoke();
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



    private void CheckLimits()
    {
        _isClosed = false;
        _isOpen = false;

        float localAngleX = GetAngle(transform.localEulerAngles.x);

        if (localAngleX >= _startAngleX + _rotationLimits.x
        || localAngleX <= _startAngleX - _rotationLimits.x)
        {
            Release();
        }
    }

    private float GetAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }
}
