using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [SerializeField] private Transform _playerHead;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    [SerializeField] private CapsuleCollider _bodyCollider;
    [SerializeField] private ConfigurableJoint _headJoint;
    [SerializeField] private ConfigurableJoint _leftHandJoint;
    [SerializeField] private ConfigurableJoint _rightHandJoint;

    [SerializeField] private float _bodyHeightMin = 0.5f;
    [SerializeField] private float _bodyHeightMax = 2.0f;

    [SerializeField] private CharacterController _characterController;

    private void FixedUpdate()
    {
        _bodyCollider.height = Mathf.Clamp(_playerHead.localPosition.y, _bodyHeightMin, _bodyHeightMax);
        _bodyCollider.center = new Vector3(_playerHead.localPosition.x, _bodyCollider.height / 2, _playerHead.localPosition.z);

        _characterController.height = _bodyCollider.height;
        _characterController.center = _bodyCollider.center;

        _leftHandJoint.targetPosition = _leftHand.localPosition;
        _leftHandJoint.targetRotation = _leftHand.localRotation;

        _rightHandJoint.targetPosition = _rightHand.localPosition;
        _rightHandJoint.targetRotation = _rightHand.localRotation;

        _headJoint.targetPosition = _playerHead.localPosition;
        _headJoint.targetRotation = _playerHead.localRotation;
    }
}
