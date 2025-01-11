using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript
{
    private List<PressureSwitch> pressureSwitches;
    public List<GameObject> doorObjects;
    private int needTriggerNumbers { get; }

    public DoorScript(int triggerNumbers)
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

            if (pressureSwitches.Count == needTriggerNumbers)
                OpenDoors();
        }
    }

    public void RemoveTriggerObject(PressureSwitch triggerObject)
    {
        if (pressureSwitches.Contains(triggerObject))
        {
            pressureSwitches.Remove(triggerObject);
        }
    }

    private void OpenDoors()
    {
        Debug.Log("문이 열림");
    }
}
