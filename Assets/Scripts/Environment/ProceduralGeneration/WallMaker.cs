using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer)), ExecuteInEditMode()]
public class WallMaker : MonoBehaviour
{
    [SerializeField] private Mesh _wallMesh;
    private SplineContainer _splineContainer;
    private List<Vector3> _points, _tans;
    private GameObject _childMesh;
    private float _distance;

    private void Awake()
    {
        _splineContainer = gameObject.GetComponent<SplineContainer>();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int arg2, SplineModification modification)
    {
        if (spline == _splineContainer.Spline && modification == SplineModification.KnotModified)
        {
            CalculatePoints();
        }
    }

    private void OnValidate()
    {
        if (_wallMesh == null) return;
        _distance = _wallMesh.bounds.size.z;

        CalculatePoints();
        GenerateMesh();
    }

    private void CalculatePoints()
    {
        _points = new List<Vector3>();
        _tans = new List<Vector3>();

        Spline spline = _splineContainer.Spline;

        _points.Add(spline.EvaluatePosition(0f));
        _points.Add(spline.EvaluateTangent(0f));

        if (_distance <= 0f) return;

        spline.GetPointAtLinearDistance(0f, _distance, out float t);

        while (t < 1)
        {
            _points.Add(spline.EvaluatePosition(t));
            _tans.Add(spline.EvaluateTangent(t));
            spline.GetPointAtLinearDistance(t, _distance, out t);
        }
    }

    private void GenerateMesh()
    {
        if ( _childMesh == null)
        {
            _childMesh = new GameObject("Mesh", typeof(MeshRenderer), typeof(MeshFilter));
            _childMesh.transform.SetParent(transform, false);
        }

        List<CombineInstance> instances = new List<CombineInstance>();

        Vector3 position;
        Matrix4x4 offsetMatrix;
        CombineInstance combineInstance;
        Mesh meshInstance = Instantiate(_wallMesh);

        for (int i = 0; i < _points.Count; i++)
        {
            Vector3 dir;
            Vector3 scale = Vector3.one;
            if (i == _points.Count - 1)
            {
                float dist = Vector3.Distance(_points[0], _points[_points.Count - 1]);
                if (dist < 0.01f) return;
                dir = _points[i] - _points[0];
                scale.z = dist / meshInstance.bounds.size.z;
            }
            else
            {
                dir = _points[i] - _points[i + 1];
            }


            position = _points[i];

            Vector3 lookDir = new Vector3(dir.x, dir.y, dir.z);
            offsetMatrix = Matrix4x4.TRS(position, Quaternion.LookRotation(lookDir), Vector3.one);

            for (int s = 0; s < meshInstance.subMeshCount; s++)
            {
                combineInstance = new CombineInstance();
                combineInstance.mesh = meshInstance;
                combineInstance.transform = offsetMatrix;
                combineInstance.subMeshIndex = s;
                instances.Add(combineInstance);
            }
        }

        instances = CombineBySubmeshIndex(instances);
        List<SubMeshDescriptor> subMeshes = InstancesToSubMeshData(instances);

        Mesh finalMesh = new Mesh();
        finalMesh.name = "Wall mesh";
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        finalMesh.CombineMeshes(instances.ToArray(), false);
        finalMesh.SetSubMeshes(subMeshes.ToArray());

        MeshFilter filter = _childMesh.GetComponent<MeshFilter>();
        filter.sharedMesh = finalMesh;
    }

    private List<SubMeshDescriptor> InstancesToSubMeshData(List<CombineInstance> instances)
    {
        List<SubMeshDescriptor> descriptors = new List<SubMeshDescriptor>();

        int triangleOffset = 0;

        foreach (var ci in instances)
        {
            Mesh mesh = ci.mesh;
            Matrix4x4 transform = ci.transform;

            int[] meshTris = mesh.GetTriangles(0);
            descriptors.Add(new SubMeshDescriptor(triangleOffset, meshTris.Length));
            triangleOffset += meshTris.Length;
        }

        return descriptors;
    }

    private List<CombineInstance> CombineBySubmeshIndex(List<CombineInstance> meshes)
    {
        Dictionary<int, List<CombineInstance>> instanceDictionary = new Dictionary<int, List<CombineInstance>>();
        foreach (CombineInstance item in meshes)
        {
            if (instanceDictionary.TryGetValue(item.subMeshIndex, out List<CombineInstance> values))
            {
                values.Add(item);
            }
            else
            {
                instanceDictionary.Add(item.subMeshIndex, new List<CombineInstance>() { item });
            }
        }
        List<CombineInstance> inst = new List<CombineInstance>();

        foreach (var valueSet in instanceDictionary.Values)
        {
            List<CombineInstance> combineInstances = valueSet;
            Mesh m = new Mesh();
            m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.CombineMeshes(combineInstances.ToArray(), true);

            CombineInstance c = new CombineInstance();
            c.transform = Matrix4x4.identity;
            c.mesh = m;
            c.subMeshIndex = combineInstances[0].subMeshIndex;
            inst.Add(c);
        }

        return inst;
    }

    private void OnDrawGizmos()
    {
        if (_points == null) return;
        foreach (Vector3 point in _points)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(point, 0.25f);
        }
    }
}
