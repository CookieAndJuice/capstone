using System.Collections;
using System.Collections.Generic;
using PCG.Data_Structures;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Main : MonoBehaviour
{
    [Tooltip("Step by step generation of the map")] public bool stepByStep; 
    [Tooltip("Step by step generation of the Delaunay triangulation")] public bool pointByPoint;
    [Tooltip("Click to generate minimum spanning tree")] public bool vertexByVertex;
    
    public GameObject cellsParent;
    public GameObject corridorsParent;
    public GameObject groundPrototypePrefab;
    public GameObject midpointPrototypePrefab;
    public GameObject roomInfoPanelParent;
    public GameObject roomInfoPanelPrefab;
    public GameObject wallPrototypePrefab;
    public GameObject corridorPrototypePrefab;
    public GameObject edgesParent;
    
    public TextMeshProUGUI generationText;

    [Range(10, 200)] public int width = 100;
    [Range(10, 200)] public int height = 100;
    [Range(0f, 1f)] public float roomPercentage = 0.15f;
    [Range(1, 20)] public int generations = 6;
    [Range(0, 8)] public int birthLimit = 3;
    [Range(0, 8)] public int deathLimit = 1;
    [Range(0, 100)] public int roomThreshold = 30;
    
    private MapGenerator mapGenerator;
    private MapDisplayer mapDisplayer;
    private Map map;
    int currentGeneration = -1;
    
    DelaunayTriangulation delaunayTriangulation;
    HashSet<Edge> delaunayEdges;

    private List<Edge> minimumSpanningTreeEdges;
    private bool isMinimumSpanningTreeDone;

    private readonly PathFinder pathFinder = new PathFinder();

    public List<Room> RoomsWithWalls { get; private set; }
    private BattleRoomSpawner battleRoomSpawner;

    // chihoon
    [SerializeField] PressurePuzzleManager pressurePuzzleManager;

    /*
     * 100, 100, 0.35, 3, 3, 4, 25
     * 100, 100, 0.15, 6, 3, 1, 30
     * 100, 100, 0.35, 5, 4, 3, 20
     */

    private void Start()
    {
        if (stepByStep) return;
        RoomsWithWalls = new List<Room>();

        InstantlyGenerateMap();

        battleRoomSpawner = GetComponent<BattleRoomSpawner>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (stepByStep)
            {
                delaunayTriangulation = new DelaunayTriangulation(new List<Room>());
                delaunayTriangulation.DeleteAllEdges(edgesParent.transform);
                currentGeneration = 0;
                (mapGenerator, map) = InitiateMap();
                DisplayMap(null);
            }
            else
                InstantlyGenerateMap();
        }
        else if (Input.GetKeyDown(KeyCode.C) && 0 <= currentGeneration && currentGeneration < generations)
        {
            currentGeneration++;
            Debug.Log($"Generation {currentGeneration}");
            map = mapGenerator.CellularAutomata(map);
            if (currentGeneration == generations)
            {
                isMinimumSpanningTreeDone = false;
                var (wallMap, roomsWithWalls) = PostProcessMap();
                map = wallMap;
                DisplayMap(roomsWithWalls);
                
                delaunayTriangulation.DisplayDelaunayEdges(delaunayEdges, edgesParent.transform);
            }
            else 
                DisplayMap(null);
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentGeneration >= generations)
        {
            GetDelaunayEdges();
            delaunayTriangulation.DisplayDelaunayEdges(delaunayEdges, edgesParent.transform);
            isMinimumSpanningTreeDone = false;
            if (delaunayTriangulation.isDone && !vertexByVertex)
            {
                var minimumSpanningTree = new MinimumSpanningTree();
                minimumSpanningTreeEdges = minimumSpanningTree.GenerateMinimumSpanningTree(
                    delaunayEdges, delaunayTriangulation.MidPoints);
                delaunayTriangulation.DeleteNonMinimumSpanningTreeEdges(minimumSpanningTreeEdges,
                    edgesParent.transform);
                isMinimumSpanningTreeDone = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            // Debug.Log($"currentGeneration: {currentGeneration}, generations: {generations}, delaunayTriangulation.isDone: {delaunayTriangulation.isDone}, IsMinimumSpanningTreeDone: {isMinimumSpanningTreeDone}");
            if (currentGeneration >= generations &&
                delaunayTriangulation.isDone &&
                !isMinimumSpanningTreeDone)
            {
                var minimumSpanningTree = new MinimumSpanningTree();
                minimumSpanningTreeEdges = minimumSpanningTree.GenerateMinimumSpanningTree(
                    delaunayEdges, delaunayTriangulation.MidPoints);
                delaunayTriangulation.DeleteNonMinimumSpanningTreeEdges(minimumSpanningTreeEdges,
                    edgesParent.transform);
                isMinimumSpanningTreeDone = true;
            }
        }
    }

    private IEnumerator InstantlyGenerateMapCoroutine()
    {
        PlayManager.FreezePlayer();
        
        isMinimumSpanningTreeDone = false;
        (mapGenerator, map) = InitiateMap();
        currentGeneration = generations;
        for (var i = 0; i < generations; i++)
            map = mapGenerator.CellularAutomata(map);
        var (wallMap, roomsWithWalls) = PostProcessMap();
        map = wallMap;
        DisplayMap(roomsWithWalls);
    
        delaunayTriangulation.DisplayDelaunayEdges(delaunayEdges, edgesParent.transform);

        if (vertexByVertex) yield break;
    
        var minimumSpanningTree = new MinimumSpanningTree();
        minimumSpanningTreeEdges = minimumSpanningTree.GenerateMinimumSpanningTree(delaunayEdges,
            delaunayTriangulation.MidPoints);
        delaunayTriangulation.DeleteNonMinimumSpanningTreeEdges(minimumSpanningTreeEdges, edgesParent.transform);
    
        // Wait for one frame to ensure walls are properly initialized
        yield return null;

        var paths = pathFinder.BreakWalls(minimumSpanningTreeEdges, roomsWithWalls);
        
        map = pathFinder.FindPath(map, paths);
        if (map == null)
        {
            StartCoroutine(InstantlyGenerateMapCoroutine());
            yield break;
        }
        
        RoomsWithWalls = roomsWithWalls;
        mapDisplayer.DisplayCorridors(map);

        RoomManager.Instance.AdjacencyList = minimumSpanningTree.GenerateAdjacencyList(minimumSpanningTreeEdges,
            delaunayTriangulation.MidPoints);

        foreach (var room in RoomsWithWalls)
        {
            mapDisplayer.AddRoomCellObject(room);
            room.CalculateRelativeCoordinates();
        }

        PlayManager.PlayerSpawn();
        RoomManager.Instance.SetCurrentCellType(CellType.Room);
        RoomManager.Instance.LogAdjacencyList();
        RoomManager.Instance.InitializeRoomParents();
        
        SpawnRooms();
    }
    
    void SpawnRooms()
    {
        string roomTypes = "";
        MazeManager mazeManager = new MazeManager();
        foreach (var room in RoomsWithWalls)
        { 
            var roomIndex = room.RoomNumber;
            string roomType;
            
            room.Type = Random.Range(0, 2) == 0 ? RoomType.Battle : RoomType.Puzzle;
            // room.Type = RoomType.Battle;

            if (room.RoomNumber == 0)
                continue;
            
            if (room.Type == RoomType.Puzzle)
            {
                // do not spawn maze puzzles for rooms with only one door
                if (RoomManager.Instance.AdjacencyList[room.RoomNumber].Count == 1)
                {
                    battleRoomSpawner.SpawnBattleRoom(room);
                    roomType = "battle";
                }
                else
                {
                    mazeManager.SpawnMaze(room);
                    roomType = "puzzle";
                }
                // Debug.Log("skipping puzzle room");
            }
            else
            {
                battleRoomSpawner.SpawnBattleRoom(room);
                roomType = "battle";
                // Debug.Log("skipping battle room");
            }
            
            roomTypes += $"Room {roomIndex}: {roomType}\n";
        }
        Debug.Log(roomTypes);
    }

    private void InstantlyGenerateMap()
    {
        StartCoroutine(InstantlyGenerateMapCoroutine());
    }

    private (MapGenerator, Map) InitiateMap()
    {
        mapGenerator = new MapGenerator(width,
            height,
            roomPercentage,
            generations,
            birthLimit,
            deathLimit,
            roomThreshold);
        var rawMap = mapGenerator.GenerateMap();
        
        return (mapGenerator, rawMap);
    }
    
    private (Map, List<Room>) PostProcessMap()
    {
        Map roomMap;
        List<Room> rooms;
        (roomMap, rooms) = mapGenerator.GenerateRooms(map);

        Map wallMap;
        List<Room> roomsWithWalls;
        (wallMap, roomsWithWalls) = mapGenerator.GenerateWalls(roomMap, rooms);
        
        delaunayTriangulation = new DelaunayTriangulation(roomsWithWalls);
        GetDelaunayEdges();

        RoomsWithWalls = roomsWithWalls;
        return (wallMap, roomsWithWalls);
    }
    
    private void GetDelaunayEdges()
    {
        delaunayEdges = pointByPoint 
            ? delaunayTriangulation.GenerateDelaunayTriangulationPointByPoint() 
            : delaunayTriangulation.GenerateDelaunayTriangulationInstantly();
    }

    private void DisplayMap(List<Room> rooms)
    {
        mapDisplayer = new MapDisplayer(mapGenerator.Width,
            mapGenerator.Height,
            cellsParent,
            corridorsParent,
            groundPrototypePrefab,
            midpointPrototypePrefab,
            roomInfoPanelParent,
            roomInfoPanelPrefab,
            wallPrototypePrefab,
            corridorPrototypePrefab,
            generationText);
        mapDisplayer.RemoveRoomInfoAndCells();

        mapDisplayer.LogMap(map, currentGeneration);
        if (rooms != null) mapDisplayer.LogRoomInfo(rooms);
    }

}