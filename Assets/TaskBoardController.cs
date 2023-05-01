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

public class TaskBoardController : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    private void Start()
    {
        this.transform.localPosition = new Vector3(0, 0.3f, -12);
    }

    private void Update()
    {
        var str = "";
        foreach (var activeTask in OfficeController.INSTANCE.activeTaskSequences)
        {
            if (!activeTask.bonus)
            {
                str += activeTask.GetText() + "\n";
            }
        }
        text.SetText(str);
    }
}
