using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public MeshRenderer[] meshRenderers;
    public List<MaterialVariation> materialVariations;

    private void Start()
    {
        int index = UnityEngine.Random.Range(0, materialVariations.Count);

        MaterialVariation variation = materialVariations[index];
        Material[] newMaterials = skinnedMeshRenderers[0].materials;

        foreach (var materialInfo in variation.materialsInfo)
        {
            newMaterials[materialInfo.index] = materialInfo.material;
        }

        foreach (var mesh in skinnedMeshRenderers)
        {
            mesh.materials = newMaterials;
        }

        foreach (var mesh in meshRenderers)
        {
            mesh.materials = newMaterials;
        }

        Destroy(this);
    }
}

[Serializable]
public class MaterialVariation
{
    public List<MaterialInfo> materialsInfo;
}

[Serializable]
public class MaterialInfo
{
    public Material material;
    public int index;
}

