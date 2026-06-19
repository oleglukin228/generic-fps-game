using Kryz.CharacterStats.Examples;
using System.Collections.Generic;
using UnityEngine;

public class CorridorRoom : RoomGenerator
{
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

        return placedObjects;
    }
}
