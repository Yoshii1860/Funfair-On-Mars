using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NavMeshJoystick : SimpleHingeInteractable
{
    [SerializeField] private NavMeshRobot _robot;
    [SerializeField] private Transform _rotationParentObject;

    private GameObject _rightHandPhysics;
    private GameObject _leftHandPhysics;

    [SerializeField] private Transform _trackedObject;
    [SerializeField] private Transform _trackingObject;

    [SerializeField] private float _maxRotation = 350f;
    [SerializeField] private float _minRotation = 190f;
    private Vector3 _resetJoystickRotation;
    private Vector3 _resetCylinderRotation;

    private Transform _handInteractor;
    private Transform _saveAttachPoint;

    protected override void Start()
    {
        base.Start();
        _resetJoystickRotation = transform.localEulerAngles;
        _rightHandPhysics = GameObject.FindWithTag("RightHand");
        _leftHandPhysics = GameObject.FindWithTag("LeftHand");
    }

    protected override void Update()
    {
        base.Update();

        if (isSelected && _trackingObject != null && !IsLocked) 
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);

            LimitRotations();

            MoveRobot();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _handInteractor = args.interactorObject.transform;
    }

    protected override void ResetHinge()
    {
        if (_robot != null)
        {
            _robot.StopAgent();
        }

        transform.localEulerAngles = _resetJoystickRotation;
    }

    private void MoveRobot()
    {
        if (_robot != null)
        {
            _trackingObject.position = new Vector3(_trackedObject.position.x, _trackingObject.position.y, _trackedObject.position.z);
            _rotationParentObject.rotation = Quaternion.identity;

            _robot.MoveAgent(_trackingObject.localPosition);
        }
    }

    private void LimitRotations()
    {
        float currentXRotation = transform.localEulerAngles.x;

        if (currentXRotation <= _minRotation || currentXRotation >= _maxRotation)
        {
            Release();
        }
    }
}
