using UnityEngine;
using System.Collections.Generic;
public class Recognizer
{
    protected int N;
    protected float scoreThreshold;
    protected string recognizerName;
    protected float[] rescaleBorders;
    protected Motion recognizedMotion;

    #region GETTERS
    public int getNumberOfPoints() { return N; }
    public string getName() { return recognizerName; }
    public float getScoreThreshold() { return scoreThreshold; }
    public float[] getRescaleBorders() { return rescaleBorders; }
    public Motion getRecognizedMotion() { return recognizedMotion; }
    #endregion

    #region SETTERS
    public void setNumberOfPoints(int N) { this.N = N; }
    public void setName(string name) { this.recognizerName = name; }
    public void setScoreThreshold(float threshold) { this.scoreThreshold = threshold; }
    public void setRescaleBorders(float[] rescaleBorders) { this.rescaleBorders = rescaleBorders; }
    public void setRecognizedMotion(Motion motion)
    {
        recognizedMotion = motion;
    }
    #endregion
    public void Init(int N = 0, float scoreThreshold = 0.0f, float[] rescaleBorders = null)
    {
        this.N = N; this.scoreThreshold = scoreThreshold;
        this.rescaleBorders = rescaleBorders;
        this.recognizerName = this.GetType().ToString();
        this.recognizedMotion = new NullMotion();
    }
    public virtual void RecognitionProcess(List<Vector3> points, List<Motion> motions)
    {
    }
    public virtual Motion Recognize(List<Vector3> points, List<Motion> motions)
    {
        return new NullMotion();
    }
    public virtual List<Vector3> TranslatePoints(List<Vector3> points, Vector3 shift)
    {
        List<Vector3> translatedPoints = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            translatedPoints.Add(points[i] + shift);
        }
        return translatedPoints;
    }
    public virtual List<Vector3> ScalePoints(List<Vector3> points)
    {
        List<Vector3> scaledPoints = new List<Vector3>();
        float
            minX = float.MaxValue, minY = float.MaxValue,
            maxX = float.MinValue, maxY = float.MinValue,
            minZ = float.MaxValue, maxZ = float.MinValue;
        foreach (Vector3 point in points)
        {
            if (point.x < minX) { minX = point.x; }
            if (point.x > maxX) { maxX = point.x; }
            if (point.y < minY) { minY = point.y; }
            if (point.y > maxY) { maxY = point.y; }
            if (point.z < minZ) { minZ = point.z; }
            if (point.z > maxZ) { maxZ = point.z; }
        }
        for (int i = 0; i < points.Count; i++)
        {
            scaledPoints.Add(new Vector3(
                Utilities.Map(points[i].x, minX, maxX, rescaleBorders[0], rescaleBorders[1]),
                Utilities.Map(points[i].y, minY, maxY, rescaleBorders[0], rescaleBorders[1]),
                Utilities.Map(points[i].z, minZ, maxZ, rescaleBorders[0], rescaleBorders[1])
            ));
        }
        return scaledPoints;
    }
    public virtual List<Vector3> RotatePoints(List<Vector3> points)
    {
        return points;
    }
    public virtual List<Vector3> ResamplePoints(List<Vector3> points, int N)
    {
        List<Vector3> resampledPoints = new List<Vector3>();
        float I = Utilities.PathLength(points) / (N - 1);
        float D = 0f;
        resampledPoints.Add(points[0]);
        for (int i = 1; i < points.Count; i++)
        {
            float d = Utilities.Distance(points[i - 1], points[i]);
            if ((D + d) >= I)
            {
                Vector3 q = new Vector3(
                        (points[i - 1].x + ((I - D) / d) * (points[i].x - points[i - 1].x)),
                        (points[i - 1].y + ((I - D) / d) * (points[i].y - points[i - 1].y)),
                        (points[i - 1].z + ((I - D) / d) * (points[i].z - points[i - 1].z))
                    );
                resampledPoints.Add(q);
                points.Insert(i, q);
                D = 0;
            }
            else
            {
                D = D + d;
            }
        }
        return resampledPoints;
    }
    public virtual List<Vector3> getTransformedPoints(){return null;}
}
