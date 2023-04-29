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
        UnityEngine.Vector2 horizontalWalkDir;
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalWalkDir = UnityEngine.Vector2.left;
        }
        else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalWalkDir = UnityEngine.Vector2.right;
        }
        else
        {
            horizontalWalkDir = UnityEngine.Vector2.zero;
        }

        if (!this.office.CanWalkTo(oldPos + (Vector3) (Time.deltaTime * walkSpeed * horizontalWalkDir)))
        {
            horizontalWalkDir = UnityEngine.Vector2.zero;
        }
        
        UnityEngine.Vector2 verticalWalkDir;
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            verticalWalkDir = UnityEngine.Vector2.up;
        }
        else if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            verticalWalkDir = UnityEngine.Vector2.down;
        }
        else
        {
            verticalWalkDir = UnityEngine.Vector2.zero;
        }
        
        if (!this.office.CanWalkTo(oldPos + (Vector3) (Time.deltaTime * walkSpeed * verticalWalkDir)))
        {
            verticalWalkDir = UnityEngine.Vector2.zero;
        }
        
        // check if we would hit a wall
        var newPos = oldPos + (Vector3) (Time.deltaTime * walkSpeed * (horizontalWalkDir + verticalWalkDir));
        if (this.office.CanWalkTo(newPos))
        {
            //  && this.office.CanWalkTo(newPos + (Vector3) (this.transform.localScale * walkDir))
            this.transform.localPosition = newPos;
        }
    }
}
