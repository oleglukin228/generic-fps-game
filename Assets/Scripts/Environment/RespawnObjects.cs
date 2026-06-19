using UnityEngine;

public class RespawnObjects : MonoBehaviour
{
    public SpawnGameObject[] spawns;
    
    public void RespawnObjectsEvent()
    {
        foreach (var obj in spawns)
        {
            ObjectPoolManager.ReturnObjectToPool(obj.spawnedObject, ObjectPoolManager.PoolType.GameObjects);
            ObjectPoolManager.SpawnObject(obj.prefab, obj.transform.position, obj.transform.rotation, ObjectPoolManager.PoolType.GameObjects);
        }
    }

    public void DeleteObjectsEvent()
    {
        foreach (var obj in spawns)
        {
            ObjectPoolManager.ReturnObjectToPool(obj.spawnedObject, ObjectPoolManager.PoolType.GameObjects);
        }
    }
}
