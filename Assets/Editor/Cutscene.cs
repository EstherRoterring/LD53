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
    public bool completed = false;

    public GameObject[] charList;
    private GameObject currentChar = null;
    public string[] charNames;

    public float charTime;
    
    public TextBoxController textBox;

    private string currentSpeaker;
    private string currentMessage;
    
    public float walkSpeed;
    private float currentWalkSpeed;

    public GameObject moveChar;
    private bool waitForPathfinding = false;
    public Path followPath = null;
    private int currentWaypoint = 0;
    public float nextWaypointDistance;
    public GameObject continueButton;

    public bool introScene;
    public bool endScene;
    public bool allTasksDoneScene;

    public void Start()
    {
        // disable all at beginning
        foreach (var c in this.charList)
            if (c != null)
                c.SetActive(false);

        textBox.gameObject.SetActive(true);
        textBox.Position2();
        continueButton.SetActive(false);

        currentLine = 0;
        ShowCurrentLine();
    }

    public void SetMoveToPath(Path p)
    {

        currentWalkSpeed = walkSpeed;
        followPath = p;
        currentWaypoint = 0;
        waitForPathfinding = false;
    }
    
    public void ShowCurrentLine()
    {
        var fullLine = textSequence[currentLine];
        var splitBy = new char[] { ':' };
        var split = fullLine.Split(splitBy, 2);
        currentSpeaker = split[0].Trim();
        currentMessage = split[1].Trim();

        if (currentMessage == "[show clipboard]")
        {
            currentLine += 1;
            ShowCurrentLine();
            textBox.Position1();

            OfficeController.INSTANCE.StartGameAfterIntro();
            OfficeController.INSTANCE.taskBoardStuckOpen = true;

            return;
        }

        if (charMoveWhoSequence[currentLine] != null)
        {
            moveChar = charMoveWhoSequence[currentLine];
            currentWalkSpeed = walkSpeed;
            var whereTo = charMoveToSequence[currentLine];
            
            Seeker seeker = GetComponent<Seeker>();
            waitForPathfinding = true;
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
        continueButton.SetActive(false);
        textBox.SetText("");
        currentMessageProgress = 0;
    }

    private void Update()
    {
        if (OfficeController.INSTANCE.cutscenePlaying != this)
        {
            return;
        }
        if (completed)
            return;
        if (currentMessage == null)
            return;
        if (waitForPathfinding)
            return;

        currentMessageProgress += Time.deltaTime * charTime;
        if (moveChar != null)
        {
            MoveChars();
            if (Input.GetKeyDown("space") || (currentMessage == "" && followPath == null))
            {
                MouseDown();
            }
            return;
        }

        textBox.SetText(currentMessage.Substring(0, Math.Min((int)currentMessageProgress, currentMessage.Length)));
        if ((int)currentMessageProgress >= currentMessage.Length)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }

        if (Input.GetKeyDown("space") || currentMessage == "")
        {
            MouseDown();
        }
    }
    
    public void MouseDown()
    {
        if (moveChar != null)
        {
            if (currentMessageProgress >= 0.1f)
            {
                // teleport the guy
                currentWalkSpeed = 10f;
            }
            return;
        }
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
            DoFinished();
        }
    }

    private void DoFinished()
    {
        completed = true;
        textBox.Visible(false);
        OfficeController.INSTANCE.cutscenePlaying = null;

        if (introScene)
        {
            OfficeController.INSTANCE.manager.showExclamationMark = false;
        }
        if (allTasksDoneScene)
        {
            OfficeController.INSTANCE.manager.showExclamationMark = true;
        }
        // todo, end scene

        gameObject.SetActive(false);
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
            Vector3 velocity = currentWalkSpeed * dir;

            moveChar.transform.position += velocity * Time.deltaTime;

            PlayerController maybeIsPlayer = null;
            if (moveChar.TryGetComponent(out maybeIsPlayer))
            {
                UpdateAnimationDirection(maybeIsPlayer.anim, velocity);
            }

            if (reachedEndOfPath)
            {
                followPath = null;
                currentWaypoint = 0;
                moveChar = null;
                currentWalkSpeed = walkSpeed;

                if (maybeIsPlayer != null)
                {
                    UpdateAnimationDirection(maybeIsPlayer.anim, Vector3.zero);
                }
            }
        }
    }
    
    public void UpdateAnimationDirection(Animator anim, Vector3 velocity)
    {
        if (velocity.magnitude <= 0.01f)
        {
            anim.SetBool("standingStill", true);
            anim.SetBool("moveTowards",false);
            anim.SetBool("moveRight",false);
            anim.SetBool("moveLeft",false);
            anim.SetBool("moveAway",false);
        } else
        {
            anim.SetBool("standingStill", false);
            var xMax = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y);
            anim.SetBool("moveTowards",!xMax && velocity.y <= 0);
            anim.SetBool("moveAway",!xMax && velocity.y >= 0);
            anim.SetBool("moveLeft",xMax && velocity.x <= 0);
            anim.SetBool("moveRight",xMax && velocity.x >= 0);
        }
    }
}
