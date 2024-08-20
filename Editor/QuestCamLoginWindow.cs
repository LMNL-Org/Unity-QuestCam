using System;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class QuestCamLoginWindow : EditorWindow
{
    private static GUIStyle CenterText = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
    private static GUIStyle CenterTextRed = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
    private static GUIStyle CenterTextGreen = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
    private static GUIStyle Border = new GUIStyle(GUI.skin.box) {};

    private string _username;
    private string _password;
    private bool _rememberMe;
    private string _loginText;
    private bool _loginStatus;

    private string _registerUsername;
    private string _registerPassword;
    private string _registerRePassword;
    private string _registerEmail;
    
    private string _registerText;
    private bool _registerStatus;
    
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(QuestCamLoginWindow));
        
        CenterTextRed.normal.textColor = Color.red;
        CenterTextGreen.normal.textColor = Color.green;
    }

    private void OnLoginStatus(bool success, string text)
    {
        _loginStatus = success;
        _loginText = text;
    }

    private void OnRegisterStatus(bool success, string text)
    {
        _registerStatus = success;
        _registerText = text;
    }
        
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(Border);
        EditorGUILayout.LabelField("Login", CenterText, GUILayout.ExpandWidth(true));
        _username = EditorGUILayout.TextField("Username", _username);
        _password = EditorGUILayout.PasswordField("Password", _password);
        _rememberMe = EditorGUILayout.Toggle("Remember me", _rememberMe);
        
        EditorGUILayout.LabelField(_loginText, _loginStatus ? CenterTextGreen : CenterTextRed, GUILayout.ExpandWidth(true));
        
        if (GUILayout.Button("Login"))
            QuestCamService.Login(_username, _password, _rememberMe, OnLoginStatus);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        EditorGUILayout.BeginVertical(Border);
        EditorGUILayout.LabelField("Register", CenterText, GUILayout.ExpandWidth(true));
        _registerUsername = EditorGUILayout.TextField("Username", _registerUsername);
        _registerEmail = EditorGUILayout.TextField("Email", _registerEmail);
        _registerPassword = EditorGUILayout.PasswordField("Password", _registerPassword);
        _registerRePassword = EditorGUILayout.PasswordField("Re Password", _registerRePassword);
        
        EditorGUILayout.LabelField(_registerText, _registerStatus ? CenterTextGreen : CenterTextRed, GUILayout.ExpandWidth(true));
        
        if (GUILayout.Button("Register"))
            QuestCamService.Register(_registerUsername, _registerEmail, _registerPassword, _registerRePassword, OnRegisterStatus);
        EditorGUILayout.EndVertical();
    }
}
#endif