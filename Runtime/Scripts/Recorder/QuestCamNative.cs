using System;
using System.Runtime.InteropServices;

namespace QuestCam
{
    public static class QuestCamNative
    {
        public const string Assembly = @"QuestCamLib";
        
        public enum Status : int {
            Ok                  = 0,
            InvalidArgument     = 1,
            InvalidOperation    = 2,
            NotImplemented      = 3,
            InvalidSession      = 101,
            InvalidPlan         = 104,
            LimitedPlan         = 105,
        }
        
        public delegate void ReadbackHandler (IntPtr context, IntPtr pixelBuffer);

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(Assembly, EntryPoint = @"QCCreateGLTexutreInput")]
        public static extern void QCCreateGLTexutreInput(int width, int height, ReadbackHandler handler, out IntPtr input);

        [DllImport(Assembly, EntryPoint = @"QCCommitGLFrame")]
        public static extern void CommitGLFrame(this IntPtr iput, IntPtr texture, IntPtr callback);
        
        [DllImport(Assembly, EntryPoint = @"QCReleaseGLTextureInput")]
        public static extern void ReleaseGLTextureInput (this IntPtr input);
#else
        public static void QCCreateGLTexutreInput(int width, int height, ReadbackHandler handler, out IntPtr input) =>
            input = IntPtr.Zero;
        
        public static void CommitGLFrame(this IntPtr input, IntPtr texture, IntPtr callback) {}
        
        public static void ReleaseGLTextureInput (this IntPtr input) {}
#endif
        
        [DllImport(Assembly, EntryPoint = @"QCRecorderCreateMP4")]
        public static extern Status CreateMP4Recorder (
            [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitrate,
            int keyframeInterval,
            int audioBitRate,
            out IntPtr recorder
        );
        
        [DllImport(Assembly, EntryPoint = @"QCRecorderCommitFrame")]
        public static extern unsafe Status CommitFrame (
            this IntPtr recorder,
            void* pixelBuffer
        );

        [DllImport(Assembly, EntryPoint = @"QCRecorderCommitAudioSamples")]
        public static extern unsafe Status CommitAudioSamples(this IntPtr recorder, float* samples, int sampleCount);
        
        public delegate void RecordingHandler (IntPtr context, IntPtr path);
        
        [DllImport(Assembly, EntryPoint = @"QCRecorderFinishWriting")]
        public static extern Status FinishWriting (
            this IntPtr recorder,
            RecordingHandler handler,
            IntPtr context
        );
    }
}