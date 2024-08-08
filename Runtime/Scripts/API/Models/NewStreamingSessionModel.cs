using System;

namespace BRIJ.Models
{
    [Serializable]
    public class NewStreamingSessionModel
    {
        public string token;
        public bool recording = true;
    }
}