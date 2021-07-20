using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class MotionLibrary
{
    private List<Motion> motions;
    private static MotionLibrary motionLibrary;
    private List<Vector3> potentialNewMotionTransformedPoints = new List<Vector3>(){};
    private MotionLibrary()
    {
        LoadMotions();
    }

    public static MotionLibrary GetInstance()
    {
        if (motionLibrary == null)
            motionLibrary = new MotionLibrary();
        return motionLibrary;
    }

    public List<Motion> GetMotions()
    {
        return motions;
    }

    public void LoadMotions()
    {
        this.motions = new List<Motion>();
        string pathLoadMotions = Application.dataPath + "/MotionRecognition/Motions/";

        foreach (string file in Directory.GetFiles(pathLoadMotions))
        {
            StreamReader reader = new StreamReader(file);
            try
            {
                string text = reader.ReadToEnd();
                Motion motion = JsonUtility.FromJson<Motion>(text);
                this.motions.Add(motion);
            }
            catch (System.Exception) { }
            reader.Close();
        }
    }

    public void SaveMotion(string name, List<Vector3> points)
    {
        Motion newMotion = new Motion(name, points);
        Debug.Log(newMotion.name + " recorded");
        string motion = JsonUtility.ToJson(newMotion);
        System.IO.File.WriteAllText(
            Application.dataPath + "/MotionRecognition/Motions/" +
            newMotion.name + ".json", motion
        );
        LoadMotions();
    }

    public void setTransformedPoints(List<Vector3> transformedPoints = null){
        potentialNewMotionTransformedPoints = transformedPoints;
    }

    public List<Vector3> getTransformedPoints(){
        return potentialNewMotionTransformedPoints;
    }

}