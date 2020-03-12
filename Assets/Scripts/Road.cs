using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Road : MonoBehaviour
{
    /*
    Road mesh generation class
    */
    Mesh roadMesh;
    Field field;

    Junction junc;
    MeshRenderer rend;
    public bool drawVertexGizmos;
    public float scale = 500;
    public int roadLength = 8000;

    public OrientedPoint seed;
    public Vector3 startingPoint;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    void Update()
    {
        //GenerateMesh(seed);
    }

    public void GenerateMesh(OrientedPoint seed, int length, bool major, bool rev)
    {   
        float offset = GetComponentInParent<RoadNetwork>().offset;
        int interval = GetComponentInParent<RoadNetwork>().interval;

        field = new Field();

        List<OrientedPoint> path = field.Trace(field.Orthogonal, this.scale, offset, major, transform.position + seed.position, rev, length);

        Extrude(path);

        // Pass the seeds up to the parents list
        for (int k = 0; k < path.Count; k++)
        {
            if (k % interval == 0)
            {
                try
                {
                    transform.parent.GetComponentInParent<RoadNetwork>().seeds.Enqueue(path[k]);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }

        // Generate points for start and end with collider, as well as draw a cube there
        GameObject start = new GameObject("Start");
        start.transform.parent = this.transform;
        start.transform.position = path[0].position;
        start.transform.rotation = path[0].rotation;
        start.AddComponent<SphereCollider>();
        start.AddComponent<Junction>();
        start.GetComponent<Junction>().GenerateJunction(path[0]);

        GameObject end = new GameObject("End");
        end.transform.parent = this.transform;
        end.transform.position = path[path.Count - 1].position;
        end.transform.rotation = path[path.Count - 1].rotation;
        end.AddComponent<SphereCollider>();
        end.AddComponent<Junction>();
        end.GetComponent<Junction>().GenerateJunction(path[path.Count - 1]);
    }


    public void Extrude(List<OrientedPoint> path)
    {
        roadMesh = new Mesh();
        Material roadMat = Resources.Load("RoadMaterial", typeof(Material)) as Material;
        GetComponent<MeshRenderer>().material = roadMat;
        GetComponent<MeshFilter>().mesh = roadMesh;

        // How the actual profile will look (extrusion)
        Vector2[] shape = new Vector2[]
        {
            new Vector2(-4.5f, 0f),
            new Vector2(4.5f, 0f)
        };

        Profile profile = new Profile(shape);

        // Mesh dimensions
        int width = profile.GetNumVertices;
        int length = path.Count;

        // Arrays to store vertices and triangle indices
        vertices = new Vector3[width * length];
        triangles = new int[6 * (width - 1) * (length - 1)];
        uvs = new Vector2[vertices.Length];

        // Add vertices to vertices array
        for (int i = 0, vert = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                vertices[vert] = (path[i].position) + path[i].rotation * new Vector3(profile.VertexCoords[j].x, profile.VertexCoords[j].y, 0);
                uvs[vert] = new Vector2(profile.VertexCoords[j].x, i);
                vert++;
            }

        }

        // Calculate triangle indices
        int triIdx = 0, vertIdx = 0;
        for (int j = 0; j < length - 1; j++)
        {
            for (int i = 0; i < width - 1; i++)
            {
                triangles[triIdx] = vertIdx;
                triangles[triIdx + 1] = vertIdx + width;
                triangles[triIdx + 2] = vertIdx + width + 1;

                triangles[triIdx + 3] = vertIdx;
                triangles[triIdx + 4] = vertIdx + width + 1;
                triangles[triIdx + 5] = vertIdx + 1;
                triIdx += 6;
                vertIdx++;
            }
            vertIdx++;
        }

        roadMesh.Clear();
        roadMesh.vertices = vertices;
        roadMesh.triangles = triangles;
        roadMesh.uv = uvs;
        roadMesh.RecalculateNormals();
    }


    void OnDrawGizmos() { if (drawVertexGizmos) { foreach (Vector3 vert in vertices) { Gizmos.DrawSphere(vert, 0.1f); } } }

}
