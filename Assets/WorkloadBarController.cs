using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkloadBarController: MonoBehaviour
{
    //Speichert jeden Bar Status
    public Sprite[] progress;
    private int points = 0;

    //Workload +x
    public void Add(int points) {
        this.points = this.points + points;
        if (0 > this.points) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = progress[0];
        }
        else if (this.points > 7) {
            GetComponent<SpriteRenderer>().sprite = progress[7];
        }
        else {
            GetComponent<SpriteRenderer>().sprite = progress[this.points];
        }

    }

}
