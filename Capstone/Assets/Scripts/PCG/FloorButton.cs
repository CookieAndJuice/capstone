using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour
{
    // when pressed(collided) by player, change the current room value in the RoomManager

    public CellType CellType;
    public int RoomNumber;
    public GameObject Doors;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CellType == CellType.Corridor)
            {
                RoomManager.Instance.SetCurrentCellType(CellType);
                RoomManager.Instance.isTouchingCorridorCell = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CellType == CellType.Room)
            {
                var currentCellType = RoomManager.Instance.CurrentCellType;
                var currentRoomIndex = RoomManager.Instance.CurrentRoomNumber;
                if (currentRoomIndex == RoomNumber)
                {
                    RoomManager.Instance.OpenAllDoors();
                }
                else if (!RoomManager.Instance.isTouchingCorridorCell)
                {
                    RoomManager.Instance.SetCurrentRoomNumber(RoomNumber);
                    RoomManager.Instance.SetCurrentCellType(CellType);
                    
                    SetDoors(true);
                }
            }
            else if (CellType == CellType.Corridor)
            {
                RoomManager.Instance.isTouchingCorridorCell = false;
            }
        }
    }
    
    public void SetDoors(bool state)
    {
        Doors.SetActive(state);
    }

    private void Start()
    {
        SetDoors(false);
    }
}
