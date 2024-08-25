using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuestCam
{
    public sealed class GLTextureInput : TextureInput
    {
        private static readonly IntPtr RenderThreadCallback;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityRenderingEventAndData(int _, IntPtr data);

        static GLTextureInput()
        {
            RenderThreadCallback =
                Marshal.GetFunctionPointerForDelegate<UnityRenderingEventAndData>(OnRenderThreadInvoke);
        }

        [MonoPInvokeCallback(typeof(UnityRenderingEventAndData))]
        private static void OnRenderThreadInvoke(int _, IntPtr context)
        {
            try
            {
                var handle = (GCHandle)context;
                var action = handle.Target as Action;
                handle.Free();
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [MonoPInvokeCallback(typeof(QuestCamNative.ReadbackHandler))]
        private static void OnReadbackCompleted(IntPtr context, IntPtr pixelBuffer)
        {
            try
            {
                var handle = (GCHandle)context;
                var action = handle.Target as Action<IntPtr>;
                handle.Free();
                action?.Invoke(pixelBuffer);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void RunOnRenderThread (CommandBuffer commandBuffer, Action action) {
            var handle = GCHandle.Alloc(action, GCHandleType.Normal);
            commandBuffer.IssuePluginEventAndData(RenderThreadCallback, default, (IntPtr)handle);
        }
        
        private readonly RenderTexture _frameBuffer;
        private readonly IntPtr _frameBufferId;
        private IntPtr _input;
        private readonly object _fence;
        
        public GLTextureInput(MediaRecorder mediaRecorder, ColorSpace colorSpace) : base(mediaRecorder)
        {
            var (width, height) = mediaRecorder.FrameSize;
            _frameBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            _frameBuffer.Create();
            _frameBufferId = _frameBuffer.GetNativeTexturePtr();
            _fence = new object();
            
            QuestCamNative.QCCreateGLTextureInput(width, height, OnReadbackCompleted, colorSpace, out _input);
        }

        public override void Dispose()
        {
            lock (_fence)
            {
                _frameBuffer.Release();
                _input.ReleaseGLTextureInput();
                _input = default;
                base.Dispose();
            }
        }

        public override unsafe void CommitFrame(Texture texture, long timestamp)
        {
            lock (_fence)
            {
                if (_input == default)
                    return;
                
                Action<IntPtr> callback = pixelBuffer => _mediaRecorder.CommitFrame((void*)pixelBuffer, timestamp);
                var handle = GCHandle.Alloc(callback, GCHandleType.Normal);
                var cmd = new CommandBuffer();
                cmd.name = "GLTextureInput";
                cmd.Blit(texture, _frameBuffer);
                
                RunOnRenderThread(cmd, () =>
                {
                    lock (_fence)
                    {
                        _input.CommitGLFrame(_frameBufferId, (IntPtr) handle);
                    }
                });
                
                Graphics.ExecuteCommandBuffer(cmd);
            }
        }
    }
}