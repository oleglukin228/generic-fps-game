using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt(startPosition, new Vector3Int(dungeonWidth, 0, dungeonHeight)), minRoomWidth, minRoomHeight);

        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector3Int> roomCenters = new List<Vector3Int>();
        foreach (var room in roomsList)
        {
            var roomCenter = Vector3Int.RoundToInt(room.center);
            if (roomCenter.x % 2 != 0) roomCenter.x++;
            if (roomCenter.z % 2 != 0) roomCenter.z++;
            roomCenters.Add(roomCenter);
        }

        HashSet<Vector3Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        GameObject splineGO = new GameObject("FloorEdgeSpline");
        SplineContainer container = splineGO.AddComponent<SplineContainer>();
        Spline spline = container.Spline;
        //tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer, gridSize);
        CreateFloorMesh(floor, floorMaterial, surfaceType);
        splineGO.transform.parent = floorMesh.transform;
    }

    private HashSet<Vector3Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector3Int(Mathf.RoundToInt(roomBounds.center.x), 0, Mathf.RoundToInt(roomBounds.center.z));
            if (roomCenter.x % 2 != 0) roomCenter.x++;
            if (roomCenter.x % 2 != 0) roomCenter.z++;
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) 
                    && position.z >= (roomBounds.zMin + offset) && position.z <= (roomBounds.zMax - offset))
                {
                    if (position.x % 2 != 0) continue;
                    if (position.z % 2 != 0) continue;
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector3Int> ConnectRooms(List<Vector3Int> roomCenters)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector3Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector3Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.z != destination.z)
        {
            if (destination.z > position.z)
            {
                position += Vector3Int.forward;
            }
            else if (destination.z < position.z)
            {
                position += Vector3Int.back;
            }
            if (position.z % 2 == 0) corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector3Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector3Int.left;
            }
            if (position.x % 2 == 0) corridor.Add(position);
        }
        return corridor;
    }

    private Vector3Int FindClosestPointTo(Vector3Int currentRoomCenter, List<Vector3Int> roomCenters)
    {
        Vector3Int closest = Vector3Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector3.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector3Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col += 2)
            {
                for (int row = offset; row < room.size.z - offset; row += 2)
                {
                    Vector3Int position = room.min + new Vector3Int(col, 0, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
