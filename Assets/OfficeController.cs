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
    public RoomController managerRoom;
    public RoomController coffeeRoom;

    public TaskSequence[] allDutySmallTaskSequences;
    public TaskSequence[] allDutyBigTaskSequences;
    public TaskSequence[] allBonusTaskSequences;

    public List<TaskSequence> activeTaskSequences = new List<TaskSequence>();
    
    public GameObject taskExclamationPrefab;
    public GameObject taskBonusExclamationPrefab;
    public GameObject taskTimerPrefab;

    public TaskBoardController taskBoard;
    public bool showingTaskBoard = false;
    
    //Textbox starter
    public TextBoxController textbox;
    //Punktezaehler
    public WorkloadBarController workloadbar;

    public GameObject coffee;
    public GameObject ringingPhone;
    public GameObject managerPhone;

    public Cutscene introCutscene;
    public Cutscene outroCutscene;
    public bool gameOver = false;
    public bool debugSkipIntro = false;
    
    public Cutscene cutscenePlaying = null;
    
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
        manager.ChangeState(new ChasePlayerManagerState(5f));
        
        // intro
        cutscenePlaying = introCutscene;

        if (debugSkipIntro)
        {
            cutscenePlaying.gameObject.SetActive(false);
            cutscenePlaying = null;
            StartGameAfterIntro();
        }
        
        //worloadBar auf 0
        workloadbar.Add(0);
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
        if (HasFreeControlflow())
        {
            if (LookupRoomColor(player.transform.localPosition) == LookupRoomColor(manager.transform.localPosition)
                && Vector2.Distance(player.transform.position, manager.transform.position) <= manager.viewDistance)
            {
                gameOver = true;
                cutscenePlaying = outroCutscene;
            }
        
            // update sprite masks
            foreach (var room in rooms)
            {
                room.spriteMasks.gameObject.SetActive(manager.room == room);
            }
        }

    }

    public void FixedUpdate()
    {
        Debug.Log($"{Input.GetKeyDown(KeyCode.A)}");
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showingTaskBoard = true;
        }
        else
        {
            showingTaskBoard = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && showingTaskBoard)
        {
            showingTaskBoard = false;
        }
        taskBoard.gameObject.SetActive(showingTaskBoard);
    }

    public void SpawnCoffeeInKitchen()
    {
        coffee.SetActive(true);
    }
    
    public void CallTelephone()
    {
        ringingPhone.SetActive(true);
        managerPhone.SetActive(false);
    }

    public bool HasFreeControlflow()
    {
        return cutscenePlaying == null && !gameOver && !showingTaskBoard;
    }

    public void StartGameAfterIntro()
    {
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
        // todo, show flip chart now
    }
}
