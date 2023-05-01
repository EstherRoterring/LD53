using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleComponent : MonoBehaviour
{
    private Vector3 origin;
    public float wobbleX = 0;
    public float wobbleY = 0.2f;
    public float wobbleSpeed = 10f;

    void Start()
    {
        origin = transform.localPosition;
    }
    
    void Update()
    {
        var t = wobbleSpeed * Time.time;
        var v = origin + Mathf.Sin(t) * new Vector3(wobbleX, wobbleY, 0);
        transform.localPosition = new Vector3(v.x, v.y, transform.localPosition.z);
    }
}
