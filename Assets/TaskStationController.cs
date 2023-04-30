using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TaskStationController : MonoBehaviour
{
    public GameObject interactObject;
    public GameObject interactObjectHighlight;
    public Transform markerPosition;
    private Animator progressAnimator;

    private GameObject taskActiveMarker;

    public Color interactHighlightColor;

    public float progress = 0f;
    public float maxProgress;

    private TaskSequence currentActiveTask = null;
    private List<TaskSequence> taskQueue = new List<TaskSequence>();

    [FormerlySerializedAs("completionDelay")] public float completionDelayMax = 1f;
    private float completionDelay = 0;

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
                
            } else {
                highlightRenderer.color = new Color(1, 1, 1, 0.5f + 0.5f * Mathf.Sin(Time.time));
            }
        }
        else
        {
            highlightRenderer.color = new Color(1, 1, 1, 0);
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
                
                //Textbox ploep
                String text = currentActiveTask.stationTexts[currentActiveTask.currentStation];
                Sprite image = currentActiveTask.stationImages[currentActiveTask.currentStation];
                OfficeController.INSTANCE.textbox.SetTextBoxAutoPos(text, image);
             }

            progressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
            progressAnimator.enabled = true;
            progress += Time.deltaTime;
            if (progress >= maxProgress)
            {
                // timer is over, reset everything
                currentActiveTask.NotifyTaskCompleted();
                currentActiveTask = null;
                Destroy(taskActiveMarker);
                taskActiveMarker = null;
                progress = 0f;
                //macht Textbox weg
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
            }

            progress -= 0.2f * Time.deltaTime;
            if (progress <= 0)
            {
                progress = 0;
                progressAnimator = null;
                SpawnExclamationMark();
                //macht Textbox weg
                OfficeController.INSTANCE.textbox.Visible(false);
            }
        }
    }
}
