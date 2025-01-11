using System.Collections.Generic;
using PCG.Data_Structures;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float RoomPercentage { get; private set; }
    public int Generations { get; private set; }
    public int BirthLimit { get; private set; }
    public int DeathLimit { get; private set; }
    public int RoomThreshold { get; private set; }

    public MapGenerator(int width,
        int height,
        float roomPercentage,
        int generations,
        int birthLimit,
        int deathLimit,
        int roomThreshold)
    {
        Width = width;
        Height = height;
        RoomPercentage = roomPercentage;
        Generations = generations;
        BirthLimit = birthLimit;
        DeathLimit = deathLimit;
        RoomThreshold = roomThreshold;
    }

    public Map GenerateMap()
    {
        // create a new map
        var rawMap = new Map(Width, Height);

        // choose random room cells and set them to room type
        var totalCells = Width * Height;
        var roomCells = (int)(totalCells * RoomPercentage);
        while (roomCells > 0)
        {
            var x = Random.Range(1, Width - 1);
            var y = Random.Range(1, Height - 1);
            if (rawMap.Cells[y, x].Type == CellType.Room) continue;

            rawMap.Cells[y, x].Type = CellType.Room;
            roomCells--;
        }

        return rawMap;
    }
    
    public (Map, List<Room>) GenerateRooms(Map map)
    {
        var rooms = new List<Room>();
        var roomNumber = 0;
        for (var y = 1; y < Height - 1; y++)
            for (var x = 1; x < Width - 1; x++)
            {
                if (map.Cells[y, x].Type != CellType.Room || map.Cells[y, x] is RoomCell) continue;
                
                var room = LabelRoomWithBFS(x, y, map, roomNumber);
                if (room.RoomCells.Count < RoomThreshold)
                    foreach (var roomCell in room.RoomCells)
                        map.Cells[roomCell.Y, roomCell.X].Type = CellType.Blank;
                else
                {
                    room.CalculateRoomInfo();
                    rooms.Add(room);
                    roomNumber++;
                }
            }
        
        return (map, rooms);
    }

    // use a BFS algorithm to label each room cell with a unique room number
    private Room LabelRoomWithBFS(int startX, int startY, Map map, int roomNumber)
    {
        var room = new Room(roomNumber);
        var visited = new bool[Height][];
        for (int index = 0; index < Height; index++)
            visited[index] = new bool[Width];

        var queue = new Queue<Cell>();
        
        visited[startY][startX] = true;
        queue.Enqueue(map.Cells[startY, startX]);
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            var roomCell = new RoomCell(cell.X, cell.Y, CellType.Room, roomNumber);
            map.Cells[cell.Y, cell.X] = roomCell;
            room.AddCell(roomCell);

            var neighbours = GetNeighbours(map, cell.X, cell.Y, adjacent: true);
            foreach (var neighbour in neighbours)
                if (neighbour.Type == CellType.Room && !visited[neighbour.Y][neighbour.X])
                {
                    if (IsEdgeCellPosition(neighbour.X, neighbour.Y))
                        map.Cells[neighbour.Y, neighbour.X].Type = CellType.Blank;
                    else {
                        queue.Enqueue(neighbour);
                        visited[neighbour.Y][neighbour.X] = true;
                    }
                }
        }
        
        return room;
    }

    public Map CellularAutomata(Map map)
    {
        var birthCount = 0;
        var deathCount = 0;
        
        for (var y = 1; y < Height - 1; y++)
            for (var x = 1; x < Width - 1; x++)
            {
                var neighbours = GetNeighbours(map, x, y);
                var roomNeighbours = neighbours.FindAll(neighbour => neighbour.Type == CellType.Room);
                var roomNeighboursCount = roomNeighbours.Count;
                switch (map.Cells[y, x].Type)
                {
                    case CellType.Blank:
                    {
                        if (roomNeighboursCount > BirthLimit)
                        {
                            map.Cells[y, x].Type = CellType.Room;
                            birthCount++;
                        }
                        break;
                    }
                    case CellType.Room:
                    {
                        if (roomNeighboursCount < DeathLimit)
                        {
                            map.Cells[y, x].Type = CellType.Blank;
                            deathCount++;
                        }
                        break;
                    }
                    case CellType.Wall:
                    case CellType.Corridor:
                    default:
                        break;
                }
            }
        
        // Debug.Log($"Births: {birthCount}, Deaths: {deathCount}");
        return map;
    }

    public (Map, List<Room>) GenerateWalls(Map map, List<Room> rooms)
    {
        foreach (var room in rooms)
        {
            var uniqueNeighbours = new HashSet<Cell>();
            foreach (var roomCell in room.RoomCells)
            {
                var neighbours = GetNeighbours(map, roomCell.X, roomCell.Y, adjacent:true);
                foreach (var neighbour in neighbours)
                    if (neighbour.Type == CellType.Blank)
                        uniqueNeighbours.Add(neighbour);
            }

            foreach (var neighbour in uniqueNeighbours)
            {
                map.Cells[neighbour.Y, neighbour.X].Type = CellType.Wall;
                room.AddCell(map.Cells[neighbour.Y, neighbour.X]);
            }
        }

        return (map, rooms);
    }

    private bool IsValidCellPosition(int x, int y) // check if a given cell position is valid
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

    private bool IsEdgeCellPosition(int x, int y)
    {
        return IsValidCellPosition(x, y) && (x == 0 || x == Width - 1 || y == 0 || y == Height - 1);
    }
    
    private List<Cell> GetNeighbours(Map map, int x, int y, bool adjacent=false) // get the room cells around a given cell
    {
        var neighbours = new List<Cell>();
        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
            {
                if ((i == 0 && j == 0) || (adjacent && i * j != 0)) 
                    continue;

                var neighbourX = x + i;
                var neighbourY = y + j;
                if (IsValidCellPosition(neighbourX, neighbourY))
                    neighbours.Add(map.Cells[neighbourY, neighbourX]);
            }

        return neighbours;
    }
}
