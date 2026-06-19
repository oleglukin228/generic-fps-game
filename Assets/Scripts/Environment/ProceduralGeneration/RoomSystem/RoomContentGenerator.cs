using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomContentGenerator : MonoBehaviour
{
    [SerializeField] private PlayerRoom playerRoom;
    [SerializeField] private CorridorRoom corridorRoom;
    [SerializeField] private RoomGenerator[] enemyRoomTypes;

    List<GameObject> spawnedObjects = new List<GameObject>();
    public Transform itemParent;
    public UnityEvent RegenerateDungeon;

    public void GenerateRoomContent(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerSpawnPoint(dungeonData, tilemapVisualizer);
        SelectEnemySpawnPoints(dungeonData, tilemapVisualizer);
        PlaceCorridorTraps(dungeonData, tilemapVisualizer);
        /*foreach (GameObject item in spawnedObjects)
        {
            if (item != null)
                item.transform.SetParent(itemParent, false);
        }*/
    }

    private void PlaceCorridorTraps(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        List<GameObject> placedPrefabs = corridorRoom.ProcessRoom(
            Vector3Int.zero,
            dungeonData.corridorRoom,
            dungeonData.corridorPositions,
            dungeonData.corridorPositions
            );

        spawnedObjects.AddRange(placedPrefabs);
    }

    private void SelectPlayerSpawnPoint(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.rooms.Count);
        Vector3Int playerSpawnPoint = dungeonData.rooms.Keys.ElementAt(randomRoomIndex).roomCenter;

        //graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

        RoomData roomIndex = dungeonData.rooms.Keys.ElementAt(randomRoomIndex);

        List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
            playerSpawnPoint,
            dungeonData.rooms.Keys.ElementAt(randomRoomIndex),
            dungeonData.rooms.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
            );

        spawnedObjects.AddRange(placedPrefabs);

        dungeonData.playerController = playerRoom.GetPlayer();
        dungeonData.rooms.Remove(roomIndex);
    }

    private void SelectEnemySpawnPoints(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        foreach (KeyValuePair<RoomData, HashSet<Vector3Int>> roomData in dungeonData.rooms)
        {
            int index = Random.Range(0, enemyRoomTypes.Length);
            spawnedObjects.AddRange(
                enemyRoomTypes[index].ProcessRoom(roomData.Key.roomCenter, roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key))
            );
        }

        /*foreach (KeyValuePair<Vector3Int, HashSet<Vector3Int>> roomData in dungeonData.roomsDictionary)
        {
            int index = Random.Range(0, enemyRoomTypes.Length);
            spawnedObjects.AddRange(
                enemyRoomTypes[index].ProcessRoom(
                    roomData.Key,
                    tilemapVisualizer,
                    roomData.Value,
                    dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)
                    )
            );

        }*/
    }
}

