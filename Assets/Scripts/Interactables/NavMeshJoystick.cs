using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NavMeshJoystick : SimpleHingeInteractable
{
    [SerializeField] private NavMeshRobot _robot;
    [SerializeField] private Transform _rotationParentObject;

    [SerializeField] private Transform _trackedObject;
    [SerializeField] private Transform _trackingObject;
    [SerializeField] private float _resetSpeed = 5f;

    protected override void Update()
    {
        base.Update();
        if (isSelected && _trackingObject != null)
        {
            MoveRobot();
        }
    }

    protected override void ResetHinge()
    {
        Debug.Log("Resetting Hinge");
        if (_robot != null)
        {
            _robot.StopAgent();
        }
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
}
