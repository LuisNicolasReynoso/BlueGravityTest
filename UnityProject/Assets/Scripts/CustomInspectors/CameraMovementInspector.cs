using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CameraMovement))]
public class SmoothFollowEditor : Editor
{

    CameraMovement cameraMovement;

    public override void OnInspectorGUI()
    {
        cameraMovement = (CameraMovement)target;

        base.OnInspectorGUI();

       

        //Movement Folder----------------------------------------------------------------------------------
        cameraMovement.ShowMovement = EditorGUILayout.Foldout(cameraMovement.ShowMovement, "Movement parameters");

        if (cameraMovement.ShowMovement)
        {  
            cameraMovement.cameraTarget = (GameObject)EditorGUILayout.ObjectField("Camera Target", cameraMovement.cameraTarget, typeof(GameObject), true);
            cameraMovement.smoothTime = EditorGUILayout.Slider("Smooth Time", cameraMovement.smoothTime, 0,1);
            

        }
        EditorGUILayout.Space();
        //Initial Pos----------------------------------------------------------------------------------
        cameraMovement.ShowPos = EditorGUILayout.Foldout(cameraMovement.ShowPos, "Base parameters");

        if (cameraMovement.ShowPos)
        {
            cameraMovement.baseOffset = EditorGUILayout.FloatField("Base position", cameraMovement.baseOffset);
            cameraMovement.BaseAngle = EditorGUILayout.FloatField("Base Angle", cameraMovement.BaseAngle);
            

        }
        EditorGUILayout.Space();
        //Zoom Folder----------------------------------------------------------------------------------
        cameraMovement.ShowZoom = EditorGUILayout.Foldout(cameraMovement.ShowZoom, "Zoom parameters");

        if(cameraMovement.ShowZoom)
        {
            cameraMovement.zoomInOffset = EditorGUILayout.FloatField("ZoomInPos", cameraMovement.zoomInOffset);
            cameraMovement.ZoomSteps = EditorGUILayout.Slider("ZoomSteps", cameraMovement.ZoomSteps,1f,30f);
        }
        EditorGUILayout.Space();
               


        //Debug Folder----------------------------------------------------------------------------------
        cameraMovement.ShowDebug = EditorGUILayout.Foldout(cameraMovement.ShowDebug, "Debug");

        if (cameraMovement.ShowDebug)
        {
            cameraMovement.CurrentOffset = EditorGUILayout.FloatField("CurrentOffset", cameraMovement.CurrentOffset);
            EditorGUILayout.LabelField("Zoom percentage:   " + cameraMovement.ZoomPercentage);
            
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) { EditorUtility.SetDirty(cameraMovement); }
    }
}
