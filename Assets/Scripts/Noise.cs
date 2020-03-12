using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /*
    Class that generates noise maps using Perlin noise
    */
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        // Create the matrix of floats to store noise values
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Cap the scale value to avoid dividing by zero or negative scales
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // Sample perlin values and add to noiseMap matrix
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {   
                
                // Scale the current sample
                float sampleX = x / scale;
                float sampleZ = z / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = perlinValue;

            }
        }

        return noiseMap;
    }


}
