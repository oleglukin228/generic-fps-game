using UnityEngine;

public class SpawnGameObject : MonoBehaviour
{
    public GameObject prefab;
    public ObjectPoolManager.PoolType objectType;
    public bool destroy;
    public bool spawnOnStart;
    private MeshFilter[] prefabMeshFilters;
    private Transform[] prefabTransforms;
    public GameObject spawnedObject;
    private void Start()
    {
        if (spawnOnStart) SpawnObject();
        if (destroy) Destroy(gameObject);
    }

    void OnValidate()
    {
        // This method is called in the editor when the script is loaded or a value changes.
        // We re-cache the mesh filters and transforms whenever prefabToDraw changes.
        CachePrefabMeshesAndTransforms();
    }

    public void SpawnObject()
    {
        spawnedObject = ObjectPoolManager.SpawnObject(prefab, transform.position, transform.rotation, objectType);
    }

    private void OnDrawGizmos()
    {
        if (prefab == null || prefabMeshFilters == null || prefabMeshFilters.Length == 0)
        {
            return;
        }

        // Set the Gizmo color
        Gizmos.color = Color.cyan;

        // Iterate through all mesh filters found in the prefab
        for (int i = 0; i < prefabMeshFilters.Length; i++)
        {
            MeshFilter meshFilter = prefabMeshFilters[i];
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                continue;
            }

            // Calculate the world transformation for this specific mesh
            // We need to apply the prefab's local transform relative to the main GameObject's transform
            Vector3 worldPosition = transform.position + transform.rotation * Vector3.Scale(transform.lossyScale, prefabTransforms[i].localPosition);
            Quaternion worldRotation = transform.rotation * prefabTransforms[i].localRotation;
            Vector3 worldScale = Vector3.Scale(transform.lossyScale, prefabTransforms[i].localScale);

            // Set the Gizmo matrix to apply the mesh's transformation
            // This is crucial for drawing the mesh at its correct position, rotation, and scale
            Gizmos.matrix = Matrix4x4.TRS(worldPosition, worldRotation, worldScale);

            // Draw the wireframe of the mesh
            Gizmos.DrawWireMesh(meshFilter.sharedMesh);
        }

        // Reset the Gizmo matrix to identity after drawing, to avoid affecting other gizmos
        Gizmos.matrix = Matrix4x4.identity;
    }

    private void CachePrefabMeshesAndTransforms()
    {
        if (prefab == null)
        {
            prefabMeshFilters = null;
            prefabTransforms = null;
            return;
        }

        // Get all MeshFilters in the prefab hierarchy
        prefabMeshFilters = prefab.GetComponentsInChildren<MeshFilter>(true); // include inactive
        prefabTransforms = new Transform[prefabMeshFilters.Length];

        // Store the local transform relative to the prefab's root for each mesh
        for (int i = 0; i < prefabMeshFilters.Length; i++)
        {
            prefabTransforms[i] = prefabMeshFilters[i].transform;
        }
    }
}
