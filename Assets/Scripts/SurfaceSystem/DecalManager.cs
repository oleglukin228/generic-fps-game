using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static ObjectPoolManager;

public class DecalManager : MonoBehaviour
{
    [SerializeField] private GameObject decalProjector;
    [SerializeField] private int maxActiveDecals = 50;
    private Queue<GameObject> activeDecalProjectors = new Queue<GameObject>();
    public static DecalManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    public void SpawnDecalProjector(Vector3 position, Quaternion rotation, Vector3 normal, SurfaceEffect surfaceEffect, Transform transformParent)
    {
        if (activeDecalProjectors.Count >= maxActiveDecals)
        {
            GameObject oldestDecal = activeDecalProjectors.Dequeue();
            ObjectPoolManager.ReturnObjectToPool(oldestDecal, PoolType.Decals);
        }

        GameObject projector = ObjectPoolManager.SpawnObject(decalProjector, transformParent, position,
            Quaternion.FromToRotation(Vector3.forward, normal), PoolType.Decals);

        if (projector == null) return;

        activeDecalProjectors.Enqueue(projector);

        int randIndexMat = Random.Range(0, surfaceEffect.spawnedDecals.Length);
        var randDecal = surfaceEffect.spawnedDecals[randIndexMat];

        DecalProjector _projector = projector.GetComponent<DecalProjector>();
        _projector.material = randDecal.decalMaterial;
        _projector.size = new Vector3(randDecal.decalSize, randDecal.decalSize, randDecal.decalDepth);
        _projector.pivot = Vector3.zero;
    }
}
