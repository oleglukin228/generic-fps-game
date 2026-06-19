using AudioSystem;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;
using static ObjectPoolManager;

public class SurfaceManager : MonoBehaviour
{
    [SerializeField] private SurfaceType fallbackSurfaceType;
    [SerializeField] private SurfaceEffect fallbackSurfaceEffect;
    [SerializeField] private bool enableFallbackSurfaces;

    public static SurfaceManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    public void SpawnEffect(Vector3 position, Quaternion rotation, Vector3 normal, Collider hitCollider, SurfaceImpact surfaceImpact, Transform parent)
    {
        SurfaceType surfaceType = GetSurfaceType(hitCollider);
        if (surfaceType == null) return;
        SurfaceEffect surfaceEffect = GetSurfaceEffect(surfaceImpact, surfaceType);
        if (surfaceEffect == null) return;

        if (surfaceEffect.spawnedObjects.Length != 0)
        {
            var randomObject = surfaceEffect.spawnedObjects[Random.Range(0, surfaceEffect.spawnedObjects.Length)];
            ObjectPoolManager.SpawnObject(randomObject, parent, position, rotation, PoolType.ParticleSystems);
        }
        if (surfaceEffect.spawnedDecals.Length != 0)
        {
            DecalManager.Instance.SpawnDecalProjector(position, rotation, normal, surfaceEffect, parent);
        }
        if (surfaceEffect.audioClips.Length != 0)
        {
            var randomObject = surfaceEffect.audioClips[Random.Range(0, surfaceEffect.audioClips.Length)];
            SoundManager.Instance.CreateSoundBuilder().WithPosition(position).Play(randomObject);
            //AudioManager.Instance.SpawnAudioPlayer(position, rotation, normal, surfaceEffect, parent);
        }
    }

    private SurfaceType GetSurfaceType(Collider hitCollider)
    {
        // The SurfaceType on the SurfaceIdentifier can provide a unique SurfaceType for that collider. Therefore it should be tested first.
        var surfaceIdentifier = GetSurfaceIdentifier(hitCollider);
        if (surfaceIdentifier != null)
            if (surfaceIdentifier.SurfaceType != null)
                return surfaceIdentifier.SurfaceType;

        if (!enableFallbackSurfaces) return null;
        return null;
    }

    private SurfaceIdentifier GetSurfaceIdentifier(Collider hitCollider)
    {
        if (hitCollider == null) return null;
        SurfaceIdentifier surfaceIdentifier;
        surfaceIdentifier = hitCollider.GetComponent<SurfaceIdentifier>();
        return surfaceIdentifier;
    }

    private SurfaceEffect GetSurfaceEffect(SurfaceImpact surfaceImpact, SurfaceType surfaceType)
    {
        foreach (var impactEffect in surfaceType.ImpactEffects)
            if (surfaceImpact == impactEffect.SurfaceImpact)
                return impactEffect.SurfaceEffect;

        if (!enableFallbackSurfaces) return null;
        return fallbackSurfaceEffect;
    }
}
