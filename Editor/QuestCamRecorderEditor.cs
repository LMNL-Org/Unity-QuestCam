using System;

#if UNITY_EDITOR

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
        EditorGUILayout.LabelField("You need to get game token from our site at");
        if (EditorGUILayout.LinkButton("https://questcam.io/dev", new[] { GUILayout.ExpandWidth(true) }))
        {
            Application.OpenURL("https://questcam.io/dev");
        }
        base.OnInspectorGUI();
        
        /*bool loggedIn = QuestCamService.CheckLogin();
        
        if (!loggedIn)
            EditorGUILayout.LabelField("You are not logged in into QuestCam");

        if (GUILayout.Button("Login/Register"))
            QuestCamLoginWindow.ShowWindow();
        
        EditorGUILayout.Separator();
        
        EditorGUI.BeginDisabledGroup(!loggedIn);

        base.OnInspectorGUI();
        
        EditorGUI.EndDisabledGroup();*/
    }
}
#endif