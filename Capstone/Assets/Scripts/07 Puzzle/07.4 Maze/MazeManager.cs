using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MazeManager
{
    private MazeGenerate mazeGenerate;
    private ConvertToMap convertToMap;

    // public static Action<int[,], Room> mapGenerate;
    public static Action<Room, List<List<MazeCell>>> AGenerateMaze;   // , (int, int)[]

    // we need to select room for maze & bring it here
    private int[,] mazeArray;               // only room inside
    List<List<MazeCell>> maze;
    private List<(int x, int y)> entrances;
    private int mazeWidth;
    private int mazeHeight;

    private void InitializeMaze(Room room)
    {
        convertToMap = new ConvertToMap();
        mazeGenerate = new MazeGenerate();
        maze = new List<List<MazeCell>>();
        entrances = new List<(int x, int y)>();

        mazeHeight = room.Height + 2;
        mazeWidth = room.Width + 2;

        // initialize mazeArray & maze
        mazeArray = new int[mazeHeight, mazeWidth];
        for (int h = 0; h < mazeHeight; h++)
        {
            maze.Add(new List<MazeCell>());

            for (int w = 0; w < mazeWidth; w++)
            {
                mazeArray[h, w] = -1;
                maze[h].Add(new MazeCell((w + room.X, h + room.Y)));
            }
        }
    }

    public void SpawnMaze(Room room)
    {
        // 이전 알고리즘 문제점
        // 1. 전달받은 자료구조가 벽부터 시작하므로 이와 관련된 부분 수정
        // 2. 알고리즘이 작동하지 않는 경우가 있다는 점
        //      - 따라서 알고리즘의 탐색 방식을 수정해야 함
        // 3. 시작점을 room 내부의 칸으로 옮겨야 함. 아마 0번 인덱스부터 탐색해서 처음으로 나오는 칸을 선택하면 될 듯
        //      - entrances[0]을 시작점으로 하면 좋을 듯
        // -----------------------------------------------------

        // 계획 수정
        // - 그냥 한 칸마다 특정 형태의 칸을, 특정 방향으로 배치하는 방법으로 진행.

        InitializeMaze(room);
        convertToMap.ConvertRoomDataStructure(room, ref mazeArray);
        convertToMap.DebugPrintMap(mazeArray);

        FindDoors();

        // Debug.Log("Generate Maze!");
        
        // (int, int)[] buttons = new (int, int)[4] { (0, 0), (0, 0), (0, 0), (0, 0) };
        maze = mazeGenerate.GenerateMaze(maze, mazeArray, entrances, room);
        // Debug.Log($"nowallflag : {maze[entrances[0].y][entrances[0].x].noWallFlag}");

        // 일단 버튼은 없는 걸로
        // buttons = ((int, int)[])mazeGenerate.SelectInteractiveButtons(maze, mazeArray).Clone();
        AGenerateMaze(room, maze); //, buttons);

        // Debug.Log("Maze Generation Complete!");
    }

    private void FindDoors()
    {
        for (int h = 0;  h < mazeHeight; h++)
        {
            for (int w = 0;  w < mazeWidth; w++)
            {
                if (mazeArray[h, w] == 3)
                    entrances.Add((x: w, y: h));
            }
        }
    }
}
