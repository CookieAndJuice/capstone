using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    // path order : up, left, down, right
    private bool[] pathFlags;
    public bool noWallFlag;
    public int cellType;

    public (int x, int y) globalPosition { get; private set; }

    public Quaternion quaternion;

    public MazeCell((int x, int y) pos)
    {
        this.pathFlags = new bool[4];
        for (int i = 0; i < 4; i++)
            this.pathFlags[i] = false;

        noWallFlag = true;
        globalPosition = pos;
        cellType = 0;
    }

    public void setPathFlagTrue(int directionIndex)
    {
        if (0 <= directionIndex && directionIndex < 4)
        {
            pathFlags[directionIndex] = true;
            noWallFlag = false;
            // Debug.Log($"no wall flag {noWallFlag}");
        }
        else
            Debug.Log($"position {globalPosition.x}, setPathFlagTrue, {globalPosition.y} - Index Out of Range!");
    }

    public bool getPathFlag(int directionIndex)
    {
        if (0 <= directionIndex && directionIndex < 4)
            return pathFlags[directionIndex];
        else
        {
            Debug.Log($"position {globalPosition.x}, getPathFlag, {globalPosition.y} - Index Out of Range!");
            return false;
        }
    }

    public int getRandomFlagIndex()
    {
        int index = 0;
        List<int> array = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (pathFlags[i])
                array.Add(i);
        }

        index = array[Random.Range(0, array.Count)];

        return index;
    }
}
