using System;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class ReactModel
    {
        public string sessionToken;
        public long postId;
        public string reaction;
        public bool remove;
    }
}