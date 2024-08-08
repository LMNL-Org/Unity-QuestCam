using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestCamRecorder))]
[CanEditMultipleObjects]
public class QuestCamRecorderEditor : Editor
{
    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        bool loggedIn = QuestCamService.CheckLogin();
        
        if (!loggedIn)
            EditorGUILayout.LabelField("You are not logged in into QuestCam");

        if (GUILayout.Button("Login/Register"))
            QuestCamLoginWindow.ShowWindow();
        
        EditorGUILayout.Separator();
        
        EditorGUI.BeginDisabledGroup(!loggedIn);

        base.OnInspectorGUI();
        
        EditorGUI.EndDisabledGroup();
    }
}