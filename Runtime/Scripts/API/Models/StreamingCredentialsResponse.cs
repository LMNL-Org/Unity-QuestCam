using System;

namespace BRIJ.Models
{
    [Serializable]
    public class StreamingCredentialsResponse
    {
        public string sessionId;
        public string accountId;
        public string streamingToken;
        public string streamName;
    }
}