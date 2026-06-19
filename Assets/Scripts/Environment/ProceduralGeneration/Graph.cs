using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph 
{
    private static List<Vector3Int> neighbourse4directions = new List<Vector3Int>
    {
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(-1, 0, 0)
    };

    private static List<Vector3Int> neighbourse8directions = new List<Vector3Int>
    {
        new Vector3Int(0, 0, 3), // UP
        new Vector3Int(3, 0, 0), // RIGHT
        new Vector3Int(0, 0, -3), // DOWN
        new Vector3Int(-3, 0, 0), // LEFT
        new Vector3Int(3, 0, 3), // Diagonal
        new Vector3Int(3, 0, -3), // Diagonal
        new Vector3Int(-3, 0, 3), // Diagonal
        new Vector3Int(-3, 0, -3) // Diagonal
    };

    List<Vector3Int> graph;

    public Graph(IEnumerable<Vector3Int> vertices)
    {
        graph = new List<Vector3Int>(vertices);
    }

    public List<Vector3Int> GetNeighbours4Directions(Vector3Int startPosition)
    {
        return GetNeighbours(startPosition, neighbourse4directions);
    }

    public List<Vector3Int> GetNeighbours8Directions(Vector3Int startPosition)
    {
        return GetNeighbours(startPosition, neighbourse8directions);
    }

    private List<Vector3Int> GetNeighbours(Vector3Int startPosition, List<Vector3Int> neighboursOffsetList)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var neighbourDirection in neighboursOffsetList)
        {
            Vector3Int potentialNeighbour = startPosition + neighbourDirection;
            if (graph.Contains(potentialNeighbour))
                neighbours.Add(potentialNeighbour);
        }
        return neighbours;
    }
}
