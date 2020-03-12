using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RoadNetwork))]
public class RoadNetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoadNetwork roadNetwork = (RoadNetwork)target;

        if (DrawDefaultInspector())
        {
            if (roadNetwork.autoUpdate)
            {
                // Method for generating the roads
            }
        }

        if (GUILayout.Button("Generate"))
        {
            roadNetwork.GenerateRoadNetwork(roadNetwork.seed, roadNetwork.iter, roadNetwork.length); 
        }
    }
}
