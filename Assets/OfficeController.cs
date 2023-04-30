using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;


public class OfficeController : MonoBehaviour
{
    public static OfficeController INSTANCE;
    
    public PlayerController player;
    public ManagerController manager;

    public GameObject floorPlan;
    public Texture2D roomLookupTexture;

    public GameObject roomGraph;
    private List<RoomController> rooms = new List<RoomController>();

    public TaskSequence[] allDutySmallTaskSequences;
    public TaskSequence[] allDutyBigTaskSequences;
    public TaskSequence[] allBonusTaskSequences;

    public List<TaskSequence> activeTaskSequences = new List<TaskSequence>();
    
    public GameObject taskExclamationPrefab;
    public GameObject taskTimerPrefab;

    public OfficeController()
    {
        INSTANCE = this;
    }
    
    void Start()
    {
        // room layout
        foreach (var room in roomGraph.transform.GetComponentsInChildren<RoomController>())
        {
            rooms.Add(room);
        }
        foreach (var door in roomGraph.transform.GetComponentsInChildren<DoorController>())
        {
            door.fromRoom.doors.Add(door, door.toRoom);
            door.toRoom.doors.Add(door, door.fromRoom);
        }
        
        // manager
        manager.ChangeState(new ChasePlayerState(5f));
        
        var rnd = new System.Random();
        foreach (var sequence in allDutySmallTaskSequences.OrderBy(x => rnd.Next()).Take(player.numSmallTasks))
        {
            sequence.SpawnSequence();
        }
        foreach (var sequence in allDutyBigTaskSequences.OrderBy(x => rnd.Next()).Take(player.numBigTasks))
        {
            sequence.SpawnSequence();
        }
        foreach (var sequence in allBonusTaskSequences.OrderBy(x => rnd.Next()).Take(player.numBonusTasks))
        {
            sequence.SpawnSequence();
        }
    }

    private Color LookupRoomColor(Vector2 pos)
    {
        var floorPlanSize = 4 * floorPlan.transform.localScale * new Vector2(1, (float) roomLookupTexture.height / roomLookupTexture.width);
        Vector2 leftBottom = (Vector2) floorPlan.transform.localPosition - 0.5f * floorPlanSize;
        var discrete = (pos - leftBottom) / floorPlanSize;
        var pixel = new Vector2Int((int) (roomLookupTexture.width * discrete.x + 0.5f), (int) (roomLookupTexture.height * discrete.y + 0.5f));
        if (pixel.x < 0 || pixel.x >= roomLookupTexture.width || pixel.y < 0 || pixel.y >= roomLookupTexture.height)
        {
            return Color.black;
        }
        return roomLookupTexture.GetPixel(pixel.x, pixel.y);
    }

    void Update()
    {
        if (LookupRoomColor(player.transform.localPosition) == LookupRoomColor(manager.transform.localPosition))
        {
            Debug.Log("YOU DIE!");
        }
    }
}
