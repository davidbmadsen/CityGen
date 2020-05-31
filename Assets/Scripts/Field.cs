using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    /*
    Create a field of eigenvectors
    */

    Spline spline;

    // Step size
    static float h = 5f;

    // Search cone radius
    static int searchRadius = (int)(h * 10);

    // Search cone angle (degrees)
    static float searchAngle = 35;

    // List lookup range
    static int lookupRange = (int)(searchRadius / 10);

    public float LinearGradient(float x, float z)
    {
        return x + z;
    }

    public OrientedPoint SampleOrthogonal(Vector3 point, float scale, float offset, bool major)
    {
        /*
        Function for sampling the orthogonal vector field.
        Returns an OrientedPoint with direction along the gradient, and position at the point given
        Assumes point.y = 0        
        */

        OrientedPoint eigenVector = new OrientedPoint();
        eigenVector.neighbors = new List<OrientedPoint>();
        eigenVector.position = point;
        eigenVector.major = major;
        float x_0 = (point.x + offset) / scale;
        float z_0 = (point.z + offset) / scale;

        // Precision: 
        float dx = 0.01f;
        float dz = 0.01f;

        // Map the noise function to f for readability
        System.Func<float, float, float> f = Mathf.PerlinNoise;
        
        //System.Func<float, float, float> f = LinearGradient;
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

    bool CheckBounds(OrientedPoint point, int mapSize)
    {
        if (point.magnitude <= 0.1
        || point.position.x > mapSize
        || point.position.z > mapSize
        || point.position.x < -mapSize
        || point.position.z < -mapSize)
        {
            // Debug.Log("Bounds check failed, returning...");
            return true;
        }
        else return false;
    }

    Tuple<bool, OrientedPoint> FindClosestCollisionPoint(OrientedPoint origin, List<OrientedPoint>[,] roadPoints, float threshold, float theta)
    {
        // Set the current closest distance to something very big
        float currentBestDistance = Mathf.Infinity;
        OrientedPoint closestPoint = new OrientedPoint();
        bool flag = false;

        // Round and convert to index to access appropriate sublists 
        int indX = ((int)(origin.position.x / 10) + roadPoints.GetLength(0) / 2);
        int indZ = ((int)(origin.position.z / 10) + roadPoints.GetLength(1) / 2);

        // Double for loop getting the lists around the point found to be closest
        for (int x = -lookupRange; x < lookupRange; x++)
        {
            for (int z = -lookupRange; z < lookupRange; z++)
            {
                foreach (OrientedPoint point in roadPoints[indX + x, indZ + z])
                {

                    float dist = CalcDistance(origin, point);

                    if (CheckProximity(origin, point, threshold, theta) && dist < currentBestDistance)
                    {
                        // Debug.Log("Found better point: " + point.position);
                        flag = true;
                        currentBestDistance = dist;
                        closestPoint = point;
                    }
                }
            }
        }
        /*
        // Update neighbors
        closestPoint.neighbors.Add(origin);
        origin.neighbors.Add(closestPoint);
        */
        return new Tuple<bool, OrientedPoint>(flag, closestPoint);
    }

    bool CheckProximity(OrientedPoint origin, OrientedPoint comparison, float threshold, float theta)
    {
        // Check if the points are too close, if yes, add them as neighbors to eachother and returns true

        // Angle between origin orientation and the point
        // Check for zero look-rotation 
        if (comparison.position - origin.position == Vector3.zero)
        {
            return false;
        }
        Quaternion rotationToTarget = Quaternion.LookRotation(comparison.position - origin.position, Vector3.up);

        // Search in a 120 degree cone forwards
        float dist = CalcDistance(origin, comparison);
        if (dist >= 3 && dist <= threshold && Quaternion.Angle(origin.rotation, rotationToTarget) <= theta)
        {

            // Debug.Log("Points " + origin.position + " and " + comparison.position + " too close. (Distance " + CalcDistance(origin, comparison) + ")");
            return true;
        }
        else return false;
    }

    float CalcDistance(OrientedPoint a, OrientedPoint b)
    {
        return (b.position - a.position).magnitude;
    }

    public List<OrientedPoint> Trace(System.Func<Vector3, float, float, bool, OrientedPoint> Sample,
                            float scale,
                            float offset,
                            bool major,
                            Vector3 startPoint,
                            bool reverse,
                            int length,
                            List<OrientedPoint>[,] roadPoints)
    {
        // The Vector3 point at which the streamline tracing is begun
        Vector3 currentPoint = startPoint;

        int mapSize = (roadPoints.GetLength(0) * 5);

        List<OrientedPoint> hyperstreamline = new List<OrientedPoint>();

        for (int i = 0; i < length; i++)
        {
            OrientedPoint pointAlongStreamline = Sample(currentPoint, scale, offset, major);

            // Magnitude and bounds check
            if (CheckBounds(pointAlongStreamline, mapSize)) { break; }

            // Check if the current point is close to the rest of the road points
            // try-catch in case the list of road points is empty

            // Update the starting point for next iteration            
            if (reverse)
            {
                // Rotate the orientation 180 degrees when reversing (points y-axis up by default)
                pointAlongStreamline.rotation = Quaternion.LookRotation(pointAlongStreamline.rotation * (-Vector3.forward));

                currentPoint += h * (pointAlongStreamline.rotation * Vector3.forward);
            }

            else
            {
                currentPoint += h * (pointAlongStreamline.rotation * Vector3.forward);
            }

            try
            {
                // Check for neighbors
                Tuple<bool, OrientedPoint> proxim = FindClosestCollisionPoint(pointAlongStreamline, roadPoints, searchRadius, searchAngle);

                if (proxim.Item1)
                {
                    OrientedPoint target = proxim.Item2;

                    // Debug.Log("Connecting " + pointAlongStreamline.position + " with " + target.position);
                    spline = new Spline();

                    float theta = Vector3.SignedAngle(pointAlongStreamline.rotation * Vector3.forward, target.rotation * Vector3.forward, Vector3.up);
                    //Debug.Log("Theta = " + theta);

                    // Orient target based on approach from left or right
                    if (theta < -45 && theta > -135) // Road approaches from the right
                    {
                        target.rotation = Quaternion.LookRotation(target.rotation * Vector3.right);
                    }
                    else if (theta > 45 && theta < 135) // Road approaches from the left 
                    {
                        target.rotation = Quaternion.LookRotation(target.rotation * Vector3.left);
                    }
                    else { return hyperstreamline; }

                    // Generate a spline for the connecting path
                    List<OrientedPoint> splinePath = spline.Connect(pointAlongStreamline, target);
                    hyperstreamline = hyperstreamline.Concat(splinePath).ToList();

                    // Update neighbor in target point
                    target.neighbors.Add(hyperstreamline.Last());

                    return hyperstreamline;
                }

            }
            catch { }

            // Add to array of oriented points and repeat iteration
            hyperstreamline.Add(pointAlongStreamline);

            // Update neighbors list
            if (i > 0)
            {
                hyperstreamline[i].neighbors.Add(hyperstreamline[i - 1]);
                hyperstreamline[i - 1].neighbors.Add(hyperstreamline[i]);
                // Debug.Log("Neighbors: " + hyperstreamline[i - 1].neighbors.Count);
            }

        }
        //Debug.Log("End of trace(), path length = " + hyperstreamline.Count);
        return hyperstreamline;
    }



    // Function for debugging - draws the streamline field visually
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