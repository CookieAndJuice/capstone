using System.Collections.Generic;
using PCG.Data_Structures;
using UnityEngine;

public class DelaunayTriangulation
{
    public List<Room> Rooms;
    public List<RoomCell> MidPoints;
    private int maxX;
    private int minX;
    private int maxY;
    private int minY;
    private int width;
    private int height;
    
    private Triangle superTriangle;
    private int iteration;
    public bool isDone;
    public List<Triangle> Triangles { get; private set; }
    private readonly Material defaultMaterial = new(Shader.Find("Unlit/Color")) { color = Color.green };
    
    public DelaunayTriangulation(List<Room> rooms)
    {
        Rooms = rooms;
        iteration = 0;
        
        GetMidPointsInfo();
    }

    public void GetMidPointsInfo()
    {
        MidPoints = new List<RoomCell>();
        foreach (var room in Rooms)
            MidPoints.Add(room.CenterCell);
        if (MidPoints.Count < 3) return;

        maxX = MidPoints[0].X;
        minX = MidPoints[0].X;
        maxY = MidPoints[0].Y;
        minY = MidPoints[0].Y;
        foreach (var midPoint in MidPoints)
        {
            if (midPoint.X > maxX) maxX = midPoint.X;
            if (midPoint.X < minX) minX = midPoint.X;
            if (midPoint.Y > maxY) maxY = midPoint.Y;
            if (midPoint.Y < minY) minY = midPoint.Y;
        }

        width = maxX - minX;
        height = maxY - minY;
    }

    public void AddPoint(RoomCell midPoint)
    {
        var trianglesToRemove = new List<Triangle>();

        var uniqueEdges = new List<Edge>();
        trianglesToRemove.Clear();
        
        // Debug.Log($"Triangles before adding point: {Triangles.Count}");
        foreach (var triangle in Triangles)
            if (triangle.IsPointInsideCircumcircle(midPoint))
            {
                trianglesToRemove.Add(triangle);
                foreach (var edge in triangle.Edges)
                {
                    if (uniqueEdges.Contains(edge))
                        uniqueEdges.Remove(edge);
                    else
                        uniqueEdges.Add(edge);
                }
            }
        foreach (var triangle in trianglesToRemove)
            Triangles.Remove(triangle);
            
        foreach (var edge in uniqueEdges)
            Triangles.Add(new Triangle(edge.A, edge.B, new Vertex(midPoint.X, midPoint.Y)));
        // Debug.Log($"Triangles after adding point: {Triangles.Count}");
    }    
    
    // remove triangles that contain super triangle vertices
    public void RemoveTrianglesWithSuperTriangleVertices()
    {
        var trianglesToRemove = new List<Triangle>();
        foreach (var triangle in Triangles) // can be optimized
            foreach (var vertex in superTriangle.Vertices)
                if (triangle.Vertices.Contains(vertex))
                    trianglesToRemove.Add(triangle);
        foreach (var triangle in trianglesToRemove)
            Triangles.Remove(triangle);
        // Debug.Log($"Triangles: {Triangles.Count}");
    }
    
    // get all unique edges
    public HashSet<Edge> GetDelaunayEdges()
    {
        var delaunayEdges = new HashSet<Edge>();
        foreach (var triangle in Triangles)
            foreach (var edge in triangle.Edges)
                delaunayEdges.Add(edge);
        
        return delaunayEdges;
    }
    
    public HashSet<Edge> GenerateDelaunayTriangulationInstantly()
    {
        if (iteration != 0)
            return GetDelaunayEdges();
        // Debug.Log($"Generating Delaunay Triangulation for {MidPoints.Count} midpoints");
        
        superTriangle = new Triangle(
            new Vertex(minX - width, minY - height),
            new Vertex(maxX + width, minY - height),
            new Vertex((minX + maxX) / 2.0f, maxY + height));
        Triangles = new List<Triangle> { superTriangle };
        
        foreach (var midPoint in MidPoints)
            AddPoint(midPoint);
        
        iteration = MidPoints.Count;

        RemoveTrianglesWithSuperTriangleVertices();
        isDone = true;

        return GetDelaunayEdges();     
    }
    
    public HashSet<Edge> GenerateDelaunayTriangulationPointByPoint()
    {
        if (iteration == 0)
        {
            Debug.Log($"Generating Delaunay Triangulation for {MidPoints.Count} midpoints");
            
            superTriangle = new Triangle(
                new Vertex(minX - width, minY - height),
                new Vertex(maxX + width, minY - height),
                new Vertex((minX + maxX) / 2.0f, maxY + height));
            Triangles = new List<Triangle> { superTriangle };
            
            var midPoint = MidPoints[iteration];
            Debug.Log($"Adding point #{iteration}: {midPoint}");
            AddPoint(midPoint);
            iteration++;
            
            isDone = false;
        }
        else if (iteration <= MidPoints.Count && !isDone)
        {
            var midPoint = MidPoints[iteration];
            Debug.Log($"Adding point #{iteration}: {midPoint}");
            AddPoint(midPoint);
            iteration++;
            
            if (iteration == MidPoints.Count)
            {
                RemoveTrianglesWithSuperTriangleVertices();
                isDone = true;
            }
        }
        
        return GetDelaunayEdges();
    }
    
    public void DeleteAllEdges(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
            GameObject.Destroy(child.gameObject);
    }
    
    public void DeleteNonMinimumSpanningTreeEdges(List<Edge> minimumSpanningTreeEdges, Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            var edge = child.GetComponent<EdgeMonoBehaviour>().edge;
            if (!minimumSpanningTreeEdges.Contains(edge))
                child.gameObject.SetActive(false);
        }
    }
    
    public void DisplayDelaunayEdges(HashSet<Edge> delaunayEdges, Transform parentTransform)
    {
        var Width = 100;
        var Height = 100;
        var cellSize = 4;
        var offset = new Vector3(0, 0, 0);
        var screenTopLeft = new Vector3(-Width * cellSize / 2, Height * cellSize / 2, 0);

        if (delaunayEdges == null || !parentTransform) return;

        DeleteAllEdges(parentTransform);
        
        foreach (var edge in delaunayEdges)
        {
            var startPosition = offset + new Vector3(edge.A.X * cellSize, 0, -edge.A.Y * cellSize);
            var endPosition = offset + new Vector3(edge.B.X * cellSize, 0, -edge.B.Y * cellSize);

            var lineObject = new GameObject($"Line_{edge.A}_{edge.B}");
            lineObject.transform.parent = parentTransform;
            var lineRenderer = lineObject.AddComponent<LineRenderer>();

            lineRenderer.material = defaultMaterial;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.startWidth = 0.5f;
            lineRenderer.endWidth = 0.5f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
            
            // add an Edge component to the line object
            var edgeComponent = lineObject.AddComponent<EdgeMonoBehaviour>();
            edgeComponent.edge = edge;
        }
    }
}