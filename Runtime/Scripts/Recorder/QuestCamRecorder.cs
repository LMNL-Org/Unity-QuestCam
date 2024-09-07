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
    
    public ColorSpace overrideColorSpace = ColorSpace.Uninitialized;

    private RealtimeClock _clock;
    private MediaRecorder _mediaRecorder;
    private AudioInput _audioInput;
    private IDisposable _videoInput;
    public bool _isRecording = false;
    
    public GameObject recordButton;
    public GameObject stopRecordButton;
    public GameObject changeLandscapeButton;

    public CameraTablet tablet;

    private string m_LastRecordInternalFile;

    private async void StopRecording()
    {
        _videoInput?.Dispose();
        _videoInput = null;
        _audioInput?.Dispose();
        _audioInput = null;
        _clock = null;
        
        m_LastRecordInternalFile = await _mediaRecorder.FinishWriting();
        
        if (m_LastRecordInternalFile.Length == 0)
            return;
        
        Debug.Log("Recorded to: " + m_LastRecordInternalFile);
        
        NativeGallery.SaveVideoToGallery(m_LastRecordInternalFile, "Videos", Path.GetFileName(m_LastRecordInternalFile), OnSaveCallback);
        
        Debug.Log("Stopped recording: " + m_LastRecordInternalFile);
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

        try
        {
            File.Delete(m_LastRecordInternalFile);
            Debug.Log("Internal file deleted: " + m_LastRecordInternalFile);
        }
        catch (Exception e)
        {
            Debug.Log("Could not delete " + m_LastRecordInternalFile);
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

        _clock = new RealtimeClock();
        
        _videoInput = new CameraInput(_mediaRecorder, _clock, overrideColorSpace, cameras);
        _audioInput = new AudioInput(_mediaRecorder, FindObjectOfType<AudioListener>());
        
        _isRecording = true;
    }

    public void OnRecordingPressed()
    {
        if (gameToken.Length == 0)
        {
            Debug.LogWarning("[QuestCam] Game key is not set !");
            return;
        }
        
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
