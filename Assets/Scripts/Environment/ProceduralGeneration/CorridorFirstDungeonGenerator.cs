using System;
using System.Collections.Generic;
using System.Linq;
using TeoGames.Mesh_Combiner.Scripts.Combine;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] private bool increaseCorridors = true;
    [SerializeField][Range(0.1f, 1)] private float roomPercent = 0.8f;
    [SerializeField] GameObject roomDataPrefab;
    private Dictionary<Vector3Int, HashSet<Vector3Int>> roomsDictionary = new();
    private Dictionary<RoomData, HashSet<Vector3Int>> rooms = new ();
    private RoomData corridorRoom;
    private HashSet<Vector3Int> floorPositions, corridorPositions;
    private List<Color> roomColors = new List<Color>();
    [SerializeField] private bool showRoomGizmo = false, showCorridorGizmo;

    public UnityEvent<DungeonData, TilemapVisualizer> OnDungeonFloorReady;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
        DungeonData data = new DungeonData
        {
            rooms = this.rooms,
            roomsDictionary = this.roomsDictionary,
            corridorRoom = this.corridorRoom,
            corridorPositions = this.corridorPositions,
            floorPositions = this.floorPositions
        };
        OnDungeonFloorReady?.Invoke(data, tilemapVisualizer);
        GenerateNavMesh();
    }

    private void CorridorFirstGeneration()
    {
        Direction2D.ChangeGridSize(gridSize);
        RunClearDungeon();

        floorPositions = new HashSet<Vector3Int>();
        rooms = new Dictionary<RoomData, HashSet<Vector3Int>>();
        HashSet<Vector3Int> potentialRoomPositions = new HashSet<Vector3Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);

        GenerateRooms(potentialRoomPositions);
    }

    private void GenerateRooms(HashSet<Vector3Int> potentialRoomPositions)
    {
        HashSet<Vector3Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector3Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        //if (increaseCorridors) IncreaceCorridorBrush3by3();
        if (increaseCorridors) IncreaceCorridorBrush3by3();

        CreateFloorMesh(floorPositions, floorMaterial, surfaceType);
        FillGroundDecorations(9);
        HashSet<Vector3Int> corridorWalls = WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, gridSize, rooms);

        var corridorGO = Instantiate(roomDataPrefab, tilemapVisualizer.transform);
        var corridorData = corridorGO.GetComponent<RoomData>();
        WallGenerator.CreateCorridorWalls(corridorWalls, floorPositions, tilemapVisualizer, gridSize, corridorData);
    }

    public void FillGroundDecorations(int countPerTile)
    {
        /*foreach (var room in rooms)
        {
            foreach (var floorTile in room.Value)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    for (var x = 0; x < gridSize; x++)
                    {
                        int index = UnityEngine.Random.Range(0, tilemapVisualizer.floorDecorationVariants.Length);
                        Vector3 position = new Vector3(x + 0.5f, 0, y + 0.5f);
                        Quaternion rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
                        Instantiate(tilemapVisualizer.floorDecorationVariants[index], floorTile + position, rotation, room.Key.floorDecorationsParent);
                    }
                }
            }
        }*/
        foreach (var floorTile in floorPositions)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    int index = UnityEngine.Random.Range(0, tilemapVisualizer.floorDecorationVariants.Length);
                    Vector3 position = new Vector3(x + 0.5f, 0, y + 0.5f);
                    Quaternion rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
                    Instantiate(tilemapVisualizer.floorDecorationVariants[index], floorTile + position, rotation, tilemapVisualizer.floorMeshParent);
                }
            }
        }
    }

    /*public void FillGroundDecorations(int countPerTile)
    {
        foreach (var floorTile in floorPositions)
        {
            for (int i = 0; i < countPerTile; i++)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(0, gridSize), 0, UnityEngine.Random.Range(0, gridSize));
                int index = UnityEngine.Random.Range(0, tilemapVisualizer.floorDecorationVariants.Length);
                Instantiate(tilemapVisualizer.floorDecorationVariants[index], floorTile + position,
                    Quaternion.identity, tilemapVisualizer.floorMeshParent);
            }
        }
    }*/

    private void GenerateNavMesh()
    {
        tilemapVisualizer.gameObject.TryGetComponent(out NavMeshSurface navMesh);
        if (navMesh == null) navMesh = tilemapVisualizer.gameObject.AddComponent<NavMeshSurface>();
        navMesh.collectObjects = CollectObjects.All;
        navMesh.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        navMesh.minRegionArea = 0.9f;
        navMesh.layerMask = ~LayerMask.GetMask("Player", "Enemy", "Ignore NavMesh", "Hitbox", "Trigger");
        navMesh.BuildNavMesh();
    }

    private void IncreaceCorridorBrush3by3()
    {
        List<Vector3Int> newCorridor = new List<Vector3Int>();
        foreach (var corridorPosition in corridorPositions)
        {
            for (int x = -gridSize; x < gridSize; x += gridSize)
            {
                for (int y = -gridSize; y < gridSize; y += gridSize)
                {
                    newCorridor.Add(corridorPosition + new Vector3Int(y, 0, x));
                }
            }
        }
        corridorPositions.UnionWith(newCorridor);
        floorPositions.UnionWith(newCorridor);
    }

    private List<Vector3Int> IncreaceCorridorSizeByOne(List<Vector3Int> corridor)
    {
        List<Vector3Int> newCorridor = new List<Vector3Int>();
        Vector3Int previousDirection = Vector3Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            Vector3Int directionFromCell = corridor[i] - corridor[i - 1];
            if (previousDirection != Vector3Int.zero && directionFromCell != previousDirection)
            {
                for (int x = -1; x  < 2;  x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector3Int(x, y, 0));
                    }
                }
                previousDirection = directionFromCell;
            }
            else
            {
                Vector3Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector3Int GetDirection90From(Vector3Int direction)
    {
        if (direction == Vector3Int.forward)
            return Vector3Int.right;
        if (direction == Vector3Int.right)
            return Vector3Int.back;
        if (direction == Vector3Int.back)
            return Vector3Int.left;
        if (direction == Vector3Int.left)
            return Vector3Int.forward;
        return Vector3Int.zero;
    }

    private void CreateRoomsAtDeadEnds(List<Vector3Int> deadEnds, HashSet<Vector3Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                SaveRoomData(position, room);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector3Int> FindAllDeadEnds(HashSet<Vector3Int> floorPositions)
    {
        List<Vector3Int> deadEnds = new List<Vector3Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionalList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector3Int> CreateRooms(HashSet<Vector3Int> potentialRoomPositions)
    {
        HashSet<Vector3Int> roomPositions = new HashSet<Vector3Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector3Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        ClearRoomData();
        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);

            SaveRoomData(roomPosition, roomFloor);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private void SaveRoomData(Vector3Int roomPosition, HashSet<Vector3Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
        roomColors.Add(UnityEngine.Random.ColorHSV());

        var roomGo = Instantiate(roomDataPrefab, tilemapVisualizer.transform);
        var roomData = roomGo.GetComponent<RoomData>();
        roomData.roomCenter = roomPosition;
        rooms.Add(roomData, roomFloor);

        /*GameObject roomGO = new GameObject("room");
        roomGO.transform.SetParent(tilemapVisualizer.wallsParent);
        roomGO.transform.position = roomPosition;
        rooms.Add(roomGO, roomFloor);*/
    }

    private void ClearRoomData()
    {
        roomsDictionary.Clear();
        roomColors.Clear();
    }

    private void CreateOcclusionZone(GameObject roomGO, HashSet<Vector3Int> roomFloor)
    {
        var box = roomGO.AddComponent<SphereCollider>();
        box.isTrigger = true;
        box.excludeLayers = ~LayerMask.GetMask("Player");
        box.enabled = false;
    }

    private void CreateCorridors(HashSet<Vector3Int> floorPositions, 
        HashSet<Vector3Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (var i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        corridorPositions = new HashSet<Vector3Int>(floorPositions);

        var roomGo = Instantiate(roomDataPrefab, tilemapVisualizer.transform);
        var roomData = roomGo.GetComponent<RoomData>();
        corridorRoom = roomData;
        //SaveRoomData(Vector3Int.zero, corridorPositions);
    }

    private void OnDrawGizmos()
    {
        // Отрисовка комнат
        if (showRoomGizmo && roomsDictionary != null && roomsDictionary.Count > 0)
        {
            int colorIndex = 0;
            foreach (var room in roomsDictionary)
            {
                // Выбираем цвет для текущей комнаты
                if (colorIndex < roomColors.Count)
                {
                    Gizmos.color = roomColors[colorIndex];
                }
                else
                {
                    Gizmos.color = Color.white; // Запасной цвет
                }

                // Рисуем каждую плитку комнаты
                foreach (var tilePosition in room.Value)
                {
                    // Центрируем гизмо в зависимости от gridSize
                    Vector3 center = new Vector3(
                        tilePosition.x + gridSize / 2f,
                        tilePosition.y,
                        tilePosition.z + gridSize / 2f
                    );

                    Gizmos.DrawCube(center, new Vector3(gridSize, 0.2f, gridSize));
                }
                colorIndex++;
            }
        }

        // Отрисовка коридоров (если включено)
        if (showCorridorGizmo && corridorPositions != null)
        {
            Gizmos.color = Color.white;
            foreach (var pos in corridorPositions)
            {
                Vector3 center = new Vector3(
                    pos.x + gridSize / 2f,
                    pos.y,
                    pos.z + gridSize / 2f
                );
                Gizmos.DrawWireCube(center, new Vector3(gridSize, 0.2f, gridSize));
            }
        }
    }
}
