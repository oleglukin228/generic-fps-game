using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WallGenerator
{
    public static HashSet<Vector3Int> CreateWalls(HashSet<Vector3Int> floorPositions, TilemapVisualizer tilemapVisualizer, int gridSize)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionalList);
        //var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionalList);
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions, gridSize);
        CreateBackgroundWalls(tilemapVisualizer, basicWallPositions, floorPositions, gridSize);

        return basicWallPositions;
    }

    public static HashSet<Vector3Int> CreateWalls(HashSet<Vector3Int> floorPositions, TilemapVisualizer tilemapVisualizer, int gridSize, Dictionary<RoomData, HashSet<Vector3Int>> roomsDictionary)
    {
        var allWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionalList);
        HashSet<Vector3Int> corridorWallPositions = new HashSet<Vector3Int>(allWallPositions);
        Dictionary<RoomData, HashSet<Vector3Int>> newRoomsDictionary = new Dictionary<RoomData, HashSet<Vector3Int>>(roomsDictionary);
        foreach (var roomKeyValue in newRoomsDictionary.ToList())
        {
            var wallPositions = FindWallsInDirections(roomKeyValue.Value, Direction2D.cardinalDirectionalList);
            HashSet<Vector3Int> newValue = new HashSet<Vector3Int>(allWallPositions); 
            newValue.IntersectWith(wallPositions);
            corridorWallPositions.ExceptWith(wallPositions);
            newRoomsDictionary[roomKeyValue.Key] = newValue;
        }
        foreach (var roomWalls in newRoomsDictionary)
        {
            CreateBasicWalls1(tilemapVisualizer, roomWalls.Value, floorPositions, gridSize, roomWalls.Key.wallsParent);
            CreateBackgroundWalls(tilemapVisualizer, roomWalls.Value, floorPositions, gridSize, roomWalls.Key.backroundDecorationsParent);
        }
        //var corridorWallsGO = new GameObject("CorridorWalls");
        //corridorWallsGO.transform.SetParent(tilemapVisualizer.transform);
        //CreateBasicWalls(tilemapVisualizer, corridorWallPositions, floorPositions, gridSize, corridorWallsGO.transform);
        //CreateBackgroundWalls(tilemapVisualizer, corridorWallPositions, floorPositions, gridSize, corridorWallsGO.transform);

        //var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionalList);
        //CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions, gridSize);

        return corridorWallPositions;
    }

    public static void CreateCorridorWalls(HashSet<Vector3Int> corridorWallPositions, HashSet<Vector3Int> floorPositions, TilemapVisualizer tilemapVisualizer, int gridSize, RoomData corridorData)
    {
        CreateBasicWalls(tilemapVisualizer, corridorWallPositions, floorPositions, gridSize, corridorData.wallsParent);
        CreateBackgroundWalls(tilemapVisualizer, corridorWallPositions, floorPositions, gridSize, corridorData.backroundDecorationsParent);
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector3Int> cornerWallPositions, HashSet<Vector3Int> floorPositions, int gridSize)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            //tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, tilemapVisualizer.transform, gridSize);
        }
    }

    private static void CreateBackgroundWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector3Int> bgWallPositions, HashSet<Vector3Int> floorPositions, int gridSize, Transform parent = null)
    {
        foreach (var position in bgWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionalList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleBackgroundWall(position, neighboursBinaryType, gridSize, parent);
        }
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector3Int> basicWallPositions, HashSet<Vector3Int> floorPositions, int gridSize, Transform parent = null)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionalList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, gridSize, parent);
        }
    }

    private static void CreateBasicWalls1(TilemapVisualizer tilemapVisualizer, HashSet<Vector3Int> basicWallPositions, HashSet<Vector3Int> floorPositions, int gridSize, Transform parent = null)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionalList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, gridSize, parent);
        }
    }

    private static HashSet<Vector3Int> FindWallsInDirections(HashSet<Vector3Int> floorPositions, List<Vector3Int> directionList)
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }

    private static List<Vector3Int> SortSplinePointsV1(HashSet<Vector3Int> nodes)
    {
        if (nodes == null || nodes.Count == 0) return new List<Vector3Int>();

        List<Vector3Int> sorted = new List<Vector3Int>();

        // Выбираем произвольный первый элемент (так как в HashSet нет порядка)
        Vector3Int current = nodes.First();
        sorted.Add(current);
        nodes.Remove(current);

        while (nodes.Count > 0)
        {
            Vector3Int nextNode = default;
            float minDist = float.MaxValue;

            // В HashSet перебор выполняется через foreach
            foreach (var node in nodes)
            {
                // Используем sqrMagnitude для производительности (избегаем извлечения корня)
                float dist = (node - current).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nextNode = node;
                }
            }

            current = nextNode;
            sorted.Add(current);
            nodes.Remove(nextNode); // Удаление из HashSet происходит за O(1)
        }

        return sorted;
    }

    private static List<List<Vector3Int>> SortSplinePointsV2(HashSet<Vector3Int> nodes)
    {
        List<List<Vector3Int>> allChains = new List<List<Vector3Int>>();

        while (nodes.Count > 0)
        {
            // Начинаем новую цепочку
            List<Vector3Int> currentChain = new List<Vector3Int>();

            // Берем любую стартовую точку из оставшихся
            Vector3Int current = nodes.First();
            nodes.Remove(current);
            currentChain.Add(current);

            bool foundNeighbor = true;
            while (foundNeighbor)
            {
                foundNeighbor = false;
                Vector3Int nextNode = default;
                float minDist = float.MaxValue;

                foreach (var node in nodes)
                {
                    float dist = (node - current).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nextNode = node;
                    }
                }

                // Если ближайшая точка существует и она достаточно близко
                if (nodes.Count > 0)
                {
                    current = nextNode;
                    currentChain.Add(current);
                    nodes.Remove(nextNode);
                    foundNeighbor = true; // Продолжаем эту цепочку
                }
                else
                {
                    // Соседей в пределах дистанции нет — цепочка разорвана
                    foundNeighbor = false;
                }
            }

            // Добавляем готовую цепочку в общий список
            allChains.Add(currentChain);
        }

        return allChains;
    }

    // Вспомогательный метод для поиска ближайшего соседа
    private static bool TryGetNearest(Vector3Int origin, HashSet<Vector3Int> nodes, float sqrMaxDist, out Vector3Int result)
    {
        result = default;
        float minDist = float.MaxValue;
        bool found = false;

        foreach (var node in nodes)
        {
            float dist = (node - origin).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                result = node;
                found = true;
            }
        }

        return found;
    }

    private static HashSet<Vector3Int> SortWallPositions(HashSet<Vector3Int> wallPositions)
    {
        if (wallPositions == null || wallPositions.Count == 0)
            return new HashSet<Vector3Int>();

        HashSet<Vector3Int> sortedList = new HashSet<Vector3Int>();
        // Копируем во временный набор, чтобы не испортить оригинал
        HashSet<Vector3Int> remainingPoints = new HashSet<Vector3Int>(wallPositions);

        // 1. Берем первую попавшуюся точку для начала
        Vector3Int lastCheckDirection;
        Vector3Int current = remainingPoints.First();
        sortedList.Add(current);
        remainingPoints.Remove(current);

        // 2. Ищем соседей, пока точки не закончатся
        while (remainingPoints.Count > 0)
        {
            Vector3Int nextPoint = FindNearestNeighbor(current, remainingPoints);
            lastCheckDirection = nextPoint;

            if (nextPoint != current) // Если сосед найден
            {
                sortedList.Add(nextPoint);
                remainingPoints.Remove(nextPoint);
                current = nextPoint;
            }
            else
            {
                // Если мы здесь, значит цепочка прервалась (есть дырка в стене)
                // Можно либо закончить, либо взять случайную новую точку
                Debug.LogWarning("Цепочка стен прервана! Возможно, в стене есть разрывы.");

                current = remainingPoints.First();
                sortedList.Add(current);
                remainingPoints.Remove(current);
            }
        }

        return sortedList;
    }

    private static Vector3Int FindNearestNeighbor(Vector3Int current, HashSet<Vector3Int> points)
    {
        // Проверяем область вокруг текущей точки (3x3x3 куб)
        for (int x = -2; x <= 2; x += 1)
        {
            for (int z = -2; z <= 2; z += 1)
            {
                if (x == 0 && z == 0) continue;

                Vector3Int neighborCheck = current + new Vector3Int(x, 0, z);
                if (points.Contains(neighborCheck))
                {
                    return neighborCheck;
                }
            }
        }
        // Если прямого соседа нет, можно расширить поиск или вернуть ту же точку
        return current;
    }
}
