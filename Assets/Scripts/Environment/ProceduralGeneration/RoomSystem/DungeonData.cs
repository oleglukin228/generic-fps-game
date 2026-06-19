using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public PlayerController playerController;
    public Dictionary<RoomData, HashSet<Vector3Int>> rooms;
    public Dictionary<Vector3Int, HashSet<Vector3Int>> roomsDictionary;
    public HashSet<Vector3Int> floorPositions;
    public RoomData corridorRoom;
    public HashSet<Vector3Int> corridorPositions;

    public HashSet<Vector3Int> GetRoomFloorWithoutCorridors(RoomData dictionaryKey)
    {
        HashSet<Vector3Int> roomFloorNoCorridors = new HashSet<Vector3Int>(rooms[dictionaryKey]);
        roomFloorNoCorridors.ExceptWith(corridorPositions);
        return roomFloorNoCorridors;
    }
}