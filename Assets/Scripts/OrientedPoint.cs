using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OrientedPoint
{
    // Source:
    // https://docs.google.com/presentation/d/10XjxscVrm5LprOmG-VB2DltVyQ_QygD26N6XC2iap2A/edit#slide=id.gb871cf6ef_0_66
    public Vector3 position;
    public Quaternion rotation;
    public float magnitude;
    public OrientedPoint(Vector3 position, Quaternion rotation, float magnitude)
    {
        this.position = position;
        this.rotation = rotation;
        this.magnitude = magnitude;
    }

    public Vector3 LocalToWorld(Vector3 point)
    {
        return position + rotation * point;
    }

    public Vector3 WorldToLocal(Vector3 point)
    {
        return Quaternion.Inverse(rotation) * (point - position);
    }

    public Vector3 LocalToWorldDirection(Vector3 dir)
    {
        return rotation * dir;
    }

}

