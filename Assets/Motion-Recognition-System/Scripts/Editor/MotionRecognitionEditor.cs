using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MotionRecognition), true)]
[CanEditMultipleObjects]
public class MotionRecognitionEditor : Editor
{

    SerializedProperty motionsNames;
    SerializedProperty newMotionName;
    SerializedProperty N;
    SerializedProperty scoreThreshold;
    SerializedProperty rescaleBorders;
    MotionLibrary motionLibrary;
    private void OnEnable()
    {
        motionLibrary = MotionLibrary.GetInstance();
        motionsNames = serializedObject.FindProperty("motionsNames");
        newMotionName = serializedObject.FindProperty("newMotionName");
        N = serializedObject.FindProperty("NumberOfPoints");
        scoreThreshold = serializedObject.FindProperty("scoreThreshold");
        rescaleBorders = serializedObject.FindProperty("rescaleBorders");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LoadedMotionsListGUI();

        GUILayout.Space(10);

        SaveMotionGUI();

        GUILayout.Space(10);

        RecognizerGUI();

        serializedObject.ApplyModifiedProperties();
    }

    void RecognizerGUI()
    {
        GUILayout.Label(UtilitiesGUI.UpperSectionLimit(EditorGUIUtility.currentViewWidth));
        GUILayout.Label("Number of points (N) : " + N.intValue.ToString());
        string rescaleBordersString = "Scale Limits : ";
        rescaleBordersString += "(" + 
            rescaleBorders.GetArrayElementAtIndex(0).floatValue.ToString() + 
            ", " + rescaleBorders.GetArrayElementAtIndex(1).floatValue.ToString() + ")";
        GUILayout.Label(rescaleBordersString);
        GUILayout.Label("Score Threshold : " + scoreThreshold.floatValue.ToString());
        GUILayout.Label(UtilitiesGUI.LowerSectionLimit(EditorGUIUtility.currentViewWidth));
    }
    public void SaveMotionGUI()
    {
        GUILayout.Label(UtilitiesGUI.UpperSectionLimit(EditorGUIUtility.currentViewWidth));
        GUILayout.Label("Save Motion");
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(newMotionName, GUILayout.Width(247));
        GUILayout.Space(5);
        if (motionLibrary != null)
        {
            if (motionLibrary.getTransformedPoints() != null)
            {
                GUILayout.Label("Transformed Points Length : " + motionLibrary.getTransformedPoints().Count);
                GUILayout.Space(5);
                if (GUILayout.Button("Save", GUILayout.Width(247)))
                {
                    motionLibrary.SaveMotion(newMotionName.stringValue, motionLibrary.getTransformedPoints());
                }
            }
        }
        GUILayout.Label(UtilitiesGUI.LowerSectionLimit(EditorGUIUtility.currentViewWidth));
    }

    public void LoadedMotionsListGUI()
    {
        GUILayout.Label(UtilitiesGUI.UpperSectionLimit(EditorGUIUtility.currentViewWidth));
        GUILayout.Label("Loaded Motions");
        GUILayout.Space(5);
        for (int i = 0; i < motionsNames.arraySize; i++)
        {
            GUILayout.Label("> " + motionsNames.GetArrayElementAtIndex(i).stringValue);
        }
        GUILayout.Label(UtilitiesGUI.LowerSectionLimit(EditorGUIUtility.currentViewWidth));
    }

}