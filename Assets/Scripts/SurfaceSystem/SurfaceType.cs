using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generic fps/Surface Type")]
public class SurfaceType : ScriptableObject
{
    [SerializeField] protected ImpactEffect[] impactEffects;
    [SerializeField] protected bool allowFootprints = true;
    public ImpactEffect[] ImpactEffects { get { return impactEffects; } }
    public bool AllowFootprints { get { return allowFootprints; } }
}

[System.Serializable]
public struct ImpactEffect
{
    [SerializeField] private SurfaceImpact surfaceImpact;
    [SerializeField] private SurfaceEffect surfaceEffect;
    public SurfaceImpact SurfaceImpact { get { return surfaceImpact; } }
    public SurfaceEffect SurfaceEffect { get { return surfaceEffect; } }
}