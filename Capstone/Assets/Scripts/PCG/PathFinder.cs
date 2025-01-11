using System.Collections.Generic;
using PCG.Data_Structures;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    private readonly int cellSize = 4;
    
    // receive the MST edges and shoot ray from the center of each room to the center of the other room for each edge
    // if the ray hits a wall, delete the wall
    public List<Path> BreakWalls(List<Edge> minimumSpanningTreeEdges, List<Room> roomsWithWalls)
    {
        var paths = new List<Path>();
        foreach (var edge in minimumSpanningTreeEdges)
        {
            var deletedVertexA = DeleteFirstWall(edge.A, edge.B);
            var deletedVertexB = DeleteFirstWall(edge.B, edge.A);
            paths.Add(new Path(deletedVertexA, deletedVertexB)); 
            
            // change the room type of the deleted cell to corridor
            var centerCellA = new RoomCell((int)edge.A.X, (int)edge.A.Y);
            var centerCellB = new RoomCell((int)edge.B.X, (int)edge.B.Y);
            var deletedCellA = new Cell((int)deletedVertexA.X, (int)deletedVertexA.Y, CellType.Wall);
            var deletedCellB = new Cell((int)deletedVertexB.X, (int)deletedVertexB.Y, CellType.Wall);
            
            var flagA = false;
            var flagB = false;
            foreach (var room in roomsWithWalls)
            {
                if (flagA && flagB) break;

                if (!flagA && room.CenterCell.Equals(centerCellA))
                {
                    room.DeleteCell(deletedCellA);
                    deletedCellA.Type = CellType.Corridor;
                    room.AddCell(deletedCellA);
                    flagA = true;
                    
                    // Debug.Log($"Deleted wall A: {deletedCellA}");
                }

                if (!flagB && room.CenterCell.Equals(centerCellB))
                {
                    room.DeleteCell(deletedCellB);
                    deletedCellB.Type = CellType.Corridor;
                    room.AddCell(deletedCellB);
                    flagB = true;
                    
                    // Debug.Log($"Deleted wall B: {deletedCellB}");
                }
            }
        }
        
        return paths;
    }

    private Vertex DeleteFirstWall(Vertex a, Vertex b)
    {
        Vertex deletedVertex = null;
        var room1Center = new Vector3(a.X * cellSize, cellSize, -a.Y * cellSize);
        var room2Center = new Vector3(b.X * cellSize, cellSize, -b.Y * cellSize);

        var direction = room2Center - room1Center;
        var distance = direction.magnitude;
        var ray = new Ray(room1Center, direction);
        // Debug.DrawRay(room1Center, direction, Color.red, 1000f);
        if (Physics.Raycast(ray, out var hit, distance) && hit.collider.CompareTag("Wall"))
        {
            // Debug.Log($"Destroying wall: {hit.collider.gameObject.name}");
            Object.Destroy(hit.collider.gameObject);
            deletedVertex = hit.transform.GetComponent<VertexMonoBehaviour>().vertex;
        }

        // Debug.Log($"Deleted vertex: {deletedVertex}");
        return deletedVertex;
    }
    
    // do an A* search between the vertices of each path
    public Map FindPath(Map map, List<Path> paths)
    {
        List<Cell> pathCells = null;
        
        foreach (var path in paths)
        {
            // first, remove the walls that were deleted in the previous step
            var startVertex = path.A;
            var endVertex = path.B;
            var startCell = map.Cells[(int)startVertex.Y, (int)startVertex.X];
            var endCell = map.Cells[(int)endVertex.Y, (int)endVertex.X];
            
            map.DeleteCell(startCell);
            map.DeleteCell(endCell);
            
            // then, do an A* search between the vertices of each path
            pathCells = AStarSearch(map, startCell, endCell);
            
            if (pathCells.Count == 0)
            {
                Debug.LogWarning($"No path found between {startCell} and {endCell}!");
                return null;
            }
            
            // Debug.Log($"Finding path between {startCell} and {endCell}");
            foreach (var cell in pathCells)
            {
                cell.Type = CellType.Corridor;
                // Debug.Log($"Path Cell: {cell}");
            }
        }

        return map;
    }

    private List<Cell> AStarSearch(Map map, Cell startCell, Cell endCell)
    {
        var openSet = new List<Cell> { startCell };
        var cameFrom = new Dictionary<Cell, Cell>();

        var gScore = new Dictionary<Cell, float>();
        gScore[startCell] = 0;

        var fScore = new Dictionary<Cell, float>();
        fScore[startCell] = Heuristic(startCell, endCell);

        var closedSet = new HashSet<Cell>();

        while (openSet.Count > 0)
        {
            // Get the cell with the lowest fScore
            openSet.Sort((cell1, cell2) => fScore[cell1].CompareTo(fScore[cell2]));
            var current = openSet[0];

            if (current.Equals(endCell))
            {
                // Reconstruct and return the path
                return ReconstructPath(cameFrom, current);
            }

            openSet.RemoveAt(0);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(map, current))
            {
                if (closedSet.Contains(neighbor) || neighbor.Type == CellType.Wall)
                    continue;

                var tentativeGScore = gScore[current] + 1; // Distance between neighbors is 1

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endCell);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // Return an empty path if no path is found
        return new List<Cell>();
    }

    private List<Cell> GetNeighbors(Map map, Cell cell)
    {
        var neighbors = new List<Cell>();
        var directions = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            int neighborX = cell.X + dx;
            int neighborY = cell.Y + dy;

            if (neighborX >= 0 && neighborX < map.Width && neighborY >= 0 && neighborY < map.Height)
            {
                var neighbor = map.Cells[neighborY, neighborX];
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
    {
        var totalPath = new List<Cell> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }


    // manhattan distance as heuristic
    private float Heuristic(Cell startCell, Cell endCell)
    {
        return Mathf.Abs(startCell.X - endCell.X) + Mathf.Abs(startCell.Y - endCell.Y);
    }
}
