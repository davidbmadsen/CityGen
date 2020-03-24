using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NetworkGenerator))]
public class NetworkGenEditor : Editor
{
    Field field = new Field();
    public override void OnInspectorGUI()
    {
        NetworkGenerator netGen = (NetworkGenerator)target;


        // Toggle auto-update when values are changed
        if (DrawDefaultInspector())
        {
            if (netGen.autoUpdate)
            {   
                netGen.DrawStreamlines();
                field.TensorfieldGrid(field.Orthogonal, netGen.scale, netGen.offset, true, netGen.mapHeight, netGen.mapWidth);
            }
            // netGen.DrawLines();
            // netGen.DrawStreamlines();
        };

        // Button to generate a junction at the current position
        if (GUILayout.Button("Generate Junction"))
        {
            netGen.CreateJunction(netGen.transform.position);
        }

        // Button to destroy all junctions
        if (GUILayout.Button("Destroy Junctions"))
        {
            foreach (Transform child in netGen.transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        // Button to draw lines (roads) between gameObjects (junctions)
        if (GUILayout.Button("Streamline"))
        {
            netGen.DrawStreamlines();
        }

        // Button to draw lines (roads) between gameObjects (junctions)
        if (GUILayout.Button("Debug: Draw tensorfield"))
        {
            field.TensorfieldGrid(field.Orthogonal, netGen.scale, netGen.offset, true, netGen.mapHeight, netGen.mapWidth);
        }

        /*
        // Button to draw lines (roads) between gameObjects (junctions)
        if (GUILayout.Button("Draw tensor field"))
        {
            netGen.DrawTensorField();
        }
        
        */
    }
}
