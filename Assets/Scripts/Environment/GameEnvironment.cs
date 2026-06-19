using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameEnvironment : MonoBehaviour
{
    [SerializeField] private static List<GameObject> _bushes;
    [SerializeField] private static List<GameObject> _entities;
    private void Start()
    {
        _bushes = GameObject.FindGameObjectsWithTag("Bush").ToList();
    }

    public static Vector3 GetClosestBush(Vector3 position)
    {
        Transform closest = null;
        float minDistSqr = float.MaxValue;
        for (int i = 0; i < _bushes.Count; i++)
        {
            float distSqr = Vector3.SqrMagnitude(_bushes[i].transform.position - position);
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                closest = _bushes[i].transform;
            }
        }
        return closest.position;
    }

    public static void AddBush(GameObject bush) => _bushes.Add(bush);
    public static void AddEntity(GameObject entity) => _entities.Add(entity);
    public static void CleanBushes() => _bushes.Clear();

    public static void RemoveEntity(GameObject entity) => _entities.Remove(entity);
}
