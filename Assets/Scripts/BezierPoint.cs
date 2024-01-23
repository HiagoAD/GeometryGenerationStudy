using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BezierPoint
{
    public Vector3 pos;
    public Quaternion rot;

    public BezierPoint(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }

    public Vector3 GetGlobalPosition(Vector3 localPosition)
    {
        return (rot * localPosition) + pos;
    }

    public Vector3 RotateNormal(Vector3 normal)
    {
        return (rot * normal).normalized;
    }
}
