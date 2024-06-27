using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandReference : MonoBehaviour
{
    public enum HandModelType { Left, Right }

    [Header("Physics Hand Reference")]
    public Transform HandReference;
    public HandModelType HandType;
    [Space(10)]

    [Header ("Toggle Hands Components")]
    [SerializeField] private Material _blueMaterial;
    [SerializeField] private Material _whiteMaterial;
    private HandPresencePhysics _handPresencePhysics;
    private SkinnedMeshRenderer _physicsHandMesh;
    private SkinnedMeshRenderer _handMesh;
    private LayerMask _nonPhysicsLayer = 12;
    private LayerMask _defaultLayer = 0;
    private bool _isPhysics = true;

    private void Start()
    {
        _physicsHandMesh = HandReference.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        _handMesh = transform.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        _handPresencePhysics = HandReference.GetComponent<HandPresencePhysics>();
    }

    public void DisableHandMeshes()
    {
        Debug.Log("DisableHandMeshes");
        _handMesh.enabled = false;
        _physicsHandMesh.enabled = false;
        _handPresencePhysics.ShowNonPhysicalHand = false;
    }

    public void EnableHandMeshes()
    {
        Debug.Log("EnableHandMeshes");
        _handMesh.enabled = true;
        _physicsHandMesh.enabled = true;
        _handPresencePhysics.ShowNonPhysicalHand = true;
    }

    public void SwitchHandToNonPhysics()
    {
        Debug.Log("SwitchHandToNonPhysics");
        if (!_isPhysics) return;

        _handPresencePhysics.ShowNonPhysicalHand = false;
        _handPresencePhysics.DisableHandCollider();
        _physicsHandMesh.enabled = false;

        _handMesh.material = _whiteMaterial;
        _handMesh.gameObject.layer = _defaultLayer;
        _handMesh.enabled = true;

        _isPhysics = false;
    }

    public void SwitchHandToPhysics()
    {
        Debug.Log("SwitchHandToPhysics");
        if (_isPhysics) return;

        _handPresencePhysics.ShowNonPhysicalHand = true;
        _handPresencePhysics.EnableHandCollider();
        _physicsHandMesh.enabled = true;

        _handMesh.material = _blueMaterial;
        _handMesh.gameObject.layer = _nonPhysicsLayer;
        _handMesh.enabled = false;

        _isPhysics = true;
    }
}
