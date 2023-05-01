using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Pathfinding;

public class Cutscene : MonoBehaviour
{
    public string[] textSequence;
    [FormerlySerializedAs("playerMoveSequence")] public Transform[] charMoveToSequence;
    public GameObject[] charMoveWhoSequence;

    public int currentLine = 0;
    public float currentMessageProgress = 0;

    public GameObject[] charList;
    private GameObject currentChar = null;
    public string[] charNames;

    public float charTime;
    
    public TextBoxController textBox;

    private string currentSpeaker;
    private string currentMessage;
    
    public float walkSpeed;

    public GameObject moveChar;
    public Path followPath = null;
    private int currentWaypoint = 0;
    public float nextWaypointDistance;

    public void Start()
    {
        
        // disable all at beginning
        foreach (var c in this.charList)
            if (c != null)
                c.SetActive(false);

        textBox.Position2();

        currentLine = 0;
        ShowCurrentLine();
        
    }

    public void SetMoveToPath(Path p)
    {
        followPath = p;
        currentWaypoint = 0;
        Debug.Log("set follow path");
    }
    
    public void ShowCurrentLine()
    {
        var fullLine = textSequence[currentLine];
        var splitBy = new char[] { ':' };
        var split = fullLine.Split(splitBy, 2);
        currentSpeaker = split[0].Trim();
        currentMessage = split[1].Trim();

        if (charMoveWhoSequence[currentLine] != null)
        {
            moveChar = charMoveWhoSequence[currentLine];
            var whereTo = charMoveToSequence[currentLine];
            Debug.Log($"move {moveChar} to {whereTo.position}");
            
            Seeker seeker = GetComponent<Seeker>();
            seeker.StartPath(moveChar.transform.position, whereTo.position, SetMoveToPath);
        }

        // find char
        if (currentChar != null)
            currentChar.SetActive(false);
        currentChar = charList[Array.IndexOf(this.charNames, this.currentSpeaker)];
        if (currentChar != null)
        {
            currentChar.SetActive(true);
        }

        textBox.Visible(true);
        textBox.SetText("");
        currentMessageProgress = 0;
    }

    private void Update()
    {
        if (currentMessage == null)
            return;
        if (moveChar != null)
        {
            MoveChars();
            return;
        }

        if (currentMessageProgress < currentMessage.Length)
        {
            currentMessageProgress += Time.deltaTime * charTime;
        }

        textBox.SetText(currentMessage.Substring(0, Math.Min((int)currentMessageProgress, currentMessage.Length)));

        if (Input.GetKeyDown("space") || currentMessage == "")
        {
            MouseDown();
        }
    }
    
    public void MouseDown()
    {
        Debug.Log("click");
        if (currentMessageProgress < currentMessage.Length)
        {
            currentMessageProgress = currentMessage.Length;
            return;
        }

        if (currentLine + 1 < textSequence.Length)
        {
            currentLine += 1;
            ShowCurrentLine();
        }
        else
        {
            textBox.Visible(false);
            OfficeController.INSTANCE.cutscenePlaying = null;
        }
    }

    private void MoveChars()
    {
        if (followPath != null)
        {
            bool reachedEndOfPath = false;
            float distanceToWaypoint;
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(moveChar.transform.position, followPath.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    if (currentWaypoint + 1 < followPath.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
                
            Vector3 dir = (followPath.vectorPath[currentWaypoint] - moveChar.transform.position).normalized;
            Vector3 velocity = walkSpeed * dir;
            // UpdateAnimationDirection(velocity);

            Debug.Log($"move {moveChar} from {moveChar.transform.position} along {dir} / {velocity}, {currentWaypoint} dist {distanceToWaypoint}");
            moveChar.transform.position += velocity * Time.deltaTime;

            if (reachedEndOfPath)
            {
                followPath = null;
                currentWaypoint = 0;
                moveChar = null;
            }
        }
    }
}
