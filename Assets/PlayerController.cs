using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.UI;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public OfficeController office;
    
    void Update()
    {
        var oldPos = transform.localPosition;
        var walkDir = UnityEngine.Vector2.zero;
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            walkDir += UnityEngine.Vector2.left;
        }
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            walkDir += UnityEngine.Vector2.right;
        }
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            walkDir += UnityEngine.Vector2.up;
        }
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            walkDir += UnityEngine.Vector2.down;
        }
        
        // check if we would hit a wall
        var newPos = oldPos + (Vector3) (Time.deltaTime * walkSpeed * walkDir);
        if (this.office.CanWalkTo(newPos) && this.office.CanWalkTo(newPos + (Vector3) (this.transform.localScale * walkDir)))
        {
            this.transform.localPosition = newPos;
        }
    }
}
