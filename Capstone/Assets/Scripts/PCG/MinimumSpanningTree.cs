using System;
using System.Collections.Generic;
using System.Linq;
using PCG.Data_Structures;
using UnityEngine;

public class MinimumSpanningTree
{
    public List<Edge> GenerateMinimumSpanningTree(HashSet<Edge> delaunayEdges, List<RoomCell> midPoints)
    {
        if (delaunayEdges.Count == 0) return new List<Edge>();
        
        var adjacencyList = new Dictionary<Vertex, List<Tuple<Vertex, float>>>();
        foreach (var edge in delaunayEdges)
        {
            if (!adjacencyList.ContainsKey(edge.A))
                adjacencyList[edge.A] = new List<Tuple<Vertex, float>>();
            if (!adjacencyList.ContainsKey(edge.B))
                adjacencyList[edge.B] = new List<Tuple<Vertex, float>>();
            
            var edgeDistanceSquared = GetEdgeDistanceSquared(edge);
            adjacencyList[edge.A].Add(new Tuple<Vertex, float>(edge.B, edgeDistanceSquared));
            adjacencyList[edge.B].Add(new Tuple<Vertex, float>(edge.A, edgeDistanceSquared));
        }
        
        var minimumSpanningTree = new List<Edge>();
        var visitedVertices = new HashSet<Vertex>();
        var distanceSquared = new Dictionary<Vertex, float>();
        var parent = new Dictionary<Vertex, Vertex>();
        
        foreach (var vertex in adjacencyList.Keys)
        {
            distanceSquared[vertex] = Mathf.Infinity;
            parent[vertex] = null;
        }

        var startVertex = adjacencyList.Keys.First();
        distanceSquared[startVertex] = 0;
        
        while (visitedVertices.Count < midPoints.Count)
        {
            var minDistance = Mathf.Infinity;
            Vertex minVertex = null;
            foreach (var vertex in adjacencyList.Keys)
                if (distanceSquared[vertex] < minDistance && !visitedVertices.Contains(vertex))
                {
                    minDistance = distanceSquared[vertex];
                    minVertex = vertex;
                }
            if (minVertex == null) 
                break;
            
            visitedVertices.Add(minVertex);
            if (parent[minVertex] != null)
                minimumSpanningTree.Add(new Edge(parent[minVertex], minVertex));
            
            foreach (var (adjacentVertex, dSquared) in adjacencyList[minVertex])
                if (dSquared < distanceSquared[adjacentVertex] && !visitedVertices.Contains(adjacentVertex))
                {
                    distanceSquared[adjacentVertex] = dSquared;
                    parent[adjacentVertex] = minVertex;
                }
        }
        
        // Debug.Log($"number of edges in minimum spanning tree: {minimumSpanningTree.Count}");
        return minimumSpanningTree;
    }
    
    private static float GetEdgeDistanceSquared(Edge edge)
    {
        return Mathf.Pow(edge.A.X - edge.B.X, 2) + Mathf.Pow(edge.A.Y - edge.B.Y, 2);
    }
    
    public Dictionary<int, List<int>> GenerateAdjacencyList(List<Edge> minimumSpanningTree, List<RoomCell> midPoints)
    {
        // create a dictionary to store the midPoint cells' coordinates
        var midPointVerticesDictionary = new Dictionary<Vertex, int>();
        foreach (var midPoint in midPoints)
            midPointVerticesDictionary[new Vertex(midPoint.X, midPoint.Y)] = midPoint.RoomNumber;
        
        var adjacencyList = new Dictionary<int, List<int>>();

        // Initialize adjacency list with empty lists for all midPoints
        for (int i = 0; i < midPoints.Count; i++)
            adjacencyList[i] = new List<int>();

        // Build the adjacency list from MST edges
        foreach (var edge in minimumSpanningTree)
        {
            var indexA = midPointVerticesDictionary[edge.A];
            var indexB = midPointVerticesDictionary[edge.B];

            adjacencyList[indexA].Add(indexB);
            adjacencyList[indexB].Add(indexA);
        }

        return adjacencyList;
    }
}
