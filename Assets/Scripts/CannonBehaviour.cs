using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CannonBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _metalCylinder;
    [SerializeField] private ParticleSystem _shootEffect;
    [SerializeField] private Transform _cylinderCenter;
    public NavMeshAgent Agent;
    private float _cylinderRadius;
    private Vector3 _initialPosition;
    [Space(10)]

    [Header("Game Settings")]
    [SerializeField] private float _minShootInterval = 5f;
    [SerializeField] private float _maxShootInterval = 15f;   
    private float _shootInterval = 0f;
    private float _shootTimer = 0f;

    [SerializeField] private float _moveInterval = 3f;
    private float _moveTimer = 0f;

    public bool IsRunning = false;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        _cylinderRadius = _cylinderCenter.localScale.x / 2 * 0.9f;
        _initialPosition = transform.position;
    }

    private void Update()
    {
        if (IsRunning)
        {
           _moveTimer += Time.deltaTime;

            if (_moveTimer >= _moveInterval || Agent.remainingDistance < 1f || Agent.velocity.magnitude < 0.1f)
            {
                MoveToRandomPointOnCylinder();
                _moveTimer = 0f;
            }

            RotateTowardsPlayer();
            RotateMetalCylinder();

            _shootTimer += Time.deltaTime;

            if (_shootInterval == 0f)
            {
                _shootInterval = Random.Range(_minShootInterval, _maxShootInterval);
            }

            if (_shootTimer >= _shootInterval)
            {
                _shootTimer = 0f;
                _shootInterval = 0f;
                Shoot();
            } 
        }
    }

    private void MoveToRandomPointOnCylinder()
    {
        Vector2 randomPoint = Random.insideUnitCircle * _cylinderRadius;
        Vector3 finalPosition = new Vector3(randomPoint.x + _cylinderCenter.position.x, _cylinderCenter.position.y, randomPoint.y + _cylinderCenter.position.z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(finalPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            Agent.SetDestination(hit.position);
        }
    }

    private void RotateTowardsPlayer()
    {
        if (_player != null)
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void RotateMetalCylinder()
    {
        if (Agent.velocity.magnitude > 0.1f)
        {
            if (_metalCylinder != null)
            {
                _metalCylinder.Rotate(Vector3.up, Time.deltaTime * 100f);
            }
        }
    }

    private void Shoot()
    {
        _shootEffect.Play();
    }

    public void ResetCannon()
    {
        Agent.isStopped = false;
        Agent.SetDestination(_initialPosition);
        _shootTimer = 0f;
        _moveTimer = 0f;
        IsRunning = false;
    }

}