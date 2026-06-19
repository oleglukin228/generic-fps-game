using Unity.Entities;
using Unity.Mathematics;

public struct PrefabSpawnPoints : IComponentData
{
    public BlobAssetReference<PrefabSpawnPointsBlob> Value;
}

public struct PrefabSpawnPointsBlob
{
    public BlobArray<float3> Value;
}

