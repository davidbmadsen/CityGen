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
    public float scale;
    public int roadLength = 5000;

    public OrientedPoint seed;
    public Vector3 startingPoint;

    public List<OrientedPoint> path;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    void Update()
    {
        //GenerateMesh(seed);
    }

    public void GenerateRoad(OrientedPoint seed, int length, bool major, bool rev)
    {
        float offset = GetComponentInParent<RoadNetwork>().offset;
        int interval = GetComponentInParent<RoadNetwork>().interval;
        List<OrientedPoint>[,] roadPoints = GetComponentInParent<RoadNetwork>().roadPoints;

        scale = GetComponentInParent<CityGenerator>().scale;
        field = new Field();
        path = field.Trace(field.Orthogonal, scale, offset, major, transform.position + seed.position, rev, length, roadPoints);
        if (path.Count == 0) { return; }

        Extrude(path);

    
        // Pass road path to parent
        foreach (OrientedPoint point in path)
        {
            int indX = ((int)(point.position.x / 10) + roadPoints.GetLength(0) / 2);
            int indZ = ((int)(point.position.z / 10) + roadPoints.GetLength(1) / 2);
            // Debug.Log("point: " + point.position +
            // "\nindX: " + indX + "\nindZ: " + indZ);

            GetComponentInParent<RoadNetwork>().roadPoints[indX, indZ].Add(point);
        }
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
