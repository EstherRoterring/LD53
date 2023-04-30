using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public OfficeController office;

    public RoomController room;
    public bool moveLeft, moveRight, moveTowards, moveAway, standingStill;
    private Animator anim;
    
    [FormerlySerializedAs("numTasks")]
    public int numSmallTasks;
    public int numBigTasks;
    public int numBonusTasks;
    public float interactDistance;

    public TaskStationController closestStation = null;

    void Start()
    {
        anim=GetComponent<Animator>();
    }
    void Update()
    {
        moveAway=false;
        moveLeft=false;
        moveRight=false;
        moveTowards =false;
        standingStill=true;
        anim.SetBool("standingStill",false);
        anim.SetBool("moveTowards",false);
        anim.SetBool("moveRight",false);
        anim.SetBool("moveLeft",false);
        anim.SetBool("moveAway",false);

        var oldPos = transform.localPosition;
        UnityEngine.Vector2 horizontalWalkDir;
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalWalkDir = UnityEngine.Vector2.left;
            moveLeft=true;
            standingStill=false;

        }
        else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalWalkDir = UnityEngine.Vector2.right;
            moveRight=true;
            standingStill=false;
        }
        else
        {
            horizontalWalkDir = UnityEngine.Vector2.zero;
        }
        
        UnityEngine.Vector2 verticalWalkDir;
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            verticalWalkDir = UnityEngine.Vector2.up;
            moveAway=true;
            standingStill=false;
            moveLeft=false;
            moveRight=false;
        }
        else if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            verticalWalkDir = UnityEngine.Vector2.down;
            moveTowards=true;
            standingStill=false;
            moveLeft=false;
            moveRight=false;
        }
        else
        {
            verticalWalkDir = UnityEngine.Vector2.zero;
        }
        
        var newPos = oldPos + (Vector3) (Time.deltaTime * walkSpeed * (horizontalWalkDir + verticalWalkDir));
        this.transform.localPosition = newPos;
        
        if (moveAway==true){anim.SetBool("moveAway",true);}        
        if(moveLeft==true){anim.SetBool("moveLeft",true);}
        if(moveRight==true){anim.SetBool("moveRight",true);}
        if(moveTowards==true){anim.SetBool("moveTowards",true);}
        if(standingStill==true){anim.SetBool("standingStill",true);}

        var closestStationDist = Mathf.Infinity;
        var lastClosestStation = closestStation;
        closestStation = null;
        foreach (var task in office.activeTaskSequences)
        {
            foreach (var station in task.stations) {
                var dist = Vector3.Distance(station.interactObject.transform.position, transform.position);
                if (dist < closestStationDist)
                {
                    closestStation = station;
                    closestStationDist = dist;
                }
            }
        }
        if (closestStationDist > interactDistance)
        {
            closestStation = null;
        }
        if (closestStation != null && closestStation.limitAccessFromRoom != null &&
            closestStation.limitAccessFromRoom != room)
        {
            closestStation = null;
        }

        foreach (var task in new List<TaskSequence>(office.activeTaskSequences))
        {
            foreach (var station in task.stations)
            {
                if (station == closestStation && (Input.GetKey("e") || Input.GetKey(KeyCode.Space))) {
                    station.UpdateWithInteraction();
                }
                else
                {
                    station.UpdateWithoutInteraction();
                }
            }
        }
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
