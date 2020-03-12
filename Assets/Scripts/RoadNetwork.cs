using System.Collections;
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

    public Queue<OrientedPoint> seeds = new Queue<OrientedPoint>();
    public bool autoUpdate;
    public int iter;
    
    public float offset;

    public int interval = 50;
    public OrientedPoint seed = new OrientedPoint();
    public int length;

    void Start()
    {   
        // Generate the road network
        GenerateRoadNetwork(this.seed, iter, length);
    }

    public void GenerateRoadNetwork(OrientedPoint startingSeed, int iter, int length)
    {   
        // Clear the queue before new generation starts
        seeds.Clear();

        // Function structure
        GameObject mainRoad = new GameObject("Main Road");
        mainRoad.AddComponent<Road>();
        mainRoad.transform.parent = this.transform;

        // Generate the first road from the starting point
        mainRoad.GetComponent<Road>().GenerateMesh(startingSeed, length, true, false);

        // Seed queue is now full of seed points.

        while (seeds.Count != 0 && iter >= 0)
        {   
            OrientedPoint currentSeed = seeds.Dequeue();
            // Debug.Log("Dequeued seed pos: " + currentSeed.position + "\nQueue length: " + seeds.Count.ToString());

            GameObject childRoadLeft = GenerateChildRoad(iter);
            GameObject childRoadRight = GenerateChildRoad(iter);

            // Generate the minor roads out from the major one
            childRoadLeft.GetComponent<Road>().GenerateMesh(currentSeed, length, false, false);
            childRoadRight.GetComponent<Road>().GenerateMesh(currentSeed, length, false, true);

            iter--;
        }


        // Generate a road of X length from seed point, using Road.GenerateMesh() from seed point

        // Road.GenerateMesh() returns new seed points

        // Add these seeds to queue, flip the road direction, and reiterate untill seeds is empty

        // -> Road network complete
    }

    GameObject GenerateChildRoad(int num)
    {
        // Function for generating a child gameObject with a Road() attached
        var newRoad = new GameObject("Child Road " + num);

        // Add road script
        newRoad.AddComponent<Road>();

        newRoad.transform.parent = this.transform;

        return newRoad;

    }

}
