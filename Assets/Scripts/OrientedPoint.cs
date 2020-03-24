using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public struct OrientedPoint
{
    // Source:
    // https://docs.google.com/presentation/d/10XjxscVrm5LprOmG-VB2DltVyQ_QygD26N6XC2iap2A/edit#slide=id.gb871cf6ef_0_66
    public Vector3 position;
    public Quaternion rotation;


    public float magnitude;
    public bool major;

    public List<OrientedPoint> neighbors;

    public OrientedPoint(Vector3 position, Quaternion rotation, float magnitude, List<OrientedPoint> neighbors, bool major = true)
    {
        this.position = position;
        this.rotation = rotation;
        this.magnitude = magnitude;
        this.neighbors = neighbors;
        this.major = major;
    }

    public bool Major 
    { 
        get { return major; }
    }

    public List<OrientedPoint> GetNeighbors
    {
        get { return neighbors; }
    }

    public void AddNeighbor(OrientedPoint neighbor)
    {
        neighbors.Add(neighbor);
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

