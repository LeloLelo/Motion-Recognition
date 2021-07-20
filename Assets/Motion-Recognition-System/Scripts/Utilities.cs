using System;
using UnityEngine;
using System.Collections.Generic;


public class Utilities
{
    static float GoldenRatio = 0.5f * (1 + Mathf.Sqrt(5));
    public static List<Vector3> centroids = new List<Vector3>() { Vector3.zero };
    public static float MeanSquareError(List<Vector3> path1, List<Vector3> path2)
    {
        float result = 0.0f;
        for (int i = 0; i < Mathf.Min(path1.Count, path2.Count); i++)
        {
            result +=
                (Mathf.Pow(path1[i].x - path2[i].x, 2) +
                Mathf.Pow(path1[i].y - path2[i].y, 2) +
                Mathf.Pow(path1[i].z - path2[i].z, 2));
        }
        result *= (1.0f / Mathf.Min(path1.Count, path2.Count));
        return result;
    }
    public static void DrawPath(List<Vector3> points, Color color, bool drawCentroid)
    {
        if (drawCentroid)
        {
            Vector3 centroidCoordinate = ComputeCentroid(points);
            centroids.Add(centroidCoordinate);
        }
        for (int i = 1; i < points.Count; i++)
        {
            Debug.DrawLine(points[i - 1], points[i], color, 100.0f);
        }
    }
    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        if (fromSource > toSource || fromSource < toSource)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
        return fromSource;
    }
    public static Vector3 ComputeCentroid(List<Vector3> points)
    {
        Vector3 tmp = new Vector3(0, 0, 0);
        foreach (Vector3 point in points)
        {
            tmp += point;
        }
        tmp /= points.Count;
        return tmp;
    }
    public static float PathLength(List<Vector3> points)
    {
        float result = 0.0f;
        for (int i = 1; i < points.Count; i++)
        {
            result = result + Distance(points[i - 1], points[i]);
        }
        return result;
    }
    public static float Distance(Vector3 A, Vector3 B)
    {
        Vector3 AB = B - A;
        return AB.magnitude;
    }
    public static void DrawBoundingBox(float[] rescaleBorders, Vector3 center)
    {
        Debug.DrawLine(new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[1]) + center, new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[0]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[1]) + center, new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[1]) + center, new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[0]) + center, new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[0]) + center, new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[0]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[0]) + center, new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[0]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[0]) + center, new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[0]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[0]) + center, new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[0]) + center, new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[0]) + center, new Vector3(rescaleBorders[1], rescaleBorders[1], rescaleBorders[0]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[1]) + center, new Vector3(rescaleBorders[0], rescaleBorders[1], rescaleBorders[1]) + center, Color.green, 1000);
        Debug.DrawLine(new Vector3(rescaleBorders[0], rescaleBorders[0], rescaleBorders[1]) + center, new Vector3(rescaleBorders[1], rescaleBorders[0], rescaleBorders[1]) + center, Color.green, 1000);
    }
    public static List<Vector3> RotatePoints(List<Vector3> points, Vector3 axis, float angle)
    {
        List<Vector3> newPoints = new List<Vector3>();
        foreach (Vector3 point in points)
        {
            newPoints.Add(Quaternion.AngleAxis(-angle, axis) * point);
        }
        return newPoints;
    }
    public static List<Vector3> GSS(Func<List<Vector3>, List<Vector3>, float> function, List<Vector3> candidate, List<Vector3> template, float a, float b, Vector3 axis, float cuttoff_angle)
    {
        float c = b - (b - a) / GoldenRatio;
        float d = a + (b - a) / GoldenRatio;
        List<Vector3> candidateD = null;
        List<Vector3> candidateC = null;
        while (Mathf.Abs(b - a) > cuttoff_angle)
        {
            candidateC = new List<Vector3>(candidate);
            candidateD = new List<Vector3>(candidate);
            candidateC = RotatePoints(candidateC, axis, c);
            candidateD = RotatePoints(candidateD, axis, d);

            float dC = function(candidateC, template);
            float dD = function(candidateD, template);
            if (dC < dD)
            {
                b = d;
            }
            else
            {
                a = c;
            }
            c = b - (b - a) / GoldenRatio;
            d = a + (b - a) / GoldenRatio;
        }
        float optimal_angle = (b + a) / 2.0f;
        List<Vector3> final_candidate = RotatePoints(candidate, axis, optimal_angle);
        return final_candidate;
    }
}
