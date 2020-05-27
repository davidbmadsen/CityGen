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

    public bool drawGizmos;
    public int _iter;
    public int _interval;
    public int _offset;
    public int _length;
    
    int ctr;

    public OrientedPoint seed = new OrientedPoint();
    public Vector3 startPoint = new Vector3();
    public int length; // = 5000; // Global max length for each road generated

    public void GenerateRoadNetwork(OrientedPoint startingSeed, int iter, int length, int interval)
    {

        // Reference to parent
        CityGenerator parent = gameObject.GetComponentInParent<CityGenerator>();
        
        _interval = parent.interval;
        _offset = parent.offset;
        _length = parent.length;
        _iter = parent.length;
        ctr = _iter;

        // Create and initialize new lookup matrix (chunks)
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

        Road road = mainRoad.GetComponent<Road>();
        // Generate the first road from the starting point
        road.GenerateRoad(startingSeed, length, true, false);

        // Extract seeds from the road and add to queue
        AddCandidatesToQueue(road.path, interval);


        while (seeds.Count != 0 && iter > 0)
        {
            OrientedPoint roadSeed = seeds.Dequeue();
            GameObject leftRoad = GenerateChildRoad();
            GameObject rightRoad = GenerateChildRoad();

            // Generate the subroads with alternating major flag
            // Flip the major flag for the next road
            bool major = !roadSeed.major;

            Road left = leftRoad.GetComponent<Road>();
            Road right = rightRoad.GetComponent<Road>();

            // Generate roads in either direction from seed

            GenerateRoads(left, right, roadSeed, interval, major);

            iter--;
            ctr--;
        }
    }

    void GenerateRoads(Road left, Road right, OrientedPoint seed, int interval, bool majorFlag)
    {
        // Function for generating left and right roads from a candidate point
        // Assumes existing gameobjects with Road.cs component attached

        // Generate the roads
        left.GenerateRoad(seed, _length, majorFlag, false);
        right.GenerateRoad(seed, _length, majorFlag, true);
        
        // Update neighbors in the newly generated roads and their seed points
        if (left.path.Any())
        {
            seed.neighbors.Add(left.path.First());
            left.path.First().neighbors.Add(seed);

            AddCandidatesToQueue(left.path, interval);
        }

        if (right.path.Any())
        {
            seed.neighbors.Add(right.path.First());
            right.path.First().neighbors.Add(seed);

            AddCandidatesToQueue(right.path, interval);
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
                        if (point.neighbors.Count > 2){
                            Gizmos.DrawSphere(point.position, 5f);
                        }
                        Gizmos.DrawRay(point.position, point.rotation * Vector3.forward * 2f);
                    }
                }
            }
            catch { }
        }

    }

    /*
    void OnGUI()
    {   
        GUI.Label(new Rect(10, 10, 100, 20), "Roads: " + _iter + "/" + ctr);
    }
    */

    GameObject GenerateChildRoad()
    {
        // Function for generating a child gameObject with a Road() attached
        var newRoad = new GameObject("Child Road");

        // Add road script
        newRoad.AddComponent<Road>().transform.parent = this.transform;

        return newRoad;
    }

}
