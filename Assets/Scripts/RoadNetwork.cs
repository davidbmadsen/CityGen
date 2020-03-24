using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadNetwork : MonoBehaviour
{
    /*
    
    Parent class to the road class
    
    This class is responsible for instantiating roads, using the Road methods
    
    */

    Road road;

    // List over all the points in the road system
    public List<OrientedPoint>[,] roadPoints;

    public Queue<OrientedPoint> seeds = new Queue<OrientedPoint>();
    public bool autoUpdate;
    public int iter;

    public bool drawGizmos;
    public float offset = 15000;
    public int scale;

    public int interval = 50;
    public OrientedPoint seed = new OrientedPoint();
    public Vector3 startPoint = new Vector3();
    public int length;

    public void GenerateRoadNetwork(OrientedPoint startingSeed, int iter, int length)
    {
        // Create and initialize new lookup matrix 
        roadPoints = new List<OrientedPoint>[500, 500];
        for (int x = 0; x < roadPoints.GetLength(0); x++)
        {
            for (int z = 0; z < roadPoints.GetLength(1); z++)
            {
                roadPoints[x, z] = new List<OrientedPoint>();
            }
        }

        // Clear the queue before new generation starts
        seeds.Clear();

        // Function structure
        GameObject mainRoad = new GameObject("Main Road");
        mainRoad.AddComponent<Road>().transform.parent = this.transform;

        // Generate the first road from the starting point
        mainRoad.GetComponent<Road>().GenerateRoad(startingSeed, length, true, false);

        // Extract seeds from the road and add to queue
        AddCandidatesToQueue(mainRoad.GetComponent<Road>().path, interval);


        while (seeds.Count != 0 && iter > 0)
        {
            OrientedPoint roadSeed = seeds.Dequeue();
            GameObject leftRoad = GenerateChildRoad();
            GameObject rightRoad = GenerateChildRoad();

            // Generate the subroads with alternating major flag
            // Flip the major flag for the next road
            bool major = !roadSeed.major;

            // Generate roads in either direction from seed
            leftRoad.GetComponent<Road>().GenerateRoad(roadSeed, length, major, false);
            rightRoad.GetComponent<Road>().GenerateRoad(roadSeed, length, major, true);


            iter--;
        }

    }

    void AddCandidatesToQueue(List<OrientedPoint> path, int interval)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (i % (interval / 3) == 0)
            {
                this.seeds.Enqueue(path[i]);
            }
        }
    }

    void OnDrawGizmos()
    {

        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            try
            {
                foreach (List<OrientedPoint> pointsList in roadPoints)
                {
                    foreach (OrientedPoint point in pointsList)
                    {
                        Gizmos.DrawSphere(point.position, 1f);
                        Gizmos.DrawRay(point.position, point.rotation * Vector3.forward * 2f);
                    }
                }
            }
            catch { }
        }
    }

    GameObject GenerateChildRoad()
    {
        // Function for generating a child gameObject with a Road() attached
        var newRoad = new GameObject("Child Road");

        // Add road script
        newRoad.AddComponent<Road>().transform.parent = this.transform;

        return newRoad;
    }

}
