using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Profile
{
    // Struct that contains the mesh profile

    public Vector2[] VertexCoords;

    public Profile(Vector2[] coords)
    {
        this.VertexCoords = coords;
    }

    public int GetNumVertices
    {
        get { return VertexCoords.Length; }
    }

}
