using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomController : MonoBehaviour
{
    public Transform roomCenter;
    public Transform spriteMasks;

    public Dictionary<DoorController, RoomController> doors = new Dictionary<DoorController, RoomController>();
}
