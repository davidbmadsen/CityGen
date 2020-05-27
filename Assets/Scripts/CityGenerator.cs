using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityGenerator : MonoBehaviour
{
    // Highest parent class to the city generation, namely road network generator and the building generator.
    // Calls the road network generator, then the building generator (for now)
    OrientedPoint startingSeed;
    public Vector3 startingPoint;

    // Starting parameters with some default values
    public int iter = 40;
    public int length = 5000;
    public int offset = 15000;
    public int scale = 350;
    public int interval = 50;

    public void GenerateCity()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        startingSeed = new OrientedPoint();
        startingSeed.position = startingPoint;

        InitiateGameObjects();

        // Generate the road network
        GameObject.Find("Road Network").GetComponent<RoadNetwork>().GenerateRoadNetwork(startingSeed, iter, length, interval);
        
        stopwatch.Stop();
        long roadTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();

        // Generate the building network
        List<OrientedPoint>[,] roadPoints = this.GetComponentInChildren<RoadNetwork>().roadPoints;
        GameObject.Find("Buildings").GetComponent<BuildingGenerator>().BuildHouses(roadPoints);
        stopwatch.Stop();

        long buildingTime = stopwatch.ElapsedMilliseconds;

        int n = 0;
        foreach (List<OrientedPoint> list in roadPoints)
        {
            n += list.Count;

        }

        int h = 0;
        foreach (Transform t in GameObject.Find("Buildings").GetComponentsInChildren<Transform>())
        {
            h += 1;
        }

        Debug.Log("Number of road points: " + n + " (time: " + roadTime + "ms)" +
            "\nTotal Buildings: "+ (h-1) + " (time: " + buildingTime + "ms)" +
            "\nTotal time: " + (long)(roadTime + buildingTime) + "ms");
    }

    void InitiateGameObjects()
    {
        // Create the essential GameObjects for the city generation:

        //  RoadNetwork
        GameObject roadNetwork = new GameObject("Road Network");
        roadNetwork.transform.parent = this.transform;
        roadNetwork.AddComponent<RoadNetwork>();

        //  Buildings
        GameObject buildings = new GameObject("Buildings");
        buildings.transform.parent = this.transform;
        buildings.AddComponent<BuildingGenerator>();
    }



}
