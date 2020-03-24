using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CityGenerator))]
public class CityGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CityGenerator cityGen = (CityGenerator)target;
        if (DrawDefaultInspector())
        {
            
        }
        if (GUILayout.Button("Generate"))
        {
            cityGen.GenerateCity();
        }

    }
}
