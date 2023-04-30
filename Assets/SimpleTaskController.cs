using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleTaskController : MonoBehaviour
{
    public GameObject interactObject;
    public GameObject interactObjectHighlight;
    public Transform markerPosition;
    private Animator progressAnimator;

    public bool active;
    public bool finished;
    private GameObject taskActiveMarker;

    public Color interactHighlightColor;

    public float progress = 0f;
    public float maxProgress;

    public void SetActive()
    {
        active = true;
        taskActiveMarker = Instantiate(OfficeController.INSTANCE.taskExclamationPrefab, markerPosition);
    }

    void Update()
    {
        var highlightRenderer = interactObjectHighlight.GetComponent<SpriteRenderer>();
        if (active)
        {
            if (OfficeController.INSTANCE.player.closestTaskController == this)
            {
                highlightRenderer.color = interactHighlightColor;
                
            } else {
                highlightRenderer.color = new Color(1, 1, 1, 0.5f + 0.5f * Mathf.Sin(Time.time));
            }
        }
        else
        {
            highlightRenderer.color = new Color(1, 1, 1, 0);
        }
    }

    public void UpdateWithInteraction()
    {
        if (active)
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
                // timer is over
                finished = true;
                Destroy(gameObject);
                // blabla give the player some kudos
            }
        }
    }

    public void UpdateWithoutInteraction()
    {
        if (active && progressAnimator != null)
        {
            progressAnimator.enabled = false;
        }
    }
}
