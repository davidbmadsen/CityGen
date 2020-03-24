using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGenerator : MonoBehaviour
{
    /*
    Class for generating the road network.

    This class instantiates each junction according to the underlying tensor field (eigen fields)
    and draws gizmos to represent the street junctions

    Algorithm:
        1.  Generate tensor field and other relevant scalar fields (population density, etc.)

        2.  Trace major hyperstreamlines and generate junctions spaced appropriately
            with child GameObjects attached

        3.  Connect spatially neighboring junctions
    */

    
    Field field = new Field();
    public bool autoUpdate;
    public float scale;
    public float offset;
    
    public Vector3 pos;
    public int minorLength;
    public int majorLength;
    public int interval;
    public int amount;
    public int mapHeight, mapWidth;
    public void CreateJunction(Vector3 position)
    {

        // Create new junction at NetworkGen's (parent) position
        var junction = new GameObject("Junction at" + position.ToString());
        Debug.Log("Created junction at " + position);

        // Add gizmo drawing script
        junction.AddComponent<DrawGizmos>();
        junction.AddComponent<SphereCollider>();

        // Collider info
        SphereCollider sphereCollider;
        sphereCollider = junction.GetComponent<SphereCollider>();
        sphereCollider.radius = 5;

        // Assign as child
        junction.transform.parent = this.transform;

        // Set position
        junction.transform.position = position;
        Debug.Log("Position: " + junction.transform.position);

    } 


    // Debug function to draw streamlines in the form of lines
    public void DrawStreamlines()
    {

        /*
        Function that traces a streamline from a starting position, using the Field.Trace() function

        Store points along the streamline in a List<Vector3> 

        Make a new list for every substreamline
        
        Return a list of new seed points (?)
        */

        // Interval between seed points for the minor 

        Stack<Vector3> seeds = new Stack<Vector3>();

        // Generate streamline from origin
        Vector3 origin = this.transform.position + this.pos;

        // Index for keeping track of seed distances
        int idx = 0;

        for (int i = 0; i < this.amount; i++)
        {
            List<OrientedPoint>[,] roadPoints = new List<OrientedPoint>[5000,5000];
            List<OrientedPoint> streamline = field.Trace(field.Orthogonal, scale, offset, true, origin + new Vector3(i * 100, 0, 0), false, majorLength, roadPoints);

            // Loop through streamline points and draw rays between
            Vector3 prev = streamline[0].position;
            for (int j = 1; j < streamline.Count; j++)
            {
                Debug.DrawLine(prev, streamline[j].position, Color.red);
                prev = streamline[j].position;

                // If a multiple of the interval, add point as seed for minor tracing 
                if (idx % interval == 0) { seeds.Push(streamline[j].position); }

                idx++;
            }
        }

        // Draw the streamlines of the minor points found by seed in both directions
        /*
        while (seeds.Count != 0)
        {
            // Pop the top of the stack and trace that
            Vector3 seed = seeds.Pop();
            Vector3[] minorStreamlineLeft = field.Trace(field.Orthogonal, scale, false, seed, false, minorLength);
            Vector3[] minorStreamlineRight = field.Trace(field.Orthogonal, scale, false, seed, true, minorLength);
            

            Vector3 prev = seed;
            for (int k = 1; k < minorStreamlineLeft.Length; k++)
            {
                Debug.DrawLine(prev, minorStreamlineLeft[k], Color.magenta);
                prev = minorStreamlineLeft[k];
            }

            prev = seed;
            for (int k = 1; k < minorStreamlineRight.Length; k++)
            {
                Debug.DrawLine(prev, minorStreamlineRight[k], Color.cyan);
                prev = minorStreamlineRight[k];
            }
        }
        */
    }
}