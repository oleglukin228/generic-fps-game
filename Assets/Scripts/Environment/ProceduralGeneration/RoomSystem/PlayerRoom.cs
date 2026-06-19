using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoom : RoomGenerator
{
    public GameObject player, campfire;

    public List<ItemPlacementData> itemData;

    [SerializeField]
    private PrefabPlacer prefabPlacer;

    public override List<GameObject> ProcessRoom(
        Vector3Int roomCenter,
        RoomData roomData,
        HashSet<Vector3Int> roomFloor,
        HashSet<Vector3Int> roomFloorNoCorridors)
    {

        ItemPlacementHelper itemPlacementHelper =
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        List<GameObject> placedObjects =
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper, roomData.roomDecorationsParent);

        Vector3Int playerSpawnPoint = roomCenter;

        PlayerController existingPlayer = FindAnyObjectByType<PlayerController>();
        if (existingPlayer != null) 
        {
            existingPlayer.transform.position = playerSpawnPoint + new Vector3(0.5f, 0, 0.5f);
            existingPlayer.transform.SetParent(roomData.entitiesParent);
            placedObjects.Add(existingPlayer.gameObject);
        }
        else
        {
            GameObject playerObject = prefabPlacer.CreateObject(player, playerSpawnPoint + new Vector3(0.5f, 0, 0.5f), roomData.entitiesParent);
            placedObjects.Add(playerObject);
        }

        GameObject campfireObject = prefabPlacer.CreateObject(campfire, playerSpawnPoint + new Vector3(1f, 0, 1f), roomData.roomDecorationsParent);
        placedObjects.Add(campfireObject);

        return placedObjects;
    }

    public PlayerController GetPlayer() => player.GetComponent<PlayerController>();
}

public abstract class PlacementData
{
    [Min(0)]
    public int minQuantity = 0;
    [Min(0)]
    [Tooltip("Max is inclusive")]
    public int maxQuantity = 0;
    public int Quantity
        => UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
}

[Serializable]
public class ItemPlacementData : PlacementData
{
    public ItemData itemData;
}

[Serializable]
public class EnemyPlacementData : PlacementData
{
    public GameObject enemyPrefab;
    public Vector3Int enemySize = Vector3Int.one;
}
