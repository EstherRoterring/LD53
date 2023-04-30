using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

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
                    var prefab = OfficeController.INSTANCE.taskExclamationPrefab;
                    if (currentActiveTask.bonus)
                    {
                        prefab = OfficeController.INSTANCE.taskBonusExclamationPrefab;
                    }
                    taskActiveMarker = Instantiate(prefab, markerPosition);
                }
            }
        }
    }

    public void UpdateWithInteraction()
    {
        if (currentActiveTask != null)
        {
            if (progress == 0f)
            {
                // spawn the timer!!!
                if (taskActiveMarker != null)
                {
                    Destroy(taskActiveMarker);
                }
                taskActiveMarker = Instantiate(OfficeController.INSTANCE.taskTimerPrefab, markerPosition);
                progressAnimator = taskActiveMarker.GetComponent<Animator>();
                progressAnimator.speed = 1.4f / maxProgress;
            }

            progressAnimator.Play("TaskProgressTimerAnim", 0, progress / maxProgress);
            progressAnimator.enabled = true;
            progress += Time.deltaTime;
            Debug.Log(progress);
            if (progress >= maxProgress)
            {
                // timer is over, reset everything
                currentActiveTask.NotifyTaskCompleted();
                currentActiveTask = null;
                Destroy(taskActiveMarker);
                taskActiveMarker = null;
                progress = 0f;
            }
        }
    }

    public void UpdateWithoutInteraction()
    {
        if (currentActiveTask != null && progressAnimator != null)
        {
            progressAnimator.enabled = false;
        }
    }
}
