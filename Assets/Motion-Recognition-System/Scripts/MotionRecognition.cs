using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotionRecognition : MonoBehaviour
{
    #region RECOGNIZER
    public GameObject trackedObject;
    private Recognizer recognizer;
    private MotionLibrary motionLibrary;
    private List<Vector3> points;
    #endregion

    #region GUI
    public List<string> motionsNames;
    public string newMotionName = "";
    #endregion

    #region RECOGNIZER PARAMETERS
    public int NumberOfPoints = 150;
    public float scoreThreshold = 0.75f;  
    public float[] rescaleBorders = new float[2] { -1.0f, 1.0f };
    #endregion

    void Start()
    {
        points = new List<Vector3>() { };
        motionLibrary = MotionLibrary.GetInstance();
        recognizer = new ThreeDollar();
        recognizer.Init(NumberOfPoints, scoreThreshold, rescaleBorders);
        motionsNames = new List<string>() { };
        motionLibrary.GetMotions().ForEach(
            motion => { motionsNames.Add(motion.name); }
        );
    }

    void Update()
    {
        GetPoints();
        LaunchRecognition();

        UpdateGUI();
    }

    void UpdateGUI()
    {
        motionsNames.Clear();
        motionLibrary.GetMotions().ForEach(
            motion => { motionsNames.Add(motion.name); }
        );
    }

    void GetPoints()
    {
        if (Input.GetMouseButtonDown(0) && points.Count > 0)
        { this.points.Clear(); }
        if (Input.GetMouseButton(0))
        {
            this.points.Add(Camera.main.ScreenToWorldPoint(
               new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
           ));
        }
        if (recognizer.getTransformedPoints() != null)
        {
            motionLibrary.setTransformedPoints(recognizer.getTransformedPoints());
        }
    }

    public void LaunchRecognition()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Start Recognition !");
            recognizer.RecognitionProcess(points, motionLibrary.GetMotions());
        }
    }
}
