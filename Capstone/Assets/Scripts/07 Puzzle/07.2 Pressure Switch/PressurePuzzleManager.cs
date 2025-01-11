using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using PCG.Data_Structures;

public class PressurePuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject parentMap;
    [SerializeField] private GameObject puzzlePrefab;
    [SerializeField] private int switchNumbers;

    private OrderDoorScript orderDoorScript;

    // �ذ��ؾ� �� ��
    // 1. ���� ���� ����Ʈ ���ϱ� -> ��������, ������ ������ �� ����
    // 2. door ������ ���� room�� �������� �� door�� �����Ǿ ���Ա��� ������, ������ �ذ��ϸ� ���Ա��� ������ ��
    // 3. door���� �����Ǹ� OrderDoor ��ũ��Ʈ���� ������Ʈ���� �����ؾ� ��.
    // 4. PressureSwitch�� OrderDoor ��ũ��Ʈ�� ���� �����ؾ� ��.
    // -> �ƴϸ� OrderDoor ��ũ��Ʈ�� �ٸ� ���� ������Ʈ���� �����ϰ�, PressureSwitch�� ���� ������Ʈ�� ������ �ǳ״� ������ �ذ��ؾ� ��

    private void Awake()
    {
        orderDoorScript = new OrderDoorScript(switchNumbers);
    }

    public void SetDoorObjects(GameObject doorObject)
    {
        orderDoorScript.AddDoorObject(doorObject);
    }

    public void SpawnPressureButtonPuzzle(Room room)
    {
        // spawn puzzles in rooms
        if (room.CenterCell == null)
            return;

        GameObject puzzleCell = GameObject.Instantiate(puzzlePrefab, parentMap.transform);
        puzzleCell.name = $"puzzle {room.RoomNumber}";

        // randomly spawn switches
        List<(int x, int y)> entrances = new List<(int x, int y)>();
        List<Vector3> spawnPoints = DecideSpawnPoints(room, ref entrances);
        Debug.Log($"spawnPoints length : {spawnPoints.Count}");

        PressureSwitchSpawner pressureSwitchSpawner = puzzleCell.GetComponent<PressureSwitchSpawner>();
        pressureSwitchSpawner.SpawnSwitches(room, entrances, puzzleCell, switchNumbers, spawnPoints, orderDoorScript);

    }

    private List<Vector3> DecideSpawnPoints(Room room, ref List<(int x, int y)> entrances)
    {
        List<Vector3> selectedPoints = new List<Vector3>();
        List<RoomCell> selectedCells = new List<RoomCell>();
        
        int roomCellsCount = room.RoomCells.Count;
        int[] randomIndices = new int[roomCellsCount];
        for (int i = 0; i < roomCellsCount; i++)
        {
            randomIndices[i] = i;
        }
        ShuffleArray(randomIndices);

        foreach (var corridorCell in room.CorridorCells)
        {
            if (corridorCell.Type == CellType.Corridor)
                entrances.Add((x: corridorCell.X, y: corridorCell.Y));
        }

        for (int i = 0; selectedCells.Count < switchNumbers; i++)
        {
            Debug.Log($"random indices : {randomIndices[i]}");
            RoomCell selectedCell = room.RoomCells[randomIndices[i]];
            if (IsOutOfEntranceRange(room, entrances, selectedCell))
            {
                selectedCells.Add(selectedCell);
            }
        }

        foreach (var roomCell in selectedCells)
        {
            var roomCellObject = parentMap.transform.Find($"Room ({roomCell.X}, {roomCell.Y})").gameObject;
            if (roomCellObject)
            {
                selectedPoints.Add(new Vector3(roomCell.X * 4, 0, -roomCell.Y * 4));
                Debug.Log($"roomCellObject position : {roomCell.X}, {roomCell.Y}, {roomCellObject.transform.position}");
            }
        }

        return selectedPoints;
    }

    private void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            var randomIndex = Random.Range(i, array.Length);

            var temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;            
        }
    }

    private bool IsOutOfEntranceRange(Room room, List<(int x, int y)> entrances, RoomCell selectedCell)
    {
        bool outOfRangeFlag = true;
        const int rangeWidth = 3;
        const int rangeHeight = 3;

        foreach (var entrance in entrances)
        {
            var startPos = (x: entrance.x - 1, y: entrance.y - 1);

            if (startPos.x <= selectedCell.X && selectedCell.X <= startPos.x + rangeWidth && startPos.y <= selectedCell.Y && selectedCell.Y <= startPos.y + rangeHeight)
            {
                outOfRangeFlag = false;
                break;
            }
        }

        return outOfRangeFlag;
    }
}
