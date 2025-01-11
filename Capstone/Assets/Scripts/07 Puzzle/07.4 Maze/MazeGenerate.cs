using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Sentis.Model;

public class MazeGenerate
{
    private enum EDirection { NONE = -1, UP, LEFT, DOWN, RIGHT }

    public List<List<MazeCell>> GenerateMaze (List<List<MazeCell>> maze, int[,] map, List<(int, int)> entrances, Room room)
    {
        // 상 좌 하 우, 탐색 방향 순서
        int[] dx = { 0, -1, 0, 1 };
        int[] dy = { -1, 0, 1, 0 };
        EDirection[] directionOrder = { EDirection.UP, EDirection.LEFT, EDirection.DOWN, EDirection.RIGHT };

        // 미로 가로, 세로 길이
        int mazeWidth = map.GetLength(1);
        int mazeHeight = map.GetLength(0);

        int[,] visited = new int[mazeHeight, mazeWidth];

        // 미로 생성 시작점
        (int x, int y) startPos = entrances[0];
        // Debug.Log($"start : {startPos}");

        // 좌표 임시 저장할 변수
        var nowPos = (x: 0, y: 0);

        // DFS base
        var recursiveStack = new Stack<(int x, int y)>();
        recursiveStack.Push(startPos);
        visited[startPos.y, startPos.x] = 1;

        // Recursive Backtracking
        while(recursiveStack.Count != 0)
        {
            nowPos = recursiveStack.Pop();          // 현재 위치
            visited[nowPos.y, nowPos.x] = 1;

            ShuffleArray(directionOrder);           // 탐색 방향 순서 섞기

            for (int i = 0; i < 4; i++)
            {
                var visitDir = directionOrder[i];
                (int x, int y) visitPos = (nowPos.x + dx[(int)visitDir], nowPos.y + dy[(int)visitDir]);

                // 탐색 중인 칸이 맵 범위 내에 있는지 확인
                if (0 <= visitPos.x && 0 <= visitPos.y && visitPos.x < mazeWidth && visitPos.y < mazeHeight)
                {
                    // blank면 무시
                    if (map[visitPos.y, visitPos.x] == -1)
                        continue;

                    // 방문 여부 확인
                    if (visited[visitPos.y, visitPos.x] == 0)
                    {
                        // push nowPos for backtracking
                        recursiveStack.Push(nowPos);
                        recursiveStack.Push(visitPos);

                        maze[nowPos.y][nowPos.x].setPathFlagTrue((int)visitDir);
                        maze[visitPos.y][visitPos.x].setPathFlagTrue((int)returnOppositeDirection(visitDir));
                        break;
                    }
                }
            }
        }

        // CreateFloorAroundEntrance(ref map, entrances);

        return maze;
    }

    private void CreateFloorAroundEntrance(ref int[,] maze, List<(int, int)> entrances)
    {
        int mazeWidth = maze.GetLength(1);
        int mazeHeight = maze.GetLength(0);

        // 각 문 출입구 근처 길 뚫어주기
        foreach ((int x, int y) entrancePos in entrances)
        {
            if (entrancePos.y % 2 == 0 && entrancePos.x % 2 == 0)
            {
                var visitPos = (x: 0, y: 0);

                if (entrancePos.x == (mazeWidth - 1))
                    visitPos = (entrancePos.x - 1, entrancePos.y);
                else if (entrancePos.y == (mazeHeight - 1))
                    visitPos = (entrancePos.x, entrancePos.y - 1);

                if (maze[visitPos.y, visitPos.x] != 3)
                    maze[visitPos.y, visitPos.x] = 0;
            }
        }
    }

    public (int, int)[] SelectInteractiveButtons(List<List<MazeCell>> maze, int[,] mazeArray)
    {
        (int, int)[] buttons = new (int, int)[4];
        int tempX = 0;
        int tempY = 0;
        var mazeWidth = maze[0].Count;
        var mazeHeight = maze.Count;

        // 버튼 배치 방식 변경해야 함
        // Map의 구역을 4개로 나눠서 구역마다 버튼을 배치해야 함
        (int x, int y)[] maxList = { (mazeWidth/2, mazeHeight/2), (mazeWidth, mazeHeight/2), (mazeWidth/2, mazeHeight), (mazeWidth, mazeHeight) };
        (int x, int y)[] minList = { (0, 0), (mazeWidth/2 + 1, 0), (0, mazeHeight/2 + 1), (mazeWidth/ 2 + 1, mazeHeight/ 2 + 1) };

        string output = null;
        for (int i = 0; i < 4; i++)
        {
            output = null;

            for (int j = 0; j < 10; j++)
            {
                tempX = Random.Range(minList[i].x, maxList[i].x);
                tempY = Random.Range(minList[i].y, maxList[i].y);

                output += maze[tempY][tempX].noWallFlag + "\t";

                if (maze[tempY][tempX].noWallFlag == false)
                    break;
            }

            Debug.Log($"maze buttons {i}" + output);

            // 여기가 문제
            //while (true)
            //{
            //    tempX = Random.Range(minList[i].x, maxList[i].x);
            //    tempY = Random.Range(minList[i].y, maxList[i].y);

            //    if (!maze[tempY][tempX].noWallFlag)
            //        break;
            //}
            buttons[i] = (tempX, tempY);
        }

        return buttons;
    }

    private bool CheckInRange(int[,] maze, (int x, int y) nowPos)
    {
        bool inRangeFlag = true;
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            (int x, int y) tempPos = (nowPos.x + dx[i], nowPos.y + dy[i]);
            if (tempPos.x < 0 || tempPos.y < 0 || tempPos.x >= maze.GetLength(1) || tempPos.y >= maze.GetLength(0))
            {
                inRangeFlag = false;
                break;
            }
        }

        return inRangeFlag;
    }

    // Fisher-Yates Shuffle
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            var randomIndex = Random.Range(i, array.Length);
            Swap(array, i, randomIndex);
        }
    }

    private void Swap<T>(T[] array, int a, int b)
    {
        var temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }

    private EDirection returnOppositeDirection(EDirection dir)
    {
        switch (dir)
        {
            case EDirection.LEFT:
                return EDirection.RIGHT;
            case EDirection.RIGHT:
                return EDirection.LEFT;
            case EDirection.UP:
                return EDirection.DOWN;
            case EDirection.DOWN:
                return EDirection.UP;
            default: throw new System.ArgumentOutOfRangeException();
        }
    }
}
