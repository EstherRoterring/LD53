using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OfficeController : MonoBehaviour
{
    public PlayerController player;
    public GameObject manager;

    public FloorPlanController floorPlan;

    private Color GetFloorPlanTextureColor(Vector2 pos)
    {
        var texture = floorPlan.texture;
        var floorPlanSize = 4 * floorPlan.transform.localScale * new Vector2(1, (float)texture.height / texture.width);
        Vector2 leftBottom = (Vector2) floorPlan.transform.localPosition - 0.5f * floorPlanSize;
        var discrete = (pos - leftBottom) / floorPlanSize;
        var pixel = new Vector2Int((int) (texture.width * discrete.x + 0.5f), (int) (texture.height * discrete.y + 0.5f));
        if (pixel.x < 0 || pixel.x >= texture.width || pixel.y < 0 || pixel.y >= texture.height)
        {
            return Color.black;
        }
        return floorPlan.texture.GetPixel(pixel.x, pixel.y);
    }
    
    public bool CanWalkTo(Vector2 pos)
    {
        return GetFloorPlanTextureColor(pos) != Color.black;
    }
}
