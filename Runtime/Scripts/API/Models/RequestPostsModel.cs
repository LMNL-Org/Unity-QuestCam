using System;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class RequestPostsModel
    {
        public string sessionToken;
        public int gameId;
        public int pageNumber;
        public int pageSize;
    }
}