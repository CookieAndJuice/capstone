using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RoomManager: MonoBehaviour
{
    // a singleton class to manage the rooms
    private static RoomManager _instance;
    public static RoomManager Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType<RoomManager>();

            return _instance;
        }
    }
    
    public Dictionary<int, GameObject> RoomParents = new Dictionary<int, GameObject>();
    
    public List<FloorButton> Doors = new List<FloorButton>();

    public CellType CurrentCellType { get; set; }
    public int CurrentRoomNumber { get; private set; }
    
    public Dictionary<int, List<int>> AdjacencyList { get; set; }

    public bool isTouchingCorridorCell;

    public void InitializeRoomParents()
    {
        foreach (var roomNumber in RoomParents.Keys)
            if (!AdjacencyList[0].Contains(roomNumber) && roomNumber != 0)
                DisableRoom(roomNumber);
    }
    
    // setter for the current room number
    public void SetCurrentRoomNumber(int nextRoomNumber)
    {
        if (CurrentRoomNumber == nextRoomNumber)
            return;

        // Debug.Log($"Room number: {CurrentRoomNumber} -> {nextRoomNumber}");
        
        var roomsToDisable = new List<int>();
        foreach (var roomNumber in AdjacencyList[CurrentRoomNumber])
            if (roomNumber != nextRoomNumber)
                roomsToDisable.Add(roomNumber);
        
        var roomsToEnable = new List<int>();
        foreach (var roomNumber in AdjacencyList[nextRoomNumber])
            if (roomNumber != CurrentRoomNumber)
                roomsToEnable.Add(roomNumber);
        
        CurrentRoomNumber = nextRoomNumber;
        
        foreach (var roomNumber in roomsToDisable)
            DisableRoom(roomNumber);
        
        foreach (var roomNumber in roomsToEnable)
            EnableRoom(roomNumber);
    }
    
    public void SetCurrentCellType(CellType cellType)
    {
        if (CurrentCellType == cellType)
            return;
        
        // Debug.Log($"Cell type: {CurrentCellType} -> {cellType}");
        CurrentCellType = cellType;
    }
    
    // disable the room
    private void DisableRoom(int roomNumber)
    {
        RoomParents[roomNumber].SetActive(false);
    }
    
    // enable the room
    private void EnableRoom(int roomNumber)
    {
        RoomParents[roomNumber].SetActive(true);
    }
    
    // log the adjacency list
    public void LogAdjacencyList()
    {
        var str = "";
        foreach (var key in AdjacencyList.Keys)
        {
            str += $"Room {key}: ";
            foreach (var value in AdjacencyList[key])
                str += $"{value}, ";
            str += "\n";
        }
        Debug.Log(str);
    }

    public void OpenAllDoors()
    {
        foreach (var door in Doors)
            door.SetDoors(false);
    }
    
    public void CloseAllDoors()
    {
        foreach (var door in Doors)
            door.SetDoors(true);
    }
}
