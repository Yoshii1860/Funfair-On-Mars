using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint : MonoBehaviour
{
    public List<Waypoint> NextWaypoints;
    public AlienState[] AlienActionsArray;
    public bool HasAction = false;

    private void Awake()
    {
        if (AlienActionsArray.Length > 0)
        {
            HasAction = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        foreach (var waypoint in NextWaypoints)
        {
            if (waypoint != null)
            {
                bool bothConnected = waypoint.NextWaypoints.Contains(this);

                if (bothConnected)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(transform.position, waypoint.transform.position);
            }
        }
    }
}
