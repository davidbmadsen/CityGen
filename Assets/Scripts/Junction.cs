using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Junction : MonoBehaviour
{
    // Create a junction between roads

    Mesh mesh;
    MeshRenderer rend;

    public bool drawVertexGizmos;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    public void GenerateJunction(OrientedPoint point)
    {
        // Generate a simple quad to make the intersection (for now)
        // center the junction appropriately
        this.transform.position = point.position;

        mesh = new Mesh();

        // Basic quad 4,5m x 4,5m like the roads
        vertices = new Vector3[4] {
            new Vector3(-4.5f, 0, -4.5f),
            new Vector3( 4.5f, 0, -4.5f),
            new Vector3(-4.5f, 0,  4.5f),
            new Vector3( 4.5f, 0,  4.5f)
        };

        

        triangles = new int[6] {
            0, 2, 3,
            0, 3, 1
        };

        uvs = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1)
        }; 

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // just mesh things
        Material juncMat = Resources.Load("concrete", typeof(Material)) as Material;
        GetComponent<MeshRenderer>().material = juncMat;
        GetComponent<MeshFilter>().mesh = mesh;

        // Generate sphere colliders for each intersection point on the connection nodes (NSEW))
        GameObject north = new GameObject("North node");
        north.transform.parent = this.transform;
        north.transform.position = this.transform.position + point.rotation * Vector3.forward * 4.5f;
        north.AddComponent<SphereCollider>();
        north.GetComponent<SphereCollider>().transform.rotation = point.rotation;

        GameObject south = new GameObject("South node");
        south.transform.parent = this.transform;
        south.transform.position = this.transform.position + point.rotation * Vector3.back * 4.5f;
        south.AddComponent<SphereCollider>();
        south.GetComponent<SphereCollider>().transform.rotation = point.rotation;

        GameObject east = new GameObject("East node");
        east.transform.parent = this.transform;
        east.transform.position = this.transform.position + point.rotation * Vector3.right * 4.5f;
        east.AddComponent<SphereCollider>();
        east.GetComponent<SphereCollider>().transform.rotation = point.rotation;

        GameObject west = new GameObject("West node");
        west.transform.parent = this.transform;
        west.transform.position = this.transform.position + point.rotation * Vector3.left * 4.5f;
        west.AddComponent<SphereCollider>();
        west.GetComponent<SphereCollider>().transform.rotation = point.rotation;
    }

    void OnDrawGizmos() { if (drawVertexGizmos) { foreach (Vector3 vert in vertices) { Gizmos.DrawSphere(vert, 0.1f); } } }

}
