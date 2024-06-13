using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshRobot : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    [SerializeField] private AudioClip _collisionClip;

    public UnityEvent OnDestroyWallCube;
    public AudioClip GetCollisionClip() => _collisionClip;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void MoveAgent(Vector3 destination)
    {
        _animator.SetBool("Walk", true);
        _agent.destination = _agent.transform.position + destination;
    }

    public void StopAgent()
    {
        _animator.SetBool("Walk", false);
        _agent.ResetPath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("WallCube"))
        {
            Destroy(other.gameObject);
            OnDestroyWallCube?.Invoke();
        }
    }
}
