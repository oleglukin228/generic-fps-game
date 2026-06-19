using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class SimpleRandomWalkMapGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected int gridSize = 2;
    [SerializeField] protected SimpleRandomWalkSO randomWalkParameters;
    [SerializeField] protected Material floorMaterial;
    [SerializeField] protected SurfaceType surfaceType;
    protected GameObject floorMesh;

    void Awake()
    {
        RunProceduralGeneration();
    }

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector3Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.PaintFloorTiles(floorPositions);

        GameObject splineGO = new GameObject("FloorEdgeSpline");
        SplineContainer container = splineGO.AddComponent<SplineContainer>();
        Spline spline = container.Spline;
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, gridSize);
    }

    protected HashSet<Vector3Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector3Int position)
    {
        var currentPosition = position;
        HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

    protected virtual void CreateFloorMesh(HashSet<Vector3Int> floorPositions, Material material, SurfaceType surfaceType)
    {
        HashSet<Vector3Int> floorPositionsCopy = new HashSet<Vector3Int>();
        floorPositionsCopy.UnionWith(floorPositions);
        floorMesh = null;
        floorMesh = new GameObject("FloorMesh");
        floorMesh.transform.parent = tilemapVisualizer.floorMeshParent;
        MeshFilter meshFilter = floorMesh.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = floorMesh.AddComponent<MeshRenderer>();
        SurfaceIdentifier meshSurface = floorMesh.AddComponent<SurfaceIdentifier>();
        Mesh potentialMesh = MeshGenerator.GenerateGreedyMesh(floorPositionsCopy, 1, gridSize); // 1 — высота/размер тайла, подставьте ваше значение
        meshFilter.mesh = potentialMesh;
        meshRenderer.material = material;
        meshSurface.SurfaceType = surfaceType;
        floorMesh.AddComponent<MeshCollider>();
    }

    protected override void RunClearDungeon()
    {
        DestroyImmediate(floorMesh);

        if (tilemapVisualizer.floorMeshParent.childCount != 0)
            for (int i = tilemapVisualizer.floorMeshParent.childCount - 1; i >= 0; i--)
                DestroyImmediate(tilemapVisualizer.floorMeshParent.GetChild(i).gameObject);
    }
}
