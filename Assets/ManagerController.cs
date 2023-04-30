using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<DoorController> visitDoors = new List<DoorController>();
    
    IState currentState;
    private Animator anim;


    void Start()
    {
        anim=GetComponent<Animator>();
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }

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
            
            Vector3 dir = (followPath.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = walkSpeed * dir;
            UpdateAnimationDirection(velocity);

            transform.position += velocity * Time.deltaTime;

            if (reachedEndOfPath)
            {
                followPath = null;
                currentWaypoint = 0;
            }
        }
        if (followPath == null) {
            if (visitDoors.Count > 0)
            {
                var visitDoor = visitDoors[0];
                visitDoors.RemoveAt(0);
                Seeker seeker = GetComponent<Seeker>();
                seeker.StartPath(transform.position, visitDoor.transform.position, SetFollowPath);
            }
        }
    }

    public void UpdateAnimationDirection(Vector3 velocity)
    {
        if (velocity.magnitude <= 0.01f)
        {
            anim.SetBool("standingStill", true);
            anim.SetBool("moveTowards",false);
            anim.SetBool("moveRight",false);
            anim.SetBool("moveLeft",false);
            anim.SetBool("moveAway",false);
        } else
        {
            anim.SetBool("standingStill", false);
            var xMax = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y);
            anim.SetBool("moveTowards",!xMax && velocity.y <= 0);
            anim.SetBool("moveAway",!xMax && velocity.y >= 0);
            anim.SetBool("moveLeft",xMax && velocity.x <= 0);
            anim.SetBool("moveRight",xMax && velocity.x >= 0);
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
            ChangeState(new ChasePlayerState(5f));
        }
    }

    public void GoToRoom(RoomController newRoom)
    {
        // todo, implement search
        visitDoors.Add(newRoom.doors.Keys.First());
    }
    
    public void OnCollisionStay2D(Collision2D other)
    {
        RoomController room = null;
        other.gameObject.TryGetComponent<RoomController>(out room);
        if (room != null)
        {
            this.room = room;
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

    public ChasePlayerState(float totalTime)
    {
        timeLeft = totalTime;
    }

    public void OnEnter(ManagerController manager)
    {
        manager.GoToRoom(manager.office.player.room);
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
        manager.GoToRoom(targetRoom);
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