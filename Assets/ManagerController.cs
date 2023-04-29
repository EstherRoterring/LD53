using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor.UI;
using Random = UnityEngine.Random;

public class ManagerController : MonoBehaviour
{
    public float walkSpeed;
    public OfficeController office;

    private Path followPath = null;
    private int currentWaypoint = 0;
    public float nextWaypointDistance;

    private void Start()
    {
        Seeker seeker = GetComponent<Seeker>();
        seeker.StartPath(transform.position, office.player.transform.position, OnPathComplete);
    }

    void Update()
    {
        transform.localPosition += Time.deltaTime * walkSpeed * (Vector3) Random.insideUnitCircle;

        if (followPath != null)
        {
            bool reachedEndOfPath = false;
            float distanceToWaypoint;
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, followPath.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    if (currentWaypoint + 1 < followPath.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            Vector3 dir = (followPath.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = walkSpeed * speedFactor * dir;

            transform.position += velocity * Time.deltaTime;

            if (reachedEndOfPath)
            {
                followPath = null;
                currentWaypoint = 0;
            }
        }

        if (followPath == null)
        {
            Seeker seeker = GetComponent<Seeker>();
            seeker.StartPath(transform.position, office.player.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        followPath = p;
        currentWaypoint = 0;
    }
}
