using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;



public class TaskStationController : MonoBehaviour
{
    public GameObject interactObject;
    public GameObject interactObjectHighlight;
    public GameObject spaceBar;
    public Transform markerPosition;
    private Animator progressAnimator;
    private Animator secondProgressAnimator;

    private GameObject taskActiveMarker;

    public Color interactHighlightColor;

    public float progress = 0f;
    public float maxProgress;

    private TaskSequence currentActiveTask = null;
    private List<TaskSequence> taskQueue = new List<TaskSequence>();

    [FormerlySerializedAs("completionDelay")] public float completionDelayMax = 1f;
    private float completionDelay = 0;

    public RoomController limitAccessFromRoom = null;

    public void AddTaskToQueue(TaskSequence taskSequence)
    {
        taskQueue.Add(taskSequence);
    }

    void Update()
    {
        var highlightRenderer = interactObjectHighlight.GetComponent<SpriteRenderer>();
        if (currentActiveTask != null)
        {
            if (OfficeController.INSTANCE.player.closestStation == this)
            {
                highlightRenderer.color = interactHighlightColor;
                spaceBar.SetActive(true);
                
            } else {
                highlightRenderer.color = new Color(1, 1, 1, 0.5f + 0.5f * Mathf.Sin(Time.time));
                spaceBar.SetActive(false);
            }
        }
        else
        {
            highlightRenderer.color = new Color(1, 1, 1, 0);
            spaceBar.SetActive(false);
            // spawn new task
            completionDelay += Time.deltaTime;
            if (completionDelay > completionDelayMax)
            {
                completionDelay = 0;
                if (taskQueue.Count > 0)
                {
                    currentActiveTask = taskQueue[0];
                    taskQueue.RemoveAt(0);
                    SpawnExclamationMark();
                }
            }
        }
    }

    public void SpawnExclamationMark()
    {
        if (taskActiveMarker != null)
        {
            Destroy(taskActiveMarker);
        }
        var prefab = OfficeController.INSTANCE.taskExclamationPrefab;
        if (currentActiveTask.bonus)
        {
            prefab = OfficeController.INSTANCE.taskBonusExclamationPrefab;
        }
        taskActiveMarker = Instantiate(prefab, markerPosition);
    }
    
    public void UpdateWithInteraction()
    {
        if (currentActiveTask != null)
        {
            if (progress <= 0f)
            {
                // spawn the timer!!!
                if (taskActiveMarker != null)
                {
                    Destroy(taskActiveMarker);
                }
                taskActiveMarker = Instantiate(OfficeController.INSTANCE.taskTimerPrefab, markerPosition);
                progressAnimator = taskActiveMarker.GetComponent<Animator>();
                progressAnimator.speed = 1.4f / maxProgress;
                secondProgressAnimator = OfficeController.INSTANCE.textbox.animator;
                secondProgressAnimator.speed = 1.4f / maxProgress;

                if (currentActiveTask.currentStation < currentActiveTask.stationTexts.Length)
                {
                    //Textbox ploep
                    String text = currentActiveTask.stationTexts[currentActiveTask.currentStation];
                    Sprite image = currentActiveTask.stationImages[currentActiveTask.currentStation];
                    OfficeController.INSTANCE.textbox.SetTextBoxAutoPos(text, image);
                }
                else
                {
                    Debug.Log($"broken! {this}, currentStation={currentActiveTask.currentStation}");
                }
             }

            progressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
            progressAnimator.enabled = true;
            secondProgressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
            secondProgressAnimator.enabled = true;
            progress += Time.deltaTime;
            if (progress >= maxProgress)
            {
                // timer is over, reset everything
                currentActiveTask.NotifyTaskCompleted();
                currentActiveTask = null;
                Destroy(taskActiveMarker);
                taskActiveMarker = null;
                progress = 0f;
                OfficeController.INSTANCE.textbox.Visible(false);
            }
        }
    }

    public void UpdateWithoutInteraction()
    {
        if (currentActiveTask != null && progress > 0)
        {
            if (progressAnimator != null)
            {
                progressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
                progressAnimator.enabled = true;
                secondProgressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
                secondProgressAnimator.enabled = true;
            }

            progress -= 0.2f * Time.deltaTime;
            if (progress <= 0)
            {
                progress = 0;
                progressAnimator = null;
                secondProgressAnimator = null;
                SpawnExclamationMark();
                //macht Textbox weg
                OfficeController.INSTANCE.textbox.Visible(false);
            }
        }
    }
}
