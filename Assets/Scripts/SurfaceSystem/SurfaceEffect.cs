using AudioSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Generic fps/Surface Effect")]
public class SurfaceEffect : ScriptableObject
{
    public GameObject[] spawnedObjects;
    public SurfaceDecal[] spawnedDecals;
    public SoundData[] audioClips;
}

[System.Serializable]
public class ObjectSpawnInfo
{
    [SerializeField] private GameObject gameObject;
    [Range(0, 1)][SerializeField] private float probability = 1;
    [SerializeField] private bool randomSpin;

    public GameObject GameObject { get { return gameObject; } }
    public float Probability { get { return probability; } }
    public bool RandomSpin { get { return randomSpin; } }
    public GameObject Instantiate(Vector3 position, Vector3 normal, Vector3 gravityDirection)
    {
        if (gameObject == null)
        {
            return null;
        }

        // There is a random chance that the object cannot be spawned.
        if (UnityEngine.Random.value < probability)
        {
            var rotation = Quaternion.LookRotation(normal);
            // A random spin can be applied so the rotation isn't the same every hit.
            if (randomSpin)
            {
                rotation *= Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), normal);
            }
            var instantiatedObject = ObjectPoolManager.SpawnObject(gameObject, position, rotation);
            return instantiatedObject;
        }
        return null;
    }
}

[System.Serializable]
public class SurfaceDecal
{
    [Tooltip("An array of materials that are randomly selected from when a decal is placed. Materials must be using a Projector shader.")]
    public Material decalMaterial;
    [Tooltip("The size of the projection onto surfaces.")]
    public float decalSize = 0.3f;
    [Tooltip("Adjusts the far clipping plane of the projector. Set higher if decals are fadding out or lower if decals are being placed on multiple sides of an object.")]
    public float decalDepth = 0.6f;
}