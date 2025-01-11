using PCG.Data_Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConvertToMap
{
    public void ChoiceMazeRoom()
    {
        // Room�� ����Ʈ ��ü�� �޾Ƽ� �̷ΰ� �� ���� room�� ���� �ϳ�?
    }

    public void ConvertRoomDataStructure(Room inRoom, ref int[,] roomArray)
    {
        foreach (var roomCell in inRoom.RoomCellsRelative)
            roomArray[roomCell.Y, roomCell.X] = 0;

        foreach (var Cell in inRoom.CorridorCellsRelative)
            roomArray[Cell.Y, Cell.X] = 3;
    }

    public void ConvertWallDataStructure(Room inRoom, ref int[,] wallArray)
    {
        foreach (var Cell in inRoom.WallCellsRelative)
            wallArray[Cell.Y, Cell.X] = 1;
    }

    public void DebugPrintMap<T>(T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        
        string output = "[2D Array Contents]:\n";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                output += array[i, j] + "\t";
            }
            output += "\n";
        }
        // Debug.Log(output);
    }
}
