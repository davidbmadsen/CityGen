using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{   
    public override void OnInspectorGUI()
    {
        Spline spline = (Spline)target;

        if (DrawDefaultInspector())
        {
            if (spline.autoUpdate)
            {
                spline.ConnectChildren();
            }
        }

        if (GUILayout.Button("Connect"))
        {
            spline.ConnectChildren();
        }
    }
}
