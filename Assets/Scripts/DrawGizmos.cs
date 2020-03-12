using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{   
    /*
    Generic class for drawing the sphere gizmos at each junction
    */
    
    void OnDrawGizmos()
    {   
        // Color
        Gizmos.color = Color.red;

        // Draw sphere
        Gizmos.DrawSphere(this.transform.position, 5);
    }
}
