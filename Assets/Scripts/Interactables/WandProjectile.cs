using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WandProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 200f;
    [SerializeField] private float _lifetime = 5f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * _speed);
        Destroy(gameObject, _lifetime);
    }
}
