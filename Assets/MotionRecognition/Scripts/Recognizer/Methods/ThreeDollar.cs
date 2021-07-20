using System.Collections.Generic;
using UnityEngine;

public class ThreeDollar : Recognizer
{
    private List<Vector3>
    resampledPoints = new List<Vector3>() { },
    rotatedPoints = new List<Vector3>() { },
    scaledPoints = new List<Vector3>() { },
    translatedPoints = new List<Vector3>() { };
    public override void RecognitionProcess(List<Vector3> points, List<Motion> motions)
    {
        resampledPoints = ResamplePoints(points, N);
        rotatedPoints = RotatePoints(resampledPoints);
        scaledPoints = ScalePoints(rotatedPoints);
        Vector3 scaledCentroid = -Utilities.ComputeCentroid(scaledPoints);
        translatedPoints = TranslatePoints(scaledPoints, scaledCentroid);

        Recognize(translatedPoints, motions).ForEach(motion =>
        {
            Debug.Log("Motion : " + motion.name);
        });
    }

    public new List<Motion> Recognize(List<Vector3> points, List<Motion> motions)
    {
        Debug.Log("Recognition begin");
        Motion bestMatch = new NullMotion();
        Camera mainCamera = Camera.main;
        List<KeyValuePair<Motion, float>> motionScores = new List<KeyValuePair<Motion, float>>() { };
        foreach (Motion motion in motions)
        {
            List<Vector3> final_candidate = Utilities.GSS(Utilities.MeanSquareError, points, motion.points, -180.0f, 180.0f, Vector3.forward, 2.0f);
            final_candidate = Utilities.GSS(Utilities.MeanSquareError, final_candidate, motion.points, -180.0f, 180.0f, Vector3.right, 2.0f);
            final_candidate = Utilities.GSS(Utilities.MeanSquareError, final_candidate, motion.points, -180.0f, 180.0f, Vector3.up, 2.0f);

            // Utilities.DrawPath(final_candidate, Color.cyan, true);

            float d = Utilities.MeanSquareError(final_candidate, motion.points);
            float l = rescaleBorders[1] - rescaleBorders[0];
            float Score = (1 - (d / (0.5f * Mathf.Sqrt(3 * l * l))));

            Debug.Log(" Motion : " + motion.name + " Score : " + Score);
            motionScores.Add(new KeyValuePair<Motion, float>(motion, Score));
        }

        Utilities.DrawPath(bestMatch.points, Color.red, false);

        return Heuristic(scoreThreshold, motionScores);
    }


    public List<Motion> Heuristic(float scoreThreshold, List<KeyValuePair<Motion, float>> motionScores)
    {
        // motionScores.Sort();
        List<KeyValuePair<Motion, float>> bestMatches = new List<KeyValuePair<Motion, float>>() { };
        List<Motion> bestMotion = new List<Motion>() { };
        foreach (KeyValuePair<Motion, float> motionScore in motionScores)
        {
            if (motionScore.Value > 0.95f * scoreThreshold)
            {
                bestMatches.Add(motionScore);
                if (motionScore.Value > 1.1f * scoreThreshold)
                {
                    bestMotion.Add(motionScore.Key);
                    return bestMotion;
                }
            }
        }
        bestMatches.ForEach(bestMatch =>
        {
            bestMotion.Add(bestMatch.Key);
        });

        if (bestMotion.Count == 0)
        {
            bestMotion.Add(new NullMotion());
        }

        return bestMotion;
    }


    public override List<Vector3> RotatePoints(List<Vector3> points)
    {
        List<Vector3> rotatedPoints = new List<Vector3>();
        Vector3 centroid = Utilities.ComputeCentroid(points);
        float theta = Mathf.Acos(
            Vector3.Dot(points[0], centroid)
            / (centroid.magnitude * points[0].magnitude)
            );
        Vector3 vAxis = Vector3.Cross(points[0], centroid).normalized;
        for (int i = 0; i < points.Count; i++)
        {
            rotatedPoints.Add(Quaternion.AngleAxis(theta, vAxis) * points[i]);
        }
        return rotatedPoints;
    }

    public override List<Vector3> getTransformedPoints()
    {
        return translatedPoints;
    }

}