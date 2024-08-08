using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuestCam
{
    public class AudioInput : IDisposable
    {
        private readonly MediaRecorder _mediaRecorder;
        private AudioInputAttachment _audioInputAttachment;

        public AudioInput(MediaRecorder recorder, AudioListener audioListener)
            : this(recorder, audioListener.gameObject)
        { }
        
        public AudioInput(MediaRecorder recorder, GameObject audioGameObject)
        {
            _mediaRecorder = recorder;
            _audioInputAttachment = audioGameObject.AddComponent<AudioInputAttachment>();
            _audioInputAttachment.SampleBufferDelegate = OnSampleBuffer;
        }

        private void OnSampleBuffer(float[] data)
        {
            _mediaRecorder.CommitSamples(data);
        }

        public void Dispose() => Object.DestroyImmediate(_audioInputAttachment);

        private class AudioInputAttachment : MonoBehaviour {
            public Action<float[]> SampleBufferDelegate;
            private void OnAudioFilterRead (float[] data, int channels) => SampleBufferDelegate?.Invoke(data);
        }
    }
}