using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class JunctionNode : MonoBehaviour
{
    // Node class for storing collider info and whether or not the connection point is used
    public bool _connected;
    public bool _flipped;
    public OrientedPoint _point;

    public void Add(GameObject node, OrientedPoint point, bool connected = false, bool flipped = false)
    {
        // Function that creates the node (NSEW)

        // Some initializations
        this._point = point;
        this._connected = connected;
        this._flipped = flipped;

        // Set transform
        node.transform.position = point.position;
        node.transform.rotation = point.rotation;

        // Add sphere collider for deteciton algorithm
        node.AddComponent<SphereCollider>();
    }

    // Getter/setter for connection info
    public bool Connected
    {
        get { return _connected; }
        set { value = _connected; }
    }

    public bool Flipped
    {
        get { return _flipped; }
        set { value = _flipped; }
    }
}