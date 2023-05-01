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
    
    //Task Attribute
    public bool bonus = false;
    public bool spawnsCoffeeInKitchen = false;
    public bool duckNotHungry = false;
    public AudioClip audio_quack;
    public AudioSource audioSource;
    public bool callsTelephone = false;
    public bool mariageEnabled = false;
    
    public string boardText = "";
    
    public void SpawnSequence()
    {
        OfficeController.INSTANCE.activeTaskSequences.Add(this);
        currentStation = -1;
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
                if (stations.Length >= 2)
                {
                    OfficeController.INSTANCE.workloadbar.Add(2);
                }
                else
                {
                    OfficeController.INSTANCE.workloadbar.Add(1);
                }

                OfficeController.INSTANCE.CheckAllTasksDone();
            }
            else
            {
                OfficeController.INSTANCE.completedBonusTasks.Add(this);
            }

            if (spawnsCoffeeInKitchen)
            {
                OfficeController.INSTANCE.SpawnCoffeeInKitchen();
            }

            if (duckNotHungry)
            {
                OfficeController.INSTANCE.makeDuckBig();
                audioSource.PlayOneShot(audio_quack,1.0f);
            }

            if (mariageEnabled)
            {
                OfficeController.INSTANCE.player.flirtTaskCompleted = true;
            }

            if (callsTelephone)
            {
                OfficeController.INSTANCE.CallTelephone();
                if (OfficeController.INSTANCE.explainPhoneCutscene != null)
                {
                    OfficeController.INSTANCE.cutscenePlaying = OfficeController.INSTANCE.explainPhoneCutscene;
                    OfficeController.INSTANCE.cutscenePlaying.gameObject.SetActive(true);
                    OfficeController.INSTANCE.explainPhoneCutscene = null;
                }
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

    public string GetText()
    {
        return boardText;
    }
}
