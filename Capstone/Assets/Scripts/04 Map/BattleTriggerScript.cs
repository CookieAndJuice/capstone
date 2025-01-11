using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTriggerScript : MonoBehaviour
{
    private BattleRoomSpawner battleRoomSpawner;
    private Room curRoom;

    private void OnTriggerExit(Collider _other)
    {
        // Debug.Log(RoomManager.Instance.CurrentCellType);
        
        if (_other.CompareTag(ValueDefinition.PLAYER_TAG) &&
            !curRoom.IsBattleStarted &&
            RoomManager.Instance.CurrentCellType == CellType.Room)
        {
            PlayManager.MonsterNum = 0;
            battleRoomSpawner.ActivateBattleObject(curRoom);
            StartBattle();
        }
    }

    public void SetBattleRoom(BattleRoomSpawner _battleRoom, Room _room)
    {
        battleRoomSpawner = _battleRoom;
        curRoom = _room;
    }

    public void StartBattle()
    {
        RoomManager.Instance.CloseAllDoors();
        
        curRoom.IsBattleStarted = true;
        PlayManager.ShowBattleUI();
        PlayManager.SetBattleInfo(curRoom);
        PlayManager.StartTimer(curRoom);
    }
}
