using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.localPosition += walkSpeed * new Vector3(-1f, 0f, 0f);
        }
        else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.localPosition += walkSpeed * new Vector3(1f, 0f, 0f);
        }
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.localPosition += walkSpeed * new Vector3(0f, 1f, 0f);
        }
        else if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.localPosition += walkSpeed * new Vector3(0f, -1f, 0f);
        }
    }
}
