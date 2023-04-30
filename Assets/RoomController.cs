using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Transform roomCenter;

    public Dictionary<DoorController, RoomController> doors = new Dictionary<DoorController, RoomController>();
}
