using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FightingPitRoom : RoomGenerator
{
    [SerializeField]
    private PrefabPlacer prefabPlacer;

    public List<EnemyPlacementData> enemyPlacementData;
    public List<ItemPlacementData> itemData;

    public override List<GameObject> ProcessRoom(Vector3Int roomCenter, RoomData roomData, HashSet<Vector3Int> roomFloor, HashSet<Vector3Int> roomFloorNoCorridors)
    {
        ItemPlacementHelper itemPlacementHelper =
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        /*List<GameObject> placedObjects =
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper, tilemapVisualizer.floorMeshParent);

        placedObjects.AddRange(prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper, tilemapVisualizer.enemiesAndPlayerParent));*/

        List<GameObject> placedObjects =
            prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper, roomData.entitiesParent);

        placedObjects.AddRange(prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper, roomData.roomDecorationsParent));

        return placedObjects;
    }
}
