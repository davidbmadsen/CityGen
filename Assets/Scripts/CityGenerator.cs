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

    public int interval = 50;

    public void GenerateCity()
    {
        startingSeed = new OrientedPoint();
        startingSeed.position = startingPoint;

        InitiateGameObjects();
        
        // Generate the road network
        GameObject.Find("Road Network").GetComponent<RoadNetwork>().GenerateRoadNetwork(startingSeed, iter, length);
    
        // Generate the building network

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
