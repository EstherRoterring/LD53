using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class OfficeController : MonoBehaviour
{
    public PlayerController player;
    public GameObject manager;

    public GameObject floorPlan;
    public Texture2D roomLookupTexture;

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


public enum Room
{
    ManagerRoom, OFFICE_1, OFFICE_2, OFFICE_3, PRINTER, STORAGE, HALL_LEFT, PARK, HALL_RIGHT, TOILET, COFFEE, OFFICE_4, JANITOR, FOYER, OWN_OFFICE
}