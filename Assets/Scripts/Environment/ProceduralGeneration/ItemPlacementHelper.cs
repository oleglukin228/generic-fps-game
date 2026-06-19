using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class ItemPlacementHelper
{
    Dictionary<PlacementType, HashSet<Vector3Int>>
        tileByType = new Dictionary<PlacementType, HashSet<Vector3Int>>();


    HashSet<Vector3Int> roomFloorNoCorridor;

    public ItemPlacementHelper(HashSet<Vector3Int> roomFloor, HashSet<Vector3Int> roomFloorNoCorridor)
    {
        Graph graph = new Graph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;

        foreach (var position in roomFloorNoCorridor)
        {
            int neighboursCount8Dir = graph.GetNeighbours8Directions(position).Count;
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;

            if (tileByType.ContainsKey(type) == false)
                tileByType[type] = new HashSet<Vector3Int>();

            if (type == PlacementType.NearWall && graph.GetNeighbours4Directions(position).Count == 4)
                continue;
            tileByType[type].Add(position);
        }
    }

    public Vector3? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector3Int size, bool addOffset)
    {
        int itemArea = size.x * size.z;
        if (tileByType[placementType].Count < itemArea)
            return null;

        int iteration = 0;
        while (iteration < iterationsMax)
        {
            iteration++;
            int index = Random.Range(0, tileByType[placementType].Count);
            Vector3Int position = tileByType[placementType].ElementAt(index);

            if (itemArea > 1)
            {
                var (result, placementPositions) = PlaceBigItem(position, size, addOffset);

                if (result == false)
                    continue;

                tileByType[placementType].ExceptWith(placementPositions);
                tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
            }
            else
            {
                tileByType[placementType].Remove(position);
            }

            return position;
        }
        return null;
    }

    private (bool, List<Vector3Int>) PlaceBigItem(Vector3Int originPosition, Vector3Int size, bool addOffset)
    {
        List<Vector3Int> positions = new List<Vector3Int>() { originPosition };
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxZ = addOffset ? size.z + 1 : size.z;
        int minX = addOffset ? -1 : 0;
        int minZ = addOffset ? -1 : 0;

        for (int row = minX; row < maxX; row++)
        {
            for (int col = minZ; col < maxZ; col++)
            {
                if (col == 0 && row == 0) continue;
                Vector3Int newPosToCheck = 
                    new Vector3Int(originPosition.x + row, originPosition.y, originPosition.z + col);
                if (roomFloorNoCorridor.Contains(newPosToCheck) == false) return (false, positions);
                positions.Add(newPosToCheck);
            }
        }
        return (true, positions);
    }
}

public enum PlacementType
{
    OpenSpace,
    NearWall,
    FullSpace
}
