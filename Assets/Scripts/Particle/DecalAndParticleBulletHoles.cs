using UnityEngine;

public class DecalAndParticleBulletHoles : MonoBehaviour
{
    public GameObject metalEffectPrefab;
    public GameObject concreteBulletHolePrefab;
    public GameObject woodBulletHolePrefab;
    public GameObject fleshBulletHolePrefab;
    public GameObject sandBulletHolePrefab;

    public GameObject[] fleshDecalMaterial;
    public GameObject[] surfaceBloodDecalPrefabs;

    public GameObject GetBulletHolePrefab(string surfaceTag)
    {
        // Пример реализации для двух разных тегов

        if (surfaceTag == "Untagged")
        {
            return concreteBulletHolePrefab;
        }
        else if (surfaceTag == "Concrete")
        {
            return concreteBulletHolePrefab;
        }
        else if (surfaceTag == "Wood")
        {
            return woodBulletHolePrefab;
        }
        else if (surfaceTag == "Metal")
        {
            return metalEffectPrefab;
        }
        else if (surfaceTag == "Flesh")
        {
            return fleshBulletHolePrefab;
        }
        else if (surfaceTag == "Sand or grass")
        {
            return sandBulletHolePrefab;
        }

        return null; // Если нет соответствующего префаба дыры
    }

    public GameObject GetBulletHoleDecalMaterial(string surfaceTag)
    {
        // Возвращайте соответствующий материал для каждого тега поверхности
        // в зависимости от ваших настроек материалов декалей.

        if (surfaceTag == "Flesh")
        {
            return fleshDecalMaterial[Random.Range(0, fleshDecalMaterial.Length)];
        }

        return null; // Если нет соответствующего материала декали
    }
}
