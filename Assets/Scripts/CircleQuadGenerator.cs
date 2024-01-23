using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class QuadGeneration : MonoBehaviour
{

    [SerializeField, Range(.3f, 2f)] float innerRingRadius;
    [SerializeField, Range(0, 2f)] float ringTickness;
    [SerializeField, Range(3, 32)] int lod;

    float OutteRingRadius => innerRingRadius + ringTickness;

    Mesh mesh;

    // Update is called once per frame
    void Update()
    {
        if(mesh == null)
        {
            mesh = new();
            mesh.name = "aeHOOO";
            GetComponent<MeshFilter>().mesh = mesh;
        }

        mesh.Clear();


        float lodIncrement = 2 * Mathf.PI / lod;
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        for (int i = 0; i < lod; i++)
        {
            int next = (i + 1) % lod;

            float t = i * lodIncrement;
            Vector3 posT = new (Mathf.Cos(t), Mathf.Sin(t));

            vertices.Add(posT * OutteRingRadius);
            vertices.Add(posT * innerRingRadius);

            triangles.AddRange(new List<int>() {
                i * 2, next * 2, (i * 2) + 1,
                next * 2, (next * 2) + 1, (i * 2) + 1
            });

            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
    }

    void OnDrawGizmos()
    {
        float lodIncrement = 2 * Mathf.PI / lod;
        for (int i = 0; i < lod; i++)
        {
            float t = i * lodIncrement;
            Vector3 posT = transform.rotation * new Vector3(Mathf.Cos(t), Mathf.Sin(t));

            float nextT = ((i + 1) % lod) * lodIncrement;
            Vector3 posNextT = transform.rotation * new Vector3(Mathf.Cos(nextT), Mathf.Sin(nextT));

            Gizmos.DrawLine(posT * innerRingRadius, posNextT * innerRingRadius);
            Gizmos.DrawLine(posT * OutteRingRadius, posNextT * OutteRingRadius);
        }
    }
}
