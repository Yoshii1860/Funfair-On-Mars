using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AlienState
{
    Idle,
    Walking,
    Running,
    Wave,
    Shout,
    Talk
}

public class AlienBehavior : MonoBehaviour
{
    public AlienState currentState;
    public Waypoint[] waypoints;
    public Waypoint currentWaypoint;
    private Waypoint lastWaypoint;
    private bool actionCompleted = false;

    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float _minIdleTime = 2f;
    public float _maxIdleTime = 5f;
    public float _averageTalkTime;
    private float _waveTimer = 3f;
    private float _shoutTimer = 5f;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private Coroutine currentCoroutine;

    public void InitializeAlien(Waypoint waypoint)
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        waypoints = FindObjectsOfType<Waypoint>();

        currentWaypoint = waypoint;
        currentState = AlienState.Idle;
        StartCoroutine(StateMachine());
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case AlienState.Idle:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Idle());
                    break;
                case AlienState.Walking:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Walk());
                    break;
                case AlienState.Running:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Run());
                    break;
                case AlienState.Wave:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Wave());
                    break;
                case AlienState.Shout:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Shout());
                    break;
                case AlienState.Talk:
                    if (currentCoroutine == null)
                        currentCoroutine = StartCoroutine(Talk());
                    break;
            }

            yield return null;
        }
    }

    private IEnumerator Idle()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);

        float idleTime = Random.Range(_minIdleTime, _maxIdleTime);

        yield return new WaitForSeconds(idleTime);

        currentCoroutine = null;
        ChooseNextState();
    }

    private IEnumerator Walk()
    {

        SetNextWaypoint();

        _animator.SetBool("Walk", true);
        _animator.SetBool("Run", false);

        MoveTowards(currentWaypoint.transform.position, walkSpeed);
        
        yield return new WaitUntil(() => Vector3.Distance(transform.position, currentWaypoint.transform.position) < 1f);

        currentCoroutine = null;
        ChooseNextState();
    }

    private IEnumerator Run()
    {
        SetNextWaypoint();

        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", true);

        MoveTowards(currentWaypoint.transform.position, runSpeed);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, currentWaypoint.transform.position) < 1f);

        currentCoroutine = null;
        ChooseNextState();
    }

    private IEnumerator Wave()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);
        _animator.SetTrigger("Wave");

        yield return new WaitForSeconds(_waveTimer);

        currentCoroutine = null;
        ChooseNextState();
    }

    private IEnumerator Shout()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);
        _animator.SetTrigger("Shout");

        yield return new WaitForSeconds(_shoutTimer);

        currentCoroutine = null;
        ChooseNextState();
    }

    private IEnumerator Talk()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);
        _animator.SetTrigger("Talk");

        float talkTime = Random.Range(_averageTalkTime * 0.8f, _averageTalkTime * 1.2f);

        yield return new WaitForSeconds(talkTime);

        _animator.SetTrigger("Continue");

        currentCoroutine = null;
        ChooseNextState();
    }

    private void ChooseNextState()
    {
        if (currentWaypoint.HasAction && !actionCompleted)
        {
            actionCompleted = true;
            ChooseActionState();
        }
        else
        {
            actionCompleted = false;
            ChooseMovementState();
        }
    }

    private void ChooseActionState()
    {
        if (currentWaypoint.AlienActionsArray.Length > 0)
        {
            int randomAction = Random.Range(0, currentWaypoint.AlienActionsArray.Length);
            currentState = currentWaypoint.AlienActionsArray[randomAction];
        }
        else
        {
            ChooseMovementState();
        }
    }

    private void ChooseMovementState()
    {
        int randomState = Random.Range(0, 100);
        if (randomState > 20)
        {
            currentState = AlienState.Walking;
        }
        else
        {
            currentState = AlienState.Running;
        }
    }

    private void SetNextWaypoint()
    {
        int randomIndex = Random.Range(0, currentWaypoint.NextWaypoints.Count);

        if (currentWaypoint.NextWaypoints.Count > 1 && currentWaypoint.NextWaypoints[randomIndex] == lastWaypoint)
        {
            Waypoint nextWaypoint = lastWaypoint;

            while (nextWaypoint == lastWaypoint)
            {
                randomIndex = Random.Range(0, currentWaypoint.NextWaypoints.Count);
                nextWaypoint = currentWaypoint.NextWaypoints[randomIndex];
            }

            lastWaypoint = currentWaypoint;
            currentWaypoint = nextWaypoint;
        }
        else
        {
            lastWaypoint = currentWaypoint;
            currentWaypoint = currentWaypoint.NextWaypoints[randomIndex];
        }
    }

    private void MoveTowards(Vector3 targetPosition, float speed)
    {
        _navMeshAgent.speed = speed;
        _navMeshAgent.SetDestination(targetPosition);
    }

/*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alien"))
        {
            AlienBehavior otherAlien = other.GetComponent<AlienBehavior>();
            if (otherAlien != null)
            {
                Vector3 avoidanceDirection = (transform.position - other.transform.position).normalized;
                Vector3 newTarget = transform.position + avoidanceDirection * 1f; // Adjust the distance as needed
                navMeshAgent.SetDestination(newTarget);
            }
        }
    }
*/
}
