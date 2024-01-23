using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [Serializable]
    public class Vertice {
        public Vector2 position;
        public Vector2 normal;
        public float U;
    }

    public Vertice[] vertices;
    public int2[] lines;

    public float CalcUSpan()
    {
        float dist = 0;
        foreach(var line in lines)
        {
            dist += (vertices[line.x].position - vertices[line.y].position).magnitude;
        }
        return dist;
    }
}
