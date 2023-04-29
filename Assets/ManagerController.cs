using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    public float walkSpeed;
    public OfficeController office;

    void Update()
    {
        transform.localPosition += Time.deltaTime * walkSpeed * (Vector3) Random.insideUnitCircle;
    }
}
