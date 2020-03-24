using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spline 
{
    /* Class for connecting to roads with a spline interpolation

    // Input: two OrientedPoints; one for the beginning and one for the end

    // Output: a path of OrientedPoints to trace the connecting roads with

    */
    Road road;
    public bool autoUpdate;
    public List<OrientedPoint> pointsPath;

    public List<OrientedPoint> Connect(OrientedPoint start, OrientedPoint end)
    {
        List<OrientedPoint> path = new List<OrientedPoint>();

        int length = 13;

        for (int i = 0; i < length; i++)
        {
            // Non-normalized path
            path.Add(BezierCubic(start, end, (i / (float)(length - 1))));

            // Add current point to previous neighbor list
            if (i > 0) { path[i - 1].neighbors.Add(path[i]); }
        }
        pointsPath = path;


        return path;
    }

    public OrientedPoint LerpOrientedPoint(OrientedPoint a, OrientedPoint b, float t)
    {
        return new OrientedPoint(
            Vector3.Lerp(a.position, b.position, t),
            Quaternion.Slerp(a.rotation, b.rotation, t),
            a.magnitude + (b.magnitude - a.magnitude) * t,
            new List<OrientedPoint>()
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
            start.position + (start.rotation * Vector3.forward * 20),
            start.rotation,
            start.magnitude,
            new List<OrientedPoint>()
        );

        OrientedPoint b = new OrientedPoint(
            end.position + (end.rotation * Vector3.back * 20),
            end.rotation * Quaternion.LookRotation(Vector3.back),
            end.magnitude,
            new List<OrientedPoint>()
        );

        // Square bezier between points start - a - b
        OrientedPoint ab = BezierSquare(start, a, b, t);

        // Square bezier between points a - b - end
        OrientedPoint bc = BezierSquare(a, b, end, t);

        // Lerp between points ab - bc to produce the final point in the bezier curve
        return LerpOrientedPoint(ab, bc, t);
    }
}
