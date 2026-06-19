using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector3Int> SimpleRandomWalk(Vector3Int startPosition, int walkLength)
    {
        HashSet<Vector3Int> path = new HashSet<Vector3Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public static List<Vector3Int> RandomWalkCorridor(Vector3Int startPosition, int corridorLength)
    {
        List<Vector3Int> corridor = new List<Vector3Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currectPosition = startPosition;
        corridor.Add(currectPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currectPosition += direction;
            corridor.Add(currectPosition);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.z >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.z >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.z >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.z >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.z >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(2, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(2, room.size.z);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, room.size.y, ySplit));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y, room.min.z + ySplit),
            new Vector3Int(room.size.x, room.size.y, room.size.z - ySplit));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector3Int> cardinalDirectionalList = new List<Vector3Int>();

    public static List<Vector3Int> diagonalDirectionalList = new List<Vector3Int>();

    public static List<Vector3Int> eightDirectionsList = new List<Vector3Int>();

    public static void ChangeGridSize(int gridSize)
    {
        cardinalDirectionalList = new List<Vector3Int>
        {
            new Vector3Int(0,0,gridSize),    //up
            new Vector3Int(gridSize,0,0),    //right
            new Vector3Int(0,0,-gridSize),   //down
            new Vector3Int(-gridSize,0,0)    //left
        };

        diagonalDirectionalList = new List<Vector3Int>
        {
            new Vector3Int(gridSize,0,gridSize),    //up-right
            new Vector3Int(gridSize,0,-gridSize),   //right-down
            new Vector3Int(-gridSize,0,-gridSize),  //down-left
            new Vector3Int(-gridSize,0,gridSize)    //left-up
        };

        eightDirectionsList = new List<Vector3Int>
        {
            new Vector3Int(0, 0, gridSize),         //up
            new Vector3Int(gridSize, 0, gridSize),  //up-right
            new Vector3Int(gridSize, 0, 0),         //right
            new Vector3Int(gridSize, 0, -gridSize), //right-down
            new Vector3Int(0, 0, -gridSize),        //down
            new Vector3Int(-gridSize, 0, -gridSize),//down-left
            new Vector3Int(-gridSize, 0, 0),        //left
            new Vector3Int(-gridSize, 0, gridSize)  //left-up   
        };
    }

    public static Vector3Int GetRandomCardinalDirection()
    {
        return cardinalDirectionalList[Random.Range(0, cardinalDirectionalList.Count)];
    }
}