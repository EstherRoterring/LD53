using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    public float walkSpeed;
    
    void Update()
    {
        transform.localPosition += walkSpeed * (Vector3) Random.insideUnitCircle;
    }
}
