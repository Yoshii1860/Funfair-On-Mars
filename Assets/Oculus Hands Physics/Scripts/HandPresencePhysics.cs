using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{
    [SerializeField] private Transform _target;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rb.velocity = (_target.position - transform.position) * Time.fixedDeltaTime;

        Quaternion rotationDifferece = _target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifferece.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        _rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad * Time.fixedDeltaTime;
    }
}
