using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class OrderDoorScript : MonoBehaviour
{
    private List<PressureSwitch> pressureSwitches;
    public List<GameObject> doorObjects;
    private int needTriggerNumbers { get; }

    public OrderDoorScript(int triggerNumbers)
    {
        pressureSwitches = new List<PressureSwitch>();
        doorObjects = new List<GameObject>();
        needTriggerNumbers = triggerNumbers;
    }

    public void AddDoorObject(GameObject doorObject)
    {
        if (!doorObjects.Contains(doorObject))
            doorObjects.Add(doorObject);
    }

    public void AddTriggerObject(PressureSwitch triggerObject)
    {
        if (!pressureSwitches.Contains(triggerObject))
        {
            pressureSwitches.Add(triggerObject);

            if (CheckOrder())
            {
                if (pressureSwitches.Count == needTriggerNumbers)
                    OpenDoor();
            }
            else
            {
                Debug.Log("∂Ø!");
                pressureSwitches.Clear();
            }
        }
    }

    private bool CheckOrder()
    {
        bool rightOrderFlag = true;
        int index = 0;

        foreach (var pressureSwitch in pressureSwitches)
        {
            index++;
            if (pressureSwitch.switchNumber != index)
            {
                rightOrderFlag = false;
                break;
            }
        }

        return rightOrderFlag;
    }

    private void OpenDoor()
    {
        Debug.Log("πÆ¿Ã ø≠∏≤");

        foreach (var doorObject in doorObjects)
            doorObject.SetActive(false);
    }
}
