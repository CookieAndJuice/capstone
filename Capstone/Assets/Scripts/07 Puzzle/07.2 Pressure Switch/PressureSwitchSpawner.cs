using PCG.Data_Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitchSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> pressureSwitchObjects;

    private List<PressureSwitch> pressureSwitches;

    // instantiate switches at right position
    // and add switches in list with randomly fixed order
    public void SpawnSwitches(Room room, List<(int x, int y)> entrances, GameObject parentPuzzle, int switchNumbers, List<Vector3> selectedPoints, OrderDoorScript orderDoorScript)
    {
        pressureSwitches = new List<PressureSwitch>();

        GameObject switchPrefab = null;
        int[] randomArray = new int[switchNumbers];
        GenerateRandomArray(randomArray);

        for (int i = 0; i < switchNumbers; i++)
        {
            switchPrefab = Instantiate(pressureSwitchObjects[i], parentPuzzle.transform);
            Debug.Log($"index, selectedPoints counts : {i}, {selectedPoints.Count}");
            switchPrefab.transform.localPosition = selectedPoints[i];
            switchPrefab.GetComponentInChildren<PressureSwitch>().orderDoorScript = orderDoorScript;
            switchPrefab.GetComponentInChildren<PressureSwitch>().switchNumber = i + 1;

            AddSwitches(switchPrefab);
        }
    }

    // for DoorScript
    private void AddSwitches(GameObject switchObject)
    {
        PressureSwitch pressureSwitch = switchObject.GetComponentInChildren<PressureSwitch>();
        pressureSwitches.Add(pressureSwitch);
    }

    // OrderDoor에 맞는 함수
    //private void AddSwitches(GameObject switchObject, int switchNumber)
    //{
    //    PressureSwitch pressureSwitch = switchObject.GetComponentInChildren<PressureSwitch>();
    //    Debug.Log($"pressure switch object : {pressureSwitch}");
    //    // pressureSwitch.Door = 
    //    pressureSwitch.switchNum = switchNumber;
    //    pressureSwitch.cellPosition = switchObject.transform.localPosition;

    //    pressureSwitches.Add(pressureSwitch);
    //}

    private void GenerateRandomArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i + 1;
        }

        int randomIndex = 0;
        int temp = 0;

        // Fisher-Yates Shuffle
        for (int i = 0; i < array.Length; i++)
        {
            randomIndex = Random.Range(i, array.Length);

            temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
