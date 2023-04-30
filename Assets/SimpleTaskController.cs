using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleTaskController : MonoBehaviour
{
    public GameObject interactObject;
    public GameObject interactObjectHighlight;
    public Transform markerPosition;

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
            }
            progress += Time.deltaTime;
            if (progress >= maxProgress)
            {
                // timer is over
                finished = true;
                Destroy(this);
                
                // todo, points, blabla
            }
        }
    }
}
