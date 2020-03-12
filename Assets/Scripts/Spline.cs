using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Road))]
public class Spline : MonoBehaviour
{
    /* Class for connecting to roads with a spline interpolation

    // Input: two OrientedPoints; one for the beginning and one for the end

    // Output: a path of OrientedPoints to trace the connecting roads with

    */
    Road road;
    public bool autoUpdate;

    [SerializeField]
    public OrientedPoint pointOne;
    public OrientedPoint pointTwo;

    public float slider;

    List<OrientedPoint> pointsPath;
    void Update()
    {
        ConnectChildren();
    }
    public void ConnectChildren()
    {
        List<OrientedPoint> path = Connect(pointOne, pointTwo);

        GetComponent<Road>().Extrude(path);

    }
    public List<OrientedPoint> Connect(OrientedPoint start, OrientedPoint end)
    {
        List<OrientedPoint> path = new List<OrientedPoint>();

        int length = 50;

        for (int i = 0; i < length; i++)
        {
            path.Add(BezierCubic(start, end, (i / (float)(length - 1))));
        }

        pointsPath = path;
        return path;
    }

    public OrientedPoint LerpOrientedPoint(OrientedPoint a, OrientedPoint b, float t)
    {
        return new OrientedPoint(
            Vector3.Lerp(a.position, b.position, t),
            Quaternion.Slerp(a.rotation, b.rotation, t),
            a.magnitude + (b.magnitude - a.magnitude) * t
        );
    }
    public OrientedPoint BezierSquare(OrientedPoint start, OrientedPoint controlPoint, OrientedPoint end, float t)
    {

        // Lerp between points start - b - end
        OrientedPoint ab = LerpOrientedPoint(start, controlPoint, t);
        OrientedPoint bc = LerpOrientedPoint(controlPoint, end, t);

        OrientedPoint d = LerpOrientedPoint(ab, bc, t);

        // Since we want the tangent and not the Lerped rotation between the points, we update
        // the rotation to reflect this.
        d.rotation = Quaternion.LookRotation(bc.position - ab.position);

        return d;
    }

    public OrientedPoint BezierCubic(OrientedPoint start, OrientedPoint end, float t)
    {
        // Calculate control points, a for the start, b for the end
        OrientedPoint a = new OrientedPoint(
            start.position + (start.rotation * Vector3.forward * 10),
            start.rotation,
            start.magnitude
        );

        OrientedPoint b = new OrientedPoint(
            end.position + (end.rotation * Vector3.back * 10),
            end.rotation * Quaternion.LookRotation(Vector3.back),
            end.magnitude
        );

        // Square bezier between points start - a - b
        OrientedPoint ab = BezierSquare(start, a, b, t);

        // Square bezier between points a - b - end
        OrientedPoint bc = BezierSquare(a, b, end, t);

        // Lerp between points ab - bc to produce the final point in the bezier curve
        return LerpOrientedPoint(ab, bc, t);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointOne.position, pointOne.magnitude);
        Gizmos.DrawSphere(pointTwo.position, pointTwo.magnitude);

        try
        {
            foreach (OrientedPoint point in pointsPath)
            {
                Gizmos.DrawSphere(point.position, 0.5f);
                Gizmos.DrawRay(point.position, point.rotation * Vector3.forward);
            }


            // Gizmos.DrawSphere(Vector3.Lerp(pointOne.position, pointTwo.position, 0.5f), 1f);
            OrientedPoint ctrlPoint = new OrientedPoint(
                pointOne.position + (pointOne.rotation * Vector3.forward * 5),
                pointOne.rotation,
                1
            );

            OrientedPoint midPoint = LerpOrientedPoint(pointOne, pointTwo, 0.5f);
            OrientedPoint anotherMidPoint = BezierSquare(pointOne, ctrlPoint, pointTwo, slider / 10);

            OrientedPoint somePoint = BezierCubic(pointOne, pointTwo, slider / 10);
            //Debug.Log("Pos: " + anotherMidPoint.position);

            //Gizmos.DrawSphere(ctrlPoint.position, midPoint.magnitude);
            Gizmos.DrawCube(somePoint.position, Vector3.one * somePoint.magnitude);
        }
        catch
        {
            Debug.Log("List empty?");
        }
    }
}
