using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code for generating the mesh courtesy of Sebastian Lague (https://www.youtube.com/watch?v=4RpVBYW1r5M)

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Variables for centering the map
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertIndex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < height; x++)
            {
                // 
                meshData.vertices[vertIndex] = new Vector3(topLeftX + x, heightMap[x, z], topLeftZ - z);
                meshData.uvs[vertIndex] = new Vector2(x / (float)width, z / (float)height);
                if (x < width - 1 && z < height - 1)
                {
                    meshData.AddTriangle(vertIndex, vertIndex + width + 1, vertIndex + width);
                    meshData.AddTriangle(vertIndex + width + 1, vertIndex, vertIndex + 1);
                }

                vertIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{

    // Contains data for the vertices in the mesh
    public Vector3[] vertices;
    public int[] triangles;

    public Vector2[] uvs;
    int index;      // Index used when adding triangles
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[index] = a;
        triangles[index + 1] = b;
        triangles[index + 2] = c;
        index += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}