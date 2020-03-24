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
    JunctionNode jNode;

    public bool drawVertexGizmos;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    // bools to keep track of what connection points are already connected
    public bool north { get; set; }
    public bool south { get; set; }
    public bool east { get; set; }
    public bool west { get; set; }

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
        Material juncMat = Resources.Load("InteresectionTex", typeof(Material)) as Material;
        GetComponent<MeshRenderer>().material = juncMat;
        GetComponent<MeshFilter>().mesh = mesh;

        // Generate sphere colliders for each intersection point on the connection nodes (NSEW))
        // The north-south nodes follow the major streamlines; the east-west nodes follow the minor streamlines

        GameObject northNode = new GameObject("North node");
        northNode.transform.parent = this.transform;
        OrientedPoint northPoint = new OrientedPoint(
            this.transform.position + point.rotation * Vector3.forward * 4.5f,
            this.transform.rotation,
            4,
            new List<OrientedPoint>()
        );
        northNode.AddComponent<JunctionNode>().Add(northNode, northPoint, this.north);

        GameObject southNode = new GameObject("South node");
        southNode.transform.parent = this.transform;
        OrientedPoint southPoint = new OrientedPoint(
            this.transform.position + point.rotation * Vector3.back * 4.5f,
            this.transform.rotation,
            0,
            new List<OrientedPoint>()
        );
        southNode.AddComponent<JunctionNode>().Add(southNode, southPoint, this.south);


        GameObject eastNode = new GameObject("East node");
        eastNode.transform.parent = this.transform;
        OrientedPoint eastPoint = new OrientedPoint(
            this.transform.position + point.rotation * Vector3.right * 4.5f,
            this.transform.rotation,
            2,
            new List<OrientedPoint>()
        );
        eastNode.AddComponent<JunctionNode>().Add(eastNode, eastPoint, this.east);


        GameObject westNode = new GameObject("West node");
        westNode.transform.parent = this.transform;
        OrientedPoint westPoint = new OrientedPoint(
            this.transform.position + point.rotation * Vector3.left * 4.5f,
            this.transform.rotation,
            1,
            new List<OrientedPoint>()
        );
        westNode.AddComponent<JunctionNode>().Add(westNode, westPoint, this.west);
    }

    void OnDrawGizmos() { if (drawVertexGizmos) { foreach (Vector3 vert in vertices) { Gizmos.DrawSphere(vert, 0.1f); } } }

}
