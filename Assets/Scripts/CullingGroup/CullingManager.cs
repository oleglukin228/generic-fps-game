using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    public static CullingManager Instance { get; private set; }

    public Camera cullingCamera;
    public float maxCullingDistance = 100f;
    public LayerMask cullableLayers;
    public List<string> cullableTags = new List<string>();
    public float updateInterval = 0.1f;

    CullingGroup group;
    BoundingSphere[] spheres = new BoundingSphere[64];
    public List<CullingTarget> owners = new List<CullingTarget>(64);
    Dictionary<CullingTarget, int> map = new Dictionary<CullingTarget, int>(64);
    int count;
    float tPos;
    int[] tmp = new int[256];
    HashSet<string> tagSet;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        group = new CullingGroup();
        group.onStateChanged = OnStateChanged;
        group.targetCamera = cullingCamera;
        group.SetBoundingSpheres(spheres);
        group.SetBoundingSphereCount(0);
        group.SetDistanceReferencePoint(cullingCamera.transform);
        group.SetBoundingDistances(new float[] { maxCullingDistance }); // single band
        //group.SetBoundingDistances(new float[]{ 10f, 25f, 60f });       // example of multiple bands, increasing distances

        tagSet = new HashSet<string>(cullableTags);
    }

    public void Setup(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        //if (!cullingCamera) cullingCamera = Camera.main;
        if (!cullingCamera) cullingCamera = dungeonData.playerController.PlayerCamera;
    }

    void Update()
    {
        tPos += Time.deltaTime;

        if (tPos >= updateInterval)
        {
            for (int i = 0; i < count; i++)
            {
                var o = owners[i];
                if (!o) continue;

                var s = spheres[i];
                s.position = o.transform.position;
                s.radius = o.boundarySphereRadius;
                spheres[i] = s;
            }

            tPos = 0f;
        }
    }

    public void Register(CullingTarget t)
    {
        if (!t) return;

        if (count == spheres.Length)
        {
            System.Array.Resize(ref spheres, count * 2);
            group.SetBoundingSpheres(spheres);
        }

        owners.Add(t);
        map[t] = count;
        spheres[count] = new BoundingSphere(t.transform.position, t.boundarySphereRadius);
        count++;
        group.SetBoundingSphereCount(count);
    }

    public void Deregister(CullingTarget t)
    {
        if (group == null || !t || !map.TryGetValue(t, out int i)) return;

        group.EraseSwapBack(i);
        CullingGroup.EraseSwapBack(i, spheres, ref count);
        var last = owners.Count - 1;
        var moved = owners[last];
        owners[i] = moved;
        owners.RemoveAt(last);
        if (moved) map[moved] = i;
        map.Remove(t);
        group.SetBoundingSphereCount(count);
    }

    void OnStateChanged(CullingGroupEvent e)
    {
        var cullingTarget = owners[e.index];
        if (!cullingTarget) return;

        if (!IsCullable(cullingTarget.gameObject))
        {
            cullingTarget.ToggleOn();
            return;
        }

        bool inRange = e.currentDistance == 0;
        if (e.isVisible && inRange) cullingTarget.ToggleOn();
        else cullingTarget.ToggleOff();
    }

    bool IsCullable(GameObject obj)
    {
        return ((1 << obj.layer) & cullableLayers) != 0 && tagSet.Contains(obj.tag); // layer and tag check
    }

    bool IsWithinDistance(Vector3 p)
    {
        return Vector3.Distance(cullingCamera.transform.position, p) <= maxCullingDistance;
    }

    int GetBandTargets(int band, List<CullingTarget> outList, bool? visible = null)
    {
        if (tmp.Length < count) tmp = new int[count];
        int n = visible.HasValue
            ? group.QueryIndices(visible.Value, band, tmp, 0)
            : group.QueryIndices(band, tmp, 0);
        outList.Clear();
        for (int i = 0; i < n; i++) outList.Add(owners[tmp[i]]);
        return n;
    }

    public (int visible, int culled) Snapshot()
    {
        if (tmp.Length < count) tmp = new int[count];
        int vis = group.QueryIndices(true, tmp, 0);
        return (vis, count - vis);
    }

    void OnDisable()
    {
        if (group != null)
        {
            group.onStateChanged = null;
            group.Dispose();
            group = null;
        }
    }
}
