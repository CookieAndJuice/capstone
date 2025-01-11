using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTileMapGenerate : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private List<GameObject> prefabList;

    private enum Blocks { NONE = -1, FLOOR = 0, WALL, INTERACTIVE, ENTRANCE }
    private enum EDirection { NONE = -1, UP, LEFT, DOWN, RIGHT }    

    private void Awake()
    {
        MazeManager.AGenerateMaze += GenerateMap;
    }

    // public void GenerateMap(int[,] maze, Room mazeRoom)
    public void GenerateMap(Room room, List<List<MazeCell>> maze)   // , (int x, int y)[] buttons)
    {
        // set parent : tilemap - parentObject - maze cells
        GameObject tileMap = RoomManager.Instance.RoomParents[room.RoomNumber];
        GameObject parentObject = new GameObject($"maze {room.RoomNumber}");
        parentObject.transform.parent = tileMap.transform;

        // initialize variables
        GameObject roomObject = null;
        MazeCell nowCell = null;
        int prefabIndex = -1;
        Quaternion prefabQuaternion = Quaternion.identity;

        // set maze cells
        foreach (var roomCell in room.RoomCells)
        {
            var nowPos = (x: roomCell.X - room.X, y: roomCell.Y - room.Y);
            // Debug.Log($"roomCell X,Y : {nowPos.x}, {nowPos.y}");
            (prefabQuaternion, prefabIndex) = ChoosePrefabForCell(maze[nowPos.y][nowPos.x]);
            if (prefabIndex == -1)
                continue;

            nowCell = maze[nowPos.y][nowPos.x];
            roomObject = Instantiate(prefabList[prefabIndex], parentObject.transform);
            roomObject.transform.rotation = prefabQuaternion;
            roomObject.transform.localPosition = new Vector3(roomCell.X * 4, 0, -roomCell.Y * 4);

            //for (int i = 0; i < buttons.Length; i++)
            //{
            //    if (buttons[i].x == nowPos.x && buttons[i].y == nowPos.y)
            //    {
            //        Debug.Log($"button pos : {buttons[i].x}, {buttons[i].y}");
            //        buttonObject = GameObject.Instantiate(buttonPrefab, TileMap.transform);
            //        buttonObject.transform.rotation = RotateInteractiveButton(maze[nowPos.y][nowPos.x]);
            //    }
            //}

            // Debug.Log($"name, rotation : {roomObject.name}, {roomObject.transform.eulerAngles}");
        }

        //foreach (var roomCell in room.CorridorCells)
        //{
        //    var nowPos = (x: roomCell.X - room.X, y: roomCell.Y - room.Y);
        //    // Debug.Log($"roomCell X,Y : {nowPos.x}, {nowPos.y}");
        //    (prefabQuaternion, prefabIndex) = ChoosePrefabForCell(maze[nowPos.y][nowPos.x]);
        //    if (prefabIndex == -1)
        //        continue;

        //    nowCell = maze[nowPos.y][nowPos.x];
        //    roomObject = GameObject.Instantiate(prefabList[prefabIndex], tileMap.transform);
        //    roomObject.transform.rotation = prefabQuaternion;
        //    roomObject.transform.localPosition = new Vector3(roomCell.X * 4, 0, -roomCell.Y * 4);

        //}
    }

    private Quaternion RotateInteractiveButton(MazeCell buttonCell)
    {
        Quaternion buttonQuaternion = Quaternion.identity;

        // ----------------------------------------------------
        // decide button quaternion
        // ----------------------------------------------------
        buttonQuaternion = Quaternion.Euler(new Vector3(0, buttonCell.getRandomFlagIndex() * -90, 0));

        return buttonQuaternion;
    }

    // Fisher-Yates Shuffle
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            var randomIndex = Random.Range(i, array.Length);
            // Swap
            var temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private (Quaternion, int) ChoosePrefabForCell(MazeCell mazeCell)
    {
        int prefab = -1;
        Quaternion quaternion = Quaternion.identity;

        var pathIndexStack = new Stack<EDirection>();
        var mazeWallIndexStack = new Stack<EDirection>();

        // 프리팹을 정해야 함 -> 개수
        for (int i = 0; i < 4; i++)
        {
            if (mazeCell.getPathFlag(i))
            {
                pathIndexStack.Push((EDirection)i);
            }
            else
                mazeWallIndexStack.Push((EDirection)i);
        }

        switch (pathIndexStack.Count)
        {
            case 1:
                prefab = 0;     // one-way path
                quaternion = Quaternion.Euler(new Vector3(0, (int)pathIndexStack.Pop() * -90));

                break;
            case 2:
                var direction = pathIndexStack.Pop();
                if (mazeCell.getPathFlag((int)returnOppositeDirection(direction)))
                {
                    prefab = 1;     // two-way path
                    if (direction == EDirection.LEFT || direction == EDirection.RIGHT)
                        quaternion = Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else
                {
                    prefab = 2;     // two-way corner path

                    // up, left, down : check direction + 1 (ccw 90 degree), cause direction index range is 0~4
                    if (direction != EDirection.RIGHT)
                    {
                        if (pathIndexStack.Pop() == (EDirection)(direction + 1))
                            quaternion = Quaternion.Euler(new Vector3(0, (int)direction * -90, 0));
                        else
                            quaternion = Quaternion.Euler(new Vector3(0, (int)direction * -90 + 90, 0));
                    }
                    else // right : check direction - 1 (cw 90 degree)
                    {
                        if (pathIndexStack.Pop() == direction - 1)
                            quaternion = Quaternion.Euler(new Vector3(0, (int)direction * -90 + 90, 0));
                        else
                            quaternion = Quaternion.Euler(new Vector3(0, (int)direction * -90, 0));
                    }
                }
                break;
            case 3:
                prefab = 3;     // three-way path
                quaternion = Quaternion.Euler(new Vector3(0, (int)mazeWallIndexStack.Peek() * -90, 0));

                break;
        }

        return (quaternion, prefab);
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
