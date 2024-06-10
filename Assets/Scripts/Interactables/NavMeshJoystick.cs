using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshJoystick : SimpleHingeInteractable
{
    [SerializeField] private Transform _trackedObject;
    [SerializeField] private Transform _trackingObject;

    protected override void Update()
    {
        base.Update();
        _trackingObject.position = new Vector3(_trackedObject.position.x, _trackingObject.position.y, _trackedObject.position.z);
    }

    protected override void ResetHinge()
    {

    }
}
