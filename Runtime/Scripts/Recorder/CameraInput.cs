using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuestCam
{
    public class CameraInput : IDisposable
    {
        public readonly IReadOnlyList<Camera> Cameras;

        private readonly TextureInput _input;
        private readonly IClock _clock;
        private readonly RenderTextureDescriptor _renderTextureDescriptor;
        private readonly CameraInputAttachment _cameraInputAttachment;
        private int _frameCount;

        public CameraInput(TextureInput input, IClock clock, params Camera[] cameras)
        {
            Array.Sort(cameras, (a, b) => (int)(100 * (a.depth - b.depth)));
            var (width, height) = input.FrameSize;

            _input = input;
            _clock = clock;
            Cameras = cameras;
            _renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGBHalf, 24)
            {
                sRGB = true,
                msaaSamples = Mathf.Max(QualitySettings.antiAliasing, 1)
            };
            _cameraInputAttachment = new GameObject("QuestCam.CameraInputAttachment")
                .AddComponent<CameraInputAttachment>();
            _cameraInputAttachment.StartCoroutine(CommitFrames());
        }

        public CameraInput(MediaRecorder mediaRecorder, IClock clock, params Camera[] cameras)
            : this(TextureInput.Create(mediaRecorder), clock, cameras)
        {}
        
        public void Dispose () {
            Object.DestroyImmediate(_cameraInputAttachment.gameObject);
            _input.Dispose();
        }

        private IEnumerator CommitFrames()
        {
            var yielder = new WaitForEndOfFrame();

            for (;;)
            {
                yield return yielder;

                var frameBuffer = RenderTexture.GetTemporary(_renderTextureDescriptor);
                ClearFrame(frameBuffer);

                for (int i = 0; i < Cameras.Count; i++)
                {
                    CommitFrame(Cameras[i], frameBuffer);
                }
                
                _input.CommitFrame(frameBuffer, _clock.Timestamp);
                RenderTexture.ReleaseTemporary(frameBuffer);
            }
        }
        
        protected virtual void ClearFrame (RenderTexture renderTexture) {
            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = prevActive;
        }

        protected virtual void CommitFrame (Camera source, RenderTexture destination) {
            var prevTarget = source.targetTexture;
            source.targetTexture = destination;
            source.Render();
            source.targetTexture = prevTarget;
        }
        
        private sealed class CameraInputAttachment : MonoBehaviour { }
    }
}