using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor.UI;
using Random = UnityEngine.Random;

/**
 * states:
 *
 * - intro sequence: * spieler bisschen zeit geben damit er nicht gleich gefangen wird. dann chase mode
 *                   * kontext erklären
 *
 * [loop]
 * - manager sucht spieler: geht direkt zu deinem büro, findet dich nicht
 *
 * - manager geht auf die jagdt
 *
 * - manager arbeitet in eigenem büro:  kriegt erst anruf, geht dann zu eigenem büro, 5 sek idle
 *   ggf. auslösbar mittels anruf durch spieler
 * - manager trinkt kaffee, 5 sek idle
 *
 * - manager schlendert durch die gänge, bewegt sich langsamer oder eiert so rum
 * [/loop]
 *
 * spieler kann aktiv beeinflussen:
 *  - prank call
 *  - kaffee kochen
 *
 * andere sachen die manager beeinflussen:
 *  - toni
 *
 * - outro sequence: gewonnen oder nicht
 */

public class ManagerController : MonoBehaviour
{
    public float walkSpeed;
    public OfficeController office;
    public RoomController room;

    public Path followPath = null;
    private int currentWaypoint = 0;
    public float nextWaypointDistance;
    
    IState currentState;

    private void Start()
    {
        ChangeState(new ChasePlayerState(5f, office.player.transform.position));
    }

    void Update()
    {
        currentState.UpdateState(this);

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
    }

    public void SetFollowPath(Path p)
    {
        followPath = p;
        currentWaypoint = 0;
    }
    
    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void FindNewState()
    {
        if (currentState is ChasePlayerState)
        {
            ChangeState(new ReturnToOfficeState(5f, room));
        }
        else
        {
            ChangeState(new ChasePlayerState(5f, office.player.transform.position));
        }
    }
}

public interface IState
{
    public void OnEnter(ManagerController manager);
    public void UpdateState(ManagerController manager);
    public void OnExit(ManagerController manager);
}

public class ChasePlayerState : IState
{
    private float timeLeft;
    private Vector3 targetPosition;

    public ChasePlayerState(float totalTime, Vector3 targetPosition)
    {
        timeLeft = totalTime;
        this.targetPosition = targetPosition;
    }

    public void OnEnter(ManagerController manager)
    {
        Seeker seeker = manager.GetComponent<Seeker>();
        seeker.StartPath(manager.transform.position, targetPosition, manager.SetFollowPath);
    }

    public void UpdateState(ManagerController manager)
    {
        bool arrived = manager.followPath == null;
        if (arrived)
        {
            timeLeft -= Time.deltaTime;
        }

        if (timeLeft <= 0)
        {
            manager.FindNewState();
        }
    }

    public void OnExit(ManagerController manager)
    {
        
    }
}


public class ReturnToOfficeState : IState
{
    private float timeLeft;
    private RoomController targetRoom;

    public ReturnToOfficeState(float totalTime, RoomController room)
    {
        timeLeft = totalTime;
        this.targetRoom = room;
    }

    public void OnEnter(ManagerController manager)
    {
        Seeker seeker = manager.GetComponent<Seeker>();
        seeker.StartPath(manager.transform.position, targetRoom.transform.position, manager.SetFollowPath);
    }

    public void UpdateState(ManagerController manager)
    {
        bool arrived = manager.followPath == null;
        if (arrived)
        {
            timeLeft -= Time.deltaTime;
        }

        if (timeLeft <= 0)
        {
            manager.FindNewState();
        }
    }

    public void OnExit(ManagerController manager)
    {
        
    }
}