namespace QuestCam
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using Unity.Collections.LowLevel.Unsafe;
    
    public class AsyncTextureInput : TextureInput
    {
        private bool m_IsReady = false;
        private RenderTextureDescriptor _renderTextureDescriptor;

        public AsyncTextureInput(MediaRecorder mediaRecorder) : base(mediaRecorder)
        {
            m_IsReady = true;
            var (width, height) = _mediaRecorder.FrameSize;
            
            _renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0) {
                sRGB = true
            };
        }

        public override void CommitFrame(Texture texture, long timestamp)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(_renderTextureDescriptor);
            Graphics.Blit(texture, renderTexture);
            
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGBA32, request =>
            {
                if (m_IsReady)
                {
                    _mediaRecorder.CommitFrame(request.GetData<byte>(), timestamp);
                }
            });

            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public override void Dispose()
        {
            m_IsReady = false;
            base.Dispose();
        }
    }
}