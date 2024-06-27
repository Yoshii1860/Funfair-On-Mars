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
    [SerializeField] private bool _isClosed = true;
    private bool _isTransforming = false;

    public UnityEvent OnDestroyWallCube;
    public AudioClip GetCollisionClip() => _collisionClip;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void MoveAgent(Vector3 destination)
    {
        _animator.SetBool(_isClosed ? "Roll_Anim" : "Walk_Anim", true);
        _agent.destination = _agent.transform.position + destination;
    }

    public void StopAgent()
    {
        _animator.SetBool(_isClosed ? "Roll_Anim" : "Walk_Anim", false);
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

    public void TransformRobot()
    {
        if (!_isTransforming)
        {
            _animator.SetBool("Open_Anim", _isClosed);
            _isClosed = !_isClosed;
            StartCoroutine(TransformTimer());
        }
    }

    private IEnumerator TransformTimer()
    {
        _isTransforming = true;
        yield return new WaitForSeconds(3f);
        _isTransforming = false;
    }
}
