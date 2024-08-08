using System;

namespace BRIJ.Models
{
    [Serializable]
    public class StreamingSessionStatusResponse
    {
        public bool streaming;
        public bool recordMode;
        public bool processingVideo;
        public bool finished;
        public string videoUUID;
    }
}