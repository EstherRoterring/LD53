using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEditor.UIElements;
using Random = UnityEngine.Random;
using UnityEngine.UI;


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
    public float viewDistance;
    public OfficeController office;
    public RoomController room;

    public Path followPath = null;
    private int currentWaypoint = 0;
    public float nextWaypointDistance;

    public List<Transform> visitLocations = new List<Transform>();
    
    IManagerState _currentManagerState;
    private Animator anim;
    public bool angry = false;

    public float totalManagerTime = 0;
    public float dangerLevel = 0;

    public GameObject dangerHighlight;
    public bool showHighlight = true;
    public GameObject allTasksDoneExclamationMark;
    public bool showExclamationMark = true;
    public Color goodDangerHighlightColor;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        dangerHighlight.SetActive(showHighlight);
        dangerHighlight.transform.localScale = new Vector3(viewDistance / 4f, viewDistance / 4f, 1);
        
        allTasksDoneExclamationMark.SetActive(showExclamationMark);

        if (office.HasFreeControlflow())
        {
            totalManagerTime += Time.deltaTime;

            if (office.player.didAnyTask)
            {
                dangerLevel += Time.deltaTime;
            }

            viewDistance = 1f + (0.5f / 60f * dangerLevel);
            
            if (_currentManagerState != null)
            {
                // override tasks
                if (office.coffee.activeSelf && room == office.coffeeRoom)
                {
                    ChangeState(new DrinkCoffeeManagerState(10f));
                }
                else if (office.ringingPhone.activeSelf && !(_currentManagerState is AnswerCallInOfficeManagerState))
                {
                    ChangeState(new AnswerCallInOfficeManagerState(3f));
                }
                _currentManagerState.UpdateState(this);
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
                if (angry)
                {
                    velocity *= 1.2f;
                }
                UpdateAnimationDirection(velocity);

                transform.position += velocity * Time.deltaTime;

                if (reachedEndOfPath)
                {
                    followPath = null;
                    currentWaypoint = 0;
                }
            }
            else
            {
                UpdateAnimationDirection(Vector3.zero);
            }
            if (followPath == null) {
                if (visitLocations.Count > 0)
                {
                    var visitDoor = visitLocations[0];
                    visitLocations.RemoveAt(0);
                    Seeker seeker = GetComponent<Seeker>();
                    seeker.StartPath(transform.position, visitDoor.position, SetFollowPath);
                }
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

        if (angry==true){anim.SetBool("angry",true);}
        else{anim.SetBool("angry",false);}
    }

    public void SetFollowPath(Path p)
    {
        followPath = p;
        currentWaypoint = 0;
    }
    
    public void ChangeState(IManagerState newManagerState)
    {
        if (_currentManagerState != null)
        {
            _currentManagerState.OnExit(this);
        }
        _currentManagerState = newManagerState;
        _currentManagerState.OnEnter(this);
    }

    public void FindNewState()
    {
        List<IManagerState> nextStates = new List<IManagerState>();
        nextStates.Add(new ChasePlayerManagerState(5f));
        nextStates.Add(new ChasePlayerManagerState(5f));
        if (dangerLevel < 60)
        {
            nextStates.Add(new ReturnToOfficeManagerState(5f, office.managerRoom));
        }
        if (dangerLevel > 60)
        {
            nextStates.Add(new RageFollowPlayerManagerState(10f, 4f));
        }

        ChangeState(nextStates[Random.Range(0, nextStates.Count)]);
    }

    public void GoToRoom(RoomController newRoom)
    {
        // todo, implement search
        visitLocations.Add(newRoom.roomCenter);
    }

    public void GoTo(Transform transform)
    {
        visitLocations.Add(transform);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerStay2D(other);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        RoomController nextRoom = null;
        other.gameObject.TryGetComponent<RoomController>(out nextRoom);
        if (nextRoom != null)
        {
            room = nextRoom;
        }
    }
}

public interface IManagerState
{
    public void OnEnter(ManagerController manager);
    public void UpdateState(ManagerController manager);
    public void OnExit(ManagerController manager);
}

public class ChasePlayerManagerState : IManagerState
{
    private float timeLeft;

    public ChasePlayerManagerState(float totalTime)
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
        manager.visitLocations.Clear();
        manager.followPath = null;
    }
}


public class ReturnToOfficeManagerState : IManagerState
{
    private float timeLeft;
    private RoomController targetRoom;

    public ReturnToOfficeManagerState(float totalTime, RoomController room)
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
        manager.visitLocations.Clear();
        manager.followPath = null;
    }
}

public class AnswerCallInOfficeManagerState : IManagerState
{
    private bool startedWalking = false;
    private float timeLeft;

    public AnswerCallInOfficeManagerState(float totalTime)
    {
        timeLeft = totalTime;
    }

    public void OnEnter(ManagerController manager)
    {
        manager.GoTo(manager.office.ringingPhone.transform);
    }

    public void UpdateState(ManagerController manager)
    {
        if (!startedWalking)
        {
            if (manager.followPath != null)
                startedWalking = true;
            return;
        }
        bool arrived = manager.followPath == null;
        if (arrived)
        {
            manager.office.managerPhone.SetActive(true);
            manager.office.ringingPhone.SetActive(false);
            timeLeft -= Time.deltaTime;
        }

        if (timeLeft <= 0)
        {
            manager.FindNewState();
        }
    }

    public void OnExit(ManagerController manager)
    {
        manager.visitLocations.Clear();
        manager.followPath = null;
    }
}

public class DrinkCoffeeManagerState : IManagerState
{
    private float timeLeft;

    public DrinkCoffeeManagerState(float totalTime)
    {
        timeLeft = totalTime;
    }

    public void OnEnter(ManagerController manager)
    {
        manager.GoTo(manager.office.coffee.transform);
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
        manager.office.coffee.SetActive(false);
        manager.visitLocations.Clear();
        manager.followPath = null;
    }
}

public class RageFollowPlayerManagerState : IManagerState
{
    private float followTime;
    private float chillTime;

    public RageFollowPlayerManagerState(float totalFollowTime, float totalChillTime)
    {
        this.followTime = totalFollowTime;
        chillTime = totalChillTime;
    }

    public void OnEnter(ManagerController manager)
    {
        manager.GoTo(manager.office.player.transform);
        manager.angry = true;
    }

    public void UpdateState(ManagerController manager)
    {
        if (followTime >= 0)
        {
            followTime -= Time.deltaTime;
            manager.GoTo(manager.office.player.transform);
            return;
        }

        chillTime -= Time.deltaTime;
        if (chillTime <= 0)
        {
            manager.FindNewState();
        }
    }

    public void OnExit(ManagerController manager)
    {
        manager.angry = false;
        manager.visitLocations.Clear();
        manager.followPath = null;
    }
}