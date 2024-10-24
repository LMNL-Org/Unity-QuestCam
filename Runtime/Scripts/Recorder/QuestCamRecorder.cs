using System;
using System.IO;
using System.Threading.Tasks;
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
    private string _lastRecordInternalFile;
    
    public GameObject recordButton;
    public GameObject recordLoadingButton;
    public GameObject stopRecordButton;
    public GameObject changeLandscapeButton;
    public CameraTablet tablet;

    public bool IsRecording { get; private set; } = false;
    public float UnityVolume { get; private set; } = 1.0f;
    public float MicrophoneVolume { get; private set; } = 1.0f;
    
    private bool _waitingForRecorder = false;

    public bool IsMicrophoneMuted => MicrophoneVolume == 0.0f;
    
    public delegate void OnRecordingStartedDelegate(QuestCamRecorder recorder);
    public delegate void OnRecordingStoppedDelegate(QuestCamRecorder recorder);
    
    public event OnRecordingStartedDelegate OnRecordingStarted;
    public event OnRecordingStoppedDelegate OnRecordingStopped;

    private async void StopRecording()
    {
        if (!IsRecording)
            return;
        
        _videoInput?.Dispose();
        _videoInput = null;
        _audioInput?.Dispose();
        _audioInput = null;
        _clock = null;
        
        _lastRecordInternalFile = await _mediaRecorder.FinishWriting();
        
        if (_lastRecordInternalFile.Length == 0)
            return;
        
        Debug.Log("Recorded to: " + _lastRecordInternalFile);
        
        NativeGallery.SaveVideoToGallery(_lastRecordInternalFile, "Videos", Path.GetFileName(_lastRecordInternalFile), OnSaveCallback);
        
        Debug.Log("Stopped recording: " + _lastRecordInternalFile);
        IsRecording = false;
        
        OnRecordingStopped?.Invoke(this);
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
            File.Delete(_lastRecordInternalFile);
            Debug.Log("Internal file deleted: " + _lastRecordInternalFile);
        }
        catch (Exception e)
        {
            Debug.Log("Could not delete " + _lastRecordInternalFile);
            Debug.Log(e);
        }
    }

    private Task<MediaRecorder> _mediaRecorderCreateTask = null;

    private void Update()
    {
        if (_mediaRecorderCreateTask != null)
        {
            if (_mediaRecorderCreateTask.IsCompleted)
            {
                CompleteRecordingStart(_mediaRecorderCreateTask.Result);
                _mediaRecorderCreateTask = null;
            }
        }
    }

    private void CompleteRecordingStart(MediaRecorder recorder)
    {
        _mediaRecorder = recorder;
        _waitingForRecorder = false;

        if (_mediaRecorder == null)
        {
            IsRecording = false;
            recordButton.SetActive(true);
            stopRecordButton.SetActive(false);
            recordLoadingButton.SetActive(false);
            return;
        }
        
        _mediaRecorder.SetUnityAudioVolume(UnityVolume);
        _mediaRecorder.SetMicrophoneVolume(MicrophoneVolume);

        _clock = new RealtimeClock();
        
        _videoInput = new CameraInput(_mediaRecorder, _clock, overrideColorSpace, cameras);
        _audioInput = new AudioInput(_mediaRecorder, FindObjectOfType<AudioListener>());
        
        IsRecording = true;
        stopRecordButton.SetActive(true);
        recordLoadingButton.SetActive(false);

        OnRecordingStarted?.Invoke(this);
    }

    private void StartRecording()
    {
        if (IsRecording || _waitingForRecorder)
            return;
        
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

        _waitingForRecorder = true;
        _mediaRecorderCreateTask = MediaRecorder.Create(gameToken, w, h, 30, AudioSettings.outputSampleRate, (int)AudioSettings.speakerMode, 10_000_000, 2);
    }

    public void OnRecordingPressed()
    {
        if (gameToken.Length == 0)
        {
            Debug.LogWarning("[QuestCam] Game key is not set !");
            return;
        }
        
        if (_waitingForRecorder)
            return;
        
        if (!IsRecording)
        {
            changeLandscapeButton.SetActive(false);
            recordButton.SetActive(false);
            stopRecordButton.SetActive(false);
            recordLoadingButton.SetActive(true);
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

    public void SetUnityAudioVolume(float volume)
    {
        UnityVolume = volume;
        
        if (_mediaRecorder != null)
            _mediaRecorder.SetUnityAudioVolume(volume);
    }

    public void SetMicrophoneVolume(float volume)
    {
        MicrophoneVolume = volume;
        
        if (_mediaRecorder != null)
            _mediaRecorder.SetMicrophoneVolume(volume);
    }

    public void SetPaused(bool paused)
    {
        if (_mediaRecorder != null)
            _mediaRecorder.SetPaused(paused);
    }

    public void MuteMicrophone() => SetMicrophoneVolume(0.0f);
    
    public void UnmuteMicrophone() => SetMicrophoneVolume(1.0f);
    
    public void SetMicrophoneMuted(bool muted) => SetMicrophoneVolume(muted ? 0.0f : 1.0f);
}
