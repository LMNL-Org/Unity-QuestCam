using System;
using System.IO;
using QuestCam;
using UnityEngine;
using UnityEngine.Android;

public class QuestCamRecorder : MonoBehaviour
{
    public string gameToken;
    
    [Tooltip("Game cameras to record.")]
    public Camera[] cameras;

    private MediaRecorder _mediaRecorder;
    private AudioInput _audioInput;
    private IDisposable _videoInput;
    private bool _isRecording = false;
    
    public GameObject recordButton;
    public GameObject stopRecordButton;
    public GameObject changeLandscapeButton;

    public CameraTablet tablet;

    private async void StopRecording()
    {
        _videoInput?.Dispose();
        _videoInput = null;
        _audioInput?.Dispose();
        _audioInput = null;
        
        var recordingPath = await _mediaRecorder.FinishWriting();
        
        if (recordingPath.Length == 0)
            return;
        
        Debug.Log("Recorded to: " + recordingPath);
        
        NativeGallery.SaveVideoToGallery(recordingPath, "Videos", Path.GetFileName(recordingPath), OnSaveCallback);
        
        Debug.Log("Stopped recording: " + recordingPath);
        _isRecording = false;
    }
    
    private void OnSaveCallback(bool success, string path)
    {
        if (success)
        {
            Debug.Log("Video saved: " + path);
        }
        else
        {
            Debug.Log("Video could not be saved: " + path);
        }
    }

    public void StartRecording()
    {
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        #endif

        int w = 1280;
        int h = 720;

        if (!tablet.IsLandscape)
        {
            h = 1280;
            w = 720;
        }
        
        _mediaRecorder = MediaRecorder.Create(gameToken, w, h, 30, AudioSettings.outputSampleRate, (int)AudioSettings.speakerMode, 10_000_000, 2);
        if (_mediaRecorder == null)
            return;
        
        _videoInput = new CameraInput(_mediaRecorder, cameras);
        _audioInput = new AudioInput(_mediaRecorder, FindObjectOfType<AudioListener>());
        
        _isRecording = true;
    }

    public void OnRecordingPressed()
    {
        if (!_isRecording)
        {
            changeLandscapeButton.SetActive(false);
            recordButton.SetActive(false);
            stopRecordButton.SetActive(true);
            StartRecording();
        }
        else
        {
            StopRecording();
            recordButton.SetActive(true);
            stopRecordButton.SetActive(false);
            changeLandscapeButton.SetActive(true);
        }
    }
}
