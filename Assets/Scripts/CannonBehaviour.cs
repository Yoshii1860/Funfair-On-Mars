using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CannonBehaviour : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public Transform metalCylinder;
    public ParticleSystem shootEffect;
    private float timer = 0f;
    private float shootTimer = 0f;
    private float moveInterval = 3f; // Time in seconds between movements
    private float minShootInterval = 5f;
    private float maxShootInterval = 20f;   
    private float shootInterval = 0f;
    public Transform cylinderCenter;
    private float cylinderRadius;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cylinderRadius = cylinderCenter.localScale.x / 2 * 0.9f;
        MoveToRandomPointOnCylinder();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= moveInterval || agent.remainingDistance < 1f || agent.velocity.magnitude < 0.1f)
        {
            MoveToRandomPointOnCylinder();
            timer = 0f;
        }

        RotateTowardsPlayer();
        RotateMetalCylinder();

        shootTimer += Time.deltaTime;

        if (shootInterval == 0f)
        {
            shootInterval = Random.Range(minShootInterval, maxShootInterval);
        }

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            shootInterval = 0f;
            Shoot();
        }
    }

    void MoveToRandomPointOnCylinder()
    {
        Vector2 randomPoint = Random.insideUnitCircle * cylinderRadius;
        Vector3 finalPosition = new Vector3(randomPoint.x + cylinderCenter.position.x, cylinderCenter.position.y, randomPoint.y + cylinderCenter.position.z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(finalPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void RotateTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void RotateMetalCylinder()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            if (metalCylinder != null)
            {
                metalCylinder.Rotate(Vector3.up, Time.deltaTime * 100f);
            }
        }
    }

    void Shoot()
    {
        shootEffect.Play();
    }
}