using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    /*
    Create a field of eigenvectors
    */

    public float LinearGradient(float x, float z)
    {
        return x + z;
    }

    public OrientedPoint Orthogonal(Vector3 point, float scale, float offset, bool major)
    {
        /*
        Function for sampling the orthogonal vector field.
        Returns an OrientedPoint with direction along the gradient, and position at the point given
        Assumes point.y = 0        
        */

        OrientedPoint eigenVector = new OrientedPoint();
        eigenVector.position = point;

        float x_0 = (point.x + offset) / scale;
        float z_0 = (point.z + offset) / scale;

        // Precision: 
        float dx = 0.01f;
        float dz = 0.01f;

        // Map the noise function to f for readability
        System.Func<float, float, float> f = Mathf.PerlinNoise;

        float gradX = (f(x_0 + dx, z_0) - f(x_0, z_0)) / dx;
        float gradZ = (f(x_0, z_0 + dz) - f(x_0, z_0)) / dz;

        // Gradient vector (should be orthogonal to field lines)
        Vector3 gradient = new Vector3(gradX, 0, gradZ);

        // Orient the point in the direction of the gradient
        eigenVector.rotation = Quaternion.LookRotation(gradient, Vector3.up);

        // Rotate 90 degrees if major tensor field
        if (major) { eigenVector.rotation *= Quaternion.AngleAxis(90, Vector3.up); }

        eigenVector.magnitude = Vector3.Magnitude(gradient);

        return eigenVector;
    }



    public List<OrientedPoint> Trace(System.Func<Vector3, float, float, bool, OrientedPoint> FieldFunction,
                            float scale,
                            float offset,
                            bool major,
                            Vector3 startPoint,
                            bool reverse,
                            int length)
    {
        /*
        Trace the hyperstreamlines using RK4
        RK4 Code courtesy of Martin Evans ()
        */
        // The Vector3 point at which the streamline tracing is begun
        Vector3 currentPoint = startPoint;

        List<OrientedPoint> hyperstreamline = new List<OrientedPoint>();

        float h = 1f;

        for (int i = 0; i < length; i++)
        {   
            OrientedPoint pointAlongStreamline = FieldFunction(currentPoint, scale, offset, major);

            if (pointAlongStreamline.magnitude <= 0.1) { break; }

            // Update the starting point for next iteration            
            if (reverse)
            {   
                // Rotate the orientation 180 degrees when reversing (points y-axis up by default)
                pointAlongStreamline.rotation = Quaternion.LookRotation(pointAlongStreamline.rotation * (-Vector3.forward));

                currentPoint += h * pointAlongStreamline.magnitude * (pointAlongStreamline.rotation * Vector3.forward);
            }
            
            else
            {
                currentPoint += h * pointAlongStreamline.magnitude * (pointAlongStreamline.rotation * Vector3.forward);
            }
            
            // Add to array of oriented points and repeat iteration
            hyperstreamline.Add(pointAlongStreamline);

        }

        return hyperstreamline;

        /*
         // Perform RK4
        Vector3 k1 = FieldFunction(startPoint, scale, minor);
        Vector3 k2 = FieldFunction(startPoint + k1 / 2f, scale, minor);
        Vector3 k3 = FieldFunction(startPoint + k2 / 2f, scale, minor);
        Vector3 k4 = FieldFunction(startPoint + k3, scale, minor);
        // startPoint =  (k1 / 6f + k2 / 3f + k3 / 3f + k4 / 6f);
        */
    }


    public void TensorfieldGrid(System.Func<Vector3, float, float, bool, OrientedPoint> Gradient,
                                float scale, float offset, bool major, int mapHeight, int mapWidth)
    {
        Vector3 prevDirection = Vector3.zero;
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Draw out rays starting at the position and with the direction of the tensor field
                // Make a vector to pass to the function
                Vector3 position = new Vector3(x, 0, z);

                // Calculate gradient eigenvector
                OrientedPoint eigVec = Gradient(position, scale, offset, major);

                Vector3 gradient = eigVec.rotation * Vector3.forward * eigVec.magnitude;
                // Draw rays
                Debug.DrawRay(eigVec.position, gradient, Color.green);
            }
        }
    }

}