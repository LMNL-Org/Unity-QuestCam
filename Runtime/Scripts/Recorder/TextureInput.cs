using System;
using UnityEngine;

namespace QuestCam
{
    public class TextureInput : IDisposable
    {
        protected Texture2D _readbackTexture;
        protected MediaRecorder _mediaRecorder;

        public (int width, int height) FrameSize => _mediaRecorder.FrameSize;

        public TextureInput(MediaRecorder mediaRecorder)
        {
            _mediaRecorder = mediaRecorder;
        }

        public virtual void Dispose() => Texture2D.Destroy(_readbackTexture);

        public virtual void CommitFrame(Texture texture, long timestamp)
        {
            /*var (width, height) = _mediaRecorder.FrameSize;
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture, renderTexture);

            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            
            _readbackTexture = _readbackTexture
                ? _readbackTexture
                : new Texture2D(width, height, TextureFormat.RGBA32, false);
            _readbackTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            RenderTexture.active = prevActive;
            RenderTexture.ReleaseTemporary(renderTexture);

            _mediaRecorder.CommitFrame(_readbackTexture.GetRawTextureData<byte>(), timestamp);*/
        }

        public static TextureInput Create(MediaRecorder mediaRecorder, ColorSpace colorSpace)
        {
            return new AsyncTextureInput(mediaRecorder);
        }
    }
}