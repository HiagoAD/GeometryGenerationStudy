using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class RoadSectionGenerator : MonoBehaviour
{
    [SerializeField] Mesh2D mesh2D;
    [Range(1, 32), SerializeField] int lod;

    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    Vector3 p0 => startPoint.position;
    Vector3 p1 => startPoint.position + startPoint.rotation * Vector3.forward * startPoint.localScale.z;
    Vector3 p2 => endPoint.position - endPoint.rotation * Vector3.forward * endPoint.localScale.z;
    Vector3 p3 => endPoint.position;

    Mesh mesh;

    readonly Vector3[] _cachedPositions = new Vector3[4];
    float _cachedDistance;
    List<BezierPoint> _pointsLookuptable = new();
    List<BezierPoint> PointsLooktable
    {
        get
        {
            if (_pointsLookuptable.Count != lod + 1 || p0 != _cachedPositions[0] || p1 != _cachedPositions[1] || p2 != _cachedPositions[2] || p3 != _cachedPositions[3])
            {
                _cachedDistance = 0;
                _pointsLookuptable = new(lod + 1)
                {
                    GetPointInCurve(0)
                };
                for (int i = 1; i <= lod; i++)
                {
                    var lastPoint = _pointsLookuptable[i - 1];

                    BezierPoint point = GetPointInCurve(i / (float)lod);
                    _cachedDistance += Vector3.Distance(point.pos, lastPoint.pos);
                    _pointsLookuptable.Add(point);
                }

                _cachedPositions[0] = p0;
                _cachedPositions[1] = p1;
                _cachedPositions[2] = p2;
                _cachedPositions[3] = p3;
            }

            return _pointsLookuptable;
        }
    }

    float TotalDistance
    {
        get
        {
            return _cachedDistance;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(p0, .5f);
        Gizmos.DrawSphere(p1, 0.25f);
        Gizmos.DrawSphere(p2, 0.25f);
        Gizmos.DrawSphere(p3, .5f);
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p2, p3);
        Gizmos.color = Color.white;

        Handles.DrawBezier(p0, p3, p1, p2, Color.white, EditorGUIUtility.whiteTexture, 1f);


        var point = GetPointInCurve(0.5f);
        foreach (var line in mesh2D.lines)
        {
            Gizmos.DrawLine(point.GetGlobalPosition(mesh2D.vertices[line.x].position), point.GetGlobalPosition(mesh2D.vertices[line.y].position));
        }
    }

    void Update()
    {
        if (mesh == null)
        {
            mesh = new()
            {
                name = "aeHOOO"
            };
            GetComponent<MeshFilter>().mesh = mesh;
        }

        mesh.Clear();


        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
        List<int> triangles = new();

        var uSpan = mesh2D.CalcUSpan();
        for (int t = 0; t <= lod; t++)
        {
            var currentPoint = PointsLooktable[t];
            var V = t / (float)lod * TotalDistance / uSpan;

            for (int l = 0; l < mesh2D.lines.Length; l++)
            {
                var line = mesh2D.lines[l];

                vertices.Add(currentPoint.GetGlobalPosition(mesh2D.vertices[line.x].position));
                vertices.Add(currentPoint.GetGlobalPosition(mesh2D.vertices[line.y].position));

                normals.Add(currentPoint.RotateNormal(mesh2D.vertices[line.x].normal));
                normals.Add(currentPoint.RotateNormal(mesh2D.vertices[line.y].normal));

                UVs.Add(new(mesh2D.vertices[line.x].U, V));
                UVs.Add(new(mesh2D.vertices[line.y].U, V));

                if (t != lod)
                {
                    var cx = (l * 2) + (t * mesh2D.lines.Length * 2);
                    var cy = cx + 1;
                    var nx = cx + (mesh2D.lines.Length * 2);
                    var ny = nx + 1;
                    triangles.AddRange(new List<int>() {
                        cx, nx, cy,
                        nx, ny, cy
                    });
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, UVs);
    }

    BezierPoint GetPointInCurve(float t)
    {
        float3 a = math.lerp(p0, p1, t);
        float3 b = math.lerp(p1, p2, t);
        float3 c = math.lerp(p2, p3, t);

        float3 d = math.lerp(a, b, t);
        float3 e = math.lerp(b, c, t);


        return new(math.lerp(d, e, t), Quaternion.Lerp(startPoint.rotation, endPoint.rotation, t));
    }

}