using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace QuestCam
{
    public class MediaRecorder
    {
        private IntPtr _recorder;
        private static string directory = string.Empty;

        private readonly int _width;
        private readonly int _height;
        
        public (int width, int height) FrameSize
        {
            get
            {
                return (_width, _height);
            }
        }

        private MediaRecorder(IntPtr recorder, int width, int height)
        {
            _recorder = recorder;
            _width = width;
            _height = height;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnInitialize () => directory = Application.isEditor ?
            Directory.GetCurrentDirectory() :
            Application.persistentDataPath;
        
        internal static string CreatePath (string? extension = null, string? prefix = null) {
            // Create parent directory
            var parentDirectory = !string.IsNullOrEmpty(prefix) ? Path.Combine(directory, prefix) : directory;
            Directory.CreateDirectory(parentDirectory);
            // Get recording path
            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            var name = $"recording_{timestamp}{extension ?? string.Empty}";
            var path = Path.Combine(parentDirectory, "video.mp4");
            // Return
            return path;
        }

        public static MediaRecorder Create(
            string gameToken,
            int width = 0,
            int height = 0,
            float frameRate = 0f,
            int sampleRate = 0,
            int channelCount = 0,
            int videoBitRate = 10_000_000,
            int keyframeInterval = 2,
            float compressionQuality = 0.8f,
            int audioBitRate = 64_000,
            string prefix = null)
        {
            QuestCamNative.CreateMP4Recorder(gameToken, CreatePath(extension: @".mp4", prefix: prefix), width, height, frameRate, sampleRate, channelCount,
                videoBitRate, keyframeInterval, audioBitRate, out IntPtr recorder);

            if (recorder == IntPtr.Zero)
            {
                Debug.LogError("Could not create recorder, ptr was null!");
                return null;
            }

            return new MediaRecorder(recorder, width, height);
        }

        public unsafe void CommitFrame<T>(NativeArray<T> pixels, long timestamp) where T : unmanaged
        {
            CommitFrame(pixels.GetUnsafeReadOnlyPtr(), timestamp);
        }
        
        public unsafe void CommitFrame(void* pixels, long timestamp)
        {
            _recorder.CommitFrame(pixels, timestamp);
        }
        
        public unsafe void CommitSamples (float[] samples) {
            fixed (float* baseAddress = samples)
                CommitSamples(baseAddress, samples.Length);
        }
        
        public unsafe void CommitSamples (NativeArray<float> samples) => CommitSamples(
            (float*)samples.GetUnsafeReadOnlyPtr(),
            samples.Length
        );

        private unsafe void CommitSamples(float* samples, int sampleCount)
        {
            _recorder.CommitAudioSamples(samples, sampleCount);
        }

        public Task<string> FinishWriting()
        {
            var tcs = new TaskCompletionSource<string>();
            var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
            try
            {
                _recorder.FinishWriting(OnFinishWriting, (IntPtr)handle);
            } catch (Exception ex) {
                handle.Free();
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(QuestCamNative.RecordingHandler))]
        private static unsafe void OnFinishWriting(IntPtr context, IntPtr path)
        {
            // Get tcs
            TaskCompletionSource<string> tcs;
            try {
                var handle = (GCHandle)context;
                tcs = handle.Target as TaskCompletionSource<string>;
                handle.Free();
            } catch (Exception ex) {
                Debug.LogException(ex);
                return;
            }
            // Invoke
            if (path != IntPtr.Zero)
                tcs?.SetResult(Marshal.PtrToStringUTF8(path));
            else
                tcs?.SetException(new Exception(@"Recorder failed to finish writing"));
        }
    }
}