using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TaskSequence : MonoBehaviour
{
    public TaskStationController[] stations;
    public string[] stationTexts;
    public Sprite[] stationImages;
    public int[] stationDurations;

    public int currentStation = -1;

    public bool bonus = false;
    public bool spawnsCoffeeInKitchen = false;
    public bool callsTelephone = false;
    
    public void SpawnSequence()
    {
        OfficeController.INSTANCE.activeTaskSequences.Add(this);
        NextStation();
    }

    public void NextStation()
    {
        currentStation += 1;
        if (currentStation >= stations.Length)
        {
            Debug.Log($"{this}: sequence is completed.");
            // task sequence is done, what now?
            OfficeController.INSTANCE.player.didAnyTask = true;
            OfficeController.INSTANCE.manager.dangerLevel += 5f;
            if (!bonus)
            {
                OfficeController.INSTANCE.workloadbar.Add(stations.Length);
            }

            if (spawnsCoffeeInKitchen)
            {
                OfficeController.INSTANCE.SpawnCoffeeInKitchen();
            }

            if (callsTelephone)
            {
                OfficeController.INSTANCE.CallTelephone();
            }
            OfficeController.INSTANCE.activeTaskSequences.Remove(this);
            return;
        }
        stations[currentStation].AddTaskToQueue(this);
    }

    public void NotifyTaskCompleted()
    {
        NextStation();
    }
}
